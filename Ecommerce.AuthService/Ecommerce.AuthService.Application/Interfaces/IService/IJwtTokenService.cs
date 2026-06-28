using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Ecommerce.AuthService.Application.Interfaces.IService
{
    public interface IJwtTokenService
    {
        string GenerateToken(int userId, string email, string role);
    }
}
