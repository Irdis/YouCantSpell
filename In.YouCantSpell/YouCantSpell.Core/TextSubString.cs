namespace YouCantSpell
{
	/// <summary>
	/// A sub-string of text derived from a larger string of text.
	/// </summary>
	public class TextSubString : ITextSubString
	{
		/// <summary>
		/// The source text that this sub-string is based on.
		/// </summary>
		public readonly string Source;
		/// <summary>
		/// The offset within the source string where the sub-string begins.
		/// </summary>
		public readonly int Offset;
		/// <summary>
		/// The number of characters from the offset that make up the sub-string.
		/// </summary>
		public readonly int Length;

		private string _subStringCache;

		/// <summary>
		/// Creates a sub-string which is the same as the source string.
		/// </summary>
		/// <param name="source"></param>
		public TextSubString(string source)
			: this(source, 0, source.Length) { }

		/// <summary>
		/// Creates a sub-string from the 
		/// </summary>
		/// <param name="source">The source text.</param>
		/// <param name="offset">The offset where the sub-string starts.</param>
		/// <param name="length">The length of the sub-string.</param>
		public TextSubString(string source, int offset, int length)
		{
			Source = source;
			Offset = offset;
			Length = length;
			_subStringCache = null;
		}

		/// <summary>
		/// The sub-text derived from the source text.
		/// </summary>
		public string SubText {
			get { return _subStringCache ?? (_subStringCache = Source.Substring(Offset, Length)); }
		}

		/// <summary>
		/// The location one past the last character in the sub-string within the source string.
		/// </summary>
		public int EndOffset {
			get { return Offset + Length; }
		}

		string ITextSubString.Source { get { return Source; } }

		int ITextSubString.Offset { get { return Offset; } }

		int ITextSubString.Length { get { return Length; } }
	}
}
