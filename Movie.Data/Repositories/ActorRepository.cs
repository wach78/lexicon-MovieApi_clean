using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Movie.Core.DomainContracts;
using Movie.Core.Entities;

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

    async Task<bool> IActorRepository.AnyAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context
           .Set<Actor>()
           .AnyAsync(actor => actor.Id == id, cancellationToken);
    }

    async Task<IEnumerable<Actor>> IActorRepository.GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context
            .Set<Actor>()
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    async Task<Actor?> IActorRepository.GetAsync(Guid id, CancellationToken cancellationToken = default)
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
