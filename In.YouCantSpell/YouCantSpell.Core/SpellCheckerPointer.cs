using System;
using System.Collections.Generic;

namespace YouCantSpell
{
	public class SpellCheckerPointer : ISpellChecker, IDisposable
	{

		private ISpellChecker _core;
		private readonly bool _performDispose;

		public SpellCheckerPointer(ISpellChecker core) : this(core, true) {}

		public SpellCheckerPointer(ISpellChecker core, bool performDispose){
			if(null == core)
				throw new ArgumentNullException("core");
			_core = core;
			_performDispose = performDispose;
		}

		public void Replace(ISpellChecker newCore){
			if(null == newCore)
				throw new ArgumentNullException();

			var oldCore = _core;
			_core = newCore;

			DisposeIfRequired(oldCore);
		}

		private void DisposeIfRequired(ISpellChecker checker){
			if (!_performDispose)
				return;
			var disposable = checker as IDisposable;
			if(null != disposable)
				disposable.Dispose();
		}

		public void Add(string word) {
			_core.Add(word);
		}

		public void Add(IEnumerable<string> words) {
			_core.Add(words);
		}

		public bool Check(string word) {
			return _core.Check(word);
		}

		public string[] GetRecommendations(string word){
			return _core.GetRecommendations(word);
		}

		public void Dispose(){
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing){
			DisposeIfRequired(_core);
		}

		~SpellCheckerPointer(){
			Dispose(false);
		}

	}
}
