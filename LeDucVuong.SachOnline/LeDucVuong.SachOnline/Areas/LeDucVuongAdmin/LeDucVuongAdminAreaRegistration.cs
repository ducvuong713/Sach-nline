using System.Web.Mvc;

namespace LeDucVuong.SachOnline.Areas.LeDucVuongAdmin
{
    public class LeDucVuongAdminAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "LeDucVuongAdmin";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "LeDucVuongAdmin_default",
                "LeDucVuongAdmin/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}