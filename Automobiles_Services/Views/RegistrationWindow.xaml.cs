using Automobiles_Services.Services;
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
using Automobiles_Services.Services;
using DocumentFormat.OpenXml.Drawing;

namespace Automobiles_Services.Views
{
    /// <summary>
    /// Логика взаимодействия для RegistrationWindow.xaml
    /// </summary>
    public partial class RegistrationWindow : Window
    {
        private readonly AuthService _authService = new AuthService();

        public RegistrationWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 
        /// Показывает сообщение под кнопками.
        /// </summary>
        /// <param name="text">Текст сообщения.</param>
        /// <param name="success">true — зелёный, false — красный.</param>
        private void ShowMessage(string text, bool success)
        {
            MessageText.Text = text;
            MessageText.Foreground = success
                ? new SolidColorBrush(Color.FromRgb(34, 139, 34))
                : new SolidColorBrush(Color.FromRgb(200, 0, 0));
        }

        /// <summary>
        /// Обработчик кнопки «Регистрация».
        /// </summary>
        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            LoginWindow loginWindow = new LoginWindow();
            string login = LoginTextBox.Text.Trim();
            string password = PasswordBox.Password;

            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
            {
                ShowMessage("Заполните все поля.", false);
                return;
            }

            if (PasswordBox.Password != PasswordBox_Confirm.Password)
            {
                ShowMessage("Пароли не совпадают.", false);
                return;
            }

            RegistrationResult result = _authService.Register(login, password);

            switch (result)
            {
                case RegistrationResult.Success:
                    ShowMessage("Регистрация выполнена успешно!", true);
                    loginWindow.Show();
                    this.Close();
                    break;
                case RegistrationResult.LoginExists:
                    ShowMessage("Пользователь с таким логином уже существует.", false);
                    break;
                case RegistrationResult.WeakPassword:
                    ShowMessage(
                        "Пароль должен содержать минимум 6 символов, " +
                        "1 заглавную букву, 1 цифру и 1 символ из: ! @ # $ % ^",
                        false);
                    break;
            }


        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            LoginWindow loginWindow = new LoginWindow();
            loginWindow.Show();
            this.Close();
        }
    }
}
