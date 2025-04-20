
using System;
using System.Data;
using MovieRecommendations_DataLayer;

namespace MovieRecommendations_BusinessLayer
{
    public class clsSearchingOperations
    {
        #nullable enable

        public enum enMode { AddNew = 0, Update = 1 };
        public enMode Mode = enMode.AddNew;

        public int? SearchingID { get; set; }
        public int UserID { get; set; }
        public clsUsers? UsersInfo { get; set; }
        public int MovieID { get; set; }
        public clsMovies_Details? Movies_DetailsInfo { get; set; }


        public clsSearchingOperations()
        {
            this.SearchingID = null;
            this.SearchingID = null;
            this.UserID = 0;
            this.MovieID = 0;
            Mode = enMode.AddNew;
        }


        private clsSearchingOperations(
int? SearchingID,int UserID, int MovieID          )
        {
            this.SearchingID = SearchingID;
            this.UserID = UserID;
            this.UsersInfo = clsUsers.Find(UserID);
            this.MovieID = MovieID;
            this.Movies_DetailsInfo = clsMovies_Details.FindByID(MovieID);
            Mode = enMode.Update;
        }


       private bool _AddNewSearchingOperations()
       {
        this.SearchingID = clsSearchingOperationsData.AddNewSearchingOperations(
this.UserID, this.MovieID);

            return (this.SearchingID != null);

       }


       public static bool AddNewSearchingOperations(
ref int? SearchingID,int UserID, int MovieID)
        {
        SearchingID = clsSearchingOperationsData.AddNewSearchingOperations(
UserID, MovieID);

            return (SearchingID != null);

       }


       private bool _UpdateSearchingOperations()
       {
        return clsSearchingOperationsData.UpdateSearchingOperationsByID(
this.SearchingID, this.UserID, this.MovieID       );
       }


       public static bool UpdateSearchingOperationsByID(
int? SearchingID,int UserID, int MovieID          )
        {
        return clsSearchingOperationsData.UpdateSearchingOperationsByID(
SearchingID, UserID, MovieID);

        }


       public static clsSearchingOperations? FindBySearchingID(int? SearchingID)

        {
            if (SearchingID == null)
            {
                return null;
            }
            int UserID = 0;
            int MovieID = 0;
            bool IsFound = clsSearchingOperationsData.GetSearchingOperationsInfoByID(SearchingID,
 ref UserID,  ref MovieID);

           if(IsFound)
               return new clsSearchingOperations(
 SearchingID,  UserID,  MovieID);
            else
                return  null;
        }


       public static DataTable? GetAllSearchingOperations()
       {

        return clsSearchingOperationsData.GetAllSearchingOperations();

       }



        public bool Save()
        {
            switch (Mode)
            {
                case enMode.AddNew:
                    if(_AddNewSearchingOperations())
                    {
                        Mode = enMode.Update;
                         return true;
                    }
                    else
                    {
                        return false;
                    }
                case enMode.Update:
                    return _UpdateSearchingOperations();

            }
        
            return false;
        }



       public static bool DeleteSearchingOperations(int SearchingID)
       {

        return clsSearchingOperationsData.DeleteSearchingOperations(SearchingID);

       }


        public enum enSearchingOperationsColumns
         {
            SearchingID,
            UserID,
            MovieID
         }


        public static DataTable? SearchData(enSearchingOperationsColumns enChose, string Data)
        {
            if(!SqlHelper.IsSafeInput(Data))
                return null;
            
            return clsSearchingOperationsData.SearchData(enChose.ToString(), Data);

        }        



    }
}
