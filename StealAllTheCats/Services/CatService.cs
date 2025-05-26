using System.Text.Json;
using StealAllTheCats.Models;
using StealAllTheCats.Repositories;
using StealAllTheCats.Dto;

namespace StealAllTheCats.Services
{
    public class CatService : ICatService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ICatRepository _catRepository;
        private readonly ILogger<CatService> _logger;

        public CatService(
            IHttpClientFactory httpClientFactory,
            ICatRepository catRepository,
            ILogger<CatService> logger)
        {
            _httpClientFactory = httpClientFactory;
            _catRepository = catRepository;
            _logger = logger;
        }

        public async Task FetchAndSaveCatsAsync()
        {
            var cats = await FetchCatsAsync();

            if (cats == null || !cats.Any())
                return;

            foreach (var cat in cats)
            {
                if (await _catRepository.CatExistsAsync(cat.Id))
                {
                    _logger.LogInformation("Duplicate cat ignored: {CatId}", cat.Id);
                    continue;
                }

                var imageBytes = await DownloadCatImageAsync(cat.Url);
                var catEntity = await CreateCatEntityAsync(cat, imageBytes);

                await _catRepository.AddCatAsync(catEntity);
                await _catRepository.SaveChangesAsync();
            }
        }
        public async Task<CatEntity?> GetCatWithTagsAsync(int id)
        {
            return await _catRepository.GetCatWithTagsAsync(id);
        }
        public IQueryable<CatEntity> GetCatsWithTags()
        {
            return _catRepository.GetCatsWithTags();
        }
       
        private async Task<List<CatApiImage>> FetchCatsAsync()
        {
            var client = _httpClientFactory.CreateClient("CatApiClient");
            var response = await client.GetAsync("images/search?limit=25&has_breeds=1");
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();

            var cats = JsonSerializer.Deserialize<List<CatApiImage>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return cats ?? new List<CatApiImage>();
        }

        private async Task<byte[]> DownloadCatImageAsync(string imageUrl)
        {
            var client = _httpClientFactory.CreateClient("CatApiClient");
            return await client.GetByteArrayAsync(imageUrl);
        }

        private async Task<CatEntity> CreateCatEntityAsync(CatApiImage cat, byte[] imageBytes)
        {
            var catEntity = new CatEntity
            {
                CatId = cat.Id,
                Width = cat.Width,
                Height = cat.Height,
                Image = imageBytes,
                Created = DateTime.UtcNow,
                CatTags = new List<CatTagEntity>()
            };

            if (cat.Breeds != null)
            {
                foreach (var breed in cat.Breeds)
                {
                    if (string.IsNullOrWhiteSpace(breed.Temperament))
                        continue;

                    var temperamentList = breed.Temperament
                        .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

                    foreach (var temp in temperamentList)
                    {
                        var tag = await _catRepository.GetOrCreateTagAsync(temp);
                        catEntity.CatTags.Add(new CatTagEntity
                        {
                            Cat = catEntity,
                            Tag = tag
                        });
                    }
                }
            }

            return catEntity;
        }
    }
}