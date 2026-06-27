using Ecommerce.AuthService.Application.DTOs;
using Ecommerce.AuthService.Application.Exceptions;
using Ecommerce.AuthService.Application.Interfaces.IReposiotory;
using Ecommerce.AuthService.Application.Interfaces.IService;
using Ecommerce.AuthService.Domain.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Ecommerce.AuthService.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<UserService> _logger;

        public UserService(IUserRepository userRepository,ILogger<UserService> logger)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<UserDto> AddAsync(UserDto userDto)
        {
            try 
            {
                _logger.LogInformation("Adding a new user: {User}", JsonSerializer.Serialize(userDto));
                if (userDto == null) throw new ArgumentNullException(nameof(userDto));

                var user = ToModel(userDto);
                if (user.CreatedDate == default) user.CreatedDate = DateTime.UtcNow;
                await _userRepository.AddAsync(user);

                _logger.LogInformation("User added successfully with id: {UserId}", user.Id);

                return ToDto(user);
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding a user: {User}", JsonSerializer.Serialize(userDto));
                throw; // rethrow the exception after logging
            }
        }

        public async Task DeleteAsync(int id)
        {
            try
            {
                _logger.LogInformation("Deleting user with id: {UserId}", id);
                var entity = await _userRepository.GetByIdAsync(id);
                if (entity == null)
                {
                    _logger.LogWarning("User with id {UserId} not found.", id);
                    throw new InvalidOperationException($"User with id {id} not found.");
                }

                await _userRepository.Remove(entity);
                _logger.LogInformation("User with id {UserId} deleted successfully.", id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting user with id: {UserId}", id);
                throw;
            }
        }

        public async Task<IEnumerable<UserDto>> GetAllAsync()
        {
            try
            {
                _logger.LogInformation("Retrieving all users.");
                var entities = await _userRepository.GetAllAsync();
                var dtos = entities.Select(ToDto).ToList();
                _logger.LogInformation("Retrieved {Count} users.", dtos.Count);
                return dtos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving all users.");
                throw;
            }
        }

        public async Task<UserDto> GetByIdAsync(int id)
        {
            try
            {
                _logger.LogInformation("Retrieving user with id: {UserId}", id);
                var entity = await _userRepository.GetByIdAsync(id);
                if (entity == null)
                {
                    _logger.LogWarning("User with id {UserId} not found.", id);
                    throw new  UserException("User not found :{id}");
                }

                _logger.LogInformation("User with id {UserId} retrieved.", id);
                return ToDto(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving user with id: {UserId}", id);
                throw;
            }
        }

        public async Task<UserDto> UpdateAsync(UserDto userDto)
        {
            try
            {
                _logger.LogInformation("Updating user: {User}", JsonSerializer.Serialize(userDto));
                if (userDto == null) throw new ArgumentNullException(nameof(userDto));

                var existing = await _userRepository.GetByIdAsync(userDto.Id);
                if (existing == null)
                {
                    _logger.LogWarning("User with id {UserId} not found.", userDto.Id);
                    throw new InvalidOperationException($"User with id {userDto.Id} not found.");
                }

                // map updates
                existing.Name = userDto.Name;
                existing.Email = userDto.Email;
                existing.PhoneNumber = userDto.PhoneNumber;
                existing.Role = userDto.Role;
                // do not overwrite CreatedDate

                await _userRepository.Update(existing);

                _logger.LogInformation("User with id {UserId} updated successfully.", existing.Id);
                return ToDto(existing);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating user: {User}", JsonSerializer.Serialize(userDto));
                throw;
            }
        }

        private static UserDto ToDto(User u)
        {
            if (u == null) return null;

            return new UserDto
            {
                Id = u.Id,
                Name = u.Name,
                Email = u.Email,
                PhoneNumber = u.PhoneNumber,
                Role = u.Role,
                CreatedDate = u.CreatedDate
            };
        }

        private static User ToModel(UserDto dto)
        {
            if (dto == null) return null;

            return new User
            {
                Id = dto.Id,
                Name = dto.Name,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                Role = dto.Role,
                CreatedDate = dto.CreatedDate
            };
        }
    }
}
