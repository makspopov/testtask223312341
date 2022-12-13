using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CalculationResultViewer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void enterApp(object sender, RoutedEventArgs e)
        {
            try
            {
                var Login = tbLogin.Text;
                var Password = pbPassword.Password;
                var dct = new Dictionary<string, string>();
                string FilePath = System.IO.Path.Combine(AppContext.BaseDirectory, "4.mdb");
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
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Неверный пароль!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Попробуйте запустить приложение с правами администратора! " + ex.Message);
            }
        }
    }
}
