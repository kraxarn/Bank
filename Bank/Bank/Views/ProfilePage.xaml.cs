using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Bank.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ProfilePage : ContentPage
	{
		private readonly ImageSource[] avatars;
		private int selectedIndex;

		public ProfilePage()
		{
			InitializeComponent();

			var avatarNames = new[]
			{
				"airplane",
				"boat",
				"burger",
				"car",
				"cat",
				"dog",
				"duck",
				"hat",
				"penguin",
				"phone",
				"shoe"
			};

			avatars = new ImageSource[avatarNames.Length];
			for (var i = 0; i < avatarNames.Length; i++)
				avatars[i] = ImageSource.FromFile($"Images/Avatar/{avatarNames[i]}.png");

			selectedIndex = (int) Application.Current.Properties["avatar"];

			ImageAvatar.Source = avatars[selectedIndex];

			EntryName.Text = Application.Current.Properties["name"] as string;
		}

		private void ButtonPrevious_OnClicked(object sender, EventArgs e)
		{
			if (selectedIndex > 0)
				ImageAvatar.Source = avatars[--selectedIndex];
		}

		private void ButtonNext_OnClicked(object sender, EventArgs e)
		{
			if (selectedIndex < avatars.Length - 1)
				ImageAvatar.Source = avatars[++selectedIndex];
		}

		protected override async void OnDisappearing()
		{
			var nameLength = EntryName.Text?.Length ?? 0;

			if (nameLength < 3)
			{
				await DisplayAlert("Changes not saved", "Name must be at least 3 characters", "OK");
				return;
			}

			if (nameLength > 16)
			{
				await DisplayAlert("Changed not saved", "Name cannot be longer than 16 characters", "OK");
				return;
			}

			Application.Current.Properties["name"]   = EntryName.Text;
			Application.Current.Properties["avatar"] = selectedIndex;
			await Application.Current.SavePropertiesAsync();

			base.OnDisappearing();
		}
	}
}