from django.contrib import admin
from django.utils.html import format_html
from .models import TimeSlot, Booking

@admin.register(TimeSlot)
class TimeSlotAdmin(admin.ModelAdmin):
    """
    Időpont admin
    """
    list_display = ['business', 'date', 'start_time', 'end_time', 'is_available', 'is_booked']
    list_filter = ['business', 'date', 'is_available', 'created_at']
    search_fields = ['business__name']
    ordering = ['date', 'start_time']
    date_hierarchy = 'date'
    
    def is_booked(self, obj):
        """Mutatja, hogy lefoglalt-e az időpont"""
        booked = obj.bookings.filter(status__in=['pending', 'confirmed']).exists()
        if booked:
            return format_html('<span style="color: red;">Foglalt</span>')
        return format_html('<span style="color: green;">Szabad</span>')
    
    is_booked.short_description = 'Foglaltság'
    is_booked.admin_order_field = 'bookings'

@admin.register(Booking)
class BookingAdmin(admin.ModelAdmin):
    """
    Foglalás admin
    """
    list_display = [
        'customer_name', 
        'service', 
        'business', 
        'booking_date', 
        'booking_time',
        'status_colored', 
        'total_price',
        'created_at'
    ]
    list_filter = ['status', 'business', 'service', 'time_slot__date', 'created_at']
    search_fields = ['customer_name', 'customer_email', 'customer_phone', 'service__name']
    ordering = ['-created_at']
    date_hierarchy = 'created_at'
    
    fieldsets = (
        ('Foglalás adatok', {
            'fields': ('business', 'service', 'time_slot', 'status', 'total_price')
        }),
        ('Ügyfél adatok', {
            'fields': ('customer', 'customer_name', 'customer_email', 'customer_phone')
        }),
        ('További információk', {
            'fields': ('notes',),
            'classes': ['collapse']
        }),
    )
    
    readonly_fields = ['created_at', 'updated_at']
    
    def booking_date(self, obj):
        """Foglalás dátuma"""
        return obj.time_slot.date
    booking_date.short_description = 'Dátum'
    booking_date.admin_order_field = 'time_slot__date'
    
    def booking_time(self, obj):
        """Foglalás időpontja"""
        return f"{obj.time_slot.start_time} - {obj.time_slot.end_time}"
    booking_time.short_description = 'Időpont'
    
    def status_colored(self, obj):
        """Színezett státusz"""
        colors = {
            'pending': 'orange',
            'confirmed': 'green',
            'completed': 'blue',
            'cancelled': 'red',
        }
        color = colors.get(obj.status, 'black')
        return format_html(
            '<span style="color: {};">{}</span>',
            color,
            obj.get_status_display()
        )
    status_colored.short_description = 'Státusz'
    status_colored.admin_order_field = 'status'
    
    def get_queryset(self, request):
        """Optimalizált queryset"""
        return super().get_queryset(request).select_related(
            'business', 'service', 'time_slot', 'customer'
        )
    
    actions = ['confirm_bookings', 'cancel_bookings', 'complete_bookings']
    
    def confirm_bookings(self, request, queryset):
        """Foglalások megerősítése"""
        updated = queryset.update(status='confirmed')
        self.message_user(request, f'{updated} foglalás megerősítve.')
    confirm_bookings.short_description = 'Kiválasztott foglalások megerősítése'
    
    def cancel_bookings(self, request, queryset):
        """Foglalások lemondása"""
        updated = queryset.update(status='cancelled')
        self.message_user(request, f'{updated} foglalás lemondva.')
    cancel_bookings.short_description = 'Kiválasztott foglalások lemondása'
    
    def complete_bookings(self, request, queryset):
        """Foglalások befejezése"""
        updated = queryset.update(status='completed')
        self.message_user(request, f'{updated} foglalás befejezve.')
    complete_bookings.short_description = 'Kiválasztott foglalások befejezése'
