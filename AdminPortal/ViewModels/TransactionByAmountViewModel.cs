using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AdminPortal.Models;
using utils;

namespace AdminPortal.ViewModels
{
    public class TransactionByAmountViewModel
    {
        [RegularExpression(RegexPatterns.PositiveNumberTwoDecimal,
            ErrorMessage = "Min Amount must be a positive number with maximum two decimals")]
        [Required]
        [DataType(DataType.Currency)]
        [Display(Name ="Min Amount")]
        public decimal MinAmount { get; set; }

        [RegularExpression(RegexPatterns.PositiveNumberTwoDecimal,
            ErrorMessage = "Max Amount must be a positive number with maximum two decimals")]
        [Required]
        [DataType(DataType.Currency)]
        [Display(Name = "Max Amount")]        
        public decimal MaxAmount { get; set; }

        public List<Transaction> Transactions { get; set; }
    }
}
