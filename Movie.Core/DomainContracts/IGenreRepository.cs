using System;
using System.Collections.Generic;
using System.Text;
using Movie.Core.Entities;

namespace Movie.Core.DomainContracts;

public interface IGenreRepository
{
    Task<IEnumerable<Genre>> GetAllAsync();
    Task<Genre?> GetAsync(Guid id);
    Task<bool> AnyAsync(Guid id);
    void Add(Genre genre);
    void Update(Genre genre);
    void Remove(Genre genre);
}
