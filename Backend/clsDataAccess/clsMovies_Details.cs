
using System;
using System.Data.SqlClient;
using System.Data;
using MovieRecommendations_DataAccess;

namespace MovieRecommendations_DataLayer
{
    public class clsMovies_DetailsData
    {
        #nullable enable

        public static bool GetMovies_DetailsInfoByID(int? ID , ref string? color, ref string? director_name, ref double? num_critic_for_reviews, ref int? duration, ref double? director_facebook_likes, ref double? actor_3_facebook_likes, ref string? actor_2_name, ref double? actor_1_facebook_likes, ref double? gross, ref string? actor_1_name, ref string movie_title, ref double num_voted_users, ref double cast_total_facebook_likes, ref string? actor_3_name, ref double? facenumber_in_poster, ref string movie_imdb_link, ref double? num_user_for_reviews, ref string? language, ref string? country, ref string? content_rating, ref double? budget, ref int? title_year, ref double? actor_2_facebook_likes, ref double? imdb_score, ref double? aspect_ratio, ref int movie_facebook_likes, ref string? poster_url, ref string? trailer_url)
            {
                bool isFound = false;

                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string query = "SELECT * FROM Movies_Details WHERE ID = @ID";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@ID", ID);

                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        { 
                            if (reader.Read())
                            {

                                // The record was found
                                isFound = true;

                                color = reader["color"] != DBNull.Value ? reader["color"].ToString() : null;
                                director_name = reader["director_name"] != DBNull.Value ? reader["director_name"].ToString() : null;
                                num_critic_for_reviews = reader["num_critic_for_reviews"] != DBNull.Value ? (double?)reader["num_critic_for_reviews"] : null;
                                duration = reader["duration"] != DBNull.Value ? (int?)reader["duration"] : null;
                                director_facebook_likes = reader["director_facebook_likes"] != DBNull.Value ? (double?)reader["director_facebook_likes"] : null;
                                actor_3_facebook_likes = reader["actor_3_facebook_likes"] != DBNull.Value ? (double?)reader["actor_3_facebook_likes"] : null;
                                actor_2_name = reader["actor_2_name"] != DBNull.Value ? reader["actor_2_name"].ToString() : null;
                                actor_1_facebook_likes = reader["actor_1_facebook_likes"] != DBNull.Value ? (double?)reader["actor_1_facebook_likes"] : null;
                                gross = reader["gross"] != DBNull.Value ? (double?)reader["gross"] : null;
                                actor_1_name = reader["actor_1_name"] != DBNull.Value ? reader["actor_1_name"].ToString() : null;
                                movie_title = (string)reader["movie_title"];
                                num_voted_users = (double)reader["num_voted_users"];
                                cast_total_facebook_likes = (double)reader["cast_total_facebook_likes"];
                                actor_3_name = reader["actor_3_name"] != DBNull.Value ? reader["actor_3_name"].ToString() : null;
                                facenumber_in_poster = reader["facenumber_in_poster"] != DBNull.Value ? (double?)reader["facenumber_in_poster"] : null;
                                movie_imdb_link = (string)reader["movie_imdb_link"];
                                num_user_for_reviews = reader["num_user_for_reviews"] != DBNull.Value ? (double?)reader["num_user_for_reviews"] : null;
                                language = reader["language"] != DBNull.Value ? reader["language"].ToString() : null;
                                country = reader["country"] != DBNull.Value ? reader["country"].ToString() : null;
                                content_rating = reader["content_rating"] != DBNull.Value ? reader["content_rating"].ToString() : null;
                                budget = reader["budget"] != DBNull.Value ? (double?)reader["budget"] : null;
                                title_year = reader["title_year"] != DBNull.Value ? (int?)reader["title_year"] : null;
                                actor_2_facebook_likes = reader["actor_2_facebook_likes"] != DBNull.Value ? (double?)reader["actor_2_facebook_likes"] : null;
                                imdb_score = reader["imdb_score"] != DBNull.Value ? (double?)reader["imdb_score"] : null;
                                aspect_ratio = reader["aspect_ratio"] != DBNull.Value ? (double?)reader["aspect_ratio"] : null;
                                movie_facebook_likes = (int)reader["movie_facebook_likes"];
                                poster_url = reader["poster_url"] != DBNull.Value ? reader["poster_url"].ToString() : null;
                                trailer_url = reader["trailer_url"] != DBNull.Value ? reader["trailer_url"].ToString() : null;



                            }
                        }

                    }
                }
                return isFound;

            }

        public static DataTable GetAllMovies_Details()
{
    DataTable dt = new DataTable();

    using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
    {
        string query = "SELECT * FROM Movies_Details";

        using (SqlCommand command = new SqlCommand(query, connection))
        {

            connection.Open();

            using (SqlDataReader reader = command.ExecuteReader())
            {
                if (reader.HasRows)
                    dt.Load(reader);
            }
        }
    }
    return dt;

}

         public static int? AddNewMovies_Details(string? color, string? director_name, double? num_critic_for_reviews, int? duration, double? director_facebook_likes, double? actor_3_facebook_likes, string? actor_2_name, double? actor_1_facebook_likes, double? gross, string? actor_1_name, string movie_title, double num_voted_users, double cast_total_facebook_likes, string? actor_3_name, double? facenumber_in_poster, string movie_imdb_link, double? num_user_for_reviews, string? language, string? country, string? content_rating, double? budget, int? title_year, double? actor_2_facebook_likes, double? imdb_score, double? aspect_ratio, int movie_facebook_likes, string? poster_url, string? trailer_url)
        {
            int? ID = null;

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"Insert Into Movies_Details ([color],[director_name],[num_critic_for_reviews],[duration],[director_facebook_likes],[actor_3_facebook_likes],[actor_2_name],[actor_1_facebook_likes],[gross],[actor_1_name],[movie_title],[num_voted_users],[cast_total_facebook_likes],[actor_3_name],[facenumber_in_poster],[movie_imdb_link],[num_user_for_reviews],[language],[country],[content_rating],[budget],[title_year],[actor_2_facebook_likes],[imdb_score],[aspect_ratio],[movie_facebook_likes],[poster_url],[trailer_url])
                            Values (@color,@director_name,@num_critic_for_reviews,@duration,@director_facebook_likes,@actor_3_facebook_likes,@actor_2_name,@actor_1_facebook_likes,@gross,@actor_1_name,@movie_title,@num_voted_users,@cast_total_facebook_likes,@actor_3_name,@facenumber_in_poster,@movie_imdb_link,@num_user_for_reviews,@language,@country,@content_rating,@budget,@title_year,@actor_2_facebook_likes,@imdb_score,@aspect_ratio,@movie_facebook_likes,@poster_url,@trailer_url)
                            SELECT SCOPE_IDENTITY();";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@color", color ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@director_name", director_name ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@num_critic_for_reviews", num_critic_for_reviews ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@duration", duration ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@director_facebook_likes", director_facebook_likes ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@actor_3_facebook_likes", actor_3_facebook_likes ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@actor_2_name", actor_2_name ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@actor_1_facebook_likes", actor_1_facebook_likes ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@gross", gross ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@actor_1_name", actor_1_name ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@movie_title", movie_title);
                    command.Parameters.AddWithValue("@num_voted_users", num_voted_users);
                    command.Parameters.AddWithValue("@cast_total_facebook_likes", cast_total_facebook_likes);
                    command.Parameters.AddWithValue("@actor_3_name", actor_3_name ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@facenumber_in_poster", facenumber_in_poster ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@movie_imdb_link", movie_imdb_link);
                    command.Parameters.AddWithValue("@num_user_for_reviews", num_user_for_reviews ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@language", language ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@country", country ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@content_rating", content_rating ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@budget", budget ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@title_year", title_year ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@actor_2_facebook_likes", actor_2_facebook_likes ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@imdb_score", imdb_score ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@aspect_ratio", aspect_ratio ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@movie_facebook_likes", movie_facebook_likes);
                    command.Parameters.AddWithValue("@poster_url", poster_url ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@trailer_url", trailer_url ?? (object)DBNull.Value);


                    connection.Open();

                    object result = command.ExecuteScalar();

                    if (result != null && int.TryParse(result.ToString(), out int insertedID))
                    {
                        ID = insertedID;
                    }
                }

            }
            return ID;

        }


         public static bool UpdateMovies_DetailsByID(int? ID, string? color, string? director_name, double? num_critic_for_reviews, int? duration, double? director_facebook_likes, double? actor_3_facebook_likes, string? actor_2_name, double? actor_1_facebook_likes, double? gross, string? actor_1_name, string movie_title, double num_voted_users, double cast_total_facebook_likes, string? actor_3_name, double? facenumber_in_poster, string movie_imdb_link, double? num_user_for_reviews, string? language, string? country, string? content_rating, double? budget, int? title_year, double? actor_2_facebook_likes, double? imdb_score, double? aspect_ratio, int movie_facebook_likes, string? poster_url, string? trailer_url)
        {
            int rowsAffected = 0;

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"Update Movies_Details
                                    set 
                                         [color] = @color,
                                         [director_name] = @director_name,
                                         [num_critic_for_reviews] = @num_critic_for_reviews,
                                         [duration] = @duration,
                                         [director_facebook_likes] = @director_facebook_likes,
                                         [actor_3_facebook_likes] = @actor_3_facebook_likes,
                                         [actor_2_name] = @actor_2_name,
                                         [actor_1_facebook_likes] = @actor_1_facebook_likes,
                                         [gross] = @gross,
                                         [actor_1_name] = @actor_1_name,
                                         [movie_title] = @movie_title,
                                         [num_voted_users] = @num_voted_users,
                                         [cast_total_facebook_likes] = @cast_total_facebook_likes,
                                         [actor_3_name] = @actor_3_name,
                                         [facenumber_in_poster] = @facenumber_in_poster,
                                         [movie_imdb_link] = @movie_imdb_link,
                                         [num_user_for_reviews] = @num_user_for_reviews,
                                         [language] = @language,
                                         [country] = @country,
                                         [content_rating] = @content_rating,
                                         [budget] = @budget,
                                         [title_year] = @title_year,
                                         [actor_2_facebook_likes] = @actor_2_facebook_likes,
                                         [imdb_score] = @imdb_score,
                                         [aspect_ratio] = @aspect_ratio,
                                         [movie_facebook_likes] = @movie_facebook_likes,
                                         [poster_url] = @poster_url,
                                         [trailer_url] = @trailer_url
                                  where [ID]= @ID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ID", ID);
                    command.Parameters.AddWithValue("@color", color ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@director_name", director_name ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@num_critic_for_reviews", num_critic_for_reviews ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@duration", duration ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@director_facebook_likes", director_facebook_likes ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@actor_3_facebook_likes", actor_3_facebook_likes ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@actor_2_name", actor_2_name ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@actor_1_facebook_likes", actor_1_facebook_likes ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@gross", gross ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@actor_1_name", actor_1_name ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@movie_title", movie_title);
                    command.Parameters.AddWithValue("@num_voted_users", num_voted_users);
                    command.Parameters.AddWithValue("@cast_total_facebook_likes", cast_total_facebook_likes);
                    command.Parameters.AddWithValue("@actor_3_name", actor_3_name ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@facenumber_in_poster", facenumber_in_poster ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@movie_imdb_link", movie_imdb_link);
                    command.Parameters.AddWithValue("@num_user_for_reviews", num_user_for_reviews ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@language", language ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@country", country ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@content_rating", content_rating ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@budget", budget ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@title_year", title_year ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@actor_2_facebook_likes", actor_2_facebook_likes ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@imdb_score", imdb_score ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@aspect_ratio", aspect_ratio ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@movie_facebook_likes", movie_facebook_likes);
                    command.Parameters.AddWithValue("@poster_url", poster_url ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@trailer_url", trailer_url ?? (object)DBNull.Value);


                    connection.Open();

                    rowsAffected = command.ExecuteNonQuery();
                }

            }

            return (rowsAffected > 0);
        }


        public static bool DeleteMovies_Details(int ID)
{
    int rowsAffected = 0;

    using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
    {
        string query = @"Delete Movies_Details 
                        where ID = @ID";

        using (SqlCommand command = new SqlCommand(query, connection))
        {
            command.Parameters.AddWithValue("@ID", ID);


            connection.Open();
            
            rowsAffected = command.ExecuteNonQuery();


        }

    }
    
    return (rowsAffected > 0);

}
        
        public static DataTable SearchData(string ColumnName, string Data)
{
    DataTable dt = new DataTable();

    using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
    {
        string query = $@"select * from Movies_Details
                    where {ColumnName} Like '' + @Data + '%';";

        using (SqlCommand Command = new SqlCommand(query, connection))
        {
            Command.Parameters.AddWithValue("@Data", Data);


            connection.Open();

            using (SqlDataReader reader = Command.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    dt.Load(reader);
                }

                reader.Close();
            }
        }
        
    }

    return dt;
}
    }
}
