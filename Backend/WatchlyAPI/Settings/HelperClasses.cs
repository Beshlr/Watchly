using clsBusinessLayer;
using clsDataAccess;
using MovieRecommendations_BusinessLayer;

namespace WatchlyAPI.Settings
{
    public static class HelperClasses
    {
        public class clsMovieValidations
        {
            public static bool CheckMovieInputs(MovieDTO movieDTO, ref string message)
            {
                if (movieDTO == null)
                {
                    return false;
                    message = "Bad Request: Movie is null";
                }
                if (String.IsNullOrEmpty(movieDTO.MovieName) || String.IsNullOrEmpty(movieDTO.PosterImageURL))
                {
                    return false;
                    message = "Bad Request: Movie name or Poster Image URL is empty";
                }
                if (movieDTO.MovieName.Length < 3 || movieDTO.MovieName.Length > 100)
                {
                    return false;
                    message = "Bad Request: Movie name length should be between 3 and 100 characters";
                }
                if (movieDTO.Year < 1900 || movieDTO.Year > DateTime.Now.Year)
                {
                    return false;
                    message = "Bad Request: Year should be between 1900 and current year";
                }
                if (movieDTO.Rate < 0 || movieDTO.Rate > 5)
                {
                    return false;
                    message = "Bad Request: Rate should be between 0 and 5";
                }
                if (movieDTO.Duration == "m")
                {
                    return false;
                    message = "Bad Request: Duration should be greater than 0";
                }
                
                if (movieDTO.Language.Length < 1 || movieDTO.Language.Length > 20)
                {
                    return false;
                    message = "Bad Request: Language length should be between 1 and 20 characters";
                }
                if (movieDTO.Country.Length < 1 || movieDTO.Country.Length > 20)
                {
                    return false;
                    message = "Bad Request: Country length should be between 1 and 20 characters";
                }
                if (movieDTO.AspectRatio < 0)
                {
                    return false;
                    message = "Bad Request: Aspect Ratio should be greater than 0";
                }
                if (movieDTO.Genre.Length < 1 || movieDTO.Genre.Length > 20)
                {
                    return false;
                    message = "Bad Request: Genre length should be between 1 and 20 characters";
                }
                if (movieDTO.IMDbMovieURL.Length < 1 || movieDTO.IMDbMovieURL.Length > 100)
                {
                    return false;
                    message = "Bad Request: IMDb Movie URL length should be between 1 and 100 characters";
                }
                if (movieDTO.Keywords.Length < 1 || movieDTO.Keywords.Length > 100)
                {
                    return false;
                    message = "Bad Request: Keywords length should be between 1 and 100 characters";
                }

                return true;
            }
        }
        
        public class clsUserValidations
        {
            public static bool CheckUserAndMovieInputs(clsUserAndMovieID userAndMovieID, ref string message)
            {
                if (userAndMovieID == null)
                {
                    message = "Bad Request: User and Movie ID is null";
                    return false;
                }

                int UserID = userAndMovieID.UserID;
                int MovieID = userAndMovieID.MovieID;

                if (UserID < 1 || MovieID < 1)
                {
                    message = "Bad Request: UserID or MovieID is not valid";
                    return false;
                }
                if (!clsUsers.IsUserExist(UserID))
                {
                    message = $"Bad Request: User with ID {UserID} is not exists";
                    return false;
                }
                if (!clsMovieBasicDetails.IsMovieExist(MovieID))
                {
                    message = $"Bad Request: Movie with ID {MovieID} is not exists";
                    return false;
                }
                return true;
            }

        }

        public class clsUserAndMovieID
        {
            public clsUserAndMovieID()
            {
            }

            public clsUserAndMovieID(int movieID, int userID)
            {
                MovieID = movieID;
                UserID = userID;
            }

            public int MovieID { get; set; }
            public int UserID { get; set; }
        }

        public class clsReportAboutMovie
        {
            public clsUserAndMovieID UserAndMovieID { get; set; }
            public string ReportMessage { get; set; }
            public int ReportType { get; set; }

            public clsUsers.enReportType ReportTypeEnum
            {
                get
                {
                    return (clsUsers.enReportType)this.ReportType;
                }
            }
        }
       
        public class clsLoginUserInfo
        {
            public string Username { get; set; }
            public string Password { get; set; }
        }
       
        public class clsChangePasswordForUser
        {
            public string Username { get; set; }
            public string OldPassword { get; set; }
            public string NewPassword { get; set; }
        }

        public class clsFindMovie
        {
            public string MovieName { get; set; }
            public int Year { get; set; }

        }
        
        
    }
}
