using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using DataObjects;
using LogicLayer;

namespace DQInventoryApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private IUserManager _userManager;
        private IRoleManager _roleManager;
        private IVendorManager _vendorManager;
        private IInventoryManager _inventoryManager;
        private ISaleItemManager _saleItemManager;
        private User _user = null;

        public MainWindow()
        {
            InitializeComponent();
            _userManager = new UserManager();
            _roleManager = new RoleManager();
            _vendorManager = new VendorManager();
            _inventoryManager = new InventoryManager();
            _saleItemManager = new SaleItemManager();
        }

        private void hideLoginFields()
        {
            txtEmail.Text = "";
            pwdPassword.Password = "";
            txtEmail.IsEnabled = false;
            pwdPassword.IsEnabled = false;
            btnLogin.Content = "Logout";
            lblPassword.Visibility = Visibility.Hidden;
            lblEmail.Visibility = Visibility.Hidden;
        }

        private void showLoginFields()
        {
            txtEmail.Text = "";
            pwdPassword.Password = "";
            txtEmail.IsEnabled = true;
            pwdPassword.IsEnabled = true;
            btnLogin.Content = "Login";
            lblPassword.Visibility = Visibility.Visible;
            lblEmail.Visibility = Visibility.Visible;
            txtEmail.Focus();
        }

        private void hideAllUserTabs()
        {
            foreach (TabItem item in tabsetMain.Items)
            {
                item.Visibility = Visibility.Collapsed;
            }
        }

        private void hideDeleteButtons()
        {
            btnDeleteVendor.Visibility = Visibility.Hidden;
            btnDeleteInventoryItem.Visibility = Visibility.Hidden;
            btnDeleteSaleItem.Visibility = Visibility.Hidden;
            btnDeleteEmployee.Visibility = Visibility.Hidden;
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            var email = txtEmail.Text;
            var password = pwdPassword.Password;

            if (btnLogin.Content.ToString() == "Logout")
            {
                forceUserLogout();
                return;
            }

            if (email.Length < 7 || password.Length < 7)
            {
                MessageBox.Show("Invalid Email or Password", "Invalid Login!",
                    MessageBoxButton.OK, MessageBoxImage.Exclamation);
                txtEmail.Text = "";
                pwdPassword.Password = "";
                txtEmail.Focus();

                return;
            }

            try
            {
                // goes to catch block if not authenticated
                _user = _userManager.AuthenticateUser(email, password);

                // get user's roles and put into a string
                string roles = "";
                for (int i = 0; i < _user.Roles.Count; i++)
                {
                    roles += _user.Roles[i];
                    if (i < _user.Roles.Count - 1)
                    {
                        roles += ", ";
                    }
                }

                lblStatusMessage.Content = "Hello, " + _user.FirstName
                    + " \nYou are logged in as: " + roles;

                if (pwdPassword.Password.ToString() == "newuser") // first time logging in
                {
                    // force a password reset
                    var resetPassword = new frmUpdatePassword(_user, _userManager);
                    if (resetPassword.ShowDialog() == true)
                    {
                        // password reset successful
                        hideLoginFields();
                        showUserTabsAndButtons();
                    }
                    else
                    {
                        // password reset failed
                        showLoginFields();
                        lblStatusMessage.Content = "Password reset failed! \nPlease login to try again.";
                        return;
                    }
                }
                else // not a first time login
                {
                    hideLoginFields();
                    showUserTabsAndButtons();
                }

                //Shows the Row Definition containing Data
                DataRow.Height = new GridLength(1, GridUnitType.Star);
                DummyRow.Height = new GridLength(0);

                btnAccountSettings.Visibility = Visibility.Visible;
            }
            catch (Exception ex) // not authenticated
            {
                MessageBox.Show(ex.Message + "\n\n" + ex.InnerException.Message,
                    "Login Failed", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnAccountSettings_Click(object sender, RoutedEventArgs e)
        {
            var accountWindow = new frmAccountSettings(_user, _userManager);
            if (accountWindow.ShowDialog() == false)
            {
                //Make the user re-login
                forceUserLogout();
                MessageBox.Show("Please Login Again.",
                    "Login Again", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            populateUserList();
        }

        private void showUserTabsAndButtons()
        {
            //Loop through the user roles
            foreach (var role in _user.Roles)
            {
                //Check for each roles
                switch (role)
                {
                    case "Employee":
                        //tabNewSales.Visibility = Visibility.Visible;
                        tabInventory.Visibility = Visibility.Visible;
                        tabSaleItems.Visibility = Visibility.Visible;
                        break;
                    case "Shift Manager":
                        //tabNewSales.Visibility = Visibility.Visible;
                        tabInventory.Visibility = Visibility.Visible;
                        //tabOrders.Visibility = Visibility.Visible;
                        tabVendors.Visibility = Visibility.Visible;
                        tabSaleItems.Visibility = Visibility.Visible;
                        break;
                    case "General Manager":
                        //tabNewSales.Visibility = Visibility.Visible;
                        tabInventory.Visibility = Visibility.Visible;
                        //tabOrders.Visibility = Visibility.Visible;
                        tabVendors.Visibility = Visibility.Visible;
                        //tabManageSales.Visibility = Visibility.Visible;
                        tabSaleItems.Visibility = Visibility.Visible;
                        break;
                    case "Administrator":
                        tabAdmin.Visibility = Visibility.Visible;
                        //tabNewSales.Visibility = Visibility.Visible;
                        tabInventory.Visibility = Visibility.Visible;
                        //tabOrders.Visibility = Visibility.Visible;
                        tabVendors.Visibility = Visibility.Visible;
                        //tabManageSales.Visibility = Visibility.Visible;
                        tabSaleItems.Visibility = Visibility.Visible;
                        break;
                }
            }
            if (tabAdmin.Visibility == Visibility.Visible)
            {
                tabAdmin.IsSelected = true;
                btnDeleteVendor.Visibility = Visibility.Visible;
                btnDeleteInventoryItem.Visibility = Visibility.Visible;
                btnDeleteSaleItem.Visibility = Visibility.Visible;
                btnDeleteEmployee.Visibility = Visibility.Visible;
                btnChangeInventoryActive.Visibility = Visibility.Visible;
                btnAddInventoryItem.Visibility = Visibility.Visible;
                btnViewInventory.Visibility = Visibility.Visible;
                btnChangeSaleItemActive.Visibility = Visibility.Visible;
                btnAddSaleItem.Visibility = Visibility.Visible;
                populateUserList();
            }
            else if (tabSaleItems.Visibility == Visibility.Visible)
            {
                //User is just a standard Employee
                if (_user.Roles.Count == 0 || (_user.Roles.Count == 1 && _user.Roles[0].Equals("Employee")))
                {
                    btnChangeInventoryActive.Visibility = Visibility.Hidden;
                    btnAddInventoryItem.Visibility = Visibility.Hidden;
                    btnViewInventory.Visibility = Visibility.Hidden;
                    btnChangeSaleItemActive.Visibility = Visibility.Hidden;
                    btnAddSaleItem.Visibility = Visibility.Hidden;

                    tabInventory.IsSelected = true;
                    populateInventoryList();
                }
                //User is General Manager
                else if (_user.Roles.Contains("General Manager"))
                {
                    btnChangeInventoryActive.Visibility = Visibility.Visible;
                    btnAddInventoryItem.Visibility = Visibility.Visible;
                    btnViewInventory.Visibility = Visibility.Visible;
                    btnChangeSaleItemActive.Visibility = Visibility.Visible;
                    btnAddSaleItem.Visibility = Visibility.Visible;

                    tabSaleItems.IsSelected = true;
                    populateSaleItemTypes();
                    populateSaleItemList();
                }
                else
                {
                    btnChangeInventoryActive.Visibility = Visibility.Visible;
                    btnAddInventoryItem.Visibility = Visibility.Visible;
                    btnViewInventory.Visibility = Visibility.Visible;
                    btnChangeSaleItemActive.Visibility = Visibility.Hidden;
                    btnAddSaleItem.Visibility = Visibility.Hidden;

                    tabInventory.IsSelected = true;
                    populateInventoryList();
                }
            }
            else if (tabInventory.Visibility == Visibility.Visible)
            {
                tabInventory.IsSelected = true;

                //User is just a standard Employee
                if(_user.Roles.Count == 0 || (_user.Roles.Count == 1 && _user.Roles[0].Equals("Employee")))
                {
                    btnChangeInventoryActive.Visibility = Visibility.Hidden;
                    btnAddInventoryItem.Visibility = Visibility.Hidden;
                    btnViewInventory.Visibility = Visibility.Hidden;
                }
                else
                {
                    btnChangeInventoryActive.Visibility = Visibility.Visible;
                    btnAddInventoryItem.Visibility = Visibility.Visible;
                    btnViewInventory.Visibility = Visibility.Visible;
                }
                populateInventoryList();
            }
            else
            {
                tabInventory.IsSelected = true;
                //tabNewSales.IsSelected = true;
            }
        }

        private void forceUserLogout()
        {
            _user = null;
            showLoginFields();
            lblStatusMessage.Content = "You are not logged in. Please login to continue.";
            hideAllUserTabs();
            hideDeleteButtons();

            //Hides the Row Definition containing Data
            DataRow.Height = new GridLength(0);
            DummyRow.Height = new GridLength(1, GridUnitType.Star);

            btnAccountSettings.Visibility = Visibility.Hidden;
        }

        private void showPopulatedEmployeeDetailWindow()
        {
            User user = (User)dgUserList.SelectedItem;

            if (user != null)
            {
                frmEmployeeDetails window = new frmEmployeeDetails(user, _user, _userManager);
                if (window.ShowDialog() == true)
                {
                    populateUserList();
                    lblStatusMessage.Content = "Changes to Employee: " + user.FirstName + " " + 
                        user.LastName + ", Saved!";
                }
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            hideAllUserTabs();
        }

        private void TabAdmin_GotFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dgUserList.ItemsSource == null)
                {
                    //Updates the DataGrid's view
                    populateUserList();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.Message);
            }
        }

        private void populateUserList()
        {
            if (btnShowDeactivatedEmployees.Background == Brushes.LightGreen)
            {
                dgUserList.ItemsSource = _userManager.GetUsersByActive(false);
            }
            else
            {
                dgUserList.ItemsSource = _userManager.GetUsersByActive(true);
            }
        }

        private void DgUserList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            showPopulatedEmployeeDetailWindow();
        }

        private void BtnViewEmployee_Click(object sender, RoutedEventArgs e)
        {
            showPopulatedEmployeeDetailWindow();
        }

        private void BtnAddEmployee_Click(object sender, RoutedEventArgs e)
        {
            frmEmployeeDetails window = new frmEmployeeDetails(_userManager);
            if (window.ShowDialog() == true)
            {
                populateUserList();
                lblStatusMessage.Content = "New Employee Record Saved!";
            }
        }

        private void BtnShowDeactivatedEmployees_Click(object sender, RoutedEventArgs e)
        {
            if (btnShowDeactivatedEmployees.Background != Brushes.LightGreen)
            {
                btnShowDeactivatedEmployees.Background = Brushes.LightGreen;
                btnChangeEmployeeActive.Content = "Reactivate Selected Employee";
            }
            else
            {
                btnShowDeactivatedEmployees.ClearValue(Button.BackgroundProperty);
                btnChangeEmployeeActive.Content = "Deactivate Selected Employee";
            }
            populateUserList();
        }

        private void showPopulatedVendorDetailWindow()
        {
            Vendor vendor = (Vendor)dgVendorList.SelectedItem;

            if (vendor != null)
            {
                frmVendorDetails window = new frmVendorDetails(vendor, _vendorManager);
                if (window.ShowDialog() == true)
                {
                    populateVendorList();
                    lblStatusMessage.Content = "Changes to Vendor: " + vendor.VendorName + ", Saved!";
                }
            }
        }

        private void TabVendors_GotFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dgVendorList.ItemsSource == null)
                {
                    populateVendorList();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.Message);
            }
        }

        private void BtnAddVendor_Click(object sender, RoutedEventArgs e)
        {
            frmVendorDetails window = new frmVendorDetails(_vendorManager);
            if (window.ShowDialog() == true)
            {
                populateVendorList();
                lblStatusMessage.Content = "New Vendor Record Saved!";
            }
        }

        private void BtnViewVendor_Click(object sender, RoutedEventArgs e)
        {
            showPopulatedVendorDetailWindow();
        }

        private void DgVendorList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            showPopulatedVendorDetailWindow();
        }

        private void populateVendorList()
        {
            if (btnShowDeactivatedVendors.Background == Brushes.LightGreen)
            {
                dgVendorList.ItemsSource = _vendorManager.GetVendorsByActive(false);
            }
            else
            {
                dgVendorList.ItemsSource = _vendorManager.GetVendorsByActive(true);
            }
        }

        private void TabInventory_GotFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dgInventoryList.ItemsSource == null)
                {
                    populateInventoryList();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.Message);
            }
        }

        private void showPopulatedInventoryDetailWindow()
        {
            InventoryItem item = (InventoryItem)dgInventoryList.SelectedItem;

            if (item != null)
            {
                frmInventoryDetails window = new frmInventoryDetails(item, _inventoryManager, _vendorManager);
                if (window.ShowDialog() == true)
                {
                    populateInventoryList();
                    lblStatusMessage.Content = "Changes to Inventory Item: " + item.ItemName + ", Saved!";
                }
            }
        }

        private void populateInventoryList()
        {
            bool active;
            bool lowQuantity;

            if (btnShowLowQuantityItems.Background == Brushes.LightGreen)
            {
                lowQuantity = true;
            }
            else
            {
                lowQuantity = false;
            }
            if (btnShowDeactivatedInventory.Background == Brushes.LightGreen)
            {
                active = false;
            }
            else
            {
                active = true;
            }
            dgInventoryList.ItemsSource = _inventoryManager.GetInventoryItemsByActiveAndQuantity(active, lowQuantity);
        }

        private void BtnAddInventoryItem_Click(object sender, RoutedEventArgs e)
        {
            frmInventoryDetails window = new frmInventoryDetails(_inventoryManager, _vendorManager);
            if (window.ShowDialog() == true)
            {
                populateInventoryList();
                lblStatusMessage.Content = "New Inventory Item Saved!";
            }
        }

        private void BtnShowLowQuantityItems_Click(object sender, RoutedEventArgs e)
        {
            if (btnShowLowQuantityItems.Background != Brushes.LightGreen)
            {
                btnShowLowQuantityItems.Background = Brushes.LightGreen;
            }
            else
            {
                btnShowLowQuantityItems.ClearValue(Button.BackgroundProperty);
            }
            populateInventoryList();
        }

        private void BtnViewInventory_Click(object sender, RoutedEventArgs e)
        {
            showPopulatedInventoryDetailWindow();
        }

        private void DgInventoryList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (btnViewInventory.Visibility == Visibility.Visible)
            {
                showPopulatedInventoryDetailWindow();
            }
        }

        private void BtnDeleteVendor_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Vendor vendor = (Vendor)dgVendorList.SelectedItem;
                if (vendor != null)
                {
                    if (MessageBox.Show("Are You Sure You Want To Delete Vendor:\n\n    " 
                        + vendor.VendorName +
                        "?\n\nTHIS ACTION CANNOT BE UNDONE.", "Delete Vendor?", MessageBoxButton.YesNo,
                        MessageBoxImage.Warning) == MessageBoxResult.No)
                    {
                        return; //Don't delete the Vendor
                    }
                    _vendorManager.RemoveVendor(vendor.VendorName); //Delete the Vendor

                    populateVendorList();

                    lblStatusMessage.Content = "Vendor: " + vendor.VendorName + ", Permanently Deleted!";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n\n" + ex.InnerException.Message);
            }
        }

        private void BtnDeleteInventoryItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                InventoryItem item = (InventoryItem)dgInventoryList.SelectedItem;
                if (item != null)
                {
                    if (MessageBox.Show("Are You Sure You Want To Delete Item:\n\n    "
                        + item.ItemName +
                        "?\n\nTHIS ACTION CANNOT BE UNDONE.", "Delete Inventory Item?", MessageBoxButton.YesNo,
                        MessageBoxImage.Warning) == MessageBoxResult.No)
                    {
                        return; //Don't delete the Item
                    }
                    _inventoryManager.RemoveInventoryItem(item.ItemName); //Delete the Item

                    populateInventoryList();

                    lblStatusMessage.Content = "Inventory Item: " + item.ItemName + ", Permanently Deleted!";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n\n" + ex.InnerException.Message);
            }
        }

        private void BtnDeleteSaleItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SaleItem item = (SaleItem)dgSaleItemList.SelectedItem;
                if (item != null)
                {
                    if (MessageBox.Show("Are You Sure You Want To Delete Item:\n\n    " +
                        item.ItemSize + " " + item.Flavor + " " + item.ItemName +
                        "?\n\nTHIS ACTION CANNOT BE UNDONE.", "Delete Sale Item?", MessageBoxButton.YesNo,
                        MessageBoxImage.Warning) == MessageBoxResult.No)
                    {
                        return; //Don't delete the Item
                    }
                    _saleItemManager.RemoveSaleItem(item.SaleItemID); //Delete the Item

                    populateSaleItemTypes();
                    populateSaleItemList();

                    lblStatusMessage.Content = "Sale Item: " + item.ItemSize + " " +
                        item.Flavor + " " + item.ItemName + ", Permanently Deleted!";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n\n" + ex.InnerException.Message);
            }
        }

        private void BtnDeleteEmployee_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                User user = (User)dgUserList.SelectedItem;
                if (user != null)
                {
                    //Don't allow user to delete self
                    if (!(user.EmployeeID == _user.EmployeeID))
                    {
                        if (MessageBox.Show("Are You Sure You Want To Delete Employee:\n\n    " +
                        user.FirstName + " " + user.LastName +
                        "?\n\nTHIS ACTION CANNOT BE UNDONE.", "Delete Employee?", MessageBoxButton.YesNo,
                        MessageBoxImage.Warning) == MessageBoxResult.No)
                        {
                            return; //Don't delete the Employee
                        }
                        _userManager.RemoveEmployee(user.EmployeeID); //Delete the Employee

                        populateUserList();

                        lblStatusMessage.Content = "Employee: " + user.FirstName + " " + 
                            user.LastName + ", Permanently Deleted!";
                    }
                    //Trying to delete self
                    else
                    {
                        MessageBox.Show("Cannot Delete Yourself, Choose a Different\nEmployee or Log in as Another Administrator!");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n\n" + ex.InnerException.Message);
            }
        }

        private void TabSaleItems_GotFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dgSaleItemList.ItemsSource == null)
                {
                    populateSaleItemTypes();
                    populateSaleItemList();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.Message);
            }
        }

        private void BtnViewSaleItem_Click(object sender, RoutedEventArgs e)
        {
            showPopulatedSaleItemDetailWindow();
        }

        private void DgSaleItemList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            showPopulatedSaleItemDetailWindow();
        }
        
        private void showPopulatedSaleItemDetailWindow()
        {
            SaleItem item = (SaleItem)dgSaleItemList.SelectedItem;

            if (item != null)
            {
                frmSaleItemDetails window = new frmSaleItemDetails(
                    item, _user, _saleItemManager, _inventoryManager);
                if (window.ShowDialog() == true)
                {
                    populateSaleItemTypes();
                    populateSaleItemList();
                    lblStatusMessage.Content = "Changes to Sale Item: " + item.ItemSize + " " +
                        item.Flavor + " " + item.ItemName + ", Saved!";
                }
            }
        }

        private void populateSaleItemList()
        {
            if (btnShowDeactivatedSaleItems.Background == Brushes.LightGreen)
            {
                if (cmbShowSaleItemsByType.SelectedItem.Equals("Show All"))
                {
                    dgSaleItemList.ItemsSource = _saleItemManager.GetSaleItemsByActive(false);
                }
                else
                {
                    dgSaleItemList.ItemsSource = 
                        _saleItemManager.GetSaleItemsByActiveAndName(false, cmbShowSaleItemsByType.Text);
                }
            }
            else
            {
                if (cmbShowSaleItemsByType.SelectedItem.Equals("Show All"))
                {
                    dgSaleItemList.ItemsSource = _saleItemManager.GetSaleItemsByActive(true);
                }
                else
                {
                    dgSaleItemList.ItemsSource =
                        _saleItemManager.GetSaleItemsByActiveAndName(true, cmbShowSaleItemsByType.Text);
                }
            }
        }

        private void populateSaleItemList(string itemType)
        {
            if (btnShowDeactivatedSaleItems.Background == Brushes.LightGreen)
            {
                if (cmbShowSaleItemsByType.SelectedItem.Equals("Show All"))
                {
                    dgSaleItemList.ItemsSource = _saleItemManager.GetSaleItemsByActive(false);
                }
                else
                {
                    dgSaleItemList.ItemsSource =
                        _saleItemManager.GetSaleItemsByActiveAndName(false, itemType);
                }
            }
            else
            {
                if (cmbShowSaleItemsByType.SelectedItem.Equals("Show All"))
                {
                    dgSaleItemList.ItemsSource = _saleItemManager.GetSaleItemsByActive(true);
                }
                else
                {
                    dgSaleItemList.ItemsSource =
                        _saleItemManager.GetSaleItemsByActiveAndName(true, itemType);
                }
            }
        }

        private void populateSaleItemTypes()
        {
            cmbShowSaleItemsByType.Items.Clear();

            if (cmbShowSaleItemsByType.Items.Count == 0)
            {
                cmbShowSaleItemsByType.Items.Add("Show All");
                foreach (string itemType in _saleItemManager.GetSaleItemNames())
                {

                    cmbShowSaleItemsByType.Items.Add(itemType);
                }
            }
            cmbShowSaleItemsByType.SelectedItem = "Show All";
        }

        private void BtnChangeEmployeeActive_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                User user = (User)dgUserList.SelectedItem;

                bool activeState;
                if (user != null)
                {
                    //Don't allow user to change own active status
                    if (!(user.EmployeeID == _user.EmployeeID))
                    {
                        if (btnChangeEmployeeActive.Content.Equals("Deactivate Selected Employee"))
                        {
                            if (MessageBox.Show("Are You Sure You Want To Deactivate Employee:\n\n    "
                                + user.FirstName + " " + user.LastName +
                                "?\n\nThis Action Is Not Permanent.", "Deactivate Employee?",
                                MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
                            {
                                return; //Don't deactivate employee
                            }
                            activeState = false; //Set active bool to deactivate
                        }
                        else
                        {
                            if (MessageBox.Show("Are You Sure You Want To Activate Employee:\n\n    "
                                + user.FirstName + " " + user.LastName +
                                "?\n\nThis Action Is Not Permanent.", "Activate Employee?",
                                MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
                            {
                                return; //Don't activate employee
                            }
                            activeState = true; //Set active bool to activate
                        }
                        _userManager.SetEmployeeActiveState(activeState, user.EmployeeID);
                        populateUserList();
                    }
                    //Trying to change own active status
                    else
                    {
                        MessageBox.Show("Cannot Change Your Own Active Status, Choose a Different \nEmployee or Log in as Another Administrator!");
                    }

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n\n" + ex.InnerException.Message);
            }
        }

        private void BtnChangeInventoryActive_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                InventoryItem item = (InventoryItem)dgInventoryList.SelectedItem;

                bool activeState;
                if (item != null)
                {
                    if (btnChangeInventoryActive.Content.Equals("Deactivate Selected Item"))
                    {
                        if (MessageBox.Show("Are You Sure You Want To Deactivate Inventory Item:\n\n    " 
                            + item.ItemName +
                            "?\n\nThis Action Is Not Permanent.", "Deactivate Inventory Item?",
                            MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
                        {
                            return; //Don't deactivate inventory item
                        }
                        activeState = false; //Set active bool to deactivate
                    }
                    else
                    {
                        if (MessageBox.Show("Are You Sure You Want To Activate Inventory Item:\n\n    " 
                            + item.ItemName +
                            "?\n\nThis Action Is Not Permanent.", "Activate Inventory Item?",
                            MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
                        {
                            return; //Don't activate inventory item
                        }
                        activeState = true; //Set active bool to activate
                    }
                    _inventoryManager.SetInventoryItemActiveState(activeState, item.ItemName);
                    populateInventoryList();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n\n" + ex.InnerException.Message);
            }
        }

        private void BtnShowDeactivatedInventory_Click(object sender, RoutedEventArgs e)
        {
            if (btnShowDeactivatedInventory.Background != Brushes.LightGreen)
            {
                btnShowDeactivatedInventory.Background = Brushes.LightGreen;
                btnChangeInventoryActive.Content = "Reactivate Selected Item";
            }
            else
            {
                btnShowDeactivatedInventory.ClearValue(Button.BackgroundProperty);
                btnChangeInventoryActive.Content = "Deactivate Selected Item";
            }
            populateInventoryList();
        }

        private void BtnChangeVendorActive_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Vendor vendor = (Vendor)dgVendorList.SelectedItem;

                bool activeState;
                if (vendor != null)
                {
                    if (btnChangeVendorActive.Content.Equals("Deactivate Selected Vendor"))
                    {
                        if (MessageBox.Show("Are You Sure You Want To Deactivate Vendor:\n\n    " 
                            + vendor.VendorName +
                            "?\n\nThis Action Is Not Permanent, However Vendor\nReferences Will Be Permanently Deleted.", "Deactivate Vendor?",
                            MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
                        {
                            return; //Don't deactivate vendor
                        }
                        activeState = false; //Set active bool to deactivate
                    }
                    else
                    {
                        if (MessageBox.Show("Are You Sure You Want To Activate Vendor:\n\n    " 
                            + vendor.VendorName +
                            "?\n\nThis Action Is Not Permanent.", "Activate Vendor?",
                            MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
                        {
                            return; //Don't activate vendor
                        }
                        activeState = true; //Set active bool to activate
                    }
                    _vendorManager.SetVendorActiveState(activeState, vendor.VendorName);
                    populateVendorList();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n\n" + ex.InnerException.Message);
            }
        }

        private void BtnShowDeactivatedVendors_Click(object sender, RoutedEventArgs e)
        {
            if (btnShowDeactivatedVendors.Background != Brushes.LightGreen)
            {
                btnShowDeactivatedVendors.Background = Brushes.LightGreen;
                btnChangeVendorActive.Content = "Reactivate Selected Vendor";
            }
            else
            {
                btnShowDeactivatedVendors.ClearValue(Button.BackgroundProperty);
                btnChangeVendorActive.Content = "Deactivate Selected Vendor";
            }
            populateVendorList();
        }

        private void BtnAddSaleItem_Click(object sender, RoutedEventArgs e)
        {
            frmSaleItemDetails window = new frmSaleItemDetails(_saleItemManager, _inventoryManager);
            if (window.ShowDialog() == true)
            {
                populateSaleItemTypes();
                populateSaleItemList();
                lblStatusMessage.Content = "New Sale Item Saved!";
            }
        }

        private void BtnChangeSaleItemActive_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SaleItem saleItem = (SaleItem)dgSaleItemList.SelectedItem;

                bool activeState;
                if (saleItem != null)
                {
                    if (btnChangeSaleItemActive.Content.Equals("Deactivate Selected Item"))
                    {
                        if (MessageBox.Show("Are You Sure You Want To Deactivate Sale Item:\n\n    " + 
                            saleItem.ItemSize + " " + saleItem.Flavor + " " + saleItem.ItemName +
                            "?\n\nThis Action Is Not Permanent.", "Deactivate Sale Item?",
                            MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
                        {
                            return; //Don't deactivate sale item
                        }
                        activeState = false; //Set active bool to deactivate
                    }
                    else
                    {
                        if (MessageBox.Show("Are You Sure You Want To Activate Sale Item:\n\n    " +
                            saleItem.ItemSize + " " + saleItem.Flavor + " " + saleItem.ItemName +
                            "?\n\nThis Action Is Not Permanent.", "Activate Sale Item?",
                            MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
                        {
                            return; //Don't activate sale item
                        }
                        activeState = true; //Set active bool to activate
                    }
                    _saleItemManager.SetSaleItemActiveState(activeState, saleItem.SaleItemID);
                    populateSaleItemList();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n\n" + ex.Message);
            }
        }

        private void CmbShowSaleItemsByType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                populateSaleItemList(e.AddedItems[0].ToString());
            }
            catch (Exception ex)
            {
                populateSaleItemTypes();
                populateSaleItemList();
            }
        }

        private void BtnShowDeactivatedSaleItems_Click(object sender, RoutedEventArgs e)
        {
            if (btnShowDeactivatedSaleItems.Background != Brushes.LightGreen)
            {
                btnShowDeactivatedSaleItems.Background = Brushes.LightGreen;
                btnChangeSaleItemActive.Content = "Reactivate Selected Item";
            }
            else
            {
                btnShowDeactivatedSaleItems.ClearValue(Button.BackgroundProperty);
                btnChangeSaleItemActive.Content = "Deactivate Selected Item";
            }
            populateSaleItemList();
        }

        private void DgInventoryList_AutoGeneratedColumns(object sender, EventArgs e)
        {
            dgInventoryList.Columns.RemoveAt(7);
            dgInventoryList.Columns.RemoveAt(6);
            dgInventoryList.Columns.RemoveAt(3);
            dgInventoryList.Columns.RemoveAt(1);
            dgInventoryList.Columns[0].Header = "Item Name";
            dgInventoryList.Columns[1].Header = "Sale Unit";
            dgInventoryList.Columns[2].Header = "Quantity on Hand (Sale Units)";
            dgInventoryList.Columns[3].Header = "Reorder Level";
        }

        private void DgVendorList_AutoGeneratedColumns(object sender, EventArgs e)
        {
            dgVendorList.Columns.RemoveAt(2);
            dgVendorList.Columns[0].Header = "Vendor Name";
            dgVendorList.Columns[1].Header = "Phone Number";
        }

        private void DgUserList_AutoGeneratedColumns(object sender, EventArgs e)
        {
            dgUserList.Columns.RemoveAt(6);
            dgUserList.Columns.RemoveAt(5);
            dgUserList.Columns[0].Header = "Employee ID";
            dgUserList.Columns[1].Header = "First Name";
            dgUserList.Columns[2].Header = "Last Name";
            dgUserList.Columns[3].Header = "Email Address";
            dgUserList.Columns[4].Header = "Phone Number";
        }

        private void DgSaleItemList_AutoGeneratedColumns(object sender, EventArgs e)
        {
            dgSaleItemList.Columns.RemoveAt(5);
            dgSaleItemList.Columns.RemoveAt(0);
            dgSaleItemList.Columns[0].Header = "Item Type";
            dgSaleItemList.Columns[1].Header = "Item Size";
            dgSaleItemList.Columns[2].Header = "Flavor";
            dgSaleItemList.Columns[3].Header = "Price";
        }
    }
}