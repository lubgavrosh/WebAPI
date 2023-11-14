namespace WebStore.Data.Entitties.Base
{
  

    public class BaseEntity<T> : IEntity<T>
    {
        public T Id { get; set; } = default!;
        public bool IsDeleted { get; set; } = false;
        public DateTime CreatedAt { get; set; }
    }
}