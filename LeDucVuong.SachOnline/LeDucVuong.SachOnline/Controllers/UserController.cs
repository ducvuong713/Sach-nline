using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Web;
using System.Web.Mvc;
using LeDucVuong.SachOnline.Models;
using System.Configuration;
using System.Net.Configuration;
using System.Web.UI.WebControls;


namespace LeDucVuong.SachOnline.Controllers
{
    public class UserController : Controller
    {
        SachOnlineEntities db = new SachOnlineEntities();
        // GET: User


        public ActionResult ChiTietSach(int id)
        {
            var sach = from s in db.SACHes
                       where s.MaSach == id
                       select s;
            return View(sach.Single());
        }

        public ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public ActionResult DangNhap()
        {
            return View();
        }
        [HttpPost]
        public ActionResult DangNhap(FormCollection collection)
        {
            var sTenDN = collection["TenDN"];
            var sMatKhau = collection["MatKhau"];
            if (String.IsNullOrEmpty(sTenDN))
            {
                ViewData["Err1"] = "Bạn chưa nhập tên đăng nhập";
            }
            else if (String.IsNullOrEmpty(sMatKhau))
            {
                ViewData["Err2"] = "Phải nhập mật khẩu";
            }
            else
            {
                KHACHHANG kh = db.KHACHHANGs.SingleOrDefault(n => n.TaiKhoan == sTenDN && n.MatKhau == sMatKhau);
                if (kh != null)
                {
                    ViewBag.ThongBao = "Chúc mừng đăng nhập thành công";
                    Session["TaiKhoan"] = kh;
                    Session["TenKH"] = kh.HoTen;
                    return RedirectToAction("Index", "SachOnline");
                }

                else
                {
                    ViewBag.ThongBao = "Tên đăng nhập hoặc mật khẩu không đúng";
                }

            }
            return View();
        }
        [HttpGet]
        public ActionResult DangKy()
        {
            return View();
        }
        [HttpPost]
        public ActionResult DangKy(FormCollection collection, KHACHHANG kh)
        {
            // Gán các giá trị người dùng nhập dữ liệu cho các biến
            var sHoTen = collection["HoTen"];
            var sTenDN = collection["TenDN"];
            var sMatKhau = collection["MatKhau"];
            var sMatKhauLai = collection["MatKhauNL"];
            var sDiaChi = collection["DiaChi"];
            var sEmail = collection["Email"];
            var sDienThoai = collection["DienThoai"];
            var dNgaySinh = String.Format("{0:MM/dd/yyyy}", collection["NgaySinh"]);

            if (String.IsNullOrEmpty(sHoTen))
            {
                ViewData["err1"] = "Họ tên không được rỗng";
            }
            else if (String.IsNullOrEmpty(sTenDN))
            {
                ViewData["err2"] = "Tên đăng nhập không được rỗng";
            }
            else if (String.IsNullOrEmpty(sMatKhau))
            {
                ViewData["err3"] = "Phải nhập mật khẩu";
            }
            else if (String.IsNullOrEmpty(sMatKhauLai))
            {
                ViewData["err4"] = "Mật khẩu nhập lại không hợp lệ";
            }
            else if (sMatKhau != sMatKhauLai)
            {
                ViewData["err4"] = "Mật khẩu nhập lại không khớp";
            }
            else if (String.IsNullOrEmpty(sEmail))
            {
                ViewData["err5"] = "Email không được rỗng";
            }
            else if (String.IsNullOrEmpty(sDienThoai))
            {
                ViewData["err6"] = "Số điện thoại không được rỗng";
            }
            else if (db.KHACHHANGs.SingleOrDefault(n => n.TaiKhoan == sTenDN) != null)
            {
                ViewBag.ThongBao = "Tên đăng nhập đã tồn tại";
            }
            else if (db.KHACHHANGs.SingleOrDefault(n => n.Email == sEmail) != null)
            {
                ViewBag.ThongBao = "Email đã được sử dụng";
            }
            else
            {
                // Gán giá trị cho đối tượng tạo mới (kh)
                kh.HoTen = sHoTen;
                kh.TaiKhoan = sTenDN;
                kh.MatKhau = sMatKhau;
                kh.Email = sEmail;
                kh.DiaChi = sDiaChi;
                kh.DienThoai = sDienThoai;
                kh.NgaySinh = DateTime.Parse(dNgaySinh);
                db.KHACHHANGs.Add(kh);
                db.SaveChanges();
                return RedirectToAction("DangNhap");
            }

            return this.DangKy();
        }
        public ActionResult DangXuat()
        {
            Session.Clear();
            return RedirectToAction("Index", "SachOnline");
        }
        public ActionResult ThongKe()
        {
            var kq = from s in db.SACHes.ToList()
                     join cd in db.CHUDEs on s.MaCD equals cd.MaCD
                     group s by new { cd.MaCD, cd.TenChuDe } into g
            orderby g.Key.MaCD
                     select new ReportInfo
                     {
                         Id = g.Key.MaCD.ToString(),
                         Name = g.Key.TenChuDe,
                         Count = g.Count(),
                         Sum = g.Sum(n => n.SoLuongBan),
                         Max = g.Max(n => n.SoLuongBan),
                         Min = g.Min(n => n.SoLuongBan),
                         Avg = Convert.ToDecimal(g.Average(n => n.SoLuongBan))
                     };
            return View(kq);
        }
        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Register(KHACHHANG kh, string MatKhauNL)
        {
            // Kiểm tra tính hợp lệ của các trường bắt buộc
            if (string.IsNullOrWhiteSpace(kh.HoTen))
            {
                ModelState.AddModelError("HoTen", "Họ tên không được để trống.");
            }
            if (string.IsNullOrWhiteSpace(kh.TaiKhoan))
            {
                ModelState.AddModelError("TaiKhoan", "Tên đăng nhập không được để trống.");
            }
            if (string.IsNullOrWhiteSpace(kh.MatKhau))
            {
                ModelState.AddModelError("MatKhau", "Mật khẩu không được để trống.");
            }
            if (MatKhauNL != kh.MatKhau)
            {
                ModelState.AddModelError("MatKhauNL", "Mật khẩu nhập lại không khớp.");
            }
            if (string.IsNullOrWhiteSpace(kh.Email))
            {
                ModelState.AddModelError("Email", "Email không được để trống.");
            }
            if (string.IsNullOrWhiteSpace(kh.DienThoai))
            {
                ModelState.AddModelError("DienThoai", "Điện thoại không được để trống.");
            }
            if (string.IsNullOrWhiteSpace(kh.DiaChi))
            {
                ModelState.AddModelError("DiaChi", "Địa chỉ không được để trống.");
            }

            // Lấy giá trị Ngày Sinh từ Request.Form
            if (!DateTime.TryParse(Request.Form["NgaySinh"], out DateTime ngaySinh))
            {
                ModelState.AddModelError("NgaySinh", "Ngày sinh không được để trống.");
            }
            else
            {
                kh.NgaySinh = ngaySinh;
            }

            // Kiểm tra nếu có lỗi
            if (!ModelState.IsValid)
            {
                return View("Register", kh); // Trả về view với lỗi nếu có
            }

            // Kiểm tra tên đăng nhập và email đã tồn tại
            if (db.KHACHHANGs.Any(n => n.TaiKhoan == kh.TaiKhoan))
            {
                ModelState.AddModelError("TaiKhoan", "Tên đăng nhập đã tồn tại.");
                return View("Register", kh);
            }
            if (db.KHACHHANGs.Any(n => n.Email == kh.Email))
            {
                ModelState.AddModelError("Email", "Email đã được sử dụng.");
                return View("Register", kh);
            }

            // Lưu vào CSDL
            db.KHACHHANGs.Add(kh);
            db.SaveChanges();

            // Gọi phương thức gửi email sau khi đăng ký thành công
          //  SendConfirmationEmail(kh.Email, kh.HoTen, kh.DienThoai, kh.TaiKhoan);

            // Chuyển hướng đến trang đăng nhập
            return RedirectToAction("DangNhap");
        }





    }
}
