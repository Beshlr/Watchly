using clsDataAccess;
using MovieRecommendations_BusinessLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace clsBusinessLayer
{
    public class clsFilters
    {
        public static List<MovieDTO> GetMoviesAfterFilterItToUser(
            List<MovieDTO> movies,
            clsUsers user = null,
            string selectedGenres = null,
            bool excludeUserDislikes = true,
            bool matchAllSelectedGenres = true,
            bool exactGenresMatch = false)
        {
            if (movies == null || movies.Count == 0)
                return new List<MovieDTO>();

            List<MovieDTO> filteredMovies = new List<MovieDTO>();

            List<string> dislikedGenreCombinations = excludeUserDislikes && user != null
                ? clsUsers.GetUnLikedGenresForUser(user.UserID) ?? new List<string>()
                : new List<string>();

            List<string> selectedGenresList = !string.IsNullOrWhiteSpace(selectedGenres)
                ? selectedGenres.Split(',', StringSplitOptions.RemoveEmptyEntries)
                               .Select(g => g.Trim())
                               .Distinct()
                               .OrderBy(g => g)
                               .ToList()
                : new List<string>();
            bool IsAdult = true;
            if (user != null)
                IsAdult = user.Age > 17;

            foreach (MovieDTO movie in movies)
            {
                if (movie.Genre == null)
                    continue;

                var movieGenres = movie.Genre.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                           .Select(g => g.Trim())
                                           .Distinct()
                                           .OrderBy(g => g)
                                           .ToList();

                bool isDisliked = false;
                if (excludeUserDislikes && dislikedGenreCombinations.Count > 0)
                {
                    var sortedGenres = movieGenres.OrderBy(g => g).ToArray();
                    string normalizedGenres = string.Join(", ", sortedGenres);
                    isDisliked = dislikedGenreCombinations.Contains(normalizedGenres);
                }

                bool matchesSelectedGenres = selectedGenresList.Count == 0;
                if (selectedGenresList.Count > 0)
                {
                    if (exactGenresMatch)
                    {
                        matchesSelectedGenres = movieGenres.Count == selectedGenresList.Count &&
                                              movieGenres.SequenceEqual(selectedGenresList);
                    }
                    else
                    {
                        matchesSelectedGenres = matchAllSelectedGenres
                            ? selectedGenresList.All(genre => movieGenres.Contains(genre))
                            : selectedGenresList.Any(genre => movieGenres.Contains(genre));
                    }
                }

                if (!isDisliked && matchesSelectedGenres)
                {
                    if(user != null)
                    {
                        if (movie.RatingCode == "X")
                        {
                            movie.PosterImageURL = @"../images/placeHolderOlderMovies.jpg";
                            if(!IsAdult)
                            {
                                continue;
                            }
                        }
                            filteredMovies.Add(movie);
                        

                    }
                    else
                        filteredMovies.Add(movie);

                }
            }

            return filteredMovies;
        }

        public static MovieDTO GetSuggestedMovieWithInterstedGenres(List<MovieDTO> MoviesToFilter, List<string> SelectedGenres)
        {
            MovieDTO movie = null;

            if (MoviesToFilter == null || MoviesToFilter.Count == 0 || SelectedGenres == null || SelectedGenres.Count == 0)
                return null;

            // Convert SelectedGenres list to comma-separated string for the filter function
            string selectedGenresString = string.Join(",", SelectedGenres);

            // Get filtered movies that match ALL selected genres
            List<MovieDTO> filteredMovies = GetMoviesAfterFilterItToUser(
                movies: MoviesToFilter,
                selectedGenres: selectedGenresString,
                matchAllSelectedGenres: true,
                exactGenresMatch: false
            );

            // If there are matching movies, select one randomly
            if (filteredMovies != null && filteredMovies.Count > 0)
            {
                Random random = new Random();
                int randomIndex = random.Next(0, filteredMovies.Count);
                movie = filteredMovies[randomIndex];
            }

            return movie;
        }
    }
}
