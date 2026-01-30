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
        #region incomes
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

        private Income newIncome = new Income { ReceivedDate = DateTime.Now };

        public Income NewIncome
        {
            get => newIncome;
            set 
            { 
                newIncome = value;
                RaisePropertyChanged(nameof(NewIncome));
            }
        }

        //Property för att bilda kommandot för att lägga till en inkomst
        public DelegateCommand AddIncomeCommand { get; }
        public DelegateCommand RemoveIncomeCommand { get; }

        private bool CanRemove(object parameter) => SelectedIncome is not null;


        private void RemoveIncome(object? paramenter)
        {
            if (SelectedIncome is not null)
            {
                var incomeToRemove = SelectedIncome;

                //Tar bort från databasen
                using (var context = new BudgetDbContext())
                {
                    context.Incomes.Attach(incomeToRemove);
                    context.Incomes.Remove(incomeToRemove);
                    context.SaveChanges();
                }

                //Tar bort från ObservableCollection för att uppdatera UI:t
                Incomes.Remove(SelectedIncome);
                //Uppdaterar prognosen efter att en inkomst har tagits bort
                CalculateTotalIncomePerMonth(SelectedMonth);
                CalculateMonthlyBalance(SelectedMonth);

            }

            
        }

        //Metod med logik för att lägga till en inkomst som kallas på från View
        private void AddIncome(object? parameter)
        {
            //Skapar en ny inkomst baserat på det som skrivs in i UI:t i parametern NewIncome
            Income incomeToAddToList = new Income { Amount = NewIncome.Amount, Category = NewIncome.Category, RecurrenceType = NewIncome.RecurrenceType, ReceivedDate = NewIncome.ReceivedDate };
            
            //Spara till databasen
            using (var context = new BudgetDbContext())
            {
                if(incomeToAddToList.Category != null)
                {
                    //Kopplar inkomsten till en befintlig kategori från databasen för att undvika duplicering
                    context.Categories.Attach(incomeToAddToList.Category);
                }
                context.Incomes.Add(incomeToAddToList);
                context.SaveChanges();
            }

            //Lägger till den nya inkomsten i ObservableCollection för att uppdatera UI:t
            Incomes.Add(incomeToAddToList);

            //Sätter den nyligen tillagda inkomsten som vald
            SelectedIncome = incomeToAddToList;


            NewIncome = new Income { ReceivedDate = DateTime.Now }; //Resetar NewIncome efter tillägg

            //Uppdaterar prognosen efter att en ny inkomst har lagts till
            CalculateTotalIncomePerMonth(SelectedMonth);
            CalculateMonthlyBalance(SelectedMonth);

        }
        #endregion  

        public MainViewModel()
        {

            CalculateTotalIncomePerMonth(SelectedMonth);
            CalculateTotalExpensePerMonth(SelectedMonth);
            CalculateMonthlyBalance(SelectedMonth);

            AddIncomeCommand = new DelegateCommand(AddIncome);
            RemoveIncomeCommand = new DelegateCommand(RemoveIncome, CanRemove);

            AddExpenseCommand = new DelegateCommand(AddExpense);
            RemoveExpenseCommand = new DelegateCommand(RemoveExpense, CanRemoveExpense);

            NewIncome = new Income { ReceivedDate = DateTime.Now };
            NewExpense = new Expense { ExpenseDate = DateTime.Now };
        }


        #region expenses
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

        private Expense newExpense;

        public Expense NewExpense
        {
            get  => newExpense; 
            set 
            { 
                newExpense = value; 
                RaisePropertyChanged(nameof(NewExpense));
            }
        }


        public void AddExpense(object? parameter)
        {
            Expense expenseToAddToList = new Expense { Amount = NewExpense.Amount, Category = NewExpense.Category, RecurrenceType = NewExpense.RecurrenceType, ExpenseDate = NewExpense.ExpenseDate};

            //Spara till databasen
            using (var context = new BudgetDbContext())
            {
                if (expenseToAddToList.Category != null)
                {
                    //Kopplar utgiften till en befintlig kategori från databasen för att undvika duplicering
                    context.Categories.Attach(expenseToAddToList.Category);
                }
                context.Expenses.Add(expenseToAddToList);
                context.SaveChanges();
            }

            Expenses.Add(expenseToAddToList);
            SelectedExpense = expenseToAddToList;
            NewExpense = new Expense { ExpenseDate = DateTime.Now }; //Resetar NewExpense efter tillägg
            CalculateTotalExpensePerMonth(SelectedMonth);
            CalculateMonthlyBalance(SelectedMonth);
        }

        private bool CanRemoveExpense(object parameter) => SelectedExpense is not null;


        private void RemoveExpense(object? paramenter)
        {
            if (SelectedExpense is not null)
            {
                var expenseToRemove = SelectedExpense;

                //Tar bort från databasen
                using (var context = new BudgetDbContext())
                {
                    context.Expenses.Attach(SelectedExpense);
                    context.Expenses.Remove(SelectedExpense);
                    context.SaveChanges();
                }

                //Tar bort från ObservableCollection för att uppdatera UI:t
                Expenses.Remove(expenseToRemove);

                //Uppdaterar prognosen efter att en utgift har tagits bort
                CalculateTotalExpensePerMonth(SelectedMonth);
                CalculateMonthlyBalance(SelectedMonth);
            }
            
        }
        #endregion

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

        #region prognos
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

        private decimal annualSalary;
        public decimal AnnualSalary
        {
            get => annualSalary;
            set 
            {
                annualSalary = value;
                RaisePropertyChanged(nameof(AnnualSalary));
                CalculateMonthlyBalance(SelectedMonth);
            }
        }

        private decimal annualWorkHours;

        public decimal AnnualWorkHours
        {
            get => annualWorkHours; 
            set 
            { 
                annualWorkHours = value;
                RaisePropertyChanged(nameof(AnnualWorkHours));
                CalculateMonthlyBalance(SelectedMonth);
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

        public void CalculateTotalIncomePerMonth(DateTime month)
        {
            decimal monthlySalary = 0;
            if (AnnualWorkHours > 0)
            {
                decimal hourlyRate = AnnualSalary / AnnualWorkHours;
                monthlySalary = hourlyRate * (AnnualWorkHours / 12m);
            }

            TotalIncome = monthlySalary;

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
                        
                        if (new DateTime(income.ReceivedDate.Year, income.ReceivedDate.Month, 1) <= new DateTime(month.Year, month.Month, 1))
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
                        if (new DateTime(expense.ExpenseDate.Year, expense.ExpenseDate.Month, 1) <= new DateTime(month.Year, month.Month, 1))
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
        #endregion


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
            foreach (var income in incomesFromDb)
            {
                Incomes.Add(income);
            }

            var expensesFromDb = await context.Expenses.Include(e => e.Category).ToListAsync(); //Inkluderar relaterade Category-objekt
            foreach (var expense in expensesFromDb)
            {
                Expenses.Add(expense);

            }
        }   
    }
}
