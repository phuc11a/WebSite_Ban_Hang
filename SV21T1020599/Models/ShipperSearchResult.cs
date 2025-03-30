using SV21T1020599.DomainModels;
using SV21T1020599.Web.Models;

namespace SV21T1020599.Web.Models
{
    public class ShipperSearchResult : PaginationSearchResult
    {
        public required List<Shipper> Data { get; set; }
    }
}
