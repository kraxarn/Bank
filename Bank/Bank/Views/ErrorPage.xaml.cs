using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Bank.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ErrorPage : ContentPage
	{
		public ErrorPage(string title, Exception e)
		{
			InitializeComponent();

			Title = title;

			LabelTitle.Text   = e.GetType().FullName;
			LabelDetails.Text = e.StackTrace;
		}

		private async void ButtonDismiss_OnClicked(object sender, EventArgs e) 
			=> await Navigation.PopModalAsync();
	}
}