using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Joogle.Models;

namespace Joogle.Response
{
    public class SitesResponse
    {
        public List<Site> Sites { get; set; } = new List<Site>();

        public PageInfo PageInfo { get; set; } = new PageInfo();
    }
}