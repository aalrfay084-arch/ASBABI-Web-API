using LibaryManagement.Models;

namespace LibaryManagement.Services.Interfaces
{
    public interface IAuthorService
    {
       
        Task<IEnumerable<Author>> GetAllAsync();

     
        Task<Author?> GetByIdAsync(int id);

        
        Task<Author> AddAsync(Author author);

   
        Task<bool> UpdateAsync(int id, Author author);

        Task<bool> DeleteAsync(int id);
    }
}
