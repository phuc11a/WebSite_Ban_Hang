using SV21T1020599.DomainModels;

namespace SV21T1020599.Web.Models
{
    public class CustomerSearchResult : PaginationSearchResult
    {
        public required List<Customer> Data { get; set; }

    }
}
