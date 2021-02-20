using System.Windows.Controls;
using System.Windows.Input;

namespace AC.Pages
{
    /// <summary>
    /// Interaction logic for SettingsPage.xaml
    /// </summary>
    public partial class SettingsPage : Page
    {
        public SettingsPage()
        {
            InitializeComponent();
            SettingsPageViewModel vm = new SettingsPageViewModel(this);
            this.DataContext = vm;
        }

        private void TextBoxSelectAll(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            (sender as TextBox).SelectAll();
        }

        private void Page_KeyDown(object sender, KeyEventArgs e)
        {
            SettingsPageViewModel vm = this.DataContext as SettingsPageViewModel;
            if (vm == null) return;

            switch (e.Key)
            {
                case Key.F1:
                    vm.SetInputCommand.Execute(true);
                    break;
                case Key.F2:
                    vm.SetInput2Command.Execute(true);
                    break;
                case Key.F5:
                    vm.AutoClickCommand.Execute(true);
                    break;
                default:
                    break;
            }
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SettingsPageViewModel vm = this.DataContext as SettingsPageViewModel;
            if (vm == null) return;

            var selected = (sender as ComboBox).SelectedIndex;
            vm.ChangeClickTypeCommand.Execute(selected);
        }
    }
}
