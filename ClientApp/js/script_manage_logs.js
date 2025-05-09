document.addEventListener('DOMContentLoaded', function() {
    const baseApiUrl = 'https://watchly.runasp.net/api/LogsAPI';
    const userJson = localStorage.getItem('loggedInUser');
    const logsTableBody = document.getElementById('logsTableBody');
    const statusMessage = document.getElementById('statusMessage');
    const userFilter = document.getElementById('userFilter');
    const refreshLogsBtn = document.getElementById('refreshLogs');
    const currentUserIndicator = document.getElementById('currentUserIndicator');

    if (!userJson) {
        window.location.href = 'login.html';
        return;
    }

    let LoggedInUser = JSON.parse(userJson);
    const loginLogoutBtn = document.getElementById('log-btn');

    updateLoginUI();
    loadAllUsernames();
    loadAllLogs();

    function updateLoginUI() {
        if (LoggedInUser) {
            loginLogoutBtn.textContent = 'Logout';
            loginLogoutBtn.href = '#';
            loginLogoutBtn.onclick = () => {
                localStorage.removeItem('loggedInUser');
                window.location.href = 'login.html';
            };

           

            
        }
    }

    function getPermissionName(permission) {
        switch(permission) {
            case 1: return 'Admin';
            case 3: return 'Owner';
            default: return 'User';
        }
    }

    function getPermissionBadgeClass(permission) {
        switch(permission) {
            case 1: return 'bg-primary';
            case 3: return 'bg-success';
            default: return 'bg-secondary';
        }
    }

    function loadAllUsernames() {
        fetch('https://watchly.runasp.net/api/UsersAPI/GetAllUsernames')
            .then(response => {
                if (!response.ok) throw new Error('Network response was not ok');
                return response.json();
            })
            .then(usernames => {
                while (userFilter.options.length > 1) {
                    userFilter.remove(1);
                }
                usernames.forEach(username => {
                    const option = document.createElement('option');
                    option.value = username;
                    option.textContent = username;
                    userFilter.appendChild(option);
                });
            })
            .catch(error => {
                console.error('Error loading usernames:', error);
                showStatusMessage('Failed to load usernames: ' + error.message, 'danger');
            });
    }

    function loadAllLogs() {
        showLoadingState(true);

        const selectedUser = userFilter.value;
        let apiUrl = `${baseApiUrl}/GetAllLogs`;

        if (selectedUser !== 'all') {
            apiUrl = `${baseApiUrl}/GetAllLogsForUser/${selectedUser}`;
        }

        fetch(apiUrl)
            .then(response => {
                if (!response.ok) throw new Error('Network response was not ok');
                return response.json();
            })
            .then(logs => {
                displayLogs(logs);
                showLoadingState(false);
            })
            .catch(error => {
                console.error('Error:', error);
                logsTableBody.innerHTML = `<tr><td colspan="6" class="text-center py-5 text-danger">Error loading logs. Please try again.</td></tr>`;
                showLoadingState(false);
                showStatusMessage('Failed to load logs: ' + error.message, 'danger');
            });
    }

    function showLoadingState(show) {
        if (show) {
            logsTableBody.innerHTML = `
                <tr>
                    <td colspan="6" class="text-center py-5">
                        <div class="spinner-border text-primary" role="status">
                            <span class="visually-hidden">Loading...</span>
                        </div>
                    </td>
                </tr>
            `;
        }
    }

    function displayLogs(logs) {
        if (!logs || logs.length === 0) {
            logsTableBody.innerHTML = '<tr><td colspan="6" class="text-center py-5 text-muted">No logs found</td></tr>';
            return;
        }

        logsTableBody.innerHTML = logs.map(log => `
            <tr ${log.UserID === LoggedInUser.id ? 'class="current-user-row"' : ''}>
                <td>${log.logID}</td>
                <td>${log.userID}</td>
                <td>${log.username || 'N/A'}</td>
                <td>${log.action || 'N/A'}</td>
                <td>${formatDateTime(log.dateTime)}</td>
                <td class="action-cell">
                    <button class="btn btn-sm btn-danger delete-log-btn" data-log-id="${log.logID}">
                        <i class="bi bi-trash-fill"></i> Delete
                    </button>
                </td>
            </tr>
        `).join('');

        document.querySelectorAll('.delete-log-btn').forEach(button => {
            button.addEventListener('click', function() {
                const logId = this.getAttribute('data-log-id');
                confirmLogDeletion(logId);
            });
        });
    }

    function formatDateTime(dateTimeString) {
        const date = new Date(dateTimeString);
        return isNaN(date) ? 'Invalid Date' : date.toLocaleString();
    }

    function confirmLogDeletion(logId) {
        if (confirm(`Are you sure you want to delete this log with id ${logId}?`)) {
            deleteLog(logId);
        }
    }

    function deleteLog(logId) {
        fetch(`${baseApiUrl}/DeleteLog/${logId}`, {
            method: 'DELETE'
        })
        .then(response => {
            if (!response.ok) throw new Error('Failed to delete log');
            return response.text();
        })
        .then(() => {
            showStatusMessage('Log deleted successfully', 'success');
            loadAllLogs();
        })
        .catch(error => {
            console.error('Error:', error);
            showStatusMessage('Failed to delete log: ' + error.message, 'danger');
        });
    }

    function showStatusMessage(message, type) {
        statusMessage.textContent = message;
        statusMessage.className = `alert alert-${type} show`;
        statusMessage.style.display = 'block';

        setTimeout(() => {
            statusMessage.classList.remove('show');
            statusMessage.style.opacity = '0';
            setTimeout(() => {
                statusMessage.textContent = '';
                statusMessage.style.display = 'none';
                statusMessage.style.opacity = '1';
            }, 300);
        }, 3000);
    }

    userFilter.addEventListener('change', loadAllLogs);
    refreshLogsBtn.addEventListener('click', loadAllLogs);

    const searchInput = document.getElementById('searchInput');
    const searchResults = document.getElementById('searchResults');

    if (searchInput && searchResults) {
        searchInput.addEventListener('input', debounce(handleSearch, 300));

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
