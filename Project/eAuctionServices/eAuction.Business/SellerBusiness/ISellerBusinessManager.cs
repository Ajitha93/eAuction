using eAuction.Models.APIModels;
using eAuction.Models.EntityModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eAuction.Business.SellerBusiness
{
    public interface ISellerBusinessManager
    {
        List<BidData> GetBidData(BidDataParameters bidDataParameters);
        void CreateProduct(ProductData newProduct);
        string DeleteProduct(string productId);
        List<ProductData> GetProductList();
    }
}
