using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Joogle.Models
{
    public class PageInfo
    {
        public int PageNumber { get; set; } = 1; // номер текущей страницы
        public int PageSize { get; set; } = 1; // кол-во объектов на странице
        public int TotalItems { get; set; } // всего объектов
        public int TotalPages  // всего страниц
        {
            get { return (int)Math.Ceiling((decimal)TotalItems / PageSize); }
        }
    }
}