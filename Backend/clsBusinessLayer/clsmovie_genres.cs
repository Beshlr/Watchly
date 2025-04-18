
using System;
using System.Data;
using MovieRecommendations_DataLayer;

namespace MovieRecommendations_BusinessLayer
{
    public class clsmovie_genres
    {
        #nullable enable

        public enum enMode { AddNew = 0, Update = 1 };
        public enMode Mode = enMode.AddNew;

        public int? ID { get; set; }
        public string movie_title { get; set; }
        public int genre_id { get; set; }
        public clsgenres? genresInfo { get; set; }
        public int movie_id { get; set; }
        public clsMovies_Details? Movies_DetailsInfo { get; set; }


        public clsmovie_genres()
        {
            this.ID = null;
            this.ID = null;
            this.movie_title = "";
            this.genre_id = 0;
            this.movie_id = 0;
            Mode = enMode.AddNew;
        }


        private clsmovie_genres(
int? ID,string movie_title, int genre_id, int movie_id          )
        {
            this.ID = ID;
            this.movie_title = movie_title;
            this.genre_id = genre_id;
            this.genresInfo = clsgenres.FindBygenre_id(genre_id);
            this.movie_id = movie_id;
            this.Movies_DetailsInfo = clsMovies_Details.FindByID(movie_id);
            Mode = enMode.Update;
        }


       private bool _AddNewmovie_genres()
       {
        this.ID = clsmovie_genresData.AddNewmovie_genres(
this.movie_title, this.genre_id, this.movie_id);

            return (this.ID != null);

       }


       public static bool AddNewmovie_genres(
ref int? ID,string movie_title, int genre_id, int movie_id)
        {
        ID = clsmovie_genresData.AddNewmovie_genres(
movie_title, genre_id, movie_id);

            return (ID != null);

       }


       private bool _Updatemovie_genres()
       {
        return clsmovie_genresData.Updatemovie_genresByID(
this.ID, this.movie_title, this.genre_id, this.movie_id       );
       }


       public static bool Updatemovie_genresByID(
int? ID,string movie_title, int genre_id, int movie_id          )
        {
        return clsmovie_genresData.Updatemovie_genresByID(
ID, movie_title, genre_id, movie_id);

        }


       public static clsmovie_genres? FindByID(int? ID)

        {
            if (ID == null)
            {
                return null;
            }
            string movie_title = "";
            int genre_id = 0;
            int movie_id = 0;
            bool IsFound = clsmovie_genresData.Getmovie_genresInfoByID(ID,
 ref movie_title,  ref genre_id,  ref movie_id);

           if(IsFound)
               return new clsmovie_genres(
 ID,  movie_title,  genre_id,  movie_id);
            else
                return  null;
        }


       public static DataTable? GetAllmovie_genres()
       {

        return clsmovie_genresData.GetAllmovie_genres();

       }



        public bool Save()
        {
            switch (Mode)
            {
                case enMode.AddNew:
                    if(_AddNewmovie_genres())
                    {
                        Mode = enMode.Update;
                         return true;
                    }
                    else
                    {
                        return false;
                    }
                case enMode.Update:
                    return _Updatemovie_genres();

            }
        
            return false;
        }



       public static bool Deletemovie_genres(int ID)
       {

        return clsmovie_genresData.Deletemovie_genres(ID);

       }


        public enum enmovie_genresColumns
         {
            ID,
            movie_title,
            genre_id,
            movie_id
         }


        public static DataTable? SearchData(enmovie_genresColumns enChose, string Data)
        {
            if(!SqlHelper.IsSafeInput(Data))
                return null;
            
            return clsmovie_genresData.SearchData(enChose.ToString(), Data);

        }        



    }
}
