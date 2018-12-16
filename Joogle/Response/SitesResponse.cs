using System.Collections.Generic;
using Joogle.Models;

namespace Joogle.Response
{
    /// <summary>
    /// модель сайтов
    /// </summary>
    public class SitesResponse
    {
        /// <summary>
        /// список сайтов
        /// </summary>
        public List<Site> Sites { get; set; } = new List<Site>();

        /// <summary>
        /// информация о странице
        /// </summary>
        public PageInfo PageInfo { get; set; } = new PageInfo();
    }
}