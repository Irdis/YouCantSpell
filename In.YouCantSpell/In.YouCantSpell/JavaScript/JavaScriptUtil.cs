using System;
using System.Text.RegularExpressions;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Psi.JavaScript.Tree;
using JetBrains.ReSharper.Psi.JavaScript.Parsing;

namespace YouCantSpell.ReSharper.JavaScript
{
	public static class JavaScriptUtil
	{

		/// <summary>
		/// Used to extract the string from within a string literal without including the quotes or literal flag symbol.
		/// </summary>
		public static readonly Regex StringLiteralContentParser = new Regex(@"^""(.+)""$", RegexOptions.Compiled);

		public static bool IsKeyword(string word)
		{
			if(String.IsNullOrEmpty(word))
				return false;
			var lowerWord = word.ToLower();
#if (RSHARP6 || RSHARP7)
			return JavaScriptLexerGenerated.IsReserved(lowerWord);
#else
            return false;
#endif
		}

		public static bool IsStringLiteral(ITreeNode node) {
#if RSHARP6
			if (node is IExpression) {
				return (node as IExpression).ConstantValue.IsString();
			}
#else
			if (node is IJavaScriptLiteralExpression) {
				var expression = node as IJavaScriptLiteralExpression;
				return (node as IJavaScriptLiteralExpression).ConstantValueType == ConstantValueTypes.String;
			}
#endif
			return false;
		}

	}
}
