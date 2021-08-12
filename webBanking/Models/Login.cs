using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebBanking.Models
{
    public class Login
    {
        [RegularExpression(@"^\d{8}$")]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]

        public string LoginID { get; set; }

        [Required]
        public int CustomerID { get; set; }
        public virtual Customer Customer { get; set; }

        [Required, StringLength(64)]
        public string PasswordHash { get; set; }

        public bool Access { get; set; }
    }
}
