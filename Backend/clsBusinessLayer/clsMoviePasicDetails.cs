using clsDataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using clsDataAccess;
using System.Text.Json.Serialization;
using MovieRecommendations_DataLayer;

namespace clsBusinessLayer
{
    public class clsMoviePasicDetails
    {
        

        // Set object from data transfer object to return data
        public MovieDTO MDTO { get {
                return new MovieDTO(this.ID, this.MovieName, this.Year, this.Rate, this.PosterImageURL
            , this.TrailerURL, this.ContentRating, this.Duration, this.Language, this.Country, this.AspectRatio, this.Genre,this.IMDbMovieURL);
            } }


        public int ID { get; set; }
        public string MovieName { get; set; }
        public int Year { get; set; }
        public float Rate { get; set; }
        public string PosterImageURL { get; set; }
        public string TrailerURL { get; set; }
        public string ContentRating { get; set; }
        public string Duration { get; set; }
        public string Language { get; set; }
        public string Country { get; set; }
        public float AspectRatio { get; set; }
        public string Genre { get; set; }
        public string IMDbMovieURL { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum enGenres {
            Action,
            Adventure,
            Animation,
            Biography,
            Comedy,
            Crime,
            Documentary,
            Drama,
            Family,
            Fantasy,
            Film_Noir,
            Game_Show,
            History,
            Horror,
            Music,
            Musical,
            Mystery,
            News,
            Reality_TV,
            Romance,
            Sci_Fi,
            Short,
            Sport,
            Thriller,
            War,
            Western
        };

        public static enGenres enGenre = enGenres.Comedy;

        public static MovieDTO GetMovieByID(int ID)
        {
            return clsMoviePasicDetailsData.GetMovieByID(ID);
        }

        /// <summary>
        /// Get the movies that start with the given name.
        /// </summary>
        /// <param name="MovieName"></param>
        /// <returns></returns>
        public static List<MovieDTO> GetMoviesStartWithName(string MovieName)
        {
            return clsMoviePasicDetailsData.GetMoviesStartWithName(MovieName);
        }


        /// <summary>
        /// Get the movies that contain the given word in their name.
        /// </summary>
        /// <param name="PieceOfName"></param>
        /// <returns></returns>
        public static List<MovieDTO> GetMoviesWithNameHasWord(string PieceOfName)
        {
            return clsMoviePasicDetailsData.GetMoviesWithNameHasWord(PieceOfName);
        }

        public static string GetGenreName(enGenres genre)
        {
            
            if (genre == null)
            {
                return null;
            }

            //Check if Genre name has underscore or not.
            else if (genre == enGenres.Film_Noir)
                return "Film-Noir";
            else if (genre == enGenres.Game_Show)
                return "Game-Show";
            else if (genre == enGenres.Reality_TV)
                return "Reality-TV";
            else if (genre == enGenres.Sci_Fi)
                return "Sci-Fi";

            // If the genre name doesn't have underscore then we can use Enum.GetName to get the name of the enum.
                
            return Enum.GetName(typeof(enGenres), genre);
        }

        /// <summary>
        /// Get the top 100 movies with the genre name and order them by rating According to IMDb in descending order.
        /// </summary>
        /// <param name="genreName"></param>
        /// <returns></returns>
        public static List<MovieDTO> GetTop100MovieWithGenreAndOrderThemByRatingDESC(enGenres genre)
        {
            //Get the genre name from enum when we need to use it.

            string genreName= GetGenreName(genre);

            return clsMoviePasicDetailsData.GetTop100MovieWithGenreAndOrderThemByRatingDESC(genreName);
        }

        /// <summary>
        /// Confirm that the genre name is exist or not.
        /// </summary>
        /// <param name="genreName"></param>
        /// <returns></returns>
        public static bool ConfirmThatTheGenreNameIsExist(string genreName)
        {
            if (genreName == null)
            {
                return false;
            }
            else
            {
                foreach (string name in Enum.GetNames(typeof(enGenres)))
                {
                    
                    if (name.ToUpper() == genreName)
                    {
                        return true;
                    }
                }
                return false;
            }
        }
        /// <summary>
        /// Get the top 100 movies with the genre name and year and order them by rating According to IMDb in descending order.
        /// </summary>
        /// <param name="genreName"> </param>
        /// <param name="Year"></param>
        /// <returns></returns>
        public static List<MovieDTO> GetTop100MovieWithGenreInYearAndOrderThemByRatingDESC(enGenres genreName, int Year)
        {
            //Get the genre name from enum when we need to use it.
            string genre = GetGenreName(genreName);
            return clsMoviePasicDetailsData.GetTop100MovieWithGenreInYearAndOrderThemByRatingDESC(genre, Year);
        }

        /// <summary>
        /// Get the top 10 movies recommended for the user with the given ID according to his active in site.
        /// </summary>
        /// <param name="UserID"></param>
        /// <returns></returns>
        public static List<MovieDTO> GetTop10MoviesRecommendedForUserWithID(int UserID)
        {
            return clsMoviePasicDetailsData.GetTop10MoviesRecommendedForUserWithID(UserID);
        }

        /// <summary>
        /// Get the top 100 movies between two years and order them by rating According to IMDb in descending order.
        /// </summary>
        /// <param name="Year1"></param>
        /// <param name="Year2"></param>
        /// <returns></returns>
        public static List<MovieDTO> GetTop100MovieBetweenTwoYears(int Year1, int Year2)
        {
            return clsMoviePasicDetailsData.GetTop100MoviesBetweenTwoYears(Year1, Year2);
        }

        
    }
}
