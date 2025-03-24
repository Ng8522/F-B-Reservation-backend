using Microsoft.AspNetCore.Mvc;
using FnbReservationAPI.src.features.User;
using FnbReservationAPI.src.utils;
using Microsoft.AspNetCore.Identity.Data;

namespace FnbReservationAPI.src.features.User
{
    [ApiController]
    [Route("api/users")]
    public class UserController(IUserRepository userRepository) : ControllerBase
    {
        private readonly IUserRepository _userRepository = userRepository;

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

                var token = Token.GenerateToken(user.Id.ToString(), user.Role);

                // Set cookie options
                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTime.UtcNow.AddHours(24)
                };

                // Add the token to cookies
                Response.Cookies.Append("auth_token", token, cookieOptions);

                return Ok(new
                {
                    success = true,
                    message = "Login successful.",
                    data = new
                    {
                        user = user,
                        token = token
                    }
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
    }
}