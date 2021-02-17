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
            if (e.Key == Key.F1 || e.Key == Key.F2 || e.Key == Key.F5)
            {
                SettingsPageViewModel vm = this.DataContext as SettingsPageViewModel;
                
                if (e.Key == Key.F1)
                {
                    vm.InputX = vm.CurrentX;
                    vm.InputY = vm.CurrentY;
                }
                else if (e.Key == Key.F2)
                {
                    vm.InputX2 = vm.CurrentX;
                    vm.InputY2 = vm.CurrentY;
                }
                else // F5
                {
                    vm.AutoClickCommand.Execute(true);
                }
            }
        }
    }
}
