from django.contrib.auth.models import AbstractUser
from django.db import models

class CustomUser(AbstractUser):
    """
    Custom user model with additional fields
    """
    USER_TYPE_CHOICES = [
        ('customer', 'Ügyfél'),
        ('business_admin', 'Vállalkozói admin'),
        ('super_admin', 'Super admin'),
    ]
    
    user_type = models.CharField(
        max_length=20, 
        choices=USER_TYPE_CHOICES, 
        default='customer',
        verbose_name='Felhasználó típus'
    )
    phone_number = models.CharField(
        max_length=20, 
        blank=True, 
        verbose_name='Telefonszám'
    )
    date_of_birth = models.DateField(
        null=True, 
        blank=True, 
        verbose_name='Születési dátum'
    )
    created_at = models.DateTimeField(auto_now_add=True, verbose_name='Létrehozva')
    updated_at = models.DateTimeField(auto_now=True, verbose_name='Módosítva')

    class Meta:
        verbose_name = 'Felhasználó'
        verbose_name_plural = 'Felhasználók'
        
    def __str__(self):
        return f"{self.username} ({self.get_user_type_display()})"
    
    @property
    def is_business_admin(self):
        return self.user_type == 'business_admin'
    
    @property
    def is_super_admin(self):
        return self.user_type == 'super_admin'
