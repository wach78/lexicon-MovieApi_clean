using System;
using System.Collections.Generic;
using System.Text;

namespace Movie.Service.Contracts.Interfaces;

public interface IServiceManager
{
    IMovieService Movies { get; }

    IActorService Actors { get; }

    IReviewService Reviews { get; }

    IReportsService Reports { get; }
    IGenreService Genres { get; }
}

