using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.Serialization;
using System;

namespace UsersServiceAPI.Model;

public class AuctionhouseDTO
{
    public string Name { get; set; }
    public string Address { get; set; }
    public string CvrNumber { get; set; }

    public AuctionhouseDTO()
    {
    }
}


