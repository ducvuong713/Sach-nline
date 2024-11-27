using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LeDucVuong.SachOnline.Models;
using PagedList;
using PagedList.Mvc;
using System.Web.UI.WebControls;

namespace LeDucVuong.SachOnline.Controllers
{
    public class SachOnlineController : Controller
    {                       
        SachOnlineEntities db = new  SachOnlineEntities();
        private List<SACH> LaySachMoi(int count)
        {
            return db.SACHes.OrderByDescending(a => a.NgayCapNhat).Take(count).ToList();
        }
        public ActionResult Index(int? page)
        {
            int pageSize = 6; // Số lượng sách trên mỗi trang
            int pageNumber = (page ?? 1); // Trang hiện tại, mặc định là trang 1

            var listSachMoi = LaySachMoi(20); // Lấy danh sách sách
            return View(listSachMoi.ToPagedList(pageNumber, pageSize)); // Trả về danh sách phân trang
        }

        /// <summary>
        /// GetNav
        /// </summary>
        /// <returns>ReturnNav</returns>
        [ChildActionOnly]
        public ActionResult PartialNav()
        {
            List<MENU> lst = new List<MENU>();
            lst = db.MENUs.Where(m => m.ParentId == null).OrderBy(m => m.OrderNumber).ToList();
            int[] a = new int[lst.Count()];
            for (int i = 0; i < lst.Count; i++)
            {
                int id = lst[i].Id;
                List<MENU> l = db.MENUs.Where(m => m.ParentId == id).ToList();
                int k = l.Count();
                a[i] = k;
            }
            ViewBag.lst = a;
            return PartialView(lst);
        }
        [ChildActionOnly]
        public ActionResult LoadChildMenu(int parentID)
        {
            List<MENU> lst = new List<MENU>();
            lst = db.MENUs.Where(m => m.ParentId == parentID).OrderBy(m => m.OrderNumber).ToList();
            ViewBag.Count = lst.Count();
            int[] a = new int[lst.Count()];
            for (int i = 0; i < lst.Count; i++)
            {
                int id = lst[i].Id;
                List<MENU> l = db.MENUs.Where(m => m.ParentId == id).ToList();
                int k = l.Count();
                a[i] = k;
            }
            ViewBag.lst = a;
            return PartialView("LoadChildMenu", lst);
        }


        public ActionResult PartialSlider()
        {
            return PartialView("PartialSlider");
        }
        
        public ActionResult ChuDeNXBPartial()
        {
            var listNHAXUATBAN = from cd in db.NHAXUATBANs select cd;
            return PartialView(listNHAXUATBAN);
        }
        public ActionResult SachTheoNXB(int id, int? page)
        {
            ViewBag.MaNXB = id;
            ViewBag.NXB = db.NHAXUATBANs.Where(c => c.MaNXB == id).SingleOrDefault()?.TenNXB;

            int iSize = 2; // Số lượng sách trên mỗi trang
            int iPageNumber = (page ?? 1);

            // Sử dụng ToPagedList để trả về IPagedList
            var kq = db.SACHes.Where(s => s.MaCD == id).OrderBy(s => s.TenSach).ToPagedList(iPageNumber, iSize);

            return View(kq);  // Trả về IPagedList thay vì List
        }


        public ActionResult SachBanNhieuPartial()
        {

            var listSachMoi = LaySachMoi(6);
            return PartialView(listSachMoi);

        }
        public ActionResult FooterPartial()
        {
            return PartialView("FooterPartial");
        }
       
        public ActionResult ChuDePartial()
        {
            var listChuDe = from cd in db.CHUDEs select cd;
            return PartialView(listChuDe);
        }

        public ActionResult SachTheoChuDe(int id, int? page)
        {
            ViewBag.MaCD = id;
            ViewBag.ChuDe = db.CHUDEs.Where(c => c.MaCD == id).SingleOrDefault()?.TenChuDe;

            int iSize = 2; // Số lượng sách trên mỗi trang
            int iPageNumber = (page ?? 1);

            // Sử dụng ToPagedList để trả về IPagedList
            var kq = db.SACHes.Where(s => s.MaCD == id).OrderBy(s => s.TenSach).ToPagedList(iPageNumber, iSize);

            return View(kq);  // Trả về IPagedList thay vì List
        }

        public ActionResult ChiTietSach(int id)
        {
            var sach = from s in db.SACHes
                       where s.MaSach == id select s;
            return View(sach.Single());
        }
        public ActionResult LoginLogoutPartial()
        {
            return PartialView("LoginLogoutPartial");
        }
        public ActionResult TrangTin(string metatitle)
        {
            var tt = db.TRANGTINs.Single(t => t.MetaTitle == metatitle);
            return View(tt);
        }










    }
}