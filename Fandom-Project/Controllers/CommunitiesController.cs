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
                _logger.LogInformation($"[{DateTime.Now}] LOG: Requesting GET api/community");
                var communities = _repository.Community.GetAllCommunities();

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
                    var categoryId = communities.Where(community => community.CommunityId == index.CommunityId).Select(community => community.CategoryId).FirstOrDefault();                    
                    index.CategoryName = Convert.ToString(categoriesList.Where(category => category.CategoryId.Equals(categoryId)).Select(category => category.Name).FirstOrDefault());
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
                    message = "A error has ocurred in the service."
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

                //TODO: Add validation to check if ID is a valid number (ex: '2=' is not valid)
                //if(id.)
                //{
                //    return StatusCode(StatusCodes.Status404NotFound, new { message = $"ID with value {id} is not valid." });
                //}
                if (community == null)
                {
                    _logger.LogInformation($"[{DateTime.Now}] LOG: Community with ID {id} was not found.");
                    return StatusCode(StatusCodes.Status404NotFound, new
                    {
                        message = $"Community was not found."
                    });
                }

                _logger.LogInformation($"[{DateTime.Now}] LOG: Returned selected Community from the database.");
                
                var categoryName = _repository.Category.FindByCondition(category => category.CategoryId == community.CategoryId).FirstOrDefault(); // Returning the Category name instead of the ID
                
                var communityResult = _mapper.Map<CommunityDto>(community);
                communityResult.CategoryName = categoryName.Name;
                
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
                    message = $"A error has ocurred in the service.\nERROR: {e}"
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

                _logger.LogInformation($"[{DateTime.Now}] LOG: Community was created.");
                return StatusCode(StatusCodes.Status201Created, new
                {
                    body = communityModel,
                    message = $"Community was created."
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
    }
}
