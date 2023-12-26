using BanHangThoiTrangMVC.Models;
using BanHangThoiTrangMVC.Models.EF;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace BanHangThoiTrangMVC.Controllers
{
    public class NewsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: News
        public ActionResult Index(string Searchtext, int? page)
        {
            /*var pageSize = 5;
            if (page == null)
            {
                page = 1;
            }
            IEnumerable<News> items = db.News.OrderByDescending(x => x.CreateDate);
            var pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            items = items.ToPagedList(pageIndex, pageSize);
            ViewBag.PageSize = pageSize;
            ViewBag.Page = page;
            return View(items);*/
            var pageSize = 5;
            if (page == null)
            {
                page = 1;
            }
            IEnumerable<News> items = db.News.OrderByDescending(x => x.Id);
            if (!string.IsNullOrEmpty(Searchtext))
            {
                Searchtext = Searchtext.ToLower(); // Chuyển đổi searchText về lower case
                items = items.Where(x => x.Alias.ToLower().Contains(Searchtext) || x.Title.ToLower().Contains(Searchtext));
            }
            var pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            items = items.ToPagedList(pageIndex, pageSize);
            ViewBag.PageSize = pageSize;
            ViewBag.Page = page;
            return View(items);
        }

        public ActionResult Detail(int id)
        {
            var item = db.News.Find(id);
            item.ViewCount = item.ViewCount + 1;
            db.Entry(item).Property(x => x.ViewCount).IsModified = true;
            db.SaveChanges();
            return View(item);
        }

        public ActionResult Partial_News_Home()
        {
            var items = db.News.Take(3).ToList();
            return PartialView(items);
        }
    }
}