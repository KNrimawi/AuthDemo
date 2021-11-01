using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebApplication6.MiddleWare;

namespace WebApplication6.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        public ActionResult Index(string username , string password)
        {
            #region authentication
            //authentication

            var userId = Guid.NewGuid(); // after authentication we retrieve user id

            #endregion
            var jwtAuthorization = new JwtAuthorization();
            var jwt = jwtAuthorization.GenerateJwtToken(userId);

            HttpCookie cookie = new HttpCookie("JWT");
            cookie.Value = jwt;
            Response.SetCookie(cookie);
            Response.Cookies.Add(cookie);
            return new HttpStatusCodeResult(HttpStatusCode.OK);
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