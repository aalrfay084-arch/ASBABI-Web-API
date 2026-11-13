using LibaryManagement.Models;


namespace LibraryManagement.Services.Interfaces
{
    public interface IBorrowingService
    {
      
        Task<IEnumerable<Borrowing>> GetAllAsync();

        
        Task<Borrowing?> GetByIdAsync(int id);


        Task<IEnumerable<Borrowing>> GetMyBorrowingsAsync();


        Task<Borrowing?> GetMyBorrowingByIdAsync(int id);

        
        Task<Borrowing> AddAsync(Borrowing borrowing);

       
        Task<bool> UpdateAsync(int id, Borrowing borrowing);

       
        Task<bool> DeleteAsync(int id);

       
        Task<bool> PayLateFeeAsync(int id);
    }
}