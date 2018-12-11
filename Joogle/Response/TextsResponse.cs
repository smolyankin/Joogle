using System.Collections.Generic;
using Joogle.Models;

namespace Joogle.Response
{
    public class TextsResponse
    {
        public string Search { get; set; }
        public List<Text> Texts { get; set; } = new List<Text>();
    }
}