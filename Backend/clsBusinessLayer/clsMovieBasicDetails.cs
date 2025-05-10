using clsDataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using clsDataAccess;
using System.Text.Json.Serialization;
using MovieRecommendations_DataLayer;
using System.Xml.Linq;
using MovieRecommendations_BusinessLayer;

namespace clsBusinessLayer
{
    public class clsMovieBasicDetails
    {
        

        // Set object from data transfer object to return data
        public MovieDTO MDTO { get {
                return new MovieDTO(this.ID, this.MovieName, this.Year, this.Rate, this.PosterImageURL
            , this.TrailerURL,  this.Duration, this.Language, this.Country, this.AspectRatio, this.Genre,this.IMDbMovieURL,this.Keywords, this.Popularity);
            } }


        public int ID { get; set; }
        public string MovieName { get; set; }
        public int Year { get; set; }
        public float Rate { get; set; }
        public string PosterImageURL { get; set; }
        public string TrailerURL { get; set; }
        //public string ContentRating { get; set; }
        public string Duration { get; set; }
        public string Language { get; set; }
        public string Country { get; set; }
        public float AspectRatio { get; set; }
        public string Genre { get; set; }
        public string IMDbMovieURL { get; set; }
        public string Keywords { get; set; }
        public int Popularity { get; set; }
        public enum enMode { AddNew = 0, Update = 1 };
        public enMode Mode = enMode.AddNew;

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

        public clsMovieBasicDetails(MovieDTO MDTO, enMode mode= enMode.AddNew)
        {
            this.ID = MDTO.ID;
            this.MovieName = MDTO.MovieName;
            this.Year = MDTO.Year;
            this.Rate = MDTO.Rate;
            this.PosterImageURL = MDTO.PosterImageURL;
            this.TrailerURL = MDTO.TrailerURL;
            
            this.Duration = MDTO.Duration;
            this.Language = MDTO.Language;
            this.Country = MDTO.Country;
            this.AspectRatio = MDTO.AspectRatio;
            this.Genre = MDTO.Genre;
            this.IMDbMovieURL = MDTO.IMDbMovieURL;
            this.Keywords = MDTO.Keywords;
            Mode = mode;
        }

        public static enGenres enGenre = enGenres.Comedy;

        public static MovieDTO GetMovieByID(int ID)
        {
            return clsMovieBasicDetailsData.GetMovieByID(ID);
        }

        public static MovieDTO GetMovieByName(string MovieName)
        {
            return clsMovieBasicDetailsData.GetMovieByName(MovieName);
        }

        public static bool DeleteMovieByID(int ID)
        {
            return clsMovieBasicDetailsData.DeleteMovieByID(ID);
        }

        /// <summary>
        /// Get the movies that start with the given name.
        /// </summary>
        /// <param name="MovieName"></param>
        /// <returns></returns>
        public static List<MovieDTO> GetMoviesStartWithName(string MovieName, clsUsers LoggedInUser)
        {
            return clsFilters.GetMoviesAfterFilterItToUser(clsMovieBasicDetailsData.GetMoviesStartWithName(MovieName), LoggedInUser);
        }


        /// <summary>
        /// Get the movies that contain the given word in their name.
        /// </summary>
        /// <param name="PieceOfName"></param>
        /// <returns></returns>
        public static List<MovieDTO> GetMoviesWithNameHasWord(string PieceOfName, clsUsers LoggedInUser)
        {
            return clsFilters.GetMoviesAfterFilterItToUser(clsMovieBasicDetailsData.GetMoviesWithNameHasWord(PieceOfName), LoggedInUser);
        }

        public static List<MovieDTO> GetTop100MovieBetweenTwoYearsWithGenreAndOrderRating(string OrderValue, int StartYear,
                                                                                         int EndYear, string GenresList,float MinRatingValue, clsUsers LoggedInUser, string SortBy="Year")
        {
            //Check if the user has selected the rating order value or not.
            if (SortBy.ToLower() == "oldest first" || SortBy.ToLower() == "newest first" || SortBy.ToLower() == "year")
            {
                if (SortBy.ToLower() == "oldest first")
                    OrderValue = "ASC";
                else if(SortBy.ToLower() == "newest first")
                    OrderValue = "DESC";
                SortBy = "title_year";
            }
            else if (SortBy.ToLower() == "rating")
            {
                SortBy = "imdb_score";
            }
            else
            {
                SortBy = "title_year";
            }
            return clsFilters.GetMoviesAfterFilterItToUser(
                clsMovieBasicDetailsData.GetTop100MovieBetweenTwoYearsWithGenreAndOrderRating(
                    OrderValue, StartYear, EndYear, GenresList, MinRatingValue, SortBy), LoggedInUser,GenresList);
        }

