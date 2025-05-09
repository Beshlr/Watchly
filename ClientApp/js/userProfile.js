/**
 * User Profile Manager
 * Centralized management of user profile functionality
 * Handles: login state, permissions, avatar display, dropdown menu
 */

class UserProfileManager {
    constructor() {
        this.baseApiUrl = 'https://watchly.runasp.net/api/UsersAPI';
        this.user = this.getLoggedInUser();
        this.init();
    }

    // Initialize the user profile
    init() {
        if (!this.checkAuth()) return;
        
        this.setupDOMReferences();
        this.updateUI();
        this.setupEventListeners();
        this.loadFavorites();
    }

    // Get logged in user from storage
    getLoggedInUser() {
        const userJson = localStorage.getItem('loggedInUser') || sessionStorage.getItem('loggedInUser');
        return userJson ? JSON.parse(userJson) : null;
    }

    // Check authentication state
    checkAuth() {
        if (!this.user) {
            this.redirectToLogin();
            return false;
        }
        return true;
    }

    // Redirect to login page
    redirectToLogin() {
        window.location.href = 'login.html';
    }

    // Setup DOM references
    setupDOMReferences() {
        this.elements = {
            userProfile: document.getElementById('userProfile'),
            loginBtn: document.getElementById('log-btn'),
            userAvatar: document.getElementById('userAvatar'),
            logoutBtn: document.getElementById('logoutBtn'),
            manageUsersBtn: document.getElementById('manageUsersBtn'),
            manageLogsBtn: document.getElementById('manageLogsBtn'),
            adminDivider: document.getElementById('adminDivider'),
            welcomeText: document.getElementById('welcomeText'),
            userDropdown: document.getElementById('userDropdown')
        };
    }

    // Update UI based on user state
    updateUI() {
        const { userProfile, loginBtn, userAvatar, logoutBtn, manageUsersBtn, manageLogsBtn, adminDivider, welcomeText } = this.elements;

        // Show/hide profile and login button
        userProfile.style.display = this.user ? 'flex' : 'none';
        loginBtn.style.display = this.user ? 'none' : 'block';

        if (this.user) {
            // Set welcome text if exists
            if (welcomeText) {
                welcomeText.textContent = `Welcome, ${this.user.username}!`;
            }

            // Set avatar based on permissions
            this.setAvatarIcon();

            // Show/hide admin options
            this.handleAdminOptions();

            // Setup logout functionality
            logoutBtn.onclick = () => this.logout();
        }
    }

    // Set avatar icon based on user permissions
    setAvatarIcon() {
        const { userAvatar } = this.elements;
        userAvatar.className = 'user-avatar';
        const icon = userAvatar.querySelector('i') || document.createElement('i');
        
        if (this.user.permissions === 1) {
            userAvatar.classList.add('admin');
            icon.className = 'bi bi-emoji-sunglasses-fill';
        } else if (this.user.permissions === 3) {
            userAvatar.classList.add('super-admin');
            icon.className = 'bi bi-stars';
        } else {
            userAvatar.classList.add('normal-user');
            icon.className = 'bi bi-person-fill';
        }

        if (!userAvatar.querySelector('i')) {
            userAvatar.appendChild(icon);
        }
    }

    // Handle admin-specific UI elements
    handleAdminOptions() {
        const { manageUsersBtn, manageLogsBtn, adminDivider } = this.elements;
        const isAdmin = this.user.permissions === 1 || this.user.permissions === 3;
        
        if (manageUsersBtn) {
            manageUsersBtn.style.display = isAdmin ? 'block' : 'none';
        }
        
        if (adminDivider) {
            adminDivider.style.display = isAdmin ? 'block' : 'none';
        }
        
        if (manageLogsBtn) {
            manageLogsBtn.style.display = this.user.permissions === 3 ? 'block' : 'none';
        }
    }

    // Setup event listeners
    setupEventListeners() {
        const { userProfile, userDropdown } = this.elements;
        
        // Toggle dropdown menu
        if (userProfile && userDropdown) {
            userProfile.addEventListener('click', (e) => {
                e.stopPropagation();
                userDropdown.classList.toggle('show');
            });
        }

        // Close dropdown when clicking outside
        document.addEventListener('click', () => {
            if (this.elements.userDropdown) {
                this.elements.userDropdown.classList.remove('show');
            }
        });
    }

    // Logout functionality
    logout() {
        localStorage.removeItem('loggedInUser');
        localStorage.removeItem('userFavorites');
        window.location.href = 'login.html';
    }

    // Load user favorites
    async loadFavorites() {
        if (!this.user) return;
        
        try {
            const response = await fetch(`${this.baseApiUrl}/GetAllFavorateMoviesforUser?UserID=${this.user.id}`);
            if (!response.ok) throw new Error('Failed to load favorites');
            
            const favorites = await response.json();
            const favoriteIds = favorites.map(movie => movie.id);
            localStorage.setItem('userFavorites', JSON.stringify(favoriteIds));
        } catch (error) {
            console.error('Error loading favorites:', error);
        }
    }

    // Check if user has permission
    hasPermission(requiredPermission) {
        if (!this.user) return false;
        return this.user.permissions >= requiredPermission;
    }

    // Get current user
    getCurrentUser() {
        return this.user;
    }
}

// Initialize when DOM is loaded
document.addEventListener('DOMContentLoaded', () => {
    window.userProfileManager = new UserProfileManager();
});

/**
 * Helper functions for other scripts to use
 */

// Check if movie is favorite
window.isFavorite = function(movieId) {
    const favorites = JSON.parse(localStorage.getItem('userFavorites') || []);
    return favorites.includes(movieId);
};

// Toggle favorite status
window.toggleFavorite = async function(movieId, buttonElement) {
    const user = window.userProfileManager?.getCurrentUser();
    if (!user) {
        window.location.href = 'login.html';
        return;
    }
    
    const icon = buttonElement.querySelector('i');
    const isFav = icon.classList.contains('bi-heart-fill');
    
    try {
        const endpoint = isFav ? 'RemoveMovieFromFavorateList' : 'AddMovieToFavorate';
        const method = isFav ? 'DELETE' : 'POST';
        const url = isFav 
            ? `https://watchly.runasp.net/api/UsersAPI/${endpoint}?MovieID=${movieId}&UserID=${user.id}`
            : `https://watchly.runasp.net/api/UsersAPI/${endpoint}`;

        const options = {
            method,
            headers: { 'Content-Type': 'application/json' }
        };

        if (method === 'POST') {
            options.body = JSON.stringify({ MovieID: movieId, UserID: user.id });
        }

        const response = await fetch(url, options);
        
        if (!response.ok) {
            throw new Error(await response.text());
        }
        
        // Update icon
        icon.className = isFav ? 'bi bi-heart' : 'bi bi-heart-fill text-danger';
        
        // Update favorites in localStorage
        window.userProfileManager.loadFavorites();
        
    } catch (error) {
        console.error('Error updating favorite:', error);
        alert('Failed to update favorite. Please try again.');
    }
};