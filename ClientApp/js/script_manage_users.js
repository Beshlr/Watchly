document.addEventListener('DOMContentLoaded', function() {
    const baseApiUrl = 'https://watchly.runasp.net/api/UsersAPI';
    const userJson = localStorage.getItem('loggedInUser');
    const usersTableBody = document.getElementById('usersTableBody');
    const statusMessage = document.getElementById('statusMessage');
    const editUserModal = new bootstrap.Modal(document.getElementById('editUserModal'));
    const editUserForm = document.getElementById('editUserForm');
    const saveUserChangesBtn = document.getElementById('saveUserChanges');
    const currentUserIndicator = document.getElementById('currentUserIndicator');
    
    // Check if user is logged in
    if (!userJson) {
        window.location.href = 'login.html';
        return;
    }
    
    let LoggedInUser = JSON.parse(userJson);
    const loginLogoutBtn = document.getElementById('log-btn');
    
    // Update UI based on login state
    updateLoginUI();
    reloadLoggedInUser();
    loadAllUsers();
    
    // Function to update login UI
    function updateLoginUI() {
        if (LoggedInUser) {
            // Update login/logout button
            loginLogoutBtn.textContent = 'Logout';
            loginLogoutBtn.href = '#';
            loginLogoutBtn.onclick = () => {
                localStorage.removeItem('loggedInUser');
                window.location.href = 'login.html';
            };
            
        }
    }
    
    // Helper functions for permissions
    function getPermissionName(permission) {
        switch(permission) {
            case 1: return 'Admin';
            case 3: return 'Owner';
            default: return 'User';
        }
    }
    
    function getPermissionBadgeClass(permission) {
        switch(permission) {
            case 1: return 'permission-admin';
            case 3: return 'permission-owner';
            default: return 'permission-user';
        }
    }
    
    // Function to load all users
    function loadAllUsers() {
        showLoadingState(true);
        
        fetch(`${baseApiUrl}/GetAllUsers`)
            .then(response => {
                if (!response.ok) throw new Error('Network response was not ok');
                return response.json();
            })
            .then(users => {
                displayUsers(users);
                showLoadingState(false);
            })
            .catch(error => {
                console.error('Error:', error);
                usersTableBody.innerHTML = `<tr><td colspan="7" class="text-center py-5 text-danger">Error loading users. Please try again.</td></tr>`;
                showLoadingState(false);
                showStatusMessage('Failed to load users: ' + error.message, 'danger');
            });
    }
    
    // Function to show/hide loading state
    function showLoadingState(show) {
        if (show) {
            usersTableBody.innerHTML = `
                <tr>
                    <td colspan="7" class="text-center py-5">
                        <div class="spinner-border text-primary" role="status">
                            <span class="visually-hidden">Loading...</span>
                        </div>
                    </td>
                </tr>
            `;
        }
    }
    
    // Function to display users in the table
    function displayUsers(users) {
        if (!users || users.length === 0) {
            usersTableBody.innerHTML = '<tr><td colspan="7" class="text-center py-5 text-muted">No users found</td></tr>';
            return;
        }
        
        usersTableBody.innerHTML = users.map(user => `
            <tr ${user.id === LoggedInUser.id ? 'class="current-user-row"' : ''}>
                <td>${user.id}</td>
                <td>${user.username}</td>
                <td>${user.email || 'N/A'}</td>
                <td><span class="user-status ${user.isAcive ? 'status-active' : 'status-inactive'}">
                    ${user.isAcive ? 'Active' : 'Inactive'}
                </span></td>
                <td><span class="permission-badge ${getPermissionBadgeClass(user.permissions)}">
                    ${getPermissionName(user.permissions)}
                </span></td>
                <td>${user.dateOfBirth ? new Date(user.dateOfBirth).toLocaleDateString() : 'N/A'}</td>
                <td class="action-buttons">
                    <button class="btn btn-sm btn-primary edit-user-btn" data-user-id="${user.id}" 
                        ${user.id === LoggedInUser.id && LoggedInUser.permissions !== 3 ? 'disabled' : ''}>
                        <i class="bi bi-pencil-fill"></i> Edit
                    </button>
                    <button class="btn btn-sm btn-danger delete-user-btn" data-user-id="${user.id}" 
                        data-username="${user.username}" 
                        ${user.id === LoggedInUser.id || user.permissions === 3 ? 'disabled' : ''}>
                        <i class="bi bi-trash-fill"></i> Delete
                    </button>
                </td>
            </tr>
        `).join('');

        document.querySelectorAll('.permission-owner').forEach(element => {
            const width = element.offsetWidth;
            element.style.setProperty('--owner-width', `${width}px`);
        });

        // Add event listeners to edit buttons
        document.querySelectorAll('.edit-user-btn:not(:disabled)').forEach(button => {
            button.addEventListener('click', function() {
                const userId = this.getAttribute('data-user-id');
                openEditModal(userId);
            });
        });
        
        // Add event listeners to delete buttons
        document.querySelectorAll('.delete-user-btn:not(:disabled)').forEach(button => {
            button.addEventListener('click', function() {
                const userId = this.getAttribute('data-user-id');
                const username = this.getAttribute('data-username');
                confirmUserDeletion(userId, username);
            });
        });
    }

    // Function to confirm user deletion
    function confirmUserDeletion(userId, username) {
        if (confirm(`Are you sure you want to delete user: ${username}?`)) {
            deleteUser(userId);
        }
    }

    // Function to delete user via API
    function deleteUser(userId) {
        const loggedInUser = JSON.parse(localStorage.getItem('loggedInUser'));
        const deletedByUserId = loggedInUser.id;

        fetch(`${baseApiUrl}/GetUserInfoByID/${userId}`)
            .then(response => {
                if (!response.ok) throw new Error('Failed to fetch user info');
                return response.json();
            })
            .then(userToDelete => {
                if (userToDelete.permissions === 3) {
                    throw new Error('Cannot delete the Owner');
                }
                
                if (userToDelete.permissions === 1 && loggedInUser.permissions !== 3) {
                    throw new Error('Only Owners can delete Admins');
                }

                return fetch(`${baseApiUrl}/DeleteUser/${userId}/${deletedByUserId}`, {
                    method: 'DELETE',
                    headers: {
                        'Content-Type': 'application/json',
                    }
                });
            })
            .then(response => {
                if (!response.ok) throw new Error('Failed to delete user');
                showStatusMessage('User deleted successfully', 'success');
                loadAllUsers(); // Refresh the table
            })
            .catch(error => {
                console.error('Error:', error);
                showStatusMessage(error.message, 'danger');
            });
    }

    // Function to reload logged in user data
    function reloadLoggedInUser() {
        if (!LoggedInUser || !LoggedInUser.id) {
            console.error('LoggedInUser is not defined');
            showStatusMessage('User is not logged in', 'danger');
            return;
        }

        fetch(`${baseApiUrl}/GetUserInfoByID/${LoggedInUser.id}`)
            .then(response => {
                if (!response.ok) throw new Error('Failed to fetch user info');
                return response.json();
            })
            .then(user => {
                localStorage.setItem('loggedInUser', JSON.stringify(user));
                LoggedInUser = user;
                updateLoginUI();
            })
            .catch(error => {
                console.error('Error:', error);
                showStatusMessage(error.message || 'Error loading user data', 'danger');
            });
    }

    // Function to open edit modal with user data
    function openEditModal(userId) {
        fetch(`${baseApiUrl}/GetUserInfoByID/${userId}`)
            .then(response => {
                if (!response.ok) throw new Error('Network response was not ok');
                return response.json();
            })
            .then(user => {
                if(user.permissions == 3 && LoggedInUser.permissions != 3){
                    return showStatusMessage('Only Owners can edit themself', 'danger');
                }
                
                // Fill the form with user data
                document.getElementById('editUserId').value = user.id;
                document.getElementById('editUsername').value = user.username;
                document.getElementById('editEmail').value = user.email || '';
                document.getElementById('editStatus').value = user.isAcive;
                
                // Get permissions select element
                const permissionsSelect = document.getElementById('editPermissions');
                
                // Clear existing options
                permissionsSelect.innerHTML = '';
                
                // Add basic options
                permissionsSelect.innerHTML = `
                    <option value="2">Regular User</option>
                    <option value="1">Admin</option>
                `;
                
                // Add Owner option if current user is Owner
                if (LoggedInUser.permissions === 3) {
                    const ownerOption = document.createElement('option');
                    ownerOption.value = '3';
                    ownerOption.textContent = 'Owner';
                    permissionsSelect.appendChild(ownerOption);
                }
                
                // Set the current value
                permissionsSelect.value = user.permissions;
                
                // Format date for the date input (YYYY-MM-DD)
                const dateOfBirth = new Date(user.dateOfBirth);
                const formattedDate = dateOfBirth.toISOString().split('T')[0];
                document.getElementById('editDateOfBirth').value = formattedDate;
                
                // Show the modal
                editUserModal.show();
            })
            .catch(error => {
                console.error('Error:', error);
                showStatusMessage('Error loading user data', 'danger');
            });
    }
    
    // Save user changes
    saveUserChangesBtn.addEventListener('click', function() {
        const userId = document.getElementById('editUserId').value;
        const updatedUser = {
            username: document.getElementById('editUsername').value,
            email: document.getElementById('editEmail').value,
            isAcive: document.getElementById('editStatus').value === 'true',
            permissions: parseInt(document.getElementById('editPermissions').value),
            dateOfBirth: document.getElementById('editDateOfBirth').value,
        };
        
        if (!updatedUser.username || !updatedUser.dateOfBirth) {
            showStatusMessage('Username and Date of Birth are required', 'danger');
            return;
        }
        
        updateUser(userId, updatedUser);
    });
    
    // Function to update user
    function updateUser(userId, userData) {
        if (userData.permissions === 3 && LoggedInUser.permissions !== 3) {
            showStatusMessage('Only Owners can change owner permissions', 'danger');
            return;
        }
        
        fetch(`${baseApiUrl}/UpdateUser/${userId}/${LoggedInUser.id}`, {
            method: 'PUT',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify(userData)
        })
        .then(response => {
            if (!response.ok) throw new Error('Network response was not ok');
            return response.json();
        })
        .then(updatedUser => {
            editUserModal.hide();
            showStatusMessage('User updated successfully', 'success');
            
            // If editing current user, reload their data
            if (userId == LoggedInUser.id) {
                reloadLoggedInUser();
            }
            
            loadAllUsers(); // Refresh the table
        })
        .catch(error => {
            console.error('Error:', error);
            showStatusMessage('Failed to update user: ' + error.message, 'danger');
        });
    }
    
    // Function to show status message
    function showStatusMessage(message, type) {
        statusMessage.textContent = message;
        statusMessage.className = `alert alert-${type} show`;
        statusMessage.style.display = 'block'; // أضف هذا السطر
        
        // Hide after 3 seconds
        setTimeout(() => {
            statusMessage.classList.remove('show');
            statusMessage.style.opacity = '0'; // أضف تأثير fade-out
            setTimeout(() => {
                statusMessage.textContent = '';
                statusMessage.style.display = 'none'; // أخفِ العنصر تماماً
                statusMessage.style.opacity = '1'; // أعِد ضبط العتامة
            }, 300);
        }, 3000);
    }
    
    // Search functionality
    const searchInput = document.getElementById('searchInput');
    const searchResults = document.getElementById('searchResults');
    
    if (searchInput && searchResults) {
        searchInput.addEventListener('input', debounce(handleSearch, 300));
        
        // Hide search results when clicking outside
        document.addEventListener('click', function(e) {
            if (!searchInput.contains(e.target) && !searchResults.contains(e.target)) {
                searchResults.style.display = 'none';
            }
        });
    }
    
    function handleSearch() {
        const query = searchInput.value.trim();
        
        if (query.length < 2) {
            searchResults.style.display = 'none';
            return;
        }
        
        fetch(`https://watchly.runasp.net/api/MovieRecommenderAPI/NameHasWord/${query}/${LoggedInUser.id}`)
            .then(response => {
                if (!response.ok) throw new Error('Network response was not ok');
                return response.json();
            })
            .then(movies => {
                showSearchResults(movies.slice(0, 5));
            })
            .catch(error => {
                console.error('Search error:', error);
                searchResults.innerHTML = '<div class="p-3 text-muted">Error loading results</div>';
                searchResults.style.display = 'block';
            });
    }
    
    function showSearchResults(movies) {
        if (!movies || movies.length === 0) {
            searchResults.innerHTML = '<div class="p-3 text-muted">No results found</div>';
            searchResults.style.display = 'block';
            return;
        }
        
        searchResults.innerHTML = movies.map(movie => `
            <div class="search-result-item" onclick="window.open('${movie.imDbMovieURL || '#'}', '_blank')">
                <img src="${movie.posterImageURL || 'https://via.placeholder.com/50x75'}" 
                     alt="${movie.movieName}"
                     onerror="this.src='https://via.placeholder.com/50x75'">
                <div class="info">
                    <h6>${movie.movieName}</h6>
                    <small class="text-muted">${movie.year} • ⭐ ${movie.rate?.toFixed(1) || 'N/A'}</small>
                </div>
            </div>
        `).join('');
        
        searchResults.style.display = 'block';
    }
    
    function debounce(func, wait) {
        let timeout;
        return function() {
            const context = this, args = arguments;
            clearTimeout(timeout);
            timeout = setTimeout(() => func.apply(context, args), wait);
        };
    }
});