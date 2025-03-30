using SV21T1020599.DomainModels;

namespace SV21T1020599.Shop.Models
{
    public class ProductDetailsViewModel
    {
        /// <summary>
        /// Thông tin sản phẩm
        /// </summary>
        public Product Product { get; set; }

        /// <summary>
        /// Danh sách ảnh của sản phẩm
        /// </summary>
        public IList<ProductPhoto> Photos { get; set; }

        /// <summary>
        /// Danh sách thuộc tính của sản phẩm
        /// </summary>
        public IList<ProductAttribute> Attributes { get; set; }

        /// <summary>
        /// Danh sách sản phẩm liên quan (cùng danh mục)
        /// </summary>
        public IList<Product> RelatedProducts { get; set; }

        public ProductDetailsViewModel()
        {
            Product = new Product();
            Photos = new List<ProductPhoto>();
            Attributes = new List<ProductAttribute>();
            RelatedProducts = new List<Product>();
        }
    }
} 