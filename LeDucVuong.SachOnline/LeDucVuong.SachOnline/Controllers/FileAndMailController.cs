using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using LeDucVuong.SachOnline.Models;
using System.Net.Mail;
using System.Net;
using System.Net.Mime;



namespace LeDucVuong.SachOnline.Controllers
{
    public class FileAndMailController : Controller
    {
        // GET: FileAndMail

        public ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public ActionResult SendMail()
        {
            return View();
        }
        [HttpPost]
        public ActionResult SendMail(Mail model)
        {
            // Cấu hình thông tin email (khai báo thư viện System.Net)
            var mail = new SmtpClient("smtp.gmail.com", 587)
            {
                // Khai báo thư viện System.Net
                Credentials = new NetworkCredential("2224802010753@student.tdmu.edu.vn", "hfrt tewf yeed uirr"),
                // your email (abc@gmail.com) and your password (*****)
                EnableSsl = true
            };

            // Tạo email
            var message = new MailMessage();
            message.From = new MailAddress(model.From);
            message.ReplyToList.Add(model.From);
            message.To.Add(new MailAddress(model.To));
            message.Subject = model.Subject;
            message.Body = model.Notes;

            var f = Request.Files["attachment"];
            var path = Path.Combine(Server.MapPath("~/Upload"), f.FileName);
            if (!System.IO.File.Exists(path))
            {
                f.SaveAs(path);
            }

            // Khai báo thư viện System.Net.Mime
            Attachment data = new Attachment(Server.MapPath("~/Upload/" + f.FileName), MediaTypeNames.Application.Octet);
            message.Attachments.Add(data);

            // Gửi email
            mail.Send(message);

            return View("SendMail");
        }

    }
}