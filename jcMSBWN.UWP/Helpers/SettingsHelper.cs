using Windows.Storage;
using jcMSBWN.UWP.Enums;
using Newtonsoft.Json;

namespace jcMSBWN.UWP.Helpers {
    public class SettingsHelper {
        private readonly ApplicationDataContainerSettings _appLocalSettings;

        public SettingsHelper() {
            var _localSettings = Windows.Storage.ApplicationData.Current.RoamingSettings;

            _appLocalSettings = (ApplicationDataContainerSettings)_localSettings.Values;
        }
        
        public T GetSetting<T>(SETTINGS setting) {
            if (!_appLocalSettings.ContainsKey(setting.ToString())) {
                return default(T);
            }

            return JsonConvert.DeserializeObject<T>((string) _appLocalSettings[setting.ToString()]);
        }

        public void WriteSetting<T>(SETTINGS setting, T value) {
            if (!_appLocalSettings.ContainsKey(setting.ToString())) {
                _appLocalSettings.Add(setting.ToString(), JsonConvert.SerializeObject(value));
            }

            _appLocalSettings[setting.ToString()] = JsonConvert.SerializeObject(value);
        }

        public void RemoveSetting(SETTINGS setting) {
            _appLocalSettings.Remove(setting.ToString());
        }
    }
}