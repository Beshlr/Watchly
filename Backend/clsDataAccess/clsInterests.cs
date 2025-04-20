
using System;
using Microsoft.Data.SqlClient;
using System.Data;
using MovieRecommendations_DataAccess;

namespace MovieRecommendations_DataLayer
{
    public class clsInterestsData
    {
        #nullable enable

        public static bool GetInterestsInfoByID(int? InterestsID , ref int UserID, ref int GenresID, ref double? InterestLevel, ref DateTime? CreatedAt, ref DateTime? UpdatedAt, ref string? Source, ref bool? IsActive)
            {
                bool isFound = false;

                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string query = "SELECT * FROM Interests WHERE InterestsID = @InterestsID";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@InterestsID", InterestsID);

                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        { 
                            if (reader.Read())
                            {

                                // The record was found
                                isFound = true;

                                UserID = (int)reader["UserID"];
                                GenresID = (int)reader["GenresID"];
                                InterestLevel = reader["InterestLevel"] != DBNull.Value ? (double?)reader["InterestLevel"] : null;
                                CreatedAt = reader["CreatedAt"] != DBNull.Value ? (DateTime?)reader["CreatedAt"] : null;
                                UpdatedAt = reader["UpdatedAt"] != DBNull.Value ? (DateTime?)reader["UpdatedAt"] : null;
                                Source = reader["Source"] != DBNull.Value ? reader["Source"].ToString() : null;
                                IsActive = reader["IsActive"] != DBNull.Value ? (bool?)reader["IsActive"] : null;



                            }
                        }

                    }
                }
                return isFound;

            }

        public static DataTable GetAllInterests()
{
    DataTable dt = new DataTable();

    using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
    {
        string query = "SELECT * FROM Interests";

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

         public static int? AddNewInterests(int UserID, int GenresID, double? InterestLevel, DateTime? CreatedAt, DateTime? UpdatedAt, string? Source, bool? IsActive)
        {
            int? InterestsID = null;

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"Insert Into Interests ([UserID],[GenresID],[InterestLevel],[CreatedAt],[UpdatedAt],[Source],[IsActive])
                            Values (@UserID,@GenresID,@InterestLevel,@CreatedAt,@UpdatedAt,@Source,@IsActive)
                            SELECT SCOPE_IDENTITY();";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserID", UserID);
                    command.Parameters.AddWithValue("@GenresID", GenresID);
                    command.Parameters.AddWithValue("@InterestLevel", InterestLevel ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@CreatedAt", CreatedAt ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@UpdatedAt", UpdatedAt ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Source", Source ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@IsActive", IsActive ?? (object)DBNull.Value);


                    connection.Open();

                    object result = command.ExecuteScalar();

                    if (result != null && int.TryParse(result.ToString(), out int insertedID))
                    {
                        InterestsID = insertedID;
                    }
                }

            }
            return InterestsID;

        }


         public static bool UpdateInterestsByID(int? InterestsID, int UserID, int GenresID, double? InterestLevel, DateTime? CreatedAt, DateTime? UpdatedAt, string? Source, bool? IsActive)
        {
            int rowsAffected = 0;

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"Update Interests
                                    set 
                                         [UserID] = @UserID,
                                         [GenresID] = @GenresID,
                                         [InterestLevel] = @InterestLevel,
                                         [CreatedAt] = @CreatedAt,
                                         [UpdatedAt] = @UpdatedAt,
                                         [Source] = @Source,
                                         [IsActive] = @IsActive
                                  where [InterestsID]= @InterestsID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@InterestsID", InterestsID);
                    command.Parameters.AddWithValue("@UserID", UserID);
                    command.Parameters.AddWithValue("@GenresID", GenresID);
                    command.Parameters.AddWithValue("@InterestLevel", InterestLevel ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@CreatedAt", CreatedAt ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@UpdatedAt", UpdatedAt ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Source", Source ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@IsActive", IsActive ?? (object)DBNull.Value);


                    connection.Open();

                    rowsAffected = command.ExecuteNonQuery();
                }

            }

            return (rowsAffected > 0);
        }


        public static bool DeleteInterests(int InterestsID)
{
    int rowsAffected = 0;

    using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
    {
        string query = @"Delete Interests 
                        where InterestsID = @InterestsID";

        using (SqlCommand command = new SqlCommand(query, connection))
        {
            command.Parameters.AddWithValue("@InterestsID", InterestsID);


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
        string query = $@"select * from Interests
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
