using COCOApp.Models;

namespace COCOApp.Services
{
    public class StoreManagerService
    {
        protected StoreManagerContext _context;

        public StoreManagerService()
        {
            _context = new StoreManagerContext();

        }

    }
}
