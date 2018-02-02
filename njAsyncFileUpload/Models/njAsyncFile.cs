using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;

namespace njAsyncFileUpload.Models
{
    public class njAsyncFileModelBinder : IModelBinder
    {
        private const string fileSessionKey = "nj_eko_chb_njAsyncFiles";
        public object BindModel(ControllerContext controllerContext,
        ModelBindingContext bindingContext)
        {
            if (bindingContext.Model != null)
                throw new InvalidOperationException("Cannot update instances of njAsyncFile");
            njAsyncFile njAsyncFiles = (njAsyncFile)controllerContext.HttpContext.Session[fileSessionKey];
            if (njAsyncFiles == null)
            {
                njAsyncFiles = new Models.njAsyncFile();
                controllerContext.HttpContext.Session[fileSessionKey] = njAsyncFiles;
            }
            return njAsyncFiles;
        }
    }
    public class njAsyncFileUploadResult
    {
        public Guid Id { set; get; }
        public string Message { set; get; }
        public string FileName { set; get; }
        public bool IsSuccessful { set; get; }
        public long FileSize { set; get; }
    }
    public class njAsyncFileUpload
    {
        public njAsyncFileUpload(byte[] myData, string myFileName, Guid myId, string myContentType, bool isimage)
        { this.Data = new MemoryStream(myData); this.FileName = myFileName; this.Id = myId; this.ContentType = myContentType; this.IsImage = isimage; }
        public Guid Id { set; get; }
        public MemoryStream Data { set; get; }
        public string FileName { set; get; }
        public string ContentType { set; get; }
        public bool IsImage { set; get; }
    }
    public class njAsyncFile
    {
        private const string fileSessionKey = "nj_eko_chb_njAsyncFiles";
        private const int MaximumFileCount = 15;
        public njAsyncFile()
        {
            Files = new List<Models.njAsyncFileUpload>();
        }
        public njAsyncFile(HttpPostedFileWrapper postedfile)
        {
            Files = new List<Models.njAsyncFileUpload>();
            this.AddItem(postedfile);
        }
        public static void Empty()
        {
            try { HttpContext.Current.Session.Remove(fileSessionKey); } catch { }
        }
        public njAsyncFileUploadResult AddItem(HttpPostedFileWrapper postedfile, bool RequireImage = false)
        {

            if (postedfile == null) return new Models.njAsyncFileUploadResult { Id = new Guid(), Message = "فایل ارسال نشد", IsSuccessful = false, FileName = "خطا" };
            if (postedfile.ContentLength == 0) return new Models.njAsyncFileUploadResult { Id = new Guid(), Message = "فایل ارسالی خالی می باشد", IsSuccessful = false, FileName = postedfile.FileName };
            bool IsImage = postedfile.IsImage();
            if (!IsImage && RequireImage) return new Models.njAsyncFileUploadResult { Id = new Guid(), Message = "لطفاً عکس ارسال نمایید", IsSuccessful = false, FileName = postedfile.FileName };
            if (MaximumFileCount <= Files.Count) return new Models.njAsyncFileUploadResult { Id = new Guid(), Message = "فایل ارسال نشد! شما حداکثر " + MaximumFileCount + " فایل میتوانید ارسال کنید.", IsSuccessful = false, FileName = postedfile.FileName };

            Guid newid = Guid.NewGuid();
            byte[] fileData = null;
            using (var binaryReader = new BinaryReader(postedfile.InputStream))
            {
                fileData = binaryReader.ReadBytes(postedfile.ContentLength);
            }
            this.Files.Add(new Models.njAsyncFileUpload(fileData, postedfile.FileName, newid, postedfile.ContentType, IsImage));
            return new Models.njAsyncFileUploadResult { FileName = postedfile.FileName, Id = newid, IsSuccessful = true, FileSize = fileData.Length, Message = "فایل باموفقیت ارسال شد" };
        }
        public List<njAsyncFileUploadResult> FilesResult()
        {
            List<njAsyncFileUploadResult> result = new List<Models.njAsyncFileUploadResult>();
            foreach (var item in this.Files)
                result.Add(new Models.njAsyncFileUploadResult { FileName = item.FileName, Id = item.Id, IsSuccessful = true, FileSize = item.Data.Length });
            return result;
        }

        public List<njAsyncFileUpload> Files { set; get; }

        ~njAsyncFile() { if (this.Files != null) foreach (var item in this.Files) { item.Data.Close(); item.Data.Dispose(); } }
    }
}