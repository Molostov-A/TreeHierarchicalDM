using CatalogWebApplication.Context;
using CatalogWebApplication.Data;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;

namespace CatalogWebApplication.Repository
{
    public class MongoCatalogRepository: ICatalogRepository
    {
        private readonly MongoDbContext _dbContext;

        public MongoCatalogRepository(MongoDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Catalog>> GetAllAsync() =>
            await _dbContext.Catalogs.Find(_ => true).ToListAsync();

        public async Task<Catalog> GetByIdAsync(string id) =>
            await _dbContext.Catalogs.Find(c => c.Id == id).FirstOrDefaultAsync();

        public async Task CreateAsync(Catalog catalog) =>
            await _dbContext.Catalogs.InsertOneAsync(catalog);

        public async Task UpdateAsync(Catalog catalog) =>
            await _dbContext.Catalogs.ReplaceOneAsync(c => c.Id == catalog.Id, catalog);

        public async Task DeleteAsync(string id) =>
            await _dbContext.Catalogs.DeleteOneAsync(c => c.Id == id);

        public async Task<bool> AnyAsync() =>
            await _dbContext.Catalogs.CountDocumentsAsync(_ => true) > 0;
    }
}
