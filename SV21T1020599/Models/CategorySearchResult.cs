using SV21T1020599.DomainModels;
using SV21T1020599.Web.Models;

namespace SV21T1020599.Web.Models
{
    public class CategorySearchResult : PaginationSearchResult
    {
        public required List<Category> Data { get; set; }
    }
}
