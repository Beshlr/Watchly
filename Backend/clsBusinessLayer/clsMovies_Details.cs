
using System;
using System.Data;
using MovieRecommendations_DataLayer;

namespace MovieRecommendations_BusinessLayer
{
    public class clsMovies_Details
    {
        #nullable enable    

        public enum enMode { AddNew = 0, Update = 1 };
        public enMode Mode = enMode.AddNew;

        public int? ID { get; set; }
        public clsMovies? MoviesInfo { get; private set; }
        public string? color { get; set; }
        public string? director_name { get; set; }
        public double? num_critic_for_reviews { get; set; }
        public int? duration { get; set; }
        public double? director_facebook_likes { get; set; }
        public double? actor_3_facebook_likes { get; set; }
        public string? actor_2_name { get; set; }
        public double? actor_1_facebook_likes { get; set; }
        public double? gross { get; set; }
        public string? actor_1_name { get; set; }
        public string movie_title { get; set; }
        public double num_voted_users { get; set; }
        public double cast_total_facebook_likes { get; set; }
        public string? actor_3_name { get; set; }
        public double? facenumber_in_poster { get; set; }
        public string movie_imdb_link { get; set; }
        public double? num_user_for_reviews { get; set; }
        public string? language { get; set; }
        public string? country { get; set; }
        public string? content_rating { get; set; }
        public double? budget { get; set; }
        public int? title_year { get; set; }
        public double? actor_2_facebook_likes { get; set; }
        public double? imdb_score { get; set; }
        public double? aspect_ratio { get; set; }
        public int movie_facebook_likes { get; set; }
        public string? poster_url { get; set; }
        public string? trailer_url { get; set; }


        public clsMovies_Details()
        {
            this.ID = null;
            this.ID = null;
            this.color = null;
            this.director_name = null;
            this.num_critic_for_reviews = null;
            this.duration = null;
            this.director_facebook_likes = null;
            this.actor_3_facebook_likes = null;
            this.actor_2_name = null;
            this.actor_1_facebook_likes = null;
            this.gross = null;
            this.actor_1_name = null;
            this.movie_title = "";
            this.num_voted_users = 0.0;
            this.cast_total_facebook_likes = 0.0;
            this.actor_3_name = null;
            this.facenumber_in_poster = null;
            this.movie_imdb_link = "";
            this.num_user_for_reviews = null;
            this.language = null;
            this.country = null;
            this.content_rating = null;
            this.budget = null;
            this.title_year = null;
            this.actor_2_facebook_likes = null;
            this.imdb_score = null;
            this.aspect_ratio = null;
            this.movie_facebook_likes = 0;
            this.poster_url = null;
            this.trailer_url = null;
            Mode = enMode.AddNew;
        }


        private clsMovies_Details(
int? ID,string? color, string? director_name, double? num_critic_for_reviews, int? duration, double? director_facebook_likes, double? actor_3_facebook_likes, string? actor_2_name, double? actor_1_facebook_likes, double? gross, string? actor_1_name, string movie_title, double num_voted_users, double cast_total_facebook_likes, string? actor_3_name, double? facenumber_in_poster, string movie_imdb_link, double? num_user_for_reviews, string? language, string? country, string? content_rating, double? budget, int? title_year, double? actor_2_facebook_likes, double? imdb_score, double? aspect_ratio, int movie_facebook_likes, string? poster_url, string? trailer_url          )
        {
            this.ID = ID;
            this.MoviesInfo = clsMovies.FindBymovie_id(ID);
            this.color = color;
            this.director_name = director_name;
            this.num_critic_for_reviews = num_critic_for_reviews;
            this.duration = duration;
            this.director_facebook_likes = director_facebook_likes;
            this.actor_3_facebook_likes = actor_3_facebook_likes;
            this.actor_2_name = actor_2_name;
            this.actor_1_facebook_likes = actor_1_facebook_likes;
            this.gross = gross;
            this.actor_1_name = actor_1_name;
            this.movie_title = movie_title;
            this.num_voted_users = num_voted_users;
            this.cast_total_facebook_likes = cast_total_facebook_likes;
            this.actor_3_name = actor_3_name;
            this.facenumber_in_poster = facenumber_in_poster;
            this.movie_imdb_link = movie_imdb_link;
            this.num_user_for_reviews = num_user_for_reviews;
            this.language = language;
            this.country = country;
            this.content_rating = content_rating;
            this.budget = budget;
            this.title_year = title_year;
            this.actor_2_facebook_likes = actor_2_facebook_likes;
            this.imdb_score = imdb_score;
            this.aspect_ratio = aspect_ratio;
            this.movie_facebook_likes = movie_facebook_likes;
            this.poster_url = poster_url;
            this.trailer_url = trailer_url;
            Mode = enMode.Update;
        }


