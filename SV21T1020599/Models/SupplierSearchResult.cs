using SV21T1020599.DomainModels;
using SV21T1020599.Web.Models;

namespace SV21T1020599.Web.Models
{
    public class SupplierSearchResult : PaginationSearchResult
    {
        public required List<Supplier> Data { get; set; }
    }
}
