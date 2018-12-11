using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Joogle.Request
{
    /// <summary>
    /// модель создания сайта
    /// </summary>
    public class CreateSiteRequest
    {
        /// <summary>
        /// адрес сайта
        /// </summary>
        public string Url { get; set; }
    }
}