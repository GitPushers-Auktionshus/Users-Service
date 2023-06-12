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
using UsersServiceAPI.Model;

namespace UsersServiceAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class UsersController : ControllerBase
{
    private readonly ILogger<UsersController> _logger;
    private readonly IConfiguration _config;
    private readonly IUserRepository _mongoService;

    public UsersController(ILogger<UsersController> logger, IConfiguration config, IUserRepository mongoService)
    {
        // Injects MongoService into the controller constructor
        _mongoService = mongoService;

        _logger = logger;
        _config = config;
    }

    // GET - Fetches a user from the database by Id.
    [Authorize]
    [HttpGet("getUser/{userId}")]
    public async Task<User> GetUser(string userId)
    {
        _logger.LogInformation($"[GET] getUser endpoint reached");

        return await _mongoService.GetUserById(userId);
    }

    // GET - Fetches all users from the database.
    [Authorize]
    [HttpGet("getAllUsers")]
    public async Task<List<User>> GetAll()
    {
        _logger.LogInformation($"[GET] getAllUsers endpoint reached");

        return await _mongoService.GetAllUsers();
    }

    // DEL - Deletes a user from the databse by Id.
    [Authorize]
    [HttpDelete("deleteUser/{userId}")]
    public async Task<IActionResult> DeleteUser(string userId)
    {
        _logger.LogInformation($"[DELETE] deleteUser/{userId} endpoint reached");

        await _mongoService.DeleteOneUser(userId);

        return Ok($"User with Id: {userId} has been deleted.");
    }

    // PUT - Updates a users information by Id.
    [Authorize]
    [HttpPut("updateUser/{userId}")]
    public async Task<IActionResult> UpdateUser(string userId, UserDTO userDTO)
    {
        _logger.LogInformation($"[PUT] updateUser/{userId} endpoint reached");

        await _mongoService.UpdateOneUser(userId, userDTO);

        return Ok($"User with Id: {userId} has been updated in the database.");
    }

    // PUT - Updates a users password by Id.
    [Authorize]
    [HttpPut("updatePassword/{userId}")]
    public async Task<IActionResult> UpdatePassword(string userId, UserDTO userDTO)
    {
        _logger.LogInformation($"[PUT] updatePassword/{userId} endpoint reached");

        await _mongoService.UpdateUserPassword(userId, userDTO);

        return Ok($"User with Id: {userId} has changed password");
    }

    // POST - Adds a user to the database.
    [HttpPost("addUser")]
    public async Task<IActionResult> AddUser(UserDTO newUser)
    {
        _logger.LogInformation($"[POST] addUser endpoint reached");

        try
        {
            User user = await _mongoService.AddNewUser(newUser);
            return CreatedAtAction("GetUser", new { userId = user.UserId }, user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while adding a new user.");
            return BadRequest();
        }

    }

    // POST - Adds an auctionhouse to the database.
    [HttpPost("addAuctionhouse")]
    public async Task<IActionResult> AddAuctionHouse(AuctionhouseDTO newAuctionhouse)
    {
        _logger.LogInformation($"[POST] addAuctionhouse endpoint reached");

        try
        {
            Auctionhouse auctionhouse = await _mongoService.AddNewAuctionhouse(newAuctionhouse);
            return new ObjectResult(auctionhouse)
            {
                StatusCode = 201
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while adding a new user.");
            return BadRequest();
        }

    }

    // GET - Fetches a user from the database by Id.
    [HttpGet("/")]
    public Task<StatusCodeResult> Test()
    {
        return StatusCode(201);
    }

}