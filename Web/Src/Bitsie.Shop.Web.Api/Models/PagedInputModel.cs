using Bitsie.Shop.Domain;

namespace Bitsie.Shop.Web.Api.Models
{
    public class PagedInputModel
    {
        public string SearchTerm { get; set; }
        public int CurrentPage { get; set; }
        public int NumPerPage { get; set; }
        public string SortColumn { get; set; }
        public SortDirection SortDirection { get; set; }
        public bool Export { get; set; }

        public PagedInputModel()
        {
            CurrentPage = 1;
            NumPerPage = 20;
        }
    }
}