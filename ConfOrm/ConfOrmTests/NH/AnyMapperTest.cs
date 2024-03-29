using System;
using System.Linq;
using ConfOrm;
using ConfOrm.NH;
using NHibernate;
using NHibernate.Cfg.MappingSchema;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests.NH
{
	public class AnyMapperTest
	{
		private class MyClass
		{
			public int Id { get; set; }
			public MyReferenceClass MyReferenceClass { get; set; }
		}

		private class MyReferenceClass
		{
			public int Id { get; set; }			
		}

		[Test]
		public void AtCreationSetIdType()
		{
			var hbmMapping = new HbmMapping();
			var hbmAny = new HbmAny();
			new AnyMapper(null, typeof(int), hbmAny, hbmMapping);
			hbmAny.idtype.Should().Be("Int32");
		}

		[Test]
		public void AtCreationSetTheTwoRequiredColumnsNodes()
		{
			var hbmMapping = new HbmMapping();
			var hbmAny = new HbmAny();
			new AnyMapper(null, typeof(int), hbmAny, hbmMapping);
			hbmAny.Columns.Should().Have.Count.EqualTo(2);
			hbmAny.Columns.Select(c => c.name).All(n => n.Satisfy(name=> !string.IsNullOrEmpty(name)));
		}

		[Test]
		public void CanSetIdTypeThroughIType()
		{
			var hbmMapping = new HbmMapping();
			var hbmAny = new HbmAny();
			var mapper = new AnyMapper(null, typeof(int), hbmAny, hbmMapping);
			mapper.IdType(NHibernateUtil.Int64);
			hbmAny.idtype.Should().Be("Int64");
		}

		[Test]
		public void CanSetIdTypeThroughGenericMethod()
		{
			var hbmMapping = new HbmMapping();
			var hbmAny = new HbmAny();
			var mapper = new AnyMapper(null, typeof(int), hbmAny, hbmMapping);
			mapper.IdType<long>();
			hbmAny.idtype.Should().Be("Int64");
		}

		[Test]
		public void CanSetIdTypeThroughType()
		{
			var hbmMapping = new HbmMapping();
			var hbmAny = new HbmAny();
			var mapper = new AnyMapper(null, typeof(int), hbmAny, hbmMapping);
			mapper.IdType(typeof(long));
			hbmAny.idtype.Should().Be("Int64");
		}

		[Test]
		public void CanSetMetaTypeThroughIType()
		{
			var hbmMapping = new HbmMapping();
			var hbmAny = new HbmAny();
			var mapper = new AnyMapper(null, typeof(int), hbmAny, hbmMapping);
			mapper.MetaType(NHibernateUtil.Character);
			hbmAny.MetaType.Should().Be("Char");
		}

		[Test]
		public void CanSetMetaTypeThroughGenericMethod()
		{
			var hbmMapping = new HbmMapping();
			var hbmAny = new HbmAny();
			var mapper = new AnyMapper(null, typeof(int), hbmAny, hbmMapping);
			mapper.MetaType<char>();
			hbmAny.MetaType.Should().Be("Char");
		}

		[Test]
		public void CanSetMetaTypeThroughType()
		{
			var hbmMapping = new HbmMapping();
			var hbmAny = new HbmAny();
			var mapper = new AnyMapper(null, typeof(int), hbmAny, hbmMapping);
			mapper.MetaType(typeof(char));
			hbmAny.MetaType.Should().Be("Char");
		}

		[Test]
		public void CanSetCascade()
		{
			var hbmMapping = new HbmMapping();
			var hbmAny = new HbmAny();
			var mapper = new AnyMapper(null, typeof(int), hbmAny, hbmMapping);
			mapper.Cascade(Cascade.All);
			hbmAny.cascade.Should().Be("all");
		}

		[Test]
		public void AutoCleanInvalidCascade()
		{
			var hbmMapping = new HbmMapping();
			var hbmAny = new HbmAny();
			var mapper = new AnyMapper(null, typeof(int), hbmAny, hbmMapping);
			mapper.Cascade(Cascade.All | Cascade.DeleteOrphans);
			hbmAny.cascade.Should().Be("all");
		}

		[Test]
		public void CanSetIndex()
		{
			var hbmMapping = new HbmMapping();
			var hbmAny = new HbmAny();
			var mapper = new AnyMapper(null, typeof(int), hbmAny, hbmMapping);
			mapper.Index("pizza");
			hbmAny.index.Should().Be("pizza");
		}

		[Test]
		public void CanSetLazy()
		{
			var hbmMapping = new HbmMapping();
			var hbmAny = new HbmAny();
			var mapper = new AnyMapper(null, typeof(int), hbmAny, hbmMapping);
			mapper.Lazy(true);
			hbmAny.lazy.Should().Be(true);
		}

		[Test]
		public void WhenSetIdColumnPropertiesThenWorkOnSameHbmColumnCreatedAtCtor()
		{
			var hbmMapping = new HbmMapping();
			var hbmAny = new HbmAny();
			var mapper = new AnyMapper(null, typeof(int), hbmAny, hbmMapping);
			var columnsBefore = hbmAny.Columns.ToArray();
			mapper.Columns(idcm => idcm.Length(10), metacm => { });
			var columnsAfter = hbmAny.Columns.ToArray();
			columnsBefore[0].Should().Be.SameInstanceAs(columnsAfter[0]);
			columnsBefore[0].length.Should().Be("10");
		}

		[Test]
		public void WhenSetMetaColumnPropertiesThenWorkOnSameHbmColumnCreatedAtCtor()
		{
			var hbmMapping = new HbmMapping();
			var hbmAny = new HbmAny();
			var mapper = new AnyMapper(null, typeof(int), hbmAny, hbmMapping);
			var columnsBefore = hbmAny.Columns.ToArray();
			mapper.Columns(idcm => { }, metacm => metacm.Length(500));
			var columnsAfter = hbmAny.Columns.ToArray();
			columnsBefore[1].Should().Be.SameInstanceAs(columnsAfter[1]);
			columnsBefore[1].length.Should().Be("500");
		}

		[Test]
		public void MetaTypeShouldBeImmutable()
		{
			var hbmMapping = new HbmMapping();
			var hbmAny = new HbmAny();
			var mapper = new AnyMapper(null, typeof(int), hbmAny, hbmMapping);
			mapper.MetaValue('A', typeof(MyReferenceClass));
			ActionAssert.Throws<ArgumentException>(() => mapper.MetaType(NHibernateUtil.Int32));
			ActionAssert.Throws<ArgumentException>(mapper.MetaType<int>);
		}

		[Test]
		public void WhenNullParameterThenThrow()
		{
			var hbmMapping = new HbmMapping();
			var hbmAny = new HbmAny();
			var mapper = new AnyMapper(null, typeof(int), hbmAny, hbmMapping);
			ActionAssert.Throws<ArgumentNullException>(() => mapper.MetaValue(null, typeof (MyReferenceClass)));
			ActionAssert.Throws<ArgumentNullException>(() => mapper.MetaValue('A', null));
		}

		[Test]
		public void WhenSetFirstMetaValueThenSetMetaTypeIfNotClass()
		{
			var hbmMapping = new HbmMapping();
			var hbmAny = new HbmAny();
			var mapper = new AnyMapper(null, typeof(int), hbmAny, hbmMapping);
			mapper.MetaValue('A', typeof (MyReferenceClass));
			hbmAny.MetaType.Should().Be("Char");
		}

		[Test]
		public void WhenSetMetaValueWithClassThenThrow()
		{
			var hbmMapping = new HbmMapping();
			var hbmAny = new HbmAny();
			var mapper = new AnyMapper(null, typeof(int), hbmAny, hbmMapping);
			ActionAssert.Throws<ArgumentOutOfRangeException>(()=>mapper.MetaValue(typeof(MyReferenceClass), typeof(MyReferenceClass)));
		}

		[Test]
		public void WhenSetSecondMetaValueThenCheckCompatibility()
		{
			var hbmMapping = new HbmMapping();
			var hbmAny = new HbmAny();
			var mapper = new AnyMapper(null, typeof(int), hbmAny, hbmMapping);
			mapper.MetaValue('A', typeof(MyReferenceClass));
			ActionAssert.Throws<ArgumentException>(() => mapper.MetaValue(5, typeof(MyClass)));
		}

		[Test]
		public void WhenDuplicatedMetaValueThenRegisterOne()
		{
			var hbmMapping = new HbmMapping();
			var hbmAny = new HbmAny();
			var mapper = new AnyMapper(null, typeof(int), hbmAny, hbmMapping);
			mapper.MetaValue('A', typeof(MyReferenceClass));
			mapper.MetaValue('A', typeof(MyReferenceClass));
			hbmAny.metavalue.Should().Have.Count.EqualTo(1);
		}

		[Test]
		public void WhenDuplicatedMetaValueWithDifferentTypeThenThrow()
		{
			var hbmMapping = new HbmMapping();
			var hbmAny = new HbmAny();
			var mapper = new AnyMapper(null, typeof(int), hbmAny, hbmMapping);
			mapper.MetaValue('A', typeof(MyReferenceClass));
			ActionAssert.Throws<ArgumentException>(() => mapper.MetaValue('A', typeof (MyClass)));
		}

		[Test]
		public void WhenSetTwoMetaValueThenHasTwoMetaValues()
		{
			var hbmMapping = new HbmMapping();
			var hbmAny = new HbmAny();
			var mapper = new AnyMapper(null, typeof(int), hbmAny, hbmMapping);
			mapper.MetaValue('A', typeof(MyReferenceClass));
			mapper.MetaValue('B', typeof(MyClass));
			hbmAny.metavalue.Should().Have.Count.EqualTo(2);
			hbmAny.metavalue.Select(mv => mv.value).Should().Have.SameValuesAs("A", "B");
			hbmAny.metavalue.Select(mv => mv.@class).Satisfy(c => c.Any(clazz => clazz.Contains("MyReferenceClass")));
			hbmAny.metavalue.Select(mv => mv.@class).Satisfy(c => c.Any(clazz => clazz.Contains("MyClass")));
		}

		[Test]
		public void AtCreationSetColumnsUsingMemberName()
		{
			var member = typeof(MyClass).GetProperty("MyReferenceClass");
			var hbmMapping = new HbmMapping();
			var hbmAny = new HbmAny();
			new AnyMapper(member, typeof(int), hbmAny, hbmMapping);
			hbmAny.Columns.ElementAt(0).name.Should().Contain("MyReferenceClass");
			hbmAny.Columns.ElementAt(1).name.Should().Contain("MyReferenceClass");
		}

		[Test]
		public void IdMetaTypeShouldBeImmutableAfterAddMetaValues()
		{
			var hbmMapping = new HbmMapping();
			var hbmAny = new HbmAny();
			var mapper = new AnyMapper(null, typeof(int), hbmAny, hbmMapping);
			mapper.MetaValue('A', typeof(MyReferenceClass));
			ActionAssert.NotThrow(() => mapper.IdType(NHibernateUtil.Int32));
			ActionAssert.NotThrow(mapper.IdType<int>);
			ActionAssert.Throws<ArgumentException>(mapper.IdType<string>);
			ActionAssert.Throws<ArgumentException>(()=> mapper.IdType(NHibernateUtil.String));
		}

		[Test]
		public void CanSetUpdate()
		{
			var hbmMapping = new HbmMapping();
			var hbmAny = new HbmAny();
			var mapper = new AnyMapper(null, typeof(int), hbmAny, hbmMapping);

			mapper.Update(false);
			hbmAny.update.Should().Be.False();
		}

		[Test]
		public void CanSetInsert()
		{
			var hbmMapping = new HbmMapping();
			var hbmAny = new HbmAny();
			var mapper = new AnyMapper(null, typeof(int), hbmAny, hbmMapping);

			mapper.Insert(false);
			hbmAny.insert.Should().Be.False();
		}
	}
}