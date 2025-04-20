
using System;
using System.Data;
using MovieRecommendations_DataLayer;

namespace MovieRecommendations_BusinessLayer
{
    public class clsInterests
    {
        #nullable enable

        public enum enMode { AddNew = 0, Update = 1 };
        public enMode Mode = enMode.AddNew;

        public int? InterestsID { get; set; }
        public int UserID { get; set; }
        public clsUsers? UsersInfo { get; set; }
        public int GenresID { get; set; }
        public clsgenres? genresInfo { get; set; }
        public double? InterestLevel { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? Source { get; set; }
        public bool? IsActive { get; set; }


        public clsInterests()
        {
            this.InterestsID = null;
            this.InterestsID = null;
            this.UserID = 0;
            this.GenresID = 0;
            this.InterestLevel = null;
            this.CreatedAt = null;
            this.UpdatedAt = null;
            this.Source = null;
            this.IsActive = null;
            Mode = enMode.AddNew;
        }


        private clsInterests(
int? InterestsID,int UserID, int GenresID, double? InterestLevel, DateTime? CreatedAt, DateTime? UpdatedAt, string? Source, bool? IsActive          )
        {
            this.InterestsID = InterestsID;
            this.UserID = UserID;
            this.UsersInfo = clsUsers.Find(UserID);
            this.GenresID = GenresID;
            this.genresInfo = clsgenres.FindBygenre_id(GenresID);
            this.InterestLevel = InterestLevel;
            this.CreatedAt = CreatedAt;
            this.UpdatedAt = UpdatedAt;
            this.Source = Source;
            this.IsActive = IsActive;
            Mode = enMode.Update;
        }


       private bool _AddNewInterests()
       {
        this.InterestsID = clsInterestsData.AddNewInterests(
this.UserID, this.GenresID, this.InterestLevel, this.CreatedAt, this.UpdatedAt, this.Source, this.IsActive);

            return (this.InterestsID != null);

       }


       public static bool AddNewInterests(
ref int? InterestsID,int UserID, int GenresID, double? InterestLevel, DateTime? CreatedAt, DateTime? UpdatedAt, string? Source, bool? IsActive)
        {
        InterestsID = clsInterestsData.AddNewInterests(
UserID, GenresID, InterestLevel, CreatedAt, UpdatedAt, Source, IsActive);

            return (InterestsID != null);

       }


       private bool _UpdateInterests()
       {
        return clsInterestsData.UpdateInterestsByID(
this.InterestsID, this.UserID, this.GenresID, this.InterestLevel, this.CreatedAt, this.UpdatedAt, this.Source, this.IsActive       );
       }


       public static bool UpdateInterestsByID(
int? InterestsID,int UserID, int GenresID, double? InterestLevel, DateTime? CreatedAt, DateTime? UpdatedAt, string? Source, bool? IsActive          )
        {
        return clsInterestsData.UpdateInterestsByID(
InterestsID, UserID, GenresID, InterestLevel, CreatedAt, UpdatedAt, Source, IsActive);

        }


       public static clsInterests? FindByInterestsID(int? InterestsID)

        {
            if (InterestsID == null)
            {
                return null;
            }
            int UserID = 0;
            int GenresID = 0;
            double? InterestLevel = null;
            DateTime? CreatedAt = null;
            DateTime? UpdatedAt = null;
            string? Source = null;
            bool? IsActive = null;
            bool IsFound = clsInterestsData.GetInterestsInfoByID(InterestsID, ref UserID,  ref GenresID,
                ref InterestLevel,  ref CreatedAt,  ref UpdatedAt,  ref Source,  ref IsActive);

           if(IsFound)
               return new clsInterests(InterestsID,  UserID,  GenresID,  InterestLevel,  CreatedAt,
                                        UpdatedAt,  Source,  IsActive);
            else
                return  null;
        }


       public static DataTable? GetAllInterests()
       {

        return clsInterestsData.GetAllInterests();

       }



        public bool Save()
        {
            switch (Mode)
            {
                case enMode.AddNew:
                    if(_AddNewInterests())
                    {
                        Mode = enMode.Update;
                         return true;
                    }
                    else
                    {
                        return false;
                    }
                case enMode.Update:
                    return _UpdateInterests();

            }
        
            return false;
        }



       public static bool DeleteInterests(int InterestsID)
       {

        return clsInterestsData.DeleteInterests(InterestsID);

       }


        public enum enInterestsColumns
         {
            InterestsID,
            UserID,
            GenresID,
            InterestLevel,
            CreatedAt,
            UpdatedAt,
            Source,
            IsActive
         }


        public static DataTable? SearchData(enInterestsColumns enChose, string Data)
        {
            if(!SqlHelper.IsSafeInput(Data))
                return null;
            
            return clsInterestsData.SearchData(enChose.ToString(), Data);

        }        



    }
}
