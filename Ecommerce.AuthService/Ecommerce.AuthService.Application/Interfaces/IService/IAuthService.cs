using Ecommerce.AuthService.Application.DTOs;

namespace Ecommerce.AuthService.Application.Interfaces.IService
{
    public interface IAuthService
    {
        public Task<AuthResponseDto> Login(LoginDto loginDto );
        public Task<AuthResponseDto> SingUp(UserDto userDto);
        
    }
}
