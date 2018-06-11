using System;
using System.Diagnostics;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Bank.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class MoneyPage
	{
		public enum Mode
		{
			OtherPlayer,
			SelfAdd,
			SelfRemove
		}

		private string money;
		private char   moneyUnit;
		private readonly User fromUser, toUser;
		private readonly Client client;
		private readonly Mode mode;

		private string FormattedMoney => $"{money}{moneyUnit}";

		private uint TotalMoney
		{
			get
			{
				var total = money.Substring(1);

				if (total.StartsWith("."))
					total = $"0{total}";

				if (!float.TryParse(total, out var m))
					return 0;

				switch (moneyUnit)
				{
					case 'k': m *= 1000;    break;
					case 'm': m *= 1000000; break;
				}

				return (uint) m;
			}
		}

		public MoneyPage(Client client, Mode mode, User from, User to = null)
		{
			InitializeComponent();

			this.client = client;
			this.mode   = mode;

			// Set title
			switch (mode)
			{
				case Mode.SelfAdd:
					Title = "Send from bank";
					break;

				case Mode.SelfRemove:
					Title = "Send to bank";
					break;
			}

			// Set users
			fromUser = from;
			toUser   = to;

			// Set 'You'
			LabelSelfMoney.Text = from.FormattedMoney;

			// Set 'User'
			LabelUserName.Text  = to?.Name;
			LabelUserMoney.Text = to?.FormattedMoney;

			if (to == null)
				LabelArrow.Text = null;

			// Set default stuff
			money = "$";
			moneyUnit = ' ';

			// Increase font size for buttons
			// TODO: Make this an option
			foreach (var child in GridNumpad.Children)
				((Button) child).FontSize = 18;
		}

		private async void Button_OnClicked(object sender, EventArgs e)
		{
			var text = (sender as Button)?.Text;

			switch (text)
			{
				case "Erase":
					if (money != "$")
						money = money.Substring(0, money.Length - 1);
					break;

				case "Cancel":
					await Navigation.PopModalAsync();
					break;

				case "Send":
					SendMoney();
					await Navigation.PopModalAsync();
					break;

				case "." when money.Contains("."):
					break;

				default:
					if (text != "0" || money != "$0")
						money += text;
					break;
			}

			UpdateLabels();
		}

		private void ButtonUnit_OnClicked(object sender, EventArgs e)
		{
			var text = (sender as Button)?.Text;

			switch (text)
			{
				case "K" when moneyUnit == 'k':
					moneyUnit = ' ';
					break;

				case "K":
					moneyUnit = 'k';
					break;

				case "M" when moneyUnit == 'm':
					moneyUnit = ' ';
					break;

				case "M":
					moneyUnit = 'm';
					break;
			}

			UpdateLabels();
		}

		private void UpdateLabels()
		{
			EntryMoney.Text = FormattedMoney;
			var totalMoney  = TotalMoney;

			ButtonSave.Text = totalMoney == 0 ? "Cancel" : "Send";

			if (mode != Mode.SelfAdd && ButtonSave.Text == "Send" && totalMoney > fromUser.Money)
				ButtonSave.IsEnabled = false;
			else
				ButtonSave.IsEnabled = true;

			LabelTotalMoney.Text = $"${Tools.Seperate(totalMoney)}";
		}

		private void SendMoney()
		{
			Exception e;

			switch (mode)
			{
				/*
				 * Send to other player
				 * Remove 'from' and add 'to'
				 */
				case Mode.OtherPlayer:
					// First, remove from local, then add to other user
					if (!client.Send($"REM,{fromUser.Address},{TotalMoney}", out e))
						DisplayAlert("Failed to remove money", e.Message, "Dismiss");
					else if (!client.Send($"ADD,{toUser.Address},{TotalMoney}", out e))
						DisplayAlert("Failed to add money", e.Message, "Dismiss");
					break;

				/*
				 * Add to self
				 * Add 'from'
				 */
				case Mode.SelfAdd:
					if (!client.Send($"ADD,{fromUser.Address},{TotalMoney}", out e))
						DisplayAlert("Failed to add money", e.Message, "Dismiss");
					break;

				/*
				 * Remove from self
				 * Remove 'from'
				 */
				case Mode.SelfRemove:
					if (!client.Send($"REM,{fromUser.Address},{TotalMoney}", out e))
						DisplayAlert("Failed to remove money", e.Message, "Dismiss");
					break;
			}
		}
	}
}