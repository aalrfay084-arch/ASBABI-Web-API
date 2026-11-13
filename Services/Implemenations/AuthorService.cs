using LibaryManagement.Data;
using LibaryManagement.Models;
using Microsoft.EntityFrameworkCore;
using LibaryManagement.Services.Interfaces;

namespace LibaryManagement.Services.Implemenations
{
    public class AuthorService:IAuthorService
    {
        private readonly ApplicationDbContext _context ;
        public AuthorService(ApplicationDbContext context)
        {
            _context = context;
        }


        public async Task<IEnumerable<Author>> GetAllAsync()
        {
            return await _context.Authors.ToListAsync();
        }

    
        public async Task<Author?> GetByIdAsync(int id)
        {
            return await _context.Authors.FindAsync(id);
        }

        
        public async Task<Author> AddAsync(Author author)
        {
            _context.Authors.Add(author);

            await _context.SaveChangesAsync();

            return author;
        }

        
        public async Task<bool> UpdateAsync(int id, Author author)
        {
            var existingAuthor = await _context.Authors.FindAsync(id);

            if (existingAuthor == null)
                return false;

            existingAuthor.Name = author.Name;
            existingAuthor.Biography = author.Biography;

            await _context.SaveChangesAsync();

            return true;
        }


        public async Task<bool> DeleteAsync(int id)
        {
            var author = await _context.Authors.FindAsync(id);

            if (author == null)
                return false;

            _context.Authors.Remove(author);

            await _context.SaveChangesAsync();

            return true;
        }
    }
}
    
    

