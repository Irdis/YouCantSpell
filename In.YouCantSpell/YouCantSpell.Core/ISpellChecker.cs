using System.Collections.Generic;

namespace YouCantSpell
{
	/// <summary>
	/// Defines operations that offer spell checking and spelling fix suggestions.
	/// </summary>
	public interface ISpellChecker
	{
		/// <summary>
		/// Adds a word to the dictionary.
		/// </summary>
		/// <param name="word">The word to add.</param>
		void Add(string word);

		/// <summary>
		/// Adds multiple words to the dictionary.
		/// </summary>
		/// <param name="words">The words to add.</param>
		void Add(IEnumerable<string> words);
		
		/// <summary>
		/// Checks the spelling of a word against the dictionary.
		/// </summary>
		/// <param name="word">The word to check.</param>
		/// <returns>true when the word is found or spelled correctly.</returns>
		bool Check(string word);

		/// <summary>
		/// Gets a list of recommended alternative spellings based on the given word.
		/// </summary>
		/// <param name="word">The word to get suggestions for.</param>
		/// <returns>Word suggestions.</returns>
		string[] GetRecommendations(string word);

	}
}
