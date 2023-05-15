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
using UsersServiceAPI.Service;

namespace UsersServiceAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class UsersController : ControllerBase
{
    private readonly ILogger<UsersController> _logger;
    private readonly IConfiguration _config;
    private readonly string _secret;
    private readonly string _issuer;
    private readonly MongoDBService _mongoService;

    public UsersController(ILogger<UsersController> logger, IConfiguration config, MongoDBService mongoService)
    {
        // Injects MongoService into the controller constructor
        _mongoService = mongoService;

        _logger = logger;
        _config = config;

        _secret = config["Secret"] ?? "Secret missing";
        _issuer = config["Issuer"] ?? "Issue'er missing";
    }

    // GET - Fetches a user from the database by Id.
    [HttpGet("getUser/{userId}")]
    public async Task<User> GetUser(string userId)
    {
        return await _mongoService.GetUserById(userId);

    }

    // POST - Adds a user to the database.
    [HttpPost("addUser")]
    public async Task<IActionResult> AddUser(UserDTO newUser)
    {
        await _mongoService.AddNewUser(newUser);

        return Ok("New user has been added to the database.");

    }



}