using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV21T1020599.BusinessLayers;
using SV21T1020599.DomainModels;
using SV21T1020599.Shop.Models;
using System.Globalization;

namespace SV21T1020599.Shop.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        private const string SHOPPING_CART = "shopping_cart";
        private const int WEB_ORDER_EMPLOYEE_ID = 1; // ID của nhân viên mặc định

        /// <summary>
        /// Hiển thị giỏ hàng
        /// </summary>
        public IActionResult ShoppingCart()
        {
            var cart = GetShoppingCart();
            return View(cart);
        }

        /// <summary>
        /// Thêm sản phẩm vào giỏ hàng
        /// </summary>
        [HttpPost]
        public IActionResult AddToCart([FromForm] CartItem item)
        {
            try
            {
                if (item.SalePrice <= 0)
                    return Json("Giá bán không hợp lệ");

                if (item.Quantity <= 0)
                    return Json("Số lượng phải lớn hơn 0");

                var cart = GetShoppingCart();
                var existItem = cart.FirstOrDefault(p => p.ProductID == item.ProductID);
                if (existItem == null)
                {
                    cart.Add(item);
                }
                else
                {
                    existItem.Quantity += item.Quantity;
                }

                ApplicationContext.SetSessionData(SHOPPING_CART, cart);
                return Json("");
            }
            catch (Exception ex)
            {
                return Json($"Lỗi: {ex.Message}");
            }
        }

        /// <summary>
        /// Cập nhật số lượng sản phẩm trong giỏ hàng
        /// </summary>
        [HttpPost]
        public IActionResult UpdateQuantity(int productId, int quantity)
        {
            if (quantity <= 0)
                return Json("Số lượng phải lớn hơn 0");

            var cart = GetShoppingCart();
            var item = cart.FirstOrDefault(p => p.ProductID == productId);
            if (item != null)
            {
                item.Quantity = quantity;
                ApplicationContext.SetSessionData(SHOPPING_CART, cart);
            }
            return Json("");
        }

        /// <summary>
        /// Xóa sản phẩm khỏi giỏ hàng
        /// </summary>
        public IActionResult RemoveFromCart(int id)
        {
            var cart = GetShoppingCart();
            int index = cart.FindIndex(m => m.ProductID == id);
            if (index >= 0)
                cart.RemoveAt(index);
            ApplicationContext.SetSessionData(SHOPPING_CART, cart);
            return RedirectToAction("ShoppingCart");
        }

        /// <summary>
        /// Xóa toàn bộ giỏ hàng
        /// </summary>
        public IActionResult ClearCart()
        {
            try
            {
                ApplicationContext.RemoveSessionData(SHOPPING_CART);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error clearing cart: {ex.Message}");
            }
            return RedirectToAction("ShoppingCart");
        }

        /// <summary>
        /// Khởi tạo đơn hàng
        /// </summary>
        [HttpPost]
        public IActionResult Init(int customerID, string deliveryProvince, string deliveryAddress)
        {
            var cart = GetShoppingCart();
            if (!cart.Any())
                return Json("Giỏ hàng trống!");

            // Chuyển dữ liệu từ CartItem sang OrderDetail
            var orderDetails = cart.Select(item => new OrderDetail()
            {
                ProductID = item.ProductID,
                ProductName = item.ProductName,
                Photo = item.Photo,
                Unit = item.Unit,
                Quantity = item.Quantity,
                SalePrice = item.SalePrice
            }).ToList();

            // Khởi tạo đơn hàng
            int employeeID = 0; // Có thể lấy từ user đăng nhập nếu là nhân viên
            int orderID = OrderDataService.InitOrder(
                employeeID,
                customerID,
                deliveryProvince,
                deliveryAddress,
                orderDetails
            );

            if (orderID > 0)
            {
                ClearCart();
                return Json("");
            }

            return Json("Không thể tạo đơn hàng!");
        }

        /// <summary>
        /// Lấy số lượng sản phẩm trong giỏ hàng
        /// </summary>
        [HttpGet]
        public IActionResult GetCartCount()
        {
            var cart = GetShoppingCart();
            return Json(cart?.Count ?? 0);
        }

        /// <summary>
        /// Lấy tổng tiền giỏ hàng
        /// </summary>
        public IActionResult GetCartTotal()
        {
            var cart = GetShoppingCart();
            return Json(cart.Sum(m => m.TotalPrice));
        }

        /// <summary>
        /// Lấy giỏ hàng từ session
        /// </summary>      

        /// <summary>
        /// Lưu giỏ hàng vào session
        /// </summary>
        private void SaveCart(List<CartItem> cart)
        {
            ApplicationContext.SetSessionData(SHOPPING_CART, cart);
        }

        /// <summary>
        /// Lấy thông tin giỏ hàng
        /// </summary>
        [HttpGet]
        public IActionResult GetCartInfo()
        {
            var cart = GetShoppingCart();
            return Json(new
            {
                count = cart.Count,
                total = cart.Sum(m => m.TotalPrice)
            });
        }

        /// <summary>
        /// Lấy giỏ hàng từ session
        /// </summary>
        private List<CartItem> GetShoppingCart()
        {
            try
            {
                var cart = ApplicationContext.GetSessionData<List<CartItem>>(SHOPPING_CART);
                return cart ?? new List<CartItem>();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting cart: {ex.Message}");
                return new List<CartItem>();
            }
        }

        public IActionResult Checkout()
        {
            // Kiểm tra giỏ hàng trước
            var cart = GetShoppingCart();
            if (cart == null || !cart.Any())
            {
                return RedirectToAction("ShoppingCart");
            }

            // Lấy thông tin user từ session
            var userData = ApplicationContext.GetSessionData<WebUserData>("UserData");
            if (userData == null || !int.TryParse(userData.UserId, out int customerId))
            {
                return RedirectToAction("Login", "Account", new { returnUrl = Url.Action("Checkout", "Order") });
            }

            // Lấy thông tin chi tiết khách hàng từ database
            var customer = CommonDataService.GetCustomer(customerId);
            if (customer == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Tạo model cho view checkout
            var model = new CheckoutViewModel
            {
                Cart = cart,
                CustomerName = customer.CustomerName,
                CustomerContactName = customer.ContactName,
                CustomerAddress = customer.Address,
                CustomerPhone = customer.Phone,
                CustomerEmail = customer.Email,
                DeliveryProvince = customer.Province,
                DeliveryAddress = customer.Address
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult Checkout(CheckoutViewModel model)
        {
            try
            {
                var cart = GetShoppingCart();
                if (cart == null || !cart.Any())
                {
                    return Json(new { success = false, message = "Giỏ hàng trống" });
                }

                var userData = ApplicationContext.GetSessionData<WebUserData>("UserData");
                if (userData == null || !int.TryParse(userData.UserId, out int customerId))
                {
                    return Json(new { success = false, message = "Vui lòng đăng nhập lại" });
                }

                var orderDetails = cart.Select(item => new OrderDetail
                {
                    ProductID = item.ProductID,
                    ProductName = item.ProductName,
                    Photo = item.Photo,
                    Unit = item.Unit,
                    Quantity = item.Quantity,
                    SalePrice = item.SalePrice
                }).ToList();

                // Sử dụng ID nhân viên mặc định
                int orderId = OrderDataService.InitOrder(
                    employeeID: WEB_ORDER_EMPLOYEE_ID, // Sử dụng ID nhân viên mặc định
                    customerID: customerId,
                    deliveryProvince: model.DeliveryProvince,
                    deliveryAddress: model.DeliveryAddress,
                    details: orderDetails
                );

                if (orderId > 0)
                {
                    ClearCart();
                    TempData["OrderId"] = orderId;
                    TempData["CustomerName"] = model.CustomerName;
                    return Json(new { success = true, redirectUrl = Url.Action("OrderSuccess", "Order") });
                }

                return Json(new { success = false, message = "Không thể tạo đơn hàng" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        /// <summary>
        /// Hiển thị trang xác nhận đặt hàng thành công
        /// </summary>
        public IActionResult OrderSuccess()
        {
            // Kiểm tra có thông tin đơn hàng trong TempData không
            if (TempData["OrderId"] == null)
            {
                return RedirectToAction("Index", "Home");
            }

            ViewBag.OrderId = TempData["OrderId"];
            ViewBag.CustomerName = TempData["CustomerName"];
            return View();
        }
        /// <summary>
        /// Xem chi tiết đơn hàng
        /// </summary>
        public IActionResult OrderDetails(int id)
        {
            try
            {
                var order = OrderDataService.GetOrder(id);
                if (order == null)
                {
                    return RedirectToAction("Index", "Home");
                }

                // Kiểm tra xem đơn hàng có thuộc về khách hàng hiện tại không
                var userData = ApplicationContext.GetSessionData<WebUserData>("UserData");
                if (userData == null || !int.TryParse(userData.UserId, out int customerId))
                {
                    return RedirectToAction("Login", "Account");
                }

                if (order.CustomerID != customerId)
                {
                    return RedirectToAction("Index", "Home");
                }

                // Lấy chi tiết đơn hàng
                order.Details = OrderDataService.ListOrderDetails(id);

                return View(order);
            }
            catch (Exception ex)
            {
                // Log error
                return RedirectToAction("Index", "Home");
            }
        }

        /// <summary>
        /// Hiển thị trang theo dõi đơn hàng
        /// </summary>
        public IActionResult TrackOrder()
        {
            // Lấy thông tin user từ session
            var userData = ApplicationContext.GetSessionData<WebUserData>("UserData");
            if (userData == null || !int.TryParse(userData.UserId, out int customerId))
            {
                return RedirectToAction("Login", "Account");
            }

            // Lấy danh sách đơn hàng của khách hàng
            int totalRecords = 0; // Biến để nhận tổng số bản ghi
            var orders = OrderDataService.ListOrders(
                out totalRecords,    // Thêm tham số out
                1,                  // page
                0,                  // pageSize (0 để lấy tất cả)
                0,                  // status (0 để lấy tất cả trạng thái)
                null,               // fromTime
                null,               // toTime
                ""                  // searchValue
            ).Where(o => o.CustomerID == customerId)
             .OrderByDescending(o => o.OrderTime)
             .ToList();

            // Lấy chi tiết cho mỗi đơn hàng
            foreach (var order in orders)
            {
                order.Details = OrderDataService.ListOrderDetails(order.OrderID);
            }

            return View(orders);
        }

        /// <summary>
        /// API lấy trạng thái đơn hàng
        /// </summary>
        [HttpGet]
        public IActionResult GetOrderStatus(int orderId)
        {
            try
            {
                var userData = ApplicationContext.GetSessionData<WebUserData>("UserData");
                if (userData == null || !int.TryParse(userData.UserId, out int customerId))
                {
                    return Json(new { success = false, message = "Vui lòng đăng nhập" });
                }

                var order = OrderDataService.GetOrder(orderId);
                if (order == null || order.CustomerID != customerId)
                {
                    return Json(new { success = false, message = "Không tìm thấy đơn hàng" });
                }

                string status = order.Status switch
                {
                    1 => "Chờ xác nhận",
                    2 => "Đã xác nhận",
                    3 => "Đang giao hàng",
                    4 => "Đã giao hàng",
                    -1 => "Đã hủy",
                    _ => "Không xác định"
                };

                return Json(new { success = true, status = status, statusCode = order.Status });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}