using System.Windows;

namespace Automobiles_Services.Views
{
    public partial class SearchServiceCenterWindow : Window
    {
        public string CompanyName { get; private set; } = "";

        public SearchServiceCenterWindow()
        {
            InitializeComponent();
        }

        private void Search_Click(object sender, RoutedEventArgs e)
        {
            CompanyName = CompanyTextBox.Text.Trim();

            DialogResult = true;
            Close();
        }
    }
}