
using System;
using System.Data.SqlClient;
using System.Data;
using MovieRecommendations_DataAccess;

namespace MovieRecommendations_DataLayer
{
    public class clsUsersData
    {
        #nullable enable

        public static bool GetUsersInfoByID(int? UserID , ref string Username, ref string Password, ref bool? IsAcive, ref byte Permissions, ref byte Age)
            {
                bool isFound = false;

                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string query = "SELECT * FROM Users WHERE UserID = @UserID";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@UserID", UserID);

                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        { 
                            if (reader.Read())
                            {

                                // The record was found
                                isFound = true;

                                Username = (string)reader["Username"];
                                Password = (string)reader["Password"];
                                IsAcive = reader["IsAcive"] != DBNull.Value ? (bool?)reader["IsAcive"] : null;
                                Permissions = (byte)reader["Permissions"];
                                Age = (byte)reader["Age"];



                            }
                        }

                    }
                }
                return isFound;

            }

        public static DataTable GetAllUsers()
{
    DataTable dt = new DataTable();

    using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
    {
        string query = "SELECT * FROM Users";

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

         public static int? AddNewUsers(string Username, string Password, bool? IsAcive, byte Permissions, byte Age)
        {
            int? UserID = null;

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"Insert Into Users ([Username],[Password],[IsAcive],[Permissions],[Age])
                            Values (@Username,@Password,@IsAcive,@Permissions,@Age)
                            SELECT SCOPE_IDENTITY();";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Username", Username);
                    command.Parameters.AddWithValue("@Password", Password);
                    command.Parameters.AddWithValue("@IsAcive", IsAcive ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Permissions", Permissions);
                    command.Parameters.AddWithValue("@Age", Age);


                    connection.Open();

                    object result = command.ExecuteScalar();

                    if (result != null && int.TryParse(result.ToString(), out int insertedID))
                    {
                        UserID = insertedID;
                    }
                }

            }
            return UserID;

        }


         public static bool UpdateUsersByID(int? UserID, string Username, string Password, bool? IsAcive, byte Permissions, byte Age)
        {
            int rowsAffected = 0;

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"Update Users
                                    set 
                                         [Username] = @Username,
                                         [Password] = @Password,
                                         [IsAcive] = @IsAcive,
                                         [Permissions] = @Permissions,
                                         [Age] = @Age
                                  where [UserID]= @UserID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserID", UserID);
                    command.Parameters.AddWithValue("@Username", Username);
                    command.Parameters.AddWithValue("@Password", Password);
                    command.Parameters.AddWithValue("@IsAcive", IsAcive ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Permissions", Permissions);
                    command.Parameters.AddWithValue("@Age", Age);


                    connection.Open();

                    rowsAffected = command.ExecuteNonQuery();
                }

            }

            return (rowsAffected > 0);
        }


        public static bool DeleteUsers(int UserID)
{
    int rowsAffected = 0;

    using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
    {
        string query = @"Delete Users 
                        where UserID = @UserID";

        using (SqlCommand command = new SqlCommand(query, connection))
        {
            command.Parameters.AddWithValue("@UserID", UserID);


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
        string query = $@"select * from Users
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
