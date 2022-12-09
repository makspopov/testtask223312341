// See https://aka.ms/new-console-template for more information
//Console.WriteLine("Hello, World!");


//using System.Data;
//using System.Data.SqlClient;

//System.Data.SqlClient.SqlConnection con;
//con = new System.Data.SqlClient.SqlConnection();
//con.ConnectionString = $@"Data Source=(localdb)\mssqllocaldb;
//                          AttachDbFilename=D:\db.mdf;
//                          Database=calculationsdb;
//                          Integrated Security=True;
//                          Connect Timeout=30;";
//con.Open();
//Console.WriteLine("Connection opened");
////SqlCommand command = new SqlCommand();
////command.CommandText = "SELECT * FROM Users";
////command.Connection = con;

//SqlDataAdapter adapter = new SqlDataAdapter("select cells from Results", con);
//// Создаем объект Dataset
//DataSet ds = new DataSet();
//// Заполняем Dataset
//adapter.Fill(ds);

//ds.Tables[0].AsEnumerable().ToList().ForEach(x => Console.WriteLine(x[0].ToString()));


////command.Exec
//con.Close();
//Console.WriteLine("Connection closed");



using System.Data.OleDb;

string FilePath = "C:\\Users\\max35\\Documents\\Тестовое тз\\1\\4.mdb"; 
string OledbConnectionString = string.Format("Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Jet OLEDB:Database Password=admin;Mode=ReadWrite|Share Deny None;Persist Security Info=False;", FilePath);
using (OleDbConnection con = new OleDbConnection(OledbConnectionString))
{
    con.Open();

    

    //string sql = string.Empty;
    //sql = string.Format("ALTER DATABASE PASSWORD 'Admin' '1' ");
    //OleDbCommand cmd = new OleDbCommand(sql, con);

    
    try
    {
        OleDbCommand select = new OleDbCommand();
        select.Connection = con;
        select.CommandText = "Select cells From Results ";
        OleDbDataReader reader = select.ExecuteReader();
        while (reader.Read())
        {
            //listBox1.Items.Add(reader[1].ToString() + "," + reader[2].ToString());
            Console.WriteLine(reader.GetString(0));
        }
    }
    finally
    {
        con.Close();
    }   
}