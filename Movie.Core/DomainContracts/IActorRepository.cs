using System;
using System.Collections.Generic;
using System.Text;
using Movie.Core.Entities;

namespace Movie.Core.DomainContracts;

public interface IActorRepository
{
    Task<IEnumerable<Actor>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Actor?> GetAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> AnyAsync(Guid id, CancellationToken cancellationToken = default);
    void Add(Actor actor);
    void Update(Actor actor);
    void Remove(Actor actor);
}

