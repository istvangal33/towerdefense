from django.core.management.base import BaseCommand
from django.contrib.auth import get_user_model
from django.utils import timezone
from datetime import datetime, time, date, timedelta
from businesses.models import Business, Service
from bookings.models import TimeSlot

User = get_user_model()

class Command(BaseCommand):
    help = 'Load initial test data for RelaxZone Masszázsstúdió'

    def handle(self, *args, **options):
        self.stdout.write(self.style.SUCCESS('Kezdeti teszt adatok betöltése...'))
        
        # Create business admin user
        admin_user, created = User.objects.get_or_create(
            username='relaxzone_admin',
            defaults={
                'email': 'admin@relaxzone.hu',
                'first_name': 'RelaxZone',
                'last_name': 'Admin',
                'user_type': 'business_admin',
                'phone_number': '+36301234567',
            }
        )
        if created:
            admin_user.set_password('admin123')
            admin_user.save()
            self.stdout.write(f'✓ Admin felhasználó létrehozva: {admin_user.username}')
        
        # Create test customer
        customer_user, created = User.objects.get_or_create(
            username='teszt_ugyfel',
            defaults={
                'email': 'teszt@email.hu',
                'first_name': 'Teszt',
                'last_name': 'Ügyfél',
                'user_type': 'customer',
                'phone_number': '+36309876543',
            }
        )
        if created:
            customer_user.set_password('teszt123')
            customer_user.save()
            self.stdout.write(f'✓ Teszt ügyfél létrehozva: {customer_user.username}')
        
        # Create RelaxZone business
        business, created = Business.objects.get_or_create(
            name='RelaxZone Masszázsstúdió',
            defaults={
                'description': 'Professzionális masszázs szolgáltatások relaxációs és gyógyászati célokra. Tapasztalt masszőrök, nyugodt környezet, minőségi kezelések.',
                'address': '1051 Budapest, Nádor utca 15.',
                'phone': '+36301234567',
                'email': 'info@relaxzone.hu',
                'website': 'https://www.relaxzone.hu',
                
                # Nyitvatartás beállítása
                'monday_open': time(9, 0),
                'monday_close': time(19, 0),
                'tuesday_open': time(9, 0),
                'tuesday_close': time(19, 0),
                'wednesday_open': time(9, 0),
                'wednesday_close': time(19, 0),
                'thursday_open': time(9, 0),
                'thursday_close': time(19, 0),
                'friday_open': time(9, 0),
                'friday_close': time(19, 0),
                'saturday_open': time(10, 0),
                'saturday_close': time(16, 0),
                'sunday_open': None,
                'sunday_close': None,
            }
        )
        if created:
            business.admins.add(admin_user)
            self.stdout.write(f'✓ Vállalkozás létrehozva: {business.name}')
        
        # Create services
        services_data = [
            {
                'name': 'Relaxációs masszázs',
                'description': 'Teljes test masszázs stressz oldásra és relaxációra. Természetes olajokkal, nyugodt környezetben.',
                'duration': 60,
                'price': 8000,
            },
            {
                'name': 'Gyógymasszázs',
                'description': 'Terápiás masszázs izmok és ízületek kezelésére, fájdalom csillapítására.',
                'duration': 45,
                'price': 6500,
            },
            {
                'name': 'Aromaterápia',
                'description': 'Különleges aromaterápiás masszázs illóolajokkal, teljes relaxációért.',
                'duration': 90,
                'price': 12000,
            },
        ]
        
        for service_data in services_data:
            service, created = Service.objects.get_or_create(
                business=business,
                name=service_data['name'],
                defaults=service_data
            )
            if created:
                self.stdout.write(f'✓ Szolgáltatás létrehozva: {service.name} - {service.price} Ft')
        
        # Create time slots for the next 14 days
        self.stdout.write('Időpontok létrehozása...')
        
        today = date.today()
        for day_offset in range(0, 14):  # Next 14 days
            current_date = today + timedelta(days=day_offset)
            weekday = current_date.weekday()  # 0=Monday, 6=Sunday
            
            # Skip Sundays (6)
            if weekday == 6:
                continue
                
            # Determine opening hours
            if weekday == 5:  # Saturday
                start_hour, end_hour = 10, 16
            else:  # Monday-Friday
                start_hour, end_hour = 9, 19
                
            # Create time slots every hour
            for hour in range(start_hour, end_hour):
                time_slot, created = TimeSlot.objects.get_or_create(
                    business=business,
                    date=current_date,
                    start_time=time(hour, 0),
                    defaults={
                        'end_time': time(hour + 1, 0),
                        'is_available': True,
                    }
                )
                if created and day_offset < 3:  # Only show first 3 days
                    self.stdout.write(f'  ✓ {current_date} {time_slot.start_time}-{time_slot.end_time}')
        
        self.stdout.write(self.style.SUCCESS('\n🎉 Kezdeti adatok sikeresen betöltve!'))
        self.stdout.write(self.style.SUCCESS('\nBejelentkezési adatok:'))
        self.stdout.write(f'Admin: username=admin, password=admin (Django admin)')
        self.stdout.write(f'Vállalkozói admin: username=relaxzone_admin, password=admin123')
        self.stdout.write(f'Teszt ügyfél: username=teszt_ugyfel, password=teszt123')