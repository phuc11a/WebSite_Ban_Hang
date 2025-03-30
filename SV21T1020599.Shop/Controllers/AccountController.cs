using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV21T1020599.BusinessLayers;
using SV21T1020599.DomainModels;

namespace SV21T1020599.Shop.Controllers
{
    public class AccountController : Controller
    {
        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password, string? returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl;
            ViewBag.Username = username;

            // Kiểm tra thông tin đăng nhập (sử dụng UserTypes.Customer vì đây là shop)
            var userAccount = UserAccountService.Authorize(UserTypes.Customer, username, password);

            if (userAccount == null)
            {
                ModelState.AddModelError("Error", "Tên đăng nhập hoặc mật khẩu không đúng!");
                return View();
            }

            // Tạo đối tượng lưu thông tin người dùng
            var userData = new WebUserData()
            {
                UserId = userAccount.UserId,
                UserName = userAccount.UserName,
                Displayname = userAccount.Displayname,
                Roles = userAccount.RoleNames?.Split(',').ToList()
            };

            // Lưu thông tin user vào session
            ApplicationContext.SetSessionData("UserData", userData);

            // Đăng nhập
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                userData.CreatePrincipal(),
                new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTime.UtcNow.AddDays(30)
                }
            );

            if (!string.IsNullOrEmpty(returnUrl))
                return LocalRedirect(returnUrl);
            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public IActionResult ChangePassword(string oldPassword, string newPassword, string confirmPassword)
        {
            if (string.IsNullOrEmpty(oldPassword))
            {
                ModelState.AddModelError("Error", "Vui lòng nhập mật khẩu cũ");
                return View();
            }

            if (string.IsNullOrEmpty(newPassword))
            {
                ModelState.AddModelError("Error", "Vui lòng nhập mật khẩu mới");
                return View();
            }

            if (newPassword != confirmPassword)
            {
                ModelState.AddModelError("Error", "Xác nhận mật khẩu không khớp");
                return View();
            }

            var userData = User.GetUserData();
            if (userData == null)
                return RedirectToAction("Login");

            // Kiểm tra mật khẩu cũ
            if (UserAccountService.Authorize(UserTypes.Customer, userData.UserName, oldPassword) == null)
            {
                ModelState.AddModelError("Error", "Mật khẩu cũ không đúng!");
                return View();
            }

            // Đổi mật khẩu
            if (UserAccountService.ChangedPassword(userData.UserName, newPassword))
            {
                TempData["ChangedPassword"] = "Đổi mật khẩu thành công!";
                return View();
            }

            ModelState.AddModelError("Error", "Không thể đổi mật khẩu. Vui lòng thử lại!");
            return View();
        }

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            // Xóa session data
            ApplicationContext.RemoveSessionData("UserData");
            
            // Đăng xuất
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(Customer model, string password, string confirmPassword)
        {
            if (!ModelState.IsValid)
                return View(model);

            if (string.IsNullOrEmpty(password))
            {
                ModelState.AddModelError("", "Vui lòng nhập mật khẩu");
                return View(model);
            }

            if (password != confirmPassword)
            {
                ModelState.AddModelError("", "Xác nhận mật khẩu không khớp");
                return View(model);
            }

            try
            {
                // Kiểm tra email đã tồn tại
                if (UserAccountService.Authorize(UserTypes.Customer, model.Email, "") != null)
                {
                    ModelState.AddModelError("", "Email đã được sử dụng");
                    return View(model);
                }

                // Thêm phương thức Register vào UserAccountService
                bool result = UserAccountService.Register(
                    model.Email, 
                    password, 
                    model.CustomerName, 
                    model.Address, 
                    model.Phone
                );

                if (result)
                {
                    TempData["RegisterSuccess"] = "Đăng ký thành công! Vui lòng đăng nhập.";
                    return RedirectToAction("Login");
                }
            }
            catch
            {
                ModelState.AddModelError("", "Có lỗi xảy ra. Vui lòng thử lại!");
                return View(model);
            }

            ModelState.AddModelError("", "Không thể tạo tài khoản. Vui lòng thử lại!");
            return View(model);
        }
    }
}