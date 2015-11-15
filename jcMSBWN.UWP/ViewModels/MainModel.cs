using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Band;
using Microsoft.Band.Tiles;

using Windows.Devices.WiFi;
using Windows.UI.Xaml.Media.Imaging;

using jcMSBWN.UWP.Enums;
using jcMSBWN.UWP.Helpers;

namespace jcMSBWN.UWP.ViewModels {
    public class MainModel : BaseModel {
        readonly SettingsHelper _setting = new SettingsHelper();

        private WiFiAdapter _adapter;
        
        private ObservableCollection<WiFiAvailableNetwork> _networks;

        public ObservableCollection<WiFiAvailableNetwork> Networks {
            get { return _networks; }
            set { _networks = value; OnPropertyChanged(); }
        }

        private List<string> _selectedNetworks;

        public List<string> SelectedNetworks {
            get { return _selectedNetworks; }
            set { _selectedNetworks = value; OnPropertyChanged(); }
        }

        public MainModel() {
            Networks = new ObservableCollection<WiFiAvailableNetwork>();
            SelectedNetworks = new List<string>();
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
                
                _setting.WriteSetting(SETTINGS.TILE_ID, tile.TileId);
            }

            return true;
        }

        public bool SaveNetworks() {
            _setting.WriteSetting(SETTINGS.NETWORKS, SelectedNetworks);

            return true;
        }

        private async Task<bool> SendMessage(string title, string body) {
            var setting = new SettingsHelper();

            var pairedBands = await BandClientManager.Instance.GetBandsAsync();

            using (var bandClient = await BandClientManager.Instance.ConnectAsync(pairedBands[0])) {
                await bandClient.NotificationManager.ShowDialogAsync(setting.GetSetting<Guid>(SETTINGS.TILE_ID), title, body);
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

            SelectedNetworks = _setting.GetSetting<List<string>>(SETTINGS.NETWORKS) ?? new List<string>();

            return true;
        }
    }
}