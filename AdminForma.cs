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
    public partial class AdminForma : Form
    {
        dataSetKlasa dataSetKlasa;

        public AdminForma()
        {
            InitializeComponent();
            dataSetKlasa = new dataSetKlasa();
            dataSetKlasa.popuniDataSet();
            dataGridView1.DataSource = dataSetKlasa.dataSet.Proizvod;
            dataGridView2.DataSource = dataSetKlasa.dataSet.Narudzba;
            dataGridView3.DataSource = dataSetKlasa.dataSet.Račun;
        }
        private void AdminForma_Load(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
            listBox2.Items.Clear();
            signalizacijaKolicine();
            signalizacijaPreuzetihNarudzbi();
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            string upit = "SELECT * FROM Proizvod WHERE brend LIKE '"+textBox2.Text+"%' OR model LIKE '"+textBox2.Text+ "%' OR idProizvod LIKE '"+textBox2.Text+"%'";
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
        public void signalizacijaKolicine() 
        {
            string upit = "SELECT idProizvod, brend, model FROM Proizvod WHERE kolicina<=3";
            listBox1.Items.Clear();
            using (SqlConnection connection = new SqlConnection(dataSetKlasa.connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(upit, connection))
                {
                    SqlDataReader reader = command.ExecuteReader();
                    string message = "Proizvodi sa količinom manjom ili jednakom 3:\n\n";
                    string message2 = "";
                    if (reader.HasRows) {
                        while (reader.Read())
                        {
                            int idProizvod = reader.GetInt32(0);
                            string brend = reader.GetString(1);
                            string model = reader.GetString(2);
                            listBox1.Items.Add(message);
                            message2 = $"ID Proizvoda: {idProizvod}, Brend: {brend}, Model: {model}\n\n";
                            listBox1.Items.Add(message2);
                        }
                        reader.Close();
                    }                 
                }
            }
        }
         
        public void signalizacijaPreuzetihNarudzbi()
        {
            string upit = "select count(idNarudzba) from Narudzba where statusNarudzbe = 'Preuzeta'";
            listBox2.Items.Clear();
            using (SqlConnection connection = new SqlConnection(dataSetKlasa.connectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(upit, connection))
                {
                    int ukupno = (int)command.ExecuteScalar();
                    string message = "Pruezeto je vise od 10 narudzbi za isporuku:\n\n";
                    string message2 = "";
                    if (ukupno >= 1) {
                        listBox2.Items.Add(message);
                        message2 = message;
                    }
                }
            }
        }


        private void button1_Click(object sender, EventArgs e)
        {
            dataSetKlasa.popuniDataSet();
            dataGridView1.DataSource = dataSetKlasa.dataSet.Proizvod;
            textBox2.Text = "";
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            string upit = "SELECT * FROM Narudzba WHERE idNarudzba LIKE '"+textBox1.Text+"%'";

            using (SqlConnection connection = new SqlConnection(dataSetKlasa.connectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(upit, connection))
                {

                    using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                    {
                        DataSet ds = new DataSet();
                        adapter.Fill(ds);
                        dataGridView2.DataSource = ds.Tables[0];
                    }
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            dataGridView2.DataSource = dataSetKlasa.dataSet.Narudzba;
            textBox1.Text = "";

        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            DateTime selectedDate = dateTimePicker1.Value;
            string sqlFormattedDate = selectedDate.ToString("yyyy-MM-dd");
            string upit = "SELECT * FROM Narudzba WHERE CONVERT(DATE, datumKreiranjaNarudzbe) = '"+sqlFormattedDate+"'";

            using (SqlConnection connection = new SqlConnection(dataSetKlasa.connectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(upit, connection))
                {

                    using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                    {
                        DataSet ds = new DataSet();
                        adapter.Fill(ds);
                        dataGridView2.DataSource = ds.Tables[0];
                    }
                }
            }
        }
        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            string upit = "SELECT * FROM Račun WHERE idRacun LIKE '" + textBox3.Text + "%'";

            using (SqlConnection connection = new SqlConnection(dataSetKlasa.connectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(upit, connection))
                {

                    using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                    {
                        DataSet ds = new DataSet();
                        adapter.Fill(ds);
                        dataGridView3.DataSource = ds.Tables[0];
                    }
                }
            }
        }

        private void dateTimePicker2_ValueChanged(object sender, EventArgs e)
        {
            DateTime selectedDate = dateTimePicker2.Value;
            string sqlFormattedDate = selectedDate.ToString("yyyy-MM-dd");
            string upit = "SELECT * FROM Račun WHERE CONVERT(DATE, datum) = '" + sqlFormattedDate + "'";

            using (SqlConnection connection = new SqlConnection(dataSetKlasa.connectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(upit, connection))
                {

                    using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                    {
                        DataSet ds = new DataSet();
                        adapter.Fill(ds);
                        dataGridView3.DataSource = ds.Tables[0];
                    }
                }
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            dataGridView3.DataSource = dataSetKlasa.dataSet.Račun;
            textBox3.Text = "";
        }

        private void button5_Click(object sender, EventArgs e)
        {
            detaljiNarudzbe detaljiNarudzbe = new detaljiNarudzbe();
            detaljiNarudzbe.Show();
        }

        private void odjaviSeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Da li ste sigurni da želite da se odjavite?", "Potvrda", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                Form1 form1 = new Form1();
                form1.FormClosed += (s, args) => this.Close(); // Dodajte ovu liniju
                form1.Show();
                this.Hide();
            }

        }

        private void napraviNovogKorisnikaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            noviKorisnik noviKorisnik = new noviKorisnik();
            noviKorisnik.Show();
        }

        private void pregledajSveKorisnikeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            korisniciIzmeniForma korisniciIzmeniForma = new korisniciIzmeniForma();
            korisniciIzmeniForma.Show();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Da li želite da sačuvate izmene?", "Potvrda", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                using (SqlConnection connection = new SqlConnection(dataSetKlasa.connectionString))
                {
                    connection.Open();

                    foreach (DataGridViewRow row in dataGridView2.Rows)
                    {
                        if (row.Cells["idNarudzba"].Value != null)
                        {
                            int primaryKeyValue = Convert.ToInt32(row.Cells["idNarudzba"].Value);

                            // Kreirajte SQL upit za ažuriranje svake kolone osim "idKorisnik"
                            string updateQuery = "UPDATE Narudzba SET ";
                            bool firstColumn = true;

                            foreach (DataGridViewCell cell in row.Cells)
                            {
                                string columnName = dataGridView2.Columns[cell.ColumnIndex].Name;

                                // Preskočite "idKorisnik" kolonu
                                if (columnName != "idNarudzba" && columnName != "datumKreiranjaNarudzbe")
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

                            updateQuery += " WHERE idNarudzba = @PrimaryKey";

                            using (SqlCommand command = new SqlCommand(updateQuery, connection))
                            {
                                // Dodajte parametre za ažuriranje svake kolone osim "idKorisnik"
                                foreach (DataGridViewCell cell in row.Cells)
                                {
                                    string columnName = dataGridView2.Columns[cell.ColumnIndex].Name;

                                    // Preskočite "idKorisnik" kolonu
                                    if (columnName != "idNarudzba" && columnName != "datumKreiranjaNarudzbe")
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
                MessageBox.Show("Narudzbe su ažurirane");
                signalizacijaPreuzetihNarudzbi();
            }
            else
            {
                dataGridView2.DataSource = dataSetKlasa.dataSet.Narudzba;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Da li želite da sačuvate izmene?", "Potvrda", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                using (SqlConnection connection = new SqlConnection(dataSetKlasa.connectionString))
                {
                    connection.Open();
                    foreach (DataGridViewRow row in dataGridView1.Rows)
                    {
                        if (row.Cells["idProizvod"].Value != null)
                        {
                            int primaryKeyValue = Convert.ToInt32(row.Cells["idProizvod"].Value);
                            string updateQuery = "UPDATE Proizvod SET ";
                            bool firstColumn = true;
                            foreach (DataGridViewCell cell in row.Cells)
                            {
                                string columnName = dataGridView1.Columns[cell.ColumnIndex].Name;
                                if (columnName != "idProizvod")
                                {
                                    object newValue = cell.Value;
                                    if (columnName == "cena")
                                    {
                                        decimal decimalValue;
                                        if (decimal.TryParse(newValue.ToString(), out decimalValue))
                                        {
                                            newValue = decimalValue;
                                        }
                                    }
                                    if (!firstColumn)
                                    {
                                        updateQuery += ", ";
                                    }
                                    updateQuery += $"{columnName} = @NewValue_{columnName}";
                                    firstColumn = false;
                                }
                            }
                            updateQuery += " WHERE idProizvod = @PrimaryKey";
                            using (SqlCommand command = new SqlCommand(updateQuery, connection))
                            {
                                foreach (DataGridViewCell cell in row.Cells)
                                {
                                    string columnName = dataGridView1.Columns[cell.ColumnIndex].Name;
                                    if (columnName != "idProizvod")
                                    {
                                        object newValue = cell.Value; 
                                        command.Parameters.AddWithValue($"@NewValue_{columnName}", newValue);
                                    }
                                }
                                command.Parameters.AddWithValue("@PrimaryKey", primaryKeyValue);
                                command.ExecuteNonQuery();
                            }
                        }
                    }
                }

                MessageBox.Show("Uspešno sačuvano.");
            }
            else
            {
                dataGridView1.DataSource = dataSetKlasa.dataSet.Proizvod;
            }
            signalizacijaKolicine();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            dodajProizvodForma dodajProizvodForma = new dodajProizvodForma();
            dodajProizvodForma.Show();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox4.Text))
            {
                MessageBox.Show("Unesite ID proizvoda koji želite obrisati.", "Greška", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            DialogResult result = MessageBox.Show("Da li želite da obrišete proizvod?", "Potvrda", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                string upit = "DELETE FROM Proizvod WHERE idProizvod = @idProizvod";

                using (SqlConnection connection = new SqlConnection(dataSetKlasa.connectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(upit, connection))
                    {
                        command.Parameters.AddWithValue("@idProizvod", textBox4.Text);
                        try
                        {
                            int izmenjeno = command.ExecuteNonQuery();

                            if (izmenjeno > 0)
                            {
                                MessageBox.Show("Proizvod je uspješno obrisan.");
                                dataGridView1.Refresh();
                                dataSetKlasa.popuniDataSet();
                                dataGridView1.DataSource = dataSetKlasa.dataSet.Proizvod;
                                textBox4.Text = "";
                                textBox2.Text = "";
                            }
                            else
                            {
                                MessageBox.Show("Nema proizvoda s tim ID-om.");
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Greška prilikom brisanja proizvoda: " + ex.Message);
                        }
                    }
                }
                textBox4.Text = "";
            }
        }

        private void izveštajProdajeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            izvestajForma izvestajForma = new izvestajForma();
            izvestajForma.Show();
        }

        private void izveštajNarudžbiToolStripMenuItem_Click(object sender, EventArgs e)
        {
            izvestajNarudzbi izvestajNarudzbi = new izvestajNarudzbi();
            izvestajNarudzbi.Show();
        }

        private void timer1_Tick_1(object sender, EventArgs e)
        {
            int lastIndex = tabControl1.TabPages.Count - 1;
            if (listBox1 != null && listBox2 != null && listBox1.Items.Count != 0 || listBox2.Items.Count != 0)
            {
                tabControl1.TabPages[lastIndex].Text = "Obaveštenja*";
            }
            else
            {
                tabControl1.TabPages[lastIndex].Text = "Obaveštenja";
            }

        }
    }
}
