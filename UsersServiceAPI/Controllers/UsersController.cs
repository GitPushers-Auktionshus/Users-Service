using System.Net.Http.Headers;
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
    private readonly MongoDBService _mongoService;

    public UsersController(ILogger<UsersController> logger, IConfiguration config, MongoDBService mongoService)
    {
        // Injects MongoService into the controller constructor
        _mongoService = mongoService;

        _logger = logger;
        _config = config;
    }

    // GET - Fetches a user from the database by Id.
    [HttpGet("getUser/{userId}")]
    public async Task<User> GetUser(string userId)
    {
        return await _mongoService.GetUserById(userId);
    }

    // GET - Fetches all users from the database.
    [HttpGet("getAllUsers")]
    public async Task<List<User>> GetAll()
    {
        return await _mongoService.GetAllUsers();
    }

    // DEL - Deletes a user from the databse by Id.
    [HttpDelete("deleteUser/{userId}")]
    public async Task<IActionResult> DeleteUser(string userId)
    {
        await _mongoService.DeleteOneUser(userId);

        return Ok($"User with Id: {userId} has been deleted.");
    }

    // PUT - Updates a users information by Id.
    [HttpPut("updateUser/{userId}")]
    public async Task<IActionResult> UpdateUser(string userId, UserDTO userDTO)
    {
        await _mongoService.UpdateOneUser(userId, userDTO);

        return Ok($"User with Id: {userId} has been updated in the database.");
    }

    // POST - Adds a user to the database.
    [HttpPost("addUser")]
    public async Task<IActionResult> AddUser(UserDTO newUser)
    {
        await _mongoService.AddNewUser(newUser);

        return Ok($"New user has been added to the database: {newUser.FirstName} {newUser.LastName}");
    }


}