using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebBanking.Data;
using WebBanking.Models;
using SimpleHashing;

namespace WebBanking.Controllers
{

    public class LoginController : Controller
    {
        // database access context
        private readonly WebBankContext _context;

        public LoginController(WebBankContext context) => _context = context;

        // GET: Login
        public ActionResult Login() => View();
        

        // POST: Login
        [HttpPost]
        public ActionResult Login(string loginID, string password)
        {
            // look for a match in the database
            Login login = _context.Login.Find(loginID);
            // if no match or incorrect password, display error message and prepopulate with entered liginID
            if (login == null || !PBKDF2.Verify(login.PasswordHash, password))
            {
                ModelState.AddModelError("LoginFailed", "Login failed, please try again.");
                return View(new Login { LoginID = loginID });
            }

            // there was a match, login customer.
            // store Customer ID and Customer name in session as key value pairs
            HttpContext.Session.SetInt32(nameof(Customer.CustomerID), login.CustomerID);
            HttpContext.Session.SetString(nameof(Customer.Name), login.Customer.Name);

            return RedirectToAction("Index", "Customer");
        }

        // GET: Logout
        public ActionResult Logout()
        {
            // clear the session
            HttpContext.Session.Clear();
            // redirect to homepage
            return RedirectToAction("Index", "Home");
        }
    }
}