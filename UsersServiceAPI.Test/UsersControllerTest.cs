using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using UsersServiceAPI.Controllers;
using NUnit.Framework;
using Moq;
using UsersServiceAPI.Model;
using Microsoft.AspNetCore.Mvc;
using UsersServiceAPI.Service;

namespace UsersServiceAPI.Test;

public class UsersControllerTest
{

    private ILogger<UsersController> _logger = null!;
    private IConfiguration _configuration = null!;


    [SetUp]
    public void Setup()
    {
        _logger = new Mock<ILogger<UsersController>>().Object;

        var myConfiguration = new Dictionary<string, string?>
        {
            {"UsersServiceBrokerHost", "http://testhost.local"}
        };

        _configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(myConfiguration)
            .Build();
    }

    [Test]
    public async Task TestAddUserEndpoint_valid_dto()
    {
        // Arrange
        var userDTO = CreateUser("TestUser");
        
        var stubRepo = new Mock<IUserRepository>();

        stubRepo.Setup(svc => svc.AddNewUser(userDTO))
            .Returns(Task.FromResult<UserDTO?>(userDTO));

        var controller = new UsersController(_logger, _configuration, stubRepo.Object);

        // Act        
        var result = await controller.AddUser(userDTO);

        // Assert
        Assert.That(result, Is.TypeOf<CreatedAtActionResult>());
        Assert.That((result as CreatedAtActionResult)?.Value, Is.TypeOf<UserDTO>());
    }

    [Test]
    public async Task TestAddUserEndpoint_failure_posting()
    {
        // Arrange
        var userDTO = CreateUser("TestUser");

        var stubRepo = new Mock<IUserRepository>();

        stubRepo.Setup(svc => svc.AddNewUser(userDTO))
            .ThrowsAsync(new Exception());

        var controller = new UsersController(_logger, _configuration, stubRepo.Object);

        // Act        
        var result = await controller.AddUser(userDTO);

        // Assert
        Assert.That(result, Is.TypeOf<BadRequestResult>());
    }

    /// <summary>
    /// Helper method for creating UserDTO instance.
    /// </summary>
    /// <param name="username"></param>
    /// <returns></returns>
    private UserDTO CreateUser(string username)
    {
        var userDTO = new UserDTO()
        {
            Username = username,
            FirstName = "Test FirstName",
            LastName = "Test LastName",
            Address = "Test Address",
            Phone = "Test Phone",
            Email = "Test Email",
            Password = "Test Password",
            Verified = true,
            Rating = 0
        };

        return userDTO;
    }
}