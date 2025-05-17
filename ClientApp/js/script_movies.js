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
    const genreCheckboxes = document.querySelectorAll('input[type="checkbox"][id^="genre-"], #action, #adventure, #comedy, #drama, #sci-fi, #horror, #romance, #thriller, #animation, #fantasy, #family, #mystery, #crime, #history, #music, #war, #western');    const searchInput = document.getElementById('searchInput');
    const searchResults = document.getElementById('searchResults');
    const searchBtn = document.getElementById('searchButton');
    const sortBy = document.getElementById('sortBy');
    
    let currentPage = 1;
    const moviesPerPage = 80;
    let totalMovies = 0;
    let allMovies = [];
    
    const apiConfig = {
        baseUrl: 'https://watchly.runasp.net/api/MovieRecommenderAPI',
        endpoints: {
            byGenre: '/GetTop100MovieWithGenre',
            byGenreAndYear: '/GetTop100MovieWithGenreInYear',
            byYearsRange: '/GetTop100MovieBetweenTwoYears',
            byYearsRangeAndGenreSorted: '/GetTop100MovieWithFiltersAndSorting'
        }
    };
    const userJson = localStorage.getItem('loggedInUser') || sessionStorage.getItem('loggedInUser');
    const loggedInUser = JSON.parse(userJson);

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
    
    } else {
        window.location.href = 'login.html';
        return;
    }
    
    // تهيئة الصفحة
    initPage();

    // تعريف الدوال الأساسية
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

    async function searchMovies(query, displayInGrid = true) {
        if (query.length < 2) {
            loadInitialMovies();
            searchResults.style.display = 'none'; 
            if (!displayInGrid) {
                return [];
            }
            throw new Error('Please enter at least 2 characters to search.');
        }
        
        const response = await fetch(`${apiConfig.baseUrl}/NameHasWord/${query}/${loggedInUser.id}`);
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
            const response = await fetch(`https://watchly.runasp.net/api/UsersAPI/GetUserInfoByID/${userId}`);
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

    async function displayMovies(movies) {
        if (!movies || movies.length === 0) {
            moviesGrid.innerHTML = `
                <div class="col-12 text-center py-5">
                    <p class="text-muted">No movies found matching your criteria</p>
                </div>
            `;
            updatePaginationUI();
            return;
        }
    
        allMovies = movies;
        totalMovies = movies.length;
    
        const startIndex = (currentPage - 1) * moviesPerPage;
        const endIndex = Math.min(startIndex + moviesPerPage, totalMovies);
        const moviesToDisplay = allMovies.slice(startIndex, endIndex);
    
        GetUserInfoByID(loggedInUser.id);
        const favorites = JSON.parse(localStorage.getItem('userFavorites') || '[]');
        const userJson = localStorage.getItem('loggedInUser') || sessionStorage.getItem('loggedInUser');
        const user = userJson ? JSON.parse(userJson) : null;
        const isAdminOrOwner = user && (user.permissions === 1 || user.permissions === 3);
    
        const unlikedResponse = await fetch(`https://watchly.runasp.net/api/UsersAPI/GetAllUnlikedMoviesToUser/${user.id}`);
        const unlikedMovies = unlikedResponse.ok ? await unlikedResponse.json() : [];
        const unlikedIds = unlikedMovies.map(movie => movie.id);
        localStorage.setItem('userUnliked', JSON.stringify(unlikedIds));

        const watchedResponse = await fetch(`https://watchly.runasp.net/api/UsersAPI/GetAllWatchedMoviesForUser/${user.id}`);
        const watchedMovies = watchedResponse.ok ? await watchedResponse.json() : [];
        const watchedIds = watchedMovies.map(movie => movie.id);
        localStorage.setItem('userWatched', JSON.stringify(watchedIds));
            
        // ✅ هنا الجزء الصحيح فقط
        // داخل دالة displayMovies
moviesGrid.innerHTML = moviesToDisplay.map(movie => `
    <div class="col-md-4 col-lg-3 mb-4 movie-card-container">
        <a href="#" 
            class="card h-100 d-block text-decoration-none" 
            onclick="event.preventDefault(); handleMovieClick(${movie.id}, '${movie.imDbMovieURL || '#'}')" 
            style="cursor: pointer;">
            <div class="position-relative">
                <img src="${movie.posterImageURL || 'https://via.placeholder.com/300x450'}" 
                    class="card-img-top" 
                    alt="${movie.movieName}"
                    onerror="this.src='https://via.placeholder.com/300x450?text=Poster+Not+Found'">
                
                ${isAdminOrOwner ? `
                <div class="position-absolute top-0 end-0 d-flex flex-column m-2">
                    <button class="btn btn-sm btn-favorite mb-1" 
                        onclick="event.preventDefault(); event.stopPropagation(); toggleFavorite(${movie.id}, this)"
                        style="background-color: rgba(255, 255, 255, 0.9); border-radius: 50%; width: 36px; height: 36px; display: flex; align-items: center; justify-content: center; box-shadow: 0 2px 5px rgba(0,0,0,0.2);">
                        <i class="bi bi-heart${favorites.includes(movie.id) ? '-fill text-danger' : ''}" style="font-size: 1.2rem;"></i>
                    </button>
                    
                    <button class="btn btn-sm btn-watched mb-1" 
                        onclick="event.preventDefault(); event.stopPropagation(); toggleWatched(${movie.id}, this)"
                        style="background-color: rgba(255, 255, 255, 0.9); margin-top:7px; border-radius: 50%; width: 36px; height: 36px; display: flex; align-items: center; justify-content: center; box-shadow: 0 2px 5px rgba(0,0,0,0.2);">
                        <i class="bi bi-eye${watchedIds.includes(movie.id) ? '-fill text-success' : ''}" style="font-size: 1.2rem;"></i>
                    </button>
                </div>
                ` : `
                <div class="position-absolute top-0 end-0 d-flex flex-column m-2">
                    <button class="btn btn-sm btn-favorite mb-1" 
                        onclick="event.preventDefault(); event.stopPropagation(); toggleFavorite(${movie.id}, this)"
                        style="background-color: rgba(255, 255, 255, 0.9); border-radius: 50%; width: 36px; height: 36px; display: flex; align-items: center; justify-content: center; box-shadow: 0 2px 5px rgba(0,0,0,0.2);">
                        <i class="bi bi-heart${favorites.includes(movie.id) ? '-fill text-danger' : ''}" style="font-size: 1.2rem;"></i>
                    </button>
                    
                    <button class="btn btn-sm btn-watched mb-1" 
                        onclick="event.preventDefault(); event.stopPropagation(); toggleWatched(${movie.id}, this)"
                        style="background-color: rgba(255, 255, 255, 0.9); margin-top:7px; border-radius: 50%; width: 36px; height: 36px; display: flex; align-items: center; justify-content: center; box-shadow: 0 2px 5px rgba(0,0,0,0.2);">
                        <i class="bi bi-eye${watchedIds.includes(movie.id) ? '-fill text-success' : ''}" style="font-size: 1.2rem;"></i>
                    </button>
                </div>
                `}
                
                ${isAdminOrOwner ? `
                    <div class="position-absolute top-0 start-0 d-flex flex-column m-2">
                        <button class="btn btn-sm btn-danger delete-movie-btn mb-1" 
                            onclick="event.preventDefault(); event.stopPropagation(); confirmDeleteMovie(${movie.id}, '${movie.movieName.replace(/'/g, "\\'")}')"
                            style="border-radius: 50%; width: 36px; height: 36px; display: flex; align-items: center; justify-content: center; box-shadow: 0 2px 5px rgba(0,0,0,0.2);">
                            <i class="bi bi-trash" style="font-size: 1rem;"></i>
                        </button>
                        
                        <button class="btn btn-sm btn-dark adult-movie-btn" 
                            onclick="event.preventDefault(); event.stopPropagation(); ${movie.posterImageURL.includes('placeHolderOlderMovies.jpg') ? 'unmarkAsAdult' : 'markAsAdult'}(${movie.id}, this)"
                            style="border-radius: 50%; width: 36px; height: 36px; display: flex; align-items: center; justify-content: center; box-shadow: 0 2px 5px rgba(0,0,0,0.2); margin-top: 60px;">
                            <i class="bi bi-eye-slash${movie.posterImageURL.includes('placeHolderOlderMovies.jpg') ? '-fill text-danger' : ''}" style="font-size: 1rem;"></i>
                        </button>
                    </div>
                    ` : ''}
                
                <button class="btn btn-sm btn-warning position-absolute ${isAdminOrOwner ? 'start-0 m-2' : 'top-0 start-0 m-2'} dislike-movie-btn" 
                    onclick="event.preventDefault(); event.stopPropagation(); toggleUnlike(${movie.id}, this)"
                    style="${isAdminOrOwner ? 'top: 50px;' : ''} border-radius: 50%; width: 36px; height: 36px; display: flex; align-items: center; justify-content: center; box-shadow: 0 2px 5px rgba(0,0,0,0.2);">
                    <i class="bi bi-hand-thumbs-down${unlikedIds.includes(movie.id) ? '-fill text-warning' : ''}" style="font-size: 1rem;"></i>
                </button>
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
    
        updatePaginationUI();
    }
    

    window.handleMovieClick = function(movieId, imdbUrl) {
        if (imdbUrl) {
            window.open(imdbUrl, '_blank');
        }
        addMovieToSearchingList(movieId);
    };

    async function addMovieToSearchingList(movieId) {
        const userJson = localStorage.getItem('loggedInUser') || sessionStorage.getItem('loggedInUser');
        if (!userJson) return;
    
        const user = JSON.parse(userJson);
    
        try {
            const response = await fetch('https://watchly.runasp.net/api/UsersAPI/AddMovieToSearchingList', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({
                    MovieID: movieId,
                    UserID: user.id
                })
            });
    
            if (!response.ok) {
                const error = await response.text();
                console.error('Error adding to searching list:', error);
            } else {
                console.log('✅ Movie added to searching list');
            }
        } catch (error) {
            console.error('❌ Network error:', error);
        }
    }

    window.addToUnlikeList = async function(movieId) {
        const userJson = localStorage.getItem('loggedInUser') || sessionStorage.getItem('loggedInUser');
        if (!userJson) {
            window.location.href = 'login.html';
            return;
        }
        
        const user = JSON.parse(userJson);
        
        try {
            const response = await fetch('https://watchly.runasp.net/api/UsersAPI/AddMovieToUnlikeList', {
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
                const error = await response.text();
                throw new Error(error);
            }
    
            alert('Movie added to the unlike list successfully!');
        } catch (error) {
            console.error('Error adding to unlike list:', error);
            alert('There is an error: ' + error);
        }
    }

    window.toggleWatched = async function(movieId, buttonElement) {
        const userJson = localStorage.getItem('loggedInUser') || sessionStorage.getItem('loggedInUser');
        if (!userJson) {
            window.location.href = 'login.html';
            return;
        }
        
        const user = JSON.parse(userJson);
        const icon = buttonElement.querySelector('i');
        const isWatched = icon.classList.contains('bi-eye-fill');
        
        try {
            const endpoint = isWatched ? 'RemoveMovieFromWatchedList' : 'AddMovieToWatchingList';
            const method = isWatched ? 'DELETE' : 'POST';
    
            let response;
            if (method === 'POST') {
                response = await fetch(`https://watchly.runasp.net/api/UsersAPI/${endpoint}`, {
                    method: method,
                    headers: {
                        'Content-Type': 'application/json',
                    },
                    body: JSON.stringify({
                        UserID: user.id,
                        MovieID: movieId
                    })
                });
            } else {
                response = await fetch(`https://watchly.runasp.net/api/UsersAPI/${endpoint}?MovieID=${movieId}&UserID=${user.id}`, {
                    method: method,
                    headers: {
                        'Content-Type': 'application/json',
                    }
                });
            }
    
            if (!response.ok) {
                const error = await response.text();
                throw new Error(error);
            }
    
            // تحديث الأيقونة
            if (isWatched) {
                icon.classList.remove('bi-eye-fill', 'text-success');
                icon.classList.add('bi-eye');
            } else {
                icon.classList.remove('bi-eye');
                icon.classList.add('bi-eye-fill', 'text-success');
            }
    
            // تحديث localStorage
            let watched = JSON.parse(localStorage.getItem('userWatched') || '[]');
            if (isWatched) {
                watched = watched.filter(id => id !== movieId);
            } else {
                if (!watched.includes(movieId)) {
                    watched.push(movieId);
                }
            }
            localStorage.setItem('userWatched', JSON.stringify(watched));
    
            // رسالة تأكيد
            alert(`Movie ${isWatched ? 'removed from' : 'added to'} watched list successfully!`);
        } catch (error) {
            console.error('Error updating watched list:', error);
            alert('Failed to update watched list. Please try again.');
        }
    }
    
    async function loadWatchedMovies() {
        const userJson = localStorage.getItem('loggedInUser') || sessionStorage.getItem('loggedInUser');
        if (!userJson) return;
        
        const user = JSON.parse(userJson);
        
        try {
            const response = await fetch(`https://watchly.runasp.net/api/UsersAPI/GetAllWatchedMoviesForUser/${user.id}`);
            if (!response.ok) throw new Error('Failed to load watched movies');
            
            const watchedMovies = await response.json();
            const watchedIds = watchedMovies
                .filter(movie => movie && movie.id !== undefined)
                .map(movie => movie.id);
    
            localStorage.setItem('userWatched', JSON.stringify(watchedIds));
        } catch (error) {
            console.error('Error loading watched movies:', error);
        }
    }
    
    document.addEventListener('DOMContentLoaded', loadWatchedMovies);

    window.toggleUnlike = async function(movieId, buttonElement) {
        const userJson = localStorage.getItem('loggedInUser') || sessionStorage.getItem('loggedInUser');
        if (!userJson) {
            window.location.href = 'login.html';
            return;
        }
        
        const user = JSON.parse(userJson);
        const icon = buttonElement.querySelector('i');
        const isUnliked = icon.classList.contains('bi-hand-thumbs-down-fill');
        
        try {
            const endpoint = isUnliked ? 'RemoveMovieFromUnlikedList' : 'AddMovieToUnlikeList';
            const method = isUnliked ? 'DELETE' : 'POST';
    
            let response;
            if (method === 'POST') {
                response = await fetch(`https://watchly.runasp.net/api/UsersAPI/${endpoint}`, {
                    method: method,
                    headers: {
                        'Content-Type': 'application/json',
                    },
                    body: JSON.stringify({
                        UserID: user.id,
                        MovieID: movieId
                    })
                });
            } else {
                response = await fetch(`https://watchly.runasp.net/api/UsersAPI/${endpoint}?MovieID=${movieId}&UserID=${user.id}`, {
                    method: method,
                    headers: {
                        'Content-Type': 'application/json',
                    }
            });
            }
    
            if (!response.ok) {
                const error = await response.text();
                throw new Error(error);
            }
    
            // تحديث الأيقونة
            if (isUnliked) {
                icon.classList.remove('bi-hand-thumbs-down-fill', 'text-warning');
                icon.classList.add('bi-hand-thumbs-down');
            } else {
                icon.classList.remove('bi-hand-thumbs-down');
                icon.classList.add('bi-hand-thumbs-down-fill', 'text-warning');
            }
    
            // تحديث localStorage
            let unliked = JSON.parse(localStorage.getItem('userUnliked') || '[]');
            if (isUnliked) {
                unliked = unliked.filter(id => id !== movieId);
            } else {
                if (!unliked.includes(movieId)) {
                    unliked.push(movieId);
                }
            }
            localStorage.setItem('userUnliked', JSON.stringify(unliked));
    
            // رسالة تأكيد
            alert(`Movie ${isUnliked ? 'removed from' : 'added to'} unlike list successfully!`);
        } catch (error) {
            console.error('Error updating unlike list:', error);
            alert('Failed to update unlike list. Please try again.');
        }
    }

    function updatePaginationUI() {
        const pagination = document.querySelector('.pagination');
        if (!pagination) return;
        
        const totalPages = Math.ceil(totalMovies / moviesPerPage);
        
        let paginationHTML = `
            <li class="page-item ${currentPage === 1 ? 'disabled' : ''}">
                <a class="page-link" href="#" onclick="changePage(${currentPage - 1})">Previous</a>
            </li>
        `;
        
        // عرض أرقام الصفحات مع إضافة "..." عند الحاجة
        const maxVisiblePages = 5;
        let startPage, endPage;
        
        if (totalPages <= maxVisiblePages) {
            startPage = 1;
            endPage = totalPages;
        } else {
            const halfVisible = Math.floor(maxVisiblePages / 2);
            
            if (currentPage <= halfVisible + 1) {
                startPage = 1;
                endPage = maxVisiblePages;
            } else if (currentPage >= totalPages - halfVisible) {
                startPage = totalPages - maxVisiblePages + 1;
                endPage = totalPages;
            } else {
                startPage = currentPage - halfVisible;
                endPage = currentPage + halfVisible;
            }
        }
        
        // إضافة الصفحة الأولى إذا لزم الأمر
        if (startPage > 1) {
            paginationHTML += `
                <li class="page-item ${1 === currentPage ? 'active' : ''}">
                    <a class="page-link" href="#" onclick="changePage(1)">1</a>
                </li>
            `;
            if (startPage > 2) {
                paginationHTML += `
                    <li class="page-item disabled">
                        <a class="page-link" href="#">...</a>
                    </li>
                `;
            }
        }
        
        // عرض الصفحات المرئية
        for (let i = startPage; i <= endPage; i++) {
            paginationHTML += `
                <li class="page-item ${i === currentPage ? 'active' : ''}">
                    <a class="page-link" href="#" onclick="changePage(${i})">${i}</a>
                </li>
            `;
        }
        
        // إضافة الصفحة الأخيرة إذا لزم الأمر
        if (endPage < totalPages) {
            if (endPage < totalPages - 1) {
                paginationHTML += `
                    <li class="page-item disabled">
                        <a class="page-link" href="#">...</a>
                    </li>
                `;
            }
            paginationHTML += `
                <li class="page-item ${totalPages === currentPage ? 'active' : ''}">
                    <a class="page-link" href="#" onclick="changePage(${totalPages})">${totalPages}</a>
                </li>
            `;
        }
        
        paginationHTML += `
            <li class="page-item ${currentPage === totalPages ? 'disabled' : ''}">
                <a class="page-link" href="#" onclick="changePage(${currentPage + 1})">Next</a>
            </li>
        `;
        
        pagination.innerHTML = paginationHTML;
    }
    window.changePage = function(newPage) {
        if (newPage < 1 || newPage > Math.ceil(totalMovies / moviesPerPage)) {
            return;
        }
        
        currentPage = newPage;
        displayMovies(allMovies); // إعادة عرض الأفلام للصفحة الجديدة
        
        // التمرير لأعلى الصفحة
        window.scrollTo({
            top: 0,
            behavior: 'smooth'
        });
    };

    window.confirmDeleteMovie = function(movieId, movieName) {
        if (confirm(`Are you sure you want to delete the movie "${movieName}"?`)) {
            deleteMovie(movieId);
        }
    };

    async function deleteMovie(movieId) {
        try {
            const response = await fetch(`https://watchly.runasp.net/api/MovieRecommenderAPI/DeleteMovie/${movieId}/${loggedInUser.id}`, {
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

    async function loadInitialMovies() {
        try {
            showLoadingState();
            currentPage = 1; // إعادة تعيين إلى الصفحة الأولى عند التحميل الأولي
            
            // تحميل الأفلام الأولية (على سبيل المثال: الأفلام الأعلى تقييماً)
            const response = await fetch(`${apiConfig.baseUrl}${apiConfig.endpoints.byYearsRange}/1980/2025/Sci_Fi/${loggedInUser.id}`);
            
            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }
            
            const movies = await response.json();
            
            // تحميل قوائم المستخدم إذا كان مسجل الدخول
            if (userJson) {
                const user = JSON.parse(userJson);
                
                // تحميل المفضلة
                const favResponse = await fetch(`https://watchly.runasp.net/api/UsersAPI/GetAllFavorateMoviesforUser?UserID=${user.id}`);
                if (favResponse.ok) {
                    const favorites = await favResponse.json();
                    const favoriteIds = favorites.map(movie => movie.id);
                    localStorage.setItem('userFavorites', JSON.stringify(favoriteIds));
                }
                
                // تحميل Unlike
                const unlikedResponse = await fetch(`https://watchly.runasp.net/api/UsersAPI/GetAllUnlikedMoviesToUser/${user.id}`);
                if (unlikedResponse.ok) {
                    const unlikedMovies = await unlikedResponse.json();
                    const unlikedIds = unlikedMovies.map(movie => movie.id);
                    localStorage.setItem('userUnliked', JSON.stringify(unlikedIds));
                }
                
                // تحميل Watched
                const watchedResponse = await fetch(`https://watchly.runasp.net/api/UsersAPI/GetAllWatchedMoviesForUser/${user.id}`);
                if (watchedResponse.ok) {
                    const watchedMovies = await watchedResponse.json();
                    const watchedIds = watchedMovies.map(movie => movie.id);
                    localStorage.setItem('userWatched', JSON.stringify(watchedIds));
                }
            }
            
            displayMovies(movies);
        } catch (error) {
            console.error('Failed to load initial movies:', error);
            showErrorState(error);
        }
    }

    async function applyFilters() {
        try {
            showLoadingState();
            currentPage = 1; // إعادة تعيين إلى الصفحة الأولى عند تطبيق فلاتر جديدة
            
            const selectedGenres = Array.from(genreCheckboxes)
                .filter(checkbox => checkbox.checked)
                .map(checkbox => {
                    switch(checkbox.id) {
                        case 'action': return 'Action';
                        case 'adventure': return 'Adventure';
                        case 'animation': return 'Animation';
                        case 'comedy': return 'Comedy';
                        case 'crime': return 'Crime';
                        case 'documentary': return 'Documentary';
                        case 'drama': return 'Drama';
                        case 'family': return 'Family';
                        case 'fantasy': return 'Fantasy';
                        case 'history': return 'History';
                        case 'horror': return 'Horror';
                        case 'music': return 'Music';
                        case 'mystery': return 'Mystery';
                        case 'romance': return 'Romance';
                        case 'sci-fi': return 'Sci_Fi';
                        case 'sport': return 'Sport';
                        case 'thriller': return 'Thriller';
                        case 'war': return 'War';
                        case 'western': return 'Western';
                        default: return checkbox.id;
                    }
                });
    
            // إذا لم يتم اختيار أي أنواع، قم بتحميل الأفلام الأولية
            if (selectedGenres.length === 0) {
                await loadInitialMovies();
                return;
            }
            
            const selectedGenresString = selectedGenres.join(',');
            const startYear = StartYear.value;
            const endYear = EndYear.value;
            const minRating = parseFloat(ratingRange.value);
            const SortByValue = sortBy.value;
            const OrderValue = SortByValue === 'rating' ? 'DESC' : SortByValue === 'newest' ? 'DESC' : 'ASC';
            
            let apiUrl = `${apiConfig.baseUrl}${apiConfig.endpoints.byYearsRangeAndGenreSorted}/${startYear}/${endYear}/${selectedGenresString}/${minRating}/${SortByValue}/${OrderValue}/${loggedInUser.id}`;
            
            console.log("Request URL:", apiUrl);
            
            const response = await fetch(apiUrl);
            
            if (!response.ok) {
                if (response.status === 404) {
                    throw new Error(`No movies found for the selected filters. Please check your unlike movies.`);
                } else {
                    throw new Error(`HTTP error! status: ${response.status}`);
                }
            }
            
            const movies = await response.json();
            
            // تحميل قوائم المستخدم (المفضلة، المشاهدة، غير المرغوبة)
            if (userJson) {
                const user = JSON.parse(userJson);
                
                // تحميل المفضلة
                const favResponse = await fetch(`https://watchly.runasp.net/api/UsersAPI/GetAllFavorateMoviesforUser?UserID=${user.id}`);
                if (favResponse.ok) {
                    const favorites = await favResponse.json();
                    const favoriteIds = favorites.map(movie => movie.id);
                    localStorage.setItem('userFavorites', JSON.stringify(favoriteIds));
                }
                
                // تحميل Unlike
                const unlikedResponse = await fetch(`https://watchly.runasp.net/api/UsersAPI/GetAllUnlikedMoviesToUser/${user.id}`);
                if (unlikedResponse.ok) {
                    const unlikedMovies = await unlikedResponse.json();
                    const unlikedIds = unlikedMovies.map(movie => movie.id);
                    localStorage.setItem('userUnliked', JSON.stringify(unlikedIds));
                }
                
                // تحميل Watched
                const watchedResponse = await fetch(`https://watchly.runasp.net/api/UsersAPI/GetAllWatchedMoviesForUser/${user.id}`);
                if (watchedResponse.ok) {
                    const watchedMovies = await watchedResponse.json();
                    const watchedIds = watchedMovies.map(movie => movie.id);
                    localStorage.setItem('userWatched', JSON.stringify(watchedIds));
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
            const response = await fetch(`https://watchly.runasp.net/api/UsersAPI/CheckIfMovieIsFavorate?UserID=${userId}&MovieID=${movieId}`);

            if (!response.ok) {
                return false;
            }

            const message = await response.text();
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
            const response = await fetch(`https://watchly.runasp.net/api/UsersAPI/CheckIfMovieIsFavorate?UserID=${user.id}&MovieID=${movieId}`);
            return response.ok;
        } catch (error) {
            console.error('Error checking favorite:', error);
            return false;
        }
    }

    window.markAsAdult = async function(movieId, buttonElement) {
        const userJson = localStorage.getItem('loggedInUser') || sessionStorage.getItem('loggedInUser');
        if (!userJson) {
            window.location.href = 'login.html';
            return;
        }
        
        const user = JSON.parse(userJson);
        const icon = buttonElement.querySelector('i');
        
        try {
            const response = await fetch('https://watchly.runasp.net/api/MovieRecommenderAPI/MarkMovieAsAdult', {
                method: 'PUT',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify({
                    UserID: user.id,
                    MovieID: movieId
                })
            });
    
            if (!response.ok) {
                const error = await response.text();
                throw new Error(error);
            }
    
            // تحديث الأيقونة
            icon.classList.remove('bi-eye-slash');
            icon.classList.add('bi-eye-slash-fill', 'text-danger');
            alert('Movie marked as adult successfully!');
        } catch (error) {
            console.error('Error marking movie as adult:', error);
            alert('Failed to mark movie as adult: ' + error.message);
        }
    }
    
    window.unmarkAsAdult = async function(movieId, buttonElement) {
        const userJson = localStorage.getItem('loggedInUser') || sessionStorage.getItem('loggedInUser');
        if (!userJson) {
            window.location.href = 'login.html';
            return;
        }
        
        const user = JSON.parse(userJson);
        const icon = buttonElement.querySelector('i');
        
        try {
            const response = await fetch('https://watchly.runasp.net/api/MovieRecommenderAPI/UnMarkMovieAsAdult', {
                method: 'PUT',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify({
                    UserID: user.id,
                    MovieID: movieId
                })
            });
    
            if (!response.ok) {
                const error = await response.text();
                throw new Error(error);
            }
    
            // تحديث الأيقونة
            icon.classList.remove('bi-eye-slash-fill', 'text-danger');
            icon.classList.add('bi-eye-slash');
            alert('Movie unmarked as adult successfully!');
        } catch (error) {
            console.error('Error unmarking movie as adult:', error);
            alert('Failed to unmark movie as adult: ' + error.message);
        }
    }
    
    async function loadFavorites() {
        const userJson = localStorage.getItem('loggedInUser') || sessionStorage.getItem('loggedInUser');
        if (!userJson) return;
        
        const user = JSON.parse(userJson);
        
        try {
            const response = await fetch(`https://watchly.runasp.net/api/UsersAPI/GetAllFavorateMoviesforUser?UserID=${user.id}`);
            if (!response.ok) throw new Error('Failed to load favorites');
            
            const favorites = await response.json();
            const favoriteIds = favorites
            .filter(movie => movie && movie.id !== undefined)
            .map(movie => movie.id);

        if (favorites.length === 0) {
            displayEmptyListMessage('No favorite movies found');
            return;
        }

            localStorage.setItem('userFavorites', JSON.stringify(favoriteIds));
        displayMovies(favorites);
        } catch (error) {
            console.error('Error loading favorites:', error);
        }
    }
    
    document.addEventListener('DOMContentLoaded', loadFavorites);

    window.applyFilters = applyFilters;
});