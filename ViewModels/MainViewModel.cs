using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using System.Collections.ObjectModel;
using WpfBudgetplanerare.Command;
using WpfBudgetplanerare.Data;
using WpfBudgetplanerare.Models;
using WpfBudgetplanerare.Models.Enums;

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

        //Property för att välja månad i kalendern
        private DateTime selectedMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
        public DateTime SelectedMonth
        {
            get { return selectedMonth; }
            set
            {
                selectedMonth = value;
                RaisePropertyChanged(nameof(SelectedMonth));

                //Uppdaterar prognosen när månaden ändras
                CalculateMonthlyBalance(SelectedMonth);
            }
        }

        //Property för att bilda kommandot för att lägga till en inkomst
        public DelegateCommand AddIncomeCommand { get; }
        public DelegateCommand RemoveIncomeCommand { get; }

        public MainViewModel()
        {

            CalculateTotalIncomePerMonth(SelectedMonth);
            CalculateTotalExpensePerMonth(SelectedMonth);
            CalculateMonthlyBalance(SelectedMonth);

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

            CalculateTotalIncomePerMonth(SelectedMonth);
            CalculateMonthlyBalance(SelectedMonth);
        }

        //Metod med logik för att lägga till en inkomst som kallas på från View vid click event
        private void AddIncome(object? parameter)
        {
            Income income = new Income { Amount = 0, Category = new Category { CategoryId = 0, Name = "Ny Kategori" }, RecurrenceType = Recurrence.OneTime, ReceivedDate = System.DateTime.Now };
            Incomes.Add(income);
            SelectedIncome = income;
            CalculateTotalIncomePerMonth(SelectedMonth);
            CalculateMonthlyBalance(SelectedMonth);

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
            Expense expense = new Expense { Amount = 0, Category = new Category { CategoryId = 0, Name = "Ny Kategori" }, RecurrenceType = Recurrence.OneTime, ExpenseDate = System.DateTime.Now };
            Expenses.Add(expense);
            SelectedExpense = expense;
            CalculateTotalExpensePerMonth(SelectedMonth);
            CalculateMonthlyBalance(SelectedMonth);
        }

        private bool CanRemoveExpense(object parameter) => SelectedExpense is not null;


        private void RemoveExpense(object? paramenter)
        {
            if (SelectedExpense is not null)
            {
                Expenses.Remove(SelectedExpense);
            }
            CalculateTotalExpensePerMonth(SelectedMonth);
            CalculateMonthlyBalance(SelectedMonth);
        }

        //CATEGORIES

        private ObservableCollection<Category> categories = new ObservableCollection<Category>();
        public ObservableCollection<Category> Categories
        {
            get => categories;
            set
            {
                categories = value;
                RaisePropertyChanged(nameof(Categories));
            }

        }

        //PROGNOS

        public Array RecurrenceTypes => Enum.GetValues(typeof(Recurrence)); //Hämtar alla värden från enum Recurrence för att binda till ComboBox i View

        private decimal totalIncome;
        public decimal TotalIncome { get => totalIncome;
            set
            {
                totalIncome = value;
                RaisePropertyChanged(nameof(TotalIncome));
            }
        }
        private decimal totalExpense;
        public decimal TotalExpense { get => totalExpense;
            set
            {
                totalExpense = value;
                RaisePropertyChanged(nameof(TotalExpense));
            }
        }

        private decimal monthlyBalance;
        public decimal MonthlyBalance { get => monthlyBalance;
            set
            {
                monthlyBalance = value;
                RaisePropertyChanged(nameof(MonthlyBalance));
            }
        }

        public void CalculateTotalIncomePerMonth(DateTime month)
        {
            TotalIncome = 0;

            foreach (var income in Incomes)
            {
                switch (income.RecurrenceType)
                {
                    case Recurrence.OneTime:
                        if (income.ReceivedDate.Month == month.Month && income.ReceivedDate.Year == month.Year)
                        {
                            TotalIncome += income.Amount;
                        }
                        break;
                    case Recurrence.Monthly:
                        var startMonth = new DateTime(income.ReceivedDate.Year, income.ReceivedDate.Month, 1);
                        var selectedMonth = new DateTime(month.Year, month.Month, 1);

                        if (startMonth <= selectedMonth)
                        {
                            TotalIncome += income.Amount;
                        }
                        break;
                    case Recurrence.Yearly:
                        if (income.ReceivedDate <= new DateTime(month.Year, month.Month, 1))
                        {
                            TotalIncome += income.Amount / 12m; //12m för att få månadsbelopp i typen decimal
                        }
                        break;
                }

            }

            RaisePropertyChanged(nameof(TotalIncome));
        }

        public void CalculateTotalExpensePerMonth(DateTime month)
        {
            TotalExpense = 0;
            foreach (var expense in Expenses)
            {
                switch (expense.RecurrenceType)
                {
                    case Recurrence.OneTime:
                        if (expense.ExpenseDate.Month == month.Month && expense.ExpenseDate.Year == month.Year)
                        {
                            TotalExpense += expense.Amount;
                        }
                        break;
                    case Recurrence.Monthly:
                        var startMonth = new DateTime(expense.ExpenseDate.Year, expense.ExpenseDate.Month, 1);
                        var selectedMonth = new DateTime(month.Year, month.Month, 1);

                        if (startMonth <= selectedMonth)
                        {
                            TotalExpense += expense.Amount;
                        }
                        break;
                    case Recurrence.Yearly:
                        if (expense.ExpenseDate.Month == month.Month && expense.ExpenseDate.Year <= month.Year)
                        {
                            TotalExpense += expense.Amount;
                        }
                        break;
                }
            }

            RaisePropertyChanged(nameof(TotalExpense));
        }

        public void CalculateMonthlyBalance(DateTime month)
        {
            CalculateTotalIncomePerMonth(month);
            CalculateTotalExpensePerMonth(month);
            MonthlyBalance = TotalIncome - TotalExpense;
            RaisePropertyChanged(nameof(MonthlyBalance));
        }



        //Metod för att ladda data asynkront från databasen vid uppstart av applikationen
        public async Task LoadAsync()
        {
            using var context = new BudgetDbContext(); //Skapar en instans av DbContext för att interagera med databasen. Använder using för att objektet ska tas bort automatiskt efter användning.

            var categoriesFromDb = await context.Categories.ToListAsync();
            Categories.Clear(); //Rensar befintliga kategorier för att undvika dubbletter vid omladdning
            foreach (var category in categoriesFromDb)
            {
                Categories.Add(category);
            }

            var incomesFromDb = await context.Incomes.Include(i => i.Category).ToListAsync(); //Inkluderar relaterade Category-objekt
            //Incomes.Clear();
            foreach (var income in incomesFromDb)
            {
                Incomes.Add(income);
            }

            var expensesFromDb = await context.Expenses.Include(e => e.Category).ToListAsync(); //Inkluderar relaterade Category-objekt
            //Expenses.Clear();
            foreach (var expense in expensesFromDb)
            {
                Expenses.Add(expense);

            }
        }   
    }
}
