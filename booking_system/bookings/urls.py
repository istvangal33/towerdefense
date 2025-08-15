from django.urls import path, include
from rest_framework.routers import DefaultRouter
from .views import TimeSlotViewSet, BookingViewSet

router = DefaultRouter()
router.register(r'timeslots', TimeSlotViewSet)
router.register(r'bookings', BookingViewSet)

urlpatterns = [
    path('api/', include(router.urls)),
]