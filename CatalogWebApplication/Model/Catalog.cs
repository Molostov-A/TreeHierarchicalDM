namespace CatalogWebApplication.Data
{
    public class Catalog
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; } = string.Empty;
        public string? ParentId { get; set; }
        public List<string>? ChildIds { get; set; } = new List<string>();
    }
}
