document.addEventListener('DOMContentLoaded', function() {
    // العناصر الأساسية
    const moviesGrid = document.getElementById('moviesGrid');
    const applyFiltersBtn = document.getElementById('applyFilters');
    const resetFiltersBtn = document.getElementById('resetFilters');
    const StartYear = document.getElementById('startYear');
    const EndYear = document.getElementById('endYear');
    const StartYearMaxValue = document.getElementById('startYearMaxValue');
    const EndYearMaxValue = document.getElementById('endYearMaxValue');
    const ratingRange = document.getElementById('ratingRange');
    const ratingValue = document.getElementById('ratingValue');
    const genreCheckboxes = document.querySelectorAll('input[type="checkbox"][id^="genre-"], #action, #comedy, #drama, #sci-fi, #horror, #romance, #thriller, #animation');
    const searchInput = document.getElementById('searchInput');
    const searchResults = document.getElementById('searchResults');
    const searchBtn = document.getElementById('searchButton');
    const sortBy = document.getElementById('sortBy');
    // إعدادات API
    const apiConfig = {
        baseUrl: 'http://watchly.runasp.net/api/MovieRecommenderAPI',
        endpoints: {
            byGenre: '/GetTop100MovieWithGenre',
            byGenreAndYear: '/GetTop100MovieWithGenreInYear',
            byYearsRange: '/GetTop100MovieBetweenTwoYears',
            byYearsRangeAndGenreSorted: '/GetTop100MovieWithFiltersAndSorting'
        }
    };
    const userJson = localStorage.getItem('loggedInUser') || sessionStorage.getItem('loggedInUser');
    const loggedInUser = JSON.parse(userJson);

    // Check authentication and update UI
    if (userJson) {
        const user = JSON.parse(userJson);
        
        const loginBtn = document.getElementById('log-btn');
        loginBtn.textContent = 'Logout';
        loginBtn.href = '#';
        loginBtn.onclick = () => {
            localStorage.removeItem('loggedInUser');
            sessionStorage.removeItem('loggedInUser');
            window.location.href = 'login.html';
        };
    if (user && user.permissions === 1 || user.permissions === 3) {
        document.getElementById('manageUsersNavItem').style.display = 'block';
    }
    } else {
        window.location.href = 'login.html';
        return;
    }
    // تهيئة الصفحة
    initPage();

    // تعريف الدوال الأساسية
    // في دالة initPage، تأكد من تعيين القيم الافتراضية بشكل صحيح
function initPage() {
    // تعيين القيم الافتراضية للمنزلقات
    StartYear.min = 1980;
    StartYear.max = 2025;
    StartYear.value = 2021;
    StartYearMaxValue.textContent = '2021';
    
    EndYear.min = 1980;
    EndYear.max = 2025;
    EndYear.value = 2025;
    EndYearMaxValue.textContent = '2025';

    ratingRange.value = 6;
    ratingValue.textContent = '6.0';
    
    // إضافة مستمعي الأحداث
    setupEventListeners();
    
    // تحميل الأفلام عند البدء
    loadInitialMovies();
}

// في دالة resetFilters، تأكد من إعادة التعيين بشكل صحيح
resetFiltersBtn.addEventListener('click', function() {
    // إعادة تعيين صناديق الاختيار
    genreCheckboxes.forEach(checkbox => {
        checkbox.checked = ['action', 'comedy', 'drama', 'sci-fi'].includes(checkbox.id);
    });
    
    // إعادة تعيين المنزلقات
    StartYear.value = 2021;
    StartYearMaxValue.textContent = 2021;

    EndYear.value = 2025;
    EndYearMaxValue.textContent = 2025;
    
    ratingRange.value = 6;
    ratingValue.textContent = '6.0';
    
    // تطبيق الفلاتر من جديد
    applyFilters();
});

    function setupEventListeners() {
        // تحديث عرض قيمة السنة عند التغيير
        StartYear.addEventListener('input', function() {
            StartYearMaxValue.textContent = this.value;
        });
        EndYear.addEventListener('input', function() {
            EndYearMaxValue.textContent = this.value;
        });
        
        // تحديث عرض قيمة التقييم عند التغيير
        ratingRange.addEventListener('input', function() {
            ratingValue.textContent = this.value;
        });
        
        // When the user clicks on applyFilters button, apply the filters
        applyFiltersBtn.addEventListener('click', applyFilters);
        
        // When the user clicks on search button, search for movies
        searchBtn.addEventListener('click', getMoviesBySearch);
    }

 
// دالة جلب نتائج البحث من API
async function searchMovies(query, displayInGrid = true) {
    if (query.length < 2) {
        loadInitialMovies();
        searchResults.style.display = 'none'; 
        if (!displayInGrid) {
            return [];
        }
        throw new Error('Please enter at least 2 characters to search.');
    }
    
    const response = await fetch(`${apiConfig.baseUrl}/NameHasWord/${query}`);
    if (!response.ok) {
        const error = await response.text();
        throw new Error(`${error}`);
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

async function GetUserInfoByID(userId) {
    try {
        const response = await fetch(`http://watchly.runasp.net/api/UsersAPI/GetUserInfoByID/${userId}`);
        if (!response.ok) {
            throw new Error('Failed to fetch user info');
        }

        const updatedUser = await response.json();

        // حدد هل البيانات كانت في localStorage أو sessionStorage
        const isInLocal = localStorage.getItem('loggedInUser') !== null;
        const storage = isInLocal ? localStorage : sessionStorage;

        // حدث البيانات داخل storage
        storage.setItem('loggedInUser', JSON.stringify(updatedUser));

        return updatedUser;
    } catch (error) {
        console.error('Error fetching user info:', error);
        return null;
    }
}



// دالة عرض نتائج البحث
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

    

    // Function to show circular loading state
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
                <div class="alert alert-danger">
                    Error: ${error.message || 'Failed to load movies'}
                </div>
                <button class="btn btn-primary mt-3" onclick="window.location.reload()">Try Again</button>
            </div>
        `;
    }

    // أضف هذه الدالة مع الدوال الأخرى في ملف script_movies.js
    function generateStarRating(rating) {
        if (!rating || rating === 0) {
            return '<span class="text-muted">No rating</span>';
        }
        
        const fullStars = Math.floor(rating / 2);
        const halfStar = rating % 2 >= 0.5 ? 1 : 0;
        const emptyStars = 5 - fullStars - halfStar;
        
        let stars = '';
        for (let i = 0; i < fullStars; i++) stars += '<i class="bi bi-star-fill text-warning"></i>';
        if (halfStar) stars += '<i class="bi bi-star-half text-warning"></i>';
        if(emptyStars)
            for (let i = 0; i < emptyStars; i++) stars += '<i class="bi bi-star text-warning"></i>';
        
        return stars;
    }

    // في دالة displayMovies، سأعدل كود عرض الأفلام ليتضمن:
// ... (الكود السابق يبقى كما هو حتى دالة displayMovies)

function displayMovies(movies) {
    if (!movies || movies.length === 0) {
        moviesGrid.innerHTML = `
            <div class="col-12 text-center py-5">
                <p class="text-muted">No movies found matching your criteria</p>
            </div>
        `;
        return;
    }
    GetUserInfoByID(loggedInUser.id);
    // جلب قائمة المفضلة من localStorage
    const favorites = JSON.parse(localStorage.getItem('userFavorites') || '[]');
    const userJson = localStorage.getItem('loggedInUser') || sessionStorage.getItem('loggedInUser');
    const user = userJson ? JSON.parse(userJson) : null;
    const isAdmin = user && user.permissions === 1;

    moviesGrid.innerHTML = movies.map(movie => `
    <div class="col-md-4 col-lg-3 mb-4 movie-card-container">
        <a href="${movie.imDbMovieURL || '#'}" 
            class="card h-100 d-block text-decoration-none" 
            onclick="window.open('${movie.imDbMovieURL || '#'}', '_blank'); return false;" 
            style="cursor: pointer;">
            <div class="position-relative">
                <img src="${movie.posterImageURL || 'https://via.placeholder.com/300x450'}" 
                    class="card-img-top" 
                    alt="${movie.movieName}"
                    onerror="this.src='https://via.placeholder.com/300x450?text=Poster+Not+Found'">
                <button class="btn btn-sm btn-favorite position-absolute top-0 end-0 m-2" 
                    onclick="event.preventDefault(); event.stopPropagation(); toggleFavorite(${movie.id}, this)"
                    style="background-color: rgba(255, 255, 255, 0.9); border-radius: 50%; width: 36px; height: 36px; display: flex; align-items: center; justify-content: center; box-shadow: 0 2px 5px rgba(0,0,0,0.2);">
                <i class="bi bi-heart${favorites.includes(movie.id) ? '-fill text-danger' : ''}" style="font-size: 1.2rem;"></i>
                </button>

                ${isAdmin ? `
                <button class="btn btn-sm btn-danger position-absolute top-0 start-0 m-2 delete-movie-btn" 
                    onclick="event.preventDefault(); event.stopPropagation(); confirmDeleteMovie(${movie.id}, '${movie.movieName.replace(/'/g, "\\'")}')"
                    style="border-radius: 50%; width: 36px; height: 36px; display: flex; align-items: center; justify-content: center; box-shadow: 0 2px 5px rgba(0,0,0,0.2);">
                    <i class="bi bi-trash" style="font-size: 1rem;"></i>
                </button>
                ` : ''}
            </div>
            <div class="card-body">
                <h5 class="card-title">${movie.movieName}</h5>
                <p class="card-text">
                    <span class="text-muted">${movie.year}</span>
                    <span class="float-end rating-stars">
                        ${generateStarRating(movie.rate)}
                        <small>${movie.rate?.toFixed(1) || 'N/A'}</small>
                    </span>
                </p>
            </div>
        </a>
    </div>
    `).join('');
}

// إضافة دالة تأكيد الحذف
window.confirmDeleteMovie = function(movieId, movieName) {
    if (confirm(`Are you sure you want to delete the movie "${movieName}"?`)) {
        deleteMovie(movieId);
    }
};

// إضافة دالة حذف الفيلم
async function deleteMovie(movieId) {
    try {
        const response = await fetch(`http://watchly.runasp.net/api/MovieRecommenderAPI/DeleteMovie/${movieId}`, {
            method: 'DELETE',
            headers: {
                'Content-Type': 'application/json',
            }
        });

        if (!response.ok) {
            throw new Error(`Failed to delete movie: ${response.status}`);
        }

        alert('Movie deleted successfully!');
        applyFilters(); // Refresh the movie list
    } catch (error) {
        console.error('Error deleting movie:', error);
        alert('Failed to delete movie. Please try again.');
    }
}

