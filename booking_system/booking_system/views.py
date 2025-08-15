from django.shortcuts import render, redirect
from django.contrib.auth.decorators import login_required
from django.contrib.auth import login, authenticate
from django.contrib.auth.forms import UserCreationForm
from django.http import JsonResponse
from django.utils import timezone
from datetime import timedelta
from businesses.models import Business, Service
from bookings.models import TimeSlot, Booking

def home(request):
    """Home page with business information and services"""
    business = Business.objects.filter(is_active=True).first()
    services = Service.objects.filter(is_active=True, business=business) if business else []
    
    context = {
        'business': business,
        'services': services,
    }
    return render(request, 'home.html', context)

def booking_page(request):
    """Booking page with interactive booking form"""
    business = Business.objects.filter(is_active=True).first()
    services = Service.objects.filter(is_active=True, business=business) if business else []
    
    # Get available time slots for the next 14 days
    today = timezone.now().date()
    end_date = today + timedelta(days=14)
    
    available_slots = TimeSlot.objects.filter(
        business=business,
        date__gte=today,
        date__lte=end_date,
        is_available=True
    ).order_by('date', 'start_time')
    
    context = {
        'business': business,
        'services': services,
        'available_slots': available_slots,
    }
    return render(request, 'booking.html', context)

@login_required
def dashboard(request):
    """User dashboard"""
    user = request.user
    
    if user.is_business_admin:
        # Business admin dashboard
        businesses = user.managed_businesses.all()
        recent_bookings = Booking.objects.filter(
            business__in=businesses
        ).order_by('-created_at')[:10]
        
        context = {
            'user_type': 'business_admin',
            'businesses': businesses,
            'recent_bookings': recent_bookings,
        }
        return render(request, 'dashboard/business_admin.html', context)
    
    elif user.is_super_admin:
        # Super admin dashboard
        all_businesses = Business.objects.all()
        all_bookings = Booking.objects.all().order_by('-created_at')[:20]
        total_users = Business.objects.count()
        
        context = {
            'user_type': 'super_admin',
            'businesses': all_businesses,
            'bookings': all_bookings,
            'total_users': total_users,
        }
        return render(request, 'dashboard/super_admin.html', context)
    
    else:
        # Customer dashboard
        user_bookings = Booking.objects.filter(customer=user).order_by('-created_at')
        
        context = {
            'user_type': 'customer',
            'bookings': user_bookings,
        }
        return render(request, 'dashboard/customer.html', context)

@login_required
def calendar_view(request):
    """Calendar view for bookings"""
    user = request.user
    
    if user.is_business_admin:
        bookings = Booking.objects.filter(business__admins=user)
    elif user.is_super_admin:
        bookings = Booking.objects.all()
    else:
        bookings = Booking.objects.filter(customer=user)
    
    context = {
        'bookings': bookings,
    }
    return render(request, 'calendar.html', context)

def register(request):
    """User registration"""
    if request.method == 'POST':
        form = UserCreationForm(request.POST)
        if form.is_valid():
            user = form.save()
            username = form.cleaned_data.get('username')
            password = form.cleaned_data.get('password1')
            user = authenticate(username=username, password=password)
            login(request, user)
            return redirect('home')
    else:
        form = UserCreationForm()
    
    return render(request, 'registration/register.html', {'form': form})

# AJAX endpoints
def get_available_slots(request):
    """AJAX endpoint to get available slots for a date and service"""
    if request.method == 'GET':
        date = request.GET.get('date')
        service_id = request.GET.get('service_id')
        
        if not date or not service_id:
            return JsonResponse({'error': 'Date and service_id are required'}, status=400)
        
        try:
            service = Service.objects.get(id=service_id)
            slots = TimeSlot.objects.filter(
                business=service.business,
                date=date,
                is_available=True
            ).order_by('start_time')
            
            slots_data = [
                {
                    'id': slot.id,
                    'start_time': slot.start_time.strftime('%H:%M'),
                    'end_time': slot.end_time.strftime('%H:%M'),
                }
                for slot in slots
            ]
            
            return JsonResponse({'slots': slots_data})
            
        except Service.DoesNotExist:
            return JsonResponse({'error': 'Service not found'}, status=404)
        except Exception as e:
            return JsonResponse({'error': str(e)}, status=500)
    
    return JsonResponse({'error': 'Only GET method allowed'}, status=405)