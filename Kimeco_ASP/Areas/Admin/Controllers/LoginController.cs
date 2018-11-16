using Kimeco_ASP.Models;
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
        KimecoEntities db = new KimecoEntities();
        // GET: Admin/Login
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(string username, string password, bool RememberMe = false)
        {
            if (Membership.ValidateUser(username, password) && ModelState.IsValid)
            {
                FormsAuthentication.SetAuthCookie(username, RememberMe);
                User user = db.Users.First(x => x.Username == username);
                if (user != null)
                {
                    Response.Cookies["position"].Value = user.Position;
                    Response.Cookies["position"].Expires = DateTime.Now.AddYears(1);
                    Response.Cookies["username"].Value = user.Username;
                    Response.Cookies["username"].Expires = DateTime.Now.AddYears(1);
                }

                return Redirect("/admin/home");
            }
            else
            {
                ModelState.AddModelError("", "Wrong username or password!");
            }
            return View();
        }
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            Response.Cookies["position"].Value = "";
            Response.Cookies["username"].Value = "";
            return RedirectToAction("Index", "Home");
        }
    }
}