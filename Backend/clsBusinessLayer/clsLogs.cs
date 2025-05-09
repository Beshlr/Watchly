using clsDataAccess;
using MovieRecommendations_DataLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace clsBusinessLayer
{
    public class clsLogs
    {
        public clsLogsDTO LogsDTO {
            get
            {
                return new clsLogsDTO(this.LogID,this.UserID, this.Username, this.Action, this.DateOfAction);
            }
        }

        public int LogID { get; set; }
        public int UserID { get; set; }
        public string Username { get; set; }
        public string Action { get; set; }
        public DateTime DateOfAction { get; set; }
        public clsLogs()
        {
            this.LogID = -1;
            this.UserID = -1;
            this.Username = "";
            this.Action = "";
            this.DateOfAction = DateTime.Now;
        }
        public clsLogs(clsLogsDTO logsDTO)
        {
            this.LogID = logsDTO.LogID;
            this.UserID = logsDTO.UserID;
            this.Username = logsDTO.Username;
            this.Action = logsDTO.Action;
            this.DateOfAction = logsDTO.DateTime;
        }

        public static List<clsLogsDTO> GetAllLogsOfUser(string Username)
        {
            return clsLogsData.GetAllLogsOfUser(Username);
        }

        public static List<clsLogsDTO> GetAllLogs()
        {
            return clsLogsData.GetAllLogs();
        }

        public static bool CheckIfLogExists(int logID)
        {
            return clsLogsData.CheckIfLogExists(logID);
        }
        public static bool DeleteLog(int logID)
        {
            return clsLogsData.DeleteLog(logID);
        }
    }
}
