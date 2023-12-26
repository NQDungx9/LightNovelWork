using BanHangThoiTrangMVC.Models;
using BanHangThoiTrangMVC.Models.EF;
using Microsoft.AspNet.Identity.Owin;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BanHangThoiTrangMVC.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public OrderController()
        {
        }

        public OrderController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }


        // GET: Order
        public ActionResult Index(int? page)
        {
            /*var currentTime = DateTime.Now;
            var oneMinuteAgo = currentTime.AddMinutes(-2);*/
            /*            var items = db.Orders.Where(o => o.CreateDate >= oneMinuteAgo && o.CreateDate <= currentTime).ToList();
            */
            var user = UserManager.FindByNameAsync(User.Identity.Name).Result;
            if (user != null)
            {
                IEnumerable<Order> userOrders = db.Orders
                    .Where(x => x.Email == user.Email) // Giả sử có một trường UserId trong đối tượng Order
                    .OrderByDescending(x => x.CreateDate)
                    .Where(x => x.TotalAmount > 0)
                    .ToList();
                if (page == null)
                {
                    page = 1;
                }
                var pageNumber = page ?? 1;
                var pageSize = 5;
                ViewBag.PageSize = pageSize;
                ViewBag.Page = pageNumber;
                return View(userOrders.ToPagedList(pageNumber, pageSize));
            }
            else
            {
                return View();
            }
            /*IEnumerable<Order> items = db.Orders.OrderByDescending(x => x.CreateDate).ToList();
            items = items.Where(x => x.TotalAmount > 0);*/

        }

        [HttpPost]
        public ActionResult CancelOrder(int id, int trangthai)
        {
            /*var item = db.Orders.Find(id);
            if (item != null)
            {
                item.Status = 4;
                db.Entry(item).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return Json(new { success = true, CancelOrder = item.Status });
            }
            return Json(new { success = false });*/


            var item = db.Orders.Find(id);
            if (item != null)
            {
                db.Orders.Attach(item);
                item.Status = trangthai;
                db.Entry(item).Property(x => x.Status).IsModified = true;
                db.SaveChanges();
                return Json(new { message = "Success", Success = true });
            }
            return Json(new { message = "UnSuccess", Success = false });
        }
        public ActionResult ViewOrder(int id)
        {
            var item = db.Orders.Find(id);
            return View(item);
        }

        public ActionResult Partial_SanPham(int id)
        {
            var items = db.OrderDetails.Where(x => x.OrderId == id).ToList();
            return PartialView(items);
        }
    }
}