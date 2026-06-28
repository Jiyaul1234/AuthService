using System;
using System.Threading.Tasks;
using Ecommerce.AuthService.Application.DTOs;
using Ecommerce.AuthService.Application.Interfaces.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Ecommerce.AuthService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Login endpoint - authenticates user and returns JWT token
        /// </summary>
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Login request with invalid model state.");
                    return BadRequest(ModelState);
                }

                _logger.LogInformation("Login request received for email: {Email}", loginDto.Email);
                var token = await _authService.Login(loginDto);

                return Ok(new
                {
                    success = true,
                    message = "Login successful",
                    token = token,
                    tokenType = "Bearer"
                });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Login failed for email: {Email}", loginDto?.Email);
                return Unauthorized(new
                {
                    success = false,
                    message = ex.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred during login");
                return StatusCode(500, new
                {
                    success = false,
                    message = "An unexpected error occurred"
                });
            }
        }

        /// <summary>
        /// SignUp endpoint - registers a new user
        /// </summary>
        [HttpPost("signup")]
        [AllowAnonymous]
        public async Task<IActionResult> SignUp([FromBody] UserDto userDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("SignUp request with invalid model state.");
                    return BadRequest(ModelState);
                }

                _logger.LogInformation("SignUp request received for email: {Email}", userDto.Email);
                await _authService.SingUp(userDto);

                return Ok(new
                {
                    success = true,
                    message = "User registered successfully. Please login with your credentials."
                });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "SignUp failed for email: {Email}", userDto?.Email);
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred during signup");
                return StatusCode(500, new
                {
                    success = false,
                    message = "An unexpected error occurred"
                });
            }
        }

        /// <summary>
        /// Logout endpoint - invalidates the token (client-side deletion)
        /// </summary>
        [HttpPost("logout")]
        [Authorize]
        public IActionResult Logout()
        {
            try
            {
                var email = User.FindFirst("email")?.Value ?? "Unknown";
                _logger.LogInformation("Logout request from user: {Email}", email);

                return Ok(new
                {
                    success = true,
                    message = "Logout successful. Please discard your token."
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during logout");
                return StatusCode(500, new
                {
                    success = false,
                    message = "An error occurred during logout"
                });
            }
        }
    }
}
