using LibaryManagement.Models;
using LibaryManagement.Services.Interfaces;
using LibaryManagement.ResgisterDTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace LibaryManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PurchasesController : ControllerBase
    {
        private readonly IPurchaseService _purchaseService;

        public PurchasesController(IPurchaseService purchaseService)
        {
            _purchaseService = purchaseService;
        }

        // GET: api/Purchases
        // عرض جميع المشتريات للمسؤولين
        [HttpGet]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> GetAll()
        {
            var purchases = await _purchaseService.GetAllAsync();

            return Ok(purchases);
        }

        // GET: api/Purchases/my
        // عرض مشتريات المستخدم الحالي
        [HttpGet("my")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> GetMyPurchases()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var purchases = await _purchaseService.GetMyPurchasesAsync(userId);

            return Ok(purchases);
        }

        // GET: api/Purchases/my/{id}
        // عرض عملية شراء محددة تخص المستخدم الحالي
        [HttpGet("my/{id}")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> GetMyPurchaseById(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var purchase = await _purchaseService.GetMyPurchaseByIdAsync(id, userId);

            if (purchase == null)
                return NotFound("طلب الشراء غير موجود.");

            return Ok(purchase);
        }

        // GET: api/Purchases/{id}
        // عرض عملية شراء حسب ID للمسؤولين
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> GetById(int id)
        {
            var purchase = await _purchaseService.GetByIdAsync(id);

            if (purchase == null)
                return NotFound("طلب الشراء غير موجود.");

            return Ok(purchase);
        }

        // POST: api/Purchases
        // إضافة عملية شراء للمستخدم
        [HttpPost]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> Add(CreatePurchaseDto dto)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                var userName = User.FindFirstValue(ClaimTypes.Name)
                               ?? User.Identity?.Name;

                if (string.IsNullOrEmpty(userId))
                    return Unauthorized();

                if (string.IsNullOrWhiteSpace(userName))
                    return Unauthorized("User name not found in token.");

                var purchase = new Purchase
                {
                    BookId = dto.BookId,
                    Quantity = dto.Quantity,
                    UserId = userId,
                    UserName = userName
                };

                var result = await _purchaseService.AddAsync(purchase);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT: api/Purchases/{id}
        // تحديث حالة الطلب للمسؤولين
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> Update(int id, UpdatePurchaseStatusDto dto)
        {
            try
            {
                var result = await _purchaseService.UpdateAsync(id, dto.Status);

                if (!result)
                    return NotFound("طلب الشراء غير موجود.");

                return Ok("تم تحديث حالة طلب الشراء بنجاح.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // DELETE: api/Purchases/{id}
        // حذف عملية شراء للمسؤولين
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var result = await _purchaseService.DeleteAsync(id);

                if (!result)
                    return NotFound("طلب الشراء غير موجود.");

                return Ok("تم حذف طلب الشراء بنجاح.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}