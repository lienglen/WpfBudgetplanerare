using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfBudgetplanerare.Models
{
    public class Expense
    {
        public decimal Amount { get; set; }
        public Category Category { get; set; }
        public bool IsRecurring { get; set; }
        public DateTime ExpenseDate { get; set; }

    }
}
