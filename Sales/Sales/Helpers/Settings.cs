namespace Sales.Helpers
{
    using Plugin.Settings;
    using Plugin.Settings.Abstractions;
    public static class Settings
    {
        private static ISettings AppSettings
        {
            get
            {
                return CrossSettings.Current;
            }
        }

        #region Setting Constants

        private const string tonkenType = "TonkenType";
        private const string accesToken = "AccesToken";
        private const string isRemembered = "IsRemembered";
        private static readonly string strinDefault = string.Empty;
        private static readonly bool booleanDefault = false;

        #endregion


        public static string TonkenType
        {
            get
            {
                return AppSettings.GetValueOrDefault(tonkenType, strinDefault);
            }
            set
            {
                AppSettings.AddOrUpdateValue(tonkenType, value);
            }
        }

        public static string AccesToken
        {
            get
            {
                return AppSettings.GetValueOrDefault(accesToken, strinDefault);
            }
            set
            {
                AppSettings.AddOrUpdateValue(accesToken, value);
            }
        }

        public static bool IsRemembered
        {
            get
            {
                return AppSettings.GetValueOrDefault(isRemembered, booleanDefault);
            }
            set
            {
                AppSettings.AddOrUpdateValue(isRemembered, value);
            }
        }
    }
}
