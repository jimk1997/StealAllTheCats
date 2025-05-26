using StealAllTheCats.Models;

namespace StealAllTheCats.Services
{
    public interface ICatService
    {
        Task FetchAndSaveCatsAsync();
        Task<CatEntity?> GetCatWithTagsAsync(int id);
        IQueryable<CatEntity> GetCatsWithTags();
    }
}