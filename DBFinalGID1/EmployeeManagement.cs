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
using System.Text.RegularExpressions;

namespace DBFinalGID1
{

    public partial class EmployeeManagement : Form
    {
        int page=0;
        private static EmployeeManagement emp;
        private int empIDtodelete;
        private float fullpayment = 0;
        private SqlConnection con = Configuration.getInstance().getConnection();
        float CurrentCompanyBalance = 0;
        private EmployeeManagement()
        {
            InitializeComponent();
            EmployeeManagementPages.SetPage("MainScreenPage");

        }
        //
        //submenu settings start here
        //
        public static EmployeeManagement instance()
        {
            if (emp == null)
            {
                emp = new EmployeeManagement();
            }
            return emp;
        }

        private void HideAllSubMenu()
        {
            EmployeeSubMenuPanel.Visible = false;
        }

        private void showsubmenu(Panel currentPanel)
        {
            if (currentPanel.Visible == false)
            {

                HideAllSubMenu();
                currentPanel.Visible = true;
            }
            else
            {
                currentPanel.Visible = false;
            }
        }

        private void EmployeeMenuButton_Click(object sender, EventArgs e)
        {
            showsubmenu(EmployeeSubMenuPanel);
        }

        private void EmployeeManagement_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'allEmployeesDataSet.Employees' table. You can move, or remove it, as needed.
            this.employeesTableAdapter.Fill(this.allEmployeesDataSet.Employees);
            // TODO: This line of code loads data into the 'employeeTypeDataSet.LookUp' table. You can move, or remove it, as needed.
            this.lookUpTableAdapter.Fill(this.employeeTypeDataSet.LookUp);
            //fill Employee Type Combo Box
            fillByEmployeeTypeToolStripButton_Click(sender, e);
            fillByToolStripButton_Click(sender, e);
            fillByregionToolStripButton_Click(sender, e);
        }

        private void fillByEmployeeTypeToolStripButton_Click(object sender, EventArgs e)
        {
            try
            {
                this.lookUpTableAdapter.FillByEmployeeType(this.employeeTypeDataSet.LookUp);
            }
            catch (System.Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
            }

        }
        //
        //Function to reset text boxes for ADD EMPLOYEE SCREEN
        //
        private void ReSetAddEmpPageTextBoxes()
        {
            DepartmentComboBox.SelectedIndex = -1;
            EmpTypeComboBox.SelectedIndex = -1;
            GenderComboBox.SelectedIndex = -1;
            RegistrationNumberTextBox.Text = "";
            FNameTextBox.Text = "";
            LNameTextBox.Text = "";
            ContactTextBox.Text = "";
            EmailTextBox.Text = "";
            CNICTextBox.Text = "";
            AddressTextBox.Text = "";
            AccountNumTextBox.Text = "";
            SalaryTextBox.Text = "";
            SqlCommand cmd = new SqlCommand("SELECT MAX(ID) FROM Person", con);
            cmd.ExecuteNonQuery();
            int now = DateTime.Now.Year;
            if (cmd.ExecuteScalar().ToString() != "")
            {
                int roll = int.Parse(cmd.ExecuteScalar().ToString()) % 10000;
                if (roll + 1 < 10)
                {
                    RegistrationNumberTextBox.Text = now.ToString() + "-EM-000" + (roll + 1).ToString();
                }
                else if (roll + 1 < 100)
                {
                    RegistrationNumberTextBox.Text = now.ToString() + "-EM-00" + (roll + 1).ToString();

                }
                else if (roll + 1 < 1000)
                {
                    RegistrationNumberTextBox.Text = now.ToString() + "-EM-0" + (roll + 1).ToString();
                }
                else if (roll + 1 < 10000)
                {
                    RegistrationNumberTextBox.Text = now.ToString() + "-EM-" + (roll + 1).ToString();

                }
                else
                {
                    SqlCommand newcmd = new SqlCommand("SELECT Max(EmployeeRegNo) FROM Employees", con);
                    newcmd.ExecuteNonQuery();
                    if (newcmd.ExecuteScalar().ToString().Contains(DateTime.Now.Year.ToString()))
                    {
                        MessageBox.Show("You cannot add more employees for this year", "Maximum Employees reached");
                    }
                    else
                    {
                        RegistrationNumberTextBox.Text = now.ToString() + "-EM-0001";
                    }
                }
            }
            else
            {
                RegistrationNumberTextBox.Text = now.ToString() + "-EM-0001";
            }
        }

        private void AddEmployeeNavButton_Click(object sender, EventArgs e)
        {
            //
            //Sets Page
            //
            EmployeeManagementPages.SetPage("AddEmployeePage");
            HideAllSubMenu();
            //
            //Sets the text in windows heading
            //
            this.Text = "Add an Employee";
            //
            //initialises text boxes
            //
            ReSetAddEmpPageTextBoxes();
            this.Text = "Add an Employee";
        }

        private void RunQuery()
        {
            try
            {
                //
                //Beginning a transaction
                //
                AddEmployeeRecordButton.Enabled = false;
                SqlCommand cmd1 = new SqlCommand("SET TRANSACTION ISOLATION LEVEL SERIALIZABLE; BEGIN TRANSACTION", con);
                cmd1.ExecuteNonQuery();
                //
                //Query to get ID of PersonType Employee from Lookup
                //
                cmd1 = new SqlCommand("SELECT ID FROM LookUp WHERE Value = 'Employee' ", con);
                cmd1.ExecuteNonQuery();
                //
                //query to insert into Person Table
                //
                SqlCommand cmd = new SqlCommand("INSERT INTO Person VALUES (@PersonType)", con);
                cmd.Parameters.AddWithValue("@PersonType", int.Parse(cmd1.ExecuteScalar().ToString()));
                cmd.ExecuteNonQuery();
                //
                //query to insert into Account Table
                //
                cmd = new SqlCommand("INSERT INTO Account VALUES(@AccountNo, (SELECT MAX(ID) FROM Person) , @Balance)", con);
                cmd.Parameters.AddWithValue("@AccountNo", AccountNumTextBox.Text);
                cmd.Parameters.AddWithValue("@Balance", 0);
                cmd.ExecuteNonQuery();
                //
                //query to insert into Employee Table
                //
                cmd = new SqlCommand("INSERT INTO Employees Values ( (SELECT MAX(ID) FROM Person) ,  @EmployeeRegNo, @FirstName, @LastName, @CNIC, @Gender, @Address, @PhoneNumber, @AccountNo1,  @DepartmentID, @EmployeeType, @Email, @DateOfJoining, @NetSalary)", con);
                cmd.Parameters.AddWithValue("@EmployeeRegNo", RegistrationNumberTextBox.Text);
                cmd.Parameters.AddWithValue("@FirstName", FNameTextBox.Text);
                cmd.Parameters.AddWithValue("@LastName", LNameTextBox.Text);
                cmd.Parameters.AddWithValue("@CNIC", CNICTextBox.Text);
                //
                //check  for gender
                //
                if (GenderComboBox.Text == "Male")
                {
                    cmd.Parameters.AddWithValue("@Gender", 'M');
                }
                else if (GenderComboBox.Text == "Female")
                {
                    cmd.Parameters.AddWithValue("@Gender", 'F');
                }
                else
                {
                    cmd.Parameters.AddWithValue("@Gender", 'U');
                }

                cmd.Parameters.AddWithValue("@Address", AddressTextBox.Text);
                cmd.Parameters.AddWithValue("@PhoneNumber", ContactTextBox.Text);
                cmd.Parameters.AddWithValue("@AccountNo1", AccountNumTextBox.Text);
                //
                //check for employee type
                //
                SqlCommand getTypeId = new SqlCommand("SELECT ID FROM LookUp WHERE Category = 'EmployeeType' AND Value = @EmployeeTypeID", con);
                getTypeId.Parameters.AddWithValue("@EmployeeTypeID", EmpTypeComboBox.Text);
                getTypeId.ExecuteNonQuery();
                cmd.Parameters.AddWithValue("@EmployeeType", int.Parse(getTypeId.ExecuteScalar().ToString()));
                //
                //getting Department ID
                //
                string depttype = "";
                if (EmpTypeComboBox.Text == "HR Officer" || EmpTypeComboBox.Text == "Training Officer" || EmpTypeComboBox.Text == "HR Manager" || EmpTypeComboBox.Text == "HR Exceutive" || EmpTypeComboBox.Text == "Recruiter")
                {
                    depttype = "HR";
                }
                else if (EmpTypeComboBox.Text == "Packing Supervisor" || EmpTypeComboBox.Text == "Packer")
                {
                    depttype = "Packing and Processing";
                }
                else if (EmpTypeComboBox.Text == "Analyst" || EmpTypeComboBox.Text == "QC Manager")
                {
                    depttype = "Quality Control";
                }
                else if (EmpTypeComboBox.Text == "Janitor" || EmpTypeComboBox.Text == "Maintenance Supervisor" || EmpTypeComboBox.Text == "Peon")
                {
                    depttype = "Maintenance";
                }
                else if (EmpTypeComboBox.Text == "Accountant")
                {
                    depttype = "Finances";
                }
                else
                {
                    depttype = "Distribution";
                }
                getTypeId = new SqlCommand("SELECT ID FROM LookUp WHERE Category = 'Department' AND Value = @DeptType ", con);
                getTypeId.Parameters.AddWithValue("DeptType", depttype);
                getTypeId.ExecuteNonQuery();

                cmd.Parameters.AddWithValue("@DepartmentID", int.Parse(getTypeId.ExecuteScalar().ToString()));
                cmd.Parameters.AddWithValue("@Email", EmailTextBox.Text);
                cmd.Parameters.AddWithValue("@DateOfJoining", DOJDatePicker.Value);
                cmd.Parameters.AddWithValue("@NetSalary", float.Parse(SalaryTextBox.Text));
                cmd.ExecuteNonQuery();
                //
                //Committing transaction
                //
                cmd = new SqlCommand("COMMIT TRANSACTION", con);
                cmd.ExecuteNonQuery();
                AddEmployeeRecordButton.Enabled = true;
                ReSetAddEmpPageTextBoxes();

            }
            catch (SqlException ex)
            {
                if (ex.Message.Contains("Account"))
                {
                    MessageBox.Show("Duplication in Account Number");
                }
                if (ex.Message.Contains("UQ")) {
                    MessageBox.Show("Duplication in Either Contact Number, CNIC or Email ");
                }
                SqlCommand cmd = new SqlCommand("ROLLBACK TRANSACTION", con);
                cmd.ExecuteNonQuery();
                AddEmployeeRecordButton.Enabled = true;
            }
        }

