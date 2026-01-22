using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfBudgetplanerare.Models;
using WpfBudgetplanerare.Models.Enums;

namespace WpfBudgetplanerare.Data
{
    public class BudgetDbContext : DbContext
    {
        public DbSet<Income> Incomes { get; set; }
        public DbSet<Expense> Expenses { get; set; }
        public DbSet<Category> Categories { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=BudgetPlannerDb;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False");
        }

        //Seedar in kategorier
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>().HasData(
                new Category { CategoryId = 1, Name = "Housing" },
                new Category { CategoryId = 2, Name = "Food" },
                new Category { CategoryId = 3, Name = "Transportation" },
                new Category { CategoryId = 4, Name = "Entertainment" },
                new Category { CategoryId = 5, Name = "Utilities" },
                new Category { CategoryId = 6, Name = "Salery"},
                new Category { CategoryId = 7, Name = "Gift"}
            );

            modelBuilder.Entity<Income>().HasData(
            new Income { IncomeId = -1, Amount = 5000, CategoryId = 6, RecurrenceType = Recurrence.Monthly, ReceivedDate = new DateTime(2026,1,1) },
            new Income { IncomeId = -2, Amount = 200, CategoryId = 7, RecurrenceType = Recurrence.OneTime, ReceivedDate = new DateTime(2026,1,1)}
            );

            modelBuilder.Entity<Expense>().HasData(
            new Expense { ExpenseId = -1, Amount = 1500, CategoryId = 1, RecurrenceType = Recurrence.Monthly, ExpenseDate = new DateTime(2026,1,1) },
            new Expense { ExpenseId = -2, Amount = 300, CategoryId = 2, RecurrenceType = Recurrence.Monthly, ExpenseDate = new DateTime(2026,1,1) }
            );

        }
    }
}
