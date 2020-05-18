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
    /// Interaction logic for frmVendorDetails.xaml
    /// </summary>
    public partial class frmVendorDetails : Window
    {
        private Vendor _vendor = null;
        private IVendorManager _vendorManager = null;

        private bool _addMode = true;

        public frmVendorDetails(IVendorManager vendorManager) //The window will be used for creating a vendor
        {
            InitializeComponent();

            _vendorManager = vendorManager;
        }

        public frmVendorDetails(Vendor vendor, IVendorManager vendorManager) //The window will be used for editting a Vendor
        {
            InitializeComponent();

            _vendor = vendor;
            _vendorManager = vendorManager;
            _addMode = false;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (_addMode == false) //Viewing Details
            {
                //Set up fields for viewing details
                txtVendorName.Text = _vendor.VendorName;
                txtPhoneNumber.Text = _vendor.VendorPhone;
                chkActive.IsChecked = _vendor.Active;

                //Disable changes and change to gray
                txtVendorName.IsReadOnly = true;
                txtPhoneNumber.IsReadOnly = true;
                txtVendorName.Background = Brushes.LightGray;
                txtPhoneNumber.Background = Brushes.LightGray;
                chkActive.IsEnabled = false;
            }
            else //Creating a Vendor
            {
                //Set up fields for creating a vendor
                txtVendorName.Focus();
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
            //Set up fields to Edit Vendor
            txtVendorName.IsReadOnly = false;
            txtPhoneNumber.IsReadOnly = false;
            txtVendorName.Background = Brushes.White;
            txtPhoneNumber.Background = Brushes.White;
            chkActive.IsEnabled = true;
            txtVendorName.Focus();

            showSaveButton();
        }
        
        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (validateInputs()) //True if inputs are valid
            {
                if (_vendor == null) //Adding new Vendor
                {
                    createNewVendor();
                }
                else //Updating existing Vendor
                {
                    editSelectedVendor();
                }
            }
        }

        private void createNewVendor()
        {
            string vendorName = txtVendorName.Text.Trim();
            string phoneNumber = txtPhoneNumber.Text.Trim();
            bool vendorCreated = false;

            //Attempt to add the new Vendor
            try
            {
                vendorCreated = _vendorManager.CreateVendor(vendorName, phoneNumber);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n\n" + ex.InnerException.Message,
                    "Vendor Creation Failed", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            if (vendorCreated == false) //Vendor not created
            {
                MessageBox.Show("Vendor Could Not Be Created With Supplied Fields", "Vendor Creation Failed",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                this.DialogResult = false;
            }
            else
            {
                MessageBox.Show("Vendor Successfully Created", "Vendor Created",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                this.DialogResult = true;
            }
        }

        private void editSelectedVendor()
        {
            string newVendorName = txtVendorName.Text.Trim();
            string newVendorPhone = txtPhoneNumber.Text.Trim();
            bool newActiveStatus = (bool)chkActive.IsChecked;

            string oldVendorName = _vendor.VendorName;
            string oldVendorPhone = _vendor.VendorPhone;
            bool vendorUpdated = false;

            //Attempt to edit the vendor
            try
            {
                vendorUpdated = _vendorManager.UpdateVendor(newVendorName, newVendorPhone, 
                    newActiveStatus, oldVendorName, oldVendorPhone);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n\n" + ex.InnerException.Message,
                    "Vendor Update Failed", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            if (vendorUpdated == false) //Vendor not updated OR multiple updated
            {
                MessageBox.Show("Vendor Could Not Be Updated With Supplied Fields", "Vendor Update Failed",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                this.DialogResult = false;
            }
            else
            {
                MessageBox.Show("Vendor Successfully Updated", "Vendor Updated",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                this.DialogResult = true;
            }
        }

        private bool validateInputs()
        {
        bool inputsAreValid = true;
        string invalidInputMessage = "";

        //No blank values
        if (string.IsNullOrWhiteSpace(txtVendorName.Text) || string.IsNullOrWhiteSpace(txtPhoneNumber.Text))
        {
            invalidInputMessage += "No Values Can Be Left Blank!";
        }
        //Minimum length of 10 for phone number
        if (txtPhoneNumber.Text.Length < 10)
        {
            if (invalidInputMessage.Equals(""))
            {
                invalidInputMessage += "Phone Number Must Contain At Least 10 Characters!";
            }
            else
            {
                invalidInputMessage += "\n\nPhone Number Must Contain At Least 10 Characters!";
            }
        }
        //No spaces in phone number
        if (txtPhoneNumber.Text.ToString().Trim().Contains(" "))
        {
            if (invalidInputMessage.Equals(""))
            {
                invalidInputMessage += "Phone Number Must Contain Only Numbers!";
            }
            else
            {
                invalidInputMessage += "\n\nPhone Number Must Contain Only Numbers!";
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

        private void TxtPhoneNumber_KeyDown(object sender, KeyEventArgs e)
        {
            string keyCode = e.Key.ToString();

            //If a digit is NOT pressed (keyCode contains "NumPad" or is 2 characters ending with a digit)
            if (!(keyCode.Contains("NumPad") || (keyCode.Length == 2 && char.IsDigit(keyCode[1]))))
            {
                if (e.Key == Key.Tab)
                {
                    e.Handled = false; //Don't cancel tabs
                }
                else
                {
                    e.Handled = true; //Cancels the input
                }
            }
        }

        private void TxtPhoneNumber_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                e.Handled = true; //Cancel spaces
            }
        }
    }
}
