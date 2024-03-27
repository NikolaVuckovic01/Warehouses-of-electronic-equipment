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
    public partial class dodajProizvodForma : Form
    {
        public dodajProizvodForma()
        {
            InitializeComponent();
            numericUpDown1Kolicina.Minimum = 1;
            numericUpDown2Cena.Minimum = 1;
            numericUpDown3Popust.Minimum = 0;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Dodati novi proizvod?", "Potvrda", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                string kategorija = textBox1Kategorija.Text;
                string brend = textBox2Brend.Text;
                string model = textBox3Model.Text;
                int kolicina = (int)numericUpDown1Kolicina.Value;
                double cena = (double)numericUpDown2Cena.Value;
                float popust = (float)numericUpDown3Popust.Value;

                using (SqlConnection connection = new SqlConnection(dataSetKlasa.connectionString))
                {
                    connection.Open();
                    string provjeraUpit = "SELECT COUNT(*) FROM Proizvod WHERE brend = @Brend AND model = @Model";
                    using (SqlCommand provjeraCommand = new SqlCommand(provjeraUpit, connection))
                    {
                        provjeraCommand.Parameters.AddWithValue("@Brend", brend);
                        provjeraCommand.Parameters.AddWithValue("@Model", model);

                        int brojPronadenihProizvoda = (int)provjeraCommand.ExecuteScalar();

                        if (brojPronadenihProizvoda > 0)
                        {
                            MessageBox.Show("Proizvod sa istim brendom i modelom već postoji.");
                        }
                        else
                        {
                            // Ako proizvod ne postoji, možemo ga dodati
                            string query = "INSERT INTO Proizvod(kategorija, brend, model, kolicina, cena, popust) VALUES (@Kategorija, @Brend, @Model, @Kolicina, @Cena, @Popust)";

                            using (SqlCommand command = new SqlCommand(query, connection))
                            {
                                command.Parameters.AddWithValue("@Kategorija", kategorija);
                                command.Parameters.AddWithValue("@Brend", brend);
                                command.Parameters.AddWithValue("@Model", model);
                                command.Parameters.AddWithValue("@Kolicina", kolicina);
                                command.Parameters.AddWithValue("@Cena", cena);
                                command.Parameters.AddWithValue("@Popust", popust);

                                try
                                {
                                    int rowsAffected = command.ExecuteNonQuery();

                                    if (rowsAffected > 0)
                                    {
                                        MessageBox.Show("Proizvod je uspešno kreiran.\nOsvežite prikaz proizvoda!");
                                        // Ostatak koda za resetiranje polja i druge akcije
                                    }
                                    else
                                    {
                                        MessageBox.Show("Kreiranje nije uspelo!");
                                    }
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show("Greška prilikom kreiranja: " + ex.Message);
                                }
                            }
                        }
                    }
                }
                textBox1Kategorija.Text = "";
                textBox2Brend.Text = "";
                textBox3Model.Text = "";
                numericUpDown1Kolicina.Value = 1;
                numericUpDown2Cena.Value = 1;
                numericUpDown3Popust.Value = 0;
            }
            else
            {
                // Korisnik je izabrao "Ne" ili zatvorio dijalog
                Console.WriteLine("Radnja je otkazana.");
            }
        }

    }
}
