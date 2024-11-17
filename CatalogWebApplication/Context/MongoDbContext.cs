using CatalogWebApplication.Data;
using MongoDB.Driver;

namespace CatalogWebApplication.Context
{
    public class MongoDbContext
    {
        private readonly IMongoDatabase _database;

        public MongoDbContext(IConfiguration configuration)
        {
            var client = new MongoClient(configuration.GetConnectionString("MongoDb"));
            _database = client.GetDatabase("TreeHierarchyDb");
        }

        public IMongoCollection<Catalog> Catalogs => _database.GetCollection<Catalog>("Catalogs");
    }
}
