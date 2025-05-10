document.addEventListener('DOMContentLoaded', () => {
    const baseMovieApiUrl = 'https://watchly.runasp.net/api/MovieRecommenderAPI';
    const baseUsersApiUrl = 'https://watchly.runasp.net/api/UsersAPI';
    const userJson = localStorage.getItem('loggedInUser') || sessionStorage.getItem('loggedInUser');
    let favoriteMovies = null; 

    const user = JSON.parse(userJson);
    

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
        
        
        

    } else {
        alert('You are not logged in. Redirecting to login page...');
        window.location.href = 'login.html';
        return;
    }

    // Load personalized recommendations with actual user ID
    const userId = JSON.parse(userJson).id;
    loadRecommendedMovies(`${baseUsersApiUrl}/GetAllRecommendedMoviesForUser/${user.id}`, 'personalRecommendations');
    
    // Load favorite genres movies
    loadFavoriteGenresMovies(userId);
    
    
    // Load trending movies
    loadMovies(`GetTop15TrendingMovies/${user.id}`, 'trendingMovies');
    
    // Initialize lazy loading
    initLazyLoading();


// دالة لجلب الأنواع المفضلة وعرض الأفلام
async function loadFavoriteGenresMovies(userId) {
    try {
        // جلب أفضل 3 أنواع للمستخدم
        const genresResponse = await fetch(`${baseUsersApiUrl}/GetTop5GenresUserInterstIn/${userId}`);
        if (!genresResponse.ok) throw new Error('Failed to load favorite genres');
        
        const genres = await genresResponse.json();
        if (!genres || genres.length === 0) return;
        
        // عرض تبويبات الأنواع
        const genreTabs = document.getElementById('genreTabs');
        const genreSectionsContainer = document.getElementById('genreSectionsContainer');
        
        genreTabs.innerHTML = '';
        genreSectionsContainer.innerHTML = '';
        
        genres.forEach((genreDisplay, index) => {
            let genreId = genreDisplay === 'Science Fiction' ? 'Sci_Fi' : `genre-${genreDisplay.replace(/\s+/g, '-').toLowerCase()}`;
        
            const tab = document.createElement('div');
            tab.className = `genre-tab ${index === 0 ? 'active' : ''}`;
            tab.textContent = genreDisplay;
            tab.dataset.genre = genreDisplay;
            tab.dataset.genreId = genreId;
            tab.style.cursor = 'pointer';
            tab.style.transition = '0.3s';
            tab.onclick = () => switchGenreTab(genreDisplay);
            genreTabs.appendChild(tab);
        
            const section = document.createElement('div');
            section.className = `genre-section ${index === 0 ? 'active' : ''}`;
            section.id = genreId;
        
            section.innerHTML = `
                <div class="movie-container">
                    <button class="scroll-btn left" onclick="scrollSection('${genreId}-movies', -1)">
                        <i class="bi bi-chevron-left"></i>
                    </button>
                    <div class="movie-grid" id="${genreId}-movies">
                        <div class="text-center py-5" style="min-width: 100%;">
                            <div class="spinner-border text-primary" role="status">
                                <span class="visually-hidden">Loading...</span>
                            </div>
                        </div>
                    </div>
                    <button class="scroll-btn right" onclick="scrollSection('${genreId}-movies', 1)">
                        <i class="bi bi-chevron-right"></i>
                    </button>
                </div>
            `;
        
            genreSectionsContainer.appendChild(section);
        
            // تحميل الأفلام
            loadMoviesForGenre(genreDisplay === 'Science Fiction' ? 'Sci_Fi' : genreDisplay, `${genreId}-movies`);
        });        
    } catch (error) {
        console.error('Error loading favorite genres:', error);
        document.getElementById('genreSectionsContainer').innerHTML = `
            <div class="col-12 text-center py-5">
                <p class="text-muted">Could not load your favorite genres. Try again later.</p>
            </div>
        `;
    }
}

function switchGenreTab(genre) {
    const genreId = genre === 'Science Fiction' ? 'Sci_Fi' : `genre-${genre.replace(/\s+/g, '-').toLowerCase()}`;
    
    document.querySelectorAll('.genre-tab').forEach(tab => {
        tab.classList.toggle('active', tab.dataset.genre === genre);
    });

    document.querySelectorAll('.genre-section').forEach(section => {
        section.classList.toggle('active', section.id === genreId);
    });
}


async function loadMoviesForGenre(genre, containerId) {
    try {
        const response = await fetch(`${baseMovieApiUrl}/GetTop100MovieWithGenre/${user.id}?GenreName=${encodeURIComponent(genre)}`);
        if (!response.ok) throw new Error(`Failed to load ${genre} movies`);
        
        const movies = await response.json();
        const userId = JSON.parse(localStorage.getItem('loggedInUser') || sessionStorage.getItem('loggedInUser')).id;
        
        // التحقق من الأفلام المفضلة لكل فيلم
        const moviesWithFavorites = await Promise.all(movies.slice(0, 15).map(async movie => {
            const isFav = await checkIfMovieIsFavorite(userId, movie.id);
            return { ...movie, isFavorite: isFav };
        }));
        
        displayMovies(moviesWithFavorites, containerId);
    } catch (error) {
        console.error(`Error loading ${genre} movies:`, error);
        document.getElementById(containerId).innerHTML = `
            <div class="col-12 text-center py-5" style="min-width: 100%;">
                <p class="text-danger">Error loading ${genre} movies. Please try again later.</p>
            </div>
        `;
    }
}

async function loadMovies(endpoint, containerId) {
    const container = document.getElementById(containerId);
    
    try {
        let response = null;
        if(containerId !== 'trendingMovies') {
            response = await fetch(`${baseMovieApiUrl}/${endpoint}`);
        }
        else
        {
            response = await fetch(`${baseUsersApiUrl}/GetTop15TrendingMovies/${user.id}`);

        }
        if (!response.ok) throw new Error('Network response was not ok');
        const movies = await response.json();
        
        // Check favorites for each movie
        const userId = JSON.parse(localStorage.getItem('loggedInUser') || sessionStorage.getItem('loggedInUser')).id;
        const moviesWithFavorites = await Promise.all(movies.slice(0,15).map(async movie => {
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
    
    try {
        // جلب الأفلام الموصى بها من نقطة النهاية الجديدة
        const response = await fetch(`${baseUsersApiUrl}/GetAllRecommendedMoviesForUser/${user.id}`);
        
        if (!response.ok) {
            throw new Error(`Failed to load recommendations (Status: ${response.status})`);
        }

        const movies = await response.json();

        if (!movies || movies.length === 0) {
            // إذا لم تكن هناك أفلام موصى بها، نعرض رسالة بديلة
            container.innerHTML = `
                <div class="col-12 text-center py-5">
                    <p class="text-muted">No recommendations found. Try adding some favorite movies first.</p>
                    <a href="favorites.html" class="btn btn-primary mt-2">Go to Favorites</a>
                </div>
            `;
            return;
        }

        // التحقق من الأفلام المفضلة لكل فيلم
        const moviesWithFavorites = await Promise.all(movies.map(async movie => {
            const isFav = await checkIfMovieIsFavorite(user.id, movie.id);
            return { ...movie, isFavorite: isFav };
        }));

        displayMovies(moviesWithFavorites, containerId);
        
    } catch (error) {
        console.error("Error loading recommended movies:", error);
        container.innerHTML = `
            <div class="col-12 text-center py-5">
                <p class="text-danger">Error loading recommendations. Please try again later.</p>
                <button onclick="loadRecommendedMovies('${endpoint}', '${containerId}')" 
                        class="btn btn-sm btn-outline-primary mt-2">
                    Retry
                </button>
            </div>
        `;
    }
}

// دالة محسنة للتمرير السلس مع تأثيرات
function scrollSection(sectionId, direction) {
    const section = document.getElementById(sectionId);
    if (!section) return;
    
    const scrollAmount = section.clientWidth * 0.8; // تمرير 80% من عرض الشاشة
    const currentScroll = section.scrollLeft;
    const targetScroll = currentScroll + (direction * scrollAmount);
    
    // تطبيق التمرير السلس مع easing
    const duration = 500; // مدة التمرير بالمللي ثانية
    const startTime = performance.now();
    
    function animateScroll(currentTime) {
        const elapsed = currentTime - startTime;
        const progress = Math.min(elapsed / duration, 1);
        const easeProgress = easeOutQuad(progress);
        
        section.scrollLeft = currentScroll + (easeProgress * (targetScroll - currentScroll));
        
        if (progress < 1) {
            requestAnimationFrame(animateScroll);
        }
    }
    
    function easeOutQuad(t) {
        return t * (2 - t);
    }
    
    requestAnimationFrame(animateScroll);
}

function displayMovies(movies, containerId) {
    const container = document.getElementById(containerId);
    
    if (!movies || movies.length === 0) {
        container.innerHTML = '<div class="col-12 text-center py-5" style="min-width: 100%;"><p class="text-muted">No movies found</p></div>';
        return;
    }
    
    container.innerHTML = movies.map(movie => `
    <div class="movie-card">
        <a href="${movie.imDbMovieURL || '#'}" target="_blank" class="text-decoration-none d-block">
            <div class="position-relative">
                <img src="${movie.posterImageURL || 'https://via.placeholder.com/300x450'}" 
                    class="card-img-top lazy-load" 
                    alt="${movie.movieName}"
                    onerror="this.onerror=null; this.src='https://via.placeholder.com/300x450'"
                    style="height: 270px; width: 100%; object-fit: cover;">
                <button class="btn btn-sm btn-favorite position-absolute top-0 end-0 m-2" 
                        onclick="event.preventDefault(); toggleFavorite(${movie.id}, this)">
                    <i class="bi bi-heart${movie.isFavorite ? '-fill text-danger' : ''}"></i>
                </button>
            </div>
            <div class="card-body p-2">
                <h6 class="card-title mb-1 text-dark">${movie.movieName}</h6>
                <small class="text-muted">${movie.year} • ⭐ ${movie.rate?.toFixed(1) || 'N/A'}</small>
            </div>
        </a>
    </div>
    `).join('');
    
    // Initialize lazy loading for newly added images
    initLazyLoading();
}

async function checkIfMovieIsFavorite(userId, movieId) {
    try {
        const response = await fetch(`${baseUsersApiUrl}/CheckIfMovieIsFavorate?UserID=${userId}&MovieID=${movieId}`);
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
        location.reload();

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

function initLazyLoading() {
    const lazyLoadObserver = new IntersectionObserver((entries) => {
        entries.forEach(entry => {
            if (entry.isIntersecting) {
                const img = entry.target;
                img.src = img.dataset.src || img.src;
                lazyLoadObserver.unobserve(img);
            }
        });
    }, {
        rootMargin: '200px' 
    });

    document.querySelectorAll('.lazy-load').forEach(img => {
        if (!img.src) {
            img.dataset.src = img.getAttribute('src');
            img.removeAttribute('src');
        }
        lazyLoadObserver.observe(img);
    });
}

// Search functionality
const searchBtn = document.getElementById('searchButton');
const searchInput = document.getElementById('searchInput');
const searchResults = document.getElementById('searchResults');

searchBtn.addEventListener('click', getMoviesBySearch);

async function searchMovies(query, displayInGrid = true) {
    if (query.length < 2) {
        searchResults.style.display = 'none'; 
        return [];
    }
    
    try {
        const response = await fetch(`${baseMovieApiUrl}/NameHasWord/${query}/${user.id}`);
        
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

    searchResults.innerHTML = movies.map(movie => `
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
}});