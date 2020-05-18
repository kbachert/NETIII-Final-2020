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
    /// Interaction logic for frmInventoryDetails.xaml
    /// </summary>
    public partial class frmInventoryDetails : Window
    {
        private InventoryItem _inventoryItem = null;
        private IInventoryManager _inventoryManager = null;
        private IVendorManager _vendorManager = null;

        //Varible _unassignedVendors and _itemVendors used as ItemsSource, so ListBox contents can change
        List<String> _unassignedVendors = new List<String>();
        List<String> _itemVendors = new List<String>();

        private bool _addMode = true;
        private string noPreferenceText;

        public frmInventoryDetails()
        {
            InitializeComponent();

            _inventoryManager = new InventoryManager();
            _vendorManager = new VendorManager();
        }

        //The window will be used for creating an InventoryItem
        public frmInventoryDetails(IInventoryManager inventoryManager, IVendorManager vendorManager)
        {
            InitializeComponent();

            _inventoryManager = inventoryManager;
            _vendorManager = vendorManager;
        }

        //The window will be used for editting an InventoryItem
        public frmInventoryDetails(InventoryItem inventoryItem, IInventoryManager inventoryManager, IVendorManager vendorManager)
        {
            InitializeComponent();

            _inventoryItem = inventoryItem;
            _inventoryManager = inventoryManager;
            _vendorManager = vendorManager;

            _addMode = false;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _unassignedVendors = _vendorManager.GetActiveVendorNames();
            lstUnassignedVendors.ItemsSource = _unassignedVendors;
            lstItemVendors.ItemsSource = _itemVendors;

            noPreferenceText = "No Preference";

            //Populating ComboBox with all active Vendors
            cmbPreferredVendor.Items.Add(noPreferenceText);
            foreach (string vendorName in _vendorManager.GetActiveVendorNames())
            {
                cmbPreferredVendor.Items.Add(vendorName);
            }

            if (_addMode == false) //Viewing Details
            {
                //Set up fields for viewing details
                txtItemName.Text = _inventoryItem.ItemName;
                txtPurchaseUnit.Text = _inventoryItem.PurchaseUnit;
                txtSaleUnit.Text = _inventoryItem.SaleUnit;
                txtSaleUnitsPerPurchaseUnit.Text = _inventoryItem.SaleUnitsPerPurchaseUnit.ToString();
                txtQuantityOnHand.Text = _inventoryItem.QuantityOnHand.ToString();
                txtReorderLevel.Text = _inventoryItem.ReorderLevel.ToString();
                chkActive.IsChecked = _inventoryItem.Active;

                //Set current preferred vendor
                string currentPreferredVendor = _vendorManager.RetrievePreferredVendor(_inventoryItem.ItemName);
                if (currentPreferredVendor != "" && currentPreferredVendor != null) //Preferred Vendor found
                {
                    cmbPreferredVendor.SelectedItem = currentPreferredVendor;
                }
                else
                {
                    cmbPreferredVendor.SelectedItem = noPreferenceText;
                }

                //Disable changes and change to gray
                txtItemName.IsReadOnly = true;
                txtPurchaseUnit.IsReadOnly = true;
                txtSaleUnit.IsReadOnly = true;
                txtSaleUnitsPerPurchaseUnit.IsReadOnly = true;
                txtQuantityOnHand.IsReadOnly = true;
                txtReorderLevel.IsReadOnly = true;
                cmbPreferredVendor.IsEnabled = false;
                txtItemName.Background = Brushes.LightGray;
                txtPurchaseUnit.Background = Brushes.LightGray;
                txtSaleUnit.Background = Brushes.LightGray;
                txtSaleUnitsPerPurchaseUnit.Background = Brushes.LightGray;
                txtQuantityOnHand.Background = Brushes.LightGray;
                txtReorderLevel.Background = Brushes.LightGray;
                chkActive.IsEnabled = false;

                _itemVendors = _vendorManager.GetVendorsByInventoryID(_inventoryItem.ItemName);
                lstItemVendors.ItemsSource = _itemVendors; //Won't update display without this line again

                //Removes Item's vendors from UnassignedVendors
                updateUnassignedVendors();
            }
            else //Creating an Inventory Item
            {
                //Set up fields for creating an Inventory Item
                chkActive.IsChecked = true;
                chkActive.IsEnabled = false;
                txtItemName.Focus();
                cmbPreferredVendor.SelectedItem = noPreferenceText;

                showSaveButton();
            }
        }

        private void updateUnassignedVendors()
        {
            foreach (String vendor in _itemVendors)
            {
                //This must start from the last index, so indexes don't change as they're removed
                for (int n = lstUnassignedVendors.Items.Count - 1; n >= 0; --n)
                {
                    if (lstUnassignedVendors.Items[n].ToString().Equals(vendor))
                    {
                        //Must remove from ItemsSource instead of the ListBox
                        _unassignedVendors.RemoveAt(n);
                    }
                }
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
            //Set up fields to Edit Inventory Item
            txtItemName.IsReadOnly = false;
            txtPurchaseUnit.IsReadOnly = false;
            txtSaleUnit.IsReadOnly = false;
            txtSaleUnitsPerPurchaseUnit.IsReadOnly = false;
            txtQuantityOnHand.IsReadOnly = false;
            txtReorderLevel.IsReadOnly = false;
            cmbPreferredVendor.IsEnabled = true;
            txtItemName.Background = Brushes.White;
            txtPurchaseUnit.Background = Brushes.White;
            txtSaleUnit.Background = Brushes.White;
            txtSaleUnitsPerPurchaseUnit.Background = Brushes.White;
            txtQuantityOnHand.Background = Brushes.White;
            txtReorderLevel.Background = Brushes.White;
            chkActive.IsEnabled = true;

            txtItemName.Focus();
            showSaveButton();
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (validateInputs()) //True if inputs are valid
            {
                if (_inventoryItem == null) //Adding new Inventory Item
                {
                    createNewInventoryItem();
                }
                else //Updating existing Inventory Item
                {
                    editSelectedInventoryItem();
                }
            }
        }

        private void createNewInventoryItem()
        {
            string itemName = null; //Name Needed for setting preferred vendor

            InventoryItem item = getInventoryItemFromInput();

            try
            {
                itemName = _inventoryManager.AddInventoryItem(item);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n\n" + ex.InnerException.Message,
                    "Inventory Item Creation Failed", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            if (itemName == null) //Item not created
            {
                MessageBox.Show("Item Could Not Be Created With Supplied Fields", "Inventory Item Creation Failed",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                this.DialogResult = false;
            }
            else //Item created
            {
                //Sets preferred and selected vendors
                setVendorsToSelected(itemName);

                MessageBox.Show("Inventory Item Successfully Created", "Inventory Item Created",
                    MessageBoxButton.OK, MessageBoxImage.Information);

                this.DialogResult = true;
            }
        }

        private void editSelectedInventoryItem()
        {
            string itemName = txtItemName.Text; //Name Needed for setting preferred vendor

            bool itemUpdated = false;

            InventoryItem newItem = getInventoryItemFromInput();

            try
            {
                itemUpdated = _inventoryManager.EditInventoryItem(_inventoryItem, newItem);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n\n" + ex.InnerException.Message,
                    "Inventory Item Update Failed", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            if (itemUpdated == false) //Item not updated OR multiple items updated
            {
                MessageBox.Show("Item Could Not Be Created With Supplied Fields", "Inventory Item Creation Failed",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                this.DialogResult = false;
            }
            else //Item updated
            {
                //Deletes all of selected item's vendors (for next step)
                _inventoryManager.DeleteInventoryItemFromVendorItems(itemName);

                //Updates preferred and selected vendors
                setVendorsToSelected(itemName);

                MessageBox.Show("Inventory Item Successfully Updated", "Inventory Item Updated",
                    MessageBoxButton.OK, MessageBoxImage.Information);

                this.DialogResult = true;
            }
        }

        private InventoryItem getInventoryItemFromInput()
        {
            string itemName = txtItemName.Text.Trim();
            string purchaseUnit = txtPurchaseUnit.Text.Trim();
            string saleUnit = txtSaleUnit.Text.Trim();
            decimal saleUnitsPerPurchaseUnit = Convert.ToDecimal(txtSaleUnitsPerPurchaseUnit.Text.Trim());
            decimal quantityOnHand = Convert.ToDecimal(txtQuantityOnHand.Text.Trim());
            decimal reorderLevel = Convert.ToDecimal(txtReorderLevel.Text.Trim());
            bool active = (bool)chkActive.IsChecked;

            InventoryItem item = new InventoryItem()
            {
                ItemName = itemName,
                PurchaseUnit = purchaseUnit,
                SaleUnit = saleUnit,
                SaleUnitsPerPurchaseUnit = saleUnitsPerPurchaseUnit,
                QuantityOnHand = quantityOnHand,
                ReorderLevel = reorderLevel,
                Active = active
            };

            return item;
        }

        private void setVendorsToSelected(string itemName)
        {
            //Adds selected Vendors to the selected Item
            foreach (String vendor in lstItemVendors.ItemsSource)
            {
                try
                {
                    _inventoryManager.AddVendorItem(itemName, vendor);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message + "\n\n" + ex.InnerException.Message,
                    "Vendor " + vendor + " for Item, Could Not Be Added", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            //No need to set preferred vendor if no preference
            if (!(cmbPreferredVendor.SelectedItem.ToString() == noPreferenceText))
            {
                try
                {
                    _vendorManager.SetPreferredVendor(itemName, cmbPreferredVendor.SelectedItem.ToString());
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message + "\n\n" + ex.InnerException.Message,
                        "Preferred Vendor Could Not Be Set", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private bool validateInputs()
        {
            bool inputsAreValid = true;
            string invalidInputMessage = "";

            //No blank values
            if (string.IsNullOrWhiteSpace(txtItemName.Text) || string.IsNullOrWhiteSpace(txtPurchaseUnit.Text) ||
                string.IsNullOrWhiteSpace(txtSaleUnit.Text) || string.IsNullOrWhiteSpace(txtSaleUnitsPerPurchaseUnit.Text) ||
                string.IsNullOrWhiteSpace(txtQuantityOnHand.Text) || string.IsNullOrWhiteSpace(txtReorderLevel.Text))
            {
                invalidInputMessage += "No Values Can Be Left Blank! ";
            }
            //No spaces in numerical fields
            if (txtSaleUnitsPerPurchaseUnit.Text.ToString().Trim().Contains(" ") ||
                txtQuantityOnHand.Text.ToString().Trim().Contains(" ") || 
                txtReorderLevel.Text.ToString().Trim().Contains(" "))
            {
                if (invalidInputMessage.Equals(""))
                {
                    invalidInputMessage += "No Numerical Field Can Contain Spaces!";
                }
                else
                {
                    invalidInputMessage += "\n\nNo Numerical Field Can Contain Spaces!";
                }
            } 
            //Cannot end with a period
            try
            {
                if (txtSaleUnitsPerPurchaseUnit.Text.ToString()
                        [txtSaleUnitsPerPurchaseUnit.Text.ToString().Length - 1] == '.' ||
                    txtQuantityOnHand.Text.ToString()
                        [txtQuantityOnHand.Text.ToString().Length - 1] == '.' ||
                    txtReorderLevel.Text.ToString()
                        [txtReorderLevel.Text.ToString().Length - 1] == '.')
                {
                    if (invalidInputMessage.Equals(""))
                    {
                        invalidInputMessage += "No Numerical Field Can End With a Period (ex. 123.)!";
                    }
                    else
                    {
                        invalidInputMessage += "\n\nNo Numerical Field Can End With a Period (ex. 123.)!";
                    }
                }
            }
            catch (Exception)
            {
                //Can't check because the field is blank, already validated for blank
            }

            //3 try catches to validate that each decimal value is in proper decimal format
            try
            {
                Convert.ToDecimal(txtQuantityOnHand.Text.Trim());
            }
            catch (Exception ex)
            {
                if (invalidInputMessage.Equals(""))
                {
                    invalidInputMessage += "Quantity on Hand is Not Valid!";
                }
                else
                {
                    invalidInputMessage += "\n\nQuantity on Hand is Not Valid!";
                }
            }
            try
            {
                Convert.ToDecimal(txtSaleUnitsPerPurchaseUnit.Text.Trim());
            }
            catch (Exception ex)
            {
                if (invalidInputMessage.Equals(""))
                {
                    invalidInputMessage += "Sale Units Per Purchase Unit is Not Valid!";
                }
                else
                {
                    invalidInputMessage += "\n\nSale Units Per Purchase Unit is Not Valid!";
                }
            }
            try
            {
                Convert.ToDecimal(txtReorderLevel.Text.Trim());
            }
            catch (Exception ex)
            {
                if (invalidInputMessage.Equals(""))
                {
                    invalidInputMessage += "Reorder Level is Not Valid!";
                }
                else
                {
                    invalidInputMessage += "\n\nReorder Level is Not Valid!";
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

        private void TxtDecimalValues_KeyDown(object sender, KeyEventArgs e)
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
                        ((TextBox)sender).SelectionStart = ((TextBox)sender).Text.Length;
                        ((TextBox)sender).SelectionLength = 0;
                        e.Handled = false; //Places decimal at the end
                    }
                }
                else if (e.Key == Key.Tab){
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
                if (((TextBox)sender).Text.Length > 2 &&
                   ((TextBox)sender).Text.Contains(".") &&
                   !((TextBox)sender).Text.Substring(((TextBox)sender).Text.Length - 2, 2).Contains("."))
                {
                    e.Handled = true;
                }
            }
        }

        private void LstUnassignedVendors_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            //Only if an item is selected, and the edit button is hidden (meaning it has been clicked)
            if (lstUnassignedVendors.SelectedItem != null && btnEdit.Visibility == Visibility.Hidden)
            {
                _itemVendors.Add(lstUnassignedVendors.SelectedItem.ToString());
                _unassignedVendors.RemoveAt(lstUnassignedVendors.SelectedIndex);
                lstUnassignedVendors.Items.Refresh();
                lstItemVendors.Items.Refresh();
            }
        }

        private void LstItemVendors_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            //Only if an item is selected, and the edit button is hidden (meaning it has been clicked)
            if (lstItemVendors.SelectedItem != null && btnEdit.Visibility == Visibility.Hidden)
            {
                _unassignedVendors.Add(lstItemVendors.SelectedItem.ToString());
                _itemVendors.RemoveAt(lstItemVendors.SelectedIndex);
                lstUnassignedVendors.Items.Refresh();
                lstItemVendors.Items.Refresh();
            }
        }

        private void TxtDecimalValues_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                e.Handled = true; //Cancel spaces
            }
        }
    }
}
