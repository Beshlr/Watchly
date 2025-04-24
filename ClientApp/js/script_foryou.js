document.addEventListener('DOMContentLoaded', function() {
            const baseApiUrl = 'https://localhost:7009/api/MovieRecommenderAPI';
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
                    window.location.href = 'login.html';
                };
            } else {
                window.location.href = 'login.html';
                return;
            }
            
            // Load personalized recommendations with actual user ID
            const userId = JSON.parse(userJson).id;
            loadMovies(`GetRecommandedMovies/1`, 'personalRecommendations');
            
            // Load similar movies (Sci-Fi as example)
            loadMovies('GetTop100MovieWithGenre?GenreName=Sci_FI', 'similarMovies');
            
            // Load trending movies
            loadMovies('GetTop100MovieBetweenTwoYears/2023/2025/Animation', 'trendingMovies');
            
            function loadMovies(endpoint, containerId) {
                const container = document.getElementById(containerId);
                
                fetch(`${baseApiUrl}/${endpoint}`)
                    .then(response => {
                        if (!response.ok) throw new Error('Network response was not ok');
                        return response.json();
                    })
                    .then(movies => {
                        displayMovies(movies.slice(0, 5), containerId);
                    })
                    .catch(error => {
                        console.error('Error:', error);
                        container.innerHTML = '<div class="col-12 text-center py-5"><p class="text-danger">Error loading movies</p></div>';
                    });
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
                            <i class="bi bi-heart${isFavorite(movie.id) ? '-fill text-danger' : ''}"></i>
                        </button>
                    </div>
                    <div class="card-body p-2">
                        <h6 class="card-title mb-1">${movie.movieName}</h6>
                        <small class="text-muted">${movie.year} • ⭐ ${movie.rate?.toFixed(1) || 'N/A'}</small>
                    </div>
                </div>
                `).join('');
            }

            // تحقق مما إذا كان الفيلم مفضلاً
function isFavorite(movieId) {
    const userJson = localStorage.getItem('loggedInUser') || sessionStorage.getItem('loggedInUser');
    if (!userJson) return false;
    
    // يمكنك تخزين قائمة المفضلة في localStorage للتحسين
    const favorites = JSON.parse(localStorage.getItem('userFavorites') || '[]');
    return favorites.includes(movieId);
}

// تبديل حالة المفضلة
window.toggleFavorite= async function(movieId, buttonElement) {
    const userJson = localStorage.getItem('loggedInUser') || sessionStorage.getItem('loggedInUser');
    if (!userJson) {
        window.location.href = 'login.html';
        return;
    }
    
    const user = JSON.parse(userJson);
    const isFav = isFavorite(movieId);
    const icon = buttonElement.querySelector('i');
    
    try {
        const response = await fetch('https://localhost:7009/api/UsersAPI/AddMovieToFavorate', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify({
                UserID: user.id,
                MovieID: movieId
            })
        });
        
        if (!response.ok) {
            throw new Error('Failed to update favorite');
        }
        
        // تحديث الواجهة
        if (isFav) {
            icon.classList.remove('bi-heart-fill', 'text-danger');
            icon.classList.add('bi-heart');
            // إزالة من localStorage
            const favorites = JSON.parse(localStorage.getItem('userFavorites') || []);
            localStorage.setItem('userFavorites', JSON.stringify(favorites.filter(id => id !== movieId)));
        } else {
            icon.classList.remove('bi-heart');
            icon.classList.add('bi-heart-fill', 'text-danger');
            // إضافة إلى localStorage
            const favorites = JSON.parse(localStorage.getItem('userFavorites') || []);
            favorites.push(movieId);
            localStorage.setItem('userFavorites', JSON.stringify(favorites));
        }
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
        const response = await fetch(`https://localhost:7009/api/UsersAPI/GetAllFavorateMoviesforUser?UserID=${user.id}`);
        if (!response.ok) throw new Error('Failed to load favorites');
        
        const favorites = await response.json();
        const favoriteIds = favorites.map(movie => movie.movieID);
        localStorage.setItem('userFavorites', JSON.stringify(favoriteIds));
    } catch (error) {
        console.error('Error loading favorites:', error);
    }
}

// استدعاء loadFavorites عند تحميل الصفحة
document.addEventListener('DOMContentLoaded', loadFavorites);

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