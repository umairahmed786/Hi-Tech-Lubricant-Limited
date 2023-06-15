using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Text.RegularExpressions;

namespace DBFinalGID1
{
    public partial class LoginForm : Form
    {
        public static string distRegNoLoggedIn="";
        Form MainForm;
        private static LoginForm Lform;
        private LoginForm()
        {
            InitializeComponent();
            
        }
        public static LoginForm instance() {
            if (Lform==null) {
                Lform = new LoginForm();
            }
            return Lform;
        }
        public static void close() {
            Lform.Close();
        }

        private void bunifuTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void CheckLogin()
        {

            // Check Login Admin

            if (UsernameBox.Text == "Admin" && PasswordBox.Text == "admin123")
            {

                this.Hide();
                MainForm = MainAdminScreen.instance();
                MainForm.Show();
                UsernameBox.Text = "";
                PasswordBox.Text = "";
                return;
            }

            ////////////////////////////////////////////////////
            //Check Login Distributor
            if (UsernameBox.Text != "" && PasswordBox.Text != "")
            {
                var con = Configuration.getInstance().getConnection();
                SqlCommand cmds = new SqlCommand("BEGIN TRAN checkDistributorCredentials", con);
                SqlCommand cmd1 = new SqlCommand("EXEC sp_GetLoginDistributor @password = @Password, @username = @Username", con);
                SqlCommand cmde = new SqlCommand("COMMIT TRAN checkDistributorCredentials", con);
                cmds.ExecuteNonQuery();
                cmd1.Parameters.AddWithValue("@Password", PasswordBox.Text);
                cmd1.Parameters.AddWithValue("@Username", UsernameBox.Text);
                if (cmd1.ExecuteScalar() != null)
                {
                    distRegNoLoggedIn = cmd1.ExecuteScalar().ToString();
                }
                else
                {
                    distRegNoLoggedIn = "";
                }
                    cmde.ExecuteNonQuery();
                if (distRegNoLoggedIn != "")
                {
                    this.Hide();
                    MainForm = new DistributorMain();
                    MainForm.Show();
                    UsernameBox.Text = "";
                    PasswordBox.Text = "";
               
                }
                else
                {
                    MessageBox.Show("Invalid credentials added", "Invalid Account");
                }
            }
            else
            {
                MessageBox.Show("Username or Password cannot be empty", "");
            }
        }
        private void EnterLogin(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13) {
                CheckLogin();
            }
        }

