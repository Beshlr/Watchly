document.addEventListener('DOMContentLoaded', function() {
    const baseApiUrl = 'http://watchly.runasp.net/api/UsersAPI';
    const userJson = localStorage.getItem('loggedInUser') || sessionStorage.getItem('loggedInUser');
    
    // Check authentication and update UI
    if (userJson) {
        const user = JSON.parse(userJson);
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
    } else {
        alert('You are not logged in. Redirecting to login page...');
        window.location.href = 'login.html';
        return;
    }
    
    // Load favorite movies
    loadFavoriteMovies();

    const searchBtn = document.getElementById('searchButton');
    const searchInput = document.getElementById('searchInput');
    const searchResults = document.getElementById('searchResults');
    searchBtn.addEventListener('click', getMoviesBySearch);
    const baseUrl = 'http://watchly.runasp.net/api/MovieRecommenderAPI'; 

    async function searchMovies(query, displayInGrid = true) {
        if (query.length < 2) {
            searchResults.style.display = 'none'; 
            if (!displayInGrid) {
                return [];
            }
            throw new Error('Please enter at least 2 characters to search.');
        }
        
        const response = await fetch(`${baseUrl}/NameHasWord/${query}`);
        
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
    
    // دالة اختيار نتيجة البحث
    window.selectMovie = function(url) {
        if (url) {
            window.open(url, '_blank');
        }
        // searchResults.style.display = 'none';
        // searchInput.value = '';
    };
    
    // إخفاء نتائج البحث عند النقر خارجها
    document.addEventListener('click', function(e) {
        if (!searchInput.contains(e.target) && !searchResults.contains(e.target)) {
            searchResults.style.display = 'none';
        }
    });

    async function loadFavoriteMovies() {
        const container = document.getElementById('favoriteMoviesContainer');
        const userId = JSON.parse(userJson).id;
        
        try {
            const response = await fetch(`${baseApiUrl}/GetAllFavorateMoviesforUser?UserID=${userId}`);
            if (!response.ok) throw new Error('Network response was not ok');
            
            const movies = await response.json();
            // Add isFavorite flag to each movie
            const moviesWithFavorites = movies.map(movie => ({ ...movie, isFavorite: true }));
            displayFavoriteMovies(moviesWithFavorites);
            
            // Update localStorage
            const favoriteIds = movies.map(movie => movie.movieID);
            localStorage.setItem('userFavorites', JSON.stringify(favoriteIds));
        } catch (error) {
            console.error('Error:', error);
            container.innerHTML = `
                <div class="empty-state">
                    <div class="empty-state-icon">
                        <i class="bi bi-heartbreak"></i>
                    </div>
                    <h3>Error loading favorite movies</h3>
                    <p>Please try again later.</p>
                </div>
            `;
        }
    }

    function displayFavoriteMovies(movies) {
        const container = document.getElementById('favoriteMoviesContainer');
        
        if (!movies || movies.length === 0) {
            container.innerHTML = `
                <div class="empty-state" style="width: 100%; text-align: center; padding: 20px;">
                    <div class="empty-state-icon">
                        <i class="bi bi-heart"></i>
                    </div>
                    <h3>No favorite movies yet</h3>
                    <p>Start adding movies to your favorites to see them here.</p>
                    <a href="explore_Movies.html" class="btn btn-primary mt-3">Browse Movies</a>
                </div>
            `;
            return;
        }
        
        container.innerHTML = movies.map(movie => `
            <div class="movie-card">
                <div class="position-relative">
                    <img src="${movie.posterImageURL || 'https://via.placeholder.com/300x450'}" 
                        class="card-img-top" 
                        alt="${movie.movieName}"
                        onerror="this.src='https://via.placeholder.com/300x450'">
                    <button class="btn btn-sm btn-favorite position-absolute top-0 end-0 m-2" 
                            onclick="event.stopPropagation(); toggleFavorite(${movie.id}, this)">
                        <i class="bi bi-heart-fill text-danger"></i>
                    </button>
                </div>
                <a href="${movie.imDbMovieURL || '#'}"
                   class="card-link" 
                   target="_blank" style="text-decoration: none; color: inherit;">
                    <div class="card-body p-2">
                        <h6 class="card-title mb-1">${movie.movieName}</h6>
                        <small class="text-muted">${movie.year} • ⭐ ${movie.rate?.toFixed(1) || 'N/A'}</small>
                    </div>
                </a>
            </div>
        `).join('');
    }

    // Toggle favorite function
    window.toggleFavorite = async function(movieId, buttonElement) {
        const userJson = localStorage.getItem('loggedInUser') || sessionStorage.getItem('loggedInUser');
        if (!userJson) {
            window.location.href = 'login.html';
            return;
        }
        
        const user = JSON.parse(userJson);
        const icon = buttonElement.querySelector('i');
        
        try {
            const response = await fetch(`http://watchly.runasp.net/api/UsersAPI/RemoveMovieFromFavorateList?MovieID=${movieId}&UserID=${user.id}`, {
                method: 'DELETE',
                headers: {
                    'Content-Type': 'application/json'
                }
            });



            
            if (!response.ok) {
                if(response.status === 500) {
                    alert('Movie not found in favorites or already removed.');
                    return;
                }
                throw new Error('Failed to remove favorite');
            }
            
            // Update UI and localStorage
            let favorites = JSON.parse(localStorage.getItem('userFavorites') || []);
            favorites = favorites.filter(id => id !== movieId);
            localStorage.setItem('userFavorites', JSON.stringify(favorites));
            
            // Remove the movie card from the UI
            buttonElement.closest('.movie-card').remove();
            
            // If no movies left, show empty state
            if (document.querySelectorAll('.movie-card').length === 0) {
                displayFavoriteMovies([]);
            }
        } catch (error) {
            console.error('Error removing favorite:', error);
            alert('Failed to remove favorite. Please try again.');
        }
    }
});