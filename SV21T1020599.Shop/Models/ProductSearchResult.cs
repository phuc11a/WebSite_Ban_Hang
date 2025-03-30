using SV21T1020599.DomainModels;

namespace SV21T1020599.Shop.Models
{
    public class ProductsSearchResult : PaginationSearchResult
    {
        public int CategoryID { get; set; } = 0;
        public int SupplierID { get; set; } = 0;
        public decimal MinPrice { get; set; } = 0;
        public decimal MaxPrice { get; set; } = 0;
        public List<Product> Data { get; set; } = new List<Product>();
    }
}
