
using System;
using System.Data;
using MovieRecommendations_DataLayer;

namespace MovieRecommendations_BusinessLayer
{
    public class clsgenres
    {
        #nullable enable

        public enum enMode { AddNew = 0, Update = 1 };
        public enMode Mode = enMode.AddNew;

        public int? genre_id { get; set; }
        public string genre_name { get; set; }


        public clsgenres()
        {
            this.genre_id = null;
            this.genre_id = null;
            this.genre_name = "";
            Mode = enMode.AddNew;
        }


        private clsgenres(
int? genre_id,string genre_name          )
        {
            this.genre_id = genre_id;
            this.genre_name = genre_name;
            Mode = enMode.Update;
        }


       private bool _AddNewgenres()
       {
        this.genre_id = clsgenresData.AddNewgenres(
this.genre_name);

            return (this.genre_id != null);

       }


       public static bool AddNewgenres(
ref int? genre_id,string genre_name)
        {
        genre_id = clsgenresData.AddNewgenres(
genre_name);

            return (genre_id != null);

       }


       private bool _Updategenres()
       {
        return clsgenresData.UpdategenresByID(
this.genre_id, this.genre_name       );
       }


       public static bool UpdategenresByID(
int? genre_id,string genre_name          )
        {
        return clsgenresData.UpdategenresByID(
genre_id, genre_name);

        }


       public static clsgenres? FindBygenre_id(int? genre_id)

        {
            if (genre_id == null)
            {
                return null;
            }
            string genre_name = "";
            bool IsFound = clsgenresData.GetgenresInfoByID(genre_id,
 ref genre_name);

           if(IsFound)
               return new clsgenres(
 genre_id,  genre_name);
            else
                return  null;
        }


       public static DataTable? GetAllgenres()
       {

        return clsgenresData.GetAllgenres();

       }



        public bool Save()
        {
            switch (Mode)
            {
                case enMode.AddNew:
                    if(_AddNewgenres())
                    {
                        Mode = enMode.Update;
                         return true;
                    }
                    else
                    {
                        return false;
                    }
                case enMode.Update:
                    return _Updategenres();

            }
        
            return false;
        }



       public static bool Deletegenres(int genre_id)
       {

        return clsgenresData.Deletegenres(genre_id);

       }


        public enum engenresColumns
         {
            genre_id,
            genre_name
         }


        public static DataTable? SearchData(engenresColumns enChose, string Data)
        {
            if(!SqlHelper.IsSafeInput(Data))
                return null;
            
            return clsgenresData.SearchData(enChose.ToString(), Data);

        }        



    }
}
