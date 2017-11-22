namespace YouCantSpell
{
	public interface ITextSubString
	{

		string Source { get; }
		string SubText { get; }
		int Offset { get; }
		int Length { get; }
		int EndOffset { get; }

	}
}
