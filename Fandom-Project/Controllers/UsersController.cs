using Microsoft.AspNetCore.Mvc;
using Fandom_Project.Models;
using Fandom_Project.Repository.Interfaces;
using AutoMapper;
using Fandom_Project.Models.DataTransferObjects.UserModel;
using Fandom_Project.Models.DataTransferObjects.UserCommunityModel;
using Fandom_Project.Models.DataTransferObjects.CommunityModel;

namespace Fandom_Project.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IRepositoryWrapper _repository;
        private readonly ILogger _logger;
        private readonly IMapper _mapper;

        public UsersController(IRepositoryWrapper repository, ILogger<UsersController> logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }

        /// <summary>
        /// Returns all Users added in the database
        /// </summary>
        /// <returns></returns>
        /// <response code="200">Returns all Users</response>
        /// <response code="404">No User is registered in the database</response>
        // GET: api/Users
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = null)]
        [HttpGet]
        public IActionResult GetAllUsers()
        {            
            try
            {                
                _logger.LogInformation($"[{DateTime.Now}] LOG: Requesting GET api/user");
                var users = _repository.User.GetAllUsers();

                if(users.Count() == 0)
                {
                    _logger.LogInformation($"[{DateTime.Now}] LOG: No User was found.");
                    return StatusCode(StatusCodes.Status404NotFound, new 
                    { 
                        message = "No User was found in the database." 
                    });
                }
                
                _logger.LogInformation($"[{DateTime.Now}] LOG: Returned all Users from the database.");
                var usersResult = _mapper.Map<IEnumerable<UserDto>>(users);                
                return StatusCode(StatusCodes.Status200OK, new 
                { 
                    body = usersResult, 
                    message = "Returned all Users from the database." 
                });
            }
            catch (Exception e)
            {
                _logger.LogError($"[{DateTime.Now}] ERROR: {e}");
                return StatusCode(StatusCodes.Status500InternalServerError, new 
                { 
                    message = "A error has ocurred in the service."
                });
            }
        }

        /// <summary>
        /// Return a specific User by using it's ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <response code="200">Returns specific User</response>
        /// <response code="404">User was not found</response>
        // GET: api/Users/{id}
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = null)]
        [HttpGet("{id}", Name = "GetUserById")]
        public IActionResult GetUserById(int id)
        {
            try
            {
                _logger.LogInformation($"[{DateTime.Now}] LOG: Requesting GET api/user/{id}");
                var user = _repository.User.GetUserById(id);

                //TODO: Add validation to check if ID is a valid number (ex: '2=' is not valid)
                //if(id.)
                //{
                //    return StatusCode(StatusCodes.Status404NotFound, new { message = $"ID with value {id} is not valid." });
                //}
                if(user == null)
                {
                    _logger.LogInformation($"[{DateTime.Now}] LOG: User with ID {id} was not found.");
                    return StatusCode(StatusCodes.Status404NotFound, new 
                    { 
                        message = $"User was not found." 
                    });
                }
                
                _logger.LogInformation($"[{DateTime.Now}] LOG: Returned selected User from the database.");
                var userResult = _mapper.Map<UserDto>(user);
                return StatusCode(StatusCodes.Status200OK, new 
                {
                    body = userResult, 
                    message = "Returned selected User from the database." 
                });                                
            }
            catch (Exception e)
            {
                _logger.LogError($"[{DateTime.Now}] ERROR: {e}");
                return StatusCode(StatusCodes.Status500InternalServerError, new 
                { 
                    message = "A error has ocurred in the service." 
                });
            }
        }

        /// <summary>
        /// Update data from a specific User
        /// </summary>
        /// <param name="id"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        /// <response code="201">User data was updated successfully</response>
        /// <response code="400">Request data body is null</response>
        /// <response code="404">User ID was not found in the database</response>
        // PUT: api/Users/{id}        
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(UserDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = null)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = null)]
        [HttpPut("{id}")]
        public IActionResult UpdateUser(int id, [FromBody]UserUpdateDto user)
        {
            try
            {
                _logger.LogInformation($"[{DateTime.Now}] LOG: Requesting PUT api/user/{id}");

                if (user == null)
                {
                    _logger.LogError($"[{DateTime.Now}] ERROR: User object sent from client is null.");
                    return StatusCode(StatusCodes.Status400BadRequest, new 
                    { 
                        message = $"Request data sent from client is null." 
                    });
                }
                else if (!ModelState.IsValid)
                {
                    _logger.LogError($"[{DateTime.Now}] ERROR: Invalid User object sent from client.");
                    return StatusCode(StatusCodes.Status400BadRequest, new
                    {
                        message = $"Invalid User object sent from client."
                    });
                }

                var userModel = _repository.User.GetUserById(id);

                if(userModel == null)
                {
                    _logger.LogError($"[{DateTime.Now}] ERROR: User with ID {id} was not found.");
                    return StatusCode(StatusCodes.Status404NotFound, new 
                    { 
                        message = $"User was not found." 
                    });
                }

                _mapper.Map(user, userModel);
                
                userModel.ModifiedDate = DateTime.Now;

                _repository.User.UpdateUser(userModel);
                _repository.Save();

                _logger.LogInformation($"[{DateTime.Now}] LOG: User with ID {userModel.UserId} updated.");                
                return StatusCode(StatusCodes.Status201Created, new
                {
                    body = userModel,
                    message = $"Data from User was updated successfully."
                });
            }
            catch (Exception e)
            {
                _logger.LogError($"[{DateTime.Now}] ERROR: {e}");
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    message = "A error has ocurred in the service."
                });
            }
        }

        /// <summary>
        /// Register a User on the database
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        /// <response code="201">User was registered successfully</response>
        /// <response code="400">Request data body is null</response>
        /// <response code="400">Email already exists in the database</response>
        // POST: api/Users        
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(UserDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = null)]
        [HttpPost]
        public IActionResult CreateUser([FromBody]UserCreationDto user)
        {
            try
            {
                _logger.LogInformation($"[{DateTime.Now}] LOG: Requesting POST api/user");                

                if(user == null)
                {
                    _logger.LogError($"[{DateTime.Now}] ERROR: User object sent from client is null.");
                    return StatusCode(StatusCodes.Status400BadRequest, new
                    {
                        message = $"User object sent from client is null."
                    });
                }

                var isEmailOnDatabase = _repository.User.FindByCondition(userDb => userDb.Email.ToLower() == user.Email.ToLower()).FirstOrDefault();
                
                if (isEmailOnDatabase != null)
                {
                    _logger.LogError($"[{DateTime.Now}] ERROR: Email already exists on database, choose another Email.");
                    return StatusCode(StatusCodes.Status400BadRequest, new
                    {                        
                        message = $"Email already exists on database, choose another Email."
                    });
                }

                var userModel = _mapper.Map<User>(user);
                // Default values on User creation
                userModel.CreatedDate = DateTime.Now;
                userModel.ModifiedDate = DateTime.Now;
                userModel.Slug = userModel.FullName.Replace(" ", "-").ToLower();

                _repository.User.CreateUser(userModel);
                _repository.Save();

                _logger.LogInformation($"[{DateTime.Now}] LOG: User was created.");                
                return StatusCode(StatusCodes.Status201Created, new
                {
                    body = userModel,
                    message = $"User was created."
                });            
            }
            catch (Exception e)
            {
                _logger.LogError($"[{DateTime.Now}] ERROR: {e}");
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    message = "A error has ocurred in the service."
                });
            }
        }

        /// <summary>
        /// Remove a User from the database by using it's ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <response code="204">User was removed from database</response>
        /// <response code="404">User ID was not found in the database</response>
        // DELETE: api/Users/{id}
        [ProducesResponseType(StatusCodes.Status204NoContent, Type = null)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = null)]
        [HttpDelete("{id}")]
        public IActionResult DeleteUser(int id)
        {
            try
            {
                _logger.LogInformation($"[{DateTime.Now}] LOG: Requesting DELETE api/user/{id}");
                var user = _repository.User.GetUserById(id);
                if(user == null)
                {
                    _logger.LogError($"[{DateTime.Now}] ERROR: User with ID {id} was not found.");
                    return StatusCode(StatusCodes.Status404NotFound, new
                    {
                        message = $"User was not found."
                    });
                }

                _repository.User.DeleteUser(user);
                _repository.Save();
                return StatusCode(StatusCodes.Status204NoContent, new
                {
                    message = $"User was removed from database."
                });
            }
            catch (Exception e)
            {
                _logger.LogError($"[{DateTime.Now}] ERROR: {e}");
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    message = "A error has ocurred in the service."
                });
            }
        }

        /// <summary>
        /// Method that authenticate the User credentials in the system
        /// </summary>
        /// <param name="login"></param>
        /// <returns></returns>
        /// <response code="200">User is authenticated</response>
        /// <response code="400">Request data body cannot be null</response>
        /// <response code="404">Invalid Email / Password was sent</response>
        // POST: api/Users/authentication
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = null)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = null)]
        [HttpPost("authentication")]
        public IActionResult UserAuthentication([FromBody] UserAuthenticationDto login)
        {
            try
            {
                _logger.LogInformation($"[{DateTime.Now}] LOG: Requesting POST api/user/authenticate");

                if (login == null)
                {
                    _logger.LogError($"[{DateTime.Now}] ERROR: Email or Password cannot be null.");
                    return StatusCode(StatusCodes.Status400BadRequest, new
                    {
                        message = "Email or Password cannot be null."
                    });
                }
                else if (!ModelState.IsValid)
                {
                    _logger.LogError($"[{DateTime.Now}] ERROR: Invalid data sent from client.");
                    return StatusCode(StatusCodes.Status400BadRequest, new
                    {
                        message = "Invalid data sent from client."
                    });
                }

                var email = login.Email;
                var password = login.Password;

                var user = _repository.User.UserAuthentication(email, password);

                if (user == null)
                {
                    _logger.LogError($"[{DateTime.Now}] ERROR: Invalid Email / Password was sent.");
                    return StatusCode(StatusCodes.Status404NotFound, new
                    {
                        message = "Invalid Email / Password was sent."
                    });
                }

                _logger.LogInformation($"[{DateTime.Now}] LOG: User with email {email} is authenticated.");
                var userResult = _mapper.Map<UserDto>(user);                
                return StatusCode(StatusCodes.Status200OK, new
                {
                    body = userResult,
                    message = $"User is authenticated."
                });
            }
            catch (Exception e)
            {
                _logger.LogError($"[{DateTime.Now}] ERROR: {e}");
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    message = "A error has ocurred in the service."
                });
            }
        }

        /// <summary>
        /// User password recovery method
        /// </summary>
        /// <param name="login"></param>
        /// <returns></returns>
        /// <response code="200">User password was changed sucessfully</response>
        /// <response code="400">Email or Password cannot be null</response>
        /// <response code="404">No User was found using this Email</response>
        [ProducesResponseType(StatusCodes.Status200OK, Type = null)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = null)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = null)]
        [HttpPut("reset-password")]
        public IActionResult ResetPassword([FromBody] UserAuthenticationDto login)
        {
            try
            {
                _logger.LogInformation($"[{DateTime.Now}] LOG: Requesting PUT api/user/reset-password");

                if (login == null)
                {
                    _logger.LogError($"[{DateTime.Now}] ERROR: Email or Password cannot be null.");
                    return StatusCode(StatusCodes.Status400BadRequest, new
                    {
                        message = "Email or Password cannot be null."
                    });
                }
                else if (!ModelState.IsValid)
                {
                    _logger.LogError($"[{DateTime.Now}] ERROR: Invalid data sent from client.");
                    return StatusCode(StatusCodes.Status400BadRequest, new
                    {
                        message = "Invalid data sent from client."
                    });
                }

                var email = login.Email;
                var password = login.Password;

                if (_repository.User.ResetPassword(email, password) == false)
                {
                    _logger.LogError($"[{DateTime.Now}] ERROR: No User was found using this Email.");
                    return StatusCode(StatusCodes.Status404NotFound, new
                    {
                        message = "No User was found using this Email."
                    });
                }

                _repository.Save();
                _logger.LogInformation($"[{DateTime.Now}] LOG: User password was changed sucessfully");
                return StatusCode(StatusCodes.Status200OK, new
                {
                    message = "User password was changed sucessfully."
                });
            }
            catch (Exception e)
            {
                _logger.LogError($"[{DateTime.Now}] ERROR: {e}");
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    message = "A error has ocurred in the service."
                });
            }
        }

        /// <summary>
        /// Method to register when a User has followed a specific Community
        /// </summary>
        /// <param name="userCommunity"></param>
        /// <returns></returns>
        /// <response code="201">User was added to this community</response>
        /// <response code="400">Request data body is null</response>
        /// <response code="400">User is already on this community</response>
        [ProducesResponseType(StatusCodes.Status201Created, Type = null)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = null)]
        [HttpPost("community")]
        public IActionResult AddUserToCommunity(UserCommunity userCommunity)
        {
            try
            {
                if (userCommunity == null)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, new
                    {
                        message = "Data sent from client cannot be null."
                    });
                }

                // Checking if user is already on this community
                var isUserOnCommunity = _repository.UserCommunity.FindByCondition(user => user.CommunityId.Equals(userCommunity.CommunityId) && user.UserId.Equals(userCommunity.UserId)).FirstOrDefault();

                if (isUserOnCommunity != null)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, new
                    {
                        message = "User is already on this community"
                    });
                }

                _repository.UserCommunity.Create(userCommunity);
                _repository.Save();

                return StatusCode(StatusCodes.Status201Created, new
                {                    
                    message = "User was added to this community."
                });
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    message = "A error has ocurred in the service."
                });
            }
        }

        /// <summary>
        /// Returns all Communities a specific User has followed
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <response code="200">Returned all communities of this user</response>
        /// <response code="204">This user has no communities followed</response>
        /// <response code="400">User ID was not found on the database</response>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CommunityDto))]
        [ProducesResponseType(StatusCodes.Status204NoContent, Type = null)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = null)]
        [HttpGet("{id}/communities")]
        public IActionResult GetCommunitiesByUser(int id)
        {           
            var isUserOnDatabase = _repository.User.FindByCondition(user => user.UserId.Equals(id)).FirstOrDefault();

            if(isUserOnDatabase == null)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new
                {
                    message = "Cannot find a user with this ID on the database."
                });
            }

            var communities = _repository.UserCommunity.GetCommunitiesByUser(id);

            if(communities.Count() == 0)
            {
                return StatusCode(StatusCodes.Status204NoContent, new
                {                    
                    message = "This user has no communities followed."
                });
            }

            var communitiesDetails = new List<Community>();

            foreach(var index in communities)
            {
                communitiesDetails.Add(_repository.Community.GetCommunityById(index.CommunityId));
            }

            var communityResult = _mapper.Map<IEnumerable<CommunityDto>>(communitiesDetails);

            var categoriesList = _repository.Category.GetAllCategories(); // Saving all categories in a variable so we don't need to keep checking database          

            // Retrieving the category name for each community on the database
            foreach (var index in communityResult)
            {
                var categoryId = communitiesDetails.Where(community => community.CommunityId == index.CommunityId).Select(community => community.CategoryId).FirstOrDefault();
                index.CategoryName = Convert.ToString(categoriesList.Where(category => category.CategoryId.Equals(categoryId)).Select(category => category.Name).FirstOrDefault());
            }

            return StatusCode(StatusCodes.Status200OK, new
            {
                body = communityResult,
                message = "Returned all communities of this user."
            });
        }

        /// <summary>
        /// Method to change User role in a specific community
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userCommunityUpdate"></param>
        /// <returns></returns>
        /// <response code="204">User role updated sucessfully</response>
        /// <response code="400">Request data body is null</response>
        /// <response code="404">User ID was not found in the database</response>
        /// <response code="404">This user has no communities related to his ID</response>
        [ProducesResponseType(StatusCodes.Status204NoContent, Type = null)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = null)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = null)]
        [HttpPut("{id}/communities")]
        public IActionResult UpdateUserRoleOnCommunity(int id, [FromBody] UserCommunityUpdateDto userCommunityUpdate)
        {
            if (userCommunityUpdate == null)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new
                {
                    message = "Data sent from client cannot be null."
                });
            }

            var isUserOnDatabase = _repository.User.FindByCondition(user => user.UserId.Equals(id)).FirstOrDefault();

            if (isUserOnDatabase == null)
            {
                return StatusCode(StatusCodes.Status404NotFound, new
                {
                    body = isUserOnDatabase,
                    message = "Cannot find a user with this ID on the database."
                });
            }

            var userCommunitiesList = _repository.UserCommunity.FindByCondition(user => user.UserId.Equals(id)).FirstOrDefault();

            if(userCommunitiesList == null)
            {
                return StatusCode(StatusCodes.Status404NotFound, new
                {
                    body = userCommunitiesList,
                    message = "This user has no communities related to his ID."
                });
            }

            var userCommunity = _repository.UserCommunity.FindByCondition(user => user.UserId.Equals(id) && user.CommunityId.Equals(userCommunityUpdate.CommunityId)).FirstOrDefault();

            _mapper.Map(userCommunityUpdate, userCommunity);            
            _repository.UserCommunity.Update(userCommunity);
            _repository.Save();

            return StatusCode(StatusCodes.Status204NoContent, new
            {                
                message = "User role updated sucessfully"
            });
        }

        // TODO REALIZAR TESTES EM TODOS NOVOS MÉTODOS

        //[HttpDelete("{id}/community")]
        //public IActionResult RemoveUserFromCommunity
    }
}
