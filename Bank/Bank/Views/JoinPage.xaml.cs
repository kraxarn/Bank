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
	public partial class JoinPage : ContentPage
	{
		public Client client;
		public string addressPrefix;

		public JoinPage()
		{
			InitializeComponent();

			var ip = Tools.GetIPAddress();
			addressPrefix = ip.Substring(0, ip.LastIndexOf('.') + 1);
		}

		private void EntryRoom_OnTextChanged(object sender, TextChangedEventArgs e)
		{
			if (int.TryParse(EntryRoom.Text, out var id))
				ButtonConnect.IsEnabled = id > 0 && id < 256;
			else
				ButtonConnect.IsEnabled = false;
		}

		private void ButtonConnect_OnClicked(object sender, EventArgs e)
		{
			client = new Client($"{addressPrefix}{EntryRoom.Text}");

			if (!client.TestConnection(out var msg))
			{
				Application.Current.MainPage.DisplayAlert("Error", $"Connection failed: {msg}", "OK");
				return;
			}

			client.Connect();
		}
	}
}