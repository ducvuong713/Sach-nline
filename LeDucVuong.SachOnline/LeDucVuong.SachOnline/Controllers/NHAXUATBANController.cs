using LeDucVuong.SachOnline.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LeDucVuong.SachOnline.Controllers
{
    public class NHAXUATBANController : Controller
    {
        // GET: NHAXUATBAN
        SachOnlineEntities db = new SachOnlineEntities();
        // GET: NHAXUATBAN
        public ActionResult Index()
        {
            var data = db.NHAXUATBANs.ToList();
            return View(data);
        }
        public ActionResult Details(int id)
        {
            int manxb = int.Parse(Request.QueryString["id"]);
            var data = db.NHAXUATBANs.Where(nxb => nxb.MaNXB == id).SingleOrDefault();
            return View(data);
        }
        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Luu(FormCollection f)
        {
            NHAXUATBAN nxb = new NHAXUATBAN();
            nxb.TenNXB = f["TenNXB"];
            nxb.DiaChi = f["DiaChi"];
            nxb.DienThoai = f["DienThoai"];
            db.NHAXUATBANs.Add(nxb);
            db.SaveChanges();
            return View("Create");
        }
        [HttpPost]
        public ActionResult Create(NHAXUATBAN nxb)
        {
            db.NHAXUATBANs.Add(nxb);
            db.SaveChanges();
            return RedirectToAction("Index", "NHAXUATBAN");
        }
        [HttpGet]
        public ActionResult Edit(int id)
        {
            var data = db.NHAXUATBANs.Where(nxb => nxb.MaNXB == id).SingleOrDefault();
            return View(data);
        }
        [HttpPost]
        public ActionResult Edit(FormCollection f)
        {
            int maNXB = int.Parse(f["MaNXB"]);
            NHAXUATBAN nxb = db.NHAXUATBANs.Where(n => n.MaNXB == maNXB).SingleOrDefault();
            nxb.TenNXB = f["TenNXB"];
            nxb.DiaChi = f["DiaChi"];
            nxb.DienThoai = f["DienThoai"];
            db.SaveChanges();
            return RedirectToAction("Index", "NHAXUATBAN");
        }
    }
}