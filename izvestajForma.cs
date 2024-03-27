using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Diplomski
{
    public partial class izvestajForma : Form
    {
        private DateTime selectedMonth = new DateTime();

        public izvestajForma()
        {
            InitializeComponent();
            dateTimePicker1.Format = DateTimePickerFormat.Custom;
            dateTimePicker1.CustomFormat = "MMMM yyyy";
        }

        private void izvestajForma_Load(object sender, EventArgs e)
        {
            dateTimePicker1.Value = DateTime.Now;
            dataGridView1.Refresh();
            prodajaMeseca();
            ukupnoZaradjenoZaMesec();
            ukupnoZaradjenoZaGodinu(label5);
            ukupnoZaradjenoZaGodinu(label10);
            danasnjaProdaja();
            godisnjaProdaja();
        }

        private void prodajaMeseca() 
        {
            selectedMonth = dateTimePicker1.Value;
            DateTime firstDayOfMonth = new DateTime(selectedMonth.Year, selectedMonth.Month, 1);
            DateTime lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

            using (SqlConnection connection = new SqlConnection(dataSetKlasa.connectionString))
            {
                connection.Open();
                string upit = @"SELECT p.brend AS BrendProizvoda, p.model AS ModelProizvoda, SUM(sr.kolicina) AS Prodano
                    FROM StavkaRacuna sr
                    INNER JOIN Račun r ON sr.idRacun = r.idRacun
                    INNER JOIN Proizvod p ON sr.idProizvod = p.idProizvod
                    WHERE r.datum BETWEEN @StartDate AND @EndDate
                    GROUP BY sr.idProizvod,p.brend,p.model
                    ORDER BY Prodano DESC";
                using (SqlCommand command = new SqlCommand(upit, connection))
                {
                    command.Parameters.AddWithValue("@StartDate", firstDayOfMonth);
                    command.Parameters.AddWithValue("@EndDate", lastDayOfMonth);
                    using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                    {
                        DataTable dataTable = new DataTable();
                        adapter.Fill(dataTable);
                        dataGridView1.DataSource = dataTable;
                    }
                }
            }
        }

        private void danasnjaProdaja() 
        {

            using (SqlConnection connection = new SqlConnection(dataSetKlasa.connectionString))
            {
                connection.Open();
                string upit = @"SELECT p.brend AS BrendProizvoda, p.model AS ModelProizvoda, SUM(sr.kolicina) AS Prodano
                                FROM StavkaRacuna sr
                                INNER JOIN Račun r ON sr.idRacun = r.idRacun
                                INNER JOIN Proizvod p ON sr.idProizvod = p.idProizvod
                                WHERE CAST(r.datum AS DATE) = CAST(GETDATE() AS DATE)
                                GROUP BY sr.idProizvod, p.brend, p.model
                                ORDER BY Prodano DESC;";
                using (SqlCommand command = new SqlCommand(upit, connection))
                {
                    using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                    {
                        DataTable dataTable = new DataTable();
                        adapter.Fill(dataTable);
                        dataGridView2.DataSource = dataTable;
                    }
                }
            }
            string sql = @"SELECT TOP 5 p.brend AS BrendProizvoda, p.model AS ModelProizvoda, SUM(sr.kolicina) AS Prodano
                            FROM StavkaRacuna sr
                            INNER JOIN Račun r ON sr.idRacun = r.idRacun
                            INNER JOIN Proizvod p ON sr.idProizvod = p.idProizvod
                            WHERE CAST(r.datum AS DATE) = CAST(GETDATE() AS DATE)
                            GROUP BY sr.idProizvod, p.brend, p.model
                            ORDER BY Prodano DESC;";

            using (SqlConnection connection = new SqlConnection(dataSetKlasa.connectionString))
            {
                connection.Open();

                SqlCommand cmd = new SqlCommand(sql, connection);

                SqlDataReader reader = cmd.ExecuteReader();

                chart2.Series[0].Points.Clear();

                while (reader.Read())
                {
                    string brend = reader["BrendProizvoda"].ToString();
                    string model = reader["ModelProizvoda"].ToString();
                    int prodano = Convert.ToInt32(reader["Prodano"]);

                    // Dodajte podatke u Chart kontrolu
                    chart2.Series[0].Points.AddXY($"{brend} {model}\nKoličina:{prodano}", prodano);
                }

                reader.Close();
                connection.Close();
            }
            using (SqlConnection connection = new SqlConnection(dataSetKlasa.connectionString))
            {
                connection.Open();
                string upit = @"select round((sum(ukupnaCena)),2)
                                from Račun r
                                where CAST(r.datum AS DATE) = CAST(GETDATE() AS DATE)";
                using (SqlCommand command = new SqlCommand(upit, connection))
                {

                    float ukupno;
                    float.TryParse(command.ExecuteScalar().ToString(), out ukupno);
                    label11.Text = "Današnja zarada: " + ukupno.ToString();
                }
            }
        }

        public void godisnjaProdaja() 
        {
            string sql = @"SELECT TOP 10 p.brend AS BrendProizvoda, p.model AS ModelProizvoda, SUM(sr.kolicina) AS Prodano
                            FROM StavkaRacuna sr
                            INNER JOIN Račun r ON sr.idRacun = r.idRacun
                            INNER JOIN Proizvod p ON sr.idProizvod = p.idProizvod
                            WHERE YEAR(r.datum) = YEAR(GETDATE())
                            GROUP BY sr.idProizvod, p.brend, p.model
                            ORDER BY Prodano DESC;";

            using (SqlConnection connection = new SqlConnection(dataSetKlasa.connectionString))
            {
                connection.Open();

                SqlCommand cmd = new SqlCommand(sql, connection);

                SqlDataReader reader = cmd.ExecuteReader();

                chart3.Series[0].Points.Clear();

                while (reader.Read())
                {
                    string brend = reader["BrendProizvoda"].ToString();
                    string model = reader["ModelProizvoda"].ToString();
                    int prodano = Convert.ToInt32(reader["Prodano"]);

                    chart3.Series[0].Points.AddXY($"{brend} {model}\nKoličina:{prodano}", prodano);
                }

                reader.Close();
                connection.Close();
            }
        }

        private void ukupnoZaradjenoZaMesec() 
        {
            selectedMonth = dateTimePicker1.Value;
            DateTime firstDayOfMonth = new DateTime(selectedMonth.Year, selectedMonth.Month, 1);
            DateTime lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);
            using (SqlConnection connection = new SqlConnection(dataSetKlasa.connectionString))
            {
                connection.Open();
                string upit = @"select round((sum(ukupnaCena)),2)
                                from Račun
                                where datum BETWEEN @StartDate AND @EndDate";
                using (SqlCommand command = new SqlCommand(upit, connection))
                {
                    command.Parameters.AddWithValue("@StartDate", firstDayOfMonth);
                    command.Parameters.AddWithValue("@EndDate", lastDayOfMonth);

                    float ukupno;
                    float.TryParse(command.ExecuteScalar().ToString(),out ukupno);
                    label4.Text = "Ukupno zarađeno za izabrani mesec: " + ukupno.ToString();
                }
            }
        }

        private void ukupnoZaradjenoZaGodinu(Label labela)
        {
            DateTime godina = dateTimePicker1.Value;
            using (SqlConnection connection = new SqlConnection(dataSetKlasa.connectionString))
            {
                connection.Open();
                string upit = @"select round((sum(ukupnaCena)),2)
                                from Račun
                                where YEAR(datum)=YEAR(@godina)";
                using (SqlCommand command = new SqlCommand(upit, connection))
                {
                    command.Parameters.AddWithValue("@godina", godina);

                    float ukupno;
                    float.TryParse(command.ExecuteScalar().ToString(), out ukupno);
                    if (labela == label10)
                    {
                        labela.Text = "Ukupno zarađeno ove godine: " + ukupno.ToString();
                    }
                    else {
                        labela.Text = "Ukupno zarađeno za izabranu godinu: " + ukupno.ToString();
                    }
                }
            }
        }

        private void dateTimePicker1_ValueChanged_1(object sender, EventArgs e)
        {
            selectedMonth = dateTimePicker1.Value;
            DateTime firstDayOfMonth = new DateTime(selectedMonth.Year, selectedMonth.Month, 1);
            DateTime lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);
            string sql = @"SELECT TOP 5 p.brend AS BrendProizvoda, p.model AS ModelProizvoda, SUM(sr.kolicina) AS Prodano
                            FROM StavkaRacuna sr
                            INNER JOIN Račun r ON sr.idRacun = r.idRacun
                            INNER JOIN Proizvod p ON sr.idProizvod = p.idProizvod
                            WHERE r.datum BETWEEN @StartDate AND @EndDate
                            GROUP BY sr.idProizvod,p.brend,p.model
                            ORDER BY Prodano DESC";

            using (SqlConnection connection = new SqlConnection(dataSetKlasa.connectionString))
            {
                connection.Open();

                SqlCommand cmd = new SqlCommand(sql, connection);
                cmd.Parameters.AddWithValue("@StartDate", firstDayOfMonth);
                cmd.Parameters.AddWithValue("@EndDate", lastDayOfMonth);

                SqlDataReader reader = cmd.ExecuteReader();

                // Očisti postojeće podatke u Chart kontroli
                chart1.Series[0].Points.Clear();

                while (reader.Read())
                {
                    string brend = reader["BrendProizvoda"].ToString();
                    string model = reader["ModelProizvoda"].ToString();
                    int prodano = Convert.ToInt32(reader["Prodano"]);

                    // Dodajte podatke u Chart kontrolu
                    chart1.Series[0].Points.AddXY($"{brend} {model}\nKoličina:{prodano}", prodano);
                }

                reader.Close();
                connection.Close();
            }
            prodajaMeseca();
            ukupnoZaradjenoZaMesec();
            ukupnoZaradjenoZaGodinu(label5);
        }
    }
}
