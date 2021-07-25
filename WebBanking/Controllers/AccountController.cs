using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebBanking.Data;
using WebBanking.Models;
using WebBanking.ViewModels;

namespace WebBanking.Controllers
{
    public class AccountController : Controller
    {
        private readonly WebBankContext _context;

        public AccountController(WebBankContext context) => _context = context;

        //GET: Customer/Deposit/5
        public async Task<IActionResult> Deposit(int? id)
        {
            var customerID = HttpContext.Session.GetInt32(nameof(Customer.CustomerID));
            var customer = await _context.Customer
                .FirstOrDefaultAsync(m => m.CustomerID == customerID);
            if (customer.Accounts.FirstOrDefault(x => x.AccountNumber == id) == null)
                return NotFound();


            var account = customer.Accounts.FirstOrDefault(x => x.AccountNumber == id);


            return View(new TransactionViewModel { AccountNumber = (int)id });
        }

        [HttpPost]
        //POST: Customer/Deposit/5
        public async Task<IActionResult> Deposit(TransactionViewModel transaction)
        {
            var customerID = HttpContext.Session.GetInt32(nameof(Customer.CustomerID));
            var customer = await _context.Customer.FindAsync(customerID);
            if (customer == null)
                return NotFound();

            var account = customer.Accounts.FirstOrDefault(x => x.AccountNumber == transaction.AccountNumber);
            if (account == null)
                return NotFound();

            if (!ModelState.IsValid)
                return View(transaction);

            HttpContext.Session.SetInt32("depositAccountNum", transaction.AccountNumber);
            HttpContext.Session.SetString("depositAmount", transaction.Amount.ToString());

            return RedirectToAction(nameof(DepositConfirm));
        }

        public IActionResult DepositConfirm()
        {
            int? AccountNum = HttpContext.Session.GetInt32("depositAccountNum");
            decimal Amount;
            if (! Decimal.TryParse(HttpContext.Session.GetString("depositAmount"), out Amount) || AccountNum == null) 
                return NotFound();

            return View(new TransactionViewModel { AccountNumber = (int)AccountNum, Amount = Amount });
        }

        [HttpPost]
        //POST: Customer/Deposit/5
        public async Task<IActionResult> DepositConfirm(int id = 1)
        {
            int? AccountNum = HttpContext.Session.GetInt32("depositAccountNum");
            var customerID = HttpContext.Session.GetInt32(nameof(Customer.CustomerID));
            var customer = await _context.Customer.FindAsync(customerID);
            decimal Amount;
            if (!Decimal.TryParse(HttpContext.Session.GetString("depositAmount"), out Amount) || AccountNum == null || Amount <= 0 || customer == null)
                return NotFound();

            var account = customer.Accounts.FirstOrDefault(x => x.AccountNumber == AccountNum);
            if (account == null)
                return NotFound();
            
            // Note this code could be moved out of the controller, e.g., into the Model.
            account.Balance += Amount;
            account.Transactions.Add(
                new Transaction
                {
                    TransactionType = TransactionType.Deposit,
                    Amount = Amount,
                    TransactionTimeUtc = DateTime.UtcNow
                });

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

    }
}
