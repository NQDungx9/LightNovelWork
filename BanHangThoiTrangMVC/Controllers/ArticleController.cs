using BanHangThoiTrangMVC.Models;
using BanHangThoiTrangMVC.Models.EF;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BanHangThoiTrangMVC.Controllers
{
    public class ArticleController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Article
        public ActionResult Index(int? page)
        {
            var pageSize = 5;
            if (page == null)
            {
                page = 1;
            }
            IEnumerable<Posts> items = db.Posts.OrderByDescending(x => x.CreateDate);
            var pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            items = items.ToPagedList(pageIndex, pageSize);
            ViewBag.PageSize = pageSize;
            ViewBag.Page = page;
            return View(items);
        }

        public ActionResult Detail(int id)
        {
            var item = db.Posts.Find(id);
            return View(item);
        }

        public ActionResult Partial_Posts_Home()
        {
            var items = db.Posts.Take(3).ToList();
            return PartialView(items);
        }
    }
}