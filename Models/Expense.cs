using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfBudgetplanerare.Models.Enums;
using WpfBudgetplanerare.ViewModels;

namespace WpfBudgetplanerare.Models
{
    public class Expense : BaseViewModel
    {
        public int ExpenseId { get; set; }
        public decimal Amount { get; set; }
        public Recurrence RecurrenceType  { get; set; } = Recurrence.OneTime;
        public DateTime ExpenseDate { get; set; }

        public int CategoryId { get; set; } // För att EF ska fatta relationen
        //Måste implementera INotifyPropertyChanged för att kunna uppdatera UI när Category ändras
        private Category _category;
        public Category Category { get => _category;
            set
            {
                _category = value;
                RaisePropertyChanged(nameof(Category));
                
            }
                
        }

    }
}
