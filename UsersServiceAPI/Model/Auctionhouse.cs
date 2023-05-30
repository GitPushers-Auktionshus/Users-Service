using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.Serialization;
using System;

namespace UsersServiceAPI.Model;

public class Auctionhouse
{
    [BsonId]
    public string AuctionhouseID { get; set; }
    public string? Name { get; set; }
    public string? Address { get; set; }
    public string? CvrNumber { get; set; }

    public Auctionhouse()
    {
    }

}


