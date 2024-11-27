using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace LeDucVuong.SachOnline
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.MapRoute(
                name: "Trang chu",
                url: "",
                defaults: new { controller = "SachOnline", action = "Index", id = UrlParameter.Optional }
            );
            routes.MapRoute(
                name: "Sach theo Chu de",
                url: "sach-theo-chu-de-{id}",
                defaults: new { controller = "SachOnline", action = "SachTheoChuDe", id = UrlParameter.Optional },
                namespaces: new string[] { "LeDucVuong.SachOnline.Controllers" }
            );
            routes.MapRoute(
                name: "Sach theo NXB",
                url: "sach-theo-nxb-{id}",
                defaults: new { controller = "SachOnline", action = "BookByPublisher", id = UrlParameter.Optional },
                namespaces: new string[] { "LeDucVuong.SachOnline.Controllers" }
            );
            routes.MapRoute(
                name: "Chi tiet sach",
                url: "chi-tiet-sach-{id}",
                defaults: new { controller = "SachOnline", action = "BookDetail", id = UrlParameter.Optional },
                namespaces: new string[] { "LeDucVuong.SachOnline.Controllers" }
            );
            routes.MapRoute(
                name: "Dang ky",
                url: "dang-ky",
                defaults: new { controller = "User", action = "Register" },
                namespaces: new string[] { "LeDucVuong.SachOnline.Controllers" }
            );

            routes.MapRoute(
                name: "Dang nhap",
                url: "dang-nhap",
                defaults: new { controller = "User", action = "Login", url = UrlParameter.Optional },
                namespaces: new string[] { "LeDucVuong.SachOnline.Controllers" }
            );

            routes.MapRoute(
                name: "Gio hang",
                url: "gio-hang",
                defaults: new { controller = "GioHang", action = "GioHang" },
                namespaces: new string[] { "LeDucVuong.SachOnline.Controllers" }
            );

            routes.MapRoute(
                name: "Dat hang",
                url: "dat-hang",
                defaults: new { controller = "GioHang", action = "DatHang" },
                namespaces: new string[] { "LeDucVuong.SachOnline.Controllers" }
            );

            routes.MapRoute(
                name: "Trang tin",
                url: "{metatitle}",
                defaults: new { controller = "SachOnline", action = "TrangTin", metatitle = UrlParameter.Optional },
                namespaces: new string[] { "LeDucVuong.SachOnline.Controllers" }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );

        }

    }
}
