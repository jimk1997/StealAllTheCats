namespace StealAllTheCats.Models
{
    public class TagEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime Created { get; set; }
        public ICollection<CatTagEntity> CatTags { get; set; }
    }
}