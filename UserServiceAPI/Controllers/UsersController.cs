using System.Runtime.Serialization;
using System.Threading;
using System.Collections.Generic;
using System;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using MongoDB.Bson;
using System.Text;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Authorization;
using BCrypt.Net;

namespace UsersServiceAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class UsersController : ControllerBase
{
    private readonly ILogger<UsersController> _logger;
    private readonly IConfiguration _config;
    private readonly IMongoCollection<User> _user;
    private readonly string _secret;
    private readonly string _issuer;

    public UsersController(ILogger<UsersController> logger, IConfiguration config)
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
        _user = database.GetCollection<User>(_config["CollectionName"]);
        _logger.LogInformation($"[*] COLLECTION: {_config["CollectionName"]}");
    }

    // GET - Fetches a user from the database by UserId
    [HttpGet("getUser/{userId}")]
    public ActionResult<User> getUser(string userId)
    {
        try
        {
            // Finds a user in the database with the same UserId as the input parameter.
            var user = _user.Find(u => u.UserId == userId).FirstOrDefault();

            _logger.LogInformation($"[*] Fetching user information from userId: {user.UserId}");

            return user;
        }
        catch (Exception ex)
        {
            _logger.LogError("[*] User with the specified ID not found.");
            throw;
        }
    }


    // POST - Adds a user to the database.
    [HttpPost("addUser")]
    public async Task addUser(UserDTO newUser)
    {
        try
        {
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
                Rating = Math.Round(newUser.Rating, 2),
                Username = newUser.Username
            };

            // Logging userinformation.
            _logger.LogInformation($"[*] New user added:\nUserId: {user.UserId}\nFull name: {user.FirstName} {user.LastName}\nPhone: {user.Phone}\nUsername: {user.Username}\nAddress: {user.Address}\nEmail: {user.Email}\nPassword: {user.Password}\nVerified: {user.Verified}\nRating: {user.Rating}");

            // Inserts into user-collection.
            await _user.InsertOneAsync(user);

            return; 
        }
        catch (Exception ex)
        {
            _logger.LogError("[*] Couldn't add a new user to the database.");
            throw;
        }

    }

    // Method for password hashing.
    // Using BCrypt package to salt and hash a password string.
    public static string HashPassword(string password)
    {
        string salt = BCrypt.Net.BCrypt.GenerateSalt();
        string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password, salt);

        return hashedPassword;
    }

}