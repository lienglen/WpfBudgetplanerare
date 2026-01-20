using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfBudgetplanerare.Models.Enums;

namespace WpfBudgetplanerare.Models
{
    public class Expense
    {
        public decimal Amount { get; set; }
        public Category Category { get; set; }
        public Recurrence RecurrenceType  { get; set; } = Recurrence.OneTime;
        public DateTime ExpenseDate { get; set; }

    }
}
