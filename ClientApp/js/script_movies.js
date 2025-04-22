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
        baseUrl: 'https://localhost:7009/api/MovieRecommenderAPI',
        endpoints: {
            byGenre: '/GetTop100MovieWithGenre',
            byGenreAndYear: '/GetTop100MovieWithGenreInYear',
            byYearsRange: '/GetTop100MovieBetweenTwoYears',
            byYearsRangeAndGenreSorted: '/GetTop100MovieWithFiltersAndSorting'
        }
    };

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
        
        // تطبيق الفلاتر عند النقر على الزر
        applyFiltersBtn.addEventListener('click', applyFilters);
        
        searchBtn.addEventListener('click', getMoviesBySearch);

        // إعادة تعيين الفلاتر
        resetFiltersBtn.addEventListener('click', function() {
            // إعادة تعيين صناديق الاختيار
            genreCheckboxes.forEach(checkbox => {
                checkbox.checked = ['action', 'comedy', 'drama', 'sci-fi'].includes(checkbox.id);
            });
            
            // إعادة تعيين المنزلقات
            // const currentYear = new Date().getFullYear();
            StartYear.value = 2021;
            StartYearMaxValue.textContent = 2021;
            
            EndYear.value = 2025;
            EndYearMaxValue.textContent = 2025;
            
            ratingRange.value = 6;
            ratingValue.textContent = '6.0';
            
            // تطبيق الفلاتر من جديد
            applyFilters();
        });
    }

    
    
    async function loadInitialMovies() {
        try {
            showLoadingState();
            
            // جلب الأفلام الشائعة (أفلام الأكشن كمثال)
            const response = await fetch(`${apiConfig.baseUrl}${apiConfig.endpoints.byGenre}?GenreName=Sci_Fi`);
            
            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }
            
            const movies = await response.json();
            displayMovies(movies);
        } catch (error) {
            console.error('Failed to load initial movies:', error);
            showErrorState(error);
        }
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
    searchResults.style.display = 'none';
    searchInput.value = '';
};

// إخفاء نتائج البحث عند النقر خارجها
document.addEventListener('click', function(e) {
    if (!searchInput.contains(e.target) && !searchResults.contains(e.target)) {
        searchResults.style.display = 'none';
    }
});



    async function applyFilters() {
        try {
            showLoadingState();
            
            // 1. جمع معايير التصفية
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
            displayMovies(movies);
            
        } catch (error) {
            console.error('Failed to apply filters:', error);
            showErrorState(error);
        }
    }
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
        for (let i = 0; i < emptyStars; i++) stars += '<i class="bi bi-star text-warning"></i>';
        
        return stars;
    }

    function displayMovies(movies) {
        if (!movies || movies.length === 0) {
            moviesGrid.innerHTML = `
                <div class="col-12 text-center py-5">
                    <p class="text-muted">No movies found matching your criteria</p>
                </div>
            `;
            return;
        }
        
        moviesGrid.innerHTML = movies.map(movie => `
            <div class="col-md-4 col-lg-3 mb-4 movie-card-container">
                <div class="card h-100" onclick="window.open('${movie.imDbMovieURL || '#'}', '_blank')" style="cursor: pointer;">
                    <img src="${movie.posterImageURL || 'https://via.placeholder.com/300x450'}" 
                         class="card-img-top" 
                         alt="${movie.movieName}"
                         onerror="this.src='https://via.placeholder.com/300x450?text=Poster+Not+Found'">
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
                </div>
            </div>
        `).join('');
    }
    
    // جعل الدالة متاحة عالمياً لأزرار إعادة المحاولة
    window.applyFilters = applyFilters;
});