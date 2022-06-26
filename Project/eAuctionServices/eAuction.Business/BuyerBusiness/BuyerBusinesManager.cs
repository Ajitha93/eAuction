using AutoMapper;
using eAuction.DataAccess.Repositories.BuyerRepositories;
using eAuction.DataAccess.Repositories.ProductRepositories;
using eAuction.Models.APIModels;
using eAuction.Models.EntityModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eAuction.Business.BuyerBusiness
{
    public class BuyerBusinesManager : IBuyerBusinesManager
    {
        private readonly IBuyerRepository _buyerRepository;
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;

        public BuyerBusinesManager(IBuyerRepository buyerRepository, IProductRepository productRepository, IMapper mapper)
        {
            _buyerRepository = buyerRepository;
            _productRepository = productRepository;
            _mapper = mapper;
        }

        public string CreateBid(BidDetails newBid)
        {
            Buyers buyer = _mapper.Map<Buyers>(newBid);
            _buyerRepository.Create(buyer);
            return "Created BidData Successfully!!";
        }

        public string CreateBidValidations(BidDetails newBid)
        {
            var product = _productRepository.GetProduct(newBid.ProductId);
            var bidData = _buyerRepository.GetBuyer(newBid.ProductId, newBid.Email);
            if (product == null)
                return "Bid Can't be placed as Product doesnot exist";
            else if (product.BidEndDate <= DateTime.Now)
                return "Bid Can't be placed as BidEndDate is past EndDate";
            else if (bidData?.Count > 0)
                return "Already Bid exists for the product";
            else
                return "valid";
        }


        public string UpdateBid(string email, string productId, decimal bidAmount)
        {
            var product = _productRepository.GetProduct(productId);
            var bidData = _buyerRepository.GetBuyer(productId, email)?.FirstOrDefault();
            if (product.BidEndDate < DateTime.Now)
                return "Bid Can't be updated as BidEndDate is past EndDate";
            else if (bidData == null)
                return "No Bid Data found";
            else
            {
                bidData.BidAmount = bidAmount;
                _buyerRepository.Update(bidData.BuyerId, bidData);
                return "Bid Data updated Successfully!!";
            }
        }
    }
}
