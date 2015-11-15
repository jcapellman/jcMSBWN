using jcMSBWN.UWP.Enums;

namespace jcMSBWN.UWP.Helpers {
    public class SettingsHelper {
        private readonly Windows.Storage.ApplicationDataContainer _localSettings = Windows.Storage.ApplicationData.Current.RoamingSettings;

        public T GetSetting<T>(SETTINGS setting) {
            if (!_localSettings.Values.ContainsKey(setting.ToString())) {
                return default(T);
            }

            return (T)_localSettings.Values[setting.ToString()];
        }

        public void WriteSetting<T>(SETTINGS setting, T value) {
            _localSettings.Values[setting.ToString()] = value;
        }
    }
}