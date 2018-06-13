using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Bank.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class WaitPage
	{
		private readonly Client client;

		public WaitPage(Client client, JoinPage parent)
		{
			InitializeComponent();
			this.client = client;

			client.Ready += async () =>
			{
				// TODO: Do this way or check client.Users size?
				await Navigation.PopModalAsync();
				parent.GoToGamePage();
			};
		}

		private async void ButtonLeave_OnClicked(object sender, EventArgs e)
		{
			// We don't need host to be able to leave
			var ip = Tools.IPAddress;

			if (client.Send($"BYE,{ip}", out var err))
				await Navigation.PopModalAsync();
			else
			{
				if (!await DisplayAlert($"Failed to leave ({err.GetType().FullName})", err.Message, "Dismiss", "Leave anyway"))
					await Navigation.PopModalAsync();
			}
		}
	}
}