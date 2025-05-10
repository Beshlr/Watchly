using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Data.SqlTypes;
using System.Data;
using Microsoft.Data.SqlClient;
using MovieRecommendations_DataAccess;
using System.Security.Cryptography.Pkcs;
using MovieRecommendations_DataLayer;

namespace clsDataAccess
{
    public class MovieDTO
    {
        #nullable disable

        public MovieDTO(int iD, string movieName, int year, float rate, string posterImageURL,
            string trailerURL, string duration, string language,
            string country, float aspectRatio, string genre, string iMDbMovieURL, string keywords="", double popularity = -1,string ratingCode="")
        {

            ID = iD;
            MovieName = movieName;
            Year = year;
            Rate = rate;
            PosterImageURL = posterImageURL;
            TrailerURL = trailerURL;
            Duration = duration;
            Language = language;
            Country = country;
            AspectRatio = aspectRatio;
            Genre = genre;
            IMDbMovieURL = iMDbMovieURL;
            Keywords = keywords;
            Popularity = popularity;
            RatingCode = ratingCode;
        }

        public int ID { get; set; }
        public string MovieName { get; set; }
        public int Year { get; set; }
        public float Rate { get; set; }
        public string PosterImageURL { get; set; }
        public string TrailerURL { get; set; }
        public string Duration { get; set; }
        public int DurationInMinutes
        {
            get
            {
                if (string.IsNullOrEmpty(Duration))
                    return 0;
                string[] parts = Duration.Split(' ');
                if (parts.Length > 0 && int.TryParse(parts[0], out int minutes))
                {
                    return minutes;
                }
                return 0;
            }
            set
            {
                Duration = $"{value} min";
            }
        }

        public string Language { get; set; }
        public string Country { get; set; }
        public float AspectRatio { get; set; }
        public string Genre { get; set; }
        public string IMDbMovieURL { get; set; }
        public string Keywords { get; set; }
        public double Popularity { get; set; }
        public string RatingCode { get; set; }
        public override bool Equals(object obj)
        {
            if (obj is not MovieDTO other)
                return false;

            return string.Equals(MovieName, other.MovieName, StringComparison.OrdinalIgnoreCase)
                   && Year == other.Year;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(MovieName?.ToLowerInvariant(), Year);
        }

    }

    public class clsMovieBasicDetailsData
    {
        public static MovieDTO GetMovieByID(int ID)
        {
            MovieDTO MDTO = null;

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {

                using (SqlCommand cmd = new SqlCommand("SP_GetMovieBasicInfo", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@MovieID", ID);
                    var ReturnValue = new SqlParameter("@ReturnValue", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.Output
                    };

                    connection.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            MDTO = new MovieDTO
                            (
                                ID,
                                reader.IsDBNull(reader.GetOrdinal("movie_title")) ? null : reader.GetString(reader.GetOrdinal("movie_title")),
                                reader.IsDBNull(reader.GetOrdinal("title_year")) ? 0 : reader.GetInt32(reader.GetOrdinal("title_year")),
                                reader.IsDBNull(reader.GetOrdinal("imdb_score")) ? 0f : (float)reader.GetDouble(reader.GetOrdinal("imdb_score")),
                                reader.IsDBNull(reader.GetOrdinal("poster_url")) ? @"https://www.movienewz.com/img/films/poster-holder.jpg"
                                : reader.GetString(reader.GetOrdinal("poster_url")),
                                reader.IsDBNull(reader.GetOrdinal("trailer_url")) ? null : reader.GetString(reader.GetOrdinal("trailer_url")),
                                reader.IsDBNull(reader.GetOrdinal("Duration")) ? null : reader.GetString(reader.GetOrdinal("duration")),
                                reader.IsDBNull(reader.GetOrdinal("language")) ? null : reader.GetString(reader.GetOrdinal("language")),
                                reader.IsDBNull(reader.GetOrdinal("country")) ? null : reader.GetString(reader.GetOrdinal("country")),
                                reader.IsDBNull(reader.GetOrdinal("aspect_ratio")) ? 0f : (float)reader.GetDouble(reader.GetOrdinal("aspect_ratio")),
                                reader.IsDBNull(reader.GetOrdinal("Genres")) ? null : reader.GetString(reader.GetOrdinal("Genres")),
                                reader.IsDBNull(reader.GetOrdinal("movie_imdb_link")) ? null : reader.GetString(reader.GetOrdinal("movie_imdb_link")),"",-1,
                                reader.IsDBNull(reader.GetOrdinal("RatingCode")) || reader.GetString(reader.GetOrdinal("RatingCode")) == "N/A" ? null 
                                                                                    : reader.GetString(reader.GetOrdinal("RatingCode")) 
                                
                            );
                        }
                    }
                }
            }

            return MDTO;
        }

