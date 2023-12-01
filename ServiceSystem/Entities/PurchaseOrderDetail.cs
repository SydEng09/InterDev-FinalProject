﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ServiceSystem.Entities
{
    public partial class PurchaseOrderDetail
    {
        public PurchaseOrderDetail()
        {
            ReceiveOrderDetails = new HashSet<ReceiveOrderDetail>();
            ReturnedOrderDetails = new HashSet<ReturnedOrderDetail>();
        }

        [Key]
        public int PurchaseOrderDetailID { get; set; }
        public int PurchaseOrderID { get; set; }
        public int PartID { get; set; }
        public int Quantity { get; set; }
        [Column(TypeName = "money")]
        public decimal PurchasePrice { get; set; }
        [StringLength(50)]
        public string VendorPartNumber { get; set; }

        [ForeignKey("PartID")]
        [InverseProperty("PurchaseOrderDetails")]
        public virtual Part Part { get; set; }
        [ForeignKey("PurchaseOrderID")]
        [InverseProperty("PurchaseOrderDetails")]
        public virtual PurchaseOrder PurchaseOrder { get; set; }
        [InverseProperty("PurchaseOrderDetail")]
        public virtual ICollection<ReceiveOrderDetail> ReceiveOrderDetails { get; set; }
        [InverseProperty("PurchaseOrderDetail")]
        public virtual ICollection<ReturnedOrderDetail> ReturnedOrderDetails { get; set; }
    }
}