        private void guna2CircleButton1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void MaximiseButton_Click(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Maximized)
            {
                this.WindowState = FormWindowState.Normal;
            }
            else {
                this.WindowState = FormWindowState.Maximized;
            }
        }

        private void minimiseButton_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void SignInButton_Click(object sender, EventArgs e)
        {
            LoginSignupPage4000.SetPage(0);
        }

        private void SignUpButton_Click(object sender, EventArgs e)
        {

            var con = Configuration.getInstance().getConnection();
            SqlDataReader reader;
            SqlCommand sc = new SqlCommand("SELECT Value FROM LookUp WHERE Category = 'Region'", con);
            reader = sc.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Columns.Add("Value", typeof(string));
            dt.Load(reader);
            Console.WriteLine(dt);
            DataRow dr = dt.NewRow();
            dr["Value"] = "Select Region";
            dt.Rows.InsertAt(dr, 0);

            signUpDistRegion.ValueMember = "Value";
            signUpDistRegion.DataSource = dt;
            //signUpDistRegion.Items.Insert(0, "Select Region");


            LoginSignupPage4000.SetPage(1);

        }

        private void LoginButton_Click(object sender, EventArgs e)
        {
            CheckLogin();
        }

        private void PasswordEntered(object sender, EventArgs e)
        {

        }

        private void ConfirmPassEntered(object sender, EventArgs e)
        {
            signUpDistConfirmPassword.PasswordChar = '●';
        }

        private void bunifuTextBox4_TextChanged(object sender, EventArgs e)
        {

        }

        private void bunifuButton1_Click(object sender, EventArgs e)
        {
            if ((Regex.IsMatch(signUpDistName.Text, @"(^[a-z A-Z]{3,50}$)")))
            {
                if ((Regex.IsMatch(signUpDistUsername.Text, @"(^[a-zA-Z-_0-9]{5,50}$)")))
                {
                    if ((Regex.IsMatch(signUpDistPassword.Text, @"(^[a-zA-Z-_0-9@!#$%^&*]{6,50}$)")))
                    {
                        if (signUpDistPassword.Text == signUpDistConfirmPassword.Text)
                        {
                            if ((Regex.IsMatch(signUpDistPhoneNumber.Text, @"(^[0]{1}[0-9]{10}$)")))
                            {
                                if (signUpDistRegion.SelectedIndex != 0)
                                {
                                    if ((Regex.IsMatch(signUpDistAddress.Text, @"(^[a-zA-Z -0-9]{1,50}$)")))
                                    {
                                        if ((Regex.IsMatch(signUpDistAccountNumber.Text, @"(^[0-9]{16,20}$)")))
                                        {
                                            var con = Configuration.getInstance().getConnection();
                                            SqlCommand CheckUserName = new SqlCommand("select UserNanme from Distributors where UserNanme=@username", con);
                                            CheckUserName.Parameters.AddWithValue("@username", signUpDistUsername.Text);
                                            CheckUserName.ExecuteNonQuery();
                                            if (CheckUserName.ExecuteScalar() == null)
                                            {

                                                SqlCommand cmds = new SqlCommand("BEGIN TRAN SignUpDistributor;", con);
                                                SqlCommand cmd1 = new SqlCommand("INSERT INTO Person(PersonType) VALUES ((SELECT ID FROM LookUp WHERE Value = 'Distributor'));", con);
                                                SqlCommand cmd2 = new SqlCommand("SELECT MAX(ID) FROM Person", con);
                                                SqlCommand cmd3 = new SqlCommand("EXEC sp_getSelectedRegionFromComboBox @Value = @Region", con);
                                                SqlCommand cmd4 = new SqlCommand("INSERT INTO Account(AccountNo , HolderID , Balance) VALUES (@AccountNo1 , @HolderID1 , 0) ;INSERT INTO Distributors VALUES (@DisRegNo ,@HolderID2 , @Username , @Password , @Name , @Region , @AccountNo2 , @Address , @PhoneNumber);", con);
                                                SqlCommand cmde = new SqlCommand("COMMIT TRAN SignUpDistributor", con);
                                                cmds.ExecuteNonQuery();
                                                cmd1.ExecuteNonQuery();
                                                var HolderID = Convert.ToInt32(cmd2.ExecuteScalar());
                                                cmd4.Parameters.AddWithValue("@AccountNo1", signUpDistAccountNumber.Text);
                                                cmd4.Parameters.AddWithValue("@AccountNo2", signUpDistAccountNumber.Text);

                                                cmd4.Parameters.AddWithValue("@HolderID1", HolderID);
                                                cmd4.Parameters.AddWithValue("@HolderID2", HolderID);

                                                cmd4.Parameters.AddWithValue("@DisRegNo", DateTime.Now.Year.ToString() + "-DI-" + HolderID.ToString("0000"));
                                                cmd4.Parameters.AddWithValue("@Username", signUpDistUsername.Text);
                                                cmd4.Parameters.AddWithValue("@Password", signUpDistPassword.Text);
                                                cmd4.Parameters.AddWithValue("@Name", signUpDistName.Text);
                                                cmd3.Parameters.AddWithValue("@Region", signUpDistRegion.SelectedValue);
                                                cmd4.Parameters.AddWithValue("@Region", Convert.ToInt32(cmd3.ExecuteScalar()));
                                                cmd4.Parameters.AddWithValue("@Address", signUpDistAddress.Text);
                                                cmd4.Parameters.AddWithValue("@PhoneNumber", signUpDistPhoneNumber.Text);

                                                cmd2.ExecuteNonQuery();
                                                cmd3.ExecuteNonQuery();
                                                cmd4.ExecuteNonQuery();
                                                cmde.ExecuteNonQuery();
                                                MessageBox.Show("Successfully created account");

                                                signUpDistAccountNumber.Text = "";
                                                signUpDistAddress.Text = "";
                                                signUpDistName.Text = "";
                                                signUpDistRegion.SelectedIndex = 0;
                                                signUpDistPassword.Text = "";
                                                signUpDistConfirmPassword.Text = "";
                                                signUpDistPhoneNumber.Text = "";
                                                signUpDistUsername.Text = "";

                                                LoginSignupPage4000.SetPage("sign_in");
                                            }
                                            else
                                            {
                                                MessageBox.Show("Person with this username is already registered");

                                            }


                                        }
                                        else
                                        {
                                            MessageBox.Show("Account Number should contain only digits from 0 to 9 and its length should be more than 15 digits", "Invalid Account Number");
                                        }
                                    }
                                    else
                                    {
                                        MessageBox.Show("Address can contain only of alphabets, digits 0-9, dashes and should not be empty", "Invalid Address");
                                    }
                                }
                                else
                                {
                                    MessageBox.Show("Select a region", "Unselected Region");
                                }
                            }
                            else
                            {
                                MessageBox.Show("Phone Number should contain 11 digits starting with a 0 containing no space or dash", "Invalid Phone Number");
                            }
                        }
                        else
                        {
                            MessageBox.Show("Confirmed password should match the entered password", "Alert");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Password should be of length greater than 5 and contain alphabets, digits and special characters", "Incorrect password format");
                    }
                }
                else
                {
                    MessageBox.Show("Username should contain only of alphabets, digits, dashes and underscores", "Invalid Username");
                }
            }
            else
            {
                MessageBox.Show("Name should be of length greater than 2 and contain only of alphabets", "Invalid Name");
            }


        }   

        private void bunifuGradientPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void tabPage2_Click(object sender, EventArgs e)
        {

        }

        private void signUpDistRegion_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void signUpDistRegion_MouseClick(object sender, MouseEventArgs e)
        {

        }

        private void signUpDistPassword_Click(object sender, EventArgs e)
        {
            signUpDistPassword.PasswordChar = '●';
        }

        private void signUpDistAccountNumber_TextChanged(object sender, EventArgs e)
        {

        }

        private void signUpDistConfirmPassword_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
