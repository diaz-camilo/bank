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
        [RegularExpression(@"^\d{4}$")]
        public int CustomerID { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [StringLength(11)]
        [RegularExpression(@"^[0-9]{9}$", ErrorMessage = "TFN must be 9 digit number, no spaces")]
        public string TFN { get; set; }

        [StringLength(50)]
        public string Address { get; set; }

        [StringLength(40)]
        public string Suburb { get; set; }

        [StringLength(3)]
        [RegularExpression(
            @"^(?i)(vic|nsw|qld|nt|sa|tas|wa|act)$",
            ErrorMessage = "Must be a 2 or 3 lettered Australian sate. eg: VIC")]
        public string State { get; set; }

        [StringLength(4)]
        [RegularExpression(@"^\d{4}$", ErrorMessage = "Postcode must be a 4 digit number, no spaces")]
        public string Postcode { get; set; }

        [StringLength(12)]
        [RegularExpression(@"^04\d{2}(?:\s\d{3}){2}$", ErrorMessage ="Australian mobile number in the format 0444 111 222")]
        public string Mobile { get; set; }

        public virtual List<Account> Accounts { get; set; }

        //public virtual Login Login { get; set; }

        
        public virtual AppUser ID { get; set; }
    }
}
