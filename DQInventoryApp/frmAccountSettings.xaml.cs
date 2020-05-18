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
    /// Interaction logic for frmAccountSettings.xaml
    /// </summary>
    public partial class frmAccountSettings : Window
    {
        User _user = null;
        IUserManager _userManager = null;
        bool _makeUserLogout = false;

        public frmAccountSettings(User user, IUserManager userManager)
        {
            InitializeComponent();

            _user = user;
            _userManager = userManager;
        }

        private void BtnChangePassword_Click(object sender, RoutedEventArgs e)
        {
            var updatePassword = new frmUpdatePassword(_user, _userManager);
            if (updatePassword.ShowDialog() == true)
            {
                _makeUserLogout = true;
            }
        }

        private void BtnSubmit_Click(object sender, RoutedEventArgs e)
        {
            if (validateInputs())
            {
                bool infoChanged = false;
                //If phone number was changed
                if (!(txtPhoneNumber.Text.Trim().Equals(_user.PhoneNumber)))
                {
                    try
                    {
                        _userManager.EditPhoneNumber(
                            _user.EmployeeID, _user.PhoneNumber, txtPhoneNumber.Text);
                        infoChanged = true;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message + "\n\n" + ex.InnerException.Message);
                    }
                }
                //If email was changed
                if (!(txtEmailAddress.Text.Trim().Equals(_user.Email)))
                {
                    try
                    {
                        _userManager.EditEmail(_user.Email, txtEmailAddress.Text);
                        _makeUserLogout = true;
                        infoChanged = true;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message + "\n\n" + ex.InnerException.Message);
                    }
                }
                if (infoChanged)
                {
                    MessageBox.Show("Changes Saved!", "Saved!",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }

                this.DialogResult = !_makeUserLogout;
            }
        }

        private bool validateInputs()
        {
            bool inputsAreValid = true;
            string invalidInputMessage = "";

            //No blank values
            if (string.IsNullOrWhiteSpace(txtEmailAddress.Text) || 
                string.IsNullOrWhiteSpace(txtPhoneNumber.Text))
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

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = !_makeUserLogout;
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

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.DialogResult = !_makeUserLogout;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            txtPhoneNumber.Text = _user.PhoneNumber;
            txtEmailAddress.Text = _user.Email;
        }
    }
}
