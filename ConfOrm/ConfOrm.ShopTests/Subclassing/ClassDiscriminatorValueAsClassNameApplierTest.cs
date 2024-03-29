using System;
using ConfOrm.Mappers;
using ConfOrm.Shop.Subclassing;
using Moq;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrm.ShopTests.Subclassing
{
	public class ClassDiscriminatorValueAsClassNameApplierTest
	{
		private class MyClass
		{
		}
		[Test]
		public void WhenEntityIsNotTablePerHierarchyThenNoMatch()
		{
			var orm = new Mock<IDomainInspector>();
			orm.Setup(x => x.IsTablePerClassHierarchy(typeof(MyClass))).Returns(false);
			var applier = new ClassDiscriminatorValueAsClassNameApplier(orm.Object);
			applier.Match(typeof(MyClass)).Should().Be.False();
		}

		[Test]
		public void WhenEntityIsTablePerHierarchyThenMatch()
		{
			var orm = new Mock<IDomainInspector>();
			orm.Setup(x => x.IsTablePerClassHierarchy(typeof(MyClass))).Returns(true);
			var applier = new ClassDiscriminatorValueAsClassNameApplier(orm.Object);
			applier.Match(typeof(MyClass)).Should().Be.True();
		}

		[Test]
		public void AlwaysApplyDicriminatorValueAsClassName()
		{
			var orm = new Mock<IDomainInspector>();
			var applier = new ClassDiscriminatorValueAsClassNameApplier(orm.Object);

			var mapper = new Mock<IClassAttributesMapper>();

			applier.Apply(typeof(MyClass), mapper.Object);

			mapper.Verify(cm=> cm.DiscriminatorValue(It.Is<string>(n=> "MyClass".Equals(n))));
		}

		//[Test]
		// Is better to avoid double responsibility and leave the default length
		//public void AlwaysApplyDicriminatorColumnLength()
		//{
		//  var orm = new Mock<IDomainInspector>();
		//  var applier = new ClassDiscriminatorValueAsClassNameApplier(orm.Object);

		//  var mapper = new Mock<IClassAttributesMapper>();
		//  var discriminatorMapper = new Mock<IDiscriminatorMapper>();
		//  mapper.Setup(x => x.Discriminator(It.IsAny<Action<IDiscriminatorMapper>>())).Callback<Action<IDiscriminatorMapper>>(
		//    x => x.Invoke(discriminatorMapper.Object));

		//  applier.Apply(typeof(MyClass), mapper.Object);

		//  discriminatorMapper.Verify(m => m.Length(It.Is<int>(l => l > 0)));
		//}
	}
}