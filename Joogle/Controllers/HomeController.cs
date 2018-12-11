using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Joogle.Response;
using Joogle.Services;
using Joogle.Request;

namespace Joogle.Controllers
{
    public class HomeController : Controller
    {
        JoogleService joogleService = new JoogleService();

        public ActionResult Index()
        {
            var model = new TextsResponse();
            
            return View(model);
        }

        [HttpPost]
        public ActionResult Index(string search)
        {
            var model = joogleService.Search(search).GetAwaiter().GetResult();
            return View(model);
        }

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