using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Data.SqlTypes;
using Microsoft.Data.SqlClient;
using MovieRecommendations_DataAccess;

namespace clsDataAccess
{
    public class MovieDTO
    {
        #nullable disable

        public MovieDTO(int iD, string movieName, int year, float rate, string posterImageURL,
            string trailerURL, string contentRating, string duration, string language,
            string country, float aspectRatio, string genre)
        {
            ID = iD;
            MovieName = movieName;
            Year = year;
            Rate = rate;
            PosterImageURL = posterImageURL;
            TrailerURL = trailerURL;
            ContentRating = contentRating;
            Duration = duration;
            Language = language;
            Country = country;
            AspectRatio = aspectRatio;
            Genre = genre;
        }

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

    }

    public class clsMoviePasicDetailsData
    {
        public static MovieDTO GetMovieByID(int ID)
        {
            MovieDTO MDTO = null;

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = "Select * From vw_MovieBasicInfo Where ID = @ID";

                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@ID", ID);
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
                                reader.IsDBNull(reader.GetOrdinal("content_rating")) ? null : reader.GetString(reader.GetOrdinal("content_rating")),
                                reader.IsDBNull(reader.GetOrdinal("Duration")) ? null : reader.GetString(reader.GetOrdinal("duration")),
                                reader.IsDBNull(reader.GetOrdinal("language")) ? null : reader.GetString(reader.GetOrdinal("language")),
                                reader.IsDBNull(reader.GetOrdinal("country")) ? null : reader.GetString(reader.GetOrdinal("country")),
                                reader.IsDBNull(reader.GetOrdinal("aspect_ratio")) ? 0f : (float)reader.GetDouble(reader.GetOrdinal("aspect_ratio")),
                                reader.IsDBNull(reader.GetOrdinal("Genres")) ? null : reader.GetString(reader.GetOrdinal("Genres"))
                            );
                        }
                    }
                }
            }

            return MDTO;
        }

        public static List<MovieDTO> GetMoviesStartWithName(string MovieName)
        {
            List<MovieDTO> MDTOs = new List<MovieDTO>();

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = "Select Top(10) * From vw_MovieBasicInfo Where movie_title LIKE @MovieName + '%' Order By imdb_score DESC";

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
                                reader.IsDBNull(reader.GetOrdinal("content_rating")) ? null : reader.GetString(reader.GetOrdinal("content_rating")),
                                reader.IsDBNull(reader.GetOrdinal("Duration")) ? null : reader.GetString(reader.GetOrdinal("duration")),
                                reader.IsDBNull(reader.GetOrdinal("language")) ? null : reader.GetString(reader.GetOrdinal("language")),
                                reader.IsDBNull(reader.GetOrdinal("country")) ? null : reader.GetString(reader.GetOrdinal("country")),
                                reader.IsDBNull(reader.GetOrdinal("aspect_ratio")) ? 0f : (float)reader.GetDouble(reader.GetOrdinal("aspect_ratio")),
                                reader.IsDBNull(reader.GetOrdinal("Genres")) ? null : reader.GetString(reader.GetOrdinal("Genres"))
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
                                reader.IsDBNull(reader.GetOrdinal("content_rating")) ? null : reader.GetString(reader.GetOrdinal("content_rating")),
                                reader.IsDBNull(reader.GetOrdinal("Duration")) ? null : reader.GetString(reader.GetOrdinal("duration")),
                                reader.IsDBNull(reader.GetOrdinal("language")) ? null : reader.GetString(reader.GetOrdinal("language")),
                                reader.IsDBNull(reader.GetOrdinal("country")) ? null : reader.GetString(reader.GetOrdinal("country")),
                                reader.IsDBNull(reader.GetOrdinal("aspect_ratio")) ? 0f : (float)reader.GetDouble(reader.GetOrdinal("aspect_ratio")),
                                reader.IsDBNull(reader.GetOrdinal("Genres")) ? null : reader.GetString(reader.GetOrdinal("Genres"))
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
                                reader.IsDBNull(reader.GetOrdinal("content_rating")) ? null : reader.GetString(reader.GetOrdinal("content_rating")),
                                reader.IsDBNull(reader.GetOrdinal("Duration")) ? null : reader.GetString(reader.GetOrdinal("duration")),
                                reader.IsDBNull(reader.GetOrdinal("language")) ? null : reader.GetString(reader.GetOrdinal("language")),
                                reader.IsDBNull(reader.GetOrdinal("country")) ? null : reader.GetString(reader.GetOrdinal("country")),
                                reader.IsDBNull(reader.GetOrdinal("aspect_ratio")) ? 0f : (float)reader.GetDouble(reader.GetOrdinal("aspect_ratio")),
                                reader.IsDBNull(reader.GetOrdinal("Genres")) ? null : reader.GetString(reader.GetOrdinal("Genres"))
                            );

                            MDTOs.Add(MDTO);
                        }
                    }
                }
            }

            return MDTOs;
        }
    
        public static List<MovieDTO> GetTop100MovieWithGenreInYearAndOrderThemByRatingDESC(string Genre,int Year)
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
                                reader.IsDBNull(reader.GetOrdinal("content_rating")) ? null : reader.GetString(reader.GetOrdinal("content_rating")),
                                reader.IsDBNull(reader.GetOrdinal("Duration")) ? null : reader.GetString(reader.GetOrdinal("duration")),
                                reader.IsDBNull(reader.GetOrdinal("language")) ? null : reader.GetString(reader.GetOrdinal("language")),
                                reader.IsDBNull(reader.GetOrdinal("country")) ? null : reader.GetString(reader.GetOrdinal("country")),
                                reader.IsDBNull(reader.GetOrdinal("aspect_ratio")) ? 0f : (float)reader.GetDouble(reader.GetOrdinal("aspect_ratio")),
                                reader.IsDBNull(reader.GetOrdinal("Genres")) ? null : reader.GetString(reader.GetOrdinal("Genres"))
                            );

                            MDTOs.Add(MDTO);
                        }
                    }
                }
            }

            return MDTOs;
        }
    
        
    }
}
