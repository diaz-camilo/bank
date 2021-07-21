using System;
using System.ComponentModel.DataAnnotations;
using utils;

namespace WebBanking.ViewModels
{
    public class TransactionViewModel
    {
        [Required]
        [RegularExpression(@"\d{4}",ErrorMessage = "Account number must be a 4 digit number")]
        [Display(Name = "Origin Account")]
        public int AccountNumber { get; set; }


        [RegularExpression(@"\d{4}", ErrorMessage = "Account number must be a 4 digit number")]
        [Display(Name = "Destination Account")]
        public int? DestinationAccountNumber { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        [RegularExpression(RegexPatterns.PositiveNumberTwoDecimal,
            ErrorMessage ="only one or two decimal places allowed and no negative numbers")]
        public decimal Amount { get; set; }

        [StringLength(255)]
        public string Comment { get; set; }
    }
}

