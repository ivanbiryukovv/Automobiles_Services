using Automobiles_Services.Data;
using Automobiles_Services.Models;
using System.Windows;
using System.Windows.Controls;

namespace Automobiles_Services.Views
{
    /// <summary>
    /// Форма добавления и редактирования пользователя.
    /// </summary>
    public partial class UserFormWindow : Window
    {
        private readonly User? _user;

        public UserFormWindow(User? user = null)
        {
            InitializeComponent();
            _user = user;
            TitleText.Text = user == null ? "Новый пользователь" : "Редактировать пользователя";
            Title = TitleText.Text;
            if (user != null) LoadData();
        }

        private void LoadData()
        {
            LoginTextBox.Text = _user!.Login;
            PasswordTextBox.Text = _user.Password;
            RoleComboBox.SelectedItem = RoleComboBox.Items
                .Cast<ComboBoxItem>()
                .FirstOrDefault(i => i.Content.ToString() == _user.Role);
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(LoginTextBox.Text))
            {
                MessageBox.Show("Введите логин.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (string.IsNullOrWhiteSpace(PasswordTextBox.Text))
            {
                MessageBox.Show("Введите пароль.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (RoleComboBox.SelectedItem is not ComboBoxItem selectedRole)
            {
                MessageBox.Show("Выберите роль.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            using AppDbContext db = new AppDbContext();

            if (_user == null)
            {
                if (db.Users.Any(u => u.Login == LoginTextBox.Text.Trim()))
                {
                    MessageBox.Show("Пользователь с таким логином уже существует.",
                        "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                db.Users.Add(new User
                {
                    Login = LoginTextBox.Text.Trim(),
                    Password = PasswordTextBox.Text.Trim(),
                    Role = selectedRole.Content.ToString()!
                });
            }
            else
            {
                var entity = db.Users.FirstOrDefault(u => u.IdUser == _user.IdUser);
                if (entity == null) return;
                entity.Login = LoginTextBox.Text.Trim();
                entity.Password = PasswordTextBox.Text.Trim();
                entity.Role = selectedRole.Content.ToString()!;
            }

            db.SaveChanges();
            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}