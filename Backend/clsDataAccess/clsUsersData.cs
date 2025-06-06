
using System;
using Microsoft.Data.SqlClient;
using System.Data;
using MovieRecommendations_DataAccess;
using System.Security.Cryptography;
using clsDataAccess;
using System.Reflection.Metadata.Ecma335;
using Microsoft.Identity.Client;
namespace MovieRecommendations_DataLayer
{
#nullable enable
    public class UserDTO
    {

        public UserDTO(int ID, string Username, string Password,string Email, bool IsAcive, byte Permissions, int Age, DateTime dateOfBirth)
        {
            this.ID = ID;
            this.Username = Username;
            this.Password = Password;
            this.Email = Email;
            this.IsAcive = IsAcive;
            this.Permissions = Permissions;
            this.Age = Age;
            DateOfBirth = dateOfBirth;
        }

        public int ID { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public bool IsAcive { get; set; }
        public byte Permissions { get; set; }
        public int Age { get; set; }
        public DateTime DateOfBirth { get; set; }
    }

    public class AddUserInfoDTO
    {
        public AddUserInfoDTO(string username, string password, DateTime dateOfBirth, string email)
        {
            Username = username;
            Password = password;
            DateOfBirth = dateOfBirth;
            Email = email;
        }

        public int ID { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Email { get; set; }
    }

