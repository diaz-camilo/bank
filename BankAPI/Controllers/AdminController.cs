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


        /*
         returns all available transactions of a given account number.
        if account number does not exist.returns an empty collection.

        Parameters: int accountNum, the account number.

        Return: an unordered Collection of transactions.
         */
        [HttpGet("transactions/{accountNum}")]
        public IEnumerable<Transaction> GetTransactions(int accountNum)
        {
            return _repo.GetTransactionsByDate(accountNum);
        }

        /*
         returns all available transactions of a given account number
        within the start date and end date (inclusive) range.
        start date and end date are required.
        if account number does not exist, returns an empty collection.

        Parameters:
        int accountNum, the account number.
        string start, the date of the earliest transaction to include.
        string end, the date of the last transaction to include
        Note: date format is yyyy-mm-dd eg. 2020-05-22

        Return: an unordered Collection of transactions
         */
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

            // if start date is later than end date.
            if (DateTime.Compare(startDate, endDate) > 0)
                throw new Exception();

            return _repo.GetTransactionsByDate(accountNum, startDate, endDate);
        }

        /*
         Gets all the transactions from all customers registered in the system
        that are within the given brackets for minimum and maximum amounts.
        minimum amount must be greater or equal to max amount.

        Parameters:

        decimal min: the lowest amount in the range.
        decimal max: the highest amount in the range.

        Return: an unordered Collection of transactions
         */
        [HttpGet("transactionsByAmount/{min}/{max}")]
        public IEnumerable<Transaction> GetTransactions(decimal min, decimal max)
        {

            if (min > max)
                throw new Exception();

            return _repo.GetTransactionsByAmount(min, max);
        }

        /*
         gets all the customer details of a customer with the given Customer ID

        Parameters:
        int customerID, the customer id

        Return: the customer's details
         */
        [HttpGet("GetCustomerDetails/{customerID}")]
        public Customer GetCustomer(int customerID)
        {
            return _repo.GetCustomerDetails(customerID);
        }

        /*
         Updates the customer's details of a given customer provided that the
        model is valid and the customer exists.

        Parameter:
        Customer customer, a valid customer object.

        Returns:
        true if customer details were updated.
         */
        [HttpPut("UpdateCustomerDetails")]
        public bool PutCustomer([FromBody] Customer customer)
        {
            if (!ModelState.IsValid)
                throw new Exception();

            return _repo.UdtateCustomer(customer);
        }

        /*
         sets whether a customer can or can not login into the application

        Parameters:
        int customerID, the id of the customer.
        bool isLocked, true to lockout the user, false to allow the user.

        Returns:
        true if the operation was successful.
         */
        [HttpPut("BlockCustomerByID/{customerID}")]
        public bool PutCustomerAccess(int customerID,[FromBody] bool isLocked)
        {
            return _repo.UpdateCustomerAccess(customerID, isLocked);
        }


        /*
         Blocks or unblocks a bill with the given billPay ID.

        Parameters:
        int billID, the id of the bill
        bool isBlocked, true to set the bill's state as blocked, false to set the bill's state as active

        Returns:
        true if the operation was successful.
         */
        [HttpPut("BlockBillPayByID/{billID}/{state}")]
        public bool PutBillPayState(int billID, bool state)
        {
            return _repo.UpdateBillPayState(billID, state);
        }
    }
}
