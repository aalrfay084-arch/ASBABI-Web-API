using LibaryManagement.Identity;
using LibaryManagement.ResgisterDTO;
using LibaryManagement.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using static LibaryManagement.ResgisterDTO.LoginDTO;

namespace LibaryManagement.Services.Implemenations
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;

        public AuthService(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
        }

        // إنشاء حساب جديد
        public async Task<string> RegisterAsync(RegisterDto model)
        {
            var existingUser = await _userManager
                .FindByEmailAsync(model.Email);

            if (existingUser != null)
                throw new Exception("البريد الإلكتروني مستخدم بالفعل.");

            var user = new ApplicationUser
            {
                FullName = model.FullName,
                UserName = model.Email,
                Email = model.Email
            };

            var result = await _userManager
                .CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                throw new Exception(
                    string.Join(", ", result.Errors.Select(e => e.Description)));
            }

            // التأكد من وجود دور User
            if (!await _roleManager.RoleExistsAsync("User"))
            {
                await _roleManager.CreateAsync(
                    new IdentityRole("User"));
            }

            // جميع الحسابات الجديدة تحصل على دور User
            await _userManager.AddToRoleAsync(user, "User");

            return "تم إنشاء الحساب بنجاح.";
        }

        // تسجيل الدخول
        public async Task<string> LoginAsync(LoginDto model)
        {
            var user = await _userManager
                .FindByEmailAsync(model.Email);

            if (user == null)
                throw new Exception("البريد الإلكتروني أو كلمة المرور غير صحيحة.");

            var passwordValid = await _userManager
                .CheckPasswordAsync(user, model.Password);

            if (!passwordValid)
                throw new Exception("البريد الإلكتروني أو كلمة المرور غير صحيحة.");

            var userRoles = await _userManager.GetRolesAsync(user);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName!),
                new Claim(ClaimTypes.Email, user.Email!)
            };

            foreach (var role in userRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(
                    _configuration["Jwt:Key"]!));

            var credentials = new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(
                    double.Parse(
                        _configuration["Jwt:DurationInMinutes"]!)),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler()
                .WriteToken(token);
        }

        public async Task<string> CreateAdminAsync(CreateAdminDto model)
        {
            // التأكد من عدم وجود البريد مسبقاً
            var existingUser = await _userManager.FindByEmailAsync(model.Email);

            if (existingUser != null)
                throw new Exception("البريد الإلكتروني مستخدم بالفعل.");

            // إنشاء حساب المسؤول
            var admin = new ApplicationUser
            {
                FullName = model.FullName,
                UserName = model.Email,
                Email = model.Email
            };

            var result = await _userManager.CreateAsync(admin, model.Password);

            if (!result.Succeeded)
            {
                throw new Exception(
                    string.Join(", ", result.Errors.Select(e => e.Description))
                );
            }

            // إسناد دور Admin تلقائياً
            var roleResult = await _userManager.AddToRoleAsync(admin, "Admin");

            if (!roleResult.Succeeded)
            {
                await _userManager.DeleteAsync(admin);

                throw new Exception(
                    string.Join(", ", roleResult.Errors.Select(e => e.Description))
                );
            }

            return "تم إنشاء حساب المسؤول بنجاح.";
        }
    }
}
