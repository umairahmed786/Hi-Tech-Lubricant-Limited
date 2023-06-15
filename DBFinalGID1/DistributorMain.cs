using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Collections;
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
    public partial class DistributorMain : Form
    {
        List<DistributorCartItems> cartItems = new List<DistributorCartItems>();
        public string perLitrePrice = "0";
        public string availQntOfItemSelected = "0";
        public float totalOrderPrice = 0;


        static Dictionary<string, Image> picturesDetails = new Dictionary<string, Image>();
        public DistributorMain()
        {
            InitializeComponent();
            try
            {
                picturesDetails.Add("Motorbike Oil", global::DBFinalGID1.Properties.Resources.motorbike);
                picturesDetails.Add("Gasoline and Hybrid", global::DBFinalGID1.Properties.Resources.gasoline);
                picturesDetails.Add("Diesel Oil", global::DBFinalGID1.Properties.Resources.diesel);
                picturesDetails.Add("Gear Oil", global::DBFinalGID1.Properties.Resources.gearOil);
                picturesDetails.Add("Coolant", global::DBFinalGID1.Properties.Resources.coolant);
                picturesDetails.Add("Brake Fluid", global::DBFinalGID1.Properties.Resources.breakFluid);
                picturesDetails.Add("Delete Icon", global::DBFinalGID1.Properties.Resources.remove);

            }
            catch { }


        }

        private void ShutDown(object sender, FormClosedEventArgs e)
        {
            LoginForm.close();
        }

        private void gunaAdvenceButton1_Click(object sender, EventArgs e)
        {
            distributorPages4000.SetPage("buyProductspage");

        }

        private void gunaAdvenceButton2_Click(object sender, EventArgs e)
        {
            distributorPages4000.SetPage("tabPage3");
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
            distributorPages4000.SetPage("tabPage4");
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

        private void tabPage2_Click(object sender, EventArgs e)
        {

        }

        private void guna2PictureBox5_Click(object sender, EventArgs e)
        {

        }

        private void gunaLabel7_Click(object sender, EventArgs e)
        {

        }

        private void guna2TextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void gunaComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

            float quantityAvailableToBuy = float.Parse(availQntOfItemSelected);
            foreach (DistributorCartItems i in cartItems)
            {
                if (i.productName == currentProdName.Text)
                {
                    quantityAvailableToBuy = float.Parse((quantityAvailableToBuy - float.Parse(i.productQuantity) * float.Parse(i.productVolume)).ToString());
                }
            }

            var num = 0;
            if (selectQntCombo.SelectedIndex != 0)
            {

                if (selectQntCombo.SelectedIndex == 1)
                {
                    num = ((int)(quantityAvailableToBuy / 0.5));
                }
                else if (selectQntCombo.SelectedIndex == 2)
                {
                    num = ((int)(quantityAvailableToBuy) / 1);
                }
                else if (selectQntCombo.SelectedIndex == 3)
                {
                    num = ((int)(quantityAvailableToBuy) / 2);
                }
                else if (selectQntCombo.SelectedIndex == 4)
                {
                    num = ((int)(quantityAvailableToBuy) / 3);
                }
                else if (selectQntCombo.SelectedIndex == 5)
                {
                    num = ((int)(quantityAvailableToBuy) / 4);
                }

            }
            else
            {
                num = 0;
            }
            if (num < 10) { totalQntBtn.Maximum = num; }
            else totalQntBtn.Maximum = 10;
        }

        private void guna2TextBox3_TextChanged(object sender, EventArgs e)
        {
            selectQntCombo.Focus();
        }

        private void guna2TileButton1_Click(object sender, EventArgs e)
        {

        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void guna2TextBox1_TextChanged_1(object sender, EventArgs e)
        {

        }

        private void guna2TextBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void gunaLabel1_Click(object sender, EventArgs e)
        {

        }

        private void DistributorMain_Load(object sender, EventArgs e)
        {
            try
            {
                var con = Configuration.getInstance().getConnection();
                SqlCommand cmds = new SqlCommand("SET TRANSACTION ISOLATION LEVEL SERIALIZABLE; BEGIN TRAN gettingDistData", con);
                cmds.ExecuteNonQuery();

                SqlCommand cmd = new SqlCommand("SELECT TOP(1) [PhoneNumber] , Name , Value , UserNanme , Password , AccountNumber , Address FROM Distributors , LookUp WHERE DistributorRegNo = @disregNo AND Region = LookUp.ID", con);
                SqlCommand cmd1 = new SqlCommand("SELECT SUM(Balance) FROM DistributerAccounts JOIN Orders ON Orders.ID = DistributerAccounts.OrderID JOIN LookUp ON Orders.Status = LookUp.ID WHERE DistributerAccounts.Balance IN (SELECT MIN(Balance) FROM DistributerAccounts DE WHERE DistributerAccounts.OrderID = DE.OrderID) AND LookUp.Value <> 'Discarded' AND DistributerAccounts.DistributerRegNo = @distRegNo;", con);
                cmd1.Parameters.AddWithValue("@distregNo", LoginForm.distRegNoLoggedIn);
                loggedInDistributorBalance.Text = cmd1.ExecuteScalar().ToString();
                cmd.Parameters.AddWithValue("@disRegNo", LoginForm.distRegNoLoggedIn);
                SqlDataReader reader1;
                DataTable dataTable = new DataTable();
                reader1 = cmd.ExecuteReader();


                dataTable.Columns.Add("PhoneNumber");
                dataTable.Columns.Add("Name");
                dataTable.Columns.Add("Value");
                dataTable.Columns.Add("UserNanme");

                dataTable.Load(reader1);
                reader1.Close();

                SqlCommand cmde = new SqlCommand("COMMIT TRAN gettingDistData", con);
                cmde.ExecuteNonQuery();

                
                if (loggedInDistributorBalance.Text == "") { loggedInDistributorBalance.Text = "0"; }
                loggedInDistributorPhoneNumber.Text = dataTable.Rows[0][0].ToString();
                UpdateAccountPhoneNumber.Text = dataTable.Rows[0][0].ToString();

                loggedInDistributorName.Text = dataTable.Rows[0][1].ToString();
                UpdateAccountName.Text = dataTable.Rows[0][1].ToString();

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

                UpdateAccountRegion.ValueMember = "Value";
                UpdateAccountRegion.DataSource = dt;
                UpdateAccountRegion.SelectedValue = dataTable.Rows[0][2].ToString();

                loggedInDistributorRegion.Text = dataTable.Rows[0][2].ToString();
                UpdateAccountUsername.Text = dataTable.Rows[0][3].ToString();
                loggedInDistributorUsername.Text = dataTable.Rows[0][3].ToString();

                UpdateAccountAccountNumber.Text = dataTable.Rows[0][5].ToString();
                UpdateAccountAddress.Text = dataTable.Rows[0][6].ToString();
                UpdateAccountPassword.Text = dataTable.Rows[0][4].ToString();
                loggedInDistributorRegNo.Text = LoginForm.distRegNoLoggedIn;
                MainAdminScreen.instance().UpdateOrderStatus();
                /*Console.WriteLine(MotorBike.re);*/

            }
            catch (Exception ex) { MessageBox.Show(ex.ToString()); }
        }

        private void buyProductsPagebtn_Click(object sender, EventArgs e)
        {
            distributorPages4000.SetPage("buyProductspage");
        }
        private int calculateTotalProductsiinCart()
        {
            int totalQuantityOfProductsInCart = 0;
            foreach (DistributorCartItems i in cartItems)
            {
                totalQuantityOfProductsInCart = totalQuantityOfProductsInCart + int.Parse(i.productQuantity);

            }
            return totalQuantityOfProductsInCart;
        }
        private void showCartPageButton_Click(object sender, EventArgs e)
        {
            allItemsInCartInTable.Rows.Clear();
            totalOrderPrice = 0;
            RequiredOrderDateTime4000.Value = DateTime.Today;
            int totalQuantityOfProductsInCart = calculateTotalProductsiinCart();
            foreach (DistributorCartItems i in cartItems)
            {
                allItemsInCartInTable.Rows.Add(i.productImage, i.productName, i.productVolume, i.productPerLitrePrice, i.productQuantity, i.productTotalPrice, picturesDetails["Delete Icon"]);
                totalOrderPrice = totalOrderPrice + float.Parse(i.productTotalPrice);
            }
            if (totalQuantityOfProductsInCart > 10 )
            {alert5000.Visible = true; }
            else if (totalQuantityOfProductsInCart == 0) { alert5000.Visible = false; }
            else {alert5000.Visible = false; }
            if (RequiredOrderDateTime4000.Value == DateTime.Today) { dateAlert4000.Visible = true; }
            else { dateAlert4000.Visible = false; }
            distributorPages4000.SetPage("cartPage");
            if (alert5000.Visible == true || dateAlert4000.Visible == true || cartItems.Count == 0) { confirmOrderBtn4000.Enabled = false; }
            else { confirmOrderBtn4000.Enabled = true; }
            orderTotalAmount.Text = totalOrderPrice.ToString();
            
        }
        public class DataGridViewDisableButtonCell : DataGridViewButtonCell
        {
            private bool enabledValue;
            public bool Enabled
            {
                get
                {
                    return enabledValue;
                }
                set
                {
                    enabledValue = value;
                }
            }
            // By default, enable the button cell.
            public DataGridViewDisableButtonCell()
            {
                this.enabledValue = true;
            }
        }
        private void showHistoryofPurchasesDistributorPageButton_Click(object sender, EventArgs e)
        {
            confirmOrderBtn4000.Enabled = false;
            RequiredOrderDateTime4000.Value = DateTime.Today;
            var con = Configuration.getInstance().getConnection();
            SqlCommand cmds = new SqlCommand("SET TRANSACTION ISOLATION LEVEL SERIALIZABLE; BEGIN TRAN gettingDistributorsHistory", con);
            cmds.ExecuteNonQuery();

            SqlCommand cmd = new SqlCommand("SELECT * FROM (SELECT Orders.ID, Orders.OrderDate, CASE WHEN LookUp.Value = 'Delivered' THEN 'Delivered on ' + CAST(OrderDelivered.ShippedTimeStamp AS varchar(50)) WHEN LookUp.Value = 'Shipped' THEN 'Shipped on ' + CAST(OrderDelivered.ShippedTimeStamp AS varchar(50)) + ' with expected delivery date on ' + CAST(OrderDelivered.ExpectedDeliveredDate AS varchar(50)) WHEN LookUp.Value = 'Discarded' THEN 'Discarded' ELSE 'Punched' END AS Status, CASE WHEN OrderDelivered.ShippedTimeStamp IS NULL THEN CAST('-' AS varchar(50)) ELSE CAST(OrderDelivered.ShippedTimeStamp AS varchar(50)) END AS[Shipped Date], CASE WHEN OrderDelivered.ExpectedDeliveredDate IS NULL THEN CAST('-' AS varchar(50)) ELSE CAST(OrderDelivered.ExpectedDeliveredDate AS varchar(50)) END AS[Delivery Date], CASE WHEN Orders.Cost IS NULL THEN CAST('-' AS varchar(50)) ELSE CAST(Orders.Cost AS varchar(50)) END AS Cost, CASE WHEN DistributerAccounts.Balance IS NULL THEN(CASE WHEN LookUp.Value = 'Discarded' THEN '0' WHEN Orders.Cost IS NULL THEN CAST('-' AS varchar(50)) ELSE CAST(Orders.Cost AS varchar(50)) END) ELSE CAST(DistributerAccounts.Balance AS varchar(50)) END AS Balance FROM Orders LEFT JOIN OrderDelivered ON Orders.ID = OrderDelivered.OrderID AND Orders.DistributerID = @distRegNo LEFT JOIN DistributerAccounts ON Orders.ID = DistributerAccounts.OrderID JOIN LookUp ON Orders.Status = LookUp.ID) AS DE WHERE DE.Balance IN(SELECT MIN(Balance) FROM DistributerAccounts WHERE DistributerAccounts.OrderID = DE.ID)", con);
            cmd.Parameters.AddWithValue("@distRegNo", LoginForm.distRegNoLoggedIn);
            SqlDataReader reader1;
            DataTable dataTable = new DataTable();
            reader1 = cmd.ExecuteReader();

            dataTable.Columns.Add("ID");
            dataTable.Columns.Add("OrderDate");
            dataTable.Columns.Add("Status");
            dataTable.Columns.Add("[Shipped Date]");
            dataTable.Columns.Add("[Delivery Date]");
            dataTable.Columns.Add("Cost");
            dataTable.Columns.Add("Balance");
            dataTable.Load(reader1);

            historyDataDistributorGrid.AutoGenerateColumns = false;
            historyDataDistributorGrid.Columns[0].DataPropertyName = "ID";
            historyDataDistributorGrid.Columns[1].DataPropertyName = "OrderDate";
            historyDataDistributorGrid.Columns[2].DataPropertyName = "Status";
            historyDataDistributorGrid.Columns[3].DataPropertyName = "Cost";
            historyDataDistributorGrid.Columns[4].DataPropertyName = "Balance";
            historyDataDistributorGrid.DataSource = dataTable;

            MainAdminScreen.instance().UpdateOrderStatus();
            SqlCommand cmde = new SqlCommand("COMMIT TRAN gettingDistributorsHistory", con);
            cmde.ExecuteNonQuery();
            //
            //            payAmountGridButtonEnableDisable();

            distributorPages4000.SetPage("historyPage");


        }

        private void payAmountGridButtonEnableDisable()
        {
            for (int rows = 0; rows < historyDataDistributorGrid.Rows.Count; rows++)
            {
                for (int col = 0; col < historyDataDistributorGrid.Columns.Count; col++)
                {
                    if (historyDataDistributorGrid.Rows[rows].Cells[2].Value.ToString() == "Punched")
                    {
                        //DataGridViewButtonColumn(historyDataDistributorGrid.Rows[rows].Cells[5]);
                    }
                }

            }
        }

        private void distributorHomePageButton_Click(object sender, EventArgs e)
        {
            distributorPages4000.SetPage("distHomePage");
        }

        private void logoutDistributorButton_Click(object sender, EventArgs e)
        {
            if (cartItems.Count() == 0)
            {
                this.Hide();
                LoginForm.instance().Show();
                LoginForm.distRegNoLoggedIn = "";
            }
            else
            {
                DialogResult dialogResult = MessageBox.Show("You have items present in the cart. Logging out will empty your cart. Are you sure you want to Log out?", "Alert", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    this.Close();
                    LoginForm.instance().Show();
                    LoginForm.distRegNoLoggedIn = "";
                    cartItems.Clear();
                }
                else if (dialogResult == DialogResult.No)
                { }
            }

        }

        private void MotorBike_Click(object sender, EventArgs e)
        {
            renderProductImagesAndDataInPlaceOrderPage(motorbikeOilLable4000.Text);

        }

        private void motorbikeOilLable4000_Click(object sender, EventArgs e)
        {
            renderProductImagesAndDataInPlaceOrderPage(motorbikeOilLable4000.Text);
        }

        private void gasolineLabel4000_Click(object sender, EventArgs e)
        {
            renderProductImagesAndDataInPlaceOrderPage(gasolineLabel4000.Text);
        }

        private void dieselLabel4000_Click(object sender, EventArgs e)
        {
            renderProductImagesAndDataInPlaceOrderPage(dieselLabel4000.Text);
        }

        private void gearOillabel4000_Click(object sender, EventArgs e)
        {
            renderProductImagesAndDataInPlaceOrderPage(gearOillabel4000.Text);
        }

        private void coolantlabel4000_Click(object sender, EventArgs e)
        {
            renderProductImagesAndDataInPlaceOrderPage(coolantlabel4000.Text);
        }

        private void breakFluidLabel4000_Click(object sender, EventArgs e)
        {
            renderProductImagesAndDataInPlaceOrderPage(breakFluidLabel4000.Text);
        }
        public void renderProductImagesAndDataInPlaceOrderPage(string ProductName)
        {
            try
            {
                distributorPages4000.SetPage("placeOrderpage");
                foreach (KeyValuePair<string, Image> ele1 in picturesDetails)
                {
                    Console.WriteLine(ele1.Value);
                    if (ele1.Key == ProductName)
                    {
                        currentProductBeingPlacedImageBox.Image = ele1.Value;
                        currentProdName.Text = ProductName;
                    }
                }




            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            var con = Configuration.getInstance().getConnection();
            try
            {
                /*              SqlCommand cmds = new SqlCommand("BEGIN TRAN loadDataInOrderToBePlacedPage", con);
                                cmds.ExecuteNonQuery();*/
                SqlCommand cmd = new SqlCommand("SELECT Description , Quantity , PerLitrePrice FROM Products WHERE Name = @Name", con);
                cmd.Parameters.AddWithValue("@Name", ProductName);
                //SqlCommand cmde = new SqlCommand("COMMIT TRAN loadDataInOrderToBePlacedPage", con);
                SqlDataReader reader;
                DataTable dataTable = new DataTable();
                reader = cmd.ExecuteReader();
                //cmde.ExecuteNonQuery();
                dataTable.Columns.Add("Description");
                dataTable.Columns.Add("Quantity");
                dataTable.Columns.Add("PerLitrePrice");
                dataTable.Load(reader);
                reader.Close();
                availQntOfItemSelected = dataTable.Rows[0][1].ToString();
                productDescription4000.Text = dataTable.Rows[0][0].ToString();
                float quantityAvailableToBuy = float.Parse(availQntOfItemSelected);
                foreach (DistributorCartItems i in cartItems)
                {
                    if (i.productName == currentProdName.Text)
                    {
                        quantityAvailableToBuy = float.Parse((quantityAvailableToBuy - float.Parse(i.productQuantity) * float.Parse(i.productVolume)).ToString());
                    }
                }
                productAvailableQunatity4000.Text = (quantityAvailableToBuy).ToString() + " Lt";
                perLitrePrice = dataTable.Rows[0][2].ToString();
                currentProdName.Text = ProductName;

                selectQntCombo.Items.Clear();
                selectQntCombo.Items.Add("Choose Quantity..");
                selectQntCombo.Items.Add("0.5 Lt     " + (float.Parse(perLitrePrice) * 0.5).ToString());
                selectQntCombo.Items.Add("1   Lt     " + (float.Parse(perLitrePrice) * 1).ToString());
                selectQntCombo.Items.Add("2   Lt     " + (float.Parse(perLitrePrice) * 2).ToString());
                selectQntCombo.Items.Add("3   Lt     " + (float.Parse(perLitrePrice) * 3).ToString());
                selectQntCombo.Items.Add("4   Lt     " + (float.Parse(perLitrePrice) * 4).ToString());
                selectQntCombo.StartIndex = 0;

            }
            catch (Exception e) { MessageBox.Show(e.ToString()); }


        }

        private void totalQntBtn_ValueChanged(object sender, EventArgs e)
        {

        }

        private void tabPage14000_Click(object sender, EventArgs e)
        {

        }


        private void addToCartbtn4000_Click(object sender, EventArgs e)
        {
            if (totalQntBtn.Value != 0 && selectQntCombo.SelectedIndex != 0)
            {
                double volumeOfProduct = 0;
                if (selectQntCombo.SelectedIndex == 1) { volumeOfProduct = 0.5; }
                else { volumeOfProduct = selectQntCombo.SelectedIndex - 1; }
                DistributorCartItems newItem = new DistributorCartItems(picturesDetails[currentProdName.Text], currentProdName.Text, volumeOfProduct.ToString(), totalQntBtn.Value.ToString(), (double.Parse(perLitrePrice) * volumeOfProduct).ToString(), (double.Parse(perLitrePrice) * volumeOfProduct * int.Parse(totalQntBtn.Value.ToString())).ToString(), float.Parse((Convert.ToDouble(availQntOfItemSelected) / Convert.ToDouble(volumeOfProduct)).ToString()));
                float quantityAvailableToBuy = float.Parse(availQntOfItemSelected);
                foreach (DistributorCartItems i in cartItems)
                {
                    if (i.productName == currentProdName.Text)
                    {
                        quantityAvailableToBuy = float.Parse((quantityAvailableToBuy - int.Parse(i.productQuantity) * volumeOfProduct).ToString());
                    }
                }
                bool flag = false;
                foreach (DistributorCartItems i in cartItems)
                {
                    if (i.productName == currentProdName.Text && i.productVolume == volumeOfProduct.ToString() && int.Parse(totalQntBtn.Value.ToString()) * volumeOfProduct <= quantityAvailableToBuy)
                    {
                        i.productQuantity = (int.Parse(i.productQuantity) + (int.Parse(totalQntBtn.Value.ToString()))).ToString();
                        i.productTotalPrice = (double.Parse(perLitrePrice) * volumeOfProduct * float.Parse(i.productQuantity)).ToString();
                        flag = true;
                        distributorPages4000.SetPage("buyProductspage");
                        int totalQuantityOfProductsInCart = calculateTotalProductsiinCart();
                        showCartPageButton.Text = "Cart " + totalQuantityOfProductsInCart.ToString();
                        return;
                    }
                    else if (int.Parse(totalQntBtn.Value.ToString()) * volumeOfProduct >= quantityAvailableToBuy)
                    {
                        flag = true;
                        MessageBox.Show("Selected Quantity not available");
                    }
                }
                if (flag == false)
                {
                    cartItems.Add(newItem);
                    distributorPages4000.SetPage("buyProductspage");
                    int totalQuantityOfProductsInCart = calculateTotalProductsiinCart();
                    showCartPageButton.Text = "Cart " + totalQuantityOfProductsInCart.ToString();

                }


            }
            else
            {
                MessageBox.Show("Quantity cannot be zero");
            }
        }

        private void allItemsInCartInTable_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void tabPage2_Click_1(object sender, EventArgs e)
        {

        }

        private void updateOrderBtn4000_Click(object sender, EventArgs e)
        {

            for (int rows = 0; rows < allItemsInCartInTable.Rows.Count; rows++)
            {
                int value = int.Parse(allItemsInCartInTable.Rows[rows].Cells[4].Value.ToString());
                if (value != int.Parse(cartItems[rows].productQuantity) && value <= cartItems[rows].totalQuantityPresentInStock && value != 0)
                {
                    cartItems[rows].productQuantity = value.ToString();
                    cartItems[rows].productTotalPrice = (value * int.Parse(cartItems[rows].productPerLitrePrice)).ToString();
                }
                else if (value > cartItems[rows].totalQuantityPresentInStock)
                {
                    MessageBox.Show("Only " + cartItems[rows].totalQuantityPresentInStock.ToString() + " Lt " + cartItems[rows].productName + " is left");
                }
            }
            int totalQuantityOfProductsInCart = calculateTotalProductsiinCart();
            showCartPageButton.Text = "Cart " + totalQuantityOfProductsInCart.ToString();

            showCartPageButton_Click(sender, e);


        }

        private void allItemsInCartInTable_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == -1) return; //check if row index is not selected
            else if (allItemsInCartInTable.CurrentCell.ColumnIndex.Equals(6))

            {
                DialogResult dialogResult = MessageBox.Show("Delete Product?", "Alert", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    cartItems.RemoveAt(e.RowIndex);
                    int totalQuantityOfProductsInCart = calculateTotalProductsiinCart();
                    showCartPageButton.Text = "Cart " + totalQuantityOfProductsInCart.ToString();
                    showCartPageButton_Click(sender, e);
                }
                else if (dialogResult == DialogResult.No)
                { }

            }
        }

        private void updateAccountDetailofDistributorPageButton_Click(object sender, EventArgs e)
        {
            distributorPages4000.SetPage("updateAccountDetailsPage");
        }

        private void confirmOrderBtn4000_Click(object sender, EventArgs e)
        {
            try
            {
                if (cartItems.Count() != 0)
                {
                    var con = Configuration.getInstance().getConnection();
                    SqlCommand cmds = new SqlCommand("SET TRANSACTION ISOLATION LEVEL SERIALIZABLE; BEGIN TRAN PlaceOrderTransaction", con);
                    cmds.ExecuteNonQuery();
                    float totalCost = 0;
                    foreach (DistributorCartItems i in cartItems)
                    {
                        totalCost = totalCost + float.Parse(i.productTotalPrice);
                    }
                        // INserting a new Order in Order table
                    SqlCommand cmd1 = new SqlCommand("INSERT INTO Orders(DistributerID , OrderDate , RequiredDate , Status , Cost) VALUES(@DistributorID , GETDATE() , @RequiredDate , @Status , @Cost) ", con);
                    SqlCommand cmd2 = new SqlCommand("SELECT ID FROM LookUp WHERE Value = 'Punched'", con);
                    cmd1.Parameters.AddWithValue("@DistributorID", LoginForm.distRegNoLoggedIn);
                    cmd1.Parameters.AddWithValue("@Status", cmd2.ExecuteScalar());
                    cmd1.Parameters.AddWithValue("@Cost", totalCost);
                    cmd1.Parameters.AddWithValue("@RequiredDate", RequiredOrderDateTime4000.Value);
                    cmd1.ExecuteNonQuery();
                    float OrderID = 0;
                    // Inserting new order details in Order Details table
                    SqlCommand cmd3 = new SqlCommand("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;SELECT MAX(ID) FROM Orders", con);
                    OrderID = Convert.ToInt32(cmd3.ExecuteScalar());
                    ArrayList productIDs = new ArrayList();

                    // INSERTING Orders with products having zero quantity
                    foreach (DistributorCartItems i in cartItems)
                    {
                        SqlCommand cmd4 = new SqlCommand("SELECT ID FROM Products WHERE Name = @Name AND Quantity >= @Quantity", con);
                        cmd4.Parameters.AddWithValue("@Quantity", float.Parse(i.productQuantity) * float.Parse(i.productVolume));
                        cmd4.Parameters.AddWithValue("@Name", i.productName);
                        string currProdId = "";
                        try
                        {
                            currProdId = cmd4.ExecuteScalar().ToString();
                        }
                        catch
                        {
                        }
                        if (currProdId != "" && productIDs.Contains(currProdId) == false)
                        {
                            productIDs.Add(currProdId);
                            Console.WriteLine(OrderID + " " + currProdId);
                            SqlCommand cmd5 = new SqlCommand("INSERT INTO OrderDetails VALUES (@OrderID , @ProductID , 0 , 0 , 0 , 0 , 0)", con);
                            cmd5.Parameters.AddWithValue("@OrderID", OrderID);
                            cmd5.Parameters.AddWithValue("@ProductID", int.Parse(currProdId));
                            cmd5.ExecuteNonQuery();
                        }
                        /*else
                        {
                            MessageBox.Show(i.productQuantity + " items of " + i.productVolume + "L " + i.productName + " went out of stock.", "OOPS!");
                        }*/
                    }

                    // Inserting specific quantity against each volume
                    foreach (DistributorCartItems i in cartItems)
                    {
                        SqlCommand cmd4 = new SqlCommand("SELECT ID FROM Products WHERE Name = @Name", con);
                        //cmd4.Parameters.AddWithValue("@Quantity", int.Parse(i.productQuantity));
                        cmd4.Parameters.AddWithValue("@Name", i.productName);
                        string currProdId = cmd4.ExecuteScalar().ToString();
                        string tableName = "OrderDetails";
                        var command = "UPDATE " + tableName + " SET [";
                        command = command + i.productVolume + "L] = " + i.productQuantity;
                        command = command + " WHERE OrderID = " + OrderID + " AND ProductID = " + int.Parse(currProdId);
                        SqlCommand cmd = new SqlCommand(command, con);
                        cmd.ExecuteNonQuery();

                        SqlCommand cmd5 = new SqlCommand("UPDATE Products SET Quantity = @Quantity WHERE ID = @ID", con);
                        SqlCommand cmd6 = new SqlCommand("SELECT Quantity FROM Products WHERE ID = @ID", con);
                        cmd6.Parameters.AddWithValue("@ID", int.Parse(currProdId));
                        cmd5.Parameters.AddWithValue("@Quantity", float.Parse(cmd6.ExecuteScalar().ToString()) - float.Parse(i.productVolume) * float.Parse(i.productQuantity));
                        cmd5.Parameters.AddWithValue("@ID", int.Parse(currProdId));
                        cmd5.ExecuteNonQuery();

                    }

                    //Transactions being created with 0 amount
                    SqlCommand cmd7 = new SqlCommand(" INSERT INTO Transactions(Description , TransactionDate , PayableAccount , ReceiverAccount , Amount) VALUES (@Description , GETDATE() , @PayableAccount ,@ReceiverAccount , @Amount)", con);
                    cmd7.Parameters.AddWithValue("@Description", "Transaction generated for order#" + OrderID.ToString());
                    SqlCommand cmd8 = new SqlCommand("SELECT AccountNo FROM Account WHERE HolderID IS NULL", con);
                    SqlCommand cmd9 = new SqlCommand("SELECT AccountNumber FROM Distributors WHERE DistributorRegNo = @distRegNo", con);
                    cmd9.Parameters.AddWithValue("@distRegNo", LoginForm.distRegNoLoggedIn);
                    cmd7.Parameters.AddWithValue("@ReceiverAccount", cmd8.ExecuteScalar().ToString());
                    cmd7.Parameters.AddWithValue("@PayableAccount", cmd9.ExecuteScalar().ToString());
                    cmd7.Parameters.AddWithValue("@Amount", 0);
                    cmd7.ExecuteNonQuery();


                    SqlCommand cmd10 = new SqlCommand("INSERT INTO DistributerAccounts VALUES (@TransactionID , @OrderID , @DistributerRegNo ,@Paid , @Balance)", con);
                    SqlCommand cmd11 = new SqlCommand("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; SELECT MAX(ID) FROM Transactions", con);
                    SqlCommand cmd12 = new SqlCommand("SELECT Cost FROM Orders WHERE ID = @ID", con);
                    cmd12.Parameters.AddWithValue("@ID", OrderID);
                    cmd10.Parameters.AddWithValue("@TransactionID", int.Parse(cmd11.ExecuteScalar().ToString()));
                    cmd10.Parameters.AddWithValue("@OrderID", OrderID);
                    cmd10.Parameters.AddWithValue("@DistributerRegNo", LoginForm.distRegNoLoggedIn);
                    cmd10.Parameters.AddWithValue("@Balance", float.Parse(cmd12.ExecuteScalar().ToString()));
                    cmd10.Parameters.AddWithValue("@Paid", 0);
                    cmd10.ExecuteNonQuery();

                    SqlCommand cmd13 = new SqlCommand("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;SELECT SUM(Balance) FROM DistributerAccounts WHERE DistributerRegNo = @distRegNo", con);
                    cmd13.Parameters.AddWithValue("@distRegNo", LoginForm.distRegNoLoggedIn);
                    SqlCommand cmd14 = new SqlCommand("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;UPDATE Account SET Balance = @Balance WHERE Account.HolderID IN (SELECT ID FROM Distributors WHERE DistributorRegNo = @distRegNo)", con);
                    cmd14.Parameters.AddWithValue("@distRegNo", LoginForm.distRegNoLoggedIn);
                    cmd14.Parameters.AddWithValue("@Balance", float.Parse(cmd13.ExecuteScalar().ToString()));
                    cmd14.ExecuteNonQuery();

                    SqlCommand cmd15 = new SqlCommand("SELECT SUM(Balance) FROM DistributerAccounts JOIN Orders ON Orders.ID = DistributerAccounts.OrderID JOIN LookUp ON Orders.Status = LookUp.ID WHERE DistributerAccounts.Balance IN (SELECT MIN(Balance) FROM DistributerAccounts DE WHERE DistributerAccounts.OrderID = DE.OrderID) AND LookUp.Value <> 'Discarded' AND DistributerAccounts.DistributerRegNo = @distRegNo;", con);
                    cmd15.Parameters.AddWithValue("@distregNo", LoginForm.distRegNoLoggedIn);
                    loggedInDistributorBalance.Text = cmd15.ExecuteScalar().ToString();

                    SqlCommand cmde = new SqlCommand("COMMIT TRAN PlaceOrderTransaction", con);
                    cmde.ExecuteNonQuery();
                    cartItems.Clear();
                    RequiredOrderDateTime4000.Value = DateTime.Today;
                    showCartPageButton_Click(sender, e);
                    showCartPageButton.Text = "Cart 0";
                }
                else
                {
                    MessageBox.Show("Cart is empty");
                }
            }
            catch {
                var con = Configuration.getInstance().getConnection();
                SqlCommand cmde = new SqlCommand("ROLLBACK TRAN PlaceOrderTransaction", con);
                cmde.ExecuteNonQuery();
                MessageBox.Show("Sorry, we could not proceed your request");
            }

        }

        private void RequiredOrderDateTime4000_ValueChanged(object sender, EventArgs e)
        {
            //MessageBox.Show(RequiredOrderDateTime4000.Value.ToString("MM/dd/yyyy") + "  " + DateTime.Now.ToString("MM/dd/yyyy"));
            if (String.Compare(RequiredOrderDateTime4000.Value.ToString("MM/dd/yyyy"), DateTime.Now.ToString("MM/dd/yyyy")) == 0 || (String.Compare(RequiredOrderDateTime4000.Value.ToString("MM/dd/yyyy"), DateTime.Now.ToString("MM/dd/yyyy")) > 0))
            {
                dateAlert4000.Visible = false;
            }
            else { dateAlert4000.Visible = true;}
            if (alert5000.Visible == true || dateAlert4000.Visible == true || cartItems.Count == 0) { confirmOrderBtn4000.Enabled = false; }
            else { confirmOrderBtn4000.Enabled = true; }
        }

        private void backButtonToBuyProduct_Click(object sender, EventArgs e)
        {
            distributorPages4000.SetPage("buyProductspage");
        }
        public int balanceOfCurrentOrderToBePayed = 0;
        private void historyDataDistributorGrid_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex != -1)
            {
                if (historyDataDistributorGrid.CurrentCell.ColumnIndex.Equals(6) && e.RowIndex != -1)
                {
                    OrderDetailGrid4000.DataSource = null;
                    OrderDetailGrid4000.Rows.Clear();
                    var con = Configuration.getInstance().getConnection();
                    oderIDViewParticularOrder.Text = historyDataDistributorGrid.Rows[e.RowIndex].Cells[0].Value.ToString();
                    totalPriceOfParticularOrder.Text = historyDataDistributorGrid.Rows[e.RowIndex].Cells[3].Value.ToString();

                    SqlCommand cmd1 = new SqlCommand("  SELECT Name , PerLitrePrice ,[4L] ,[3L] ,[2L] ,[1L],[0.5L], (PerLitrePrice *[4L] + PerLitrePrice *[1L] + PerLitrePrice *[2L] + PerLitrePrice *[3L] + PerLitrePrice *[0.5L]) AS[Total Price] FROM [OrderDetails] JOIN Products ON Products.ID = ProductID WHERE OrderDetails.OrderID = @OI", con);
                    cmd1.Parameters.AddWithValue("@OI", int.Parse(oderIDViewParticularOrder.Text));

                    SqlDataReader reader1;
                    DataTable dataTable = new DataTable();
                    reader1 = cmd1.ExecuteReader();

                    dataTable.Columns.Add("Name");
                    dataTable.Columns.Add("PerLitrePrice");
                    dataTable.Columns.Add("[4L]");
                    dataTable.Columns.Add("[3L]");
                    dataTable.Columns.Add("[2L]");
                    dataTable.Columns.Add("[1L]");
                    dataTable.Columns.Add("[0.5L]");
                    dataTable.Columns.Add("[Total Price]");
                    dataTable.Load(reader1);

                    OrderDetailGrid4000.AutoGenerateColumns = false;
                    OrderDetailGrid4000.Columns[0].DataPropertyName = "Name";
                    OrderDetailGrid4000.Columns[1].DataPropertyName = "PerLitrePrice";
                    OrderDetailGrid4000.Columns[2].DataPropertyName = "4L";
                    OrderDetailGrid4000.Columns[3].DataPropertyName = "3L";
                    OrderDetailGrid4000.Columns[4].DataPropertyName = "2L";
                    OrderDetailGrid4000.Columns[5].DataPropertyName = "1L";
                    OrderDetailGrid4000.Columns[6].DataPropertyName = "0.5L";
                    OrderDetailGrid4000.Columns[7].DataPropertyName = "Total Price";
                    OrderDetailGrid4000.DataSource = dataTable;
                    distributorPages4000.SetPage("viewParticularOrder");
                }
                else if (e.RowIndex != -1 && e.ColumnIndex != 6 && (historyDataDistributorGrid.Rows[e.RowIndex].Cells[2].Value.ToString() == "Discarded" || historyDataDistributorGrid.Rows[e.RowIndex].Cells[4].Value.ToString() == "0" || historyDataDistributorGrid.Rows[e.RowIndex].Cells[4].Value.ToString() == "-") || float.Parse(historyDataDistributorGrid.Rows[e.RowIndex].Cells[4].Value.ToString()) <= 0)
                {
                    MessageBox.Show("Payment cannot be made");
                }
                else
                {
                    if (historyDataDistributorGrid.CurrentCell.ColumnIndex.Equals(5) && e.RowIndex != -1)
                    {
                        var con = Configuration.getInstance().getConnection();
                        orderIdDisplayTextBox4000.Text = historyDataDistributorGrid.Rows[e.RowIndex].Cells[0].Value.ToString();
                        orderBalanceDistributor4000.Text = historyDataDistributorGrid.Rows[e.RowIndex].Cells[4].Value.ToString();
                        SqlCommand cmd1 = new SqlCommand("SELECT AccountNumber FROM Distributors WHERE DistributorRegNo = @RegNo", con);
                        cmd1.Parameters.AddWithValue("@RegNo", LoginForm.distRegNoLoggedIn);
                        distributorAccountNumber.Text = cmd1.ExecuteScalar().ToString();
                        SqlCommand cmd2 = new SqlCommand("SELECT AccountNo FROM Account WHERE HolderID IS NULL", con);
                        receiverAccountNumberOrder.Text = cmd2.ExecuteScalar().ToString();
                        distributorPages4000.SetPage("makePaymentForOrder");
                    }
                }
            }
            
        }

        private void gunaPictureBox1_Click(object sender, EventArgs e)
        {
        }

        private void guna2TextBox5_TextChanged(object sender, EventArgs e)
        {

        }

        private void amountBeingPaidForOrder_TextChanged(object sender, EventArgs e)
        {

        }

        private void confirmPayment_Click(object sender, EventArgs e)
        {

            if ((System.Text.RegularExpressions.Regex.IsMatch(amountBeingPaidForOrder.Text, @"(^[0-9]{1,20}.{0,1}[0-9]{1,20}$)")))
            {
                if (float.Parse(amountBeingPaidForOrder.Text) <= float.Parse(orderBalanceDistributor4000.Text))
                {

                    var con = Configuration.getInstance().getConnection();
                    try
                    {
                        SqlCommand cmds = new SqlCommand("BEGIN TRAN PayOrderAmount", con);
                        cmds.ExecuteNonQuery();

                        SqlCommand cmd1 = new SqlCommand(" INSERT INTO Transactions(Description , TransactionDate , PayableAccount , ReceiverAccount , Amount) VALUES (@Description , GETDATE() , @PayableAccount ,@ReceiverAccount , @Amount)", con);
                        cmd1.Parameters.AddWithValue("@Description", "Ammount Paid to Account# " + receiverAccountNumberOrder.Text + " for Order# " + orderIdDisplayTextBox4000.Text + " by " + LoginForm.distRegNoLoggedIn);
                        cmd1.Parameters.AddWithValue("@ReceiverAccount", receiverAccountNumberOrder.Text);
                        cmd1.Parameters.AddWithValue("@PayableAccount", distributorAccountNumber.Text);
                        cmd1.Parameters.AddWithValue("@Amount", float.Parse(amountBeingPaidForOrder.Text));
                        cmd1.ExecuteNonQuery();

                        SqlCommand cmd2 = new SqlCommand("INSERT INTO DistributerAccounts VALUES (@TransactionID , @OrderID , @DistributerRegNo ,@Paid , @Balance)", con);
                        SqlCommand cmd3 = new SqlCommand("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; SELECT MAX(ID) FROM Transactions", con);
                        SqlCommand cmd4 = new SqlCommand("SELECT Cost FROM Orders WHERE ID = @ID", con);
                        SqlCommand cmd5 = new SqlCommand("UPDATE Account SET Balance += @Balance WHERE HolderID IS NULL", con);
                        cmd5.Parameters.AddWithValue("@Balance", float.Parse(amountBeingPaidForOrder.Text));
                        cmd4.Parameters.AddWithValue("@ID", int.Parse(orderIdDisplayTextBox4000.Text));
                        cmd2.Parameters.AddWithValue("@TransactionID", int.Parse(cmd3.ExecuteScalar().ToString()));
                        cmd2.Parameters.AddWithValue("@OrderID", int.Parse(orderIdDisplayTextBox4000.Text));
                        cmd2.Parameters.AddWithValue("@DistributerRegNo", LoginForm.distRegNoLoggedIn);
                        cmd2.Parameters.AddWithValue("@Balance", float.Parse(orderBalanceDistributor4000.Text) - float.Parse(amountBeingPaidForOrder.Text));
                        cmd2.Parameters.AddWithValue("@Paid", float.Parse(amountBeingPaidForOrder.Text));
                        cmd2.ExecuteNonQuery();
                        cmd5.ExecuteNonQuery();
                        SqlCommand cmdu = new SqlCommand("SELECT SUM(Balance) FROM DistributerAccounts JOIN Orders ON Orders.ID = DistributerAccounts.OrderID JOIN LookUp ON Orders.Status = LookUp.ID WHERE DistributerAccounts.Balance IN (SELECT MIN(Balance) FROM DistributerAccounts DE WHERE DistributerAccounts.OrderID = DE.OrderID) AND LookUp.Value <> 'Discarded' AND DistributerAccounts.DistributerRegNo = @distRegNo;", con);
                        cmdu.Parameters.AddWithValue("@distregNo", LoginForm.distRegNoLoggedIn);
                        loggedInDistributorBalance.Text = cmdu.ExecuteScalar().ToString();

                        SqlCommand cmde = new SqlCommand("COMMIT TRAN PayOrderAmount", con);
                        cmde.ExecuteNonQuery();
                        showHistoryofPurchasesDistributorPageButton_Click(sender, e);
                        amountBeingPaidForOrder.Text = "";
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.ToString());
                    }
                }
                else
                {
                    MessageBox.Show("You can not pay more than the remaining balance");
                }
            }
            else
            {
                MessageBox.Show("Amount should be containing only digits or it can contain a decimal point followd by some digits");
            }

        }

        private void goBackToAllOrders_Click(object sender, EventArgs e)
        {
            showHistoryofPurchasesDistributorPageButton_Click(sender, e);
        }

        private void UpdateAccountButton_Click(object sender, EventArgs e)
        {
            if ((Regex.IsMatch(UpdateAccountName.Text, @"(^[a-z A-Z]{3,50}$)")))
            {
                if ((Regex.IsMatch(UpdateAccountUsername.Text, @"(^[a-zA-Z-_0-9]{5,50}$)")))
                {
                    if ((Regex.IsMatch(UpdateAccountPassword.Text, @"(^[a-zA-Z-_0-9@!#$%^&*]{6,50}$)")))
                    {

                        if ((Regex.IsMatch(UpdateAccountPhoneNumber.Text, @"(^[0]{1}[0-9]{10}$)")))
                        {
                            if (UpdateAccountRegion.SelectedIndex != 0)
                            {
                                if ((Regex.IsMatch(UpdateAccountAddress.Text, @"(^[a-zA-Z -0-9]{1,50}$)")))
                                {
                                    if ((Regex.IsMatch(UpdateAccountAccountNumber.Text, @"(^[0-9]{16,20}$)")))
                                    {
                                        var con = Configuration.getInstance().getConnection();
                                        SqlCommand cmd1 = new SqlCommand("UPDATE Distributors SET UserNanme = @Username , Password = @Password , Name = @Name , Region = @Region , PhoneNumber = @PhoneNumber , Address = @Address WHERE DistributorRegNo = @distRegNo", con);
                                        cmd1.Parameters.AddWithValue("@PhoneNumber", UpdateAccountPhoneNumber.Text);
                                        cmd1.Parameters.AddWithValue("@Name", UpdateAccountName.Text);
                                        cmd1.Parameters.AddWithValue("@Username", UpdateAccountUsername.Text);
                                        cmd1.Parameters.AddWithValue("@Password", UpdateAccountPassword.Text);
                                        cmd1.Parameters.AddWithValue("@Address", UpdateAccountAddress.Text);
                                        cmd1.Parameters.AddWithValue("@AccountNumber", UpdateAccountAccountNumber.Text);
                                        SqlCommand cmd3 = new SqlCommand("EXEC sp_getSelectedRegionFromComboBox @Value = @Region", con);
                                        cmd3.Parameters.AddWithValue("@Region", UpdateAccountRegion.SelectedValue);
                                        cmd1.Parameters.AddWithValue("@Region", Convert.ToInt32(cmd3.ExecuteScalar()));
                                        cmd1.Parameters.AddWithValue("@distRegNo", LoginForm.distRegNoLoggedIn);
                                        cmd1.ExecuteScalar();
                                        MessageBox.Show("Updated Successfully");
                                        DistributorMain_Load(sender, e);
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

        private void goBackToHome_Click(object sender, EventArgs e)
        {
            distributorPages4000.SetPage("distHomePage");
        }

        private void historyDataDistributorGrid_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void gunaLabel340001_Click(object sender, EventArgs e)
        {

        }

        private void viewTransactionButton_Click(object sender, EventArgs e)
        {
            transactionGrid.DataSource = null;
            transactionGrid.Rows.Clear();
            var con = Configuration.getInstance().getConnection();
            tranctionOrderID.Text = oderIDViewParticularOrder.Text; 
            
            SqlCommand cmd1 = new SqlCommand("SELECT ID , Description , TransactionDate , ReceiverAccount , Paid , Balance FROM Transactions JOIN DistributerAccounts ON Transactions.ID = TransactionID AND OrderID = @OI", con);
            cmd1.Parameters.AddWithValue("@OI", int.Parse(oderIDViewParticularOrder.Text));

            SqlDataReader reader1;
            DataTable dataTable = new DataTable();
            reader1 = cmd1.ExecuteReader();

            dataTable.Columns.Add("ID");
            dataTable.Columns.Add("Description");
            dataTable.Columns.Add("TransactionDate");
            dataTable.Columns.Add("ReceiverAccount");
            dataTable.Columns.Add("Paid");
            dataTable.Columns.Add("Balance");
            dataTable.Load(reader1);
            reader1.Close();
            transactionGrid.AutoGenerateColumns = false;
            transactionGrid.Columns[0].DataPropertyName = "ID";
            transactionGrid.Columns[1].DataPropertyName = "Description";
            transactionGrid.Columns[2].DataPropertyName = "TransactionDate";
            transactionGrid.Columns[3].DataPropertyName = "ReceiverAccount";
            transactionGrid.Columns[4].DataPropertyName = "Paid";
            transactionGrid.Columns[5].DataPropertyName = "Balance";

            transactionGrid.DataSource = dataTable;
            distributorPages4000.SetPage("viewTransactionPage");
        }

        private void goBackToOrderDetails_Click(object sender, EventArgs e)
        {
            distributorPages4000.SetPage("viewParticularOrder");
        }

        private void goBackAllOrders_Click(object sender, EventArgs e)
        {
            distributorPages4000.SetPage("historyPage");
        }

        
        private void crystalReportViewer1_Load(object sender, EventArgs e)
        {

        }

        private void printOrderReportDist_Click(object sender, EventArgs e)
        {
            
            distributorPages4000.SetPage("distributorPerOrderReport");
           
        }

        private void OrderDetailPerOrderReport1_InitReport(object sender, EventArgs e)
        {

        }

        private void printAllOrders_Click(object sender, EventArgs e)
        {
            distributorPages4000.SetPage("allOrdersDistributorReport");
        }

        private void Gasoline_Click(object sender, EventArgs e)
        {
            renderProductImagesAndDataInPlaceOrderPage(gasolineLabel4000.Text);
        }

        private void Break_Click(object sender, EventArgs e)
        {
            renderProductImagesAndDataInPlaceOrderPage(gearOillabel4000.Text);
        }

        private void Coolant_Click(object sender, EventArgs e)
        {
            renderProductImagesAndDataInPlaceOrderPage(coolantlabel4000.Text);
        }

        private void BreakFluid_Click(object sender, EventArgs e)
        {
            renderProductImagesAndDataInPlaceOrderPage(breakFluidLabel4000.Text);
        }

        private void Diesel_Click(object sender, EventArgs e)
        {
            renderProductImagesAndDataInPlaceOrderPage(dieselLabel4000.Text);
        }
    }
}
