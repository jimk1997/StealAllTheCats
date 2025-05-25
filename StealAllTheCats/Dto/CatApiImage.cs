namespace StealAllTheCats.Dto
{
    public class CatApiImage
    {
        public string Id { get; set; }
        public string Url { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public List<CatApiBreed> Breeds { get; set; }
    }

    public class CatApiBreed
    {
        public string Temperament { get; set; }
    }
}