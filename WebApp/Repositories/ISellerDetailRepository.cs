using COCOApp.Models;
using System.Collections.Generic;

namespace COCOApp.Repositories
{
    public interface ISellerDetailRepository
    {
        void AddSellerDetails(SellerDetail details);
        SellerDetail GetSellerDetailsById(int id);
        void UpdateSellerDetails(int userId, SellerDetail detail);
    }
}
