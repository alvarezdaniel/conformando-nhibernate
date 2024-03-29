using System.Collections.Generic;
using System.Reflection;
using ConfOrm.Patterns;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests.Patterns
{
	public class ArrayCollectionPatternTest
	{
		private class Entity
		{
			private ICollection<string> others;
			private string[] emails;
			public string[] NickNames { get; set; }
			public byte[] Photo { get; set; }

			public ICollection<string> Emails
			{
				get { return emails; }
			}

			public ICollection<string> Others
			{
				get { return others; }
			}
			public IList<string> NoArray
			{
				get { return null; }
			}
			public string Simple { get; set; }
		}

		[Test]
		public void MatchWithArrayProperty()
		{
			var mi = typeof(Entity).GetProperty("NickNames");
			var p = new ArrayCollectionPattern();
			p.Match(mi).Should().Be.True();
		}

		[Test]
		public void MatchWithArrayField()
		{
			var mi = typeof(Entity).GetField("emails", BindingFlags.NonPublic | BindingFlags.Instance);
			var p = new ArrayCollectionPattern();
			p.Match(mi).Should().Be.True();
		}

		[Test]
		public void MatchWithCollectionPropertyAndArrayField()
		{
			var mi = typeof(Entity).GetProperty("Emails");
			var p = new ArrayCollectionPattern();
			p.Match(mi).Should().Be.True();
		}

		[Test]
		public void NotMatchWithCollectionField()
		{
			var mi = typeof(Entity).GetField("others", BindingFlags.NonPublic | BindingFlags.Instance);
			var p = new ArrayCollectionPattern();
			p.Match(mi).Should().Be.False();
		}

		[Test]
		public void NotMatchWithCollectionProperty()
		{
			var mi = typeof(Entity).GetProperty("Others");
			var p = new ArrayCollectionPattern();
			p.Match(mi).Should().Be.False();
		}

		[Test]
		public void NotMatchWithNoArrayCollectionProperty()
		{
			var mi = typeof(Entity).GetProperty("NoArray");
			var p = new ArrayCollectionPattern();
			p.Match(mi).Should().Be.False();
		}

		[Test]
		public void NotMatchWithStringProperty()
		{
			var mi = typeof(Entity).GetProperty("Simple");
			var p = new ArrayCollectionPattern();
			p.Match(mi).Should().Be.False();
		}

		[Test]
		public void NotMatchWithByteArrayProperty()
		{
			var mi = typeof(Entity).GetProperty("Photo");
			var p = new ArrayCollectionPattern();
			p.Match(mi).Should().Be.False();
		}
	}
}