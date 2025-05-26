using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestStealAllTheCats.IntegrationTests
{
    [TestClass]
    public class CatApiHealthCheckTests
    {
        [TestMethod]
        [TestCategory("Integration")]
        public async Task TestCatApiIsAvailable()
        {
            using var client = new HttpClient
            {
                BaseAddress = new Uri("https://api.thecatapi.com/v1/")
            };

            var response = await client.GetAsync("images/search?limit=1");

            Assert.IsTrue(response.IsSuccessStatusCode, $"API is not reachable. Status code: {response.StatusCode}");
        }
    }
}