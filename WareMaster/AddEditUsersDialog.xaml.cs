﻿using ControlzEx.Standard;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WareMaster
{
    /// <summary>
    /// Interaction logic for AddEditUsersDialog.xaml
    /// </summary>
    public partial class AddEditUsersDialog : Window
    {
        User currUser = new User();
        UserInputValidator validator = new UserInputValidator();
        string errorMessage;

        public AddEditUsersDialog(User currUser = null)
        {
            InitializeComponent();
            
            this.currUser = currUser;
            if (currUser != null) // update, load select values
            {
                UserId.Content = currUser.id;
                UsernameInput.Text = currUser.Username;
                EmailInput.Text = currUser.Email;
                PasswordInput.Text = currUser.Password;
                RoleComboBox.SelectedItem = currUser.Role;
            }
        }

        private void UsernameInput_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!User.IsUserNameValid(UsernameInput.Text, out errorMessage))
            {
                LblErrUsername.Visibility = Visibility.Visible;
                LblErrUsername.Content = errorMessage;
            }
            else
            {
                LblErrUsername.Visibility = Visibility.Hidden;
            }
        }

        private void EmailInput_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!User.IsEmailValid(EmailInput.Text, out errorMessage))
            {
                LblErrEmail.Visibility = Visibility.Visible;
                LblErrEmail.Content = errorMessage;
            }
            else
            {
                LblErrEmail.Visibility = Visibility.Hidden;
            }
        }

        private void RoleComboBox_LostFocus(object sender, RoutedEventArgs e) 
        {
            if (!User.IsRoleValid(RoleComboBox.SelectedItem.ToString(), out errorMessage))
            {
                LblErrRole.Visibility = Visibility.Visible;
                LblErrRole.Content = errorMessage;
            }
            else
            {
                LblErrRole.Visibility = Visibility.Hidden;
            }
        }

        private void PasswordInput_LostFocus(object sender, RoutedEventArgs e) 
        {
            if (!User.IsPasswordValid(PasswordInput.Text, out errorMessage))
            {
                LblErrPassword.Visibility = Visibility.Visible;
                LblErrPassword.Content = errorMessage;
                Console.WriteLine(PasswordInput.Text);
            }
            else
            {
                LblErrPassword.Visibility = Visibility.Hidden;
                Console.WriteLine(PasswordInput.Text);
                Console.WriteLine("Password is valid. Hiding LblErrPassword.");
            }
        }

        private bool IsItemnameDuplicate(string itemname, out string error)
        {
            // get current itemname list
            List<string> allNames = Globals.wareMasterEntities.Items.Select(item => item.Itemname).ToList();
            if(allNames.Contains(itemname))
            {
                error = "Itemname must be unique";
                return false;
            }
            error = null;
            return true;
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(UsernameInput.Text) ||
                    string.IsNullOrWhiteSpace(EmailInput.Text) ||
                    string.IsNullOrWhiteSpace(PasswordInput.Text) ||
                    RoleComboBox.SelectedItem == null)
                {
                    throw new ArgumentException("Input incompleted");
                }

                string role = (RoleComboBox.SelectedItem as ComboBoxItem)?.Tag as string;
                if (currUser != null)
                { // update
                    var userToUpdate = Globals.wareMasterEntities.Users.FirstOrDefault(user => user.id == currUser.id);
                    userToUpdate.Username = UsernameInput.Text;
                    userToUpdate.Email = EmailInput.Text;
                    userToUpdate.SetHashedPassword(PasswordInput.Text);
                    if (Enum.TryParse(role, out RoleEnum roleEnumValue))
                    {
                        userToUpdate.Role = roleEnumValue;
                    }
                    else
                    {
                        throw new ArgumentException("Role error");
                    }
                    var result = validator.Validate(userToUpdate);
                    if(!result.IsValid)
                    {
                        throw new ArgumentException(result.ToString(Environment.NewLine));
                    }
                    Console.WriteLine(currUser);
                }
                else // add
                {
                    User newUser;

                    if (Enum.TryParse(role, out RoleEnum roleEnumValue))
                    {
                        newUser = new User
                        {
                            Username = UsernameInput.Text,
                            Email = EmailInput.Text,
                            Role = roleEnumValue
                        };
                        newUser.SetHashedPassword(PasswordInput.Text);
                    }
                    else
                    {
                        throw new ArgumentException("Role error");
                    }
                    var result = validator.Validate(newUser);
                    if (!result.IsValid)
                    {
                        throw new ArgumentException(result.ToString(Environment.NewLine));
                    }
                    Console.WriteLine(newUser);
                    Globals.wareMasterEntities.Users.Add(newUser);
                }
                Globals.wareMasterEntities.SaveChanges();
                this.DialogResult = true; // dismiss the dialog
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(this, ex.Message, "Input error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (SystemException ex)
            {
                MessageBox.Show(this, "Error reading from database\n" + ex.Message, "Database error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

       
    }
}
