using AdminPortal.Models;


namespace AdminPortal.ViewModels
{
    public class IndexViewModel
    {
        public Customer Customer { get; set; }

        public TransactionByAccountViewModel TransactionByAccountViewModel { get; set; }

        public TransactionByAmountViewModel TransactionByAmountViewModel { get; set; }

        public CustomerAccessViewModel CustomerAccessViewModel { get; set; }

        public BillPayStateViewModel BillPayStateViewModel { get; set; }
    }
}
