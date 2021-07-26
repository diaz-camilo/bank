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
using Newtonsoft;
using Newtonsoft.Json;

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
            if (!Decimal.TryParse(HttpContext.Session.GetString("depositAmount"), out Amount) || AccountNum == null)
                return NotFound();

            return View(new TransactionViewModel { AccountNumber = (int)AccountNum, Amount = Amount });
        }

        [HttpPost]
        //POST: Customer/Deposit/5
        public async Task<IActionResult> DepositConfirm(int id = 1)
        {
            decimal Amount;
            int? AccountNum = HttpContext.Session.GetInt32("depositAccountNum");
            bool isAmountSet = Decimal.TryParse(HttpContext.Session.GetString("depositAmount"), out Amount);
            HttpContext.Session.Remove("depositAccountNum");
            HttpContext.Session.Remove("depositAmount");
            var customerID = HttpContext.Session.GetInt32(nameof(Customer.CustomerID));
            var customer = await _context.Customer.FindAsync(customerID);

            if (!isAmountSet || AccountNum == null || Amount <= 0 || customer == null)
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

            return RedirectToAction(nameof(ClearTransactions));
        }

        //GET: Customer/Withdraw/5
        public async Task<IActionResult> Withdraw(int? id)
        {
            var customerID = HttpContext.Session.GetInt32(nameof(Customer.CustomerID));
            var customer = await _context.Customer.FindAsync(customerID);
            if (customer == null
                || customer.Accounts.FirstOrDefault(x => x.AccountNumber == id) == null)
                return NotFound();

            return View(new TransactionViewModel() { AccountNumber = (int)id });
        }

        [HttpPost]
        //POST: Customer/Withdraw/5
        public async Task<IActionResult> Withdraw(TransactionViewModel transaction)
        {
            var customerID = HttpContext.Session.GetInt32(nameof(Customer.CustomerID));
            var customer = await _context.Customer.FindAsync(customerID);
            if (customer == null)
                return NotFound();

            var account = customer.Accounts.FirstOrDefault(x => x.AccountNumber == transaction.AccountNumber);
            if (account == null)
                return NotFound();


            decimal fee = account.FreeTransactions <= 0 ? 0.1m : 0;

            // validate suficient funds acording to account type
            if (account.Type == AccountType.Savings)
                if (account.Balance - transaction.Amount - fee < 0)
                    ModelState.AddModelError(nameof(transaction.Amount), "Insuficient funds");
            if (account.Type == AccountType.Checking)
                if (account.Balance - transaction.Amount - fee < 200)
                    ModelState.AddModelError(nameof(transaction.Amount), "Insuficient funds");

            if (!ModelState.IsValid)
                return View(transaction);

            account.Balance -= transaction.Amount;
            account.FreeTransactions--;
            account.Transactions.Add(
                new Transaction
                {
                    TransactionType = TransactionType.Withdraw,
                    Amount = transaction.Amount,
                    TransactionTimeUtc = DateTime.UtcNow
                });
            if (fee > 0)
            {
                account.Balance -= fee;
                account.Transactions.Add(
                new Transaction
                {
                    TransactionType = TransactionType.ServiceCharge,
                    Amount = fee,
                    TransactionTimeUtc = DateTime.UtcNow,
                    Comment = "Withdraw fee"
                });
            }
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index), nameof(Customer));
        }

        public async Task<IActionResult> Transfer()
        {
            var customerID = HttpContext.Session.GetInt32(nameof(Customer.CustomerID));
            var customer = await _context.Customer
                .FirstOrDefaultAsync(m => m.CustomerID == customerID);
            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        public async Task<IActionResult> TransferForm(int? id)
        {
            var customerID = HttpContext.Session.GetInt32(nameof(Customer.CustomerID));
            var customer = await _context.Customer
                .FirstOrDefaultAsync(m => m.CustomerID == customerID);
            if (customer == null)
                return NotFound();
            var account = customer.Accounts.FirstOrDefault(x => x.AccountNumber == id);
            if (account == null)
                return NotFound();



            return View(new TransactionViewModel() { AccountNumber = account.AccountNumber });
        }

        [HttpPost]
        public async Task<IActionResult> TransferForm(TransactionViewModel transaction)
        {
            var customerID = HttpContext.Session.GetInt32(nameof(Customer.CustomerID));
            var customer = await _context.Customer
                .FirstOrDefaultAsync(m => m.CustomerID == customerID);
            if (customer == null)
                return NotFound();

            var originAccount = await _context.Account.FindAsync(transaction.AccountNumber);
            decimal fee = originAccount.FreeTransactions <= 0 ? 0.20m : 0; // 0.20 for transfers

            var destinationAccount = await _context.Account.FindAsync(transaction.DestinationAccountNumber);

            // validate that origin account number belongs to customer
            if (customer.Accounts.FirstOrDefault(x => x.AccountNumber == transaction.AccountNumber) == null)
                return NotFound();

            // validate origin account is diferent from destination account
            if (transaction.AccountNumber == transaction.DestinationAccountNumber)
                ModelState.AddModelError(nameof(transaction.DestinationAccountNumber), "Destination account must be different from origin account");

            // validate destination account exist.
            if (destinationAccount == null)
                ModelState.AddModelError(nameof(transaction.DestinationAccountNumber), "Account does not exist");

            // validate suficient funds
            if (originAccount.Type == AccountType.Savings)
                if (originAccount.Balance - transaction.Amount - fee < 0)
                    ModelState.AddModelError(nameof(transaction.Amount), "Insuficient funds");

            if (originAccount.Type == AccountType.Checking)
                if (originAccount.Balance - transaction.Amount - fee < 200)
                    ModelState.AddModelError(nameof(transaction.Amount), "Insuficient funds");

            if (!ModelState.IsValid)
                return View(transaction);

            HttpContext.Session.SetString("jsonTransactionViewModelObject", JsonConvert.SerializeObject(transaction));
            return RedirectToAction(nameof(TransferFormConfirm));
        }



        public async Task<IActionResult> TransferFormConfirm()
        {
            var customerID = HttpContext.Session.GetInt32(nameof(Customer.CustomerID));
            var customer = await _context.Customer
                .FirstOrDefaultAsync(m => m.CustomerID == customerID);
            if (customer == null)
                return NotFound();

            var jsonTransaction = HttpContext.Session.GetString
                ("jsonTransactionViewModelObject");

            TransactionViewModel transaction = jsonTransaction == null ?
                null :
                JsonConvert.DeserializeObject<TransactionViewModel>(jsonTransaction);

             

            return transaction == null ?
                RedirectToAction(nameof(Index), nameof(Customer)) :
                View(transaction);


        }

        [HttpPost]

        public async Task<IActionResult> TransferFormConfirm(int i)
        {
            var customerID = HttpContext.Session.GetInt32(nameof(Customer.CustomerID));
            var customer = await _context.Customer
                .FirstOrDefaultAsync(m => m.CustomerID == customerID);
            if (customer == null)
                return NotFound();

            TransactionViewModel transaction =
                JsonConvert.DeserializeObject<TransactionViewModel>
                (HttpContext.Session.GetString("jsonTransactionViewModelObject"));

            var originAccount = await _context.Account.FindAsync(transaction.AccountNumber);
            decimal fee = originAccount.FreeTransactions <= 0 ? 0.20m : 0; // 0.20 for transfers

            var destinationAccount = await _context.Account.FindAsync(transaction.DestinationAccountNumber);

            // validate that origin account number belongs to customer
            if (customer.Accounts.FirstOrDefault(x => x.AccountNumber == transaction.AccountNumber) == null)
                return NotFound();

            // validate origin account is diferent from destination account
            if (transaction.AccountNumber == transaction.DestinationAccountNumber)
                ModelState.AddModelError(nameof(transaction.DestinationAccountNumber), "Destination account must be different from origin account");

            // validate destination account exist.
            if (destinationAccount == null)
                ModelState.AddModelError(nameof(transaction.DestinationAccountNumber), "Account does not exist");

            // validate suficient funds
            if (originAccount.Type == AccountType.Savings)
                if (originAccount.Balance - transaction.Amount - fee < 0)
                    ModelState.AddModelError(nameof(transaction.Amount), "Insuficient funds");

            if (originAccount.Type == AccountType.Checking)
                if (originAccount.Balance - transaction.Amount - fee < 200)
                    ModelState.AddModelError(nameof(transaction.Amount), "Insuficient funds");

            if (!ModelState.IsValid)
                return View(transaction);


            // Note this code could be moved out of the controller, e.g., into the Model.

            originAccount.Balance -= transaction.Amount;
            destinationAccount.Balance += transaction.Amount;
            originAccount.FreeTransactions--;
            originAccount.Transactions.Add(
                new Transaction
                {
                    TransactionType = TransactionType.OutgoingTransfer,
                    Amount = transaction.Amount,
                    DestinationAccountNumber = transaction.DestinationAccountNumber,
                    Comment = transaction.Comment,
                    TransactionTimeUtc = DateTime.UtcNow
                });
            destinationAccount.Transactions.Add(
                new Transaction
                {
                    TransactionType = TransactionType.IncomingTransfer,
                    Amount = transaction.Amount,
                    Comment = transaction.Comment,
                    TransactionTimeUtc = DateTime.UtcNow
                });
            if (fee > 0)
            {
                originAccount.Balance -= fee;
                originAccount.Transactions.Add(
                new Transaction
                {
                    TransactionType = TransactionType.ServiceCharge,
                    Amount = fee,
                    TransactionTimeUtc = DateTime.UtcNow,
                    Comment = "Transfer fee"
                });
            }
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(ClearTransactions));
        }

        public IActionResult ClearTransactions()
        {
            HttpContext.Session.Remove("depositAccountNum");
            HttpContext.Session.Remove("depositAmount");
            HttpContext.Session.Remove("jsonTransactionViewModelObject");

            return RedirectToAction(nameof(Index), nameof(Customer));
        }

    }
}
