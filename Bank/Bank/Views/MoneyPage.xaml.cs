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
	public partial class MoneyPage : ContentPage
	{
		public enum Mode
		{
			OtherPlayer,
			SelfAdd,
			SelfRemove
		}

		public MoneyPage(Mode mode)
		{
			InitializeComponent();

			switch (mode)
			{
				case Mode.SelfAdd:
					Title = "Increase Money";
					break;

				case Mode.SelfRemove:
					Title = "Reduce Money";
					break;
			}
		}

		private async void Button_OnClicked(object sender, EventArgs e)
		{
			var text = ((Button) sender).Text;
			//EntryMoney.Text += text;

			switch (text)
			{
				case "Erase":
					if (!string.IsNullOrEmpty(EntryMoney.Text))
						EntryMoney.Text = EntryMoney.Text.Substring(0, EntryMoney.Text.Length - 1);
					break;

				case "Cancel":
				case "Send":
					await Navigation.PopModalAsync();
					break;

				default:
					if (text != "0" || !string.IsNullOrEmpty(EntryMoney.Text))
						EntryMoney.Text += text;
					break;
			}

			ButtonSave.Text = string.IsNullOrEmpty(EntryMoney.Text) ? "Cancel" : "Send";
		}
	}
}