using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalesSystem.ViewModels
{
    public class SaleDetailView
    {
        public int PartId { get; set; }
        public string PartName { get; set; }
        public int Qty { get; set; }
        public decimal PartPrice { get; set; }
        public int QoH { get; set; }
        public decimal total { get; set; }
    }
}
