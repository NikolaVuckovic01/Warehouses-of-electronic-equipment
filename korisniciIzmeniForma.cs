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
    public partial class korisniciIzmeniForma : Form
    {
        dataSetKlasa dataSetKlasa = new dataSetKlasa();
        public korisniciIzmeniForma()
        {
            InitializeComponent();
            dataSetKlasa.popuniDataSet();
            dataGridView1.DataSource = dataSetKlasa.dataSet.Korisnici;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            string upit = "SELECT * FROM Korisnici WHERE idKorisnik LIKE '" + textBox1.Text + "%' OR email LIKE '" + textBox1.Text + "%'";

            using (SqlConnection connection = new SqlConnection(dataSetKlasa.connectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(upit, connection))
                {

                    using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                    {
                        DataSet ds = new DataSet();
                        adapter.Fill(ds);
                        dataGridView1.DataSource = ds.Tables[0];
                    }
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            dataGridView1.DataSource = dataSetKlasa.dataSet.Korisnici;
            textBox1.Text = "";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Da li želite da sačuvate izmene?", "Potvrda", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes) {
                using (SqlConnection connection = new SqlConnection(dataSetKlasa.connectionString))
                {
                    connection.Open();

                    foreach (DataGridViewRow row in dataGridView1.Rows)
                    {
                        if (row.Cells["idKorisnik"].Value != null)
                        {
                            int primaryKeyValue = Convert.ToInt32(row.Cells["idKorisnik"].Value);

                            // Kreirajte SQL upit za ažuriranje svake kolone osim "idKorisnik"
                            string updateQuery = "UPDATE Korisnici SET ";
                            bool firstColumn = true;

                            foreach (DataGridViewCell cell in row.Cells)
                            {
                                string columnName = dataGridView1.Columns[cell.ColumnIndex].Name;

                                // Preskočite "idKorisnik" kolonu
                                if (columnName != "idKorisnik")
                                {
                                    string newValue = cell.Value != null ? cell.Value.ToString() : null;

                                    if (!firstColumn)
                                    {
                                        updateQuery += ", ";
                                    }

                                    updateQuery += $"{columnName} = @NewValue_{columnName}";
                                    firstColumn = false;
                                }
                            }

                            updateQuery += " WHERE idKorisnik = @PrimaryKey";

                            using (SqlCommand command = new SqlCommand(updateQuery, connection))
                            {
                                // Dodajte parametre za ažuriranje svake kolone osim "idKorisnik"
                                foreach (DataGridViewCell cell in row.Cells)
                                {
                                    string columnName = dataGridView1.Columns[cell.ColumnIndex].Name;

                                    // Preskočite "idKorisnik" kolonu
                                    if (columnName != "idKorisnik")
                                    {
                                        string newValue = cell.Value != null ? cell.Value.ToString() : null;
                                        command.Parameters.AddWithValue($"@NewValue_{columnName}", newValue);
                                    }
                                }

                                // Dodajte parametar za primarni ključ
                                command.Parameters.AddWithValue("@PrimaryKey", primaryKeyValue);

                                // Izvršite SQL upit
                                command.ExecuteNonQuery();
                            }
                        }
                    }

                }
                MessageBox.Show("Korisnici su ažurirani");
            }
            else{
                dataGridView1.DataSource = dataSetKlasa.dataSet.Korisnici;
            }

           
        }

        private void button3_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Da li želite da obrišete korisnika?", "Potvrda", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes){
                string upit = "DELETE FROM Korisnici WHERE idKorisnik = @idKorisnik";

                using (SqlConnection connection = new SqlConnection(dataSetKlasa.connectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(upit, connection))
                    {
                        command.Parameters.AddWithValue("@idKorisnik", textBox2.Text);

                        try
                        {
                            int rowsAffected = command.ExecuteNonQuery();

                            if (rowsAffected > 0)
                            {
                                // Brisanje je uspješno izvršeno
                                MessageBox.Show("Korisnik je uspješno obrisan.");
                                dataGridView1.Refresh(); // Osvježi prikaz
                                dataSetKlasa.popuniDataSet();
                                dataGridView1.DataSource = dataSetKlasa.dataSet.Korisnici;
                                textBox2.Text = "";
                                textBox1.Text = "";
                            }
                            else
                            {
                                // Nema korisnika s tim ID-om
                                MessageBox.Show("Nema korisnika s tim ID-om.");
                            }
                        }
                        catch (Exception ex)
                        {
                            // Prikazati grešku u slučaju problema s brisanjem
                            MessageBox.Show("Greška prilikom brisanja korisnika: " + ex.Message);
                        }
                    }
                }
            }
            else
            {
            }
        }
    }
}
