using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Band;
using Microsoft.Band.Tiles;

using Windows.Devices.WiFi;
using Windows.UI.Xaml.Media.Imaging;
using jcMSBWN.UWP.Helpers;

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

        public async Task<bool> RegisterTile() {
            var pairedBands = await BandClientManager.Instance.GetBandsAsync();

            using (var bandClient = await BandClientManager.Instance.ConnectAsync(pairedBands[0])) {
                var tiles = await bandClient.TileManager.GetTilesAsync();

                var tileCapacity = await bandClient.TileManager.GetRemainingTileCapacityAsync();

                var tileIconBitmap = new WriteableBitmap(46, 46);
                
                var tileIcon = tileIconBitmap.ToBandIcon();

                var tileGuid = Guid.NewGuid();

                var tile = new BandTile(tileGuid) { 
                    IsBadgingEnabled = true,   
                    Name = "jcMSBWN",     
                    TileIcon = tileIcon
                };

                await bandClient.TileManager.AddTileAsync(tile);

                var setting = new SettingsHelper();

                setting.WriteSetting("TileID", tile.TileId);
            }

            return true;
        }

        private async Task<bool> SendMessage(string title, string body) {
            var setting = new SettingsHelper();

            var pairedBands = await BandClientManager.Instance.GetBandsAsync();

            using (var bandClient = await BandClientManager.Instance.ConnectAsync(pairedBands[0])) {
                await bandClient.NotificationManager.ShowDialogAsync(setting.GetSetting<Guid>("TileID"), title, body);
            }
        
            return true;
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

            Networks = new ObservableCollection<WiFiAvailableNetwork>(Networks.OrderByDescending(a => a.SignalBars));
            return true;
        }
    }
}