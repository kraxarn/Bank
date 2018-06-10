using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace Bank
{
	public class UserCollection : Collection<User>, INotifyCollectionChanged, INotifyPropertyChanged
	{
		/// <inheritdoc />
		/// <summary>
		/// Item was added. removed or list cleared.
		/// </summary>
		public event NotifyCollectionChangedEventHandler CollectionChanged;

		/// <inheritdoc />
		/// <summary>
		/// Property changed value.
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;

		public UserCollection(IEnumerable<User> items)
		{
			foreach (var item in items)
				AddItem(item);
		}

		private void NotifyCollectionChanged(NotifyCollectionChangedAction action)
		{
			CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(action));
		}

		private void NotifyPropertyChanged()
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(""));
		}

		public void AddItem(User item)
		{
			Add(item);
			NotifyCollectionChanged(NotifyCollectionChangedAction.Add);
		}

		public void RemoveItem(User item)
		{
			Remove(item);
			NotifyCollectionChanged(NotifyCollectionChangedAction.Remove);
		}

		// User specific stuff

		/// <summary>
		/// Sets money for a specific user
		/// </summary>
		/// <param name="address"> Address to search for </param>
		/// <param name="money"> Money to set it to </param>
		/// <returns> If it was successful </returns>
		public bool SetMoney(string address, uint money)
		{
			var user = this.FirstOrDefault(u => u.Address == address);

			if (user == default(User))
				return false;

			user.Money = money;

			NotifyPropertyChanged();
			return true;
		}
	}
}