﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ServiceSystem.Entities
{
    public partial class Employee
    {
        public Employee()
        {
            JobDetails = new HashSet<JobDetail>();
            Jobs = new HashSet<Job>();
            PurchaseOrders = new HashSet<PurchaseOrder>();
            SaleRefunds = new HashSet<SaleRefund>();
            Sales = new HashSet<Sale>();
        }

        [Key]
        public int EmployeeID { get; set; }
        [Required]
        [StringLength(9)]
        [Unicode(false)]
        public string SocialInsuranceNumber { get; set; }
        [Required]
        [StringLength(30)]
        [Unicode(false)]
        public string LastName { get; set; }
        [Required]
        [StringLength(30)]
        [Unicode(false)]
        public string FirstName { get; set; }
        [StringLength(40)]
        [Unicode(false)]
        public string Address { get; set; }
        [StringLength(20)]
        [Unicode(false)]
        public string City { get; set; }
        [StringLength(2)]
        [Unicode(false)]
        public string Province { get; set; }
        [StringLength(6)]
        [Unicode(false)]
        public string PostalCode { get; set; }
        [Required]
        [StringLength(12)]
        [Unicode(false)]
        public string ContactPhone { get; set; }
        public bool Textable { get; set; }
        [Required]
        [StringLength(30)]
        [Unicode(false)]
        public string EmailAddress { get; set; }
        public int PositionID { get; set; }

        [ForeignKey("PositionID")]
        [InverseProperty("Employees")]
        public virtual Position Position { get; set; }
        [InverseProperty("Employee")]
        public virtual ICollection<JobDetail> JobDetails { get; set; }
        [InverseProperty("Employee")]
        public virtual ICollection<Job> Jobs { get; set; }
        [InverseProperty("Employee")]
        public virtual ICollection<PurchaseOrder> PurchaseOrders { get; set; }
        [InverseProperty("Employee")]
        public virtual ICollection<SaleRefund> SaleRefunds { get; set; }
        [InverseProperty("Employee")]
        public virtual ICollection<Sale> Sales { get; set; }
    }
}