using COCOApp.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace COCOApp.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly StoreManagerContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserRepository(StoreManagerContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public List<User> GetUsers()
        {
            var query = _context.Users.AsQueryable();
            query = query.Include(u => u.UserDetail);
            return query.ToList();
        }

        public List<User> GetUsers(string nameQuery, int pageNumber, int pageSize)
        {
            pageNumber = Math.Max(pageNumber, 1);

            var query = _context.Users.AsQueryable();
            if (!string.IsNullOrEmpty(nameQuery))
            {
                query = query.Where(c => c.Username.Contains(nameQuery));
            }
            query = query.OrderByDescending(p => p.Id);
            return query.Skip((pageNumber - 1) * pageSize)
                        .Take(pageSize)
                        .ToList();
        }

        public int GetTotalUsers(string nameQuery)
        {
            var query = _context.Users.AsQueryable();
            if (!string.IsNullOrEmpty(nameQuery))
            {
                query = query.Where(c => c.Username.Contains(nameQuery));
            }
            return query.Count();
        }

        public void AddUser(User user)
        {
            if (_context.Users.Any(u => u.Email == user.Email))
            {
                throw new ArgumentException("Email is already in use.");
            }

            if (_context.Users.Any(u => u.Username == user.Username))
            {
                throw new ArgumentException("Username is already in use.");
            }
            _context.Users.Add(user);
            _context.SaveChanges();
        }

        public void UpdateUser(int userId, User user)
        {
            var existingUser = _context.Users.SingleOrDefault(u => u.Id == userId);

            if (existingUser == null)
            {
                throw new ArgumentException("User not found.");
            }

            User userByName = GetUserByUsername(user.Username);
            User userByEmail = GetUserByEmail(user.Email);
            if (userByEmail != null && userByName != null && (userByName.Id != user.Id || userByEmail.Id != user.Id))
            {
                throw new ArgumentException("Duplicated name or email.");
            }

            existingUser.Email = user.Email;
            existingUser.Username = user.Username;
            existingUser.UpdatedAt = user.UpdatedAt;
            if (user.Role > 0)
            {
                existingUser.Role = user.Role;
            }
            if (user.Status != null)
            {
                existingUser.Status = user.Status;
            }

            _context.SaveChanges();
        }

        public void UpdateUserPassword(int userId, string password)
        {
            var existingUser = _context.Users.SingleOrDefault(u => u.Id == userId);

            if (existingUser == null)
            {
                throw new ArgumentException("User not found.");
            }

            existingUser.Password = password;

            _context.SaveChanges();
        }

        public User GetUserByNameAndPass(string username, string password)
        {
            var user = _context.Users.Include(u => u.UserDetail)
                .Include(s => s.SellerDetail)
                .FirstOrDefault(u => u.Username == username);

            if (user != null && user.Status == true && BCrypt.Net.BCrypt.Verify(password, user.Password))
            {
                return user;
            }

            return null; // User not found or password incorrect
        }

        public User GetUserById(int userId)
        {
            return _context.Users.Include(u => u.UserDetail)
                .Include(s => s.SellerDetail)
                .FirstOrDefault(u => u.Id == userId);
        }

        public User GetActiveUserByEmail(string email)
        {
            var user = _context.Users.Include(u => u.UserDetail)
                .FirstOrDefault(u => u.Email == email);

            if (user != null && user.Status == true)
            {
                return user;
            }

            return null; // User not found 
        }

        public User GetUserByUsername(string username)
        {
            return _context.Users.Include(u => u.UserDetail)
                                 .FirstOrDefault(u => u.Username == username);
        }

        public User GetUserByEmail(string email)
        {
            return _context.Users.Include(u => u.UserDetail)
                .FirstOrDefault(u => u.Email == email);
        }

        public async Task UpdateUserPasswordResetTokenAsync(string email)
        {
            var user = await _context.Users.Include(u => u.UserDetail)
                .FirstOrDefaultAsync(u => u.Email == email);

            if (user != null)
            {
                user.ResetPasswordToken = Guid.NewGuid().ToString();

                await _context.SaveChangesAsync();

                var expirationTime = DateTime.UtcNow.AddHours(24);
                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    Expires = expirationTime
                };

                _httpContextAccessor.HttpContext.Response.Cookies.Append("PasswordResetToken", user.ResetPasswordToken, cookieOptions);
                _httpContextAccessor.HttpContext.Response.Cookies.Append("PasswordResetTokenExpiration", expirationTime.ToString("o"), cookieOptions);

                Console.WriteLine($"Password reset token updated for: {email}");
            }
            else
            {
                Console.WriteLine($"No user found with email: {email}");
            }
        }

        public async Task<bool> CheckPasswordResetTokenAsync(string email, string resetToken)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

            if (user != null && user.ResetPasswordToken == resetToken)
            {
                Console.WriteLine("Password reset token is valid and matches the database.");
                return true;
            }
            else
            {
                return false;
            }
        }
        public async Task UpdateRemembermeTokenAsync(String username)
        {
            var user = await _context.Users.Include(u => u.UserDetail)
                .FirstOrDefaultAsync(u => u.Username == username);

            if (user != null)
            {
                user.RememberToken = Guid.NewGuid().ToString();

                await _context.SaveChangesAsync();

                var expirationTime = DateTime.UtcNow.AddDays(30);
                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    Expires = expirationTime
                };

                _httpContextAccessor.HttpContext.Response.Cookies.Append("RememberMeToken", user.RememberToken, cookieOptions);
                _httpContextAccessor.HttpContext.Response.Cookies.Append("RememberMeTokenTokenExpiration", expirationTime.ToString("o"), cookieOptions);

            }
            else
            {
                Console.WriteLine($"No user found with username: {username}");
            }
        }
        public async Task RemoveRemembermeTokenAsync(String username)
        {
            var user = await _context.Users.Include(u => u.UserDetail)
                .FirstOrDefaultAsync(u => u.Username == username);

            if (user != null)
            {
                user.RememberToken = "";

                await _context.SaveChangesAsync();

            }
            else
            {
                Console.WriteLine($"No user found with username: {username}");
            }
        }
        public async Task<User> CheckRemembermeTokenAsync()
        {
            var tokenExpiration = _httpContextAccessor.HttpContext.Request.Cookies["RememberMeTokenTokenExpiration"];
            var tokenFromCookie = _httpContextAccessor.HttpContext.Request.Cookies["RememberMeToken"];

            if (tokenExpiration != null && tokenFromCookie != null)
            {
                if (DateTime.TryParse(tokenExpiration, out DateTime expirationTime))
                {
                    if (DateTime.UtcNow <= expirationTime)
                    {

                        var user = await _context.Users.Include(u => u.UserDetail)
                          .Include(s => s.SellerDetail)
                          .FirstOrDefaultAsync(u => u.RememberToken == tokenFromCookie);

                        if (user != null)
                        {
                            Console.WriteLine("Remember me token is valid and matches the database.");
                            return user;
                        }
                        else
                        {
                            Console.WriteLine("Remember me token does not match or user not found.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Remember me token has expired.");
                    }
                }
            }
            else
            {
                Console.WriteLine("No Remember me token found.");
            }
            return null;
        }
    }
}
