﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ServiceSystem.Entities
{
    public partial class CustomerVehicle
    {
        public CustomerVehicle()
        {
            Jobs = new HashSet<Job>();
        }

        [Key]
        [StringLength(13)]
        public string VehicleIdentification { get; set; }
        [Required]
        [StringLength(20)]
        public string Make { get; set; }
        [Required]
        [StringLength(30)]
        public string Model { get; set; }
        public int CustomerID { get; set; }

        [ForeignKey("CustomerID")]
        [InverseProperty("CustomerVehicles")]
        public virtual Customer Customer { get; set; }
        [InverseProperty("VehicleIdentificationNavigation")]
        public virtual ICollection<Job> Jobs { get; set; }
    }
}