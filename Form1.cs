using System;
using System.Windows.Forms;

namespace DES
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        Fanction des = new Fanction();
        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                string shifr = des.EncryptionStart(textBox1.Text, textBox4.Text);
                textBox2.Text = shifr;
            }
            catch
            {
                string m = null;
                textBox2.Text = m; 
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                string deshifr = des.DecryptionStart(textBox2.Text, textBox4.Text);
                textBox3.Text = deshifr;
            }
            catch
            {
                string m = null;
                textBox3.Text = m;
            }
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            
               
        }

        private void groupBox2_Enter(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
