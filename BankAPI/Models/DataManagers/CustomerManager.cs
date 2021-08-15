using System;
using System.Collections.Generic;
using System.Linq;
using BankAPI.Data;
using Microsoft.AspNetCore.Localization;
using utils.Enums;

namespace BankAPI.Models.DataManagers
{
    public class CustomerManager
    {
        private readonly WebBankContext _context;

        public CustomerManager(WebBankContext context)
        {
            _context = context;
        }

        public IEnumerable<Transaction> GetTransactionsByDate(int accountNum, DateTime? start = null, DateTime? end = null)
        {
            if (start == null && end == null)
                return _context.Transaction.Where(x => x.AccountNumber == accountNum);
            if (start == null && end != null)
                return _context.Transaction.Where(x => x.AccountNumber == accountNum && x.TransactionTimeUtc <= end);
            if (start != null && end == null)
                return _context.Transaction.Where(x => x.AccountNumber == accountNum && x.TransactionTimeUtc >= start);

            return _context.Transaction.Where(x => x.AccountNumber == accountNum && x.TransactionTimeUtc >= start && x.TransactionTimeUtc <= end);
        }

        public IEnumerable<Transaction> GetTransactionsByAmount(decimal min, decimal max)
        {
            return _context.Transaction.Where(x => x.Amount >= min && x.Amount <= max);
        }

        public bool UdtateCustomer(Customer customer)
        {
            _context.Customer.Update(customer);
            _context.SaveChanges();

            return true;
        }

        public bool UpdateCustomerAccess(int customerID, bool isLocked)
        {
           
            var customer = _context.Customer.Find(customerID);
                customer.ID.IsLocked = isLocked;

            _context.SaveChanges();

            return true;
        }

        public bool UpdateBillPayState(int billID, bool state)
        {
            _context.BillPay.Find(billID).State = state == true ? State.blocked : State.active;
            _context.SaveChanges();

            return true;
        }

        public Customer GetCustomerDetails(int customerID)
        {
            var customer = _context.Customer.Find(customerID);
            if (customer == null)
                throw new Exception("Customer not found");

            return customer;
        }
    }
}
