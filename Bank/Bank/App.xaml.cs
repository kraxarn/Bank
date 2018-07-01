using Bank.Views;

namespace Bank
{
	public partial class App
	{
		public delegate void SleepEvent();
		public delegate void ResumeEvent();

		public static event SleepEvent  OnAppSleep;
		public static event ResumeEvent OnAppResume;

		public App()
		{
			InitializeComponent();

			MainPage = new MainPage();
		}

		protected override void OnStart ()
		{
			// Handle when your app starts
		}

		protected override void OnSleep ()
		{
			// Handle when your app sleeps
			OnAppSleep?.Invoke();
		}

		protected override void OnResume ()
		{
			// Handle when your app resumes
			OnAppResume?.Invoke();
		}
	}
}
