using System.Collections.Generic;
using System.Linq;

namespace YouCantSpell.Utility
{
	public static class CollectionUtility
	{

		public static bool NotNullAndHasAny<T>(this IEnumerable<T> items) {
			return null != items && items.Any();
		}

		public static bool NotNullAndHasAny<T>(this T[] items) {
			return null != items && 0 != items.Length;
		}

	}
}
