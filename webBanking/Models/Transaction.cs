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

        public TransactionType TransactionType { get; set; }

        public int AccountNumber { get; set; }
        public virtual Account Account { get; set; }

        [ForeignKey("DestinationAccount")]
        public int? DestinationAccountNumber { get; set; }
        public virtual Account DestinationAccount { get; set; }

        [Column(TypeName = "money")]
        public decimal Amount { get; set; }

        [StringLength(255)]
        public string Comment { get; set; }

        public DateTime TransactionTimeUtc { get; set; }
    }
}
