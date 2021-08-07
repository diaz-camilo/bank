using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using AdminPortal.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;
using AdminPortal.Models;
using System.Text.RegularExpressions;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace AdminPortal.Controllers
{
    public class AdminController : Controller
    {
        private readonly IHttpClientFactory _clientFactory;

        private HttpClient Client =>
            _clientFactory.CreateClient("api");


        public AdminController(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        // GET: /<controller>/
        public IActionResult Index(AdminIndexErrorEnum error, string errorMessage)
        {
            //if (errorMessage == null)
            //    return View(new IndexViewModel { Error = AdminIndexErrorEnum.noError });

            return View(new IndexViewModel
                ()
            //{ Error = error, ErrorMessage = errorMessage }
            );
        }

        [HttpPost]
        [HttpGet]
        public async Task<IActionResult> TransactionsByAccountNum(TransactionByAccountViewModel transactionByAccountViewModel)
        {
            if (!ModelState.IsValid)
                return View(transactionByAccountViewModel);


            if (transactionByAccountViewModel.StartDate != null && transactionByAccountViewModel.EndDate != null)
            {

                var startDate = transactionByAccountViewModel.StartDate.Split('-');
                var endDate = transactionByAccountViewModel.EndDate.Split('-');



                if (new DateTime(Int32.Parse(startDate[0]), Int32.Parse(startDate[1]), Int32.Parse(startDate[2])) >
                    new DateTime(Int32.Parse(endDate[0]), Int32.Parse(endDate[1]), Int32.Parse(endDate[2])))
                {
                    ModelState.AddModelError(nameof(transactionByAccountViewModel.StartDate), "Start date must be before End date");
                    return View(transactionByAccountViewModel);
                }
            }

            HttpResponseMessage response;

            if (transactionByAccountViewModel.StartDate == null || transactionByAccountViewModel.EndDate == null)
                response = await Client.GetAsync($"/BankAPI/Admin/transactions/{transactionByAccountViewModel.AccountNum}");
            else
                response = await Client.GetAsync($"/BankAPI/Admin/transactionsByDate/{transactionByAccountViewModel.AccountNum}/{transactionByAccountViewModel.StartDate}/{transactionByAccountViewModel.EndDate}");

            if (!response.IsSuccessStatusCode)
                throw new Exception();



            // Storing the response details received from web api.
            var result = await response.Content.ReadAsStringAsync();

            // Deserializing the response received from web api and storing into a list.
            var transactions = JsonConvert.DeserializeObject<List<Transaction>>(result);

            transactions.OrderBy(x => x.TransactionTimeUtc);

            transactionByAccountViewModel.transactions = transactions;


            return View(transactionByAccountViewModel);
        }

        public async Task<IActionResult> TransactionsByAmount(TransactionByAmountViewModel model)
        {
            if (model.MaxAmount <= model.MinAmount)
                ModelState.AddModelError(nameof(model.MaxAmount), "Max Amount must be greater than Min Amount");

            if (!ModelState.IsValid)
                return View(model);

            var response = await Client.GetAsync($"/BankAPI/Admin/transactionsByAmount/{model.MinAmount}/{model.MaxAmount}");

            if (!response.IsSuccessStatusCode)
                throw new Exception();

            var result = await response.Content.ReadAsStringAsync();

            var transactions = JsonConvert.DeserializeObject<List<Transaction>>(result);

            transactions.OrderBy(x => x.TransactionTimeUtc);

            model.Transactions = transactions;

            return View(model);
        }

        public async Task<IActionResult> CustomerAccess(CustomerAccessViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var response = await Client.PutAsync($"/BankAPI/Admin/BlockCustomerByID/{model.CustomerID}", JsonContent.Create(model.LoginState));

            if (!response.IsSuccessStatusCode)
                model.Response = @"Customer was not found";
            else
                model.Response = "Customer access has been updated";

            return View(model);
        }

        public async Task<IActionResult> BillPayState(BillPayStateViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var response = await Client.PutAsync($"/BankAPI/Admin/BlockBillPayByID/{model.BillPayID}/{model.BillPayState}", JsonContent.Create(model.BillPayState));

            if (!response.IsSuccessStatusCode)
                model.Response = @"BillPay was not found";
            else
                model.Response = "BillPay State has been updated";

            return View(model);

        }

        [HttpGet]
        public async Task<IActionResult> UpdateCustomerDetails(int? id,[Bind(nameof(Customer.CustomerID))] Customer customer)
        {
            ModelState.Clear();

            if (customer == null || !Regex.IsMatch(customer.CustomerID.ToString(), @"^\d{4}$"))
            {
                ModelState.AddModelError(nameof(customer.CustomerID), "Customer ID must be a 4 digit number");
                return View(new Customer());
            }

            var response = await Client.GetAsync($"/BankAPI/Admin/GetCustomerDetails/{customer.CustomerID}");

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError(nameof(customer.CustomerID), "Customer not found, please check the customer ID and try again");
                return View(customer);
            }

            var content = await response.Content.ReadAsStringAsync();

            customer = JsonConvert.DeserializeObject<Customer>(content);

            return View(customer);


        }

        [HttpPost]
        public async Task<IActionResult> UpdateCustomerDetails(Customer customer)
        {
            if (!ModelState.IsValid)
                return View("UpdateCustomerDetailsError", customer);

            var response = await Client.PutAsync($"/BankAPI/Admin/UpdateCustomerDetails/{customer.CustomerID}", JsonContent.Create(customer));

            if (!response.IsSuccessStatusCode)
                ModelState.AddModelError(nameof(customer.CustomerID), "Error while trying to update customer details");

            
            return View("UpdateCustomerDetailsSuccess",customer);
        }
    }
}