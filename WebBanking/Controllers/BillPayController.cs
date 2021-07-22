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

        public BillPayController(WebBankContext context)
        {
            _context = context;
        }

        // GET: BillPay
        public async Task<IActionResult> Index()
        {
            var webBankContext = _context.BillPay.Include(b => b.Payee);
            return View(await webBankContext.ToListAsync());
        }

        // GET: BillPay/Details/5
        public async Task<IActionResult> Details(int? id)
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

        // GET: BillPay/Create
        public IActionResult Create()
        {
            //ViewData["PayeeID"] = new SelectList(_context.Payee, "PayeeID", "PayeeID");
            //ViewData["Period"] = new SelectList(Enum.GetValues<Period>());
            //return View();
            //
            var customerID = HttpContext.Session.GetInt32(nameof(Customer.CustomerID));
            var customer = _context.Customer
                .FirstOrDefault(m => m.CustomerID == customerID);
            if (customer == null)
            {
                return NotFound();
            }

            var model = new BillPayViewModel();
            model.Accounts = new SelectList(_context.Account.Where(x => x.CustomerID == customerID).Select(x => x.AccountNumber));
            model.Payees = new SelectList(_context.Payee, "PayeeID","PayeeID");
            model.ScheduleTimeUtc = DateTime.UtcNow.ToLocalTime();

            return View(model);
        }

        // POST: BillPay/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BillPayID,AccountNumber,PayeeID,Amount,ScheduleTimeUtc,Period")] BillPay billPay)
        {
            if (ModelState.IsValid)
            {
                _context.Add(billPay);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["PayeeID"] = new SelectList(_context.Payee, "PayeeID", "PayeeID", billPay.PayeeID);
            return View(billPay);
        }

        // GET: BillPay/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var billPay = await _context.BillPay.FindAsync(id);
            if (billPay == null)
            {
                return NotFound();
            }
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
