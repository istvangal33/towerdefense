from django.contrib import admin
from .models import Business, Service

class ServiceInline(admin.TabularInline):
    """
    Szolgáltatások inline admin
    """
    model = Service
    extra = 1
    fields = ['name', 'description', 'duration', 'price', 'is_active']

@admin.register(Business)
class BusinessAdmin(admin.ModelAdmin):
    """
    Vállalkozás admin
    """
    list_display = ['name', 'email', 'phone', 'is_active', 'created_at']
    list_filter = ['is_active', 'created_at']
    search_fields = ['name', 'email', 'phone', 'address']
    ordering = ['-created_at']
    inlines = [ServiceInline]
    
    fieldsets = (
        ('Alapadatok', {
            'fields': ('name', 'description', 'address', 'phone', 'email', 'website', 'logo', 'is_active')
        }),
        ('Nyitvatartás', {
            'fields': (
                ('monday_open', 'monday_close'),
                ('tuesday_open', 'tuesday_close'),
                ('wednesday_open', 'wednesday_close'),
                ('thursday_open', 'thursday_close'),
                ('friday_open', 'friday_close'),
                ('saturday_open', 'saturday_close'),
                ('sunday_open', 'sunday_close'),
            ),
            'classes': ['collapse']
        }),
        ('Admin felhasználók', {
            'fields': ('admins',),
            'classes': ['collapse']
        }),
    )
    
    filter_horizontal = ['admins']

@admin.register(Service)
class ServiceAdmin(admin.ModelAdmin):
    """
    Szolgáltatás admin
    """
    list_display = ['name', 'business', 'duration', 'price', 'is_active', 'created_at']
    list_filter = ['business', 'is_active', 'created_at']
    search_fields = ['name', 'description', 'business__name']
    ordering = ['business', 'name']
    
    fieldsets = (
        ('Alapadatok', {
            'fields': ('business', 'name', 'description', 'duration', 'price', 'is_active')
        }),
    )
