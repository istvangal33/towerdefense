from django.shortcuts import render, redirect
from django.contrib.auth.decorators import login_required
from django.contrib.auth import login, authenticate
from django.contrib.auth.forms import UserCreationForm
from django.http import JsonResponse
from django.utils import timezone
from django.contrib import messages
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
    
    if request.method == 'POST' and request.user.is_authenticated:
        try:
            # Get form data
            service_id = request.POST.get('service_id')
            time_slot_id = request.POST.get('time_slot_id')
            customer_name = request.POST.get('customer_name')
            customer_email = request.POST.get('customer_email')
            customer_phone = request.POST.get('customer_phone')
            notes = request.POST.get('notes', '')
            
            # Validate data
            if not all([service_id, time_slot_id, customer_name, customer_email, customer_phone]):
                messages.error(request, 'Kérjük, töltse ki az összes kötelező mezőt!')
                return redirect('booking')
            
            # Get objects
            service = Service.objects.get(id=service_id, business=business, is_active=True)
            time_slot = TimeSlot.objects.get(id=time_slot_id, business=business, is_available=True)
            
            # Check if time slot is still available
            if not time_slot.is_available:
                messages.error(request, 'A kiválasztott időpont már nem elérhető!')
                return redirect('booking')
            
            # Create booking
            booking = Booking.objects.create(
                business=business,
                service=service,
                time_slot=time_slot,
                customer=request.user,
                customer_name=customer_name,
                customer_email=customer_email,
                customer_phone=customer_phone,
                notes=notes,
                total_price=service.price,
                status='pending'
            )
            
            messages.success(request, 'Foglalás sikeresen elküldve! Hamarosan e-mail megerősítést fog kapni.')
            return redirect('dashboard')
            
        except (Service.DoesNotExist, TimeSlot.DoesNotExist):
            messages.error(request, 'Érvénytelen szolgáltatás vagy időpont!')
        except Exception as e:
            messages.error(request, 'Hiba történt a foglalás során. Kérjük, próbálja újra!')
    
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
        'today': today,
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
        from users.models import CustomUser
        total_users = CustomUser.objects.count()
        
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
            messages.success(request, 'Sikeres regisztráció! Üdvözöljük a RelaxZone-ban!')
            
            # Redirect to booking if that's where they came from
            next_url = request.GET.get('next')
            if next_url:
                return redirect(next_url)
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
            
            # Filter out past slots for today
            now = timezone.now()
            today = now.date()
            current_time = now.time()
            
            if str(today) == date:
                slots = slots.filter(start_time__gt=current_time)
            
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