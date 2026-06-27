using Ecommerce.AuthService.Appliation.Interfaces.IReposiotory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.AuthService.Infrastructure.Repository
{
    public class BaseRepository<T> : IBaseReposiotory<T> where T : class
    {
        private readonly AppDbContext dbContext;

        public BaseRepository(AppDbContext dbContext)
        {
            this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task AddAsync(T entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            await dbContext.Set<T>().AddAsync(entity);
            await dbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));

            return await dbContext.Set<T>().Where(predicate).ToListAsync();
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await dbContext.Set<T>().ToListAsync();
        }

        public async Task<T> GetByIdAsync(object id)
        {
            if (id == null) throw new ArgumentNullException(nameof(id));

            var entity = await dbContext.Set<T>().FindAsync(id);
            return entity == null ? null : entity;
        }

        public async Task Remove(T entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            dbContext.Set<T>().Remove(entity);
            await  dbContext.SaveChangesAsync();
        }

        public async Task Update(T entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            dbContext.Set<T>().Update(entity);
           await dbContext.SaveChangesAsync();
        }
    }
}
