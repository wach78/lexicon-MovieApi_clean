namespace Movie.Core.DTOs.Actor;

public sealed class MostActiveActorDto
{
    public Guid ActorId { get; init; }

    public string Name { get; init; } = string.Empty;

    public int MovieCount { get; init; }
}
