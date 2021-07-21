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

    public record Transaction
    {
        public int TransactionID { get; set; }

        [Required]
        [Display(Name = "Type")]
        public TransactionType TransactionType { get; set; }

        [Required]
        [ForeignKey("Account")]
        [Display(Name = "Account Number")]
        public int AccountNumber { get; set; }
        public virtual Account Account { get; set; }

        [ForeignKey("DestinationAccount")]
        [Display(Name = "Destination Account")]
        public int? DestinationAccountNumber { get; set; }
        public virtual Account DestinationAccount { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        [Column(TypeName = "money")]
        [RegularExpression(@"^[0-9]*(\.[0-9]{1,2})?$")]
        public decimal Amount { get; set; }

        [StringLength(255)]
        public string Comment { get; set; }

        [Required]
        [Display(Name = "Date")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy hh:mm tt}", ApplyFormatInEditMode = true)]
        public DateTime TransactionTimeUtc { get; set; }
    }
}
