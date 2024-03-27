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
    public partial class detaljiNarudzbe : Form
    {
        
        public detaljiNarudzbe()
        {
            InitializeComponent();
            dataSetKlasa dataSetKlasa = new dataSetKlasa();
            using (SqlConnection connection = new SqlConnection(dataSetKlasa.connectionString))
            {
                connection.Open();
                string upit = "select * from detaljiNarudzbePogled";
                using (SqlCommand command = new SqlCommand(upit, connection))
                {

                    using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                    {
                        DataTable dataTable = new DataTable();
                        adapter.Fill(dataTable);

                        // Postavite DataGridView.DataSource na DataTable
                        dataGridView1.DataSource = dataTable;
                    }
                }
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            using (SqlConnection connection = new SqlConnection(dataSetKlasa.connectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand("detaljiRacuna", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@idNarudzba", textBox1.Text); 

                    using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                    {
                        DataTable dataTable = new DataTable();
                        adapter.Fill(dataTable);

                        dataGridView1.DataSource = dataTable;
                    }
                }
            }

            using (SqlConnection connection = new SqlConnection(dataSetKlasa.connectionString))
            {
                string upit = "SELECT round(ukupnaCena,2) FROM Račun WHERE idRacun=@idRacun";
                connection.Open();

                using (SqlCommand command = new SqlCommand(upit, connection))
                {
                    command.Parameters.AddWithValue("@idRacun", textBox1.Text);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            // Čitanje ukupne cene iz rezultata upita kao string
                            string ukupnaCena = reader[0].ToString();

                            label2.Text="Ukupna cena: ";
                            // Postavljanje vrednosti u labelu
                            label2.Text += ukupnaCena;
                        }
                        else{
                            label2.Text = "Ukupna cena:";
                        }
                    }
                }
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (SqlConnection connection = new SqlConnection(dataSetKlasa.connectionString))
            {
                connection.Open();
                string upit = "select * from detaljiNarudzbePogled";
                using (SqlCommand command = new SqlCommand(upit, connection))
                {

                    using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                    {
                        DataTable dataTable = new DataTable();
                        adapter.Fill(dataTable);

                        // Postavite DataGridView.DataSource na DataTable
                        dataGridView1.DataSource = dataTable;
                    }
                }
            }
        }
    }
}
