using LeDucVuong.SachOnline.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Linq.Dynamic;
using System.Linq.Expressions;

namespace LeDucVuong.SachOnline.Controllers
{
    public class LeDucVuongSearchController : Controller

    {
        SachOnlineEntities db = new SachOnlineEntities();
        // GET: Search
        public ActionResult Search(string strSearch = "")
        {
            ViewBag.strSearch = strSearch;
            var kq = db.SACHes.Where(s => s.TenSach.Contains(strSearch)).ToList();
            return View(kq);
        }
        public ActionResult PhanTrang(int? page, string strSearch = null)
        {
            ViewBag.Search = strSearch;
            if (!string.IsNullOrEmpty(strSearch))
            {
                int iSize = 3;
                int iPageNumber = (page ?? 1);
                var kq = (from s in db.SACHes
                          where s.TenSach.Contains(strSearch) ||
                s.MoTa.Contains(strSearch)
                          select s).ToList();
                return View(kq.ToPagedList(iPageNumber, iSize));
            }
            return View();

        }
        public ActionResult SearchPhanTrangSapXep(int? page, string sortProperty, string sortOrder = "", string strSearch = null)
        {
            ViewBag.Search = strSearch;
            if (!string.IsNullOrEmpty(strSearch))
            {
                int iSize = 3;
                int iPageNumber = (page ?? 1);
                // Gián giá trị cho biến sortOrder
                if (sortOrder == "") ViewBag.SortOrder = "desc";
                if (sortOrder == "desc") ViewBag.SortOrder = "";
                if (sortOrder == "") ViewBag.SortOrder = "asc";
                // Tạo thuộc tính sắp xếp mặc định là "Tên Sách"
                if (String.IsNullOrEmpty(sortProperty))
                    sortProperty = "TenSach";
                // Gián giá trị cho biến sortProperty
                ViewBag.SortProperty = sortProperty;
                // Truy vấn
                var kq = from s in db.SACHes
                         where s.TenSach.Contains(strSearch) ||
                s.MoTa.Contains(strSearch)
                         select s;
                //Sắp xếp tăng/giảm bằng phương thức OrderBy sử dụng trong thư viện Dynamic LINQ
                //if (sortOrder == "desc")
                kq = kq.OrderBy(sortProperty + " " + sortOrder);
                //else
                // kqkq. OrderBy (sortProperty);
                return View(kq.ToPagedList(iPageNumber, iSize));
            }
            return View();
        }
        public ActionResult SearchPhanTrangTuyChon(int? size, int? page, string strSearch = null)
        {
            //1 List để lấy nguồn cho Combobox chọn số lượng sản phẩm
            List<SelectListItem> items = new List<SelectListItem>();
            items.Add(new SelectListItem { Text = "3", Value = "3" });
            items.Add(new SelectListItem { Text = "5", Value = "5" });
            items.Add(new SelectListItem { Text = "10", Value = "10" });
            items.Add(new SelectListItem { Text = "20", Value = "20" });
            items.Add(new SelectListItem { Text = "25", Value = "25" });
            items.Add(new SelectListItem { Text = "50", Value = "50" });
            //1.1 Giữ trạng thái kích thước trang được chọn trên DropDownList
            foreach (var item in items)
            {
                if (item.Value == size.ToString()) item.Selected = true;
            }
            //1.2. Tạo các biến ViewBag
            ViewBag.size = items; // ViewBag DropDownList
            ViewBag.currentSize = size; // tạo biến kích thước trang hiện tại
            ViewBag.Search = strSearch;

            int iSize = (size ?? 3); //Mặc định 3 item trên 1 page
            int iPageNumber = (page ?? 1);

            if (!string.IsNullOrEmpty(strSearch))
            {
                var kq = (from s in db.SACHes
                          where s.TenSach.Contains(strSearch) || s.MoTa.Contains(strSearch)
                          select s).ToList();
                return View(kq.ToPagedList(iPageNumber, iSize));
            }
            return View();
        }
        public ActionResult SearchTheoDanhMuc(string strSearch = null, int maCD = 0)
        {
            // 1. Lưu từ khóa tìm kiếm ViewBag.Search = strSearch;

            //2.Tạo câu truy cơ bản
            var kq = db.SACHes.Select(b => b);
            //3. Tìm kiếm theo searchString
            if (!String.IsNullOrEmpty(strSearch))
                kq = kq.Where(b => b.TenSach.Contains(strSearch));

            //4. Tìm kiếm theo MaCD if (maCD != 0)
            {
                kq = kq.Where(b => b.CHUDE.MaCD == maCD);
            }
            //5. Tạo danh sách danh mục để hiển thị ở giao diện View thông qua DropDownList


            ViewBag.MaCD = new SelectList(db.CHUDEs, "MaCD", "TenChuDe"); // danh sách chủ đề
                                                                         //ViewBag.cd = db.CHUDEs.ToList();
            return View(kq.ToList());
        }
        public ActionResult Group()
        {
            //var kq = from s in db.SACHes group s by s.MaCD;
            var kq = db.SACHes.GroupBy(s => s.MaCD);

            return View(kq);
        }
    }
}