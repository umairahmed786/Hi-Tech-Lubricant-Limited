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
    public partial class MainAdminScreen : Form
    {
        string regNo = "";
        int orderID = 0;
        private static MainAdminScreen Lform;
        
        
        private MainAdminScreen()
        {
            UpdateOrderStatus();
            InitializeComponent();
            HideSubMenu();

        }
        public static MainAdminScreen instance()
        {
            if (Lform == null)
            {
                Lform = new MainAdminScreen();
            }
            return Lform;
        }
        public void UpdateOrderStatus()
        {
            var con = Configuration.getInstance().getConnection();

            SqlCommand StatusUpdate = new SqlCommand("update Orders set Status=(select ID from LookUp where Category='OrderStatus' and Value='Delivered') where ID in(select OrderID from OrderDelivered where ExpectedDeliveredDate<GETDATE())", con);
            StatusUpdate.ExecuteNonQuery();
        }
        /// <summary>
        /// Trader section begins from here
        /// </summary>
        public void InsertData()
        {
            try
            {
                var con = Configuration.getInstance().getConnection();

                SqlCommand BeginTrans = new SqlCommand("Set Transaction Isolation Level Serializable Begin Transaction AddTrader", con);
                BeginTrans.ExecuteNonQuery();

                SqlCommand cmd1 = new SqlCommand("Select ID from Lookup where Category='PersonType' AND Value='Trader'", con);
                cmd1.ExecuteNonQuery();

                SqlCommand cmd2 = new SqlCommand("Insert into Person values (@PersonType)", con);
                cmd2.Parameters.AddWithValue("@PersonType", int.Parse(cmd1.ExecuteScalar().ToString()));
                cmd2.ExecuteNonQuery();

                SqlCommand getID = new SqlCommand("SELECT MAX(Id) FROM Person", con);
                getID.ExecuteNonQuery();

                SqlCommand cmd4 = new SqlCommand("Insert into Account values(@AccountNo,@HolderID,@Balance)", con);
                cmd4.Parameters.AddWithValue("@AccountNo", submitTraderAccountNumber.Text);
                cmd4.Parameters.AddWithValue("@HolderID", int.Parse(getID.ExecuteScalar().ToString()));
                cmd4.Parameters.AddWithValue("@Balance", float.Parse("0"));
                cmd4.ExecuteNonQuery();

                SqlCommand cmd3 = new SqlCommand("Insert into Traders values(@RegNo,@ID,@Name,@Region,@Address,@AccountNo,@TraderType)", con);



                SqlCommand getRegion = new SqlCommand("Select ID from Lookup where Category='Region' AND Value=@TraderRegion", con);
                getRegion.Parameters.AddWithValue("TraderRegion", submitTraderRegion.Text);
                getRegion.ExecuteNonQuery();

                SqlCommand getType = new SqlCommand("Select ID from Lookup where Category='TraderType' AND Value=@TraderType", con);
                getType.Parameters.AddWithValue("TraderType", submitTraderType.Text);
                getType.ExecuteNonQuery();

                regNo = DateTime.Now.Year.ToString() + "-TR-" + int.Parse(getID.ExecuteScalar().ToString());
                cmd3.Parameters.AddWithValue("@RegNo", DateTime.Now.Year.ToString() + "-TR-" + int.Parse(getID.ExecuteScalar().ToString()));
                cmd3.Parameters.AddWithValue("@Id", int.Parse(getID.ExecuteScalar().ToString()));
                cmd3.Parameters.AddWithValue("@Name", submitTraderName.Text);
                cmd3.Parameters.AddWithValue("@Region", int.Parse(getRegion.ExecuteScalar().ToString()));
                cmd3.Parameters.AddWithValue("@Address", submitTraderAddress.Text);
                cmd3.Parameters.AddWithValue("@TraderType", int.Parse(getType.ExecuteScalar().ToString()));
                cmd3.Parameters.AddWithValue("@AccountNo", submitTraderAccountNumber.Text);
                cmd3.ExecuteNonQuery();


                SqlCommand EndTrans = new SqlCommand("Commit Transaction AddTrader", con);
                EndTrans.ExecuteNonQuery();
            }
            catch
            {
                var con = Configuration.getInstance().getConnection();
                SqlCommand EndTrans = new SqlCommand("Commit Transaction AddTrader", con);
                EndTrans.ExecuteNonQuery();
                MessageBox.Show("You are unable to perform this task");
            }

        }
        public bool CheckConstraintsForTraderInsertion()
        {
            if(submitTraderName.Text!= "Registered Name")
            {
                if (submitTraderRegion.Text != "Region")
                {
                    if (submitTraderAddress.Text != "Exact Address of the Registered Office")
                    {
                        if(submitTraderType.Text!="TraderType")
                        {
                            if(submitTraderAccountNumber.Text!= "Account Number" && Regex.IsMatch(submitTraderAccountNumber.Text, @"(^[0-9]{16,20}$)"))
                            {
                                return true;
                                
                            }
                            else
                            {
                                MessageBox.Show("Please check the Account Number");
                                return false;

                            }
                        }
                        else
                        {
                            MessageBox.Show("Please check the Trader Type");
                            return false;
                        }
                    }
                    else
                    {
                        MessageBox.Show("Please check the Trader Address");
                        return false;
                    }
                }
                else
                {
                    MessageBox.Show("Please check the Trader Region");
                    return false;
                }
            }
            else
            {
                MessageBox.Show("Please check the Trader Name");
                return false;
            }
        }
        public void PopulateComboboxForTraderInsertion()
        {
            submitTraderRegion.Items.Clear();
            submitTraderType.Items.Clear();
            var con = Configuration.getInstance().getConnection();
            SqlCommand cmd1 = new SqlCommand("select Value from LookUp where Category='Region'", con);
            submitTraderRegion.Items.Add("Region");
            SqlDataReader ds1 = cmd1.ExecuteReader();
            while(ds1.Read())
            {
                submitTraderRegion.Items.Add(ds1[0]);
            }
            ds1.Close();
            submitTraderRegion.SelectedIndex = 0;


            SqlCommand cmd2 = new SqlCommand("select Value from LookUp where Category='TraderType'", con);
            submitTraderType.Items.Add("TraderType");
            SqlDataReader ds2 = cmd2.ExecuteReader();
            while (ds2.Read())
            {
                submitTraderType.Items.Add(ds2[0]);
            }
            ds2.Close();
            submitTraderType.SelectedIndex = 0;

        }
        public void PopulateComboboxForTraderUpdation()
        {
            updateDeleteTraderRegion.Items.Clear();
            updateDeleteTraderType.Items.Clear();
            searchForTraderUpdateDelete.Items.Clear();
            var con = Configuration.getInstance().getConnection();
            SqlCommand cmd1 = new SqlCommand("select Value from LookUp where Category='Region'", con);
            updateDeleteTraderRegion.Items.Add("Region");
            SqlDataReader ds1 = cmd1.ExecuteReader();
            while (ds1.Read())
            {
                updateDeleteTraderRegion.Items.Add(ds1[0]);
            }
            ds1.Close();
            updateDeleteTraderRegion.SelectedIndex = 0;


            SqlCommand cmd2 = new SqlCommand("select Value from LookUp where Category='TraderType'", con);
            updateDeleteTraderType.Items.Add("TraderType");
            SqlDataReader ds2 = cmd2.ExecuteReader();
            while (ds2.Read())
            {
                updateDeleteTraderType.Items.Add(ds2[0]);
            }
            ds2.Close();
            updateDeleteTraderType.SelectedIndex = 0;


            SqlCommand cmd3 = new SqlCommand("Select TraderRegNo from Traders", con);
            searchForTraderUpdateDelete.Items.Add("Search");
            SqlDataReader ds3 = cmd3.ExecuteReader();
            while (ds3.Read())
            {
                searchForTraderUpdateDelete.Items.Add(ds3[0]);
            }
            ds3.Close();
            searchForTraderUpdateDelete.SelectedIndex = 0;

        }
        private void HideSubMenu()
        {
            traderSubMenu.Visible = false;
            rawSubmenu.Visible = false;
            purchasedProductSubMenu.Visible = false;
            compProd.Visible = false;
            servucProvidersubmenu.Visible = false;
            ordersSubmenu.Visible = false;
        }
        private void showsubmenu(Panel submenu)
        {

            if (submenu.Visible == false)
            {
                HideSubMenu();
                submenu.Visible = true;
            }
            else
            {
                submenu.Visible = false;
            }
        }
        private void tabPage1_Click(object sender, EventArgs e)
        {

        }

        private void gunaAdvenceButton2_Click(object sender, EventArgs e)
        {
            showsubmenu(traderSubMenu);
            slider.Top = ((Control)sender).Top;
        }

        private void gunaLabel1_Click(object sender, EventArgs e)
        {

        }
        public void InitializeAddTraderPage()
        {
            submitTraderName.Text = "Registered Name";
            submitTraderAddress.Text = "Exact Address of the Registered Office";
            submitTraderAccountNumber.Text = "Account Number";
        }
        private void bunifuButton1_Click(object sender, EventArgs e)
        {
            InitializeAddTraderPage();
            PopulateComboboxForTraderInsertion();
            bunifuPages1.SetPage("addTrader");
        }

        private void region_DropDown(object sender, EventArgs e)
        {
            submitTraderRegion.BorderColor = Color.ForestGreen;
        }

        private void region_DropDownClosed(object sender, EventArgs e)
        {
            submitTraderRegion.BorderColor = Color.DarkRed;
        }

        

        

        private void traderName_Enter(object sender, EventArgs e)
        {
            submitTraderName.Text = "";
        }

        private void traderAddress_Enter(object sender, EventArgs e)
        {
            submitTraderAddress.Text = "";
        }

        private void traderAccountNumber_Enter(object sender, EventArgs e)
        {
            submitTraderAccountNumber.Text = "";
        }

        private void gunaShadowPanel1_Paint(object sender, PaintEventArgs e)
        {
            submitTraderRegion.Text = "Select Region";
        }

        private void traderName_Leave(object sender, EventArgs e)
        {
            if (submitTraderName.Text == "")
            {
                submitTraderName.Text = "Regestered Name";
            }
        }

        private void traderAddress_Leave(object sender, EventArgs e)
        {
            if (submitTraderAddress.Text == "")
            {
                submitTraderAddress.Text = "Exact Address of the Registered Office";
            }
        }

        private void traderAccountNumber_Leave(object sender, EventArgs e)
        {
            if (submitTraderAccountNumber.Text == "")
            {
                submitTraderAccountNumber.Text = "Account Number";
            }
        }
        public void InitializeUpdateTraderPage()
        {
            updateDeleteTraderName.Text = "Regestered Name";
            updateDeleteTraderAddress.Text = "Exact Address of the Registered Office";
            updateDeleteTraderAccountNumber.Text = "Account Number";
            updateDeleteTraderRegion.Items.Clear();
            updateDeleteTraderType.Items.Clear();
        }
        private void bunifuButton2_Click(object sender, EventArgs e)
        {
            updateTraderGrid.Hide();
            InitializeUpdateTraderPage();
            PopulateComboboxForTraderUpdation();
            bunifuPages1.SetPage("updateTrader");

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void search_Enter(object sender, EventArgs e)
        {
        }

        private void search_Leave(object sender, EventArgs e)
        {
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(CheckConstraintsForTraderInsertion())
            {
                try
                {
                    InsertData();
                    MessageBox.Show("Trader is added successfully\nThe reg no assinged to the the Trader by the system is "+regNo+" .\nPlease remember this one to access your data");
                    InitializeAddTraderPage();
                    PopulateComboboxForTraderInsertion();
                }
                catch
                {
                    MessageBox.Show("Trader with this account number is already register OR \n Your are unable to perform this task");
                }
            }
            
        }

        private void searchForTraderUpdateDelete_SelectedValueChanged(object sender, EventArgs e)
        {
            if(searchForTraderUpdateDelete.Text!="Search")
            {

                updateDeleteTraderName.ReadOnly = false;
                updateDeleteTraderRegion.Enabled = true;
                updateDeleteTraderType.Enabled = true;
                updateDeleteTraderAddress.ReadOnly = false;
                var con = Configuration.getInstance().getConnection();

                SqlCommand cmd1 = new SqlCommand("Select * from Traders where TraderRegNo=@RegNo", con);
                cmd1.Parameters.AddWithValue("@RegNo", searchForTraderUpdateDelete.Text);
                SqlDataAdapter da = new SqlDataAdapter(cmd1);
                DataTable dt = new DataTable();
                da.Fill(dt);
                updateTraderGrid.DataSource = dt;

                SqlCommand getRegion = new SqlCommand("Select Value from Lookup where Category='Region' AND ID=@TraderRegion", con);
                getRegion.Parameters.AddWithValue("TraderRegion", updateTraderGrid.Rows[0].Cells[3].FormattedValue.ToString());
                getRegion.ExecuteNonQuery();

                SqlCommand getType = new SqlCommand("Select Value from Lookup where Category='TraderType' AND ID=@TraderRegion", con);
                getType.Parameters.AddWithValue("TraderRegion", updateTraderGrid.Rows[0].Cells[6].FormattedValue.ToString());
                getType.ExecuteNonQuery();

                updateDeleteTraderName.Text = updateTraderGrid.Rows[0].Cells[2].FormattedValue.ToString();
                updateDeleteTraderRegion.Text = getRegion.ExecuteScalar().ToString();
                updateDeleteTraderAddress.Text = updateTraderGrid.Rows[0].Cells[4].FormattedValue.ToString();
                updateDeleteTraderType.Text = getType.ExecuteScalar().ToString();
                updateDeleteTraderAccountNumber.Text = updateTraderGrid.Rows[0].Cells[5].FormattedValue.ToString();
            }
            else
            {
                updateDeleteTraderName.ReadOnly = true;
                updateDeleteTraderRegion.Enabled = false;
                updateDeleteTraderType.Enabled = false;
                updateDeleteTraderAddress.ReadOnly = true;


                updateDeleteTraderName.Text = "Regestered Name";
                updateDeleteTraderRegion.SelectedIndex = 0;
                updateDeleteTraderType.SelectedIndex = 0;
                updateDeleteTraderAddress.Text = "Exact Address of the Registerd Office";
                updateDeleteTraderAccountNumber.Text = "Account Number";


            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (searchForTraderUpdateDelete.Text!="Search")
            {
                DialogResult dialogResult = MessageBox.Show("Are you sure to delete?", "Confirmation", MessageBoxButtons.YesNo);

                if (dialogResult == DialogResult.Yes)
                {
                    try
                    {
                        var con = Configuration.getInstance().getConnection();
                        SqlCommand cmd1 = new SqlCommand("DELETE FROM Traders WHERE ID=@ID", con);
                        cmd1.Parameters.AddWithValue("@ID", updateTraderGrid.Rows[0].Cells[1].FormattedValue.ToString());
                        cmd1.ExecuteNonQuery();
                        MessageBox.Show("Trader is deleted successfully");
                        InitializeUpdateTraderPage();
                        PopulateComboboxForTraderUpdation();
                    }
                    catch
                    {
                        MessageBox.Show("Sorry for the inconvience");
                    }
                }
            }
            else
            {
                
                MessageBox.Show("Please select the RegNo from the given list");
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {

            if (searchForTraderUpdateDelete.Text != "Search")
            {
                if (updateDeleteTraderName.Text != "")
                {
                    if (updateDeleteTraderRegion.Text != "Region")
                    {
                        if (updateDeleteTraderAddress.Text != "")
                        {

                            if (updateDeleteTraderType.Text != "TraderType")
                            {
                                DialogResult dialogResult = MessageBox.Show("Are you sure to update?", "Confirmation", MessageBoxButtons.YesNo);

                                if (dialogResult == DialogResult.Yes)
                                {
                                    try
                                    {
                                        var con = Configuration.getInstance().getConnection();

                                        SqlCommand getRegion = new SqlCommand("Select ID from Lookup where Category='Region' AND Value=@TraderRegion", con);
                                        getRegion.Parameters.AddWithValue("TraderRegion", updateDeleteTraderRegion.Text);
                                        getRegion.ExecuteNonQuery();

                                        SqlCommand getType = new SqlCommand("Select ID from Lookup where Category='TraderType' AND Value=@TraderType", con);
                                        getType.Parameters.AddWithValue("TraderType", updateDeleteTraderType.Text);
                                        getType.ExecuteNonQuery();

                                        SqlCommand cmd1 = new SqlCommand("UPDATE Traders SET Name=@Name,Address=@Address,Region=@Region,Type=@Type WHERE [TraderRegNo]=@RegNo", con);
                                        cmd1.Parameters.AddWithValue("@RegNo", searchForTraderUpdateDelete.Text);
                                        cmd1.Parameters.AddWithValue("@Name", updateDeleteTraderName.Text);
                                        cmd1.Parameters.AddWithValue("@Region", int.Parse(getRegion.ExecuteScalar().ToString()));
                                        cmd1.Parameters.AddWithValue("@Address", updateDeleteTraderAddress.Text);
                                        cmd1.Parameters.AddWithValue("@Type", int.Parse(getType.ExecuteScalar().ToString()));
                                        cmd1.ExecuteNonQuery();


                                        MessageBox.Show("Data is updated successfully");
                                        InitializeUpdateTraderPage();
                                        PopulateComboboxForTraderUpdation();
                                    }
                                    catch
                                    {
                                        MessageBox.Show("You are unable to perform this computation");
                                    }
                                }
                            }
                            else
                            {

                                MessageBox.Show("Please select the Trader Type");
                            }
                        }
                        else
                        {
                            MessageBox.Show("Please check Trader Address");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Please check Trader Region");
                    }
                }
                else
                {
                    MessageBox.Show("Please check Trader Name");
                }
            }
            else
            {
                MessageBox.Show("Please select the registration number from the given list");
            }
        }

        private void bunifuButton3_Click(object sender, EventArgs e)
        {
            try
            {
                bunifuPages1.SetPage("showTrader");
                var con = Configuration.getInstance().getConnection();
                SqlCommand cmd = new SqlCommand("select TraderRegNo,Name,temp.Value AS Region,Address,AccountNumber,LookUp.Value AS TraderType from LookUp, (select TraderRegNo,Name,Value,Address,AccountNumber,Type from Traders,LookUp where LookUp.Category='Region' and LookUp.ID=Traders.Region) as temp where LookUp.Category='TraderType' and LookUp.ID=[Type]", con);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                ShowTraderGrid.DataSource = dt;
            }
            catch
            {
                MessageBox.Show("Sorry for the inconvience");
            }
}

        private void gunaAdvenceButton3_Click(object sender, EventArgs e)
        {
            showsubmenu(rawSubmenu);
        }


        //Trader section ends here



        //***Raw Material*****
        
        public void InitializeAddRawMaterial()
        {
            rawCost.Text = "MaterialCost";
            rawQuantity.SelectedIndex = -1;
            rawTransCharges.Text = "TransportCharges";
            rawLabourCharges.Text = "Labour Charges";
            paidAmount.Text = "Paid Amount";
        }
        private void Addition_Click(object sender, EventArgs e)
        {
            InitializeAddRawMaterial();
            PopulateRawEntryCombo();
            bunifuPages1.SetPage("rawMaterialEntry");
        }


        private void rawSupplierName_SelectedValueChanged(object sender, EventArgs e)
        {
            var con = Configuration.getInstance().getConnection();
            rawSupplierID.Items.Clear();
            SqlCommand cmd1 = new SqlCommand("select TraderRegNo from Traders where Traders.Name=@traderName", con);
            cmd1.Parameters.AddWithValue("@traderName", rawSupplierName.Text);
            SqlDataReader ds1 = cmd1.ExecuteReader();
            while (ds1.Read())
            {
                rawSupplierID.Items.Add(ds1[0]);
            }
            ds1.Close();
            rawSupplierID.SelectedIndex = 0;
        }
        public void PopulateRawEntryCombo()
        {
            rawSupplierID.Items.Clear();
            rawQuality.Items.Clear();
            rawSupplierName.Items.Clear();
            var con = Configuration.getInstance().getConnection();
            //SqlCommand cmd1 = new SqlCommand("select TraderRegNo from Traders where Traders.Type in (select ID from LookUp where Category='TraderType' and Value='OilSupplier')", con);
            //SqlDataReader ds1 = cmd1.ExecuteReader();
            //while (ds1.Read())
            //{
            //    rawSupplierID.Items.Add(ds1[0]);
            //}
            //ds1.Close();


            SqlCommand cmd2 = new SqlCommand("select Value from LookUp where Category='RawMaterialQuality'", con);
            SqlDataReader ds2 = cmd2.ExecuteReader();
            while (ds2.Read())
            {
                rawQuality.Items.Add(ds2[0]);
            }
            ds2.Close();


            SqlCommand cmd3 = new SqlCommand("select Distinct(Name) from Traders where Traders.Type in (select ID from LookUp where Category='TraderType' and Value='OilSupplier')", con);
            SqlDataReader ds3 = cmd3.ExecuteReader();
            while (ds3.Read())
            {
                rawSupplierName.Items.Add(ds3[0]);
            }
            ds3.Close();
        }
        public bool RawEntryCheckConstraints()
        {
            if (rawSupplierID.Text != "")
            {
                if (rawQuantity.Text != "")
                {
                    if (rawQuality.Text != "")
                    {
                        if (float.Parse(rawCost.Text)>0)
                        {
                            if (float.Parse(rawTransCharges.Text) > 0)
                            {
                                if(float.Parse(rawLabourCharges.Text) > 0)
                                {
                                    if (float.Parse(paidAmount.Text) > 0)
                                    {
                                        return true;
                                    }
                                    else
                                    {
                                        MessageBox.Show("Please check Paid Amount");
                                        return false;
                                    }
                                }
                                else
                                {
                                    MessageBox.Show("Please check Labour Charges");
                                    return false;
                                }
                            }
                            else
                            {
                                MessageBox.Show("Please check Transport Charges");
                                return false;
                            }
                        }
                        else
                        {
                            MessageBox.Show("Please check Raw Material Cpst");
                            return false;
                        }
                    }
                    else
                    {
                        MessageBox.Show("Please check Quality");
                        return false;
                    }
                }
                else
                {
                    MessageBox.Show("Please check Quantity");
                    return false;
                }
            }
            else
            {
                MessageBox.Show("Please check Supplier RegNo");
                return false;
            }
        }

        private void rawCost_TextChanged(object sender, EventArgs e)
        {

        }

        private void rawCost_Enter(object sender, EventArgs e)
        {
            rawCost.Text = "";
        }

        private void rawCost_Leave(object sender, EventArgs e)
        {
            if(rawCost.Text=="")
            {
                rawCost.Text = "MaterialCost";
            }
        }

        private void rawTransCharges_Enter(object sender, EventArgs e)
        {
            rawTransCharges.Text = "";
        }

        private void rawTransCharges_Leave(object sender, EventArgs e)
        {
            if (rawTransCharges.Text == "")
            {
                rawTransCharges.Text = "TransportCharges";
            }
        }

        private void rawLabourCharges_Enter(object sender, EventArgs e)
        {
            rawLabourCharges.Text = "";
        }

        private void rawLabourCharges_Leave(object sender, EventArgs e)
        {
            if (rawLabourCharges.Text == "")
            {
                rawLabourCharges.Text = "LabourCharges";
            }
        }
        private void paidAmount_Enter(object sender, EventArgs e)
        {
            paidAmount.Text = "";
        }
        private void paidAmount_Leave(object sender, EventArgs e)
        {
            if (paidAmount.Text == "")
            {
                paidAmount.Text = "Paid Amount";
            }
        }
        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                if (RawEntryCheckConstraints())
                {
                    try
                    {
                        var con = Configuration.getInstance().getConnection();

                        SqlCommand BeginTrans = new SqlCommand("Set Transaction Isolation Level Serializable Begin Transaction AddRawMaterial", con);
                        BeginTrans.ExecuteNonQuery();

                        SqlCommand getID = new SqlCommand("select ID from Traders where TraderRegNo=@regNo", con);
                        getID.Parameters.AddWithValue("@regNo", rawSupplierID.Text);
                        getID.ExecuteNonQuery();

                        SqlCommand getQuality = new SqlCommand("select ID from LookUp where Category='RawMaterialQuality' and Value=@value", con);
                        getQuality.Parameters.AddWithValue("@value", rawQuality.Text);
                        getQuality.ExecuteNonQuery();


                        SqlCommand cmd2 = new SqlCommand("EXECUTE AddExpences @raw,@trans,@labour,@net", con);
                        cmd2.Parameters.AddWithValue("@raw", rawCost.Text);
                        cmd2.Parameters.AddWithValue("@trans", rawTransCharges.Text);
                        cmd2.Parameters.AddWithValue("@labour", rawLabourCharges.Text);
                        cmd2.Parameters.AddWithValue("@net", float.Parse(rawCost.Text) + float.Parse(rawTransCharges.Text) + float.Parse(rawLabourCharges.Text));
                        cmd2.ExecuteNonQuery();

                        SqlCommand batchID = new SqlCommand("SELECT MAX(BatchID) FROM Expenses", con);
                        batchID.ExecuteNonQuery();

                        SqlCommand cmd1 = new SqlCommand("Insert into RawMaterial values(@batchID,@ProviderID,@Quantity,@Quality,@date)", con);
                        cmd1.Parameters.AddWithValue("@batchID", int.Parse(batchID.ExecuteScalar().ToString()));
                        cmd1.Parameters.AddWithValue("@ProviderID", rawSupplierID.Text);
                        cmd1.Parameters.AddWithValue("@Quantity", float.Parse(rawQuantity.Text));
                        cmd1.Parameters.AddWithValue("@Quality", int.Parse(getQuality.ExecuteScalar().ToString()));
                        cmd1.Parameters.AddWithValue("@date", DateTime.Now);
                        cmd1.ExecuteNonQuery();







                        SqlCommand getAccountBalance = new SqlCommand("select Balance from Account where AccountNo=(select AccountNumber from Traders where TraderRegNo=@regNo)", con);
                        getAccountBalance.Parameters.AddWithValue("@regNo", rawSupplierID.Text);
                        getAccountBalance.ExecuteNonQuery();

                        SqlCommand getAccountNo = new SqlCommand("select AccountNumber from Traders where TraderRegNo=@regNo", con);
                        getAccountNo.Parameters.AddWithValue("@regNo", rawSupplierID.Text);
                        getAccountNo.ExecuteNonQuery();

                        SqlCommand cmd4 = new SqlCommand("EXECUTE UpdateAccount @account,@balance", con);
                        cmd4.Parameters.AddWithValue("@account", getAccountNo.ExecuteScalar().ToString());
                        cmd4.Parameters.AddWithValue("@balance", float.Parse(getAccountBalance.ExecuteScalar().ToString()) + (float.Parse(rawCost.Text) + float.Parse(rawTransCharges.Text) + float.Parse(rawLabourCharges.Text)) - float.Parse(paidAmount.Text));
                        cmd4.ExecuteNonQuery();

                        SqlCommand getCompanyAccountBalance = new SqlCommand("select Balance from Account where AccountNo='1111'", con);
                        getCompanyAccountBalance.ExecuteNonQuery();

                        SqlCommand cmd5 = new SqlCommand("EXECUTE UpdateAccount '1111',@balance", con);
                        cmd5.Parameters.AddWithValue("@balance", float.Parse(getCompanyAccountBalance.ExecuteScalar().ToString()) - float.Parse(paidAmount.Text));
                        cmd5.ExecuteNonQuery();




                        SqlCommand cmd6 = new SqlCommand("Insert into Transactions values(@des,@date,@paidby,@receiver,@Amount)", con);
                        cmd6.Parameters.AddWithValue("@des", "Amount paid in the proccess of purchasing raw material against BatchID=" + batchID.ExecuteScalar().ToString());
                        cmd6.Parameters.AddWithValue("@date", DateTime.Now);
                        cmd6.Parameters.AddWithValue("@paidby", "1111");
                        cmd6.Parameters.AddWithValue("@receiver", getAccountNo.ExecuteScalar().ToString());
                        cmd6.Parameters.AddWithValue("Amount", float.Parse(paidAmount.Text));
                        cmd6.ExecuteNonQuery();

                        SqlCommand transID = new SqlCommand("SELECT MAX(ID) FROM Transactions", con);
                        transID.ExecuteNonQuery();


                        SqlCommand cmd7 = new SqlCommand("insert into TraderAccounts values(@transID,@traderID,@batchID,@payable,@paid,@balance)", con);
                        cmd7.Parameters.AddWithValue("@transID", int.Parse(transID.ExecuteScalar().ToString()));
                        cmd7.Parameters.AddWithValue("@traderID", rawSupplierID.Text);
                        cmd7.Parameters.AddWithValue("@batchID", int.Parse(batchID.ExecuteScalar().ToString()));
                        cmd7.Parameters.AddWithValue("@payable", float.Parse(rawCost.Text) + float.Parse(rawTransCharges.Text) + float.Parse(rawLabourCharges.Text));
                        cmd7.Parameters.AddWithValue("@paid", float.Parse(paidAmount.Text));
                        cmd7.Parameters.AddWithValue("@balance", float.Parse(getAccountBalance.ExecuteScalar().ToString()));
                        cmd7.ExecuteNonQuery();


                        SqlCommand EndTrans = new SqlCommand("Commit Transaction AddRawMaterial", con);
                        EndTrans.ExecuteNonQuery();
                        MessageBox.Show("Data is added successfully");
                        InitializeAddRawMaterial();
                        PopulateRawEntryCombo();
                    }
                    catch (Exception ex)
                    {
                        var con = Configuration.getInstance().getConnection();

                        SqlCommand EndTrans = new SqlCommand("RollBack Transaction AddRawMaterial", con);
                        EndTrans.ExecuteNonQuery();

                        MessageBox.Show( ex.ToString() +"Sorry for the inconvience caused by the system");
                    }
                }
            }
            catch
            {
                MessageBox.Show("Please check input fields");
            }
        }

        private void bunifuButton4_Click(object sender, EventArgs e)
        {
            try
            {
                bunifuPages1.SetPage("RawMaterial");
                var con = Configuration.getInstance().getConnection();
                SqlCommand cmd = new SqlCommand("select BatchID,RawMaterial.ProviderID as TraderRegNo,Traders.Name as TraderName,QuantityInBarrel,LookUp.Value as QualityLevel,Date as DateOfPurchase from RawMaterial,LookUp,Traders where Traders.TraderRegNo=RawMaterial.ProviderID and  RawMaterial.QualityLevel=LookUp.ID and LookUp.Category='RawMaterialQuality' and   RawMaterial.BatchID NOT IN(select BatchID from OutBatches)", con);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                rawMaterialGrid.DataSource = dt;
            }
            catch
            {
                MessageBox.Show("Sorry for thr inconvience");
            }
        }


        ///****Raw Material section ends here
        ///Purchased Product section
        public bool PurEntryCheckConstraints()
        {
            if (purProTrReg.Text != "")
            {
                if (purPoQty.Text != "0")
                {
                    if (purProDes.Text != "Description")
                    {
                        if (float.Parse(purProUnitPr.Text) > 0)
                        {
                            if (float.Parse(purProTransCharg.Text) > 0)
                            {
                                if (float.Parse(purProLabCharg.Text) > 0)
                                {
                                    if (float.Parse(purProPaidAmn.Text) > 0)
                                    {
                                        return true;
                                    }
                                    else
                                    {
                                        MessageBox.Show("Please check Paid Amount");
                                        return false;
                                    }
                                }
                                else
                                {
                                    MessageBox.Show("Please check Labour Charges");
                                    return false;
                                }
                            }
                            else
                            {
                                MessageBox.Show("Please check Transport Charges");
                                return false;
                            }
                        }
                        else
                        {
                            MessageBox.Show("Please check unit price");
                            return false;
                        }
                    }
                    else
                    {
                        MessageBox.Show("Please check Description");
                        return false;
                    }
                }
                else
                {
                    MessageBox.Show("Please check Quantity");
                    return false;
                }
            }
            else
            {
                MessageBox.Show("Please check supplier Reg No");
                return false;
            }
        }


        private void purProTraderName_SelectedValueChanged(object sender, EventArgs e)
        {
            var con = Configuration.getInstance().getConnection();
            purProTrReg.Items.Clear();
            SqlCommand cmd1 = new SqlCommand("select TraderRegNo from Traders where Traders.Name=@traderName", con);
            cmd1.Parameters.AddWithValue("@traderName", purProTraderName.Text);
            SqlDataReader ds1 = cmd1.ExecuteReader();
            while (ds1.Read())
            {
                purProTrReg.Items.Add(ds1[0]);
            }
            ds1.Close();
            purProTrReg.SelectedIndex = 0;
        }

        public void PpopulateAddPurchseProduct()
        {
            purProTrReg.Items.Clear();
            purProTraderName.Items.Clear();
            var con = Configuration.getInstance().getConnection();
            //SqlCommand cmd1 = new SqlCommand("select TraderRegNo from Traders where Traders.Type in (select ID from LookUp where Category='TraderType' and Value<>'OilSupplier')", con);
            //SqlDataReader ds1 = cmd1.ExecuteReader();
            //while (ds1.Read())
            //{
            //    purProTrReg.Items.Add(ds1[0]);
            //}
            //ds1.Close();




            SqlCommand cmd3 = new SqlCommand("select Distinct(Name) from Traders where Traders.Type  in (select ID from LookUp where Category='TraderType' and Value<>'OilSupplier')", con);
            SqlDataReader ds3 = cmd3.ExecuteReader();
            while (ds3.Read())
            {
                purProTraderName.Items.Add(ds3[0]);
            }
            ds3.Close();
        }
        private void gunaAdvenceButton4_Click(object sender, EventArgs e)
        {
            showsubmenu(purchasedProductSubMenu);
        }
        public void InitializePurProd()
        {
            purPoQty.Value = 0;
            purProDes.Text = "Description";
            purProUnitPr.Text = "Unit Price";
            purProTransCharg.Text = "TransportCharges";
            purProLabCharg.Text = "Labour Charges";
            purProPaidAmn.Text = "Paid Amount";
        }
        private void bunifuButton5_Click(object sender, EventArgs e)
        {
            PpopulateAddPurchseProduct();
            InitializePurProd();
            bunifuPages1.SetPage("addPurProd");
           

        }

        private void purProDes_Enter(object sender, EventArgs e)
        {
            purProDes.Text = "";
        }

        private void purProDes_Leave(object sender, EventArgs e)
        {
            if(purProDes.Text=="")
            {
                purProDes.Text = "Description";
            }
        }

        private void purProUnitPr_Leave(object sender, EventArgs e)
        {
            if (purProUnitPr.Text == "")
            {
                purProUnitPr.Text = "Unit Price";
            }
        }

        private void purProUnitPr_Enter(object sender, EventArgs e)
        {
            purProUnitPr.Text = "";
        }

        private void purProLabCharg_Enter(object sender, EventArgs e)
        {
            purProLabCharg.Text = "";
        }

        private void purProLabCharg_Leave(object sender, EventArgs e)
        {
            if (purProLabCharg.Text == "")
            {
                purProLabCharg.Text = "Labour Charges";
            }
        }

        private void purProTransCharg_Leave(object sender, EventArgs e)
        {
            if(purProTransCharg.Text=="")
            {
                purProTransCharg.Text = "Transport Charges";
            }
        }

        private void purProTransCharg_Enter(object sender, EventArgs e)
        {
            purProTransCharg.Text = "";
        }

        private void purProPaidAmn_Leave(object sender, EventArgs e)
        {
            if(purProPaidAmn.Text=="")
            {
                purProPaidAmn.Text=("Paid Amount");
            }
        }

        private void purProPaidAmn_Enter(object sender, EventArgs e)
        {
            purProPaidAmn.Text = "";
        }

        private void button5_Click(object sender, EventArgs e)
        {
            try
            {
                if (PurEntryCheckConstraints())
                {
                    try
                    {
                        var con = Configuration.getInstance().getConnection();
                        SqlCommand BeginTrans = new SqlCommand("Set Transaction Isolation Level Serializable Begin Transaction AddPurchaseProduct", con);
                        BeginTrans.ExecuteNonQuery();

                        SqlCommand cmd2 = new SqlCommand("EXECUTE AddExpences @raw,@trans,@labour,@net", con);
                        cmd2.Parameters.AddWithValue("@raw", float.Parse((float.Parse(purPoQty.Value.ToString()) * float.Parse(purProUnitPr.Text)).ToString()));
                        cmd2.Parameters.AddWithValue("@trans", purProTransCharg.Text);
                        cmd2.Parameters.AddWithValue("@labour", purProLabCharg.Text);
                        cmd2.Parameters.AddWithValue("@net", float.Parse(purPoQty.Value.ToString()) * float.Parse(purProUnitPr.Text) + float.Parse(purProTransCharg.Text) + float.Parse(purProLabCharg.Text));
                        cmd2.ExecuteNonQuery();

                        SqlCommand batchID = new SqlCommand("SELECT MAX(BatchID) FROM Expenses", con);
                        batchID.ExecuteNonQuery();

                        SqlCommand cmd1 = new SqlCommand("Insert into PurchasedProducts values(@batchID,@ProviderID,@des,@time,@quan,@unit)", con);
                        cmd1.Parameters.AddWithValue("@batchID", int.Parse(batchID.ExecuteScalar().ToString()));
                        cmd1.Parameters.AddWithValue("@ProviderID", purProTrReg.Text);
                        cmd1.Parameters.AddWithValue("@des", purProDes.Text);
                        cmd1.Parameters.AddWithValue("@quan", int.Parse(purPoQty.Value.ToString()));
                        cmd1.Parameters.AddWithValue("@unit", float.Parse(purProUnitPr.Text));
                        cmd1.Parameters.AddWithValue("@time", DateTime.Now);
                        cmd1.ExecuteNonQuery();



                        SqlCommand getAccountBalance = new SqlCommand("select Balance from Account where AccountNo=(select AccountNumber from Traders where TraderRegNo=@regNo)", con);
                        getAccountBalance.Parameters.AddWithValue("@regNo", purProTrReg.Text);
                        getAccountBalance.ExecuteNonQuery();

                        SqlCommand getAccountNo = new SqlCommand("select AccountNumber from Traders where TraderRegNo=@regNo", con);
                        getAccountNo.Parameters.AddWithValue("@regNo", purProTrReg.Text);
                        getAccountNo.ExecuteNonQuery();

                        SqlCommand cmd4 = new SqlCommand("EXECUTE UpdateAccount @account,@balance", con);
                        cmd4.Parameters.AddWithValue("@account", getAccountNo.ExecuteScalar().ToString());
                        cmd4.Parameters.AddWithValue("@balance", float.Parse(getAccountBalance.ExecuteScalar().ToString()) + (float.Parse(purPoQty.Value.ToString()) * float.Parse(purProUnitPr.Text) + float.Parse(purProTransCharg.Text) + float.Parse(purProLabCharg.Text)) - float.Parse(purProPaidAmn.Text));
                        cmd4.ExecuteNonQuery();


                        SqlCommand getCompanyAccountBalance = new SqlCommand("select Balance from Account where AccountNo='1111'", con);
                        getCompanyAccountBalance.ExecuteNonQuery();

                        SqlCommand cmd5 = new SqlCommand("EXECUTE UpdateAccount '1111',@balance", con);
                        cmd5.Parameters.AddWithValue("@balance", float.Parse(getCompanyAccountBalance.ExecuteScalar().ToString()) - float.Parse(purProPaidAmn.Text));
                        cmd5.ExecuteNonQuery();


                        SqlCommand cmd6 = new SqlCommand("Insert into Transactions values(@des,@date,@paidby,@receiver,@Amount)", con);
                        cmd6.Parameters.AddWithValue("@des", "Amount paid in the proccess of purchasing product against BatchID=" + batchID.ExecuteScalar().ToString());
                        cmd6.Parameters.AddWithValue("@date", DateTime.Now);
                        cmd6.Parameters.AddWithValue("@paidby", "1111");
                        cmd6.Parameters.AddWithValue("@receiver", getAccountNo.ExecuteScalar().ToString());
                        cmd6.Parameters.AddWithValue("Amount", float.Parse(purProPaidAmn.Text));
                        cmd6.ExecuteNonQuery();

                        SqlCommand transID = new SqlCommand("SELECT MAX(ID) FROM Transactions", con);
                        transID.ExecuteNonQuery();



                        SqlCommand cmd7 = new SqlCommand("insert into TraderAccounts values(@transID,@traderID,@batchID,@payable,@paid,@balance)", con);
                        cmd7.Parameters.AddWithValue("@transID", int.Parse(transID.ExecuteScalar().ToString()));
                        cmd7.Parameters.AddWithValue("@traderID", purProTrReg.Text);
                        cmd7.Parameters.AddWithValue("@batchID", int.Parse(batchID.ExecuteScalar().ToString()));
                        cmd7.Parameters.AddWithValue("@payable", float.Parse(purPoQty.Value.ToString()) * float.Parse(purProUnitPr.Text) + float.Parse(purProTransCharg.Text) + float.Parse(purProLabCharg.Text));
                        cmd7.Parameters.AddWithValue("@paid", float.Parse(purProPaidAmn.Text));
                        cmd7.Parameters.AddWithValue("@balance", float.Parse(getAccountBalance.ExecuteScalar().ToString()));
                        cmd7.ExecuteNonQuery();


                        SqlCommand EndTrans = new SqlCommand("Commit Transaction AddRawMaterial", con);
                        EndTrans.ExecuteNonQuery();
                        MessageBox.Show("Data is added successfully");
                        PpopulateAddPurchseProduct();
                        InitializePurProd();
                    }
                    catch
                    {
                        var con = Configuration.getInstance().getConnection();
                        SqlCommand EndTrans = new SqlCommand("Commit Transaction AddRawMaterial", con);
                        EndTrans.ExecuteNonQuery();
                        MessageBox.Show("Sorry for the inconvience");
                    }

                }
            }
            catch
            {
                MessageBox.Show("Please check the input fields");
            }
        }

        private void bunifuButton6_Click(object sender, EventArgs e)
        {
            try
            {
                bunifuPages1.SetPage("showPurProd");
                var con = Configuration.getInstance().getConnection();
                SqlCommand cmd = new SqlCommand("select BatchID,Traders.Name as TraderName,LookUp.Value as TraderType,Description,TimeStamp,Quantity,UnitPrice  from PurchasedProducts,Traders,LookUp  where Traders.TraderRegNo=PurchasedProducts.TraderID and Traders.Type=LookUp.ID and LookUp.Category='TraderType'", con);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                purchasedProductGrid.DataSource = dt;
            }
            catch
            {
                MessageBox.Show("Sorry for thr inconvience caused by system");
            }
        }



        ///*****Purchased Products section ends

        //*** Company Products section starts here***
        public void InitializeAddComPro()
        {
            compProName.Text = "Product Name";
            compProDes.Text = "Description";
            compProUnitPrice.Text = "Unit Price";
        }
        private void bunifuButton7_Click(object sender, EventArgs e)
        {
            InitializeAddComPro();
            bunifuPages1.SetPage("addProduct");
        }

        private void gunaAdvenceButton5_Click(object sender, EventArgs e)
        {
            showsubmenu(compProd);
        }

        private void gunaLineTextBox4_Leave(object sender, EventArgs e)
        {
            if(compProName.Text=="")
            {
                compProName.Text = "Product Name";
            }
        }

        private void compProName_Enter(object sender, EventArgs e)
        {
            compProName.Text = "";
        }

        private void compProDes_Leave(object sender, EventArgs e)
        {
            if(compProDes.Text=="")
            {
                compProDes.Text = "Description";
            }
        }

        private void compProDes_Enter(object sender, EventArgs e)
        {
            compProDes.Text = "";
        }

        private void compProUnitPrice_Leave(object sender, EventArgs e)
        {
            if(compProUnitPrice.Text=="")
            {
                compProUnitPrice.Text = "Unit Price";
            }
        }

        private void compProUnitPrice_Enter(object sender, EventArgs e)
        {
            compProUnitPrice.Text = "";
        }

        private void button6_Click(object sender, EventArgs e)
        {
            try
            {
                if(compProName.Text!="Product Name")
                {
                    if(compProDes.Text!="Description")
                    {
                        if(float.Parse(compProUnitPrice.Text)>0)
                        {
                            try
                            {
                                var con = Configuration.getInstance().getConnection();
                                SqlCommand cmd1 = new SqlCommand("insert into Products values(@name,@des,@quant,@unitPrice)", con);
                                cmd1.Parameters.AddWithValue("@name", compProName.Text);
                                cmd1.Parameters.AddWithValue("@des", compProDes.Text);

                                cmd1.Parameters.AddWithValue("@quant",float.Parse("0"));
                                cmd1.Parameters.AddWithValue("@unitPrice", float.Parse(compProUnitPrice.Text));

                                cmd1.ExecuteNonQuery();
                                MessageBox.Show("Product is added successfully");
                                InitializeAddComPro();
                            }
                            catch
                            {
                                MessageBox.Show("Sorry for thr inconvience caused by the system");
                            }
                        }
                        else
                        {
                            MessageBox.Show("Please check the unit price");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Please enter the description");
                    }
                }
                else
                {
                    MessageBox.Show("Please entre the product name");
                }
            }
            catch
            {
                MessageBox.Show("Please check the unit price field");
            }
        }
        public void PopulateUpdateCompProduct()
        {
            updDelProSearch.Items.Clear();
            var con = Configuration.getInstance().getConnection();
            SqlCommand cmd1 = new SqlCommand("select Name from Products", con);
            updDelProSearch.Items.Add("Search");
            SqlDataReader ds1 = cmd1.ExecuteReader();
            while (ds1.Read())
            {
                updDelProSearch.Items.Add(ds1[0]);
            }
            ds1.Close();
            updDelProSearch.SelectedIndex = 0;
        }
        public void InitializeUpdateComPro()
        {
            updDelProName.Text = "Product Name";
            updDelProDescription.Text = "Description";
            updDelProUnitPrice.Text = "Unit Price";
        }
        private void bunifuButton8_Click(object sender, EventArgs e)
        {
            PopulateUpdateCompProduct();
            InitializeUpdateComPro();
            bunifuPages1.SetPage("updateComProduct");
        }

        private void updDelProSearch_SelectedValueChanged(object sender, EventArgs e)
        {
            if (updDelProSearch.Text != "Search")
            {
                updDelProDescription.ReadOnly = false;
                updDelProUnitPrice.ReadOnly = false;
                var con = Configuration.getInstance().getConnection();
                SqlCommand cmd1 = new SqlCommand("select * from Products where Name=@name", con);
                cmd1.Parameters.AddWithValue("@name", updDelProSearch.Text);
                SqlDataAdapter da = new SqlDataAdapter(cmd1);
                DataTable dt = new DataTable();
                da.Fill(dt);
                updDelComProGrid.DataSource = dt;
                updDelProName.Text = updDelComProGrid.Rows[0].Cells[1].FormattedValue.ToString();
                updDelProDescription.Text = updDelComProGrid.Rows[0].Cells[2].FormattedValue.ToString();
                updDelProUnitPrice.Text = updDelComProGrid.Rows[0].Cells[4].FormattedValue.ToString();
            }
            else
            {
                updDelProDescription.ReadOnly = true;
                updDelProUnitPrice.ReadOnly = true;

                updDelProName.Text = "Name";
                updDelProDescription.Text = "Description";
                updDelProUnitPrice.Text = "Unit Price";
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            if(updDelProSearch.Text!="Search")
            {
                try
                {
                    DialogResult dialogResult = MessageBox.Show("Are you sure to delete?", "Confirmation", MessageBoxButtons.YesNo);

                    if (dialogResult == DialogResult.Yes)
                    {
                        var con = Configuration.getInstance().getConnection();
                        SqlCommand cmd1 = new SqlCommand("DELETE FROM Products WHERE Name=@name", con);
                        cmd1.Parameters.AddWithValue("@name", updDelProSearch.Text);
                        cmd1.ExecuteNonQuery();
                        MessageBox.Show("Product is deleted successfully");
                        InitializeUpdateComPro();
                    }
                }
                catch
                {
                    MessageBox.Show("Sorry for the inconvience caused by the system");
                }
            }
            else
            {
                MessageBox.Show("Please select the Product from given list");
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            try
            {
                if (updDelProSearch.Text != "Search")
                {
                    if (updDelProDescription.Text != "")
                    {
                        if (float.Parse(updDelProUnitPrice.Text) > 0)
                        {
                            try
                            {
                                DialogResult dialogResult = MessageBox.Show("Are you sure to update?", "Confirmation", MessageBoxButtons.YesNo);

                                if (dialogResult == DialogResult.Yes)
                                {
                                    var con = Configuration.getInstance().getConnection();
                                    SqlCommand cmd1 = new SqlCommand("Update Products set Description=@des,PerLitrePrice=@unitPrice where Name=@name", con);
                                    cmd1.Parameters.AddWithValue("@name", updDelProSearch.Text);
                                    cmd1.Parameters.AddWithValue("@des", updDelProDescription.Text);
                                    cmd1.Parameters.AddWithValue("@unitPrice", updDelProUnitPrice.Text);
                                    cmd1.ExecuteNonQuery();
                                    MessageBox.Show("Product is updated successfully");
                                    InitializeUpdateComPro();
                                }
                            }
                            catch
                            {
                                MessageBox.Show("Sorry for the inconvience caused by the system");
                            }
                        }
                        else
                        {
                            MessageBox.Show("Please check the Unit Price");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Please enter the description");
                    }
                }
                else
                {
                    MessageBox.Show("Please select the product from the given list");
                }
            }
            catch
            {
                MessageBox.Show("Please check the unit price");
            }
        }

        private void bunifuButton9_Click(object sender, EventArgs e)
        {
        }

        private void MainAdminScreen_Load(object sender, EventArgs e)
        {

        }

        private void bunifuButton9_Click_1(object sender, EventArgs e)
        {
            bunifuPages1.SetPage("ShowCompPro");
            var con = Configuration.getInstance().getConnection();
            SqlCommand cmd = new SqlCommand("select * from Products", con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            showComProGrid.DataSource = dt;
        }
        //***Company Products end here***


        //****Out BATCHES SECTION STARTS HERE
        public void PoupulateOutBatchesProductQuantity()
        {
            var con = Configuration.getInstance().getConnection();
            SqlCommand cmd = new SqlCommand("select Name from Products order by ID", con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            dt.Columns.Add(new DataColumn("OutputQuantity", typeof(string)));
            OutBatchesProduct.DataSource = dt;
            OutBatchesProduct.Columns["Name"].ReadOnly = true;

        }
        public void PopulateOutBatchesSearch()
        {
            outBatchesSearch.Items.Clear();
            var con = Configuration.getInstance().getConnection();
            SqlCommand cmd1 = new SqlCommand("select BatchID from RawMaterial where BatchID not in( select BatchID from OutBatches)", con);
            outBatchesSearch.Items.Add("Search");
            SqlDataReader ds1 = cmd1.ExecuteReader();
            while (ds1.Read())
            {
                outBatchesSearch.Items.Add(ds1[0]);
            }
            ds1.Close();
            outBatchesSearch.SelectedIndex = 0;
        }
        private void gunaAdvenceButton6_Click(object sender, EventArgs e)
        {
            HideSubMenu();
            bunifuPages1.SetPage("outBatches");
            var con = Configuration.getInstance().getConnection();
            SqlCommand cmd = new SqlCommand("select BatchID,ProviderID,QuantityInBarrel,Value as QualityLevel,Date AS DateOfPurchase  from RawMaterial,LookUp where BatchID=@batchID and LookUp.ID=RawMaterial.QualityLevel and Category='RawMaterialQuality'", con);
            cmd.Parameters.AddWithValue("@batchID", int.Parse("0"));
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            OutBatchesGrid.DataSource = dt;
            PopulateOutBatchesSearch();
            PoupulateOutBatchesProductQuantity();

        }

        private void outBatchesSearch_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void outBatchesSearch_SelectedValueChanged(object sender, EventArgs e)
        {
            if(outBatchesSearch.Text!="Search" )
            {
                var con = Configuration.getInstance().getConnection();
                SqlCommand cmd = new SqlCommand("select BatchID,ProviderID,QuantityInBarrel,Value as QualityLevel,Date AS DateOfPurchase  from RawMaterial,LookUp where BatchID=@batchID and LookUp.ID=RawMaterial.QualityLevel and Category='RawMaterialQuality'", con);
                cmd.Parameters.AddWithValue("@batchID", int.Parse(outBatchesSearch.Text));
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                OutBatchesGrid.DataSource = dt;
            }
        }
        public void AddProductOfOutbatches()
        {
            try
            {
                var con = Configuration.getInstance().getConnection();
                SqlCommand cmd0 = new SqlCommand("Begin Transaction OutBatches", con);
                cmd0.ExecuteNonQuery();




                SqlCommand cmd1 = new SqlCommand("insert into OutBatches values(@batchID,@date)", con);
                cmd1.Parameters.AddWithValue("@batchID", int.Parse(OutBatchesGrid.Rows[0].Cells[0].FormattedValue.ToString()));
                cmd1.Parameters.AddWithValue("date", DateTime.Now);
                cmd1.ExecuteNonQuery();


                SqlCommand getBatchNo = new SqlCommand("SELECT MAX(BatchNo) FROM OutBatches", con);
                getBatchNo.ExecuteNonQuery();




                for (int x = 1; x <= OutBatchesProduct.Rows.Count; x++)
                {
                    var con2 = Configuration.getInstance().getConnection();

                    SqlCommand productQuantity = new SqlCommand("select Quantity from Products where Name=@name", con2);
                    productQuantity.Parameters.AddWithValue("@name", OutBatchesProduct.Rows[x - 1].Cells[0].FormattedValue.ToString());
                    productQuantity.ExecuteNonQuery();

                    SqlCommand updateProductQuantity = new SqlCommand("update Products set Quantity=@quantity where Name=@name", con2);
                    updateProductQuantity.Parameters.AddWithValue("@name", OutBatchesProduct.Rows[x - 1].Cells[0].FormattedValue.ToString());
                    updateProductQuantity.Parameters.AddWithValue("@quantity", float.Parse(productQuantity.ExecuteScalar().ToString()) + float.Parse(OutBatchesProduct.Rows[x - 1].Cells[1].FormattedValue.ToString()));
                    updateProductQuantity.ExecuteNonQuery();


                    SqlCommand getProductID = new SqlCommand("Select ID from Products where Name=@name", con2);
                    getProductID.Parameters.AddWithValue("@name", OutBatchesProduct.Rows[x - 1].Cells[0].FormattedValue.ToString());
                    getProductID.ExecuteNonQuery();


                    SqlCommand cmd2 = new SqlCommand("insert into Inventory values(@batchNo,@productID,@quantity)", con2);
                    cmd2.Parameters.AddWithValue("@batchNo", int.Parse(getBatchNo.ExecuteScalar().ToString()));
                    cmd2.Parameters.AddWithValue("@productID", int.Parse(getProductID.ExecuteScalar().ToString()));
                    cmd2.Parameters.AddWithValue("@quantity", float.Parse(OutBatchesProduct.Rows[x - 1].Cells[1].FormattedValue.ToString()));
                    cmd2.ExecuteNonQuery();
                }


                var con3 = Configuration.getInstance().getConnection();
                SqlCommand cmd3 = new SqlCommand("Commit Transaction OutBatches", con3);
                cmd3.ExecuteNonQuery();
            }
            catch
            {
                var con3 = Configuration.getInstance().getConnection();
                SqlCommand cmd3 = new SqlCommand("RollBack Transaction OutBatches", con3);
                cmd3.ExecuteNonQuery();
                MessageBox.Show("You are unable to perform this task");
            }
        }
        public bool checkOutProductsQuantity()
        {
            float Quantity = 0;
            for (int x = 1; x < OutBatchesProduct.Rows.Count; x++)
            {
                if(OutBatchesProduct.Rows[x - 1].Cells[1].FormattedValue.ToString()!=null)
                {
                    try
                    {
                        float.Parse(OutBatchesProduct.Rows[x - 1].Cells[1].FormattedValue.ToString());
                    }
                    catch
                    {
                        return false;
                    }
                    if(float.Parse(OutBatchesProduct.Rows[x - 1].Cells[1].FormattedValue.ToString())>0)
                    {
                        Quantity = Quantity + float.Parse(OutBatchesProduct.Rows[x - 1].Cells[1].FormattedValue.ToString());
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            if (Quantity <= (float.Parse(OutBatchesGrid.Rows[0].Cells[2].FormattedValue.ToString()) * 159))
            {
                return true;
            }
            return false;
        }
        private void button9_Click(object sender, EventArgs e)
        {
            if(OutBatchesGrid.Rows.Count>0)
            {
                if (checkOutProductsQuantity())
                {
                        
                    AddProductOfOutbatches();
                    OutBatchesGrid.DataSource = "";
                    PopulateOutBatchesSearch();
                    MessageBox.Show("The selected raw material batch is registered as refined");
                }
                else
                {
                    MessageBox.Show("Please check the quantity of the output products");
                }

            }
            else
            {
                MessageBox.Show("Please select the outbatch from the given list");
            }




        }

        
        private void gunaAdvenceButton2_Click_1(object sender, EventArgs e)
        {
            showsubmenu(servucProvidersubmenu);
        }


        //***outBatches secttion ends here***
        ////***Service Provider
        private void bunifuButton10_Click(object sender, EventArgs e)
        {
            InitializeServiceProviderSubmit();
            bunifuPages1.SetPage("addServiceProvider");
        }

        public void PopulateSubmitServiceProvider()
        {
            subSerProType.Items.Clear();
            var con = Configuration.getInstance().getConnection();
            SqlCommand cmd1 = new SqlCommand("select Value from LookUp where Category='ServiceProviderType'", con);
            subSerProType.Items.Add("Type");
            SqlDataReader ds1 = cmd1.ExecuteReader();
            while (ds1.Read())
            {
                subSerProType.Items.Add(ds1[0]);
            }
            ds1.Close();
            subSerProType.SelectedIndex = 0;
        }
        public void InitializeServiceProviderSubmit()
        {
            subSerProName.Text = "Registered Name";
            subSerProAccountNumber.Text = "Account Number";
            PopulateSubmitServiceProvider();
        }
        private void subSerProName_Leave(object sender, EventArgs e)
        {
            if(subSerProName.Text=="")
            {
                subSerProName.Text = "Registered Name";
            }
        }

        private void subSerProName_Enter(object sender, EventArgs e)
        {
            subSerProName.Text = "";
        }

        private void subSerProAccountNumber_Leave(object sender, EventArgs e)
        {
            if(subSerProAccountNumber.Text=="")
            {
                subSerProAccountNumber.Text="Account Number";
            }
        }

        private void subSerProAccountNumber_Enter(object sender, EventArgs e)
        {
            subSerProAccountNumber.Text = "";
        }
        public void InsertServiceProvider()
        {
            var con = Configuration.getInstance().getConnection();

            SqlCommand BeginTrans = new SqlCommand("Set Transaction Isolation Level Serializable Begin Transaction AddServiceProvider", con);
            BeginTrans.ExecuteNonQuery();

            SqlCommand cmd1 = new SqlCommand("Select ID from Lookup where Category='PersonType' AND Value='ServiceProvider'", con);
            cmd1.ExecuteNonQuery();

            SqlCommand cmd2 = new SqlCommand("Insert into Person values (@PersonType)", con);
            cmd2.Parameters.AddWithValue("@PersonType", int.Parse(cmd1.ExecuteScalar().ToString()));
            cmd2.ExecuteNonQuery();

            SqlCommand getID = new SqlCommand("SELECT MAX(Id) FROM Person", con);
            getID.ExecuteNonQuery();

            SqlCommand cmd4 = new SqlCommand("Insert into Account values(@AccountNo,@HolderID,@Balance)", con);
            cmd4.Parameters.AddWithValue("@AccountNo", subSerProAccountNumber.Text);
            cmd4.Parameters.AddWithValue("@HolderID", int.Parse(getID.ExecuteScalar().ToString()));
            cmd4.Parameters.AddWithValue("@Balance", float.Parse("0"));
            cmd4.ExecuteNonQuery();

            SqlCommand cmd3 = new SqlCommand("Insert into ServiceProviders values(@RegNo,@ID,@SPType,@Name)", con);




            SqlCommand getType = new SqlCommand("Select ID from Lookup where Category='ServiceProviderType' AND Value=@Type", con);
            getType.Parameters.AddWithValue("@Type", subSerProType.Text);
            getType.ExecuteNonQuery();

            regNo = DateTime.Now.Year.ToString() + "-SP-" + int.Parse(getID.ExecuteScalar().ToString());
            cmd3.Parameters.AddWithValue("@RegNo", DateTime.Now.Year.ToString() + "-SP-" + int.Parse(getID.ExecuteScalar().ToString()));
            cmd3.Parameters.AddWithValue("@Id", int.Parse(getID.ExecuteScalar().ToString()));
            cmd3.Parameters.AddWithValue("@Name", subSerProName.Text);
            cmd3.Parameters.AddWithValue("@SPType", int.Parse(getType.ExecuteScalar().ToString()));
            cmd3.ExecuteNonQuery();


            SqlCommand EndTrans = new SqlCommand("Commit Transaction AddServiceProvider", con);
            EndTrans.ExecuteNonQuery();

        }
        public bool validatesubSerPro()
        {
            if (subSerProName.Text != "Registered Name")
            {
                if(subSerProType.Text!="Type")
                {
                    if(subSerProAccountNumber.Text != "Account Number" && Regex.IsMatch(subSerProAccountNumber.Text, @"(^[0-9]{16,20}$)"))
                    {
                        return true;
                    }
                    else
                    {
                        MessageBox.Show("Please check Accout number");
                        return false;
                    }
                }
                else
                {
                    MessageBox.Show("Please select type");
                    return false;
                }
            }
            else
            {
                MessageBox.Show("Please enter the name");
                return false;
            }
        }
        private void button10_Click(object sender, EventArgs e)
        {
            if (validatesubSerPro())
            {
                
                
                InsertServiceProvider();
                MessageBox.Show("Trader is added successfully\nThe reg no assinged to the the Trader by the system is " + regNo + " .\nPlease remember this one to access your data");
                InitializeServiceProviderSubmit();
            }
        }
        public void PopulateSerProUpdateCombo()
        {
            updSerProSearch.Items.Clear();

            updSerProType.Items.Clear();
            var con = Configuration.getInstance().getConnection();
            SqlCommand cmd2 = new SqlCommand("select Value from LookUp where Category='ServiceProviderType'", con);
            updSerProType.Items.Add("Type");
            SqlDataReader ds2 = cmd2.ExecuteReader();
            while (ds2.Read())
            {
                updSerProType.Items.Add(ds2[0]);
            }
            ds2.Close();






            SqlCommand cmd1 = new SqlCommand("select RegNo from ServiceProviders", con);
            updSerProSearch.Items.Add("Search");
            SqlDataReader ds1 = cmd1.ExecuteReader();
            while (ds1.Read())
            {
                updSerProSearch.Items.Add(ds1[0]);
            }
            ds1.Close();
            updSerProSearch.SelectedIndex = 0;




            

        }
        private void bunifuButton11_Click(object sender, EventArgs e)
        {
            PopulateSerProUpdateCombo();   
            bunifuPages1.SetPage("updSerPro");
        }

        private void updSerProSearch_SelectedValueChanged(object sender, EventArgs e)
        {
            if (updSerProSearch.Text != "Search")
            {
                updSerProName.ReadOnly = false;
                updSerProType.Enabled = true;
                var con = Configuration.getInstance().getConnection();
                SqlCommand cmd1 = new SqlCommand("select RegNo,Name,Value,AccountNo from ServiceProviders,Account,LookUp where RegNo=@regNo and Account.HolderID=ServiceProviders.ID and LookUp.Category='ServiceProviderType' and LookUp.ID=ServiceProviders.Type", con);
                cmd1.Parameters.AddWithValue("@regNo", updSerProSearch.Text);
                SqlDataAdapter da = new SqlDataAdapter(cmd1);
                DataTable dt = new DataTable();
                da.Fill(dt);
                updSerProGrid.DataSource = dt;
                updSerProName.Text = updSerProGrid.Rows[0].Cells[1].FormattedValue.ToString();
                updSerProType.Text = updSerProGrid.Rows[0].Cells[2].FormattedValue.ToString();
                updSerProAccountNumber.Text = updSerProGrid.Rows[0].Cells[3].FormattedValue.ToString();
            }
            else
            {
                updSerProName.ReadOnly = true;
                updSerProType.Enabled = false;
                updSerProName.Text = "Registered Name";
                updSerProType.SelectedIndex = 0;
                updSerProAccountNumber.Text = "Account Nunmber";
            }
        }

        private void button11_Click(object sender, EventArgs e)
        {
            if (searchForTraderUpdateDelete.Text != "Search")
            {
                DialogResult dialogResult = MessageBox.Show("Are you sure to delete?", "Confirmation", MessageBoxButtons.YesNo);

                if (dialogResult == DialogResult.Yes)
                {
                    try
                    {
                        var con = Configuration.getInstance().getConnection();
                        SqlCommand cmd1 = new SqlCommand("delete from ServiceProviders where RegNo=@regNo", con);
                        cmd1.Parameters.AddWithValue("@regNo", updSerProGrid.Rows[0].Cells[0].FormattedValue.ToString());
                        cmd1.ExecuteNonQuery();
                        MessageBox.Show("Service Provider is deleted successfully");
                        updSerProName.ReadOnly = true;
                        updSerProType.Enabled = false;
                        updSerProName.Text = "Registered Name";
                        updSerProType.SelectedIndex = 0;
                        updSerProAccountNumber.Text = "Account Nunmber";
                        PopulateSerProUpdateCombo();
                    }
                    catch
                    {
                        MessageBox.Show("Sorry for the inconvience");
                    }
                }
            }
            else
            {

                MessageBox.Show("Please select the RegNo from the given list");
            }
        }

        private void button12_Click(object sender, EventArgs e)
        {
            if(updSerProSearch.Text!="Search")
            {
                if(updSerProName.Text!="")
                {
                    if(updSerProType.Text!="Type")
                    {
                        DialogResult dialogResult = MessageBox.Show("Are you sure to update?", "Confirmation", MessageBoxButtons.YesNo);

                        if (dialogResult == DialogResult.Yes)
                        {
                            try
                            {
                                var con = Configuration.getInstance().getConnection();


                                SqlCommand getType = new SqlCommand("Select ID from Lookup where Category='ServiceProviderType' AND Value=@value", con);
                                getType.Parameters.AddWithValue("@value", updSerProType.Text);
                                getType.ExecuteNonQuery();

                                SqlCommand cmd1 = new SqlCommand("UPDATE ServiceProviders SET Name=@Name,Type=@Type WHERE [RegNo]=@RegNo", con);
                                cmd1.Parameters.AddWithValue("@RegNo", updSerProSearch.Text);
                                cmd1.Parameters.AddWithValue("@Name",updSerProName.Text);
                                cmd1.Parameters.AddWithValue("@Type", int.Parse(getType.ExecuteScalar().ToString()));
                                cmd1.ExecuteNonQuery();


                                MessageBox.Show("Data is updated successfully");
                                InitializeUpdateTraderPage();
                                PopulateComboboxForTraderUpdation();
                            }
                            catch
                            {
                                MessageBox.Show("You are unable to perform this computation");
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("Please sekect the type");
                    }
                }
                else
                {
                    MessageBox.Show("Please enter the name");
                }
            }
            else
            {
                MessageBox.Show("Please select the REG NO from the given list");
            }
        }

        private void bunifuButton12_Click(object sender, EventArgs e)
        {
            bunifuPages1.SetPage("showSerPro");
            try
            {
                var con = Configuration.getInstance().getConnection();
                SqlCommand cmd = new SqlCommand("select RegNo,Name,Value as Type,AccountNo from ServiceProviders,Account,LookUp where   Account.HolderID=ServiceProviders.ID and LookUp.Category='ServiceProviderType' and LookUp.ID=ServiceProviders.Type", con);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                showSerProGrid.DataSource = dt;
            }
            catch
            {
                MessageBox.Show("Sorry for thr inconvience caused by system");
            }

        }
        //****Services section starts from here
        private void serDes_Leave(object sender, EventArgs e)
        {
            if(serDes.Text=="")
            {
                serDes.Text = "Description";
            }
        }

        private void serDes_Enter(object sender, EventArgs e)
        {
            serDes.Text = "";
        }

        private void serCost_Leave(object sender, EventArgs e)
        {
            if(serCost.Text=="")
            {
                serCost.Text = "Total Cost";
            }
        }

        private void serCost_Enter(object sender, EventArgs e)
        {
            serCost.Text = "";
        }

        private void serPaidAmount_Leave(object sender, EventArgs e)
        {
            if(serPaidAmount.Text=="")
            {
                serPaidAmount.Text = "Paid Amount";
            }
        }

        private void serPaidAmount_Enter(object sender, EventArgs e)
        {
            serPaidAmount.Text = "";
        }
        private void serProName_SelectedValueChanged(object sender, EventArgs e)
        {
            serProRegNo.Items.Clear();
            var con = Configuration.getInstance().getConnection();
            SqlCommand cmd2 = new SqlCommand("select RegNo from ServiceProviders where Name=@name", con);
            cmd2.Parameters.AddWithValue("@name", serProName.Text);
            SqlDataReader ds2 = cmd2.ExecuteReader();
            while (ds2.Read())
            {
                serProRegNo.Items.Add(ds2[0]);
            }
            ds2.Close();
            serProRegNo.SelectedIndex = 0;
        }
        public void InitializeServices()
        {
            serProName.Items.Clear();
            var con = Configuration.getInstance().getConnection();
            //SqlCommand cmd2 = new SqlCommand("select RegNo from ServiceProviders", con);
            SqlCommand cmd2 = new SqlCommand("select Name from ServiceProviders", con);
            SqlDataReader ds2 = cmd2.ExecuteReader();
            while (ds2.Read())
            {
                serProName.Items.Add(ds2[0]);
            }
            ds2.Close();
            serDes.Text = "Description";
            serCost.Text = "Total Cost";
            serPaidAmount.Text = "Paid Amount";
        }
        public bool ValidateServices()
        {
            if(serProRegNo.Text!="")
            {
                if(serDes.Text!="Description")
                {
                    if(float.Parse(serCost.Text)>0)
                    {
                        if (float.Parse(serPaidAmount.Text) > 0)
                        {
                            return true;
                        }
                        else
                        {
                            MessageBox.Show("Please check the paidamount");
                            return false;
                        }
                    }
                    else
                    {
                        MessageBox.Show("Please check the service cost");
                        return false;
                    }
                }
                else
                {
                    MessageBox.Show("Please enter the description");
                    return false;
                }
            }
            else
            {
                MessageBox.Show("Please select service provider");
                return false;
            }
        }
        private void gunaAdvenceButton3_Click_1(object sender, EventArgs e)
        {
            HideSubMenu();
            InitializeServices();
            bunifuPages1.SetPage("services");
        }
        public void AddService()
        {
            var con = Configuration.getInstance().getConnection();
            SqlCommand BeginTrans = new SqlCommand("Begin Transaction AddService", con);
            BeginTrans.ExecuteNonQuery();





            SqlCommand getAccountNo = new SqlCommand("select AccountNo from ServiceProviders,Account where ServiceProviders.ID=Account.HolderID and RegNo=@regNo", con);
            getAccountNo.Parameters.AddWithValue("@regNo", serProRegNo.Text);
            getAccountNo.ExecuteNonQuery();


            SqlCommand getAccountBalance = new SqlCommand("select Balance from Account where AccountNo=@account", con);
            getAccountBalance.Parameters.AddWithValue("@account", getAccountNo.ExecuteScalar().ToString());
            getAccountBalance.ExecuteNonQuery();



            SqlCommand cmd4 = new SqlCommand("EXECUTE UpdateAccount @account,@balance", con);
            cmd4.Parameters.AddWithValue("@account", getAccountNo.ExecuteScalar().ToString());
            cmd4.Parameters.AddWithValue("@balance", float.Parse(getAccountBalance.ExecuteScalar().ToString()) + float.Parse(serCost.Text) - float.Parse(serPaidAmount.Text));
            cmd4.ExecuteNonQuery();


            SqlCommand getCompanyAccountBalance = new SqlCommand("select Balance from Account where AccountNo='1111'", con);
            getCompanyAccountBalance.ExecuteNonQuery();

            SqlCommand cmd5 = new SqlCommand("EXECUTE UpdateAccount '1111',@balance", con);
            cmd5.Parameters.AddWithValue("@balance", float.Parse(getCompanyAccountBalance.ExecuteScalar().ToString()) - float.Parse(serPaidAmount.Text));
            cmd5.ExecuteNonQuery();


            SqlCommand cmd6 = new SqlCommand("Insert into Transactions values(@des,@date,@paidby,@receiver,@Amount)", con);
            cmd6.Parameters.AddWithValue("@des", "Amount paid against services provided by " + serProRegNo.Text);
            cmd6.Parameters.AddWithValue("@date", DateTime.Now);
            cmd6.Parameters.AddWithValue("@paidby", "1111");
            cmd6.Parameters.AddWithValue("@receiver", getAccountNo.ExecuteScalar().ToString());
            cmd6.Parameters.AddWithValue("Amount", float.Parse(serPaidAmount.Text));
            cmd6.ExecuteNonQuery();

            SqlCommand transID = new SqlCommand("SELECT MAX(ID) FROM Transactions", con);
            transID.ExecuteNonQuery();



            SqlCommand cmd7 = new SqlCommand("insert into ServiceProviderAccounts values(@transID,@serProRegNo,@des,@cost,@paid,@balance)", con);
            cmd7.Parameters.AddWithValue("@transID", int.Parse(transID.ExecuteScalar().ToString()));
            cmd7.Parameters.AddWithValue("@serProRegNo", serProRegNo.Text);
            cmd7.Parameters.AddWithValue("@des", serDes.Text);
            cmd7.Parameters.AddWithValue("@cost", float.Parse(serCost.Text));
            cmd7.Parameters.AddWithValue("@paid", float.Parse(serPaidAmount.Text));
            cmd7.Parameters.AddWithValue("@balance", float.Parse(getAccountBalance.ExecuteScalar().ToString()));
            cmd7.ExecuteNonQuery();


            SqlCommand EndTrans = new SqlCommand("Commit Transaction AddService", con);
            EndTrans.ExecuteNonQuery();
        }
        private void button13_Click(object sender, EventArgs e)
        {
            try
            {
                if(ValidateServices())
                {
                    try
                    {
                        AddService();
                        MessageBox.Show("Service is added successfully");
                        InitializeServices();
                    }
                    catch
                    {
                        var con = Configuration.getInstance().getConnection();
                        SqlCommand EndTrans = new SqlCommand("RollBack Transaction AddService", con);
                        EndTrans.ExecuteNonQuery();
                        MessageBox.Show("Sorry, You can not perform this transaction");
                    }
                }
            }
            catch
            {
                MessageBox.Show("Please check the amount fields");
            }
        }

        private void gunaAdvenceButton4_Click_1(object sender, EventArgs e)
        {
            showsubmenu(ordersSubmenu);
        }
        public void PopulateOrdersGrid()
        {
            ordersGrid.Columns.Clear();
            var con = Configuration.getInstance().getConnection();
            SqlCommand cmd = new SqlCommand("select Orders.ID,DistributerID,OrderDate,RequiredDate from Orders,LookUp where Category='OrderStatus' and LookUp.ID=Status and Value='Punched'", con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            ordersGrid.DataSource = dt;


            DataGridViewButtonColumn btn1 = new DataGridViewButtonColumn();
            btn1.Name = "Discard Order";
            btn1.Text = "Discard";
            ordersGrid.Columns.Add(btn1);
            btn1.UseColumnTextForButtonValue = true;

            DataGridViewButtonColumn btn2 = new DataGridViewButtonColumn();
            btn2.Name = "Deliver Order";
            btn2.Text = "Deliver";
            ordersGrid.Columns.Add(btn2);
            btn2.UseColumnTextForButtonValue = true;
            for (int x = 0; x < ordersGrid.RowCount; x++)
            {
                ordersGrid.Rows[x].Cells[4].Style.BackColor = System.Drawing.Color.LightSalmon;
                ordersGrid.Rows[x].Cells[5].Style.BackColor = System.Drawing.Color.LightGreen;
            }
        }

        private void bunifuButton13_Click(object sender, EventArgs e)
        {
            bunifuPages1.SetPage("punchedOrder");
            try
            {

                PopulateOrdersGrid();

            }
            catch
            {
                MessageBox.Show("Sorry for thr inconvience caused by system");
            }
        }

        private void ordersGrid_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 4)
            {
                DialogResult dialogResult = MessageBox.Show("Are you sure to discard the order?", "Confirmation", MessageBoxButtons.YesNo);
                if(dialogResult == DialogResult.Yes)
                {
                    try
                    {
                        var con = Configuration.getInstance().getConnection();
                        SqlCommand BeginTrans = new SqlCommand("Set Transaction Isolation Level Serializable Begin Transaction DiscardOrder", con);
                        BeginTrans.ExecuteNonQuery();

                        SqlCommand cmd1 = new SqlCommand("EXECUTE UpdateOrderStatus @id,@value", con);
                        cmd1.Parameters.AddWithValue("@id", int.Parse(ordersGrid.Rows[e.RowIndex].Cells[0].FormattedValue.ToString()));
                        cmd1.Parameters.AddWithValue("@value", "Discarded");
                        cmd1.ExecuteNonQuery();

                        SqlCommand cmd2 = new SqlCommand("select * from OrderDetails where OrderID=@id", con);
                        cmd2.Parameters.AddWithValue("@id", int.Parse(ordersGrid.Rows[e.RowIndex].Cells[0].FormattedValue.ToString()));
                        SqlDataAdapter da = new SqlDataAdapter(cmd2);
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        discardOrderDetailsGrid.DataSource = dt;

                        for (int x = 0; x < discardOrderDetailsGrid.RowCount; x++)
                        {
                            var con1 = Configuration.getInstance().getConnection();
                            SqlCommand cmd3 = new SqlCommand("EXECUTE UpdateProduct @id,@value", con1);
                            cmd3.Parameters.AddWithValue("@id", int.Parse(discardOrderDetailsGrid.Rows[x].Cells[1].FormattedValue.ToString()));
                            cmd3.Parameters.AddWithValue("@value", float.Parse(((float.Parse(discardOrderDetailsGrid.Rows[x].Cells[2].FormattedValue.ToString()) + float.Parse(discardOrderDetailsGrid.Rows[x].Cells[3].FormattedValue.ToString()) + float.Parse(discardOrderDetailsGrid.Rows[x].Cells[4].FormattedValue.ToString()) + float.Parse(discardOrderDetailsGrid.Rows[x].Cells[5].FormattedValue.ToString()) + float.Parse(discardOrderDetailsGrid.Rows[x].Cells[6].FormattedValue.ToString())) * 1).ToString()));
                            cmd3.ExecuteNonQuery();

                        }

                        SqlCommand EndTrans = new SqlCommand("Commit Transaction DisardOrder", con);
                        EndTrans.ExecuteNonQuery();
                    }
                    catch
                    {
                        var con = Configuration.getInstance().getConnection();
                        SqlCommand EndTrans = new SqlCommand("RollBack Transaction DiscardOrder", con);
                        EndTrans.ExecuteNonQuery();
                        MessageBox.Show("Sorry you are unable to discard this order");
                    }

                    PopulateOrdersGrid();

                }
            }
            if(e.ColumnIndex==5)
            {
                bunifuPages1.SetPage("deliverOrder");
                DelivDisRegNo.Text = ordersGrid.Rows[e.RowIndex].Cells[1].FormattedValue.ToString();
                DelivOrderDate.Text = ordersGrid.Rows[e.RowIndex].Cells[2].FormattedValue.ToString();
                DelivRequiredDate.Text = ordersGrid.Rows[e.RowIndex].Cells[3].FormattedValue.ToString();

                orderID = int.Parse(ordersGrid.Rows[e.RowIndex].Cells[0].FormattedValue.ToString());

                DelivCourComp.Items.Clear();
                var con = Configuration.getInstance().getConnection();
                SqlCommand cmd2 = new SqlCommand("select ServiceProviders.Name from ServiceProviders,LookUp where ServiceProviders.Type=LookUp.ID and LookUp.Category='ServiceProviderType' and Value='Courier Service'", con);
                DelivCourComp.Items.Add("Courier Company");
                SqlDataReader ds2 = cmd2.ExecuteReader();
                while (ds2.Read())
                {
                    DelivCourComp.Items.Add(ds2[0]);
                }
                ds2.Close();
                DelivCourComp.SelectedIndex = 0;


                SqlCommand cmd3 = new SqlCommand("select * from OrderDetails where OrderID=@id", con);
                cmd3.Parameters.AddWithValue("@id", int.Parse(ordersGrid.Rows[e.RowIndex].Cells[0].FormattedValue.ToString()));
                SqlDataAdapter da = new SqlDataAdapter(cmd3);
                DataTable dt = new DataTable();
                da.Fill(dt);
                delivOrderDetailGrid.DataSource = dt;


            }
        }
        public void PopulateDiscardedOrdersGrid()
        {
            discaededOrdersGrid.Columns.Clear();
            var con = Configuration.getInstance().getConnection();
            SqlCommand cmd = new SqlCommand("select Orders.ID,DistributerID,OrderDate,RequiredDate from Orders,LookUp where Category='OrderStatus' and LookUp.ID=Status and Value='Discarded'", con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            discaededOrdersGrid.DataSource = dt;


            DataGridViewButtonColumn btn1 = new DataGridViewButtonColumn();
            btn1.Name = "Restore Order";
            btn1.Text = "Restore";
            discaededOrdersGrid.Columns.Add(btn1);
            btn1.UseColumnTextForButtonValue = true;

            for (int x = 0; x < discaededOrdersGrid.RowCount; x++)
            {
                discaededOrdersGrid.Rows[x].Cells[4].Style.BackColor = System.Drawing.Color.LightGreen;
            }
        }
        private void bunifuButton14_Click(object sender, EventArgs e)
        {
            bunifuPages1.SetPage("discardedOrders");
            PopulateDiscardedOrdersGrid();
            

        }

        private void discaededOrdersGrid_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 4)
            {
                DialogResult dialogResult = MessageBox.Show("Are you sure to restore the order?", "Confirmation", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    try
                    {
                        var con = Configuration.getInstance().getConnection();
                        SqlCommand BeginTrans = new SqlCommand("Set Transaction Isolation Level Serializable Begin Transaction RestoreOrder", con);
                        BeginTrans.ExecuteNonQuery();

                        SqlCommand cmd1 = new SqlCommand("EXECUTE UpdateOrderStatus @id,@value", con);
                        cmd1.Parameters.AddWithValue("@id", int.Parse(discaededOrdersGrid.Rows[e.RowIndex].Cells[0].FormattedValue.ToString()));
                        cmd1.Parameters.AddWithValue("@value", "Punched");
                        cmd1.ExecuteNonQuery();

                        SqlCommand cmd2 = new SqlCommand("select * from OrderDetails where OrderID=@id", con);
                        cmd2.Parameters.AddWithValue("@id", int.Parse(discaededOrdersGrid.Rows[e.RowIndex].Cells[0].FormattedValue.ToString()));
                        SqlDataAdapter da = new SqlDataAdapter(cmd2);
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        discardOrderDetailsGrid.DataSource = dt;

                        for (int x = 0; x < discardOrderDetailsGrid.RowCount; x++)
                        {
                            var con1 = Configuration.getInstance().getConnection();
                            SqlCommand cmd3 = new SqlCommand("EXECUTE UpdateProduct @id,@value", con1);
                            cmd3.Parameters.AddWithValue("@id", int.Parse(discardOrderDetailsGrid.Rows[x].Cells[1].FormattedValue.ToString()));
                            cmd3.Parameters.AddWithValue("@value", float.Parse(((float.Parse(discardOrderDetailsGrid.Rows[x].Cells[2].FormattedValue.ToString()) + float.Parse(discardOrderDetailsGrid.Rows[x].Cells[3].FormattedValue.ToString()) + float.Parse(discardOrderDetailsGrid.Rows[x].Cells[4].FormattedValue.ToString()) + float.Parse(discardOrderDetailsGrid.Rows[x].Cells[5].FormattedValue.ToString()) + float.Parse(discardOrderDetailsGrid.Rows[x].Cells[6].FormattedValue.ToString())) * -1).ToString()));
                            cmd3.ExecuteNonQuery();

                        }

                        dt.Rows.Clear();
                        dt.Columns.Clear();
                        SqlCommand EndTrans = new SqlCommand("Commit Transaction RestoreOrder", con);
                        EndTrans.ExecuteNonQuery();
                    }
                    catch
                    {
                        var con = Configuration.getInstance().getConnection();
                        SqlCommand EndTrans = new SqlCommand("RollBack Transaction RestoreOrder", con);
                        EndTrans.ExecuteNonQuery();
                        MessageBox.Show("Sorry you are unable to restore this order");
                    }

                PopulateDiscardedOrdersGrid();
                }
            }
        }

        private void button14_Click(object sender, EventArgs e)
        {
            try
            {
                if (DelivCourComp.Text != "Courier Company")
                {
                    if (float.Parse(DelivShipCharges.Text) > 0)
                    {
                        try
                        {
                            var con = Configuration.getInstance().getConnection();
                            SqlCommand BeginTrans = new SqlCommand("Set Transaction Isolation Level Serializable; Begin Transaction DeliverOrder", con);
                            BeginTrans.ExecuteNonQuery();

                            SqlCommand cmd1 = new SqlCommand("EXECUTE UpdateOrderStatus @id,@value", con);
                            cmd1.Parameters.AddWithValue("@id", orderID);
                            cmd1.Parameters.AddWithValue("@value", "Shipped");
                            cmd1.ExecuteNonQuery();


                            SqlCommand getCourierRegNo = new SqlCommand("select ServiceProviders.RegNo from ServiceProviders,LookUp where ServiceProviders.Type=LookUp.ID and LookUp.Category='ServiceProviderType' and Value='Courier Service' and ServiceProviders.Name=@name", con);
                            getCourierRegNo.Parameters.AddWithValue("@name", DelivCourComp.Text);
                            getCourierRegNo.ExecuteNonQuery();



                            SqlCommand cmd2 = new SqlCommand("insert into OrderDelivered values(@ID,@shipvia,@date,@expdate)", con);
                            cmd2.Parameters.AddWithValue("@ID", orderID);
                            cmd2.Parameters.AddWithValue("@shipvia", getCourierRegNo.ExecuteScalar().ToString());
                            cmd2.Parameters.AddWithValue("@date", DateTime.Now);
                            cmd2.Parameters.AddWithValue("@expdate", DelivExpectDate.Value);
                            cmd2.ExecuteNonQuery();


                            SqlCommand getCompanyAccountBalance = new SqlCommand("select Balance from Account where AccountNo='1111'", con);
                            getCompanyAccountBalance.ExecuteNonQuery();

                            SqlCommand cmd5 = new SqlCommand("EXECUTE UpdateAccount '1111',@balance", con);
                            cmd5.Parameters.AddWithValue("@balance", float.Parse(getCompanyAccountBalance.ExecuteScalar().ToString()) - float.Parse(DelivShipCharges.Text));
                            cmd5.ExecuteNonQuery();





                            SqlCommand getAccountNo = new SqlCommand("select AccountNo from ServiceProviders,Account where ServiceProviders.ID=Account.HolderID and RegNo=@regNo", con);
                            getAccountNo.Parameters.AddWithValue("@regNo", getCourierRegNo.ExecuteScalar().ToString());
                            getAccountNo.ExecuteNonQuery();

                            SqlCommand cmd6 = new SqlCommand("Insert into Transactions values(@des,@date,@paidby,@receiver,@Amount)", con);
                            cmd6.Parameters.AddWithValue("@des", "Amount paid against services provided by " + getCourierRegNo.ExecuteScalar().ToString());
                            cmd6.Parameters.AddWithValue("@date", DateTime.Now);
                            cmd6.Parameters.AddWithValue("@paidby", "1111");
                            cmd6.Parameters.AddWithValue("@receiver", getAccountNo.ExecuteScalar().ToString());
                            cmd6.Parameters.AddWithValue("Amount", float.Parse(DelivShipCharges.Text));
                            cmd6.ExecuteNonQuery();

                            SqlCommand transID = new SqlCommand("SELECT MAX(ID) FROM Transactions", con);
                            transID.ExecuteNonQuery();


                            SqlCommand getAccountBalance = new SqlCommand("select Balance from Account where AccountNo=@account", con);
                            getAccountBalance.Parameters.AddWithValue("@account", getAccountNo.ExecuteScalar().ToString());
                            getAccountBalance.ExecuteNonQuery();


                            SqlCommand cmd7 = new SqlCommand("insert into ServiceProviderAccounts values(@transID,@serProRegNo,@des,@cost,@paid,@balance)", con);
                            cmd7.Parameters.AddWithValue("@transID", int.Parse(transID.ExecuteScalar().ToString()));
                            cmd7.Parameters.AddWithValue("@serProRegNo", getCourierRegNo.ExecuteScalar().ToString());
                            cmd7.Parameters.AddWithValue("@des", "Amount paid in ordert to provide service against orderID" + orderID.ToString());
                            cmd7.Parameters.AddWithValue("@cost", float.Parse(DelivShipCharges.Text));
                            cmd7.Parameters.AddWithValue("@paid", float.Parse(DelivShipCharges.Text));
                            cmd7.Parameters.AddWithValue("@balance", float.Parse(getAccountBalance.ExecuteScalar().ToString()));
                            cmd7.ExecuteNonQuery();


                            SqlCommand getDisAccountNo = new SqlCommand("select AccountNumber from Distributors where DistributorRegNo=@regNo1", con);
                            getDisAccountNo.Parameters.AddWithValue("@regNo1", DelivDisRegNo.Text);
                            getDisAccountNo.ExecuteNonQuery();

                            float Bill = 0;

                            for (int x = 0; x < delivOrderDetailGrid.RowCount; x++)
                            {
                                var con1 = Configuration.getInstance().getConnection();
                                SqlCommand cmd3 = new SqlCommand("select PerLitrePrice from Products where ID=@id", con1);
                                cmd3.Parameters.AddWithValue("@id", int.Parse(delivOrderDetailGrid.Rows[x].Cells[1].FormattedValue.ToString()));
                                cmd3.ExecuteNonQuery();
                                Bill = Bill + ((float.Parse(delivOrderDetailGrid.Rows[x].Cells[2].FormattedValue.ToString()) + float.Parse(delivOrderDetailGrid.Rows[x].Cells[3].FormattedValue.ToString()) + float.Parse(delivOrderDetailGrid.Rows[x].Cells[4].FormattedValue.ToString()) + float.Parse(delivOrderDetailGrid.Rows[x].Cells[5].FormattedValue.ToString()) + float.Parse(delivOrderDetailGrid.Rows[x].Cells[6].FormattedValue.ToString())) * float.Parse(cmd3.ExecuteScalar().ToString()));
                            }


                            SqlCommand cmd8 = new SqlCommand("EXECUTE UpdateAccount @account,@balance", con);
                            cmd8.Parameters.AddWithValue("@account", getDisAccountNo.ExecuteScalar().ToString());
                            cmd8.Parameters.AddWithValue("@balance", Bill);
                            cmd8.ExecuteNonQuery();




                            SqlCommand EndTrans = new SqlCommand("Commit Transaction DeliverOrder", con);
                            EndTrans.ExecuteNonQuery();



                            MessageBox.Show("Order is delivered successfully");
                            DelivCourComp.SelectedIndex = 0;
                            
                            DelivShipCharges.Text = "";
                            bunifuButton13_Click(sender, e);
                        }
                        catch
                        {
                            var con = Configuration.getInstance().getConnection();
                            SqlCommand EndTrans = new SqlCommand("RollBack Transaction DeliverOrder", con);
                            EndTrans.ExecuteNonQuery();
                            MessageBox.Show("You are unable to deliver this order");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Please check shipped charges");
                    }
                }
                else
                {
                    MessageBox.Show("Please select the courier company");
                }
            }
            catch
            {
                MessageBox.Show("Please check Shipped charges");
            }
        }

        private void bunifuButton15_Click(object sender, EventArgs e)
        {
            bunifuPages1.SetPage("sentOrder");
            deliveredOrderedGrid.Columns.Clear();
            var con = Configuration.getInstance().getConnection();
            SqlCommand cmd = new SqlCommand("select DistributerID as DisRegNo,OrderID,OrderDate,RequiredDate,Name as ShipVia,ShippedTimeStamp as ShipTime,ExpectedDeliveredDate as ReachedDate from Orders,OrderDelivered,ServiceProviders where Orders.ID=OrderDelivered.OrderID and ServiceProviders.RegNo=OrderDelivered.ShipVia", con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            deliveredOrderedGrid.DataSource = dt;
        }

        private void gunaAdvenceButton6_Click_1(object sender, EventArgs e)
        {
            bunifuPages1.SetPage("accountDetails");
            accountDetailsGrid.Columns.Clear();
            var con = Configuration.getInstance().getConnection();
            SqlCommand cmd = new SqlCommand("select * from Transactions", con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            accountDetailsGrid.DataSource = dt;

            SqlCommand cmd1 = new SqlCommand("select sum(Amount) from Transactions", con);
            cmd1.ExecuteNonQuery();


            SqlCommand cmd2 = new SqlCommand("select Balance from Account where AccountNo='1111'", con);
            cmd2.ExecuteNonQuery();

            paid.Text = cmd1.ExecuteScalar().ToString();
            remaining.Text = cmd2.ExecuteScalar().ToString();



        }
        public void PopulatePaymentComboBox()
        {
            if (payPersonType.Text == "Traders")
            {
                payRegNo.Items.Clear();
                var con = Configuration.getInstance().getConnection();
                SqlCommand cmd2 = new SqlCommand("select TraderRegNo from Traders", con);
                cmd2.Parameters.AddWithValue("@Type", payPersonType.Text);
                SqlDataReader ds2 = cmd2.ExecuteReader();
                while (ds2.Read())
                {
                    payRegNo.Items.Add(ds2[0]);
                }
                ds2.Close();
                payRegNo.SelectedIndex = -1;
            }
            else
            {
                payRegNo.Items.Clear();
                var con = Configuration.getInstance().getConnection();
                SqlCommand cmd2 = new SqlCommand("select RegNo from ServiceProviders", con);
                cmd2.Parameters.AddWithValue("@Type", payPersonType.Text);
                SqlDataReader ds2 = cmd2.ExecuteReader();
                while (ds2.Read())
                {
                    payRegNo.Items.Add(ds2[0]);
                }
                ds2.Close();
                payRegNo.SelectedIndex = -1;

            }
            balance.Text = "";

        }
        private void gunaAdvenceButton5_Click_1(object sender, EventArgs e)
        {
            HideSubMenu();
            bunifuPages1.SetPage("payment");
            PopulatePaymentComboBox();
        }

        private void payPersonType_SelectedValueChanged(object sender, EventArgs e)
        {
            PopulatePaymentComboBox();
        }

        private void payRegNo_SelectedValueChanged(object sender, EventArgs e)
        {
            if(payPersonType.Text=="Traders")
            {
                var con = Configuration.getInstance().getConnection();
                SqlCommand getAccountBalance = new SqlCommand("select Balance from Account where AccountNo=(select AccountNumber from Traders where TraderRegNo=@regNo)", con);
                getAccountBalance.Parameters.AddWithValue("@regNo", payRegNo.Text);
                getAccountBalance.ExecuteNonQuery();
                balance.Text = getAccountBalance.ExecuteScalar().ToString();
            }
            else if(payPersonType.Text=="ServiceProviders")
            {
                var con = Configuration.getInstance().getConnection();
                SqlCommand getAccountNo = new SqlCommand("select AccountNo from ServiceProviders,Account where ServiceProviders.ID=Account.HolderID and RegNo=@regNo", con);
                getAccountNo.Parameters.AddWithValue("@regNo", payRegNo.Text);
                getAccountNo.ExecuteNonQuery();

                SqlCommand getAccountBalance = new SqlCommand("select Balance from Account where AccountNo=@account", con);
                getAccountBalance.Parameters.AddWithValue("@account", getAccountNo.ExecuteScalar().ToString());
                getAccountBalance.ExecuteNonQuery();
                balance.Text = getAccountBalance.ExecuteScalar().ToString();

            }
        }
        public void insertAmount()
        {
            if(payPersonType.Text=="Traders")
            {
                try
                {
                    var con = Configuration.getInstance().getConnection();

                    SqlCommand BeginTrans = new SqlCommand("Set Transaction Isolation Level Serializable Begin Transaction PayTrader", con);
                    BeginTrans.ExecuteNonQuery();

                    SqlCommand getAccountBalance = new SqlCommand("select Balance from Account where AccountNo=(select AccountNumber from Traders where TraderRegNo=@regNo)", con);
                    getAccountBalance.Parameters.AddWithValue("@regNo", payRegNo.Text);
                    getAccountBalance.ExecuteNonQuery();

                    SqlCommand getAccountNo = new SqlCommand("select AccountNumber from Traders where TraderRegNo=@regNo", con);
                    getAccountNo.Parameters.AddWithValue("@regNo", payRegNo.Text);
                    getAccountNo.ExecuteNonQuery();

                    SqlCommand cmd4 = new SqlCommand("EXECUTE UpdateAccount @account,@balance", con);
                    cmd4.Parameters.AddWithValue("@account", getAccountNo.ExecuteScalar().ToString());
                    cmd4.Parameters.AddWithValue("@balance", float.Parse(getAccountBalance.ExecuteScalar().ToString()) - float.Parse(payAmount.Text));
                    cmd4.ExecuteNonQuery();

                    SqlCommand getCompanyAccountBalance = new SqlCommand("select Balance from Account where AccountNo='1111'", con);
                    getCompanyAccountBalance.ExecuteNonQuery();

                    SqlCommand cmd5 = new SqlCommand("EXECUTE UpdateAccount '1111',@balance", con);
                    cmd5.Parameters.AddWithValue("@balance", float.Parse(getCompanyAccountBalance.ExecuteScalar().ToString()) - float.Parse(payAmount.Text));
                    cmd5.ExecuteNonQuery();




                    SqlCommand cmd6 = new SqlCommand("Insert into Transactions values(@des,@date,@paidby,@receiver,@Amount)", con);
                    cmd6.Parameters.AddWithValue("@des", "Amount paid against remaining balance");
                    cmd6.Parameters.AddWithValue("@date", DateTime.Now);
                    cmd6.Parameters.AddWithValue("@paidby", "1111");
                    cmd6.Parameters.AddWithValue("@receiver", getAccountNo.ExecuteScalar().ToString());
                    cmd6.Parameters.AddWithValue("Amount", float.Parse(payAmount.Text));
                    cmd6.ExecuteNonQuery();

                    SqlCommand transID = new SqlCommand("SELECT MAX(ID) FROM Transactions", con);
                    transID.ExecuteNonQuery();


                    SqlCommand cmd7 = new SqlCommand("insert into TraderAccounts values(@transID,@traderID,Null,@payable,@paid,@balance)", con);
                    cmd7.Parameters.AddWithValue("@transID", int.Parse(transID.ExecuteScalar().ToString()));
                    cmd7.Parameters.AddWithValue("@traderID", payRegNo.Text);
                    // cmd7.Parameters.AddWithValue("@batchID", "Null");
                    cmd7.Parameters.AddWithValue("@payable", float.Parse("0"));
                    cmd7.Parameters.AddWithValue("@paid", float.Parse(payAmount.Text));
                    cmd7.Parameters.AddWithValue("@balance", float.Parse(getAccountBalance.ExecuteScalar().ToString()));
                    cmd7.ExecuteNonQuery();


                    SqlCommand EndTrans = new SqlCommand("Commit Transaction PayTrader", con);
                    EndTrans.ExecuteNonQuery();
                    MessageBox.Show("Transacrtion is performed successfully");

                }
                catch
                {
                    var con = Configuration.getInstance().getConnection();
                    SqlCommand EndTrans1 = new SqlCommand("RollBack Transaction PayTrader", con);
                    EndTrans1.ExecuteNonQuery();

                    MessageBox.Show("Sorry you are unable to perform this transaction");
                }
            }
            else if(payPersonType.Text=="ServiceProviders")
            {
                try
                {
                    var con = Configuration.getInstance().getConnection();
                    SqlCommand BeginTrans = new SqlCommand("Set Transaction Isolation Level Serializable Begin Transaction DeliverOrder", con);
                    BeginTrans.ExecuteNonQuery();

                    SqlCommand getCompanyAccountBalance = new SqlCommand("select Balance from Account where AccountNo='1111'", con);
                    getCompanyAccountBalance.ExecuteNonQuery();

                    SqlCommand cmd5 = new SqlCommand("EXECUTE UpdateAccount '1111',@balance", con);
                    cmd5.Parameters.AddWithValue("@balance", float.Parse(getCompanyAccountBalance.ExecuteScalar().ToString()) - float.Parse(payAmount.Text));
                    cmd5.ExecuteNonQuery();

                    SqlCommand getAccountNo = new SqlCommand("select AccountNo from ServiceProviders,Account where ServiceProviders.ID=Account.HolderID and RegNo=@regNo", con);
                    getAccountNo.Parameters.AddWithValue("@regNo", payRegNo.Text);
                    getAccountNo.ExecuteNonQuery();

                    SqlCommand cmd6 = new SqlCommand("Insert into Transactions values(@des,@date,@paidby,@receiver,@Amount)", con);
                    cmd6.Parameters.AddWithValue("@des", "Amount paid against remaining balance");
                    cmd6.Parameters.AddWithValue("@date", DateTime.Now);
                    cmd6.Parameters.AddWithValue("@paidby", "1111");
                    cmd6.Parameters.AddWithValue("@receiver", getAccountNo.ExecuteScalar().ToString());
                    cmd6.Parameters.AddWithValue("Amount", float.Parse(payAmount.Text));
                    cmd6.ExecuteNonQuery();


                    SqlCommand getAccountBalance = new SqlCommand("select Balance from Account where AccountNo=@account", con);
                    getAccountBalance.Parameters.AddWithValue("@account", getAccountNo.ExecuteScalar().ToString());
                    getAccountBalance.ExecuteNonQuery();

                    SqlCommand cmd8 = new SqlCommand("EXECUTE UpdateAccount @account,@balance", con);
                    cmd8.Parameters.AddWithValue("@account", getAccountNo.ExecuteScalar().ToString());
                    cmd8.Parameters.AddWithValue("@balance", float.Parse(getAccountBalance.ExecuteScalar().ToString()) - float.Parse(payAmount.Text));
                    cmd8.ExecuteNonQuery();


                    SqlCommand transID = new SqlCommand("SELECT MAX(ID) FROM Transactions", con);
                    transID.ExecuteNonQuery();





                    SqlCommand cmd7 = new SqlCommand("insert into ServiceProviderAccounts values(@transID,@serProRegNo,@des,@cost,@paid,@balance)", con);
                    cmd7.Parameters.AddWithValue("@transID", int.Parse(transID.ExecuteScalar().ToString()));
                    cmd7.Parameters.AddWithValue("@serProRegNo", payRegNo.Text);
                    cmd7.Parameters.AddWithValue("@des", "Amount paid against remaining balance");
                    cmd7.Parameters.AddWithValue("@cost", float.Parse("0"));
                    cmd7.Parameters.AddWithValue("@paid", float.Parse(payAmount.Text));
                    cmd7.Parameters.AddWithValue("@balance", float.Parse(getAccountBalance.ExecuteScalar().ToString()));
                    cmd7.ExecuteNonQuery();

                    SqlCommand EndTrans = new SqlCommand("Commit Transaction DeliverOrder", con);
                    EndTrans.ExecuteNonQuery();
                    MessageBox.Show("Transaction is performed successfully");
                }
                catch
                {
                    var con = Configuration.getInstance().getConnection();
                    SqlCommand EndTrans = new SqlCommand("RollBack Transaction DeliverOrder", con);
                    EndTrans.ExecuteNonQuery();
                    MessageBox.Show("You are unable to oerform this transaction");
                }
            }
            else
            {
                balance.Text = "";
            }
        }
        private void button15_Click(object sender, EventArgs e)
        {
            try
            {
                if (payRegNo.Text != "")
                {
                    if (float.Parse(payAmount.Text) > 0)
                    {
                        insertAmount();
                    }
                    else
                    {
                        MessageBox.Show("Please check amount");
                    }
                }
                else
                {
                    MessageBox.Show("Please select the RegNo");
                }
            }
            catch
            {
                MessageBox.Show("Please check amount");
            }
        }

        private void submitTraderAccountNumber_TextChanged(object sender, EventArgs e)
        {

        }

        private void gunaAdvenceButton7_Click(object sender, EventArgs e)
        {
            HideSubMenu();
            this.Hide();
            LoginForm.instance().Show();

        }

        private void gunaAdvenceButton8_Click(object sender, EventArgs e)
        {
            HideSubMenu();
            bunifuPages1.SetPage("mainPage");
        }

        private void EmployeeManagementPageButtton_Click(object sender, EventArgs e)
        {
            Form frm = EmployeeManagement.instance();
            frm.Show();
            this.Hide();
        }

        private void closeApplication(object sender, FormClosedEventArgs e)
        {
            LoginForm.close();
        }

        private void UpdateCompanyAccountBalance_Click(object sender, EventArgs e)
        {
            if (CompanyUpdatedBalanceBox.Text != "" )
            {
                try {
                    float amount = float.Parse(CompanyUpdatedBalanceBox.Text);
                    var con = Configuration.getInstance().getConnection();
                    SqlCommand cmd = new SqlCommand("Update Account SET Balance += @Balance WHERE AccountNo = '1111'", con);
                    cmd.Parameters.AddWithValue("@Balance", amount);
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Updated Succesffully");
                    CompanyUpdatedBalanceBox.Text = "";
                }
                catch (Exception ex) {
                    MessageBox.Show("Enter a number","Invalid Fund amount");
                }
            }
        }
        private void gunaAdvenceButton9_Click(object sender, EventArgs e)
        {
            HideSubMenu();
            bunifuPages1.SetPage("Report1Page");
            AdminMainReportViewer.ReportSource = new AdminReport1();
        }

        private void Report2NavButton_Click(object sender, EventArgs e)
        {
            HideSubMenu();
            bunifuPages1.SetPage("Report1Page");
            AdminMainReportViewer.ReportSource = new AdminReport2();
        }
    }
}
