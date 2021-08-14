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
using Microsoft.AspNetCore.Authorization;

namespace AdminPortal.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IHttpClientFactory _clientFactory;

        private HttpClient Client =>
            _clientFactory.CreateClient("api");


        public AdminController(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }


        public IActionResult Index()
        {
            return View(new IndexViewModel
            {
                BillPayStateViewModel = new BillPayStateViewModel(),
                CustomerAccessViewModel = new CustomerAccessViewModel()
            });
        }

        [HttpGet]
        public IActionResult TransactionsByAccountNum()
        {
            return View(new TransactionByAccountViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> TransactionsByAccountNum(TransactionByAccountViewModel transactionByAccountViewModel)
        {
            if (!ModelState.IsValid)
                return View(transactionByAccountViewModel);

            // Validate that start date is before end date
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

            // Get transactions for the specified time interval or get all transactions if no interval provided or only one date provided
            if (transactionByAccountViewModel.StartDate == null || transactionByAccountViewModel.EndDate == null)
                response = await Client.GetAsync($"/BankAPI/Admin/transactions/{transactionByAccountViewModel.AccountNum}");
            else
                response = await Client.GetAsync($"/BankAPI/Admin/transactionsByDate/{transactionByAccountViewModel.AccountNum}/{transactionByAccountViewModel.StartDate}/{transactionByAccountViewModel.EndDate}");

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError(nameof(TransactionByAccountViewModel.AccountNum), "The Server enconter an error, please try again.");
                return View(transactionByAccountViewModel);
            }

            // deserialize response into a list of transactions and pass it to the view
            var result = await response.Content.ReadAsStringAsync();

            var transactions = JsonConvert.DeserializeObject<List<Transaction>>(result);

            transactions.OrderBy(x => x.TransactionTimeUtc);

            transactionByAccountViewModel.transactions = transactions;

            return View(transactionByAccountViewModel);
        }

        [HttpGet]
        public IActionResult TransactionsByAmount()
        {
            return View(new TransactionByAmountViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> TransactionsByAmount(TransactionByAmountViewModel transactionByAmountViewModel)
        {
            if (transactionByAmountViewModel.MaxAmount <= transactionByAmountViewModel.MinAmount)
                ModelState.AddModelError(nameof(transactionByAmountViewModel.MaxAmount), "Max Amount must be greater than Min Amount");

            if (!ModelState.IsValid)
                return View(transactionByAmountViewModel);

            var response = await Client.GetAsync($"/BankAPI/Admin/transactionsByAmount/{transactionByAmountViewModel.MinAmount}/{transactionByAmountViewModel.MaxAmount}");

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError(nameof(TransactionByAmountViewModel.MinAmount), "The Server enconter an error, please try again.");
                return View(transactionByAmountViewModel);
            }

            // deserialize response into a list of transactions and pass it to the view
            var result = await response.Content.ReadAsStringAsync();

            var transactions = JsonConvert.DeserializeObject<List<Transaction>>(result);

            transactions.OrderBy(x => x.TransactionTimeUtc);

            transactionByAmountViewModel.Transactions = transactions;

            return View(transactionByAmountViewModel);
        }

        [HttpGet]
        public IActionResult CustomerAccess()
        {
            return View(new CustomerAccessViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> CustomerAccess(CustomerAccessViewModel customerAccessViewModel)
        {
            if (!ModelState.IsValid)
                return View(customerAccessViewModel);

            var response = await Client.PutAsync($"/BankAPI/Admin/BlockCustomerByID/{customerAccessViewModel.CustomerID}", JsonContent.Create(customerAccessViewModel.LoginState));

            if (!response.IsSuccessStatusCode)
                customerAccessViewModel.Response = @"Customer was not found";
            else
                customerAccessViewModel.Response = "Customer access has been updated";

            return View(customerAccessViewModel);
        }

        [HttpGet]
        public IActionResult BillPayState()
        {
            return View(new BillPayStateViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> BillPayState(BillPayStateViewModel billPayStateViewModel)
        {
            if (!ModelState.IsValid)
                return View(billPayStateViewModel);

            var response = await Client.PutAsync($"/BankAPI/Admin/BlockBillPayByID/{billPayStateViewModel.BillPayID}/{billPayStateViewModel.BillPayState}", JsonContent.Create(billPayStateViewModel.BillPayState));

            if (!response.IsSuccessStatusCode)
                billPayStateViewModel.Response = @"BillPay was not found";
            else
                billPayStateViewModel.Response = "BillPay State has been updated";

            return View(billPayStateViewModel);

        }

        [HttpGet]
        public async Task<IActionResult> UpdateCustomerDetails(int? id, [Bind(nameof(Customer.CustomerID))] Customer customer)
        {
            // clear model errors as they will be manually validated below
            ModelState.Clear();

            // if coming from nav link return view
            if (id.HasValue && id.Value == 1)
                return View(new Customer());

            // Validate Customer ID
            if (customer == null || !Regex.IsMatch(customer.CustomerID.ToString(), @"^\d{4}$"))
            {
                ModelState.AddModelError(nameof(customer.CustomerID), "Customer ID must be a 4 digit number");
                return View(new Customer());
            }

            // get customer details from API
            var response = await Client.GetAsync($"/BankAPI/Admin/GetCustomerDetails/{customer.CustomerID}");

            // if no Customer found add error message and return view
            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError(nameof(customer.CustomerID), "Customer not found, please check the customer ID and try again");
                return View(customer);
            }

            // deserialize response and return view
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


            return View("UpdateCustomerDetailsSuccess", customer);
        }
    }
}