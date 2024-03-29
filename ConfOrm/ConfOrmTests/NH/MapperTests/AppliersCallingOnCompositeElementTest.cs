using System;
using System.Collections.Generic;
using System.Reflection;
using ConfOrm;
using ConfOrm.NH;
using Moq;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests.NH.MapperTests
{
	public class AppliersCallingOnCompositeElementTest
	{
		private class MyClass
		{
			public int Id { get; set; }
			public ICollection<MyComponent> Components { get; set; }
		}

		private class MyComponent
		{
			public string Simple { get; set; }
			public MyManyToOneRelated ManyToOne { get; set; }
			public MyNestedComponent NestedComponent { get; set; }
		}

		private class MyManyToOneRelated
		{
			public int Id { get; set; }
		}

		private class MyNestedComponent
		{
			public string ReSimple { get; set; }
			public MyManyToOneRelated ReManyToOne { get; set; }
		}

		private Mock<IDomainInspector> GetMockedDomainInspector()
		{
			var orm = new Mock<IDomainInspector>();
			orm.Setup(m => m.IsEntity(It.Is<Type>(t => t == typeof(MyClass)))).Returns(true);
			orm.Setup(m => m.IsRootEntity(It.Is<Type>(t => t == typeof(MyClass)))).Returns(true);
			orm.Setup(m => m.IsTablePerClass(It.IsAny<Type>())).Returns(true);
			orm.Setup(m => m.IsPersistentId(It.Is<MemberInfo>(mi => mi.Name == "Id"))).Returns(true);
			orm.Setup(m => m.IsPersistentProperty(It.Is<MemberInfo>(mi => mi.Name != "Id"))).Returns(true);
			orm.Setup(m => m.IsComponent(It.Is<Type>(t => t == typeof(MyComponent) || t == typeof(MyNestedComponent)))).Returns(true);
			orm.Setup(m => m.IsBag(It.Is<MemberInfo>(p => p == typeof(MyClass).GetProperty("Components")))).Returns(true);
			orm.Setup(m => m.IsManyToOne(It.Is<Type>(t => t == typeof(MyComponent)), It.Is<Type>(t => t == typeof(MyManyToOneRelated)))).Returns(true);
			orm.Setup(m => m.IsManyToOne(It.Is<Type>(t => t == typeof(MyNestedComponent)), It.Is<Type>(t => t == typeof(MyManyToOneRelated)))).Returns(true);
			return orm;
		}

		[Test]
		public void CallPatternAppliersOnCompositeElements()
		{
			Mock<IDomainInspector> orm = GetMockedDomainInspector();
			var mapper = new Mapper(orm.Object);
			bool simplePropertyCalled = false;
			bool manyToOneCalled = false;

			mapper.AddPropertyPattern(mi => mi.DeclaringType == typeof (MyComponent), pm => simplePropertyCalled = true);
			mapper.AddManyToOnePattern(mi => mi.DeclaringType == typeof(MyComponent), pm => manyToOneCalled = true);


			mapper.CompileMappingFor(new[] { typeof(MyClass) });
			simplePropertyCalled.Should().Be.True();
			manyToOneCalled.Should().Be.True();
		}

		[Test]
		public void CallPatternAppliersOnNestedCompositeElements()
		{
			Mock<IDomainInspector> orm = GetMockedDomainInspector();
			var mapper = new Mapper(orm.Object);
			bool reSimplePropertyCalled = false;
			bool reManyToOneCalled = false;

			mapper.AddPropertyPattern(mi => mi.DeclaringType == typeof(MyNestedComponent), pm => reSimplePropertyCalled = true);
			mapper.AddManyToOnePattern(mi => mi.DeclaringType == typeof(MyNestedComponent), pm => reManyToOneCalled = true);

			mapper.CompileMappingFor(new[] { typeof(MyClass) });
			reSimplePropertyCalled.Should().Be.True();
			reManyToOneCalled.Should().Be.True();
		}
	}
}