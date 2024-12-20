using System.Linq.Expressions;
namespace GokstadFriidrettsforeningAPI.Features.Repositories.Interfaces;

public interface IRepositry<T> where T: class
{
    Task<IEnumerable<T>> GetPagedAsync(int pageNumber, int pageSize);
    Task<T?> AddAsync(T entity);
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
    Task<T?> GetByIdAsync(int id);
    Task<T?> DeleteByIdAsync(int id);
    Task<T?> UpdateByIdAsync(int id, T entity);
}