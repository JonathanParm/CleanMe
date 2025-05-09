﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanMe.Application.ViewModels
{
    public class AddressViewModel
    {
        [DisplayName("Line 1")]
        [Required]
        [Column(TypeName = "varchar(100)")]
        [StringLength(20, ErrorMessage = "An address line cannot exceed 100 characters.")]
        [MinLength(2, ErrorMessage = "An address line must be at least 2 characters.")]
        public string Line1 { get; set; } = string.Empty;

        [DisplayName("Line 2")]
        [Column(TypeName = "varchar(100)")]
        [StringLength(20, ErrorMessage = "An address line cannot exceed 100 characters.")]
        [MinLength(2, ErrorMessage = "An address line must be at least 2 characters.")]
        public string? Line2 { get; set; }

        [DisplayName("Suburb")]
        [Column(TypeName = "varchar(50)")]
        [StringLength(20, ErrorMessage = "A suburb name cannot exceed 50 characters.")]
        [MinLength(2, ErrorMessage = "A suburb name must be at least 2 characters.")]
        public string? Suburb { get; set; }

        [DisplayName("Town/City")]
        [Required]
        [Column(TypeName = "varchar(100)")]
        [StringLength(20, ErrorMessage = "A town/city name cannot exceed 100 characters.")]
        [MinLength(2, ErrorMessage = "A town/city name must be at least 2 characters.")]
        public string TownOrCity { get; set; } = string.Empty;

        [DisplayName("Postcode")]
        [Required]
        [Column(TypeName = "varchar(10)")]
        [StringLength(20, ErrorMessage = "A postcode cannot exceed 10 characters.")]
        [MinLength(4, ErrorMessage = "A postcode must be at least 4 characters.")]
        public string Postcode { get; set; } = string.Empty;
    }
}
