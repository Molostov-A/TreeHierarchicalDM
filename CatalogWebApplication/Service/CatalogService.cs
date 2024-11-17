using CatalogWebApplication.Data;
using CatalogWebApplication.Repository;

namespace CatalogWebApplication.Service
{
    public class CatalogService
    {
        private readonly ICatalogRepository _repository;

        public CatalogService(ICatalogRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<Catalog>> GetAllAsync() =>
            await _repository.GetAllAsync();

        public async Task<Catalog?> GetByIdAsync(string id) =>
            await _repository.GetByIdAsync(id);

        public async Task<Catalog> CreateAsync(string name, string? parentId)
        {
            var mainCategoryId = await EnsureMainCategoryAsync();

            if (parentId == null)
            {
                parentId = mainCategoryId;
            }
            else
            {
                var parent = await _repository.GetByIdAsync(parentId);
                if (parent == null)
                    throw new Exception("Parent not found.");
            }

            var newCategory = new Catalog
            {
                Name = name,
                ParentId = parentId
            };

            await _repository.CreateAsync(newCategory);

            if (parentId != null)
            {
                var parent = await _repository.GetByIdAsync(parentId);
                parent?.ChildIds?.Add(newCategory.Id);
                if (parent != null)
                {
                    await _repository.UpdateAsync(parent);
                }
            }

            return newCategory;
        }

        public async Task DeleteAsync(string id)
        {
            var category = await _repository.GetByIdAsync(id);
            if (category == null)
                throw new Exception("Category not found.");

            if (category.ParentId != null)
            {
                var parent = await _repository.GetByIdAsync(category.ParentId);
                parent?.ChildIds?.Remove(id);
                if (parent != null)
                {
                    await _repository.UpdateAsync(parent);
                }
            }

            await _repository.DeleteAsync(id);
        }

        private async Task<string> EnsureMainCategoryAsync()
        {
            Catalog mainCategory;
            if (!await _repository.AnyAsync())
            {
                mainCategory = new Catalog { Name = "Main" };
                await _repository.CreateAsync(mainCategory);
                return mainCategory.Id;
            }

            mainCategory = (await _repository.GetAllAsync()).FirstOrDefault(c => c.Name == "Main");
            return mainCategory?.Id ?? throw new Exception("Main category not found.");
        }
    }
}
