using LibaryManagement.Data;
using LibaryManagement.Models;
using LibaryManagement.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LibaryManagement.Services.Implemenations
{
    public class BookService : IBookService
    {
        private readonly ApplicationDbContext _context;

        public BookService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Book>> GetAllAsync()
        {
            return await _context.Books
                .Include(b => b.Author)
                .Include(b => b.Category)
                .ToListAsync();
        }

        public async Task<Book?> GetByIdAsync(int id)
        {
            return await _context.Books
                .Include(b => b.Author)
                .Include(b => b.Category)
                .FirstOrDefaultAsync(b => b.BookId == id);
        }

        public async Task<Book> AddAsync(Book book)
        {
            bool authorExists = await _context.Authors
                .AnyAsync(a => a.AuthorId == book.AuthorId);

            bool categoryExists = await _context.Categories
                .AnyAsync(c => c.CategoryId == book.CategoryId);

            if (!authorExists)
                throw new ArgumentException("Author does not exist.");

            if (!categoryExists)
                throw new ArgumentException("Category does not exist.");

            _context.Books.Add(book);
            await _context.SaveChangesAsync();

            return book;
        }

        public async Task<bool> UpdateAsync(int id, Book book)
        {
            var existingBook = await _context.Books.FindAsync(id);

            if (existingBook == null)
                return false;

            bool authorExists = await _context.Authors
                .AnyAsync(a => a.AuthorId == book.AuthorId);

            bool categoryExists = await _context.Categories
                .AnyAsync(c => c.CategoryId == book.CategoryId);

            if (!authorExists || !categoryExists)
                throw new ArgumentException("Invalid Author or Category.");

            existingBook.Title = book.Title;
            existingBook.Price = book.Price;
            existingBook.Quantity = book.Quantity;
            existingBook.AuthorId = book.AuthorId;
            existingBook.CategoryId = book.CategoryId;

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var book = await _context.Books.FindAsync(id);

            if (book == null)
                return false;

            _context.Books.Remove(book);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
