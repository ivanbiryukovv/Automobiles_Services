using Automobiles_Services.Data;
using Automobiles_Services.Models;
using System.Text.RegularExpressions;
using System.Windows;

namespace Automobiles_Services.Views
{
    public partial class AddServiceCenterWindow : Window
    {
        public AddServiceCenterWindow()
        {
            InitializeComponent();
        }

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

            if (!Regex.IsMatch(CityTextBox.Text.Trim(), @"[а-яА-ЯёЁa-zA-Z]"))
            {
                MessageBox.Show("Название города должно содержать хотя бы одну букву.",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (string.IsNullOrWhiteSpace(StreetTextBox.Text))
            {
                MessageBox.Show("Введите улицу.",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!Regex.IsMatch(StreetTextBox.Text.Trim(), @"[а-яА-ЯёЁa-zA-Z]"))
            {
                MessageBox.Show("Название улицы должно содержать хотя бы одну букву.",
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
            db.ServiceCenters.Add(new ServiceCenter
            {
                CompanyName = CompanyNameTextBox.Text.Trim(),
                NameOfTheCenter = NameOfTheCenterTextBox.Text.Trim(),
                NumberOfSeats = seats,
                City = CityTextBox.Text.Trim(),
                Street = StreetTextBox.Text.Trim(),
                House = HouseTextBox.Text.Trim()
            });
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