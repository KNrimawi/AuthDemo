using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using WebApplication6.MiddleWare;

namespace WebApplication2.Controllers
{
    public class DemoController : ApiController
    {
        [HttpGet, Route("api/items")]

        public IHttpActionResult Items()
        {
            var jwtAuth = new JwtAuthorization();

            var jwtToken = HttpContext.Current.Request.Cookies["JWT"]?.Value;

            if (string.IsNullOrWhiteSpace(jwtToken))
                return Unauthorized();

            //validate token


            return Ok(true);
        }
    }
}
