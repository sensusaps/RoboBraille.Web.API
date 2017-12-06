using System.Web;
using System.Web.Mvc;

namespace RoboBraille.WebApi
{
    /// <summary>
    /// FilterConfig
    /// </summary>
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
