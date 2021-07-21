using System;
using System.ComponentModel.DataAnnotations;
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
        [RegularExpression(@"^[0-9]*(\.[0-9]{1,2})?$",
            ErrorMessage ="only one or two decimal places allowed and no negative numbers")]
        public decimal Amount { get; set; }

        [StringLength(255)]
        public string Comment { get; set; }
    }
}
