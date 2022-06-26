using eAuction.Models.ModelValidations;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eAuction.Models.APIModels
{
    public class ProductData
    {
        [BsonId]
        public string ProductId { get; set; }

        [Required]
        [StringLength(30, ErrorMessage = "Product Name is not null, min 5 and max 30 characters", MinimumLength = 5)]
        public string ProductName { get; set; } = null!;

        public string ShortDescription { get; set; } = null!;

        public string DetailedDescription { get; set; } = null!;

        [CategoryValidation]
        public string Category { get; set; } = null!;

        [DataType(DataType.Currency)]
        public decimal StartingPrice { get; set; }

        [DateLessThanOrEqualToToday]
        public DateTime BidEndDate { get; set; }

        [Required]
        [StringLength(30, ErrorMessage = "FirstName is not null, min 5 and max 30 characters", MinimumLength = 5)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(25, ErrorMessage = "LastName is not null, min 3 and max 25 characters", MinimumLength = 5)]
        public string LastName { get; set; }

        public string Address { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public string Pin { get; set; }

        [Required]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Not a valid phone number")]
        public string Phone { get; set; }

        [Required]
        [RegularExpression(@"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$", ErrorMessage = "Email is not valid.")]
        public string Email { get; set; }
    }
}
