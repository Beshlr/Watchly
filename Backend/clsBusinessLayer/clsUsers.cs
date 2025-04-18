
using System;
using System.Data;
using MovieRecommendations_DataLayer;

namespace MovieRecommendations_BusinessLayer
{
    public class clsUsers
    {
        #nullable enable

        public enum enMode { AddNew = 0, Update = 1 };
        public enMode Mode = enMode.AddNew;

        public int? UserID { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public bool? IsAcive { get; set; }
        public byte Permissions { get; set; }
        public byte Age { get; set; }


        public clsUsers()
        {
            this.UserID = null;
            this.UserID = null;
            this.Username = "";
            this.Password = "";
            this.IsAcive = null;
            this.Permissions = default(byte);
            this.Age = default(byte);
            Mode = enMode.AddNew;
        }


        private clsUsers(
int? UserID,string Username, string Password, bool? IsAcive, byte Permissions, byte Age          )
        {
            this.UserID = UserID;
            this.Username = Username;
            this.Password = Password;
            this.IsAcive = IsAcive;
            this.Permissions = Permissions;
            this.Age = Age;
            Mode = enMode.Update;
        }


       private bool _AddNewUsers()
       {
        this.UserID = clsUsersData.AddNewUsers(
this.Username, this.Password, this.IsAcive, this.Permissions, this.Age);

            return (this.UserID != null);

       }


       public static bool AddNewUsers(
ref int? UserID,string Username, string Password, bool? IsAcive, byte Permissions, byte Age)
        {
        UserID = clsUsersData.AddNewUsers(
Username, Password, IsAcive, Permissions, Age);

            return (UserID != null);

       }


       private bool _UpdateUsers()
       {
        return clsUsersData.UpdateUsersByID(
this.UserID, this.Username, this.Password, this.IsAcive, this.Permissions, this.Age       );
       }


       public static bool UpdateUsersByID(
int? UserID,string Username, string Password, bool? IsAcive, byte Permissions, byte Age          )
        {
        return clsUsersData.UpdateUsersByID(
UserID, Username, Password, IsAcive, Permissions, Age);

        }


       public static clsUsers? FindByUserID(int? UserID)

        {
            if (UserID == null)
            {
                return null;
            }
            string Username = "";
            string Password = "";
            bool? IsAcive = null;
            byte Permissions = default(byte);
            byte Age = default(byte);
            bool IsFound = clsUsersData.GetUsersInfoByID(UserID,
 ref Username,  ref Password,  ref IsAcive,  ref Permissions,  ref Age);

           if(IsFound)
               return new clsUsers(
 UserID,  Username,  Password,  IsAcive,  Permissions,  Age);
            else
                return  null;
        }


       public static DataTable? GetAllUsers()
       {

        return clsUsersData.GetAllUsers();

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


        public enum enUsersColumns
         {
            UserID,
            Username,
            Password,
            IsAcive,
            Permissions,
            Age
         }


        public static DataTable? SearchData(enUsersColumns enChose, string Data)
        {
            if(!SqlHelper.IsSafeInput(Data))
                return null;
            
            return clsUsersData.SearchData(enChose.ToString(), Data);

        }        



    }
}
