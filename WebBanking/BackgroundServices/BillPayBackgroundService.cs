using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using WebBanking.Data;
using WebBanking.Models;

namespace WebBanking.BackgroundServices
{
    public class BillPayBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _services;
        private readonly ILogger<BillPayBackgroundService> _logger;

        public BillPayBackgroundService(IServiceProvider services, ILogger<BillPayBackgroundService> logger)
        {
            _logger = logger;
            _services = services;

        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("BillPay Bg services is running");

            while (!stoppingToken.IsCancellationRequested)
            {
                await ProcessBillPays(stoppingToken);

                _logger.LogInformation($"Next payments will run at {DateTime.UtcNow.AddMinutes(1).ToLocalTime().ToShortTimeString()}");
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }

        private async Task ProcessBillPays(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Bg service is working");

            using var scope = _services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<WebBankContext>();

            // here you do the work that needs to be done

            var timeNow = DateTime.UtcNow.ToLocalTime();
            var BillPays = context.BillPay.
                Where(bill => bill.ScheduleTimeUtc < timeNow).
                Where(bill => bill.State != State.failed).
                ToList();
            
            _logger.LogInformation($"Bills to pay {BillPays.Count}");

            foreach (var bill in BillPays)
            {
                _logger.LogInformation($"{bill.BillPayID} {bill.Payee.Name} - {bill.ScheduleTimeUtc.ToShortTimeString()}");
                var availableBalance = bill.Account.Balance - (bill.Account.Type == AccountType.Checking ? 200 : 0);
                if (availableBalance < bill.Amount)
                {
                    bill.State = State.failed;
                    bill.Account.Transactions.Add(
                    new Transaction
                    {
                        Amount = 0,
                        Comment = $"Your scheduled payment for {bill.Payee.Name} failed (Insuficient Funds)",
                        TransactionTimeUtc = DateTime.UtcNow,
                        TransactionType = TransactionType.BillPay
                    });
                }
                else
                {
                    bill.Account.Balance -= bill.Amount;
                    bill.Account.Transactions.Add(
                        new Transaction
                        {
                            Amount = bill.Amount,
                            Comment = $"Your scheduled payment for {bill.Payee.Name} has been processed",
                            TransactionTimeUtc = DateTime.UtcNow,
                            TransactionType = TransactionType.BillPay
                        });
                    switch (bill.Period)
                    {
                        case Period.Annually:
                            bill.ScheduleTimeUtc = bill.ScheduleTimeUtc.AddYears(1);
                            break;
                        case Period.Quarterly:
                            bill.ScheduleTimeUtc = bill.ScheduleTimeUtc.AddMonths(3);
                            break;
                        case Period.Monthly:
                            bill.ScheduleTimeUtc = bill.ScheduleTimeUtc.AddMonths(1);
                            break;
                        case Period.Once:
                            context.BillPay.Remove(bill);
                            break;
                        default:
                            _logger.LogError($"Bill with ID {bill.BillPayID} has no period set, Therefore it failed");
                            break;
                    }
                }

            }



            // here you do the work that needs to be done

            await context.SaveChangesAsync(stoppingToken);

            _logger.LogInformation("this is the result of the Bg service");

        }

        private List<BillPay> GetBillPaysDue(WebBankContext context) =>
            context.BillPay.Where(bill => DateTime.UtcNow.CompareTo(bill.ScheduleTimeUtc) > 0).
            //Where(bill => bill.State != State.failed).
            ToList();


    }
}
