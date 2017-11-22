using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace YouCantSpell.Utility
{
	public static class StringUtil
	{

		/// <summary>
		/// Finds text that is only letters.
		/// </summary>
		public static readonly Regex LetterParserRegex = new Regex(@"[\p{L}]+", RegexOptions.Compiled);

        public static readonly Regex WordParserRegex = new Regex(@"\w+", RegexOptions.Compiled);

		/// <summary>
		/// Replaces a section of a string.
		/// </summary>
		/// <param name="source">The source text, to replace a part within.</param>
		/// <param name="replace">The text that will replace a part of the given source string.</param>
		/// <param name="offset">The offset within the source string to start replacing.</param>
		/// <param name="length">The number of characters within the source string to replace from the offset.</param>
		/// <returns></returns>
		public static string ReplaceSection(string source, string replace, int offset, int length) {
			var result = offset > 0
				? (source.Substring(0, offset) + replace)
				: replace;
			
			var postIndex = offset + length;
			return postIndex < source.Length
				? result + source.Substring(postIndex)
				: result;
		}

		private static TextCaseClassification MergeClassifications(TextCaseClassification a, TextCaseClassification b) {
			if(a == b || b == TextCaseClassification.Unknown)
				return a;
			if(a == TextCaseClassification.Unknown)
				return b;
			// they are not equal and are either mixed, upper, or lower so combined they must be mixed
			return TextCaseClassification.Mixed;
		}

		/// <summary>
		/// Classifies the casing of a letter.
		/// </summary>
		/// <param name="letter">The letter to classify.</param>
		/// <returns>The letter case classification.</returns>
		public static TextCaseClassification ClassifyCharCase(char letter)
		{
			return Char.IsUpper(letter)
				? TextCaseClassification.Upper
				: Char.IsLower(letter)
				? TextCaseClassification.Lower
				: TextCaseClassification.Unknown;
		}

		/// <summary>
		/// Classifies the casing of a string.
		/// </summary>
		/// <param name="letters">The text to classify.</param>
		/// <returns>The letter casing of the entire string.</returns>
		public static TextCaseClassification ClassifyCharCase(string letters) {
			var current = TextCaseClassification.Unknown;
			for(int i = letters.Length-1; i >= 0; i--){
				current = MergeClassifications(current, ClassifyCharCase(letters[i]));
				if(current == TextCaseClassification.Mixed)
					return TextCaseClassification.Mixed;
			}
			return current;
		}

		/// <summary>
		/// Classifies the casing of a string.
		/// </summary>
		/// <param name="letters">The text to classify.</param>
		/// <returns>The letter casing of the entire string.</returns>
		public static TextCaseClassification ClassifyCharCase(IEnumerable<char> letters) {
			var current = TextCaseClassification.Unknown;
			foreach(var letter in letters){
				current = MergeClassifications(current, ClassifyCharCase(letter));
				if(current == TextCaseClassification.Mixed)
					return TextCaseClassification.Mixed;
			}
			return current;
		}

		/// <summary>
		/// Determines if text is wrapped in quotes.
		/// </summary>
		/// <param name="text">The text to test.</param>
		/// <returns>True if the word is wrapped in quotes.</returns>
		public static bool IsWrappedInQuotes(string text)
		{
			if(text != null && text.Length >= 2) {
				if((text[0] == '\"' || text[0] == '\'')) {
					return text[0] == text[text.Length - 1];
				}
				if(text[0] == '`') {
					return '\'' == text[text.Length - 1];
				}
			}
			return false;
		}

	}
}
