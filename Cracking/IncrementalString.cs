using System.Text;

namespace Cracking
{
	public class IncrementalString
	{
		private readonly char[] _characters;
		private readonly int[] _indeces;

		public IncrementalString(char[] characters, int length)
		{
			_characters = characters;
			_indeces = new int[length];
			for (var i = 0; i < length; i++) _indeces[i] = -1;
		}

		public string Next()
		{
			for (var i = _indeces.Length - 1; i >= 0; i--)
			{
				if (++_indeces[i] == _characters.Length)
				{
					_indeces[i] = 0;
					continue;
				}
				break;
			}
			return ToString();
		}

		public override string ToString()
		{
			var builder = new StringBuilder();
			foreach (var i in _indeces)
				if (i >= 0)
					builder.Append(_characters[i]);
			return builder.ToString();
		}
	}
}
