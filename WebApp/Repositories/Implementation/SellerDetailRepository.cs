using COCOApp.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace COCOApp.Repositories
{
    public class SellerDetailRepository : ISellerDetailRepository
    {
        private readonly StoreManagerContext _context;

        public SellerDetailRepository(StoreManagerContext context)
        {
            _context = context;
        }
        public void AddSellerDetails(SellerDetail details)
        {
            _context.SellerDetails.Add(details);
            _context.SaveChanges();
        }
        public SellerDetail GetSellerDetailsById(int id)
        {
            var user = _context.SellerDetails.Include(u => u.User)
                                           .FirstOrDefault(u => u.UserId == id);
            if (user != null)
            {
                return user;
            }

            return null; // User not found 
        }
        public void UpdateSellerDetails(int userId, SellerDetail detail)
        {
            var existingDetail = _context.SellerDetails.SingleOrDefault(d => d.UserId == userId);

            if (existingDetail == null)
            {
                throw new ArgumentException("Seller detail not found.");
            }

            existingDetail.BusinessName = detail.BusinessName;
            existingDetail.BusinessAddress = detail.BusinessAddress;
            existingDetail.ImageData = detail.ImageData;

            _context.SaveChanges();
        }
    }
}
