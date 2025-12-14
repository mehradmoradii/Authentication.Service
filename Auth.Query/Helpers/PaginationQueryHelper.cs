


using Auth.Query.Helpers;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using IConfigurationProvider = AutoMapper.IConfigurationProvider;

public static class PaginationHelper
{
    public static async Task<PagedResult<TDto>> ToPagedResultDynamic<TEntity, TDto>(
        IQueryable<TEntity> query,
        int page,
        int pageSize,
        IConfigurationProvider mapperConfig,
        CancellationToken ct = default)
    {
        var total = await query.CountAsync(ct);

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ProjectTo<TDto>(mapperConfig)
            .ToListAsync(ct);

        return new PagedResult<TDto>
        {
            Page = page,
            PageSize = pageSize,
            TotalCount = total,
            Items = items
        };
    }
}
