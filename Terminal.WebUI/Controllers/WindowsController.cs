using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Terminal.WebUI.Controllers
{
    public class WindowsController : Controller
    {
        public ActionResult Index() {
            return View();
        }

        public ActionResult Choose(int id) {
            return View(id);
        }
    }
}