namespace GokstadFriidrettsforeningAPI.Features.Repositories;

public interface IRepositry<T> where T: class
{
    Task<IEnumerable<T>> GetPagedAsync(int pageNumber, int pageSize);
}