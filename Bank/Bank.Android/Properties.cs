using System;
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
		{
			switch (fallback)
			{
				case string _:
					return Prefs.GetString(key, (string) fallback);

				case int _:
					return Prefs.GetInt(key, (int) fallback);

				case float _:
					return Prefs.GetFloat(key, (float) fallback);

				case bool _:
					return Prefs.GetBoolean(key, (bool) fallback);

				default:
					throw new InvalidOperationException($"Invalid type: {fallback.GetType()}");
			}
		}

		public void SetProperty(string key, object value)
		{
			var editor = Prefs.Edit();

			switch (value)
			{
				case string _:
					editor.PutString(key, (string) value);
					break;

				case int _:
					editor.PutInt(key, (int) value);
					break;

				case float _:
					editor.PutFloat(key, (float) value);
					break;

				case bool _:
					editor.PutBoolean(key, (bool) value);
					break;

				default:
					throw new InvalidOperationException($"Invalid type: {value.GetType()}");
			}

			editor.Apply();
		}
	}
}