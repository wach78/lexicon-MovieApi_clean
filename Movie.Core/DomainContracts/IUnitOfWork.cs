using System;
using System.Collections.Generic;
using System.Text;

namespace Movie.Core.DomainContracts;

public interface IUnitOfWork
{
    IMovieRepository Movies { get; }
    IReviewRepository Reviews { get; }
    IActorRepository Actors { get; }
    IMovieDetailsRepository MovieDetails { get; }
    IGenreRepository Genres { get; }
    Task CompleteAsync(CancellationToken cancellationToken = default);
}
