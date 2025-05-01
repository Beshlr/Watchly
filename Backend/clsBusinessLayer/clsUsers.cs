using System;
using System.Data;
using System.Net.Mail;
using System.Runtime.CompilerServices;
using clsDataAccess;
using MailKit.Net.Smtp;
using MimeKit;
using MovieRecommendations_DataLayer;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;
using System.Configuration;
using clsBusinessLayer;
namespace MovieRecommendations_BusinessLayer
{
    public class clsUsers
    {
        public UserDTO UDTO {
            get
            {
                return new UserDTO(this.UserID, this.Username, this.Password,this.Email, this.IsAcive, this.Permissions, this.Age);
            }
        }

        public AddUserInfoDTO AUserInfoDTO{
            get
            {
                return new AddUserInfoDTO(this.Username, this.Password, this.Age, this.Email);
            }
        }

        public enum enMode { AddNew = 0, Update = 1 };
        public enMode Mode = enMode.AddNew;
        public enum enReportType { RateMovie = 1, ReportMovie = 2 };
        public enReportType ReportType = enReportType.RateMovie;
        public int UserID { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public bool IsAcive { get; set; }
        public byte Permissions { get; set; }
        public int Age { get; set; }


        public clsUsers()
        {
            this.UserID = -1;
            this.Username = "";
            this.Password = "";
            this.IsAcive = false;
            this.Permissions = default(byte);
            this.Age = default(byte);
            Mode = enMode.AddNew;
        }


        private clsUsers(UserDTO userDTO)
        {
            this.UserID = userDTO.ID;
            this.Username = userDTO.Username;
            this.Password = userDTO.Password;
            this.Email = userDTO.Email;
            this.IsAcive = userDTO.IsAcive;
            this.Permissions = userDTO.Permissions;
            this.Age = userDTO.Age;
            Mode = enMode.Update;
        }


       private bool _AddNewUsers()
       {
            this.UserID = clsUsersData.AddNewUsers(AUserInfoDTO);

            return (this.UserID != null);

       }

       public static bool AddNewUsers(AddUserInfoDTO AUserInfoDTO)
        {

            AUserInfoDTO.ID = clsUsersData.AddNewUsers(AUserInfoDTO);

            return (AUserInfoDTO.ID != null);

       }


       private bool _UpdateUsers()
       {

            return clsUsersData.UpdateUsersByID(
this.UserID, this.Username, this.Password, this.IsAcive, this.Permissions, this.Age);
       }


        public static bool ChangeUserPassword(int UserID, string NewPassword, string OldPassword)
        {
            return clsUsersData.ChangeUserPassword(UserID, NewPassword, OldPassword);
        }

        public static clsUsers Find(int UserID)

        {
            UserDTO uDTO = clsUsersData.GetUsersInfoByID(UserID);
            if (uDTO != null)
                return new clsUsers(uDTO);
            else
                return null;

        }

        public static clsUsers Find(string Username)
        {
            UserDTO uDTO = clsUsersData.GetUsersInfoByUsername(Username);
            if (uDTO != null)
                return new clsUsers(uDTO);
            else
                return null;
        }


        public bool Save()
        {
            switch (Mode)
            {
                case enMode.AddNew:
                    if(_AddNewUsers())
                    {
                        Mode = enMode.Update;
                         return true;
                    }
                    else
                    {
                        return false;
                    }
                case enMode.Update:
                    return _UpdateUsers();

            }
        
            return false;
        }



       public static bool DeleteUsers(int UserID)
       {

        return clsUsersData.DeleteUsers(UserID);

       }

        public static bool IsUserExist(int UserID)
        {
            return clsUsersData.CheckUserExistByUserID(UserID);
        }

        public static bool IsUserActive(int UserID)
        {
            return clsUsersData.IsUserActive(UserID);
        }

        public static bool IsUserExist(string Username)
        {
            Username = Username.ToLower();
            return clsUsersData.CheckIsUsernameExist(Username);
        }

        public static bool CheckIfUsernameAndPasswordIsTrue(string Username, string Password)
        {
            return clsUsersData.CheckUsernameAndPassword(Username, Password);
        }

        public static bool CheckIfUserEnterOldPassword(int UserID, string EnterdPassword, ref string TheDiffBetweenTwoDates)
        {
            DateTime dt = DateTime.Now;

            if(clsUsersData.CheckIfUserEnterOldPassword(UserID, EnterdPassword,ref dt))
            {
                if(dt < DateTime.Now.AddMinutes(-3))
                {
                    TheDiffBetweenTwoDates = clsSettings.GetTimeDifference(dt, DateTime.Now);
                }
                else
                {
                    TheDiffBetweenTwoDates = "a few seconds";
                }
                return true;
            }

            return false;
        }

        public static bool CheckIfEmailNotUsed(string Email)
        {
            return clsUsersData.CheckIfEmailNotUsed(Email);
        }

        public static bool AddMovieToFavorate(int MovieID, int UserID)
        {
            return clsUsersData.AddMovieToFavorate(MovieID, UserID);
        }

        public static List<string> GetAllGenresThatUserInterstOn(int UserID)
        {
            return clsUsersData.GetAllGenresThatUserInterstOn(UserID);
        }

        public static bool CheckIfMovieInFavorateList(int MovieID, int UserID)
        {
            return clsUsersData.CheckIfMovieIsFavorateForUser(MovieID, UserID);
        }

        public static bool RemoveMovieFromFavorate(int MovieID, int UserID, ref string message)
        {
            return clsUsersData.RemoveMovieFromFavorate(MovieID, UserID, ref message);
        }

        public static bool AddMovieToSearchingList(int MovieID, int UserID)
        {
            return clsUsersData.AddMovieToSearchingList(MovieID, UserID);
        }

        public static bool AddMovieToWatchingList(int MovieID, int UserID, bool AddToFav, ref string message)
        {
            return clsUsersData.AddMovieToWatchingList(MovieID, UserID, AddToFav, ref message);
        }

        public static bool CheckIfMovieInWatchedList(int MovieID, int UserID)
        {
            return clsUsersData.CheckIfMovieWatchedByUser(MovieID, UserID);
        }

        public static bool RemoveMovieFromWatchedList(int MovieID, int UserID, ref string message)
        {
            return clsUsersData.RemoveMovieFromWatchedList(MovieID, UserID,ref message);
        }

        public static List<MovieDTO> GetAllFavorateMoviesForUser(int UserID)
        {
            return clsUsersData.GetAllFavorateMoviesForUser(UserID);
        }

        public static bool IsMovieInFavorateList(int movieID, int userID)
        {
            return clsUsersData.CheckIfMovieIsFavorateForUser(movieID, userID);
        }
        public static bool AddNewReport(int UserID, int MovieID, string ReportMessage, enReportType reportType)
        
        {
            return clsUsersData.AddNewReportAboutMovie(UserID, MovieID, ReportMessage, (int)reportType);
        }
    }

    
}
