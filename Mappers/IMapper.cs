namespace GokstadFriidrettsforeningAPI.Mappers;

public interface IMapper<TModel, TResponse>
{
    TResponse MapToResonse(TModel model);
    TModel MapToModel(TResponse response);
}