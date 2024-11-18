using CatalogWebApplication.Data;
using MongoDB.Driver;

namespace CatalogWebApplication.Context
{
    public class MongoDbContext
    {
        private readonly IMongoDatabase _database;

        public MongoDbContext(IConfiguration configuration)
        {
            var connectionString = configuration.GetSection("Database:ConnectionStrings:MongoDb").Value;
            var client = new MongoClient(connectionString);
            _database = client.GetDatabase("TreeHierarchyDb");
        }

        public IMongoCollection<Catalog> Catalogs => _database.GetCollection<Catalog>("Catalogs");
    }
}
