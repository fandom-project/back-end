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

namespace Fandom_Project.Controllers
{
    [Route("api/categories")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly IRepositoryWrapper _repository;
        private readonly ILogger _logger;
        private readonly IMapper _mapper;

        public CategoriesController(IRepositoryWrapper repository, ILogger<UsersController> logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }

        /// <summary>
        /// Returns all registered Categories
        /// </summary>
        /// <returns></returns>
        /// <response code="200">Returns all Categories</response>
        /// <response code="404">No Category is registered in the database</response>
        // GET: api/categories
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Category))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = null)]
        public IActionResult GetAllCategories()
        {
            try
            {
                _logger.LogInformation($"[{DateTime.Now}] LOG: Requesting GET api/categories");
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
