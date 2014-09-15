namespace SonarCompanion.API
{
    public class Paging
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int Total { get; set; }
        public int Pages { get; set; }
    }
}