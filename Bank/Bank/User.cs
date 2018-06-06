using Xamarin.Forms;

namespace Bank
{
    public class User
    {
	    public string      Name    { get; set; }
	    public ImageSource Avatar  { get; set; }
	    public string      Address { get; set; }

	    public uint Money;

	    public string FormattedMoney
	    {
		    get
		    {
				// M
			    if (Money >= 1000000)
				    return $"${Money / 100000}m";
				// K
			    if (Money >= 1000)
				    return $"${Money / 1000}k";
				// -
			    return $"${Money}";
		    }
	    }

	    public User(string name, int avatar, string address)
	    {
		    Name = name;
		    Address = address;
			
	        var avatarNames = new[]
	        {
		        "airplane",
		        "boat",
		        "burger",
		        "car",
		        "cat",
		        "dog",
		        "duck",
		        "hat",
		        "penguin",
		        "phone",
		        "shoe"
	        };

		    Avatar = ImageSource.FromFile(Device.RuntimePlatform == Device.UWP 
			    ? $"images/avatar/48/{avatarNames[avatar]}.png" 
			    : $"images/avatar/{avatarNames[avatar]}.png");
	    }
    }
}
