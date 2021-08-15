# s3820251-a2

## Record Type

the record type was used in the transaction class. it offers the benefit of preventing easy modification of data by mistake. since transactions record the movement of money of each bank account and they not change once set, I believe is a perfect use case for record type.

## ASP.NET core Identity

Identity was implemented in both customer project and Admin project for the HD part of the Assignment

## Charts

Charts were implemented in the Home/Index page of the customer website as a form of dashboard.

one represents the amount of money that came in and out of the Customers primary account.

The second one, represent the amount of ach transactions.

Both use the transactions for the previous 14 days and use the Google Charts API to dinamicaly prodice the charts.

# Note

I Have intentionally set up the background service to record fail transactions in the users statements as a transaction with a value of $0. this is not a bug and can be easly removed if this would affect my marks.

Please refere to BillPayBackgroundService.cs line 66

## Camilo Diaz - s3820251