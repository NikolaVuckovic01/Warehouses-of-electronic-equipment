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
    public partial class dostavljacForma : Form
    {
        List<int> selektovaneNarudzbe = new List<int>();

        public dostavljacForma()
        {
            InitializeComponent();
            dataSetKlasa dataSetKlasa = new dataSetKlasa();

            using (SqlConnection connection = new SqlConnection(dataSetKlasa.connectionString))
            {
                connection.Open();
                string upit = "select * from Narudzba where statusNarudzbe = 'Na cekanju'";
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

        private void button1_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                // Dobijte selektovani red iz dataGridView1
                DataGridViewRow selectedRow = dataGridView1.SelectedRows[0];

                // Dobijte vrednost idNarudzbe iz selektovanog reda
                if (selectedRow.Cells["idNarudzba"].Value != null)
                {
                    int idNarudzbe = Convert.ToInt32(selectedRow.Cells["idNarudzba"].Value);

                    // Dodajte idNarudzbe u listu
                    selektovaneNarudzbe.Add(idNarudzbe);
                    label2.Text += selectedRow.Cells["idNarudzba"].Value + ", ";
                }
                dataGridView1.Rows.Remove(selectedRow);
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Da li želite da preuzmete narudžbe?", "Potvrda", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                if (selektovaneNarudzbe.Count > 0)
                {
                    string sql = "UPDATE Narudzba SET statusNarudzbe = 'Preuzeta' WHERE idNarudzba IN (";
                    for (int i = 0; i < selektovaneNarudzbe.Count; i++)
                    {
                        if (i > 0)
                        {
                            sql += ",";
                        }
                        sql += "@idNarudzba" + i;
                    }
                    sql += ")";
                    using (SqlConnection connection = new SqlConnection(dataSetKlasa.connectionString))
                    {
                        connection.Open();
                        using (SqlCommand command = new SqlCommand(sql, connection))
                        {
                            for (int i = 0; i < selektovaneNarudzbe.Count; i++)
                            {
                                command.Parameters.AddWithValue("@idNarudzba" + i, selektovaneNarudzbe[i]);
                            }
                            int brojAžuriranihRedova = command.ExecuteNonQuery();
                            if (brojAžuriranihRedova > 0)
                            {
                                MessageBox.Show("Narudžbe su preuzete za isporuku.");
                                label2.Text = "";
                                string stavke = string.Join(", ", selektovaneNarudzbe);
                                selektovaneNarudzbe.Clear();
                                DataTable dataTable = new DataTable();
                                using (SqlDataAdapter adapter = new SqlDataAdapter($"SELECT n.idNarudzba, n.datumKreiranjaNarudzbe, n.adresa, p.kategorija, p.brend, p.model, sr.kolicina FROM Narudzba n INNER JOIN Račun r ON n.idNarudzba = r.idNarudzba INNER JOIN StavkaRacuna sr ON r.idRacun = sr.idRacun INNER JOIN Proizvod p ON sr.idProizvod = p.idProizvod WHERE n.idNarudzba IN ({stavke})", connection))
                                {
                                    adapter.Fill(dataTable);
                                }
                                printaj(dataTable);
                            }
                            else
                            {
                                MessageBox.Show("Nema narudžbi sa odabranim ID-ima.");
                            }
                        }
                        connection.Close();
                    }
                }
                else
                {
                    MessageBox.Show("Nema selektovanih narudžbi za ažuriranje.");
                }
                AdminForma adminForma = new AdminForma();
                adminForma.signalizacijaPreuzetihNarudzbi();
            }
        }
        private void pregledajIliIzemniPreuzeteNarudžbeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            izmenaNarudzbiDostavljac izmenaNarudzbiDostavljac = new izmenaNarudzbiDostavljac();
            izmenaNarudzbiDostavljac.Show();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            selektovaneNarudzbe.Clear();
            label2.Text = "";
            using (SqlConnection connection = new SqlConnection(dataSetKlasa.connectionString))
            {
                connection.Open();
                string upit = "select * from Narudzba where statusNarudzbe = 'Na cekanju'";
                using (SqlCommand command = new SqlCommand(upit, connection))
                {

                    using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                    {
                        DataTable dataTable = new DataTable();
                        adapter.Fill(dataTable);

                        dataGridView1.DataSource = dataTable;
                    }
                }
            }
        }

        private void odjaviSeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Da li ste sigurni da želite da se odjavite?", "Potvrda", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                Form1 form1 = new Form1();
                form1.FormClosed += (s, args) => this.Close();
                form1.Show();
                this.Hide();
            }
        }

        private void pregledajDnevniIzveštajToolStripMenuItem_Click(object sender, EventArgs e)
        {
            izvestajDostavljac izvestajDostavljac = new izvestajDostavljac();
            izvestajDostavljac.Show();
        }

        private void printaj(DataTable dataTable)
        {
            List<int> stampaneStavke = new List<int>();

            PrintDocument printDocument = new PrintDocument();
            PrintPreviewDialog printPreviewDialog = new PrintPreviewDialog();

            printDocument.PrintPage += new PrintPageEventHandler((s, ev) =>
            {
                int yPosition = 50;
                Font titleFont = new Font("Arial", 16, FontStyle.Bold);
                Font dataFont = new Font("Arial", 14);

                foreach (DataRow row in dataTable.Rows)
                {
                    int idNarudzba = Convert.ToInt32(row["idNarudzba"]);

                    // Proverite da li je stavka već štampana
                    if (!stampaneStavke.Contains(idNarudzba))
                    {
                        // Stavka nije štampana, pa je štampajte
                        string idNarudzbaStr = idNarudzba.ToString();
                        string datum = row["datumKreiranjaNarudzbe"].ToString();
                        string adresa = row["adresa"].ToString();
                        string kategorija = row["kategorija"].ToString();
                        string brend = row["brend"].ToString();
                        string model = row["model"].ToString();
                        string kolicina = row["kolicina"].ToString();

                        string ispis = $"ID Narudžbe: {idNarudzbaStr}, Datum: {datum}, Adresa: {adresa}, Kategorija: {kategorija}\n Brend: {brend}, Model: {model}, Količina: {kolicina}, Status:\n----------------------------------------------------";

                        int lineHeight = (int)ev.Graphics.MeasureString(ispis, new Font("Arial", 12)).Height;
                        ev.Graphics.DrawString(ispis, new Font("Arial", 12), Brushes.Black, new PointF(10, yPosition));
                        yPosition += lineHeight + 20;

                        // Dodajte stavku u listu štampanih stavki
                        stampaneStavke.Add(idNarudzba);
                    }
                }
            });

            printPreviewDialog.Width = 1000;
            printPreviewDialog.Height = 800;
            printPreviewDialog.Document = printDocument;
            printPreviewDialog.ShowDialog();
        }



        private void printDocument_PrintPage(object sender, PrintPageEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            string ispisStavki = string.Join(", ", selektovaneNarudzbe);
            MessageBox.Show(ispisStavki.ToString());
        }
    }
}
