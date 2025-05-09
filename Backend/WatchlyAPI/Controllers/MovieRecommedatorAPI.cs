using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MovieRecommendations_BusinessLayer;
using clsDataAccess;
using clsBusinessLayer;
using MovieRecommendations_DataLayer;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.IdentityModel.Tokens;
using WatchlyAPI.Settings;
using System.Threading.Tasks;

namespace MovieRecommendationAPI.Controllers
{
    // Start Movie API
    [Route("api/MovieRecommenderAPI")]
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
                return NotFound($"Not Found: No movie has word {PieceOfName}");
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

        [HttpGet("GetTop100MovieBetweenTwoYears/{Year1}/{Year2}/{Genre}", Name = "GetTop100MovieBetweenTwoYears")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public ActionResult<IEnumerable<MovieDTO>> GetTop100MovieBetweenTwoYears(int Year1, int Year2, string Genre="Action")
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

        [HttpGet("GetTop100MovieWithFiltersAndSorting/{StartYear}/{EndYear}/{GenresList}/{MinRatingValue}/{OrderBy}/{OrderValue}",
            Name = "GetTop100MovieWithFiltersAndSorting")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<List<MovieDTO>> GetTop100MovieWithFiltersAndSorting(int StartYear,int EndYear,
                                                    string GenresList,float MinRatingValue,string OrderBy="Year", string OrderValue="DESC")
        {
            if(OrderValue.ToUpper() != "ASC" && OrderValue.ToUpper() != "DESC")
            {
                return BadRequest($"Bad Request: Rating Order Value: {OrderValue} Is Not valid");
            }
            if (StartYear < 1900 || StartYear > DateTime.Now.Year || EndYear < 1900 || EndYear > DateTime.Now.Year || StartYear > EndYear)
            {
                return BadRequest($"Bad Request: Year: {StartYear} or {EndYear} Is Not valid");
            }
            if (String.IsNullOrEmpty(GenresList))
            {
                return BadRequest("Bad Request: Genres List is empty");
            }
            List<MovieDTO> movies = clsMoviePasicDetails.GetTop100MovieBetweenTwoYearsWithGenreAndOrderRating(
                                                                OrderValue, StartYear, EndYear, GenresList, MinRatingValue,OrderBy);
            if (movies == null || movies.Count == 0)
            {
                return NotFound($"Not Found: No movie found between {StartYear} and {EndYear} with Genre: {GenresList}");
            }
            if (MinRatingValue > 0)
            {
                movies = movies.Where(m => m.Rate >= MinRatingValue).ToList();
            }
            

            return Ok(movies);
        }

        [HttpPost("CheckIfMovieExist", Name = "CheckIfMovieExist")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<MovieDTO> CheckIfMovieExist([FromBody]HelperClasses.clsFindMovie findMovieDetails)
        {
            if(String.IsNullOrEmpty(findMovieDetails.MovieName) || findMovieDetails.Year < 1900 || findMovieDetails.Year > DateTime.Now.Year)
            {
                return BadRequest("Bad Request: Movie name or year is not valid");
            }
            int MovieID = -1;
            if (clsMoviePasicDetails.IsMovieExist(findMovieDetails.MovieName, findMovieDetails.Year, ref MovieID))
            {
                MovieDTO movie = clsMoviePasicDetails.GetMovieByID(MovieID);
                return Ok(movie);
            }

            return NotFound($"Not Found: This Movie is not exists");
        }

        [HttpPost("AddNewMovie", Name = "AddNewMovie")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<MovieDTO> AddNewMovie([FromBody]MovieDTO movie)
        {
            int MovieID = -1;
            if (clsMoviePasicDetails.IsMovieExist(movie.MovieName,movie.Year, ref MovieID))
            {
                return BadRequest($"Bad Request: Movie with name {movie.MovieName} is already exists");
            }
            MovieDTO movieDTO = movie;

            string message = string.Empty;

            if (!HelperClasses.clsMovieValidations.CheckMovieInputs(movieDTO, ref message))
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

        [HttpDelete("DeleteMovie/{ID}", Name = "UpdateMovie")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult DeleteMoiveByID(int ID)
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
            if (!clsMoviePasicDetails.DeleteMovieByID(ID))
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error: Movie not deleted");
            }
            return Ok("Movie deleted successfully");
        }
    }
    // End Movies API

    // =======================================

    //Start Users API
    [Route("api/UsersAPI")]
    [ApiController]
    public class UsersAPI : ControllerBase
    {
        private readonly EmailService _emailService;

        public UsersAPI(EmailService emailService)
        {
            this._emailService = emailService;
        }

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

        [HttpGet("GetAllUsers", Name="GetAllUsers")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<List<UserDTO>> GetAllUsers()
        {
            List<UserDTO> users = clsUsers.GetAllUsers();
            if (users == null || users.Count == 0)
            {
                return NotFound("Not Found: No user found");
            }
            return Ok(users);
        }

        [HttpGet("GetAllUsernames", Name = "GetAllUsernames")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<List<string>> GetAllUsernames()
        {
            List<string> usernames = clsUsers.GetAllUsernames();
            if (usernames == null || usernames.Count == 0)
            {
                return NotFound("Not Found: No user found");
            }
            return Ok(usernames);
        }

        [HttpGet("GetAllGenresThatUserInterstOn/{UserID}", Name = "GetAllGenresThatUserInterstOn")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<List<string>> GetAllGenresThatUserInterstOn(int UserID)
        {
            if (UserID < 1)
            {
                return BadRequest($"Bad Request: UserID: {UserID} Is Not valid");
            }
            if (!clsUsers.IsUserExist(UserID))
            {
                return NotFound($"Bad Request: User with ID {UserID} is not exists");
            }
            List<string> genres = clsUsers.GetAllGenresThatUserInterstOn(UserID);

            if (genres == null || genres.Count == 0)
            {
                return NotFound($"Not Found: No genres found for user with ID {UserID} Check Favorate movies");
            }

            return Ok(genres);
        }

        [HttpGet("GetTop5GenresUserInterstIn/{UserID}", Name = "GetTop5GenresUserInterstIn")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<List<string>> GetTop5GenresUserInterstIn(int UserID)
        {
            List<string> genres = new List<string>();
            if (UserID < 1)
            {
                return BadRequest($"Bad Request: UserID: {UserID} Is Not valid");
            }
            if (!clsUsers.IsUserExist(UserID))
            {
                return NotFound($"Bad Request: User with ID {UserID} is not exists");
            }

            UserDTO userInfo = clsUsers.Find(UserID).UDTO;

            genres = clsUsers.GetTop5GenresUserInterstIn(UserID);

            if (genres.Count < 1)
            {
                return NotFound($"No genres found for user with Username: {userInfo.Username} Check Favorate movies");
            }
            return Ok(genres);
        }

        [HttpGet("CheckIfMovieIsWatched", Name = "CheckIfMovieIsWatched")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult CheckIfMovieIsWatchedByUser([FromQuery] HelperClasses.clsUserAndMovieID userAndMovieID)
        {
            string errorMessage = string.Empty;
            if (!HelperClasses.clsUserValidations.CheckUserAndMovieInputs(userAndMovieID, ref errorMessage))
            {
                return BadRequest(errorMessage);
            }
            int MovieID = userAndMovieID.MovieID;
            int UserID = userAndMovieID.UserID;
            if (clsUsers.CheckIfMovieInWatchedList(MovieID, UserID))
            {
                return BadRequest($"This Movie With ID {MovieID} Is already Watched");
            }
            return Ok();
        }

        [HttpGet("GetAllFavorateMoviesforUser", Name = "GetAllFavorateMoviesforUser")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<List<MovieDTO>> GetAllFavorateMoviesforUser(int UserID)
        {
            if (UserID < 1)
            {
                return BadRequest($"Bad Request: UserID: {UserID} Is Not valid");
            }
            if (!clsUsers.IsUserExist(UserID))
            {
                return NotFound($"Bad Request: User with ID {UserID} is not exists");
            }
            List<MovieDTO> movies = clsUsers.GetAllFavorateMoviesForUser(UserID);
            if (movies == null || movies.Count == 0)
            {
                return NotFound($"Not Found: No movie found for user with ID {UserID}");
            }
            return Ok(movies);
        }

        [HttpGet("CheckIfMovieIsFavorate", Name = "CheckIfMovieIsFavorate")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult CheckIfMovieIsFavorateForUser([FromQuery] HelperClasses.clsUserAndMovieID userAndMovieID)
        {
            string errorMessage = string.Empty;
            if (!HelperClasses.clsUserValidations.CheckUserAndMovieInputs(userAndMovieID, ref errorMessage))
            {
                return BadRequest(errorMessage);
            }
            int MovieID = userAndMovieID.MovieID;
            int UserID = userAndMovieID.UserID;

            if (!clsUsersData.CheckIfMovieIsFavorateForUser(MovieID, UserID))
            {
                return BadRequest($"This Movie With ID {MovieID} Is Not Favorate");
            }

            return Ok("Movie is in the favorate list");
        }

        [HttpGet("SendCodeToUserEmail/{Username}", Name = "SendCodeToUserEmail")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<string>> SendCodeToUserEmail(string Username)
        {
            if (String.IsNullOrEmpty(Username))
            {
                return BadRequest("Bad Request: Username is empty");
            }
            if (!clsUsers.IsUserExist(Username))
            {
                return NotFound($"Not Found: User with username {Username} is not exists");
            }

            clsUsers user = clsUsers.Find(Username);
            if (user == null)
            {
                return NotFound($"Not Found: User with username {Username} is not found");
            }
            if (!clsUsers.IsUserActive(user.UserID))
            {
                return BadRequest($"Bad Request: User with username {Username} is not active");
            }
            if (String.IsNullOrEmpty(user.Email))
            {
                return BadRequest($"Bad Request: User with username {Username} has no email. Please call your admin");
            }

            // Generate a random 6-digit code
            Random random = new Random();
            string code = random.Next(100000, 999999).ToString();

            if (String.IsNullOrEmpty(code))
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error: Code not sent");
            }

            // Prepare the email content
            string subject = "Password Reset Code";
            string body = $"Your password reset code is: {code}";

            // Send the email
            await _emailService.SendEmailAsync(user.Email, subject, body);

            return Ok(code);
        }

        [HttpGet("GetAllFavorateMoviesNameForUser/{UserID}", Name = "GetAllFavorateMoviesNameForUser")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<List<string>> GetAllFavorateMoviesNameForUser(int UserID)
        {
            if (UserID < 1)
            {
                return BadRequest($"Bad Request: UserID: {UserID} Is Not valid");
            }
            if (!clsUsers.IsUserExist(UserID))
            {
                return NotFound($"Bad Request: User with ID {UserID} is not exists");
            }
            List<string> movies = clsUsers.GetAllFavorateMoviesNameForUser(UserID);
            if (movies == null || movies.Count == 0)
            {
                return NotFound($"Not Found: No movie found for user with ID {UserID}");
            }
            return Ok(movies);
        }

        [HttpGet("GetAllSearchedMoviesForUser/{UserID}", Name = "GetAllSearchedMoviesForUser")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<List<MovieDTO>> GetAllSearchedMoviesForUser(int UserID)
        {
            if (UserID < 1)
            {
                return BadRequest($"Bad Request: UserID: {UserID} Is Not valid");
            }
            if (!clsUsers.IsUserExist(UserID))
            {
                return NotFound($"Bad Request: User with ID {UserID} is not exists");
            }
            List<MovieDTO> movies = clsUsers.GetAllSearchedMoviesForUser(UserID);
            if (movies == null || movies.Count == 0)
            {
                return NotFound($"Not Found: No movie found for user with ID {UserID}");
            }
            return Ok(movies);
        }

        [HttpGet("GetAllWatchedMoviesForUser/{UserID}", Name = "GetAllWatchedMoviesForUser")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<List<MovieDTO>> GetAllWatchedMoviesForUser(int UserID)
        {
            if (UserID < 1)
            {
                return BadRequest($"Bad Request: UserID: {UserID} Is Not valid");
            }
            if (!clsUsers.IsUserExist(UserID))
            {
                return NotFound($"Bad Request: User with ID {UserID} is not exists");
            }
            List<MovieDTO> movies = clsUsers.GetAllWatchedMoviesForUser(UserID);
            if (movies == null || movies.Count == 0)
            {
                return NotFound($"Not Found: No movie found for user with ID {UserID}");
            }
            return Ok(movies);
        }

        [HttpGet("GetAllUnlikedMoviesToUser/{UserID}", Name = "GetAllUnlikedMoviesToUser")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<List<MovieDTO>> GetAllUnlikedMoviesToUser(int UserID)
        {
            if (UserID < 1)
            {
                return BadRequest($"Bad Request: UserID: {UserID} Is Not valid");
            }
            if (!clsUsers.IsUserExist(UserID))
            {
                return NotFound($"Bad Request: User with ID {UserID} is not exists");
            }
            List<MovieDTO> movies = clsUsers.GetAllUnlikedMoviesToUser(UserID);
            if (movies == null || movies.Count == 0)
            {
                return NotFound($"Not Found: No movie found for user with ID {UserID}");
            }
            return Ok(movies);
        }

        [HttpGet("CheckIfMovieInUnlikedList", Name = "CheckIfMovieInUnlikedList")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult CheckIfMovieInUnlikedList([FromQuery] HelperClasses.clsUserAndMovieID userAndMovieID)
        {
            string errorMessage = string.Empty;
            if (!HelperClasses.clsUserValidations.CheckUserAndMovieInputs(userAndMovieID, ref errorMessage))
            {
                return BadRequest(errorMessage);
            }
            int MovieID = userAndMovieID.MovieID;
            int UserID = userAndMovieID.UserID;
            if (!clsUsers.CheckIfMovieInUnlikedList(MovieID, UserID))
            {
                return BadRequest($"This Movie With ID {MovieID} Is Not Unliked");
            }
            return Ok("Movie is in the unlike list");
        }

        [HttpPost("AddMovieToSearchingList", Name = "AddMovieToSearchingList")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult AddMovieToSearchingList([FromBody] HelperClasses.clsUserAndMovieID searchingListRequest)
        {
            string errorMessage = string.Empty;
            if (!HelperClasses.clsUserValidations.CheckUserAndMovieInputs(searchingListRequest, ref errorMessage))
            {
                return BadRequest(errorMessage);
            }

            int MovieID = searchingListRequest.MovieID;
            int UserID = searchingListRequest.UserID;

            if(clsUsers.CheckIfMovieInSearchingList(MovieID, UserID))
            {
                return BadRequest($"Bad Request: Movie with ID {MovieID} is already in the searching list");
            }

            if (!clsUsers.AddMovieToSearchingList(MovieID, UserID))
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error: Movie not added to searching list");
            }
            return Ok("Movie added to searching list successfully");
        }

        [HttpPost("CheckPasswordForUsername", Name = "CheckPasswordForUsername")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<UserBasicInfoDTO> CheckPasswordForUsername([FromBody] HelperClasses.clsLoginUserInfo LoginUserInfo)
        {
            if (String.IsNullOrEmpty(LoginUserInfo.Username) || String.IsNullOrEmpty(LoginUserInfo.Password))
            {
                return BadRequest($"Bad Request: Invaild Username Or Password");
            }

            

            if (!clsUsers.IsUserExist(LoginUserInfo.Username))
            {
                return BadRequest($"Bad Request: User with username {LoginUserInfo.Username} is not exists");
            }

            clsUsers user = clsUsers.Find(LoginUserInfo.Username);

            if (user == null)
            {
                return BadRequest("Bad Request: User is not found");
            }

            if(!clsUsers.IsUserActive(user.UserID))
            {
                return BadRequest($"Bad Request: User with username {LoginUserInfo.Username} is not active");
            }
            
            bool CheckPassword = clsUsers.Login(LoginUserInfo.Username,EncryptionHelper.Encrypt(LoginUserInfo.Password));

            if (!CheckPassword)
            {
                return BadRequest("Password Is Uncorrect");
            }

            UserBasicInfoDTO userDTO = user.userBasicInfoDTO ;

            if (userDTO == null)
            {
                return NotFound("Not Found: User is not Exist");
            }

            return Ok(userDTO);
        }

        [HttpPost("AddNewUser", Name = "AddNewUser")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<UserDTO> AddNewUser([FromBody] AddUserInfoDTO AUserInfoDTO)
        {
            clsUsers user = new clsUsers();
            user.Username = AUserInfoDTO.Username;
            user.Password = EncryptionHelper.Encrypt(AUserInfoDTO.Password);
            user.IsAcive = true;
            user.Email = AUserInfoDTO.Email;
            user.Permissions = 2;
            user.DateOfBirth = AUserInfoDTO.DateOfBirth;
            user.Age = (DateTime.Now.Year - AUserInfoDTO.DateOfBirth.Year);

            if (String.IsNullOrEmpty(user.Username) || String.IsNullOrEmpty(user.Password))
            {
                return BadRequest("Bad Request: Username or Password is empty");
            }

            if (clsUsers.IsUserExist(user.Username))
            {
                return BadRequest($"Bad Request: User with username [ {AUserInfoDTO.Username} ] is already exists");
            }

            if (!clsUsers.CheckIfEmailNotUsed(user.Email))
            {
                return BadRequest("Bad Request: Email is not avalible to using");
            }

            if (user.Username.Length < 3 || user.Username.Length > 20)
            {
                return BadRequest("Bad Request: Username length should be between 3 and 20 characters");
            }

            if (AUserInfoDTO.Password.Length < 3 || AUserInfoDTO.Password.Length > 20)
            {
                return BadRequest("Bad Request: Password length should be between 3 and 20 characters");
            }

            if (user.Age < 1 || user.Age > 120)
            {
                return BadRequest("Bad Request: Age should be between 1 and 120");
            }


            if (!user.Save())
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error: User not added");
            }

            return Ok(user.UDTO);
        }

        [HttpPost("AddMovieToFavorate", Name = "AddMovieToFavorate")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult AddMovieToFavorate([FromBody] HelperClasses.clsUserAndMovieID favorateRequest)
        {
            string errorMessage = string.Empty;
            if (!HelperClasses.clsUserValidations.CheckUserAndMovieInputs(favorateRequest, ref errorMessage))
            {
                return BadRequest(errorMessage);
            }

            int MovieID = favorateRequest.MovieID;
            int UserID = favorateRequest.UserID;


            if (clsUsers.IsMovieInFavorateList(MovieID, UserID))
            {
                return BadRequest($"Bad Request: Movie with ID {MovieID} is already in the favorites list");
            }
            if (!clsUsers.AddMovieToFavorate(MovieID, UserID))
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error: Movie not added to favorites");
            }
            return Ok("Movie added to favorites successfully");
        }

        [HttpPost("AddMovieToWatchingList", Name = "AddMovieToWatchingList")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult AddMovieToWatchingList([FromBody] HelperClasses.clsUserAndMovieID watchingListRequest, bool AddedToFavorate = false)
        {
            string errorMessage = string.Empty;
            if (!HelperClasses.clsUserValidations.CheckUserAndMovieInputs(watchingListRequest, ref errorMessage))
            {
                return BadRequest(errorMessage);
            }

            int MovieID = watchingListRequest.MovieID;
            int UserID = watchingListRequest.UserID;
            string message = string.Empty;

            if (!clsUsers.AddMovieToWatchingList(MovieID, UserID, AddedToFavorate, ref message))
            {
                return StatusCode(StatusCodes.Status500InternalServerError, message);
            }
            return Ok("Movie added to watching list successfully");
        }

        [HttpPost("AddReportAboutMovie", Name = "AddReportAboutMovie")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult AddReportAboutMovie([FromBody] HelperClasses.clsReportAboutMovie reportAboutMovie)
        {
            if (reportAboutMovie == null)
            {
                return BadRequest("Bad Request: Report about movie is null");
            }
            string message = string.Empty;
            if (!HelperClasses.clsUserValidations.CheckUserAndMovieInputs(reportAboutMovie.UserAndMovieID, ref message))
            {
                return BadRequest(message);
            }

            int MovieID = reportAboutMovie.UserAndMovieID.MovieID;
            int UserID = reportAboutMovie.UserAndMovieID.UserID;
            string ReportMessage = reportAboutMovie.ReportMessage;
            clsUsers.enReportType ReportType = reportAboutMovie.ReportTypeEnum;

            if (!clsUsers.AddNewReport(UserID, MovieID, ReportMessage, ReportType))
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error: Report not added");
            }

            return Ok("Report added successfully");
        }

        [HttpPost("AddMovieToUnlikeList", Name = "AddMovieToUnlikeList")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult AddMovieToUnlikeList([FromBody] HelperClasses.clsUserAndMovieID userAndMovie)
        {
            string errorMessage = string.Empty;
            if (!HelperClasses.clsUserValidations.CheckUserAndMovieInputs(userAndMovie, ref errorMessage))
            {
                return BadRequest(errorMessage);
            }
            if(clsUsers.CheckIfMovieInUnlikedList(userAndMovie.MovieID, userAndMovie.UserID))
            {
                return BadRequest($"This Movie With ID {userAndMovie.MovieID} Is Already in UnlikedList");
            }
            if(!clsUsers.AddMovieToUnlikeList(userAndMovie.UserID, userAndMovie.MovieID))
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error: Movie not added to unlike list");
            }
            return Ok("Movie added to unlike list successfully");
        }

        [HttpPut("UpdateUser/{UserID}/{UpdatedByUserID}", Name = "UpdateUser")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<UserBasicInfoDTO> UpdateUserInfo(int UserID,int UpdatedByUserID, [FromBody] UserBasicInfoDTO UDTO)
        {
            clsUsers user = clsUsers.Find(UserID);
            if (user == null)
            {
                return NotFound($"Not Found: User with id {UserID} is not found");
            }

            clsUsers updatedUser = clsUsers.Find(UpdatedByUserID);
            if (updatedUser == null)
            {
                return NotFound($"Not Found: Admin user with id {UpdatedByUserID} is not found");
            }

            UDTO.ID = user.UserID;
            user.Username = UDTO.Username;
            user.IsAcive = UDTO.IsAcive == null ? false : UDTO.IsAcive;
            user.Permissions = UDTO.Permissions;
            user.DateOfBirth = UDTO.DateOfBirth;

            if (!clsUsers.UpdateUser(UDTO, UpdatedByUserID))
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error: User not updated");
            }

            return Ok(user.userBasicInfoDTO);

        }

        [HttpPut("ChangePasswordForUser/", Name = "ChangePasswordForUser")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult ChangePasswordForUser([FromBody] HelperClasses.clsChangePasswordForUser UserInfoForChangePassword)
        {
            clsUsers user = clsUsers.Find(UserInfoForChangePassword.Username);

            if (user == null)
            {
                return NotFound($"Not Found: User with username {UserInfoForChangePassword.Username} is not found");
            }

            if (String.IsNullOrEmpty(UserInfoForChangePassword.OldPassword) || String.IsNullOrEmpty(UserInfoForChangePassword.NewPassword))
            {
                return BadRequest("Bad Request: Old Password or New Password is empty");
            }

            if (user.Password != EncryptionHelper.Encrypt(UserInfoForChangePassword.OldPassword))
            {
                return BadRequest("Bad Request: Old Password is incorrect");
            }

            string LastChangeForPassword = String.Empty;

            if (clsUsers.CheckIfUserEnterOldPassword(user.UserID,EncryptionHelper.Encrypt(UserInfoForChangePassword.NewPassword), ref LastChangeForPassword))
            {
                return BadRequest($"Bad Request: New Password is Used before: {LastChangeForPassword}. please enter anoter one");
            }

            if (UserInfoForChangePassword.OldPassword == UserInfoForChangePassword.NewPassword)
            {
                return BadRequest("Bad Request: New Password should be different from Old Password");
            }

            string encryptedNewPassword = EncryptionHelper.Encrypt(UserInfoForChangePassword.NewPassword);
            user.Password = encryptedNewPassword;

            if (!clsUsers.ChangeUserPassword(user.UserID, encryptedNewPassword,EncryptionHelper.Encrypt(UserInfoForChangePassword.OldPassword)))
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error: Password not changed");
            }
            return Ok("Password changed successfully");
        }

       [HttpPut("ChangePasswordForUserWhenForgetIt", Name = "ChangePasswordForUserWhenForgetIt")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult ChangePasswordForUser([FromBody]HelperClasses.clsLoginUserInfo UserLoginInfo)
        {
            clsUsers user = clsUsers.Find(UserLoginInfo.Username);
            int userID = user.UserID;
            string OldPassword = user.Password;
            if (user == null)
            {
                return NotFound($"User with username {UserLoginInfo.Username} is not found");
            }

            if (OldPassword == EncryptionHelper.Encrypt(UserLoginInfo.Password))
            {
                return BadRequest($"New Password is The same of last password");

            }

            string LastChangeForPassword = String.Empty;

            if (clsUsers.CheckIfUserEnterOldPassword(user.UserID,EncryptionHelper.Encrypt(UserLoginInfo.Password), ref LastChangeForPassword))
            {
                return BadRequest($"New Password is Used before: {LastChangeForPassword}. please enter anoter one");
            }

            string encryptedNewPassword = EncryptionHelper.Encrypt(UserLoginInfo.Password);
            user.Password = encryptedNewPassword;

            if (!clsUsers.ChangeUserPassword(user.UserID, encryptedNewPassword,OldPassword))
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error: Password not changed");
            }
            return Ok("Password changed successfully");
        }

        [HttpDelete("DeleteUser/{UserID}/{DeletedByUserID}", Name = "DeleteUser")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult DeleteUser(int UserID, int DeletedByUserID)
        {
            if (UserID < 1 || DeletedByUserID < 1)
            {
                return BadRequest($"Bad Request: UserID: {UserID} or DeletedByUserID: {DeletedByUserID} Is Not valid");
            }

            clsUsers userToDelete = clsUsers.Find(UserID);
            if (userToDelete == null)
            {
                return NotFound($"Not Found: User with id {UserID} is not found");
            }
            if(userToDelete.Permissions == 3)
            {
                return BadRequest($"User with id {UserID} is an Owner and cannot be deleted");
            }
            
            clsUsers deletedByUser = clsUsers.Find(DeletedByUserID);
            if (deletedByUser == null)
            {
                return NotFound($"Not Found: Admin user with id {DeletedByUserID} is not found");
            }

            if(deletedByUser.Permissions != 3 && userToDelete.Permissions == 2)
            {
                return BadRequest($"User with id {DeletedByUserID} is not an Owner and cannot delete Admins");
            }

            if (!clsUsers.DeleteUser(UserID,DeletedByUserID))
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error: User not deleted");
            }

            return Ok("User deleted successfully");
        }
        
        [HttpDelete("RemoveMovieFromWatchedList", Name = "RemoveMovieFromWatchedList")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult RemoveMovieFromWatchedList([FromQuery] HelperClasses.clsUserAndMovieID userAndMovieID)
        {
            string errorMessage = string.Empty;
            if (!HelperClasses.clsUserValidations.CheckUserAndMovieInputs(userAndMovieID, ref errorMessage))
            {
                return BadRequest(errorMessage);
            }
            int MovieID = userAndMovieID.MovieID;
            int UserID = userAndMovieID.UserID;
            string ErrorMessage = string.Empty;

            if (!clsUsers.RemoveMovieFromWatchedList(MovieID, UserID, ref ErrorMessage))
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ErrorMessage);
            }
            return Ok("Movie removed from watched list successfully");
        }

        [HttpDelete("RemoveMovieFromFavorateList", Name = "RemoveMovieFromFavorateList")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult RemoveMovieFromFavorateList([FromQuery] HelperClasses.clsUserAndMovieID userAndMovieID)
        {
            string errorMessage = string.Empty;
            if (!HelperClasses.clsUserValidations.CheckUserAndMovieInputs(userAndMovieID, ref errorMessage))
            {
                return BadRequest(errorMessage);
            }
            int MovieID = userAndMovieID.MovieID;
            int UserID = userAndMovieID.UserID;
            string ErrorMessage = string.Empty;

            if (!clsUsers.RemoveMovieFromFavorate(MovieID, UserID,ref errorMessage))
            {
                return StatusCode(StatusCodes.Status500InternalServerError, errorMessage);
            }
            return Ok("Movie removed from favorate list successfully");
        }

        [HttpDelete("RemoveMovieFromUnlikedList", Name = "RemoveMovieFromUnlikedList")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult RemoveMovieFromUnlikedList([FromQuery] HelperClasses.clsUserAndMovieID userAndMovieID)
        {
            string errorMessage = string.Empty;
            if (!HelperClasses.clsUserValidations.CheckUserAndMovieInputs(userAndMovieID, ref errorMessage))
            {
                return BadRequest(errorMessage);
            }
            int MovieID = userAndMovieID.MovieID;
            int UserID = userAndMovieID.UserID;
            if(!clsUsers.CheckIfMovieInUnlikedList(MovieID, UserID))
            {
                return BadRequest($"This Movie With ID {MovieID} Is Not Unliked");
            }
            if (!clsUsers.DeleteMovieFromUnlikedList(MovieID, UserID))
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error: Movie not removed from unliked list");
            }

            return Ok("Movie removed from unliked list successfully");
        }

        [HttpDelete("RemoveMovieFromSearchList", Name = "RemoveMovieFromSearchingList")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult RemoveMovieFromSearchList([FromQuery] HelperClasses.clsUserAndMovieID userAndMovieID)
        {
            string errorMessage = string.Empty;
            if (!HelperClasses.clsUserValidations.CheckUserAndMovieInputs(userAndMovieID, ref errorMessage))
            {
                return BadRequest(errorMessage);
            }
            int MovieID = userAndMovieID.MovieID;
            int UserID = userAndMovieID.UserID;

            if (!clsUsers.CheckIfMovieInSearchingList(MovieID, UserID))
            {
                return BadRequest($"This Movie With ID {MovieID} Is Not in the searching list");
            }

            if (!clsUsers.RemoveMovieFromSearchingList(MovieID, UserID))
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error: Movie not removed from searching list");
            }
            return Ok("Movie removed from searching list successfully");
        }

        // End Users API
    }

    // =======================================

    [Route("api/LogsAPI")]
    [ApiController]
    public class LogsController : ControllerBase
    {
        // Start Logs API


        [HttpGet("GetAllLogs", Name = "GetAllLogs")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<List<clsLogsDTO>> GetAllLogs()
        {
            List<clsLogsDTO> logs = clsLogs.GetAllLogs();

            if (logs == null || logs.Count == 0)
            {
                return NotFound("Not Found: No logs found");
            }
            return Ok(logs);
        }

        [HttpGet("GetAllLogsForUser/{Username}", Name = "GetAllLogsForUser")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<List<clsLogsDTO>> GetAllLogsForUser(string Username)
        {
            if (String.IsNullOrEmpty(Username))
            {
                return BadRequest("Bad Request: Username is empty");
            }
            if (!clsUsers.IsUserExist(Username))
            {
                return NotFound($"Not Found: User with username {Username} is not exists");
            }
            List<clsLogsDTO> logs = clsLogs.GetAllLogsOfUser(Username);
            if (logs == null || logs.Count == 0)
            {
                return NotFound($"Not Found: No logs found for user with username {Username}");
            }
            return Ok(logs);
        }

        [HttpDelete("DeleteLog/{id}", Name = "DeleteLog")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult DeleteLog(int id)
        {
            if (id < 1)
            {
                return BadRequest($"Bad Request: ID: {id} Is Not valid");
            }
            if (!clsLogs.CheckIfLogExists(id))
            {
                return NotFound($"NotFound: Log with ID {id} is not exists");
            }

            if (!clsLogs.DeleteLog(id))
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error: Log not deleted");
            }

            return Ok("Log deleted successfully");
        }
    
        // End Logs API
    }

    // =======================================

    [Route("api/RecommendationAPI")]
    [ApiController]
    public class RecommedationController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public RecommedationController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [HttpPost("GetMovieRecommendation", Name = "GetMovieRecommendation")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetMovieRecommendation([FromBody] clsMoviesRequest request)
        {
            if (request?.Movies_Name == null || request.Movies_Name.Count == 0)
            {
                return BadRequest(new { message = "Movies_Name list is empty or null." });
            }

            var client = _httpClientFactory.CreateClient();
            var allRecommendations = new List<MovieRecommendationResponse>();

            try
            {
                foreach (var movieName in request.Movies_Name)
                {
                    var aiResponse = await client.PostAsJsonAsync(
                        "http://62.84.183.216:8089/recommend",
                        new { movie_name = movieName }
                    );

                    aiResponse.EnsureSuccessStatusCode();

                    var recommendations = await aiResponse.Content.ReadFromJsonAsync<List<MovieRecommendationResponse>>();
                    if (recommendations != null)
                        allRecommendations.AddRange(recommendations);
                }


                var movieList = new HashSet<MovieDTO>();

                foreach (var movie in allRecommendations)
                {
                    var movieDTO = clsMoviePasicDetails.GetMovieByName(movie.movie_title);
                    if (movieDTO != null)
                    {
                        if(!movieList.Contains(movieDTO))
                            movieList.Add(movieDTO);
                    }
                }

                return Ok(movieList);
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(500, new { message = "Error calling AI endpoint", error = ex.Message });
            }
        }


    }

    
}
