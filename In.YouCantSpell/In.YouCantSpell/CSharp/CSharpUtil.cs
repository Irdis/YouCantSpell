namespace YouCantSpell.ReSharper.CSharp
{
  using System.Collections.Generic;
  using System.Text.RegularExpressions;
  using JetBrains.ReSharper.Psi.CSharp.Parsing;

  public static class CSharpUtil
  {
    /// <summary>
    /// Used to extract the string from within a string literal without including the quotes or literal flag symbol.
    /// </summary>
    public static readonly Regex StringLiteralContentParser = new Regex(@"^[@]?""(.+)""$", RegexOptions.Compiled | RegexOptions.Singleline);

    /// <summary>
    /// A list of XML Documentation tag names.
    /// </summary>
    private static readonly HashSet<string> XmlDocTagNames = new HashSet<string> 
    { 
      "c", 
      "code", 
      "example", 
      "exception", 
      "include", 
      "list", 
      "para", 
      "param", 
      "paramref", 
      "permission", 
      "remarks", 
      "returns", 
      "see", 
      "seealso", 
      "summary", 
      "value", 
      "typeparam", 
      "cref", 
      "inheritdoc", 
      "filterpriority", 
      "langword" 
    };

    /// <summary>
    /// Determines if the word is a C# or XmlDoc keyword.
    /// </summary>
    /// <param name="word">The word to test.</param>
    /// <returns>True if a keyword.</returns>
    public static bool IsKeyword(string word)
    {
      if (string.IsNullOrEmpty(word))
      {
        return false;
      }

      var lowerWord = word.ToLower();
      return CSharpLexer.IsKeyword(lowerWord) || XmlDocTagNames.Contains(lowerWord);
    }

  }
}
