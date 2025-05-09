using clsDataAccess;
using MovieRecommendations_BusinessLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace clsBusinessLayer
{
    public class clsFilters
    {
        public static List<MovieDTO> GetMoviesAfterFilterItToUser(List<MovieDTO> movies, clsUsers user)
        {
            List<MovieDTO> filteredMovies = new List<MovieDTO>();

            //if(user.Age > 18)
                return movies;
            
            //foreach (MovieDTO movie in movies)
            //{

            //    if (movie.ContentRating != "X")
            //        filteredMovies.Add(movie);
            //}

            return filteredMovies;
        }
    }
}
