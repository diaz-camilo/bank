using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebBanking.Data;
using WebBanking.Models;
using WebBanking.ViewModels;
using X.PagedList;

using Microsoft.AspNetCore.Http;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Authorization;

namespace WebBanking.Views
{
    [Authorize]
    public class BillPayController : Controller
    {
        private readonly WebBankContext _context;

        private Customer GetActiveCustomer()
        {
            var customerID = HttpContext.Session.GetInt32(nameof(Customer.CustomerID));
            var customer = _context.Customer.Include(x => x.Accounts).ThenInclude(x => x.BillPays)
                .FirstOrDefault(m => m.CustomerID == customerID);
            return customer;
        }

        public BillPayController(WebBankContext context) => _context = context;

        // GET: BillPay
        public async Task<IActionResult> Index(int page = 1)
        {
            // Validate customer
            var customer = GetActiveCustomer();
            if (customer == null)
                return NotFound();

            // Get active customer accounts
            var accounts = _context.Account.
                Where(x => x.CustomerID == customer.CustomerID).
                Select(x => x.AccountNumber).
                ToList();

            // Get active Bills
            var billPays = _context.BillPay.
                Where(x => accounts.Contains(x.AccountNumber)).
                Where(x => x.State == State.active).
                OrderBy(bill => bill.ScheduleTimeUtc);

            return View(await billPays.ToPagedListAsync(page, 6));
        }

        public async Task<IActionResult> FailedBills(int page = 1)
        {
            // Validate customer
            var customer = GetActiveCustomer();
            if (customer == null)
                return NotFound();

            // Get active customer accounts
            var accounts = _context.Account.
                Where(x => x.CustomerID == customer.CustomerID).
                Select(x => x.AccountNumber).
                ToList();

            // Get faild Bills
            var billPays = _context.BillPay.
                Where(x => accounts.Contains(x.AccountNumber)).
                Where(x => x.State == State.failed).
                OrderBy(bill => bill.ScheduleTimeUtc);

            return View(await billPays.ToPagedListAsync(page, 6));
        }

        public async Task<IActionResult> BlockedBills(int page = 1)
        {
            // Validate customer
            var customer = GetActiveCustomer();
            if (customer == null)
                return NotFound();

            // Get active customer accounts
            var accounts = _context.Account.
                Where(x => x.CustomerID == customer.CustomerID).
                Select(x => x.AccountNumber).
                ToList();

            // Get blocked Bills
            var billPays = _context.BillPay.
                Where(x => accounts.Contains(x.AccountNumber)).
                Where(x => x.State == State.blocked).
                OrderBy(bill => bill.ScheduleTimeUtc);

            return View(await billPays.ToPagedListAsync(page, 6));
        }




        // GET: BillPay/Create
        public IActionResult Create()
        {
            var customer = GetActiveCustomer();
            if (customer == null)
                return NotFound();


            var model = new BillPayViewModel()
            {
                Accounts = SelectListItemsOfCustomerAccounts(customer),
                Payees = SelectListItemsOfPayees(),
                ScheduleTimeUtc = DateTime.UtcNow.AddDays(1).ToLocalTime().Date
            };


            return View(model);
        }

