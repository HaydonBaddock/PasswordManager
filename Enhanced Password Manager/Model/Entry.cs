using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Enhanced_Password_Manager.Annotations;

namespace Enhanced_Password_Manager.Model
{
	/// <summary>
	/// Represents a place that the user has an eccount.
	/// </summary>
	[Serializable]
	public class Entry : IComparable, INotifyPropertyChanged
	{
		private string _title;
		private string _userName;
		private string _email;
		private string _password;
		private string _description;

		public string Title
		{
			get { return _title; }
			set { _title = value; OnPropertyChanged(nameof(Title)); }
		}

		public string UserName
		{
			get { return _userName; }
			set { _userName = value; OnPropertyChanged(nameof(UserName)); }
		}

		public string Email
		{
			get { return _email; }
			set { _email = value; OnPropertyChanged(nameof(Email)); }
		}

		public string Password
		{
			get { return _password; }
			set { _password = value; OnPropertyChanged(nameof(Password)); }
		}

		public string Description
		{
			get { return _description; }
			set { _description = value; OnPropertyChanged(nameof(Description)); }
		}

		/// <summary>
		/// Determines if this entry is alphabetically before another.
		/// </summary>
		/// <param name="obj">Another Entry</param>
		/// <returns>An int indicating its relative position</returns>
		public int CompareTo(object obj)
		{
			if (obj == null) return 1;
			var otherEntry = obj as Entry;
			if (otherEntry == null) throw new ArgumentException("Object is not an Entry");
			return string.Compare(Title, otherEntry.Title, StringComparison.OrdinalIgnoreCase);
		}

		[field:NonSerialized]
		public event PropertyChangedEventHandler PropertyChanged;
		[NotifyPropertyChangedInvocator]
		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
