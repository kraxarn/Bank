using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Bank.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class JoinPage
	{
		private Client client;
		private readonly string addressPrefix;

		public JoinPage()
		{
			InitializeComponent();

			var ip = Tools.IPAddress;
			addressPrefix = ip.Substring(0, ip.LastIndexOf('.') + 1);
		}

		private void EntryRoom_OnTextChanged(object sender, TextChangedEventArgs e)
		{
			if (int.TryParse(EntryRoom.Text, out var id))
				ButtonConnect.IsEnabled = id > 0 && id < 256;
			else
				ButtonConnect.IsEnabled = false;
		}

		private async void ButtonConnect_OnClicked(object sender, EventArgs e)
		{
			client = new Client($"{addressPrefix}{EntryRoom.Text}");

			if (!client.TestConnection(out var msg))
			{
				await Application.Current.MainPage.DisplayAlert("Conection failed", msg, "OK");
				return;
			}

			client.Connect(out _);

			await Navigation.PushModalAsync(Tools.CreateNavigationPage(new WaitPage(client, this)));
		}

		public async void GoToGamePage() 
			=> await Navigation.PushModalAsync(Tools.CreateNavigationPage(new GamePage(client, client.Users)));
	}
}