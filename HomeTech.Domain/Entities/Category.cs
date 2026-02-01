namespace HomeTech.Domain.Entities
{
    public class Category : BaseEntity
    {
        public string Name { get; private set; } = null!;
        public bool IsActive { get; private set; }
        public byte[] RowVersion { get; private set; } = null!;

        private Category() { }

        public Category(string name, bool isActive)
        {
            SetName(name);
            IsActive = isActive;
        }

        public void SetName(string name)
            => Name = name.Trim();

        public void SetActive(bool isActive)
            => IsActive = isActive;
    }
}