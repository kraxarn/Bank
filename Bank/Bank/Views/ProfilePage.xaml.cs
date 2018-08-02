﻿using System;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Bank.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ProfilePage
	{
		private readonly ImageSource[] avatars;
		private int selectedIndex;

		private double geastureStart;

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
				avatars[i] = ImageSource.FromFile($"images/avatar/{avatarNames[i]}.png");

			selectedIndex = (int) Tools.GetProperty("avatar", 0);

			ImageAvatar.Source = avatars[selectedIndex];

			EntryName.Text = Tools.GetProperty("name", "NAME") as string;

			// Fix button sizes on Android
			if (Device.RuntimePlatform == Device.Android)
				ButtonPrevious.WidthRequest = ButtonNext.WidthRequest = 64;
		}

		private void ButtonPrevious_OnClicked(object sender, EventArgs e)
			=> PreviousAvatar();

		private void ButtonNext_OnClicked(object sender, EventArgs e)
			=> NextAvatar();

		private void PreviousAvatar()
		{
			if (selectedIndex > 0)
				ImageAvatar.Source = avatars[--selectedIndex];
		}

		private void NextAvatar()
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
				await DisplayAlert("Changes not saved", "Name cannot be longer than 16 characters", "OK");
				return;
			}

			Tools.SetProperty("name", EntryName.Text);
			Tools.SetProperty("avatar", selectedIndex);
			await Tools.SavePropertiesAsync();

			base.OnDisappearing();
		}

		private void PanUpdated(object sender, PanUpdatedEventArgs e)
		{
			switch (e.StatusType)
			{
				case GestureStatus.Started:
					geastureStart = 0;
					break;

				case GestureStatus.Running:
					geastureStart += e.TotalX;
					break;

				case GestureStatus.Completed when e.TotalX > geastureStart:
					NextAvatar();
					break;

				case GestureStatus.Completed:
					PreviousAvatar();
					break;
			}
		}

		private void EntryName_OnPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (!string.IsNullOrEmpty(EntryName.Text) && EntryName.Text.Length > 16)
				EntryName.Text = EntryName.Text.Substring(0, 16);
		}
	}
}