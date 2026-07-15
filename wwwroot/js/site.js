// VoteHub JavaScript

document.addEventListener('DOMContentLoaded', function () {
    console.log('VoteHub loaded successfully');
});

// Candidate Selection
function selectCandidate(cardElement, candidateId, candidateName) {
    // Remove selection from all cards
    document.querySelectorAll('.candidate-card').forEach(card => {
        card.classList.remove('border-success', 'border-3');
        card.style.boxShadow = '';
    });

    // Add selection to clicked card
    cardElement.classList.add('border-success', 'border-3');
    cardElement.style.boxShadow = '0 0 15px rgba(40, 167, 69, 0.5)';

    // Check the radio button
    const radioButton = cardElement.querySelector('input[type="radio"]');
    if (radioButton) {
        radioButton.checked = true;
    }

    // Enable confirm button
    const confirmBtn = document.getElementById('confirmBtn');
    if (confirmBtn) {
        confirmBtn.disabled = false;
    }

    // Update modal
    const candidateNameDisplay = document.getElementById('candidateName');
    if (candidateNameDisplay) {
        candidateNameDisplay.innerText = candidateName;
    }
}

// Confirm Vote
function submitVote() {
    const form = document.getElementById('ballotForm');
    if (form) {
        form.submit();
    }
}

// Delete Confirmation
function confirmDelete(message = 'Are you sure you want to delete this?') {
    return confirm(message);
}

// Show Toast Notification
function showToast(message, type = 'success') {
    const toastHTML = `
        <div class="alert alert-${type} alert-dismissible fade show" role="alert">
            ${message}
            <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
        </div>
    `;
    document.body.insertAdjacentHTML('afterbegin', toastHTML);

    setTimeout(() => {
        const toast = document.querySelector('.alert');
        if (toast) toast.remove();
    }, 3000);
}

// Format Date
function formatDate(dateString) {
    const options = { year: 'numeric', month: 'long', day: 'numeric', hour: '2-digit', minute: '2-digit' };
    return new Date(dateString).toLocaleDateString('en-US', options);
}

// Validate Email
function validateEmail(email) {
    const regex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    return regex.test(email);
}

// Password Strength Indicator
function checkPasswordStrength(password) {
    let strength = 0;
    if (password.length >= 8) strength++;
    if (/[a-z]/.test(password)) strength++;
    if (/[A-Z]/.test(password)) strength++;
    if (/[0-9]/.test(password)) strength++;
    if (/[^a-zA-Z0-9]/.test(password)) strength++;
    return strength;
}

// Navbar height + scroll behavior
(function () {
    function updateNavHeight() {
        var nav = document.getElementById('mainNavbar');
        if (!nav) return;
        var computed = window.getComputedStyle(nav);
        var height = 0;
        if (computed && computed.height) {
            height = Math.ceil(parseFloat(computed.height)) || 0;
        }
        if (!height) height = nav.offsetHeight || 0;
        document.documentElement.style.setProperty('--nav-height', height + 'px');
        try {
            document.body.style.paddingTop = height + 'px';
        } catch (e) {
            // ignore if body style cannot be set
        }
    }

    function onScroll() {
        var nav = document.getElementById('mainNavbar');
        if (!nav) return;
        if ((window.scrollY || window.pageYOffset) > 20) {
            nav.classList.add('navbar-scrolled');
        } else {
            nav.classList.remove('navbar-scrolled');
        }
    }

    var resizeTimer = null;
    function onResizeDebounced() {
        if (resizeTimer) clearTimeout(resizeTimer);
        resizeTimer = setTimeout(function () {
            updateNavHeight();
            onScroll();
        }, 100);
    }

    document.addEventListener('DOMContentLoaded', function () {
        updateNavHeight();
        onScroll();
        window.addEventListener('scroll', onScroll, { passive: true });
        window.addEventListener('resize', onResizeDebounced);
    });
})();