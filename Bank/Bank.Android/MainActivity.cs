using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Preferences;
using Xamarin.Forms;

namespace Bank.Droid
{
    [Activity(Label = "Bank", Icon = "@drawable/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
	        var prefs = PreferenceManager.GetDefaultSharedPreferences(this);

			if (prefs.GetString("theme", "light") == "dark")
				SetTheme(Resource.Style.MainThemeDark);

			TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(bundle);

			Forms.Init(this, bundle);
			DeviceInfo.SetContext(this);
			Properties.SetContext(this);
            LoadApplication(new App());
		}
    }
}

