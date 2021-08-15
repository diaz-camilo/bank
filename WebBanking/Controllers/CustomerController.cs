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
using Microsoft.AspNetCore.Authorization;
using WebBanking.Repository;
using utils.Enums;

namespace WebBanking.Controllers
{
    [Authorize(Roles = nameof(RoleEnum.Customer))]

    public class CustomerController : Controller
    {
        private readonly WebBankContext _context;
        private readonly IUserRepository _userRepository;

        // Get Customer ID from Claims
        private int GetCustomerID() => Int32.Parse(HttpContext.User.FindFirst("CustomerID").Value);

        // Get Customer Object from DB
        private async Task<Customer> getActiveCustomerAsync() => await _context.Customer.FindAsync(GetCustomerID());

        public CustomerController(WebBankContext context, IUserRepository userRepository)
        {
            _context = context;
            _userRepository = userRepository;
        }


        // Display accounts
        // GET: Customer
        public async Task<IActionResult> Index()
        {
            var customer = await getActiveCustomerAsync();
            return View(customer);
        }

        // GET: Customer/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            var customer = await getActiveCustomerAsync();
            return View(customer);
        }

        //GET: Customer/Statements/5
        public async Task<IActionResult> Statements(int? id, int page = 1)
        {
            var transactions = await _context.Transaction.
                Where(x => x.AccountNumber == id).
                OrderByDescending(y => y.TransactionTimeUtc).
                ToPagedListAsync(page, 4);
            
            return View(transactions);
        }


        // GET: Customer/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            var customer = await getActiveCustomerAsync();
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
            var customerID = GetCustomerID();
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
    }
}
