using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eAuction.Models.APIModels
{
    public class BidDataParameters
    {
        #region Pagination
        const int maxPageSize = 50;
        public int PageNumber { get; set; } = 1;
        private int _pageSize = 10;
        public int PageSize
        {
            get
            {
                return _pageSize;
            }
            set
            {
                _pageSize = (value > maxPageSize) ? maxPageSize : value;
            }
        }
        #endregion

        #region Filtering
        public string FilterBy { get; set; } = "";
        #endregion

        #region Sorting
        public string sortBy { get; set; }
        #endregion

        public string productId { get; set; }
    }
}
