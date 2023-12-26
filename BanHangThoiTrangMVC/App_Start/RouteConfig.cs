using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace BanHangThoiTrangMVC
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Contact",
                url: "lien-he",
                defaults: new { controller = "Contact", action = "Index", alias = UrlParameter.Optional },
                namespaces: new[] { "BanHangThoiTrangMVC.Controllers" }
            );

            routes.MapRoute(
                name: "Giỏ Hàng",
                url: "gio-hang",
                defaults: new { controller = "ShoppingCart", action = "Index", alias = UrlParameter.Optional },
                namespaces: new[] { "BanHangThoiTrangMVC.Controllers" }
            );

            routes.MapRoute(
                name: "Thanh Toán",
                url: "thanh-toan",
                defaults: new { controller = "ShoppingCart", action = "CheckOut", alias = UrlParameter.Optional },
                namespaces: new[] { "BanHangThoiTrangMVC.Controllers" }
            );
            routes.MapRoute(
                name: "vnpay_return",
                url: "vnpay_return",
                defaults: new { controller = "ShoppingCart", action = "VnpayReturn", alias = UrlParameter.Optional },
                namespaces: new[] { "BanHangThoiTrangMVC.Controllers" }
            );

            routes.MapRoute(
                name: "CategoryProduct",
                url: "danh-muc-san-pham/{alias}-{id}",
                defaults: new { controller = "Products", action = "ProductCategory", id = UrlParameter.Optional },
                namespaces: new[] { "BanHangThoiTrangMVC.Controllers" }
            );

            //Artical controller
            routes.MapRoute(
                name: "BaiViet",
                url: "bai-viet",
                defaults: new { controller = "Article", action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "BanHangThoiTrangMVC.Controllers" }
            );

            routes.MapRoute(
                name: "Product",
                url: "san-pham",
                defaults: new { controller = "Products", action = "Index", alias = UrlParameter.Optional },
                namespaces: new[] { "BanHangThoiTrangMVC.Controllers" }
            );

            //News
            routes.MapRoute(
                name: "News",
                url: "cong-novels",
                defaults: new { controller = "News", action = "Index", alias = UrlParameter.Optional },
                namespaces: new[] { "BanHangThoiTrangMVC.Controllers" }
            );

            routes.MapRoute(
                name: "DetailNews",
                url: "chi-tiet-novel/{alias}-n{id}",
                defaults: new { controller = "News", action = "Detail", id = UrlParameter.Optional },
                namespaces: new[] { "BanHangThoiTrangMVC.Controllers" }
            );
            //chi tiết bài viết
            routes.MapRoute(
                name: "DetailPost",
                url: "chi-tiet-bai-viet/{alias}-n{id}",
                defaults: new { controller = "Article", action = "Detail", id = UrlParameter.Optional },
                namespaces: new[] { "BanHangThoiTrangMVC.Controllers" }
            );
            routes.MapRoute(
                name: "DetailProduct",
                url: "chi-tiet/{alias}-p{id}",
                defaults: new { controller = "Products", action = "Detail", alias = UrlParameter.Optional },
                namespaces: new[] { "BanHangThoiTrangMVC.Controllers" }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "BanHangThoiTrangMVC.Controllers" }
            );
        }
    }
}
