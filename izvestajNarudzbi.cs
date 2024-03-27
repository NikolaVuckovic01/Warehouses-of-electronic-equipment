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
using System.Windows.Forms.DataVisualization.Charting;

namespace Diplomski
{
    public partial class izvestajNarudzbi : Form
    {
        public izvestajNarudzbi()
        {
            InitializeComponent();
        }

        private void izvestajNarudzbi_Load(object sender, EventArgs e)
        {
            SqlConnection connection = new SqlConnection(dataSetKlasa.connectionString);
            string query = @"select YEAR(datumKreiranjaNarudzbe) AS Godina,MONTH(datumKreiranjaNarudzbe) AS Mesec,count(idNarudzba) as brojNarudzbi
                            from Narudzba
                            GROUP BY
                            YEAR(datumKreiranjaNarudzbe),
                            MONTH(datumKreiranjaNarudzbe)";

            SqlCommand command = new SqlCommand(query, connection);
            connection.Open();


            SqlDataAdapter adapter = new SqlDataAdapter(command);
            DataTable table = new DataTable();
            adapter.Fill(table);
            chart1.DataSource = table;
            chart1.Series.Clear(); 
            Series series = chart1.Series.Add("Broj kreiranih narudžbi");
            series.ChartType = SeriesChartType.Column;
            series.XValueMember = "Mesec";
            series.YValueMembers = "brojNarudzbi";
            chart1.ChartAreas[0].AxisX.Title = "Mesec";
            chart1.ChartAreas[0].AxisY.Title = "Broj kreiranih narudžbi";
            connection.Close();

            string mesecniQuery = "SELECT COUNT(idNarudzba) AS BrojNarudzbi " +
                                      "FROM Narudzba " +
                                      "WHERE YEAR(datumKreiranjaNarudzbe) = YEAR(GETDATE()) " +
                                      "AND MONTH(datumKreiranjaNarudzbe) = MONTH(GETDATE())";
            string godisnjilQuery = "SELECT COUNT(idNarudzba) AS BrojNarudzbi " +
                                  "FROM Narudzba " +
                                  "WHERE YEAR(datumKreiranjaNarudzbe) = YEAR(GETDATE())";

            string kolicinaAvg = "select avg(StavkaRacuna.kolicina) as 'Prosečna količina proizvoda u narudžbi' from StavkaRacuna";
            string cenaNarudzbiAvg = "select round(avg(ukupnaCena),2) as 'Prosečna cena narudžbi' from Račun";
            string cenaProizvodaAvg = "select round(avg(cena), 2) as 'Prosečna cena proizvoda' from Proizvod";



            connection.Open();

            SqlCommand trenutniMesecSql = new SqlCommand(mesecniQuery, connection);
            int trenutniMesec = (int)trenutniMesecSql.ExecuteScalar();

            SqlCommand godisnjiSql = new SqlCommand(godisnjilQuery, connection);
            int godisnji = (int)godisnjiSql.ExecuteScalar();

            SqlCommand kolicina = new SqlCommand(kolicinaAvg, connection);
            int kolicinaInt = (int)kolicina.ExecuteScalar();

            SqlCommand cenaNarudzbiAvgSql = new SqlCommand(cenaNarudzbiAvg, connection);
            Object cenaNarudzbiAvgFloat = cenaNarudzbiAvgSql.ExecuteScalar();

            SqlCommand cenaProizvodaAvgSql = new SqlCommand(cenaProizvodaAvg, connection);
            Object cenaProizvodaAvgFloat = cenaProizvodaAvgSql.ExecuteScalar();


            label1.Text = "Broj narudžbi za trenutni mesec: " + trenutniMesec.ToString();
            label2.Text = "Godišnji broj narudžbi: " + godisnji.ToString();
            label3.Text = "Prosečna količina proizvoda u narudžbi " + kolicinaInt.ToString();
            label4.Text = "Prosečna ukupna cena u narudžbi: " + cenaNarudzbiAvgFloat.ToString();
            label5.Text = "Prosečna cena proizvoda: " + cenaProizvodaAvgFloat.ToString();

            // Zatvaranje veze sa bazom podataka
            connection.Close();
        }
    }
}
