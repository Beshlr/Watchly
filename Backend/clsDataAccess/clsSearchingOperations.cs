
using System;
using Microsoft.Data.SqlClient;
using System.Data;
using MovieRecommendations_DataAccess;

namespace MovieRecommendations_DataLayer
{
    public class clsSearchingOperationsData
    {
        #nullable enable

        public static bool GetSearchingOperationsInfoByID(int? SearchingID , ref int UserID, ref int MovieID)
            {
                bool isFound = false;

                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string query = "SELECT * FROM SearchingOperations WHERE SearchingID = @SearchingID";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@SearchingID", SearchingID);

                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        { 
                            if (reader.Read())
                            {

                                // The record was found
                                isFound = true;

                                UserID = (int)reader["UserID"];
                                MovieID = (int)reader["MovieID"];



                            }
                        }

                    }
                }
                return isFound;

            }

        public static DataTable GetAllSearchingOperations()
{
    DataTable dt = new DataTable();

    using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
    {
        string query = "SELECT * FROM SearchingOperations";

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

         public static int? AddNewSearchingOperations(int UserID, int MovieID)
        {
            int? SearchingID = null;

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"Insert Into SearchingOperations ([UserID],[MovieID])
                            Values (@UserID,@MovieID)
                            SELECT SCOPE_IDENTITY();";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserID", UserID);
                    command.Parameters.AddWithValue("@MovieID", MovieID);


                    connection.Open();

                    object result = command.ExecuteScalar();

                    if (result != null && int.TryParse(result.ToString(), out int insertedID))
                    {
                        SearchingID = insertedID;
                    }
                }

            }
            return SearchingID;

        }


         public static bool UpdateSearchingOperationsByID(int? SearchingID, int UserID, int MovieID)
        {
            int rowsAffected = 0;

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"Update SearchingOperations
                                    set 
                                         [UserID] = @UserID,
                                         [MovieID] = @MovieID
                                  where [SearchingID]= @SearchingID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@SearchingID", SearchingID);
                    command.Parameters.AddWithValue("@UserID", UserID);
                    command.Parameters.AddWithValue("@MovieID", MovieID);


                    connection.Open();

                    rowsAffected = command.ExecuteNonQuery();
                }

            }

            return (rowsAffected > 0);
        }


        public static bool DeleteSearchingOperations(int SearchingID)
{
    int rowsAffected = 0;

    using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
    {
        string query = @"Delete SearchingOperations 
                        where SearchingID = @SearchingID";

        using (SqlCommand command = new SqlCommand(query, connection))
        {
            command.Parameters.AddWithValue("@SearchingID", SearchingID);


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
        string query = $@"select * from SearchingOperations
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
