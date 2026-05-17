using Automobiles_Services.Data;
using Automobiles_Services.Models;
using System.Windows;

namespace Automobiles_Services.Views
{
    /// <summary>
    /// Диалоговое окно редактирования сервисного центра.
    /// </summary>
    public partial class EditServiceCenterWindow : Window
    {
        private readonly ServiceCenter _serviceCenter;

        /// <summary>
        /// Принимает существующий объект и подгружает его данные в поля.
        /// </summary>
        /// <param name="serviceCenter">Редактируемый сервисный центр.</param>
        public EditServiceCenterWindow(ServiceCenter serviceCenter)
        {
            InitializeComponent();
            _serviceCenter = serviceCenter;
            LoadData();
        }

        /// <summary>
        /// Заполняет поля формы данными выбранного объекта.
        /// </summary>
        private void LoadData()
        {
            CompanyNameTextBox.Text = _serviceCenter.CompanyName;
            NameOfTheCenterTextBox.Text = _serviceCenter.NameOfTheCenter;
            NumberOfSeatsTextBox.Text = _serviceCenter.NumberOfSeats.ToString();
            CityTextBox.Text = _serviceCenter.City;
            StreetTextBox.Text = _serviceCenter.Street;
            HouseTextBox.Text = _serviceCenter.House;
        }

        /// <summary>
        /// Сохраняет изменения в базе данных.
        /// </summary>
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(CompanyNameTextBox.Text))
            {
                MessageBox.Show("Введите название компании.",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (string.IsNullOrWhiteSpace(NameOfTheCenterTextBox.Text))
            {
                MessageBox.Show("Введите название центра.",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!int.TryParse(NumberOfSeatsTextBox.Text, out int seats) || seats <= 0)
            {
                MessageBox.Show("Количество мест должно быть целым положительным числом.",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (string.IsNullOrWhiteSpace(CityTextBox.Text))
            {
                MessageBox.Show("Введите город.",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (string.IsNullOrWhiteSpace(StreetTextBox.Text))
            {
                MessageBox.Show("Введите улицу.",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (string.IsNullOrWhiteSpace(HouseTextBox.Text))
            {
                MessageBox.Show("Введите номер дома.",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            using AppDbContext db = new AppDbContext();
            ServiceCenter? entity = db.ServiceCenters
                .FirstOrDefault(s => s.IdServiceCenter == _serviceCenter.IdServiceCenter);

            if (entity == null)
            {
                MessageBox.Show("Запись не найдена в базе данных.",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            entity.CompanyName = CompanyNameTextBox.Text.Trim();
            entity.NameOfTheCenter = NameOfTheCenterTextBox.Text.Trim();
            entity.NumberOfSeats = seats;
            entity.City = CityTextBox.Text.Trim();
            entity.Street = StreetTextBox.Text.Trim();
            entity.House = HouseTextBox.Text.Trim();

            db.SaveChanges();

            DialogResult = true;
            Close();
        }

        /// <summary>
        /// Закрывает окно без сохранения.
        /// </summary>
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}