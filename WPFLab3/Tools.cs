using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace WPFLab3
{
	public class DelegateCommand : ICommand
	{
		private Action execute;
		private Func<object, bool> canExecute;

		public event EventHandler CanExecuteChanged
		{
			add { CommandManager.RequerySuggested += value; }
			remove { CommandManager.RequerySuggested -= value; }
		}

		public DelegateCommand(Action execute, Func<object, bool> canExecute = null)
		{
			this.execute = execute;
			this.canExecute = canExecute;
		}

		public bool CanExecute(object parameter)
		{
			return canExecute == null || this.canExecute(parameter);
		}

		public void Execute(object parameter)
		{
			this.execute.Invoke();
		}
	}
}
