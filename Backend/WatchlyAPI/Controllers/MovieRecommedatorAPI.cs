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

            if (!clsMovieValidations.CheckMovieInputs(movieDTO, ref message))
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

        [HttpGet("CheckPasswordForUsername", Name = "CheckPasswordForUsername")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<UserDTO> CheckPasswordForUsername(string Username, string Password)
        {
            if (String.IsNullOrEmpty(Username) || String.IsNullOrEmpty(Password))
            {
                return BadRequest($"Bad Request: Invaild Username Or Password");
            }

            //string decodedPassword = Uri.UnescapeDataString(Password);
            //string decodedUsername = Uri.UnescapeDataString(Username);

            if (!clsUsers.IsUserExist(Username))
            {
                return BadRequest($"Bad Request: User with username {Username} is not exists");
            }

            clsUsers user = clsUsers.Find(Username);

            if(!clsUsers.IsUserActive(user.UserID))
            {
                return BadRequest($"Bad Request: User with username {Username} is not active");
            }
            
            bool CheckPassword = clsUsers.CheckIfUsernameAndPasswordIsTrue(Username,EncryptionHelper.Encrypt(Password));

            if (!CheckPassword)
            {
                return BadRequest("Password Is Uncorrect");
            }

            UserDTO userDTO = user.UDTO ;

            if (userDTO == null)
            {
                return NotFound("Not Found: User is not Exist");
            }

            return Ok(userDTO);
        }

        [HttpPut("UpdateUser/{UserID}", Name = "UpdateUser")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<UserDTO> UpdateUserInfo(int UserID,[FromBody] UserDTO UDTO)
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
        public ActionResult<UserDTO> AddNewUser([FromBody]AddUserInfoDTO AUserInfoDTO)
        {
            clsUsers user = new clsUsers();
            user.Username = AUserInfoDTO.Username;
            user.Password = AUserInfoDTO.Password;
            user.IsAcive = true;
            user.Email = AUserInfoDTO.Email;
            user.Permissions = 2;
            user.Age = AUserInfoDTO.Age;

            if (String.IsNullOrEmpty(user.Username) || String.IsNullOrEmpty(user.Password))
            {
                return BadRequest("Bad Request: Username or Password is empty");
            }

            if(!clsUsers.CheckIfEmailNotUsed(user.Email))
            {
                return BadRequest("Bad Request: Email is not avalible to using");
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
                return BadRequest($"Bad Request: User with username {AUserInfoDTO.Username} is already exists");
            }

            if (!user.Save())
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error: User not added");
            }

            return Ok(user.UDTO);
        }

        [HttpPut("ChangePasswordForUser/", Name = "ChangePasswordForUser")]
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

            // تحقق من كلمة المرور القديمة (بعد التشفير)
            if (user.Password != EncryptionHelper.Encrypt(OldPassword))
            {
                return BadRequest("Bad Request: Old Password is incorrect");
            }

            string LastChangeForPassword = String.Empty;

            // تحقق إذا كانت كلمة المرور الجديدة مستخدمة سابقاً
            if (clsUsers.CheckIfUserEnterOldPassword(user.UserID,EncryptionHelper.Encrypt(NewPassword), ref LastChangeForPassword))
            {
                return BadRequest($"Bad Request: New Password is Used before: {LastChangeForPassword}. please enter anoter one");
            }

            // تحقق إذا كانت كلمة المرور الجديدة مطابقة للقديمة
            if (OldPassword == NewPassword)
            {
                return BadRequest("Bad Request: New Password should be different from Old Password");
            }

            string encryptedNewPassword = EncryptionHelper.Encrypt(NewPassword);
            user.Password = encryptedNewPassword;

            if (!clsUsers.ChangeUserPassword(user.UserID, encryptedNewPassword,EncryptionHelper.Encrypt(OldPassword)))
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error: Password not changed");
            }
            return Ok("Password changed successfully");
        }

       [HttpPut("ChangePasswordForUserWhenForgetIt/", Name = "ChangePasswordForUserWhenForgetIt")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult ChangePasswordForUser(string username, string NewPassword)
        {
            clsUsers user = clsUsers.Find(username);
            int userID = user.UserID;
            string OldPassword = user.Password;
            if (user == null)
            {
                return NotFound($"User with username {username} is not found");
            }

            if (OldPassword == EncryptionHelper.Encrypt(NewPassword))
            {
                return BadRequest($"New Password is The same of last password");

            }

            string LastChangeForPassword = String.Empty;

            // تحقق إذا كانت كلمة المرور الجديدة مستخدمة سابقاً
            if (clsUsers.CheckIfUserEnterOldPassword(user.UserID,EncryptionHelper.Encrypt(NewPassword), ref LastChangeForPassword))
            {
                return BadRequest($"New Password is Used before: {LastChangeForPassword}. please enter anoter one");
            }

            string encryptedNewPassword = EncryptionHelper.Encrypt(NewPassword);
            user.Password = encryptedNewPassword;

            if (!clsUsers.ChangeUserPassword(user.UserID, encryptedNewPassword,OldPassword))
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error: Password not changed");
            }
            return Ok("Password changed successfully");
        }

       

        [HttpPost("AddMovieToFavorate", Name = "AddMovieToFavorate")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult AddMovieToFavorate([FromBody] clsUserAndMovieID favorateRequest)
        {
            string errorMessage = string.Empty;
            if (!clsUserValidations.CheckUserAndMovieInputs(favorateRequest, ref errorMessage))
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

        [HttpGet("GetAllGenresThatUserInterstOn/{UserID}", Name = "GetAllGenresThatUserInterstOn")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<List<string>> GetAllGenresThatUserInterstOn(int UserID)
        {
            if(UserID < 1)
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

        [HttpPost("AddMovieToSearchingList", Name = "AddMovieToSearchingList")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult AddMovieToSearchingList([FromBody] clsUserAndMovieID searchingListRequest)
        {
            string errorMessage = string.Empty;
            if (!clsUserValidations.CheckUserAndMovieInputs(searchingListRequest, ref errorMessage))
            {
                return BadRequest(errorMessage);
            }

            int MovieID = searchingListRequest.MovieID;
            int UserID = searchingListRequest.UserID;
            
            if (!clsUsers.AddMovieToSearchingList(MovieID, UserID))
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error: Movie not added to searching list");
            }
            return Ok("Movie added to searching list successfully");
        }

        [HttpPost("AddMovieToWatchingList", Name = "AddMovieToWatchingList")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult AddMovieToWatchingList([FromBody] clsUserAndMovieID watchingListRequest, bool AddedToFavorate = false)
        {
            string errorMessage = string.Empty;
            if(!clsUserValidations.CheckUserAndMovieInputs(watchingListRequest, ref errorMessage))
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

        [HttpGet("CheckIfMovieIsWatched", Name = "CheckIfMovieIsWatched")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult CheckIfMovieIsWatchedByUser([FromQuery] clsUserAndMovieID userAndMovieID)
        {
            string errorMessage = string.Empty;
            if (!clsUserValidations.CheckUserAndMovieInputs(userAndMovieID, ref errorMessage))
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

        [HttpDelete("RemoveMovieFromWatchedList", Name = "RemoveMovieFromWatchedList")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult RemoveMovieFromWatchedList([FromQuery] clsUserAndMovieID userAndMovieID)
        {
            string errorMessage = string.Empty;
            if (!clsUserValidations.CheckUserAndMovieInputs(userAndMovieID, ref errorMessage))
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
        public ActionResult CheckIfMovieIsFavorateForUser([FromQuery] clsUserAndMovieID userAndMovieID)
        {
            string errorMessage = string.Empty;
            if (!clsUserValidations.CheckUserAndMovieInputs(userAndMovieID, ref errorMessage))
            {
                return BadRequest(errorMessage);
            }
            int MovieID = userAndMovieID.MovieID;
            int UserID = userAndMovieID.UserID;

            if(!clsUsersData.CheckIfMovieIsFavorateForUser(MovieID, UserID))
            {
                return BadRequest($"This Movie With ID {MovieID} Is Not Favorate");
            }

            return Ok("Movie is in the favorate list");
        }
        
        [HttpDelete("RemoveMovieFromFavorateList", Name = "RemoveMovieFromFavorateList")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult RemoveMovieFromFavorateList([FromQuery] clsUserAndMovieID userAndMovieID)
        {
            string errorMessage = string.Empty;
            if (!clsUserValidations.CheckUserAndMovieInputs(userAndMovieID, ref errorMessage))
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

        [HttpPost("AddReportAboutMovie", Name = "AddReportAboutMovie")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult AddReportAboutMovie([FromBody]clsReportAboutMovie reportAboutMovie)
        {
            if (reportAboutMovie == null)
            {
                return BadRequest("Bad Request: Report about movie is null");
            }
            string message = string.Empty;
            if (!clsUserValidations.CheckUserAndMovieInputs(reportAboutMovie.UserAndMovieID, ref message))
            {
                return BadRequest(message);
            }

            int MovieID = reportAboutMovie.UserAndMovieID.MovieID;
            int UserID = reportAboutMovie.UserAndMovieID.UserID;
            string ReportMessage = reportAboutMovie.ReportMessage;
            clsUsers.enReportType ReportType = reportAboutMovie.ReportTypeEnum;

            if (!clsUsers.AddNewReport(UserID,MovieID,ReportMessage,ReportType))
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error: Report not added");
            }

            return Ok("Report added successfully");
        }
        // End Users API
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
        public async Task<IActionResult> GetMovieRecommendation([FromBody] clsGenreRequest request)
        {
            var client = _httpClientFactory.CreateClient();

            try
            {
                var aiResponse = await client.PostAsJsonAsync(
                        "https://montaser14-movies.hf.space/recommend/genre",
                        request
                    );

                aiResponse.EnsureSuccessStatusCode();

                var movies = await aiResponse.Content.ReadFromJsonAsync<List<string>>();
                
                List<MovieDTO> movieList = new List<MovieDTO>();

                foreach (var movie in movies)
                {
                    MovieDTO movieDTO = clsMoviePasicDetails.GetMovieByName(movie);
                    if (movieDTO != null)
                    {
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

    public class clsMovieValidations
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
    public class clsUserValidations
    {
        public static bool CheckUserAndMovieInputs(clsUserAndMovieID userAndMovieID, ref string message)
        {
            if (userAndMovieID == null)
            {
                message = "Bad Request: User and Movie ID is null";
                return false;
            }
            
            int UserID = userAndMovieID.UserID;
            int MovieID = userAndMovieID.MovieID;

            if (UserID < 1 || MovieID < 1)
            {
                message = "Bad Request: UserID or MovieID is not valid";
                return false;
            }
            if (!clsUsers.IsUserExist(UserID))
            {
                message = $"Bad Request: User with ID {UserID} is not exists";
                return false;
            }
            if (!clsMoviePasicDetails.IsMovieExist(MovieID))
            {
                message = $"Bad Request: Movie with ID {MovieID} is not exists";
                return false;
            }
            return true;
        }
        
    }

    public class clsUserAndMovieID
    {
        public clsUserAndMovieID(int movieID, int userID)
        {
            MovieID = movieID;
            UserID = userID;
        }

        public int MovieID { get; set; }
        public int UserID { get; set; }
    }

    public class clsReportAboutMovie
    {
        public clsUserAndMovieID UserAndMovieID { get; set; } 
        public string ReportMessage { get; set; }
        public int ReportType { get; set; }

        public clsUsers.enReportType ReportTypeEnum
        {
            get
            {
                return (clsUsers.enReportType)this.ReportType;
            }
        }
    }
}