        public static bool IsMovieExist(string MovieName,int Year, ref int MovieID)
        {
            return clsMovieBasicDetailsData.IsMovieExist(MovieName, Year, ref MovieID);
        }

        public static bool IsMovieExist(int MovieID)
        {
            return clsMovieBasicDetailsData.IsMovieExist(MovieID);
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
                return "Sci_Fi";

            // If the genre name doesn't have underscore then we can use Enum.GetName to get the name of the enum.
                
            return Enum.GetName(typeof(enGenres), genre);
        }

        /// <summary>
        /// Get the top 100 movies with the genre name and order them by rating According to IMDb in descending order.
        /// </summary>
        /// <param name="genreName"></param>
        /// <returns></returns>
        public static List<MovieDTO> GetTop100MovieWithGenreAndOrderThemByRatingDESC(enGenres genre, clsUsers LoggedInUser)
        {
            //Get the genre name from enum when we need to use it.

            string genreName= GetGenreName(genre);

            return clsFilters.GetMoviesAfterFilterItToUser(
                clsMovieBasicDetailsData.GetTop100MovieWithGenreAndOrderThemByRatingDESC(genreName), LoggedInUser);
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
        public static List<MovieDTO> GetTop100MovieWithGenreInYearAndOrderThemByRatingDESC(enGenres genreName, int Year, clsUsers LoggedInUser)
        {
            //Get the genre name from enum when we need to use it.
            string genre = GetGenreName(genreName);
            return clsFilters.GetMoviesAfterFilterItToUser(
                clsMovieBasicDetailsData.GetTop100MovieWithGenreInYearAndOrderThemByRating(genre, Year), LoggedInUser);
        }

        /// <summary>
        /// Get the top 10 movies recommended for the user with the given ID according to his active in site.
        /// </summary>
        /// <param name="UserID"></param>
        /// <returns></returns>
        public static List<MovieDTO> GetTop10MoviesRecommendedForUserWithID(int UserID)
        {
            clsUsers LoggedInUser = clsUsers.Find(UserID);

            return clsFilters.GetMoviesAfterFilterItToUser(clsMovieBasicDetailsData.GetTop10MoviesRecommendedForUserWithID(UserID), LoggedInUser);
        }

        /// <summary>
        /// Get the top 100 movies between two years and order them by rating According to IMDb in descending order.
        /// </summary>
        /// <param name="Year1"></param>
        /// <param name="Year2"></param>
        /// <returns></returns>
        public static List<MovieDTO> GetTop100MovieBetweenTwoYears(int Year1, int Year2, clsUsers LoggedInUser, string Genre="Action")
        {
            return clsFilters.GetMoviesAfterFilterItToUser(clsMovieBasicDetailsData.GetTop100MoviesBetweenTwoYears(Year1, Year2, Genre), LoggedInUser);
        }

        public static List<MovieDTO> GetTop10MoviesWithKeyword(string Keyword)
        {
            return clsMovieBasicDetailsData.GetTop10MoviesWithKeyword(Keyword);
        }

        public static List<MovieDTO> GetTop15TrendingMovies(clsUsers LoggedInUser )
        {
            return clsFilters.GetMoviesAfterFilterItToUser(clsMovieBasicDetailsData.GetTop15TrendingMovies(), LoggedInUser);
        }

        public static MovieDTO GetRandomSuggestedMovieByGenres(string selectedGenres)
        {
            List<string> selectedGenresList = !string.IsNullOrWhiteSpace(selectedGenres)
                 ? selectedGenres.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                .Select(g => g.Trim())
                                .Distinct()
                                .OrderBy(g => g)
                                .ToList()
                 : new List<string>();

            Random rnd = new Random();
            int number = rnd.Next(0, selectedGenresList.Count());


            List<MovieDTO> movies = clsMovieBasicDetailsData.GetMoviesWithGenre(selectedGenresList[number]);

            return clsFilters.GetSuggestedMovieWithInterstedGenres(movies, selectedGenresList);
        }

        private bool _AddNewMovie()
        {
            this.ID = clsMovieBasicDetailsData.AddNewMovie(this.MDTO);

            return this.ID != -1;
        }

        public bool Save()
        {
            switch(Mode)
            {
                case enMode.AddNew:
                    if(_AddNewMovie())
                    {
                        this.Mode = enMode.Update;
                        return true;
                    }
                    else
                        return false;
                case enMode.Update:
                    if (_UpdateMovieByID())
                    {
                        return true;
                    }
                    else
                        return false;

            }

            return false;
        }

        private bool _UpdateMovieByID()
        {
            throw new NotImplementedException();
        }

        
    }
}
