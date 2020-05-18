using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using DataObjects;
using LogicLayer;

namespace DQInventoryApp
{
    /// <summary>
    /// Interaction logic for frmSaleItemDetails.xaml
    /// </summary>
    public partial class frmSaleItemDetails : Window
    {
        private SaleItem _saleItem = null;
        private ISaleItemManager _saleItemManager = null;
        private IInventoryManager _inventoryManager = null;

        //Varible _unassignedItems and _saleItemInventory used as ItemsSource, so ListBox contents change
        List<String> _unassignedItems = new List<String>();
        List<InventoryQuantity> _saleItemInventory = new List<InventoryQuantity>();

        private bool _addMode = true;

        public frmSaleItemDetails()
        {
            InitializeComponent();

            _saleItemManager = new SaleItemManager();
            _inventoryManager = new InventoryManager();
        }

        //The window will be used for creating a SaleItem
        public frmSaleItemDetails(ISaleItemManager saleItemManager, IInventoryManager inventoryManager)
        {
            InitializeComponent();

            _saleItemManager = saleItemManager;
            _inventoryManager = inventoryManager;
        }

        //The window will be used for editting a SaleItem
        public frmSaleItemDetails(SaleItem saleItem, User user,
            ISaleItemManager saleItemManager, IInventoryManager inventoryManager)
        {
            InitializeComponent();

            _saleItem = saleItem;
            _saleItemManager = saleItemManager;
            _inventoryManager = inventoryManager;

            //User not allowed to Edit
            if (!(user.Roles.Contains("General Manager") || user.Roles.Contains("Administrator")))
            {
                btnEdit.IsEnabled = false;
            }

            _addMode = false;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            txtSaleItemID.IsReadOnly = true;

            //Adding all InventoryItem names to a List, as the ListBox ItemsSource
            foreach (InventoryItem item in _inventoryManager.GetAllInventoryItems())
            {
                _unassignedItems.Add(item.ItemName);
            }

            lstUnassignedItems.ItemsSource = _unassignedItems;
            dgSaleItemInventory.ItemsSource = _saleItemInventory;

            if (_addMode == false) //Viewing Details
            {
                //Set up fields for viewing details
                txtSaleItemID.Text = _saleItem.SaleItemID.ToString();
                txtItemName.Text = _saleItem.ItemName;
                txtItemSize.Text = _saleItem.ItemSize;
                txtFlavor.Text = _saleItem.Flavor;
                txtPrice.Text = _saleItem.Price.ToString();
                chkActive.IsChecked = _saleItem.Active;

                //Set current sale item inventory
                _saleItemInventory =
                    _inventoryManager.GetInventoryQuantitiesBySaleItem(_saleItem.SaleItemID);
                dgSaleItemInventory.ItemsSource = _saleItemInventory;

                //Disable changes and change to gray
                txtItemName.IsReadOnly = true;
                txtItemSize.IsReadOnly = true;
                txtFlavor.IsReadOnly = true;
                txtPrice.IsReadOnly = true;
                txtItemName.Background = Brushes.LightGray;
                txtItemSize.Background = Brushes.LightGray;
                txtFlavor.Background = Brushes.LightGray;
                txtPrice.Background = Brushes.LightGray;
                chkActive.IsEnabled = false;
                btnAddSelectedItem.IsEnabled = false;
                btnRemoveSelectedItem.IsEnabled = false;

                updateUnassignedItems();
            }
            else //Creating an Employee
            {
                //Set up fields for creating an employee
                txtSaleItemID.Text = "Auto Generated Value";
                txtItemName.Focus();
                chkActive.IsChecked = true;
                chkActive.IsEnabled = false;
                showSaveButton();
            }
        }

        private void showSaveButton()
        {
            btnSave.Visibility = Visibility.Visible;
            btnEdit.Visibility = Visibility.Hidden;
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            //Set up fields to Edit Sale Item
            txtItemName.IsReadOnly = false;
            txtItemSize.IsReadOnly = false;
            txtFlavor.IsReadOnly = false;
            txtPrice.IsReadOnly = false;
            txtItemName.Background = Brushes.White;
            txtItemSize.Background = Brushes.White;
            txtFlavor.Background = Brushes.White;
            txtPrice.Background = Brushes.White;
            chkActive.IsEnabled = true;
            btnAddSelectedItem.IsEnabled = true;
            btnRemoveSelectedItem.IsEnabled = true;

            txtItemName.Focus();
            showSaveButton();
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (validateInputs()) //True if inputs are valid
            {
                if (_saleItem == null) //Adding new Sale Item
                {
                    createNewSaleItem();
                }
                else //Updating existing Sale Item
                {
                    editSelectedSaleItem();
                }
            }
        }

