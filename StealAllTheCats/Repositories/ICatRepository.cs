using StealAllTheCats.Models;

namespace StealAllTheCats.Repositories
{
    public interface ICatRepository
    {
        Task<bool> CatExistsAsync(string catId);
        Task AddCatAsync(CatEntity cat);
        Task<TagEntity> GetOrCreateTagAsync(string tagName);
        Task SaveChangesAsync();
    }
}