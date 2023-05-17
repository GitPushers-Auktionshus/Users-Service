using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.Serialization;
using System;

namespace UsersServiceAPI.Model;

public class User
    {
        [BsonId]
        public string? UserId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Address { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public bool Verified { get; set; }
        public double Rating { get; set; }
        public string? Username { get; set; }

        public User()
        {
        }
    }


