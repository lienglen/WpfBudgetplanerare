using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfBudgetplanerare.Models
{
    public class Income
    {
        public decimal Amount { get; set; }
        public Category Category  { get; set; }
        public bool IsRecurring { get; set; }
        public DateTime ReceivedDate { get; set; }

        //Skapar en metod för att visa inkomster i textform
        //public override string ToString()
        //{
        //    return $"{Amount} - {Category.Name} - {(IsRecurring ? "Recurring" : "One-time")} - {ReceivedDate.ToShortDateString()}";
        //}
    }
}
