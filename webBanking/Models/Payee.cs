using System;
using utils;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebBanking.Models
{
    public class Payee
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int PayeeID { get; set; }

        [StringLength(50)]
        public string Name { get; set; }

        [StringLength(50)]
        public string Address { get; set; }

        [StringLength(40)]
        public string Suburb { get; set; }

        [RegularExpression(
            @"^(?i)(vic|nsw|qld|nt|sa|tas|wa|act)$",
            ErrorMessage = "Must be a 2 or 3 lettered Australian sate. eg: VIC")]
        public string State { get; set; }

        [RegularExpression(@"^\d{4}$")]
        public string Postcode { get; set; }

        [RegularExpression(RegexPatterns.PhoneNumberWithParentheses)]
        public string Phone { get; set; }

        public virtual List<BillPay> BillPays { get; set; }

    }
}
