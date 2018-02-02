using njAsyncFileUpload.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace njAsyncFileUpload.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Index(njAsyncFile njAsyncFile)
        {if (njAsyncFile.Files.Count == 0)
                ModelState.AddModelError("", "حداقل یک فایل باید ارسال نمایید");
            foreach(var item in njAsyncFile.Files)
            {
                System.IO.File.WriteAllBytes(Server.MapPath("~/App_Data/"+item.FileName), item.Data.ToArray());
            }
            njAsyncFile.Empty();
            return View();
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