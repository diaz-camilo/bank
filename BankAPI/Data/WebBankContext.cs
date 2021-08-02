using System;
using Microsoft.EntityFrameworkCore;
using BankAPI.Models;

namespace BankAPI.Data
{

    // Context is the database.
    public class WebBankContext : DbContext
    {
        public WebBankContext(DbContextOptions<WebBankContext> options) : base(options)
        { }

        // properties to represent tables.
        public DbSet<Account> Account { get; set; }
        public DbSet<BillPay> BillPay { get; set; }
        public DbSet<Customer> Customer { get; set; }
        public DbSet<Login> Login { get; set; }
        public DbSet<Payee> Payee { get; set; }
        public DbSet<Transaction> Transaction { get; set; }
    }
}
