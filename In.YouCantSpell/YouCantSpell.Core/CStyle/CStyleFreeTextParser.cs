using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using YouCantSpell.Utility;

namespace YouCantSpell.CStyle
{
	public class CStyleFreeTextParser
	{

		/// <summary>
		/// The characters that may be striped from the start or end of words.
		/// </summary>
		private static readonly char[] WordTrimChars = new[] { '!', '.', '(', ')', '[', ']', ',', ':', ';', '?', '{', '}', '<', '>' };
		/// <summary>
		/// Locates XML styled tags within a string.
		/// </summary>
        public static readonly Regex XmlNodeSplitRegex = new Regex(@"<([/])?\s*?([\p{L}\p{N}]+)\s*(.*?)?\s*?([/])?>", RegexOptions.Compiled | RegexOptions.Singleline);
		/// <summary>
		/// Used to extract non space parts (words or expressions) from a free form text string.
		/// </summary>
        public static readonly Regex StringLiteralWordParser = new Regex(@"\S+", RegexOptions.Compiled | RegexOptions.Singleline);
		/// <summary>
		/// Identifies words that appear to be like C styled identifiers.
		/// </summary>
        public static readonly Regex CodeWordDetectionRegex = new Regex(@"^\p{Lu}+$|^\p{Ll}+\p{Lu}|^(\p{Lu}\p{Ll}*){2,}", RegexOptions.Compiled | RegexOptions.Singleline);
		/// <summary>
		/// Determines if a word is composed only of valid characters.
		/// </summary>
		public static readonly Regex WordParserRegex = new Regex(@"^[\p{L}\']([\p{L}-\']*[\p{L}\'])?$", RegexOptions.Compiled);

		/// <summary>
		/// Trims punctuation characters from the two ends of a word.
		/// </summary>
		/// <param name="word">The word to be trimmed.</param>
		/// <param name="frontTrimCount">Output: The number of characters trimmed from the front.</param>
		/// <returns>The trimmed text.</returns>
		/// <remarks>
		/// The length and frontTrimCount can be used to determine how many characters were trimmed from the end.
		/// </remarks>
		private static string TrimWordEndCharacters(string word, out int frontTrimCount) {
			frontTrimCount = 0;
			while(frontTrimCount < word.Length && WordTrimChars.Contains(word[frontTrimCount]))
				frontTrimCount++;

			if(frontTrimCount > 0)
				word = word.Substring(frontTrimCount);

			return word.TrimEnd(WordTrimChars);
		}

		/// <summary>
		/// Trims punctuation characters from the two ends of a word.
		/// </summary>
		/// <param name="subString">The area to trim;</param>
		/// <returns></returns>
		private static TextSubString TrimWordEndCharacters(TextSubString subString)
		{
			int frontTrimCount;
			var resultWord = TrimWordEndCharacters(subString.SubText, out frontTrimCount);
			if(frontTrimCount == 0 && resultWord.Length == subString.Length)
				return subString;
			return new TextSubString(subString.Source, subString.Offset + frontTrimCount, resultWord.Length);
		}

		/// <summary>
		/// Returns true if the word looks like a C style identifier.
		/// </summary>
		/// <param name="word">The word to test.</param>
		/// <returns>True if the word looks like a C style identifier.</returns>
		public static bool LooksLikeCodeWord(string word) {
			var looksLikeCode = CodeWordDetectionRegex.IsMatch(word);
			return looksLikeCode;
		}

		/// <summary>
		/// Verifies that a word has only characters that are letters or common word punctuation.
		/// </summary>
		/// <param name="word">The word to test.</param>
		/// <returns>True if the word contains normal word characters.</returns>
		public static bool WordHasAllValidChars(string word) {
			return WordParserRegex.IsMatch(word);
		}

		private readonly int _minWordSize = 2;

		/// <summary>
		/// Locates text sections between, before, and after XML tags.
		/// </summary>
		/// <param name="textData">The text to parse.</param>
		/// <returns>The text portions between tags.</returns>
		public IEnumerable<TextSubString> ParseXmlTextParts(ITextSubString textData)
		{
			var textDataOffset = textData.Offset;
			int localTextStartIndex = 0;
			foreach(Match xmlNodeMatch in XmlNodeSplitRegex.Matches(textData.SubText))
			{
				// The first free text part is from location 0 to the start of the first found tag.
				// After that, except for the last part the free text part is located between the end
				// of the previous tag and the start of the current tag.
				yield return new TextSubString(
					textData.Source,
					localTextStartIndex + textDataOffset,
					xmlNodeMatch.Index - localTextStartIndex
				);
				localTextStartIndex = xmlNodeMatch.Index + xmlNodeMatch.Length;
			}
			// The last free text part is from the end of the last tag to the end of the text string.
			// If there are no tags found the entire text string is used.
			yield return new TextSubString(
				textData.Source,
				localTextStartIndex + textDataOffset,
				textData.Length - localTextStartIndex
			);
		}

		public IEnumerable<TextSubString> ParseSentenceChunks(ITextSubString textData)
		{
			return StringLiteralWordParser.Matches(textData.SubText)
				.Cast<Match>()
				.Select(x => new TextSubString(
					textData.Source,
					textData.Offset + x.Index,
					x.Length
				));
		}

		public IEnumerable<TextSubString> ParseSentenceWordsForSpellCheck(ITextSubString textData)
		{
			foreach(var sentenceChunk in ParseSentenceChunks(textData))
			{
				var word = sentenceChunk.SubText;
				if(StringUtil.IsWrappedInQuotes(word))
					continue; // If something is wrapped in quotes we should probably leave it alone.

				// Trim the special characters such as periods and parenthesis from the ends of the word as we can safely ignore them, usually.
				var wordChunk = TrimWordEndCharacters(sentenceChunk);
				if(!ReferenceEquals(wordChunk, sentenceChunk))
					word = wordChunk.SubText; // if the word was updated we need to change the string variable to match

				// Check to make sure that the word is large enough to bother checking.
				// Check to make sure that the word is composed of valid characters so we don't spell check code or URIs.
				if(wordChunk.Length < _minWordSize || !WordHasAllValidChars(word))
					continue;

				yield return wordChunk;
			}
		}


	}
}
