using Android.App;
using Android.Content;
using Android.Preferences;
using Bank.Droid;

[assembly: Xamarin.Forms.Dependency(typeof(Properties))]
namespace Bank.Droid
{
	public class Properties : IProperties
	{
		private static Activity _activity;

		public static void SetContext(Activity activity) => _activity = activity;

		private static ISharedPreferences Prefs => PreferenceManager.GetDefaultSharedPreferences(_activity);

		public object GetProperty(string key, object fallback) 
			=> Prefs.GetString(key, $"{fallback}");

		public void SetProperty(string key, object value)
		{
			var editor = Prefs.Edit();
			editor.PutString(key, $"{value}");
			editor.Apply();
		}
	}
}