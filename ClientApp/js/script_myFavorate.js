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

    function loadFavoriteMovies() {
        const container = document.getElementById('favoriteMoviesContainer');
        const userId = JSON.parse(userJson).id;
        
        fetch(`${baseApiUrl}/GetAllFavorateMoviesforUser?UserID=${userId}`)
            .then(response => {
                if (!response.ok) throw new Error('Network response was not ok');
                return response.json();
            })
            .then(movies => {
                displayFavoriteMovies(movies);
            })
            .catch(error => {
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
            });
    }

    function displayFavoriteMovies(movies) {
        const container = document.getElementById('favoriteMoviesContainer');
        
        if (!movies || movies.length === 0) {
            container.innerHTML = `
                <div class="empty-state">
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
                <div class="card-body p-2">
                    <h6 class="card-title mb-1">${movie.movieName}</h6>
                    <small class="text-muted">${movie.year} • ⭐ ${movie.rate?.toFixed(1) || 'N/A'}</small>
                </div>
            </div>
        `).join('');
    }

    // Toggle favorite function (same as in other pages)
    window.toggleFavorite = async function(movieId, buttonElement) {
        const userJson = localStorage.getItem('loggedInUser') || sessionStorage.getItem('loggedInUser');
        if (!userJson) {
            window.location.href = 'login.html';
            return;
        }
        
        const user = JSON.parse(userJson);
        const icon = buttonElement.querySelector('i');
        
        try {
            const response = await fetch('http://watchly.runasp.net/api/UsersAPI/AddMovieToFavorate', {
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
            console.error('Error updating favorite:', error);
            alert('Failed to update favorite. Please try again.');
        }
    }
});