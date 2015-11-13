using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

using Windows.Devices.WiFi;

namespace jcMSBWN.UWP.ViewModels {
    public class MainModel : BaseModel {
        private WiFiAdapter _adapter;
        
        private ObservableCollection<WiFiAvailableNetwork> _networks;

        public ObservableCollection<WiFiAvailableNetwork> Networks {
            get { return _networks; }
            set { _networks = value; OnPropertyChanged(); }
        }

        public MainModel() {
            Networks = new ObservableCollection<WiFiAvailableNetwork>();
        }

        public async Task<bool> ScanNetworks() {
            var access = await WiFiAdapter.RequestAccessAsync();

            if (access != WiFiAccessStatus.Allowed) {
                return false;
            }

            var result = await Windows.Devices.Enumeration.DeviceInformation.FindAllAsync(WiFiAdapter.GetDeviceSelector());

            if (result.Count > 0) {
                _adapter = await WiFiAdapter.FromIdAsync(result[0].Id);
            }

            await _adapter.ScanAsync();

            foreach (var network in _adapter.NetworkReport.AvailableNetworks)  {
                Networks.Add(network);
            }

            return true;
        }
    }
}