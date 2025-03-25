using Microsoft.AspNetCore.Mvc;
using FnbReservationAPI.src.features.User;
using FnbReservationAPI.src.features.Jwt;
using FnbReservationAPI.src.utils;
using Microsoft.AspNetCore.Identity.Data;

namespace FnbReservationAPI.src.features.User
{
    [ApiController]
    [Route("api/users")]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtService _jwtService;

        public UserController(IUserRepository userRepository, IJwtService jwtService)
        {
            _userRepository = userRepository;
            _jwtService = jwtService;
        }

        [HttpGet("{role}/get-all")]
        public async Task<ActionResult<object>> GetAllUsersByRole([FromRoute] string role, [FromQuery] int pageNumber, [FromQuery] int pageSize)
        {
            try
            {
                var users = await _userRepository.GetAllUsersByRoleAsync(role, pageNumber, pageSize);

                if (users == null || users.Count == 0)
                {
                    return Ok(new
                    {
                        success = false,
                        message = "No users found.",
                    });
                }

                if (pageNumber < 1 || pageSize < 1)
                {
                    pageNumber = 1;
                    pageSize = 10;
                }

                return Ok(new
                {
                    success = true,
                    message = "Users retrieved successfully.",
                    data = new
                    {
                        items = users,
                        pageNumber = pageNumber,
                        pageSize = pageSize,
                        totalCount = users.Count,
                        totalPages = (int)Math.Ceiling(users.Count / (double)pageSize),
                        hasPreviousPage = pageNumber > 1,
                        hasNextPage = users.Count > pageNumber * pageSize
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "An error occurred while fetching users.",
                    error = ex.Message
                });
            }
        }

        [HttpGet("search")]
        public async Task<ActionResult<User>> SearchUser([FromQuery] string query, [FromQuery] string role)
        {
            try
            {
                var user = await _userRepository.GetUserBySearchAsync(query, role);
                if (user == null)
                    return Ok(new
                    {
                        success = true,
                        message = "User not found."
                    });

                return Ok(user);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "An error occurred while searching user.",
                    error = ex.Message
                });
            }
        }

        [HttpPost("register")]
        public async Task<ActionResult<User?>> RegisterUser([FromBody] User user)
        {
            try
            {
                var checkExist = await _userRepository.UserExistsAsync(user.Email, user.ContactNumber);
                if (checkExist)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "User already exists."
                    });
                }

                string hashedPassword = Password.HashPassword(user.Password);
                user.Password = hashedPassword;

                var newUser = await _userRepository.RegisterUserAsync(user);
                return Ok(new
                {
                    success = true,
                    message = "User registered successfully.",
                    data = newUser
                });

            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "An error occurred while registering user.",
                    error = ex.Message
                });
            }
        }

        public class LoginRequest
        {
            public string Email { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
        }

        [HttpPost("login")]
        public async Task<ActionResult<object>> LoginUser([FromBody] LoginRequest loginRequest)
        {
            try
            {
                var user = await _userRepository.GetUserByEmailAsync(loginRequest.Email);
                if (user == null)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "User not found."
                    });
                }

                var verifyPassword = Password.ComparePassword(loginRequest.Password, user.Password);
                if (!verifyPassword)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Wrong password."
                    });
                }

                var token = _jwtService.GenerateToken(user);

                return Ok(new
                {
                    success = true,
                    message = "Login successful.",
                    token = token
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "An error occurred while logging in.",
                    error = ex.Message
                });
            }
        }

        [HttpPut("update/{id}")]
        public async Task<ActionResult<User>> UpdateUser(Guid id, User user)
        {
            var updatedUser = await _userRepository.UpdateUserAsync(id, user);
            if (updatedUser == null)
                return NotFound();

            return Ok(updatedUser);
        }

        [HttpGet("check-exist")]
        public async Task<ActionResult<object>> CheckUserExist([FromQuery] string email, [FromQuery] string contactNumber)
        {
            try
            {
                var exists = await _userRepository.UserExistsAsync(email, contactNumber);
                return Ok(new
                {
                    success = true,
                    message = exists ? "User exists." : "User does not exist.",
                    data = new { exists }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "An error occurred while checking user existence.",
                    error = ex.Message
                });
            }
        }

        [HttpPost("logout")]
        public ActionResult<object> Logout()
        {
            Response.Cookies.Delete("auth_token", new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict
            });

            return Ok(new
            {
                success = true,
                message = "Logged out successfully."
            });
        }

        [HttpGet("self")]
        public async Task<ActionResult<User>> GetSelfUser()
        {
            try
            {
                if (!Request.Headers.TryGetValue("Authorization", out var authHeader) || string.IsNullOrEmpty(authHeader))
                {
                    return Unauthorized(new
                    {
                        success = false,
                        message = "No authorization token provided."
                    });
                }

                var token = authHeader.ToString().Replace("Bearer ", "").Trim();

                if (string.IsNullOrEmpty(token))
                {
                    return Unauthorized(new
                    {
                        success = false,
                        message = "Invalid authorization token."
                    });
                }

                var user = await _userRepository.GetUserBySelfAsync(token);

                if (user == null)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = "User not found."
                    });
                }

                return Ok(new
                {
                    success = true,
                    message = "User retrieved successfully.",
                    data = user
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "An error occurred while fetching user details.",
                    error = ex.Message
                });
            }
        }

    }
}