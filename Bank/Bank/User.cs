using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Xamarin.Forms;

namespace Bank
{
    public class User
    {
	    public string      Name    { get; set; }
	    public ImageSource Avatar  { get; set; }
	    public string      Address { get; set; }

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
		    Avatar = ImageSource.FromFile($"Images/Avatar/{avatarNames[avatar]}.png");
	    }
    }
}
