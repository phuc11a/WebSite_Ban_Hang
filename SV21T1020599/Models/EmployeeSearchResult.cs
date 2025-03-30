using SV21T1020599.DomainModels;

namespace SV21T1020599.Web.Models
{
    public class EmployeeSearchResult : PaginationSearchResult
    {
        public required List<Employee> Data { get; set; }
    }
}
