using System;

namespace ConfOrm.Mappers
{
	public interface IMapKeyRelation
	{
		void Element(Action<IMapKeyMapper> mapping);
		void ManyToMany(Action<IMapKeyManyToManyMapper> mapping);
		void Component(Action<IComponentMapKeyMapper> mapping);
	}

	public interface IMapKeyRelation<TKey>
	{
		void Element(Action<IMapKeyMapper> mapping);
		void ManyToMany(Action<IMapKeyManyToManyMapper> mapping);
		void Component(Action<IComponentMapKeyMapper<TKey>> mapping);
	}
}