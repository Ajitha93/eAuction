using AutoMapper;
using eAuction.Models.APIModels;
using eAuction.Models.EntityModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eAuction.Business.Helpers
{
    public class MappingProfile:Profile
    {
        public MappingProfile()
        {
            this.CreateMap<ProductData,Products>();
            this.CreateMap<BidDetails, Buyers>();
            this.CreateMap<Products, ProductData>();
        }
    }
}
