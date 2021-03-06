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
using utils.Enums;
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
            

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await ProcessBillPays(stoppingToken);
                }
                catch (Exception ex)
                {

                }

                _logger.LogInformation($"Next payments will run at {DateTime.UtcNow.AddMinutes(1).ToLocalTime().ToShortTimeString()}");
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }

        private async Task ProcessBillPays(CancellationToken stoppingToken)
        {
            
            using var scope = _services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<WebBankContext>();

            
            // get all active bills
            var timeNow = DateTime.UtcNow.ToLocalTime();
            var BillPays = context.BillPay.
                Where(bill => bill.ScheduleTimeUtc < timeNow).
                Where(bill => bill.State == State.active).
                ToList();
            
            _logger.LogInformation($"Bills to process {BillPays.Count}");

            foreach (var bill in BillPays)
            {
                
                var availableBalance = bill.Account.Balance - (bill.Account.Type == AccountType.Checking ? 200 : 0);
                if (availableBalance < bill.Amount)
                {
                    bill.State = State.failed;

                    // Adds a transaction with $0 amount to the statements to keep a record of when the transaction failed for future references.
                    bill.Account.Transactions.Add(
                    new Transaction
                    {
                        Amount = 0,
                        Comment = $"Bill - {bill.Payee.Name} failed",
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
                            Comment = $"Bill - {bill.Payee.Name} paid",
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
            

            await context.SaveChangesAsync(stoppingToken);

        }

        


    }
}
