document.addEventListener('DOMContentLoaded', function () {
    const loginForm = document.getElementById('loginForm');

    if (loginForm) {
        loginForm.addEventListener('submit', async function (e) {
            e.preventDefault();

            const email = document.getElementById('email').value;
            const password = document.getElementById('password').value;
            const errorMessage = document.getElementById('errorMessage');

            try {
                const response = await fetch('/api/auth/login', {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json'
                    },
                    body: JSON.stringify({ email, password })
                });

                if (response.ok) {
                    const data = await response.json();
                    localStorage.setItem('token', data.token);
                    localStorage.setItem('userEmail', data.email);
                    window.location.href = '/Courses';
                } else {
                    const error = await response.json();
                    errorMessage.textContent = error.message || 'Login failed';
                    errorMessage.classList.remove('d-none');
                }
            } catch (error) {
                errorMessage.textContent = 'An error occurred';
                errorMessage.classList.remove('d-none');
            }
        });
    }
});

function setupRegisterForm() {
    const registerForm = document.getElementById('registerForm');
    if (registerForm) {
        registerForm.addEventListener('submit', async function (e) {
            e.preventDefault();

            const firstName = document.getElementById('firstName').value;
            const lastName = document.getElementById('lastName').value;
            const email = document.getElementById('email').value;
            const password = document.getElementById('password').value;
            const confirmPassword = document.getElementById('confirmPassword').value;
            const errorMessage = document.getElementById('errorMessage');

            if (password !== confirmPassword) {
                errorMessage.textContent = 'Passwords do not match';
                errorMessage.classList.remove('d-none');
                return;
            }

            try {
                const response = await fetch('/api/auth/register', {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json'
                    },
                    body: JSON.stringify({ firstName, lastName, email, password, confirmPassword })
                });

                if (response.ok) {
                    // Auto login or redirect to login
                    alert('Registration successful! Please login.');
                    window.location.href = '/auth/login';
                } else {
                    const error = await response.json();
                    errorMessage.textContent = error.message || 'Registration failed';
                    errorMessage.classList.remove('d-none');
                }
            } catch (error) {
                errorMessage.textContent = 'An error occurred';
                errorMessage.classList.remove('d-none');
            }
        });
    }
}

function logout() {
    localStorage.removeItem('token');
    localStorage.removeItem('userEmail');
    window.location.href = '/auth/login';
}

function getAuthHeaders() {
    const token = localStorage.getItem('token');
    return {
        'Authorization': `Bearer ${token}`,
        'Content-Type': 'application/json'
    };
}

// Check auth on protected pages
function checkAuth() {
    const token = localStorage.getItem('token');
    if (!token && !window.location.pathname.includes('/auth/login')) {
        window.location.href = '/auth/login';
    }
}
