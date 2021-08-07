using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AdminPortal.ViewModels
{
    public class BillPayStateViewModel
    {
        [Display(Name = "BillPay State")]
        public bool BillPayState { get; set; }

        public List<SelectListItem> BillPayStates { get; } = new List<SelectListItem>()
            { new SelectListItem("Block Bill", "true"),
            new SelectListItem("Unblock Bill", "false") };

        [Range(1, 99999), Display(Name = "BillPay ID")]
        public int BillPayID { get; set; }

        public string Response { get; set; }
    }
}
