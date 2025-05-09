document.addEventListener('DOMContentLoaded', function() {
    const baseApiUrl = 'https://watchly.runasp.net/api/UsersAPI';
    const userJson = localStorage.getItem('loggedInUser') || sessionStorage.getItem('loggedInUser');
    const user = JSON.parse(userJson);

    function showStatusMessage(message, type) {
        let statusElement = document.getElementById('statusMessage');
        
        if (!statusElement) {
            statusElement = document.createElement('div');
            statusElement.id = 'statusMessage';
            statusElement.style.position = 'fixed';
            statusElement.style.top = '20px';
            statusElement.style.right = '20px';
            statusElement.style.zIndex = '1000';
            statusElement.style.padding = '10px 20px';
            statusElement.style.borderRadius = '5px';
            statusElement.style.boxShadow = '0 2px 10px rgba(0,0,0,0.2)';
            statusElement.style.transition = 'all 0.3s ease';
            document.body.appendChild(statusElement);
        }
        
        statusElement.textContent = message;
        statusElement.className = `alert alert-${type} show`;
        statusElement.style.display = 'block';
    
        setTimeout(() => {
            statusElement.style.opacity = '0';
            setTimeout(() => {
                statusElement.style.display = 'none';
                statusElement.style.opacity = '1';
            }, 300);
        }, 3000);
    }

    // Check authentication and update UI
    if (userJson) {
        document.getElementById('welcomeText').textContent = `Welcome, ${user.username}!`;
        
        const loginBtn = document.getElementById('log-btn');
        loginBtn.textContent = 'Logout';
        loginBtn.href = '#';
        loginBtn.onclick = () => {
            localStorage.removeItem('loggedInUser');
            sessionStorage.removeItem('loggedInUser');
            localStorage.removeItem('userFavorites');
            window.location.href = 'login.html';
        };
        
        // Load all movie lists
        loadMovieLists(user.id);
    } else {
        alert('You are not logged in. Redirecting to login page...');
        window.location.href = 'login.html';
        return;
    }

    // Search functionality (same as before)
    const searchBtn = document.getElementById('searchButton');
    const searchInput = document.getElementById('searchInput');
    const searchResults = document.getElementById('searchResults');
    searchBtn.addEventListener('click', getMoviesBySearch);
    const baseUrl = 'https://watchly.runasp.net/api/MovieRecommenderAPI'; 

    async function searchMovies(query, displayInGrid = true) {
        if (query.length < 2) {
            searchResults.style.display = 'none'; 
            if (!displayInGrid) {
                return [];
            }
            throw new Error('Please enter at least 2 characters to search.');
        }
        
        const response = await fetch(`${baseUrl}/NameHasWord/${query}/${user.id}`);
        
        if (!response.ok) {
            throw new Error(`API request failed with status ${response.status}`);
        }
        
        const movies = await response.json();
        
        if (displayInGrid) {
            displayMovies(movies);
        }
        
        return movies;
    }

    async function getMoviesBySearch(e) {
        if (e) e.preventDefault();
        
        try {
            showLoadingState();
            searchResults.style.display = 'none';
            await searchMovies(searchInput.value.trim(), true);
        } catch (error) {
            console.error('Search error:', error);
            showErrorState(error);
        }
    }

    searchInput.addEventListener('input', async function(e) {
        const query = e.target.value.trim();
        
        if (query.length === 0) {
            searchResults.style.display = 'none';
            return;
        }
        
        try {
            const movies = await searchMovies(query, false);
            showSearchResults(movies);
        } catch (error) {
            console.error('Search error:', error);
            searchResults.innerHTML = '<div class="p-2 text-danger">Error loading results</div>';
            searchResults.style.display = 'block';
        }
    });

    function showSearchResults(movies) {
        if (!movies || movies.length === 0) {
            searchResults.innerHTML = '<div class="p-2 text-muted">No results found</div>';
            searchResults.style.display = 'block';
            return;
        }
    
        searchResults.innerHTML = movies.slice(0, 5).map(movie => `
            <div class="search-result-item p-2 border-bottom" 
                 onclick="selectMovie('${movie.imDbMovieURL}')">
                <div class="d-flex">
                    <img src="${movie.posterImageURL || 'https://via.placeholder.com/50x75'}" 
                         class="me-2" 
                         width="50" 
                         height="75"
                         onerror="this.src='https://via.placeholder.com/50x75'">
                    <div>
                        <h6 class="mb-1">${movie.movieName}</h6>
                        <small class="text-muted">${movie.year} • ⭐ ${movie.rate?.toFixed(1) || 'N/A'}</small>
                    </div>
                </div>
            </div>
        `).join('');
    
        searchResults.style.display = 'block';
    }
    
    window.selectMovie = function(url) {
        if (url) {
            window.open(url, '_blank');
        }
    };
    
    document.addEventListener('click', function(e) {
        if (!searchInput.contains(e.target) && !searchResults.contains(e.target)) {
            searchResults.style.display = 'none';
        }
    });

    // Main function to load all movie lists
    async function loadMovieLists(userId) {
        try {
            await Promise.all([
                loadMoviesList(`${baseApiUrl}/GetAllFavorateMoviesforUser?UserID=${userId}`, 'favoritesMovies'),
                loadMoviesList(`${baseApiUrl}/GetAllWatchedMoviesForUser/${userId}`, 'watchedMovies'),
                loadMoviesList(`${baseApiUrl}/GetAllSearchedMoviesForUser/${userId}`, 'searchedMovies'),
                loadMoviesList(`${baseApiUrl}/GetAllUnlikedMoviesToUser/${userId}`, 'unlikedMovies')
            ]);
        } catch (error) {   
            console.error('Error loading movie lists:', error);
            showStatusMessage('Error loading movie lists. Please try again.', 'danger');
        }
    }

    // Generic function to load a movie list
    async function loadMoviesList(endpoint, containerId) {
        const container = document.getElementById(containerId);
        
        try {
            const response = await fetch(endpoint);
            if (!response.ok) throw new Error(`Failed to load ${containerId} movies`);
            
            const movies = await response.json();
            
            if (!movies || movies.length === 0) {
                container.innerHTML = `
                    <div class="empty-state">
                        <div class="empty-state-icon">
                            <i class="bi bi-collection"></i>
                        </div>
                        <h3>No movies found</h3>
                        <p>Your ${containerId.replace('Movies', '').toLowerCase()} list is empty.</p>
                    </div>
                `;
                return;
            }
            
            displayMovieList(movies, containerId);
        } catch (error) {
            console.error(`Error loading ${containerId}:`, error);
            container.innerHTML = `
                <div class="empty-state">
                    <div class="empty-state-icon">
                        <i class="bi bi-exclamation-triangle"></i>
                    </div>
                    <h3>Error loading movies</h3>
                    <p>Please try again later.</p>
                </div>
            `;
        }
    }

    // Display movies in a horizontal scrollable list
    function displayMovieList(movies, containerId) {
        const container = document.getElementById(containerId);
        const listType = containerId.replace('Movies', '').toLowerCase();
        
        container.innerHTML = movies.map(movie => `
            <div class="movie-card">
                <a href="${movie.imDbMovieURL || '#'}" target="_blank" class="text-decoration-none d-block">
                    <div class="position-relative">
                        <img src="${movie.posterImageURL || 'https://via.placeholder.com/300x450'}" 
                            class="card-img-top" 
                            alt="${movie.movieName}"
                            onerror="this.src='https://via.placeholder.com/300x450'">
                        <button class="btn-delete" 
                                onclick="event.preventDefault(); removeFromList(${movie.id}, '${listType}', this)">
                            <i class="bi bi-trash"></i>
                        </button>
                    </div>
                    <div class="card-body p-2">
                        <h6 class="card-title mb-1">${movie.movieName}</h6>
                        <small class="text-muted">${movie.year} • ⭐ ${movie.rate?.toFixed(1) || 'N/A'}</small>
                    </div>
                </a>
            </div>
        `).join('');
    }

    // Function to remove a movie from a list
    window.removeFromList = async function(movieId, listType, buttonElement) {
        const userJson = localStorage.getItem('loggedInUser') || sessionStorage.getItem('loggedInUser');
        if (!userJson) {
            window.location.href = 'login.html';
            return;
        }
        
        const user = JSON.parse(userJson);
        let endpoint = '';
        let method = 'DELETE';
        
        // Determine the correct endpoint based on list type
        switch(listType) {
            case 'favorites':
                endpoint = `${baseApiUrl}/RemoveMovieFromFavorateList?MovieID=${movieId}&UserID=${user.id}`;
                break;
            case 'watched':
                endpoint = `${baseApiUrl}/RemoveMovieFromWatchedList?MovieID=${movieId}&UserID=${user.id}`;
                break;
            case 'searched':
                // Note: You might need to implement this endpoint in your API
                endpoint = `${baseApiUrl}/RemoveMovieFromSearchList?MovieID=${movieId}&UserID=${user.id}`;
                break;
            case 'unliked':
                endpoint = `${baseApiUrl}/RemoveMovieFromUnlikedList?MovieID=${movieId}&UserID=${user.id}`;
                break;
            default:
                showStatusMessage('Invalid list type', 'danger');
                return;
        }
        
        try {
            const response = await fetch(endpoint, {
                method: method,
                headers: {
                    'Content-Type': 'application/json'
                }
            });

            if (!response.ok) {
                const error = await response.text();
                throw new Error(error || 'Failed to remove movie');
            }
            
            // Remove the movie card from UI
            buttonElement.closest('.movie-card').remove();
            
            // Show success message
            showStatusMessage('Movie removed successfully', 'success');
            
            // If no movies left, show empty state
            const container = document.getElementById(`${listType}Movies`);
            if (container.querySelectorAll('.movie-card').length === 0) {
                container.innerHTML = `
                    <div class="empty-state">
                        <div class="empty-state-icon">
                            <i class="bi bi-collection"></i>
                        </div>
                        <h3>No movies found</h3>
                        <p>Your ${listType} list is now empty.</p>
                    </div>
                `;
            }
        } catch (error) {
            console.error(`Error removing movie from ${listType}:`, error);
            showStatusMessage(`Failed to remove movie: ${error.message}`, 'danger');
        }
    }

    function showLoadingState() {
        const moviesGrid = document.getElementById('moviesGrid');
        if (moviesGrid) {
            moviesGrid.innerHTML = `
                <div class="col-12 text-center py-5">
                    <div class="spinner-border text-primary" role="status">
                        <span class="visually-hidden">Loading...</span>
                    </div>
                    <p class="mt-2">Loading movies...</p>
                </div>
            `;
        }
    }

    function showErrorState(error) {
    const moviesGrid = document.getElementById('moviesGrid');
    if (moviesGrid) {
        moviesGrid.innerHTML = `
            <div class="col-12 text-center py-5">
                <p class="text-danger">Error: ${error.message || 'Failed to load movies'}</p>
            </div>
        `;
    }
    }
});