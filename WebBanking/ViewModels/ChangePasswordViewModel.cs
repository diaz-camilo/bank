using System;
using System.ComponentModel.DataAnnotations;

namespace WebBanking.ViewModels
{
    public class ChangePasswordViewModel
    {
        [Required, DataType(DataType.Password), Display(Name ="New Password")]
        public string NewPassword { get; set; }

        [Required, DataType(DataType.Password), Display(Name = "CurrentPassword")]
        public string CurrentPassword { get; set; }

        [Required, DataType(DataType.Password), Display(Name = "Confirm New Password")]
        [Compare(nameof(NewPassword), ErrorMessage = "Password does not match")]
        public string ConfirmNewPassword { get; set; }
    }
}
