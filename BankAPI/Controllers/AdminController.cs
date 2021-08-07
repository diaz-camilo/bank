using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BankAPI.Models;
using BankAPI.Models.DataManagers;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Serialization;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BankAPI.Controllers
{


    [ApiController]
    [Route("BankAPI/[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly CustomerManager _repo;

        public AdminController(CustomerManager repo)
        {
            _repo = repo;
        }

        [HttpGet("transactions/{accountNum}")]
        public IEnumerable<Transaction> GetTransactions(int accountNum)
        {
            return _repo.GetTransactionsByDate(accountNum);
        }


        [HttpGet("transactionsByDate/{AccountNum}/{start}/{end}")]
        public IEnumerable<Transaction> GetTransactions(int accountNum, string start, string end)
        {
            var startDateArray = start.Split('-');
            var endDateArray = end.Split('-');
            int startYear, startMonth,startDay, endYear,endMonth, endDay;

            Int32.TryParse(startDateArray[0], out startYear);
            Int32.TryParse(startDateArray[1], out startMonth);
            Int32.TryParse(startDateArray[2], out startDay);
            Int32.TryParse(endDateArray[0], out endYear);
            Int32.TryParse(endDateArray[1], out endMonth);
            Int32.TryParse(endDateArray[2], out endDay);
            endDay++; // to make end day inclusive

            var startDate = new DateTime(startYear,startMonth,startDay);
            var endDate = new DateTime(endYear,endMonth,endDay);

            return _repo.GetTransactionsByDate(accountNum, startDate, endDate);
        }

        [HttpGet("transactionsByAmount/{min}/{max}")]
        public IEnumerable<Transaction> GetTransactions(decimal min, decimal max)
        {
            return _repo.GetTransactionsByAmount(min, max);
        }

        [HttpGet("GetCustomerDetails/{customerID}")]
        public Customer GetCustomer(int customerID)
        {
            return _repo.GetCustomerDetails(customerID);
        }

        [HttpPut("UpdateCustomerDetails/{customerID}")]
        public bool PutCustomer(int customerID, [FromBody] Customer customer)
        {
            return _repo.UdtateCustomer(customer);
        }

        [HttpPut("BlockCustomerByID/{customerID}")]
        public bool PutCustomerAccess(int customerID,[FromBody] bool access)
        {
            return _repo.UpdateCustomerAccess(customerID, access);
        }

        [HttpPut("BlockBillPayByID/{billID}/{state}")]
        public bool PutBillPayState(int billID, bool state)
        {
            return _repo.UpdateBillPayState(billID, state);
        }
    }
}
