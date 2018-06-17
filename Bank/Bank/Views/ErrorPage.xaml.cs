using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Bank.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ErrorPage : ContentPage
	{
		public ErrorPage(Exception e)
		{
			InitializeComponent();

			LabelTitle.Text = e.GetType().FullName;
			LabelDetails.Text = e.StackTrace;
		}
	}
}