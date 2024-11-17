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
            var mainCatalogId = await EnsureMainCatalogAsync();

            if (parentId == null)
            {
                parentId = mainCatalogId;
            }
            else
            {
                var parent = await _repository.GetByIdAsync(parentId);
                if (parent == null)
                    throw new Exception("Parent not found.");
            }

            var newCatalog = new Catalog
            {
                Name = name,
                ParentId = parentId
            };

            await _repository.CreateAsync(newCatalog);

            if (parentId != null)
            {
                var parent = await _repository.GetByIdAsync(parentId);
                parent?.ChildIds?.Add(newCatalog.Id);
                if (parent != null)
                {
                    await _repository.UpdateAsync(parent);
                }
            }

            return newCatalog;
        }

        public async Task DeleteAsync(string id)
        {
            var catalog = await _repository.GetByIdAsync(id);
            if (catalog == null)
                throw new Exception("Category not found.");

            if (catalog.ParentId != null)
            {
                var parent = await _repository.GetByIdAsync(catalog.ParentId);
                parent?.ChildIds?.Remove(id);
                if (parent != null)
                {
                    await _repository.UpdateAsync(parent);
                }
            }

            await _repository.DeleteAsync(id);
        }

        private async Task<string> EnsureMainCatalogAsync()
        {
            Catalog mainCatalog;
            if (!await _repository.AnyAsync())
            {
                mainCatalog = new Catalog { Name = "Main" };
                await _repository.CreateAsync(mainCatalog);
                return mainCatalog.Id;
            }

            mainCatalog = (await _repository.GetAllAsync()).FirstOrDefault(c => c.Name == "Main");
            return mainCatalog?.Id ?? throw new Exception("Main category not found.");
        }
    }
}
