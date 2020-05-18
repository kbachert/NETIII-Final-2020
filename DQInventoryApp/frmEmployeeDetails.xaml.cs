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
    /// Interaction logic for frmEmployeeDetails.xaml
    /// </summary>
    public partial class frmEmployeeDetails : Window
    {
        private User _user = null;
        private User _currentUser = null;
        private IUserManager _userManager = null;
        private IRoleManager _roleManager = null;

        //Varible _allRoles and _userRoles used as ItemsSource, so ListBox contents can change
        List<String> _unassignedRoles = new List<String>();
        List<String> _userRoles = new List<String>();

        private bool _addMode = true;

        public frmEmployeeDetails(IUserManager userManager) //The window will be used for creating an Employee
        {
            InitializeComponent();

            _userManager = userManager;
        }

        public frmEmployeeDetails(User user, User currentUser, IUserManager userManager) //The window will be used for editting an Employee
        {
            InitializeComponent();

            _user = user;
            _currentUser = currentUser;
            _userManager = userManager;
            _addMode = false;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _roleManager = new RoleManager();
            _unassignedRoles = _roleManager.GetAllRoles();
            lstUnassignedRoles.ItemsSource = _unassignedRoles;
            txtEmployeeID.IsReadOnly = true;

            if (_addMode == false) //Viewing Details
            {
                //Set up fields for viewing details
                txtEmployeeID.Text = _user.EmployeeID.ToString();
                txtFirstName.Text = _user.FirstName;
                txtLastName.Text = _user.LastName;
                txtEmailAddress.Text = _user.Email;
                txtPhoneNumber.Text = _user.PhoneNumber;
                chkActive.IsChecked = _user.Active;

                //Disable changes and change to gray
                txtFirstName.IsReadOnly = true;
                txtLastName.IsReadOnly = true;
                txtEmailAddress.IsReadOnly = true;
                txtPhoneNumber.IsReadOnly = true;
                txtFirstName.Background = Brushes.LightGray;
                txtLastName.Background = Brushes.LightGray;
                txtEmailAddress.Background = Brushes.LightGray;
                txtPhoneNumber.Background = Brushes.LightGray;
                chkActive.IsEnabled = false;

                //Fills in selected Employee's roles
                _userRoles = _roleManager.GetEmployeeRoles(_user.EmployeeID);
                lstEmployeeRoles.ItemsSource = _userRoles;

                //Removes Employee's roles from UnassignedRoles
                updateUnassignedRoles();
            }
            else //Creating an Employee
            {
                //Set up fields for creating an employee
                txtEmployeeID.Text = "Auto Generated Value";
                txtFirstName.Focus();
                chkActive.IsChecked = true;
                chkActive.IsEnabled = false;
                showSaveButton();

                //Creates empty ItemsSource for new Employees
                lstEmployeeRoles.ItemsSource = _userRoles;
            }
        }

        //Parameter must be the ItemsSource for the UnassignedRoles ListBox
        private void updateUnassignedRoles()
        {
            foreach (String role in lstEmployeeRoles.ItemsSource)
            {
                //This must start from the last index, so indexes don't change as they're removed
                for (int n = lstUnassignedRoles.Items.Count - 1; n >= 0; --n)
                {
                    if (lstUnassignedRoles.Items[n].ToString().Equals(role))
                    {
                        //Must remove from ItemsSource instead of the ListBox
                        _unassignedRoles.RemoveAt(n);
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

        private void LstUnassignedRoles_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            //Only if an item is selected, and the edit button is hidden (meaning it has been clicked)
            if (lstUnassignedRoles.SelectedItem != null && btnEdit.Visibility == Visibility.Hidden)
            {
                _userRoles.Add(lstUnassignedRoles.SelectedItem.ToString());
                _unassignedRoles.RemoveAt(lstUnassignedRoles.SelectedIndex);
                lstUnassignedRoles.Items.Refresh();
                lstEmployeeRoles.Items.Refresh();
            }
        }

        private void LstEmployeeRoles_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            //Only if an item is selected, and the edit button is hidden (meaning it has been clicked)
            if (lstEmployeeRoles.SelectedItem != null && btnEdit.Visibility == Visibility.Hidden)
            {
                _unassignedRoles.Add(lstEmployeeRoles.SelectedItem.ToString());
                _userRoles.RemoveAt(lstEmployeeRoles.SelectedIndex);
                lstUnassignedRoles.Items.Refresh();
                lstEmployeeRoles.Items.Refresh();
            }
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            //Set up fields to Edit Employee
            txtFirstName.IsReadOnly = false;
            txtLastName.IsReadOnly = false;
            txtEmailAddress.IsReadOnly = false;
            txtPhoneNumber.IsReadOnly = false;
            txtFirstName.Background = Brushes.White;
            txtLastName.Background = Brushes.White;
            txtEmailAddress.Background = Brushes.White;
            txtPhoneNumber.Background = Brushes.White;

            //If Employee being modified, is the current user
            if (!(_currentUser.EmployeeID == _user.EmployeeID))
            {
                chkActive.IsEnabled = true;
            }
            else
            {
                lblCantChangeActiveStatus.Visibility = Visibility.Visible;
            }
            txtFirstName.Focus();

            showSaveButton();
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (validateInputs()) //True if inputs are valid
            {
                if (_user == null) //Adding new Employee
                {
                    createNewEmployee();
                }
                else //Updating existing employee
                {
                    editSelectedEmployee();
                }
            }
        }

        private void createNewEmployee()
        {
            int employeeID = 0; //ID Needed for getting roles
            string firstName = txtFirstName.Text.Trim();
            string lastName = txtLastName.Text.Trim();
            string phoneNumber = txtPhoneNumber.Text.Trim();
            string email = txtEmailAddress.Text.Trim();
            bool employeeCreated = false;

            //Attempt to add the new employee
            try
            {
                employeeCreated = _userManager.CreateEmployee(firstName, lastName, phoneNumber, email);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n\n" + ex.InnerException.Message,
                    "Employee Creation Failed", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            if (employeeCreated == false) //Employee not created
            {
                MessageBox.Show("Employee Could Not Be Created With Supplied Fields", "Employee Creation Failed",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                this.DialogResult = false;
            }
            else //Employee created
            {
                //Getting the new Employee's EmployeeID for adding Roles
                try
                {
                    User user = _userManager.GetEmployeeByEmail(email);
                    employeeID = user.EmployeeID;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message + "\n\n" + ex.InnerException.Message,
                        "New Employee Not Found", MessageBoxButton.OK, MessageBoxImage.Error);
                }

                //Adds selected Employee Roles to the selected Employee
                foreach (String s in lstEmployeeRoles.ItemsSource)
                {
                    try
                    {
                        _userManager.CreateEmployeeRole(employeeID, s);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message + "\n\n" + ex.InnerException.Message,
                        "Employee Role " + s + " Could Not Be Added", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }

                MessageBox.Show("Employee Successfully Created", "Employee Created",
                    MessageBoxButton.OK, MessageBoxImage.Information);

                this.DialogResult = true;
            }
        }

        public void editSelectedEmployee()
        {
            int employeeID = _user.EmployeeID;
            string newFirstName = txtFirstName.Text.Trim();
            string newLastName = txtLastName.Text.Trim();
            string newPhoneNumber = txtPhoneNumber.Text.Trim();
            string newEmail = txtEmailAddress.Text.Trim();
            bool newActiveStatus = (bool)chkActive.IsChecked;

            string oldFirstName = _user.FirstName;
            string oldLastName = _user.LastName;
            string oldPhoneNumber = _user.PhoneNumber;
            string oldEmail = _user.Email;
            bool employeeUpdated = false;

            //Attempt to edit the employee
            try
            {
                employeeUpdated = _userManager.UpdateEmployee(employeeID, newFirstName,
                    newLastName, newPhoneNumber, newEmail, newActiveStatus, oldFirstName,
                    oldLastName, oldPhoneNumber, oldEmail);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n\n" + ex.InnerException.Message,
                    "Employee Update Failed", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            if (employeeUpdated == false) //Employee not updated OR multiple updated
            {
                MessageBox.Show("Employee Could Not Be Updated With Supplied Fields", "Employee Update Failed",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                this.DialogResult = false;
            }
            else //Employee updated
            {
                //Deletes all of selected employee's roles (for next step)
                _roleManager.DeleteEmployeesRoles(employeeID);

                //Adds selected Employee Roles to the selected Employee
                foreach (String s in lstEmployeeRoles.ItemsSource)
                {
                    try
                    {
                        _userManager.CreateEmployeeRole(employeeID, s);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message + "\n\n" + ex.InnerException.Message,
                        "Employee Role " + s + " Could Not Be Added", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }

                MessageBox.Show("Employee Successfully Updated", "Employee Updated",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                this.DialogResult = true;
            }
        }

        private bool validateInputs()
        {
            bool inputsAreValid = true;
            string invalidInputMessage = "";

            //No blank values
            if (string.IsNullOrWhiteSpace(txtFirstName.Text) || string.IsNullOrWhiteSpace(txtLastName.Text) ||
                string.IsNullOrWhiteSpace(txtEmailAddress.Text) || string.IsNullOrWhiteSpace(txtPhoneNumber.Text))
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
            //Minimum length of 8 for email address
            if (txtEmailAddress.Text.Length < 8)
            {
                if (invalidInputMessage.Equals(""))
                {
                    invalidInputMessage += "Email Address Must Contain At Least 8 Characters!";
                }
                else
                {
                    invalidInputMessage += "\n\nEmail Address Must Contain At Least 8 Characters!";
                }
            }
            //Must contain '.' and '@', and cannot contain a space
            if (!(txtEmailAddress.Text.ToString().Contains("@") &&
                txtEmailAddress.Text.ToString().Contains(".")) ||
                txtEmailAddress.Text.ToString().Trim().Contains(" "))
            {
                if (invalidInputMessage.Equals(""))
                {
                    invalidInputMessage += "Email Address Is Invalid Format!";
                }
                else
                {
                    invalidInputMessage += "\n\nEmail Address Is Invalid Format!";
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

