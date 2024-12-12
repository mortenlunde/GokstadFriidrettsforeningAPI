namespace GokstadFriidrettsforeningAPI.Features.Services.Interfaces;

public interface IService<T> where T: class
{
    Task<IEnumerable<T>> GetPagedAsync(int pageNumber, int pageSize);
    Task<T> GetByIdAsync(int id);
    Task<IEnumerable<T>> FindAsync(SearchParameters searchParameters);
    Task<T> DeleteByIdAsync(int id);
}