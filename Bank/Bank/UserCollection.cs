using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

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

		private void NotifyPropertyChanged([CallerMemberName] string name = "")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
		}

		public uint this[string address]
		{
			get => this.First(u => u.Address == address).Money;
			set
			{
				this.First(u => u.Address == address).Money = value;
				NotifyPropertyChanged();
			}
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
	}
}