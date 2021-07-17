using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace WebBanking.Models
{
    public class Customer
    {

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [RegularExpression(@"^\d\d\d\d$")]
        public int CustomerID { get; set; }

        [Required]
        public string Name { get; set; }

        public string TFN { get; set; }

        public string Address { get; set; }

        public string Suburb { get; set; }

        public string State { get; set; }

        public string Postcode { get; set; }

        public string Mobile { get; set; }

        public List<Account> Accounts { get; set; }

    }
}
