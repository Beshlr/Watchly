
using System;
using Microsoft.Data.SqlClient;
using System.Data;
using MovieRecommendations_DataAccess;

namespace MovieRecommendations_DataLayer
{
    public class clsKeywordsData
    {
        #nullable enable

        public static bool GetKeywordsInfoByID(int? KeywordID , ref string Keyword, ref int MovieID)
            {
                bool isFound = false;

                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string query = "SELECT * FROM Keywords WHERE KeywordID = @KeywordID";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@KeywordID", KeywordID);

                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        { 
                            if (reader.Read())
                            {

                                // The record was found
                                isFound = true;

                                Keyword = (string)reader["Keyword"];
                                MovieID = (int)reader["MovieID"];

                            }
                        }

                    }
                }
                return isFound;

            }

        public static DataTable GetAllKeywords()
{
    DataTable dt = new DataTable();

    using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
    {
        string query = "SELECT * FROM Keywords";

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

         public static int? AddNewKeywords(string Keyword, int MovieID)
        {
            int? KeywordID = null;

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"Insert Into Keywords ([Keyword],[MovieID])
                            Values (@Keyword,@MovieID)
                            SELECT SCOPE_IDENTITY();";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Keyword", Keyword);
                    command.Parameters.AddWithValue("@MovieID", MovieID);


                    connection.Open();

                    object result = command.ExecuteScalar();

                    if (result != null && int.TryParse(result.ToString(), out int insertedID))
                    {
                        KeywordID = insertedID;
                    }
                }

            }
            return KeywordID;

        }


         public static bool UpdateKeywordsByID(int? KeywordID, string Keyword, int MovieID)
        {
            int rowsAffected = 0;

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"Update Keywords
                                    set 
                                         [Keyword] = @Keyword,
                                         [MovieID] = @MovieID
                                  where [KeywordID]= @KeywordID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@KeywordID", KeywordID);
                    command.Parameters.AddWithValue("@Keyword", Keyword);
                    command.Parameters.AddWithValue("@MovieID", MovieID);


                    connection.Open();

                    rowsAffected = command.ExecuteNonQuery();
                }

            }

            return (rowsAffected > 0);
        }


        public static bool DeleteKeywords(int KeywordID)
{
    int rowsAffected = 0;

    using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
    {
        string query = @"Delete Keywords 
                        where KeywordID = @KeywordID";

        using (SqlCommand command = new SqlCommand(query, connection))
        {
            command.Parameters.AddWithValue("@KeywordID", KeywordID);


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
        string query = $@"select * from Keywords
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
