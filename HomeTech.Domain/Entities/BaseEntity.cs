namespace HomeTech.Domain.Entities
{
    public abstract class BaseEntity
    {
        public int Id { get; protected set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }
        public bool IsDeleted { get; private set; }


        public void MarkCreated()
            => CreatedAt = DateTime.UtcNow;

        public void MarkUpdated()
            => UpdatedAt = DateTime.UtcNow;

        public void SoftDelete()
        {
            IsDeleted = true;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}