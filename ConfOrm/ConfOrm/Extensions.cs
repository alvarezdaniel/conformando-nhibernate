namespace ConfOrm
{
	public static class Extensions
	{
		public static bool Has(this Cascade source, Cascade value)
		{
			return (source & value) == value;
		}
	}
}