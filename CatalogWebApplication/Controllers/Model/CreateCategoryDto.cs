namespace CatalogWebApplication.Controllers.Model
{
    public class CreateCategoryDto
    {
        public string Name { get; set; } = string.Empty;
        public string? ParentId { get; set; }
    }

}
