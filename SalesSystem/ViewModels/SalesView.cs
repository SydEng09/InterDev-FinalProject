using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalesSystem.ViewModels
{
    public class SalesView
    {
        public int EmployeeId { get; set; }
        public decimal subTotal { get; set; }
        public decimal Tax { get; set; }
        public int? CouponId { get; set; }
        public string PaymentType { get; set; }
        public List<SaleDetailView> SaleDetails { get; set; } = new();
    }
}
