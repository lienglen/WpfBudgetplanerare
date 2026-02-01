using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace WpfBudgetplanerare.ViewModels
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        //Genom att implemnetera INotifyPropertyChanged får UI vet när properties ändras i ViewModel så att man slipper att uppdatera manuellt
        public event PropertyChangedEventHandler? PropertyChanged;
        public void RaisePropertyChanged([CallerMemberName]string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
