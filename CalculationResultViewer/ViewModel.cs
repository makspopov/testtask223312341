using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CalculationResultViewer
{
    internal class ViewModel : INotifyPropertyChanged
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
        public ObservableCollection<Result> Results { get; set; }

        public ViewModel()
        {
            var lst = new List<Result>();
            string FilePath = Path.Combine(AppContext.BaseDirectory, "4.mdb");
            string OledbConnectionString = string.Format("Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Jet OLEDB:Database Password=admin;Mode=ReadWrite|Share Deny None;Persist Security Info=False;", FilePath);
            int countRows = 0;
            using (OleDbConnection con = new OleDbConnection(OledbConnectionString))
            {
                con.Open();

                try
                {
                    OleDbCommand select = new OleDbCommand();
                    select.Connection = con;
                    select.CommandText = "Select count(*) From Results ";
                    OleDbDataReader reader = select.ExecuteReader();
                    reader.Read();
                    countRows = reader.GetInt32(0);
                }
                finally
                {
                    con.Close();
                }
            }

            if (countRows == 0)
            {
                //var dct = File.ReadAllLines(@"D:\Работа\source\NotepadClones\CalculationResultViewer\Input Data_ПО.csv")
                var dct = File.ReadAllLines(Path.Combine(AppContext.BaseDirectory, "Input Data_ПО.csv"))
                    .Select(x => x.Split(",")).ToDictionary(x => x[0], x => Double.Parse(x[5], CultureInfo.InvariantCulture));

                var cols = new List<string>() { "C01", "D01", "F01" }.Select(x => new { name = x, value = dct[x] }).ToList();
                var rows = new List<string>() { "E01", "G01", "H01" }.Select(x => new { name = x, value = dct[x] }).ToList();

                var res_rows = cols.SelectMany(col => rows.Select(row => new
                {
                    cells = col.name + "_" + row.name,
                    value = Math.Round(Math.Pow(2, Math.Round((col.value - row.value), 2)), 2)
                })).ToList();

                using (OleDbConnection con = new OleDbConnection(OledbConnectionString))
                {
                    con.Open();
                    try
                    {
                        res_rows.Select((v, i) => new { i, v.cells, v.value }).ToList().ForEach(x =>
                        {
                            OleDbCommand cmd = new OleDbCommand();
                            cmd.CommandType = CommandType.Text;
                            cmd.CommandText = "INSERT INTO results (id, cells, COL_VALUES) VALUES (@id, @cells, @value1);";
                            cmd.Parameters.AddWithValue("@id", x.i);
                            cmd.Parameters.AddWithValue("@cells", x.cells);
                            cmd.Parameters.AddWithValue("@value1", x.value);
                            cmd.Connection = con;
                            cmd.ExecuteNonQuery();
                        });
                    }
                    finally
                    {
                        con.Close();
                    }
                }
            }

            using (OleDbConnection con = new OleDbConnection(OledbConnectionString))
            {
                con.Open();

                try
                {
                    OleDbCommand select = new OleDbCommand();
                    select.Connection = con;
                    select.CommandText = "Select cells, COL_VALUES From Results ";
                    OleDbDataReader reader = select.ExecuteReader();
                    while (reader.Read())
                    {
                        lst.Add(new Result() { cells = reader.GetString(0), val = reader.GetDouble(1) });
                    }
                }
                finally
                {
                    con.Close();
                }
            }
            Results = new ObservableCollection<Result>(lst); 
        }

        
    }

    class Result
    {
        public string cells { get; set; }
        public double val { get; set; }
    }
}

public class RelayCommand : ICommand
{
    Action execute;
    Func<object?, bool>? canExecute;

    public event EventHandler? CanExecuteChanged
    {
        add { CommandManager.RequerySuggested += value; }
        remove { CommandManager.RequerySuggested -= value; }
    }

    public RelayCommand(Action execute, Func<object?, bool>? canExecute = null)
    {
        this.execute = execute;
        this.canExecute = canExecute;
    }

    public bool CanExecute(object? parameter)
    {
        return canExecute == null || canExecute(parameter);
    }

    public void Execute(object? parameter)
    {
        execute();
    }
}
