
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Web;
using System.Web.Mvc;
using PagedList;
using PagedList.Mvc;
using System.Web.Management;
using System.Linq.Dynamic;
using System.Linq.Expressions;
using Antlr.Runtime;
using Microsoft.Ajax.Utilities;
using System.Configuration;
using System.Net.Mail;
using System.Net;
using System.Net.NetworkInformation;
using System.Security.Policy;
using LeDucVuong.SachOnline.Models;
using LeDucVuong.SachOnline;

namespace LeDucVuong.SachOnline.Controllers
{
    public class GioHangController : Controller
    {
        // GET: NguyenLeTuanVuGioHang
        SachOnlineEntities db = new SachOnlineEntities();
        public ActionResult Index()
        {
            return View();
        }
        public List<GioHang> LayGioHang()
        {
            List<GioHang> lstGioHang = Session["GioHang"] as List<GioHang>;
            if (lstGioHang == null)
            {
                lstGioHang = new List<GioHang>();
                Session["GioHang"] = lstGioHang;
            }
            return lstGioHang;
        }
        public ActionResult ThemGioHang(int id, string url)
        {
            List<GioHang> lstGioHang = LayGioHang();
            GioHang sp = lstGioHang.Find(n => n.iMaSach == id);
            if (sp == null)
            {
                sp = new GioHang(id);
                lstGioHang.Add(sp);
            }
            else
            {
                sp.iSoLuong++;
            }
            return Redirect(url);
        }
        private int TongSoLuong()
        {
            int iTongSoLuong = 0;
            List<GioHang> lstGioHang = Session["GioHang"] as List<GioHang>;
            if (lstGioHang != null)
            {
                iTongSoLuong = lstGioHang.Sum(n => n.iSoLuong);
            }
            return iTongSoLuong;
        }
        private double TongTien()
        {
            double dTongTien = 0;
            List<GioHang> lstGioHang = Session["GioHang"] as List<GioHang>;
            if (lstGioHang != null)
            {
                dTongTien = lstGioHang.Sum(n => n.dThanhTien);
            }
            return dTongTien;
        }
        public ActionResult GioHang()
        {
            List<GioHang> lstGioHang = LayGioHang();
            if (lstGioHang.Count == 0)
            {
                return RedirectToAction("Index", "SachOnline");
            }
            ViewBag.TongSoLuong = TongSoLuong();
            ViewBag.TongTien = TongTien();
            return View(lstGioHang);
        }
        public ActionResult GioHangPartial()
        {
            ViewBag.TongSoLuong = TongSoLuong();
            ViewBag.TongTien = TongTien();
            return PartialView();
        }
        public ActionResult CapNhatGioHang(int iMaSach, FormCollection f)
        {
            List<GioHang> lstGioHang = LayGioHang();
            GioHang sp = lstGioHang.SingleOrDefault(n => n.iMaSach == iMaSach);
            if (sp != null)
            {
                sp.iSoLuong = int.Parse(f["txtSoLuong"].ToString());
            }
            return RedirectToAction("GioHang");
        }
        public ActionResult XoaGioHang()
        {
            List<GioHang> lstGioHang = LayGioHang();
            lstGioHang.Clear();
            return RedirectToAction("Index", "SachOnline");
        }
        public ActionResult XoaSPKhoiGioHang(int iMaSach)
        {
            List<GioHang> lstGioHang = LayGioHang();
            GioHang sp = lstGioHang.SingleOrDefault(n => n.iMaSach == iMaSach);
            if (sp != null)
            {
                lstGioHang.RemoveAll(n => n.iMaSach == iMaSach);
                if (lstGioHang.Count == 0)
                {
                    return RedirectToAction("Index", "SachOnline");
                }
            }
            return RedirectToAction("GioHang");
        }
        [HttpGet]
        public ActionResult DatHang()
        {
            if (Session["TaiKhoan"] == null || Session["TaiKhoan"].ToString() == "")
            {
                return RedirectToAction("DangNhap", "User");
            }
            if (Session["GioHang"] == null)
            {
                return RedirectToAction("Index", "SachOnline");
            }
            List<GioHang> lstGioHang = LayGioHang();
            ViewBag.TongSoLuong = TongSoLuong();
            ViewBag.TongTien = TongTien();
            return View(lstGioHang);
        }
        [HttpPost]
        public ActionResult DatHang(FormCollection f)
        {
            List<GioHang> lstGioHang = LayGioHang();
            ViewBag.TongSoLuong = TongSoLuong();
            ViewBag.TongTien = TongTien();
            string strThanhToan = f["thanhtoan"];
            switch (strThanhToan)
            {
                case "vivnpay":
                    // Xử lý thanh toán qua VNPay
                    return RedirectToAction("PaymentVNPay", "GioHang", new { ngayGiao = f["NgayGiao"] });
                case "vimomo":
                    // Xử lý thanh toán qua VNPay
                    return RedirectToAction("PaymentMomo", "GioHang");
                case "nhanhang":
                    // Xử lý thanh toán khi nhận hàng
                    LuuDonHang(f["NgayGiao"], false);
                    return RedirectToAction("ConfrimOrder", "GioHang");
                default:
                    // Xử lý cho trường hợp không hợp lệ
                    break;
            }
            return View(lstGioHang);
        }
        public ActionResult XacNhanDonHang()
        {
            return View();
        }
        private void SendOrderConfirmationEmail(string email, string name, string phoneNumber, int orderId, string orderDetails)
        {
            // Lấy thông tin từ web.config
            string emailUsername = ConfigurationManager.AppSettings["EmailUsername"];
            string emailPassword = ConfigurationManager.AppSettings["EmailPassword"];
            string smtpHost = ConfigurationManager.AppSettings["SmtpHost"];
            int smtpPort = int.Parse(ConfigurationManager.AppSettings["SmtpPort"]);

            var mail = new SmtpClient(smtpHost, smtpPort)
            {
                Credentials = new NetworkCredential(emailUsername, emailPassword),
                EnableSsl = true
            };

            var message = new MailMessage
            {
                From = new MailAddress(emailUsername),
                Subject = "Xác nhận đơn hàng",
                Body = $@"
        <html>
        <body>
            <h1 style='color: red;'>Xin chào {name}</h1>
            <p>Bạn đã đặt hàng thành công.</p>
            <p><b>Mã đơn hàng:</b> {orderId}</p>
            <p><b>Thông tin chi tiết đơn hàng:</b></p>
            {orderDetails} <!-- Sử dụng bảng chi tiết đơn hàng ở đây -->
        </body>
        </html>",
                IsBodyHtml = true
            };

            message.To.Add(new MailAddress(email));

            // Gửi email
            try
            {
                mail.Send(message);
            }
            catch (Exception ex)
            {
                // Xử lý ngoại lệ nếu có lỗi xảy ra
                Console.WriteLine("Có lỗi khi gửi email: " + ex.Message);
            }
        }
        private string GenerateOrderDetails(List<GioHang> lstGioHang)
        {
            double totalAmount = lstGioHang.Sum(item => item.dDonGia * item.iSoLuong); // Tính tổng giá tiền

            string orderDetails = @"
    <table style='width:100%; border-collapse: collapse;'>
        <tr>
            <th style='border: 1px solid black; padding: 8px;'>Tên sách</th>
            <th style='border: 1px solid black; padding: 8px;'>Số lượng</th>
            <th style='border: 1px solid black; padding: 8px;'>Đơn giá (VNĐ)</th>
            <th style='border: 1px solid black; padding: 8px;'>Tổng (VNĐ)</th>
        </tr>";

            foreach (var item in lstGioHang)
            {
                double itemTotal = item.dDonGia * item.iSoLuong; // Tính tổng giá cho từng mặt hàng
                orderDetails += $@"
        <tr>
            <td style='border: 1px solid black; padding: 8px;'>{item.sTenSach}</td>
            <td style='border: 1px solid black; padding: 8px;'>{item.iSoLuong}</td>
            <td style='border: 1px solid black; padding: 8px;'>{item.dDonGia} VNĐ</td>
            <td style='border: 1px solid black; padding: 8px;'>{itemTotal} VNĐ</td>
        </tr>";
            }

            orderDetails += $@"
        <tr>
            <td colspan='3' style='border: 1px solid black; padding: 8px; text-align: right;'><strong>Tổng cộng:</strong></td>
            <td style='border: 1px solid black; padding: 8px;'><strong>{totalAmount} VNĐ</strong></td>
        </tr>
    </table>";

            return orderDetails;
        }
        public ActionResult PaymentVNPay(int? id, string ngayGiao)
        {
            string url = ConfigurationManager.AppSettings["Url"];
            string returnUrl = ConfigurationManager.AppSettings["ReturnUrl"];
            string tmnCode = ConfigurationManager.AppSettings["TmnCode"];
            string hashSecret = ConfigurationManager.AppSettings["HashSecret"];
            PayLib pay = new PayLib();
            pay.AddRequestData("vnp_Version", "2.1.0"); //Phiên bản api mà merchant kết nối.Phiên bản hiện tại là 2.1.0
            pay.AddRequestData("vnp_Command", "pay"); //Mã API sử dụng, mã cho giao dịch thanh toán là 'pay'
            pay.AddRequestData("vnp_TmnCode", tmnCode); //Mã website của merchant trên hệ thống của VNPAY(khi đăng ký tài khoản sẽ có trong mail VNPAY gửi về)
            pay.AddRequestData("vnp_Amount", TongTien().ToString() + "00"); //số tiền cần thanh toán, công thức: số tiền *100 - ví dụ 10.000(mười nghìn đồng)-- > 1000000 
            //TotalAmount() là phương thức trả về tổng tiền của đơn hàng.
            pay.AddRequestData("vnp_BankCode", "NCB"); //Mã Ngân hàng thanh toán (tham khảo: https://sandbox.vnpayment.vn/apis/danh-sach-ngan-hang/), có thể để trống, người dùng có thể chọn trên cổng thanh toán VNPAY
            pay.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss")); //ngày thanh toán theo định dạng yyyyMMddHHmmss
            pay.AddRequestData("vnp_CurrCode", "VND"); //Đơn vị tiền tệ sử dụng thanh toán. Hiện tại chỉ hỗ trợ VND hàng thực hiện giao dịch
            pay.AddRequestData("vnp_IpAddr", Util.GetIpAddress());
            pay.AddRequestData("vnp_Locale", "vn"); //Ngôn ngữ giao diện hiển thị - Tiếng Việt(vn), Tiếng Anh(en)
            pay.AddRequestData("vnp_OrderInfo", "Thanh toan don hang"); //Thông tin mô tả nội dung thanh toán
            pay.AddRequestData("vnp_OrderType", "other"); //topup: Nạp tiền điện thoại - billpayment: Thanh toán hóa đơn -fashion: Thời trang -other: Thanh toán trực tuyến
            pay.AddRequestData("vnp_ReturnUrl", returnUrl); //URL thông báo kết quả giao dịch khi Khách hàng kết thúc thanh toán
            pay.AddRequestData("vnp_TxnRef", DateTime.Now.Ticks.ToString()); //mã hóa đơn
            string paymentUrl = pay.CreateRequestUrl(url, hashSecret);
            LuuDonHang(ngayGiao, true); //Lưu đơn hàng với trạng thái đã thanh toán là true
            return Redirect(paymentUrl);
        }
        public ActionResult PaymentConfirm()
        {
            if (Request.QueryString.Count > 0)
            {
                string hashSecret = ConfigurationManager.AppSettings["HashSecret"];
                var vnpayData = Request.QueryString;
                PayLib pay = new PayLib();
                //lấy toàn bộ dữ liệu được trả về
                foreach (string s in vnpayData)
                {
                    if (!string.IsNullOrEmpty(s) && s.StartsWith("vnp_"))
                    {
                        pay.AddResponseData(s, vnpayData[s]);
                    }
                }
                long orderId = Convert.ToInt64(pay.GetResponseData("vnp_TxnRef")); //mã hóa đơn
                long vnpayTranId = Convert.ToInt64(pay.GetResponseData("vnp_TransactionNo")); //mã giao dịch tại hệ thống VNPAY
                string vnp_ResponseCode = pay.GetResponseData("vnp_ResponseCode");
                //response code: 00 - thành công, khác 00 - xem thêm https://sandbox.vnpayment.vn/apis/docs/bang-ma-loi/
                string vnp_SecureHash = Request.QueryString["vnp_SecureHash"]; //hash của dữ liệu trả về
                bool checkSignature = pay.ValidateSignature(vnp_SecureHash, hashSecret);
                //check chữ ký đúng hay không?
                if (checkSignature)
                {
                    if (vnp_ResponseCode == "00")
                    {
                        //Thanh toán thành công
                        ViewBag.Message = "Thanh toán thành công hóa đơn " + orderId + " | Mã giao dịch: " + vnpayTranId;
                    }
                    else
                    {
                        //Thanh toán không thành công. Mã lỗi: vnp_ResponseCode
                        ViewBag.Message = "Có lỗi xảy ra trong quá trình xử lý hóa đơn " + orderId + " | Mã giao dịch: " + vnpayTranId + " | Mã lỗi: " + vnp_ResponseCode;
                    }
                }
                else
                {
                    ViewBag.Message = "Có lỗi xảy ra trong quá trình xử lý";
                }
            }
            return View();
        }
        private void LuuDonHang(string NgayGiao, Boolean daThanhToan)
        {
            //Thêm đơn hàng
            DONDATHANG ddh = new DONDATHANG();
            KHACHHANG kh = (KHACHHANG)Session["TaiKhoan"];
            List<GioHang> lstGioHang = LayGioHang();
            ddh.MaKH = kh.MaKH;
            ddh.NgayDat = DateTime.Now;
            var ngayGiao = String.Format("{0:MM/dd/yyyy}", NgayGiao);
            ddh.NgayGiao = DateTime.Parse(ngayGiao);
            ddh.TinhTrangGiaoHang = 1;
            ddh.DaThanhToan = daThanhToan;
            db.DONDATHANGs.Add(ddh);
            db.SaveChanges();
            //Thêm chi tiết đơn hàng
            foreach (var item in lstGioHang)
            {
                CHITIETDATHANG ctdh = new CHITIETDATHANG();
                ctdh.MaDonHang = ddh.MaDonHang;
                ctdh.MaSach = item.iMaSach;
                ctdh.SoLuong = item.iSoLuong;
                ctdh.DonGia = (decimal)item.dDonGia;
                db.CHITIETDATHANGs.Add(ctdh);
            }
            db.SaveChanges();
            Session["GioHang"] = null;
        }

    }
}