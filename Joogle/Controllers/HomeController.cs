using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using Joogle.Models;
using Joogle.Response;
using Joogle.Services;
using Joogle.Request;

namespace Joogle.Controllers
{
    public class HomeController : Controller
    {
        JoogleService joogleService = new JoogleService();

        /// <summary>
        /// главная страница
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ActionResult Index(TextsResponse model)
        {
            return View(model);
        }

        /// <summary>
        /// страница поиска
        /// </summary>
        /// <param name="model"></param>
        /// <param name="page">номер страницы</param>
        /// <returns>результат поиска с пагинацией</returns>
        [HttpGet]
        public ActionResult Search(TextsResponse model, int page = 1)
        {
            model.PageInfo = new PageInfo
            {
                PageNumber = page,
                PageSize = 10
            };
            model = joogleService.Search(model, model.PageInfo).GetAwaiter().GetResult();
            
            return View(model);
        }

        /// <summary>
        /// страница сайтов
        /// </summary>
        /// <param name="model"></param>
        /// <param name="page"></param>
        /// <returns>список сайтов с пагинацией</returns>
        [HttpGet]
        public ActionResult Sites(SitesResponse model, int page = 1)
        {
            model.PageInfo = new PageInfo
            {
                PageNumber = page,
                PageSize = 10
            };
            model = joogleService.GetAllSites(model, model.PageInfo).GetAwaiter().GetResult();
            
            return View(model);
        }

        /// <summary>
        /// страница добавления сайта
        /// </summary>
        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// добавление сайта
        /// </summary>
        /// <param name="request"></param>
        /// <returns>возврат на страницу списка сайтов</returns>
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
        /// страница парсера
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Parser()
        {
            var model = joogleService.InfoBeforeParse().GetAwaiter().GetResult();
            return View(model);
        }

        /// <summary>
        /// запуск парсера
        /// </summary>
        /// <param name="request"></param>
        /// <returns>результат парсинга</returns>
        [HttpPost]
        public ActionResult Parser(ParseResponse model)
        {
            try
            {
                joogleService.StartParseAllSites(model).GetAwaiter().GetResult();

                return View(model);
            }
            catch
            {
                model.Sites = 0;
                model.Time = new TimeSpan();
                model.Finished = false;
                return View(model);
            }
        }

        /// <summary>
        /// страница изменения сайта
        /// </summary>
        /// <param name="model"></param>
        /// <returns>информация о сайте</returns>
        public ActionResult Edit(long id)
        {
            var model = joogleService.GetDetailSite(id).GetAwaiter().GetResult();
            return View(model);
        }

        /// <summary>
        /// изменить сайт
        /// </summary>
        /// <param name="model"></param>
        /// <returns>возврат на страницу списка сайтов</returns>
        [HttpPost]
        public ActionResult Edit(Site model)
        {
            try
            {
                joogleService.EditSite(model).GetAwaiter().GetResult();

                return RedirectToAction("Sites");
            }
            catch
            {
                return View();
            }
        }

        /// <summary>
        /// страница удаления сайта и связанного текста
        /// </summary>
        /// <param name="model"></param>
        public ActionResult Delete(long id)
        {
            var model = joogleService.GetDetailSite(id).GetAwaiter().GetResult();
            return View(model);
        }

        /// <summary>
        /// удаление сайта и связанного текста
        /// </summary>
        /// <param name="site"></param>
        /// <returns>на страницу сайтов</returns>
        [HttpPost]
        public ActionResult Delete(Site site)
        {
            try
            {
                joogleService.DeleteSite(site).GetAwaiter().GetResult();

                return RedirectToAction("Sites");
            }
            catch
            {
                return View();
            }
        }

        /// <summary>
        /// очистка базы данных
        /// </summary>
        /// <returns>на страницу сайтов</returns>
        public ActionResult Clear()
        {
            try
            {
                joogleService.ClearDatabase().GetAwaiter().GetResult();
                return RedirectToAction("Sites");
            }
            catch
            {
                return RedirectToAction("Sites");
            }
            
        }
    }
}