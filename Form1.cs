using System;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace Diplomski
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            this.BackColor = System.Drawing.ColorTranslator.FromHtml("#ECEFF1");
        }
        
        private void button1_Click(object sender, EventArgs e)
        {
            string query = "SELECT COUNT(*) FROM Korisnici WHERE email=@Email AND lozinka=@Lozinka AND status!='administrator'";

            using (SqlConnection connection = new SqlConnection(dataSetKlasa.connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Email", textBox1.Text);
                    command.Parameters.AddWithValue("@Lozinka", textBox2.Text);

                    int rowCount = (int)command.ExecuteScalar();
                    if (rowCount == 0)
                    {
                        MessageBox.Show("Ne postoji korisnik sa ponuđenim emailom i lozinkom");
                    }
                    else
                    {
                        string queryStatus = "SELECT status FROM Korisnici WHERE email=@Email AND lozinka=@Lozinka";
                        using (SqlCommand command2 = new SqlCommand(queryStatus, connection))
                        {
                            command2.Parameters.AddWithValue("@Email", textBox1.Text);
                            command2.Parameters.AddWithValue("@Lozinka", textBox2.Text);

                            string status = (string)command2.ExecuteScalar();
                            if (status == "dostavljac")
                            {
                                dostavljacForma dostavljacForma = new dostavljacForma();
                                dostavljacForma.FormClosed += (s, args) => this.Close();
                                dostavljacForma.Show();
                                this.Hide();
                            }
                            else if (status == "prodavac")
                            {
                                KorisnikForma korisnikForma = new KorisnikForma();
                                korisnikForma.FormClosed += (s, args) => this.Close();
                                korisnikForma.Show();
                                this.Hide();
                            }
                            else
                            {
                                MessageBox.Show("Status radnika nije pronađen u bazi");
                            }
                        }
                    }
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string query = "SELECT COUNT(*) FROM Korisnici WHERE email=@Email AND lozinka=@Lozinka AND status='administrator'";

            using (SqlConnection connection = new SqlConnection(dataSetKlasa.connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Email", textBox1.Text);
                    command.Parameters.AddWithValue("@Lozinka", textBox2.Text);

                    int rowCount = (int)command.ExecuteScalar();
                    if (rowCount == 0)
                    {
                        MessageBox.Show("Ne postoji administrator sa ponuđenim emailom i lozinkom");
                    }
                    else
                    {
                        AdminForma adminForma = new AdminForma();
                        adminForma.FormClosed += (s, args) => this.Close();
                        adminForma.Show();
                        this.Hide();
                    }
                }
            }
        }
    }
}
