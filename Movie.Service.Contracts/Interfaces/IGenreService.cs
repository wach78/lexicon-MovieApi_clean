using System;
using System.Collections.Generic;
using System.Text;
using Movie.Core.Entities;

namespace Movie.Service.Contracts.Interfaces;

public interface IGenreService
{
    Task<IReadOnlyList<Genre>> GetAllAsync();

    Task<Genre?> GetAsync(Guid id);

    Task<bool> AnyAsync(Guid id);

    Task AddAsync(Genre genre);

    Task UpdateAsync(Genre genre);

    Task<bool> DeleteAsync(Guid id);
}
