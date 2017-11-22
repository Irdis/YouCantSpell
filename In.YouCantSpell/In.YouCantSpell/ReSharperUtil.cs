using System.Text.RegularExpressions;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Naming.Settings;
using JetBrains.ReSharper.Psi.Tree;

namespace YouCantSpell.ReSharper
{
	public static class ReSharperUtil
	{

		internal static readonly int MaxSuggestions = 13;

		/// <summary>
		/// Locates resharper in-code configuration comments.
		/// </summary>
		public static readonly Regex ReSharperLineRegex = new Regex(@"ReSharper\s+(disable|restore)\s+(.*)", RegexOptions.Compiled);

		/// <summary>
		/// THIS NAMING RULE GIVES US OGRE CAPS
		/// </summary>
		public static readonly NamingRule OgreCaps = new NamingRule{NamingStyleKind = NamingStyleKinds.AA_BB};

		/// <summary>
		/// Extracts the declaration from a tree node.
		/// </summary>
		/// <param name="node">The node to get the declaration from.</param>
		/// <returns>The declaration node.</returns>
		public static IDeclaration GetDeclaration(ITreeNode node) {
			while (null != node) {
				var declaration = node as IDeclaration;
				if (null != declaration)
					return declaration;
				node = node.Parent;
			}
			return null;
		}

		/// <summary>
		/// Extracts a declared element from an node or its parent.
		/// </summary>
		/// <param name="node">The node to extract the declared element from.</param>
		/// <returns>A declared element or null if no declared element is found.</returns>
		/// <remarks>
		/// This method walks up the parent nodes to find a declared element.
		/// </remarks>
		public static IDeclaredElement GetDeclaredElement(ITreeNode node) {
			var declaration = GetDeclaration(node);
			return null == declaration ? null : declaration.DeclaredElement;
		}

	}
}
