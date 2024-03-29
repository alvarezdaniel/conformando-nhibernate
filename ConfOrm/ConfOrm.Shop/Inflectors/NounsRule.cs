using System.Text.RegularExpressions;

namespace ConfOrm.Shop.Inflectors
{
	public class NounsRule : AbstractInflectorRule
	{
		public NounsRule(string pattern, string replacement) : base(pattern, replacement) { }

		protected override Regex CreateRegex()
		{
			return new Regex(Pattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);
		}

		public override string Apply(string word)
		{
			if (!Regex.IsMatch(word))
			{
				return null;
			}

			return Regex.Replace(word, Replacement);
		}
	}
}