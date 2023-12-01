using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalesSystem.ViewModels
{
    public class ReturnDetailsView
    {
        public int PartId { get; set; }
        public string PartName { get; set; }
        public int OrgQty { get; set; }
        public decimal Price { get; set; }
        public int? RtnQty { get; set; }
        public string Ref { get; set; }
        public int Qty { get; set; }
        public string? Reason { get; set; }
    }
}
