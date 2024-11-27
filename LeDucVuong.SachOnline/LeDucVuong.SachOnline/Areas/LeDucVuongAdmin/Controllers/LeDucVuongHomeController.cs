using LeDucVuong.SachOnline.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LeDucVuong.SachOnline.Areas.LeDucVuongAdmin.Controllers
{
    public class LeDucVuongHomeController : Controller
    {
        // GET: LeDucVuongAdmin/LeDucVuongHome
        SachOnlineEntities db = new SachOnlineEntities();

        public ActionResult Index()
        {
            ViewBag.ThongBao = TempData["ThongBao"];
            ViewBag.IsSuccess = TempData["IsSuccess"] ?? true; // Mặc định là true nếu không có lỗi
            return View();
        }

        [HttpGet]
        public ActionResult Login()
        {
            // Đặt lại các thông báo trước đó khi vào trang đăng nhập
            TempData["ThongBao"] = null;
            TempData["IsSuccess"] = null;
            return View();
        }

        [HttpPost]
        public ActionResult Login(FormCollection f)
        {
            var sTenDN = f["UserName"];
            var sMatKhau = f["Password"];

            // Tìm kiếm admin với tên đăng nhập và mật khẩu đã nhập
            ADMIN ad = db.ADMINs.SingleOrDefault(n => n.TenDN == sTenDN && n.MatKhau == sMatKhau);

            if (ad != null)
            {
                // Đăng nhập thành công
                Session["Admin"] = ad;
                Session["TenAdmin"] = ad.HoTen;
                TempData["ThongBao"] = "Đăng nhập thành công!";
                TempData["IsSuccess"] = true;
                return RedirectToAction("Index", "LeDucVuongHome");
            }
            else
            {
                // Đăng nhập thất bại, giữ nguyên view đăng nhập để hiển thị thông báo và lưu lại UserName
                TempData["ThongBao"] = "Tên đăng nhập hoặc mật khẩu không đúng";
                TempData["IsSuccess"] = false;
                ViewBag.UserName = sTenDN; // Optional: You can keep the username in the input field
                return View();
            }
        }





        public ActionResult Logout()
        {
            Session.Clear();
            TempData["ThongBao"] = "Bạn đã đăng xuất thành công!";
            TempData["IsSuccess"] = true;
            return RedirectToAction("Index", "LeDucVuongHome");
        }
    }
}