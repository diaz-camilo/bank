using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using utils.Enums;
using WebBanking.Models;
using WebBanking.Repository;

namespace WebBanking.Data
{
    public static class SeedData
    {
        public static void CreateCustomers(WebBankContext context)
        {
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
            context.SaveChanges();
        }

        public static void CreateAccounts(WebBankContext context)
        {
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
            context.SaveChanges();
        }

        public static void CreatePayees(WebBankContext context)
        {
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
                    PayeeID = 789,
                    Name = "Netflix Au",
                    Address = "123 Flinders st",
                    Postcode = "3000",
                    Suburb = "Melbourne",
                    State = "VIC",
                    Phone = "(03) 7878 1122"
                });
            context.SaveChanges();
        }

        public static void AddInitialTransactions(WebBankContext context)
        {
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
            context.SaveChanges();
        }

        public static void CreateUserLogin(IUserRepository userRepository)
        {
            userRepository.CreateUserAsync(new SignupUser
            {
                Name = "Matthew Bolger",
                LoginID = "12345678",
                CustomerID = 2100,
                password = "abc123"
            }).Wait();


            userRepository.CreateUserAsync(new SignupUser
            {
                Name = "Rodney Cocker",
                LoginID = "38074569",
                CustomerID = 2200,
                password = "ilovermit2020"
            }).Wait();


            userRepository.CreateUserAsync(new SignupUser
            {
                Name = "Shekhar Kalra",
                LoginID = "17963428",
                CustomerID = 2300,
                password = "youWill_n0tGuess-This!"
            }).Wait();


            // Admin
            userRepository.CreateAdminAsync(new SignupUser
            {
                Name = "admin",
                LoginID = "admin",
                password = "admin"
            }).Wait();
        }

        public static void AddTransactions(WebBankContext context)
        {
            for (int j = 0; j < 4; j++)
            {

                var accNum =
                    j == 0 ? 4100 :
                    j == 1 ? 4101 :
                    j == 2 ? 4200 :
                    4300;

                var randomNum = new Random();
                int randInt = randomNum.Next();
                var startDate = DateTime.UtcNow.AddDays(-30);
                var account = context.Account.Find(accNum);

                int i = 0;

                while (DateTime.UtcNow.AddDays(30) > startDate)
                {
                    startDate = startDate.AddHours(randomNum.Next(10, 24));
                    var amount = randomNum.Next(50, 500);
                    var typeDecider = randomNum.Next();
                    var type =
                        typeDecider % 5 == 0 ? TransactionType.Withdraw :
                        typeDecider % 11 == 0 ? TransactionType.OutgoingTransfer :
                        typeDecider % 17 == 0 ? TransactionType.BillPay :
                        TransactionType.Deposit;

                    if (type == TransactionType.Withdraw)
                    {
                        account.Transactions.Add(
                            new Transaction
                            {
                                TransactionTimeUtc = startDate,
                                Amount = amount,
                                TransactionType = type,
                            });
                        if (account.FreeTransactions < 1)
                        {
                            account.Transactions.Add(
                                new Transaction
                                {
                                    TransactionTimeUtc = startDate.AddMilliseconds(10),
                                    Amount = 0.10m,
                                    TransactionType = TransactionType.ServiceCharge,
                                    Comment = $"Withdraw Service Charge"
                                });
                        }
                        account.Balance -= 0.1m;
                        account.FreeTransactions--;
                    }
                    if (type == TransactionType.OutgoingTransfer)
                    {
                        if (account.FreeTransactions < 1)
                        {
                            account.Transactions.Add(
                                new Transaction
                                {
                                    TransactionTimeUtc = startDate.AddMilliseconds(10),
                                    Amount = 0.20m,
                                    TransactionType = TransactionType.ServiceCharge,
                                    Comment = $"Transfer Service Charge"
                                });
                            account.Balance -= 0.2m;
                        }
                        account.FreeTransactions--;

                        int accountDecider;
                        int destiAccount;
                        do
                        {
                            accountDecider = randomNum.Next(0, 999) % 4;
                            destiAccount =
                            accountDecider == 0 ? 4100 :
                            accountDecider == 1 ? 4101 :
                            accountDecider == 2 ? 4200 :
                            4300;

                            if (account.AccountNumber != destiAccount)
                            {


                                var destinationAccount = context.Account.Find(destiAccount);
                                destinationAccount.Transactions.Add(

                                    new Transaction
                                    {
                                        TransactionTimeUtc = startDate,
                                        Amount = amount,
                                        TransactionType = TransactionType.IncomingTransfer,

                                    });
                                destinationAccount.Balance += amount;

                                account.Transactions.Add(
                                    new Transaction
                                    {
                                        TransactionTimeUtc = startDate,
                                        Amount = amount,
                                        TransactionType = type,
                                        DestinationAccountNumber = destinationAccount.AccountNumber
                                    });
                            }


                        } while (account.AccountNumber == destiAccount);

                    }
                    if (type == TransactionType.BillPay)
                    {
                        account.Transactions.Add(
                                new Transaction
                                {
                                    TransactionTimeUtc = startDate,
                                    Amount = amount,
                                    TransactionType = type,
                                });
                        account.Balance -= amount;
                    }
                    if (type == TransactionType.Deposit)
                    {
                        account.Transactions.Add(
                            new Transaction
                            {
                                TransactionTimeUtc = startDate,
                                Amount = amount,
                                TransactionType = type,
                            });
                    }
                    account.Balance += type == TransactionType.Deposit ? amount : (-amount);
                    i++;
                    //_context.SaveChanges();
                }
            }

            context.SaveChanges();
        }
        
        public static void Initialize(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetRequiredService<WebBankContext>();

            var any = context.Customer.Any();

            // Look for customers.
            if (any)
                return; // DB has already been seeded.

            var userRepository = serviceProvider.GetRequiredService<IUserRepository>();

            userRepository.CreateRolesAsync().Wait();
            CreateCustomers(context);
            CreateAccounts(context);
            AddInitialTransactions(context);
            CreatePayees(context);
            AddTransactions(context);

            CreateUserLogin(userRepository);




            context.SaveChanges();

        }

    }
}
