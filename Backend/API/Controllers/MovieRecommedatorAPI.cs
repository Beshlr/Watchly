using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MovieRecommendations_BusinessLayer;
using clsDataAccess;
using clsBusinessLayer;
using MovieRecommendations_DataLayer;
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

        [HttpGet("GetRecommandedMovies/{UserID}", Name = "GetTop10MoviesRecommendedForUserWithID")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<IEnumerable<MovieDTO>> GetTop10MoviesRecommendedForUserWithID(int UserID)
        {
            List<MovieDTO> movies = clsMoviePasicDetails.GetTop10MoviesRecommendedForUserWithID(UserID);

            if (movies == null || movies.Count == 0)
            {
                return NotFound("Not Found: No movie found");
            }

            return Ok(movies);
        }

        [HttpGet("GetTop100MovieBetweenTwoYears/{Year1}/{Year2}", Name = "GetTop100MovieBetweenTwoYears")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public ActionResult<IEnumerable<MovieDTO>> GetTop100MovieBetweenTwoYears(int Year1, int Year2)
        {
            if (Year1 < 1900 || Year1 > DateTime.Now.Year || Year2 < 1900 || Year2 > DateTime.Now.Year || Year1 > Year2)
            {
                return BadRequest($"Bad Request: Year: {Year1} or {Year2} Is Not valid");
            }
            List<MovieDTO> movies = clsMoviePasicDetails.GetTop100MovieBetweenTwoYears(Year1, Year2);
            if (movies == null || movies.Count == 0)
            {
                return NotFound($"Not Found: No movie found between {Year1} and {Year2}");
            }
            return Ok(movies);
        }

        [HttpGet("GetUserInfoByID/{id}", Name = "GetUserInfoByID")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<UserDTO> GetUserInfoByID(int id)
        {
            if(id < 1)
            {
                return BadRequest($"Bad Request: ID: {id} Is Not valid");
            }

            clsUsers user = clsUsers.FindByUserID(id);

            if (user == null)
            {
                return NotFound($"Not Found: User with id {id} is not found");
            }

            return Ok(user.UDTO);
        }

        [HttpGet("CheckPasswordForUsername/{Username}/{Password}", Name = "CheckPasswordForUsername")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]    

        public ActionResult CheckPasswordForUsername(string Username, string Password)
        {
            if(String.IsNullOrEmpty(Username) || String.IsNullOrEmpty(Password))
            {
                return BadRequest($"Bad Request: Invaild Username Or Password");
            }

            bool found = clsUsers.CheckIfUsernameExist(Username);

            if(!found)
            {
                return NotFound("Not Found: User is not Exist");
            }

            bool CheckPassword = clsUsers.CheckIfUsernameAndPasswordIsTrue(Username, Password);

            if(!CheckPassword)
            {
                return BadRequest("Password Is Uncorrect");
            }

            return Ok("Password is Correct");
        }

        [HttpPut("AddNewUser/{UserID}", Name = "AddNewUser")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<UserDTO> UpdateUserInfo(int UserID,UserDTO UDTO)
        {
            clsUsers user = clsUsers.FindByUserID(UserID);
            if (user == null)
            {
                return NotFound($"Not Found: User with id {UserID} is not found");
            }

            user.Username = UDTO.Username;
            user.Password = UDTO.Password;
            user.IsAcive = UDTO.IsAcive == null ? true : false;
            user.Permissions = UDTO.Permissions;
            user.Age = UDTO.Age;

            if (!user.Save())
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error: User not updated");
            }

            return Ok(user.UDTO);

        }
    
        //public ActionResult ChangeUserPassword()
    }
}
