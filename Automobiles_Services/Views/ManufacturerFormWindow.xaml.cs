using Automobiles_Services.Data;
using Automobiles_Services.Models;
using System.Windows;

namespace Automobiles_Services.Views
{
    /// <summary>
    /// Форма добавления и редактирования завода-производителя.
    /// </summary>
    public partial class ManufacturerFormWindow : Window
    {
        private readonly Manufacturer? _manufacturer;

        public ManufacturerFormWindow(Manufacturer? manufacturer = null)
        {
            InitializeComponent();
            _manufacturer = manufacturer;
            TitleText.Text = manufacturer == null ? "Новый завод-производитель" : "Редактировать завод-производитель";
            Title = TitleText.Text;
            if (manufacturer != null) LoadData();
        }

        private void LoadData()
        {
            NameTextBox.Text = _manufacturer!.Name;
            CountryTextBox.Text = _manufacturer.Country;
            CityTextBox.Text = _manufacturer.City;
            StreetTextBox.Text = _manufacturer.Street;
            HouseTextBox.Text = _manufacturer.House;

            // Convert DateOnly to DateTime for DatePicker
            FoundingDatePicker.SelectedDate = _manufacturer.FoundingDate.ToDateTime(TimeOnly.MinValue);
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameTextBox.Text))
            {
                MessageBox.Show("Введите название.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (string.IsNullOrWhiteSpace(CountryTextBox.Text))
            {
                MessageBox.Show("Введите страну.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (string.IsNullOrWhiteSpace(CityTextBox.Text))
            {
                MessageBox.Show("Введите город.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (string.IsNullOrWhiteSpace(StreetTextBox.Text))
            {
                MessageBox.Show("Введите улицу.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (string.IsNullOrWhiteSpace(HouseTextBox.Text))
            {
                MessageBox.Show("Введите номер дома.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (FoundingDatePicker.SelectedDate == null)
            {
                MessageBox.Show("Выберите дату основания.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            using AppDbContext db = new AppDbContext();

            if (_manufacturer == null)
            {
                db.Manufacturers.Add(new Manufacturer
                {
                    Name = NameTextBox.Text.Trim(),
                    Country = CountryTextBox.Text.Trim(),
                    City = CityTextBox.Text.Trim(),
                    Street = StreetTextBox.Text.Trim(),
                    House = HouseTextBox.Text.Trim(),
                    // Convert DateTime to DateOnly
                    FoundingDate = DateOnly.FromDateTime(FoundingDatePicker.SelectedDate!.Value)
                });
            }
            else
            {
                var entity = db.Manufacturers
                    .FirstOrDefault(m => m.IdManufacturer == _manufacturer.IdManufacturer);
                if (entity == null) return;
                entity.Name = NameTextBox.Text.Trim();
                entity.Country = CountryTextBox.Text.Trim();
                entity.City = CityTextBox.Text.Trim();
                entity.Street = StreetTextBox.Text.Trim();
                entity.House = HouseTextBox.Text.Trim();
                // Convert DateTime to DateOnly
                entity.FoundingDate = DateOnly.FromDateTime(FoundingDatePicker.SelectedDate!.Value);
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