// ... (بقية الكود يبقى كما هو)

// تعديل دالة toggleFavorite لاستخدام API بشكل صحيح
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
             response = await fetch(`http://watchly.runasp.net/api/UsersAPI/${endpoint}`, {
                method: method,
                headers: {
                    'Content-Type': 'application/json',
                },
                body: body
            });
        }
        else if(method === 'DELETE') {
             response = await fetch(`http://watchly.runasp.net/api/UsersAPI/${endpoint}?MovieID=${movieId}&UserID=${user.id}`, {
                method: method,
                headers: {
                    'Content-Type': 'application/json',
                },
                
            });
        }
        
        if (!response.ok) {
            if(method === 'DELETE' && response.status === 400) {
                alert('Movie not found in favorites or already removed.');
            }
            else if(method === 'POST' && response.status === 400) {
                alert('Movie already in favorites.');
            }
            
        }
        
        if (isFav) {
            icon.classList.remove('bi-heart-fill', 'text-danger');
            icon.classList.add('bi-heart');
        } else {
            icon.classList.remove('bi-heart');
            icon.classList.add('bi-heart-fill', 'text-danger');
        }
        
        // تحديث localStorage
        let favorites = JSON.parse(localStorage.getItem('userFavorites') || '[]');
        if (isFav) {
            favorites = favorites.filter(id => id !== movieId);
        } else {
            if (!favorites.includes(movieId)) {
                favorites.push(movieId);
            }
        }
        localStorage.setItem('userFavorites', JSON.stringify(favorites));
        console.log('✅ Updated userFavorites:', favorites);

    } catch (error) {
        console.error('Error updating favorite:', error);
        alert('Failed to update favorite. Please try again.');
    }
}

