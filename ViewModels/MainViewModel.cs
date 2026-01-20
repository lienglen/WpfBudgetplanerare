using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfBudgetplanerare.Command;
using WpfBudgetplanerare.Models;

namespace WpfBudgetplanerare.ViewModels
{
    internal class MainViewModel : BaseViewModel //Implementerar för att View måste bli medveten om ändringar i ViewModel
    {
        //INCOMES
        //Skapar lista som visar inkomster och ger notiser när de ändras
        private ObservableCollection<Income> incomes = new();

        public ObservableCollection<Income> Incomes
        {
            get { return incomes; }
            set { incomes = value;
                RaisePropertyChanged(nameof(Incomes));
            }
        }

        private Income selectedIncome;

        public Income SelectedIncome
        {
            get { return selectedIncome; }
            set { selectedIncome = value;
                RaisePropertyChanged(nameof(SelectedIncome));
                RemoveIncomeCommand.RaiseCanExecuteChanged();
            }
        }

        //Property för att bilda kommandot för att lägga till en inkomst
        public DelegateCommand AddIncomeCommand { get; }
        public DelegateCommand RemoveIncomeCommand { get; }

        public MainViewModel() 
        {
            Incomes.Add(new Income { Amount = 5000, Category = new Category { Id = 1, Name = "Lön", Description = "Månadslön" }, IsRecurring = true, ReceivedDate = DateTime.Now });
            Incomes.Add(new Income { Amount = 200, Category = new Category { Id = 2, Name = "Gåva", Description = "Födelsedagspresent" }, IsRecurring = false, ReceivedDate = DateTime.Now });
            Incomes.Add(new Income { Amount = 150, Category = new Category { Id = 3, Name = "Sälj", Description = "Sålde gamla möbler" }, IsRecurring = false, ReceivedDate = DateTime.Now });

            Expenses.Add(new Expense { Amount = 3000, Category = new Category { Id = 1, Name = "Hyra", Description = "Månadshyra" }, IsRecurring = true, ExpenseDate = DateTime.Now });
            Expenses.Add(new Expense { Amount = 500, Category = new Category { Id = 2, Name = "Mat", Description = "Veckohandling" }, IsRecurring = false, ExpenseDate = DateTime.Now });
            Expenses.Add(new Expense { Amount = 200, Category = new Category { Id = 3, Name = "Transport", Description = "Busskort" }, IsRecurring = false, ExpenseDate = DateTime.Now });

            CalculateTotalIncome();
            CalculateTotalExpense();
            CalculateMonthlyBalance();

            AddIncomeCommand = new DelegateCommand(AddIncome);
            RemoveIncomeCommand = new DelegateCommand(RemoveIncome, CanRemove);

            AddExpenseCommand = new DelegateCommand(AddExpense);
            RemoveExpenseCommand = new DelegateCommand(RemoveExpense, CanRemoveExpense);

            
        }

        private bool CanRemove(object parameter) => SelectedIncome is not null;
        

        private void RemoveIncome(object? paramenter)
        {
            if (SelectedIncome is not null)
            {
                Incomes.Remove(SelectedIncome);
            }

            CalculateTotalIncome();
            CalculateMonthlyBalance();
        }


       

        //Metod med logik för att lägga till en inkomst som kallas på från View vid click event
        private void AddIncome(object? parameter)
        {
            Income income = new Income { Amount = 0, Category = new Category { Id = 0, Name = "Ny Kategori", Description = "" }, IsRecurring = false, ReceivedDate = System.DateTime.Now };
            Incomes.Add(income);
            SelectedIncome = income;
            CalculateTotalIncome();
            CalculateMonthlyBalance();

        }



        //EXPENSES
        private ObservableCollection<Expense> expenses = new();
        public DelegateCommand AddExpenseCommand { get; }
        public DelegateCommand RemoveExpenseCommand { get; }

        public ObservableCollection<Expense> Expenses
        {
            get { return expenses; }
            set { expenses = value;
            RaisePropertyChanged(nameof(Expenses));
            }
        }

        private Expense selectedExpense;

        public Expense SelectedExpense
        {
            get { return selectedExpense; }
            set { selectedExpense = value;
                RaisePropertyChanged(nameof(SelectedExpense));
                RemoveExpenseCommand.RaiseCanExecuteChanged();
            }
        }

        public void AddExpense(object? parameter)
        {
            Expense expense = new Expense { Amount = 0, Category = new Category { Id = 0, Name = "Ny Kategori", Description = "" }, IsRecurring = false, ExpenseDate = System.DateTime.Now };
            Expenses.Add(expense);
            SelectedExpense = expense;
            CalculateTotalExpense();
            CalculateMonthlyBalance();
        }

        private bool CanRemoveExpense(object parameter) => SelectedExpense is not null;


        private void RemoveExpense(object? paramenter)
        {
            if (SelectedExpense is not null)
            {
                Expenses.Remove(SelectedExpense);
            }
            CalculateTotalExpense();
            CalculateMonthlyBalance();
        }

        //CATEGORIES

        public ObservableCollection<Category> Categories { get; set; } = new ObservableCollection<Category>()
        {
            new Category { Id = 1, Name = "Lön", Description = "Månadslön" },
            new Category { Id = 2, Name = "Gåva", Description = "Födelsedagspresent" },
            new Category { Id = 3, Name = "Sälj", Description = "Sålde gamla möbler" },
            new Category { Id = 4, Name = "Hyra", Description = "Månadshyra" },
            new Category { Id = 5, Name = "Mat", Description = "Veckohandling" },
            new Category { Id = 6, Name = "Transport", Description = "Busskort" }
        };

        //PROGNOS

        private decimal totalIncome;
        public decimal TotalIncome {get => totalIncome;
            set 
            {
                totalIncome = value;
                RaisePropertyChanged(nameof(TotalIncome));
            }
        }
        private decimal totalExpense;
        public decimal TotalExpense {get => totalExpense;
            set 
            { 
                totalExpense = value;
                RaisePropertyChanged(nameof(TotalExpense));
            }
        }

        private decimal monthlyBalance;
        public decimal MonthlyBalance {get => monthlyBalance;
            set 
            {
                monthlyBalance = value;
                RaisePropertyChanged(nameof(MonthlyBalance));
            } 
        }

        public void CalculateTotalIncome()
        {
            TotalIncome = 0;

            foreach (var income in Incomes)
            {
                if (income.IsRecurring == true)
                {
                    TotalIncome += income.Amount;
                }
               
            }

            RaisePropertyChanged(nameof(TotalIncome));
        }

        public void CalculateTotalExpense()
        {
            TotalExpense = 0;
            foreach (var expense in Expenses)
            {
                TotalExpense += expense.Amount;
            }
            RaisePropertyChanged(nameof(TotalExpense));
        }

        public void CalculateMonthlyBalance()
        {
            MonthlyBalance = TotalIncome - TotalExpense;
            RaisePropertyChanged(nameof(MonthlyBalance));
        }

        //Filtrera på datum och "isReacurring"



        public async Task LoadAsync()
        {
            if (Incomes.Any())
            {
                return;
            }

            //var incomes = await 
            //foreach income in incomes
            //incoms.Add(income);

        }
    }
}
