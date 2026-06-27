using Ecommerce.AuthService.Application.DTOs;

namespace Ecommerce.AuthService.Application.Interfaces.IService
{
    public interface IUserService
    {
        // add, update, delete, get by id, get all
        Task<UserDto> AddAsync(UserDto user);
        Task<UserDto> UpdateAsync(UserDto user);
        Task DeleteAsync(int id);
        Task<UserDto> GetByIdAsync(int id);
        Task<IEnumerable<UserDto>> GetAllAsync();
    }
}
