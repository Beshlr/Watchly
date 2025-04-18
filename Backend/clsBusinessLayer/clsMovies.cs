
using System;
using System.Data;
using MovieRecommendations_DataLayer;

namespace MovieRecommendations_BusinessLayer
{
    public class clsMovies
    {
        #nullable enable

        public enum enMode { AddNew = 0, Update = 1 };
        public enMode Mode = enMode.AddNew;

        public int? movie_id { get; set; }
        public string? movie_title { get; set; }


        public clsMovies()
        {
            this.movie_id = null;
            this.movie_id = null;
            this.movie_title = null;
            Mode = enMode.AddNew;
        }


        private clsMovies(
int? movie_id,string? movie_title          )
        {
            this.movie_id = movie_id;
            this.movie_title = movie_title;
            Mode = enMode.Update;
        }


       private bool _AddNewMovies()
       {
        this.movie_id = clsMoviesData.AddNewMovies(
this.movie_title);

            return (this.movie_id != null);

       }


       public static bool AddNewMovies(
ref int? movie_id,string? movie_title)
        {
        movie_id = clsMoviesData.AddNewMovies(
movie_title);

            return (movie_id != null);

       }


       private bool _UpdateMovies()
       {
        return clsMoviesData.UpdateMoviesByID(
this.movie_id, this.movie_title       );
       }


       public static bool UpdateMoviesByID(
int? movie_id,string? movie_title          )
        {
        return clsMoviesData.UpdateMoviesByID(
movie_id, movie_title);

        }


       public static clsMovies? FindBymovie_id(int? movie_id)

        {
            if (movie_id == null)
            {
                return null;
            }
            string? movie_title = null;
            bool IsFound = clsMoviesData.GetMoviesInfoByID(movie_id,
 ref movie_title);

           if(IsFound)
               return new clsMovies(
 movie_id,  movie_title);
            else
                return  null;
        }


       public static DataTable GetAllMovies()
       {

        return clsMoviesData.GetAllMovies();

       }

        public static List<clsMovies> GetAllMoviesWithBasicInfo()
        {
            List<clsMovies> movies = new List<clsMovies>();

            DataTable dt = GetAllMovies();

            foreach(DataRow row in dt.Rows)
            {
                clsMovies movie = new clsMovies();

                movie.movie_id = Convert.ToInt32(row["movie_id"]);
                movie.movie_title = Convert.ToString(row["movie_title"]);

                movies.Add(movie);
            }

            return movies;
        }

        public bool Save()
        {
            switch (Mode)
            {
                case enMode.AddNew:
                    if(_AddNewMovies())
                    {
                        Mode = enMode.Update;
                         return true;
                    }
                    else
                    {
                        return false;
                    }
                case enMode.Update:
                    return _UpdateMovies();

            }
        
            return false;
        }



       public static bool DeleteMovies(int movie_id)
       {

        return clsMoviesData.DeleteMovies(movie_id);

       }


        public enum enMoviesColumns
         {
            movie_id,
            movie_title
         }


        public static DataTable? SearchData(enMoviesColumns enChose, string Data)
        {
            if(!SqlHelper.IsSafeInput(Data))
                return null;
            
            return clsMoviesData.SearchData(enChose.ToString(), Data);

        }        



    }
}
