using WebApplication1.Models.Entities;

namespace WebApplication1.Repositories.Interfaces;

public interface ITagRepository
{
    Task<Tag?> GetByIdAsync(int id);
    Task<List<Tag>> GetAllAsync();
    Task<List<Tag>> GetByIdsAsync(List<int> ids);
    Task<Tag> CreateAsync(Tag tag);
}

