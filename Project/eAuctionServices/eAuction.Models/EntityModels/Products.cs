using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eAuction.Models.EntityModels
{
    public class Products
    {
        [BsonId]
        public string ProductId { get; set; }

        public string ProductName { get; set; } = null!;

        public string ShortDescription { get; set; } = null!;

        public string DetailedDescription { get; set; } = null!;

        public string Category { get; set; } = null!;

        public decimal StartingPrice { get; set; }

        public DateTime BidEndDate { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Address { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public string Pin { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }
    }
}
