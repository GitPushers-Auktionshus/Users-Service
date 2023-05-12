using Microsoft.AspNetCore.Mvc;

namespace UserServiceAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    private readonly ILogger<UserController> _logger;
    private readonly IConfiguration _config;
    private readonly IMongoCollection<User> _users;
    private readonly string _secret;
    private readonly string _issuer;

    public UserController(ILogger<UserController> logger, IConfiguration config)
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
        _users = database.GetCollection<User>(_config["CollectionName"]);
        _logger.LogInformation($"[*] COLLECTION: {_config["CollectionName"]}");
    }




}
