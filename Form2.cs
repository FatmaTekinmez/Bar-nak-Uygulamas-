using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace barinak
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void Form2_Load(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            string girilenKullaniciAdi = textBox2.Text;
            string girilenSifre = textBox1.Text;

            // Doğru kullanıcı adı ve şifre
            string dogruKullaniciAdi = "admin";
            string dogruSifre = "12345";

            // Kontrol yap
            if (girilenKullaniciAdi == dogruKullaniciAdi && girilenSifre == dogruSifre)
            {
                // Kullanıcı adı ve şifre doğruysa Form3'ü aç
                Form5 form5 = new Form5();
                form5.Show();

                // Bu formu gizlemek istiyorsanız:
                this.Hide();
            }
            else
            {
                // Kullanıcı adı veya şifre yanlışsa mesaj göster
                MessageBox.Show("Kullanıcı adı veya şifre yanlış!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
