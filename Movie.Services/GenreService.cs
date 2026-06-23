using Movie.Core.DomainContracts;
using Movie.Core.Entities;
using Movie.Service.Contracts.Interfaces;

namespace Movie.Services;

public sealed class GenreService : IGenreService
{
    private readonly IUnitOfWork _unitOfWork;

    public GenreService(IUnitOfWork unitOfWork)
    {
        ArgumentNullException.ThrowIfNull(unitOfWork);

        _unitOfWork = unitOfWork;
    }

    public async Task<IReadOnlyList<Genre>> GetAllAsync()
    {
        IEnumerable<Genre> genres =
            await _unitOfWork.Genres.GetAllAsync();

        return genres.ToList();
    }

    public async Task<Genre?> GetAsync(Guid id)
    {
        return await _unitOfWork.Genres.GetAsync(id);
    }

    public async Task<bool> AnyAsync(Guid id)
    {
        return await _unitOfWork.Genres.AnyAsync(id);
    }

    public async Task AddAsync(Genre genre)
    {
        ArgumentNullException.ThrowIfNull(genre);

        _unitOfWork.Genres.Add(genre);

        await _unitOfWork.CompleteAsync();
    }

    public async Task UpdateAsync(Genre genre)
    {
        ArgumentNullException.ThrowIfNull(genre);

        _unitOfWork.Genres.Update(genre);

        await _unitOfWork.CompleteAsync();
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        Genre? genre = await _unitOfWork.Genres.GetAsync(id);

        if (genre is null)
        {
            return false;
        }

        _unitOfWork.Genres.Remove(genre);

        await _unitOfWork.CompleteAsync();

        return true;
    }
}
