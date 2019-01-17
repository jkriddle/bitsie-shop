using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bitsie.Shop.Services
{
    public class HtmlHelper
    {
        public static string Sanitize(string html)
        {
            // Strip script tags
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);

            var nodes = doc.DocumentNode.SelectNodes("//script");

            if (nodes != null)
            {
                foreach (var node in nodes)
                    node.ParentNode.RemoveChild(node);
            }

            return doc.DocumentNode.OuterHtml;
        }
    }
}
