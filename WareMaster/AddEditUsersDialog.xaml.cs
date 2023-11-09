using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
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

namespace WareMaster
{
    /// <summary>
    /// Interaction logic for AddEditUsersDialog.xaml
    /// </summary>
    public partial class AddEditUsersDialog : Window
    {
        User currUser = new User();
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

        //private void UsernameInput_LostFocus()
        //{

        //}

        //private void EmailInputt_LostFocus()
        //{

        //}
        //private void RoleComboBox_LostFocus() { }

        //private void PasswordInput_LostFocus() { }

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
                    Console.WriteLine("ENTER EDIT");
                    var userToUpdate = Globals.wareMasterEntities.Users.FirstOrDefault(user => user.id == currUser.id);
                    userToUpdate.Username = UsernameInput.Text;
                    userToUpdate.Email = EmailInput.Text;
                    userToUpdate.Password = PasswordInput.Text;
                    if (Enum.TryParse(role, out RoleEnum roleEnumValue))
                    {
                        userToUpdate.Role = roleEnumValue;
                    }
                    else
                    {
                        throw new ArgumentException("Role error");
                    }

                    Console.WriteLine(currUser);
                }
                else // add
                {
                    Console.WriteLine("enter add");
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
