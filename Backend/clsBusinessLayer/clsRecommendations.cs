
using System;
using System.Data;
using MovieRecommendations_DataLayer;

namespace MovieRecommendations_BusinessLayer
{
    public class clsRecommendations
    {
        #nullable enable

        public enum enMode { AddNew = 0, Update = 1 };
        public enMode Mode = enMode.AddNew;

        public int? RecommendID { get; set; }
        public int UserID { get; set; }
        public clsUsers? UsersInfo { get; set; }
        public int MovieID { get; set; }
        public clsMovies_Details? Movies_DetailsInfo { get; set; }


        public clsRecommendations()
        {
            this.RecommendID = null;
            this.RecommendID = null;
            this.UserID = 0;
            this.MovieID = 0;
            Mode = enMode.AddNew;
        }


        private clsRecommendations(
int? RecommendID,int UserID, int MovieID          )
        {
            this.RecommendID = RecommendID;
            this.UserID = UserID;
            this.UsersInfo = clsUsers.Find(UserID);
            this.MovieID = MovieID;
            this.Movies_DetailsInfo = clsMovies_Details.FindByID(MovieID);
            Mode = enMode.Update;
        }


       private bool _AddNewRecommendations()
       {
        this.RecommendID = clsRecommendationsData.AddNewRecommendations(
this.UserID, this.MovieID);

            return (this.RecommendID != null);

       }


       public static bool AddNewRecommendations(
ref int? RecommendID,int UserID, int MovieID)
        {
        RecommendID = clsRecommendationsData.AddNewRecommendations(
UserID, MovieID);

            return (RecommendID != null);

       }


       private bool _UpdateRecommendations()
       {
        return clsRecommendationsData.UpdateRecommendationsByID(
this.RecommendID, this.UserID, this.MovieID       );
       }


       public static bool UpdateRecommendationsByID(
int? RecommendID,int UserID, int MovieID          )
        {
        return clsRecommendationsData.UpdateRecommendationsByID(
RecommendID, UserID, MovieID);

        }


       public static clsRecommendations? FindByRecommendID(int? RecommendID)

        {
            if (RecommendID == null)
            {
                return null;
            }
            int UserID = 0;
            int MovieID = 0;
            bool IsFound = clsRecommendationsData.GetRecommendationsInfoByID(RecommendID,
 ref UserID,  ref MovieID);

           if(IsFound)
               return new clsRecommendations(
 RecommendID,  UserID,  MovieID);
            else
                return  null;
        }


       public static DataTable? GetAllRecommendations()
       {

        return clsRecommendationsData.GetAllRecommendations();

       }



        public bool Save()
        {
            switch (Mode)
            {
                case enMode.AddNew:
                    if(_AddNewRecommendations())
                    {
                        Mode = enMode.Update;
                         return true;
                    }
                    else
                    {
                        return false;
                    }
                case enMode.Update:
                    return _UpdateRecommendations();

            }
        
            return false;
        }



       public static bool DeleteRecommendations(int RecommendID)
       {

        return clsRecommendationsData.DeleteRecommendations(RecommendID);

       }


        public enum enRecommendationsColumns
         {
            RecommendID,
            UserID,
            MovieID
         }


        public static DataTable? SearchData(enRecommendationsColumns enChose, string Data)
        {
            if(!SqlHelper.IsSafeInput(Data))
                return null;
            
            return clsRecommendationsData.SearchData(enChose.ToString(), Data);

        }        



    }
}
