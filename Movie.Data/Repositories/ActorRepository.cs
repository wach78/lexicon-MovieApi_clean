using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Movie.Core.DomainContracts;
using Movie.Core.Entities;
using Movie.Core.Models.Pagination;
using Movie.Core.Parameters;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Movie.Data.Repositories;

public class ActorRepository : IActorRepository
{
    private readonly MovieApiContext _context;

    public ActorRepository(MovieApiContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        _context = context;
    }

    void IActorRepository.Add(Actor actor)
    {
        ArgumentNullException.ThrowIfNull(actor);

        _context.Set<Actor>().Add(actor);
    }

    async Task<bool> IActorRepository.AnyAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context
           .Set<Actor>()
           .AnyAsync(actor => actor.Id == id, cancellationToken);

    }

    async Task<PagedResult<Actor>> IActorRepository.GetAllAsync(PaginationParameters paginationParameters, CancellationToken cancellationToken)
    {
        IQueryable<Actor> query = _context
            .Set<Actor>()
            .AsNoTracking();

        int totalItems = await query.CountAsync(cancellationToken);

        IReadOnlyList<Actor> items = await query
            .Skip((paginationParameters.Page - 1) * paginationParameters.PageSize)
            .Take(paginationParameters.PageSize)
            .ToListAsync(cancellationToken);

        int totalPages = (int)Math.Ceiling(
            totalItems / (double)paginationParameters.PageSize
        );

        return new PagedResult<Actor>
        {
            Items = items,
            TotalItems = totalItems,
            CurrentPage = paginationParameters.Page,
            TotalPages = totalPages,
            PageSize = paginationParameters.PageSize
        };
    }

    async Task<Actor?> IActorRepository.GetAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context
          .Set<Actor>()
          .FirstOrDefaultAsync(movie => movie.Id == id, cancellationToken);
    }

    async void IActorRepository.Remove(Actor actor)
    {
        ArgumentNullException.ThrowIfNull(actor);

        _context.Set<Actor>().Remove(actor);
    }

    void IActorRepository.Update(Actor actor)
    {
        ArgumentNullException.ThrowIfNull(actor);

        _context.Set<Actor>().Update(actor);
    }
}
