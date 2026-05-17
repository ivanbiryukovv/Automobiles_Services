using Automobiles_Services.Data;
using Automobiles_Services.Models;
using Automobiles_Services.Services;
using ClosedXML.Excel;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace Automobiles_Services.Views
{
    public partial class MainWindow : Window
    {
        private readonly User _currentUser;
        private readonly ServiceCenterService _serviceCenterService = new();
        private readonly OwnerService _ownersService = new();
        private readonly ManufacturerService _manufacturerService = new();
        private readonly AutomobileService _automobileService = new();
        private readonly PassengerCapacityService _passengerCapacityService = new();
        private readonly AgreementService _agreementService = new();
        private readonly UserService _userService = new();
        private readonly AppDbContext _context = new();

        private string _currentSection = "ServiceCenters";
        private string _parentSection = "";
        private object _parentItem = null;
        private readonly Dictionary<string, Button> _menuButtons;
        private bool _isRelatedView = false;

        private System.Collections.IEnumerable _currentFullList = null;

        public string CurrentUserInfo => $"Пользователь: {_currentUser.Login} | Роль: {_currentUser.Role.Trim()}";

        public MainWindow(User user)
        {
            InitializeComponent();
            _currentUser = user;
            DataContext = this;

            _menuButtons = new Dictionary<string, Button>
            {
                { "ServiceCenters", MenuServiceCenters },
                { "Owners", MenuOwners },
                { "Manufacturers", MenuManufacturers },
                { "Automobiles", MenuAutomobiles },
                { "PassengerCapacities", MenuPassengerCapacities },
                { "Agreements", MenuAgreements },
                { "Users", MenuUsers }
            };

            RoleRestriction();
            ShowSection("ServiceCenters");
        }


        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var query = SearchBox.Text.Trim().ToLower();

            SearchPlaceholder.Visibility = string.IsNullOrEmpty(SearchBox.Text)
                ? Visibility.Visible : Visibility.Collapsed;

            ClearSearchButton.Visibility = string.IsNullOrEmpty(SearchBox.Text)
                ? Visibility.Collapsed : Visibility.Visible;

            if (_currentFullList == null) return;

            if (string.IsNullOrEmpty(query))
            {
                CardsListBox.ItemsSource = _currentFullList;
                return;
            }

            CardsListBox.ItemsSource = _currentSection switch
            {
                "ServiceCenters" => _currentFullList.Cast<ServiceCenter>()
                    .Where(x => (x.NameOfTheCenter?.ToLower().Contains(query) ?? false)
                             || (x.CompanyName?.ToLower().Contains(query) ?? false)
                             || (x.City?.ToLower().Contains(query) ?? false)),

                "Owners" => _currentFullList.Cast<Owner>()
                    .Where(x => (x.LastName?.ToLower().Contains(query) ?? false)
                             || (x.FirstName?.ToLower().Contains(query) ?? false)
                             || (x.Surname?.ToLower().Contains(query) ?? false)
                             || (x.PhoneNumber?.ToLower().Contains(query) ?? false)),

                "Manufacturers" => _currentFullList.Cast<Manufacturer>()
                    .Where(x => (x.Name?.ToLower().Contains(query) ?? false)
                             || (x.Country?.ToLower().Contains(query) ?? false)
                             || (x.City?.ToLower().Contains(query) ?? false)),

                "Automobiles" => _currentFullList.Cast<Automobile>()
                    .Where(x => (x.Series?.ToLower().Contains(query) ?? false)
                             || (x.Color?.ToLower().Contains(query) ?? false)
                             || (x.BodyType?.ToLower().Contains(query) ?? false)),

                "PassengerCapacities" => _currentFullList.Cast<PassengerCapacity>()
                    .Where(x => x.NumberOfPassengers.ToString().Contains(query)
                             || x.IdAutomobile.ToString().Contains(query)),

                "Agreements" => _currentFullList.Cast<Agreement>()
                    .Where(x => (x.TypeOfService?.ToLower().Contains(query) ?? false)),

                "Users" => _currentFullList.Cast<User>()
                    .Where(x => (x.Login?.ToLower().Contains(query) ?? false)
                             || (x.Role?.ToLower().Contains(query) ?? false)),

                _ => _currentFullList
            };
        }

        private void ClearSearch_Click(object sender, RoutedEventArgs e)
        {
            SearchBox.Text = "";
            SearchBox.Focus();
        }

        private void ResetSearch()
        {
            SearchBox.Text = "";
            SearchPlaceholder.Visibility = Visibility.Visible;
            ClearSearchButton.Visibility = Visibility.Collapsed;
        }


        private void MenuClick(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is string section)
            {
                _isRelatedView = false;
                ShowSection(section);
            }
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            _isRelatedView = false;
            ShowSection(_parentSection);
        }

        private void ShowSection(string section)
        {
            _currentSection = section;
            _parentSection = section;
            _parentItem = null;

            ResetSearch();

            foreach (var kvp in _menuButtons)
                kvp.Value.Style = (Style)FindResource(
                    kvp.Key == section ? "SideMenuButtonActive" : "SideMenuButton");

            BackButton.Visibility = _isRelatedView ? Visibility.Visible : Visibility.Collapsed;
            ParentInfo.Visibility = _isRelatedView ? Visibility.Visible : Visibility.Collapsed;
            ActionButtons.Visibility = _isRelatedView ? Visibility.Collapsed : Visibility.Visible;
            ShowAgreementsButton.Visibility = Visibility.Collapsed;

            if (ExportButton != null)
                ExportButton.Visibility = (section == "Owners" && !_isRelatedView)
                    ? Visibility.Visible : Visibility.Collapsed;

            var sections = new Dictionary<string, (string title, System.Action loader)>
            {
                { "ServiceCenters",      ("Автосервисы",           LoadServiceCenters) },
                { "Owners",              ("Владельцы",              LoadOwners) },
                { "Manufacturers",       ("Заводы производители",   LoadManufacturers) },
                { "Automobiles",         ("Автомобили",             LoadAutomobiles) },
                { "PassengerCapacities", ("Вместимость автомобиля", LoadPassengerCapacities) },
                { "Agreements",          ("Договора",               LoadAgreements) },
                { "Users",               ("Пользователи",           LoadUsers) }
            };

            if (sections.TryGetValue(section, out var info))
            {
                SectionTitle.Text = info.title;
                info.loader();
            }
        }


        private void LoadServiceCenters()
        {
            CardsListBox.SelectionChanged -= ServiceCenters_SelectionChanged;
            CardsListBox.SelectionChanged -= Owners_SelectionChanged;

            CardsListBox.ItemTemplate = BuildTemplate(
                new[] { ("CompanyName", true), ("NameOfTheCenter", true) },
                " | ",
                new[] { ("Город", "City"), ("Улица", "Street"), ("Дом", "House") },
                ("Мест: ", "NumberOfSeats"),
                null
            );
            var list = _serviceCenterService.GetAll();
            _currentFullList = list;
            CardsListBox.ItemsSource = list;
            CardsListBox.SelectionChanged += ServiceCenters_SelectionChanged;
        }

        private void ServiceCenters_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CardsListBox.SelectedItem is ServiceCenter selected)
            {
                ShowAgreementsButton.Visibility = Visibility.Visible;
                ShowAgreementsButton.Tag = selected;
            }
            else
            {
                ShowAgreementsButton.Visibility = Visibility.Collapsed;
            }
        }

        private void LoadOwners()
        {
            CardsListBox.SelectionChanged -= ServiceCenters_SelectionChanged;
            CardsListBox.SelectionChanged -= Owners_SelectionChanged;

            CardsListBox.ItemTemplate = BuildOwnerTemplate();
            var list = _ownersService.GetAll();
            _currentFullList = list;
            CardsListBox.ItemsSource = list;
            CardsListBox.SelectionChanged += Owners_SelectionChanged;
        }

        private void Owners_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CardsListBox.SelectedItem is Owner selected)
            {
                ShowAgreementsButton.Visibility = Visibility.Visible;
                ShowAgreementsButton.Tag = selected;
            }
            else
            {
                ShowAgreementsButton.Visibility = Visibility.Collapsed;
            }
        }

        private void LoadManufacturers()
        {
            CardsListBox.SelectionChanged -= ServiceCenters_SelectionChanged;
            CardsListBox.SelectionChanged -= Owners_SelectionChanged;

            CardsListBox.ItemTemplate = BuildTemplate(
                new[] { ("Name", true) },
                "",
                new[] { ("Страна", "Country"), ("Город", "City") },
                ("", "Country"),
                null
            );
            var list = _manufacturerService.GetAll();
            _currentFullList = list;
            CardsListBox.ItemsSource = list;
        }

        private void LoadAutomobiles()
        {
            CardsListBox.SelectionChanged -= ServiceCenters_SelectionChanged;
            CardsListBox.SelectionChanged -= Owners_SelectionChanged;

            CardsListBox.ItemTemplate = BuildTemplate(
                new[] { ("Series", true) },
                "",
                new[] { ("Мощность", "EnginePower"), ("Цвет", "Color") },
                ("", "BodyType"),
                null
            );
            var list = _automobileService.GetAll();
            _currentFullList = list;
            CardsListBox.ItemsSource = list;
        }

        private void LoadPassengerCapacities()
        {
            CardsListBox.SelectionChanged -= ServiceCenters_SelectionChanged;
            CardsListBox.SelectionChanged -= Owners_SelectionChanged;

            CardsListBox.ItemTemplate = BuildTemplate(
                new[] { ("IdAutomobile", false) },
                "",
                new[] { ("Пассажиров", "NumberOfPassengers") },
                ("", "NumberOfPassengers"),
                null
            );
            var list = _passengerCapacityService.GetAll();
            _currentFullList = list;
            CardsListBox.ItemsSource = list;
        }

        private void LoadAgreements()
        {
            CardsListBox.SelectionChanged -= ServiceCenters_SelectionChanged;
            CardsListBox.SelectionChanged -= Owners_SelectionChanged;

            CardsListBox.ItemTemplate = BuildTemplate(
                new[] { ("TypeOfService", true) },
                "",
                new[] { ("Владелец ID", "IdOwner"), ("Центр ID", "IdServiceCenter") },
                ("Гарантия: ", "GuaranteeUpTo"),
                null
            );
            var list = _agreementService.GetAll();
            _currentFullList = list;
            CardsListBox.ItemsSource = list;
        }

        private void LoadUsers()
        {
            CardsListBox.SelectionChanged -= ServiceCenters_SelectionChanged;
            CardsListBox.SelectionChanged -= Owners_SelectionChanged;

            CardsListBox.ItemTemplate = BuildTemplate(
                new[] { ("Login", true) },
                "",
                new[] { ("Роль", "Role") },
                ("", "Role"),
                null
            );
            var list = _userService.GetAll();
            _currentFullList = list;
            CardsListBox.ItemsSource = list;
        }


        private void LoadOwnerAutomobiles(Owner owner)
        {
            _isRelatedView = true;
            _parentSection = "Owners";
            _parentItem = owner;

            ResetSearch();
            ParentInfo.Text = $"Владелец: {owner.LastName} {owner.FirstName}";
            SectionTitle.Text = "Автомобили владельца";
            BackButton.Visibility = Visibility.Visible;
            ActionButtons.Visibility = Visibility.Collapsed;
            ShowAgreementsButton.Visibility = Visibility.Collapsed;

            using var db = new AppDbContext();
            var automobiles = db.OwnerAutomobiles
                .Where(oa => oa.IdOwner == owner.IdOwner)
                .Join(db.Automobiles,
                    oa => oa.IdAutomobile,
                    a => a.IdAutomobile,
                    (oa, a) => a)
                .ToList();

            _currentFullList = automobiles;
            CardsListBox.ItemTemplate = BuildTemplate(
                new[] { ("Series", true) },
                "",
                new[] { ("Мощность", "EnginePower"), ("Цвет", "Color") },
                ("", "BodyType"),
                null
            );
            CardsListBox.ItemsSource = automobiles;
        }

        private void LoadServiceCenterAgreements(ServiceCenter serviceCenter)
        {
            _isRelatedView = true;
            _parentSection = "ServiceCenters";
            _parentItem = serviceCenter;

            ResetSearch();
            ParentInfo.Text = $"Сервис: {serviceCenter.NameOfTheCenter}";
            SectionTitle.Text = "Договоры сервиса";
            BackButton.Visibility = Visibility.Visible;
            ActionButtons.Visibility = Visibility.Collapsed;
            ShowAgreementsButton.Visibility = Visibility.Collapsed;
            
            using var db = new AppDbContext();
            var agreements = db.Agreements
                .Where(a => a.IdServiceCenter == serviceCenter.IdServiceCenter)
                .ToList();

            _currentFullList = agreements;
            CardsListBox.ItemTemplate = BuildTemplate(
                new[] { ("TypeOfService", true) },
                "",
                new[] { ("Владелец ID", "IdOwner"), ("Дата", "DateOfService") },
                ("Гарантия: ", "GuaranteeUpTo"),
                null
            );
            CardsListBox.ItemsSource = agreements;
        }

        private void LoadOwnerAgreements(Owner owner)
        {
            _isRelatedView = true;
            _parentSection = "Owners";
            _parentItem = owner;

            ResetSearch();
            ParentInfo.Text = $"Владелец: {owner.LastName} {owner.FirstName}";
            SectionTitle.Text = "Договоры владельца";
            BackButton.Visibility = Visibility.Visible;
            ActionButtons.Visibility = Visibility.Collapsed;
            ShowAgreementsButton.Visibility = Visibility.Collapsed;

            using var db = new AppDbContext();
            var agreements = db.Agreements
                .Where(a => a.IdOwner == owner.IdOwner)
                .ToList();

            _currentFullList = agreements;
            CardsListBox.ItemTemplate = BuildTemplate(
                new[] { ("TypeOfService", true) },
                "",
                new[] { ("Сервис ID", "IdServiceCenter"), ("Дата", "DateOfService") },
                ("Гарантия: ", "GuaranteeUpTo"),
                null
            );
            CardsListBox.ItemsSource = agreements;
        }

        private void ShowAgreements_Click(object sender, RoutedEventArgs e)
        {
            if (ShowAgreementsButton.Tag is ServiceCenter sc)
                LoadServiceCenterAgreements(sc);
            else if (ShowAgreementsButton.Tag is Owner ow)
                LoadOwnerAgreements(ow);
        }


        private DataTemplate BuildOwnerTemplate()
        {
            var template = new DataTemplate();
            var stack = new FrameworkElementFactory(typeof(StackPanel));

            var topGrid = new FrameworkElementFactory(typeof(Grid));
            var col1 = new FrameworkElementFactory(typeof(ColumnDefinition));
            col1.SetValue(ColumnDefinition.WidthProperty, new GridLength(1, GridUnitType.Star));
            var col2 = new FrameworkElementFactory(typeof(ColumnDefinition));
            col2.SetValue(ColumnDefinition.WidthProperty, GridLength.Auto);
            topGrid.AppendChild(col1);
            topGrid.AppendChild(col2);

            var leftStack = new FrameworkElementFactory(typeof(StackPanel));
            leftStack.SetValue(Grid.ColumnProperty, 0);

            var titleTb = CreateTextBlock(14, FontWeights.SemiBold, "#222222");
            var titleMb = new MultiBinding();
            titleMb.Bindings.Add(new Binding("LastName"));
            titleMb.Bindings.Add(new Binding("FirstName"));
            titleMb.Bindings.Add(new Binding("Surname"));
            titleMb.StringFormat = "{0} {1} {2}";
            titleTb.SetBinding(TextBlock.TextProperty, titleMb);
            leftStack.AppendChild(titleTb);

            var phoneTb = CreateTextBlock(12, FontWeights.Normal, "#666666");
            phoneTb.SetValue(TextBlock.MarginProperty, new Thickness(0, 2, 0, 0));
            var phoneMb = new MultiBinding { StringFormat = "Телефон: {0}" };
            phoneMb.Bindings.Add(new Binding("PhoneNumber"));
            phoneTb.SetBinding(TextBlock.TextProperty, phoneMb);
            leftStack.AppendChild(phoneTb);
            topGrid.AppendChild(leftStack);

            var badgeTb = CreateTextBlock(13, FontWeights.Bold, "#444444");
            badgeTb.SetValue(Grid.ColumnProperty, 1);
            badgeTb.SetValue(TextBlock.VerticalAlignmentProperty, VerticalAlignment.Center);
            var badgeMb = new MultiBinding { StringFormat = "ВУ: {0}" };
            badgeMb.Bindings.Add(new Binding("DriversLicenseNumber"));
            badgeTb.SetBinding(TextBlock.TextProperty, badgeMb);
            topGrid.AppendChild(badgeTb);
            stack.AppendChild(topGrid);

            var autoBorder = new FrameworkElementFactory(typeof(Border));
            autoBorder.SetValue(Border.MarginProperty, new Thickness(0, 8, 0, 0));
            autoBorder.SetValue(Border.PaddingProperty, new Thickness(8));
            autoBorder.SetValue(Border.BackgroundProperty, new SolidColorBrush(Color.FromRgb(245, 250, 248)));
            autoBorder.SetValue(Border.CornerRadiusProperty, new CornerRadius(6));
            autoBorder.SetValue(Border.VisibilityProperty, Visibility.Collapsed);
            autoBorder.SetValue(Border.NameProperty, "AutoPanel");

            var borderContentStack = new FrameworkElementFactory(typeof(StackPanel));

            var autoHeader = new FrameworkElementFactory(typeof(TextBlock));
            autoHeader.SetValue(TextBlock.TextProperty, "Автомобили владельца:");
            autoHeader.SetValue(TextBlock.FontSizeProperty, 11.0);
            autoHeader.SetValue(TextBlock.FontWeightProperty, FontWeights.SemiBold);
            autoHeader.SetValue(TextBlock.ForegroundProperty, new SolidColorBrush(Color.FromRgb(68, 68, 68)));
            borderContentStack.AppendChild(autoHeader);

            var autoList = new FrameworkElementFactory(typeof(ListBox));
            autoList.SetValue(ListBox.MarginProperty, new Thickness(0, 4, 0, 0));
            autoList.SetValue(ListBox.BackgroundProperty, Brushes.Transparent);
            autoList.SetValue(ListBox.BorderThicknessProperty, new Thickness(0));
            autoList.SetValue(ListBox.NameProperty, "AutoListBox");
            autoList.SetValue(ListBox.ItemTemplateProperty, BuildSimpleAutoTemplate());
            borderContentStack.AppendChild(autoList);

            autoBorder.AppendChild(borderContentStack);
            stack.AppendChild(autoBorder);

            var toggleBtn = new FrameworkElementFactory(typeof(TextBlock));
            toggleBtn.SetValue(TextBlock.TextProperty, "Показать автомобили...");
            toggleBtn.SetValue(TextBlock.FontSizeProperty, 11.0);
            toggleBtn.SetValue(TextBlock.ForegroundProperty, new SolidColorBrush(Color.FromRgb(200, 148, 154)));
            toggleBtn.SetValue(TextBlock.MarginProperty, new Thickness(0, 4, 0, 0));
            toggleBtn.SetValue(TextBlock.CursorProperty, System.Windows.Input.Cursors.Hand);
            toggleBtn.AddHandler(TextBlock.MouseLeftButtonDownEvent,
                new System.Windows.Input.MouseButtonEventHandler(ToggleOwnerAutos));
            stack.AppendChild(toggleBtn);

            template.VisualTree = stack;
            return template;
        }

        private DataTemplate BuildSimpleAutoTemplate()
        {
            var template = new DataTemplate();
            var tb = new FrameworkElementFactory(typeof(TextBlock));
            tb.SetValue(TextBlock.FontSizeProperty, 11.0);
            tb.SetValue(TextBlock.ForegroundProperty, new SolidColorBrush(Color.FromRgb(85, 85, 85)));
            tb.SetValue(TextBlock.MarginProperty, new Thickness(0, 2, 0, 2));
            var mb = new MultiBinding { StringFormat = "• {0} ({1}) - {2}" };
            mb.Bindings.Add(new Binding("Series"));
            mb.Bindings.Add(new Binding("BodyType"));
            mb.Bindings.Add(new Binding("Color"));
            tb.SetBinding(TextBlock.TextProperty, mb);
            template.VisualTree = tb;
            return template;
        }

        private void ToggleOwnerAutos(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (sender is TextBlock toggle && toggle.Parent is StackPanel parent)
            {
                Border autoBorder = null;
                ListBox autoList = null;

                foreach (var child in LogicalTreeHelper.GetChildren(parent))
                    if (child is Border b && b.Name == "AutoPanel")
                        autoBorder = b;

                if (autoBorder != null)
                {
                    autoList = FindChild<ListBox>(autoBorder, "AutoListBox");

                    if (autoBorder.Visibility == Visibility.Collapsed)
                    {
                        if (parent.DataContext is Owner owner)
                        {
                            using var db = new AppDbContext();
                            var autos = db.OwnerAutomobiles
                                .Where(oa => oa.IdOwner == owner.IdOwner)
                                .Join(db.Automobiles,
                                    oa => oa.IdAutomobile,
                                    a => a.IdAutomobile,
                                    (oa, a) => a)
                                .ToList();
                            autoList.ItemsSource = autos;
                        }
                        autoBorder.Visibility = Visibility.Visible;
                        toggle.Text = "Свернуть";
                    }
                    else
                    {
                        autoBorder.Visibility = Visibility.Collapsed;
                        toggle.Text = "Показать автомобили...";
                    }
                }
            }
            e.Handled = true;
        }

        private T FindChild<T>(DependencyObject parent, string name) where T : DependencyObject
        {
            if (parent == null) return null;
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is T typed && (child as FrameworkElement)?.Name == name)
                    return typed;
                var result = FindChild<T>(child, name);
                if (result != null) return result;
            }
            return null;
        }

        private DataTemplate BuildTemplate(
            (string prop, bool isText)[] titleParts,
            string titleSeparator,
            (string label, string prop)[] summaryLines,
            (string prefix, string prop) badge,
            (string label, string prop)[] detailLines)
        {
            var template = new DataTemplate(
                );
            var stack = new FrameworkElementFactory(typeof(StackPanel));

            var topGrid = new FrameworkElementFactory(typeof(Grid));
            var col1 = new FrameworkElementFactory(typeof(ColumnDefinition));
            col1.SetValue(ColumnDefinition.WidthProperty, new GridLength(1, GridUnitType.Star));
            var col2 = new FrameworkElementFactory(typeof(ColumnDefinition));
            col2.SetValue(ColumnDefinition.WidthProperty, GridLength.Auto);
            topGrid.AppendChild(col1);
            topGrid.AppendChild(col2);

            var leftStack = new FrameworkElementFactory(typeof(StackPanel));
            leftStack.SetValue(Grid.ColumnProperty, 0);

            var titleTb = CreateTextBlock(14, FontWeights.SemiBold, "#222222");
            var titleMb = new MultiBinding();
            string titleFmt = "";
            int ti = 0;
            foreach (var part in titleParts)
            {
                if (ti > 0) titleFmt += titleSeparator;
                titleMb.Bindings.Add(new Binding(part.prop));
                titleFmt += "{" + ti + "}";
                ti++;
            }
            titleMb.StringFormat = titleFmt;
            titleTb.SetBinding(TextBlock.TextProperty, titleMb);
            leftStack.AppendChild(titleTb);

            foreach (var line in summaryLines)
            {
                var tb = CreateTextBlock(12, FontWeights.Normal, "#666666");
                tb.SetValue(TextBlock.MarginProperty, new Thickness(0, 2, 0, 0));
                var mb = new MultiBinding { StringFormat = line.label + ": {0}" };
                mb.Bindings.Add(new Binding(line.prop));
                tb.SetBinding(TextBlock.TextProperty, mb);
                leftStack.AppendChild(tb);
            }

            topGrid.AppendChild(leftStack);

            var badgeTb = CreateTextBlock(13, FontWeights.Bold, "#444444");
            badgeTb.SetValue(Grid.ColumnProperty, 1);
            badgeTb.SetValue(TextBlock.VerticalAlignmentProperty, VerticalAlignment.Center);
            badgeTb.SetValue(TextBlock.MarginProperty, new Thickness(20, 0, 0, 0));
            var badgeMb = new MultiBinding { StringFormat = badge.prefix + "{0}" };
            badgeMb.Bindings.Add(new Binding(badge.prop));
            badgeTb.SetBinding(TextBlock.TextProperty, badgeMb);
            topGrid.AppendChild(badgeTb);

            stack.AppendChild(topGrid);
            template.VisualTree = stack;
            return template;
        }

        private FrameworkElementFactory CreateTextBlock(double size, FontWeight weight, string color)
        {
            var tb = new FrameworkElementFactory(typeof(TextBlock));
            tb.SetValue(TextBlock.FontSizeProperty, size);
            tb.SetValue(TextBlock.FontWeightProperty, weight);
            tb.SetValue(TextBlock.ForegroundProperty,
                new BrushConverter().ConvertFromString(color) as SolidColorBrush);
            tb.SetValue(TextBlock.TextWrappingProperty, TextWrapping.Wrap);
            return tb;
        }


        private void Add_Click(object sender, RoutedEventArgs e)
        {
            Window window = _currentSection switch
            {
                "ServiceCenters" => new AddServiceCenterWindow(),
                "Owners" => new OwnerFormWindow(),
                "Manufacturers" => new ManufacturerFormWindow(),
                "Automobiles" => new AutomobileFormWindow(),
                "PassengerCapacities" => new PassengerCapacityFormWindow(),
                "Agreements" => new AgreementFormWindow(),
                "Users" => new UserFormWindow(),
                _ => null
            };
            if (window == null) return;
            window.Owner = this;
            if (window.ShowDialog() == true) ShowSection(_currentSection);
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            var selected = CardsListBox.SelectedItem;
            if (selected == null)
            {
                MessageBox.Show("Выберите запись для редактирования.",
                    "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            Window window = _currentSection switch
            {
                "ServiceCenters" => new EditServiceCenterWindow((ServiceCenter)selected),
                "Owners" => new OwnerFormWindow((Owner)selected),
                "Manufacturers" => new ManufacturerFormWindow((Manufacturer)selected),
                "Automobiles" => new AutomobileFormWindow((Automobile)selected),
                "PassengerCapacities" => new PassengerCapacityFormWindow((PassengerCapacity)selected),
                "Agreements" => new AgreementFormWindow((Agreement)selected),
                "Users" => new UserFormWindow((User)selected),
                _ => null
            };
            if (window == null) return;
            window.Owner = this;
            if (window.ShowDialog() == true) ShowSection(_currentSection);
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            var selected = CardsListBox.SelectedItem;
            if (selected == null)
            {
                MessageBox.Show("Выберите запись для удаления.",
                    "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string name = _currentSection switch
            {
                "ServiceCenters" => ((ServiceCenter)selected).NameOfTheCenter,
                "Owners" => $"{((Owner)selected).LastName} {((Owner)selected).FirstName}",
                "Manufacturers" => ((Manufacturer)selected).Name,
                "Automobiles" => ((Automobile)selected).Series,
                "PassengerCapacities" => "запись о вместимости",
                "Agreements" => ((Agreement)selected).TypeOfService,
                "Users" => ((User)selected).Login,
                _ => ""
            };

            if (MessageBox.Show($"Удалить \"{name}\"?", "Подтверждение",
                MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
                return;

            using var db = new AppDbContext();

            switch (_currentSection)
            {
                case "ServiceCenters":
                    var sc = db.ServiceCenters.FirstOrDefault(x => x.IdServiceCenter == ((ServiceCenter)selected).IdServiceCenter);
                    if (sc != null) db.ServiceCenters.Remove(sc);
                    break;
                case "Owners":
                    var ow = db.Owners.FirstOrDefault(x => x.IdOwner == ((Owner)selected).IdOwner);
                    if (ow != null) db.Owners.Remove(ow);
                    break;
                case "Manufacturers":
                    var mn = db.Manufacturers.FirstOrDefault(x => x.IdManufacturer == ((Manufacturer)selected).IdManufacturer);
                    if (mn != null) db.Manufacturers.Remove(mn);
                    break;
                case "Automobiles":
                    var au = db.Automobiles.FirstOrDefault(x => x.IdAutomobile == ((Automobile)selected).IdAutomobile);
                    if (au != null) db.Automobiles.Remove(au);
                    break;
                case "PassengerCapacities":
                    var pc = db.PassengerCapacities.FirstOrDefault(x => x.IdPassengerCapacity == ((PassengerCapacity)selected).IdPassengerCapacity);
                    if (pc != null) db.PassengerCapacities.Remove(pc);
                    break;
                case "Agreements":
                    var ag = db.Agreements.FirstOrDefault(x => x.IdAgreement == ((Agreement)selected).IdAgreement);
                    if (ag != null) db.Agreements.Remove(ag);
                    break;
                case "Users":
                    var us = db.Users.FirstOrDefault(x => x.IdUser == ((User)selected).IdUser);
                    if (us != null) db.Users.Remove(us);
                    break;
            }

            db.SaveChanges();
            ShowSection(_currentSection);
        }


        public void RoleRestriction()
        {
            if (_currentUser.Role.Trim() == "Service_Employee")
            {
                MenuUsers.Visibility = Visibility.Collapsed;
                MenuManufacturers.Visibility = Visibility.Collapsed;
                MenuPassengerCapacities.Visibility = Visibility.Collapsed;
            }
        }

        private void ExportOwnersToExcel_Click(object sender, RoutedEventArgs e)
        {
            var owners = _ownersService.GetAll();

            if (owners == null || !owners.Any())
            {
                MessageBox.Show("Нет данных для экспорта.", "Информация",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var sfd = new SaveFileDialog
            {
                Filter = "Excel Files (*.xlsx)|*.xlsx",
                FileName = $"Список_Владельцев_{DateTime.Now:yyyy-MM-dd}",
                DefaultExt = ".xlsx"
            };

            if (sfd.ShowDialog() == true)
            {
                try
                {
                    using (var workbook = new XLWorkbook())
                    {
                        var ws = workbook.Worksheets.Add("Владельцы");

                        string[] headers = { "Фамилия", "Имя", "Отчество", "Телефон", "Серия ВУ", "Номер ВУ" };
                        for (int i = 0; i < headers.Length; i++)
                        {
                            var cell = ws.Cell(1, i + 1);
                            cell.Value = headers[i];
                            cell.Style.Font.Bold = true;
                            cell.Style.Fill.BackgroundColor = XLColor.FromHtml("#E0A9AF");
                            cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        }

                        int row = 2;
                        foreach (var owner in owners)
                        {
                            ws.Cell(row, 1).Value = owner.LastName;
                            ws.Cell(row, 2).Value = owner.FirstName;
                            ws.Cell(row, 3).Value = owner.Surname ?? "";
                            ws.Cell(row, 4).Value = owner.PhoneNumber;
                            ws.Cell(row, 5).Value = owner.DriversLicenseSeries;
                            ws.Cell(row, 6).Value = owner.DriversLicenseNumber;
                            row++;
                        }

                        ws.Columns().AdjustToContents();
                        workbook.SaveAs(sfd.FileName);
                    }

                    if (MessageBox.Show("Экспорт завершен! Открыть файл?", "Успех",
                        MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        Process.Start(new ProcessStartInfo(sfd.FileName) { UseShellExecute = true });
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при сохранении: {ex.Message}", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            LoginWindow loginWindow = new LoginWindow();
            loginWindow.Show();
            this.Close();
        }
    }
}