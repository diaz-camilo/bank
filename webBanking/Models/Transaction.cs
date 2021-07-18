using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebBanking.Models
{
    public enum TransactionType
    {
        Deposit = 1,
        Withdraw = 2,
        IncomingTransfer = 3,
        OutgoingTransfer = 4,
        ServiceCharge = 5,
        BillPay = 6
    }

    public class Transaction
    {
        public int TransactionID { get; set; }

        [Required]
        public TransactionType TransactionType { get; set; }

        [Required]
        public int AccountNumber { get; set; }
        public virtual Account Account { get; set; }

        [ForeignKey("DestinationAccount")]
        public int? DestinationAccountNumber { get; set; }
        public virtual Account DestinationAccount { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        [Column(TypeName = "money")]
        [RegularExpression(@"^[1-9][0-9]*(\.[0-9]{1,2})?$")]
        public decimal Amount { get; set; }

        [StringLength(255)]
        public string Comment { get; set; }

        [Required]
        public DateTime TransactionTimeUtc { get; set; }
    }
}
