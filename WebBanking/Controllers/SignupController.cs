using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using utils.Enums;
using WebBanking.Data;
using WebBanking.Models;
using WebBanking.Repository;
using WebBanking.ViewModels;

namespace WebBanking.Controllers
{
    public class SignupController : Controller
    {
        private readonly WebBankContext _context;
        private readonly IUserRepository _userRepository;

        public SignupController(WebBankContext context, IUserRepository userRepository)
        {
            _context = context;
            _userRepository = userRepository;
        }

        private int? GetCustomerID()
        {
            try
            {
                return Int32.Parse(HttpContext.User.FindFirst("CustomerID").Value);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public IActionResult NewCustomer()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> NewCustomer(SignupUser model)
        {
            // validate initial deposit
            if ((model.AccountType == AccountType.Checking && model.InicialDeposit < 500) ||
                (model.AccountType == AccountType.Savings && model.InicialDeposit < 100))
                ModelState.AddModelError(nameof(model.InicialDeposit),
                    "The minimum balance to open a savings account is $100 and " +
                    "$500 for a checking account.");

            if (!ModelState.IsValid)
                return View();

            var rand = new Random();

            // Find an unused Custmer ID and Account number
            var activeCustomerIDs = await _context.Customer.
                Select(x => x.CustomerID).ToListAsync();

            var activeAccountNums = await _context.Account.
                Select(x => x.AccountNumber).ToListAsync();

            var activeLoginIDs = await _context.Users.
                Select(x => x.Id).ToListAsync();

            // Generate unique CustomerID, AccountNumber and LoginID
            var newCustomerID = rand.Next(1000, 9999);
            var newAccountNum = rand.Next(1000, 9999);
            var newLoginID = rand.Next(10000000, 99999999);

            while (activeCustomerIDs.Contains(newCustomerID))
                newCustomerID = rand.Next(1000, 9999);
            model.CustomerID = newCustomerID;

            while (activeAccountNums.Contains(newAccountNum))
                newAccountNum = rand.Next(1000, 9999);
            model.AccountNum = newAccountNum;

            while (activeLoginIDs.Contains(newLoginID))
                newLoginID = rand.Next(10000000, 99999999);
            model.LoginID = newLoginID.ToString();

            // Create Customer
            _context.Customer.Add(new Customer
            {
                CustomerID = newCustomerID,
                Name = model.Name
            });

            // Create Account
            _context.Account.Add(new Account
            {
                AccountNumber = newAccountNum,
                Type = model.AccountType,
                FreeTransactions = 4,
                Balance = model.InicialDeposit,
                CustomerID = newCustomerID,
            });

            // Add initial Deposit
            _context.Transaction.Add(new Transaction
            {
                AccountNumber = newAccountNum,
                Amount = model.InicialDeposit,
                Comment = "Opening Balance",
                TransactionTimeUtc = DateTime.UtcNow,
                TransactionType = TransactionType.Deposit,
            });
            await _context.SaveChangesAsync();

            // Add Logins
            var resultCreateUser = await _userRepository.CreateUserAsync(model);

            if (!resultCreateUser.Succeeded)
            {
                foreach (var error in resultCreateUser.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View(model);
            }
            return View("SignupSuccess", model);
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel login)
        {
            if (!ModelState.IsValid)
                return View(login);



            var result = await _userRepository.LoginUserAsync(login);

            if (result == null)
            {
                ModelState.AddModelError(nameof(login.LoginID), $"User {login.LoginID} is locked out, please contact the Admin");

                return View(login);
            }

            if (result.Succeeded)
            {
                var user = _context.Users.FirstOrDefault(x => x.UserName == login.LoginID);

                HttpContext.Session.SetInt32(nameof(Customer.CustomerID), user.CustomerID.Value);
                HttpContext.Session.SetString(nameof(Customer.Name), user.Customer.Name);

                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError(nameof(login.LoginID), "Invalid Credentials");

            return View(login);
        }

        [Authorize(Roles =nameof(RoleEnum.Customer))]
        public async Task<IActionResult> Logout()
        {
            await _userRepository.LogoutUserAsync();
            HttpContext.Session.Clear();

            return RedirectToAction("Index", "Home");
        }

        [Authorize(Roles = nameof(RoleEnum.Customer))]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost, Authorize(Roles = nameof(RoleEnum.Customer))]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var id = HttpContext.User.Identity.Name;

            var result = await _userRepository.ChangePasswordAsync(model, id.ToString());

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Check your password and try again");
                return View(model);
            }

            ViewBag.isSuccess = true;
            return View();
        }
    }
}
