using njAsyncFileUpload.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace njAsyncFileUpload.Controllers
{
    public class njAsyncFileUploadController : Controller
    {
        // GET: njAsyncFileUpload
        [HttpPost]
        public JsonResult Index(njAsyncFile njAsyncFile)
        {
            try
            {
                return Json(new { Result = "OK", Records = njAsyncFile.FilesResult(), });
            }
            catch  
            {
                return Json(new { Result = "ERROR", Message = "خطا" });
            }
        }

        // POST: njAsyncFileUpload/Create
        [HttpPost]
        public JsonResult Add(System.Web.HttpPostedFileWrapper postedfile, bool RequireImage, njAsyncFile njAsyncFile)
        {
            try
            {
                var result = njAsyncFile.AddItem(postedfile, RequireImage);
                return Json(new { Result = "OK", Records =new List<njAsyncFileUploadResult> { result }, });
            }
            catch  
            {
                return Json(new { Result = "ERROR", Message = "ثبت فایل انجام نشد" });
            }
        }

        // POST: njAsyncFileUpload/Delete/5
        [HttpPost]
        public ActionResult Delete(Guid id, njAsyncFile njAsyncFile)
        {
            try
            {
                if (id != new Guid())
                    njAsyncFile.Files.Remove(njAsyncFile.Files.First(x => x.Id == id));
                return Json(new { Result = "OK", Records = new njAsyncFileUploadResult { IsSuccessful = true, Message = "فایل حذف شد", Id = id }, });
            }
            catch
            {
                return Json(new { Result = "ERROR", Message = "حذف فایل انجام نشد" });
            }
        }
    }
}
