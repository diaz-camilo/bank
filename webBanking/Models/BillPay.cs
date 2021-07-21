using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;
using utils;

namespace WebBanking.Models
{
    public enum Period
    {
        Monthly = 0,
        Quarterly = 1,
        Annually = 3,
        Once = 4
    }

    public class BillPay
    {


        public int BillPayID { get; set; }

        [Required]
        public int AccountNumber { get; set; }
        public virtual Account Account { get; set; }

        [Required]
        public int PayeeID { get; set; }
        public virtual Payee Payee { get; set; }

        
        [DataType(DataType.Currency)]
        [Required]
        [Column(TypeName = "money")]
        [RegularExpression(RegexPatterns.PositiveNumberTwoDecimal)]
        public decimal Amount { get; set; }

        [Required]
        public DateTime ScheduleTimeUtc { get; set; }

        [Required]
        public Period Period { get; set; }
    }
}
