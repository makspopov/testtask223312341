using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace CalculationResultViewer
{
    internal class EnterViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private string logPath;
        protected virtual void OnPropertyChanged([CallerMemberName] string PropertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
        }
        protected virtual bool Set<T>(ref T field, T value, [CallerMemberName] string PropertyName = null)
        {
            if (Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(PropertyName);
            return true;
        }
        public string Login { get; set; }
        public string Password { get; set; }

        public ICommand LoginCommand { get => new HelperCommand(RegisterUser, CanRegister); }

        private void RegisterUser(object parameter)
        {
            var dct = new Dictionary<string, string>();
            string FilePath = Path.Combine(AppContext.BaseDirectory, "4.mdb");
            string OledbConnectionString = string.Format("Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Jet OLEDB:Database Password=admin;Mode=ReadWrite|Share Deny None;Persist Security Info=False;", FilePath);
            using (OleDbConnection con = new OleDbConnection(OledbConnectionString))
            {
                con.Open();

                try
                {
                    OleDbCommand select = new OleDbCommand();
                    select.Connection = con;
                    select.CommandText = "Select col_login, col_password From users ";
                    OleDbDataReader reader = select.ExecuteReader();
                    while (reader.Read())
                    {
                        dct.Add(reader.GetString(0), reader.GetString(1));
                    }
                }
                finally
                {
                    con.Close();
                }
            }
            if (dct.ContainsKey(Login) && dct[Login] == Password)
            {
                MainView mainView = new MainView();
                mainView.Show();
                Window wnd = parameter as Window;
                wnd?.Close();
            }
            else
            {
                MessageBox.Show("Неверный пароль!");
            }            
        }

        private bool CanRegister(object parameter) => true;

        public class HelperCommand : ICommand
        {
            private readonly Action<object> _execute;
            private readonly Predicate<object> _canExecute;
            public HelperCommand(Action<object> execute) : this(execute, canExecute: null) { }
            public HelperCommand(Action<object> execute, Predicate<object> canExecute)
            {
                if (execute == null) throw new ArgumentNullException("execute");
                this._execute = execute;
                this._canExecute = canExecute;
            }
            public event EventHandler CanExecuteChanged;
            public bool CanExecute(object parameter) => this._canExecute == null ? true : this._canExecute(parameter);
            public void Execute(object parameter) => this._execute(parameter);
            public void RaiseCanExecuteChanged() => this.CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
