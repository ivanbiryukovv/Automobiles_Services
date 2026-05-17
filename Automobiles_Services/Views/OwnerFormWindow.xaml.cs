using Automobiles_Services.Data;
using Automobiles_Services.Models;
using System.Windows;

namespace Automobiles_Services.Views
{
    /// <summary>
    /// Форма добавления и редактирования владельца.
    /// </summary>
    public partial class OwnerFormWindow : Window
    {
        private readonly Owner? _owner;

        /// <param name="owner">null — режим добавления, иначе — редактирование.</param>
        public OwnerFormWindow(Owner? owner = null)
        {
            InitializeComponent();
            _owner = owner;
            TitleText.Text = owner == null ? "Новый владелец" : "Редактировать владельца";
            Title = TitleText.Text;
            if (owner != null) LoadData();
        }

        private void LoadData()
        {
            LastNameTextBox.Text = _owner!.LastName;
            FirstNameTextBox.Text = _owner.FirstName;
            SurnameTextBox.Text = _owner.Surname;
            PhoneTextBox.Text = _owner.PhoneNumber;
            LicenseSeriesTextBox.Text = _owner.DriversLicenseSeries;
            LicenseNumberTextBox.Text = _owner.DriversLicenseNumber;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(LastNameTextBox.Text))
            {
                MessageBox.Show("Введите фамилию.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (string.IsNullOrWhiteSpace(FirstNameTextBox.Text))
            {
                MessageBox.Show("Введите имя.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (PhoneTextBox.Text.Length != 11 || !PhoneTextBox.Text.All(char.IsDigit))
            {
                MessageBox.Show("Телефон должен содержать ровно 11 цифр.",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (LicenseSeriesTextBox.Text.Length != 4)
            {
                MessageBox.Show("Серия ВУ должна содержать ровно 4 символа.",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (LicenseNumberTextBox.Text.Length != 6)
            {
                MessageBox.Show("Номер ВУ должен содержать ровно 6 символов.",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            using AppDbContext db = new AppDbContext();

            if (_owner == null)
            {
                db.Owners.Add(new Owner
                {
                    LastName = LastNameTextBox.Text.Trim(),
                    FirstName = FirstNameTextBox.Text.Trim(),
                    Surname = string.IsNullOrWhiteSpace(SurnameTextBox.Text)
                        ? null : SurnameTextBox.Text.Trim(),
                    PhoneNumber = PhoneTextBox.Text.Trim(),
                    DriversLicenseSeries = LicenseSeriesTextBox.Text.Trim(),
                    DriversLicenseNumber = LicenseNumberTextBox.Text.Trim()
                });
            }
            else
            {
                var entity = db.Owners.FirstOrDefault(o => o.IdOwner == _owner.IdOwner);
                if (entity == null) return;
                entity.LastName = LastNameTextBox.Text.Trim();
                entity.FirstName = FirstNameTextBox.Text.Trim();
                entity.Surname = string.IsNullOrWhiteSpace(SurnameTextBox.Text)
                    ? null : SurnameTextBox.Text.Trim();
                entity.PhoneNumber = PhoneTextBox.Text.Trim();
                entity.DriversLicenseSeries = LicenseSeriesTextBox.Text.Trim();
                entity.DriversLicenseNumber = LicenseNumberTextBox.Text.Trim();
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