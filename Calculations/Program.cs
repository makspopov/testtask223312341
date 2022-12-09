// See https://aka.ms/new-console-template for more information
//Console.WriteLine("Hello, World!");

using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Data.OleDb;
using System.Data;
//using ADOX;

var dct = File.ReadAllLines(@"D:\Работа\source\NotepadClones\CalculationResultViewer\Input Data_ПО.csv")
    .Select(x => x.Split(",")).ToDictionary(x => x[0], x => Double.Parse(x[5], CultureInfo.InvariantCulture));

var cols = new List<string>() { "C01", "D01", "F01" }.Select(x => new { name = x, value = dct[x] }).ToList();
var rows = new List<string>() { "E01", "G01", "H01" }.Select(x => new { name = x, value = dct[x] }).ToList();

var res_rows = cols.SelectMany(col => rows.Select(row => new
{
    cells = col.name + "_" + row.name,
    value = Math.Round(Math.Pow(2, Math.Round((col.value - row.value), 2)), 2)
})).ToList();
//res_rows.ForEach(x => Console.WriteLine($"{x.cells} - {x.value}"));


//if (File.Exists("Test.sdf"))
//    File.Delete("Test.sdf");

//string connStr = "Data Source = Test.sdf; Password = <password>";

//SqlCeEngine engine = new SqlCeEngine(connStr);
//engine.CreateDatabase();
//engine.Dispose();

//SqlCeConnection conn = null;

//try
//{
//    conn = new SqlCeConnection(connStr);
//    conn.Open();

//    SqlCeCommand cmd = conn.CreateCommand();
//    cmd.CommandText = "CREATE TABLE myTable (col1 int, col2 ntext)";
//    cmd.ExecuteNonQuery();
//}
//catch { }
//finally
//{
//    conn.Close();
//}


//SQLiteConnection.CreateFile("MyDatabase.sqlite");

//SQLiteConnection m_dbConnection = new SQLiteConnection("Data Source=MyDatabase.sqlite;Version=3;");
//m_dbConnection.SetPassword("admin"); 
//m_dbConnection.Open();

//string sql = "create table results (cells varchar(20), value real)";

//SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection);
//command.ExecuteNonQuery();

//res_rows.ForEach(row =>
//{
//    sql = $"insert into results (cells, real) values ('{row.cells}', {row.value})";

//    command = new SQLiteCommand(sql, m_dbConnection);
//    command.ExecuteNonQuery();
//});

//m_dbConnection.Close();

//// добавление данных
//using (ApplicationContext db = new ApplicationContext())
//{
//    db.Database.EnsureDeleted();
//    db.Database.EnsureCreated();
//    // создаем два объекта User
//    //Results user1 = new Results { cells = "Tom", Age = 33 };
//    //Results user2 = new Results { Name = "Alice", Age = 26 };
//    var lst = new List<Results>();


//    res_rows.ForEach(row =>
//    {
//        //sql = $"insert into results (cells, real) values ('{row.cells}', {row.value})";
//        //command = new SQLiteCommand(sql, m_dbConnection);
//        //command.ExecuteNonQuery();
//        lst.Add(new Results { cells = row.cells, value = row.value });
//    });

//    // добавляем их в бд
//    db.Results.AddRange(lst);
//    db.SaveChanges();
//}
//// получение данных
//using (ApplicationContext db = new ApplicationContext())
//{
//    // получаем объекты из бд и выводим на консоль
//    var users = db.Results.ToList();
//    Console.WriteLine("Results list:");
//    foreach (Results u in users)
//    {
//        Console.WriteLine($"{u.cells} - {u.value}");
//    }
//}



string FilePath = "C:\\Users\\max35\\Documents\\Тестовое тз\\1\\4.mdb";
string OledbConnectionString = string.Format("Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Jet OLEDB:Database Password=admin;Persist Security Info=False;", FilePath);
using (OleDbConnection con = new OleDbConnection(OledbConnectionString))
{
    con.Open();



    //string sql = string.Empty;
    //sql = string.Format("ALTER DATABASE PASSWORD 'Admin' '1' ");
    //OleDbCommand cmd = new OleDbCommand(sql, con);


    try
    {
        //res_rows.ForEach(x => Console.WriteLine($"{x.cells} - {x.value}"));
        res_rows.Select((v, i) => new { i, v.cells, v.value }).ToList().ForEach(x =>
        {
            OleDbCommand cmd = new OleDbCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "INSERT INTO results (id, cells, COL_VALUES) VALUES (@id, @cells, @value1);";
            //cmd.CommandText = "INSERT INTO results (id, cells) VALUES (@id, @cells);";
            //cmd.CommandText = "INSERT INTO results (id) VALUES (@id);";
            //cmd.CommandText = $"INSERT INTO results (id, cells, value) VALUES ({x.i}, '{x.cells}', {x.value}); ";
            //cmd.CommandText = $"INSERT INTO results (id, cells, value) VALUES ('{x.i}', '{x.cells}', '{x.value.ToString().Replace(",", ".")}'); ";
            //cmd.CommandText = $"INSERT INTO results (id, cells, value) VALUES ('{x.i}', '{x.cells}', '{x.value.ToString().Replace(",", ".")}'); ";
            cmd.Parameters.AddWithValue("@id", x.i);
            cmd.Parameters.AddWithValue("@cells", x.cells);
            //cmd.Parameters.AddWithValue("@value", x.value.ToString().Replace(",", "."));
            //cmd.Parameters.AddWithValue("@value1", x.value.ToString(CultureInfo.InvariantCulture));
            cmd.Parameters.AddWithValue("@value1", x.value);
            cmd.Connection = con;
            cmd.ExecuteNonQuery();
        });


        //OleDbCommand select = new OleDbCommand();
        //select.Connection = con;
        //select.CommandText = "Select cells From Results ";
        //OleDbDataReader reader = select.ExecuteReader();
        //while (reader.Read())
        //{
        //    //listBox1.Items.Add(reader[1].ToString() + "," + reader[2].ToString());
        //    Console.WriteLine(reader.GetString(0));
        //}
    }
    finally
    {
        con.Close();
    }
}

public class Results
{
    public int Id { get; set; }
    public string cells { get; set; }
    public double value { get; set; }
}

public class ApplicationContext : DbContext
{
    public DbSet<Results> Results { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        //optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;AttachDbFileName=d:\data\yourDBname.mdf;Database=calculationsdb;Trusted_Connection=True;Password=admin;");
        optionsBuilder.UseSqlServer($@"Server=(localdb)\mssqllocaldb;AttachDbFileName={Path.Combine(AppContext.BaseDirectory, "db.mdf")};Database=calculationsdb;Trusted_Connection=True;Password='admin';");
        //optionsBuilder.UseSqlServer($@"Server=(localdb)\mssqllocaldb;AttachDbFileName=db.mdf;Database=calculationsdb2;Trusted_Connection=True;Password=admin;");
        //optionsBuilder.UseSqlServer(@"Server=./SQLEXPRESS;Database=calculationsdb;Trusted_Connection=True;Password=admin;");
    }
}