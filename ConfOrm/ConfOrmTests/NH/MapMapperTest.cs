using System.Collections.Generic;
using System.Linq;
using ConfOrm.Mappers;
using ConfOrm.NH;
using NHibernate.Cfg.MappingSchema;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests.NH
{
	public class MapMapperTest
	{
		private class Animal
		{
			public int Id { get; set; }
			private IDictionary<string, Animal> children;
			public IDictionary<string, Animal> Children
			{
				get { return children; }
			}
		}

		[Test]
		public void SetMapKey()
		{
			var mapdoc = new HbmMapping();
			var hbm = new HbmMap();
			new MapMapper(typeof (Animal), typeof (string), typeof (Animal), hbm, mapdoc);
			hbm.Item.Should().Not.Be.Null().And.Be.OfType<HbmMapKey>();
		}

		[Test]
		public void SetInverse()
		{
			var mapdoc = new HbmMapping();
			var hbm = new HbmMap();
			var mapper = new MapMapper(typeof(Animal), typeof(string), typeof(Animal), hbm, mapdoc);
			mapper.Inverse(true);
			hbm.Inverse.Should().Be.True();
			mapper.Inverse(false);
			hbm.Inverse.Should().Be.False();
		}

		[Test]
		public void SetMutable()
		{
			var mapdoc = new HbmMapping();
			var hbm = new HbmMap();
			var mapper = new MapMapper(typeof(Animal), typeof(string), typeof(Animal), hbm, mapdoc);
			mapper.Mutable(true);
			hbm.Mutable.Should().Be.True();
			mapper.Mutable(false);
			hbm.Mutable.Should().Be.False();
		}

		[Test]
		public void SetWhere()
		{
			var mapdoc = new HbmMapping();
			var hbm = new HbmMap();
			var mapper = new MapMapper(typeof(Animal), typeof(string), typeof(Animal), hbm, mapdoc);
			mapper.Where("c > 10");
			hbm.Where.Should().Be.EqualTo("c > 10");
		}

		[Test]
		public void SetBatchSize()
		{
			var mapdoc = new HbmMapping();
			var hbm = new HbmMap();
			var mapper = new MapMapper(typeof(Animal), typeof(string), typeof(Animal), hbm, mapdoc);
			mapper.BatchSize(10);
			hbm.BatchSize.Should().Be.EqualTo(10);
		}

		[Test]
		public void SetLazy()
		{
			var mapdoc = new HbmMapping();
			var hbm = new HbmMap();
			var mapper = new MapMapper(typeof(Animal), typeof(string), typeof(Animal), hbm, mapdoc);
			mapper.Lazy(CollectionLazy.Extra);
			hbm.Lazy.Should().Be.EqualTo(HbmCollectionLazy.Extra);
			mapper.Lazy(CollectionLazy.NoLazy);
			hbm.Lazy.Should().Be.EqualTo(HbmCollectionLazy.False);
			mapper.Lazy(CollectionLazy.Lazy);
			hbm.Lazy.Should().Be.EqualTo(HbmCollectionLazy.True);
		}

		[Test]
		public void SetSort()
		{
			var mapdoc = new HbmMapping();
			var hbm = new HbmMap();
			var mapper = new MapMapper(typeof(Animal), typeof(string), typeof(Animal), hbm, mapdoc);
			mapper.Sort();
			hbm.Sort.Should().Be.EqualTo("natural");
		}

		[Test]
		public void CallKeyMapper()
		{
			var mapdoc = new HbmMapping();
			var hbm = new HbmMap();
			var mapper = new MapMapper(typeof(Animal), typeof(string), typeof(Animal), hbm, mapdoc);
			bool kmCalled = false;
			mapper.Key(km => kmCalled = true);
			hbm.Key.Should().Not.Be.Null();
			kmCalled.Should().Be.True();
		}

		[Test]
		public void WhenMapKeyTypeIsNotPrimitiveTypeThenKeyIsMapKeyManyToManyWithClassAssigned()
		{
			var mapdoc = new HbmMapping();
			var hbm = new HbmMap();
			new MapMapper(typeof(Animal), typeof(Animal), typeof(string), hbm, mapdoc);
			hbm.Item.Should().Not.Be.Null().And.Be.OfType<HbmMapKeyManyToMany>();
			var mapKey = (HbmMapKeyManyToMany)hbm.Item;
			mapKey.Class.Should().Contain(typeof(Animal).Name);
		}

		[Test]
		public void SetCollectionTypeByGenericType()
		{
			var mapdoc = new HbmMapping();
			var hbm = new HbmMap();
			var mapper = new MapMapper(typeof(Animal), typeof(Animal), typeof(string), hbm, mapdoc);
			mapper.Type<FakeUserCollectionType>();
			hbm.CollectionType.Should().Contain("FakeUserCollectionType");
		}

		[Test]
		public void SetCollectionTypeByType()
		{
			var mapdoc = new HbmMapping();
			var hbm = new HbmMap();
			var mapper = new MapMapper(typeof(Animal), typeof(Animal), typeof(string), hbm, mapdoc);
			mapper.Type(typeof(FakeUserCollectionType));
			hbm.CollectionType.Should().Contain("FakeUserCollectionType");
		}

		[Test]
		public void CanChangeAccessor()
		{
			var mapdoc = new HbmMapping();
			var hbm = new HbmMap { name = "Children" };
			var mapper = new MapMapper(typeof(Animal), typeof(Animal), typeof(string), hbm, mapdoc);
			mapper.Access(Accessor.Field);

			hbm.Access.Should().Not.Be.Null();
		}

		[Test]
		public void CanSetCache()
		{
			var mapdoc = new HbmMapping();
			var hbm = new HbmMap();
			var mapper = new MapMapper(typeof(Animal), typeof(Animal), typeof(string), hbm, mapdoc);
			mapper.Cache(x => x.Region("pizza"));

			hbm.cache.Should().Not.Be.Null();
		}

		[Test]
		public void WhenSetTwoCachePropertiesInTwoActionsThenSetTheTwoValuesWithoutLostTheFirst()
		{
			var mapdoc = new HbmMapping();
			var hbm = new HbmMap();
			var mapper = new MapMapper(typeof(Animal), typeof(Animal), typeof(string), hbm, mapdoc);
			mapper.Cache(ch => ch.Region("pizza"));
			mapper.Cache(ch => ch.Usage(CacheUsage.NonstrictReadWrite));

			var hbmCache = hbm.cache;
			hbmCache.Should().Not.Be.Null();
			hbmCache.region.Should().Be("pizza");
			hbmCache.usage.Should().Be(HbmCacheUsage.NonstrictReadWrite);
		}

		[Test]
		public void CanSetAFilterThroughAction()
		{
			var mapdoc = new HbmMapping();
			var hbm = new HbmMap();
			var mapper = new MapMapper(typeof(Animal), typeof(Animal), typeof(string), hbm, mapdoc);
			mapper.Filter("filter1", f => f.Condition("condition1"));
			hbm.filter.Length.Should().Be(1);
			hbm.filter[0].Satisfy(f => f.name == "filter1" && f.condition == "condition1");
		}

		[Test]
		public void CanSetMoreFiltersThroughAction()
		{
			var mapdoc = new HbmMapping();
			var hbm = new HbmMap();
			var mapper = new MapMapper(typeof(Animal), typeof(Animal), typeof(string), hbm, mapdoc);
			mapper.Filter("filter1", f => f.Condition("condition1"));
			mapper.Filter("filter2", f => f.Condition("condition2"));
			hbm.filter.Length.Should().Be(2);
			hbm.filter.Satisfy(filters => filters.Any(f => f.name == "filter1" && f.condition == "condition1"));
			hbm.filter.Satisfy(filters => filters.Any(f => f.name == "filter2" && f.condition == "condition2"));
		}

		[Test]
		public void WhenSameNameThenOverrideCondition()
		{
			var mapdoc = new HbmMapping();
			var hbm = new HbmMap();
			var mapper = new MapMapper(typeof(Animal), typeof(Animal), typeof(string), hbm, mapdoc);
			mapper.Filter("filter1", f => f.Condition("condition1"));
			mapper.Filter("filter2", f => f.Condition("condition2"));
			mapper.Filter("filter1", f => f.Condition("anothercondition1"));
			hbm.filter.Length.Should().Be(2);
			hbm.filter.Satisfy(filters => filters.Any(f => f.name == "filter1" && f.condition == "anothercondition1"));
			hbm.filter.Satisfy(filters => filters.Any(f => f.name == "filter2" && f.condition == "condition2"));
		}

		[Test]
		public void WhenActionIsNullThenAddFilterName()
		{
			var mapdoc = new HbmMapping();
			var hbm = new HbmMap();
			var mapper = new MapMapper(typeof(Animal), typeof(Animal), typeof(string), hbm, mapdoc);
			mapper.Filter("filter1", null);
			hbm.filter.Length.Should().Be(1);
			hbm.filter[0].Satisfy(f => f.name == "filter1" && f.condition == null);
		}

		[Test]
		public void SetFetchMode()
		{
			var mapdoc = new HbmMapping();
			var hbm = new HbmMap();
			var mapper = new MapMapper(typeof(Animal), typeof(Animal), typeof(string), hbm, mapdoc);
			mapper.Fetch(CollectionFetchMode.Subselect);
			hbm.fetch.Should().Be(HbmCollectionFetchMode.Subselect);
			hbm.fetchSpecified.Should().Be.True();
			mapper.Fetch(CollectionFetchMode.Join);
			hbm.fetch.Should().Be(HbmCollectionFetchMode.Join);
			hbm.fetchSpecified.Should().Be.True();
			mapper.Fetch(CollectionFetchMode.Select);
			hbm.fetch.Should().Be(HbmCollectionFetchMode.Select);
			hbm.fetchSpecified.Should().Be.False();
		}
	}
}