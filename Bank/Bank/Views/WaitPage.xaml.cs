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
	public partial class WaitPage : ContentPage
	{
		private readonly Client client;

		public WaitPage(Client client)
		{
			InitializeComponent();
			this.client = client;
		}

		private async void ButtonLeave_OnClicked(object sender, EventArgs e)
		{
			// We don't need host to be able to leave
			var ip = Tools.GetIPAddress();

			if (client.Send($"BYE,{ip}", out var err))
				await Navigation.PopModalAsync();
			else
				await Application.Current.MainPage.DisplayAlert($"Failed to leave ({err.GetType().FullName})", err.Message, "Dismiss");
		}
	}
}