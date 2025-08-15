from django.db import models
from django.contrib.auth import get_user_model

User = get_user_model()

class Business(models.Model):
    """
    Vállalkozás modell
    """
    name = models.CharField(max_length=200, verbose_name='Vállalkozás neve')
    description = models.TextField(verbose_name='Leírás')
    address = models.CharField(max_length=300, verbose_name='Cím')
    phone = models.CharField(max_length=20, verbose_name='Telefon')
    email = models.EmailField(verbose_name='E-mail')
    website = models.URLField(blank=True, verbose_name='Weboldal')
    logo = models.ImageField(upload_to='business_logos/', blank=True, verbose_name='Logó')
    
    # Nyitvatartás
    monday_open = models.TimeField(null=True, blank=True, verbose_name='Hétfő nyitás')
    monday_close = models.TimeField(null=True, blank=True, verbose_name='Hétfő zárás')
    tuesday_open = models.TimeField(null=True, blank=True, verbose_name='Kedd nyitás')
    tuesday_close = models.TimeField(null=True, blank=True, verbose_name='Kedd zárás')
    wednesday_open = models.TimeField(null=True, blank=True, verbose_name='Szerda nyitás')
    wednesday_close = models.TimeField(null=True, blank=True, verbose_name='Szerda zárás')
    thursday_open = models.TimeField(null=True, blank=True, verbose_name='Csütörtök nyitás')
    thursday_close = models.TimeField(null=True, blank=True, verbose_name='Csütörtök zárás')
    friday_open = models.TimeField(null=True, blank=True, verbose_name='Péntek nyitás')
    friday_close = models.TimeField(null=True, blank=True, verbose_name='Péntek zárás')
    saturday_open = models.TimeField(null=True, blank=True, verbose_name='Szombat nyitás')
    saturday_close = models.TimeField(null=True, blank=True, verbose_name='Szombat zárás')
    sunday_open = models.TimeField(null=True, blank=True, verbose_name='Vasárnap nyitás')
    sunday_close = models.TimeField(null=True, blank=True, verbose_name='Vasárnap zárás')
    
    is_active = models.BooleanField(default=True, verbose_name='Aktív')
    created_at = models.DateTimeField(auto_now_add=True, verbose_name='Létrehozva')
    updated_at = models.DateTimeField(auto_now=True, verbose_name='Módosítva')
    
    # Kapcsolódó admin felhasználók
    admins = models.ManyToManyField(
        User, 
        related_name='managed_businesses',
        limit_choices_to={'user_type': 'business_admin'},
        verbose_name='Admin felhasználók'
    )

    class Meta:
        verbose_name = 'Vállalkozás'
        verbose_name_plural = 'Vállalkozások'

    def __str__(self):
        return self.name

class Service(models.Model):
    """
    Szolgáltatás modell
    """
    business = models.ForeignKey(
        Business, 
        on_delete=models.CASCADE, 
        related_name='services',
        verbose_name='Vállalkozás'
    )
    name = models.CharField(max_length=200, verbose_name='Szolgáltatás neve')
    description = models.TextField(verbose_name='Leírás')
    duration = models.PositiveIntegerField(verbose_name='Időtartam (perc)')
    price = models.DecimalField(
        max_digits=10, 
        decimal_places=0, 
        verbose_name='Ár (Ft)'
    )
    is_active = models.BooleanField(default=True, verbose_name='Aktív')
    created_at = models.DateTimeField(auto_now_add=True, verbose_name='Létrehozva')
    updated_at = models.DateTimeField(auto_now=True, verbose_name='Módosítva')

    class Meta:
        verbose_name = 'Szolgáltatás'
        verbose_name_plural = 'Szolgáltatások'
        ordering = ['name']

    def __str__(self):
        return f"{self.name} - {self.price} Ft ({self.duration} perc)"
