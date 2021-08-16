using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;
using utils;
using utils.Enums;

namespace WebBanking.Models
{
    

    public class BillPay
    {

        [Required]
        public int BillPayID { get; set; }

        [Required]
        [RegularExpression(@"\d{4}", ErrorMessage = "Account number must be a 4 digit number")]
        [DisplayName("Account to Debit")]

      
        [ForeignKey("Account")]
        public int AccountNumber { get; set; }
        public virtual Account Account { get; set; }

        [Required]
        [RegularExpression(@"\d{3}", ErrorMessage = "Account number must be a 3 digit number")]

        public int PayeeID { get; set; }
        public virtual Payee Payee { get; set; }

        
        [DataType(DataType.Currency)]
        [Required]
        [Column(TypeName = "money")]
        [RegularExpression(RegexPatterns.PositiveNumberTwoDecimal)]
        public decimal Amount { get; set; }

        [Required]
        [DisplayName("Schedule Date and Time")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy hh:mm tt}", ApplyFormatInEditMode = true)]
        public DateTime ScheduleTimeUtc { get; set; }

        [Required]
        [DisplayName("Frequency")]
        public Period Period { get; set; }

        [Required]
        public State State { get; set; }
    }
}
