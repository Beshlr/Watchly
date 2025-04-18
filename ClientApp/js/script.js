document.addEventListener('DOMContentLoaded', function() {
    const searchForm = document.querySelector('.d-flex');
    const searchInput = document.querySelector('.form-control.me-2');
    const popularMoviesContainer = document.querySelector('.row:first-of-type');
    const recommendedMoviesContainer = document.querySelector('.row:last-of-type');
    const baseApiUrl = 'https://localhost:7009/api/MovieRecommenderAPI';

    async function fetchData(url) {
        try {
            const response = await fetch(url);
            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }
            return await response.json();
        } catch (error) {
            console.error('Error fetching data:', error);
            return null;
        }
    }

    async function loadPopularMovies() {
        const movies = await fetchData(`${baseApiUrl}/GetTop100MovieWithGenre?GenreName=Action`);
        if (movies) {
            displayMovies(movies.slice(0, 4), popularMoviesContainer);
        } else {
            popularMoviesContainer.innerHTML = '<p class="text-danger">Error loading popular movies.</p>';
        }
    }

    async function searchMovies(query) {
        const movies = await fetchData(`${baseApiUrl}/StartName/${query}`);
        if (movies) {
            displayMovies(movies, recommendedMoviesContainer);
        } else {
            recommendedMoviesContainer.innerHTML = `<p class="text-danger">No results for "${query}"</p>`;
        }
    }

    function displayMovies(movies, container) {
        container.innerHTML = movies?.map(movie => `
            <div class="col-md-4 col-lg-3">
                <div class="card movie-card">
                    <img src="${movie.posterImageURL || 'https://via.placeholder.com/300x450'}" class="card-img-top" alt="${movie.Title}">
                    <div class="card-body">
                        <h5 class="card-title">${movie.movieName}</h5>
                        <p class="card-text">${movie.year} • ★ ${movie.rate}</p>
                        <a href="#" class="btn btn-sm btn-outline-primary">Details</a>
                    </div>
                </div>
            </div>
        `).join('') || '<p class="text-muted">No movies found.</p>';
    }

    searchForm.addEventListener('submit', (e) => {
        e.preventDefault();
        const query = searchInput.value.trim();
        if (query) searchMovies(query);
    });

    loadPopularMovies();
    searchInput.addEventListener('input', async function(e) {
        const query = e.target.value.trim();
        
        if (query.length === 0) 
            searchResults.style.display = 'none';
        else if (query.length >= 2) { 
            const movies = await fetchData(`${baseApiUrl}/NameHasWord/${query}`);
            showQuickResults(movies?.slice(0, 3)); // عرض أول 3 نتائج فقط
        }
        else {
            searchResults.style.display = 'none';
        }
    });

    function showQuickResults(movies) {
        if (!movies || movies.length === 0) {
            searchResults.style.display = 'none';
            return;
        }

        searchResults.innerHTML = movies.map(movie => `
            <div class="search-result-item" onclick="selectMovie('${movie.id}')">
                <img src="${movie.posterImageURL || 'https://via.placeholder.com/50x75'}" alt="${movie.Title}">
                <div class="info">
                    <h6>${movie.movieName}</h6>
                    <small>${movie.year} • ★ ${movie.rate}</small>
                </div>
            </div>
        `).join('');

        searchResults.style.display = 'block';
    }

    window.selectMovie = function(movieId) {
        window.location.href = `movie-details.html?id=${movieId}`; // انتقل لصفحة التفاصيل
    };

    document.addEventListener('click', function(e) {
        if (!searchForm.contains(e.target)) {
            searchResults.style.display = 'none';
        }
    });
});