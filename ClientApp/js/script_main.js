document.addEventListener('DOMContentLoaded', function() {
    let baseApiUrl = 'https://watchly.runasp.net/api/UsersAPI';
    const userJson = localStorage.getItem('loggedInUser');
    
    const user = JSON.parse(userJson);
    const loginLogoutBtn = document.querySelector('.log-btn');
    
    // Update UI based on login state
    if (user) {
        loginLogoutBtn.textContent = 'Logout';
        loginLogoutBtn.href = '#';
        loginLogoutBtn.onclick = () => {
            localStorage.removeItem('loggedInUser');
            window.location.href = 'login.html';
        };
        
        loadMovies(`GetTop15TrendingMovies/${user.id}`, 'popularMovies');
    }
    else
    loadMovies(`GetTop15TrendingMovies/${-1}`, 'popularMovies');

    // Show Manage Users link only for admins

    // Load popular movies
    
    
    // Load recommended movies (using user ID 1 for demo)
    loadMovies(`GetAllRecommendedMoviesForUser/${user.id}`, 'recommendedMovies');
    if (user) {
        document.getElementById('welcomeText').innerText = `Welcome, ${user.username}!`;
    }
    // Search functionality
    const searchInput = document.getElementById('searchInput');
    searchInput.addEventListener('input', debounce(handleSearch, 300));
    
    function loadMovies(endpoint, containerId) {
        const container = document.getElementById(containerId);
        container.innerHTML = '<div class="col-12 text-center py-5"><div class="spinner-border text-primary" role="status"><span class="visually-hidden">Loading...</span></div></div>';
        if(containerId === 'popularMovies')
        {
            
        }
        fetch(`${baseApiUrl}/${endpoint}`)
            .then(response => {
                if (!response.ok) throw new Error('Network response was not ok');
                return response.json();
            })
            .then(movies => {
                displayMovies(movies.slice(0, 4), containerId);
            })
            .catch(error => {
                console.error('Error:', error);
                container.innerHTML = `<div class="col-12 text-center py-5"><p class="text-danger">Error loading movies. Please try again.</p></div>`;
            });
    }
    
    function displayMovies(movies, containerId) {
        const container = document.getElementById(containerId);
        
        if (!movies || movies.length === 0) {
            container.innerHTML = '<div class="col-12 text-center py-5"><p class="text-muted">No movies found</p></div>';
            return;
        }
        
        const userJson = localStorage.getItem('loggedInUser');
        const user = userJson ? JSON.parse(userJson) : null;
        const favorites = JSON.parse(localStorage.getItem('userFavorites') || '[]');
    
        container.innerHTML = movies.map(movie => `
            <div class="col-md-6 col-lg-3 mb-4 movie-card-container">
                <div class="card h-100 position-relative">
                    <!-- Link covering the entire card (except favorite button) -->
                    <a href="${movie.imDbMovieURL || '#'}" 
                       class="stretched-link card-link"
                       target="_blank"
                       style="z-index: 1;"></a>
                    
                    <!-- Movie Poster -->
                    <img src="${movie.posterImageURL || 'https://via.placeholder.com/300x450'}" 
                        class="card-img-top" 
                        alt="${movie.movieName}"
                        onerror="this.src='https://via.placeholder.com/300x450?text=Poster+Not+Found'">
                    
                    <!-- Favorite Button (positioned above the link) -->
                    <button class="btn btn-sm btn-favorite position-absolute top-0 end-0 m-2" 
                            onclick="event.preventDefault(); event.stopPropagation(); toggleFavorite(${movie.id}, this)"
                            style="z-index: 2; background-color: rgba(255, 255, 255, 0.9); border-radius: 50%; width: 36px; height: 36px; display: flex; align-items: center; justify-content: center; box-shadow: 0 2px 5px rgba(0,0,0,0.2);">
                        <i class="bi bi-heart${favorites.includes(movie.id) ? '-fill text-danger' : ''}" style="font-size: 1.2rem;"></i>
                    </button>
                    
                    <!-- Movie Info -->
                    <div class="card-body" style="position: relative; z-index: 1;">
                        <h5 class="card-title">${movie.movieName}</h5>
                        <p class="card-text">
                            <span class="text-muted">${movie.year}</span>
                            <span class="float-end">
                                ⭐ ${movie.rate?.toFixed(1) || 'N/A'}
                            </span>
                        </p>
                    </div>
                </div>
            </div>
        `).join('');
    }
    
    function isFavorite(movieId) {
        const userJson = localStorage.getItem('loggedInUser') || sessionStorage.getItem('loggedInUser');
        if (!userJson) return false;
        
        // يمكنك تخزين قائمة المفضلة في localStorage للتحسين
        const favorites = JSON.parse(localStorage.getItem('userFavorites') || '[]');
        return favorites.includes(movieId);
    }
    
    // تبديل حالة المفضلة
    window.toggleFavorite = async function(movieId, buttonElement) {
        const userJson = localStorage.getItem('loggedInUser') || sessionStorage.getItem('loggedInUser');
       
        
        const user = JSON.parse(userJson);
        const icon = buttonElement.querySelector('i');
        const isFav = icon.classList.contains('bi-heart-fill');
        
        try {
            const endpoint = isFav ? 'RemoveMovieFromFavorateList' : 'AddMovieToFavorate';
            const method = isFav ? 'DELETE' : 'POST';

            var response;

            if(method === 'POST') {
                const body = JSON.stringify({ MovieID: movieId, UserID: user.id });
                 response = await fetch(`https://watchly.runasp.net/api/UsersAPI/${endpoint}`, {
                    method: method,
                    headers: {
                        'Content-Type': 'application/json',
                    },
                    body: body
                });
            }
            else if(method === 'DELETE') {
                 response = await fetch(`https://watchly.runasp.net/api/UsersAPI/${endpoint}?MovieID=${movieId}&UserID=${user.id}`, {
                    method: method,
                    headers: {
                        'Content-Type': 'application/json',
                    },
                    
                });
            }
            
            if (!response.ok) {
                alert(response.statusText);
                return;
                
            }
            
            if (isFav) {
                icon.classList.remove('bi-heart-fill', 'text-danger');
                icon.classList.add('bi-heart');
            } else {
                icon.classList.remove('bi-heart');
                icon.classList.add('bi-heart-fill', 'text-danger');
            }
            
            // تحديث localStorage
            let favorites = JSON.parse(localStorage.getItem('userFavorites') || []);
            if (isFav) {
                favorites = favorites.filter(id => id !== movieId);
            } else {
                favorites.push(movieId);
            }
            localStorage.setItem('userFavorites', JSON.stringify(favorites));
            
        } catch (error) {
            console.error('Error updating favorite:', error);
            alert('Failed to update favorite. Please try again.');
        }
    }
    
    // تحميل قائمة المفضلة عند بدء التشغيل
    async function loadFavorites() {
        const userJson = localStorage.getItem('loggedInUser') || sessionStorage.getItem('loggedInUser');
        if (!userJson) return;
        
        const user = JSON.parse(userJson);
        
        try {
            const response = await fetch(`https://watchly.runasp.net/api/UsersAPI/GetAllFavorateMoviesforUser?UserID=${user.id}`);
            if (!response.ok) throw new Error('Failed to load favorites');
            
            const favorites = await response.json();
            const favoriteIds = favorites.map(movie => movie.id);
            localStorage.setItem('userFavorites', JSON.stringify(favoriteIds));
        } catch (error) {
            console.error('Error loading favorites:', error);
        }
    }
    loadFavorites();
    // استدعاء loadFavorites عند تحميل الصفحة
    

    function handleSearch() {
        const query = searchInput.value.trim();
        const resultsContainer = document.getElementById('searchResults');
        
        if (query.length < 2) {
            resultsContainer.style.display = 'none';
            return;
        }
        
        fetch(`${baseApiUrl}/NameHasWord/${query}/${user.id}`)
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