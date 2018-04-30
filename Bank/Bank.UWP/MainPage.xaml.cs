using Windows.Foundation;
using Windows.UI.ViewManagement;

namespace Bank.UWP
{
    public sealed partial class MainPage
    {
        public MainPage()
        {
            InitializeComponent();

			ApplicationView.PreferredLaunchViewSize = new Size(540, 960);
	        ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.PreferredLaunchViewSize;

            LoadApplication(new Bank.App());
		}
    }
}