        private void AddEmployeeRecordButton_Click(object sender, EventArgs e)
        {
            if (Regex.IsMatch(FNameTextBox.Text, @"(^[a-zA-Z]{3,50}$)"))
            {
                if (Regex.IsMatch(LNameTextBox.Text, @"(^[a-zA-Z]{3,50}$)"))
                {
                    if (Regex.IsMatch(ContactTextBox.Text, @"(^[0-9]{11}$)"))
                    {
                        if (Regex.IsMatch(EmailTextBox.Text, @"(^([a-zA-Z0-9_\-\.]+)@([a-zA-Z0-9_\-\.]+)\.([a-zA-Z]{2,5})$)"))
                        {
                            if (Regex.IsMatch(CNICTextBox.Text, @"(^([0-9]{5}[-]{1}[0-9]{7}[-]{1}[0-9]{1})$)"))
                            {
                                if (Regex.IsMatch(AccountNumTextBox.Text, @"(^([0-9]{7,20})$)"))
                                {
                                    if (Regex.IsMatch(SalaryTextBox.Text, @"(^([0-9]{5,20})$)") && int.Parse(SalaryTextBox.Text) >= 15000)
                                    {
                                        if (EmpTypeComboBox.SelectedIndex != -1)
                                        {
                                            if (int.Parse(DOJDatePicker.Value.Year.ToString()) >= 2012)
                                            {
                                                RunQuery();

                                            }
                                            else
                                            {
                                                MessageBox.Show("please select an appropriate Date of joining", "Invalid Date");
                                            }
                                        }
                                        else
                                        {
                                            MessageBox.Show("Please select the appropriate designation", "Invalid Designation");
                                        }
                                    }
                                    else
                                    {
                                        MessageBox.Show("Please enter a salary of atleast 15000", "Invalid Salary");
                                    }
                                }
                                else
                                {
                                    MessageBox.Show("Please enter an account number of 7-20 digits", "Invalid account Number");
                                }
                            }
                            else
                            {
                                MessageBox.Show("Please Enter a CNIC of form: \n 12345-1234567-1", "Invalid CNIC Number");
                            }
                        }
                        else
                        {
                            MessageBox.Show("Please enter a valid email address", "Invalid Email");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Enter a Contact Number of 11 Digits without any - or +", "Incorrect Contact Number");
                    }
                }
                else
                {
                    MessageBox.Show("Enter a name with only alphabets of atleast 3 characters", "Invalid Last Name");
                }
            }
            else
            {
                MessageBox.Show("Enter a name with only alphabets of atleast 3 characters", "Invalid First Name");
            }
        }

        private void DeptChange(object sender, EventArgs e)
        {
            if (EmpTypeComboBox.SelectedIndex == 0 || EmpTypeComboBox.SelectedIndex == 1 || EmpTypeComboBox.SelectedIndex == 2 || EmpTypeComboBox.SelectedIndex == 3 || EmpTypeComboBox.SelectedIndex == 4)
            {
                DepartmentComboBox.SelectedIndex = 0;
            }
            else if (EmpTypeComboBox.SelectedIndex == 5 || EmpTypeComboBox.SelectedIndex == 6)
            {
                DepartmentComboBox.SelectedIndex = 1;
            }
            else if (EmpTypeComboBox.SelectedIndex == 7 || EmpTypeComboBox.SelectedIndex == 8)
            {
                DepartmentComboBox.SelectedIndex = 2;
            }
            else if (EmpTypeComboBox.SelectedIndex == 9 || EmpTypeComboBox.SelectedIndex == 10 || EmpTypeComboBox.SelectedIndex == 11)
            {
                DepartmentComboBox.SelectedIndex = 3;
            }
            else if (EmpTypeComboBox.SelectedIndex == 12)
            {
                DepartmentComboBox.SelectedIndex = 4;
            }
            else
            {
                DepartmentComboBox.SelectedIndex = 5;
            }
        }
        //
        //UpdateEmployee
        //
        private void UpdateEmployeeNavButton_Click(object sender, EventArgs e)
        {
            EmployeeManagementPages.SetPage("UpdateEmployeePage");
            HideAllSubMenu();
            this.Text = "Update Employee Record";

            ReSetUpdEmpPageTextBoxes();
            this.Text = "Update Employee Records";
        }

        private void ReSetUpdEmpPageTextBoxes()
        {
            UpdateRegBox.Text = "";
            UpdateFNameBox.Text = "";
            UpdateLNameBox.Text = "";
            UpdateContactBox.Text = "";
            UpdateEmailBox.Text = "";
            UpdateCNICBox.Text = "";
            UpdateAddressBox.Text = "";
            UpdateAccNumBox.Text = "";
            UpdateSalaryBox.Text = "";
            UpdateGenderBox.SelectedIndex = -1;
            UpdateEmpTypeBox.SelectedIndex = -1;
        }

        private void SearchEmployeeRecordButton_Click(object sender, EventArgs e)
        {
            try
            {
                var con = Configuration.getInstance().getConnection();
                SqlCommand cmd = new SqlCommand("SELECT * FROM Employees WHERE EmployeeRegNo = @EmployeeRegNo", con);
                cmd.Parameters.AddWithValue("@EmployeeRegNo", UpdateRegBox.Text);
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                sda.Fill(dt);
                empIDtodelete = int.Parse(dt.Rows[0][0].ToString());
                UpdateFNameBox.Text = dt.Rows[0][2].ToString();
                UpdateLNameBox.Text = dt.Rows[0][3].ToString();
                UpdateCNICBox.Text = dt.Rows[0][4].ToString();
                if (dt.Rows[0][5].ToString() == "M")
                {
                    UpdateGenderBox.SelectedIndex = 0;
                }
                else if (dt.Rows[0][5].ToString() == "F")
                {
                    UpdateGenderBox.SelectedIndex = 1;
                }
                else
                {
                    UpdateGenderBox.SelectedIndex = 2;
                }
                UpdateAddressBox.Text = dt.Rows[0][6].ToString();
                UpdateContactBox.Text = dt.Rows[0][7].ToString();

                UpdateAccNumBox.Text = dt.Rows[0][8].ToString();
                if (dt.Rows[0][10].ToString() == "7")
                {
                    UpdateEmpTypeBox.SelectedIndex = 0;
                }
                else if (dt.Rows[0][10].ToString() == "8")
                {
                    UpdateEmpTypeBox.SelectedIndex = 1;
                }
                else if (dt.Rows[0][10].ToString() == "9")
                {
                    UpdateEmpTypeBox.SelectedIndex = 2;
                }
                else if (dt.Rows[0][10].ToString() == "10")
                {
                    UpdateEmpTypeBox.SelectedIndex = 3;
                }
                else if (dt.Rows[0][10].ToString() == "11")
                {
                    UpdateEmpTypeBox.SelectedIndex = 4;
                }
                else if (dt.Rows[0][10].ToString() == "12")
                {
                    UpdateEmpTypeBox.SelectedIndex = 5;
                }
                else if (dt.Rows[0][10].ToString() == "13")
                {
                    UpdateEmpTypeBox.SelectedIndex = 6;
                }
                else if (dt.Rows[0][10].ToString() == "14")
                {
                    UpdateEmpTypeBox.SelectedIndex = 7;
                }
                else if (dt.Rows[0][10].ToString() == "15")
                {
                    UpdateEmpTypeBox.SelectedIndex = 8;
                }
                else if (dt.Rows[0][10].ToString() == "16")
                {
                    UpdateEmpTypeBox.SelectedIndex = 9;
                }
                else if (dt.Rows[0][10].ToString() == "17")
                {
                    UpdateEmpTypeBox.SelectedIndex = 10;
                }
                else if (dt.Rows[0][10].ToString() == "18")
                {
                    UpdateEmpTypeBox.SelectedIndex = 11;
                }
                else if (dt.Rows[0][10].ToString() == "19")
                {
                    UpdateEmpTypeBox.SelectedIndex = 12;
                }
                else
                {
                    UpdateEmpTypeBox.SelectedIndex = 13;
                }

                UpdateEmailBox.Text = dt.Rows[0][11].ToString();
                UpdateDOJ.Text = dt.Rows[0][12].ToString();
                UpdateSalaryBox.Text = dt.Rows[0][13].ToString();


                UpdateEmployeeSeparateValuesButton.Enabled = false;
                SearchEmployeeRecordButton.Enabled = false;
                UpdateValuesButtonEnablesFields.Enabled = true;
                DeleteEmployeeRecordButton.Enabled = true;

            }
            catch (Exception)
            {
                MessageBox.Show("The Registration number doesnt exist in records", "Invalid Registration Number");
            }
        }

        private void UpdateValuesButtonEnablesFields_Click(object sender, EventArgs e)
        {
            enableUpdateBoxes();
        }

        private void enableUpdateBoxes()
        {
            UpdateRegBox.Enabled = false;
            UpdateFNameBox.Enabled = true;
            UpdateLNameBox.Enabled = true;
            UpdateCNICBox.Enabled = true;
            UpdateAddressBox.Enabled = true;
            UpdateContactBox.Enabled = true;
            UpdateEmailBox.Enabled = true;
            UpdateEmpTypeBox.Enabled = true;
            UpdateGenderBox.Enabled = true;
            UpdateSalaryBox.Enabled = true;
            DeleteEmployeeRecordButton.Enabled = false;
            UpdateValuesButtonEnablesFields.Enabled = false;
            UpdateEmployeeRecordButton.Enabled = true;
        }

        private void DisableUpdateBoxes()
        {
            UpdateEmployeeRecordButton.Enabled = false;
            SearchEmployeeRecordButton.Enabled = true;
            UpdateEmployeeSeparateValuesButton.Enabled = true;
            UpdateRegBox.Enabled = true;
            UpdateFNameBox.Enabled = false;
            UpdateLNameBox.Enabled = false;
            UpdateCNICBox.Enabled = false;
            UpdateAddressBox.Enabled = false;
            UpdateContactBox.Enabled = false;
            UpdateEmailBox.Enabled = false;
            UpdateEmpTypeBox.Enabled = false;
            UpdateGenderBox.Enabled = false;
            UpdateSalaryBox.Enabled = false;
        }
        //
        //Update all attributes
        //
        private void UpdateEmployeeRecordButton_Click(object sender, EventArgs e)
        {
            if (Regex.IsMatch(UpdateFNameBox.Text, @"(^[a-zA-Z]{3,50}$)"))
            {
                if (Regex.IsMatch(UpdateLNameBox.Text, @"(^[a-zA-Z]{3,50}$)"))
                {
                    if (Regex.IsMatch(UpdateContactBox.Text, @"(^[0-9]{11,13}$)"))
                    {
                        if (Regex.IsMatch(UpdateEmailBox.Text, @"(^([a-zA-Z0-9_\-\.]+)@([a-zA-Z0-9_\-\.]+)\.([a-zA-Z]{2,5})$)"))
                        {
                            if (Regex.IsMatch(UpdateCNICBox.Text, @"(^([0-9]{5}[-]{1}[0-9]{7}[-]{1}[0-9]{1})$)"))
                            {
                                if (Regex.IsMatch(UpdateAccNumBox.Text, @"(^([0-9]{7,20})$)"))
                                {
                                    if (Regex.IsMatch(UpdateSalaryBox.Text, @"(^([0-9]{5,20})$)") && int.Parse(UpdateSalaryBox.Text) >= 15000)
                                    {
                                        //
                                        //SQL query here to add the record
                                        //
                                        if (int.Parse(UpdateDOJ.Value.Year.ToString()) >= 2012)
                                        {
                                            try
                                            {
                                                var con = Configuration.getInstance().getConnection();
                                                SqlCommand cmd1 = new SqlCommand("SET TRANSACTION ISOLATION LEVEL SERIALIZABLE; BEGIN TRANSACTION", con);
                                                cmd1.ExecuteNonQuery();
                                                cmd1 = new SqlCommand("SELECT ID FROM LookUp WHERE Value = 'Employee' ", con);
                                                cmd1.ExecuteNonQuery();
                                                int empId = int.Parse(cmd1.ExecuteScalar().ToString());
                                                SqlCommand cmd = new SqlCommand("exec spUpdateAllEmployeeAttributes @EmpID ,@EmpRegNo,@Fname ,@Lname , @CNIC , @Gender , @Address, @PhoneNumber, @DepartmentID, @EmployeeType, @Email, @DateOfJoining, @NetSalary ", con);

                                                cmd.Parameters.AddWithValue("@EmpID", empIDtodelete);


                                                cmd.Parameters.AddWithValue("@EmpRegNo", UpdateRegBox.Text);
                                                cmd.Parameters.AddWithValue("@FName", UpdateFNameBox.Text);
                                                cmd.Parameters.AddWithValue("@LName", UpdateLNameBox.Text);
                                                cmd.Parameters.AddWithValue("@CNIC", UpdateCNICBox.Text);

                                                if (UpdateGenderBox.Text == "Male")
                                                {
                                                    cmd.Parameters.AddWithValue("@Gender", 'M');
                                                }
                                                else if (UpdateGenderBox.Text == "Female")
                                                {
                                                    cmd.Parameters.AddWithValue("@Gender", 'F');
                                                }
                                                else
                                                {
                                                    cmd.Parameters.AddWithValue("@Gender", 'U');
                                                }

                                                cmd.Parameters.AddWithValue("@Address", UpdateAddressBox.Text);
                                                cmd.Parameters.AddWithValue("@PhoneNumber", UpdateContactBox.Text);
                                                //
                                                //check for employee type
                                                //
                                                if (UpdateEmpTypeBox.Text == "HR Officer")
                                                {
                                                    cmd.Parameters.AddWithValue("@EmployeeType", 7);
                                                    cmd.Parameters.AddWithValue("@DepartmentID", 1);
                                                }
                                                else if (UpdateEmpTypeBox.Text == "Training Officer")
                                                {
                                                    cmd.Parameters.AddWithValue("@EmployeeType", 8);
                                                    cmd.Parameters.AddWithValue("@DepartmentID", 1);
                                                }
                                                else if (UpdateEmpTypeBox.Text == "HR Executive")
                                                {
                                                    cmd.Parameters.AddWithValue("@EmployeeType", 9);
                                                    cmd.Parameters.AddWithValue("@DepartmentID", 1);
                                                }
                                                else if (UpdateEmpTypeBox.Text == "HR Manager")
                                                {
                                                    cmd.Parameters.AddWithValue("@EmployeeType", 10);
                                                    cmd.Parameters.AddWithValue("@DepartmentID", 1);
                                                }
                                                else if (UpdateEmpTypeBox.Text == "Recruiter")
                                                {
                                                    cmd.Parameters.AddWithValue("@EmployeeType", 11);
                                                    cmd.Parameters.AddWithValue("@DepartmentID", 1);
                                                }
                                                else if (UpdateEmpTypeBox.Text == "Packing Supervisor")
                                                {
                                                    cmd.Parameters.AddWithValue("@EmployeeType", 12);
                                                    cmd.Parameters.AddWithValue("@DepartmentID", 2);
                                                }
                                                else if (UpdateEmpTypeBox.Text == "Packer")
                                                {
                                                    cmd.Parameters.AddWithValue("@EmployeeType", 13);
                                                    cmd.Parameters.AddWithValue("@DepartmentID", 2);
                                                }
                                                else if (UpdateEmpTypeBox.Text == "Analyst")
                                                {
                                                    cmd.Parameters.AddWithValue("@EmployeeType", 14);
                                                    cmd.Parameters.AddWithValue("@DepartmentID", 4);
                                                }
                                                else if (UpdateEmpTypeBox.Text == "QC Manager")
                                                {
                                                    cmd.Parameters.AddWithValue("@EmployeeType", 15);
                                                    cmd.Parameters.AddWithValue("@DepartmentID", 4);
                                                }
                                                else if (UpdateEmpTypeBox.Text == "Janitor")
                                                {
                                                    cmd.Parameters.AddWithValue("@EmployeeType", 16);
                                                    cmd.Parameters.AddWithValue("@DepartmentID", 6);
                                                }
                                                else if (UpdateEmpTypeBox.Text == "Maintenance Supervisor")

                                                {
                                                    cmd.Parameters.AddWithValue("@EmployeeType", 17);
                                                    cmd.Parameters.AddWithValue("@DepartmentID", 6);
                                                }
                                                else if (UpdateEmpTypeBox.Text == "Peon")
                                                {
                                                    cmd.Parameters.AddWithValue("@EmployeeType", 18);
                                                    cmd.Parameters.AddWithValue("@DepartmentID", 6);
                                                }
                                                else if (UpdateEmpTypeBox.Text == "Accountant")
                                                {
                                                    cmd.Parameters.AddWithValue("@EmployeeType", 19);
                                                    cmd.Parameters.AddWithValue("@DepartmentID", 5);
                                                }
                                                else if (UpdateEmpTypeBox.Text == "Distribution Supervisor")
                                                {
                                                    cmd.Parameters.AddWithValue("@EmployeeType", 20);
                                                    cmd.Parameters.AddWithValue("@DepartmentID", 3);

                                                }
                                                cmd.Parameters.AddWithValue("@Email", UpdateEmailBox.Text);
                                                cmd.Parameters.AddWithValue("@DateOfJoining", UpdateDOJ.Value);
                                                cmd.Parameters.AddWithValue("@NetSalary", float.Parse(UpdateSalaryBox.Text));
                                                cmd.ExecuteNonQuery();
                                                cmd1 = new SqlCommand("COMMIT", con);
                                                cmd1.ExecuteNonQuery();
                                                DisableUpdateBoxes();
                                                ReSetUpdEmpPageTextBoxes();
                                            }
                                            catch (SqlException ex)
                                            {
                                                MessageBox.Show(ex.Message);
                                            }
                                        }
                                        else
                                        {
                                            MessageBox.Show("Please Select an appropriate date", "Invalid Date");
                                        }
                                    }
                                    else
                                    {
                                        MessageBox.Show("Please enter a salary of atleast 15000", "Invalid Salary");
                                    }
                                }
                                else
                                {
                                    MessageBox.Show("Please enter an account number of 7-20 digits", "Invalid account Number");
                                }
                            }
                            else
                            {
                                MessageBox.Show("Please Enter a CNIC of form: \n 12345-1234567-1", "Invalid CNIC Number");
                            }
                        }
                        else
                        {
                            MessageBox.Show("Please enter a valid email address", "Invalid Email");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Enter a Contact Number of 11-13 Digits without any - or +", "Incorrect Contact Number");
                    }
                }
                else
                {
                    MessageBox.Show("Enter a name with only alphabets of atleast 3 characters", "Invalid Last Name");
                }
            }
            else
            {
                MessageBox.Show("Enter a name with only alphabets of atleast 3 characters", "Invalid First Name");
            }
        }
        //
        //Updating specific values for employee screen
        //
        private void UpdateEmployeeSeparateValuesButton_Click(object sender, EventArgs e)
        {
            EmployeeManagementPages.SetPage("UpdateSpecificEmpVals");
            UpdateSpecificEmpTypeCombo.SelectedIndex = -1;
        }
        private void UpdateEmployeeRecordSpecificValuesButton_Click(object sender, EventArgs e)
        {
            string query = "UPDATE Employees SET ";
            string actual = "UPDATE Employees SET ";

            if (FirstNameSpecific.Text != "")
            {
                query += ", FirstName = @FirstName";
            }
            if (LastNameSpecific.Text != "")
            {
                query += ", LastName = @LastName";
            }
            if (CNICSpecific.Text != "")
            {
                query += ", CNIC = @CNIC";
            }
            if (GenderSpecificCombo.Text != "")
            {
                query += ", Gender = @Gender";
            }
            if (AddressSpecific.Text != "")
            {
                query += ", Address = @Address";
            }
            if (ContactSpecific.Text != "")
            {
                query += ", PhoneNumber = @PhoneNumber";
            }
            if (UpdateSpecificEmpTypeCombo.Text != "")
            {
                query += ", EmployeeType = @EmployeeType , DepartmentID = @DepartmentID";
            }
            if (EmailSpecific.Text != "")
            {
                query += ", Email = @Email";
            }

            if (SalarySpecific.Text != "")
            {
                query += ", NetSalary = @Salary";
            }


            try
            {
                actual += query.Substring(23) + " WHERE EmployeeRegNo = @EmpRegNo";
                SqlCommand cmd1 = new SqlCommand("SET TRANSACTION ISOLATION LEVEL SERIALIZABLE; BEGIN TRANSACTION", con);
                cmd1.ExecuteNonQuery();
                SqlCommand cmd = new SqlCommand(actual, con);
                cmd.Parameters.AddWithValue("@EmpRegNo", UpdateSpecifcRegBox.Text);
                if (FirstNameSpecific.Text != "")
                {
                    cmd.Parameters.AddWithValue("@FirstName", FirstNameSpecific.Text);
                }
                if (LastNameSpecific.Text != "")
                {
                    cmd.Parameters.AddWithValue("@LastName", LastNameSpecific.Text);
                }
                if (CNICSpecific.Text != "")
                {
                    cmd.Parameters.AddWithValue("@CNIC", CNICSpecific.Text);
                }
                if (GenderSpecificCombo.Text != "")
                {
                    if (GenderSpecificCombo.Text == "Male")
                    {
                        cmd.Parameters.AddWithValue("@Gender", 'M');
                    }
                    else if (GenderSpecificCombo.Text == "Female")
                    {
                        cmd.Parameters.AddWithValue("@Gender", 'F');
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@Gender", 'U');
                    }
                }
                if (AddressSpecific.Text != "")
                {
                    cmd.Parameters.AddWithValue("@Address", AddressSpecific.Text);
                }
                if (ContactSpecific.Text != "")
                {
                    cmd.Parameters.AddWithValue("@PhoneNumber", ContactSpecific.Text);
                }
                if (UpdateSpecificEmpTypeCombo.Text != "")
                {
                    if (UpdateSpecificEmpTypeCombo.Text == "HR Officer")
                    {
                        cmd.Parameters.AddWithValue("@EmployeeType", 7);
                        cmd.Parameters.AddWithValue("@DepartmentID", 1);
                    }
                    else if (UpdateSpecificEmpTypeCombo.Text == "Training Officer")
                    {
                        cmd.Parameters.AddWithValue("@EmployeeType", 8);
                        cmd.Parameters.AddWithValue("@DepartmentID", 1);
                    }
                    else if (UpdateSpecificEmpTypeCombo.Text == "HR Executive")
                    {
                        cmd.Parameters.AddWithValue("@EmployeeType", 9);
                        cmd.Parameters.AddWithValue("@DepartmentID", 1);
                    }
                    else if (UpdateSpecificEmpTypeCombo.Text == "HR Manager")
                    {
                        cmd.Parameters.AddWithValue("@EmployeeType", 10);
                        cmd.Parameters.AddWithValue("@DepartmentID", 1);
                    }
                    else if (UpdateSpecificEmpTypeCombo.Text == "Recruiter")
                    {
                        cmd.Parameters.AddWithValue("@EmployeeType", 11);
                        cmd.Parameters.AddWithValue("@DepartmentID", 1);
                    }
                    else if (UpdateSpecificEmpTypeCombo.Text == "Packing Supervisor")
                    {
                        cmd.Parameters.AddWithValue("@EmployeeType", 12);
                        cmd.Parameters.AddWithValue("@DepartmentID", 2);
                    }
                    else if (UpdateSpecificEmpTypeCombo.Text == "Packer")
                    {
                        cmd.Parameters.AddWithValue("@EmployeeType", 13);
                        cmd.Parameters.AddWithValue("@DepartmentID", 2);
                    }
                    else if (UpdateSpecificEmpTypeCombo.Text == "Analyst")
                    {
                        cmd.Parameters.AddWithValue("@EmployeeType", 14);
                        cmd.Parameters.AddWithValue("@DepartmentID", 4);
                    }
                    else if (UpdateSpecificEmpTypeCombo.Text == "QC Manager")
                    {
                        cmd.Parameters.AddWithValue("@EmployeeType", 15);
                        cmd.Parameters.AddWithValue("@DepartmentID", 4);
                    }
                    else if (UpdateSpecificEmpTypeCombo.Text == "Janitor")
                    {
                        cmd.Parameters.AddWithValue("@EmployeeType", 16);
                        cmd.Parameters.AddWithValue("@DepartmentID", 6);
                    }
                    else if (UpdateSpecificEmpTypeCombo.Text == "Maintenance Supervisor")

                    {
                        cmd.Parameters.AddWithValue("@EmployeeType", 17);
                        cmd.Parameters.AddWithValue("@DepartmentID", 6);
                    }
                    else if (UpdateSpecificEmpTypeCombo.Text == "Peon")
                    {
                        cmd.Parameters.AddWithValue("@EmployeeType", 18);
                        cmd.Parameters.AddWithValue("@DepartmentID", 6);
                    }
                    else if (UpdateSpecificEmpTypeCombo.Text == "Accountant")
                    {
                        cmd.Parameters.AddWithValue("@EmployeeType", 19);
                        cmd.Parameters.AddWithValue("@DepartmentID", 5);
                    }
                    else if (UpdateSpecificEmpTypeCombo.Text == "Distribution Supervisor")
                    {
                        cmd.Parameters.AddWithValue("@EmployeeType", 20);
                        cmd.Parameters.AddWithValue("@DepartmentID", 3);

                    }
                }
                if (EmailSpecific.Text != "")
                {
                    cmd.Parameters.AddWithValue("@Email", EmailSpecific.Text);
                }

                if (SalarySpecific.Text != "")
                {
                    cmd.Parameters.AddWithValue("@Salary", SalarySpecific.Text);
                }
                cmd.ExecuteNonQuery();
                cmd1 = new SqlCommand("COMMIT", con);
                cmd1.ExecuteNonQuery();

                UpdateEmployeeNavButton_Click(sender, e);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                SqlCommand cmd1 = new SqlCommand("ROLLBACK", con);
                cmd1.ExecuteNonQuery();
            }
        }
        //
        //Delete employee record
        //
        private void DeleteEmployeeRecordButton_Click(object sender, EventArgs e)
        {
            SqlCommand begintran = new SqlCommand("SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;  BEGIN TRANSACTION", con);
            begintran.ExecuteNonQuery();
            SqlCommand cmd = new SqlCommand("DELETE FROM Employees WHERE EmployeeRegNo = @EmpRegNo", con);
            cmd.Parameters.AddWithValue("@EmpRegNo", UpdateRegBox.Text);
            cmd.ExecuteNonQuery();
            SqlCommand commit = new SqlCommand("COMMIT", con);
            commit.ExecuteNonQuery();
            DisableUpdateBoxes();
            ReSetUpdEmpPageTextBoxes();
            DeleteEmployeeRecordButton.Enabled = false;
            UpdateValuesButtonEnablesFields.Enabled = false;
        }
        //
        //ShowEmployee Screen
        //
        private void ShowEmployeesNavButton_Click(object sender, EventArgs e)
        {
            EmployeeManagementPages.SetPage("ShowEmp");
            fillAllEmpTable();
            HideAllSubMenu();
            this.Text = "Show All Employees";
        }

        private void fillAllEmpTable()
        {
            SqlCommand cmd = new SqlCommand("SELECT TOP (35) EmployeeRegNo, FirstName, LastName, CNIC, CASE WHEN Gender = 'M' THEN 'Male' WHEN Gender = 'F' THEN 'Female' Else 'U' END Gender, Address, PhoneNumber, AccountNo, CASE WHEN DepartmentID = '1' THEN 'HR' WHEN DepartmentID = '2' THEN 'Packing and Processing' WHEN DepartmentID = 3 THEN 'Distribution' WHEN DepartmentID = 4 THEN 'Quality Control' WHEN DepartmentID = '5' THEN 'Finances' ELSE 'Maintenance' END Department, CASE WHEN EmployeeType = 7 THEN 'HR Officer' WHEN EmployeeType = 8 THEN 'Training Officer' WHEN EmployeeType = 9 THEN 'HR Executive' WHEN EmployeeType = 10 THEN 'HR Manager' WHEN EmployeeType = 11 THEN 'Recruiter' WHEN EmployeeType = 12 THEN 'Packing Supervisor' WHEN EmployeeType = 13 THEN 'Packer'  WHEN EmployeeType = 14 THEN 'Analyst' WHEN EmployeeType = 15 THEN 'QC Manager' WHEN EmployeeType = 16 THEN 'Janitor'  WHEN EmployeeType = 17 THEN 'Maintenance Supervisor' WHEN EmployeeType = 18 THEN 'Peon' WHEN EmployeeType = 19 THEN 'Accountant' ELSE 'Distribution Supervisor' END Designation , Email, DateOfJoining, NetSalary FROM Employees", con);
            SqlDataAdapter sda = new SqlDataAdapter(cmd);

            DataTable dt = new DataTable();

            sda.Fill(dt);

            AllEmployeeDataGrid.DataSource = dt;
        }

        private void SortEmployeeDataButton_Click(object sender, EventArgs e)
        {
            page = 0;
            string orderby="";
            if (SortEmployeeComboBox.SelectedIndex != -1 && OrderInCombo.SelectedIndex != -1)
            {
                if (OrderInCombo.Text == "Ascending")
                {
                    orderby = "ASC";
                }
                else {
                    orderby = "DESC";
                }
                SqlCommand cmd = new SqlCommand("SELECT TOP (35) EmployeeRegNo, FirstName, LastName, CNIC, CASE WHEN Gender = 'M' THEN 'Male' WHEN Gender = 'F' THEN 'Female' Else 'U' END Gender, Address, PhoneNumber, AccountNo, CASE WHEN DepartmentID = '1' THEN 'HR' WHEN DepartmentID = '2' THEN 'Packing and Processing' WHEN DepartmentID = 3 THEN 'Distribution' WHEN DepartmentID = 4 THEN 'Quality Control' WHEN DepartmentID = '5' THEN 'Finances' ELSE 'Maintenance' END Department, CASE WHEN EmployeeType = 7 THEN 'HR Officer' WHEN EmployeeType = 8 THEN 'Training Officer' WHEN EmployeeType = 9 THEN 'HR Executive' WHEN EmployeeType = 10 THEN 'HR Manager' WHEN EmployeeType = 11 THEN 'Recruiter' WHEN EmployeeType = 12 THEN 'Packing Supervisor' WHEN EmployeeType = 13 THEN 'Packer'  WHEN EmployeeType = 14 THEN 'Analyst' WHEN EmployeeType = 15 THEN 'QC Manager' WHEN EmployeeType = 16 THEN 'Janitor'  WHEN EmployeeType = 17 THEN 'Maintenance Supervisor' WHEN EmployeeType = 18 THEN 'Peon' WHEN EmployeeType = 19 THEN 'Accountant' ELSE 'Distribution Supervisor' END Designation , Email, DateOfJoining, NetSalary FROM Employees ORDER BY " + SortEmployeeComboBox.Text +" "+ orderby, con);
                SqlDataAdapter sda = new SqlDataAdapter(cmd);

                DataTable dt = new DataTable();

                sda.Fill(dt);

                AllEmployeeDataGrid.DataSource = dt;
            }
        }

        private void PrevRecordsButton_Click(object sender, EventArgs e)
        {
            page -= 1;
            SqlCommand getcount = new SqlCommand("SELECT Count(*) FROM Employees", con);
            getcount.ExecuteNonQuery();
            if (35 * page>=0)
            {
                SqlCommand cmd = new SqlCommand("SELECT TOP (35) [EmployeeRegNo] ,[FirstName] ,[LastName] ,[CNIC] ,CASE WHEN Gender = 'M' THEN 'Male' WHEN Gender = 'F' THEN 'Female' Else 'U' END Gender ,[Address] ,[PhoneNumber] ,[AccountNo] ,CASE WHEN DepartmentID = '1' THEN 'HR' WHEN DepartmentID = '2' THEN 'Packing and Processing' WHEN DepartmentID = 3 THEN 'Distribution' WHEN DepartmentID = 4 THEN 'Quality Control' WHEN DepartmentID = '5' THEN 'Finances' ELSE 'Maintenance' END Department, CASE WHEN EmployeeType = 7 THEN 'HR Officer' WHEN EmployeeType = 8 THEN 'Training Officer' WHEN EmployeeType = 9 THEN 'HR Executive' WHEN EmployeeType = 10 THEN 'HR Manager' WHEN EmployeeType = 11 THEN 'Recruiter' WHEN EmployeeType = 12 THEN 'Packing Supervisor' WHEN EmployeeType = 13 THEN 'Packer'  WHEN EmployeeType = 14 THEN 'Analyst' WHEN EmployeeType = 15 THEN 'QC Manager' WHEN EmployeeType = 16 THEN 'Janitor'  WHEN EmployeeType = 17 THEN 'Maintenance Supervisor' WHEN EmployeeType = 18 THEN 'Peon' WHEN EmployeeType = 19 THEN 'Accountant' ELSE 'Distribution Supervisor' END Designation ,[Email] ,[DateOfJoining] ,[NetSalary] FROM (SELECT  ROW_NUMBER() OVER(Order By ID) AS rownum,[ID] ,[EmployeeRegNo] ,[FirstName] ,[LastName] ,[CNIC] ,[Gender] ,[Address] ,[PhoneNumber] ,[AccountNo] ,[DepartmentID] ,[EmployeeType] ,[Email] ,[DateOfJoining] ,[NetSalary] FROM Employees) AS emp1 WHERE rownum>" + (35 * page).ToString() + " ORDER By ID ", con);
                SqlDataAdapter sda = new SqlDataAdapter(cmd);

                DataTable dt = new DataTable();

                sda.Fill(dt);

                AllEmployeeDataGrid.DataSource = dt;
            }
            else
            {
                MessageBox.Show("No more records to show");
                page += 1;
            }
        }

        private void NextRecordsButton_Click(object sender, EventArgs e)
        {
            page += 1;
            SqlCommand getcount = new SqlCommand("SELECT Count(*) FROM Employees", con);
            getcount.ExecuteNonQuery();
            if (int.Parse(getcount.ExecuteScalar().ToString()) > 35 * page)
            {
                SqlCommand cmd = new SqlCommand("SELECT TOP (35) [EmployeeRegNo] ,[FirstName] ,[LastName] ,[CNIC] ,CASE WHEN Gender = 'M' THEN 'Male' WHEN Gender = 'F' THEN 'Female' Else 'U' END Gender ,[Address] ,[PhoneNumber] ,[AccountNo] ,CASE WHEN DepartmentID = '1' THEN 'HR' WHEN DepartmentID = '2' THEN 'Packing and Processing' WHEN DepartmentID = 3 THEN 'Distribution' WHEN DepartmentID = 4 THEN 'Quality Control' WHEN DepartmentID = '5' THEN 'Finances' ELSE 'Maintenance' END Department, CASE WHEN EmployeeType = 7 THEN 'HR Officer' WHEN EmployeeType = 8 THEN 'Training Officer' WHEN EmployeeType = 9 THEN 'HR Executive' WHEN EmployeeType = 10 THEN 'HR Manager' WHEN EmployeeType = 11 THEN 'Recruiter' WHEN EmployeeType = 12 THEN 'Packing Supervisor' WHEN EmployeeType = 13 THEN 'Packer'  WHEN EmployeeType = 14 THEN 'Analyst' WHEN EmployeeType = 15 THEN 'QC Manager' WHEN EmployeeType = 16 THEN 'Janitor'  WHEN EmployeeType = 17 THEN 'Maintenance Supervisor' WHEN EmployeeType = 18 THEN 'Peon' WHEN EmployeeType = 19 THEN 'Accountant' ELSE 'Distribution Supervisor' END Designation ,[Email] ,[DateOfJoining] ,[NetSalary] FROM (SELECT  ROW_NUMBER() OVER(Order By ID) AS rownum,[ID] ,[EmployeeRegNo] ,[FirstName] ,[LastName] ,[CNIC] ,[Gender] ,[Address] ,[PhoneNumber] ,[AccountNo] ,[DepartmentID] ,[EmployeeType] ,[Email] ,[DateOfJoining] ,[NetSalary] FROM Employees) AS emp1 WHERE rownum>" + (35 * page).ToString() + " ORDER By ID ", con);
                SqlDataAdapter sda = new SqlDataAdapter(cmd);

                DataTable dt = new DataTable();

                sda.Fill(dt);

                AllEmployeeDataGrid.DataSource = dt;
            }
            else
            {
                MessageBox.Show("No more records to show");
                page -= 1;
            }
        }
        private void AlterColumnResizing(object sender, EventArgs e)
        {
            if (this.Size.Width > 1050)
            {
                AllEmployeeDataGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                AllEmployeeDataGrid.RowHeadersWidth = 80;
            }
            else
            {
                AllEmployeeDataGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            }
        }
        //////////////////////////////////////////////////////////////////
        //
        //
        //Employee Salaries
        //
        //
        /////////////////////////////////////////////////////////////////
        private void EmployeeSalaryNavButton_Click(object sender, EventArgs e)
        {
            EmployeeManagementPages.SetPage("EmployeeSalaries");
            HideAllSubMenu();
            LoadRequiredData();
            this.Text = "Employee Salaries";
        }

        private void LoadRequiredData()
        {
            //
            //GETTING COMPANY BALANCE
            //
            SqlCommand cmd = new SqlCommand("SELECT Balance FROM Account WHERE AccountNo = '1111' ", con);
            cmd.ExecuteNonQuery();
            CompanyBalanceTextBox.Text = cmd.ExecuteScalar().ToString();
            CurrentCompanyBalance = float.Parse(CompanyBalanceTextBox.Text);

            //
            // COMPANY BALANCE AFTER 100 PERCENT SALARY PAYMENT
            //
            cmd = new SqlCommand("SELECT SUM(NetSalary) FROM Employees", con);
            cmd.ExecuteNonQuery();
            if (cmd.ExecuteScalar().ToString() != "")
            {
                After100PercentBox.Text = (CurrentCompanyBalance - float.Parse(cmd.ExecuteScalar().ToString())).ToString();
                fullpayment = float.Parse(cmd.ExecuteScalar().ToString());
                After100PercentBox.ForeColor = System.Drawing.Color.Red;
            }
            else
            {
                After100PercentBox.Text = CompanyBalanceTextBox.Text;
                After100PercentBox.ForeColor = System.Drawing.Color.Black;
            }

            cmd = new SqlCommand("SELECT [EmployeeRegNo],CASE WHEN Purpose = 46 THEN 'Salary' WHEN Purpose = 47 THEN 'Loan' ELSE 'Tour' END [Purpose],[Payable],[Paid],[Balance]FROM [OilRefinery].[dbo].[EmployeeAccounts]", con);
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();

            sda.Fill(dt);
            PendingPaymentsDataGrid.DataSource = dt;
        }
        //
        //event on datagrid To fill the registrationbox for the employee being payed
        //

        private void Transfer100PercentAmount_Click(object sender, EventArgs e)
        {
            Transfer100PercentAmount.Enabled = false;

            SqlCommand begintran = new SqlCommand("SET TRANSACTION ISOLATION LEVEL SERIALIZABLE; BEGIN TRANSACTION salary", con);
            begintran.ExecuteNonQuery();
            //
            //making sure the balance hasnt changed since the form was opened till the transaction began
            //
            SqlCommand checkchange = new SqlCommand("SELECT Balance FROM Account WHERE AccountNo = '1111'", con);
            checkchange.ExecuteNonQuery();

            if (float.Parse(checkchange.ExecuteScalar().ToString()) == CurrentCompanyBalance && float.Parse(After100PercentBox.Text) >= 0)
            {

                try
                {

                    //
                    //getting the required data i.e list of account numbers, registration numbers and net salaries of all the employees
                    //
                    SqlCommand cmd = new SqlCommand("SELECT AccountNo, NetSalary, EmployeeRegNo, FirstName+' '+LastName FROM Employees", con);
                    SqlDataAdapter sda = new SqlDataAdapter(cmd);

                    DataTable dt = new DataTable();
                    sda.Fill(dt);

                    //
                    //GETTING THE PURPOSE FROM LOOKUP
                    //
                    SqlCommand getPurpose = new SqlCommand("SELECT ID FROM LookUp WHERE Category = 'PaymentPurpose' AND Value = 'Salary'", con);
                    getPurpose.ExecuteNonQuery();


                    //
                    // INSERTING INTO EMPLOYEEACOUNNTS AND TRANSACTIONS AND UPDATING ACCOUNTS AS WELL
                    //
                    float SumOfLoans = 0;
                    SqlDataAdapter sda1;
                    DataTable dt1;
                    SqlCommand getLoanCount;
                    SqlCommand trydeduct;
                    SqlCommand updateAcc;
                    SqlCommand transaction;
                    SqlCommand empAccounts;
                    SqlCommand CompAccount;
                    float AllSalariesSum = 0;
                    foreach (DataRow row in dt.Rows)
                    {


                        getLoanCount = new SqlCommand("SELECT ID, (PercentageOfDeduction/100) FROM Loan WHERE AmountPending>0 AND EmployeeRegNo = @EmpReg", con);
                        getLoanCount.Parameters.AddWithValue("@EmpReg", row[2].ToString());
                        sda1 = new SqlDataAdapter(getLoanCount);
                        dt1 = new DataTable();
                        sda1.Fill(dt1);
                        SumOfLoans = 0;
                        foreach (DataRow row1 in dt1.Rows)
                        {
                            try
                            {
                                trydeduct = new SqlCommand("UPDATE Loan SET AmountPending -= @amount WHERE EmployeeRegNo = @empReg AND ID = @Id", con);
                                trydeduct.Parameters.AddWithValue("@amount", float.Parse(row[1].ToString()) * float.Parse(row1[1].ToString()));
                                trydeduct.Parameters.AddWithValue("@empReg", row[2].ToString());
                                trydeduct.Parameters.AddWithValue("@Id", int.Parse(row1[0].ToString()));
                                trydeduct.ExecuteNonQuery();

                                SumOfLoans += float.Parse(row[1].ToString()) * float.Parse(row1[1].ToString());
                            }
                            catch (SqlException)
                            {
                                trydeduct = new SqlCommand("SELECT AmountPending FROM Loan WHERE ID = @Id");
                                trydeduct.Parameters.AddWithValue("@Id", int.Parse(row1[0].ToString()));
                                trydeduct.ExecuteNonQuery();
                                SumOfLoans += float.Parse(trydeduct.ExecuteScalar().ToString());

                                trydeduct = new SqlCommand("UPDATE Loan SET AmountPending = 0 WHERE EmployeeRegNo = @EmpRegNo AND ID = @Id", con);
                                trydeduct.Parameters.AddWithValue("@EmpRegNo", row[2].ToString());
                                trydeduct.Parameters.AddWithValue("@Id", int.Parse(row1[0].ToString()));
                                trydeduct.ExecuteNonQuery();
                            }

                        }
                        //
                        //Updating Account Table
                        //
                        updateAcc = new SqlCommand("UPDATE Account SET Balance -= @amount WHERE AccountNo = @AccNum", con);
                        updateAcc.Parameters.AddWithValue("@amount", SumOfLoans);
                        updateAcc.Parameters.AddWithValue("@AccNum", row[0].ToString());
                        updateAcc.ExecuteNonQuery();
                        //
                        //INSERTING INTO TRANSACTIONS TABLE
                        //
                        transaction = new SqlCommand("INSERT INTO Transactions VALUES ('Salary being paid to ' + @EmpFullName,GETDATE(),'1111', @RecAcc,  @amount)", con);
                        transaction.Parameters.AddWithValue("@RecAcc", row[0].ToString());
                        transaction.Parameters.AddWithValue("@amount", float.Parse(row[1].ToString()) - SumOfLoans);
                        transaction.Parameters.AddWithValue("@EmpFullName", row[3].ToString());
                        transaction.ExecuteNonQuery();
                        AllSalariesSum += float.Parse(row[1].ToString()) - SumOfLoans;
                        //
                        //INSERTING INTO EmployeeAccounts Table
                        //
                        empAccounts = new SqlCommand("INSERT INTO EmployeeAccounts VALUES ((SELECT MAX(ID) FROM Transactions WHERE ReceiverAccount = @TranRecAcc), @empRegNo, @purpose, NULL, @payable, @paid, (SELECT Balance FROM Account WHERE AccountNo = @AccountNumber))", con);
                        empAccounts.Parameters.AddWithValue("@TranRecAcc", row[0].ToString());
                        empAccounts.Parameters.AddWithValue("@empRegNo", row[2].ToString());
                        empAccounts.Parameters.AddWithValue("@purpose", int.Parse(getPurpose.ExecuteScalar().ToString()));
                        empAccounts.Parameters.AddWithValue("@payable", float.Parse(row[1].ToString()));
                        empAccounts.Parameters.AddWithValue("@paid", float.Parse(row[1].ToString()) - SumOfLoans);
                        empAccounts.Parameters.AddWithValue("@AccountNumber", row[0].ToString());
                        empAccounts.ExecuteNonQuery();
                    }
                    //
                    //Updating Company Account
                    //
                    CompAccount = new SqlCommand("UPDATE Account SET Balance -= @amount WHERE AccountNo = '1111'", con);
                    CompAccount.Parameters.AddWithValue("@amount", AllSalariesSum);
                    CompAccount.ExecuteNonQuery();
                    //
                    //COMMITTING TRANSACTION
                    //
                    begintran = new SqlCommand("COMMIT TRANSACTION salary", con);
                    begintran.ExecuteNonQuery();
                    Transfer100PercentAmount.Enabled = true;
                    LoadRequiredData();
                    MessageBox.Show("Salaries Paid Successfully");

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    if (ex.Message.Contains("Balance"))
                    {
                        MessageBox.Show("You have run out of funds", "Bankruptcy Error");
                    }
                    else
                    {
                        MessageBox.Show(ex.Message);
                    }
                    begintran = new SqlCommand("ROLLBACK TRANSACTION salary", con);
                    begintran.ExecuteNonQuery();
                    Transfer100PercentAmount.Enabled = true;
                }
            }
            else
            {
                MessageBox.Show("Sorry for the inconvenience", "Insufficient Funds or Change in balance");
                begintran = new SqlCommand("ROLLBACK TRANSACTION salary", con);
                begintran.ExecuteNonQuery();
                Transfer100PercentAmount.Enabled = true;
            }
        }

        //////////////////////////////////////////////////////////////////
        //
        //
        //EmployeeLoans
        //
        //
        //////////////////////////////////////////////////////////////////
        private void EmployeeLoansNavButton_Click(object sender, EventArgs e)
        {
            EmployeeManagementPages.SetPage("EmployeeLoanPage");
            HideAllSubMenu();
            LoanDeductionCombo.SelectedIndex = -1;
            LoanRegistrationNum.Text = "";
            AmountTakenTextBox.Text = "";
            this.Text = "Employee Loans";
            SqlCommand cmd = new SqlCommand("SELECT [EmployeeRegNo],[AmountTaken],[PercentageOfDeduction],[AmountPending],[DateTaken] FROM [OilRefinery].[dbo].[Loan]", con);
            SqlDataAdapter sda = new SqlDataAdapter(cmd);

            DataTable dt = new DataTable();
            sda.Fill(dt);

            EmployeeLoansDataGrid.DataSource = dt;
            
        }

        private void AlterLoanDataGridSize(object sender, EventArgs e)
        {
            if (this.Size.Width > 1050)
            {
                EmployeeLoansDataGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            }
            else
            {
                EmployeeLoansDataGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            }
        }

        private void LendLoanButton_Click(object sender, EventArgs e)//YAHAN PER I THINK SERIALIZABLE HONA CHAIYE
        {

            if (Regex.IsMatch(AmountTakenTextBox.Text, @"(^[0-9]{4,6}$)"))
            {
                SqlCommand checReg = new SqlCommand("SELECT EmployeeRegNo FROM Employees WHERE EmployeeRegNo = @EmpReg", con);
                checReg.Parameters.AddWithValue("@EmpReg", LoanRegistrationNum.Text);
                checReg.ExecuteNonQuery();
                if (LoanDeductionCombo.SelectedIndex != -1 && checReg.ExecuteScalar() != null)
                {
                    SqlCommand checkPossibility = new SqlCommand("Select COUNT(*) FROM Loan WHERE EmployeeRegNo = @EmpReg AND AmountPending > 0", con);
                    checkPossibility.Parameters.AddWithValue("@EmpReg", LoanRegistrationNum.Text);
                    checkPossibility.ExecuteNonQuery();
                    SqlCommand companyAccount = new SqlCommand("SELECT Balance FROM Account WHERE AccountNo = '1111'", con);
                    companyAccount.ExecuteNonQuery();
                    SqlCommand checkliabilities = new SqlCommand("SELECT COUNT(*) FROM Account WHERE AccountNo <> '1111' AND Balance < 0 ", con);
                    checkliabilities.ExecuteNonQuery();
                    if (int.Parse(checkliabilities.ExecuteScalar().ToString()) == 0)
                    {
                        if (int.Parse(checkPossibility.ExecuteScalar().ToString()) < 3 && float.Parse(companyAccount.ExecuteScalar().ToString()) > 1000000)
                        {
                            try
                            {
                                LendLoanButton.Enabled = false;
                                SqlCommand begintran = new SqlCommand("SET TRANSACTION ISOLATION LEVEL SERIALIZABLE; BEGIN TRANSACTION", con);
                                begintran.ExecuteNonQuery();
                                //
                                //GETTING ACCOUNT NUMBER ASSOCIATED WITH THE REGISTRATION NUMBER
                                //
                                SqlCommand getaccnum = new SqlCommand("SELECT AccountNo FROM Employees WHERE EmployeeRegNo = @EmpReg", con);
                                getaccnum.Parameters.AddWithValue("@EmpReg", LoanRegistrationNum.Text);
                                getaccnum.ExecuteNonQuery();
                                //
                                //GETTING ACCOUNT BALANCE ASSOCIATED WITH THE REGISTRATION NUMBER
                                //
                                SqlCommand getempBalance = new SqlCommand("SELECT Balance FROM Account WHERE AccountNo = @EmpAccNum", con);
                                getempBalance.Parameters.AddWithValue("@EmpAccNum", getaccnum.ExecuteScalar().ToString());
                                getempBalance.ExecuteNonQuery();
                                //
                                //UPDATING Employee's Account BALANCE
                                //
                                SqlCommand updateEmpAccount = new SqlCommand("exec UpdateAccount @EmpAccountNo, @balance", con);
                                updateEmpAccount.Parameters.AddWithValue("@balance", float.Parse(AmountTakenTextBox.Text) + float.Parse(getempBalance.ExecuteScalar().ToString()));
                                updateEmpAccount.Parameters.AddWithValue("@EmpAccountNo", getaccnum.ExecuteScalar().ToString());
                                updateEmpAccount.ExecuteNonQuery();
                                //
                                // INSERTING INTO LOANS TABLE
                                //
                                SqlCommand lendloan = new SqlCommand("INSERT INTO Loan VALUES (@EmpReg, @AmountTaken, @POD, @pendingPayment, @DateTaken)", con);
                                lendloan.Parameters.AddWithValue("@EmpReg", LoanRegistrationNum.Text);
                                lendloan.Parameters.AddWithValue("@AmountTaken", float.Parse(AmountTakenTextBox.Text));
                                lendloan.Parameters.AddWithValue("@POD", float.Parse(LoanDeductionCombo.Text));
                                lendloan.Parameters.AddWithValue("@pendingPayment", float.Parse(AmountTakenTextBox.Text));
                                lendloan.Parameters.AddWithValue("@DateTaken", DateTime.Now);
                                lendloan.ExecuteNonQuery();
                                //
                                //DEbitting Company Account
                                //
                                SqlCommand CompAcc = new SqlCommand("UPDATE Account SET Balance -= @balanceVal WHERE AccountNo = '1111'", con);
                                CompAcc.Parameters.AddWithValue("@balanceVal", float.Parse(AmountTakenTextBox.Text));
                                CompAcc.ExecuteNonQuery();

                                //
                                //INSERTING INTO TRANSACTIONS TABLE
                                //
                                SqlCommand getName = new SqlCommand("SELECT FirstName + ' ' + LastName FROM Employees WHERE EmployeeRegNo = @EmpReg", con);
                                getName.Parameters.AddWithValue("@EmpReg", LoanRegistrationNum.Text);
                                getName.ExecuteNonQuery();
                                SqlCommand CompAccount = new SqlCommand("INSERT INTO Transactions VALUES ('Loan lent to '+ @EmpName,GETDATE(),1111, @ReceivableAcc, @Amount)", con);
                                CompAccount.Parameters.AddWithValue("@ReceivableAcc", getaccnum.ExecuteScalar().ToString());
                                CompAccount.Parameters.AddWithValue("@Amount", float.Parse(AmountTakenTextBox.Text));
                                CompAccount.Parameters.AddWithValue("@EmpName", getName.ExecuteScalar().ToString());
                                CompAccount.ExecuteNonQuery();
                                //
                                //INSERTING INTO EMPLOYEEACCOUNTS TABLE
                                //
                                SqlCommand accountinc = new SqlCommand("INSERT INTO EmployeeAccounts VALUES ((SELECT MAX(ID) FROM Transactions WHERE ReceiverAccount = @TranAccNum), @EmpReg, (SELECT ID FROM LookUp WHERE Category = 'PaymentPurpose' AND Value = 'Loan'), (SELECT MAX(ID) FROM Loan WHERE EmployeeRegNo = @EmpRegLoan), @payable, @paid, @balance)", con);
                                accountinc.Parameters.AddWithValue("@TranAccNum", getaccnum.ExecuteScalar().ToString());
                                accountinc.Parameters.AddWithValue("@EmpReg", LoanRegistrationNum.Text);
                                accountinc.Parameters.AddWithValue("@EmpRegLoan", LoanRegistrationNum.Text);
                                accountinc.Parameters.AddWithValue("@payable", float.Parse(AmountTakenTextBox.Text));
                                accountinc.Parameters.AddWithValue("@paid", float.Parse(AmountTakenTextBox.Text));
                                //
                                //getting the sum of all pending balances of this employee
                                //
                                SqlCommand getEmpBalance = new SqlCommand("SELECT Balance FROM Account WHERE AccountNo = @EmpAccNum", con);
                                getEmpBalance.Parameters.AddWithValue("@EmpAccNum", getaccnum.ExecuteScalar().ToString());
                                getEmpBalance.ExecuteNonQuery();
                                accountinc.Parameters.AddWithValue("@balance", float.Parse(getEmpBalance.ExecuteScalar().ToString()));
                                accountinc.ExecuteNonQuery();

                                SqlCommand commitTran = new SqlCommand("COMMIT TRANSACTION", con);
                                commitTran.ExecuteNonQuery();
                                LendLoanButton.Enabled = true;

                                EmployeeLoansNavButton_Click(sender, e);
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message);
                                SqlCommand rollbackTran = new SqlCommand("ROLLBACK TRANSACTION", con);
                                rollbackTran.ExecuteNonQuery();
                                LendLoanButton.Enabled = true;
                            }
                        }
                        else
                        {
                            MessageBox.Show("This Employee already has 3 loans pending", "Loan Rejected");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Company Already owes other Employees", "Loan Rejected");
                    }
                }
                else
                {
                    MessageBox.Show("Select a proper Registration number and a percentage of deduction", "Value not Selected");
                }
            }
            else
            {
                MessageBox.Show("Enter a valid amount of upto 999999 maximum", "Invalid Value Entered");
            }
        }

        //////////////////////////////////////////////////////////////////
        //
        //
        //EmployeeTours
        //
        //
        //////////////////////////////////////////////////////////////////
        private void fillByToolStripButton_Click(object sender, EventArgs e)
        {
            try
            {
                this.lookUpTableAdapter.FillBy(this.employeeTypeDataSet.LookUp);
            }
            catch (System.Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
            }

        }

        private void EmployeeTourNavButton_Click(object sender, EventArgs e)
        {
            EmployeeManagementPages.SetPage("EmployeeTourPage");
            HideAllSubMenu();
            TourRegistrationNum.Text = "";
            TourRegionBox.SelectedIndex = -1;
            SqlCommand cmd = new SqlCommand("SELECT [EmployeeRegNo],[Region],[TourDate] FROM [OilRefinery].[dbo].[Tour]", con);
            SqlDataAdapter sda = new SqlDataAdapter(cmd);

            DataTable dt = new DataTable();
            sda.Fill(dt);

            EmployeeToursDataGrid.DataSource = dt;
            this.Text = "Employee Tours";
        }

        private void AlotTourButton_Click(object sender, EventArgs e)//YAHAN PER I THINK SERIALIZABLE HONA CHAIYE
        {
            SqlCommand checReg = new SqlCommand("SELECT EmployeeRegNo FROM Employees WHERE EmployeeRegNo = @EmpReg", con);
            checReg.Parameters.AddWithValue("@EmpReg", TourRegistrationNum.Text);
            checReg.ExecuteNonQuery();
            if (checReg.ExecuteScalar() != null)
            {
                if (TourRegionBox.SelectedIndex != -1)
                {
                    int dotdate = int.Parse(TourDOT.Value.Year.ToString()); ;
                    if (dotdate > 2011)
                    {
                        try
                        {
                            AlotTourButton.Enabled = false;
                            SqlCommand begintran = new SqlCommand("SET TRANSACTION ISOLATION LEVEL SERIALIZABLE; BEGIN TRANSACTION", con);
                            begintran.ExecuteNonQuery();
                            //
                            //INSERTING INTO TOUR TABLE
                            //
                            SqlCommand cmd = new SqlCommand("INSERT INTO Tour VALUES (@EmpReg, @Region, @TourDate)", con);
                            cmd.Parameters.AddWithValue("@EmpReg", TourRegistrationNum.Text);
                            cmd.Parameters.AddWithValue("@Region", TourRegionBox.Text);
                            cmd.Parameters.AddWithValue("@TourDate", TourDOT.Value);
                            cmd.ExecuteNonQuery();
                            //
                            //GETTING THE ACCOUNT NUMBER ASSOCIATED WITH REGISTRATION NUMBER
                            //
                            SqlCommand getregnum = new SqlCommand("SELECT AccountNo FROM Employees WHERE EmployeeRegNo = @EmpReg", con);
                            getregnum.Parameters.AddWithValue("@EmpReg", TourRegistrationNum.Text);
                            getregnum.ExecuteNonQuery();
                            //
                            //INSERTING INTO TRANSACTIONS TABLE
                            //
                            SqlCommand getName = new SqlCommand("SELECT FirstName + ' ' + LastName FROM Employees WHERE EmployeeRegNo = @EmpReg", con);
                            getName.Parameters.AddWithValue("@EmpReg", TourRegistrationNum.Text);
                            getName.ExecuteNonQuery();
                            SqlCommand tran = new SqlCommand("INSERT INTO Transactions VALUES ('Amount Paid to ' + @EmpName + ' For tour to ' + @Region, GETDATE(),'1111', @accNum, @amount)", con);
                            tran.Parameters.AddWithValue("@accNum", getregnum.ExecuteScalar().ToString());
                            tran.Parameters.AddWithValue("@EmpName", getName.ExecuteScalar().ToString());
                            tran.Parameters.AddWithValue("@Region", TourRegionBox.Text);
                            float paidamount = 0;
                            if (TourRegionBox.Text == "Punjab")
                            {
                                tran.Parameters.AddWithValue("@amount", 40000.0);
                                paidamount = 40000;
                            }
                            else if (TourRegionBox.Text == "KPK")
                            {
                                tran.Parameters.AddWithValue("@amount", 80000.0);
                                paidamount = 80000;
                            }
                            else if (TourRegionBox.Text == "Sindh")
                            {
                                tran.Parameters.AddWithValue("@amount", 120000.0);
                                paidamount = 120000;
                            }
                            else
                            {
                                tran.Parameters.AddWithValue("@amount", 160000.0);
                                paidamount = 160000;
                            }
                            tran.ExecuteNonQuery();
                            //
                            //INSERTING INTO EMPLOYEEACCOUNTS TABLE
                            //
                            SqlCommand accountinc = new SqlCommand("INSERT INTO EmployeeAccounts VALUES ((SELECT MAX(ID) FROM Transactions WHERE ReceiverAccount = @TranAccNum), @EmpReg, (SELECT ID FROM LookUp WHERE Category = 'PaymentPurpose' AND Value = 'Tour'), (SELECT MAX(ID) FROM Tour WHERE EmployeeRegNo = @EmpRegTour), @payable, @paid, @balance)", con);
                            accountinc.Parameters.AddWithValue("@TranAccNum", getregnum.ExecuteScalar().ToString());
                            accountinc.Parameters.AddWithValue("@EmpReg", TourRegistrationNum.Text);
                            accountinc.Parameters.AddWithValue("@EmpRegTour", TourRegistrationNum.Text);
                            accountinc.Parameters.AddWithValue("@payable", paidamount);
                            accountinc.Parameters.AddWithValue("@paid", paidamount);
                            SqlCommand getpendingpaymentsum = new SqlCommand("SELECT Balance FROM Account WHERE AccountNo = @EmpAccNum", con);
                            getpendingpaymentsum.Parameters.AddWithValue("@EmpAccNum", getregnum.ExecuteScalar().ToString());
                            getpendingpaymentsum.ExecuteNonQuery();
                            accountinc.Parameters.AddWithValue("@balance", float.Parse(getpendingpaymentsum.ExecuteScalar().ToString()));
                            accountinc.ExecuteNonQuery();
                            //
                            //Updating Company Account Balance
                            //
                            SqlCommand CompAcc = new SqlCommand("UPDATE Account SET Balance -= @balanceVal WHERE AccountNo = '1111'", con);
                            CompAcc.Parameters.AddWithValue("@balanceVal", paidamount);
                            CompAcc.ExecuteNonQuery();
                            //
                            //COMMITTING TRANSACTION
                            //
                            SqlCommand commitTran = new SqlCommand("COMMIT TRANSACTION", con);
                            commitTran.ExecuteNonQuery();
                            AlotTourButton.Enabled = true;
                            EmployeeTourNavButton_Click(sender, e);
                        }
                        catch (Exception ex)
                        {
                            //MessageBox.Show(ex.Message);
                            SqlCommand rollbackTran = new SqlCommand("ROLLBACK TRANSACTION", con);
                            rollbackTran.ExecuteNonQuery();
                            AlotTourButton.Enabled = true;
                        }
                    }
                    else
                    {
                        MessageBox.Show("Please enter a date later than 2012", "Invalid Date");
                    }

                }
                else
                {
                    MessageBox.Show("Please Select a region", "Region Not Selected");
                }
            }
            else
            {
                MessageBox.Show("Please Select a Registration Number of the Employee", "ID not Selected");
            }

        }

        private void fillByregionToolStripButton_Click(object sender, EventArgs e)
        {
            
        }

        private void PurposeFilterButton_Click(object sender, EventArgs e)
        {
            if (PurposeComboBox.SelectedIndex != -1)
            {

                SqlCommand cmd = new SqlCommand("SELECT [EmployeeRegNo],CASE WHEN Purpose = 46 THEN 'Salary' WHEN Purpose = 47 THEN 'Loan' ELSE 'Tour' END [Purpose],[Payable],[Paid],[Balance]FROM [OilRefinery].[dbo].[EmployeeAccounts] WHERE Purpose = (SELECT ID FROM LookUp WHERE Value = @val) ", con);
                cmd.Parameters.AddWithValue("@val", PurposeComboBox.Text);
                SqlDataAdapter sda = new SqlDataAdapter(cmd);

                DataTable dt = new DataTable();

                sda.Fill(dt);

                PendingPaymentsDataGrid.DataSource = dt;
            }
        }

        private void GoBackButton_Click(object sender, EventArgs e)
        {
            Form fm = MainAdminScreen.instance();
            fm.Show();
            this.Hide();
            fm.BringToFront();
        }

        private void EmployeeReportsNavButton_Click(object sender, EventArgs e)
        {
            EmployeeManagementPages.SetPage("EmployeeReportsPage");
        }

        private void GenerateReport1Button_Click(object sender, EventArgs e)
        {
            EmployeeManagementPages.SetPage("EmployeeReport1Page");
            crystalReportViewer1.ReportSource = new EmployeeReport1();
        }

        private void GenerateReport2Button_Click(object sender, EventArgs e)
        {
            EmployeeManagementPages.SetPage("EmployeeReport1Page");
            crystalReportViewer1.ReportSource = new EmployeeReport2();
        }

        private void GenerateReport3Button_Click(object sender, EventArgs e)
        {
            EmployeeManagementPages.SetPage("EmployeeReport1Page");
            crystalReportViewer1.ReportSource = new EmployeeReport3();
        }

        private void GenerateReport4Button_Click(object sender, EventArgs e)
        {
            EmployeeManagementPages.SetPage("EmployeeReport1Page");
            crystalReportViewer1.ReportSource = new EmployeeReport4();
        }

        private void CloseApplication(object sender, FormClosedEventArgs e)
        {
            LoginForm.close();
        }
    }
}