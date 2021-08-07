using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AdminPortal.ViewModels
{
    public class CustomerAccessViewModel
    {
        [RegularExpression(@"^\d{4}$", ErrorMessage = "Enter a 4 digit Customer ID")]
        [Required]
        public int CustomerID { get; set; }


        [Required]
        public bool LoginState { get; set; }

        public List<SelectListItem> LoginStates { get; } = new List<SelectListItem>() { new SelectListItem("Block User", "true"), new SelectListItem("Unblock User", "false") };

        public string Response { get; set; }

    }
}
