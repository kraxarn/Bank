using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Bank.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class HostPage : ContentPage
	{
		private readonly ObservableCollection<User> users;

		public HostPage()
		{
			InitializeComponent();

			users = new ObservableCollection<User>();
			ViewUsers.ItemsSource = users;

			string ip;
			using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
			{
				socket.Connect("8.8.8.8", 65530);
				var endPoint = socket.LocalEndPoint as IPEndPoint;
				ip = endPoint?.Address.ToString();
			}

			var name = Application.Current.Properties["name"] as string;
			var avatar = (int)Application.Current.Properties["avatar"];

			users.Add(new User(name, avatar, ip));
			LabelRoom.Text += ip.Substring(ip.LastIndexOf('.') + 1);
		}

		private void ButtonStart_OnClicked(object sender, EventArgs e)
		{
			if (users.Count < 2)
			{
				Application.Current.MainPage.DisplayAlert("Not enough players", "Try adding more players and try again", "OK");
				return;
			}
		}
	}
}