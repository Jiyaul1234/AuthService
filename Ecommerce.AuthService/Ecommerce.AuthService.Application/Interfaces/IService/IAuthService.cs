using Ecommerce.AuthService.Application.DTOs;

namespace Ecommerce.AuthService.Application.Interfaces.IService
{
    public interface IAuthService
    {
        public Task<string> Login(LoginDto loginDto );
        public Task SingUp(UserDto userDto);
        
    }
}
