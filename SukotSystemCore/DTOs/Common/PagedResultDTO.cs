using System.Collections.Generic;

namespace SukotSystemCore.DTOs.Common
{
    // Generic wrapper for real (database-level) pagination results - requirement 6.
    // Services return this instead of a bare IEnumerable<T> whenever a list endpoint paginates.
    public class PagedResultDTO<T>
    {
        public IEnumerable<T> Items { get; set; } = new List<T>();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
    }
}
