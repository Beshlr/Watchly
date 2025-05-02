using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace clsBusinessLayer
{
    public class MovieRecommendationResponse
    {
        public string movie_title { get; set; }
        public string genres { get; set; }

    }

    public class clsMoviesRequest
    {
        public List<string> Movies_Name { get; set; }
    }
}
