from rest_framework import serializers
from .models import TimeSlot, Booking
from businesses.serializers import ServiceSerializer, BusinessSerializer

class TimeSlotSerializer(serializers.ModelSerializer):
    business = BusinessSerializer(read_only=True)
    is_booked = serializers.SerializerMethodField()
    
    class Meta:
        model = TimeSlot
        fields = [
            'id', 'business', 'date', 'start_time', 'end_time', 
            'is_available', 'is_booked', 'is_past'
        ]
    
    def get_is_booked(self, obj):
        return obj.bookings.filter(status__in=['pending', 'confirmed']).exists()

class BookingSerializer(serializers.ModelSerializer):
    business = BusinessSerializer(read_only=True)
    service = ServiceSerializer(read_only=True)
    time_slot = TimeSlotSerializer(read_only=True)
    status_display = serializers.CharField(source='get_status_display', read_only=True)
    
    # Write fields
    business_id = serializers.IntegerField(write_only=True)
    service_id = serializers.IntegerField(write_only=True)
    time_slot_id = serializers.IntegerField(write_only=True)
    
    class Meta:
        model = Booking
        fields = [
            'id', 'business', 'service', 'time_slot', 'customer',
            'customer_name', 'customer_email', 'customer_phone',
            'status', 'status_display', 'notes', 'total_price',
            'created_at', 'updated_at',
            # Write only fields
            'business_id', 'service_id', 'time_slot_id'
        ]
        read_only_fields = ['customer', 'total_price', 'created_at', 'updated_at']
    
    def create(self, validated_data):
        # Extract foreign key IDs
        business_id = validated_data.pop('business_id')
        service_id = validated_data.pop('service_id')
        time_slot_id = validated_data.pop('time_slot_id')
        
        # Get the objects
        from businesses.models import Business, Service
        business = Business.objects.get(id=business_id)
        service = Service.objects.get(id=service_id)
        time_slot = TimeSlot.objects.get(id=time_slot_id)
        
        # Create the booking
        booking = Booking.objects.create(
            business=business,
            service=service,
            time_slot=time_slot,
            customer=self.context['request'].user,
            **validated_data
        )
        
        return booking