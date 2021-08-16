using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using utils.Enums;

namespace BankAPI.Models
{
    

    public class Account
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Display(Name = "Account Number")] // Used by HTML helpers
        [RegularExpression(@"^\d{4}$")] // wrong
        public int AccountNumber { get; set; }

        [Required]
        [Display(Name = "Type")]
        public AccountType Type { get; set; }

        [Required]
        public int CustomerID { get; set; }
        public virtual Customer Customer { get; set; }

        [Required]
        [Column(TypeName = "money")]
        [DataType(DataType.Currency)] // used by HTML helper, displays dollar sign and comas
        public decimal Balance { get; set; }

        public int FreeTransactions { get; set; }

        [InverseProperty("Account")]
        public virtual List<Transaction> Transactions { get; set; }

        public virtual List<BillPay> BillPays { get; set; }



    }
}
