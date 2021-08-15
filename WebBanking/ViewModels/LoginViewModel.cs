using System;
using System.ComponentModel.DataAnnotations;

namespace WebBanking.ViewModels
{
    public class LoginViewModel
    {
        [Required]
        public string LoginID { get; set; }

        [DataType(DataType.Password)]
        [Required]
        public string Password { get; set; }


    }
}