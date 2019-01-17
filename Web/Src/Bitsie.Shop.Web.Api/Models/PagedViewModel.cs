using Bitsie.Shop.Services;
using Bitsie.Shop.Web.Api.Models;

namespace Bitsie.Shop.Web.Api.Models
{
    public class PagedViewModel<T> : BaseViewModel
    {
        public int CurrentPage { get; set; }
        public int NumPages { get; set; }
        public int NumPerPage { get; set; }
        public bool HasMorePages { get { return CurrentPage < NumPages; } }

        public PagedViewModel(IPagedList<T> pagedList)
        {
            if (pagedList == null) return;
            CurrentPage = pagedList.CurrentPage;
            NumPages = pagedList.TotalPages;
            NumPerPage = pagedList.PageSize;
        }
    }
}