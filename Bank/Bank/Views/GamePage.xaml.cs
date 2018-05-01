using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Bank.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class GamePage : ContentPage
	{
		private Client client;

		public GamePage(Client client, IEnumerable<User> users)
		{
			InitializeComponent();

			this.client = client;

			// Get local user
			// TODO: This doesn't work if we're connecting to a server
			var currentUser = users.Single(u => u.Address == "127.0.0.1");

			ImageAvatar.Source = currentUser.Avatar;
			LabelName.Text     = currentUser.Name;
			LabelMoney.Text    = currentUser.FormattedMoney;
		}
	}
}