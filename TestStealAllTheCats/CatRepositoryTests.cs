using Microsoft.EntityFrameworkCore;
using StealAllTheCats.Data;
using StealAllTheCats.Models;
using StealAllTheCats.Repositories;

namespace TestStealAllTheCats
{
    [TestClass]
    public class CatRepositoryTests
    {
        private ApplicationDbContext CreateInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new ApplicationDbContext(options);
        }

        [TestMethod]
        public async Task CatExistsAsync_ReturnsTrue_WhenCatExists()
        {
            using var context = CreateInMemoryDbContext();
            var repo = new CatRepository(context);

            var cat = new CatEntity { CatId = "abc123", Width = 200, Height = 300, Image = new byte[0], Created = DateTime.UtcNow };
            await context.Cats.AddAsync(cat);
            await context.SaveChangesAsync();

            var exists = await repo.CatExistsAsync("abc123");

            Assert.IsTrue(exists);
        }

        [TestMethod]
        public async Task GetCatWithTagsAsync_ReturnsCat_WhenCatExists()
        {
            using var context = CreateInMemoryDbContext();
            var repo = new CatRepository(context);

            var cat = new CatEntity { CatId = "abc123", Width = 200, Height = 300, Image = new byte[0], Created = DateTime.UtcNow };
            await context.Cats.AddAsync(cat);
            await context.SaveChangesAsync();

            var result = await repo.GetCatWithTagsAsync(cat.Id);

            Assert.IsNotNull(result);
            Assert.AreEqual("abc123", result.CatId);
        }

        [TestMethod]
        public async Task GetCatsWithTags_ReturnsCats()
        {
            using var context = CreateInMemoryDbContext();
            var repo = new CatRepository(context);

            var cat1 = new CatEntity { CatId = "abc1", Width = 100, Height = 100, Image = new byte[0], Created = DateTime.UtcNow };
            var cat2 = new CatEntity { CatId = "abc2", Width = 200, Height = 200, Image = new byte[0], Created = DateTime.UtcNow };
            await context.Cats.AddRangeAsync(cat1, cat2);
            await context.SaveChangesAsync();

            var cats = repo.GetCatsWithTags().ToList();

            Assert.AreEqual(2, cats.Count);
            Assert.IsTrue(cats.Any(c => c.CatId == "abc1"));
            Assert.IsTrue(cats.Any(c => c.CatId == "abc2"));
        }

        [TestMethod]
        public async Task GetOrCreateTagAsync_CreatesNewTag_WhenNotExists()
        {
            using var context = CreateInMemoryDbContext();
            var repo = new CatRepository(context);

            var tag = await repo.GetOrCreateTagAsync("Friendly");

            Assert.IsNotNull(tag);
            Assert.AreEqual("Friendly", tag.Name);
            Assert.IsTrue(tag.Id > 0);
        }

        [TestMethod]
        public async Task GetOrCreateTagAsync_ReturnsExistingTag_WhenExists()
        {
            using var context = CreateInMemoryDbContext();
            var repo = new CatRepository(context);

            context.Tags.Add(new TagEntity { Name = "Playful", Created = DateTime.UtcNow });
            await context.SaveChangesAsync();

            var tag = await repo.GetOrCreateTagAsync("Playful");

            Assert.IsNotNull(tag);
            Assert.AreEqual("Playful", tag.Name);
        }
    }
}