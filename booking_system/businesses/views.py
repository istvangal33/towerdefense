from rest_framework import viewsets, permissions
from rest_framework.decorators import action
from rest_framework.response import Response
from .models import Business, Service
from .serializers import BusinessSerializer, ServiceSerializer

class BusinessViewSet(viewsets.ReadOnlyModelViewSet):
    """
    Business API ViewSet - read only
    """
    queryset = Business.objects.filter(is_active=True)
    serializer_class = BusinessSerializer
    permission_classes = [permissions.AllowAny]  # Public access for viewing businesses
    
    @action(detail=True, methods=['get'])
    def services(self, request, pk=None):
        """Get services for a specific business"""
        business = self.get_object()
        services = business.services.filter(is_active=True)
        serializer = ServiceSerializer(services, many=True)
        return Response(serializer.data)

class ServiceViewSet(viewsets.ReadOnlyModelViewSet):
    """
    Service API ViewSet - read only
    """
    queryset = Service.objects.filter(is_active=True)
    serializer_class = ServiceSerializer
    permission_classes = [permissions.AllowAny]  # Public access for viewing services
    
    def get_queryset(self):
        queryset = Service.objects.filter(is_active=True)
        business_id = self.request.query_params.get('business', None)
        if business_id is not None:
            queryset = queryset.filter(business=business_id)
        return queryset
