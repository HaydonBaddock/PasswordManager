using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Enhanced_Password_Manager.Annotations;
using Enhanced_Password_Manager.Encryption;
using Enhanced_Password_Manager.Model;
using Enhanced_Password_Manager.Properties;
using Microsoft.Win32;

namespace Enhanced_Password_Manager
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : INotifyPropertyChanged
	{
		#region fields

		private readonly CollectionView _view;

		private readonly IEncrypt _encrypter;

		#endregion

		#region Properties

		public SortedObservableCollection<Entry> Entries { get; set; } = new SortedObservableCollection<Entry>();

		public string Password { get; set; }

		public string FileLocation
		{
			get { return Settings.Default.FileLocation; }
			set
			{
				Settings.Default.FileLocation = value;
				Settings.Default.Save();
				FileOpen = false;
				ClearAllForms();
				OnPropertyChanged(nameof(FileName));
			}
		}

		public string FileName => string.IsNullOrEmpty(FileLocation) ? "no file selected" : FileLocation.Substring(FileLocation.LastIndexOf("\\") + 1).Replace(".cpt", "");

		private bool _fileOpen;
		public bool FileOpen
		{
			get { return _fileOpen; }
			set
			{
				_fileOpen = value;
				OnPropertyChanged(nameof(FileOpen));
			}
		}

		private string _searchText;
		public string SearchText
		{
			get { return _searchText; }
			set
			{
				_searchText = value;
				_view.Refresh();
			}
		}

		private Entry _selectedEntry;
		public Entry SelectedEntry
		{
			get { return _selectedEntry; }
			set
			{
				_selectedEntry = value;
				IsEntrySelected = value != null;
				OnPropertyChanged(nameof(SelectedEntry));
			}
		}

		private bool _isEntrySelected;
		public bool IsEntrySelected
		{
			get { return _isEntrySelected; }
			set
			{
				_isEntrySelected = value;
				OnPropertyChanged(nameof(IsEntrySelected));
			}
		}

		#endregion

		public MainWindow()
		{
			InitializeComponent();
			_view = (CollectionView) CollectionViewSource.GetDefaultView(Entries);
			_view.Filter = ItemFilter;
			_encrypter = new OneTimePad();
			PasswordTextBox.Focus();
		}

		private bool ItemFilter(object item)
		{
			if (string.IsNullOrEmpty(SearchText)) return true;
			return ((Entry) item).Title.IndexOf(SearchText, StringComparison.OrdinalIgnoreCase) >= 0;
		}

		private void ClearAllForms()
		{
			Entries.Clear();
			Password = string.Empty;
			SearchText = string.Empty;
		}

		private void SelectFile_Click(object sender, RoutedEventArgs e)
		{
			var openFileDialog = new OpenFileDialog
			{
				Title = "Open an Encrypted File",
				Filter = "Encrypted Data File (*.cpt)|*.cpt"
			};
			var result = openFileDialog.ShowDialog();
			if (result == true)
			{
				FileLocation = openFileDialog.FileName;
				FileOpen = false;
			}
		}

		private void NewFile_Click(object sender, RoutedEventArgs e)
		{
			var saveFileDialog = new SaveFileDialog
			{
				Title = "Save an Encrypted File",
				Filter = "Encrypted Data File (*.cpt)|*.cpt"
			};
			var result = saveFileDialog.ShowDialog();
			if (result == true)
			{
				FileLocation = saveFileDialog.FileName;
				FileOpen = true;
			}
		}

		private void Decrypt_Click(object sender, RoutedEventArgs e)
		{
			if (!CheckInputFields()) return;
			if (!File.Exists(FileLocation)) return;

			var encryptedBytes = File.ReadAllBytes(FileLocation);
			var decryptedBytes = _encrypter.Decrypt(encryptedBytes, Password);
			var memoryStream = new MemoryStream();
			var binaryFormatter = new BinaryFormatter();
			memoryStream.Write(decryptedBytes, 0, decryptedBytes.Length);
			memoryStream.Position = 0;
			try
			{
				var deserialised = (SortedObservableCollection<Entry>) binaryFormatter.Deserialize(memoryStream);
				Entries.Clear();
				Entries.AddAll(deserialised);
				FileOpen = true;
			}
			catch (Exception ex) when (ex is DecoderFallbackException || ex is SerializationException) { }
		}

		private void Encrypt_Click(object sender, RoutedEventArgs e)
		{
			if (!CheckInputFields()) return;

			var binaryFormatter = new BinaryFormatter();
			var memoryStream = new MemoryStream();
			binaryFormatter.Serialize(memoryStream, Entries);
			var decryptedBytes = memoryStream.ToArray();
			var encryptedBytes = _encrypter.Encrypt(decryptedBytes, Password);
			using (var stream = File.Open(FileLocation, FileMode.Create))
				stream.Write(encryptedBytes, 0, encryptedBytes.Length);
		}

		private bool CheckInputFields()
		{
			var valid = true;
			if (string.IsNullOrEmpty(FileLocation))
			{
				Highlight(FileNameBorder, Colors.White);
				valid = false;
			}
			if (string.IsNullOrEmpty(Password))
			{
				Highlight(PasswordTextBox, Color.FromRgb(171, 173, 179));
				valid = false;
			}
			return valid;
		}

		private void Highlight(UIElement control, Color originalColor)
		{
			var colourAnimation = new ColorAnimation(Colors.DeepPink, originalColor, new Duration(TimeSpan.FromMilliseconds(1000)));
			var thicknessAnimation = new ThicknessAnimation(new Thickness(2, 2, 2, 2), new Thickness(1, 1, 1, 1), new Duration(TimeSpan.FromMilliseconds(1000)));
			var storyBoard = new Storyboard();
			storyBoard.Duration = TimeSpan.FromMilliseconds(1000);
			storyBoard.Children.Add(colourAnimation);
			storyBoard.Children.Add(thicknessAnimation);
			Storyboard.SetTarget(colourAnimation, control);
			Storyboard.SetTarget(thicknessAnimation, control);
			Storyboard.SetTargetProperty(colourAnimation, new PropertyPath("BorderBrush.Color"));
			Storyboard.SetTargetProperty(thicknessAnimation, new PropertyPath("BorderThickness"));
			storyBoard.Begin();
		}

		private void AddEntry_Click(object sender, RoutedEventArgs e)
		{
			var newEntry = new Entry {Title = "New Entry"};
			Entries.Add(newEntry);
			EntriesListBox.SelectedIndex = Entries.IndexOf(newEntry);
		}

		private void RemoveEntry_Click(object sender, RoutedEventArgs e)
		{
			var selectedItem = EntriesListBox.SelectedItem as Entry;
			if (selectedItem == null) return;
			var index = Entries.IndexOf(selectedItem);
			Entries.Remove(selectedItem);
			EntriesListBox.SelectedIndex = Math.Min(index, Entries.Count - 1);
		}

		public event PropertyChangedEventHandler PropertyChanged;
		[NotifyPropertyChangedInvocator]
		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
