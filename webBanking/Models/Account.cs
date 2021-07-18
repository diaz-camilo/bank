using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace WebBanking.Models
{
    public enum AccountType
    {
        Savings = 0,
        Checking = 1
    }

    public class Account
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Display(Name = "Account Number")]
        [RegularExpression(@"^\d{4}$")]
        public int AccountNumber { get; set; }

        [Required]
        [Display(Name = "Type")]
        public AccountType Type { get; set; }

        [Required]
        public int CustomerID { get; set; }
        public virtual Customer Customer { get; set; }

        [Required]
        [Column(TypeName = "money")]
        [DataType(DataType.Currency)]
        public decimal Balance { get; set; }

        [InverseProperty("Account")]
        public virtual List<Transaction> Transactions { get; set; }

        public virtual List<BillPay> BillPays { get; set; }

    }
}