// تعديل دالة loadInitialMovies لتحميل حالة المفضلة
async function loadInitialMovies() {
    try {
        showLoadingState();
        
        // تحميل الأفلام
        const response = await fetch(`${apiConfig.baseUrl}${apiConfig.endpoints.byGenre}?GenreName=Sci_Fi`);
        if (!response.ok) {
            throw new Error(`HTTP error! status: ${response.status}`);
        }
        
        const movies = await response.json();
        
        // تحميل قائمة المفضلة إذا كان المستخدم مسجل الدخول
        if (userJson) {
            const user = JSON.parse(userJson);
            const favResponse = await fetch(`http://watchly.runasp.net/api/UsersAPI/GetAllFavorateMoviesforUser?UserID=${user.id}`);
            if (favResponse.ok) {
                const favorites = await favResponse.json();
                const favoriteIds = favorites.map(movie => movie.id);
                localStorage.setItem('userFavorites', JSON.stringify(favoriteIds));
            }
        }
        
        displayMovies(movies);
    } catch (error) {
        console.error('Failed to load initial movies:', error);
        showErrorState(error);
    }
}

// تعديل دالة applyFilters لتحميل حالة المفضلة
async function applyFilters() {
    try {
        showLoadingState();
        
            const selectedGenres = Array.from(genreCheckboxes)
                .filter(checkbox => checkbox.checked)
                .map(checkbox => {
                    switch(checkbox.id) {
                        case 'action': return 'Action';
                        case 'comedy': return 'Comedy';
                        case 'drama': return 'Drama';
                        case 'sci-fi': return 'Sci_FI';
                        case 'horror': return 'Horror';
                        case 'romance': return 'Romance';
                        case 'thriller': return 'Thriller';
                        case 'adventure': return 'Adventure';
                        case 'animation': return 'Animation';

                        default: return checkbox.id;
                    }
                });

                // Put All Selected Genres in the selectedGenresString variable
            const selectedGenresString = selectedGenres.join(',');
            
            const startYear = StartYear.value;
            const endYear = EndYear.value;
            const minRating = parseFloat(ratingRange.value);
            var SortByValue = sortBy.value;
            var OrderValue = SortByValue === 'rating' ? 'DESC' : SortByValue === 'newest' ? 'DESC' : 'ASC';
            let apiUrl = `${apiConfig.baseUrl}${apiConfig.endpoints.byGenre}?GenreName=Sci_Fi`;

            // 2. تسجيل URL للتحقق
            if(selectedGenres.length != 0) {
                apiUrl = `${apiConfig.baseUrl}${apiConfig.endpoints.byYearsRangeAndGenreSorted}/${startYear}/${endYear}/${selectedGenresString}/${minRating}/${SortByValue}/${OrderValue}`;
            }
            
            console.log("Request URL:", apiUrl); // أضف هذا السطر
            
            // 3. جلب البيانات
            const response = await fetch(apiUrl);
            
            if (!response.ok) {
                if( response.status === 404) 
                    throw new Error(`No movies found for the selected filters.`);
                else
                    throw new Error(`HTTP error! status: ${response.status}`);
            }        
            const movies = await response.json();
        
        // تحميل قائمة المفضلة إذا كان المستخدم مسجل الدخول
        if (userJson) {
            const user = JSON.parse(userJson);
            const favResponse = await fetch(`http://watchly.runasp.net/api/UsersAPI/GetAllFavorateMoviesforUser?UserID=${user.id}`);
            if (favResponse.ok) {
                const favorites = await favResponse.json();
                const favoriteIds = favorites.map(movie => movie.id);
                localStorage.setItem('userFavorites', JSON.stringify(favoriteIds));
            }
        }
        
        displayMovies(movies);
        
    } catch (error) {
        console.error('Failed to apply filters:', error);
        showErrorState(error);
    }
}
    
