using Microsoft.AspNetCore.Mvc;
using Fandom_Project.Models;
using Fandom_Project.Repository.Interfaces;
using AutoMapper;
using Fandom_Project.Models.DataTransferObjects.UserModel;
using Fandom_Project.Models.DataTransferObjects.UserCommunityModel;
using Fandom_Project.Models.DataTransferObjects.CommunityModel;
using System.Linq;
using Fandom_Project.Models.DataTransferObjects.PostModel;

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
        public IActionResult CreateUser([FromBody] UserCreationDto user)
        {
            try
            {               
                if(user == null)
                {                    
                    return StatusCode(StatusCodes.Status400BadRequest, new
                    {
                        message = $"User object sent from client is null."
                    });
                }                
                
                if (_repository.User.FindByCondition(userDb => userDb.Email.ToLower() == user.Email.ToLower()).FirstOrDefault() != null)
                {                    
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
                userModel.Slug = userModel.Slug.Replace(".", "");

                _repository.User.CreateUser(userModel);
                _repository.Save();
                            
                return StatusCode(StatusCodes.Status201Created, new
                {
                    body = userModel,
                    message = $"User was created."
                });            
            }
            catch (Exception e)
            {                
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
                if (login == null)
                {                    
                    return StatusCode(StatusCodes.Status400BadRequest, new
                    {
                        message = "Email or Password cannot be null."
                    });
                }                

                var email = login.Email;
                var password = login.Password;

                var user = _repository.User.UserAuthentication(email, password);

                if (user == null)
                {                    
                    return StatusCode(StatusCodes.Status404NotFound, new
                    {
                        message = "Invalid Email / Password was sent."
                    });
                }
                
                var userResult = _mapper.Map<UserDto>(user);                
                return StatusCode(StatusCodes.Status200OK, new
                {
                    body = userResult,
                    message = $"User is authenticated."
                });
            }
            catch (Exception e)
            {                
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
        /// Returns all Communities a specific User has followed
        /// </summary>
        /// <remarks>
        /// Examples of returnType:
        /// - <b>follower</b> = All communities that the user is registered into it with full details
        /// - <b>follower-simple</b> = All communities that the user is registered into it with only the Community IDs
        /// - <b>owner</b> = All communities that the user has role = 'Owner', with full details
        /// - <b>owner-simple</b> = All communities that the user has role = 'Owner', with only the Community IDs
        /// </remarks>
        /// <param name="id"></param>
        /// <param name="returnType"></param>
        /// <returns></returns>
        /// <response code="200">Returned all communities of this user</response>        
        /// <response code="400">User ID was not found on the database</response>
        /// <response code="404">This user has no communities followed</response>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CommunityDto))]        
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = null)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = null)]
        // GET: api/Users/{id}/communities?returnType={returnType}
        [HttpGet("{id}/communities")]
        public IActionResult GetCommunitiesByUser(int id, [FromQuery] string returnType)
        {
            returnType = returnType.ToLower(); // Validation so the key word is always on the same pattern

            if(_repository.User.FindByCondition(user => user.UserId.Equals(id)).FirstOrDefault() == null)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new
                {
                    message = "Cannot find a user with this ID on the database."
                });
            }            

            // Saving all information in multiple variables so we don't need to do multiple requests to the database  
            IEnumerable<Category> categoriesList = _repository.Category.GetAllCategories();
            IEnumerable<User> usersList = _repository.User.GetAllUsers();
            IEnumerable<UserCommunity> communities = _repository.UserCommunity.GetAllUserCommunities(); // All data about Users following Communities

            IEnumerable<UserCommunity> communitiesFollowed = communities.Where(userCommunity => userCommunity.UserId.Equals(id)).ToList();

            if(communitiesFollowed.Count() == 0)
            {
                return StatusCode(StatusCodes.Status200OK, new
                {                    
                    message = "This user has no communities followed."
                });
            }            

            var communitiesDetails = new List<Community>(); // Temporary list that will hold all the response information before we convert using _mapper      
            
            if (returnType == "follower-simple" || returnType == "follower")
            {
                // TODO: This need to change to avoid multiple requests to the Database
                foreach (var index in communitiesFollowed)
                {
                    communitiesDetails.Add(_repository.Community.GetCommunityById(index.CommunityId));
                }

                if(returnType == "follower-simple")
                {
                    List<CommunitySimpleDto> communitySimpleResult = _mapper.Map<List<CommunitySimpleDto>>(communitiesDetails);

                    return StatusCode(StatusCodes.Status200OK, new
                    {
                        body = communitySimpleResult,
                        message = "Returned all communities of this user."
                    });
                }

                var communityResult = _mapper.Map<IEnumerable<CommunityDto>>(communitiesDetails);
                List<int> communitiesIdList = communities.Select(userCommunity => userCommunity.CommunityId).ToList(); // A list of all CommunityId related to what the User follows, so we can track it's owner                

                // Including the rest of the information to complete the response model
                foreach (var index in communityResult)
                {                    
                    int categoryId = communitiesDetails.Where(community => community.CommunityId == index.CommunityId)
                                                       .Select(community => community.CategoryId)
                                                       .FirstOrDefault();
                    index.CategoryName = categoriesList.Where(category => category.CategoryId.Equals(categoryId))
                                                       .Select(category => category.Name)
                                                       .FirstOrDefault();                    
                    var ownerInformation = usersList.Join(communities, user => user.UserId, userCommunity => userCommunity.UserId, (user, userCommunity) => new { User = user, UserCommunity = userCommunity })
                                                    .Where(filter => filter.UserCommunity.CommunityId.Equals(index.CommunityId) && filter.UserCommunity.Role.Equals("Owner"))
                                                    .Select(user => new { user.User.FullName, user.User.UserId })
                                                    .FirstOrDefault();
                    index.OwnerName = ownerInformation.FullName;
                    index.OwnerId = ownerInformation.UserId;
                }

                return StatusCode(StatusCodes.Status200OK, new
                {
                    body = communityResult,
                    message = "Returned all communities of this user."
                });
            }

            // In case returnType is "owner" or "owner-simple"
            else
            {
                foreach (var index in communitiesFollowed)
                {
                    if (index.Role == "Owner")
                    {
                        communitiesDetails.Add(_repository.Community.GetCommunityById(index.CommunityId));
                    }
                }

                if(communitiesDetails.Count() == 0)
                {
                    return StatusCode(StatusCodes.Status200OK, new
                    {
                        message = "This user does not own a community."
                    });
                }

                if (returnType == "owner-simple")
                {
                    List<CommunitySimpleDto> communitySimpleResult = _mapper.Map<List<CommunitySimpleDto>>(communitiesDetails);

                    return StatusCode(StatusCodes.Status200OK, new
                    {
                        body = communitySimpleResult,
                        message = "Returned all communities that this User is owner."
                    });
                }
                
                var communityResult = _mapper.Map<IEnumerable<CommunityDto>>(communitiesDetails);

                // Retrieving the category name for each community on the database
                foreach (var index2 in communityResult)
                {
                    var categoryId = communitiesDetails.Where(community => community.CommunityId == index2.CommunityId)
                                                       .Select(community => community.CategoryId)
                                                       .FirstOrDefault();
                    index2.CategoryName = categoriesList.Where(category => category.CategoryId.Equals(categoryId))
                                                        .Select(category => category.Name)
                                                        .FirstOrDefault();
                    var ownerInformation = usersList.Where(user => user.UserId.Equals(id))
                                                    .Select(user => new { user.FullName, user.UserId })
                                                    .FirstOrDefault();
                    index2.OwnerName = ownerInformation.FullName;
                    index2.OwnerId = ownerInformation.UserId;
                }

                return StatusCode(StatusCodes.Status200OK, new
                {
                    body = communityResult,
                    message = "Returned all communities that this User is owner."
                });                
            }            
        }

        /// <summary>
        /// Method to change User role in a specific community
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userCommunityUpdate"></param>
        /// <returns></returns>
        /// <response code="200">User role updated sucessfully</response>
        /// <response code="400">Request data body is null</response>
        /// <response code="404">User ID was not found in the database</response>
        /// <response code="404">This user has no communities related to his ID</response>
        [ProducesResponseType(StatusCodes.Status200OK, Type = null)]
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
                    message = "Cannot find a user with this ID on the database."
                });
            }         

            var userCommunity = _repository.UserCommunity.FindByCondition(user => user.UserId.Equals(id) && user.CommunityId.Equals(userCommunityUpdate.CommunityId)).FirstOrDefault();

            if (userCommunity == null)
            {
                return StatusCode(StatusCodes.Status404NotFound, new
                {
                    message = "The User is not registered into this Community"
                });
            }

            _mapper.Map(userCommunityUpdate, userCommunity);            
            _repository.UserCommunity.Update(userCommunity);
            _repository.Save();

            return StatusCode(StatusCodes.Status200OK, new
            {                
                message = "User role updated sucessfully"
            });
        }

        /// <summary>
        /// Get all Posts related to each Community that the User is follower
        /// </summary>
        /// <param name="id"></param>
        /// <response code="200">Successfully returned all posts related to communities the user has followed</response>
        /// <response code="200">User with this ID was not found on the database</response>
        /// <response code="200">All followed communities don't have any posts registered</response>
        /// <response code="400">User ID was not found on the database</response>
        /// <returns></returns>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<PostFollowDto>))]
        [ProducesResponseType(StatusCodes.Status200OK, Type = null)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = null)]
        [HttpGet("{id}/feed")]
        public IActionResult GetPostsFromCommunitiesFollowed(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, new
                    {
                        message = "Invalid User ID was sent from client"
                    });
                }

                if(_repository.User.GetUserById(id) == null)
                {
                    return StatusCode(StatusCodes.Status200OK, new
                    {
                        message = "User with this ID was not found on the database"
                    });
                }

                // Saving all CommunityId that the User is 'Follower', so we can search all Posts related to each Community
                List<UserCommunity> communitiesFollowed = _repository.UserCommunity.FindByCondition(userCommunity => userCommunity.UserId.Equals(id) && userCommunity.Role.Equals("Follower"))
                                                                                   .ToList();

                if (communitiesFollowed.Count() == 0)
                {
                    return StatusCode(StatusCodes.Status200OK, new
                    {
                        message = "All followed communities don't have any posts registered"
                    });
                }

                IEnumerable<Post> postsList = _repository.Post.FindAll();
                List<PostFollowDto> postsCommunityFollowed = new List<PostFollowDto>();

                foreach (var index in communitiesFollowed)
                {                    
                    List<Post> tempPostList = postsList.Where(post => post.CommunityId.Equals(index.CommunityId))                                                       
                                                       .OrderByDescending(post => post.CreatedDate)
                                                       .ToList();  
                    
                    List<PostFollowDto> postDtos = _mapper.Map<List<Post>, List<PostFollowDto>>(tempPostList);
                    postsCommunityFollowed.AddRange(postDtos);
                }

                IEnumerable<User> usersList = _repository.User.GetAllUsers();
                IEnumerable<Community> communitiesList = _repository.Community.GetAllCommunities();

                foreach (var index in postsCommunityFollowed)
                {                    
                    index.AuthorName = usersList.Where(user => user.UserId.Equals(index.UserId))
                                                .Select(user => user.FullName)
                                                .FirstOrDefault();
                    index.CommunityCoverImageUrl = communitiesList.Where(community => community.CommunityId.Equals(index.CommunityId))
                                                                  .Select(community => community.CoverImage)
                                                                  .FirstOrDefault();
                    index.CommunityName = communitiesList.Where(community => community.CommunityId.Equals(index.CommunityId))
                                                                  .Select(community => community.Name)
                                                                  .FirstOrDefault();
                }                

                return StatusCode(StatusCodes.Status200OK, new
                {
                    body = postsCommunityFollowed,
                    message = "Successfully returned all posts related to communities the user has followed"
                });
                
            }
            catch (Exception e)
            {                
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    message = $"A error has ocurred in the service.\n{e}"
                });
            }
        }
    }
}