        // POST: BillPay/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AccountNumberSelected,PayeeIDSelected,Amount,ScheduleTimeUtc,PeriodSelected")] BillPayViewModel billPayViewModel)
        {

            // validate customer
            var customer = GetActiveCustomer();
            if (customer == null)
                return NotFound();
            // Check that date is in the future
            if (billPayViewModel.ScheduleTimeUtc < DateTime.UtcNow.ToLocalTime())
                ModelState.AddModelError(nameof(billPayViewModel.ScheduleTimeUtc), "Schedule date and time must be in the future");
            // validate model
            if (!ModelState.IsValid)
            {
                billPayViewModel.Accounts = SelectListItemsOfCustomerAccounts(customer);
                billPayViewModel.Payees = SelectListItemsOfPayees();
                return View(billPayViewModel);
            }

            var billPay = new BillPay
            {
                AccountNumber = billPayViewModel.AccountNumberSelected,
                Amount = billPayViewModel.Amount,
                PayeeID = billPayViewModel.PayeeIDSelected,
                ScheduleTimeUtc = billPayViewModel.ScheduleTimeUtc,
                Period = billPayViewModel.PeriodSelected
            };

            _context.Add(billPay);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));

        }

        // GET: BillPay/Reschedule/5
        public async Task<IActionResult> Reschedule(int? id)
        {
            // validate customer
            var customer = GetActiveCustomer();
            if (customer == null)
                return NotFound();

            if (id == null)
                return NotFound();

            if (!OwnsBill((int)id, customer))
                return NotFound();

            var billPay = await _context.BillPay.FindAsync(id);
            if (billPay == null || billPay.State != State.failed)
                return NotFound();


            HttpContext.Session.SetInt32("RescheduleBillID", billPay.AccountNumber);

            return View(billPay);

        }

        // POST: BillPay/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reschedule([Bind("BillPayID,ScheduleTimeUtc")] BillPay billPayPosted)
        {
            var billPayID = HttpContext.Session.GetInt32("RescheduleBillID");
            var customer = GetActiveCustomer();

            if (customer == null ||
                billPayID == null ||
                billPayID != billPayPosted.BillPayID ||
                !OwnsBill((int)billPayID, customer))

                return NotFound();

            HttpContext.Session.Remove("RescheduleBillID");


            // chech bill details posted match bill details stored



            // Check that date is in the future
            if (billPayPosted.ScheduleTimeUtc < DateTime.UtcNow.ToLocalTime())
            {
                ModelState.AddModelError(nameof(billPayPosted.ScheduleTimeUtc), "Schedule date and time must be in the future");
                return View(billPayPosted);
            }

            var billPay = await _context.BillPay.FindAsync(billPayPosted.BillPayID);

            // if bill is blocked reject request
            if (billPay.State == State.blocked)
                return NotFound();

            billPay.ScheduleTimeUtc = billPayPosted.ScheduleTimeUtc;
            billPay.State = State.active;

            try
            {
                _context.Update(billPay);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BillPayExists(billPay.BillPayID))
                    return NotFound();
                else
                    throw;
            }
            return RedirectToAction(nameof(FailedBills));
        }

        public async Task<IActionResult> TryAgain(int? id)
        {
            var customer = GetActiveCustomer();
            var billPay = _context.BillPay.Find(id);

            if (customer == null || // customer is logged in
                billPay == null || // bill exist
                !OwnsBill((int)id, customer) || // and the customer owns it
                billPay.State != State.failed ) // and the bill is not active
                
                return NotFound();

            billPay.State = State.active;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(FailedBills));
        }

        // GET: BillPay/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            // validate customer
            var customer = GetActiveCustomer();
            if (customer == null)
                return NotFound();

            if (id == null)
                return NotFound();

            if (!OwnsBill((int)id, customer))
                return NotFound();

            var billPay = await _context.BillPay.FindAsync(id);
            if (billPay == null)
                return NotFound();

            var viewModel = new BillPayViewModel
            {
                AccountNumberSelected = billPay.AccountNumber,
                Amount = billPay.Amount,
                BillPayID = billPay.BillPayID,
                ScheduleTimeUtc = billPay.ScheduleTimeUtc,
                PeriodSelected = billPay.Period,
                PayeeIDSelected = billPay.PayeeID,
                Accounts = SelectListItemsOfCustomerAccounts(customer),
                Payees = SelectListItemsOfPayees()
            };

            return View(viewModel);
        }

        // POST: BillPay/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BillPayID,AccountNumberSelected,PayeeIDSelected,Amount,ScheduleTimeUtc,PeriodSelected")] BillPayViewModel billPayViewModel)
        {
            var customer = GetActiveCustomer();
            if (customer == null || id != billPayViewModel.BillPayID)
                return NotFound();

            if (!OwnsBill((int)id, customer))
                return NotFound();

            // Check that date is in the future
            if (billPayViewModel.ScheduleTimeUtc < DateTime.UtcNow.ToLocalTime())
                ModelState.AddModelError(nameof(billPayViewModel.ScheduleTimeUtc), "Schedule date and time must be in the future");

            if (!ModelState.IsValid)
            {
                billPayViewModel.Accounts = SelectListItemsOfCustomerAccounts(customer);
                billPayViewModel.Payees = SelectListItemsOfPayees();
                return View(billPayViewModel);
            }

            var billPay = await _context.BillPay.FindAsync(id);

            billPay.AccountNumber = billPayViewModel.AccountNumberSelected;
            billPay.Amount = billPayViewModel.Amount;
            billPay.PayeeID = billPayViewModel.PayeeIDSelected;
            billPay.ScheduleTimeUtc = billPayViewModel.ScheduleTimeUtc;
            billPay.Period = billPayViewModel.PeriodSelected;

            try
            {
                _context.Update(billPay);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BillPayExists(billPay.BillPayID))
                    return NotFound();
                else
                    throw;
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: BillPay/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            var customer = GetActiveCustomer();

            if (customer == null || id == null)
                return NotFound();

            if (!OwnsBill((int)id, customer))
                return NotFound();

            var billPay = await _context.BillPay.FirstOrDefaultAsync(m => m.BillPayID == id);

            if (billPay == null)
                return NotFound();

            return View(billPay);
        }

        // POST: BillPay/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var customer = GetActiveCustomer();
            if (customer == null)
                return NotFound();

            if (!OwnsBill(id, customer))
                return NotFound();


            var billPay = await _context.BillPay.FindAsync(id);
            _context.BillPay.Remove(billPay);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BillPayExists(int id) =>
            _context.BillPay.Any(e => e.BillPayID == id);


        private bool OwnsBill(int id, Customer customer) =>
            customer.
            Accounts.
            Where(acc => acc.BillPays.
                Contains(new BillPay { BillPayID = id }, new BillComparer())).
            Any();

        private List<SelectListItem> SelectListItemsOfCustomerAccounts(Customer customer) =>
            _context.Account
                    .Where(x => x.CustomerID == customer.CustomerID)
                    .Select(x => new SelectListItem($"{x.AccountNumber} - {x.Type}", x.AccountNumber.ToString()))
                    .ToList();

        private List<SelectListItem> SelectListItemsOfPayees() =>
            _context.Payee
                    .Select(x => new SelectListItem(x.Name, x.PayeeID.ToString()))
                    .ToList();
    }

    class BillComparer : IEqualityComparer<BillPay>
    {
        public bool Equals(BillPay x, BillPay y) => x.BillPayID == y.BillPayID ? true : false;

        public int GetHashCode([DisallowNull] BillPay obj) => obj.GetHashCode();
    }
}
