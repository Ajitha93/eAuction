using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using eAuction.DataAccess.Repositories.BuyerRepositories;
using eAuction.DataAccess.Repositories.ProductRepositories;
using eAuction.Models.APIModels;
using eAuction.Models.EntityModels;

namespace eAuction.Business.SellerBusiness
{
    public class SellerBusinessManager :ISellerBusinessManager
    {
        private readonly IProductRepository _productRepository;
        private readonly IBuyerRepository _buyerRepository;
        private readonly IMapper _mapper;

        public SellerBusinessManager(IProductRepository productRepository, IBuyerRepository buyerRepository, IMapper mapper)
        {
            _productRepository = productRepository;
            _buyerRepository = buyerRepository;
            _mapper = mapper;
        }


        public List<BidData> GetBidData(BidDataParameters bidDataParameters)
        {
            //querying DB data
            var product = _productRepository.GetProduct(bidDataParameters.productId);
            var buyerData = _buyerRepository.GetBuyer(bidDataParameters.productId);

            //framing API response and paginating the data
            var bidData= (from buyer in buyerData
                          orderby buyer.BidAmount descending
                          select new BidData()
                          {
                              ShortDescription = product.ShortDescription,
                              DetailedDescription = product.DetailedDescription,
                              Category = product.Category,
                              StartingPrice = product.StartingPrice,
                              BidEndDate = product.BidEndDate,
                              BidAmount = buyer.BidAmount,
                              BuyerFirstName = buyer.FirstName,
                              BuyerLastName = buyer.LastName,
                              City = buyer.City,
                              Email=buyer.Email,
                              Mobile=buyer.Phone,
                              ProductName=product.ProductName
                          })
                         .Skip((bidDataParameters.PageNumber - 1) * bidDataParameters.PageSize)
                         .Take(bidDataParameters.PageSize)
                         .ToList();

            //filtering
            var filterBy = bidDataParameters.FilterBy?.Trim()?.ToLowerInvariant();
            if (!string.IsNullOrEmpty(filterBy))
            {
                bidData = bidData
                       .Where(m => m.ShortDescription.ToLowerInvariant().Contains(filterBy)
                       || m.DetailedDescription.ToLowerInvariant().Contains(filterBy)
                       || m.Category.ToLowerInvariant().Contains(filterBy)
                       || m.StartingPrice.ToString().ToLowerInvariant().Contains(filterBy)
                       || m.BidEndDate.ToString().ToLowerInvariant().Contains(filterBy)
                       || m.BidAmount.ToString().ToLowerInvariant().Contains(filterBy)
                       || m.BuyerFirstName.ToLowerInvariant().Contains(filterBy)
                       || m.BuyerLastName.ToLowerInvariant().Contains(filterBy)
                       || m.City.ToLowerInvariant().Contains(filterBy))?.ToList();
            }

            //sorting
            if (!string.IsNullOrEmpty(bidDataParameters.sortBy))
            {
                var propertyInfo = typeof(BidData).GetProperty(bidDataParameters.sortBy);
                return bidData.OrderBy(x => propertyInfo.GetValue(x, null)).ToList();
            }

            //response
            return bidData;
        }

        public void CreateProduct(ProductData newProduct)
        {
            Products product = _mapper.Map<Products>(newProduct);
            _productRepository.Create(product);
        }

        public string DeleteProduct(string productId)
        {
            string msg = string.Empty;
            var product = _productRepository.GetProduct(productId);
            var buyer = _buyerRepository.GetBuyer(productId);
            if (product.BidEndDate < DateTime.Now)
                msg = "Can't delete product as the BidEndDate is a past date";
            else if (buyer?.Count > 0)
                msg = "Can't delete product as " + buyer?.Count + " BidData exists for it";
            else
            {
                _productRepository.Delete(productId);
                msg = "Deleted Successfully!!";
            }
            return msg;
        }

        public List<ProductData> GetProductList()
        {
            var products = _productRepository.GetProductList();
            var prodList=_mapper.Map<List<ProductData>>(products);
            return prodList;
        }
    }
}
