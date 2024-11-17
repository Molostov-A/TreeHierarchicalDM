using CatalogWebApplication.Context;
using CatalogWebApplication.Data;
using Microsoft.EntityFrameworkCore;

namespace CatalogWebApplication.Repository
{
    public class SqlCatalogRepository : ICatalogRepository
    {
        private readonly SqlDbContext _context;

        public SqlCatalogRepository(SqlDbContext context)
        {
            _context = context;
        }

        public async Task<List<Catalog>> GetAllAsync() =>
            await _context.Categories.ToListAsync();

        public async Task<Catalog?> GetByIdAsync(string id) =>
            await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);

        public async Task CreateAsync(Catalog category)
        {
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Catalog category)
        {
            _context.Categories.Update(category);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(string id)
        {
            var category = await GetByIdAsync(id);
            if (category != null)
            {
                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> AnyAsync() =>
            await _context.Categories.AnyAsync();
    }
}
