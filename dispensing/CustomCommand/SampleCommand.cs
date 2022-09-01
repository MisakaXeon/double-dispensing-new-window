using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace dispensing.CustomCommand
{
    public class SampleCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        public Action<object> ExectionAction { get; set; }

        public Func<object, bool> CanExecuteAction { get; set; }


        public SampleCommand(Func<object, bool> isEx, Action<object> ex)
        {
            this.ExectionAction = ex;
            this.CanExecuteAction = isEx;
        }

        public bool CanExecute(object parameter)
        {
            return this.CanExecuteAction(parameter);
        }

        public void Execute(object parameter)
        {
            this.ExectionAction(parameter);
        }
    }
}
