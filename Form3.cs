using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Npgsql;

namespace barinak
{
    public partial class Form3 : Form
    {
        public class ComboBoxItem
        {
            public string Text { get; set; }
            public string Value { get; set; }

            public override string ToString()
            {
                return Text; // ComboBox'ta gösterilecek olan metin
            }
        }

        public Form3()
        {
            InitializeComponent();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }
        NpgsqlConnection baglanti = new NpgsqlConnection("server=localHost; port=5432 ; Database=barinak; user ID=postgres; password=571571 ");
        private void button2_Click(object sender, EventArgs e)
        {
            string sorgu = "SELECT \r\n    h.id AS hayvan_id,\r\n    h.dogum_tarih,\r\n    c.ad AS cins_adi,\r\n    t.ad AS tur_adi,\r\n    d.uygunluk_durum,\r\n " +
                "   b.ad AS barinak_adi\r\nFROM \r\n    hayvan h\r\nJOIN \r\n    cins c ON h.cins_id = c.id\r\nJOIN \r\n    tur t ON c.tur_id = t.id\r\nJOIN \r\n    saglik_karnesi sk ON h.saglik_id = sk.id\r\nJOIN \r\n    durum d ON sk.durum_id = d.id\r\nJOIN \r\n   " +
                " barinak_yerlesim by ON h.barinak_yer_id = by.id\r\nJOIN \r\n    barinak b ON by.barinak_id = b.id;\r\n";
            NpgsqlDataAdapter da = new NpgsqlDataAdapter(sorgu, baglanti);
            DataSet ds = new DataSet();
            da.Fill(ds);
            dataGridView1.DataSource = ds.Tables[0];

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }
        private void LoadTurData()
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    string query = "SELECT id, ad FROM tur";

                    using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
                    {
                        using (NpgsqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                // ComboBox2'ye tür bilgilerini ekle
                                comboBox2.Items.Add(new ComboBoxItem
                                {
                                    Text = reader["ad"].ToString(),
                                    Value = reader["id"].ToString()
                                });
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Hata: {ex.Message}");
                }
            }
        }



        private void Form3_Load(object sender, EventArgs e)
        {
            string connectionString = "Host=localhost;Port=5432;Username=postgres;Password=571571;Database=barinak";

            // Veritabanından hayvanların ID'lerini al ve ComboBox'a yükle
            LoadTurData();  
            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    string query = "SELECT id FROM hayvan";

                    using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
                    {
                        using (NpgsqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                // Hayvan ID'sini ComboBox'a ekle
                                comboBox1.Items.Add(reader["id"].ToString());
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Hata: {ex.Message}");
                }
            }
        }

        string connectionString = "Host=localhost;Port=5432;Username=postgres;Password=571571;Database=barinak";
        private void button1_Click(object sender, EventArgs e)
        {

            // Kullanıcıdan alınan bilgiler
            
            string ad = textBox5.Text;
            string soyad = textBox4.Text;
            DateTime dogumTarih = DateTime.Parse(maskedTextBox1.Text);
            string email = textBox2.Text;
            int hayvanId = Convert.ToInt32(comboBox1.Text);

            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    // Sahip tablosuna veri ekleme komutu
                    string insertSahipQuery = @"
                        INSERT INTO public.sahip ( ad, soyad, dogum_tarih, email)
                        VALUES ( @ad, @soyad, @dogum_tarih, @email)
                        RETURNING id;";

                    // Basvuru tablosuna veri ekleme komutu
                    string insertBasvuruQuery = @"
                        INSERT INTO public.basvuru (tarih, durum, hayvan_id, sahip_id)
                        VALUES (CURRENT_DATE, 'beklemede', @hayvan_id, @sahip_id);";

                    // Sahip tablosuna ekleme
                    using (NpgsqlCommand cmd = new NpgsqlCommand(insertSahipQuery, connection))
                    {
                        
                        cmd.Parameters.AddWithValue("@ad", ad);
                        cmd.Parameters.AddWithValue("@soyad", soyad);
                        cmd.Parameters.AddWithValue("@dogum_tarih", dogumTarih);
                        cmd.Parameters.AddWithValue("@email", email);

                        // Yeni eklenen sahip'in ID'sini al
                        int sahipId = (int)cmd.ExecuteScalar();

                        // Basvuru tablosuna ekleme
                        using (NpgsqlCommand basvuruCmd = new NpgsqlCommand(insertBasvuruQuery, connection))
                        {
                            basvuruCmd.Parameters.AddWithValue("@hayvan_id", hayvanId);
                            basvuruCmd.Parameters.AddWithValue("@sahip_id", sahipId);

                            basvuruCmd.ExecuteNonQuery();
                        }
                    }

                    MessageBox.Show("Veriler başarıyla eklendi.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Hata: {ex.Message}");
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Form1 form1 = new Form1();
            form1.Show();
            this.Hide();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            var selectedItem = comboBox2.SelectedItem as ComboBoxItem;
            if (selectedItem != null)
            {
                string selectedTurId = selectedItem.Value;

                // Hayvanları getir ve DataGridView'a yükle
                LoadHayvanData(selectedTurId);
            }
            else
            {
                MessageBox.Show("Lütfen bir tür seçiniz.");
            }
        }

        private void LoadHayvanData(string turId)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    string query = @"
                SELECT h.id AS HayvanID, h.dogum_tarih AS DogumTarihi, 
                       c.ad AS CinsAdi, t.ad AS TurAdi, 
                       d.uygunluk_durum AS UygunlukDurumu, b.ad AS BarinakAdi
                FROM hayvan h
                JOIN cins c ON h.cins_id = c.id
                JOIN tur t ON c.tur_id = t.id
                JOIN saglik_karnesi sk ON h.saglik_id = sk.id
                JOIN durum d ON sk.durum_id = d.id
                JOIN barinak_yerlesim by ON h.barinak_yer_id = by.id
                JOIN barinak b ON by.barinak_id = b.id
                WHERE t.id = @turId";

                    using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@turId", int.Parse(turId));

                        using (NpgsqlDataAdapter adapter = new NpgsqlDataAdapter(command))
                        {
                            DataTable dataTable = new DataTable();
                            adapter.Fill(dataTable);

                            // Sonuçları DataGridView'e yükle
                            dataGridView1.DataSource = dataTable;
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Hata: {ex.Message}");
                }
            }
        }
    }
}
