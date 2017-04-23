using System.Collections.Generic;

namespace Hermes.Services.Helpers.Collections
{
    public class PaginatedResults<T>
    {
        public int PageIndex { get; set; }

        public int TotalNumber { get; set; }

        public List<T> Results { get; set; }
    }
}
