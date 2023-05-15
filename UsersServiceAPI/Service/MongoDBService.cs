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

    public User getUserById(string userId)
    {
        try
        {
            var user = _userCollection.Find(u => u.UserId == userId).FirstOrDefault();

            _logger.LogInformation($"[*] Fetching user information from userId: {user.UserId}");

            return user;
        }
        catch (Exception ex)
        {
            _logger.LogError("[*] User with the specified ID not found.");
            throw;
        }
    }

}