﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace WebBanking.Models
{

    public class Customer
    {

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [RegularExpression(@"^\d{4}$")]
        public int CustomerID { get; set; }

        [Required]
        public string Name { get; set; }

        [RegularExpression(@"^[0-9]{9}$")]
        public string TFN { get; set; }

        public string Address { get; set; }

        public string Suburb { get; set; }

        [RegularExpression(
            @"^(?i)(vic|nsw|qld|nt|sa|tas|wa|act)$",
            ErrorMessage = "Must be a 2 or 3 lettered Australian sate. eg: VIC")]
        public string State { get; set; }

        [RegularExpression(@"^\d{4}$")]
        public string Postcode { get; set; }

        [RegularExpression(@"^04\d{2}(?:\s\d{3}){2}$")]
        public string Mobile { get; set; }

        public virtual List<Account> Accounts { get; set; }

        public virtual Login Login { get; set; }

    }
}
