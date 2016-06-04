using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Terminal.WebUI.Controllers
{
    public class TerminalController : Controller
    {
        public ActionResult Index() {

            return View();
        }

        [HttpPost]
        public ActionResult Print(int number) {
            return View(number);
        }
    }
}