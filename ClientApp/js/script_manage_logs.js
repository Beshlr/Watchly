class LogsManager {
    constructor() {
        this.baseApiUrl = 'https://watchly.runasp.net/api/LogsAPI';
        this.currentPage = 1;
        this.logsPerPage = 20;
        this.totalLogs = 0;
        this.allLogs = [];
        this.loggedInUser = null;
        this.paginationInitialized = false;

        this.initElements();
        this.checkAuth();
        this.initEventListeners();
        this.loadInitialData();
    }

    initElements() {
        this.elements = {
            logsTableBody: document.getElementById('logsTableBody'),
            statusMessage: document.getElementById('statusMessage'),
            statusMessageText: document.getElementById('statusMessageText'),
            userFilter: document.getElementById('userFilter'),
            refreshLogsBtn: document.getElementById('refreshLogs'),
            paginationContainer: document.getElementById('paginationContainer'),
            loginLogoutBtn: document.getElementById('log-btn'),
            searchInput: document.getElementById('searchInput'),
            searchResults: document.getElementById('searchResults')
        };
    }

    checkAuth() {
        const userJson = localStorage.getItem('loggedInUser');
        if (!userJson) {
            window.location.href = 'login.html';
            return;
        }
        this.loggedInUser = JSON.parse(userJson);
        this.updateLoginUI();
    }

    updateLoginUI() {
        if (this.loggedInUser) {
            this.elements.loginLogoutBtn.textContent = 'Logout';
            this.elements.loginLogoutBtn.href = '#';
            this.elements.loginLogoutBtn.onclick = () => {
                localStorage.removeItem('loggedInUser');
                window.location.href = 'login.html';
            };
        }
    }

    initEventListeners() {
        this.elements.userFilter.addEventListener('change', () => {
            this.currentPage = 1;
            this.loadLogs();
        });

        this.elements.refreshLogsBtn.addEventListener('click', () => {
            this.currentPage = 1;
            this.loadLogs();
        });

        if (this.elements.searchInput && this.elements.searchResults) {
            this.elements.searchInput.addEventListener('input', this.debounce(this.handleSearch.bind(this), 300));
            
            document.addEventListener('click', (e) => {
                if (!this.elements.searchInput.contains(e.target) && 
                    !this.elements.searchResults.contains(e.target)) {
                    this.elements.searchResults.style.display = 'none';
                }
            });
        }
    }

    loadInitialData() {
        this.loadAllUsernames();
        this.loadLogs();
    }

    async loadAllUsernames() {
        try {
            const response = await fetch('https://watchly.runasp.net/api/UsersAPI/GetAllUsernames');
            if (!response.ok) throw new Error('Network response was not ok');
            
            const usernames = await response.json();
            this.populateUserFilter(usernames);
        } catch (error) {
            console.error('Error loading usernames:', error);
            this.showStatusMessage('Failed to load usernames: ' + error.message, 'danger');
        }
    }

    populateUserFilter(usernames) {
        // Clear existing options except "All Users"
        while (this.elements.userFilter.options.length > 1) {
            this.elements.userFilter.remove(1);
        }
        
        usernames.forEach(username => {
            const option = document.createElement('option');
            option.value = username;
            option.textContent = username;
            this.elements.userFilter.appendChild(option);
        });
    }

    async loadLogs() {
        this.showLoading(true);
        
        try {
            const selectedUser = this.elements.userFilter.value;
            let apiUrl = `${this.baseApiUrl}/GetAllLogs`;
            
            if (selectedUser !== 'all') {
                apiUrl = `${this.baseApiUrl}/GetAllLogsForUser/${selectedUser}`;
            }
            
            const response = await fetch(apiUrl);
            if (!response.ok) throw new Error('Network response was not ok');
            
            this.allLogs = await response.json();
            this.totalLogs = this.allLogs.length;
            
            this.displayLogs();
            this.updatePaginationUI();
            this.showLoading(false);
        } catch (error) {
            console.error('Error:', error);
            this.elements.logsTableBody.innerHTML = `
                <tr><td colspan="6" class="text-center py-5 text-danger">
                    Error loading logs. Please try again.
                </td></tr>
            `;
            this.showLoading(false);
            this.showStatusMessage('Failed to load logs: ' + error.message, 'danger');
        }
    }

    displayLogs() {
        const startIndex = (this.currentPage - 1) * this.logsPerPage;
        const endIndex = Math.min(startIndex + this.logsPerPage, this.totalLogs);
        const logsToDisplay = this.allLogs.slice(startIndex, endIndex);

        if (!logsToDisplay || logsToDisplay.length === 0) {
            this.elements.logsTableBody.innerHTML = `
                <tr><td colspan="6" class="text-center py-5 text-muted">
                    No logs found
                </td></tr>
            `;
            return;
        }

        this.elements.logsTableBody.innerHTML = logsToDisplay.map(log => `
            <tr ${log.UserID === this.loggedInUser.id ? 'class="table-primary"' : ''}>
                <td>${log.logID}</td>
                <td>${log.userID}</td>
                <td>${log.username || 'N/A'}</td>
                <td>${log.action || 'N/A'}</td>
                <td>${this.formatDateTime(log.dateTime)}</td>
                <td class="action-cell">
                    <button class="btn btn-sm btn-danger delete-log-btn" data-log-id="${log.logID}">
                        <i class="bi bi-trash-fill"></i> Delete
                    </button>
                </td>
            </tr>
        `).join('');

        // Add event listeners to delete buttons
        document.querySelectorAll('.delete-log-btn').forEach(button => {
            button.addEventListener('click', (e) => {
                const logId = e.currentTarget.getAttribute('data-log-id');
                this.confirmLogDeletion(logId);
            });
        });
    }

    updatePaginationUI() {
        if (!this.elements.paginationContainer) return;
        
        const totalPages = Math.ceil(this.totalLogs / this.logsPerPage);
        
        // Clear existing pagination
        this.elements.paginationContainer.innerHTML = '';
        
        // Previous button
        const prevLi = document.createElement('li');
        prevLi.className = `page-item ${this.currentPage === 1 ? 'disabled' : ''}`;
        const prevButton = document.createElement('button');
        prevButton.className = 'page-link';
        prevButton.textContent = 'Previous';
        prevButton.addEventListener('click', (e) => {
            e.preventDefault();
            this.changePage(this.currentPage - 1);
        });
        prevLi.appendChild(prevButton);
        this.elements.paginationContainer.appendChild(prevLi);
        
        // Page numbers
        const maxVisiblePages = 5;
        let startPage = Math.max(1, this.currentPage - Math.floor(maxVisiblePages / 2));
        let endPage = Math.min(totalPages, startPage + maxVisiblePages - 1);
        
        if (endPage - startPage + 1 < maxVisiblePages) {
            startPage = Math.max(1, endPage - maxVisiblePages + 1);
        }
        
        // First page and ellipsis if needed
        if (startPage > 1) {
            const firstLi = document.createElement('li');
            firstLi.className = `page-item ${1 === this.currentPage ? 'active' : ''}`;
            const firstButton = document.createElement('button');
            firstButton.className = 'page-link';
            firstButton.textContent = '1';
            firstButton.addEventListener('click', (e) => {
                e.preventDefault();
                this.changePage(1);
            });
            firstLi.appendChild(firstButton);
            this.elements.paginationContainer.appendChild(firstLi);
            
            if (startPage > 2) {
                const ellipsisLi = document.createElement('li');
                ellipsisLi.className = 'page-item disabled';
                const ellipsisSpan = document.createElement('span');
                ellipsisSpan.className = 'page-link';
                ellipsisSpan.textContent = '...';
                ellipsisLi.appendChild(ellipsisSpan);
                this.elements.paginationContainer.appendChild(ellipsisLi);
            }
        }
        
        // Page numbers
        for (let i = startPage; i <= endPage; i++) {
            const pageLi = document.createElement('li');
            pageLi.className = `page-item ${i === this.currentPage ? 'active' : ''}`;
            const pageButton = document.createElement('button');
            pageButton.className = 'page-link';
            pageButton.textContent = i;
            pageButton.addEventListener('click', (e) => {
                e.preventDefault();
                this.changePage(i);
            });
            pageLi.appendChild(pageButton);
            this.elements.paginationContainer.appendChild(pageLi);
        }
        
        // Last page and ellipsis if needed
        if (endPage < totalPages) {
            if (endPage < totalPages - 1) {
                const ellipsisLi = document.createElement('li');
                ellipsisLi.className = 'page-item disabled';
                const ellipsisSpan = document.createElement('span');
                ellipsisSpan.className = 'page-link';
                ellipsisSpan.textContent = '...';
                ellipsisLi.appendChild(ellipsisSpan);
                this.elements.paginationContainer.appendChild(ellipsisLi);
            }
            
            const lastLi = document.createElement('li');
            lastLi.className = `page-item ${totalPages === this.currentPage ? 'active' : ''}`;
            const lastButton = document.createElement('button');
            lastButton.className = 'page-link';
            lastButton.textContent = totalPages;
            lastButton.addEventListener('click', (e) => {
                e.preventDefault();
                this.changePage(totalPages);
            });
            lastLi.appendChild(lastButton);
            this.elements.paginationContainer.appendChild(lastLi);
        }
        
        // Next button
        const nextLi = document.createElement('li');
        nextLi.className = `page-item ${this.currentPage === totalPages ? 'disabled' : ''}`;
        const nextButton = document.createElement('button');
        nextButton.className = 'page-link';
        nextButton.textContent = 'Next';
        nextButton.addEventListener('click', (e) => {
            e.preventDefault();
            this.changePage(this.currentPage + 1);
        });
        nextLi.appendChild(nextButton);
        this.elements.paginationContainer.appendChild(nextLi);
    }

    changePage(newPage) {
        if (newPage < 1 || newPage > Math.ceil(this.totalLogs / this.logsPerPage)) {
            return;
        }
        
        this.currentPage = newPage;
        this.displayLogs();
        this.updatePaginationUI(); // This line was missing in the original
        
        // Scroll to top smoothly
        window.scrollTo({
            top: 0,
            behavior: 'smooth'
        });
    }

    formatDateTime(dateTimeString) {
        const date = new Date(dateTimeString);
        return isNaN(date) ? 'Invalid Date' : date.toLocaleString();
    }

    confirmLogDeletion(logId) {
        if (confirm(`Are you sure you want to delete log with ID ${logId}?`)) {
            this.deleteLog(logId);
        }
    }

    async deleteLog(logId) {
        try {
            const response = await fetch(`${this.baseApiUrl}/DeleteLog/${logId}`, {
                method: 'DELETE'
            });
            
            if (!response.ok) throw new Error('Failed to delete log');
            
            await response.text();
            this.showStatusMessage('Log deleted successfully', 'success');
            this.loadLogs();
        } catch (error) {
            console.error('Error:', error);
            this.showStatusMessage('Failed to delete log: ' + error.message, 'danger');
        }
    }

    showStatusMessage(message, type) {
        this.elements.statusMessageText.textContent = message;
        this.elements.statusMessage.className = `alert alert-${type} alert-dismissible fade show`;
        this.elements.statusMessage.style.display = 'block';
        
        setTimeout(() => {
            const alert = new bootstrap.Alert(this.elements.statusMessage);
            alert.close();
        }, 5000);
    }

    showLoading(show) {
        if (show) {
            this.elements.logsTableBody.innerHTML = `
                <tr><td colspan="6" class="text-center py-5">
                    <div class="spinner-border text-primary" role="status">
                        <span class="visually-hidden">Loading...</span>
                    </div>
                </td></tr>
            `;
        }
    }

    // Search functionality
    async handleSearch() {
        const query = this.elements.searchInput.value.trim();
        
        if (query.length < 2) {
            this.elements.searchResults.style.display = 'none';
            return;
        }
        
        try {
            const response = await fetch(
                `https://watchly.runasp.net/api/MovieRecommenderAPI/NameHasWord/${query}/${this.loggedInUser.id}`
            );
            
            if (!response.ok) throw new Error('Network response was not ok');
            
            const movies = await response.json();
            this.showSearchResults(movies.slice(0, 5));
        } catch (error) {
            console.error('Search error:', error);
            this.elements.searchResults.innerHTML = '<div class="p-3 text-muted">Error loading results</div>';
            this.elements.searchResults.style.display = 'block';
        }
    }

    showSearchResults(movies) {
        if (!movies || movies.length === 0) {
            this.elements.searchResults.innerHTML = '<div class="p-3 text-muted">No results found</div>';
            this.elements.searchResults.style.display = 'block';
            return;
        }

        this.elements.searchResults.innerHTML = movies.map(movie => `
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

        this.elements.searchResults.style.display = 'block';
    }

    debounce(func, wait) {
        let timeout;
        return function() {
            const context = this, args = arguments;
            clearTimeout(timeout);
            timeout = setTimeout(() => func.apply(context, args), wait);
        };
    }
}

// Initialize the logs manager when DOM is loaded
document.addEventListener('DOMContentLoaded', () => {
    new LogsManager();
});