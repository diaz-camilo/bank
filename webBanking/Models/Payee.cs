using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebBanking.Models
{
    public class Payee
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int PayeeID { get; set; }

        public string Name { get; set; }

        public string Address { get; set; }

        public string Suburb { get; set; }

        public string State { get; set; }

        public string Postcode { get; set; }

        public string Mobile { get; set; }

        public List<BillPay> BillPays { get; set; }

    }
}
