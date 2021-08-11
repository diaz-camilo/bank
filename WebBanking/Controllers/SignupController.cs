using System;
using Microsoft.AspNetCore.Mvc;
using WebBanking.Models;

namespace WebBanking.Controllers
{
    public class SignupController : Controller
    {

        public IActionResult NewCustomer()
        {
            return View();
        }

        [HttpPost]
        public IActionResult NewCustomer(SignupUser model)
        {
            if (!ModelState.IsValid)
                return View();

            return View();
        }

    }
}
