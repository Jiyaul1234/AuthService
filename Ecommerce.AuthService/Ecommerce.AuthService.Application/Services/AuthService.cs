using System;
using System.Linq;
using System.Threading.Tasks;
using Ecommerce.AuthService.Application.DTOs;
using Ecommerce.AuthService.Application.Enums;
using Ecommerce.AuthService.Application.Interfaces.IReposiotory;
using Ecommerce.AuthService.Application.Interfaces.IService;
using Microsoft.Extensions.Logging;

namespace Ecommerce.AuthService.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IEncryptionService _encryptionService;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly ILogger<AuthService> _logger;

        public AuthService(IUserRepository userRepository, IEncryptionService encryptionService, IJwtTokenService jwtTokenService, ILogger<AuthService> logger)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _encryptionService = encryptionService ?? throw new ArgumentNullException(nameof(encryptionService));
            _jwtTokenService = jwtTokenService ?? throw new ArgumentNullException(nameof(jwtTokenService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<string> Login(LoginDto loginDto)
        {
            try
            {
                _logger.LogInformation("Login attempt for email: {Email}", loginDto.Email);

                if (loginDto == null) throw new ArgumentNullException(nameof(loginDto));

                // Find user by email
                var users = await _userRepository.FindAsync(u => u.Email == loginDto.Email);
                var user = users.FirstOrDefault();

                if (user == null)
                {
                    _logger.LogWarning("Login failed: User with email {Email} not found.", loginDto.Email);
                    throw new InvalidOperationException($"User with email {loginDto.Email} not found.");
                }

                // Decrypt stored password and compare
                var decryptedPassword = _encryptionService.Decrypt(user.Password);
                if (decryptedPassword != loginDto.Password)
                {
                    _logger.LogWarning("Login failed: Invalid password for email {Email}.", loginDto.Email);
                    throw new InvalidOperationException("Invalid email or password.");
                }

                // Generate JWT token
                var jwtToken = _jwtTokenService.GenerateToken(user.Id, user.Email, user.Role);

                _logger.LogInformation("Login successful for email: {Email}. JWT token generated.", loginDto.Email);
                return jwtToken;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during login for email: {Email}", loginDto.Email);
                throw;
            }
        }

        public async Task SingUp(UserDto userDto)
        {
            try
            {
                _logger.LogInformation("SignUp attempt for email: {Email}", userDto.Email);

                if (userDto == null) throw new ArgumentNullException(nameof(userDto));

                // Check if user already exists
                var existingUsers = await _userRepository.FindAsync(u => u.Email == userDto.Email);
                if (existingUsers.Any())
                {
                    _logger.LogWarning("SignUp failed: User with email {Email} already exists.", userDto.Email);
                    throw new InvalidOperationException($"User with email {userDto.Email} already exists.");
                }

                // Add new user with encrypted password
                var encryptedPassword = _encryptionService.Encrypt(userDto.Password);
                await _userRepository.AddAsync(new Domain.Models.User
                {
                    Name = userDto.Name,
                    Email = userDto.Email,
                    PhoneNumber = userDto.PhoneNumber,
                    Role = userDto.Role ?? Roles.User.ToString(),
                    Password = encryptedPassword,
                    CreatedDate = DateTime.UtcNow
                });

                _logger.LogInformation("SignUp successful for email: {Email}", userDto.Email);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during SignUp for email: {Email}", userDto.Email);
                throw;
            }
        }
    }
}