        public static MovieDTO GetMovieByName(string MovieName)
        {
            MovieDTO MDTO = null;
            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SP_GetMovieByName", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@MovieName", MovieName);
                    connection.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            MDTO = new MovieDTO
                            (
                                reader.IsDBNull(reader.GetOrdinal("ID")) ? 0 : reader.GetInt32(reader.GetOrdinal("ID")),
                                reader.IsDBNull(reader.GetOrdinal("movie_title")) ? null : reader.GetString(reader.GetOrdinal("movie_title")),
                                reader.IsDBNull(reader.GetOrdinal("title_year")) ? 0 : reader.GetInt32(reader.GetOrdinal("title_year")),
                                reader.IsDBNull(reader.GetOrdinal("imdb_score")) ? 0f : (float)reader.GetDouble(reader.GetOrdinal("imdb_score")),
                                reader.IsDBNull(reader.GetOrdinal("poster_url")) ? @"https://www.movienewz.com/img/films/poster-holder.jpg"
                                : reader.GetString(reader.GetOrdinal("poster_url")),
                                reader.IsDBNull(reader.GetOrdinal("trailer_url")) ? null : reader.GetString(reader.GetOrdinal("trailer_url")),
                                reader.IsDBNull(reader.GetOrdinal("Duration")) ? null : reader.GetString(reader.GetOrdinal("duration")),
                                reader.IsDBNull(reader.GetOrdinal("language")) ? null : reader.GetString(reader.GetOrdinal("language")),
                                reader.IsDBNull(reader.GetOrdinal("country")) ? null : reader.GetString(reader.GetOrdinal("country")),
                                reader.IsDBNull(reader.GetOrdinal("aspect_ratio")) ? 0f : (float)reader.GetDouble(reader.GetOrdinal("aspect_ratio")),
                                reader.IsDBNull(reader.GetOrdinal("Genres")) ? null : reader.GetString(reader.GetOrdinal("Genres")),
                                reader.IsDBNull(reader.GetOrdinal("movie_imdb_link")) ? null : reader.GetString(reader.GetOrdinal("movie_imdb_link")),"",-1,
                                  reader.IsDBNull(reader.GetOrdinal("RatingCode")) || reader.GetString(reader.GetOrdinal("RatingCode")) == "N/A" ? null
                                                                                    : reader.GetString(reader.GetOrdinal("RatingCode"))
                            );
                        }
                    }
                }
            }
            return MDTO;
        }

        public static bool IsMovieExist(string MovieName,int Year, ref int MovieID)
        {
            bool IsExist = false;
            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SP_CheckIfMovieExists", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@MovieName", MovieName);
                    cmd.Parameters.AddWithValue("@Year", Year);
                    var MovieIDParam = new SqlParameter("@MovieID", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.Output
                    };
                    cmd.Parameters.Add(MovieIDParam);
                    var ReturnValue = new SqlParameter("@ReturnValue", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.Output
                    };
                    cmd.Parameters.Add(ReturnValue);

                    connection.Open();
                    cmd.ExecuteNonQuery();
                    IsExist = (int)ReturnValue.Value == 1;
                    if (IsExist)
                    {
                        MovieID = (int)MovieIDParam.Value;
                    }
                    else
                    {
                        MovieID = -1;
                    }

                }
            }
            return IsExist;
        }
        
        public static bool IsMovieExist(int MovieID)
        {
            bool IsExist = false;
            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = "Select Found=1 From vw_MovieBasicInfo Where ID = @MovieID";
                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@MovieID", MovieID);
                    connection.Open();
                    object result = cmd.ExecuteScalar();
                    if (result != null && result != DBNull.Value)
                    {
                        IsExist = true;
                    }
                   
                }
            }
            return IsExist;
        }

        public static List<MovieDTO> GetMoviesStartWithName(string MovieName)
        {
            List<MovieDTO> MDTOs = new List<MovieDTO>();

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = "Select Top(5) * From vw_MovieBasicInfo Where movie_title LIKE @MovieName + '%' Order By imdb_score DESC";

                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@MovieName", MovieName);
                    connection.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            MovieDTO MDTO = null;
                            MDTO = new 
                            (
                                reader.IsDBNull(reader.GetOrdinal("ID")) ? 0 : reader.GetInt32(reader.GetOrdinal("ID")),
                                reader.IsDBNull(reader.GetOrdinal("movie_title")) ? null : reader.GetString(reader.GetOrdinal("movie_title")),
                                reader.IsDBNull(reader.GetOrdinal("title_year")) ? 0 : reader.GetInt32(reader.GetOrdinal("title_year")),
                                reader.IsDBNull(reader.GetOrdinal("imdb_score")) ? 0f : (float)reader.GetDouble(reader.GetOrdinal("imdb_score")),
                                reader.IsDBNull(reader.GetOrdinal("poster_url")) ? @"https://www.movienewz.com/img/films/poster-holder.jpg"
                                : reader.GetString(reader.GetOrdinal("poster_url")),
                                reader.IsDBNull(reader.GetOrdinal("trailer_url")) ? null : reader.GetString(reader.GetOrdinal("trailer_url")),
                                reader.IsDBNull(reader.GetOrdinal("Duration")) ? null : reader.GetString(reader.GetOrdinal("duration")),
                                reader.IsDBNull(reader.GetOrdinal("language")) ? null : reader.GetString(reader.GetOrdinal("language")),
                                reader.IsDBNull(reader.GetOrdinal("country")) ? null : reader.GetString(reader.GetOrdinal("country")),
                                reader.IsDBNull(reader.GetOrdinal("aspect_ratio")) ? 0f : (float)reader.GetDouble(reader.GetOrdinal("aspect_ratio")),
                                reader.IsDBNull(reader.GetOrdinal("Genres")) ? null : reader.GetString(reader.GetOrdinal("Genres")),
                                reader.IsDBNull(reader.GetOrdinal("movie_imdb_link")) ? null : reader.GetString(reader.GetOrdinal("movie_imdb_link")), "", -1,
                                  reader.IsDBNull(reader.GetOrdinal("RatingCode")) || reader.GetString(reader.GetOrdinal("RatingCode")) == "N/A" ? null
                                                                                    : reader.GetString(reader.GetOrdinal("RatingCode"))
                            );

                            MDTOs.Add(MDTO);
                        }
                    }
                }
            }

            return MDTOs;
        }
    
        public static List<MovieDTO> GetMoviesWithNameHasWord(string PieceOfName)
        {
            List<MovieDTO> MDTOs = new List<MovieDTO>();

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = "Select Top(10) * From vw_MovieBasicInfo Where movie_title LIKE '%' + @MovieName + '%' Order By imdb_score DESC";

                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@MovieName", PieceOfName);
                    connection.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            MovieDTO MDTO = null;
                            MDTO = new 
                            (
                                reader.IsDBNull(reader.GetOrdinal("ID")) ? 0 : reader.GetInt32(reader.GetOrdinal("ID")),
                                reader.IsDBNull(reader.GetOrdinal("movie_title")) ? null : reader.GetString(reader.GetOrdinal("movie_title")),
                                reader.IsDBNull(reader.GetOrdinal("title_year")) ? 0 : reader.GetInt32(reader.GetOrdinal("title_year")),
                                reader.IsDBNull(reader.GetOrdinal("imdb_score")) ? 0f : (float)reader.GetDouble(reader.GetOrdinal("imdb_score")),
                                reader.IsDBNull(reader.GetOrdinal("poster_url")) ? @"https://www.movienewz.com/img/films/poster-holder.jpg"
                                                                                    : reader.GetString(reader.GetOrdinal("poster_url")),
                                reader.IsDBNull(reader.GetOrdinal("trailer_url")) ? null : reader.GetString(reader.GetOrdinal("trailer_url")),
                                reader.IsDBNull(reader.GetOrdinal("Duration")) ? null : reader.GetString(reader.GetOrdinal("duration")),
                                reader.IsDBNull(reader.GetOrdinal("language")) ? null : reader.GetString(reader.GetOrdinal("language")),
                                reader.IsDBNull(reader.GetOrdinal("country")) ? null : reader.GetString(reader.GetOrdinal("country")),
                                reader.IsDBNull(reader.GetOrdinal("aspect_ratio")) ? 0f : (float)reader.GetDouble(reader.GetOrdinal("aspect_ratio")),
                                reader.IsDBNull(reader.GetOrdinal("Genres")) ? null : reader.GetString(reader.GetOrdinal("Genres")),
                                reader.IsDBNull(reader.GetOrdinal("movie_imdb_link")) ? null : reader.GetString(reader.GetOrdinal("movie_imdb_link")), "", -1,
                                  reader.IsDBNull(reader.GetOrdinal("RatingCode")) || reader.GetString(reader.GetOrdinal("RatingCode")) == "N/A" ? null
                                                                                    : reader.GetString(reader.GetOrdinal("RatingCode"))

                                );

                            MDTOs.Add(MDTO);
                        }
                    }
                }
            }

            return MDTOs;
        }
    
        public static List<MovieDTO> GetTop100MovieWithGenreAndOrderThemByRatingDESC(string Genre)
        {
            List<MovieDTO> MDTOs = new List<MovieDTO>();

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = "Select Top(100) * From vw_MovieBasicInfo Where Genres Like '%' + @Genre + '%' Order By imdb_score DESC , title_year DESC, movie_title ASC";

                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@Genre", Genre);
                    connection.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            MovieDTO MDTO = null;
                            MDTO = new
                            (
                                reader.IsDBNull(reader.GetOrdinal("ID")) ? 0 : reader.GetInt32(reader.GetOrdinal("ID")),
                                reader.IsDBNull(reader.GetOrdinal("movie_title")) ? null : reader.GetString(reader.GetOrdinal("movie_title")),
                                reader.IsDBNull(reader.GetOrdinal("title_year")) ? 0 : reader.GetInt32(reader.GetOrdinal("title_year")),
                                reader.IsDBNull(reader.GetOrdinal("imdb_score")) ? 0f : (float)reader.GetDouble(reader.GetOrdinal("imdb_score")),
                                reader.IsDBNull(reader.GetOrdinal("poster_url")) ? @"https://www.movienewz.com/img/films/poster-holder.jpg"
                                                                                 : reader.GetString(reader.GetOrdinal("poster_url")),
                                reader.IsDBNull(reader.GetOrdinal("trailer_url")) ? null : reader.GetString(reader.GetOrdinal("trailer_url")),
                                reader.IsDBNull(reader.GetOrdinal("Duration")) ? null : reader.GetString(reader.GetOrdinal("duration")),
                                reader.IsDBNull(reader.GetOrdinal("language")) ? null : reader.GetString(reader.GetOrdinal("language")),
                                reader.IsDBNull(reader.GetOrdinal("country")) ? null : reader.GetString(reader.GetOrdinal("country")),
                                reader.IsDBNull(reader.GetOrdinal("aspect_ratio")) ? 0f : (float)reader.GetDouble(reader.GetOrdinal("aspect_ratio")),
                                reader.IsDBNull(reader.GetOrdinal("Genres")) ? null : reader.GetString(reader.GetOrdinal("Genres")),
                                reader.IsDBNull(reader.GetOrdinal("movie_imdb_link")) ? null : reader.GetString(reader.GetOrdinal("movie_imdb_link")), "", -1,
                                  reader.IsDBNull(reader.GetOrdinal("RatingCode")) || reader.GetString(reader.GetOrdinal("RatingCode")) == "N/A" ? null
                                                                                    : reader.GetString(reader.GetOrdinal("RatingCode"))

                                );

                            MDTOs.Add(MDTO);
                        }
                    }
                }
            }

            return MDTOs;
        }
    
        public static List<MovieDTO> GetTop100MovieWithGenreInYearAndOrderThemByRating(string Genre,int Year)
        {
            List<MovieDTO> MDTOs = new List<MovieDTO>();

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"Select Top(100) * From vw_MovieBasicInfo Where Genres Like '%' + @Genre + '%' 
                                AND title_year = @Year Order By imdb_score DESC , title_year DESC, movie_title ASC";

                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@Genre", Genre);
                    cmd.Parameters.AddWithValue("@Year", Year);

                    connection.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            MovieDTO MDTO = null;
                            MDTO = new
                            (
                                reader.IsDBNull(reader.GetOrdinal("ID")) ? 0 : reader.GetInt32(reader.GetOrdinal("ID")),
                                reader.IsDBNull(reader.GetOrdinal("movie_title")) ? null : reader.GetString(reader.GetOrdinal("movie_title")),
                                reader.IsDBNull(reader.GetOrdinal("title_year")) ? 0 : reader.GetInt32(reader.GetOrdinal("title_year")),
                                reader.IsDBNull(reader.GetOrdinal("imdb_score")) ? 0f : (float)reader.GetDouble(reader.GetOrdinal("imdb_score")),
                                reader.IsDBNull(reader.GetOrdinal("poster_url")) ? @"https://www.movienewz.com/img/films/poster-holder.jpg"
                                                                                 : reader.GetString(reader.GetOrdinal("poster_url")),
                                reader.IsDBNull(reader.GetOrdinal("trailer_url")) ? null : reader.GetString(reader.GetOrdinal("trailer_url")),
                                reader.IsDBNull(reader.GetOrdinal("Duration")) ? null : reader.GetString(reader.GetOrdinal("duration")),
                                reader.IsDBNull(reader.GetOrdinal("language")) ? null : reader.GetString(reader.GetOrdinal("language")),
                                reader.IsDBNull(reader.GetOrdinal("country")) ? null : reader.GetString(reader.GetOrdinal("country")),
                                reader.IsDBNull(reader.GetOrdinal("aspect_ratio")) ? 0f : (float)reader.GetDouble(reader.GetOrdinal("aspect_ratio")),
                                reader.IsDBNull(reader.GetOrdinal("Genres")) ? null : reader.GetString(reader.GetOrdinal("Genres")),
                                reader.IsDBNull(reader.GetOrdinal("movie_imdb_link")) ? null : reader.GetString(reader.GetOrdinal("movie_imdb_link")), "", -1,
                                  reader.IsDBNull(reader.GetOrdinal("RatingCode")) || reader.GetString(reader.GetOrdinal("RatingCode")) == "N/A" ? null
                                                                                    : reader.GetString(reader.GetOrdinal("RatingCode"))

                            );

                            MDTOs.Add(MDTO);
                        }
                    }
                }
            }

            return MDTOs;
        }
    
        public static List<MovieDTO> GetTop10MoviesRecommendedForUserWithID(int UserID)
        {
            List<MovieDTO> MDTOs = new List<MovieDTO>();

            using (SqlConnection con = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SP_GetSuggestMoviesForUser", con))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@UserID", UserID);

                    con.Open();
                    
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            MovieDTO MDTO = null;
                            MDTO = new
                            (
                                reader.IsDBNull(reader.GetOrdinal("ID")) ? 0 : reader.GetInt32(reader.GetOrdinal("ID")),
                                reader.IsDBNull(reader.GetOrdinal("movie_title")) ? null : reader.GetString(reader.GetOrdinal("movie_title")),
                                reader.IsDBNull(reader.GetOrdinal("title_year")) ? 0 : reader.GetInt32(reader.GetOrdinal("title_year")),
                                reader.IsDBNull(reader.GetOrdinal("imdb_score")) ? 0f : (float)reader.GetDouble(reader.GetOrdinal("imdb_score")),
                                reader.IsDBNull(reader.GetOrdinal("poster_url")) ? @"https://www.movienewz.com/img/films/poster-holder.jpg"
                                                                                 : reader.GetString(reader.GetOrdinal("poster_url")),
                                reader.IsDBNull(reader.GetOrdinal("trailer_url")) ? null : reader.GetString(reader.GetOrdinal("trailer_url")),
                                reader.IsDBNull(reader.GetOrdinal("Duration")) ? null : reader.GetString(reader.GetOrdinal("duration")),
                                reader.IsDBNull(reader.GetOrdinal("language")) ? null : reader.GetString(reader.GetOrdinal("language")),
                                reader.IsDBNull(reader.GetOrdinal("country")) ? null : reader.GetString(reader.GetOrdinal("country")),
                                reader.IsDBNull(reader.GetOrdinal("aspect_ratio")) ? 0f : (float)reader.GetDouble(reader.GetOrdinal("aspect_ratio")),
                                reader.IsDBNull(reader.GetOrdinal("Genres")) ? null : reader.GetString(reader.GetOrdinal("Genres")),
                                reader.IsDBNull(reader.GetOrdinal("movie_imdb_link")) ? null : reader.GetString(reader.GetOrdinal("movie_imdb_link")), "", -1,
                                  reader.IsDBNull(reader.GetOrdinal("RatingCode")) || reader.GetString(reader.GetOrdinal("RatingCode")) == "N/A" ? null
                                                                                    : reader.GetString(reader.GetOrdinal("RatingCode"))

                            );

                            MDTOs.Add(MDTO);
                        }
                    }

                    con.Close();
                }

            }
            return MDTOs;
        }
    
        public static List<MovieDTO> GetTop100MoviesBetweenTwoYears(int StartYear, int EndYear, string Genre="Action")
        {
            using(SqlConnection con = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SP_GetTop100MoviePopularInAperiodWithGenre", con))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Start_Year", StartYear);
                    cmd.Parameters.AddWithValue("@End_Year", EndYear);
                    cmd.Parameters.AddWithValue("@Genre", Genre);

                    con.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        List<MovieDTO> MDTOs = new List<MovieDTO>();
                        while (reader.Read())
                        {
                            MovieDTO MDTO = null;
                            MDTO = new
                            (
                                reader.IsDBNull(reader.GetOrdinal("ID")) ? 0 : reader.GetInt32(reader.GetOrdinal("ID")),
                                reader.IsDBNull(reader.GetOrdinal("movie_title")) ? null : reader.GetString(reader.GetOrdinal("movie_title")),
                                reader.IsDBNull(reader.GetOrdinal("title_year")) ? 0 : reader.GetInt32(reader.GetOrdinal("title_year")),
                                reader.IsDBNull(reader.GetOrdinal("imdb_score")) ? 0f : (float)reader.GetDouble(reader.GetOrdinal("imdb_score")),
                                reader.IsDBNull(reader.GetOrdinal("poster_url")) ? @"https://www.movienewz.com/img/films/poster-holder.jpg"
                                                                                 : reader.GetString(reader.GetOrdinal("poster_url")),
                                reader.IsDBNull(reader.GetOrdinal("trailer_url")) ? null : reader.GetString(reader.GetOrdinal("trailer_url")),
                                reader.IsDBNull(reader.GetOrdinal("Duration")) ? null : reader.GetString(reader.GetOrdinal("duration")),
                                reader.IsDBNull(reader.GetOrdinal("language")) ? null : reader.GetString(reader.GetOrdinal("language")),
                                reader.IsDBNull(reader.GetOrdinal("country")) ? null : reader.GetString(reader.GetOrdinal("country")),
                                reader.IsDBNull(reader.GetOrdinal("aspect_ratio")) ? 0f : (float)reader.GetDouble(reader.GetOrdinal("aspect_ratio")),
                                reader.IsDBNull(reader.GetOrdinal("Genres")) ? null : reader.GetString(reader.GetOrdinal("Genres")),
                                reader.IsDBNull(reader.GetOrdinal("movie_imdb_link")) ? null : reader.GetString(reader.GetOrdinal("movie_imdb_link")), "", -1,
                                  reader.IsDBNull(reader.GetOrdinal("RatingCode")) || reader.GetString(reader.GetOrdinal("RatingCode")) == "N/A" ? null
                                                                                    : reader.GetString(reader.GetOrdinal("RatingCode"))
                            );
                            MDTOs.Add(MDTO);
                        }
                        con.Close();

                        return MDTOs;
                    }
                }
            }

        }

        public static List<MovieDTO> GetMoviesWithGenre(string genre)
        {
            List<MovieDTO> movies = new List<MovieDTO>();

            if (string.IsNullOrWhiteSpace(genre))
                return movies;

            try
            {
                using (SqlConnection con = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("SP_GetMoviesWithGenre", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@GenreName", genre.Trim());

                        con.Open();

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                try
                                {
                                    MovieDTO MDTO = null;
                                    MDTO = new
                                    (
                                        reader.IsDBNull(reader.GetOrdinal("ID")) ? 0 : reader.GetInt32(reader.GetOrdinal("ID")),
                                        reader.IsDBNull(reader.GetOrdinal("movie_title")) ? null : reader.GetString(reader.GetOrdinal("movie_title")),
                                        reader.IsDBNull(reader.GetOrdinal("title_year")) ? 0 : reader.GetInt32(reader.GetOrdinal("title_year")),
                                        reader.IsDBNull(reader.GetOrdinal("imdb_score")) ? 0f : (float)reader.GetDouble(reader.GetOrdinal("imdb_score")),
                                        reader.IsDBNull(reader.GetOrdinal("poster_url")) ? @"https://www.movienewz.com/img/films/poster-holder.jpg"
                                                                                             : reader.GetString(reader.GetOrdinal("poster_url")),
                                        reader.IsDBNull(reader.GetOrdinal("trailer_url")) ? null : reader.GetString(reader.GetOrdinal("trailer_url")),
                                        reader.IsDBNull(reader.GetOrdinal("Duration")) ? null : reader.GetString(reader.GetOrdinal("duration")),
                                        reader.IsDBNull(reader.GetOrdinal("language")) ? null : reader.GetString(reader.GetOrdinal("language")),
                                        reader.IsDBNull(reader.GetOrdinal("country")) ? null : reader.GetString(reader.GetOrdinal("country")),
                                        reader.IsDBNull(reader.GetOrdinal("aspect_ratio")) ? 0f : (float)reader.GetDouble(reader.GetOrdinal("aspect_ratio")),
                                        reader.IsDBNull(reader.GetOrdinal("Genres")) ? null : reader.GetString(reader.GetOrdinal("Genres")),
                                        reader.IsDBNull(reader.GetOrdinal("movie_imdb_link")) ? null : reader.GetString(reader.GetOrdinal("movie_imdb_link")),
                                        reader.IsDBNull(reader.GetOrdinal("Keywords")) ? null : reader.GetString(reader.GetOrdinal("Keywords")), -1,
                                              reader.IsDBNull(reader.GetOrdinal("RatingCode")) || reader.GetString(reader.GetOrdinal("RatingCode")) == "N/A" ? null
                                                                                                : reader.GetString(reader.GetOrdinal("RatingCode"))
                                    );
                                    movies.Add(MDTO);
                                }
                                catch (Exception ex)
                                {
                                    // Log specific movie loading error if needed
                                    // Continue with next movie
                                }
                            }
                        }
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                // Log SQL specific error
                throw new ApplicationException("Database error occurred while fetching movies by genre", sqlEx);
            }
            catch (Exception ex)
            {
                // Log general error
                throw new ApplicationException("Error occurred while fetching movies by genre", ex);
            }

            return movies;
        }

        public static List<MovieDTO> GetTop10MoviesWithKeyword(string Keyword)
        {
            using(SqlConnection con = new SqlConnection(clsDataAccessSettings.ConnectionString))
                using(SqlCommand cmd = new SqlCommand("SP_GetTop10MoviesWithKeyWord",con))
            {
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Keyword", Keyword);

                con.Open();

                using(SqlDataReader reader = cmd.ExecuteReader())
                {
                    List<MovieDTO> MDTOs = new List<MovieDTO>();
                    while (reader.Read())
                    {
                        MovieDTO MDTO = null;
                        MDTO = new
                        (
                            reader.IsDBNull(reader.GetOrdinal("ID")) ? 0 : reader.GetInt32(reader.GetOrdinal("ID")),
                            reader.IsDBNull(reader.GetOrdinal("movie_title")) ? null : reader.GetString(reader.GetOrdinal("movie_title")),
                            reader.IsDBNull(reader.GetOrdinal("title_year")) ? 0 : reader.GetInt32(reader.GetOrdinal("title_year")),
                            reader.IsDBNull(reader.GetOrdinal("imdb_score")) ? 0f : (float)reader.GetDouble(reader.GetOrdinal("imdb_score")),
                            reader.IsDBNull(reader.GetOrdinal("poster_url")) ? @"https://www.movienewz.com/img/films/poster-holder.jpg"
                                                                                 : reader.GetString(reader.GetOrdinal("poster_url")),
                            reader.IsDBNull(reader.GetOrdinal("trailer_url")) ? null : reader.GetString(reader.GetOrdinal("trailer_url")),
                            reader.IsDBNull(reader.GetOrdinal("Duration")) ? null : reader.GetString(reader.GetOrdinal("duration")),
                            reader.IsDBNull(reader.GetOrdinal("language")) ? null : reader.GetString(reader.GetOrdinal("language")),
                            reader.IsDBNull(reader.GetOrdinal("country")) ? null : reader.GetString(reader.GetOrdinal("country")),
                            reader.IsDBNull(reader.GetOrdinal("aspect_ratio")) ? 0f : (float)reader.GetDouble(reader.GetOrdinal("aspect_ratio")),
                            reader.IsDBNull(reader.GetOrdinal("Genres")) ? null : reader.GetString(reader.GetOrdinal("Genres")),
                            reader.IsDBNull(reader.GetOrdinal("movie_imdb_link")) ? null : reader.GetString(reader.GetOrdinal("movie_imdb_link")),
                            reader.IsDBNull(reader.GetOrdinal("Keywords")) ? null : reader.GetString(reader.GetOrdinal("Keywords")), -1,
                                  reader.IsDBNull(reader.GetOrdinal("RatingCode")) || reader.GetString(reader.GetOrdinal("RatingCode")) == "N/A" ? null
                                                                                    : reader.GetString(reader.GetOrdinal("RatingCode"))
                        );
                        MDTOs.Add(MDTO);
                    }
                    con.Close();
                    return MDTOs;

                }
            }
        }
        
        public static int AddNewMovie(MovieDTO MDTO)
        {
            int MovieID = -1;
            using (SqlConnection con = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SP_AddNewMovie", con))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@MovieName", MDTO.MovieName);
                    cmd.Parameters.AddWithValue("@Year", MDTO.Year);
                    cmd.Parameters.AddWithValue("@Rate", MDTO.Rate);
                    cmd.Parameters.AddWithValue("@PosterImageURL", MDTO.PosterImageURL);
                    cmd.Parameters.AddWithValue("@TrailerURL", MDTO.TrailerURL);
                    cmd.Parameters.AddWithValue("@Duration", MDTO.DurationInMinutes);
                    cmd.Parameters.AddWithValue("@Language", MDTO.Language);
                    cmd.Parameters.AddWithValue("@Country", MDTO.Country);
                    cmd.Parameters.AddWithValue("@AspectRatio", MDTO.AspectRatio);
                    cmd.Parameters.AddWithValue("@IMDbMovieURL", MDTO.IMDbMovieURL);

                    var MovieIDParam = new SqlParameter("@InsertedMovieID", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.Output
                    };
                    con.Open();

                    MovieID = (int)MovieIDParam.Value;

                }
            }
            return MovieID;
        }

        public static bool UpdateMovieByID(int MovieID,MovieDTO MDTO)
        {
            throw new NotImplementedException();
        }
    
        public static List<MovieDTO> GetTop100MovieBetweenTwoYearsWithGenreAndOrderRating(string RatingOrderValue, int StartYear,
                                                                                         int EndYear, string GenresList, float MinRatingValue, string SortBy="Year")
        {
            List<MovieDTO> MDTOs = new List<MovieDTO>();
            using (SqlConnection con = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SP_GetTop100MovieBetweenTwoYearsWithGenreAndOrderThemByRating", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@OrderValue", RatingOrderValue);
                    cmd.Parameters.AddWithValue("@StartYear", StartYear);
                    cmd.Parameters.AddWithValue("@EndYear", EndYear);
                    cmd.Parameters.AddWithValue("@GenresList", GenresList);
                    cmd.Parameters.AddWithValue("@MinRatingValue", MinRatingValue);
                    cmd.Parameters.AddWithValue("@SortBy", SortBy);
                    con.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            MovieDTO MDTO = null;
                            MDTO = new
                            (
                                reader.IsDBNull(reader.GetOrdinal("ID")) ? 0 : reader.GetInt32(reader.GetOrdinal("ID")),
                                reader.IsDBNull(reader.GetOrdinal("movie_title")) ? null : reader.GetString(reader.GetOrdinal("movie_title")),
                                reader.IsDBNull(reader.GetOrdinal("title_year")) ? 0 : reader.GetInt32(reader.GetOrdinal("title_year")),
                                reader.IsDBNull(reader.GetOrdinal("Rating")) ? 0f : (float)reader.GetDouble(reader.GetOrdinal("Rating")),
                                reader.IsDBNull(reader.GetOrdinal("poster_url")) ? @"https://www.movienewz.com/img/films/poster-holder.jpg"
                                                                                 : reader.GetString(reader.GetOrdinal("poster_url")),
                                reader.IsDBNull(reader.GetOrdinal("trailer_url")) ? null : reader.GetString(reader.GetOrdinal("trailer_url")),
                                reader.IsDBNull(reader.GetOrdinal("Duration")) ? null : reader.GetString(reader.GetOrdinal("duration")),
                                reader.IsDBNull(reader.GetOrdinal("language")) ? null : reader.GetString(reader.GetOrdinal("language")),
                                reader.IsDBNull(reader.GetOrdinal("country")) ? null : reader.GetString(reader.GetOrdinal("country")),
                                reader.IsDBNull(reader.GetOrdinal("aspect_ratio")) ? 0f : (float)reader.GetDouble(reader.GetOrdinal("aspect_ratio")),
                                reader.IsDBNull(reader.GetOrdinal("Genres")) ? null : reader.GetString(reader.GetOrdinal("Genres")),
                                reader.IsDBNull(reader.GetOrdinal("movie_imdb_link")) ? null : reader.GetString(reader.GetOrdinal("movie_imdb_link")), "", -1,
                                  reader.IsDBNull(reader.GetOrdinal("RatingCode")) || reader.GetString(reader.GetOrdinal("RatingCode")) == "N/A" ? null
                                                                                    : reader.GetString(reader.GetOrdinal("RatingCode"))
                            );
                            MDTOs.Add(MDTO);
                        }
                    }
                    con.Close();
                }
            }

            return MDTOs;
        }

        public static bool DeleteMovieByID(int MovieID)
        {
            using (SqlConnection con = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SP_DeleteMovieByID", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@MovieID", MovieID);
                    con.Open();
                    try
                    {
                        object rowsAffected = cmd.ExecuteScalar();
                        return (int)rowsAffected > 0;

                    }
                    catch(Exception ex)
                    {
                        Console.WriteLine("Error ", ex.Message);
                    }
                }
            }

            return false;
        }

        public static List<MovieDTO> GetTop15TrendingMovies()
        {
            List<MovieDTO> MDTOs = new List<MovieDTO>();
            using (SqlConnection con = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SP_GetTop15TrendingMovies", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    con.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            MovieDTO MDTO = null;
                            MDTO = new
                            (
                                reader.IsDBNull(reader.GetOrdinal("ID")) ? 0 : reader.GetInt32(reader.GetOrdinal("ID")),
                                reader.IsDBNull(reader.GetOrdinal("movie_title")) ? null : reader.GetString(reader.GetOrdinal("movie_title")),
                                reader.IsDBNull(reader.GetOrdinal("title_year")) ? 0 : reader.GetInt32(reader.GetOrdinal("title_year")),
                                reader.IsDBNull(reader.GetOrdinal("imdb_score")) ? 0f : (float)reader.GetDouble(reader.GetOrdinal("imdb_score")),
                                reader.IsDBNull(reader.GetOrdinal("poster_url")) ? @"https://www.movienewz.com/img/films/poster-holder.jpg"
                                                                                 : reader.GetString(reader.GetOrdinal("poster_url")),
                                reader.IsDBNull(reader.GetOrdinal("trailer_url")) ? null : reader.GetString(reader.GetOrdinal("trailer_url")),
                                reader.IsDBNull(reader.GetOrdinal("Duration")) ? null : reader.GetString(reader.GetOrdinal("duration")),
                                reader.IsDBNull(reader.GetOrdinal("language")) ? null : reader.GetString(reader.GetOrdinal("language")),
                                reader.IsDBNull(reader.GetOrdinal("country")) ? null : reader.GetString(reader.GetOrdinal("country")),
                                reader.IsDBNull(reader.GetOrdinal("aspect_ratio")) ? 0f : (float)reader.GetDouble(reader.GetOrdinal("aspect_ratio")),
                                reader.IsDBNull(reader.GetOrdinal("Genres")) ? null : reader.GetString(reader.GetOrdinal("Genres")),
                                reader.IsDBNull(reader.GetOrdinal("movie_imdb_link")) ? null : reader.GetString(reader.GetOrdinal("movie_imdb_link")),"",
                                reader.IsDBNull(reader.GetOrdinal("popularity")) ? 0 : reader.GetDouble(reader.GetOrdinal("popularity")),
                                  reader.IsDBNull(reader.GetOrdinal("RatingCode")) || reader.GetString(reader.GetOrdinal("RatingCode")) == "N/A" ? null
                                                                                    : reader.GetString(reader.GetOrdinal("RatingCode"))

                                );
                            MDTOs.Add(MDTO);
                        }
                    }
                    con.Close();
                }
            }
            return MDTOs;
        }
    }

    
}
