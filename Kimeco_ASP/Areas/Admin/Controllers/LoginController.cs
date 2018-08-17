using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Kimeco_ASP.Areas.Admin.Controllers
{
    public class LoginController : Controller
    {
        // GET: Admin/Login
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(string username,string password,bool RememberMe = false)
        {
            if (Membership.ValidateUser(username, password) && ModelState.IsValid)
            {
                FormsAuthentication.SetAuthCookie(username, RememberMe);
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ModelState.AddModelError("", "Wrong username or password!");
            }
            return View();
        }
    }
}