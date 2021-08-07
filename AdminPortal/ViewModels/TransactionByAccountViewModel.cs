using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AdminPortal.Models;

namespace AdminPortal.ViewModels
{
    public class TransactionByAccountViewModel
    {
        //[RegularExpression(@"^\d{4}-\d{2}-\d{2}", ErrorMessage = "enter date in the format yyyy-mm-dd ")]
        [Display(Name = "Start Date")]
        public string StartDate { get; set; }

        //[RegularExpression(@"^\d{4}-\d{2}-\d{2}", ErrorMessage = "enter date in the format yyyy-mm-dd ")]
        [Display(Name = "End Date")]
        public string EndDate { get; set; }

        [RegularExpression(@"^\d{4}$", ErrorMessage = "Enter a 4 digit Account Number")]
        [Required]
        [Display(Name = "Account Number")]
        public int AccountNum { get; set; }

        public List<Transaction> transactions { get; set; }
    }
}
