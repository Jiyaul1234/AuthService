using Ecommerce.AuthService.Application.Interfaces.IReposiotory;
using Ecommerce.AuthService.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ecommerce.AuthService.Infrastructure.Repository
{
    public class UserRepository: BaseRepository<User>,IUserRepository
    {
        public UserRepository(AppDbContext dbContext):base(dbContext)
        {
        }
    }
}
