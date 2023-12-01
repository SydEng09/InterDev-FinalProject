using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalesSystem.ViewModels
{
    public class RefundDiscountView
    {
        public int CouponId { get; set; }
        public int CouponDiscount { get; set; }
        public DateTime StartDate{get; set;}
        public DateTime EndDate { get; set; }
    }
}
