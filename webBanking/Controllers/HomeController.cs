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
using WebBanking.Data;
using WebBanking.Models;
using WebBanking.ViewModels;

namespace WebBanking.Controllers
{


    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly WebBankContext _context;

        public HomeController(ILogger<HomeController> logger, WebBankContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index(int id = 0)
        {

            //SeedData()
            

            var customerID = HttpContext.Session.GetInt32(nameof(Customer.CustomerID));
            if (customerID == null)
                return View();

            var customer = _context.Customer.Find(customerID);
            var accNumb = customer.Accounts[id].AccountNumber;

            var transactions = _context.Transaction.
                Where(trans => trans.AccountNumber == accNumb).
                Where(trans => trans.TransactionTimeUtc > DateTime.Now.AddDays(-14)).
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
                Where(x => x.TransactionTimeUtc > DateTime.Now.AddDays(-14)).
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

        private void SeedData()
        {
            for (int j = 0; j < 5; j++)
            {
                var accNum =
                    j == 0 ? 4100 :
                    j == 1 ? 4101 :
                    j == 2 ? 4200 :
                    j == 3 ? 4300 :
                    1111; // default

                var randomNum = new Random();
                int randInt = randomNum.Next();
                var startDate = DateTime.UtcNow.AddDays(-100);
                var account = _context.Account.Find(accNum);

                int i = 0;

                account.Transactions.Add(
                    new Transaction
                    {
                        TransactionTimeUtc = startDate,
                        Amount = 500,
                        TransactionType = TransactionType.Deposit,
                        Comment = $"Opening Balance"
                    });

                while (DateTime.UtcNow > startDate)
                {
                    startDate = startDate.AddHours(randomNum.Next(10, 24));
                    var amount = randomNum.Next(50, 500);
                    var typeDecider = randomNum.Next();
                    var type =
                        typeDecider % 5 == 0 ? TransactionType.Withdraw :
                        typeDecider % 11 == 0 ? TransactionType.OutgoingTransfer :
                        typeDecider % 17 == 0 ? TransactionType.BillPay :
                        TransactionType.Deposit;

                    if (type == TransactionType.Withdraw)
                    {
                        account.Transactions.Add(
                            new Transaction
                            {
                                TransactionTimeUtc = startDate,
                                Amount = amount,
                                TransactionType = type,
                            });
                        if (account.FreeTransactions < 1)
                        {
                            account.Transactions.Add(
                                new Transaction
                                {
                                    TransactionTimeUtc = startDate.AddMilliseconds(10),
                                    Amount = 0.10m,
                                    TransactionType = TransactionType.ServiceCharge,
                                    Comment = $"Withdraw Service Charge"
                                });
                        }
                        account.Balance -= 0.1m;
                        account.FreeTransactions--;
                    }
                    if (type == TransactionType.OutgoingTransfer)
                    {
                        if (account.FreeTransactions < 1)
                        {
                            account.Transactions.Add(
                                new Transaction
                                {
                                    TransactionTimeUtc = startDate.AddMilliseconds(10),
                                    Amount = 0.20m,
                                    TransactionType = TransactionType.ServiceCharge,
                                    Comment = $"Transfer Service Charge"
                                });
                            account.Balance -= 0.2m;
                        }
                        account.FreeTransactions--;

                        int accountDecider;
                        int destiAccount;
                        do
                        {
                            accountDecider = randomNum.Next(0, 999) % 5;
                            destiAccount =
                            accountDecider == 0 ? 4100 :
                            accountDecider == 1 ? 4101 :
                            accountDecider == 2 ? 4200 :
                            accountDecider == 3 ? 4300 :
                            1111; // default

                            if (account.AccountNumber != destiAccount)
                            {


                                var destinationAccount = _context.Account.Find(destiAccount);
                                destinationAccount.Transactions.Add(

                                    new Transaction
                                    {
                                        TransactionTimeUtc = startDate,
                                        Amount = amount,
                                        TransactionType = TransactionType.IncomingTransfer,

                                    });
                                destinationAccount.Balance += amount;

                                account.Transactions.Add(
                                    new Transaction
                                    {
                                        TransactionTimeUtc = startDate,
                                        Amount = amount,
                                        TransactionType = type,
                                        DestinationAccountNumber = destinationAccount.AccountNumber
                                    });
                            }


                        } while (account.AccountNumber == destiAccount);

                    }
                    if (type == TransactionType.BillPay)
                    {
                        account.Transactions.Add(
                                new Transaction
                                {
                                    TransactionTimeUtc = startDate,
                                    Amount = amount,
                                    TransactionType = TransactionType.BillPay,
                                });
                    }
                    if (type == TransactionType.Deposit)
                    {
                        account.Transactions.Add(
                            new Transaction
                            {
                                TransactionTimeUtc = startDate,
                                Amount = amount,
                                TransactionType = type,
                            });
                    }
                    account.Balance += type == TransactionType.Deposit ? amount : (-amount);
                    i++;
                    _context.SaveChanges();
                }
            }
        }
    }
}
