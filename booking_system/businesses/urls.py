from django.urls import path, include
from rest_framework.routers import DefaultRouter
from .views import BusinessViewSet, ServiceViewSet

router = DefaultRouter()
router.register(r'businesses', BusinessViewSet)
router.register(r'services', ServiceViewSet)

urlpatterns = [
    path('api/', include(router.urls)),
]