using BanHangThoiTrangMVC.Models;
using BanHangThoiTrangMVC.Models.EF;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using PagedList;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BanHangThoiTrangMVC.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin, Employee")]
    public class ProductsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Admin/Products
        public ActionResult Index(string Searchtext, int? page)
        {
            IEnumerable<Product> items = db.Products.OrderByDescending(x => x.Id);
            var pageSize = 5;
            if (page == null)
            {
                page = 1;
            }
            if (!string.IsNullOrEmpty(Searchtext))
            {
                items = items.Where(x => x.Alias.ToLower().Contains(Searchtext) || x.Title.ToLower().Contains(Searchtext));
            }
            var pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            items = items.ToPagedList(pageIndex, pageSize);
            ViewBag.PageSize = pageSize;
            ViewBag.Page = page;
            return View(items);
        }

        public ActionResult Add()
        {
            ViewBag.ProductCategory = new SelectList(db.ProductCategories.ToList(), "Id", "Title");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(Product model, List<string> Images, List<int> rDefault)
        {
            if (ModelState.IsValid)
            {
                if (Images != null && Images.Count > 0)
                {
                    for (int i = 0; i < Images.Count; i++)
                    {
                        if (i + 1 == rDefault[0])
                        {
                            model.Image = Images[i];
                            model.ProductImages.Add(new ProductImage
                            {

                                ProductId = model.Id,
                                Image = Images[i],
                                IsDefault = true
                            });
                        }
                        else
                        {
                            model.ProductImages.Add(new ProductImage
                            {
                                ProductId = model.Id,
                                Image = Images[i],
                                IsDefault = false
                            });
                        }
                    }
                }
                model.CreateDate = DateTime.Now;
                model.ModifiedDate = DateTime.Now;
                if (string.IsNullOrEmpty(model.SeoTitle))
                {
                    model.SeoTitle = model.Title;
                }
                if (string.IsNullOrEmpty(model.Alias))
                    model.Alias = BanHangThoiTrangMVC.Models.Common.Filter.FilterChar(model.Title);
                db.Products.Add(model);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ProductCategory = new SelectList(db.ProductCategories.ToList(), "Id", "Title");
            return View(model);
        }
        public ActionResult Edit(int id)
        {
            ViewBag.ProductCategory = new SelectList(db.ProductCategories.ToList(), "Id", "Title");
            var item = db.Products.Find(id);
            return View(item);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Product model)
        {
            if (ModelState.IsValid)
            {
                model.CreateDate = DateTime.Now;
                model.ModifiedDate = DateTime.Now;
                model.Alias = BanHangThoiTrangMVC.Models.Common.Filter.FilterChar(model.Title);
                db.Products.Attach(model);
                TempData["success"] = "Chỉnh sửa thành công";
                db.Entry(model).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("index");
            }
            return View(model);
        }



        [HttpPost]
        public ActionResult Delete(int id)
        {
            var item = db.Products.Find(id);
            if (item != null)
            {
                db.Products.Remove(item);
                db.SaveChanges();
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }


        [HttpPost]
        public ActionResult DeleteAll(string ids)
        {
            if (!string.IsNullOrEmpty(ids))
            {
                var items = ids.Split(',');
                if (items != null && items.Any())
                {
                    foreach (var item in items)
                    {
                        var obj = db.Products.Find(Convert.ToInt32(item));
                        db.Products.Remove(obj);
                        db.SaveChanges();
                    }
                }
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }

        [HttpPost]
        public ActionResult IsActive(int id)
        {
            var item = db.Products.Find(id);
            if (item != null)
            {
                item.IsActive = !item.IsActive;
                db.Entry(item).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return Json(new { success = true, IsActive = item.IsActive });
            }
            return Json(new { success = false });
        }

        [HttpPost]
        public ActionResult IsHome(int id)
        {
            var item = db.Products.Find(id);
            if (item != null)
            {
                item.IsHome = !item.IsHome;
                db.Entry(item).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return Json(new { success = true, IsHome = item.IsHome });
            }
            return Json(new { success = false });
        }

        [HttpPost]
        public ActionResult IsSale(int id)
        {
            var item = db.Products.Find(id);
            if (item != null)
            {
                item.IsSale = !item.IsSale;
                db.Entry(item).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return Json(new { success = true, IsSale = item.IsSale });
            }
            return Json(new { success = false });
        }

        Border border;
        public ActionResult ExportToExcel()
        {
            var products = from m in db.Products
                           select m;
            byte[] fileContents;
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            ExcelPackage Ep = new ExcelPackage();
            ExcelWorksheet Sheet = Ep.Workbook.Worksheets.Add("ProductInfo");
            Sheet.Cells["B1:I2"].Value = "Danh Sách Sản Phẩm";
            Sheet.Cells["B1:I2"].Style.Font.Bold = true;
            Sheet.Cells["B1:I2"].Style.Font.Size = 22;
            Sheet.Cells["B1:I2"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            Sheet.Cells["B1:I2"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Orange);
            Sheet.Cells["B1:I2"].Merge = true;
            Sheet.Cells["B1:I2"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            Sheet.Cells["B1:I2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            Sheet.Cells["B3"].Value = "Product Name";
            Sheet.Cells["C3"].Value = "Description";
            Sheet.Cells["D3"].Value = "Price";
            Sheet.Cells["E3"].Value = "PriceSale";
            Sheet.Cells["F3"].Value = "Quantity";
            Sheet.Cells["G3"].Value = "CreateDate";
            Sheet.Cells["H3"].Value = "CreateBy";
            Sheet.Cells["I3"].Value = "ViewCount";
            Sheet.Cells["B3:I3"].Style.Font.Size = 14;
            Sheet.Cells["B3:I3"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            Sheet.Cells["B3:I3"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Yellow);
            //
            using (var range = Sheet.Cells["B3:I3"])
            {
                range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                range.Style.Border.Top.Color.SetColor(Color.Black);
                range.Style.Border.Left.Color.SetColor(Color.Black);
                range.Style.Border.Right.Color.SetColor(Color.Black);
                range.Style.Border.Bottom.Color.SetColor(Color.Black);
            }
            //
            Sheet.Cells["B3"].Style.Font.Bold = true;
            Sheet.Cells["C3"].Style.Font.Bold = true;
            Sheet.Cells["D3"].Style.Font.Bold = true;
            Sheet.Cells["E3"].Style.Font.Bold = true;
            Sheet.Cells["F3"].Style.Font.Bold = true;
            Sheet.Cells["G3"].Style.Font.Bold = true; ;
            Sheet.Cells["H3"].Style.Font.Bold = true;
            Sheet.Cells["I3"].Style.Font.Bold = true;
            //

            int row = 4;
            foreach (var item in products)
            {
                Sheet.Cells[string.Format("B{0}", row)].Value = item.Title;
                Sheet.Cells[string.Format("C{0}", row)].Value = item.Description;
                Sheet.Cells[string.Format("D{0}", row)].Value = item.Price;
                Sheet.Cells[string.Format("E{0}", row)].Value = item.PriceSale;
                Sheet.Cells[string.Format("F{0}", row)].Value = item.Quantity;
                Sheet.Cells[string.Format("G{0}", row)].Value = item.CreateDate.ToString("dd/MM/yyyy");
                Sheet.Cells[string.Format("H{0}", row)].Value = item.CreateBy;
                Sheet.Cells[string.Format("I{0}", row)].Value = item.ViewCount;
                // Đặt đường viền cho các ô trong dòng hiện tại
                using (var range = Sheet.Cells[string.Format("B{0}:I{0}", row)])
                {
                    range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                    range.Style.Border.Top.Color.SetColor(Color.Black);
                    range.Style.Border.Left.Color.SetColor(Color.Black);
                    range.Style.Border.Right.Color.SetColor(Color.Black);
                    range.Style.Border.Bottom.Color.SetColor(Color.Black);
                }
                row++;
            }
            Sheet.Cells["A:AZ"].AutoFitColumns();
            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            fileContents = Ep.GetAsByteArray();

            if (fileContents == null || fileContents.Length == 0)
            {
                return HttpNotFound();
            }

            return File(
                fileContents: fileContents,
                contentType: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                fileDownloadName: "List-Products.xlsx"
            );
        }
    }
}