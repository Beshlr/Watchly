
using System;
using System.Data;
using System.Runtime.CompilerServices;
using MovieRecommendations_DataLayer;

namespace MovieRecommendations_BusinessLayer
{
    public class clsUsers
    {
        public UserDTO UDTO {
            get
            {
                return new UserDTO(this.UserID, this.Username, this.Password, this.IsAcive, this.Permissions, this.Age);
            }
        }

        public enum enMode { AddNew = 0, Update = 1 };
        public enMode Mode = enMode.AddNew;

        public int UserID { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public bool IsAcive { get; set; }
        public byte Permissions { get; set; }
        public byte Age { get; set; }


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
            this.IsAcive = userDTO.IsAcive;
            this.Permissions = userDTO.Permissions;
            this.Age = userDTO.Age;
            Mode = enMode.Update;
        }


       private bool _AddNewUsers()
       {
            string HashedPassword = SqlHelper.ComputeHash(this.Password);
            this.UserID = clsUsersData.AddNewUsers(
this.Username, HashedPassword, this.IsAcive, this.Permissions, this.Age);

            return (this.UserID != null);

       }


       public static bool AddNewUsers(
ref int? UserID,string Username, string Password, bool IsAcive, byte Permissions, byte Age)
        {
            string HashedPassword = SqlHelper.ComputeHash(Password);

            UserID = clsUsersData.AddNewUsers(
Username, HashedPassword, IsAcive, Permissions, Age);

            return (UserID != null);

       }


       private bool _UpdateUsers()
       {
            string HashedPassword = SqlHelper.ComputeHash(this.Password);

            return clsUsersData.UpdateUsersByID(
this.UserID, this.Username, HashedPassword, this.IsAcive, this.Permissions, this.Age       );
       }


       public static bool UpdateUsersByID(
int? UserID,string Username, string Password, bool? IsAcive, byte Permissions, byte Age          )
        {
            string HashedPassword = SqlHelper.ComputeHash(Password);

            return clsUsersData.UpdateUsersByID(
UserID, Username, HashedPassword, IsAcive, Permissions, Age);

        }


       public static clsUsers FindByUserID(int UserID)

        {
            clsUsers user = new clsUsers(clsUsersData.GetUsersInfoByID(UserID));
            
            return user;
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

        public static bool IsUserExist(int UserID)
        {
            return clsUsersData.CheckUserExistByUserID(UserID);
        }

        public static bool CheckIfUsernameExist(string Username)
        {
            return clsUsersData.CheckIsUsernameExist(Username);
        }

        public static bool CheckIfUsernameAndPasswordIsTrue(string Username, string Password)
        {
            string HashedPassword = SqlHelper.ComputeHash(Password);

            return clsUsersData.CheckUsernameAndPassword(Username, HashedPassword);
        }

    }
}
