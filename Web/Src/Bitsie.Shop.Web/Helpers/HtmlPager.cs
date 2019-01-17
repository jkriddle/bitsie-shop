using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Web;

namespace Bitsie.Shop.Web.Helpers
{
    public static class HtmlPager
    {
        public static HtmlString Render(string urlBase, int currentPage, int totalPages)
        {
            var sb = new StringBuilder();
            var exclude = new List<string> {"page"};
            var maxPagesToShow = 10;

            // Create URL
            var pagerQueryString = new NameValueCollection(HttpContext.Current.Request.QueryString);
            
            // Add at least one querystring parameter so the URL manipulation is easier below.
            pagerQueryString.Add("paged", "paged");

            foreach(string key in exclude)
            {
                pagerQueryString.Remove(key);
            }

            // Create base pager URL from allowed querystring parameters
            var pagerUrl = urlBase + "?" +  string.Join("&", Array.ConvertAll(pagerQueryString.AllKeys, key => string.Format("{0}={1}", 
                HttpUtility.UrlEncode(key), HttpUtility.UrlEncode(pagerQueryString[key]))));

            // Open wrapper
            sb.Append(@"<div class=""table-pager"">");

            // First
            sb.Append(@"<a class=""first ui-corner-tl ui-corner-bl ui-button ui-state-default ");
            if (currentPage == 1)
            {
                sb.Append(@"ui-state-disabled");
            }
            sb.Append(@""" href=""" + pagerUrl + @"&page=1"">First</a>");
            
            // Previous
            sb.Append(@"<a class=""previous ui-button ui-state-default ");
            if (currentPage == 1)
            {
                sb.Append(@"ui-state-disabled");
            }
            sb.Append(@""" href=""" + pagerUrl + @"&page=" + (currentPage - 1 < 1 ? currentPage - 1 : 1) + @""">Previous</a>");

            // Open pages wrapper
            sb.Append(
                @"<span>");

            // Determine start and end pages
            int numToShowDiff = (int)Math.Floor(Convert.ToDouble(maxPagesToShow / 2));
            int start = Convert.ToInt32(currentPage - numToShowDiff);
            int end = (int)(start + (numToShowDiff * 2)) - 1;
            int diff = 0;
            if (start < 1)
            {
                diff = 1 - start;
                start = 1;
                end += diff;
            }
            if (end > totalPages)
            {
                diff = end - totalPages;
                end = totalPages;
                start -= diff;
            }
            if (start < 1) start = 1;

            // Output pages
            for(int i = start; i <= end; i++)
            {
                sb.Append(@"<a class=""ui-button ui-state-default ");
                if (currentPage == i)
                {
                    sb.Append(@"ui-state-disabled");
                }  
                sb.Append(@"""href=""" + pagerUrl + @"&page=" + i + @""">" + i + "</a>");
            }
            
            sb.Append(@"</span>");

            // Next
            sb.Append(@"<a tabindex=""0"" class=""next ui-button ui-state-default ");
            if (currentPage == totalPages)
            {
                sb.Append(@"ui-state-disabled");
            }
            sb.Append(@""" href=""" + pagerUrl + @"&page=" + (currentPage + 1 > totalPages ? currentPage + 1 : totalPages) + @""">Next</a>");

            // Last
            sb.Append(@"<a tabindex=""0"" class=""last ui-corner-tr ui-corner-br ui-button ui-state-default ");
            if (currentPage == totalPages)
            {
                sb.Append(@"ui-state-disabled");
            }
            sb.Append(@""" href=""" + pagerUrl + @"&page=" + totalPages + @""">Last</a>");

            // Close wrapper
            sb.Append("</div>");
            return new HtmlString(sb.ToString());
        }
    }
}