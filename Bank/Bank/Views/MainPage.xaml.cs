using System;
using Xamarin.Forms;

namespace Bank.Views
{
	public partial class MainPage : ContentPage
	{
		private readonly ImageSource[] avatars;
		private int selectedIndex;

		public MainPage()
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
				
			selectedIndex = new Random().Next(avatarNames.Length);

			ImageAvatar.Source = avatars[selectedIndex];
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

		private void ButtonSave_OnClicked(object sender, EventArgs e)
		{
			var nameLength = EntryName.Text?.Length ?? 0;

			if (nameLength < 3)
			{
				DisplayAlert("Invalid Name", "Name must be at least 3 characters", "OK");
				return;
			}

			if (nameLength > 16)
			{
				DisplayAlert("Invalid Name", "Name cannot be longer than 16 characters", "OK");
				return;
			}
		}
	}
}
