using COCOApp.Models;
using COCOApp.Repositories;

namespace COCOApp.Services
{
    public class UserDetailsService : StoreManagerService
    {
        private readonly IUserDetailRepository _userDetailRepository;

        public UserDetailsService(IUserDetailRepository userDetailRepository)
        {
            _userDetailRepository = userDetailRepository;
        }

        public void AddUserDetails(UserDetail details)
        {
            _userDetailRepository.AddUserDetails(details);
        }
        public UserDetail GetUserDetailsById(int id)
        {
            return _userDetailRepository.GetUserDetailsById(id);
        }
        public void UpdateUserDetails(int userId, UserDetail detail)
        {
            _userDetailRepository.UpdateUserDetails(userId, detail);
        }
    }
}
