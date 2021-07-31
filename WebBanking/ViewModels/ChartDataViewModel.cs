using System;
using System.Collections.Generic;
using WebBanking.Models;

namespace WebBanking.ViewModels
{
    public class ChartDataViewModel
    {
        public List<(string date, decimal amount, TransactionType type)> Transactions { get; set; }
        public List<(string type, int count)> TransactionsCount { get; set; }

    }
}
