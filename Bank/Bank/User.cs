using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Xamarin.Forms;

namespace Bank
{
    public class User : INotifyPropertyChanged
    {
	    public event PropertyChangedEventHandler PropertyChanged;

	    public string      Name    { get; }
	    public ImageSource Avatar  { get; }
	    public string      Address { get; }
		public int     AvatarIndex { get; }
		
	    private uint money;

	    public uint Money
	    {
			get => money;
		    set
		    {
			    money = value;
				NotifyPropertyChanged();
			    NotifyPropertyChanged("FormattedMoney");
			}
	    }

		/// <summary>
		/// Shortens and formats the money. Also adds $.
		/// </summary>
	    public string FormattedMoney
	    {
		    get
		    {
				// M
			    if (money >= 1000000)
				    return $"${money / 1000000f:F}m";
				// K
			    if (money >= 1000)
				    return $"${money / 1000f:F1}k";
				// -
			    return $"${money}";
		    }
	    }

	    /// <summary>
	    /// Shortens the address to just ID or 'Host'
	    /// </summary>
	    public string FormattedAddress => Address == localAddress ? "You" : $"User {ID}";

	    public byte ID => byte.Parse(Address.Substring(Address.LastIndexOf(".", StringComparison.Ordinal) + 1));

	    private readonly string localAddress;

	    public User(string name, int avatar, string address, uint money)
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

		    localAddress = Tools.IPAddress;
		    AvatarIndex  = avatar;
		    this.money   = money;
	    }

		private void NotifyPropertyChanged([CallerMemberName] string name = "") 
			=> PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

		public override string ToString() => $"{Name} ({Address})";
	}
}
