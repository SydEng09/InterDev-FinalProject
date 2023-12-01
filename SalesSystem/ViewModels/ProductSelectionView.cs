using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalesSystem.ViewModels
{
    public class ProductSelectionView
    {
        public int PartId { get; set; }
        public string ProductName { get; set; }
        public int QoH { get; set; }
        public decimal Price { get; set; }
        
    }
}
