using BanHangThoiTrangMVC.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace BanHangThoiTrangMVC.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]

    public class AccountController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private readonly UserManager<ApplicationUser> _userManager1;
        private readonly RoleManager<IdentityRole> _roleManager;
        private ApplicationDbContext db = new ApplicationDbContext();
        private IEnumerable<object> roles;

        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager, UserManager<ApplicationUser> userManager1, RoleManager<IdentityRole> roleManager)
        {
            _userManager1 = userManager1;
            UserManager = userManager;
            SignInManager = signInManager;
            _roleManager = roleManager;
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
        // GET: Admin/Account
        public async Task<ActionResult> Index(int? page, string SearchText)
        {
            var pageSize = 5;
            if (page == null)
            {
                page = 1;
            }
            var pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            IEnumerable<UserViewModel> item = (from u in db.Users
                                                 select new UserViewModel
                                                 {
                                                     Id = u.Id,
                                                     UserName = u.UserName,
                                                     FullName = u.Fullname,
                                                     Phone = u.Phone,
                                                     Email = u.Email,
                                                     EmailConfirmed = u.EmailConfirmed,
                                                     Roles = (from userRole in u.Roles
                                                                  join role in db.Roles on userRole.RoleId equals role.Id
                                                                  select role.Name).ToList()
                                                 }).ToList();
            /*IEnumerable<UserViewModel> item = (IEnumerable<UserViewModel>)db.Users.ToList();*/
            if (!string.IsNullOrEmpty(SearchText))
            {
                item = item.Where(x => x.FullName.ToLower().Contains(SearchText));
            }
            item = item.OrderByDescending(x => x.Id);
            item = item.ToPagedList(pageIndex, pageSize);
            ViewBag.PageSize = pageSize;
            ViewBag.Page = page;
            return View(item);
        }

        //[Authorize(Roles = "Admin")]
        //public async Task<ActionResult> Index(int? page, string SearchText)
        //{
        //    var pageSize = 5;
        //    if (page == null)
        //    {
        //        page = 1;
        //    }
        //    var pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;

        //    var usersQuery = (from u in db.Users
        //                      select new
        //                      {
        //                          Id = u.Id,
        //                          UserName = u.UserName,
        //                          Fullname = u.Fullname,
        //                          Phone = u.Phone,
        //                          Email = u.Email,
        //                          RoleNames = (from userRole in u.Roles
        //                                       join role in db.Roles on userRole.RoleId equals role.Id
        //                                       select role.Name).ToList()
        //                      });

        //    var users = usersQuery.ToList().Select(u => new UserViewModel
        //    {
        //        Id = u.Id,
        //        UserName = u.UserName,
        //        FullName = u.Fullname,
        //        Phone = u.Phone,
        //        Email = u.Email,
        //        Roles = u.RoleNames // Gán danh sách vai trò vào thuộc tính Roles
        //    }).ToList();

        //    users = users.OrderByDescending(x => x.Id).ToList();

        //    var pagedList = users.ToPagedList(pageIndex, pageSize);

        //    ViewBag.PageSize = pageSize;
        //    ViewBag.Page = page;
        //    return View(pagedList);
        //}



        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Create()
        {
            ViewBag.Role = new SelectList(db.Roles.ToList(), "Name", "Name");
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(CreateAccountViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.UserName,
                    Email = model.Email,
                    Fullname = model.FullName,
                    Phone = model.Phone
                };
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    if(model.Roles != null)
                    {
                        foreach(var r in model.Roles)
                        {
                            UserManager.AddToRole(user.Id, r);
                        }
                    }
                     // Lưu ý dòng này khi đăng ký thành công để nó add vào table UserRole
                    /*await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);*/
                    // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=320771
                    // Send an email with this link
                    // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    // await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");

                    return RedirectToAction("Index", "Account");
                }
                AddErrors(result);
            }
            ViewBag.Role = new SelectList(db.Roles.ToList(), "Name", "Name");
            // If we got this far, something failed, redisplay form
            return View(model);
        }
        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }



        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, change to shouldLockout: true
            var result = await SignInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, shouldLockout: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid login attempt.");
                    return View(model);
            }
        }
        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }
        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index", "Home");
        }
        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        /*public async Task<ActionResult> Edit(string id)
        {
            var item = await UserManager.FindByIdAsync(id);

            *//* var selecList = new SelectList(db.Roles.ToList(), "Id", "Name");
             ViewBag.Role = selecList;*//*

            var roles = db.Roles.Select(r => new SelectListItem
            {
                Value = r.Id,
                Text = r.Name
            }).ToList();

            ViewBag.Role = new SelectList(roles, "Value", "Text");
            return View(item);
        }*/


        /*[HttpPost]
        [ValidateAntiForgeryToken]*/
        /*public async Task<ActionResult> Edit(ApplicationUser model)
        {
            if (ModelState.IsValid)
            {
                var oldItem = db.Users.Find(model.Id);
                oldItem.UserName = model.UserName;
                oldItem.Fullname = model.Fullname;
                oldItem.Phone = model.Phone;
                oldItem.Email = model.Email;
                oldItem.RoleNames = model.RoleNames;
                *//*oldItem.RoleNames = model.RoleNames;*//*
                db.SaveChanges();
                return RedirectToAction("Index");
                *//*db.Users.Attach(model);
                db.Entry(model).State = System.Data.Entity.EntityState.Modified;
                model.LockoutEnabled = true;
                db.SaveChanges();
                return RedirectToAction("Index");*//*

            }
            return View(model);
        }*/

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        /*public ActionResult Edit(string id)
        {
            var item = UserManager.FindById(id);
            var newUser = new EditAccountViewModel();
            if (item != null)
            {
                var rolesForUser = UserManager.GetRoles(id);
                var roles = new List<string>();
                if (rolesForUser != null)
                {
                    foreach (var role in rolesForUser)
                    {
                        roles.Add(role);
                    }
                }
                newUser.FullName = item.Fullname;
                newUser.Email = item.Email;
                newUser.Phone = item.Phone;
                newUser.UserName = item.UserName;
                newUser.Roles = item.RoleNames;
            }
            ViewBag.Role = new SelectList(db.Roles.ToList(), "Name", "Name");
            return View(newUser);
        }
        // POST: /Account/edit
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(EditAccountViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = UserManager.FindByName(model.UserName);
                user.Fullname = model.FullName;
                user.Phone = model.Phone;
                user.Email = model.Email;
                user.RoleNames = model.Roles;
                var result = await UserManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    var rolesForUser = UserManager.GetRoles(user.Id);
                    if (model.Roles != null)
                    {

                        foreach (var r in model.Roles)
                        {
                            var checkRole = rolesForUser.FirstOrDefault(x => x.Equals(r));
                            if (checkRole != null)
                            {
                                UserManager.AddToRole(user.Id, r);
                            }
                        }
                    }
                    // Lưu ý dòng này khi đăng ký thành công để nó add vào table UserRole
                    //*await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);*//*
                    // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=320771
                    // Send an email with this link
                    // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    // await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");

                    return RedirectToAction("Index", "Account");
                }
                AddErrors(result);
            }
            ViewBag.Role = new SelectList(db.Roles.ToList(), "Name", "Name");
            // If we got this far, something failed, redisplay form
            return View(model);
        }*/


        public ActionResult Edit(string id)
        {
            var user = UserManager.FindById(id);
            var newUser = new EditAccountViewModel();

            if (user != null)
            {
                newUser.FullName = user.Fullname;
                newUser.Email = user.Email;
                newUser.Phone = user.Phone;
                newUser.UserName = user.UserName;
                newUser.Roles = user.RoleNames;

                // Assuming the user has only one role
                var userRoles = UserManager.GetRoles(id);
                if (userRoles.Any())
                {
                    newUser.SelectedRole = userRoles.FirstOrDefault(); // Select the first role
                }
            }

            var rolesList = db.Roles.Select(r => r.Name).ToList(); // Fetch role names from database
            ViewBag.Role = new SelectList(rolesList, newUser.SelectedRole); // Create SelectList with roles and set the selected role
            return View(newUser);
        }


        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(EditAccountViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = UserManager.FindByName(model.UserName);
                if (user == null)
                {
                    // Handle user not found scenario
                    return HttpNotFound();
                }

                user.Fullname = model.FullName;
                user.Phone = model.Phone;
                user.Email = model.Email;

                var result = await UserManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    // Remove user from all existing roles
                    var rolesForUser = await UserManager.GetRolesAsync(user.Id);
                    foreach (var userRole in rolesForUser)
                    {
                        await UserManager.RemoveFromRoleAsync(user.Id, userRole);
                    }

                    // Add user to the selected role
                    if (model.Roles != null)
                    {
                        foreach (var r in model.Roles)
                        {
                            UserManager.AddToRole(user.Id, r);
                        }
                    }

                    return RedirectToAction("Index", "Account");
                }

                AddErrors(result);
            }

            ViewBag.Role = new SelectList(db.Roles.ToList(), "Name", "Name");
            return View(model);
        }











        [HttpPost]
        public async Task<ActionResult> Delete(string user, string id)
        {
            var code = new { Success = false };
            var item = UserManager.FindByName(user);
            if (item != null)
            {
                var rolesForUser = UserManager.GetRoles(id);
                if (rolesForUser != null)
                {
                    foreach (var role in rolesForUser)
                    {
                        await UserManager.RemoveFromRoleAsync(id, role);
                    }
                }


                var result = await UserManager.DeleteAsync(item);
                code = new { Success = result.Succeeded };
            }
            return Json(code);
        }

        [HttpPost]
        public async Task<ActionResult> IsLock(string id)
        {
            var user = await UserManager.FindByIdAsync(id);
            if (user != null)
            {
                user.LockoutEnabled = !user.LockoutEnabled;
                db.Entry(user).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return Json(new { success = true, IsLock = user.LockoutEnabled });
            }
            return Json(new { success = false });
        }
    }
}