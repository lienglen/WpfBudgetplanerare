using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace WpfBudgetplanerare.Command
{
    //Frikopplad klass som kan återanvändas för att skapa kommandon i ViewModels i andra projekt också
    //ICommand används för att bestämma om knappar ska vara aktiva eller inaktiva beroende på logik i ViewModel
    public class DelegateCommand : ICommand
    {
        private readonly Action<object?> execute;
        private readonly Func<object, bool>? canExecute;

        public event EventHandler? CanExecuteChanged;

        //Metod för att uppdatera CanExecute när något ändras som påverkar om kommandot kan köras eller inte
        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        // Konstruktor för DelegateCommand för att initiera execute och canExecute
        public DelegateCommand(Action<object?> execute, Func<object, bool>? canExecute=null)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }

        //Metod för att kolla om kommandot kan execute innan metoden Execute kan anropas från View
        public bool CanExecute(object? parameter)
        {
            if (canExecute == null)
                { 
                    return true;
                }
            else
                {
                    return canExecute(parameter!);
                }
        }

        //Metod för att execute när kommandot anropas från View
        public void Execute(object? parameter)
        {
            execute(parameter);
        }
    }
}
