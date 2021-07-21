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
using SimpleHashing;
using System.Text.RegularExpressions;
using X.PagedList;

namespace WebBanking.Controllers
{
    public class CustomerController : Controller
    {
        private readonly WebBankContext _context;

        public CustomerController(WebBankContext context)
        {
            _context = context;
        }


        // Display accounts
        // GET: Customer
        public async Task<IActionResult> Index()
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

        // GET: Customer/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _context.Customer
                .FirstOrDefaultAsync(m => m.CustomerID == id);
            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        //GET: Customer/Deposit/5
        public async Task<IActionResult> Deposit(int? id)
        {
            var customerID = HttpContext.Session.GetInt32(nameof(Customer.CustomerID));
            var customer = await _context.Customer
                .FirstOrDefaultAsync(m => m.CustomerID == customerID);
            if (customer.Accounts.FirstOrDefault(x => x.AccountNumber == id) == null)
            {
                return NotFound();
            }

            var account = customer.Accounts.FirstOrDefault(x => x.AccountNumber == id);


            return View(account);
        }

        [HttpPost]
        //POST: Customer/Deposit/5
        public async Task<IActionResult> Deposit(int id, decimal amount)
        {
            var account = await _context.Account.FindAsync(id);

            if (amount <= 0)
                ModelState.AddModelError(nameof(amount), "Amount must be positive.");
            if (!Regex.IsMatch(amount.ToString(), @"^[0-9]+(\.[0-9]{1,2})?$"))
                ModelState.AddModelError(nameof(amount), "Amount cannot have more than 2 decimal places.");
            if (!ModelState.IsValid)
            {
                ViewBag.Amount = amount;
                return View(account);
            }

            // Note this code could be moved out of the controller, e.g., into the Model.
            account.Balance += amount;
            account.Transactions.Add(
                new Transaction
                {
                    TransactionType = TransactionType.Deposit,
                    Amount = amount,
                    TransactionTimeUtc = DateTime.UtcNow
                });

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        //GET: Customer/Withdraw/5
        public async Task<IActionResult> Withdraw(int? id)
        {
            var customerID = HttpContext.Session.GetInt32(nameof(Customer.CustomerID));
            var customer = await _context.Customer
                .FirstOrDefaultAsync(m => m.CustomerID == customerID);
            if (customer.Accounts.FirstOrDefault(x => x.AccountNumber == id) == null)
            {
                return NotFound();
            }

            var account = customer.Accounts.FirstOrDefault(x => x.AccountNumber == id);


            return View(account);
        }

        [HttpPost]
        //POST: Customer/Withdraw/5
        public async Task<IActionResult> Withdraw(int id, decimal amount)
        {
            var account = await _context.Account.FindAsync(id);
            decimal fee = account.FreeTransactions <= 0 ? 0.1m : 0;

            if (amount <= 0)
                ModelState.AddModelError(nameof(amount), "Amount must be positive.");
            if (!Regex.IsMatch(amount.ToString(), @"^[0-9]+(\.[0-9]{1,2})?$"))
                ModelState.AddModelError(nameof(amount), "Amount cannot have more than 2 decimal places.");
            if (account.Type == AccountType.Savings)
                if (account.Balance - amount - fee < 0)
                    ModelState.AddModelError(nameof(amount), "Insuficient funds");
            if (account.Type == AccountType.Checking)
                if (account.Balance - amount - fee < 200)
                    ModelState.AddModelError(nameof(amount), "Insuficient funds");
            if (!ModelState.IsValid)
            {
                ViewBag.Amount = amount;
                return View(account);
            }

            // Note this code could be moved out of the controller, e.g., into the Model.

            account.Balance -= amount;
            account.FreeTransactions--;
            account.Transactions.Add(
                new Transaction
                {
                    TransactionType = TransactionType.Withdraw,
                    Amount = amount,
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

            return RedirectToAction(nameof(Index));
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



            return View(new Transaction() { AccountNumber = account.AccountNumber });
        }

        [HttpPost]

        public async Task<IActionResult> TransferForm(Transaction transaction)
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

            // validate amount decimal places
            if (!Regex.IsMatch(transaction.Amount.ToString(), @"^[0-9]+(\.[0-9]{1,2})?$"))
                ModelState.AddModelError(nameof(transaction.Amount), "Amount cannot have more than 2 decimal places.");

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

            return RedirectToAction(nameof(Index));
        }

        //GET: Customer/Statements/5
        public async Task<IActionResult> Statements(int? id, int page = 1)
        {

            var customerID = HttpContext.Session.GetInt32(nameof(Customer.CustomerID));
            var transactions = await _context.Transaction.Where(x => x.AccountNumber == id).OrderByDescending(y => y.TransactionTimeUtc).ToPagedListAsync(page, 4);
            if (customerID == null)
                return NotFound();
            return View(transactions);
        }


        // GET: Customer/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Customer/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CustomerID,Name,TFN,Address,Suburb,State,Postcode,Mobile")] Customer customer)
        {
            if (ModelState.IsValid)
            {
                _context.Add(customer);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(customer);
        }

        public async Task<IActionResult> Profile(int? id)
        {
            if (id == null || id != HttpContext.Session.GetInt32(nameof(Customer.CustomerID)))
            {
                return NotFound();
            }

            var customer = await _context.Customer.FindAsync(id);
            if (customer == null)
            {
                return NotFound();
            }
            return View(customer);
        }

        // GET: Customer/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || id != HttpContext.Session.GetInt32(nameof(Customer.CustomerID)))
            {
                return NotFound();
            }

            var customer = await _context.Customer.FindAsync(id);
            if (customer == null)
            {
                return NotFound();
            }
            return View(customer);
        }

        // POST: Customer/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CustomerID,Name,TFN,Address,Suburb,State,Postcode,Mobile")] Customer customer)
        {
            if (id != customer.CustomerID || id != HttpContext.Session.GetInt32(nameof(Customer.CustomerID)))
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(customer);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CustomerExists(customer.CustomerID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(customer);
        }

        // GET: Customer/ChangePassword/5
        public async Task<IActionResult> ChangePassword(int? id)
        {
            if (id == null || id != HttpContext.Session.GetInt32(nameof(Customer.CustomerID)))
            {
                return NotFound();
            }

            var customer = await _context.Customer.FindAsync(id);
            if (customer == null)
            {
                return NotFound();
            }
            return View(customer);
        }

        // POST: Customer/ChangePassword/5
        [HttpPost]
        public async Task<IActionResult> ChangePassword(int CustomerID, string password)
        {
            if (CustomerID != HttpContext.Session.GetInt32(nameof(Customer.CustomerID)))
            {
                return NotFound();
            }

            Login login = await _context.Login.FirstOrDefaultAsync(x => x.CustomerID == CustomerID);
            login.PasswordHash = PBKDF2.Hash(password);
            await _context.SaveChangesAsync();
            ViewBag.success = "Password has been changed";

            return View();
        }

        // GET: Customer/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _context.Customer
                .FirstOrDefaultAsync(m => m.CustomerID == id);
            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        // POST: Customer/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var customer = await _context.Customer.FindAsync(id);
            _context.Customer.Remove(customer);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CustomerExists(int id)
        {
            return _context.Customer.Any(e => e.CustomerID == id);
        }

        private Customer getCustomerFromSession()
        {
            int? customerID = HttpContext.Session.GetInt32(nameof(Customer.CustomerID));
            var customer = customerID == null ? null : _context.Customer
                .FirstOrDefault(m => m.CustomerID == customerID);

            return customer;
        }


    }
}
