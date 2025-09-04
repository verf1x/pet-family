using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using PetFamily.SharedKernel.Models;

namespace PetFamily.SharedKernel.Extensions;

public static class QueriesExtensions
{
    public static async Task<PagedList<T>> ToPagedList<T>(
        this IQueryable<T> source,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        int petsCount = await source.CountAsync(cancellationToken);

        List<T> items = await source
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedList<T> { Items = items, PageSize = pageSize, PageNumber = pageNumber, TotalCount = petsCount };
    }

    public static IQueryable<T> WhereIf<T>(
        this IQueryable<T> source,
        bool condition,
        Expression<Func<T, bool>> predicate) =>
        condition ? source.Where(predicate) : source;
}