using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using AdminPortal.Models;
using AdminPortal.ViewModels;
using AdminPortal.Repository;

namespace AdminPortal.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUserRepository _userRepository;

        public HomeController(ILogger<HomeController> logger, IUserRepository userRepository)
        {
            _logger = logger;
            this._userRepository = userRepository;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> LogoutAsync()
        {
            await _userRepository.LogoutUserAsync();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> IndexAsync(LoginViewModel login)
        {
            if (!ModelState.IsValid)
                return View(login);



            var result = await _userRepository.LoginUserAsync(login);

            if (result.Succeeded)
            {
                return View();
            }

            ModelState.AddModelError(nameof(login.LoginID), "Invalid Credentials");

            return View(login);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
