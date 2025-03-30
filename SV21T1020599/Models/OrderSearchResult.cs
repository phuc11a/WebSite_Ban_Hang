using SV21T1020599.DomainModels;
using SV21T1020599.Web.Models;

namespace SV21T1020599.Web.Models
{
    public class OrderSearchResult : PaginationSearchResult
    {
        public int Status { get; set; } = 0;
        public string TimeRange { get; set; } = "";
        public List<Order> Data { get; set; } = new List<Order>();
    }
}
