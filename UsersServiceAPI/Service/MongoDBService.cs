using System.Runtime.Serialization;
using System.Threading;
using System.Collections.Generic;
using System;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Bson;
using System;
using UsersServiceAPI.Model;

namespace UsersServiceAPI.Service;

// Inhertis from IUserRepository interface.
public class MongoDBService : IUserRepository
{
    private readonly IConfiguration _config;

    private readonly ILogger<MongoDBService> _logger;

    private readonly string _connectionURI;

    private readonly string _salt;

    private readonly string _usersDatabase;

    private readonly string _userCollectionName;

    private readonly IMongoCollection<User> _userCollection;

    public MongoDBService(IConfiguration config, ILogger<MongoDBService> logger, EnviromentVariables vaultSecrets)
    {
        _logger = logger;
        _config = config;

        try
        {
            // Retrieves enviroment variables from program.cs, from injected EnviromentVariables class
            _connectionURI = vaultSecrets.dictionary["ConnectionURI"];
            _salt = vaultSecrets.dictionary["Salt"];

            // Retrieves User database and collections
            _usersDatabase = config["UserDatabase"] ?? "Auctionsdatabase missing";
            _userCollectionName = config["UserCollection"] ?? "Auctioncollection name missing";

            _logger.LogInformation($"AuctionService secrets: ConnectionURI: {_connectionURI}, Salt: {_salt}");
            _logger.LogInformation($"User Database and Collections: Database: {_usersDatabase}, Collection: {_userCollectionName}");

        }
        catch (Exception ex)
        {
            _logger.LogError($"Error retrieving enviroment variables {ex.Message}");

            throw;
        }

        try
        {
            // Sets MongoDB client
            var mongoClient = new MongoClient(_connectionURI);
            _logger.LogInformation($"[*] CONNECTION_URI: {_connectionURI}");

            // Sets MongoDB Database
            var auctionsDatabase = mongoClient.GetDatabase(_usersDatabase);
            _logger.LogInformation($"[*] DATABASE: {_usersDatabase}");

            // Collections
            _userCollection = auctionsDatabase.GetCollection<User>(_userCollectionName);
            _logger.LogInformation($"[*] COLLECTION: {_userCollectionName}");

        }
        catch (Exception ex)
        {
            _logger.LogError($"Error trying to connect to database: {ex.Message}");
            throw;
        }

    }

    // Method to fetch a specific user from the userId.
    public async Task<User> GetUserById(string userId)
    {
        try
        {
            // Finds a user in the database with the same UserId as the input parameter.
            var user = await _userCollection.Find(u => u.UserId == userId).FirstOrDefaultAsync();

            _logger.LogInformation($"[*] GetUserById(string userId) called: Fetching user information from userId: {user.UserId}");

            if (user == null)
            {
                _logger.LogError($"Error finding user: {userId}");

                return null;
            }
            else
            {
                return user;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"EXCEPTION CAUGHT: {ex.Message}");
            throw;
        }
    }

    // Method to fetch all users from the database.
    public async Task<List<User>> GetAllUsers()
    {
        try
        {
            _logger.LogInformation($"[*] GetAllUsers() called: Fetching all users from the database.");

            List<User> userList = new List<User>();

            userList = await _userCollection.Find(new BsonDocument()).ToListAsync();

            if (userList == null)
            {
                _logger.LogInformation("No users found");

                return null;
            }
            else
            {
                return userList;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"EXCEPTION CAUGHT: {ex.Message}");
            throw;
        }
    }

    // Method to delete a user in the database
    public async Task DeleteOneUser(string userId)
    {
        try
        {
            _logger.LogInformation($"[*] DeleteOneUser(string userId) called: Deleting a user from the database.");

            // Finds a user with the input UserId and deletes it.
            await _userCollection.DeleteOneAsync(u => u.UserId == userId);

        }
        catch (Exception ex)
        {
            _logger.LogError($"EXCEPTION CAUGHT: {ex.Message}");
            throw;          
        }
    }

