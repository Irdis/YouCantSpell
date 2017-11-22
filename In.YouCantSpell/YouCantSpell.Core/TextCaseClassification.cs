namespace YouCantSpell
{
	/// <summary>
	/// Classifications for the casing of text.
	/// </summary>
	public enum TextCaseClassification
	{
		/// <summary>
		/// The casing of the text is unknown.
		/// </summary>
		Unknown = 0,
		/// <summary>
		/// The casing of the text is a mixture of upper and lower-case characters.
		/// </summary>
		Mixed,
		/// <summary>
		/// The casing of the text is upper-case.
		/// </summary>
		Upper,
		/// <summary>
		/// The casing of the text is lower-case.
		/// </summary>
		Lower
	}
}
