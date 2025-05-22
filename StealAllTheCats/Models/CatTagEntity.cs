namespace StealAllTheCats.Models
{
    public class CatTagEntity
    {
        public int CatEntityId { get; set; }
        public CatEntity Cat { get; set; }

        public int TagEntityId { get; set; }
        public TagEntity Tag { get; set; }
    }
}