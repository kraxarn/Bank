using System;
using System.Net;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Bank.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class WhatsNewPage
	{
		public WhatsNewPage()
		{
			InitializeComponent();

			using (var client = new WebClient())
			{
				client.DownloadStringAsync(new Uri("https://web.kraxarn.com/api/ver/github.php?app=Bank&type=body_beta&ver=force"));

				client.DownloadStringCompleted += (sender, args) =>
				{
					// Get result
					var result = args.Result;

					// Set title
					var lines = result.Split('\n');
					Title = lines[0];

					// Check if should display 'old version' text
					if (Title != Tools.Version)
						LabelNotice.IsVisible = true;

					// Remove first line since it's just the version number
					lines[0] = "";

					// Loop through and format nicely
					var log = new FormattedString();

					foreach (var line in lines)
					{
						var span = new Span
						{
							Text = line
						};

						if (line.StartsWith("*"))
						{
							// It's a point, change markdown * to a nice little emoji
							span.Text = line.Replace("*", "🔹");
						}
						else if (line.StartsWith("##"))
						{
							// It's a title, make it larger and bolder
							span.FontSize = 24;
							span.FontAttributes = FontAttributes.Bold;
							span.Text = span.Text.Substring(3);
						}

						// Add new line on Android
						if (Device.RuntimePlatform == Device.Android)
							span.Text += "\n";

						// Add the lime
						log.Spans.Add(span);
					}

					// Set label as the lines
					LabelLog.FormattedText = log;
				};
			}
		}

		private async void ButtonLeave_OnClicked(object sender, EventArgs e) => await Navigation.PopModalAsync();
	}
}