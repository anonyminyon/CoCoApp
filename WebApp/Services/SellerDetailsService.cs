using COCOApp.Models;
using COCOApp.Repositories;

namespace COCOApp.Services
{
    public class SellerDetailsService : StoreManagerService
    {
        private readonly ISellerDetailRepository _sellerDetailRepository;

        public SellerDetailsService(ISellerDetailRepository sellerDetailRepository)
        {
            _sellerDetailRepository = sellerDetailRepository;
        }

        public void AddSellerDetails(SellerDetail details)
        {
            _sellerDetailRepository.AddSellerDetails(details);
        }
        public void UpdateSellerDetails(int userId, SellerDetail detail)
        {
            _sellerDetailRepository.UpdateSellerDetails(userId, detail);
        }
    }
}
