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

using Microsoft.AspNetCore.Http;

namespace WebBanking.Views
{
    public class BillPayController : Controller
    {
        private readonly WebBankContext _context;

        private Customer GetActiveCustomer()
        {
            var customerID = HttpContext.Session.GetInt32(nameof(Customer.CustomerID));
            var customer = _context.Customer
                .FirstOrDefault(m => m.CustomerID == customerID);
            return customer;
        }

        public BillPayController(WebBankContext context)
        {
            _context = context;
        }

        // GET: BillPay
        public async Task<IActionResult> Index()
        {
            // Validate customer
            var customer = GetActiveCustomer();
            if (customer == null)
                return NotFound();

            // Get active customer accounts
            var accounts = _context.Account.Where(x => x.CustomerID == customer.CustomerID).Select(x => x.AccountNumber).ToList();

            var billPays = _context.BillPay.Where(x => accounts.Contains(x.AccountNumber));
            return View(await billPays.ToListAsync());
        }


        // GET: BillPay/Create
        public IActionResult Create()
        {
            var customer = GetActiveCustomer();
            if (customer == null)
                return NotFound();


            var model = new BillPayViewModel()
            {
                Accounts = _context.Account
                    .Where(x => x.CustomerID == customer.CustomerID)
                    .Select(x => new SelectListItem($"{x.AccountNumber} - {x.Type}", x.AccountNumber.ToString()))
                    .ToList(),

                Payees = _context.Payee
                    .Select(x => new SelectListItem(x.Name, x.PayeeID.ToString()))
                    .ToList(),

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
            if (billPayViewModel.ScheduleTimeUtc < DateTime.UtcNow)
                ModelState.AddModelError(nameof(billPayViewModel.ScheduleTimeUtc), "Schedule date and time must be in the future");
            // validate model
            if (!ModelState.IsValid)
            {
                billPayViewModel.Accounts = _context.Account
                    .Where(x => x.CustomerID == customer.CustomerID)
                    .Select(x => new SelectListItem($"{x.AccountNumber} - {x.Type}", x.AccountNumber.ToString()))
                    .ToList();

                billPayViewModel.Payees = _context.Payee
                    .Select(x => new SelectListItem(x.Name, x.PayeeID.ToString()))
                    .ToList();

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

        // GET: BillPay/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var billPay = await _context.BillPay.FindAsync(id);
            if (billPay == null)
                return NotFound();



            ViewData["PayeeID"] = new SelectList(_context.Payee, "PayeeID", "PayeeID", billPay.PayeeID);
            return View(billPay);
        }

        // POST: BillPay/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BillPayID,AccountNumber,PayeeID,Amount,ScheduleTimeUtc,Period")] BillPay billPay)
        {
            if (id != billPay.BillPayID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(billPay);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BillPayExists(billPay.BillPayID))
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
            ViewData["PayeeID"] = new SelectList(_context.Payee, "PayeeID", "PayeeID", billPay.PayeeID);
            return View(billPay);
        }

        // GET: BillPay/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var billPay = await _context.BillPay
                .Include(b => b.Payee)
                .FirstOrDefaultAsync(m => m.BillPayID == id);
            if (billPay == null)
            {
                return NotFound();
            }

            return View(billPay);
        }

        // POST: BillPay/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var billPay = await _context.BillPay.FindAsync(id);
            _context.BillPay.Remove(billPay);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BillPayExists(int id)
        {
            return _context.BillPay.Any(e => e.BillPayID == id);
        }
    }
}
