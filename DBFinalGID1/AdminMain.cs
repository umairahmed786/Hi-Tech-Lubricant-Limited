using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DBFinalGID1
{
    public partial class AdminMain : Form
    {
        public AdminMain()
        {
            InitializeComponent();
            
        }

        private void ShutDown(object sender, FormClosedEventArgs e)
        {
            LoginForm.close();
        }

        private void gunaAdvenceButton1_Click(object sender, EventArgs e)
        {
            bunifuMainPages.SetPage("dashBoard");
          
        }

        private void gunaAdvenceButton2_Click(object sender, EventArgs e)
        {
            bunifuMainPages.SetPage("tabPage2");
        }

        private void gunaAdvenceButton3_Click(object sender, EventArgs e)
        {
  //          bunifuNavbarPages.SetPage("employeeNavBar");
        }

        private void gunaAdvenceButton4_Click(object sender, EventArgs e)
        {
//            bunifuNavbarPages.SetPage("tabPage4");
        }

        private void gunaAdvenceButton2_Click_1(object sender, EventArgs e)
        {

        }

        private void gunaAdvenceButton3_Click_1(object sender, EventArgs e)
        {

        }

        private void gunaAdvenceButton4_Click_1(object sender, EventArgs e)
        {

        }

        private void gunaAdvenceButton5_Click(object sender, EventArgs e)
        {

        }

        private void gunaAdvenceButton6_Click(object sender, EventArgs e)
        {

        }

        private void gunaAdvenceButton7_Click(object sender, EventArgs e)
        {

        }

        private void gunaAdvenceButton8_Click(object sender, EventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void guna2CustomGradientPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void guna2PictureBox1_MouseHover(object sender, EventArgs e)
        {
            guna2PictureBox1.Size = new System.Drawing.Size(350, 250);
        }

        private void guna2PictureBox1_MouseLeave(object sender, EventArgs e)
        {
            guna2PictureBox1.Size = new System.Drawing.Size(300, 200);
        }

        private void guna2PictureBox2_MouseHover(object sender, EventArgs e)
        {
            guna2PictureBox2.Size = new System.Drawing.Size(350, 250);

        }

        private void guna2PictureBox2_MouseLeave(object sender, EventArgs e)
        {
            guna2PictureBox2.Size = new System.Drawing.Size(300, 200);

        }

        private void guna2PictureBox5_MouseHover(object sender, EventArgs e)
        {
            guna2PictureBox5.Size = new System.Drawing.Size(350, 250);

        }

        private void guna2PictureBox5_MouseLeave(object sender, EventArgs e)
        {
            guna2PictureBox5.Size = new System.Drawing.Size(300, 200);

        }

        private void guna2PictureBox3_MouseHover(object sender, EventArgs e)
        {
            guna2PictureBox3.Size = new System.Drawing.Size(350, 250);

        }

        private void guna2PictureBox3_MouseLeave(object sender, EventArgs e)
        {
            guna2PictureBox3.Size = new System.Drawing.Size(300, 200);

        }

        private void guna2PictureBox4_MouseHover(object sender, EventArgs e)
        {
            guna2PictureBox4.Size = new System.Drawing.Size(350, 250);

        }

        private void guna2PictureBox4_MouseLeave(object sender, EventArgs e)
        {
            guna2PictureBox4.Size = new System.Drawing.Size(300, 200);

        }

        private void guna2PictureBox6_MouseHover(object sender, EventArgs e)
        {
            guna2PictureBox6.Size = new System.Drawing.Size(350, 250);

        }

        private void guna2PictureBox6_MouseLeave(object sender, EventArgs e)
        {
            guna2PictureBox6.Size = new System.Drawing.Size(300, 200);

        }
    }
}
