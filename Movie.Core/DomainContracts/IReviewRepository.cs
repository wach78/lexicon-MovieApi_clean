using System;
using System.Collections.Generic;
using System.Text;
using Movie.Core.Entities;

namespace Movie.Core.DomainContracts;

public interface IReviewRepository
{
    Task<IEnumerable<Review>> GetAllAsync();
    Task<Review?> GetAsync(Guid id);
    Task<bool> AnyAsync(Guid id);
    void Add(Review actor);
    void Update(Review actor);
    void Remove(Review actor);
}
