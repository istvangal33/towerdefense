// Main JavaScript for RelaxZone Booking System

$(document).ready(function() {
    // Initialize tooltips
    var tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
    var tooltipList = tooltipTriggerList.map(function (tooltipTriggerEl) {
        return new bootstrap.Tooltip(tooltipTriggerEl);
    });

    // Smooth scrolling for anchor links
    $('a[href*="#"]').on('click', function(e) {
        const target = $(this.getAttribute('href'));
        if (target.length) {
            e.preventDefault();
            $('html, body').stop().animate({
                scrollTop: target.offset().top - 70
            }, 1000);
        }
    });

    // Auto-hide alerts after 5 seconds
    $('.alert').delay(5000).fadeOut('slow');
    
    // Initialize booking functionality
    initBookingSystem();
});

function initBookingSystem() {
    const serviceSelect = $('#service-select');
    const dateInput = $('#date-input');
    const timeSlotsContainer = $('#time-slots-container');
    const bookingForm = $('#booking-form');
    
    // Service and date change handlers
    serviceSelect.on('change', loadTimeSlots);
    dateInput.on('change', loadTimeSlots);
    
    // Time slot selection
    $(document).on('click', '.time-slot:not(.unavailable)', function() {
        $('.time-slot').removeClass('selected');
        $(this).addClass('selected');
        $('#selected-time-slot').val($(this).data('slot-id'));
        
        // Enable booking button
        $('.btn-book').prop('disabled', false);
    });
    
    // Booking form submission
    if (bookingForm.length) {
        bookingForm.on('submit', handleBookingSubmission);
    }
    
    // Load initial time slots if service and date are already selected
    if (serviceSelect.val() && dateInput.val()) {
        loadTimeSlots();
    }
}

function loadTimeSlots() {
    const serviceId = $('#service-select').val();
    const selectedDate = $('#date-input').val();
    const timeSlotsContainer = $('#time-slots-container');
    
    if (!serviceId || !selectedDate) {
        timeSlotsContainer.html('<p class="text-muted">Válasszon szolgáltatást és dátumot az elérhető időpontokért.</p>');
        return;
    }
    
    // Show loading spinner
    timeSlotsContainer.html(`
        <div class="loading-spinner">
            <i class="fas fa-spinner fa-spin"></i>
            <p class="mt-2">Időpontok betöltése...</p>
        </div>
    `);
    
    // AJAX call to get available slots
    $.ajax({
        url: '/ajax/get-available-slots/',
        method: 'GET',
        data: {
            service_id: serviceId,
            date: selectedDate
        },
        success: function(response) {
            displayTimeSlots(response.slots);
        },
        error: function(xhr, status, error) {
            console.error('Error loading time slots:', error);
            timeSlotsContainer.html(`
                <div class="alert alert-warning">
                    <i class="fas fa-exclamation-triangle me-2"></i>
                    Hiba történt az időpontok betöltése során. Kérjük, próbálja újra.
                </div>
            `);
        }
    });
}

function displayTimeSlots(slots) {
    const timeSlotsContainer = $('#time-slots-container');
    
    if (slots.length === 0) {
        timeSlotsContainer.html(`
            <div class="alert alert-info">
                <i class="fas fa-info-circle me-2"></i>
                Nincsenek elérhető időpontok ezen a napon.
            </div>
        `);
        return;
    }
    
    let slotsHtml = '<div class="row">';
    slots.forEach(function(slot) {
        slotsHtml += `
            <div class="col-md-3 col-sm-4 col-6 mb-2">
                <div class="time-slot" data-slot-id="${slot.id}">
                    <i class="fas fa-clock me-2"></i>
                    ${slot.start_time} - ${slot.end_time}
                </div>
            </div>
        `;
    });
    slotsHtml += '</div>';
    
    timeSlotsContainer.html(slotsHtml);
    
    // Disable booking button until time slot is selected
    $('.btn-book').prop('disabled', true);
}

