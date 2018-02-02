using njAsyncFileUpload.Models;
using System.Web;
using System.Web.Mvc;

namespace njAsyncFileUpload
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            ModelBinders.Binders.Add(typeof(njAsyncFile), new njAsyncFileModelBinder());
        }
    }
}
