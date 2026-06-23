using System;
using System.Collections.Generic;
using System.Text;
using Movie.Core.Entities;

namespace Movie.Core.DomainContracts;

public interface IMovieDetailsRepository
{
    Task<IEnumerable<MovieDetails>> GetAllAsync();
    Task<MovieDetails?> GetAsync(Guid id);
    Task<bool> AnyAsync(Guid id);
    void Add(MovieDetails movieDetails);
    void Update(MovieDetails movieDetails);
    void Remove(MovieDetails movieDetails);
}
