using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfBudgetplanerare.Models.Enums;

namespace WpfBudgetplanerare.Models
{
    public class Income
    {
        public decimal Amount { get; set; }
        public Category Category  { get; set; }
        public Recurrence RecurrenceType { get; set; }
        public DateTime ReceivedDate { get; set; }

        //Skapar en metod för att visa inkomster i textform
        //public override string ToString()
        //{
        //    return $"{Amount} - {Category.Name} - {(IsRecurring ? "Recurring" : "One-time")} - {ReceivedDate.ToShortDateString()}";
        //}
    }
}
