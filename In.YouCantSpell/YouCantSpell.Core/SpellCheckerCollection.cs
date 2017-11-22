using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace YouCantSpell
{
	public class SpellCheckerCollection : Collection<ISpellChecker>, ISpellChecker, IDisposable
	{

		public SpellCheckerCollection(IEnumerable<ISpellChecker> spellCheckers)
			: base((spellCheckers ?? Enumerable.Empty<ISpellChecker>()).ToList()) { }

		public void Add(string word){
			var first = this.FirstOrDefault();
			if(null != first)
				first.Add(word);
		}

		public void Add(IEnumerable<string> words) {
			var first = this.FirstOrDefault();
			if (null != first)
				first.Add(words);
		}

		public bool Check(string word){
			return this.Any(x => x.Check(word));
		}

		public string[] GetRecommendations(string word){
			return this
				.SelectMany(x => x.GetRecommendations(word))
				.Distinct()
				.ToArray();
		}

		protected virtual void Dispose(bool disposing){
			foreach (var item in this.OfType<IDisposable>()){
				item.Dispose();
			}
		}

		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		~SpellCheckerCollection(){
			Dispose(false);
		}

	}
}
