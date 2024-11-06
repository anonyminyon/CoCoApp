using COCOApp.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace COCOApp.Repositories
{
    public class UserDetailRepository : IUserDetailRepository
    {
        private readonly StoreManagerContext _context;

        public UserDetailRepository(StoreManagerContext context)
        {
            _context = context;
        }
        public void AddUserDetails(UserDetail details)
        {
            _context.UserDetails.Add(details);
            _context.SaveChanges();
        }
        public UserDetail GetUserDetailsById(int id)
        {
            var user = _context.UserDetails.Include(u => u.User)
                                           .FirstOrDefault(u => u.UserId == id);
            if (user != null)
            {
                return user;
            }

            return null; // User not found 
        }
        public void UpdateUserDetails(int userId, UserDetail detail)
        {
            var existingDetail = _context.UserDetails.SingleOrDefault(d => d.UserId == userId);

            if (existingDetail == null)
            {
                throw new ArgumentException("Seller detail not found.");
            }

            existingDetail.Fullname = detail.Fullname;
            existingDetail.Dob = detail.Dob;
            existingDetail.Gender = detail.Gender;
            existingDetail.Address = detail.Address;
            existingDetail.Phone = detail.Phone;

            _context.SaveChanges();
        }
    }
}
