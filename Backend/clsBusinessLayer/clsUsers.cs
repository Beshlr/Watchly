
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
            this.Password = SqlHelper.ComputeHash(UDTO.Password);
            this.UserID = clsUsersData.AddNewUsers(UDTO);

            return (this.UserID != null);

       }

       public static bool AddNewUsers(UserDTO UDTO)
        {
            UDTO.Password = SqlHelper.ComputeHash(UDTO.Password);

            UDTO.ID = clsUsersData.AddNewUsers(UDTO);

            return (UDTO.ID != null);

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

        public static bool ChangeUserPassword(int UserID, string NewPassword)
        {
            string HashedPassword = SqlHelper.ComputeHash(NewPassword);
            return clsUsersData.ChangeUserPassword(UserID, HashedPassword);
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


        public static List<UserDTO> GetAllUsers()
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

        public static bool IsUserExist(string Username)
        {
            Username = Username.ToLower();
            return clsUsersData.CheckIsUsernameExist(Username);
        }

        public static bool CheckIfUsernameAndPasswordIsTrue(string Username, string Password)
        {
            string HashedPassword = SqlHelper.ComputeHash(Password);

            return clsUsersData.CheckUsernameAndPassword(Username, HashedPassword);
        }

    }
}
