using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace CS392_Demo3.Models
{
    [BsonIgnoreExtraElements]
    public class Product
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string name { get; set; } = string.Empty;
        public double price { get; set; }
        public string category { get; set; } = string.Empty;
        public bool inStock { get; set; }

        public List<string> tags { get; set; } = new();
        [BsonIgnore]
        public string tagsString { get; set; } = string.Empty;

        // These were causing ModelState to fail — now fixed
        public List<string> sizes { get; set; } = new();
        public string brand { get; set; } = string.Empty;
        public Specifications specifications { get; set; } = new();
        public List<Review> reviews { get; set; } = new();
    }

    public class Specifications
    {
        public string color { get; set; } = string.Empty;
        public string brand { get; set; } = string.Empty;
        public string players { get; set; } = string.Empty;
        public string charge { get; set; } = string.Empty;
        public string warranty { get; set; } = string.Empty;
    }

    public class Review
    {
        public string user { get; set; } = string.Empty;
        public int rating { get; set; }
        public string comment { get; set; } = string.Empty;
    }
}
