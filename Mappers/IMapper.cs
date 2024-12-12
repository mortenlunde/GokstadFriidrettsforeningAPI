namespace GokstadFriidrettsforeningAPI.Mappers;

public interface IMapper<TModel, TResponse>
{
    TResponse MapToResponse(TModel model);
    TModel MapToModel(TResponse response);
}