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

        // Start Movie API
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

        [HttpGet("NameHasWord/{PieceOfName}", Name = "GetByNameHasWord")]
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

            if (Year < 1900 || Year > DateTime.Now.Year)
            {
                return BadRequest($"Bad Request: Year: {Year} Is Not valid");
            }

            string GenreNameString = clsMoviePasicDetails.GetGenreName(GenreName);

            List<MovieDTO> movies = clsMoviePasicDetails.GetTop100MovieWithGenreInYearAndOrderThemByRatingDESC(GenreName, Year);

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

        [HttpGet("GetTop10MoviesWithKeyword/{Keyword}", Name = "GetTop10MoviesWithKeyword")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<IEnumerable<MovieDTO>> GetTop10MoviesWithKeyword(string Keyword)
        {
            if (String.IsNullOrEmpty(Keyword))
            {
                return BadRequest("Movie keyword can't be empty to search for it.");
            }
            List<MovieDTO> movies = clsMoviePasicDetails.GetTop10MoviesWithKeyword(Keyword);
            if (movies == null || movies.Count == 0)
            {
                return NotFound($"Not Found: No movie has Keyword {Keyword}");
            }
            return Ok(movies);
        }

        [HttpPost("AddNewMovie", Name = "AddNewMovie")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<MovieDTO> AddNewMovie(MovieDTO movie)
        {
            if (clsMoviePasicDetails.IsMovieExist(movie.MovieName))
            {
                return BadRequest($"Bad Request: Movie with name {movie.MovieName} is already exists");
            }
            MovieDTO movieDTO = movie;

            string message = string.Empty;

            if (!clsValidations.CheckMovieInputs(movieDTO, ref message))
            {
                return BadRequest(movie);

            }

            clsMoviePasicDetails movieBasicDetails = new clsMoviePasicDetails(movie);
            if (movieBasicDetails.Save())
            {
                return Ok(movieBasicDetails.MDTO);
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error: Movie not added");
            }

        }

        // End Movie API

        // =======================================

        //Start User API

        [HttpGet("GetUserInfoByID/{id}", Name = "GetUserInfoByID")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<UserDTO> GetUserInfoByID(int id)
        {
            if (id < 1)
            {
                return BadRequest($"Bad Request: ID: {id} Is Not valid");
            }

            clsUsers user = clsUsers.Find(id);

            if (user == null)
            {
                return NotFound($"Not Found: User with id {id} is not found");
            }

            return Ok(user.UDTO);
        }

        [HttpGet("CheckPasswordForUsername/{Username}", Name = "CheckPasswordForUsername")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public ActionResult CheckPasswordForUsername(string Username, string Password)
        {
            if (String.IsNullOrEmpty(Username) || String.IsNullOrEmpty(Password))
            {
                return BadRequest($"Bad Request: Invaild Username Or Password");
            }

            bool found = clsUsers.IsUserExist(Username);

            if (!found)
            {
                return NotFound("Not Found: User is not Exist");
            }

            bool CheckPassword = clsUsers.CheckIfUsernameAndPasswordIsTrue(Username, Password);

            if (!CheckPassword)
            {
                return BadRequest("Password Is Uncorrect");
            }

            return Ok("Password is Correct");
        }

        [HttpPut("UpdateUser/{UserID}", Name = "UpdateUser")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<UserDTO> UpdateUserInfo(int UserID, UserDTO UDTO)
        {
            clsUsers user = clsUsers.Find(UserID);
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

        [HttpPost("AddNewUser", Name = "AddNewUser")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<UserDTO> AddNewUser(UserDTO UDTO)
        {
            clsUsers user = new clsUsers();
            user.Username = UDTO.Username;
            user.Password = UDTO.Password;
            user.IsAcive = UDTO.IsAcive == null ? true : false;
            user.Permissions = UDTO.Permissions;
            user.Age = UDTO.Age;

            if (String.IsNullOrEmpty(user.Username) || String.IsNullOrEmpty(user.Password))
            {
                return BadRequest("Bad Request: Username or Password is empty");
            }

            if (user.Username.Length < 3 || user.Username.Length > 20)
            {
                return BadRequest("Bad Request: Username length should be between 3 and 20 characters");
            }

            if (user.Password.Length < 3 || user.Password.Length > 20)
            {
                return BadRequest("Bad Request: Password length should be between 3 and 20 characters");
            }

            if (user.Age < 1 || user.Age > 120)
            {
                return BadRequest("Bad Request: Age should be between 1 and 120");
            }

            if (clsUsers.IsUserExist(user.Username))
            {
                return BadRequest($"Bad Request: User with username {UDTO.Username} is already exists");
            }

            if (!user.Save())
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error: User not added");
            }

            return Ok(user.UDTO);
        }

        [HttpPut("ChangePasswordForUser/{username}", Name = "ChangePasswordForUser")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult ChangePasswordForUser(string username, string OldPassword, string NewPassword)
        {
            clsUsers user = clsUsers.Find(username);

            if (user == null)
            {
                return NotFound($"Not Found: User with username {username} is not found");
            }

            if (String.IsNullOrEmpty(OldPassword) || String.IsNullOrEmpty(NewPassword))
            {
                return BadRequest("Bad Request: Old Password or New Password is empty");
            }

            if (user.Password != SqlHelper.ComputeHash(OldPassword))
            {
                return BadRequest("Bad Request: Old Password is incorrect");
            }

            if (user.Password == SqlHelper.ComputeHash(NewPassword))
            {
                return BadRequest("Bad Request: New Password should be different from Old Password");
            }

            user.Password = NewPassword;

            if (!clsUsers.ChangeUserPassword(user.UserID, NewPassword))
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error: Password not changed");
            }
            return Ok("Password changed successfully");
        }
    

    
    }
    public class clsValidations
    {
            public static bool CheckMovieInputs(MovieDTO movieDTO, ref string message)
        {
            if (movieDTO == null)
            {
                return false;
                message = "Bad Request: Movie is null";
            }
            if (String.IsNullOrEmpty(movieDTO.MovieName) || String.IsNullOrEmpty(movieDTO.PosterImageURL))
            {
                return false;
                message = "Bad Request: Movie name or Poster Image URL is empty";
            }
            if (movieDTO.MovieName.Length < 3 || movieDTO.MovieName.Length > 100)
            {
                return false;
                message = "Bad Request: Movie name length should be between 3 and 100 characters";
            }
            if (movieDTO.Year < 1900 || movieDTO.Year > DateTime.Now.Year)
            {
                return false;
                message = "Bad Request: Year should be between 1900 and current year";
            }
            if (movieDTO.Rate < 0 || movieDTO.Rate > 5)
            {
                return false;
                message = "Bad Request: Rate should be between 0 and 5";
            }
            if (movieDTO.Duration == "m")
            {
                return false;
                message = "Bad Request: Duration should be greater than 0";
            }
            if (movieDTO.ContentRating.Length < 1 || movieDTO.ContentRating.Length > 10)
            {
                return false;
                message = "Bad Request: Content Rating length should be between 1 and 10 characters";
            }
            if (movieDTO.Language.Length < 1 || movieDTO.Language.Length > 20)
            {
                return false;
                message = "Bad Request: Language length should be between 1 and 20 characters";
            }
            if (movieDTO.Country.Length < 1 || movieDTO.Country.Length > 20)
            {
                return false;
                message = "Bad Request: Country length should be between 1 and 20 characters";
            }
            if (movieDTO.AspectRatio < 0)
            {
                return false;
                message = "Bad Request: Aspect Ratio should be greater than 0";
            }
            if (movieDTO.Genre.Length < 1 || movieDTO.Genre.Length > 20)
            {
                return false;
                message = "Bad Request: Genre length should be between 1 and 20 characters";
            }
            if (movieDTO.IMDbMovieURL.Length < 1 || movieDTO.IMDbMovieURL.Length > 100)
            {
                return false;
                message = "Bad Request: IMDb Movie URL length should be between 1 and 100 characters";
            }
            if (movieDTO.Keywords.Length < 1 || movieDTO.Keywords.Length > 100)
            {
                return false;
                message = "Bad Request: Keywords length should be between 1 and 100 characters";
            }

            return true;
        }
    }        

}
