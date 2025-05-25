using Microsoft.EntityFrameworkCore;
using StealAllTheCats.Data;
using StealAllTheCats.Models;

namespace StealAllTheCats.Repositories
{
    public class CatRepository : ICatRepository
    {
        private readonly ApplicationDbContext _context;

        public CatRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> CatExistsAsync(string catId)
        {
            return await _context.Cats.AnyAsync(c => c.CatId == catId);
        }

        public async Task AddCatAsync(CatEntity cat)
        {
            await _context.Cats.AddAsync(cat);
        }

        public async Task<TagEntity> GetOrCreateTagAsync(string tagName)
        {
            var tag = await _context.Tags.FirstOrDefaultAsync(t => t.Name == tagName);

            if (tag == null)
            {
                tag = new TagEntity
                {
                    Name = tagName,
                    Created = DateTime.UtcNow
                };
                _context.Tags.Add(tag);
                await _context.SaveChangesAsync(); // Save so ID is generated
            }

            return tag;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}