using AutoMapper;
using EmployeeOnboarding_DDMS.Aplication.DTOs.Auth;
using EmployeeOnboarding_DDMS.Aplication.Interfaces;
using EmployeeOnboarding_DDMS.Aplication.Wrappers;
using EmployeeOnboarding_DDMS.Domain.Entities;
using EmployeeOnboarding_DDMS.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace EmployeeOnboarding_DDMS.Aplication.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;

        public AuthService(
            IUserRepository userRepository,
            IConfiguration configuration,
            IMapper mapper)
        {
            _userRepository = userRepository;
            _configuration = configuration;
            _mapper = mapper;
        }

        public async Task<Response<AuthResponseDto>> LoginAsync(LoginDto loginDto)
        {
            var user = await _userRepository.GetByEmailAsync(loginDto.Email);
            if (user == null)
            {
                return new Response<AuthResponseDto>("Invalid email or password.");
            }

            if (!user.IsActive)
            {
                return new Response<AuthResponseDto>("Account is deactivated.");
            }

            if (!BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
            {
                return new Response<AuthResponseDto>("Invalid email or password.");
            }

            user.LastLogin = DateTime.UtcNow;
            await _userRepository.UpdateAsync(user);

            var token = GenerateJwtToken(user);
            var authResponse = new AuthResponseDto
            {
                UserId = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Role = user.Role,
                Token = token,
                Expires = DateTime.UtcNow.AddHours(8), // Token expires in 8 hours
                MustChangePassword = user.MustChangePassword
            };

            return new Response<AuthResponseDto>(authResponse, "Login successful.");
        }

        public async Task<Response<AuthResponseDto>> RegisterAsync(RegisterDto registerDto)
        {
            // Check if email already exists
            if (await _userRepository.EmailExistsAsync(registerDto.Email))
            {
                return new Response<AuthResponseDto>("Email already exists.");
            }

            // Hash password
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password);

            var user = new User
            {
                Email = registerDto.Email,
                PasswordHash = passwordHash,
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName,
                Role = registerDto.Role,
                IsActive = true,
                CreatedBy = "System",
                Created = DateTime.UtcNow,
                LastModifiedBy = "System"
            };

            var createdUser = await _userRepository.AddAsync(user);
            var token = GenerateJwtToken(createdUser);

            var authResponse = new AuthResponseDto
            {
                UserId = createdUser.Id,
                Email = createdUser.Email,
                FirstName = createdUser.FirstName,
                LastName = createdUser.LastName,
                Role = createdUser.Role,
                Token = token,
                Expires = DateTime.UtcNow.AddHours(8)
            };

            return new Response<AuthResponseDto>(authResponse, "Registration successful.");
        }

        public async Task<Response<bool>> ChangePasswordAsync(int userId, string oldPassword, string newPassword)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                return new Response<bool>("User not found.");
            }

            if (!BCrypt.Net.BCrypt.Verify(oldPassword, user.PasswordHash))
            {
                return new Response<bool>("Current password is incorrect.");
            }

            // Validate new password
            if (newPassword.Length < 8)
            {
                return new Response<bool>("Password must be at least 8 characters.");
            }

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
            user.MustChangePassword = false; // Clear the flag after password change
            await _userRepository.UpdateAsync(user);

            return new Response<bool>(true, "Password changed successfully.");
        }

        private string GenerateJwtToken(User user)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings["SecretKey"] ?? "YourSuperSecretKeyThatShouldBeAtLeast32CharactersLong!";
            var issuer = jwtSettings["Issuer"] ?? "EmployeeOnboardingDDMS";
            var audience = jwtSettings["Audience"] ?? "EmployeeOnboardingDDMS";
            var expiryMinutes = int.Parse(jwtSettings["ExpiryMinutes"] ?? "480"); // 8 hours default

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
                new Claim(ClaimTypes.Role, user.Role.ToString()),
                new Claim("UserId", user.Id.ToString())
            };

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}

