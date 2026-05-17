using Automobiles_Services.Data;
using Automobiles_Services.Models;
using System.Windows;
using System.Windows.Controls;

namespace Automobiles_Services.Views
{
    /// <summary>
    /// Форма добавления и редактирования автомобиля.
    /// </summary>
    public partial class AutomobileFormWindow : Window
    {
        private readonly Automobile? _automobile;

        public AutomobileFormWindow(Automobile? automobile = null)
        {
            InitializeComponent();
            _automobile = automobile;
            TitleText.Text = automobile == null ? "Новый автомобиль" : "Редактировать автомобиль";
            Title = TitleText.Text;
            LoadManufacturers();
            if (automobile != null) LoadData();
        }

        /// <summary>Загружает список производителей в ComboBox.</summary>
        private void LoadManufacturers()
        {
            using AppDbContext db = new AppDbContext();
            ManufacturerComboBox.ItemsSource = db.Manufacturers.ToList();
        }

        private void LoadData()
        {
            ManufacturerComboBox.SelectedValue = _automobile!.IdManufacturer;
            ManufacturerComboBox.SelectedItem = (ManufacturerComboBox.ItemsSource
                as List<Manufacturer>)?.FirstOrDefault(m => m.IdManufacturer == _automobile.IdManufacturer);
            SeriesTextBox.Text = _automobile.Series;
            EnginePowerTextBox.Text = _automobile.EnginePower;
            WeightTextBox.Text = _automobile.Weight.ToString();
            ColorTextBox.Text = _automobile.Color;
            AirConditioningComboBox.SelectedItem = AirConditioningComboBox.Items
                .Cast<ComboBoxItem>()
                .FirstOrDefault(i => i.Content.ToString() == _automobile.AvailabilityOfAirConditioning);

            // Convert DateOnly to DateTime for DatePicker
            YearOfManufacturePicker.SelectedDate = _automobile.YearOfManufacture.ToDateTime(TimeOnly.MinValue);

            BodyTypeTextBox.Text = _automobile.BodyType;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (ManufacturerComboBox.SelectedItem is not Manufacturer selectedManufacturer)
            {
                MessageBox.Show("Выберите завод-производитель.",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (string.IsNullOrWhiteSpace(SeriesTextBox.Text))
            {
                MessageBox.Show("Введите серию.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (string.IsNullOrWhiteSpace(EnginePowerTextBox.Text))
            {
                MessageBox.Show("Введите мощность двигателя.",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (!int.TryParse(WeightTextBox.Text, out int weight) || weight <= 0)
            {
                MessageBox.Show("Вес должен быть целым положительным числом.",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (string.IsNullOrWhiteSpace(ColorTextBox.Text))
            {
                MessageBox.Show("Введите цвет.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (AirConditioningComboBox.SelectedItem is not ComboBoxItem selectedAC)
            {
                MessageBox.Show("Выберите наличие кондиционера.",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (YearOfManufacturePicker.SelectedDate == null)
            {
                MessageBox.Show("Выберите дату выпуска.",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (string.IsNullOrWhiteSpace(BodyTypeTextBox.Text))
            {
                MessageBox.Show("Введите тип кузова.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            using AppDbContext db = new AppDbContext();

            if (_automobile == null)
            {
                db.Automobiles.Add(new Automobile
                {
                    IdManufacturer = selectedManufacturer.IdManufacturer,
                    Series = SeriesTextBox.Text.Trim(),
                    EnginePower = EnginePowerTextBox.Text.Trim(),
                    Weight = weight,
                    Color = ColorTextBox.Text.Trim(),
                    AvailabilityOfAirConditioning = selectedAC.Content.ToString()!,
                    // Convert DateTime to DateOnly
                    YearOfManufacture = DateOnly.FromDateTime(YearOfManufacturePicker.SelectedDate!.Value),
                    BodyType = BodyTypeTextBox.Text.Trim()
                });
            }
            else
            {
                var entity = db.Automobiles
                    .FirstOrDefault(a => a.IdAutomobile == _automobile.IdAutomobile);
                if (entity == null) return;
                entity.IdManufacturer = selectedManufacturer.IdManufacturer;
                entity.Series = SeriesTextBox.Text.Trim();
                entity.EnginePower = EnginePowerTextBox.Text.Trim();
                entity.Weight = weight;
                entity.Color = ColorTextBox.Text.Trim();
                entity.AvailabilityOfAirConditioning = selectedAC.Content.ToString()!;
                // Convert DateTime to DateOnly
                entity.YearOfManufacture = DateOnly.FromDateTime(YearOfManufacturePicker.SelectedDate!.Value);
                entity.BodyType = BodyTypeTextBox.Text.Trim();
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