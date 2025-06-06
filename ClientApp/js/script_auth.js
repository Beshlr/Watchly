document.addEventListener('DOMContentLoaded', () => {
    const baseApiUrl = 'https://watchly.runasp.net/api/UsersAPI';
    const errorToast = new bootstrap.Toast(document.getElementById('errorToast'));
    const userJson = localStorage.getItem('loggedInUser');

    const user = JSON.parse(userJson);
    const loginLogoutBtn = document.querySelector('.log-btn');
    
    if (user) {
        loginLogoutBtn.textContent = 'Logout';
        loginLogoutBtn.href = '#';
        alert(`Welcome back, ${user.username}, you will be redirected to the main page`);
        window.location.href = 'main.html';
    }

    // إزالة رسائل الخطأ عند التركيز على الحقل
    function clearError(fieldId) {
        const field = document.getElementById(fieldId);
        if (field) {
            field.classList.remove('is-invalid');
            const feedback = field.nextElementSibling;
            if (feedback && feedback.classList.contains('invalid-feedback')) {
                feedback.style.display = 'none';
            }
        }
    }

    // إضافة مستمعات الأحداث للحقول
    function setupFieldValidation(fieldId, validationFn, errorMessage) {
        const field = document.getElementById(fieldId);
        if (field) {
            field.addEventListener('focus', () => clearError(fieldId));
            field.addEventListener('blur', () => {
                if (!validationFn(field.value)) {
                    showError(errorMessage, fieldId);
                }
            });
            field.addEventListener('input', () => {
                if (validationFn(field.value)) {
                    clearError(fieldId);
                }
            });
        }
    }

    // Login Handling
    const loginForm = document.getElementById('loginForm');
    if (loginForm) {
        // إعداد تحقق الحقول
        setupFieldValidation('username', 
            val => val.length >= 3 && val.length <= 20,
            'Username must be between 3-20 characters');
            
            setupFieldValidation('password',
                val => val.length >= 6 && !/^\d+$/.test(val),
                'Password must be at least 6 characters and cannot contain only numbers');

        loginForm.addEventListener('submit', async (e) => {
            e.preventDefault();
            let isValid = true;

            // التحقق من صحة الحقول
            const username = document.getElementById('username').value.trim();
            if (username.length < 3 || username.length > 20) {
                showError('Username must be between 3-20 characters', 'username');
                isValid = false;
            }

            const password = document.getElementById('password').value;
            if (password.length < 6) {
                showError('Password must be at least 6 characters', 'password');
                isValid = false;
            }

            if (!isValid) return;

            const userLoginData = {
                Username: username,
                Password: password
            };

            try {
                const response = await fetch(`${baseApiUrl}/CheckPasswordForUsername`, {
                    method: 'Post',
                    headers: {
                        'Content-Type': 'application/json'
                    },
                    body: JSON.stringify(userLoginData)
                });

                if (response.ok) {
                    const userData = await response.json();
                    localStorage.setItem('loggedInUser', JSON.stringify(userData));
                    window.location.href = '../pages/main.html';
                } else {
                    const error = await response.text();
                    showError(error);
                }
            } catch (error) {
                console.error('Login error:', error);
                showError('Network error. Please try again.');
            }
        });
    }

    // Signup Handling
    const signupForm = document.getElementById('signupForm');
    if (signupForm) {
        // إعداد تحقق الحقول
        setupFieldValidation('username', 
            val => val.length >= 3 && val.length <= 20,
            'Username must be between 3-20 characters');
            
        setupFieldValidation('password',
            val => val.length >= 6,
            'Password must be at least 6 characters');
            
        setupFieldValidation('confirmPassword',
            val => val === document.getElementById('password').value,
            'Passwords do not match');
            
        setupFieldValidation('dateOfBirth',
            val => {
                if (!val) return false;
                const dob = new Date(val);
                const today = new Date();
                const minDate = new Date();
                minDate.setFullYear(today.getFullYear() - 120); // 120 سنة كحد أقصى
                const maxDate = new Date();
                maxDate.setFullYear(today.getFullYear() - 1); // يجب أن يكون عمره سنة على الأقل
                return dob >= minDate && dob <= maxDate;
            },
            'Please enter a valid date of birth (you must be at least 1 year old)');
            
        setupFieldValidation('email',
            val => /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(val),
            'Please enter a valid email address');
            setupFieldValidation('password',
                val => val.length >= 6 && !/^\d+$/.test(val),
                'Password must be at least 6 characters and cannot contain only numbers');

        signupForm.addEventListener('submit', async (e) => {
            e.preventDefault();
            let isValid = true;

            // التحقق من صحة الحقول
            const username = document.getElementById('username').value.trim();
            if (username.length < 3 || username.length > 20) {
                showError('Username must be between 3-20 characters', 'username');
                isValid = false;
            }

            const password = document.getElementById('password').value;
            if (password.length < 6) {
                showError('Password must be at least 6 characters', 'password');
                isValid = false;
            }

            const confirmPassword = document.getElementById('confirmPassword').value;
            if (password !== confirmPassword) {
                showError('Passwords do not match', 'confirmPassword');
                isValid = false;
            }

            const dateOfBirth = document.getElementById('dateOfBirth').value;
            if (!dateOfBirth) {
                showError('Please enter your date of birth', 'dateOfBirth');
                isValid = false;
            } else {
                const dob = new Date(dateOfBirth);
                const today = new Date();
                let age = today.getFullYear() - dob.getFullYear();
                const monthDiff = today.getMonth() - dob.getMonth();
                
                if (monthDiff < 0 || (monthDiff === 0 && today.getDate() < dob.getDate())) {
                    age--;
                }
                
                if (age < 1 || age > 120) {
                    showError('You must be between 1 and 120 years old', 'dateOfBirth');
                    isValid = false;
                }
            }

            const email = document.getElementById('email').value.trim();
            if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email)) {
                showError('Please enter a valid email address', 'email');
                isValid = false;
            }

            if (!document.getElementById('terms').checked) {
                showError('You must agree to the terms and conditions');
                isValid = false;
            }

            if (!isValid) return;

            // حساب العمر من تاريخ الميلاد
            const dob = new Date(dateOfBirth);
            const today = new Date();
            let age = today.getFullYear() - dob.getFullYear();
            const monthDiff = today.getMonth() - dob.getMonth();
            
            if (monthDiff < 0 || (monthDiff === 0 && today.getDate() < dob.getDate())) {
                age--;
            }

            const userData = {
                Username: username,
                Password: password,
                Age: age,
                DateOfBirth: dateOfBirth,
                Email: email,
            };

            try {
                const response = await fetch(`${baseApiUrl}/AddNewUser`, {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json'
                    },
                    body: JSON.stringify(userData)
                });

                if (response.ok) {
                    showError('Account created successfully! Redirecting to login...');
                    setTimeout(() => {
                        window.location.href = 'login.html';
                    }, 2000);
                } else {
                    const error = await response.text();
                    showError(error);
                }
            } catch (error) {
                console.error('Login error:', error);
                showError('Network error. Please try again.');
            }
        });
    }

    function showError(message, fieldId = null) {
        // إذا كان هناك حقل محدد، عرض الخطأ تحته
        if (fieldId) {
            const field = document.getElementById(fieldId);
            if (field) {
                field.classList.add('is-invalid');
                const feedback = field.nextElementSibling;
                if (feedback && feedback.classList.contains('invalid-feedback')) {
                    feedback.textContent = message;
                    feedback.style.display = 'block';
                }
            }
        } else {
            // عرض الخطأ في Toast كحالة افتراضية
            const errorToastBody = document.getElementById('errorToastBody');
            if (errorToastBody) {
                errorToastBody.textContent = message;
                errorToast.show();
            }
        }
    }
});