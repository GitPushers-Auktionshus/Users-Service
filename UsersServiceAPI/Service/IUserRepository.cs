using System;
using UsersServiceAPI.Model;
using Microsoft.AspNetCore.Mvc;

namespace UsersServiceAPI.Service
{
	public interface IUserRepository
	{
		public Task<User> GetUserById(string userId);

        public Task<List<User>> GetAllUsers();

        public Task DeleteOneUser(string userId);

        public Task UpdateOneUser(string userId, UserDTO userDTO);

        public Task UpdateUserPassword(string userId, UserDTO userDTO);

        public Task AddNewUser(UserDTO newUser);
        
    }

}
