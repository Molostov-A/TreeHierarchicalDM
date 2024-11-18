using CatalogWebApplication.Data;

namespace CatalogWebApplication.Repository
{
    public interface ICatalogRepository
    {
        Task<List<Catalog>> GetAllAsync();
        Task<Catalog?> GetByIdAsync(string id);
        Task CreateAsync(Catalog catalog);
        Task UpdateAsync(Catalog catalog);
        Task DeleteAsync(string id);
        Task<bool> AnyAsync(); // Checking the availability of records
    }
}
