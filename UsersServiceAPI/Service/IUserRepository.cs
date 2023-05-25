using System;
using UsersServiceAPI.Model;
using Microsoft.AspNetCore.Mvc;

namespace UsersServiceAPI.Service
{
	public interface IUserRepository
	{
        /// <summary>
        /// Gets a specific user from the database based on the provided user ID
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>The user matching the ID</returns>
		public Task<User> GetUserById(string userId);

        /// <summary>
        /// Gets a list containing all users
        /// </summary>
        /// <returns>A list containing all users</returns>
        public Task<List<User>> GetAllUsers();

        /// <summary>
        /// Deletes a specific user from the database based on the provided user ID
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public Task DeleteOneUser(string userId);

        /// <summary>
        /// Updates a user matching the provided ID. Requires a DTO with the updated properties
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="userDTO"></param>
        /// <returns></returns>
        public Task UpdateOneUser(string userId, UserDTO userDTO);

        /// <summary>
        /// Updates a user's password. Requires the ID of the user to be updated
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="userDTO"></param>
        /// <returns></returns>
        public Task UpdateUserPassword(string userId, UserDTO userDTO);

        /// <summary>
        /// Adds a new user to the database.
        /// </summary>
        /// <param name="newUser"></param>
        /// <returns></returns>
        public Task<User> AddNewUser(UserDTO newUser);

    }

}
