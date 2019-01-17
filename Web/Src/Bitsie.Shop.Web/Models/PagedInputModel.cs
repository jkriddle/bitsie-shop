namespace Bitsie.Shop.Web.Models
{
    public class PagedInputModel
    {
        public string SearchQuery { get; set; }
        public int CurrentPage { get; set; }
        public int NumPerPage { get; set; }

        public PagedInputModel()
        {
            CurrentPage = 1;
            NumPerPage = 20;
        }
    }
}