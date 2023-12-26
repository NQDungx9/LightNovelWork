using BanHangThoiTrangMVC.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BanHangThoiTrangMVC.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class StatisticalController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Admin/Statistical
        public ActionResult Index()
        {
            return View();
        }

        /*[HttpGet]*/
        /*public ActionResult GetStatistical(string fromDate, string toDate)
        {
            var query = from o in db.Orders
                        join od in db.OrderDetails
                        on o.Id equals od.OrderId
                        join p in db.Products
                        on od.ProductId equals p.Id
                        select new
                        {
                            CreatedDate = o.CreateDate,
                            Quantity = od.Quantity,
                            Price = od.Price,
                            OriginalPrice = p.OriginalPrice
                        };
            if (!string.IsNullOrEmpty(fromDate))
            {
                DateTime startDate = DateTime.ParseExact(fromDate, "dd/MM/yyyy", null);
                query = query.Where(x => x.CreatedDate >= startDate);
            }
            if (!string.IsNullOrEmpty(toDate))
            {
                DateTime endDate = DateTime.ParseExact(toDate, "dd/MM/yyyy", null);
                query = query.Where(x => x.CreatedDate < endDate);
            }

            var result = query.GroupBy(x => DbFunctions.TruncateTime(x.CreatedDate)).Select(x => new
            {
                Date = x.Key.Value,
                TotalBuy = x.Sum(y => y.Quantity * y.OriginalPrice),
                TotalSell = x.Sum(y => y.Quantity * y.Price),
                OrderCount = x.Count()
            }).Select(x => new
            {
                Date = x.Date,
                DoanhThu = x.TotalSell,
                LoiNhuan = x.TotalSell - x.TotalBuy,
                OrderCount = x.OrderCount
            });
            decimal totalDoanhThu = result.Sum(x => x.DoanhThu);
            decimal totalLoiNhuan = result.Sum(x => x.LoiNhuan);
            decimal totalOrderCount = result.Sum(x => x.OrderCount);
            return Json(new { Data = result, TotalDoanhThu = totalDoanhThu, TotalLoiNhuan = totalLoiNhuan, TotalOrderCount = totalOrderCount }, JsonRequestBehavior.AllowGet);
        }*/
        /*[HttpGet]
        public ActionResult GetStatistical(string fromDate, string toDate)
        {
            var query = from o in db.Orders
                        join od in db.OrderDetails on o.Id equals od.OrderId
                        join p in db.Products on od.ProductId equals p.Id
                        where o.TotalAmount > 0
                        select new
                        {
                            CreatedDate = o.CreateDate,
                            Quantity = od.Quantity,
                            Price = od.Price,
                            OriginalPrice = p.OriginalPrice,
                            ProductName = p.Id // Để có thông tin sản phẩm
                        };

            if (!string.IsNullOrEmpty(fromDate))
            {
                DateTime startDate = DateTime.ParseExact(fromDate, "dd/MM/yyyy", null);
                query = query.Where(x => x.CreatedDate >= startDate);
            }

            if (!string.IsNullOrEmpty(toDate))
            {
                DateTime endDate = DateTime.ParseExact(toDate, "dd/MM/yyyy", null);
                query = query.Where(x => x.CreatedDate < endDate);
            }

            var result = query.GroupBy(x => DbFunctions.TruncateTime(x.CreatedDate)).Select(x => new
            {
                Date = x.Key.Value,
                TotalBuy = x.Sum(y => y.Quantity * y.OriginalPrice),
                TotalSell = x.Sum(y => y.Quantity * y.Price),
                OrderCount = x.Count(),
                ProductCount = x.Count() // Số lượng sản phẩm
            }).Select(x => new
            {
                Date = x.Date,
                DoanhThu = x.TotalSell,
                LoiNhuan = x.TotalSell - x.TotalBuy,
                OrderCount = x.OrderCount,
                ProductCount = x.ProductCount // Số lượng sản phẩm
            });

            decimal totalDoanhThu = result.Sum(x => x.DoanhThu);
            decimal totalLoiNhuan = result.Sum(x => x.LoiNhuan);
            decimal totalOrderCount = result.Sum(x => x.OrderCount);
            decimal totalProductCount = result.Sum(x => x.ProductCount);
            return Json(new
            {
                Data = result,
                TotalDoanhThu = totalDoanhThu,
                TotalLoiNhuan = totalLoiNhuan,
                TotalOrderCount = totalOrderCount,
                TotalProductCount = totalProductCount 
            }, JsonRequestBehavior.AllowGet);
        }*/

        [HttpGet]
        public ActionResult GetStatistical(string fromDate, string toDate)
        {
            var query = from o in db.Orders
                        join od in db.OrderDetails on o.Id equals od.OrderId
                        join p in db.Products on od.ProductId equals p.Id
                        orderby o.CreateDate ascending
                        where o.TotalAmount > 0 &&  od.Price > 0
                        select new
                        {
                            CreatedDate = o.CreateDate,
                            Quantity = od.Quantity,
                            Price = od.Price,
                            OriginalPrice = p.OriginalPrice,
                        };

            if (!string.IsNullOrEmpty(fromDate))
            {
                DateTime startDate = DateTime.ParseExact(fromDate, "dd/MM/yyyy", null);
                query = query.Where(x => x.CreatedDate >= startDate);
            }

            if (!string.IsNullOrEmpty(toDate))
            {
                DateTime endDate = DateTime.ParseExact(toDate, "dd/MM/yyyy", null);
                query = query.Where(x => x.CreatedDate < endDate);
            }

            // Tạo truy vấn riêng để lấy danh sách sản phẩm từ bảng Products
            var productQuery = db.Products.Select(p => p.Id);
            var orderCount = db.Orders.Select(o => o.Id);

            var result = query.GroupBy(x => DbFunctions.TruncateTime(x.CreatedDate)).Select(x => new
            {
                Date = x.Key.Value,
                TotalBuy = x.Sum(y => y.Quantity * y.OriginalPrice),
                TotalSell = x.Sum(y => y.Quantity * y.Price),
                OrderCount = orderCount.Count(),
                ProductCount = productQuery.Count() // Đếm số sản phẩm trong bảng Products
            }).Select(x => new
            {
                Date = x.Date,
                DoanhThu = x.TotalSell,
                LoiNhuan = x.TotalSell - x.TotalBuy,
                OrderCount = x.OrderCount,
                ProductCount = x.ProductCount // Số lượng sản phẩm riêng
            });

            decimal totalDoanhThu = result.Sum(x => x.DoanhThu);
            decimal totalLoiNhuan = result.Sum(x => x.LoiNhuan);
            decimal totalOrderCount = result.Sum(x => x.OrderCount);
            decimal totalProductCount = result.Sum(x => x.ProductCount);
            return Json(new
            {
                Data = result,
                TotalDoanhThu = totalDoanhThu,
                TotalLoiNhuan = totalLoiNhuan,
                TotalOrderCount = totalOrderCount,
                TotalProductCount = totalProductCount
            }, JsonRequestBehavior.AllowGet);
        }
    }
}