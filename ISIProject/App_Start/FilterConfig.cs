using System.Web;
using System.Web.Mvc;
using ISIProject.Filters;

namespace ISIProject
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new InitializeSimpleMembershipAttribute());
        }
    }
}