using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AdminPortal.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using utils;


namespace AdminPortal.ViewModels
{
    public class IndexViewModel
    {
        //public AdminIndexErrorEnum Error { get; set; }
        //public string ErrorMessage { get; set; }

        //[RegularExpression(@"^\d{4}-\d{2}-\d{2}",
        //    ErrorMessage = "enter date in the format yyyy-mm-dd ")]
        //[Display(Name = "Start Date")]
        //public string StartDate { get; set; }

        //[RegularExpression(@"^\d{4}-\d{2}-\d{2}",
        //    ErrorMessage = "enter date in the format yyyy-mm-dd ")]
        //[Display(Name = "End Date")]
        //public string EndDate { get; set; }

        //[RegularExpression(@"^\d{4}$",
        //    ErrorMessage = "Enter a 4 digit Account Number")]
        //[Required]
        //[Display(Name = "Account Number")]
        //public int AccountNum { get; set; }

        [RegularExpression(RegexPatterns.PositiveNumberTwoDecimal,
            ErrorMessage = "Min Amount must be a positive number with maximum two decimals")]
        [Required]
        public decimal MinAmount { get; set; }

        [RegularExpression(RegexPatterns.PositiveNumberTwoDecimal,
            ErrorMessage = "Max Amount must be a positive number with maximum two decimals")]
        [Required]
        public decimal MaxAmount { get; set; }


        [RegularExpression(@"^\d{4}$", ErrorMessage = "Enter a 4 digit Customer ID")]
        [Required]
        [Display(Name = "Customer ID")]
        public int CustomerID { get; set; }


        public string Name { get; set; }

        [RegularExpression(@"^[0-9]{9}$",
            ErrorMessage = "TFN must be 9 digit number, no spaces")]
        public string TFN { get; set; }

        public string Address { get; set; }

        public string Suburb { get; set; }

        [RegularExpression(@"^(?i)(vic|nsw|qld|nt|sa|tas|wa|act)$",
            ErrorMessage = "Must be a 2 or 3 lettered Australian sate. eg: VIC")]
        public string State { get; set; }

        [RegularExpression(@"^\d{4}$",
            ErrorMessage = "Postcode must be a 4 digit number, no spaces")]
        public string Postcode { get; set; }

        [RegularExpression(@"^04\d{2}(?:\s\d{3}){2}$",
            ErrorMessage = "Australian mobile number in the format 0444 111 222")]
        public string Mobile { get; set; }

        [Required]
        [Display(Name = "Login State")]
        public bool LoginState { get; set; }

        public List<SelectListItem> LoginStates { get; } = new List<SelectListItem>()
            { new SelectListItem("Lock User", "true"),
            new SelectListItem( "Unlock User", "false") };

        [Display(Name = "BillPay State")]
        public bool BillPayState { get; set; }

        public List<SelectListItem> BillPayStates { get; } = new List<SelectListItem>()
            { new SelectListItem("Block Bill", "true"),
            new SelectListItem("Unblock Bill", "false") };

        [Range(1, 99999), Display(Name = "BillPay ID")]
        public int BillPayID { get; set; }

        public Customer Customer { get; set; }

        public TransactionByAccountViewModel TransactionByAccountViewModel { get; set; }


    }
}
