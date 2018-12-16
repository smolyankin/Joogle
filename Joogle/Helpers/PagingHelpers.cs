using System;
using System.Text;
using System.Web.Mvc;
using Joogle.Models;

namespace Joogle.Helpers
{
    /// <summary>
    /// вспомогательные методы для страниц
    /// </summary>
    public static class PagingHelpers
    {
        /// <summary>
        /// пагинация
        /// </summary>
        /// <param name="html"></param>
        /// <param name="pageInfo"></param>
        /// <param name="pageUrl"></param>
        /// <returns></returns>
        public static MvcHtmlString PageLinks(this HtmlHelper html, PageInfo pageInfo, Func<int, string> pageUrl)
        {
            StringBuilder result = new StringBuilder();
            var min = 1;
            var max = pageInfo.TotalPages;
            var range = 10;
            for (int i = pageInfo.PageNumber - range; i <= pageInfo.PageNumber + range; i++)
            {
                if (i < 1 || i > max)
                    continue;
                TagBuilder tag = new TagBuilder("a");
                tag.MergeAttribute("href", pageUrl(i));
                tag.InnerHtml = i.ToString();
                if (i == pageInfo.PageNumber)
                {
                    tag.AddCssClass("selected");
                    tag.AddCssClass("btn-primary");
                }
                tag.AddCssClass("btn btn-default");
                result.Append(tag.ToString());
            }
            return MvcHtmlString.Create(result.ToString());
        }
    }
}