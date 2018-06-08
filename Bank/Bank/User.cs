using System;
using Xamarin.Forms;

namespace Bank
{
    public class User
    {
	    public string      Name    { get; set; }
	    public ImageSource Avatar  { get; set; }
	    public string      Address { get; set; }

	    public uint Money;

		/// <summary>
		/// Shortens and formats the money. Also adds $.
		/// </summary>
	    public string FormattedMoney
	    {
		    get
		    {
				// TODO: This rounds of pretty harshly (15.7k => 15k)

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

	    /// <summary>
	    /// Shortens the address to just ID or 'Host'
	    /// </summary>
	    public string FormattedAddress => Address == "127.0.0.1" ? "Host" : $"User {ID}";

	    public byte ID => byte.Parse(Address.Substring(Address.LastIndexOf(".", StringComparison.Ordinal)));

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
