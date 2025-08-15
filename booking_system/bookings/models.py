from django.db import models
from django.contrib.auth import get_user_model
from django.core.mail import send_mail
from django.utils import timezone
from businesses.models import Business, Service

User = get_user_model()

class TimeSlot(models.Model):
    """
    Időpont modell
    """
    business = models.ForeignKey(
        Business, 
        on_delete=models.CASCADE, 
        related_name='time_slots',
        verbose_name='Vállalkozás'
    )
    date = models.DateField(verbose_name='Dátum')
    start_time = models.TimeField(verbose_name='Kezdő időpont')
    end_time = models.TimeField(verbose_name='Befejező időpont')
    is_available = models.BooleanField(default=True, verbose_name='Elérhető')
    created_at = models.DateTimeField(auto_now_add=True, verbose_name='Létrehozva')

    class Meta:
        verbose_name = 'Időpont'
        verbose_name_plural = 'Időpontok'
        ordering = ['date', 'start_time']
        unique_together = ['business', 'date', 'start_time']

    def __str__(self):
        return f"{self.business.name} - {self.date} {self.start_time}-{self.end_time}"
    
    @property
    def is_past(self):
        """Ellenőrzi, hogy múltbeli időpont-e"""
        now = timezone.now()
        slot_datetime = timezone.make_aware(
            timezone.datetime.combine(self.date, self.start_time)
        )
        return slot_datetime < now

class Booking(models.Model):
    """
    Foglalás modell
    """
    STATUS_CHOICES = [
        ('pending', 'Függőben'),
        ('confirmed', 'Megerősítve'),
        ('completed', 'Befejezve'),
        ('cancelled', 'Lemondva'),
    ]
    
    business = models.ForeignKey(
        Business, 
        on_delete=models.CASCADE, 
        related_name='bookings',
        verbose_name='Vállalkozás'
    )
    service = models.ForeignKey(
        Service, 
        on_delete=models.CASCADE, 
        related_name='bookings',
        verbose_name='Szolgáltatás'
    )
    time_slot = models.ForeignKey(
        TimeSlot, 
        on_delete=models.CASCADE, 
        related_name='bookings',
        verbose_name='Időpont'
    )
    customer = models.ForeignKey(
        User, 
        on_delete=models.CASCADE, 
        related_name='bookings',
        verbose_name='Ügyfél'
    )
    
    # Ügyfél adatok
    customer_name = models.CharField(max_length=200, verbose_name='Ügyfél neve')
    customer_email = models.EmailField(verbose_name='Ügyfél e-mail')
    customer_phone = models.CharField(max_length=20, verbose_name='Ügyfél telefon')
    
    # Foglalás részletei
    status = models.CharField(
        max_length=20, 
        choices=STATUS_CHOICES, 
        default='pending',
        verbose_name='Státusz'
    )
    notes = models.TextField(blank=True, verbose_name='Megjegyzések')
    total_price = models.DecimalField(
        max_digits=10, 
        decimal_places=0, 
        verbose_name='Teljes ár (Ft)'
    )
    
    created_at = models.DateTimeField(auto_now_add=True, verbose_name='Létrehozva')
    updated_at = models.DateTimeField(auto_now=True, verbose_name='Módosítva')

    class Meta:
        verbose_name = 'Foglalás'
        verbose_name_plural = 'Foglalások'
        ordering = ['-created_at']

    def __str__(self):
        return f"{self.customer_name} - {self.service.name} - {self.time_slot.date} {self.time_slot.start_time}"
    
    def save(self, *args, **kwargs):
        # Automatikus ár beállítás
        if not self.total_price:
            self.total_price = self.service.price
            
        # Időpont foglalttá tétele
        if self.status in ['confirmed', 'pending']:
            self.time_slot.is_available = False
            self.time_slot.save()
        elif self.status == 'cancelled':
            self.time_slot.is_available = True
            self.time_slot.save()
            
        super().save(*args, **kwargs)
        
        # E-mail értesítés küldése
        self.send_notification_email()
    
    def send_notification_email(self):
        """E-mail értesítés küldése foglalásról"""
        subject = f"Foglalás - {self.service.name}"
        
        if self.status == 'confirmed':
            message = f"""
            Kedves {self.customer_name}!
            
            Foglalása megerősítésre került:
            
            Szolgáltatás: {self.service.name}
            Dátum: {self.time_slot.date}
            Időpont: {self.time_slot.start_time} - {self.time_slot.end_time}
            Helyszín: {self.business.name}
            Ár: {self.total_price} Ft
            
            Üdvözlettel,
            {self.business.name}
            """
        elif self.status == 'cancelled':
            message = f"""
            Kedves {self.customer_name}!
            
            Foglalása lemondásra került:
            
            Szolgáltatás: {self.service.name}
            Dátum: {self.time_slot.date}
            Időpont: {self.time_slot.start_time} - {self.time_slot.end_time}
            
            Üdvözlettel,
            {self.business.name}
            """
        else:
            message = f"""
            Kedves {self.customer_name}!
            
            Új foglalás érkezett:
            
            Szolgáltatás: {self.service.name}
            Dátum: {self.time_slot.date}
            Időpont: {self.time_slot.start_time} - {self.time_slot.end_time}
            Státusz: {self.get_status_display()}
            
            Üdvözlettel,
            {self.business.name}
            """
        
        try:
            send_mail(
                subject,
                message,
                'noreply@relaxzone.hu',
                [self.customer_email],
                fail_silently=True,
            )
        except Exception as e:
            print(f"Email küldési hiba: {e}")
