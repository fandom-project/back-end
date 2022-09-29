﻿using System;
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

        // GET: api/Communities
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

        // GET: api/Communities/{id}
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
                        message = $"User was not found."
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

        // PUT: api/Communities/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
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

                _mapper.Map(community, communityModel);

                communityModel.ModifiedDate = DateTime.Now;

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
                    message = "A error has ocurred in the service."
                });
            }
        }

        // POST: api/Communities
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
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
                else if (!ModelState.IsValid)
                {
                    _logger.LogError($"[{DateTime.Now}] ERROR: Invalid Community object sent from client.");
                    return StatusCode(StatusCodes.Status400BadRequest, new
                    {
                        message = $"Invalid Community object sent from client."
                    });
                }

                var communityModel = _mapper.Map<Community>(community);

                // Default values on User creation
                communityModel.CreatedDate = DateTime.Now;
                communityModel.ModifiedDate = DateTime.Now;
                communityModel.Slug = communityModel.Name.Replace(" ", "-").ToLower();
                communityModel.MemberCount = 0;

                _repository.Community.CreateCommunity(communityModel);
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

        //// DELETE: api/Communities/5
        //[HttpDelete("{id}")]
        //public async Task<IActionResult> DeleteCommunity(int id)
        //{
        //    var community = await _context.Community.FindAsync(id);
        //    if (community == null)
        //    {
        //        return NotFound();
        //    }

        //    _context.Community.Remove(community);
        //    await _context.SaveChangesAsync();

        //    return NoContent();
        //}

        //private bool CommunityExists(int id)
        //{
        //    return _context.Community.Any(e => e.CommunityId == id);
        //}

        // GET: api/Categories
        [HttpGet("categories")]
        public IActionResult GetAllCategories()
        {
            try
            {
                _logger.LogInformation($"[{DateTime.Now}] LOG: Requesting GET api/communities/categories");
                var categories = _repository.Category.GetAllCategories();

                if (categories.Count() == 0)
                {
                    _logger.LogInformation($"[{DateTime.Now}] LOG: No Category was found.");
                    return StatusCode(StatusCodes.Status404NotFound, new
                    {
                        message = "No Category was found in the database."
                    });
                }

                _logger.LogInformation($"[{DateTime.Now}] LOG: Returned all Categories from the database.");

                return StatusCode(StatusCodes.Status200OK, new
                {
                    body = categories,
                    message = "Returned all Categories from the database."
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
    }
}
