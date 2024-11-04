using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PagedList;
using System.Web.Mvc;
using System.Data.Entity;
using QLBanHang.Models;


namespace QLBanHang.Controllers
{
    public class HomeController : Controller
    {
        qlbanhangEntities db = new qlbanhangEntities();
       

        public ActionResult Index(string currentFilter, int?page, int maloaisp = 0, string SearchString = "")
        {
            

            if (SearchString !="")
            {
                page = 1;
                var sanPhams = db.SanPhams.Include(s => s.LoaiSP).Where(x => x.TenSP.ToUpper().Contains(SearchString.ToUpper()));
                return View(sanPhams.ToList());
            }
            else
            {
                SearchString = currentFilter;
            }
            ViewBag.CurrentFilter = currentFilter;
             if (maloaisp == 0)
            {
                int pageSize = 12;
                int pageNumber = (page ?? 1);
                var sanPhams = db.SanPhams.Include(s => s.LoaiSP).OrderBy(x =>x.TenSP);
               // Phải sắp xếp trước khi skip
                  var pagedList = sanPhams.ToPagedList(pageNumber, pageSize);
                return View(pagedList);
                
            }  
            else
            {
                var sanPhams = db.SanPhams.Include(s => s.LoaiSP).Where(x => x.MaLoaiSP == maloaisp).OrderBy(x => x.MaSP);
                return View(sanPhams.ToList());
            }    
            
            //else if (maloaisp == 0)
            //{
            //    int pageSize = 12;
            //    int pageNumber = (page ?? 1);
            //    sanPhams = sanPhams.Where(x => x.MaLoaiSP == maloaisp).OrderBy(x => x.MaSP);
            //    // Phải sắp xếp trước khi skip
            //    var pagedList = sanPhams.ToPagedList(pageNumber, pageSize);
            //    return View(pagedList);
            //}

            //var sanPhamList = sanPhams.ToList();

            //if (sanPhamList.Count == 0)
            //{
            //    // Trả về view mặc định nếu không có kết quả nào được tìm thấy
            //    return View("NoResults");
            //}

            //return View(sanPhamList);
    }



    public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}