    public class UserBasicInfoDTO
    {
        public UserBasicInfoDTO(int ID, string Username, string Email, bool IsAcive, byte Permissions, DateTime dateOfBirth)
        {
            this.ID = ID;
            this.Username = Username;
            this.Email = Email;
            this.IsAcive = IsAcive;
            this.Permissions = Permissions;
            DateOfBirth = dateOfBirth;
        }
        public int ID { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public bool IsAcive { get; set; }
        public byte Permissions { get; set; }
        public DateTime DateOfBirth { get; set; }
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
                             reader["Username"] != DBNull.Value ? (string)reader["Username"] : "",
                             reader["Password"] != DBNull.Value ? (string)reader["Password"] :"",
                             reader["Email"] != DBNull.Value ? (string)reader["Email"] : "",
                             reader["IsActive"] != DBNull.Value ? (bool)reader["IsActive"] : false,
                             (byte)reader["Permissions"],
                            (int)reader["Age"],
                             (DateTime)reader["DateOfBirth"]);

                            }
                        }

                    }
                }
            return userDTO;
            }

        public static UserDTO GetUsersInfoByUsername(string username)
        {
            using(SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = "SELECT * FROM Users WHERE LOWER(Username) COLLATE SQL_Latin1_General_CP1_CS_AS = @Username";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Username", username.ToLower());
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new UserDTO(
                                (int)reader["UserID"],
                                (string)reader["Username"],
                                (string)reader["Password"],
                                reader["Email"] != DBNull.Value ? (string)reader["Email"] : null,
                                reader["IsActive"] != DBNull.Value ? (bool)reader["IsActive"] : false,
                                (byte)reader["Permissions"],
                                (int)reader["Age"],
                                (DateTime)reader["DateOfBirth"]);
                        }
                    }
                }
            }
            return null;
        }

        public static List<UserDTO> GetAllUsers()
        {

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = "SELECT * FROM Users";

                using (SqlCommand command = new SqlCommand(query, connection))
                {

                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            List<UserDTO> users = new List<UserDTO>();
                            do
                            {
                                users.Add(new UserDTO(
                                    (int)reader["UserID"],
                                    (string)reader["Username"],
                                    (string)reader["Password"],
                                    reader["Email"] != DBNull.Value ? (string)reader["Email"] : "",
                                    reader["IsActive"] != DBNull.Value ? (bool)reader["IsActive"] : false,
                                    (byte)reader["Permissions"],
                                    (int)reader["Age"],
                                    (DateTime)reader["DateOfBirth"]));
                            } while (reader.Read());
                            return users;
                        }
                        else
                        {
                            return null;
                        }
                    }
                }
            }

        }

        public static List<string> GetAllUsernames()
        {
            List<string> usernames = new List<string>();
            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                using (SqlCommand command = new SqlCommand("SP_GetAllUsersname", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            usernames.Add((string)reader["Username"]);
                        }
                    }
                }
            }
            return usernames;
        }

        public static int AddNewUsers(AddUserInfoDTO userInfo)
        {
            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                using (SqlCommand command = new SqlCommand("SP_AddNewUser", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@Username", userInfo.Username);
                    command.Parameters.AddWithValue("@Password", userInfo.Password);
                    command.Parameters.AddWithValue("@IsActive", true);
                    command.Parameters.AddWithValue("@Email", userInfo.Email);
                    command.Parameters.AddWithValue("@Permissions", 2);
                    command.Parameters.AddWithValue("@DateOfBirth", userInfo.DateOfBirth);

                    var outputParam = new SqlParameter("@NewUserID", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.Output
                    };
                    command.Parameters.Add(outputParam);

                    connection.Open();

                    command.ExecuteNonQuery();

                    // Retrieve the value of the output parameter
                    return (int)outputParam.Value;
                }

            }

        }

        public static bool CheckIfEmailNotUsed(string Email)
        {
            using(SqlConnection con = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                using(SqlCommand cmd = new SqlCommand("SP_CheckIfEmailNotUsed", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Email", Email);
                    var ReturnValue = new SqlParameter("@ReturnValue", SqlDbType.Int);
                    ReturnValue.Direction = ParameterDirection.ReturnValue;
                    cmd.Parameters.Add(ReturnValue);
                    con.Open();
                    try
                    {
                        cmd.ExecuteNonQuery();
                        return (int)ReturnValue.Value == 1;
                    }
                    catch (Exception ex) { }

                }
            }

            return true;
        }

        public static bool UpdateUsersByID(UserBasicInfoDTO userDTO, int UpdatedByUserID = 22)
        {

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                
                using (SqlCommand command = new SqlCommand("SP_UpdateUserInfo", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@UserID", userDTO.ID);
                    command.Parameters.AddWithValue("@Username", userDTO.Username);
                    command.Parameters.AddWithValue("@Email",userDTO.Email);
                    command.Parameters.AddWithValue("@IsActive", userDTO.IsAcive);
                    command.Parameters.AddWithValue("@Permissions", userDTO.Permissions);
                    command.Parameters.AddWithValue("@DateOfBirth", userDTO.DateOfBirth);
                    command.Parameters.AddWithValue("@UpdatedByUserID", UpdatedByUserID);

                    var ReturnValue = new SqlParameter("@ReturnValue", SqlDbType.Int);
                    ReturnValue.Direction = ParameterDirection.ReturnValue;
                    command.Parameters.Add(ReturnValue);

                    connection.Open();
                    try
                    {
                        command.ExecuteNonQuery();
                        if ((int)ReturnValue.Value == 1)
                            return true;

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error ", ex.Message);
                    }
                    finally
                    {
                        connection.Close();
                    }
                }

            }

            return false;
        }

        public static bool DeleteUser(int UserIDToDelete, int UserIDWhoDeleteHim)
        {
            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                using (SqlCommand command = new SqlCommand("SP_DeleteUserByID", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@UserIDToDelete", UserIDToDelete);
                    command.Parameters.AddWithValue("@UserIDWhoDeleteHim", UserIDWhoDeleteHim);

                    var ReturnValue = new SqlParameter("@ReturnValue", SqlDbType.Int);
                    ReturnValue.Direction = ParameterDirection.ReturnValue;
                    command.Parameters.Add(ReturnValue);

                    connection.Open();
                    try
                    {
                        command.ExecuteNonQuery();
                        if ((int)ReturnValue.Value == 1)
                            return true;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error ", ex.Message);
                    }
                    finally
                    {
                        connection.Close();
                    }
                    
                }
            }
            return false;
        }

        public static bool ChangeUserPassword(int UserID, string NewPassword,string OldPassword)
        {
            using (SqlConnection con = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SP_ChangePasswordForUser", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@UserID", UserID);
                    cmd.Parameters.AddWithValue("@NewPassword", NewPassword);
                    cmd.Parameters.AddWithValue("@OldPassword", OldPassword);
                    cmd.Parameters.AddWithValue("@DateToReset", DateTime.Now);
                    var ReturnValue = new SqlParameter("@ReturnValue", SqlDbType.Int);
                    ReturnValue.Direction = ParameterDirection.ReturnValue;
                    cmd.Parameters.Add(ReturnValue);

                    con.Open();
                    try
                    {
                        cmd.ExecuteNonQuery();
                        if ((int)ReturnValue.Value == 4)
                        {
                            Console.WriteLine("Old Password is not correct");
                            return false;
                        }
                        else if ((int)ReturnValue.Value == 0)
                        {
                            Console.WriteLine("Password didn't change");
                            return false;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error ", ex.Message);
                        return false;
                    }
                }
            }
            return true;
        }

        public static bool CheckIsUsernameExist(string Username)
        {
            bool IsFound = false;
            using (SqlConnection con = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"Select Found=1 From Users Where LOWER(Username) COLLATE SQL_Latin1_General_CP1_CS_AS = @Username";
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

        public static bool IsUserActive(int UserID)
        {
            bool IsActive = false;
            using(SqlConnection con = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"Select IsActive From Users Where UserID = @UserID";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@UserID", UserID);
                    con.Open();
                    object result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        IsActive = (bool)result;
                    }
                    con.Close();
                }
            }

            return IsActive;
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

        public static bool Login(string username, string password)
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

        public static bool CheckIfUserEnterOldPassword(int UserID, string EnteredPassword,ref DateTime LastPasswordDateChange )
        {
            using (SqlConnection con = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SP_CheckIfUserEnterOldPassword", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    
                    cmd.Parameters.AddWithValue("@UserID", UserID);
                    cmd.Parameters.AddWithValue("@Password", EnteredPassword);
                    
                    var outputDate = new SqlParameter("@Date", SqlDbType.DateTime);
                    var ReturnValue = new SqlParameter("@ReturnValue", SqlDbType.Int);
                    
                    ReturnValue.Direction = ParameterDirection.ReturnValue;
                    outputDate.Direction = ParameterDirection.Output;
                    
                    cmd.Parameters.Add(ReturnValue);
                    cmd.Parameters.Add(outputDate);

                    con.Open();
                    cmd.ExecuteNonQuery();
                    
                    if((int)ReturnValue.Value == 1)
                    {
                        LastPasswordDateChange = (DateTime)outputDate.Value;
                        return true;
                    }

                    con.Close();
                }
            }
            return false;
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
            
        public static List<MovieDTO> GetAllSearchedMoviesForUser(int UserID)
        {
            using (SqlConnection con = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SP_GetAllSearchedMoviesForUser", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@UserID", UserID);
                    con.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        List<MovieDTO> MDTOs = new List<MovieDTO>();
                        while (reader.Read())
                        {
                            MovieDTO MDTO =
                            clsMovieBasicDetailsData.GetMovieByID(reader.GetInt32(reader.GetOrdinal("MovieID")));
                            MDTOs.Add(MDTO);
                        }
                        return MDTOs;
                    }
                }
            }
        }
        
        public static bool AddMovieToSearchingList(int MovieID, int UserID)
        {
            bool IsAdded = false;
            using (SqlConnection con = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SP_AddNewSearchingOp", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@MovieID", MovieID);
                    cmd.Parameters.AddWithValue("@UserID", UserID);
                    cmd.Parameters.AddWithValue("@AddedAtDate", DateTime.Now);
                    var SearchingID = new SqlParameter("@SearchingOpID", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.Output
                    };
                    cmd.Parameters.Add(SearchingID);
                    con.Open();
                    try
                    {
                        object rowsAffected = cmd.ExecuteScalar();
                        IsAdded = (SearchingID != null);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error ", ex.Message);
                    }
                }
            }
            return IsAdded;
        }

        public static bool CheckIfMovieInSearchingList(int MovieID, int UserID)
        {
            using (SqlConnection con = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SP_CheckIfMovieInSearchingList", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@UserID", UserID);
                    cmd.Parameters.AddWithValue("@MovieID", MovieID);
                    var ReturnParam = new SqlParameter("@ReturnValue", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.ReturnValue
                    };
                    cmd.Parameters.Add(ReturnParam);
                    con.Open();
                    cmd.ExecuteNonQuery();
                    return (int)ReturnParam.Value == 1;
                }
            }
        }

        public static bool RemoveMovieFromSearchingList(int MovieID, int UserID)
        {
            bool IsDeleted = false;
            using (SqlConnection con = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SP_RemoveMovieFromSearchingList", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@MovieID", MovieID);
                    cmd.Parameters.AddWithValue("@UserID", UserID);
                    var ReturnValue = new SqlParameter("@ReturnValue", SqlDbType.Int);
                    ReturnValue.Direction = ParameterDirection.ReturnValue;
                    cmd.Parameters.Add(ReturnValue);
                    con.Open();
                    try
                    {
                        cmd.ExecuteNonQuery();
                        IsDeleted = ((int)ReturnValue.Value == 1);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error ", ex.Message);
                    }
                }
            }
            return IsDeleted;
        }

        public static List<string> GetAllFavorateMoviesNameForUser(int UserID)
        {
            List<string> moviesName = new List<string>();
            using(SqlConnection con = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SP_GetAllFavorateMoviesNameForUser", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@UserID", UserID);
                    con.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            moviesName.Add((string)reader["movie_title"]);
                        }
                    }
                }
            }

            return moviesName;
        }

        public static List<MovieDTO> GetAllWatchedMoviesForUser(int UserID)
        {
            using(SqlConnection con = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SP_GetAllWatchedMoviesForUser", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@UserID", UserID);
                    con.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        List<MovieDTO> MDTOs = new List<MovieDTO>();
                        while (reader.Read())
                        {
                            MovieDTO MDTO =
                            clsMovieBasicDetailsData.GetMovieByID(reader.GetInt32(reader.GetOrdinal("MovieID")));

                            MDTOs.Add(MDTO);
                        }
                        return MDTOs;
                    }
                }
            }
        }
        
        public static bool AddMovieToWatchingList(int MovieID, int UserID, bool AddToFavorate,ref string message)
        {
            bool IsAdded = false;
            using (SqlConnection con = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SP_AddMovieToWatchedMovies", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@MovieID", MovieID);
                    cmd.Parameters.AddWithValue("@UserID", UserID);
                    cmd.Parameters.AddWithValue("@AddedAtDate", DateTime.Now);
                    cmd.Parameters.AddWithValue("@AddedToFav", AddToFavorate);
                    var WatchingID = new SqlParameter("@WatchingID", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.Output
                    };
                    var ReturnParam = new SqlParameter("@ReturnValue", SqlDbType.Int);
                    ReturnParam.Direction = ParameterDirection.ReturnValue;
                    cmd.Parameters.Add(ReturnParam);
                    cmd.Parameters.Add(WatchingID);
                    if (AddToFavorate)
                    {
                        var FavorateID = new SqlParameter("@FavorateID", SqlDbType.Int)
                        {
                            Direction = ParameterDirection.Output
                        };
                        cmd.Parameters.Add(FavorateID);
                        var intersetID = new SqlParameter("@IntersetID", SqlDbType.Int)
                        {
                            Direction = ParameterDirection.Output
                        };
                        
                        cmd.Parameters.Add(intersetID);
                        
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@FavorateID", DBNull.Value);
                        cmd.Parameters.AddWithValue("@IntersetID", DBNull.Value);
                    }
                    con.Open();
                    try
                    {
                        cmd.ExecuteNonQuery();
                        if ((int)ReturnParam.Value == 1)
                        {
                            message = "Movie is already in your favorate list";
                        }
                        else if ((int)ReturnParam.Value == 2)
                        {
                            message = "Movie is already in your watching list";
                            return false;
                        }
                        IsAdded = (WatchingID != null && (int)ReturnParam.Value == 3);
                        if (!IsAdded)
                            message = "Movie did't added to the favorate list";
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error ", ex.Message);
                    }
                }
            }
            return IsAdded;
        }

        public static bool CheckIfMovieWatchedByUser(int MovieID, int UserID)
        {
            using (SqlConnection con = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SP_CheckIfMovieIsInWatchedListForUser", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@UserID", UserID);
                    cmd.Parameters.AddWithValue("@MovieID", MovieID);
                    var ReturnParam = new SqlParameter("@ReturnValue", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.ReturnValue
                    };
                    cmd.Parameters.Add(ReturnParam);

                    con.Open();
                    cmd.ExecuteNonQuery();
                    return (int)ReturnParam.Value == 1;
                }
            }
        }

        public static bool RemoveMovieFromWatchedList(int MovieID, int UserID, ref string message)
        {
            bool IsRemoved = false;
            using (SqlConnection con = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SP_RemoveMovieFromWatchedListForUser", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@MovieID", MovieID);
                    cmd.Parameters.AddWithValue("@UserID", UserID);

                    var ReturnParam = new SqlParameter("@ReturnValue", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.ReturnValue
                    };
                    cmd.Parameters.Add(ReturnParam);
                    con.Open();
                    try
                    {
                        cmd.ExecuteNonQuery();
                        if((int)ReturnParam.Value == 2)
                        {
                            message = "Movie is not in your watched list";
                            return false;
                        }
                        else if ((int)ReturnParam.Value == 0)
                        {
                            message = "Movie didn't add to Watched Movies List";
                            return false;
                        }
                        
                        IsRemoved = (int)ReturnParam.Value == 1;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error ", ex.Message);
                    }
                }
            }
            return IsRemoved;
        }

        public static bool AddMovieToFavorate(int MovieID, int UserID)
        {
            bool IsAdded = false;

            using (SqlConnection con = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SP_AddNewFavorateMovie", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@MovieID", MovieID);
                    cmd.Parameters.AddWithValue("@UserID", UserID);
                    cmd.Parameters.AddWithValue("@AddedAtDate", DateTime.Now);
                    var FavorateID = new SqlParameter("@FavorateID", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.Output
                    };
                    var IntersetID = new SqlParameter("@IntersetID", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.Output
                    };
                    cmd.Parameters.Add(FavorateID);
                    cmd.Parameters.Add(IntersetID);

                    con.Open();
                    try
                    {
                        cmd.ExecuteNonQuery();

                        IsAdded = ((int)FavorateID.Value > 0);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error ", ex.Message);
                    }
                }
            }

            return IsAdded;
        }

        public static bool CheckIfMovieIsFavorateForUser(int MovieID, int UserID)
        {
            using (SqlConnection con = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SP_CheckIfMovieIsFavorateForUser", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@UserID", UserID);
                    cmd.Parameters.AddWithValue("@MovieID", MovieID);
                    var result = new SqlParameter("@FoundRows", SqlDbType.Bit)
                    {
                        Direction = ParameterDirection.Output
                    };
                    cmd.Parameters.Add(result);
                    con.Open();
                    cmd.ExecuteNonQuery();

                    return (bool)result.Value;
                }
            }

        }

        public static List<string> GetAllGenresThatUserInterstOn(int UserID)
        {
            List<string> genres = new List<string>();

            using(SqlConnection con = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                using(SqlCommand cmd = new SqlCommand("SP_GetGenresNameFromInterestsOfUser", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@UserID", UserID);
                    var ReturnValue = new SqlParameter("@ReturnValue", SqlDbType.Int);
                    ReturnValue.Direction = ParameterDirection.ReturnValue;

                    cmd.Parameters.Add(ReturnValue);

                    con.Open();

                    try
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                genres.Add((string)reader["genre_name"]);
                            }
                        }

                        con.Close();
                    }
                    catch(Exception ex) { }
                }
            }

            return genres;
        }

        public static bool RemoveMovieFromFavorate(int MovieID, int UserID, ref string message)
        {
            bool IsRemoved = false;
            using (SqlConnection con = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SP_RemoveMovieFromFavorateListForUser", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@MovieID", MovieID);
                    cmd.Parameters.AddWithValue("@UserID", UserID);

                    var ReturnParam = new SqlParameter("@ReturnValue", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.ReturnValue
                    };
                    cmd.Parameters.Add(ReturnParam);
                    con.Open();
                    try
                    {
                        cmd.ExecuteNonQuery();
                        if ((int)ReturnParam.Value == 2)
                        {
                            message = "Movie is not in your favorate list";
                            return false;
                        }
                        else if ((int)ReturnParam.Value == 0)
                        {
                            message = "Movie didn't add to favorate List";
                            return false;
                        }

                        IsRemoved = (int)ReturnParam.Value == 1;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error ", ex.Message);
                    }
                }
            }
            return IsRemoved;
        }

        public static List<MovieDTO> GetAllFavorateMoviesForUser(int UserID)
        {
            using (SqlConnection con = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SP_GetAllFavorateMoviesForUser", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@UserID", UserID);
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
                                reader.IsDBNull(reader.GetOrdinal("movie_imdb_link")) ? null : reader.GetString(reader.GetOrdinal("movie_imdb_link"))
                            );
                            MDTOs.Add(MDTO);
                        }
                        return MDTOs;
                    }
                }
            }
        }

        public static List<string> GetTop5GenresUserInterstIn(int UserID)
        {
            List<string> genres = new List<string>();

            using(SqlConnection con = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SP_GetTop5GenresUserInterstIn", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@UserID", UserID);
                    

                    con.Open();
                    try
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string genreName = reader["genre_name"].ToString();
                                genres.Add(genreName);
                            }
                            return genres;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error ", ex.Message);
                    }
                }
            }

            return genres;
        }
    
        public static bool AddNewReportAboutMovie(int UserID, int MovieID, string ReportMessage, int ReportType = 1)
        {
            using (SqlConnection con = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                using(SqlCommand cmd = new SqlCommand("SP_AddNewReport", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@UserID", UserID);
                    cmd.Parameters.AddWithValue("@MovieID", MovieID);
                    cmd.Parameters.AddWithValue("@ReportMessage", ReportMessage);
                    cmd.Parameters.AddWithValue("@ReportType", ReportType);
                    var ReturnParam = new SqlParameter("@ReturnValue", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.ReturnValue
                    };
                    cmd.Parameters.Add(ReturnParam);
                    con.Open();
                    try
                    {
                        cmd.ExecuteNonQuery();
                        return (int)ReturnParam.Value == 1;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error ", ex.Message);
                        return false;
                    }
                }
            }
        }

        public static bool AddMovieToUnlikeList(int UserID,int MovieID)
        {
            using (SqlConnection con = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SP_AddMovieToUnlikeList", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@UserID", UserID);
                    cmd.Parameters.AddWithValue("@MovieID", MovieID);
                    cmd.Parameters.AddWithValue("@AddedAtDate", DateTime.Now);
                    var InsertedID = new SqlParameter("@InsertedID", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.Output
                    };
                    cmd.Parameters.Add(InsertedID);
                    var ReturnParam = new SqlParameter("@ReturnValue", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.ReturnValue
                    };
                    cmd.Parameters.Add(ReturnParam);
                    con.Open();
                    try
                    {
                        cmd.ExecuteNonQuery();
                        return (int)ReturnParam.Value >= 1;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error ", ex.Message);
                        return false;
                    }
                }
            }
        }

        public static List<MovieDTO> GetAllUnlikedMoviesToUser(int UserID)
        {
            using (SqlConnection con = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SP_GetAllUnlikedMoviesToUser", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@UserID", UserID);
                    con.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        List<MovieDTO> MDTOs = new List<MovieDTO>();
                        while (reader.Read())
                        {
                            MovieDTO MDTO = 
                            clsMovieBasicDetailsData.GetMovieByID(reader.GetInt32(reader.GetOrdinal("MovieID")));
                            
                            MDTOs.Add(MDTO);
                        }
                        return MDTOs;
                    }
                }
            }

        }
    
        public static List<string> GetAllUnLikedGenresForUser(int UserID)
        {
            List<string> genresCombos = new List<string>();
            using (SqlConnection con = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SP_GetUnLikedGenresForUser", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@UserID", UserID);
                    con.Open();
                    try
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string GenresCombo = reader["GenresList"].ToString();
                                genresCombos.Add(GenresCombo);
                            }
                            return genresCombos;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error ", ex.Message);
                    }
                }
            }
            return genresCombos;
        }

        public static bool CheckIfMovieInUnlikedList (int UserID, int MovieID)
        {
            using (SqlConnection con = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SP_CheckIfMovieInUnlikedList", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@UserID", UserID);
                    cmd.Parameters.AddWithValue("@MovieID", MovieID);
                    var ReturnParam = new SqlParameter("@ReturnValue", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.ReturnValue
                    };
                    cmd.Parameters.Add(ReturnParam);
                    con.Open();
                    try
                    {
                        cmd.ExecuteNonQuery();
                        return (int)ReturnParam.Value == 1;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error ", ex.Message);
                        return false;
                    }
                }
            }
        }
    
        public static bool DeleteMovieFromUnlikedList(int MovieID, int UserID)
        {
            using (SqlConnection con = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SP_DeleteMovieFromUnlikedList", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@UserID", UserID);
                    cmd.Parameters.AddWithValue("@MovieID", MovieID);
                    var ReturnParam = new SqlParameter("@ReturnValue", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.ReturnValue
                    };
                    cmd.Parameters.Add(ReturnParam);
                    con.Open();
                    try
                    {
                        cmd.ExecuteNonQuery();
                        return (int)ReturnParam.Value == 1;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error ", ex.Message);
                        return false;
                    }
                }
            }
        }
    
        public static bool AddMovieToRecommendedMovies(int MovieID, int UserID)
        {
            using (SqlConnection con = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SP_AddMovieToRecommendedMovies", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@UserID", UserID);
                    cmd.Parameters.AddWithValue("@MovieID", MovieID);
                    
                    var ReturnParam = new SqlParameter("@ReturnValue", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.ReturnValue
                    };
                    cmd.Parameters.Add(ReturnParam);
                    con.Open();
                    try
                    {
                        cmd.ExecuteNonQuery();
                        return (int)ReturnParam.Value == 1;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error ", ex.Message);
                        return false;
                    }
                }
            }
        }

        public static bool CheckIfMovieInRecommendationList(int MovieID, int UserID)
        {
            using (SqlConnection con = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SP_CheckIfMovieInRecommendationList", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@UserID", UserID);
                    cmd.Parameters.AddWithValue("@MovieID", MovieID);
                    var ReturnParam = new SqlParameter("@ReturnValue", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.ReturnValue
                    };
                    cmd.Parameters.Add(ReturnParam);

                    con.Open();
                    cmd.ExecuteNonQuery();
                    return (int)ReturnParam.Value == 1;
                }
            }
        }

        public static List<MovieDTO> GetAllRecommendedMoviesForUser(int UserID)
        {
            using (SqlConnection con = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SP_GetAllRecommendedMoviesForUser", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@UserID", UserID);
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
                                reader.IsDBNull(reader.GetOrdinal("movie_imdb_link")) ? null : reader.GetString(reader.GetOrdinal("movie_imdb_link"))
                            );
                            MDTOs.Add(MDTO);
                        }
                        return MDTOs;
                    }
                }
            }
        }
    }

}
