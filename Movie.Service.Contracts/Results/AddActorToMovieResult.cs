using System;
using System.Collections.Generic;
using System.Text;

namespace Movie.Service.Contracts.Results;

public enum AddActorToMovieResult
{
    Added,
    MovieNotFound,
    ActorNotFound,
    ActorAlreadyAdded
}