        private void createNewSaleItem()
        {
            int saleItemID = 0; //ID Needed for adding Inventory Items to Sale Item

            SaleItem item = getSaleItemFromInput();

            try
            {
                saleItemID = _saleItemManager.AddSaleItem(item);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n\n" + ex.InnerException.Message,
                    "Sale Item Creation Failed", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            if (saleItemID == 0) //Item not created
            {
                MessageBox.Show("Item Could Not Be Created With Supplied Fields", "Sale Item Creation Failed",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                this.DialogResult = false;
            }
            else //Item created
            {
                //Set Sale Item's Inventory Items
                setSaleItemInventoryToSelected(saleItemID);

                MessageBox.Show("Sale Item Successfully Created", "Sale Item Created",
                    MessageBoxButton.OK, MessageBoxImage.Information);

                this.DialogResult = true;
            }
        }

        private void editSelectedSaleItem()
        {
            int saleItemID = _saleItem.SaleItemID; //ID Needed for adding Inventory Items to Sale Item

            bool itemUpdated = false;

            SaleItem newItem = getSaleItemFromInput();

            try
            {
                itemUpdated = _saleItemManager.EditSaleItem(_saleItem, newItem);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n\n" + ex.InnerException.Message,
                    "Sale Item Update Failed", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            if (itemUpdated == false) //Item not updated OR multiple items updated
            {
                MessageBox.Show("Item Could Not Be Updated With Supplied Fields", "Sale Item Update Failed",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                this.DialogResult = false;
            }
            else //Item updated
            {
                //Deletes all of selected Sale Item's Inventory Items (for next step)
                _saleItemManager.DeleteSaleItemFromSaleItemInventory(saleItemID);

                //Set Sale Item's Inventory Items
                setSaleItemInventoryToSelected(saleItemID);

                MessageBox.Show("Sale Item Successfully Updated", "Sale Item Updated",
                    MessageBoxButton.OK, MessageBoxImage.Information);

                this.DialogResult = true;
            }
        }

        private void setSaleItemInventoryToSelected(int saleItemID)
        {
            foreach (InventoryQuantity item in _saleItemInventory)
            {
                try
                {
                    _saleItemManager.AddSaleItemInventory(
                        saleItemID, item.InventoryItemName, item.Quantity);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message + "\n\n" + ex.InnerException.Message,
                        "Sale Item Inventory Could Not Be Set", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void updateUnassignedItems()
        {
            foreach (InventoryQuantity item in _saleItemInventory)
            {
                //This must start from the last index, so indexes don't change as they're removed
                for (int n = lstUnassignedItems.Items.Count - 1; n >= 0; --n)
                {
                    if (lstUnassignedItems.Items[n].ToString().Equals(item.InventoryItemName))
                    {
                        //Must remove from ItemsSource instead of the ListBox
                        _unassignedItems.RemoveAt(n);
                    }
                }
            }
        }

        private bool validateInputs()
        {
            bool inputsAreValid = true;
            string invalidInputMessage = "";

            //Checking for blank values
            if (string.IsNullOrWhiteSpace(txtItemName.Text) || string.IsNullOrWhiteSpace(txtPrice.Text))
            {
                invalidInputMessage += "Item Type and Price Cannot Be Left Blank! ";
            }

            //Number values can't contain spaces
            if (txtPrice.Text.ToString().Trim().Contains(" "))
            {
                if (invalidInputMessage.Equals(""))
                {
                    invalidInputMessage += "Price Cannot Contain Spaces!";
                }
                else
                {
                    invalidInputMessage += "\n\nPrice Cannot Contain Spaces!";
                }
            }

            //Try catch to ensure decimal value is in proper decimal format
            try
            {
                Convert.ToDecimal(txtPrice.Text.Trim());
            }
            catch (Exception ex)
            {
                if (invalidInputMessage.Equals(""))
                {
                    invalidInputMessage += "Price is Not Valid!";
                }
                else
                {
                    invalidInputMessage += "\n\nPrice is Not Valid!";
                }
            }

            //Any Inventory Items in the Sale Item must have a quantity greater than 0
            foreach (InventoryQuantity item in _saleItemInventory)
            {
                if (item.Quantity == 0)
                {
                    if (invalidInputMessage.Equals(""))
                    {
                        invalidInputMessage += "Cannot Have any Quantities of Zero!";
                    }
                    else
                    {
                        invalidInputMessage += "\n\nCannot Have any Quantities of Zero!";
                    }

                    break;
                }
            }
            //Determine if input was completely valid
            if (!invalidInputMessage.Equals(""))
            {
                MessageBox.Show(invalidInputMessage, "Invalid Input!",
                    MessageBoxButton.OK, MessageBoxImage.Exclamation);

                inputsAreValid = false;
            }

            return inputsAreValid;
        }

        private void DgSaleItemInventory_AutoGeneratedColumns(object sender, EventArgs e)
        {
            dgSaleItemInventory.Columns[0].Header = "Inventory Item Name";
            dgSaleItemInventory.Columns[1].Header = "Quantity (Sale Units)";
        }

        private void DgSaleItemInventory_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.PropertyName != "Quantity")
            {
                e.Column.IsReadOnly = true;
            }
        }

        private void DgSaleItemInventory_KeyDown(object sender, KeyEventArgs e)
        {
            string keyCode = e.Key.ToString();

            //If a digit is NOT pressed (keyCode contains "NumPad" or is 2 characters ending with a digit)
            if (!(keyCode.Contains("NumPad") || (keyCode.Length == 2 && char.IsDigit(keyCode[1]))))
            {
                if (e.Key == Key.Tab || keyCode.Equals("OemPeriod"))
                {
                    e.Handled = false; //Don't cancel tabs or decimals
                }
                else
                {
                    e.Handled = true; //Cancels the input
                }
            }
        }

        //PreviewKeyDown needed to disable Backspace and Delete
        private void DgSaleItemInventory_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (btnEdit.Visibility == Visibility.Visible) //Allow no changes while not in edit mode
            {
                e.Handled = true;
            }
        }

        private void BtnRemoveSelectedItem_Click(object sender, RoutedEventArgs e)
        {
            //Only if an item is selected, and the edit button is hidden (meaning it has been clicked)
            if (dgSaleItemInventory.SelectedItem != null && btnEdit.Visibility == Visibility.Hidden)
            {
                try
                {
                    dgSaleItemInventory.Items.Refresh(); //Goes to catch if DataGrid is being editted

                    _unassignedItems.Add(
                        ((InventoryQuantity)dgSaleItemInventory.SelectedItem).InventoryItemName);
                    _saleItemInventory.RemoveAt(dgSaleItemInventory.SelectedIndex);
                    lstUnassignedItems.Items.Refresh();
                    dgSaleItemInventory.Items.Refresh();
                }
                catch (Exception ex)
                {
                    //Doesn't attempt to save changes while editting
                    //DataGrid will automatically highlight the problem
                }
            }
        }

        private void BtnAddSelectedItem_Click(object sender, RoutedEventArgs e)
        {
            addInventoryItemToSaleItem();
        }

        private void LstUnassignedItems_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            addInventoryItemToSaleItem();
        }

        private void addInventoryItemToSaleItem()
        {
            //Only if an item is selected, and the edit button is hidden (meaning it has been clicked)
            if (lstUnassignedItems.SelectedItem != null && btnEdit.Visibility == Visibility.Hidden)
            {
                _saleItemInventory.Add(new InventoryQuantity
                {
                    InventoryItemName = lstUnassignedItems.SelectedItem.ToString()
                });
                _unassignedItems.RemoveAt(lstUnassignedItems.SelectedIndex);
                lstUnassignedItems.Items.Refresh();
                dgSaleItemInventory.Items.Refresh();
            }
        }

        private void TxtPrice_KeyDown(object sender, KeyEventArgs e)
        {
            string keyCode = e.Key.ToString();

            //If a digit is NOT pressed (keyCode contains "NumPad" or is 2 characters ending with a digit)
            if (!(keyCode.Contains("NumPad") || (keyCode.Length == 2 && char.IsDigit(keyCode[1]))))
            {
                if (keyCode.Equals("OemPeriod"))
                {
                    TextBox textBox = (TextBox)sender;
                    if (textBox.Text.Contains('.'))
                    {
                        e.Handled = true; //Cancels the input
                    }
                    else
                    {
                        txtPrice.SelectionStart = txtPrice.Text.Length;
                        txtPrice.SelectionLength = 0;
                        e.Handled = false; //Places decimal at the end
                    }
                }
                else if (e.Key == Key.Tab)
                {
                    e.Handled = false; //Don't cancel tabs
                }
                else
                {
                    e.Handled = true; //Cancels the input
                }
            }
            else
            {
                //At least 3 characters, contains a decimal, decimal is not in the last 3 characters
                if (txtPrice.Text.Length > 2 &&
                   txtPrice.Text.Contains(".") &&
                   !txtPrice.Text.Substring(txtPrice.Text.Length - 2, 2).Contains("."))
                {
                    e.Handled = true;
                }
            }
        }

        //PreviewKeyDown needed to disable Space
        private void TxtPrice_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                e.Handled = true; //Cancel spaces
            }
        }

        private SaleItem getSaleItemFromInput()
        {
            string itemName = txtItemName.Text.Trim();
            string itemSize = txtItemSize.Text.Trim();
            string flavor = txtFlavor.Text.Trim();
            decimal price = Convert.ToDecimal(txtPrice.Text.Trim());
            bool active = (bool)chkActive.IsChecked;

            SaleItem item = new SaleItem()
            {
                ItemName = itemName,
                ItemSize = itemSize,
                Flavor = flavor,
                Price = price,
                Active = active
            };

            return item;
        }
    }
}