       private bool _AddNewMovies_Details()
       {
        this.ID = clsMovies_DetailsData.AddNewMovies_Details(
this.color, this.director_name, this.num_critic_for_reviews, this.duration, this.director_facebook_likes, this.actor_3_facebook_likes, this.actor_2_name, this.actor_1_facebook_likes, this.gross, this.actor_1_name, this.movie_title, this.num_voted_users, this.cast_total_facebook_likes, this.actor_3_name, this.facenumber_in_poster, this.movie_imdb_link, this.num_user_for_reviews, this.language, this.country, this.content_rating, this.budget, this.title_year, this.actor_2_facebook_likes, this.imdb_score, this.aspect_ratio, this.movie_facebook_likes, this.poster_url, this.trailer_url);

            return (this.ID != null);

       }


       public static bool AddNewMovies_Details(
ref int? ID,string? color, string? director_name, double? num_critic_for_reviews, int? duration, double? director_facebook_likes, double? actor_3_facebook_likes, string? actor_2_name, double? actor_1_facebook_likes, double? gross, string? actor_1_name, string movie_title, double num_voted_users, double cast_total_facebook_likes, string? actor_3_name, double? facenumber_in_poster, string movie_imdb_link, double? num_user_for_reviews, string? language, string? country, string? content_rating, double? budget, int? title_year, double? actor_2_facebook_likes, double? imdb_score, double? aspect_ratio, int movie_facebook_likes, string? poster_url, string? trailer_url)
        {
        ID = clsMovies_DetailsData.AddNewMovies_Details(
color, director_name, num_critic_for_reviews, duration, director_facebook_likes, actor_3_facebook_likes, actor_2_name, actor_1_facebook_likes, gross, actor_1_name, movie_title, num_voted_users, cast_total_facebook_likes, actor_3_name, facenumber_in_poster, movie_imdb_link, num_user_for_reviews, language, country, content_rating, budget, title_year, actor_2_facebook_likes, imdb_score, aspect_ratio, movie_facebook_likes, poster_url, trailer_url);

            return (ID != null);

       }


       private bool _UpdateMovies_Details()
       {
        return clsMovies_DetailsData.UpdateMovies_DetailsByID(
this.ID, this.color, this.director_name, this.num_critic_for_reviews, this.duration, this.director_facebook_likes, this.actor_3_facebook_likes, this.actor_2_name, this.actor_1_facebook_likes, this.gross, this.actor_1_name, this.movie_title, this.num_voted_users, this.cast_total_facebook_likes, this.actor_3_name, this.facenumber_in_poster, this.movie_imdb_link, this.num_user_for_reviews, this.language, this.country, this.content_rating, this.budget, this.title_year, this.actor_2_facebook_likes, this.imdb_score, this.aspect_ratio, this.movie_facebook_likes, this.poster_url, this.trailer_url       );
       }


       public static bool UpdateMovies_DetailsByID(
int? ID,string? color, string? director_name, double? num_critic_for_reviews, int? duration, double? director_facebook_likes, double? actor_3_facebook_likes, string? actor_2_name, double? actor_1_facebook_likes, double? gross, string? actor_1_name, string movie_title, double num_voted_users, double cast_total_facebook_likes, string? actor_3_name, double? facenumber_in_poster, string movie_imdb_link, double? num_user_for_reviews, string? language, string? country, string? content_rating, double? budget, int? title_year, double? actor_2_facebook_likes, double? imdb_score, double? aspect_ratio, int movie_facebook_likes, string? poster_url, string? trailer_url          )
        {
        return clsMovies_DetailsData.UpdateMovies_DetailsByID(
ID, color, director_name, num_critic_for_reviews, duration, director_facebook_likes, actor_3_facebook_likes, actor_2_name, actor_1_facebook_likes, gross, actor_1_name, movie_title, num_voted_users, cast_total_facebook_likes, actor_3_name, facenumber_in_poster, movie_imdb_link, num_user_for_reviews, language, country, content_rating, budget, title_year, actor_2_facebook_likes, imdb_score, aspect_ratio, movie_facebook_likes, poster_url, trailer_url);

        }


