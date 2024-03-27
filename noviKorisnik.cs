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
    public partial class noviKorisnik : Form
    {
        public noviKorisnik()
        {
            InitializeComponent();
            dataSetKlasa dataSetKlasa = new dataSetKlasa();
            dataSetKlasa.popuniDataSet();
            string[] korisnici = { "administrator", "dostavljac", "prodavac"};
            comboBox1.DataSource = korisnici;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Dodati novog korisnika?", "Potvrda", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {

                string ime = textBoxIme.Text;
                string prezime = textBoxPrezime.Text;
                string email = textBoxEmail.Text;
                string lozinka = textBoxLozinka.Text;
                string brojTelefona = textBoxBrojTelefona.Text;
                string status = comboBox1.SelectedValue.ToString();

                
                using (SqlConnection connection = new SqlConnection(dataSetKlasa.connectionString))
                {
                    connection.Open();

                    // SQL upit za unos novog korisnika
                    string query = "INSERT INTO Korisnici (ime, prezime, email, lozinka, status, brojTelefona) VALUES (@Ime, @Prezime, @Email, @Lozinka, @Status, @BrojTelefona)";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Ime", ime);
                        command.Parameters.AddWithValue("@Prezime", prezime);
                        command.Parameters.AddWithValue("@Email", email);
                        command.Parameters.AddWithValue("@Lozinka", lozinka);
                        command.Parameters.AddWithValue("@Status", status);
                        command.Parameters.AddWithValue("@BrojTelefona", brojTelefona);

                        try
                        {
                            int rowsAffected = command.ExecuteNonQuery();

                            if (rowsAffected > 0)
                            {
                                MessageBox.Show("Korisnik je uspešno registrovan.");
                                textBoxIme.Text = "";
                                textBoxPrezime.Text = "";
                                textBoxEmail.Text = "";
                                textBoxLozinka.Text = "";
                                textBoxBrojTelefona.Text = "";
                            }
                            else
                            {
                                MessageBox.Show("Registracija nije uspela.");
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Greška prilikom registracije: " + ex.Message);
                        }
                    }
                }

            }
            else{
            }
        }
    }
}
