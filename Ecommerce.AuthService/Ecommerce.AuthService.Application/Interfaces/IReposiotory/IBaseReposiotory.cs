using System.Linq.Expressions;

namespace Ecommerce.AuthService.Appliation.Interfaces.IReposiotory
{
    public interface IBaseReposiotory<T> where T : class
    {
        Task<T> GetByIdAsync(object id);
        Task<IEnumerable<T>> GetAllAsync();
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
        Task AddAsync(T entity);
        Task Update(T entity);
        Task Remove(T entity);
    }
}