    // Method to update a user in the database.
    public async Task UpdateOneUser(string userId, UserDTO userDTO)
    {
        try
        {
            _logger.LogInformation($"[*] UpdateOneUser(string userId, UserDTO userDTO) called: Updating a user in the database.");

            // Finds the user with the same UserId as the input Id.
            User existingUser = await _userCollection.Find(u => u.UserId == userId).FirstOrDefaultAsync();

            if (existingUser == null)
            {
                _logger.LogError($"No user with the given userId found.");
                return;
            }

            _logger.LogInformation($"The user has been updated:\nFirstName: {existingUser.FirstName} --> {userDTO.FirstName}\nLastName: {existingUser.LastName} --> {userDTO.LastName}\nAddress: {existingUser.Address} --> {userDTO.Address}\nPhone: {existingUser.Phone} --> {userDTO.Phone}\nEmail: {existingUser.Email} --> {userDTO.Email}\nVerified: {existingUser.Verified}  --> {userDTO.Verified}\nRating: {existingUser.Rating} --> {userDTO.Rating}\nUsername: {existingUser.Username} --> {userDTO.Username}");

            // Overwrites the data in the object with the new userDTO object.
            existingUser.FirstName = userDTO.FirstName;
            existingUser.LastName = userDTO.LastName;
            existingUser.Address = userDTO.Address;
            existingUser.Phone = userDTO.Phone;
            existingUser.Email = userDTO.Email;
            existingUser.Verified = userDTO.Verified;
            existingUser.Rating = Math.Round(userDTO.Rating, 2);
            existingUser.Username = userDTO.Username;

            // Replaces the User object in the database with the new object.
            await _userCollection.ReplaceOneAsync(x => x.UserId == userId, existingUser);

            return;
        }
        catch (Exception ex)
        {
            _logger.LogError($"EXCEPTION CAUGHT: {ex.Message}");
            throw;   
        }
    }

    // Method to update a users password.
    public async Task UpdateUserPassword(string userId, UserDTO userDTO)
    {
        try
        {
            _logger.LogInformation($"[*] UpdateUserPassword(string userId, UserDTO userDTO) called: Updating a users password in the database.");

            // Finds the user with the same UserId as the input Id.
            User existingUser = await _userCollection.Find(u => u.UserId == userId).FirstOrDefaultAsync();

            if (existingUser == null)
            {
                _logger.LogError($"No user with the given userId found.");
                return;
            }

            _logger.LogInformation($"Old: {existingUser.Password}");

            // Overwrites the current password with the new one
            existingUser.Password = HashPassword(userDTO.Password, _salt);

            _logger.LogInformation($"New: {existingUser.Password}");

            // Updates the database with the new User object.
            await _userCollection.ReplaceOneAsync(x => x.UserId == userId, existingUser);

            return;

        }
        catch (Exception ex)
        {
            _logger.LogError($"EXCEPTION CAUGHT: {ex.Message}");
            throw;   
        }
    }

    // Method to add a new user to the database.
    public async Task AddNewUser(UserDTO newUser)
    {
        try
        {
            _logger.LogInformation($"[*] AddNewUser(UserDTO newUser) called: Creating a new user.");

            // Converts the UserDTO to a regular User object.
            User user = new User
            {
                UserId = ObjectId.GenerateNewId().ToString(),
                FirstName = newUser.FirstName,
                LastName = newUser.LastName,
                Address = newUser.Address,
                Phone = newUser.Phone,
                Email = newUser.Email,
                Password = HashPassword(newUser.Password, _salt),
                Verified = newUser.Verified,
                Rating = Math.Round(newUser.Rating, 2), // Converts float to use only 1 decimal.
                Username = newUser.Username
            };

            // Logging userinformation.
            _logger.LogInformation($"\n[*] New user added:\nUserId: {user.UserId}\nFull name: {user.FirstName} {user.LastName}\nPhone: {user.Phone}\nUsername: {user.Username}\nAddress: {user.Address}\nEmail: {user.Email}\nPassword: {user.Password}\nVerified: {user.Verified}\nRating: {user.Rating}");

            await _userCollection.InsertOneAsync(user);

            return;
        }
        catch (Exception ex)
        {
            _logger.LogError($"EXCEPTION CAUGHT: {ex.Message}");
            throw;
        }
    }

    // Method for password hashing.
    // Using BCrypt-package to salt and hash a password string.
    public static string HashPassword(string password, string salt)
    {
        string hashSalt = salt;
        string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password, salt);

        return hashedPassword;
    }

}