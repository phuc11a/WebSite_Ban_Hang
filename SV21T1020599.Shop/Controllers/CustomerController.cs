using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV21T1020599.BusinessLayers;
using SV21T1020599.DomainModels;

namespace SV21T1020599.Shop.Controllers
{
    [Authorize]
    public class CustomerController : Controller
    {
        public IActionResult EditProfile()
        {
            // Lấy thông tin user từ session
            var userData = User.GetUserData();
            if (userData == null || !int.TryParse(userData.UserId, out int customerId))
            {
                return RedirectToAction("Login", "Account");
            }

            var customer = CommonDataService.GetCustomer(customerId);
            if (customer == null)
                return RedirectToAction("Index", "Home");

            return View(customer);
        }

        [HttpPost]
        public IActionResult Save(Customer data)
        {
            if (!ModelState.IsValid)
                return View("EditProfile", data);

            // Lấy thông tin user từ session
            var userData = User.GetUserData();
            if (userData == null || !int.TryParse(userData.UserId, out int customerId))
            {
                return RedirectToAction("Login", "Account");
            }

            // Đảm bảo CustomerID khớp với user đang đăng nhập
            data.CustomerID = customerId;
            
            if (!CommonDataService.UpdateCustomer(data))
            {
                ModelState.AddModelError("", "Không thể cập nhật thông tin. Vui lòng thử lại!");
                return View("EditProfile", data);
            }

            TempData["Message"] = "Cập nhật thông tin thành công!";
            return RedirectToAction("EditProfile");
        }
    }
}



