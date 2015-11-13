using Windows.UI.Xaml.Controls;
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
            await viewModel.ScanNetworks();
        }
    }
}