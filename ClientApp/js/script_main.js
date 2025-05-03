document.addEventListener('DOMContentLoaded', function() {
    const baseApiUrl = 'http://beshir1-001-site1.ptempurl.com/api/MovieRecommenderAPI';
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
    }

    // Load popular movies
    loadMovies('GetTop100MovieWithGenre?GenreName=Sci_FI', 'popularMovies');
    
    // Load recommended movies (using user ID 1 for demo)
    loadMovies('GetRecommandedMovies/1', 'recommendedMovies');
    if (user) {
        document.getElementById('welcomeText').innerText = `Welcome, ${user.username}!`;
    }
    // Search functionality
    const searchInput = document.getElementById('searchInput');
    searchInput.addEventListener('input', debounce(handleSearch, 300));
    
    function loadMovies(endpoint, containerId) {
        const container = document.getElementById(containerId);
        container.innerHTML = '<div class="col-12 text-center py-5"><div class="spinner-border text-primary" role="status"><span class="visually-hidden">Loading...</span></div></div>';
        
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
        
        container.innerHTML = movies.map(movie => `
            <div class="col-md-6 col-lg-3">
                <div class="card movie-card">
                    <div class="position-relative">
                        <img src="${movie.posterImageURL || 'https://via.placeholder.com/300x450'}" 
                            class="card-img-top" 
                            alt="${movie.movieName}"
                            onerror="this.src='https://via.placeholder.com/300x450'">
                        <button class="btn btn-sm btn-favorite position-absolute top-0 end-0 m-2" 
                                onclick="event.stopPropagation(); toggleFavorite(${movie.id}, this)">
                            <i class="bi bi-heart${isFavorite(movie.id) ? '-fill text-danger' : ''}"></i>
                        </button>
                    </div>
                    <div class="card-body">
                        <h5 class="card-title">${movie.movieName}</h5>
                        <p class="card-text">${movie.year} • ⭐ ${movie.rate?.toFixed(1) || 'N/A'}</p>
                        <a href="${movie.imDbMovieURL || '#'}" 
                           class="btn btn-sm btn-outline-primary"
                           target="_blank">
                            Details
                        </a>
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

            var response;

            if(method === 'POST') {
                const body = JSON.stringify({ MovieID: movieId, UserID: user.id });
                 response = await fetch(`http://beshir1-001-site1.ptempurl.com/api/UsersAPI/${endpoint}`, {
                    method: method,
                    headers: {
                        'Content-Type': 'application/json',
                    },
                    body: body
                });
            }
            else if(method === 'DELETE') {
                 response = await fetch(`http://beshir1-001-site1.ptempurl.com/api/UsersAPI/${endpoint}?MovieID=${movieId}&UserID=${user.id}`, {
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
            const response = await fetch(`http://beshir1-001-site1.ptempurl.com/api/UsersAPI/GetAllFavorateMoviesforUser?UserID=${user.id}`);
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
        
        fetch(`${baseApiUrl}/NameHasWord/${query}`)
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