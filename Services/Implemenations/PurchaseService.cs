using LibaryManagement.Data;
using LibaryManagement.Models;
using LibaryManagement.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LibaryManagement.Services.Implemenations
{
    public class PurchaseService : IPurchaseService
    {
        private readonly ApplicationDbContext _context;

        public PurchaseService(ApplicationDbContext context)
        {
            _context = context;
        }

        // عرض جميع المشتريات
        public async Task<IEnumerable<Purchase>> GetAllAsync()
        {
            await ExpirePurchasesAsync();

            return await _context.Purchases
                .Include(p => p.Book)
                .ToListAsync();
        }

        // عرض مشتريات المستخدم الحالي
        public async Task<IEnumerable<Purchase>> GetMyPurchasesAsync(string userId)
        {
            try
            {
                await ExpirePurchasesAsync();

            return await _context.Purchases
                .Include(p => p.Book)
                .Where(p => p.UserId == userId)
                .ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("PURCHASE ERROR: " + ex.ToString());
                throw;
            }
        }

        // عرض عملية شراء حسب ID
        public async Task<Purchase?> GetByIdAsync(int id)
        {
            await ExpirePurchasesAsync();

            return await _context.Purchases
                .Include(p => p.Book)
                .FirstOrDefaultAsync(p => p.PurchaseId == id);
        }

        // عرض عملية شراء محددة تخص المستخدم الحالي
        public async Task<Purchase?> GetMyPurchaseByIdAsync(int id, string userId)
        {
            await ExpirePurchasesAsync();

            return await _context.Purchases
                .Include(p => p.Book)
                .FirstOrDefaultAsync(p =>
                    p.PurchaseId == id &&
                    p.UserId == userId);
        }

        // إضافة عملية شراء
        public async Task<Purchase> AddAsync(Purchase purchase)
        {
            var book = await _context.Books
                .FirstOrDefaultAsync(b => b.BookId == purchase.BookId);

            if (book == null)
                throw new Exception("الكتاب غير موجود.");

            if (purchase.Quantity <= 0)
                throw new Exception("الكمية يجب أن تكون أكبر من صفر.");

            if (book.Quantity < purchase.Quantity)
                throw new Exception("الكمية المطلوبة غير متوفرة.");

            purchase.UnitPrice = book.Price;

            purchase.TotalAmount = book.Price * purchase.Quantity;

            purchase.PurchaseDate = DateTime.Now;

            purchase.ApprovalDate = null;

            purchase.ExpiryDate = null;

            purchase.CompletionDate = null;

            purchase.Status = "Pending";

            purchase.PaymentStatus = "Unpaid";

            _context.Purchases.Add(purchase);

            await _context.SaveChangesAsync();

            return purchase;
        }

        // تحديث حالة الشراء
        public async Task<bool> UpdateAsync(int id, string status)
        {
            await ExpirePurchasesAsync();

            var purchase = await _context.Purchases
                .FirstOrDefaultAsync(p => p.PurchaseId == id);

            if (purchase == null)
                return false;

            if (string.IsNullOrWhiteSpace(status))
                throw new Exception("يجب تحديد حالة الطلب.");

            status = status.Trim();

            // الموافقة على الطلب
            if (status.Equals("Approved", StringComparison.OrdinalIgnoreCase))
            {
                if (purchase.Status != "Pending")
                    throw new Exception("لا يمكن الموافقة على هذا الطلب.");

                var book = await _context.Books
                    .FirstOrDefaultAsync(b => b.BookId == purchase.BookId);

                if (book == null)
                    throw new Exception("الكتاب غير موجود.");

                if (book.Quantity < purchase.Quantity)
                    throw new Exception("الكمية المطلوبة غير متوفرة.");

                // حجز الكمية من المخزون
                book.Quantity -= purchase.Quantity;

                purchase.ApprovalDate = DateTime.Now;

                // تحديد مهلة الاستلام بثلاثة أيام
                purchase.ExpiryDate = DateTime.Now.AddDays(3);

                purchase.Status = "Approved";
            }

            // رفض الطلب
            else if (status.Equals("Rejected", StringComparison.OrdinalIgnoreCase))
            {
                if (purchase.Status != "Pending")
                    throw new Exception("لا يمكن رفض هذا الطلب.");

                purchase.Status = "Rejected";
            }

            // إكمال الطلب
            else if (status.Equals("Completed", StringComparison.OrdinalIgnoreCase))
            {
                if (purchase.Status != "Approved")
                    throw new Exception("لا يمكن إكمال هذا الطلب.");

                if (!purchase.ExpiryDate.HasValue ||
                    purchase.ExpiryDate.Value <= DateTime.Now)
                {
                    throw new Exception("انتهت مهلة استلام الكتاب.");
                }

                purchase.Status = "Completed";

                purchase.PaymentStatus = "Paid";

                purchase.CompletionDate = DateTime.Now;
            }

            else
            {
                throw new Exception("حالة الطلب غير صحيحة.");
            }

            await _context.SaveChangesAsync();

            return true;
        }

        // حذف عملية شراء
        public async Task<bool> DeleteAsync(int id)
        {
            var purchase = await _context.Purchases
                .FirstOrDefaultAsync(p => p.PurchaseId == id);

            if (purchase == null)
                return false;

            if (purchase.Status == "Approved" ||
                purchase.Status == "Completed")
            {
                throw new Exception("لا يمكن حذف هذا الطلب.");
            }

            _context.Purchases.Remove(purchase);

            await _context.SaveChangesAsync();

            return true;
        }

        // معالجة الطلبات المنتهية
        public async Task ExpirePurchasesAsync()
        {
            var now = DateTime.Now;

            var expiredPurchases = await _context.Purchases
                .Where(p =>
                    p.Status == "Approved" &&
                    p.ExpiryDate.HasValue &&
                    p.ExpiryDate.Value <= now)
                .ToListAsync();

            foreach (var purchase in expiredPurchases)
            {
                var book = await _context.Books
                    .FirstOrDefaultAsync(b => b.BookId == purchase.BookId);

                if (book != null)
                {
                    // إعادة الكمية إلى المخزون
                    book.Quantity += purchase.Quantity;
                }

                purchase.Status = "Expired";
            }

            await _context.SaveChangesAsync();
        }
    }
}