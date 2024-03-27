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
    public partial class izmenaNarudzbiDostavljac : Form
    {
        public izmenaNarudzbiDostavljac()
        {
            InitializeComponent();
            dataSetKlasa dataSetKlasa = new dataSetKlasa();

            using (SqlConnection connection = new SqlConnection(dataSetKlasa.connectionString))
            {
                connection.Open();
                string upit = "select * from Narudzba where statusNarudzbe = 'Preuzeta'";
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

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            DataTable table = new DataTable();

            using (SqlConnection connection = new SqlConnection(dataSetKlasa.connectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand("detaljiNarudzbe", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@idNarudzba", textBox2.Text); // Postavite odgovarajući ID ovde

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        // Dodajte kolone u DataTable prema strukturi rezultata
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            table.Columns.Add(reader.GetName(i), reader.GetFieldType(i));
                        }

                        // Čitanje i dodavanje redova iz SqlDataReader u DataTable
                        while (reader.Read())
                        {
                            DataRow row = table.NewRow();
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                row[i] = reader[i];
                            }
                            table.Rows.Add(row);
                        }
                    }
                }

                connection.Close();
            }

            // Postavljanje DataTable kao izvor podataka za DataGridView
            dataGridView2.DataSource = table;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (int.TryParse(textBox1.Text, out int idNarudzbe))
            {
                DataGridViewRow rowToDelete = null;
                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    if (row.Cells["idNarudzba"].Value != null && Convert.ToInt32(row.Cells["idNarudzba"].Value) == idNarudzbe)
                    {
                        rowToDelete = row;
                        break;
                    }
                }
                if (rowToDelete != null)
                {
                    DialogResult result = MessageBox.Show("Da li želite označite narudžbu kao 'Isporučeno'?", "Potvrdite akciju", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        dataGridView1.Rows.Remove(rowToDelete);
                        string sql = $"UPDATE Narudzba SET statusNarudzbe = 'Isporuceno' WHERE idNarudzba=@idNarudzba";
                        using (SqlConnection connection = new SqlConnection(dataSetKlasa.connectionString))
                        {
                            connection.Open();
                            using (SqlCommand command = new SqlCommand(sql, connection))
                            {
                                command.Parameters.AddWithValue("@idNarudzba", textBox1.Text);
                                int brojAžuriranihRedova = command.ExecuteNonQuery();
                                if (brojAžuriranihRedova > 0)
                                {
                                    MessageBox.Show("Status narudžbe promenjen.");
                                    textBox1.Text = "";
                                }
                                else
                                {
                                    MessageBox.Show("Nema narudžbi sa odabranim ID-ima.");
                                }
                            }
                            connection.Close();
                            dataGridView1.Refresh();
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Nema reda sa ID-om narudžbe: " + idNarudzbe, "Upozorenje", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    textBox1.Text = "";
                }
            }
            else
            {
                MessageBox.Show("Unesite ispravan format ID-a narudžbe.", "Greška", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBox1.Text = "";
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {

        }
    }
}
