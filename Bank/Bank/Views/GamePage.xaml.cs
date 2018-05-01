using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Bank.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class GamePage : ContentPage
	{
		private Client client;

		public GamePage(Client client)
		{
			InitializeComponent();

			this.client = client;
		}
	}
}