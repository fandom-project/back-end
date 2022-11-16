using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Fandom_Project.Data;
using Fandom_Project.Models;
using Fandom_Project.Repository.Interfaces;
using AutoMapper;
using Fandom_Project.Models.DataTransferObjects.CommunityModel;
using System.Collections;
using Fandom_Project.Models.DataTransferObjects.UserModel;
using Fandom_Project.Models.DataTransferObjects.UserCommunityModel;
using Fandom_Project.Models.DataTransferObjects.PostModel;

namespace Fandom_Project.Controllers
{
    [Route("api/communities")]
    [ApiController]
    public class CommunitiesController : ControllerBase
    {
        private readonly IRepositoryWrapper _repository;
        private readonly ILogger _logger;
        private readonly IMapper _mapper;

        public CommunitiesController(IRepositoryWrapper repository, ILogger<UsersController> logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }

        /// <summary>
        /// Returns all Communities added in the database
        /// </summary>
        /// <returns></returns>
        /// <response code="404">There's no Community registered in the database</response>
        /// <response code="200">Returned all Communities from the database</response>
        // GET: api/Communities
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CommunityDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = null)]
        [HttpGet]
        public IActionResult GetAllCommunities()
        {
            try
            {

                // Saving all information in multiple variables so we don't need to do multiple requests to the database  
                IEnumerable<Community> communities = _repository.Community.GetAllCommunities();
                IEnumerable<UserCommunity> userCommunities = _repository.UserCommunity.GetAllUserCommunities(); // All data about Users following Communities
                IEnumerable<User> usersList = _repository.User.GetAllUsers();

                if (communities.Count() == 0)
                {
                    _logger.LogInformation($"[{DateTime.Now}] LOG: No Community was found.");
                    return StatusCode(StatusCodes.Status404NotFound, new
                    {
                        message = "No Community was found in the database."
                    });
                }

                _logger.LogInformation($"[{DateTime.Now}] LOG: Returned all Communities from the database.");
                
                var communityResult = _mapper.Map<IEnumerable<CommunityDto>>(communities);                

                var categoriesList = _repository.Category.GetAllCategories(); // Saving all categories in a variable so we don't need to keep checking database          

                // Retrieving the category name for each community on the database
                foreach (var index in communityResult)
                {
                    var categoryId = communities.Where(community => community.CommunityId == index.CommunityId)
                                                .Select(community => community.CategoryId)
                                                .FirstOrDefault();                    
                    index.CategoryName = categoriesList.Where(category => category.CategoryId.Equals(categoryId))
                                                       .Select(category => category.Name)
                                                       .FirstOrDefault();
                    var ownerInformation = usersList.Join(userCommunities, user => user.UserId, userCommunity => userCommunity.UserId, (user, userCommunity) => new { User = user, UserCommunity = userCommunity })
                                                    .Where(filter => filter.UserCommunity.CommunityId.Equals(index.CommunityId) && filter.UserCommunity.Role.Equals("Owner"))
                                                    .Select(user => new { user.User.FullName, user.User.UserId })
                                                    .FirstOrDefault();
                    index.OwnerName = ownerInformation.FullName;
                    index.OwnerId = ownerInformation.UserId;
                }               

                return StatusCode(StatusCodes.Status200OK, new
                {
                    body = communityResult,
                    message = "Returned all Communities from the database."
                });
            }
            catch (Exception e)
            {
                _logger.LogError($"[{DateTime.Now}] ERROR: {e}");
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    message = "A error has ocurred in the service."+$"\n{e}
                });
            }
        }

        /// <summary>
        /// Return a specific Community by using it's ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <response code="200">Returned selected Community from the database</response>
        /// <response code="404">Community was not found</response>
        // GET: api/Communities/{id}
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CommunityDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = null)]        
        [HttpGet("{id}")]
        public IActionResult GetCommunityById(int id)
        {
            try
            {
                _logger.LogInformation($"[{DateTime.Now}] LOG: Requesting GET api/communities/{id}");
                var community = _repository.Community.GetCommunityById(id);              

                if (community == null)
                {
                    _logger.LogInformation($"[{DateTime.Now}] LOG: Community with ID {id} was not found.");
                    return StatusCode(StatusCodes.Status404NotFound, new
                    {
                        message = $"Community was not found."
                    });
                }                
                
                CommunityDto communityResult = _mapper.Map<CommunityDto>(community);
                
                // Adding information left to complete the response model
                Category categoryName = _repository.Category.FindByCondition(category => category.CategoryId == community.CategoryId).FirstOrDefault(); // Returning the Category name instead of the ID
                communityResult.CategoryName = categoryName.Name;
                
                UserCommunity userCommunities = _repository.UserCommunity.FindByCondition(userCommunity => userCommunity.CommunityId.Equals(id) && userCommunity.Role.Equals("Owner")).FirstOrDefault();
                User communityOwner = _repository.User.GetUserById(userCommunities.UserId);
                communityResult.OwnerName = communityOwner.FullName;
                communityResult.OwnerId = communityOwner.UserId;                
                
                return StatusCode(StatusCodes.Status200OK, new
                {
                    body = communityResult,
                    message = "Returned selected Community from the database."
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
        /// Update data from a specific Community
        /// </summary>
        /// <param name="id"></param>
        /// <param name="community"></param>
        /// <returns></returns>
        /// <response code="200">Data from Community was updated successfully</response>
        /// <response code="400">Request data sent from client is null</response>
        /// <response code="404">Community was not found</response>
        // PUT: api/Communities/{id}        
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CommunityDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = null)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = null)]        
        [HttpPut("{id}")]
        public IActionResult UpdateCommunity(int id, [FromBody] CommunityUpdateDto community)
        {
            try
            {
                _logger.LogInformation($"[{DateTime.Now}] LOG: Requesting PUT api/communities/{id}");

                if (community == null)
                {
                    _logger.LogError($"[{DateTime.Now}] ERROR: Community object sent from client is null.");
                    return StatusCode(StatusCodes.Status400BadRequest, new
                    {
                        message = $"Request data sent from client is null."
                    });
                }
                else if (!ModelState.IsValid)
                {
                    _logger.LogError($"[{DateTime.Now}] ERROR: Invalid Community object sent from client.");
                    return StatusCode(StatusCodes.Status400BadRequest, new
                    {
                        message = $"Invalid Community object sent from client."
                    });
                }

                var communityModel = _repository.Community.GetCommunityById(id);

                if (communityModel == null)
                {
                    _logger.LogError($"[{DateTime.Now}] ERROR: Community with ID {id} was not found.");
                    return StatusCode(StatusCodes.Status404NotFound, new
                    {
                        message = $"Community was not found."
                    });
                }

                // Checking if category was updated so we need update CommunityCount on the old and new category
                var oldCategory = new Category();
                var newCategory = new Category();

                if (communityModel.CategoryId != community.CategoryId)
                {
                    oldCategory = _repository.Category.FindByCondition(category => category.CategoryId.Equals(communityModel.CategoryId)).FirstOrDefault();
                    newCategory = _repository.Category.FindByCondition(category => category.CategoryId.Equals(community.CategoryId)).FirstOrDefault();

                    oldCategory.CommunityCount -= 1;
                    newCategory.CommunityCount += 1;
                }

                _mapper.Map(community, communityModel);
                communityModel.ModifiedDate = DateTime.Now; // Updating to the DateTime of request          
                communityModel.Slug = communityModel.Name.Replace(" ", "-").ToLower();

                _repository.Category.UpdateCategory(oldCategory);
                _repository.Category.UpdateCategory(newCategory);
                _repository.Community.UpdateCommunity(communityModel);
                _repository.Save();

                _logger.LogInformation($"[{DateTime.Now}] LOG: Community with ID {communityModel.CommunityId} updated.");
                return StatusCode(StatusCodes.Status200OK, new
                {
                    body = communityModel,
                    message = $"Data from Community was updated successfully."
                });
            }
            catch (Exception e)
            {
                _logger.LogError($"[{DateTime.Now}] ERROR: {e}");
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    message = $"A error has ocurred in the service..\nERROR: {e}"
                });
            }
        }

        /// <summary>
        /// Register a Community on the database
        /// </summary>
        /// <param name="community"></param>
        /// <returns></returns>
        /// <response code="201">Community was created</response>
        /// <response code="400">Request data sent from client is null</response>
        /// <response code="400">Community already exists on database</response>
        // POST: api/Communities        
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(CommunityDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = null)]
        [HttpPost]
        public IActionResult CreateCommunity([FromBody] CommunityCreationDto community)
        {
            using (var dbContextTransaction = _repository.BeginTransaction())
            {
                try
                {
                    _logger.LogInformation($"[{DateTime.Now}] LOG: Requesting POST api/communities");

                    if (community == null)
                    {
                        _logger.LogError($"[{DateTime.Now}] ERROR: Community object sent from client is null.");
                        return StatusCode(StatusCodes.Status400BadRequest, new
                        {
                            message = $"Community object sent from client is null."
                        });
                    }

                    var isCommunityOnDatabase = _repository.Community.FindByCondition(communityDb => communityDb.Name.ToLower() == community.Name.ToLower()).FirstOrDefault();

                    if (isCommunityOnDatabase != null)
                    {
                        _logger.LogError($"[{DateTime.Now}] ERROR: Community already exists on database, choose another name.");
                        return StatusCode(StatusCodes.Status400BadRequest, new
                        {
                            message = $"Community already exists on database, choose another name."
                        });
                    }

                    var communityModel = _mapper.Map<Community>(community);

                    // Default values on User creation
                    communityModel.CreatedDate = DateTime.Now;
                    communityModel.ModifiedDate = DateTime.Now;
                    communityModel.Slug = communityModel.Name.Replace(" ", "-").ToLower();
                    communityModel.MemberCount = 0;

                    // Adding +1 to Category counter
                    var categoryUpdate = _repository.Category.FindByCondition(category => category.CategoryId == community.CategoryId).FirstOrDefault();
                    categoryUpdate.CommunityCount += 1;                    

                    _repository.Community.CreateCommunity(communityModel);
                    _repository.Category.UpdateCategory(categoryUpdate);
                    _repository.Save();

                    // Adding User to the created Community as Owner
                    UserCommunity userCommunity = new UserCommunity();

                    userCommunity.CommunityId = communityModel.CommunityId; // We need to extract the value after CreateCommunity() and Save()                    
                    userCommunity.UserId = community.UserId;
                    userCommunity.Role = "Owner";

                    _repository.UserCommunity.AddUserToCommunity(userCommunity);
                    _repository.Save();

                    dbContextTransaction.Commit();

                    _logger.LogInformation($"[{DateTime.Now}] LOG: Community was created.");
                    return StatusCode(StatusCodes.Status201Created, new
                    {                        
                        message = $"Community was created."
                    });
                }
                catch (Exception e)
                {
                    dbContextTransaction.Rollback();
                    _logger.LogError($"[{DateTime.Now}] ERROR: {e}");
                    return StatusCode(StatusCodes.Status500InternalServerError, new
                    {
                        message = "A error has ocurred in the service."
                    });
                }
            }
        }

        /// <summary>
        /// Remove a Community from the database by using it's ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <response code="200">Community was removed from database</response>
        /// <response code="404">Community was not found</response>
        // DELETE: api/Communities/{id}
        [ProducesResponseType(StatusCodes.Status200OK, Type = null)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = null)]
        [HttpDelete("{id}")]
        public IActionResult DeleteCommunity(int id)
        {
            try
            {
                _logger.LogInformation($"[{DateTime.Now}] LOG: Requesting DELETE api/communities/{id}");
                var community = _repository.Community.GetCommunityById(id);
                if (community == null)
                {
                    _logger.LogError($"[{DateTime.Now}] ERROR: Community with ID {id} was not found.");
                    return StatusCode(StatusCodes.Status404NotFound, new
                    {
                        message = $"Community was not found."
                    });
                }

                // Adding -1 to Category counter
                var categoryUpdate = _repository.Category.FindByCondition(category => category.CategoryId == community.CategoryId).FirstOrDefault();
                categoryUpdate.CommunityCount -= 1;

                _repository.Community.DeleteCommunity(community);
                _repository.Category.UpdateCategory(categoryUpdate);
                _repository.Save();
                return StatusCode(StatusCodes.Status200OK, new
                {
                    message = $"Community was removed from database."
                });
            }
            catch (Exception e)
            {
                _logger.LogError($"[{DateTime.Now}] ERROR: {e}");
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    message = $"A error has ocurred in the service.\nERROR: {e}"
                });
            }
        }

        /// <summary>
        /// Returns all Users that follows a particular Community
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <response code="200">Successfully returned all users from this community</response>
        /// <response code="204">There's no Users in this Community</response>
        /// <response code="400">Invalid Community ID</response>
        /// <response code="404">Community not found with this ID</response>
        [ProducesResponseType(StatusCodes.Status200OK, Type = null)]
        [ProducesResponseType(StatusCodes.Status204NoContent, Type = null)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = null)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = null)]
        [HttpGet("{id}/users")]
        public IActionResult GetUsersByCommunity(int id)
        {
            try
            {
                if(id <= 0)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, new
                    {
                        message = "Invalid community ID"
                    });
                }

                var users = _repository.UserCommunity.GetUsersByCommunity(id);

                if(users == null)
                {
                    return StatusCode(StatusCodes.Status404NotFound, new
                    {
                        body = users,
                        message = "Community not found with this ID."
                    });
                }
                else if(users.Count() == 0)
                {
                    return StatusCode(StatusCodes.Status204NoContent, new
                    {
                        body = users,
                        message = "There's no users in this community."
                    });
                }

                var communitiesDetails = new List<UserDto>();

                foreach(var index in users)
                {
                    var tempUser = _repository.User.FindByCondition(user => user.UserId.Equals(index.UserId)).FirstOrDefault();
                    communitiesDetails.Add(_mapper.Map<User, UserDto>(tempUser));
                }

                return StatusCode(StatusCodes.Status200OK, new
                {
                    body = communitiesDetails,
                    message = "Successfully returned all users from this community."
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
        /// Return a specific Community by using it's slug
        /// </summary>        
        /// <returns></returns>
        /// <response code="200">Returned selected Community from the database</response>
        /// <response code="400">Slug query is empty</response>
        /// <response code="404">Community was not found</response>
        // GET: api/Communities/search?slug={slug}
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CommunityDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = null)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = null)]
        [HttpGet("search")]
        public IActionResult GetCommunityBySlug([FromQuery] string slug)
        {
            try
            {
                var community = _repository.Community.GetCommunityBySlug(slug.ToLower());

                if (community == null)
                {
                    return StatusCode(StatusCodes.Status404NotFound, new
                    {
                        message = $"Community was not found."
                    });
                }

                var categoryName = _repository.Category.FindByCondition(category => category.CategoryId == community.CategoryId).FirstOrDefault(); // Returning the Category name instead of the ID

                var communityResult = _mapper.Map<CommunityDto>(community);
                communityResult.CategoryName = categoryName.Name;

                UserCommunity userCommunities = _repository.UserCommunity.FindByCondition(userCommunity => userCommunity.CommunityId.Equals(community.CommunityId) && userCommunity.Role.Equals("Owner")).FirstOrDefault();
                User communityOwner = _repository.User.GetUserById(userCommunities.UserId);
                communityResult.OwnerName = communityOwner.FullName;
                communityResult.OwnerId = communityOwner.UserId;

                return StatusCode(StatusCodes.Status200OK, new
                {
                    body = communityResult,
                    message = "Returned selected Community from the database."
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
        /// Return all Posts registered on this Community
        /// </summary>
        /// <returns></returns>
        /// <response code="200">Retrived all Posts created for this Community</response>
        /// <response code="204">There is no Posts registered to this Community</response>
        /// <response code="400">Invalid Community ID was sent from the client</response>
        /// <response code="404">A Community with this ID doesn't exist on the database</response>        
        // GET: api/Communities/{id}/posts
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PostDto))]
        [ProducesResponseType(StatusCodes.Status204NoContent, Type = null)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = null)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = null)]
        [HttpGet("{id}/posts")]
        public IActionResult GetPostsByCommunity(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, new
                    {
                        message = "Invalid Community ID was sent from the client"
                    });
                }                

                if (_repository.Community.GetCommunityById(id) == null)
                {
                    return StatusCode(StatusCodes.Status404NotFound, new
                    {
                        message = "A Community with this ID doesn't exist on the database"
                    });
                }

                IEnumerable<Post> posts = _repository.Post.GetPostsByCommunity(id);

                if(posts.Count() == 0)
                {
                    return StatusCode(StatusCodes.Status204NoContent);
                }

                // Converting to the Data Model
                List<PostDto> postsResult = new List<PostDto>();

                foreach(var index in posts)
                {
                    postsResult.Add(_mapper.Map<Post, PostDto>(index));                    
                }

                IEnumerable<User> tempUserList = _repository.User.GetAllUsers();

                foreach(var index in postsResult)
                {
                    // Including the AuthorName (UserName) to every object on the list
                    index.AuthorName = tempUserList.Where(user => user.UserId.Equals(index.UserId)).Select(user => user.FullName).FirstOrDefault();
                }
                
                return StatusCode(StatusCodes.Status200OK, new
                {
                    body = postsResult,
                    message = "Retrived all Posts created for this Community"
                });
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    message = "A error has ocurred in the service." + $"\n{e}"
                });
            }
        }

        /// <summary>
        /// Register a new Post to a Community
        /// </summary>
        /// <returns></returns>
        /// <response code="201">Post successfully registered on the Community</response>
        /// <response code="400">Invalid Community ID or User ID was sent from the client</response>
        /// <response code="400">Data from request body is null.</response>
        /// <response code="404">A Community with this ID doesn't exist on the database</response>
        // POST: api/Communities/{id}/posts
        [ProducesResponseType(StatusCodes.Status201Created, Type = null)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = null)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = null)]
        [HttpPost("{id}/posts")]
        public IActionResult AddPostToCommunity(int id, [FromBody] PostCreateDto postCreate)
        {
            try 
            {
                if (postCreate == null)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, new
                    {
                        message = "Data from request body is null."
                    });
                }

                // Checking if any IDs passed by the Client are valid
                if (id <= 0 || postCreate.UserId <= 0)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, new
                    {
                        message = "Invalid Community ID or User ID was sent from the client."
                    });
                }

                // Check if the User ID passed by the Client does exist on the database
                if (_repository.User.GetUserById(postCreate.UserId) == null)
                {
                    return StatusCode(StatusCodes.Status404NotFound, new
                    {
                        message = "User ID was not found on the database, try another."
                    });
                }                                             

                // Check if the Community ID passed by the Client does exist on the database
                if (_repository.Community.GetCommunityById(id) == null)
                {
                    return StatusCode(StatusCodes.Status404NotFound, new
                    {
                        message = "A Community with this ID doesn't exist on the database"
                    });
                }

                Post post = _mapper.Map<PostCreateDto, Post>(postCreate);

                // Added values left to complete the Post model
                post.CreatedDate = DateTime.Now;
                post.ModifiedDate = DateTime.Now;
                post.CommunityId = id;

                _repository.Post.AddPostToCommunity(post);
                _repository.Save();

                //PostDto postResult = _mapper.Map<Post, PostDto>(post);

                return StatusCode(StatusCodes.Status201Created, new
                {
                    //body = postResult,
                    message = "Post successfully registered on the Community"
                });
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    message = "A error has ocurred in the service." + $"\n{e}"
                });
            }
        }

        /// <summary>
        /// Method to register when a User has followed a specific Community
        /// </summary>        
        /// <returns></returns>
        /// <response code="201">User was added to this community</response>
        /// <response code="400">Request data body is null</response>
        /// <response code="400">User is already on this community</response>
        // POST: api/Communities/follow
        [ProducesResponseType(StatusCodes.Status201Created, Type = null)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = null)]
        [HttpPost("follow")]
        public IActionResult AddUserToCommunity([FromBody] UserCommunityCreateDto userCommunityCreate)
        {
            try
            {
                if (userCommunityCreate == null)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, new
                    {
                        message = "Data sent from client cannot be null."
                    });
                }

                // Checking if user is already on this community
                var isUserOnCommunity = _repository.UserCommunity.FindByCondition(user => user.CommunityId.Equals(userCommunityCreate.CommunityId) && user.UserId.Equals(userCommunityCreate.UserId)).FirstOrDefault();

                if (isUserOnCommunity != null)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, new
                    {
                        message = "User is already on this community"
                    });
                }

                var userCommunity = _mapper.Map<UserCommunityCreateDto, UserCommunity>(userCommunityCreate);
                userCommunity.UserId = userCommunityCreate.UserId;

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
        /// Unfollow User from a specific Community
        /// </summary>        
        /// <returns></returns>
        // GET: api/Communities/follow
        [ProducesResponseType(StatusCodes.Status200OK, Type = null)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = null)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = null)]
        [HttpDelete("follow")]
        public IActionResult RemoveUserFromCommunity([FromBody] UserCommunityDeleteDto userCommunityDelete)
        {
            try
            {
                if (userCommunityDelete == null)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, new
                    {
                        message = "Invalid parameters were send."
                    });
                }

                var userCommunity = _repository.UserCommunity.FindByCondition(register => register.CommunityId.Equals(userCommunityDelete.CommunityId) && register.UserId.Equals(userCommunityDelete.UserId)).FirstOrDefault();

                if (userCommunity == null)
                {
                    return StatusCode(StatusCodes.Status404NotFound, new
                    {
                        message = "Nothing was found on database, try another User and Community ID"
                    });
                }

                _repository.UserCommunity.Delete(userCommunity);
                _repository.Save();

                return StatusCode(StatusCodes.Status200OK, new
                {
                    message = "User was removed from this Community successfully"
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
    }
}
