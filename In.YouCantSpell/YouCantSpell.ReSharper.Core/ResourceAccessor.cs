using System.Globalization;
using System.Resources;

namespace YouCantSpell.ReSharper
{
	public static class ResourceAccessor
	{
		private static readonly ResourceManager TextResourceManager = new ResourceManager("YouCantSpell.ReSharper.Text", typeof(ResourceAccessor).Assembly);

		public static string GetString(string name) {
			return TextResourceManager.GetString(name);
		}

		public static string GetString(string name, CultureInfo culture) {
			return TextResourceManager.GetString(name, culture);
		}

	}
}
