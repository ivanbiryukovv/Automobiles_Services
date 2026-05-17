using Automobiles_Services.Data;
using Automobiles_Services.Models;
using System.Windows;

namespace Automobiles_Services.Views
{
    /// <summary>
    /// Форма добавления и редактирования договора.
    /// </summary>
    public partial class AgreementFormWindow : Window
    {
        private readonly Agreement? _agreement;

        public AgreementFormWindow(Agreement? agreement = null)
        {
            InitializeComponent();
            _agreement = agreement;
            TitleText.Text = agreement == null ? "Новый договор" : "Редактировать договор";
            Title = TitleText.Text;
            LoadComboBoxes();
            if (agreement != null) LoadData();
        }

        private void LoadComboBoxes()
        {
            using AppDbContext db = new AppDbContext();
            ServiceCenterComboBox.ItemsSource = db.ServiceCenters.ToList();

            var owners = db.Owners.ToList();
            OwnerComboBox.ItemsSource = owners;
            OwnerComboBox.DisplayMemberPath = "LastName";
        }

        private void LoadData()
        {
            ServiceCenterComboBox.SelectedItem = (ServiceCenterComboBox.ItemsSource
                as List<ServiceCenter>)?.FirstOrDefault(s => s.IdServiceCenter == _agreement!.IdServiceCenter);
            OwnerComboBox.SelectedItem = (OwnerComboBox.ItemsSource
                as List<Owner>)?.FirstOrDefault(o => o.IdOwner == _agreement!.IdOwner);
            TypeOfServiceTextBox.Text = _agreement!.TypeOfService;

            // Convert DateOnly to DateTime for DatePicker
            DateOfServicePicker.SelectedDate = _agreement.DateOfService.ToDateTime(TimeOnly.MinValue);
            GuaranteeUpToPicker.SelectedDate = _agreement.GuaranteeUpTo.ToDateTime(TimeOnly.MinValue);
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (ServiceCenterComboBox.SelectedItem is not ServiceCenter selectedCenter)
            {
                MessageBox.Show("Выберите сервисный центр.",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (OwnerComboBox.SelectedItem is not Owner selectedOwner)
            {
                MessageBox.Show("Выберите владельца.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (string.IsNullOrWhiteSpace(TypeOfServiceTextBox.Text))
            {
                MessageBox.Show("Введите тип обслуживания.",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (DateOfServicePicker.SelectedDate == null)
            {
                MessageBox.Show("Выберите дату обслуживания.",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (GuaranteeUpToPicker.SelectedDate == null)
            {
                MessageBox.Show("Выберите дату окончания гарантии.",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (GuaranteeUpToPicker.SelectedDate <= DateOfServicePicker.SelectedDate)
            {
                MessageBox.Show("Дата гарантии должна быть позже даты обслуживания.",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            using AppDbContext db = new AppDbContext();

            if (_agreement == null)
            {
                db.Agreements.Add(new Agreement
                {
                    IdServiceCenter = selectedCenter.IdServiceCenter,
                    IdOwner = selectedOwner.IdOwner,
                    TypeOfService = TypeOfServiceTextBox.Text.Trim(),
                    // Convert DateTime to DateOnly
                    DateOfService = DateOnly.FromDateTime(DateOfServicePicker.SelectedDate!.Value),
                    GuaranteeUpTo = DateOnly.FromDateTime(GuaranteeUpToPicker.SelectedDate!.Value)
                });
            }
            else
            {
                var entity = db.Agreements
                    .FirstOrDefault(a => a.IdAgreement == _agreement.IdAgreement);
                if (entity == null) return;
                entity.IdServiceCenter = selectedCenter.IdServiceCenter;
                entity.IdOwner = selectedOwner.IdOwner;
                entity.TypeOfService = TypeOfServiceTextBox.Text.Trim();
                // Convert DateTime to DateOnly
                entity.DateOfService = DateOnly.FromDateTime(DateOfServicePicker.SelectedDate!.Value);
                entity.GuaranteeUpTo = DateOnly.FromDateTime(GuaranteeUpToPicker.SelectedDate!.Value);
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