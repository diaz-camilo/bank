using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebBanking.Models
{
    public enum Period
    {
        Monthly = 0,
        Quarterly = 1,
        Annually = 3,
        Once = 4
    }

    public class BillPay
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int BillPayID { get; set; }


        public int AccountNumber { get; set; }
        public Account Account { get; set; }

        public int PayeeID { get; set; }
        public Payee Payee { get; set; }

        public decimal Amount { get; set; }

        public DateTime ScheduleTimeUtc { get; set; }

        public Period Period { get; set; }
    }
}
