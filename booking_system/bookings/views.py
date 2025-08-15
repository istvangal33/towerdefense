from rest_framework import viewsets, permissions, status
from rest_framework.decorators import action
from rest_framework.response import Response
from django.utils import timezone
from datetime import datetime, timedelta
from .models import TimeSlot, Booking
from .serializers import TimeSlotSerializer, BookingSerializer

class TimeSlotViewSet(viewsets.ReadOnlyModelViewSet):
    """
    TimeSlot API ViewSet - read only
    """
    queryset = TimeSlot.objects.all()
    serializer_class = TimeSlotSerializer
    permission_classes = [permissions.AllowAny]  # Public access for viewing time slots
    
    def get_queryset(self):
        queryset = TimeSlot.objects.filter(is_available=True)
        
        # Filter by business
        business_id = self.request.query_params.get('business', None)
        if business_id is not None:
            queryset = queryset.filter(business=business_id)
        
        # Filter by date range
        date_from = self.request.query_params.get('date_from', None)
        date_to = self.request.query_params.get('date_to', None)
        
        if date_from:
            queryset = queryset.filter(date__gte=date_from)
        if date_to:
            queryset = queryset.filter(date__lte=date_to)
        else:
            # Default to next 30 days if no end date specified
            if not date_to:
                end_date = timezone.now().date() + timedelta(days=30)
                queryset = queryset.filter(date__lte=end_date)
        
        # Exclude past time slots
        now = timezone.now()
        today = now.date()
        current_time = now.time()
        
        queryset = queryset.exclude(
            date__lt=today
        ).exclude(
            date=today, start_time__lt=current_time
        )
        
        return queryset.order_by('date', 'start_time')

class BookingViewSet(viewsets.ModelViewSet):
    """
    Booking API ViewSet
    """
    queryset = Booking.objects.all()
    serializer_class = BookingSerializer
    permission_classes = [permissions.IsAuthenticated]
    
    def get_queryset(self):
        user = self.request.user
        if user.is_super_admin:
            # Super admin sees all bookings
            return Booking.objects.all()
        elif user.is_business_admin:
            # Business admin sees only their business bookings
            return Booking.objects.filter(business__admins=user)
        else:
            # Regular users see only their own bookings
            return Booking.objects.filter(customer=user)
    
    def perform_create(self, serializer):
        # Set the customer to the current user
        serializer.save(customer=self.request.user)
    
    @action(detail=True, methods=['post'])
    def cancel(self, request, pk=None):
        """Cancel a booking"""
        booking = self.get_object()
        
        # Check permissions
        user = request.user
        if not (user == booking.customer or 
                user.is_super_admin or 
                (user.is_business_admin and booking.business.admins.filter(id=user.id).exists())):
            return Response(
                {'error': 'Nincs jogosultság a foglalás lemondásához'}, 
                status=status.HTTP_403_FORBIDDEN
            )
        
        if booking.status == 'cancelled':
            return Response(
                {'error': 'A foglalás már lemondva'}, 
                status=status.HTTP_400_BAD_REQUEST
            )
        
        booking.status = 'cancelled'
        booking.save()
        
        return Response({'status': 'cancelled'})
    
    @action(detail=True, methods=['post'])
    def confirm(self, request, pk=None):
        """Confirm a booking (business admin only)"""
        booking = self.get_object()
        user = request.user
        
        # Check permissions - only business admin or super admin
        if not (user.is_super_admin or 
                (user.is_business_admin and booking.business.admins.filter(id=user.id).exists())):
            return Response(
                {'error': 'Nincs jogosultság a foglalás megerősítéséhez'}, 
                status=status.HTTP_403_FORBIDDEN
            )
        
        if booking.status == 'confirmed':
            return Response(
                {'error': 'A foglalás már megerősítve'}, 
                status=status.HTTP_400_BAD_REQUEST
            )
        
        booking.status = 'confirmed'
        booking.save()
        
        return Response({'status': 'confirmed'})
    
    @action(detail=False, methods=['get'])
    def calendar(self, request):
        """Get bookings for calendar view"""
        queryset = self.get_queryset()
        
        # Filter by date range
        date_from = request.query_params.get('date_from')
        date_to = request.query_params.get('date_to')
        
        if date_from:
            queryset = queryset.filter(time_slot__date__gte=date_from)
        if date_to:
            queryset = queryset.filter(time_slot__date__lte=date_to)
        
        # Format for calendar
        calendar_data = []
        for booking in queryset:
            calendar_data.append({
                'id': booking.id,
                'title': f"{booking.service.name} - {booking.customer_name}",
                'start': f"{booking.time_slot.date}T{booking.time_slot.start_time}",
                'end': f"{booking.time_slot.date}T{booking.time_slot.end_time}",
                'status': booking.status,
                'customer': booking.customer_name,
                'service': booking.service.name,
                'phone': booking.customer_phone,
            })
        
        return Response(calendar_data)
