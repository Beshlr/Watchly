
using System;
using Microsoft.Data.SqlClient;
using System.Data;
using MovieRecommendations_DataAccess;

namespace MovieRecommendations_DataLayer
{
    public class clsmovie_genresData
    {
        #nullable enable

        public static bool Getmovie_genresInfoByID(int? ID , ref string movie_title, ref int genre_id, ref int movie_id)
            {
                bool isFound = false;

                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string query = "SELECT * FROM movie_genres WHERE ID = @ID";

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

                                movie_title = (string)reader["movie_title"];
                                genre_id = (int)reader["genre_id"];
                                movie_id = (int)reader["movie_id"];



                            }
                        }

                    }
                }
                return isFound;

            }

        public static DataTable GetAllmovie_genres()
{
    DataTable dt = new DataTable();

    using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
    {
        string query = "SELECT * FROM movie_genres";

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

         public static int? AddNewmovie_genres(string movie_title, int genre_id, int movie_id)
        {
            int? ID = null;

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"Insert Into movie_genres ([movie_title],[genre_id],[movie_id])
                            Values (@movie_title,@genre_id,@movie_id)
                            SELECT SCOPE_IDENTITY();";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@movie_title", movie_title);
                    command.Parameters.AddWithValue("@genre_id", genre_id);
                    command.Parameters.AddWithValue("@movie_id", movie_id);


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


         public static bool Updatemovie_genresByID(int? ID, string movie_title, int genre_id, int movie_id)
        {
            int rowsAffected = 0;

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"Update movie_genres
                                    set 
                                         [movie_title] = @movie_title,
                                         [genre_id] = @genre_id,
                                         [movie_id] = @movie_id
                                  where [ID]= @ID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ID", ID);
                    command.Parameters.AddWithValue("@movie_title", movie_title);
                    command.Parameters.AddWithValue("@genre_id", genre_id);
                    command.Parameters.AddWithValue("@movie_id", movie_id);


                    connection.Open();

                    rowsAffected = command.ExecuteNonQuery();
                }

            }

            return (rowsAffected > 0);
        }


        public static bool Deletemovie_genres(int ID)
{
    int rowsAffected = 0;

    using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
    {
        string query = @"Delete movie_genres 
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
        string query = $@"select * from movie_genres
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
