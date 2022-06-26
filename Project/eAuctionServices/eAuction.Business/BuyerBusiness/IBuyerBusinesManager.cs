using eAuction.Models.APIModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eAuction.Business.BuyerBusiness
{
    public interface IBuyerBusinesManager
    {
        string CreateBid(BidDetails newBid);
        string UpdateBid(string email, string productId, decimal bidAmount);
        string CreateBidValidations(BidDetails newBid);
    }
}
