using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auth.Query.Queries.AppProjects
{
    public class AppUrlFilter
    {
        public string? HttpMethod { get; set; }      // GET / POST / PUT / etc.
        public string? Search { get; set; }          // optional text filter
        public string? SortBy { get; set; } = "Method";
        public bool SortDesc { get; set; } = false;
    }
}
