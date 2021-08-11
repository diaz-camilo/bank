using System;
using System.ComponentModel.DataAnnotations;

namespace WebBanking.Models
{
    public class SignupUser
    {
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

    }
}
