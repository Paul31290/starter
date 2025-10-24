public interface IEntityMapper<TEntity, TDto>
{
    TDto ToDto(TEntity entity);
    TEntity ToEntity(TDto dto);
    void UpdateEntity(TEntity entity, TDto dto); // NEW
}
