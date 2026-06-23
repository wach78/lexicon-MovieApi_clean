using System;
using System.Collections.Generic;
using System.Text;
using MovieEntity = Movie.Core.Entities.Movie;
namespace Movie.Core.DomainContracts;

public interface IMovieRepository
{
    Task<IEnumerable<MovieEntity>> GetAllAsync();
    Task<MovieEntity?> GetAsync(Guid id);
    Task<bool> AnyAsync(Guid id);
    void Add(MovieEntity movie);
    void Update(MovieEntity movie);
    void Remove(MovieEntity movie);
}
