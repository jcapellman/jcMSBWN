namespace jcMSBWN.UWP.Helpers {
    public class SettingsHelper {
        private readonly Windows.Storage.ApplicationDataContainer _localSettings = Windows.Storage.ApplicationData.Current.RoamingSettings;

        public T GetSetting<T>(string setting) {
            if (!_localSettings.Values.ContainsKey(setting)) {
                return default(T);
            }

            return (T)_localSettings.Values[setting.ToString()];
        }

        public void WriteSetting<T>(string setting, T value) {
            _localSettings.Values[setting] = value;
        }
    }
}