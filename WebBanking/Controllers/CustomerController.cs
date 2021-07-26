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
using WebBanking.ViewModels;

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

            var customerID = HttpContext.Session.GetInt32(nameof(Customer.CustomerID));
            var customer = await _context.Customer
                .FirstOrDefaultAsync(m => m.CustomerID == customerID);

            if (customer == null)
                return NotFound();

            return View(customer);
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

        //public async Task<IActionResult> Profile(int? id)
        //{
        //    if (id == null || id != HttpContext.Session.GetInt32(nameof(Customer.CustomerID)))
        //    {
        //        return NotFound();
        //    }

        //    var customer = await _context.Customer.FindAsync(id);
        //    if (customer == null)
        //    {
        //        return NotFound();
        //    }
        //    return View(customer);
        //}

        // GET: Customer/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            var customerID = HttpContext.Session.GetInt32(nameof(Customer.CustomerID));
            var customer = await _context.Customer.FindAsync(customerID);

            if (customer == null)
                return NotFound();

            return View(customer);
        }

        // POST: Customer/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CustomerID,Name,TFN,Address,Suburb,State,Postcode,Mobile")] Customer customer)
        {
            // validate customer details to edit belong to logged in customer
            var customerID = HttpContext.Session.GetInt32(nameof(Customer.CustomerID));
            if (customer.CustomerID != customerID)
                return NotFound();

            // validate model
            if (!ModelState.IsValid)
                return View(customer);

            // save changes
            _context.Update(customer);
            await _context.SaveChangesAsync();

            //return To details            
            return RedirectToAction(nameof(Details));
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
