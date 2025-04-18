using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MovieRecommendations_BusinessLayer;
using clsDataAccess;
using clsBusinessLayer;
namespace MovieRecommendationAPI.Controllers
{
    [Route("api/[controller]")] 
    [ApiController]
    public class MovieRecommenderAPI : ControllerBase 
    {
        [HttpGet("id/{ID}", Name = "GetByID")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<MovieDTO> GetMovieByID(int ID)
        {
            if (ID < 1)
            {
                return BadRequest($"Bad Request: ID: {ID} Is Not valid");
            }

            MovieDTO movie = clsMoviePasicDetails.GetMovieByID(ID);

            if (movie == null)
            {
                return NotFound($"Not Found: Movie with id {ID} is not found");
            }

            return Ok(movie);
        }

        [HttpGet("StartName/{startName}", Name = "GetByStartName")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<IEnumerable<MovieDTO>> GetMoviesByStartingName(string startName)
        {

            if (String.IsNullOrEmpty(startName))
            {
                return BadRequest("Movie name can't be empty.");
            }

            List<MovieDTO> movies = clsMoviePasicDetails.GetMoviesStartWithName(startName);

            if (movies == null || movies.Count == 0)
            {
                return NotFound($"Not Found: No movie has Name {startName}");
            }

            return Ok(movies);
        }

        [HttpGet("NameHasWord/{PieceOfName}", Name="GetByNameHasWord")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<IEnumerable<MovieDTO>> GetMoviesWithNameHasWord(string PieceOfName)
        {

            if (String.IsNullOrEmpty(PieceOfName))
            {
                return BadRequest("Movie name can't be empty.");
            }

            List<MovieDTO> movies = clsMoviePasicDetails.GetMoviesWithNameHasWord(PieceOfName);

            if (movies == null || movies.Count == 0)
            {
                return NotFound($"Not Found: No movie has Name {PieceOfName}");
            }

            return Ok(movies);
        }


        [HttpGet("GetTop100MovieWithGenre", Name = "GetTop100MovieWithGenreAndOrderThemByRatingDESC")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<IEnumerable<MovieDTO>> GetTop100MovieWithGenreAndOrderThemByRatingDESC(
            [FromQuery] clsMoviePasicDetails.enGenres GenreName = clsMoviePasicDetails.enGenres.Comedy)
        {
            
            List<MovieDTO> movies = clsMoviePasicDetails.GetTop100MovieWithGenreAndOrderThemByRatingDESC(GenreName);

            if (movies == null || movies.Count == 0)
            {
                return NotFound("Not Found: No movie found");
            }

            return Ok(movies);
        }

        [HttpGet("GetTop100MovieWithGenreInYear/{Year}", Name = "GetTop100MovieWithGenreInYearAndOrderThemByRatingDESC")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<IEnumerable<MovieDTO>> GetTop100MovieWithGenreInYearAndOrderThemByRatingDESC(
            [FromQuery] clsMoviePasicDetails.enGenres GenreName, int Year)
        {
            
            if(Year < 1900 || Year > DateTime.Now.Year)
            {
                return BadRequest($"Bad Request: Year: {Year} Is Not valid");
            }

            string GenreNameString = clsMoviePasicDetails.GetGenreName(GenreName);

            List<MovieDTO> movies = clsMoviePasicDetails.GetTop100MovieWithGenreInYearAndOrderThemByRatingDESC(GenreName,Year);

            if (movies == null || movies.Count == 0)
            {
                return NotFound($"Not Found: Not Found movies with genre: {GenreNameString} in Year: {Year}");
            }

            return Ok(movies);
        }



    }
}
