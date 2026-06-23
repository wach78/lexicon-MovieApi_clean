using System;
using System.Collections.Generic;
using System.Text;
using Movie.Core.Entities;

namespace Movie.Core.DomainContracts;

public interface IActorRepository
{
    Task<IEnumerable<Actor>> GetAllAsync();
    Task<Actor?> GetAsync(Guid id);
    Task<bool> AnyAsync(Guid id);
    void Add(Actor actor);
    void Update(Actor actor);
    void Remove(Actor actor);
}

