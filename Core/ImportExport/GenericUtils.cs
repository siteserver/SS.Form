using Atom.AdditionalElements;
using Atom.Core;
using Datory;
using SS.Form.Core.Utils;

namespace SS.Form.Core.ImportExport
{
    public static class GenericUtils
    {
        public static AtomEntry GetAtomEntry(Entity entity)
        {
            var entry = AtomUtility.GetEmptyEntry();

            foreach (var keyValuePair in entity.ToDictionary())
            {
                if (keyValuePair.Value != null)
                {
                    AtomUtility.AddDcElement(entry.AdditionalElements, keyValuePair.Key, keyValuePair.Value.ToString());
                }
            }

            return entry;
        }

        public static object GetValue(ScopedElementCollection additionalElements, TableColumn tableColumn)
        {
            if (tableColumn.DataType == DataType.Boolean)
            {
                return FormUtils.ToBool(AtomUtility.GetDcElementContent(additionalElements, tableColumn.AttributeName), false);
            }
            if (tableColumn.DataType == DataType.DateTime)
            {
                return FormUtils.ToDateTime(AtomUtility.GetDcElementContent(additionalElements, tableColumn.AttributeName));
            }
            if (tableColumn.DataType == DataType.Decimal)
            {
                return FormUtils.ToDecimalWithNegative(AtomUtility.GetDcElementContent(additionalElements, tableColumn.AttributeName), 0);
            }
            if (tableColumn.DataType == DataType.Integer)
            {
                return FormUtils.ToIntWithNegative(AtomUtility.GetDcElementContent(additionalElements, tableColumn.AttributeName), 0);
            }
            if (tableColumn.DataType == DataType.Text)
            {
                return AtomUtility.Decrypt(AtomUtility.GetDcElementContent(additionalElements, tableColumn.AttributeName));
            }
            return AtomUtility.GetDcElementContent(additionalElements, tableColumn.AttributeName);
        }

        public static void SetValue(ScopedElementCollection additionalElements, TableColumn tableColumn, Entity entity)
        {
            var value = entity.Get(tableColumn.AttributeName)?.ToString();
            if (tableColumn.DataType == DataType.Text)
            {
                value = AtomUtility.Encrypt(value);
            }
            AtomUtility.AddDcElement(additionalElements, tableColumn.AttributeName, value);
        }
    }
}
