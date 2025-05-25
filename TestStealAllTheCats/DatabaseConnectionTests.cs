using Microsoft.EntityFrameworkCore;
using StealAllTheCats.Data;

namespace TestStealAllTheCats
{
    [TestClass]
    public sealed class DatabaseConnectionTests
    {
        private ApplicationDbContext CreateDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlServer("Server=localhost;Database=CatsDB;Trusted_Connection=True;TrustServerCertificate=True;")
                .Options;

            return new ApplicationDbContext(options);
        }

        [TestMethod]
        public async Task TestDatabaseConnection()
        {
            using var context = CreateDbContext();
            var canConnect = await context.Database.CanConnectAsync();
            Assert.IsTrue(canConnect, "Cannot connect to the database.");
        }
    }
}