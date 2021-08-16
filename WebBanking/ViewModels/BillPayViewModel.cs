using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using utils;
using utils.Enums;
using WebBanking.Models;

namespace WebBanking.ViewModels
{
    

    public class BillPayViewModel
    {


        public int BillPayID { get; set; }

        [Required]
        [DisplayName("Account")]
        [RegularExpression(@"\d{4}", ErrorMessage = "Account number must be a 4 digit number")]
        public int AccountNumberSelected { get; set; }
        public List<SelectListItem> Accounts { get; set; }

        [Required]
        [DisplayName("Payee")]
        [RegularExpression(@"\d{3}", ErrorMessage = "Account number must be a 3 digit number")]
        public int PayeeIDSelected { get; set; }
        public List<SelectListItem> Payees { get; set; }


        [DataType(DataType.Currency)]
        [Required]
        [Column(TypeName = "money")]
        [RegularExpression(RegexPatterns.PositiveNumberTwoDecimal,
            ErrorMessage = "only positive numbers with up to two decimal places")]        
        public decimal Amount { get; set; }

        [Required]
        [DisplayName("Schedule Time")]
        public DateTime ScheduleTimeUtc { get; set; }

        [Required]
        [DisplayName("Frequency")]
        public Period PeriodSelected { get; set; }

        [Required]
        public SelectList Periods { get; } = new SelectList(Enum.GetValues<Period>());
    }
}
