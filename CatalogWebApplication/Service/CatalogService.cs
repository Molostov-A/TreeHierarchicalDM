using CatalogWebApplication.Data;
using CatalogWebApplication.Repository;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;

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

        public async Task<bool> UpdateNameAsync(string id, string newName)
        {
            var catalog = await _repository.GetByIdAsync(id);
            if (catalog == null)  return false;

            catalog.Name = newName;
            await _repository.UpdateAsync(catalog);
            return true;
        }

        public async Task<bool> UpdateParentAsync(string id, string? newParentId)
        {
            var mainCatalogId = await EnsureMainCatalogAsync();
            var catalog = await _repository.GetByIdAsync(id);
            if (catalog == null) throw new Exception("Item not found.");

            newParentId ??= mainCatalogId;

            var oldParent = await _repository.GetByIdAsync(catalog.ParentId);
            var newParent = await _repository.GetByIdAsync(newParentId);
            if (newParent == null) throw new Exception("Parent not found.");

            catalog.ParentId = newParentId;
            newParent.ChildIds = newParent.ChildIds.Where(s => s != id).ToList();
            newParent.ChildIds.Add(id);

            oldParent.ChildIds = oldParent.ChildIds.Where(s => s != id).ToList();

            await _repository.UpdateAsync(catalog);
            await _repository.UpdateAsync(newParent);
            await _repository.UpdateAsync(oldParent);

            return true;
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

            await CascadeDelete(id);
            
        }

        public async Task CascadeDelete(string id)
        {
            var idsToDelete = new HashSet<string>();
            var catalogs = await _repository.GetAllAsync();

            await FindChildren(id, catalogs, idsToDelete);

            foreach (var childId in idsToDelete)
            {
                _repository.DeleteAsync(childId);
            }
        }

        private async Task FindChildren(string id, List<Catalog> catalogs, HashSet<string> idsToDelete)
        {            
            var currentObject = catalogs.FirstOrDefault(o => o.Id == id);
            if (currentObject != null)
            {
                idsToDelete.Add(currentObject.Id);

                foreach (var childId in currentObject.ChildIds)
                {
                    if (!idsToDelete.Contains(childId)) // To avoid a loop
                    {
                        FindChildren(childId, catalogs, idsToDelete);
                    }
                }
            }
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