function handleBookingSubmission(e) {
    e.preventDefault();
    
    const form = $(this);
    const submitBtn = form.find('button[type="submit"]');
    const originalText = submitBtn.html();
    
    // Validation
    if (!$('#selected-time-slot').val()) {
        showAlert('danger', 'Kérjük, válasszon időpontot!');
        return;
    }
    
    // Show loading state
    submitBtn.prop('disabled', true).html('<i class="fas fa-spinner fa-spin me-2"></i>Foglalás...');
    
    // Get form data
    const formData = new FormData(form[0]);
    
    // AJAX submission
    $.ajax({
        url: form.attr('action'),
        method: 'POST',
        data: formData,
        processData: false,
        contentType: false,
        success: function(response) {
            if (response.success) {
                showAlert('success', 'Foglalás sikeresen elküldve! Hamarosan e-mail megerősítést fog kapni.');
                form[0].reset();
                $('#time-slots-container').empty();
                $('.time-slot').removeClass('selected');
            } else {
                showAlert('danger', response.error || 'Hiba történt a foglalás során.');
            }
        },
        error: function(xhr, status, error) {
            console.error('Booking error:', error);
            showAlert('danger', 'Hiba történt a foglalás során. Kérjük, próbálja újra.');
        },
        complete: function() {
            // Restore button state
            submitBtn.prop('disabled', false).html(originalText);
        }
    });
}

function showAlert(type, message) {
    const alertHtml = `
        <div class="alert alert-${type} alert-dismissible fade show" role="alert">
            ${message}
            <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
        </div>
    `;
    
    // Insert alert at the top of the page
    $('main').prepend(alertHtml);
    
    // Auto-hide after 5 seconds
    setTimeout(function() {
        $('.alert').fadeOut('slow');
    }, 5000);
    
    // Scroll to top to show alert
    $('html, body').animate({ scrollTop: 0 }, 500);
}

// API helper functions
const API = {
    // Get CSRF token
    getCsrfToken: function() {
        return $('[name=csrfmiddlewaretoken]').val();
    },
    
    // Make authenticated API calls
    call: function(url, method, data, successCallback, errorCallback) {
        const headers = {
            'X-CSRFToken': this.getCsrfToken()
        };
        
        $.ajax({
            url: url,
            method: method,
            data: data,
            headers: headers,
            success: successCallback || function() {},
            error: errorCallback || function(xhr) {
                console.error('API call failed:', xhr.responseText);
            }
        });
    }
};

// Dashboard functionality
function initDashboard() {
    // Refresh booking status
    $('.refresh-bookings').on('click', function() {
        location.reload();
    });
    
    // Cancel booking functionality
    $('.cancel-booking').on('click', function(e) {
        e.preventDefault();
        const bookingId = $(this).data('booking-id');
        const bookingRow = $(this).closest('.booking-row');
        
        if (confirm('Biztosan le szeretné mondani ezt a foglalást?')) {
            API.call(`/api/bookings/${bookingId}/cancel/`, 'POST', {}, 
                function(response) {
                    bookingRow.find('.status-badge').removeClass().addClass('status-badge status-cancelled').text('Lemondva');
                    showAlert('success', 'Foglalás sikeresen lemondva.');
                },
                function(xhr) {
                    const error = xhr.responseJSON?.error || 'Hiba történt a lemondás során.';
                    showAlert('danger', error);
                }
            );
        }
    });
    
    // Confirm booking functionality (for business admins)
    $('.confirm-booking').on('click', function(e) {
        e.preventDefault();
        const bookingId = $(this).data('booking-id');
        const bookingRow = $(this).closest('.booking-row');
        
        API.call(`/api/bookings/${bookingId}/confirm/`, 'POST', {},
            function(response) {
                bookingRow.find('.status-badge').removeClass().addClass('status-badge status-confirmed').text('Megerősítve');
                showAlert('success', 'Foglalás megerősítve.');
            },
            function(xhr) {
                const error = xhr.responseJSON?.error || 'Hiba történt a megerősítés során.';
                showAlert('danger', error);
            }
        );
    });
}

// Calendar functionality
function initCalendar() {
    // Calendar initialization would go here
    // For now, we'll use a simple approach
    console.log('Calendar functionality initialized');
}

// Export functions for use in other scripts
window.BookingSystem = {
    initBookingSystem,
    initDashboard,
    initCalendar,
    showAlert,
    API
};