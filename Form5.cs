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
    public partial class Form5 : Form
    {
        public Form5()
        {
            InitializeComponent();
        }
        string connectionString = "Host=localhost;Port=5432;Username=postgres;Password=571571;Database=barinak";
        private void LoadBasvurular()
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    string query = @"
                SELECT 
                    b.id AS BasvuruID,
                    h.id AS HayvanID,
                    t.ad AS HayvanTuru,
                    c.ad AS HayvanCinsi,
                    b.tarih AS BasvuruTarihi,
                    b.durum AS BasvuruDurumu,
                    s.ad AS SahipAdi,
                    s.soyad AS SahipSoyadi,
                    s.dogum_tarih AS SahipDogumTarihi,
                    s.email AS SahipEmail
                FROM 
                    basvuru b
                JOIN 
                    hayvan h ON b.hayvan_id = h.id
                JOIN 
                    cins c ON h.cins_id = c.id
                JOIN 
                    tur t ON c.tur_id = t.id
                JOIN 
                    sahip s ON b.sahip_id = s.id;
            ";

                    using (NpgsqlDataAdapter adapter = new NpgsqlDataAdapter(query, connection))
                    {
                        DataTable dataTable = new DataTable();
                        adapter.Fill(dataTable);
                        dataGridView1.DataSource = dataTable; // DataGridView'e veriyi yükler
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Hata: {ex.Message}");
                }
            }
        }
        private void UpdateBasvuru()
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                string basvuruId = dataGridView1.SelectedRows[0].Cells["BasvuruID"].Value.ToString();
                string yeniDurum = comboBox2.SelectedItem.ToString(); // ComboBox'taki yeni durum

                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    try
                    {
                        connection.Open();

                        string query = "UPDATE basvuru SET durum = @durum WHERE id = @id";

                        using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@durum", yeniDurum);
                            command.Parameters.AddWithValue("@id", int.Parse(basvuruId));
                            command.ExecuteNonQuery();

                            MessageBox.Show("Başvuru başarıyla güncellendi!");
                            LoadBasvurular(); // Güncel veriyi yeniden yükler
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Hata: {ex.Message}");
                    }
                }
            }
            else
            {
                MessageBox.Show("Lütfen bir başvuru seçiniz.");
            }
        }

        private void DeleteBasvuru()
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                string basvuruId = dataGridView1.SelectedRows[0].Cells["BasvuruID"].Value.ToString();

                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    try
                    {
                        connection.Open();

                        string query = "DELETE FROM basvuru WHERE id = @id";

                        using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@id", int.Parse(basvuruId));
                            command.ExecuteNonQuery();

                            MessageBox.Show("Başvuru başarıyla silindi!");
                            LoadBasvurular(); // Güncel veriyi yeniden yükler
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Hata: {ex.Message}");
                    }
                }
            }
            else
            {
                MessageBox.Show("Lütfen bir başvuru seçiniz.");
            }
        }



        private void Form5_Load(object sender, EventArgs e)
        {
            LoadBasvurular();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DeleteBasvuru();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            UpdateBasvuru();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Form1 form1 = new Form1();
            form1.Show();
            this.Hide();
        }
    }
}
