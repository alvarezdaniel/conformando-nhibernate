using System;
using ConfOrm;
using ConfOrm.NH;
using NHibernate.Cfg.MappingSchema;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests.NH
{
	public class SubclassMapperTest
	{
		private class EntitySimple
		{
			public int Id { get; set; }
		}

		private class Inherited : EntitySimple
		{
		}

		private class Inherited2 : Inherited { }

		private class Z
		{
			
		}
		[Test]
		public void WhenMapDocHasDefaultSubclassElementHasClassName()
		{
			var subClass = typeof(Inherited);
			var mapdoc = new HbmMapping { assembly = subClass.Assembly.FullName, @namespace = subClass.Namespace };
			new SubclassMapper(subClass, mapdoc);
			mapdoc.SubClasses[0].Name.Should().Be.EqualTo(typeof (Inherited).Name);
		}

		[Test]
		public void WhenMapDocHasDefaultSubclassElementHasExtendsClassName()
		{
			var subClass = typeof(Inherited);
			var mapdoc = new HbmMapping { assembly = subClass.Assembly.FullName, @namespace = subClass.Namespace };
			new SubclassMapper(subClass, mapdoc);
			mapdoc.SubClasses[0].extends.Should().Be.EqualTo(typeof(EntitySimple).Name);
		}

		[Test]
		public void SetEntityName()
		{
			var subClass = typeof(Inherited);
			var mapdoc = new HbmMapping();
			var mapper = new SubclassMapper(subClass, mapdoc);
			mapper.EntityName("pepe");

			var hbmEntity = mapdoc.SubClasses[0];
			hbmEntity.EntityName.Should().Be("pepe");
		}

		[Test]
		public void SetProxy()
		{
			var subClass = typeof(Inherited);
			var mapdoc = new HbmMapping();
			var mapper = new SubclassMapper(subClass, mapdoc);
			mapper.Proxy(subClass);

			var hbmEntity = mapdoc.SubClasses[0];
			hbmEntity.Proxy.Should().Contain("Inherited");
		}

		[Test]
		public void SetWrongProxy()
		{
			var subClass = typeof(Inherited);
			var mapdoc = new HbmMapping();
			var mapper = new SubclassMapper(subClass, mapdoc);
			ActionAssert.Throws<MappingException>(() => mapper.Proxy(typeof(Z)));
		}

		[Test]
		public void SetLazy()
		{
			var subClass = typeof(Inherited);
			var mapdoc = new HbmMapping();
			var mapper = new SubclassMapper(subClass, mapdoc);
			mapper.Lazy(true);

			var hbmEntity = mapdoc.SubClasses[0];

			hbmEntity.UseLazy.Should().Not.Have.Value();

			mapper.Lazy(false);
			hbmEntity.UseLazy.Should().Be(false);
		}

		[Test]
		public void SetDynamicUpdate()
		{
			var subClass = typeof(Inherited);
			var mapdoc = new HbmMapping();
			var mapper = new SubclassMapper(subClass, mapdoc);
			mapper.DynamicUpdate(true);

			var hbmEntity = mapdoc.SubClasses[0];

			hbmEntity.DynamicUpdate.Should().Be(true);
		}

		[Test]
		public void SetDynamicInsert()
		{
			var subClass = typeof(Inherited);
			var mapdoc = new HbmMapping();
			var mapper = new SubclassMapper(subClass, mapdoc);
			mapper.DynamicInsert(true);

			var hbmEntity = mapdoc.SubClasses[0];

			hbmEntity.DynamicInsert.Should().Be(true);
		}

		[Test]
		public void SetBatchSize()
		{
			var subClass = typeof(Inherited);
			var mapdoc = new HbmMapping();
			var mapper = new SubclassMapper(subClass, mapdoc);
			mapper.BatchSize(10);

			var hbmEntity = mapdoc.SubClasses[0];

			hbmEntity.BatchSize.Should().Be(10);
		}

		[Test]
		public void SetSelectBeforeUpdate()
		{
			var subClass = typeof(Inherited);
			var mapdoc = new HbmMapping();
			var mapper = new SubclassMapper(subClass, mapdoc);
			mapper.SelectBeforeUpdate(true);

			var hbmEntity = mapdoc.SubClasses[0];

			hbmEntity.SelectBeforeUpdate.Should().Be(true);
		}

		[Test]
		public void SetLoader()
		{
			var subClass = typeof(Inherited);
			var mapdoc = new HbmMapping();
			var mapper = new SubclassMapper(subClass, mapdoc);
			mapper.Loader("blah");

			var hbmEntity = mapdoc.SubClasses[0];

			hbmEntity.SqlLoader.Should().Not.Be.Null();
			hbmEntity.SqlLoader.queryref.Should().Be("blah");
		}

		[Test]
		public void SetSqlInsert()
		{
			var subClass = typeof(Inherited);
			var mapdoc = new HbmMapping();
			var mapper = new SubclassMapper(subClass, mapdoc);
			mapper.SqlInsert("blah");

			var hbmEntity = mapdoc.SubClasses[0];

			hbmEntity.SqlInsert.Should().Not.Be.Null();
			hbmEntity.SqlInsert.Text[0].Should().Be("blah");
		}

		[Test]
		public void SetSqlUpdate()
		{
			var subClass = typeof(Inherited);
			var mapdoc = new HbmMapping();
			var mapper = new SubclassMapper(subClass, mapdoc);
			mapper.SqlUpdate("blah");

			var hbmEntity = mapdoc.SubClasses[0];

			hbmEntity.SqlUpdate.Should().Not.Be.Null();
			hbmEntity.SqlUpdate.Text[0].Should().Be("blah");
		}

		[Test]
		public void SetSqlDelete()
		{
			var subClass = typeof(Inherited);
			var mapdoc = new HbmMapping();
			var mapper = new SubclassMapper(subClass, mapdoc);
			mapper.SqlDelete("blah");

			var hbmEntity = mapdoc.SubClasses[0];

			hbmEntity.SqlDelete.Should().Not.Be.Null();
			hbmEntity.SqlDelete.Text[0].Should().Be("blah");
		}

		[Test]
		public void SetDiscriminatorValue()
		{
			var subClass = typeof(Inherited);
			var mapdoc = new HbmMapping();
			var mapper = new SubclassMapper(subClass, mapdoc);
			mapper.DiscriminatorValue("blah");

			var hbmEntity = mapdoc.SubClasses[0];

			hbmEntity.DiscriminatorValue.Should().Be("blah");

			mapper.DiscriminatorValue(null);
			hbmEntity.DiscriminatorValue.Should().Be("null");
		}

		[Test]
		public void WhenSetExtendsExplicitlyThenSetDifferentBaseType()
		{
			var subClass = typeof(Inherited2);
			var mapdoc = new HbmMapping { assembly = subClass.Assembly.FullName, @namespace = subClass.Namespace };
			var mapper = new JoinedSubclassMapper(subClass, mapdoc);
			mapper.Extends(typeof(EntitySimple));
			mapdoc.JoinedSubclasses[0].extends.Should().Be.EqualTo(typeof(EntitySimple).Name);
		}

		[Test]
		public void WhenNoSetExtendsExplicitlyThenSetBaseType()
		{
			var subClass = typeof(Inherited2);
			var mapdoc = new HbmMapping { assembly = subClass.Assembly.FullName, @namespace = subClass.Namespace };
			new JoinedSubclassMapper(subClass, mapdoc);
			mapdoc.JoinedSubclasses[0].extends.Should().Be.EqualTo(typeof(Inherited).Name);
		}

		[Test]
		public void WhenSetExtendsWithWrongBaseTypeThenThrow()
		{
			var subClass = typeof(Inherited);
			var mapdoc = new HbmMapping { assembly = subClass.Assembly.FullName, @namespace = subClass.Namespace };
			var mapper = new JoinedSubclassMapper(subClass, mapdoc);
			ActionAssert.Throws<ArgumentOutOfRangeException>(() => mapper.Extends(typeof(Z)));
			ActionAssert.Throws<ArgumentOutOfRangeException>(() => mapper.Extends(typeof(Inherited2)));
		}
	}
}