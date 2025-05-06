document.addEventListener('DOMContentLoaded', function() {
    const baseApiUrl = 'https://watchly.runasp.net/api/UsersAPI';
    const userJson = localStorage.getItem('loggedInUser');
    const usersTableBody = document.getElementById('usersTableBody');
    const statusMessage = document.getElementById('statusMessage');
    const editUserModal = new bootstrap.Modal(document.getElementById('editUserModal'));
    const editUserForm = document.getElementById('editUserForm');
    const saveUserChangesBtn = document.getElementById('saveUserChanges');
    
    // Check if user is admin
    if (!userJson) {
        window.location.href = 'login.html';
        return;
    }
    const LoggedInUser = JSON.parse(userJson);
    const loginLogoutBtn = document.querySelector('.log-btn');
    ReloadLoggedInUser();
    
    // Update UI based on login state
    if (LoggedInUser) {
        loginLogoutBtn.textContent = 'Logout';
        loginLogoutBtn.href = '#';
        loginLogoutBtn.onclick = () => {
            localStorage.removeItem('loggedInUser');
            window.location.href = 'login.html';
        };
        
        // Check if user has admin permissions (1 = admin, 2 = regular user)
        if (LoggedInUser.permissions !== 1 && LoggedInUser.permissions !== 3) {
            alert('You do not have permission to access this page.');
            window.location.href = 'main.html';
            return;
        }
    }
    
    // Load all users
    loadAllUsers();
    
    // Function to load all users
    function loadAllUsers() {
        fetch(`${baseApiUrl}/GetAllUsers`)
            .then(response => {
                if (!response.ok) throw new Error('Network response was not ok');
                return response.json();
            })
            .then(users => {
                displayUsers(users);
            })
            .catch(error => {
                console.error('Error:', error);
                usersTableBody.innerHTML = `<tr><td colspan="7" class="text-center py-5 text-danger">Error loading users. Please try again.</td></tr>`;
            });
    }
    
    // Function to display users in the table
    function displayUsers(users) {
        if (!users || users.length === 0) {
            usersTableBody.innerHTML = '<tr><td colspan="7" class="text-center py-5 text-muted">No users found</td></tr>';
            return;
        }
        
        usersTableBody.innerHTML = users.map(user => `
            <tr>
                <td>${user.id}</td>
                <td>${user.username}</td>
                <td>${user.email || 'N/A'}</td>
                <td><span class="user-status ${user.isAcive ? 'status-active' : 'status-inactive'}">
                    ${user.isAcive ? 'Active' : 'Inactive'}
                </span></td>
                <td><span class="permission-badge ${user.permissions === 1 ? 'permission-admin' : user.permissions === 3 ? "permission-owner": 'permission-user'}">

                    ${user.permissions === 1 ? 'Admin' : user.permissions === 3 ? 'Owner' : 'User'}
                </span></td>
                <td>${new Date(user.dateOfBirth).toLocaleDateString()}</td>
                <td class="action-buttons">
                    <button class="btn btn-sm btn-primary edit-user-btn" data-user-id="${user.id}">
                        <i class="bi bi-pencil-fill"></i> Edit
                    </button>
                    <button class="btn btn-sm btn-danger delete-user-btn" data-user-id="${user.id}" data-username="${user.username}">
                        <i class="bi bi-trash-fill"></i> Delete
                    </button>
                </td>
            </tr>
        `).join('');
        
        // Add event listeners to edit buttons
        document.querySelectorAll('.edit-user-btn').forEach(button => {
            button.addEventListener('click', function() {
                const userId = this.getAttribute('data-user-id');
                openEditModal(userId);
            });
        });
        document.querySelectorAll('.delete-user-btn').forEach(button => {
            button.addEventListener('click', function() {
                const userId = this.getAttribute('data-user-id');
                const username = this.getAttribute('data-username');
                confirmUserDeletion(userId, username);
            });
        // بعد دالة displayUsers مباشرةً، أضف هذا الكود لإضافة event listeners لأزرار الحذف
        });
    }

// أضف هذه الدالة لتأكيد الحذف
function confirmUserDeletion(userId, username) {
    if(userId == 22){
        return showStatusMessage('Cannot delete the Owner', 'danger');
    }

    if (confirm(`Are you sure you want to delete user: ${username}?`)) {
        deleteUser(userId);
    }
}

// أضف هذه الدالة لتنفيذ الحذف عبر API
function deleteUser(userId) {
    const loggedInUser = JSON.parse(localStorage.getItem('loggedInUser'));
    const deletedByUserId = loggedInUser.id;

    fetch(`${baseApiUrl}/DeleteUser/${userId}/${deletedByUserId}`, {
        method: 'DELETE',
        headers: {
            'Content-Type': 'application/json',
        }
    })
    .then(response => {
        if (!response.ok) throw new Error(errorMessage);
        // return response.json();
    })
    .then(() => {
        showStatusMessage('User deleted successfully', 'success');
        loadAllUsers(); // Refresh the table
    })
    .catch(error => {
        console.error('Error:', error);
        showStatusMessage('Failed to delete user: ' + error.message, 'danger');
    });
}
    function ReloadLoggedInUser() {
        fetch(`${baseApiUrl}/GetUserInfoByID/${LoggedInUser.id}`)
            .then(response => {
                if (!response.ok) throw new Error('Network response was not ok');
                return response.json();
            })
            .then(user => { 
                localStorage.setItem('loggedInUser', JSON.stringify(user));
                LoggedInUser = user;
            })
            .catch(error => {
                console.error('Error:', error);
                showStatusMessage('Error loading user data', 'danger');
            });
    }
    // Function to open edit modal with user data
    // في دالة openEditModal، بعد ملء بيانات المستخدم:
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
        fetch(`${baseApiUrl}/UpdateUser/${userId}`, {
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
        
        // Hide after 3 seconds
        setTimeout(() => {
            statusMessage.classList.remove('show');
            setTimeout(() => {
                statusMessage.classList.add('d-none');
            }, 300);
        }, 3000);
    }
    
    // Search functionality (same as main page)
    const searchInput = document.getElementById('searchInput');
    searchInput.addEventListener('input', debounce(handleSearch, 300));
    
    function handleSearch() {
        const query = searchInput.value.trim();
        const resultsContainer = document.getElementById('searchResults');
        
        if (query.length < 2) {
            resultsContainer.style.display = 'none';
            return;
        }
        
        fetch(`https://watchly.runasp.net/api/MovieRecommenderAPI/NameHasWord/${query}`)
            .then(response => {
                if (!response.ok) throw new Error('Network response was not ok');
                return response.json();
            })
            .then(movies => {
                showSearchResults(movies.slice(0, 5));
            })
            .catch(error => {
                console.error('Search error:', error);
                resultsContainer.innerHTML = '<div class="p-3 text-muted">Error loading results</div>';
                resultsContainer.style.display = 'block';
            });
    }
    
    function showSearchResults(movies) {
        const resultsContainer = document.getElementById('searchResults');
        
        if (!movies || movies.length === 0) {
            resultsContainer.innerHTML = '<div class="p-3 text-muted">No results found</div>';
            resultsContainer.style.display = 'block';
            return;
        }
        
        resultsContainer.innerHTML = movies.map(movie => `
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
        
        resultsContainer.style.display = 'block';
    }
    
    function debounce(func, wait) {
        let timeout;
        return function() {
            const context = this, args = arguments;
            clearTimeout(timeout);
            timeout = setTimeout(() => func.apply(context, args), wait);
        };
    }
    
    // Hide search results when clicking outside
    document.addEventListener('click', function(e) {
        if (!searchInput.contains(e.target) && !document.getElementById('searchResults').contains(e.target)) {
            document.getElementById('searchResults').style.display = 'none';
        }
    });
});