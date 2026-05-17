using Automobiles_Services.Data;
using Automobiles_Services.Models;
using System.Windows;

namespace Automobiles_Services.Views
{
    /// <summary>
    /// Форма добавления и редактирования вместимости автомобиля.
    /// </summary>
    public partial class PassengerCapacityFormWindow : Window
    {
        private readonly PassengerCapacity? _capacity;

        public PassengerCapacityFormWindow(PassengerCapacity? capacity = null)
        {
            InitializeComponent();
            _capacity = capacity;
            TitleText.Text = capacity == null ? "Новая запись вместимости" : "Редактировать вместимость";
            Title = TitleText.Text;
            LoadAutomobiles();
            if (capacity != null) LoadData();
        }

        private void LoadAutomobiles()
        {
            using AppDbContext db = new AppDbContext();
            AutomobileComboBox.ItemsSource = db.Automobiles.ToList();
        }

        private void LoadData()
        {
            AutomobileComboBox.SelectedItem = (AutomobileComboBox.ItemsSource
                as List<Automobile>)?.FirstOrDefault(a => a.IdAutomobile == _capacity!.IdAutomobile);
            PassengersTextBox.Text = _capacity!.NumberOfPassengers.ToString();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (AutomobileComboBox.SelectedItem is not Automobile selectedAuto)
            {
                MessageBox.Show("Выберите автомобиль.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (!int.TryParse(PassengersTextBox.Text, out int passengers) || passengers <= 0)
            {
                MessageBox.Show("Кол-во пассажиров должно быть целым положительным числом.",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            using AppDbContext db = new AppDbContext();

            if (_capacity == null)
            {
                db.PassengerCapacities.Add(new PassengerCapacity
                {
                    IdAutomobile = selectedAuto.IdAutomobile,
                    NumberOfPassengers = passengers
                });
            }
            else
            {
                var entity = db.PassengerCapacities
                    .FirstOrDefault(p => p.IdPassengerCapacity == _capacity.IdPassengerCapacity);
                if (entity == null) return;
                entity.IdAutomobile = selectedAuto.IdAutomobile;
                entity.NumberOfPassengers = passengers;
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