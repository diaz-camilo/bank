using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using utils;
using utils.Enums;

namespace WebBanking.Models
{
    public class SignupUser
    {
        public string LoginID { get; set; }
        public int? CustomerID { get; set; }
        public int AccountNum { get; set; }

        [Required(ErrorMessage ="Please enter your full name")]
        public string Name { get; set; }

        [Required(ErrorMessage ="Please enter a strong password")]
        [Compare(nameof(ConfirmPassword), ErrorMessage = "password do not match")]
        [DataType(DataType.Password)]
        public string password { get; set; }

        [Required(ErrorMessage = "Please confirm your password")]
        [Display(Name ="Confirm Password")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }

        [Required]
        public AccountType AccountType { get; set; }

        //[Display(Name ="Select an Account type to open")]
        public SelectList AccountTypes { get; } = new SelectList(Enum.GetValues<AccountType>());

        [Required(ErrorMessage ="Initial deposit is mandatory")]
        [Display(Name = "Inital Deposit")]
        [Range(100,10000,ErrorMessage = "Minimum Initial Deposit is $100 for Savings Account and $500 for Checking Account And maximum $10,000")]
        [DataType(DataType.Currency)]
        [RegularExpression(RegexPatterns.PositiveNumberTwoDecimal, ErrorMessage ="No more than two decimals allowed")]
        public decimal InicialDeposit { get; set; }

    }
}
