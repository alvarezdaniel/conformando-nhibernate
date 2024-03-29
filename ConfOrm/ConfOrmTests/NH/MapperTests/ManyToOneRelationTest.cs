using System;
using System.Linq;
using System.Reflection;
using ConfOrm;
using ConfOrm.NH;
using Moq;
using NHibernate.Cfg.MappingSchema;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests.NH.MapperTests
{
	public class ManyToOneRelationTest
	{
		private class AEntity
		{
			public int Id { get; set; }
			public BEntity B { get; set; }
			public string Name { get; set; }
		}

		private class BEntity
		{
			public int Id { get; set; }
		}

		private Mock<IDomainInspector> GetMockedDomainInspector()
		{
			var orm = new Mock<IDomainInspector>();
			orm.Setup(m => m.IsEntity(It.IsAny<Type>())).Returns(true);
			orm.Setup(m => m.IsRootEntity(It.IsAny<Type>())).Returns(true);
			orm.Setup(m => m.IsTablePerClass(It.IsAny<Type>())).Returns(true);
			orm.Setup(m => m.IsPersistentId(It.Is<MemberInfo>(mi => mi.Name == "Id"))).Returns(true);
			orm.Setup(m => m.IsPersistentProperty(It.Is<MemberInfo>(mi => mi.Name != "Id"))).Returns(true);
			orm.Setup(m => m.IsManyToOne(It.Is<Type>(t => t == typeof(AEntity)), It.Is<Type>(t => t == typeof(BEntity)))).Returns(true);
			return orm;
		}

		private HbmMapping GetMapping(IDomainInspector domainInspector)
		{
			var mapper = new Mapper(domainInspector);
			return mapper.CompileMappingFor(new[] { typeof(AEntity), typeof(BEntity) });
		}

		[Test]
		public void MappingContainsClassWithRelation()
		{
			Mock<IDomainInspector> orm = GetMockedDomainInspector();

			var domainInspector = orm.Object;
			HbmMapping mapping = GetMapping(domainInspector);

			VerifyAEntityMapping(mapping);
		}

		private void VerifyAEntityMapping(HbmMapping mapping)
		{
			HbmClass rc = mapping.RootClasses.First(r => r.Name.Contains("AEntity"));
			rc.Properties.Should().Have.Count.EqualTo(2);
			rc.Properties.Select(p => p.Name).Should().Have.SameValuesAs("Name", "B");
			var relation = rc.Properties.First(p => p.Name == "B");
			relation.Should().Be.OfType<HbmManyToOne>();
			var normalProp = rc.Properties.First(p => p.Name == "Name");
			normalProp.Should().Be.OfType<HbmProperty>();
		}

		[Test]
		public void IntegrationWithObjectRelationalMapper()
		{
			var orm = new ObjectRelationalMapper();
			orm.TablePerClass<AEntity>();
			orm.TablePerClass<BEntity>();
			orm.ManyToOne<AEntity, BEntity>();
			HbmMapping mapping = GetMapping(orm);

			VerifyAEntityMapping(mapping);
		}
	}
}