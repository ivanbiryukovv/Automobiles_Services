using Automobiles_Services.Models;
using Automobiles_Services.Services;
using System.Windows;
using System.Windows.Media;

namespace Automobiles_Services.Views
{
    /// <summary>
    /// Логика взаимодействия для LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        private readonly AuthService _authService = new AuthService();

        public LoginWindow()
        {
            InitializeComponent();
        }

        /// <summary>
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
        /// Обработчик кнопки «Войти».
        /// </summary>
        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string login = LoginTextBox.Text.Trim();
            string password = PasswordBox.Password;

            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
            {
                ShowMessage("Заполните все поля.", false);
                return;
            }

            User? user = _authService.Login(login, password);

            if (user == null)
            {
                ShowMessage("Неверный логин или пароль.", false);
                return;
            }

            ShowMessage("Вход выполнен успешно!", true);


            MainWindow mainWindow = new MainWindow(user);
            mainWindow.Show();
            this.Close();
        }

        /// <summary>
        /// Обработчик кнопки «Регистрация».
        /// </summary>
        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            RegistrationWindow regWindow = new RegistrationWindow();
            regWindow.Show();
            this.Close();
        }
    }
}