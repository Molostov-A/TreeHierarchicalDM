using CatalogWebApplication.Context;
using CatalogWebApplication.Data;
using MongoDB.Driver;

namespace CatalogWebApplication.Repository
{
    public class MongoCatalogRepository: ICatalogRepository
    {
        private readonly IMongoCollection<Catalog> _categories;

        public MongoCatalogRepository(IMongoDatabase database)
        {
            _categories = database.GetCollection<Catalog>("Categories");
        }

        public async Task<List<Catalog>> GetAllAsync() =>
            await _categories.Find(_ => true).ToListAsync();

        public async Task<Catalog> GetByIdAsync(string id) =>
            await _categories.Find(c => c.Id == id).FirstOrDefaultAsync();

        public async Task CreateAsync(Catalog category) =>
            await _categories.InsertOneAsync(category);

        public async Task UpdateAsync(Catalog category) =>
            await _categories.ReplaceOneAsync(c => c.Id == category.Id, category);

        public async Task DeleteAsync(string id) =>
            await _categories.DeleteOneAsync(c => c.Id == id);

        public async Task<bool> AnyAsync() =>
            await _categories.CountDocumentsAsync(_ => true) > 0;
    }
}
