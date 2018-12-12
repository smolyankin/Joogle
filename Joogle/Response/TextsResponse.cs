using System.Collections.Generic;
using Joogle.Models;

namespace Joogle.Response
{
    public class TextsResponse
    {
        public string Search { get; set; }
        public string SearchOld { get; set; }
        public List<Text> Texts { get; set; } = new List<Text>();
        public PageInfo PageInfo { get; set; } = new PageInfo();
    }
}