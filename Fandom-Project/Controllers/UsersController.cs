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
using Fandom_Project.Models.DataTransferObjects;

namespace Fandom_Project.Controllers
{
    [Route("api/user")]
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

        // GET: api/Users
        [HttpGet]
        public IActionResult GetAllUsers()
        {            
            try
            {                
                _logger.LogInformation($"[{DateTime.Now}] LOG: Requesting GET api/user");
                var users = _repository.User.GetAllUsers();

                if(users == null)
                {
                    _logger.LogInformation($"[{DateTime.Now}] LOG: No User was found.");
                    return StatusCode(StatusCodes.Status404NotFound);
                }
                
                _logger.LogInformation($"[{DateTime.Now}] LOG: Returned all Users from the database.");
                var usersResult = _mapper.Map<IEnumerable<UserDto>>(users);
                return StatusCode(StatusCodes.Status200OK, usersResult);                                
            }
            catch (Exception e)
            {
                _logger.LogError($"[{DateTime.Now}] ERROR: {e}");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        // GET: api/Users/{id}
        [HttpGet("{id}", Name = "GetUserById")]
        public IActionResult GetUserById(int id)
        {
            try
            {
                _logger.LogInformation($"[{DateTime.Now}] LOG: Requesting GET api/user/{id}");
                var user = _repository.User.GetUserById(id);

                if(user == null)
                {
                    _logger.LogInformation($"[{DateTime.Now}] LOG: User with ID {id} was not found.");
                    return StatusCode(StatusCodes.Status404NotFound);
                }
                
                _logger.LogInformation($"[{DateTime.Now}] LOG: Returned selected User from the database.");
                var userResult = _mapper.Map<UserDto>(user);
                return StatusCode(StatusCodes.Status200OK, userResult);                                
            }
            catch (Exception e)
            {
                _logger.LogError($"[{DateTime.Now}] ERROR: {e}");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        // PUT: api/Users/{id}
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public IActionResult UpdateUser(int id, [FromBody]UserUpdateDto user)
        {
            try
            {
                _logger.LogInformation($"[{DateTime.Now}] LOG: Requesting PUT api/user/{id}");

                if (user == null)
                {
                    _logger.LogError($"[{DateTime.Now}] ERROR: User object sent from client is null.");
                    return StatusCode(StatusCodes.Status400BadRequest);
                }
                else if (!ModelState.IsValid)
                {
                    _logger.LogError($"[{DateTime.Now}] ERROR: Invalid User object sent from client.");
                    return StatusCode(StatusCodes.Status400BadRequest);
                }

                var userModel = _repository.User.GetUserById(id);

                if(userModel == null)
                {
                    _logger.LogError($"[{DateTime.Now}] ERROR: User with ID {id} was not found.");
                    return StatusCode(StatusCodes.Status404NotFound);
                }

                _mapper.Map(user, userModel);
                
                userModel.ModifiedDate = DateTime.Now;

                _repository.User.UpdateUser(userModel);
                _repository.Save();

                _logger.LogInformation($"[{DateTime.Now}] LOG: User with ID {userModel.UserId} updated.");                
                return StatusCode(StatusCodes.Status204NoContent);
            }
            catch (Exception e)
            {
                _logger.LogError($"[{DateTime.Now}] ERROR: {e}");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        // POST: api/Users
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public IActionResult CreateUser([FromBody]UserCreationDto user)
        {
            try
            {
                _logger.LogInformation($"[{DateTime.Now}] LOG: Requesting POST api/user");                

                if(user == null)
                {
                    _logger.LogError($"[{DateTime.Now}] ERROR: User object sent from client is null.");
                    return StatusCode(StatusCodes.Status400BadRequest);
                }
                else if (!ModelState.IsValid)
                {
                    _logger.LogError($"[{DateTime.Now}] ERROR: Invalid User object sent from client.");
                    return StatusCode(StatusCodes.Status400BadRequest);
                }

                var userModel = _mapper.Map<User>(user);
                userModel.CreatedDate = DateTime.Now;
                userModel.ModifiedDate = DateTime.Now;

                _repository.User.CreateUser(userModel);
                _repository.Save();

                _logger.LogInformation($"[{DateTime.Now}] LOG: User with ID {userModel.UserId} created.");
                var createdUser = _mapper.Map<UserDto>(userModel);
                return StatusCode(StatusCodes.Status201Created, createdUser);            
            }
            catch (Exception e)
            {
                _logger.LogError($"[{DateTime.Now}] ERROR: {e}");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        // DELETE: api/Users/{id}
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
                    return StatusCode(StatusCodes.Status404NotFound);
                }

                _repository.User.DeleteUser(user);
                _repository.Save();
                return StatusCode(StatusCodes.Status204NoContent);
            }
            catch (Exception e)
            {
                _logger.LogError($"[{DateTime.Now}] ERROR: {e}");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        // POST: api/Users/authentication
        [HttpPost("authentication")]
        public IActionResult UserAuthentication([FromBody]UserAuthenticationDto login)
        {
            try
            {
                _logger.LogInformation($"[{DateTime.Now}] LOG: Requesting POST api/user/authenticate");

                if (login == null)
                {
                    _logger.LogError($"[{DateTime.Now}] ERROR: Email or Password cannot be null.");
                    return StatusCode(StatusCodes.Status400BadRequest);
                }
                else if (!ModelState.IsValid)
                {
                    _logger.LogError($"[{DateTime.Now}] ERROR: Invalid data sent from client.");
                    return StatusCode(StatusCodes.Status400BadRequest);
                }

                var email = login.Email;
                var password = login.Password;

                var userResult = _repository.User.UserAuthentication(email, password);

                if (userResult == null)
                {
                    _logger.LogError($"[{DateTime.Now}] ERROR: Invalid Email / Password was sent");
                    return StatusCode(StatusCodes.Status401Unauthorized);
                }

                _logger.LogInformation($"[{DateTime.Now}] LOG: User with email {email} is authenticated");
                return StatusCode(StatusCodes.Status200OK, userResult);
            }
            catch (Exception e)
            {
                _logger.LogError($"[{DateTime.Now}] ERROR: {e}");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPut("reset-password")]
        public IActionResult ResetPassword([FromBody] UserAuthenticationDto login)
        {
            try
            {
                _logger.LogInformation($"[{DateTime.Now}] LOG: Requesting PUT api/user/reset-password");

                if (login == null)
                {
                    _logger.LogError($"[{DateTime.Now}] ERROR: Email or Password cannot be null.");
                    return StatusCode(StatusCodes.Status400BadRequest);
                }
                else if (!ModelState.IsValid)
                {
                    _logger.LogError($"[{DateTime.Now}] ERROR: Invalid data sent from client.");
                    return StatusCode(StatusCodes.Status400BadRequest);
                }

                var email = login.Email;
                var password = login.Password;

                if (_repository.User.ResetPassword(email, password) == false)
                {
                    _logger.LogError($"[{DateTime.Now}] ERROR: No User was found using this Email.");
                    return StatusCode(StatusCodes.Status400BadRequest);
                }

                _repository.Save();
                _logger.LogInformation($"[{DateTime.Now}] LOG: User password was changed sucessfully");
                return StatusCode(StatusCodes.Status200OK);
            }
            catch (Exception e)
            {
                _logger.LogError($"[{DateTime.Now}] ERROR: {e}");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
