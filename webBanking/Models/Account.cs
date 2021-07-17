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
        public int AccountNumber { get; set; }

        [Required]
        public AccountType Type { get; set; }

        [Required]
        public int CustomerID { get; set; }
        public Customer Customer { get; set; }

        [Required]
        [Column(TypeName = "money")]
        public decimal Balance { get; set; }

        [InverseProperty("Account")]
        public List<Transaction> Transactions { get; set; }

        public List<BillPay> BillPays { get; set; }

    }
}
