using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace UsersServiceAPI.Model;

public class Login
{
    public string? Username { get; set; }
    public string? Password { get; set; }

    public Login()
    {
    }
}


