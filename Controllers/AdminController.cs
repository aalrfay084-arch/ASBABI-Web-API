using LibaryManagement.ResgisterDTO;
using LibaryManagement.Services.Interfaces;
using LibaryManagement.Data;
using LibaryManagement.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace LibaryManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public AdminController(
            IAuthService authService,
            UserManager<ApplicationUser> userManager,
            ApplicationDbContext context)
        {
            _authService = authService;
            _userManager = userManager;
            _context = context;
        }

        // إنشاء حساب Admin جديد
        [HttpPost("create")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> CreateAdmin(CreateAdminDto model)
        {
            try
            {
                var result = await _authService.CreateAdminAsync(model);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // : عرض جميع المستخدمين
        [HttpGet("users")]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _userManager.GetUsersInRoleAsync("User");

            var result = users.Select(u => new
            {
                u.Id,
                u.FullName,
                u.Email,
                u.UserName
            });

            return Ok(result);
        }

        // : حذف مستخدم
        [HttpDelete("users/{id}")]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
                return NotFound("User not found.");

            var isUser = await _userManager.IsInRoleAsync(user, "User");

            if (!isUser)
                return BadRequest("This account is not a User account.");

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (currentUserId == id)
                return BadRequest("You cannot delete your own account.");

            var hasPurchases = await _context.Purchases
                .AnyAsync(p => p.UserId == id);

            var hasBorrowings = await _context.Borrowings
                .AnyAsync(b => b.UserId == id);

            if (hasPurchases || hasBorrowings)
            {
                return BadRequest(
                    "Cannot delete this user because they have purchases or borrowings.");
            }

            var result = await _userManager.DeleteAsync(user);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok("User deleted successfully.");
        }

        // : عرض جميع المسؤولين
        [HttpGet("admins")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> GetAdmins()
        {
            var admins = await _userManager.GetUsersInRoleAsync("Admin");

            var result = admins.Select(u => new
            {
                u.Id,
                u.FullName,
                u.Email,
                u.UserName
            });

            return Ok(result);
        }

        //  حذف مسؤول
        [HttpDelete("admins/{id}")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> DeleteAdmin(string id)
        {
            var admin = await _userManager.FindByIdAsync(id);

            if (admin == null)
                return NotFound("Admin not found.");

            var isAdmin = await _userManager.IsInRoleAsync(admin, "Admin");

            if (!isAdmin)
                return BadRequest("This account is not an Admin account.");

            var isSuperAdmin = await _userManager.IsInRoleAsync(admin, "SuperAdmin");

            if (isSuperAdmin)
                return BadRequest("Cannot delete a SuperAdmin account.");

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (currentUserId == id)
                return BadRequest("You cannot delete your own account.");

            var hasPurchases = await _context.Purchases
                .AnyAsync(p => p.UserId == id);

            var hasBorrowings = await _context.Borrowings
                .AnyAsync(b => b.UserId == id);

            if (hasPurchases || hasBorrowings)
            {
                return BadRequest(
                    "Cannot delete this Admin because they have purchases or borrowings.");
            }

            var result = await _userManager.DeleteAsync(admin);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok("Admin deleted successfully.");
        }
    }
}