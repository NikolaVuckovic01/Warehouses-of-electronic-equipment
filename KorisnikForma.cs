using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Diplomski
{
    public partial class KorisnikForma : Form
    {
        dataSetKlasa dataSetKlasa = new dataSetKlasa();
        dataSetKlasa dataSetKlasa2 = new dataSetKlasa();

        public KorisnikForma()
        {
            InitializeComponent();
            dataSetKlasa.popuniDataSet();
            dataGridView1.DataSource = dataSetKlasa.dataSet.Proizvod;
            dataGridView2.DataSource = dataSetKlasa2.dataSet.Proizvod;
            numericUpDown1.Minimum = 1;

            using (SqlConnection connection = new SqlConnection(dataSetKlasa.connectionString))
            {
                connection.Open();

                string query = "SELECT DISTINCT kategorija FROM Proizvod";
                SqlCommand command = new SqlCommand(query, connection);
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    comboBox1.Items.Add(reader["kategorija"].ToString());
                }

                reader.Close();
            }

        }


        private void button2_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                // Dobijte selektovani red iz dataGridView1
                DataGridViewRow selectedRow = dataGridView1.SelectedRows[0];

                // Dobijte vrednost "Kolicina" iz numericUpDown kontrole
                int kolicinaIzNumericUpDown = (int)numericUpDown1.Value;

                // Dobijte vrednost "Kolicina" iz selektovanog reda u dataGridView1
                int kolicinaIzDataGridView1 = (int)selectedRow.Cells["Kolicina"].Value;

                if (kolicinaIzNumericUpDown > kolicinaIzDataGridView1)
                {
                    MessageBox.Show("Izabrana količina je veća od količine u skladištu.", "Greška");
                }
                else
                {
                    // Kopirajte podatke iz selektovanog reda
                    object[] rowData = new object[selectedRow.Cells.Count];

                    for (int i = 0; i < selectedRow.Cells.Count; i++)
                    {
                        // Preskočite kolonu "Kolicina"
                        if (dataGridView1.Columns[i].Name != "kolicina")
                        {
                            rowData[i] = selectedRow.Cells[i].Value;
                        }
                        else
                        {
                            // Uzmite Kolicina iz numericUpDown kontrola
                            rowData[i] = kolicinaIzNumericUpDown;
                        }
                    }

                    // Provjerite da li postoji ista vrijednost u koloni "idProizvod" u dataGridView2
                    bool duplicateExists = false;
                    int newIdProizvod = Convert.ToInt32(rowData[0]); // Konvertuj u int

                    // Dobijte DataTable koji je izvor podataka za dataGridView2
                    DataTable dataSource2 = ((DataTable)dataGridView2.DataSource);

                    foreach (DataRow row in dataSource2.Rows)
                    {
                        if (row["idProizvod"] != DBNull.Value && Convert.ToInt32(row["idProizvod"]) == newIdProizvod)
                        {
                            duplicateExists = true;
                            break;
                        }
                    }

                    // Dodajte kopirane podatke kao novi red u DataTable za dataGridView2 ako ne postoji duplikat
                    if (!duplicateExists)
                    {
                        dataSource2.Rows.Add(rowData);
                        numericUpDown1.Value = 1;

                        // Osvježite dataGridView2 kako biste videli promene
                        dataGridView2.Refresh();
                    }
                    else
                    {
                        MessageBox.Show("Artikal je već dodan na listu.", "Duplikat");
                    }
                }
            }


        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (dataGridView2.SelectedRows.Count > 0)
            {
                // Dobijte selektovani red iz dataGridView2
                DataGridViewRow selectedRow = dataGridView2.SelectedRows[0];

                // Izbrišite selektovani red iz dataGridView2
                dataGridView2.Rows.Remove(selectedRow);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            // Dobijte DataTable iz DataSet-a
            DataTable dataTable = dataSetKlasa2.dataSet.Tables["Proizvod"]; // Zamijenite "ImeDataTable-a" sa stvarnim imenom tablice

            // Očistite podatke iz DataTable-a
            dataTable.Clear();

            // Osvježite dataGridView2 kako biste videli promene
            dataGridView2.Refresh();
        }

        private void dataGridView2_RowStateChanged(object sender, DataGridViewRowStateChangedEventArgs e)
        {
            decimal ukupnaCena = 0;

            foreach (DataGridViewRow row in dataGridView2.Rows)
            {
                // Dobijte vrednosti za cenu, popust i količinu iz odgovarajućih kolona
                if (decimal.TryParse(row.Cells["Cena"].Value.ToString(), out decimal cena) &&
                    decimal.TryParse(row.Cells["Popust"].Value.ToString(), out decimal popust) &&
                    int.TryParse(row.Cells["Kolicina"].Value.ToString(), out int kolicina))
                {
                    // Izračunajte cenu sa popustom za svaki red sa uzetom količinom
                    decimal cenaSaPopustom = cena * (1 - (popust / 100)) * kolicina;
                    ukupnaCena += cenaSaPopustom;
                }
            }

            // Ažurirajte tekst labele sa ukupnom cenom
            label7.Text = ukupnaCena.ToString(); // "C" formatira u valutu

        }

        private void button5_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Da li ste sigurni da želite kreirate račun?", "Potvrda", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                if (dataGridView2.RowCount > 0 && !string.IsNullOrEmpty(textBox2.Text))
                {
                    using (SqlConnection connection = new SqlConnection(dataSetKlasa.connectionString))
                    {
                        connection.Open();
                        string sql = @"
                                    BEGIN TRY
                                        BEGIN TRANSACTION;

                                        -- Prvi upit: Insert u tabelu Narudzba sa parametrima
                                        INSERT INTO Narudzba (datumKreiranjaNarudzbe, adresa, statusNarudzbe)
                                        VALUES (@DatumKreiranja, @Adresa, 'Na cekanju');

                                        -- Drugi upit: Select MAX(idNarudzba)
                                        DECLARE @idNarudzba INT;
                                        SELECT @idNarudzba = MAX(idNarudzba)
                                        FROM Narudzba;

                                        -- Treći upit: Insert u tabelu Račun sa parametrima
                                        INSERT INTO Račun (ukupnaCena, datum, idNarudzba)
                                        VALUES (@UkupnaCena, @Datum, @idNarudzba);

                                        COMMIT;
                                    END TRY
                                    BEGIN CATCH
                                        ROLLBACK;
                                        THROW;
                                    END CATCH;";

                        using (SqlCommand command = new SqlCommand(sql, connection))
                        {
                            command.Parameters.AddWithValue("@DatumKreiranja", DateTime.Now);
                            command.Parameters.AddWithValue("@Adresa", textBox2.Text);
                            command.Parameters.AddWithValue("@UkupnaCena", float.Parse(label7.Text));
                            command.Parameters.AddWithValue("@Datum", DateTime.Now);
                            try
                            {
                                command.ExecuteNonQuery();
                                using (SqlConnection connection2 = new SqlConnection(dataSetKlasa.connectionString))
                                {
                                    connection2.Open();
                                    SqlTransaction transaction = connection2.BeginTransaction();
                                    try
                                    {
                                        int maxIdRacun;
                                        using (SqlCommand comMaxId = new SqlCommand("SELECT MAX(idRacun) FROM Račun", connection2, transaction))
                                        {
                                            maxIdRacun = (int)(comMaxId.ExecuteScalar() ?? 0);
                                        }
                                        string stavkaRacunaInsertSql = "INSERT INTO StavkaRacuna (idRacun, idProizvod, kolicina) VALUES (@IdRacun, @IdProizvod, @Kolicina)";
                                        for (int j = 0; j < dataGridView2.Rows.Count; j++)
                                        {
                                            var idProizvod = dataGridView2.Rows[j].Cells[0].Value;
                                            var kolicina = dataGridView2.Rows[j].Cells[4].Value;
                                            using (SqlCommand com3 = new SqlCommand(stavkaRacunaInsertSql, connection2, transaction))
                                            {
                                                com3.Parameters.AddWithValue("@IdRacun", maxIdRacun);
                                                com3.Parameters.AddWithValue("@IdProizvod", idProizvod);
                                                com3.Parameters.AddWithValue("@Kolicina", kolicina);

                                                com3.ExecuteNonQuery();
                                            }
                                            using (SqlCommand com4 = new SqlCommand("UPDATE Proizvod SET kolicina = kolicina - @Kolicina WHERE idProizvod = @IdProizvod", connection2, transaction))
                                            {
                                                com4.Parameters.AddWithValue("@IdProizvod", idProizvod);
                                                com4.Parameters.AddWithValue("@Kolicina", kolicina);
                                                com4.ExecuteNonQuery();
                                            }
                                        }
                                        int maxIdNarudzba = 0;
                                        using (SqlCommand com1 = new SqlCommand("SELECT MAX(idNarudzba) FROM Narudzba", connection2, transaction))
                                        {
                                            maxIdNarudzba = (int)(com1.ExecuteScalar() ?? 0);
                                        }
                                        foreach (DataGridViewRow row in dataGridView2.Rows)
                                        {
                                            var idProizvod = row.Cells[0].Value;
                                            using (SqlCommand com5 = new SqlCommand("INSERT INTO StavkaNarudzbe (idProizvod, idNarudzba) VALUES (@IdProizvod, @IdNarudzba)", connection2, transaction))
                                            {
                                                com5.Parameters.AddWithValue("@IdProizvod", idProizvod);
                                                com5.Parameters.AddWithValue("@IdNarudzba", maxIdNarudzba); 
                                                com5.ExecuteNonQuery();
                                            }
                                        }
                                        transaction.Commit();
                                        connection2.Close();
                                    }
                                    catch (Exception ex)
                                    {
                                        transaction.Rollback();
                                        MessageBox.Show("Greška prilikom izvršavanja transakcije: " + ex.Message);
                                    }
                                    finally
                                    {
                                        connection2.Close();
                                    }
                                }
                                MessageBox.Show("Transakcija uspešno izvršena.\nRačun je unesen u bazu.");
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show("Greška prilikom izvršavanja transakcije: " + ex.Message);
                            }
                        }
                        connection.Close();
                    }
                    PrintData();
                    dataSetKlasa.popuniDataSet();
                    dataGridView1.DataSource = dataSetKlasa.dataSet.Proizvod;
                    dataGridView1.Refresh();
                    DataTable dataTable = dataSetKlasa2.dataSet.Tables["Proizvod"];
                    dataTable.Clear();
                    dataGridView2.Refresh();
                    textBox2.Text = "";
                    AdminForma admin = new AdminForma();
                    admin.signalizacijaKolicine();
                }
                else
                {
                    MessageBox.Show("Izaberite artikle ili popunite adresu.");
                }
            }
        }


        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            string upit = "SELECT * FROM Proizvod WHERE brend LIKE '" + textBox1.Text + "%' OR model LIKE '" + textBox1.Text + "%'";

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


        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            using (SqlConnection connection = new SqlConnection(dataSetKlasa.connectionString))
            {
                string upit = "SELECT * FROM Proizvod WHERE kategorija = @Kategorija";

                connection.Open();

                using (SqlCommand command = new SqlCommand(upit, connection))
                {
                    command.Parameters.AddWithValue("@Kategorija", comboBox1.Text);
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
            dataSetKlasa.popuniDataSet();
            dataGridView1.DataSource = dataSetKlasa.dataSet.Proizvod;
            textBox1.Text = "";
            comboBox1.Text = "";
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

        private void PrintData()
        {
            PrintDocument printDocument = new PrintDocument();
            printDocument.PrinterSettings.Copies = 2;
            PrintPreviewDialog printPreviewDialog = new PrintPreviewDialog();

            printDocument.PrintPage += new PrintPageEventHandler((s, ev) =>
            {
                // Inicijalizujte promenljive za štampanje
                int yPosition = 50; // Pomaknite tekst od vrha strane
                int pageWidth = ev.PageBounds.Width;
                int pageHeight = ev.PageBounds.Height;
                int columnCount = dataGridView2.Columns.Count;

                // Font za štampanje većih slova
                Font titleFont = new Font("Arial", 16, FontStyle.Bold);

                // Font za štampanje podataka
                Font dataFont = new Font("Arial", 14);

                // Štampanje naslova (ime kolona)
                for (int i = 0; i < columnCount; i++)
                {
                    if (dataGridView2.Columns[i].Name != "idProizvod") {
                        string columnName = dataGridView2.Columns[i].HeaderText;
                        ev.Graphics.DrawString(columnName, titleFont, Brushes.Black, i * pageWidth / columnCount, yPosition);
                    }
                  
                }

                // Povećajte poziciju za štampanje teksta ispod naslova
                yPosition += titleFont.Height;

                // Štampanje podataka iz DataGridView2
                for (int j = 0; j < dataGridView2.Rows.Count; j++)
                {
                    for (int i = 0; i < columnCount; i++)
                    {
                        // Preskočite kolonu "idProizvod"
                        if (dataGridView2.Columns[i].Name != "idProizvod")
                        {
                            string cellValue = dataGridView2.Rows[j].Cells[i].Value.ToString();
                            ev.Graphics.DrawString(cellValue, dataFont, Brushes.Black, i * pageWidth / columnCount, yPosition);
                        }
                    }
                    yPosition += dataFont.Height;
                }

                // Dodajte ukupnu cenu
                string totalPrice = "Ukupna cena: " + label7.Text;
                ev.Graphics.DrawString(totalPrice, titleFont, Brushes.Black, 10, pageHeight - titleFont.Height - 50);

                // Dodajte adresu
                string address = "Adresa: " + textBox2.Text;
                ev.Graphics.DrawString(address, titleFont, Brushes.Black, 10, pageHeight - titleFont.Height - 100);

                // Ako ima više strana za štampanje, postavite ev.HasMorePages na true
                // Da biste omogućili štampanje sledeće strane

                int idRacun;
                int idNarudzba;
                using (SqlConnection connection = new SqlConnection(dataSetKlasa.connectionString))
                {
                    connection.Open();
                    string getIdQuery = "SELECT idRacun,idNarudzba FROM Račun where idRacun = (select MAX(idRacun) from Račun)";
                    SqlCommand getIdCommand = new SqlCommand(getIdQuery, connection);
                    SqlDataReader reader = getIdCommand.ExecuteReader();
                    if (reader.Read())
                    {
                        idRacun = reader.GetInt32(0);
                        idNarudzba = reader.GetInt32(1);
                    }
                    else
                    {
                        // U slučaju da ne može da se dohvati idRacun i idNarudzba, postavite ih na 0
                        idRacun = 0;
                        idNarudzba = 0;
                    }
                    reader.Close();
                }

                // Dodajte idRacun i idNarudzba u štampanje
                string idInfo = $"ID Računa: {idRacun}\nID Narudžbe: {idNarudzba}";
                ev.Graphics.DrawString(idInfo, titleFont, Brushes.Black, 10, pageHeight - titleFont.Height - 150 - titleFont.Height);

                // Ako ima više strana za štampanje, postavite ev.HasMorePages na true
                // Da biste omogućili štampanje sledeće strane
            });

            // Prikaži pregled štampe
            printPreviewDialog.Width = 1000;
            printPreviewDialog.Height = 800;
            printPreviewDialog.Document = printDocument;
            printPreviewDialog.ShowDialog();
        }

        private void pregledajMesečniIzveštajToolStripMenuItem_Click(object sender, EventArgs e)
        {
            izvestajForma izvestajForma = new izvestajForma();
            izvestajForma.Show();
        }
    }
}

