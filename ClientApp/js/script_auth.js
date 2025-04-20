document.addEventListener('DOMContentLoaded', function() {
    const baseApiUrl = 'https://localhost:7009/api/MovieRecommenderAPI';
    
    // Login Form Handling
    const loginForm = document.getElementById('loginForm');
    if (loginForm) {
        loginForm.addEventListener('submit', async function(e) {
            e.preventDefault();
            
            const username = document.getElementById('username').value;
            const password = document.getElementById('password').value;
            
            try {
                const response = await fetch(`${baseApiUrl}/CheckPasswordForUsername/${username}?Password=${password}`, {
                    method: 'GET'
                });
                
                if (response.ok) {
                    // Successful login
                    window.location.href = '../pages/main.html';
                } else {
                    // Handle error
                    const error = await response.text();
                    alert(error || 'Login failed. Please check your credentials.');
                }
            } catch (error) {
                console.error('Error during login:', error);
                alert('An error occurred during login. Please try again.');
            }
        });
    }
    
    // Signup Form Handling
    const signupForm = document.getElementById('signupForm');
    if (signupForm) {
        signupForm.addEventListener('submit', async function(e) {
            e.preventDefault();
            
            // Validate form
            const username = document.getElementById('username').value;
            const password = document.getElementById('password').value;
            const confirmPassword = document.getElementById('confirmPassword').value;
            const age = document.getElementById('age').value;
            
            // Client-side validation
            if (username.length < 3 || username.length > 20) {
                document.getElementById('username').classList.add('is-invalid');
                return;
            } else {
                document.getElementById('username').classList.remove('is-invalid');
            }
            
            if (password.length < 3 || password.length > 20) {
                document.getElementById('password').classList.add('is-invalid');
                return;
            } else {
                document.getElementById('password').classList.remove('is-invalid');
            }
            
            if (password !== confirmPassword) {
                document.getElementById('confirmPassword').classList.add('is-invalid');
                return;
            } else {
                document.getElementById('confirmPassword').classList.remove('is-invalid');
            }
            
            if (age < 1 || age > 120) {
                document.getElementById('age').classList.add('is-invalid');
                return;
            } else {
                document.getElementById('age').classList.remove('is-invalid');
            }
            
            // Prepare user data
            const userData = {
                Username: username,
                Password: password,
                Age: parseInt(age),
                IsAcive: true,
                Permissions: 1 // Default permissions
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
                    const user = await response.json();
                    alert('Account created successfully! Please login.');
                    window.location.href = 'login.html';
                } else {
                    const error = await response.text();
                    alert(error || 'Signup failed. Please try again.');
                }
            } catch (error) {
                console.error('Error during signup:', error);
                alert('An error occurred during signup. Please try again.');
            }
        });
    }
});