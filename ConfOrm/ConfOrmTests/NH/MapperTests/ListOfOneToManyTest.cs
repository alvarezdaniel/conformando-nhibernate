using System;
using System.Collections.Generic;
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
	public class ListOfOneToManyTest
	{
		private class Person
		{
			public int Id { get; set; }
			public IList<Animal> Pets { get; set; }
		}

		private class Animal
		{
			public int Id { get; set; }
		}

		private Mock<IDomainInspector> GetMockedDomainInspector()
		{
			var orm = new Mock<IDomainInspector>();
			orm.Setup(m => m.IsEntity(It.Is<Type>(t => t == typeof(Person) || t == typeof(Animal)))).Returns(true);
			orm.Setup(m => m.IsRootEntity(It.Is<Type>(t => t == typeof(Person) || t == typeof(Animal)))).Returns(true);
			orm.Setup(m => m.IsTablePerClass(It.IsAny<Type>())).Returns(true);
			orm.Setup(m => m.IsPersistentId(It.Is<MemberInfo>(mi => mi.Name == "Id"))).Returns(true);
			orm.Setup(m => m.IsPersistentProperty(It.Is<MemberInfo>(mi => mi.Name != "Id"))).Returns(true);
			orm.Setup(m => m.IsOneToMany(It.Is<Type>(t => t == typeof(Person)), It.Is<Type>(t => t == typeof(Animal)))).Returns(true);
			orm.Setup(m => m.IsList(It.Is<MemberInfo>(p => p == typeof(Person).GetProperty("Pets")))).Returns(true);
			return orm;
		}

		private HbmMapping GetMapping(IDomainInspector domainInspector)
		{
			var mapper = new Mapper(domainInspector);
			return mapper.CompileMappingFor(new[] { typeof(Person), typeof(Animal) });
		}

		[Test]
		public void MappingThroughMock()
		{
			Mock<IDomainInspector> orm = GetMockedDomainInspector();

			var domainInspector = orm.Object;
			HbmMapping mapping = GetMapping(domainInspector);

			VerifyMapping(mapping);
		}

		private void VerifyMapping(HbmMapping mapping)
		{
			HbmClass rc = mapping.RootClasses.First(r => r.Name.Contains("Person"));
			rc.Properties.Should().Have.Count.EqualTo(1);
			var relation = rc.Properties.First(p => p.Name == "Pets");
			relation.Should().Be.OfType<HbmList>();
			var collection = (HbmList)relation;
			collection.ElementRelationship.Should().Be.OfType<HbmOneToMany>();
			var elementRelation = (HbmOneToMany)collection.ElementRelationship;
			elementRelation.Class.Should().Not.Be.Null();
			elementRelation.Class.Should().Contain("Animal");
		}

		[Test]
		public void IntegrationWithObjectRelationalMapper()
		{
			var orm = new ObjectRelationalMapper();
			orm.TablePerClass<Person>();
			orm.TablePerClass<Animal>();
			orm.ManyToOne<Person, Animal>();
			HbmMapping mapping = GetMapping(orm);

			VerifyMapping(mapping);
		}

	}
}