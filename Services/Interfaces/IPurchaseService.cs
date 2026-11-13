using LibaryManagement.Models;

namespace LibaryManagement.Services.Interfaces
{
    public interface IPurchaseService
    {
        // عرض جميع المشتريات
        Task<IEnumerable<Purchase>> GetAllAsync();

        // عرض مشتريات المستخدم الحالي
        Task<IEnumerable<Purchase>> GetMyPurchasesAsync(string userId);

        // عرض عملية شراء حسب ID
        Task<Purchase?> GetByIdAsync(int id);

        // عرض عملية شراء محددة تخص المستخدم الحالي
        Task<Purchase?> GetMyPurchaseByIdAsync(int id, string userId);

        // إضافة عملية شراء
        Task<Purchase> AddAsync(Purchase purchase);

        // تحديث حالة الشراء
        Task<bool> UpdateAsync(int id, string status);

        // حذف عملية شراء
        Task<bool> DeleteAsync(int id);

        // معالجة الطلبات المنتهية
        Task ExpirePurchasesAsync();
    }
}