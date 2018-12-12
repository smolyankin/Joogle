using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Joogle.Models;
using Joogle.Response;
using Joogle.Services;
using Joogle.Request;
//using Joogle.Helpers;

namespace Joogle.Controllers
{
    public class HomeController : Controller
    {
        JoogleService joogleService = new JoogleService();

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult Index(TextsResponse model, string search, int page = 1)
        {
            //var model = new TextsResponse();
            model.PageInfo.PageSize = 10;
            if (!string.IsNullOrWhiteSpace(model.Search))
            {
                model.PageInfo.PageSize = 10;
                model.PageInfo = new PageInfo
                {
                    PageNumber = page,
                    PageSize = model.PageInfo.PageSize
                };
                model = joogleService.Search(model, model.PageInfo).GetAwaiter().GetResult();
            }
            else
            {
                model = new TextsResponse
                {
                    Search = search,
                    PageInfo = new PageInfo
                    {
                        PageNumber = page,
                        PageSize = model.PageInfo.PageSize
                    }
                };
            }
            return View(model);
        }
        /*
        [HttpPost]
        public ActionResult Index(TextsResponse model, int page = 1)
        {
            int pageSize = 10; // количество объектов на страницу
            model.PageInfo.PageSize = 10;
            PageInfo pageInfo = new PageInfo { PageNumber = page, PageSize = pageSize };
            model = joogleService.Search(model, model.PageInfo).GetAwaiter().GetResult();
            return View(model);
        }*/

        [HttpGet]
        public ActionResult Parser()
        {
            return View();
        }

        public ActionResult Sites(SitesResponse model)
        {
            model = joogleService.GetAllSites(model).GetAwaiter().GetResult();

            return View(model);
        }

        /// <summary>
        /// окно создания заявку
        /// </summary>
        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// создать заявку
        /// </summary>
        /// <param name="request"></param>
        /// <returns>возврат на страницу списка заявок</returns>
        [HttpPost]
        public ActionResult Create(CreateSiteRequest request)
        {
            try
            {
                joogleService.CreateSite(request).GetAwaiter().GetResult();

                return RedirectToAction("Sites");
            }
            catch
            {
                return View();
            }
        }

        /// <summary>
        /// создать заявку
        /// </summary>
        /// <param name="request"></param>
        /// <returns>возврат на страницу списка заявок</returns>
        [HttpPost]
        public ActionResult Parser(string start)
        {
            try
            {
                joogleService.StartParseAllSites().GetAwaiter().GetResult();

                return RedirectToAction("Sites");
            }
            catch
            {
                return View();
            }
        }
    }
}