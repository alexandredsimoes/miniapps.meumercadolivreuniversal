using MyML.BackendServices.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyML.BackendServices.Controllers
{
    public class HomeController : Controller
    {
        DataService _dataService = new DataService();

        public ActionResult Index()
        {
            ViewBag.Message = _dataService.EstaAutenticado();
            return View();
        }
    }
}