       public static clsMovies_Details? FindByID(int? ID)

        {
            if (ID == null)
            {
                return null;
            }
            string? color = null;
            string? director_name = null;
            double? num_critic_for_reviews = null;
            int? duration = null;
            double? director_facebook_likes = null;
            double? actor_3_facebook_likes = null;
            string? actor_2_name = null;
            double? actor_1_facebook_likes = null;
            double? gross = null;
            string? actor_1_name = null;
            string movie_title = "";
            double num_voted_users = 0.0;
            double cast_total_facebook_likes = 0.0;
            string? actor_3_name = null;
            double? facenumber_in_poster = null;
            string movie_imdb_link = "";
            double? num_user_for_reviews = null;
            string? language = null;
            string? country = null;
            string? content_rating = null;
            double? budget = null;
            int? title_year = null;
            double? actor_2_facebook_likes = null;
            double? imdb_score = null;
            double? aspect_ratio = null;
            int movie_facebook_likes = 0;
            string? poster_url = null;
            string? trailer_url = null;
            bool IsFound = clsMovies_DetailsData.GetMovies_DetailsInfoByID(ID,
 ref color,  ref director_name,  ref num_critic_for_reviews,  ref duration,  ref director_facebook_likes,  ref actor_3_facebook_likes,  ref actor_2_name,  ref actor_1_facebook_likes,  ref gross,  ref actor_1_name,  ref movie_title,  ref num_voted_users,  ref cast_total_facebook_likes,  ref actor_3_name,  ref facenumber_in_poster,  ref movie_imdb_link,  ref num_user_for_reviews,  ref language,  ref country,  ref content_rating,  ref budget,  ref title_year,  ref actor_2_facebook_likes,  ref imdb_score,  ref aspect_ratio,  ref movie_facebook_likes,  ref poster_url,  ref trailer_url);

           if(IsFound)
               return new clsMovies_Details(
 ID,  color,  director_name,  num_critic_for_reviews,  duration,  director_facebook_likes,  actor_3_facebook_likes,  actor_2_name,  actor_1_facebook_likes,  gross,  actor_1_name,  movie_title,  num_voted_users,  cast_total_facebook_likes,  actor_3_name,  facenumber_in_poster,  movie_imdb_link,  num_user_for_reviews,  language,  country,  content_rating,  budget,  title_year,  actor_2_facebook_likes,  imdb_score,  aspect_ratio,  movie_facebook_likes,  poster_url,  trailer_url);
            else
                return  null;
        }


       public static DataTable? GetAllMovies_Details()
       {

        return clsMovies_DetailsData.GetAllMovies_Details();

       }



        public bool Save()
        {
            switch (Mode)
            {
                case enMode.AddNew:
                    if(_AddNewMovies_Details())
                    {
                        Mode = enMode.Update;
                         return true;
                    }
                    else
                    {
                        return false;
                    }
                case enMode.Update:
                    return _UpdateMovies_Details();

            }
        
            return false;
        }



       public static bool DeleteMovies_Details(int ID)
       {

        return clsMovies_DetailsData.DeleteMovies_Details(ID);

       }


        public enum enMovies_DetailsColumns
         {
            ID,
            color,
            director_name,
            num_critic_for_reviews,
            duration,
            director_facebook_likes,
            actor_3_facebook_likes,
            actor_2_name,
            actor_1_facebook_likes,
            gross,
            actor_1_name,
            movie_title,
            num_voted_users,
            cast_total_facebook_likes,
            actor_3_name,
            facenumber_in_poster,
            movie_imdb_link,
            num_user_for_reviews,
            language,
            country,
            content_rating,
            budget,
            title_year,
            actor_2_facebook_likes,
            imdb_score,
            aspect_ratio,
            movie_facebook_likes,
            poster_url,
            trailer_url
         }


        public static DataTable? SearchData(enMovies_DetailsColumns enChose, string Data)
        {
            if(!SqlHelper.IsSafeInput(Data))
                return null;
            
            return clsMovies_DetailsData.SearchData(enChose.ToString(), Data);

        }        



    }
}
