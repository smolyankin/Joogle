using System.Collections.Generic;
using Joogle.Models;

namespace Joogle.Response
{
    /// <summary>
    /// модель результата поиска
    /// </summary>
    public class TextsResponse
    {
        /// <summary>
        /// поисковый запрос
        /// </summary>
        public string Search { get; set; }

        /// <summary>
        /// список текстов
        /// </summary>
        public List<Text> Texts { get; set; } = new List<Text>();

        /// <summary>
        /// информация о странице
        /// </summary>
        public PageInfo PageInfo { get; set; } = new PageInfo();
    }
}