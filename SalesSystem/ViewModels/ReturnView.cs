using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalesSystem.ViewModels
{
    public class ReturnView
    {
        public int EmployeeId { get; set; }
        public decimal subTotal { get; set; }
        public decimal Tax { get; set; }
        public List<ReturnDetailsView> ReturnDetails { get; set; } = new();
    }
}
