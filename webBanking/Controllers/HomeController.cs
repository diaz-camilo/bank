using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Schema;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using utils.Enums;
using WebBanking.Data;
using WebBanking.Models;
using WebBanking.Repository;
using WebBanking.ViewModels;

namespace WebBanking.Controllers
{


    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly WebBankContext _context;
        private readonly IUserRepository _userRepository;

        public HomeController(ILogger<HomeController> logger, WebBankContext context, IUserRepository userRepository)
        {
            _logger = logger;
            _context = context;
            _userRepository = userRepository;
        }

        public IActionResult Index(int id = 0)
        {

            var customerID = HttpContext.Session.GetInt32(nameof(Customer.CustomerID));
            if (customerID == null)
                return View();

            var customer = _context.Customer.Find(customerID);
            var accNumb = customer.Accounts[id].AccountNumber;

            var transactions = _context.Transaction.
                Where(trans => trans.AccountNumber == accNumb).
                Where(trans => trans.TransactionTimeUtc > DateTime.UtcNow.AddDays(-14)).
                Where(trans => trans.TransactionTimeUtc <= DateTime.UtcNow).
                AsEnumerable().
                Select(x =>
               (
                    Date: x.TransactionTimeUtc.ToLocalTime().ToString("yyyy-MM-dd"),
                    Amount: x.Amount,
                    type: x.TransactionType
                )).
                GroupBy(x => x.Date);



            List<(string date, decimal amount, TransactionType type)> groupedTransactions = new();

            foreach (var group in transactions)
            {
                var subgroup = group.
                    AsEnumerable().
                    GroupBy(
                        x => x.type,
                        y => y.Amount,
                        (x, y) => (date: group.Key, amount: y.Sum(), type: x)).
                    ToList();

                groupedTransactions.AddRange(subgroup);
            }

            var transactionsList = _context.Transaction.
                Where(trans => trans.TransactionTimeUtc > DateTime.UtcNow.AddDays(-14)).
                Where(trans => trans.TransactionTimeUtc <= DateTime.UtcNow).
                Where(x => x.AccountNumber == accNumb).
                AsEnumerable().
                GroupBy(x => x.TransactionType,
                        x => x.TransactionType,
                        (x, y) => (type: x.ToString(), count: y.Count())).
                ToList();




            _context.SaveChanges();
            return View(new ChartDataViewModel { Transactions = groupedTransactions, TransactionsCount = transactionsList });
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


    }
}
