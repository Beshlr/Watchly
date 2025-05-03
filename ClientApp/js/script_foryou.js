document.addEventListener('DOMContentLoaded', function() {
    const baseMovieApiUrl = 'http://beshir1-001-site1.ptempurl.com/api/MovieRecommenderAPI';
    const baseUsersApiUrl = 'http://beshir1-001-site1.ptempurl.com/api/UsersAPI';
    const userJson = localStorage.getItem('loggedInUser') || sessionStorage.getItem('loggedInUser');
    let favoriteMovies = null; // تصحيح التسمية من Favorate إلى Favorite

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
    
    // Load personalized recommendations with actual user ID
    const userId = JSON.parse(userJson).id;
    loadRecommendedMovies(`http://beshir1-001-site1.ptempurl.com/api/RecommendationAPI/GetMovieRecommendation`, 'personalRecommendations');
    
    // Load similar movies (Sci-Fi as example) - تصحيح اسم الجنس
    loadMovies('GetTop100MovieWithGenre?GenreName=Sci_Fi', 'similarMovies');
    
    // Load trending movies - تعديل المسار ليتوافق مع الـ endpoint
    loadMovies('GetTop100MovieBetweenTwoYears/2023/2025/Animation', 'trendingMovies');
    
    async function loadMovies(endpoint, containerId) {
        const container = document.getElementById(containerId);
        
        try {
            const response = await fetch(`${baseMovieApiUrl}/${endpoint}`);
            if (!response.ok) throw new Error('Network response was not ok');
            const movies = await response.json();
            
            // Check favorites for each movie
            const moviesWithFavorites = await Promise.all(movies.slice(0, 5).map(async movie => {
                const isFav = await checkIfMovieIsFavorite(userId, movie.id);
                return { ...movie, isFavorite: isFav };
            }));
            
            displayMovies(moviesWithFavorites, containerId);
        } catch (error) {
            console.error('Error loading movies:', error);
            container.innerHTML = '<div class="col-12 text-center py-5"><p class="text-danger">Error loading movies. Please try again later.</p></div>';
        }
    }

    async function getNameOfMoviesForUser(userID) {
        try {
            const response = await fetch(`${baseUsersApiUrl}/GetAllFavorateMoviesNameForUser/${userID}`);
            
            if (!response.ok) {
                return []; // إرجاع قائمة فارغة بدلاً من null
            }
            
            const movies = await response.json();
            return movies || []; // التأكد من عدم إرجاع null
        }
        catch (error) {
            console.error('Error getting favorite movies:', error);
            return [];
        }
    }
    
    async function loadRecommendedMovies(endpoint, containerId) {
        const container = document.getElementById(containerId);
        const moviesNames = await getNameOfMoviesForUser(userId) || [];
    
        if (moviesNames.length === 0) {
            container.innerHTML = '<div class="col-12 text-center py-5"><p class="text-muted">No favorite movies found to generate recommendations.</p></div>';
            return;
        }
    
        try {
            console.log("Sending request to endpoint:", endpoint);
            console.log("Request payload:", { Movies_Name: moviesNames });
    
            const response = await fetch(endpoint, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify({ Movies_Name: moviesNames })
            });
    
            console.log("Response status:", response.status);
    
            if (!response.ok) {
                const errorText = await response.text();
                console.error("Error response:", errorText);
                throw new Error(errorText || "Failed to load recommendations");
            }
    
            const movies = await response.json();
            console.log("Received movies:", movies);
    
            if (!movies || movies.length === 0) {
                container.innerHTML = '<div class="col-12 text-center py-5"><p class="text-muted">No recommendations found.</p></div>';
                return;
            }
    
            const moviesWithFavorites = await Promise.all(movies.slice(0, 5).map(async movie => {
                const isFav = await checkIfMovieIsFavorite(userId, movie.id);
                return { ...movie, isFavorite: isFav };
            }));
    
            displayMovies(moviesWithFavorites, containerId);
        } catch (error) {
            console.error("Error in loadRecommendedMovies:", error);
            container.innerHTML = '<div class="col-12 text-center py-5"><p class="text-danger">Error loading recommendations. Please try again later.</p></div>';
        }
    }
    
    // Search functionality
    const searchBtn = document.getElementById('searchButton');
    const searchInput = document.getElementById('searchInput');
    const searchResults = document.getElementById('searchResults');
    const moviesGrid = document.getElementById('moviesGrid');
    searchBtn.addEventListener('click', getMoviesBySearch);
    
    async function searchMovies(query, displayInGrid = true) {
        if (query.length < 2) {
            searchResults.style.display = 'none'; 
            return [];
        }
        
        try {
            const response = await fetch(`${baseMovieApiUrl}/NameHasWord/${query}`);
            
            if (!response.ok) {
                throw new Error(`API request failed with status ${response.status}`);
            }
            
            const movies = await response.json();
            
            if (displayInGrid) {
                displayMovies(movies, 'moviesGrid');
            }
            
            return movies;
        } catch (error) {
            console.error('Search error:', error);
            return [];
        }
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
    
    async function checkIfMovieIsFavorite(userId, movieId) {
        try {
            const response = await fetch(`${baseUsersApiUrl}/CheckIfMovieIsFavorate?UserID=${userId}&MovieID=${movieId}`);
            return response.ok;
        } catch (error) {
            console.error('Error checking favorite:', error);
            return false;
        }
    }
    
    function displayMovies(movies, containerId) {
        const container = document.getElementById(containerId);
        
        if (!movies || movies.length === 0) {
            container.innerHTML = '<div class="col-12 text-center py-5"><p class="text-muted">No movies found</p></div>';
            return;
        }
        
        container.innerHTML = movies.map(movie => `
        <div class="movie-card">
            <div class="position-relative">
                <img src="${movie.posterImageURL || 'https://via.placeholder.com/300x450'}" 
                    class="card-img-top lazy-load" 
                    alt="${movie.movieName}"
                    onerror="this.src='https://via.placeholder.com/300x450'">
                <button class="btn btn-sm btn-favorite position-absolute top-0 end-0 m-2" 
                        onclick="event.stopPropagation(); toggleFavorite(${movie.id}, this)">
                    <i class="bi bi-heart${movie.isFavorite ? '-fill text-danger' : ''}"></i>
                </button>
            </div>
            <div class="card-body p-2">
                <h6 class="card-title mb-1">${movie.movieName}</h6>
                <small class="text-muted">${movie.year} • ⭐ ${movie.rate?.toFixed(1) || 'N/A'}</small>
            </div>
        </div>
        `).join('');
    }

    window.isFavorite = async function(movieId) {
        const userJson = localStorage.getItem('loggedInUser') || sessionStorage.getItem('loggedInUser');
        if (!userJson) return false;
        
        const user = JSON.parse(userJson);
        try {
            const response = await fetch(`${baseUsersApiUrl}/CheckIfMovieIsFavorate?UserID=${user.id}&MovieID=${movieId}`);
            return response.ok;
        } catch (error) {
            console.error('Error checking favorite:', error);
            return false;
        }
    }

    window.toggleFavorite = async function(movieId, buttonElement) {
        const userJson = localStorage.getItem('loggedInUser') || sessionStorage.getItem('loggedInUser');
        if (!userJson) {
            window.location.href = 'login.html';
            return;
        }
        
        const user = JSON.parse(userJson);
        const icon = buttonElement.querySelector('i');
        const isFav = icon.classList.contains('bi-heart-fill');
        
        try {
            const endpoint = isFav ? 'RemoveMovieFromFavorateList' : 'AddMovieToFavorate';
            const method = isFav ? 'DELETE' : 'POST';
    
            let response;
    
            if (method === 'POST') {
                const body = JSON.stringify({ MovieID: movieId, UserID: user.id });
                response = await fetch(`${baseUsersApiUrl}/${endpoint}`, {
                    method: method,
                    headers: {
                        'Content-Type': 'application/json',
                    },
                    body: body
                });
            }
            else if (method === 'DELETE') {
                response = await fetch(`${baseUsersApiUrl}/${endpoint}?MovieID=${movieId}&UserID=${user.id}`, {
                    method: method,
                    headers: {
                        'Content-Type': 'application/json',
                    },
                });
            }
            
            if (!response.ok) {
                const error = await response.text();
                throw new Error(error);
            }
            
            // Update icon
            if (isFav) {
                icon.classList.remove('bi-heart-fill', 'text-danger');
                icon.classList.add('bi-heart');
            } else {
                icon.classList.remove('bi-heart');
                icon.classList.add('bi-heart-fill', 'text-danger');
            }
            
            // Update favorites list
            await loadFavorites();
            
        } catch (error) {
            console.error('Error updating favorite:', error);
            alert('Failed to update favorite. Please try again.');
        }
    }

    async function loadFavorites() {
        const userJson = localStorage.getItem('loggedInUser') || sessionStorage.getItem('loggedInUser');
        if (!userJson) return;
        
        const user = JSON.parse(userJson);
        
        try {
            const response = await fetch(`${baseUsersApiUrl}/GetAllFavorateMoviesforUser?UserID=${user.id}`);
            if (!response.ok) throw new Error('Failed to load favorites');
            
            favoriteMovies = await response.json();
            const favoriteIds = favoriteMovies?.map(movie => movie.id) || [];
            localStorage.setItem('userFavorites', JSON.stringify(favoriteIds));
        } catch (error) {
            console.error('Error loading favorites:', error);
        }
    }

    loadFavorites();

    function showLoadingState() {
        moviesGrid.innerHTML = `
            <div class="col-12 text-center py-5">
                <div class="spinner-border text-primary" role="status">
                    <span class="visually-hidden">Loading...</span>
                </div>
                <p class="mt-2">Loading movies...</p>
            </div>
        `;
    }

    function showErrorState(error) {
        moviesGrid.innerHTML = `
            <div class="col-12 text-center py-5">
                <p class="text-danger">Error: ${error.message || 'Failed to load movies'}</p>
            </div>
        `;
    }

    // Initialize lazy loading
    const lazyLoadObserver = new IntersectionObserver((entries) => {
        entries.forEach(entry => {
            if (entry.isIntersecting) {
                const img = entry.target;
                img.src = img.dataset.src;
                lazyLoadObserver.unobserve(img);
            }
        });
    });

    document.querySelectorAll('.lazy-load').forEach(img => {
        lazyLoadObserver.observe(img);
    });
});