using System;

namespace ConfOrm.Mappers
{
	public interface IEntityAttributesMapper
	{
		void EntityName(string value);
		void Proxy(Type proxy);
		void Lazy(bool value);
		void DynamicUpdate(bool value);
		void DynamicInsert(bool value);
		void BatchSize(int value);
		void SelectBeforeUpdate(bool value);
	}
}