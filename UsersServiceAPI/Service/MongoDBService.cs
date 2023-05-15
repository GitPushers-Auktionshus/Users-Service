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

namespace UsersServiceAPI.Service;

public class MongoDBService
{
    private readonly IMongoCollection<User> _userCollection;
    private readonly IConfiguration _config;
    private readonly ILogger<MongoDBService> _logger;
    private readonly string _secret;
    private readonly string _issuer;

    public MongoDBService(IConfiguration config, ILogger<MongoDBService> logger)
    {
        _logger = logger;
        _config = config;

        _secret = config["Secret"] ?? "Secret missing";
        _issuer = config["Issuer"] ?? "Issue'er missing";

        // Client
        var mongoClient = new MongoClient(_config["ConnectionURI"]);
        _logger.LogInformation($"[*] CONNECTION_URI: {_config["ConnectionURI"]}");

        // Database
        var database = mongoClient.GetDatabase(_config["DatabaseName"]);
        _logger.LogInformation($"[*] DATABASE: {_config["DatabaseName"]}");

        // Collection
        _userCollection = database.GetCollection<User>(_config["CollectionName"]);
        _logger.LogInformation($"[*] COLLECTION: {_config["CollectionName"]}");
    }

    // Method to fetch a specific user from the userId.
    public async Task<User> GetUserById(string userId)
    {
        try
        {
            // Finds a user in the database with the same UserId as the input parameter.
            var user = await _userCollection.Find(u => u.UserId == userId).FirstOrDefaultAsync();

            _logger.LogInformation($"[*] Fetching user information from userId: {user.UserId}");

            return user;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Exception caught: {ex}");
            throw;
        }
    }

    // Method to add a new user to the database.
    public async Task AddNewUser(UserDTO newUser)
    {
        try
        {
            // Converts the UserDTO to a regular User object.
            User user = new User
            {
                UserId = ObjectId.GenerateNewId().ToString(),
                FirstName = newUser.FirstName,
                LastName = newUser.LastName,
                Address = newUser.Address,
                Phone = newUser.Phone,
                Email = newUser.Email,
                Password = HashPassword(newUser.Password),
                Verified = newUser.Verified,
                Rating = Math.Round(newUser.Rating, 2), // Converts float to use only 1 decimal.
                Username = newUser.Username
            };

            // Logging userinformation.
            _logger.LogInformation($"[*] New user added:\nUserId: {user.UserId}\nFull name: {user.FirstName} {user.LastName}\nPhone: {user.Phone}\nUsername: {user.Username}\nAddress: {user.Address}\nEmail: {user.Email}\nPassword: {user.Password}\nVerified: {user.Verified}\nRating: {user.Rating}");

            await _userCollection.InsertOneAsync(user);

            return;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Exception caught: {ex}");
            throw;
        }
    }

    // Method for password hashing.
    // Using BCrypt-package to salt and hash a password string.
    public static string HashPassword(string password)
    {
        string salt = BCrypt.Net.BCrypt.GenerateSalt();
        string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password, salt);

        return hashedPassword;
    }
}