async function checkIfMovieIsFavorite(userId, movieId) {
    try {
        const response = await fetch(`http://watchly.runasp.net/api/UsersAPI/CheckIfMovieIsFavorate?UserID=${userId}&MovieID=${movieId}`);

        if (!response.ok) {
            // الفيلم مش موجود في المفضلة
            return false;
        }

        // ممكن تستخدم النص لو حبيت تعرض رسالة
        const message = await response.text(); // مثلاً "Movie is in the favorate list"
        console.log('Server Message:', message);

        return true;
    } catch (error) {
        console.error('Error checking favorite:', error);
        return false;
    }
}


window.isFavorite = async function(movieId) {
    const userJson = localStorage.getItem('loggedInUser') || sessionStorage.getItem('loggedInUser');
    if (!userJson) return false;
    
    const user = JSON.parse(userJson);
    try {
        const response = await fetch(`http://watchly.runasp.net/api/UsersAPI/CheckIfMovieIsFavorate?UserID=${user.id}&MovieID=${movieId}`);
        return response.ok;
    } catch (error) {
        console.error('Error checking favorite:', error);
        return false;
    }
}
    
    // تبديل حالة المفضلة
    async function toggleFavorite(movieId, buttonElement) {
        const userJson = localStorage.getItem('loggedInUser') || sessionStorage.getItem('loggedInUser');
        if (!userJson) {
            window.location.href = 'login.html';
            return;
        }
        
        const user = JSON.parse(userJson);
        const isFav = checkIfMovieIsFavorite(user.Id,movieId);
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
            const response = await fetch(`http://watchly.runasp.net/api/UsersAPI/GetAllFavorateMoviesforUser?UserID=${user.id}`);
            if (!response.ok) throw new Error('Failed to load favorites');
            
            const favorites = await response.json();
            const favoriteIds = favorites
            .filter(movie => movie && movie.id !== undefined)
            .map(movie => movie.id);

            localStorage.setItem('userFavorites', JSON.stringify(favoriteIds));
        } catch (error) {
            console.error('Error loading favorites:', error);
        }
    }
    
    // استدعاء loadFavorites عند تحميل الصفحة
    document.addEventListener('DOMContentLoaded', loadFavorites);

    // جعل الدالة متاحة عالمياً لأزرار إعادة المحاولة
    window.applyFilters = applyFilters;
});