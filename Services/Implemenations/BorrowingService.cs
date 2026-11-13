using LibaryManagement.Data;
using LibaryManagement.Models;
using LibaryManagement.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using LibraryManagement.Services.Interfaces;

namespace LibaryManagement.Services.Implemenations
{
    public class BorrowingService : IBorrowingService
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        private const decimal BorrowingFee = 1000m;
        private const decimal DailyLateFee = 500m;
        private const int BorrowingDays = 3;

        public BorrowingService(
            ApplicationDbContext context,
            IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        // GET: All Borrowings
        public async Task<IEnumerable<Borrowing>> GetAllAsync()
        {
            var borrowings = await _context.Borrowings
                .Include(b => b.Book)
                .ToListAsync();

            await UpdateOverdueStatusesAsync(borrowings);

            return borrowings;
        }

        // GET: Borrowing By ID
        public async Task<Borrowing?> GetByIdAsync(int id)
        {
            var borrowing = await _context.Borrowings
                .Include(b => b.Book)
                .FirstOrDefaultAsync(b => b.BorrowingId == id);

            if (borrowing == null)
                return null;

            await UpdateOverdueStatusesAsync(
                new List<Borrowing> { borrowing });

            return borrowing;
        }

        // POST: Create Borrowing
        public async Task<Borrowing> AddAsync(Borrowing borrowing)
        {
            // Get logged-in user ID from JWT
            var userId = _httpContextAccessor.HttpContext?
                .User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedAccessException(
                    "User is not authenticated.");

            var book = await _context.Books
                .FindAsync(borrowing.BookId);

            if (book == null)
                throw new ArgumentException(
                    "Book does not exist.");

            if (book.Quantity <= 0)
                throw new InvalidOperationException(
                    "Book is not available.");

            // Link borrowing to logged-in user
            borrowing.UserId = userId;

            borrowing.RequestDate = DateTime.Now;
            borrowing.BorrowDate = null;
            borrowing.DueDate = null;
            borrowing.ReturnDate = null;

            borrowing.BorrowingFee = BorrowingFee;
            borrowing.LateFee = 0m;
            borrowing.IsLateFeePaid = false;
            borrowing.LateFeePaidDate = null;
            borrowing.Status = "Pending";

            _context.Borrowings.Add(borrowing);

            await _context.SaveChangesAsync();

            return borrowing;
        }

        // PUT: Update Borrowing Status
        public async Task<bool> UpdateAsync(
            int id, Borrowing borrowing)
        {
            var existingBorrowing = await _context.Borrowings
                .Include(b => b.Book)
                .FirstOrDefaultAsync(
                    b => b.BorrowingId == id);

            if (existingBorrowing == null)
                return false;

            string oldStatus = existingBorrowing.Status;
            string newStatus = borrowing.Status?.Trim() ?? "";

            string[] validStatuses =
            {
                "Pending",
                "Approved",
                "Rejected",
                "Returned",
                "Cancelled",
                "Overdue"
            };

            if (!validStatuses.Contains(newStatus))
                throw new ArgumentException(
                    "Invalid borrowing status.");

            if (oldStatus == newStatus)
                return true;

            var book = existingBorrowing.Book;

            if (book == null)
                throw new InvalidOperationException(
                    "The associated book was not found.");

            // Approve borrowing request
            if (oldStatus == "Pending" &&
                newStatus == "Approved")
            {
                if (book.Quantity <= 0)
                    throw new InvalidOperationException(
                        "Book is not available.");

                book.Quantity--;

                existingBorrowing.BorrowDate =
                    DateTime.Now;

                existingBorrowing.DueDate =
                    DateTime.Now.AddDays(BorrowingDays);

                existingBorrowing.BorrowingFee =
                    BorrowingFee;

                existingBorrowing.LateFee = 0m;
            }

            // Reject or cancel pending request
            else if (oldStatus == "Pending" &&
                (newStatus == "Rejected" ||
                 newStatus == "Cancelled"))
            {
                // No stock change
            }

            // Return borrowed book
            else if (
                (oldStatus == "Approved" ||
                 oldStatus == "Overdue") &&
                newStatus == "Returned")
            {
                if (existingBorrowing.DueDate == null)
                    throw new InvalidOperationException(
                        "Due date is missing.");

                existingBorrowing.ReturnDate =
                    DateTime.Now;

                int lateDays = Math.Max(
                    0,
                    (existingBorrowing.ReturnDate.Value.Date -
                     existingBorrowing.DueDate.Value.Date).Days
                );

                existingBorrowing.LateFee =
                    lateDays * DailyLateFee;

                book.Quantity++;
            }

            // Cancel approved borrowing
            else if (
                oldStatus == "Approved" &&
                newStatus == "Cancelled")
            {
                book.Quantity++;
            }

            else
            {
                throw new InvalidOperationException(
                    "This status transition is not allowed.");
            }

            existingBorrowing.Status = newStatus;

            await _context.SaveChangesAsync();

            return true;
        }

        // DELETE: Borrowing
        public async Task<bool> DeleteAsync(int id)
        {
            var borrowing = await _context.Borrowings
                .FirstOrDefaultAsync(
                    b => b.BorrowingId == id);

            if (borrowing == null)
                return false;

            if (borrowing.Status == "Approved" ||
                borrowing.Status == "Overdue")
            {
                throw new InvalidOperationException(
                    "Cannot delete an active borrowing.");
            }

            _context.Borrowings.Remove(borrowing);

            await _context.SaveChangesAsync();

            return true;
        }

        // Update overdue borrowing statuses
        private async Task UpdateOverdueStatusesAsync(
            IEnumerable<Borrowing> borrowings)
        {
            DateTime today = DateTime.Now.Date;
            bool hasChanges = false;

            foreach (var borrowing in borrowings)
            {
                if (borrowing.Status == "Approved" &&
                    borrowing.DueDate.HasValue &&
                    today > borrowing.DueDate.Value.Date)
                {
                    borrowing.Status = "Overdue";
                    hasChanges = true;
                }
            }

            if (hasChanges)
                await _context.SaveChangesAsync();
        }

        // Pay late fee
        public async Task<bool> PayLateFeeAsync(int id)
        {
            var borrowing = await _context.Borrowings
                .FirstOrDefaultAsync(
                    b => b.BorrowingId == id);

            if (borrowing == null)
                return false;

            if (borrowing.Status != "Returned")
                throw new Exception(
                    "يجب إرجاع الكتاب أولاً.");

            if (borrowing.LateFee <= 0)
                throw new Exception(
                    "لا توجد غرامة مستحقة.");

            if (borrowing.IsLateFeePaid)
                throw new Exception(
                    "تم سداد الغرامة مسبقاً.");

            borrowing.IsLateFeePaid = true;
            borrowing.LateFeePaidDate = DateTime.Now;

            await _context.SaveChangesAsync();

            return true;
        }
        // GET: Borrowings belonging to logged-in user
        public async Task<IEnumerable<Borrowing>> GetMyBorrowingsAsync()
        {
            var userId = _httpContextAccessor.HttpContext?
                .User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedAccessException(
                    "User is not authenticated.");

            var borrowings = await _context.Borrowings
                .Include(b => b.Book)
                .Where(b => b.UserId == userId)
                .ToListAsync();

            await UpdateOverdueStatusesAsync(borrowings);

            return borrowings;
        }

        // GET: Specific borrowing belonging to logged-in user
        public async Task<Borrowing?> GetMyBorrowingByIdAsync(int id)
        {
            var userId = _httpContextAccessor.HttpContext?
                .User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedAccessException(
                    "User is not authenticated.");

            var borrowing = await _context.Borrowings
                .Include(b => b.Book)
                .FirstOrDefaultAsync(b =>
                    b.BorrowingId == id &&
                    b.UserId == userId);

            if (borrowing == null)
                return null;

            await UpdateOverdueStatusesAsync(
                new List<Borrowing> { borrowing });

            return borrowing;
        }
    }
}