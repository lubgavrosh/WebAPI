namespace WebStore.Data.Entitties.Base
{
    public interface IEntity<T>
    {
        T Id { get; set; }
        bool IsDeleted { get; set; }
        DateTime CreatedAt { get; set; }
    }
}
