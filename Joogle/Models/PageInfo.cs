using System;

namespace Joogle.Models
{
    /// <summary>
    /// информация о странице
    /// </summary>
    public class PageInfo
    {
        /// <summary>
        /// номер текущей страницы
        /// </summary>
        public int PageNumber { get; set; } = 1;

        /// <summary>
        /// кол-во объектов на странице
        /// </summary>
        public int PageSize { get; set; } = 1;

        /// <summary>
        /// всего объектов
        /// </summary>
        public int TotalItems { get; set; }

        /// <summary>
        /// всего страниц
        /// </summary>
        public int TotalPages
        {
            get { return (int)Math.Ceiling((decimal)TotalItems / PageSize); }
        }
    }
}