using MovieRecommendations_DataAccess;
using MovieRecommendations_DataLayer;
using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace clsDataAccess
{
    public class clsLogsDTO
    {
        public clsLogsDTO(int logID,int userID, string username, string action, DateTime dateTime)
        {
            LogID = logID;
            UserID = userID;
            Username = username;
            Action = action;
            DateTime = dateTime;
        }

        public int LogID { get; set; }
        public int UserID { get; set; }
        public string Username { get; set; }
        public string Action { get; set; }
        public DateTime DateTime { get; set; }


    }
    
    public class clsLogsData
    {
        public static List<clsLogsDTO> GetAllLogs()
        {
            List<clsLogsDTO> logs = new List<clsLogsDTO>();
            using (SqlConnection con = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SP_GetAllLogs", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    con.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            clsLogsDTO log = new clsLogsDTO(
                                reader.IsDBNull(reader.GetOrdinal("LogID")) ? 0 : reader.GetInt32(reader.GetOrdinal("LogID")),
                                reader.IsDBNull(reader.GetOrdinal("UserID")) ? 0 : reader.GetInt32(reader.GetOrdinal("UserID")),
                                reader.IsDBNull(reader.GetOrdinal("Username")) ? null : reader.GetString(reader.GetOrdinal("Username")),
                                reader.IsDBNull(reader.GetOrdinal("ActionName")) ? null : reader.GetString(reader.GetOrdinal("ActionName")),
                                reader.IsDBNull(reader.GetOrdinal("DateTimeOfAction")) ? DateTime.Now : reader.GetDateTime(reader.GetOrdinal("DateTimeOfAction"))
                            );
                            logs.Add(log);
                        }
                    }
                }
            }
            return logs;
        }

        public static List<clsLogsDTO> GetAllLogsOfUser(string Username)
        {
            List<clsLogsDTO> logs = new List<clsLogsDTO>();
            using (SqlConnection con = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SP_GetAllLogsOfUser", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Username", Username);
                    con.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while(reader.Read())
                        {
                            clsLogsDTO log = new clsLogsDTO(
                                    reader.IsDBNull(reader.GetOrdinal("LogID")) ? 0 : reader.GetInt32(reader.GetOrdinal("LogID")),
                                    reader.IsDBNull(reader.GetOrdinal("UserID")) ? 0 : reader.GetInt32(reader.GetOrdinal("UserID")),
                                    reader.IsDBNull(reader.GetOrdinal("Username")) ? null : reader.GetString(reader.GetOrdinal("Username")),
                                    reader.IsDBNull(reader.GetOrdinal("ActionName")) ? null : reader.GetString(reader.GetOrdinal("ActionName")),
                                    reader.IsDBNull(reader.GetOrdinal("DateTimeOfAction")) ? DateTime.Now : reader.GetDateTime(reader.GetOrdinal("DateTimeOfAction"))
                            );
                            logs.Add(log);
                        }
                    }
                }
            }
            return logs;
        }

        public static bool DeleteLog(int logID)
        {
            using (SqlConnection con = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SP_DeleteLog", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@LogID", logID);
                    var ReturnValue = cmd.Parameters.Add("@ReturnValue", SqlDbType.Int);
                    ReturnValue.Direction = ParameterDirection.ReturnValue;

                    con.Open();
                    cmd.ExecuteNonQuery();
                    return (int)ReturnValue.Value == 1;
                }
            }
        }

        public static bool CheckIfLogExists(int logID)
        {
            using (SqlConnection con = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SP_CheckIfLogExists", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@LogID", logID);
                    var ReturnValue = cmd.Parameters.Add("@ReturnValue", SqlDbType.Int);
                    ReturnValue.Direction = ParameterDirection.ReturnValue;
                    con.Open();
                    cmd.ExecuteNonQuery();
                    return (int)ReturnValue.Value == 1;
                }
            }
        }
    }

    
}
