using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using WebBanking.Models;

namespace WebBanking.Data
{
    public static class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using var context = new WebBankContext(serviceProvider.GetRequiredService<DbContextOptions<WebBankContext>>());

            // Look for customers.
            if (context.Customer.Any())
                return; // DB has already been seeded.

            // Stages changes
            context.Customer.AddRange(
                new Customer
                {
                    CustomerID = 2100,
                    Name = "Matthew Bolger",
                    TFN = "123456789",
                    Address = "123 Fake Street",
                    Suburb = "Melbourne",
                    Postcode = "3000",
                    Mobile = "0404123456"
                },
                new Customer
                {
                    CustomerID = 2200,
                    Name = "Rodney Cocker",
                    Address = "456 Real Road",
                    Suburb = "Melbourne",
                    Postcode = "3005"
                },
                new Customer
                {
                    CustomerID = 2300,
                    Name = "Shekhar Kalra"
                });

            context.Login.AddRange(
                new Login
                {
                    LoginID = "12345678",
                    CustomerID = 2100,
                    PasswordHash = "YBNbEL4Lk8yMEWxiKkGBeoILHTU7WZ9n8jJSy8TNx0DAzNEFVsIVNRktiQV+I8d2"
                },
                new Login
                {
                    LoginID = "38074569",
                    CustomerID = 2200,
                    PasswordHash = "EehwB3qMkWImf/fQPlhcka6pBMZBLlPWyiDW6NLkAh4ZFu2KNDQKONxElNsg7V04"
                },
                new Login
                {
                    LoginID = "17963428",
                    CustomerID = 2300,
                    PasswordHash = "LuiVJWbY4A3y1SilhMU5P00K54cGEvClx5Y+xWHq7VpyIUe5fe7m+WeI0iwid7GE"
                });

            context.Account.AddRange(
                new Account
                {
                    AccountNumber = 4100,
                    Type = AccountType.Savings,
                    CustomerID = 2100,
                    Balance = 100,
                    FreeTransactions = 4
                },
                new Account
                {
                    AccountNumber = 4101,
                    Type = AccountType.Checking,
                    CustomerID = 2100,
                    Balance = 500,
                    FreeTransactions = 4
                },
                new Account
                {
                    AccountNumber = 4200,
                    Type = AccountType.Savings,
                    CustomerID = 2200,
                    Balance = 500.95m,
                    FreeTransactions = 4
                },
                new Account
                {
                    AccountNumber = 4300,
                    Type = AccountType.Checking,
                    CustomerID = 2300,
                    Balance = 1250.50m,
                    FreeTransactions = 4
                });

            const string openingBalance = "Opening balance";
            const string format = "dd/MM/yyyy hh:mm:ss tt";
            context.Transaction.AddRange(
                new Transaction
                {
                    TransactionType = TransactionType.Deposit,
                    AccountNumber = 4100,
                    Amount = 100,
                    Comment = openingBalance,
                    TransactionTimeUtc = DateTime.ParseExact("19/12/2019 08:00:00 PM", format, null)
                },
                new Transaction
                {
                    TransactionType = TransactionType.Deposit,
                    AccountNumber = 4101,
                    Amount = 500,
                    Comment = openingBalance,
                    TransactionTimeUtc = DateTime.ParseExact("19/12/2019 08:30:00 PM", format, null)
                },
                new Transaction
                {
                    TransactionType = TransactionType.Deposit,
                    AccountNumber = 4200,
                    Amount = 500.95m,
                    Comment = openingBalance,
                    TransactionTimeUtc = DateTime.ParseExact("19/12/2019 09:00:00 PM", format, null)
                },
                new Transaction
                {
                    TransactionType = TransactionType.Deposit,
                    AccountNumber = 4300,
                    Amount = 1250.50m,
                    Comment = openingBalance,
                    TransactionTimeUtc = DateTime.ParseExact("19/12/2019 10:00:00 PM", format, null)
                });
            context.Payee.AddRange(
                new Payee
                {
                    PayeeID = 123,
                    Name = "Telstra",
                    Address = "123 Swanston st",
                    Postcode = "3000",
                    Suburb = "Melbourne",
                    State = "VIC",
                    Phone = "(03) 9876 5432"
                },
                new Payee
                {
                    PayeeID = 456,
                    Name = "Origin",
                    Address = "123 Lonsdale st",
                    Postcode = "3000",
                    Suburb = "Melbourne",
                    State = "VIC",
                    Phone = "(03) 6547 5432"
                },
                new Payee
                {
                    PayeeID = 123,
                    Name = "Netflix Au",
                    Address = "123 Flinders st",
                    Postcode = "3000",
                    Suburb = "Melbourne",
                    State = "VIC",
                    Phone = "(03) 7878 1122"
                }
                );
            // Commits changes
            context.SaveChanges();
        }
    }
}
