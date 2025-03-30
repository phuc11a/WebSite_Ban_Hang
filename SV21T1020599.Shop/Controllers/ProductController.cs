using Microsoft.AspNetCore.Mvc;
using SV21T1020599.BusinessLayers;
using SV21T1020599.DomainModels;
using SV21T1020599.Shop.Models;

namespace SV21T1020599.Shop.Controllers
{
    public class ProductController : Controller
    {
        private const int PAGE_SIZE = 10;

        /// <summary>
        /// Hiển thị danh sách sản phẩm trong _Section
        /// </summary>
        public IActionResult Index(int page = 1, string searchValue = "",
            int categoryID = 0, int supplierID = 0,
            decimal minPrice = 0, decimal maxPrice = decimal.MaxValue)
        {
            try
            {
                // Validate price range
                if (minPrice < 0) minPrice = 0;
                if (maxPrice < minPrice) maxPrice = decimal.MaxValue;

                int rowCount;
                var data = ProductDataService.ListProducts(
                    out rowCount,
                    page,
                    PAGE_SIZE,
                    searchValue?.Trim() ?? "",
                    categoryID,
                    supplierID,
                    minPrice,
                    maxPrice
                );

                var result = new ProductsSearchResult()
                {
                    Page = page,
                    PageSize = PAGE_SIZE,
                    SearchValue = searchValue,
                    RowCount = rowCount,
                    CategoryID = categoryID,
                    SupplierID = supplierID,
                    MinPrice = minPrice,
                    MaxPrice = maxPrice == decimal.MaxValue ? 0 : maxPrice,
                    Data = data
                };

                return View(result);
            }
            catch (Exception ex)
            {
                // Log error...
                return View(new ProductsSearchResult());
            }
        }

        /// <summary>
        /// Xem chi tiết sản phẩm
        /// </summary>
        public IActionResult Details(int id)
        {
            var product = ProductDataService.GetProduct(id);
            if (product == null)
                return RedirectToAction("Index");

            // Lấy danh sách ảnh của sản phẩm
            var photos = ProductDataService.ListPhotos(id);
            // Lấy danh sách thuộc tính của sản phẩm
            var attributes = ProductDataService.ListAttributes(id);

            var model = new ProductDetailsViewModel()
            {
                Product = product,
                Photos = photos,
                Attributes = attributes
            };

            return View(model);
        }

        /// <summary>
        /// API lấy danh sách sản phẩm cho phân trang
        /// </summary>
        [HttpGet]
        public IActionResult List(ProductSearchInput input)
        {
            try
            {
                int rowCount;
                var data = ProductDataService.ListProducts(
                    out rowCount,
                    input.Page,
                    PAGE_SIZE,
                    input.SearchValue,
                    input.CategoryID,
                    input.SupplierID,
                    decimal.Parse(input.MinPrice),
                    decimal.Parse(input.MaxPrice)
                );

                var result = new ProductsSearchResult()
                {
                    Page = input.Page,
                    PageSize = input.PageSize,
                    SearchValue = input.SearchValue,
                    RowCount = rowCount,
                    CategoryID = input.CategoryID,
                    SupplierID = input.SupplierID,
                    MinPrice = decimal.Parse(input.MinPrice),
                    MaxPrice = decimal.Parse(input.MaxPrice),
                    Data = data.ToList()
                };

                return Json(result);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult AddToCart([FromForm] CartItem item)
        {
            try
            {
                if (item.SalePrice <= 0)
                    return Json(new { success = false, message = "Giá bán không hợp lệ" });

                if (item.Quantity <= 0)
                    return Json(new { success = false, message = "Số lượng phải lớn hơn 0" });

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
                return Json(new { 
                    success = true, 
                    message = "Đã thêm sản phẩm vào giỏ hàng",
                    cartCount = cart.Count 
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Lỗi: {ex.Message}" });
            }
        }

        private List<CartItem> GetShoppingCart()
        {
            var cart = ApplicationContext.GetSessionData<List<CartItem>>(SHOPPING_CART);
            if (cart == null)
            {
                cart = new List<CartItem>();
                ApplicationContext.SetSessionData(SHOPPING_CART, cart);
            }
            return cart;
        }

        private const string SHOPPING_CART = "shopping_cart";
    }
}