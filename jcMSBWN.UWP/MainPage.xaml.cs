using System.Linq;
using Windows.Devices.WiFi;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Navigation;

using jcMSBWN.UWP.ViewModels;

namespace jcMSBWN.UWP {
    public sealed partial class MainPage : Page {
        private MainModel viewModel => (MainModel) DataContext;

        public MainPage() {
            this.InitializeComponent();

            DataContext = new MainModel();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e) {
            var result = await viewModel.ScanNetworks();

            if (!result || !viewModel.SelectedNetworks.Any()) {
                return;
            }

            for (var x = 0; x < viewModel.Networks.Count; x++) {
                if (viewModel.SelectedNetworks.Contains(viewModel.Networks[x].Ssid)) {
                    lstViewNetworks.SelectRange(new ItemIndexRange(x, 1));
                }
            }
        }

        private void AbbSetting_OnClick(object sender, RoutedEventArgs e) {
            pMain.IsOpen = true;
        }

        private async void BtnRegister_OnClick(object sender, RoutedEventArgs e) {
            await viewModel.RegisterTile();
        }

        private void BtnClose_OnClick(object sender, RoutedEventArgs e) {
            pMain.IsOpen = false;
        }

        private void btnSave_OnClick(object sender, RoutedEventArgs e) {
            foreach (var item in lstViewNetworks.SelectedItems) {
                var tItem = (WiFiAvailableNetwork) item;

                viewModel.SelectedNetworks.Add(tItem.Ssid);
            }

            viewModel.SaveNetworks();
        }
    }
}