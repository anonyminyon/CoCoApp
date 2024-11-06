using COCOApp.Models;
using COCOApp.Repositories;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace COCOApp.Services
{
    public class UserService : StoreManagerService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public List<User> GetUsers()
        {
            return _userRepository.GetUsers();
        }

        public List<User> GetUsers(string nameQuery, int pageNumber, int pageSize)
        {
            return _userRepository.GetUsers(nameQuery, pageNumber, pageSize);
        }

        public int GetTotalUsers(string nameQuery)
        {
            return _userRepository.GetTotalUsers(nameQuery);
        }

        public void AddUser(User user)
        {
            _userRepository.AddUser(user);
        }

        public void UpdateUser(int userId, User user)
        {
            _userRepository.UpdateUser(userId, user);
        }

        public void UpdateUserPassword(int userId, string password)
        {
            _userRepository.UpdateUserPassword(userId, password);
        }

        public User GetUserByNameAndPass(string username, string password)
        {
            return _userRepository.GetUserByNameAndPass(username, password);
        }

        public User GetUserById(int userId)
        {
            return _userRepository.GetUserById(userId);
        }

        public User GetActiveUserByEmail(string email)
        {
            return _userRepository.GetActiveUserByEmail(email);
        }

        public User GetUserByUsername(string username)
        {
            return _userRepository.GetUserByUsername(username);
        }

        public User GetUserByEmail(string email)
        {
            return _userRepository.GetUserByEmail(email);
        }

        public async Task UpdateUserPasswordResetTokenAsync(string email)
        {
            await _userRepository.UpdateUserPasswordResetTokenAsync(email);
        }

        public async Task<bool> CheckPasswordResetTokenAsync(string email, string resetToken)
        {
            return await _userRepository.CheckPasswordResetTokenAsync(email,resetToken);
        }
        public async Task UpdateRememberMeTokenAsync(string username)
        {
            await _userRepository.UpdateRemembermeTokenAsync(username);
        }
        public async Task RemoveRememberMeTokenAsync(string username)
        {
            await _userRepository.RemoveRemembermeTokenAsync(username);
        }

        public async Task<User> CheckRememberMeTokenAsync()
        {
            return await _userRepository.CheckRemembermeTokenAsync();
        }
    }
}
