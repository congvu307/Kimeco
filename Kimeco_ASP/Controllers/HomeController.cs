using Kimeco_ASP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Kimeco_ASP.Controllers
{
    public class HomeController : Controller
    {
        public KimecoEntities db = new KimecoEntities();
        public ActionResult History()
        {
           
            return PartialView();
        }
        public ActionResult Index()
        {
            //List of project in history
            var ListHistory = db.Histories.ToList().OrderByDescending(x => x.Period);
            //list Period
            var ListPeriod = new List<double?>();
            foreach (var item in ListHistory)
            {
                if (!ListPeriod.Contains(item.Period))
                {
                    ListPeriod.Add(item.Period);
                }
            }
            ViewData["ListPeriod"] = ListPeriod;
            return View(ListHistory);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}