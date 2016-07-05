using System;
using Template10.Common;
using Template10.Utils;
using Windows.UI.Xaml;

namespace MyML.UWP.Services.SettingsServices
{
    public class SettingsService : ISettingsService
    {
        public static SettingsService Instance { get; }
        static SettingsService()
        {
            // implement singleton pattern
            Instance = Instance ?? new SettingsService();
        }

        Template10.Services.SettingsService.ISettingsHelper _helper;
        private SettingsService()
        {
            _helper = new Template10.Services.SettingsService.SettingsHelper();
        }

        public bool UseShellBackButton
        {
            get { return _helper.Read<bool>(nameof(UseShellBackButton), true); }
            set
            {
                _helper.Write(nameof(UseShellBackButton), value);
                BootStrapper.Current.NavigationService.Dispatcher.Dispatch(() =>
                {
                    BootStrapper.Current.ShowShellBackButton = value;
                    BootStrapper.Current.UpdateShellBackButton();
                    BootStrapper.Current.NavigationService.Refresh();
                });
            }
        }
        

        public bool? UseHighQualityImages
        {
            get { return _helper.Read<bool?>(nameof(UseHighQualityImages), false); }
            set
            {
                _helper.Write(nameof(UseHighQualityImages), value);                
            }
        }


        public ApplicationTheme AppTheme
        {
            get
            {
                var theme = ApplicationTheme.Light;
                var value = _helper.Read<string>(nameof(AppTheme), theme.ToString());
                return Enum.TryParse<ApplicationTheme>(value, out theme) ? theme : ApplicationTheme.Dark;
            }
            set
            {
                _helper.Write(nameof(AppTheme), value.ToString());
                BootStrapper.Current.NavigationService.Frame.RequestedTheme = value.ToElementTheme();
            }
        }

        public TimeSpan CacheMaxDuration
        {
            get { return _helper.Read<TimeSpan>(nameof(CacheMaxDuration), TimeSpan.FromDays(2)); }
            set
            {
                _helper.Write(nameof(CacheMaxDuration), value);
                BootStrapper.Current.CacheMaxDuration = value;
            }
        }

        private bool _IsNotificationSigned;

        public bool IsNotificationSigned
        {
            get { return _helper.Read<bool>(nameof(IsNotificationSigned), false); }
            set
            {
                _helper.Write(nameof(IsNotificationSigned), value);                
            }
        }

    }
}

