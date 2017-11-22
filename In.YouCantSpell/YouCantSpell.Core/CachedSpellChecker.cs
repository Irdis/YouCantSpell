using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using System.Threading;

namespace YouCantSpell
{
	/// <summary>
	/// A spell checker that operates by caching results and synchronizing requests.
	/// </summary>
	public class CachedSpellChecker : ISpellChecker, IDisposable
	{
		/// <summary>
		/// This is a cache record for a requested word.
		/// </summary>
		private struct CachedSpellCheckData
		{
			public CachedSpellCheckData(bool found, string[] suggestions = null)
			{
				WordFound = found;
				Suggestions = null == suggestions ? null : Array.AsReadOnly(suggestions);
			}

			public readonly bool WordFound;
			public readonly ReadOnlyCollection<string> Suggestions;
		}

		//private readonly object _cacheLock = new object();
		private ISpellChecker _core;
		private readonly bool _ownsCore;
		private readonly Dictionary<string, CachedSpellCheckData> _cache;
		private readonly int _cacheMax = 1024; // must be larger than 0
		private readonly ReaderWriterLockSlim _cacheLock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);

		/// <summary>
		/// Wraps another spell checker to provide caching and synchronization.
		/// </summary>
		/// <param name="core">The core spell checker to be wrapped.</param>
		/// <param name="ownsCore">If true this wrapper will be responsible for disposal of the core spell checker.</param>
		public CachedSpellChecker(ISpellChecker core, bool ownsCore) {
			if(null == core)
				throw new ArgumentNullException("core");
			_core = core;
			_ownsCore = ownsCore;
			_cache = new Dictionary<string, CachedSpellCheckData>();
		}

		/// <inheritdoc/>
		public void Add(string word) {
			_cacheLock.EnterWriteLock();
			try {
				_core.Add(word);
				_cache.Clear(); // NOTE: when a new item is added the caches must be cleared as it will affect the suggestions
			}
			finally {
				_cacheLock.ExitWriteLock();
			}
		}

		/// <inheritdoc/>
		public void Add(IEnumerable<string> words) {
			_cacheLock.EnterWriteLock();
			try {
				_core.Add(words);
				_cache.Clear();  // NOTE: when a new item is added the caches must be cleared as it will affect the suggestions
			}
			finally {
				_cacheLock.ExitWriteLock();
			}
		}

		/// <inheritdoc/>
		public bool Check(string word) {
			return JustCheckCore(word).WordFound;
		}

		private CachedSpellCheckData JustCheckCore(string word)
		{
			_cacheLock.EnterUpgradeableReadLock();
			try {
				CachedSpellCheckData result;
				if (_cache.TryGetValue(word, out result))
					return result;

				_cacheLock.EnterWriteLock();
				try {
					while (_cache.Count >= _cacheMax)
						_cache.Remove(_cache.Keys.First());

					_cache[word] = result = new CachedSpellCheckData(_core.Check(word));
				}
				finally {
					_cacheLock.ExitWriteLock();
				}
				return result;
			}
			finally {
				_cacheLock.ExitUpgradeableReadLock();
			}
		}

		/// <inheritdoc/>
		public string[] GetRecommendations(string word) {
			return GetRecommendationsCore(word).Suggestions.ToArray();
		}

		private CachedSpellCheckData GetRecommendationsCore(string word) {
			_cacheLock.EnterUpgradeableReadLock();
			try {
				CachedSpellCheckData result;
				bool? wordFound = null;
				if(_cache.TryGetValue(word, out result)) {
					if(result.Suggestions != null)
						return result;

					wordFound = result.WordFound;
				}

				_cacheLock.EnterWriteLock();
				try
				{
					if (!wordFound.HasValue)
						wordFound = _core.Check(word);

					while (_cache.Count >= _cacheMax)
						_cache.Remove(_cache.Keys.First()); // TODO: this could be a lot better

					_cache[word] = result = new CachedSpellCheckData(wordFound.Value, _core.GetRecommendations(word));
				}
				finally {
					_cacheLock.ExitWriteLock();
				}
				return result;
			}
			finally {
				_cacheLock.ExitUpgradeableReadLock();
			}
		}

		protected virtual void Dispose(bool includeManagedResources) {
			if(_ownsCore && _core is IDisposable){
				(_core as IDisposable).Dispose();
				_core = null;
			}
		}

		/// <inheritdoc/>
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		~CachedSpellChecker() {
			Dispose(false);
		}
	}
}
