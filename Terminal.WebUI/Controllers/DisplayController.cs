using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Terminal.WebUI.Controllers
{
    public class DisplayController:Controller
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}