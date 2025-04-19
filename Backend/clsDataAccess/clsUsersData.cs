
using System;
using Microsoft.Data.SqlClient;
using System.Data;
using MovieRecommendations_DataAccess;
using System.Security.Cryptography;
namespace MovieRecommendations_DataLayer
{
#nullable enable
    public class UserDTO
    {

        public UserDTO(int ID, string Username, string Password, bool IsAcive, byte Permissions, byte Age)
        {
            this.ID = ID;
            this.Username = Username;
            this.Password = Password;
            this.IsAcive = IsAcive;
            this.Permissions = Permissions;
            this.Age = Age;
        }

        public int ID { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public bool IsAcive { get; set; }
        public byte Permissions { get; set; }
        public byte Age { get; set; }
    }

    public class clsUsersData
    {

        public static UserDTO GetUsersInfoByID(int UserID)
            {
            UserDTO userDTO = null;
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
                            userDTO = new UserDTO(
                                UserID,
                             (string)reader["Username"],
                             (string)reader["Password"],
                             reader["IsAcive"] != DBNull.Value ? (bool)reader["IsAcive"] : false,
                             (byte)reader["Permissions"],
                            (byte)reader["Age"]);

                            }
                        }

                    }
                }
            return userDTO;
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

         public static int AddNewUsers(string Username, string Password, bool IsAcive, byte Permissions, byte Age)
        {
            int UserID = -1;

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"Insert Into Users ([Username],[Password],[IsAcive],[Permissions],[Age])
                            Values (@Username,@Password,@IsAcive,@Permissions,@Age)
                            SELECT SCOPE_IDENTITY();";

                using (SqlCommand command = new SqlCommand(query, connection))
                {

                    command.Parameters.AddWithValue("@Username", Username);
                    command.Parameters.AddWithValue("@Password", Password);
                    if(IsAcive == null)
                        command.Parameters.AddWithValue("@IsAcive", DBNull.Value);
                    else
                        command.Parameters.AddWithValue("@IsAcive", IsAcive  );
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

        public static bool CheckIsUsernameExist(string Username)
        {
            bool IsFound = false;
            using (SqlConnection con = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"Select Found=1 From Users Where Username COLLATE SQL_Latin1_General_CP1_CS_AS = @Username";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Username", Username);

                    con.Open();

                    object result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        IsFound = true;
                    }

                    con.Close();
                }
            }
            return IsFound;
        }

        public static bool CheckUserExistByUserID(int UserID)
        {
            bool IsFound = false;
            using (SqlConnection con = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"Select Found=1 From Users Where LOWER(UserID) = @UserID";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@UserID", UserID);

                    con.Open();

                    object result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        IsFound = true;
                    }

                    con.Close();
                }
            }
            return IsFound;
        }

        public static bool CheckUsernameAndPassword(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                return false;

            try
            {
                using (var connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                using (var command = new SqlCommand("SP_CheckPasswordForUsername", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.Add("@Username", SqlDbType.NVarChar, 50).Value = username;
                    command.Parameters.Add("@Password", SqlDbType.NVarChar, 100).Value = password;

                    connection.Open();

                    var result = command.ExecuteScalar();
                    return result != null && Convert.ToInt32(result) == 1;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in CheckUsernameAndPassword: {ex.Message}");
                return false;
            }
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
