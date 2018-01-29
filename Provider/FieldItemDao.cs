using System.Collections.Generic;
using System.Data;
using SiteServer.Plugin;
using SS.Form.Model;

namespace SS.Form.Provider
{
    public class FieldItemDao
    {
        public const string TableName = "ss_form_field_item";

        public static List<TableColumn> Columns => new List<TableColumn>
        {
            new TableColumn
            {
                AttributeName = nameof(FieldItemInfo.Id),
                DataType = DataType.Integer
            },
            new TableColumn
            {
                AttributeName = nameof(FieldItemInfo.FormId),
                DataType = DataType.Integer
            },
            new TableColumn
            {
                AttributeName = nameof(FieldItemInfo.FieldId),
                DataType = DataType.Integer
            },
            new TableColumn
            {
                AttributeName = nameof(FieldItemInfo.Value),
                DataType = DataType.VarChar,
                DataLength = 200
            },
            new TableColumn
            {
                AttributeName = nameof(FieldItemInfo.IsSelected),
                DataType = DataType.Boolean
            },
            new TableColumn
            {
                AttributeName = nameof(FieldItemInfo.IsExtras),
                DataType = DataType.Boolean
            }
        };

        private readonly string _connectionString;
        private readonly IDataApi _helper;

        public FieldItemDao(string connectionString, IDataApi helper)
        {
            _connectionString = connectionString;
            _helper = helper;
        }

        public void Insert(IDbTransaction trans, FieldItemInfo itemInfo)
        {
            var sqlString = $@"INSERT INTO {TableName} (
    {nameof(FieldItemInfo.FormId)},
    {nameof(FieldItemInfo.FieldId)},
    {nameof(FieldItemInfo.Value)},
    {nameof(FieldItemInfo.IsSelected)},
    {nameof(FieldItemInfo.IsExtras)}
) VALUES (
    @{nameof(FieldItemInfo.FormId)},
    @{nameof(FieldItemInfo.FieldId)},
    @{nameof(FieldItemInfo.Value)},
    @{nameof(FieldItemInfo.IsSelected)},
    @{nameof(FieldItemInfo.IsExtras)}
)";

            var parameters = new[]
            {
                _helper.GetParameter(nameof(FieldItemInfo.FormId), itemInfo.FormId),
                _helper.GetParameter(nameof(FieldItemInfo.FieldId), itemInfo.FieldId),
                _helper.GetParameter(nameof(FieldItemInfo.Value), itemInfo.Value),
                _helper.GetParameter(nameof(FieldItemInfo.IsSelected), itemInfo.IsSelected),
                _helper.GetParameter(nameof(FieldItemInfo.IsExtras), itemInfo.IsExtras)
            };

            _helper.ExecuteNonQuery(trans, sqlString, parameters);
        }

        public void InsertItems(int formId, int fieldId, List<FieldItemInfo> items)
        {
            if (formId <= 0 || fieldId <= 0 || items == null || items.Count == 0) return;
            
            using (var conn = _helper.GetConnection(_connectionString))
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        foreach (var itemInfo in items)
                        {
                            itemInfo.FormId = formId;
                            itemInfo.FieldId = fieldId;
                            Insert(trans, itemInfo);
                        }

                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }
        }

        public void DeleteByFormId(int formId)
        {
            if (formId == 0) return;

            var sqlString = $"DELETE FROM {TableName} WHERE {nameof(FieldItemInfo.FormId)} = @{nameof(FieldItemInfo.FormId)}";

            var parms = new []
			{
				_helper.GetParameter(nameof(FieldItemInfo.FormId), formId)
			};

            _helper.ExecuteNonQuery(_connectionString, sqlString, parms);
        }

        public void DeleteByFieldId(int fieldId)
        {
            if (fieldId == 0) return;

            var sqlString = $"DELETE FROM {TableName} WHERE {nameof(FieldItemInfo.FieldId)} = @{nameof(FieldItemInfo.FieldId)}";

            var parms = new[]
            {
                _helper.GetParameter(nameof(FieldItemInfo.FieldId), fieldId)
            };

            _helper.ExecuteNonQuery(_connectionString, sqlString, parms);
        }

        public List<FieldItemInfo> GetItemInfoList(int fieldId)
        {
            var items = new List<FieldItemInfo>();

            var sqlString =
                $@"SELECT {nameof(FieldItemInfo.Id)}, {nameof(FieldItemInfo.FormId)}, {nameof(FieldItemInfo.FieldId)}, {nameof(FieldItemInfo.Value)}, {nameof(FieldItemInfo.IsSelected)}, {nameof(FieldItemInfo.IsExtras)} FROM {TableName} WHERE ({nameof(FieldItemInfo.FieldId)} = @{nameof(FieldItemInfo.FieldId)})";

            var parms = new []
			{
                _helper.GetParameter(nameof(FieldItemInfo.FieldId), fieldId)
			};

            using (var rdr = _helper.ExecuteReader(_connectionString, sqlString, parms))
            {
                while (rdr.Read())
                {
                    items.Add(GetFieldItemInfo(rdr));
                }
                rdr.Close();
            }

            return items;
        }

        private static FieldItemInfo GetFieldItemInfo(IDataRecord rdr)
        {
            if (rdr == null) return null;

            var itemInfo = new FieldItemInfo();

            var i = 0;
            itemInfo.Id = rdr.IsDBNull(i) ? 0 : rdr.GetInt32(i);
            i++;
            itemInfo.FormId = rdr.IsDBNull(i) ? 0 : rdr.GetInt32(i);
            i++;
            itemInfo.FieldId = rdr.IsDBNull(i) ? 0 : rdr.GetInt32(i);
            i++;
            itemInfo.Value = rdr.IsDBNull(i) ? string.Empty : rdr.GetString(i);
            i++;
            itemInfo.IsSelected = !rdr.IsDBNull(i) && rdr.GetBoolean(i);
            i++;
            itemInfo.IsExtras = !rdr.IsDBNull(i) && rdr.GetBoolean(i);

            return itemInfo;
        }
    }
}
