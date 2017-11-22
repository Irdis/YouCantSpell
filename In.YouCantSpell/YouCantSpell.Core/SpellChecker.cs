using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NHunspell;

namespace YouCantSpell
{
	/// <summary>
	/// The primary spell checker based on NHunspell
	/// </summary>
	public class SpellChecker : ISpellChecker, IDisposable
	{

		private const string DicFolderName = "dic";

		private static IEnumerable<string> GetPluginFolderSearchPaths()
		{
			var assembly = typeof (SpellChecker).Assembly;
			yield return Path.GetDirectoryName(assembly.Location);
			var codeBaseUri = new Uri(assembly.CodeBase);
			yield return Path.GetDirectoryName(codeBaseUri.LocalPath);
			yield return Path.GetDirectoryName(codeBaseUri.AbsolutePath);
		}

		private static bool PathContainsDictionaryFolder(string path)
		{
			if(String.IsNullOrEmpty(path) || !Directory.Exists(path))
				return false;
			var dicDir = Path.Combine(path, DicFolderName);
			return Directory.Exists(dicDir) && Directory.GetFiles(dicDir, "*.dic").Any();
		}

		public static string FindDictionaryPath()
		{
			var coreFolder = GetPluginFolderSearchPaths().FirstOrDefault(PathContainsDictionaryFolder);
			if (String.IsNullOrEmpty(coreFolder))
				return null;
			return Path.Combine(coreFolder, DicFolderName);
		}

		private static bool PathContainsNativeHunspell(string path)
		{
			return Directory.Exists(path) && Directory.GetFiles(path, "Hunspellx*.dll").Any();
		}

		public static List<string> GetAllAvailableDictionaryNames(){
			var dictionaryFolder = FindDictionaryPath();
			if (String.IsNullOrEmpty(dictionaryFolder))
				return null;

			var di = new DirectoryInfo(dictionaryFolder);
			var affFiles = di.GetFiles("*.aff").Select(x => Path.GetFileNameWithoutExtension(x.Name));
			var dicFiles = new HashSet<string>(di.GetFiles("*.dic").Select(x => Path.GetFileNameWithoutExtension(x.Name)), StringComparer.OrdinalIgnoreCase);

			return affFiles.Where(dicFiles.Contains).ToList();
		}

		static SpellChecker()
		{
			var possiblePath = GetPluginFolderSearchPaths().FirstOrDefault(PathContainsNativeHunspell);
			if(null != possiblePath)
				Hunspell.NativeDllPath = possiblePath;
		}

		private Hunspell _core;
		private string _dicName;
		private string _dicFilePath;
		private string _affFilePath;

		/// <summary>
		/// Constructs a new NHunspell spell checker.
		/// </summary>
		public SpellChecker(string name = "EN_US")
		{
			var dictionaryFolder = FindDictionaryPath();
			if(String.IsNullOrEmpty(dictionaryFolder))
				throw new FileNotFoundException("Dictionary folder not found!");

			var targetDicFilePath = Path.Combine(dictionaryFolder, Path.ChangeExtension(name, "dic"));
			if(!File.Exists(targetDicFilePath))
				throw new InvalidOperationException("Dictionary not found(.dic): " + name);

			var targetAffFilePath = Path.Combine(dictionaryFolder, Path.ChangeExtension(name, "aff"));
			if(!File.Exists(targetAffFilePath))
				throw new InvalidOperationException("Dictionary not found(.aff): " + name);

			_dicFilePath = targetDicFilePath;
			_affFilePath = targetAffFilePath;
			_dicName = name;

			_core = new Hunspell(
				targetAffFilePath,
				targetDicFilePath
			);
		}

		/// <inheritdoc/>
		public void Add(string word) {
			if(!_core.Add(word)) {
				_core.Add(word.ToUpper());
				_core.Add(word.ToLower());
			}
		}

		/// <inheritdoc/>
		public void Add(IEnumerable<string> words) {
			foreach(var word in words)
				Add(word);
		}

		/// <inheritdoc/>
		public bool Check(string word) {
			return _core.Spell(word);
		}

		/// <inheritdoc/>
		public string[] GetRecommendations(string word) {
			return _core.Suggest(word).ToArray();
		}

		protected virtual void Dispose(bool includeManagedResources) {
			if (null != _core) {
				_core.Dispose();
				_core = null;
			}
		}

		/// <inheritdoc/>
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		~SpellChecker() {
			Dispose(false);
		}

	}
}
