namespace HomeTech.Domain.Entities
{
    public class Category : BaseEntity
    {
        public string Name { get; private set; } = null!;
        public bool IsActive { get; private set; }
        public byte[] RowVersion { get; private set; } = null!;

        private Category() { }

        public Category(string name)
        {
            SetName(name);
            IsActive = true;
        }

        public void SetName(string name)
            => Name = name.Trim();

        public void SetActive(bool isActive)
            => IsActive = isActive;
    }
}