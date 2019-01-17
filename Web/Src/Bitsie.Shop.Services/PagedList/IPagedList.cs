using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bitsie.Shop.Services
{
    public interface IPagedList<T>
    {
        List<T> AllItems { get; }

        List<T> Items { get; }

        int TotalCount
        {
            get;
            set;
        }

        int PageIndex
        {
            get;
            set;
        }

        int PageSize
        {
            get;
            set;
        }

        int TotalPages
        {
            get;
        }

        int CurrentPage
        {
            get;
        }

        bool IsPreviousPage
        {
            get;
        }

        bool IsNextPage
        {
            get;
        }
    }
}