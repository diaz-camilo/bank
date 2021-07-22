using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using utils;
using WebBanking.Models;

namespace WebBanking.ViewModels
{
    

    public class BillPayViewModel
    {


        public int BillPayID { get; set; }

        [Required]
        public int AccountNumberSelected { get; set; }
        public SelectList Accounts { get; set; }

        [Required]
        public int PayeeIDSelected { get; set; }

        public SelectList Payees { get; set; }


        [DataType(DataType.Currency)]
        [Required]
        [Column(TypeName = "money")]
        [RegularExpression(RegexPatterns.PositiveNumberTwoDecimal)]
        public decimal Amount { get; set; }

        [Required]
        public DateTime ScheduleTimeUtc { get; set; }

        public Period PeriodSelected { get; set; }

        [Required]
        public SelectList Periods { get; } = new SelectList(Enum.GetValues<Period>());
    }
}
