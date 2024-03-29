using System;
using System.Linq;
using ConfOrm.Mappers;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Type;

namespace ConfOrm.NH
{
	public class DiscriminatorMapper : IDiscriminatorMapper
	{
		private readonly HbmDiscriminator discriminatorMapping;

		public DiscriminatorMapper(HbmDiscriminator discriminatorMapping)
		{
			if (discriminatorMapping == null)
			{
				throw new ArgumentNullException("discriminatorMapping");
			}
			this.discriminatorMapping = discriminatorMapping;
		}

		#region IDiscriminatorMapper Members

		public void Column(string column)
		{
			Column(cm => cm.Name(column));
		}

		public void Column(Action<IColumnMapper> columnMapper)
		{
			const string defaultColumnName = "class";
			discriminatorMapping.formula = null;
			HbmColumn hbm = discriminatorMapping.Columns.SingleOrDefault();
			hbm = hbm
				  ??
				  new HbmColumn
				  {
					  name = discriminatorMapping.column,
					  length = discriminatorMapping.length,
					  notnull = discriminatorMapping.notnull,
				  };
			columnMapper(new ColumnMapper(hbm, defaultColumnName));
			if (ColumnTagIsRequired(hbm))
			{
				discriminatorMapping.Item = hbm;
				ResetColumnPlainValues();
			}
			else
			{
				discriminatorMapping.column = !defaultColumnName.Equals(hbm.name) ? hbm.name : null;
				discriminatorMapping.length = hbm.length;
				discriminatorMapping.notnull = hbm.notnull;
				discriminatorMapping.Item = null;
			}
		}

		private bool ColumnTagIsRequired(HbmColumn hbm)
		{
			return hbm.sqltype != null || hbm.@default != null || hbm.check != null || hbm.comment != null ||
				hbm.index != null || hbm.precision != null || hbm.scale != null || hbm.unique || hbm.uniquekey != null;
		}

		private void ResetColumnPlainValues()
		{
			discriminatorMapping.column = null;
			discriminatorMapping.length = null;
			discriminatorMapping.notnull = false;
			discriminatorMapping.formula = null;
		}

		public void Type(IType persistentType)
		{
			if (persistentType != null)
			{
				discriminatorMapping.type = persistentType.Name;
			}
		}

		public void Type(IDiscriminatorType persistentType)
		{
			Type(persistentType.GetType());
		}

		public void Type<TPersistentType>() where TPersistentType : IDiscriminatorType
		{
			Type(typeof(TPersistentType));
		}

		public void Type(Type persistentType)
		{
			if (persistentType == null)
			{
				throw new ArgumentNullException("persistentType");
			}
			if (!typeof(IDiscriminatorType).IsAssignableFrom(persistentType))
			{
				throw new ArgumentOutOfRangeException("persistentType", "Expected type implementing IDiscriminatorType");
			}
			discriminatorMapping.type = persistentType.AssemblyQualifiedName;
		}

		public void Formula(string formula)
		{
			if (formula == null)
			{
				return;
			}

			ResetColumnPlainValues();
			discriminatorMapping.Item = null;
			var formulaLines = formula.Split(new[] {Environment.NewLine}, StringSplitOptions.None);
			if (formulaLines.Length > 1)
			{
				discriminatorMapping.Item = new HbmFormula {Text = formulaLines};
			}
			else
			{
				discriminatorMapping.formula = formula;
			}
		}

		public void Force(bool force)
		{
			discriminatorMapping.force = force;
		}

		public void Insert(bool applyOnInsert)
		{
			discriminatorMapping.insert = applyOnInsert;
		}

		public void NotNullable(bool isNotNullable)
		{
			discriminatorMapping.notnull = isNotNullable;
		}

		public void Length(int length)
		{
			Column(x => x.Length(length));
		}

		#endregion
	}
}