using Auth.Domain.AppProjects.Entity;
using Auth.Query.Queries.AppProjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auth.Query.Helpers
{
    public static class AppUrlQueryHelper
    {
        public static IQueryable<AppUrl> ApplyFilter(
        this IQueryable<AppUrl> query,
        AppUrlFilter filter)
        {
            if (filter == null)
                return query;

            if (!string.IsNullOrWhiteSpace(filter.HttpMethod))
            {
                query = query.Where(x => x.Method == filter.HttpMethod);
            }

            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                query = query.Where(x =>
                    x.Url.Contains(filter.Search) ||
                    x.Title.Contains(filter.Search) ||
                    x.Description.Contains(filter.Search));
            }

            // ---- Dynamic Sorting ----
            query = filter.SortBy?.ToLower() switch
            {
                "url" => filter.SortDesc ? query.OrderByDescending(x => x.Url) : query.OrderBy(x => x.Url),
                "method" => filter.SortDesc ? query.OrderByDescending(x => x.Method) : query.OrderBy(x => x.Method),
                "creationDateTime" => filter.SortDesc ? query.OrderByDescending(x => x.CreationDateTime) : query.OrderBy(x => x.CreationDateTime),
                _ => filter.SortDesc ? query.OrderByDescending(x => x.Method) : query.OrderBy(x => x.Method)
            };

            return query;
        }

    }
}
