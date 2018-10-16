using System.Collections.Generic;
using System.Data;
using SiteServer.Plugin;
using SS.Form.Core;
using SS.Form.Model;

namespace SS.Form.Provider
{
    public class FieldDao
    {
        public const string TableName = "ss_form_field";

        public static List<TableColumn> Columns => new List<TableColumn>
        {
            new TableColumn
            {
                AttributeName = nameof(FieldInfo.Id),
                DataType = DataType.Integer
            },
            new TableColumn
            {
                AttributeName = nameof(FieldInfo.FormId),
                DataType = DataType.Integer
            },
            new TableColumn
            {
                AttributeName = nameof(FieldInfo.Taxis),
                DataType = DataType.Integer
            },
            new TableColumn
            {
                AttributeName = nameof(FieldInfo.Title),
                DataType = DataType.VarChar,
                DataLength = 200
            },
            new TableColumn
            {
                AttributeName = nameof(FieldInfo.Description),
                DataType = DataType.VarChar,
                DataLength = 200
            },
            new TableColumn
            {
                AttributeName = nameof(FieldInfo.PlaceHolder),
                DataType = DataType.VarChar,
                DataLength = 200
            },
            new TableColumn
            {
                AttributeName = nameof(FieldInfo.FieldType),
                DataType = DataType.VarChar,
                DataLength = 50
            },
            new TableColumn
            {
                AttributeName = nameof(FieldInfo.Settings),
                DataType = DataType.Text
            }
        };

        private readonly string _connectionString;
        private readonly IDatabaseApi _helper;

        public FieldDao(string connectionString, IDatabaseApi helper)
        {
            _connectionString = connectionString;
            _helper = helper;
        }

        public int Insert(FieldInfo fieldInfo)
        {
            fieldInfo.Taxis = GetMaxTaxis(fieldInfo.FormId) + 1;

            string sqlString = $@"INSERT INTO {TableName}
(
    {nameof(FieldInfo.FormId)}, 
    {nameof(FieldInfo.Taxis)},
    {nameof(FieldInfo.Title)},
    {nameof(FieldInfo.Description)},
    {nameof(FieldInfo.PlaceHolder)},
    {nameof(FieldInfo.FieldType)},
    {nameof(FieldInfo.Settings)}
) VALUES (
    @{nameof(FieldInfo.FormId)}, 
    @{nameof(FieldInfo.Taxis)},
    @{nameof(FieldInfo.Title)},
    @{nameof(FieldInfo.Description)},
    @{nameof(FieldInfo.PlaceHolder)},
    @{nameof(FieldInfo.FieldType)},
    @{nameof(FieldInfo.Settings)}
)";

            var parameters = new []
			{
                _helper.GetParameter(nameof(FieldInfo.FormId), fieldInfo.FormId),
                _helper.GetParameter(nameof(FieldInfo.Taxis), fieldInfo.Taxis),
                _helper.GetParameter(nameof(FieldInfo.Title), fieldInfo.Title),
                _helper.GetParameter(nameof(FieldInfo.Description), fieldInfo.Description),
                _helper.GetParameter(nameof(FieldInfo.PlaceHolder), fieldInfo.PlaceHolder),
                _helper.GetParameter(nameof(FieldInfo.FieldType), fieldInfo.FieldType),
                _helper.GetParameter(nameof(FieldInfo.Settings), fieldInfo.Settings)
            };

            return _helper.ExecuteNonQueryAndReturnId(TableName, nameof(FieldInfo.Id), _connectionString, sqlString, parameters);
        }

        public void Update(FieldInfo info)
        {
            string sqlString = $@"UPDATE {TableName} SET
                {nameof(FieldInfo.FormId)} = @{nameof(FieldInfo.FormId)}, 
                {nameof(FieldInfo.Taxis)} = @{nameof(FieldInfo.Taxis)}, 
                {nameof(FieldInfo.Title)} = @{nameof(FieldInfo.Title)},
                {nameof(FieldInfo.Description)} = @{nameof(FieldInfo.Description)},
                {nameof(FieldInfo.PlaceHolder)} = @{nameof(FieldInfo.PlaceHolder)},
                {nameof(FieldInfo.FieldType)} = @{nameof(FieldInfo.FieldType)},
                {nameof(FieldInfo.Settings)} = @{nameof(FieldInfo.Settings)}
            WHERE {nameof(FieldInfo.Id)} = @{nameof(FieldInfo.Id)}";

            var updateParms = new []
			{
                _helper.GetParameter(nameof(FieldInfo.FormId), info.FormId),
                _helper.GetParameter(nameof(FieldInfo.Taxis), info.Taxis),
                _helper.GetParameter(nameof(FieldInfo.Title), info.Title),
                _helper.GetParameter(nameof(FieldInfo.Description), info.Description),
                _helper.GetParameter(nameof(FieldInfo.PlaceHolder), info.PlaceHolder),
                _helper.GetParameter(nameof(FieldInfo.FieldType), info.FieldType),
                _helper.GetParameter(nameof(FieldInfo.Settings), info.Settings),
                _helper.GetParameter(nameof(FieldInfo.Id), info.Id)
            };

            _helper.ExecuteNonQuery(_connectionString, sqlString, updateParms);
        }

        public void Delete(int fieldId)
        {
            string sqlString = $"DELETE FROM {TableName} WHERE {nameof(FieldInfo.Id)} = @{nameof(FieldInfo.Id)}";

            var parms = new []
            {
                _helper.GetParameter(nameof(FieldInfo.Id), fieldId)
            };

            _helper.ExecuteNonQuery(_connectionString, sqlString, parms);

            Main.FieldItemDao.DeleteByFieldId(fieldId);
        }

        public void DeleteByFormId(int formId)
        {
            string sqlString = $"DELETE FROM {TableName} WHERE {nameof(FieldInfo.FormId)} = @{nameof(FieldInfo.FormId)}";

            var parms = new[]
            {
                _helper.GetParameter(nameof(FieldInfo.FormId), formId)
            };

            _helper.ExecuteNonQuery(_connectionString, sqlString, parms);

            Main.FieldItemDao.DeleteByFormId(formId);
        }

        public List<FieldInfo> GetFieldInfoList(int formId, bool isItems)
        {
            var list = new List<FieldInfo>();

            string sqlString =
                $@"SELECT
    {nameof(FieldInfo.Id)}, 
    {nameof(FieldInfo.FormId)}, 
    {nameof(FieldInfo.Taxis)},
    {nameof(FieldInfo.Title)},
    {nameof(FieldInfo.Description)},
    {nameof(FieldInfo.PlaceHolder)},
    {nameof(FieldInfo.FieldType)},
    {nameof(FieldInfo.Settings)}
FROM {TableName} 
WHERE
    {nameof(FieldInfo.FormId)} = @{nameof(FieldInfo.FormId)}
ORDER BY {nameof(FieldInfo.Taxis)}";

            var parameters = new[]
            {
                _helper.GetParameter(nameof(FieldInfo.FormId), formId)
            };

            using (var rdr = _helper.ExecuteReader(_connectionString, sqlString, parameters))
            {
                while (rdr.Read())
                {
                    var fieldInfo = GetFieldInfo(rdr);
                    if (fieldInfo != null)
                    {
                        if (isItems)
                        {
                            if (Utils.IsSelectFieldType(fieldInfo.FieldType))
                            {
                                var items = Main.FieldItemDao.GetItemInfoList(fieldInfo.Id);
                                if (items != null && items.Count > 0)
                                {
                                    fieldInfo.Items = items;
                                }
                            }
                        }
                        list.Add(fieldInfo);
                    }
                }
                rdr.Close();
            }

            return list;
        }

        public bool IsTitleExists(int formId, string title)
        {
            var exists = false;

            string sqlString = $@"SELECT Id FROM {TableName} WHERE 
    {nameof(FieldInfo.FormId)} = @{nameof(FieldInfo.FormId)} AND 
    {nameof(FieldInfo.Title)} = @{nameof(FieldInfo.Title)}";

            var parms = new []
			{
                _helper.GetParameter(nameof(FieldInfo.FormId), formId),
                _helper.GetParameter(nameof(FieldInfo.Title), title)
            };

            using (var rdr = _helper.ExecuteReader(_connectionString, sqlString, parms))
            {
                if (rdr.Read() && !rdr.IsDBNull(0))
                {
                    exists = true;
                }
                rdr.Close();
            }

            return exists;
        }

        public int GetCount(int formId)
        {
            string sqlString = $@"SELECT COUNT(*) FROM {TableName} WHERE 
    {nameof(FieldInfo.FormId)} = @{nameof(FieldInfo.FormId)}";

            var parms = new[]
            {
                _helper.GetParameter(nameof(FieldInfo.FormId), formId)
            };

            return Main.Dao.GetIntResult(sqlString, parms);
        }

        public FieldInfo GetFieldInfo(int id, bool isItems)
        {
            FieldInfo fieldInfo = null;

            string sqlString =
                $@"SELECT
    {nameof(FieldInfo.Id)}, 
    {nameof(FieldInfo.FormId)}, 
    {nameof(FieldInfo.Taxis)},
    {nameof(FieldInfo.Title)},
    {nameof(FieldInfo.Description)},
    {nameof(FieldInfo.PlaceHolder)},
    {nameof(FieldInfo.FieldType)},
    {nameof(FieldInfo.Settings)}
FROM {TableName} 
WHERE {nameof(FieldInfo.Id)} = @{nameof(FieldInfo.Id)}";

            var parms = new []
			{
                _helper.GetParameter(nameof(FieldInfo.Id), id)
			};

            using (var rdr = _helper.ExecuteReader(_connectionString, sqlString, parms))
            {
                if (rdr.Read())
                {
                    fieldInfo = GetFieldInfo(rdr);
                }
                rdr.Close();
            }

            if (fieldInfo != null && isItems)
            {
                fieldInfo.Items = Main.FieldItemDao.GetItemInfoList(fieldInfo.Id);
            }

            return fieldInfo;
        }

        public FieldInfo GetFieldInfo(int formId, string title)
        {
            FieldInfo fieldInfo = null;

            string sqlString =
                $@"SELECT
    {nameof(FieldInfo.Id)}, 
    {nameof(FieldInfo.FormId)}, 
    {nameof(FieldInfo.Taxis)},
    {nameof(FieldInfo.Title)},
    {nameof(FieldInfo.Description)},
    {nameof(FieldInfo.PlaceHolder)},
    {nameof(FieldInfo.FieldType)},
    {nameof(FieldInfo.Settings)}
FROM {TableName} 
WHERE 
    {nameof(FieldInfo.FormId)} = @{nameof(FieldInfo.FormId)} AND 
    {nameof(FieldInfo.Title)} = @{nameof(FieldInfo.Title)}";

            var parms = new []
			{
                _helper.GetParameter(nameof(FieldInfo.FormId), formId),
                _helper.GetParameter(nameof(FieldInfo.Title), title)
            };

            using (var rdr = _helper.ExecuteReader(_connectionString, sqlString, parms))
            {
                if (rdr.Read())
                {
                    fieldInfo = GetFieldInfo(rdr);
                }
                rdr.Close();
            }

            return fieldInfo;
        }

        private int GetMaxTaxis(int formId)
        {
            string sqlString =
                $"SELECT MAX(Taxis) AS MaxTaxis FROM {TableName} WHERE {nameof(FieldInfo.FormId)} = {formId}";
            var maxTaxis = 0;

            using (var rdr = _helper.ExecuteReader(_connectionString, sqlString))
            {
                if (rdr.Read() && !rdr.IsDBNull(0))
                {
                    maxTaxis = rdr.GetInt32(0);
                }
                rdr.Close();
            }
            return maxTaxis;
        }

        public void TaxisDown(int id)
        {
            var fieldInfo = GetFieldInfo(id, false);
            if (fieldInfo == null) return;

            var sqlString = _helper.GetPageSqlString(TableName, "Id, Taxis", $"WHERE {nameof(FieldInfo.FormId)} = @{nameof(FieldInfo.FormId)} AND Taxis > (SELECT Taxis FROM {TableName} WHERE Id = @Id)", "ORDER BY Taxis", 0, 1);

            var higherId = 0;
            var higherTaxis = 0;

            var parms = new []
            {
                _helper.GetParameter(nameof(FieldInfo.FormId), fieldInfo.FormId),
                _helper.GetParameter(nameof(FieldInfo.Id), id)
            };

            using (var rdr = _helper.ExecuteReader(_connectionString, sqlString, parms))
            {
                if (rdr.Read() && !rdr.IsDBNull(0))
                {
                    higherId = rdr.GetInt32(0);
                    higherTaxis = rdr.GetInt32(1);
                }
                rdr.Close();
            }

            if (higherId != 0)
            {
                SetTaxis(id, higherTaxis);
                SetTaxis(higherId, fieldInfo.Taxis);
            }
        }

        public void TaxisUp(int id)
        {
            var fieldInfo = GetFieldInfo(id, false);
            if (fieldInfo == null) return;

            var sqlString = _helper.GetPageSqlString(TableName, "Id, Taxis", $"WHERE {nameof(FieldInfo.FormId)} = @{nameof(FieldInfo.FormId)} AND Taxis < (SELECT Taxis FROM {TableName} WHERE Id = @Id)", "ORDER BY Taxis DESC", 0, 1);
            var lowerId = 0;
            var lowerTaxis = 0;

            var parms = new []
            {
                _helper.GetParameter(nameof(FieldInfo.FormId), fieldInfo.FormId),
                _helper.GetParameter(nameof(FieldInfo.Id), id)
            };

            using (var rdr = _helper.ExecuteReader(_connectionString, sqlString, parms))
            {
                if (rdr.Read() && !rdr.IsDBNull(0))
                {
                    lowerId = rdr.GetInt32(0);
                    lowerTaxis = rdr.GetInt32(1);
                }
                rdr.Close();
            }

            if (lowerId != 0)
            {
                SetTaxis(id, lowerTaxis);
                SetTaxis(lowerId, fieldInfo.Taxis);
            }
        }

        private void SetTaxis(int id, int taxis)
        {
            var sqlString = $"UPDATE {TableName} SET Taxis = @Taxis WHERE Id = @Id";

            var parms = new []
			{
				_helper.GetParameter(nameof(FieldInfo.Taxis), taxis),
                _helper.GetParameter(nameof(FieldInfo.Id), id)
			};

            _helper.ExecuteNonQuery(_connectionString, sqlString, parms);
        }

        private static FieldInfo GetFieldInfo(IDataRecord rdr)
        {
            if (rdr == null) return null;

            var fieldInfo = new FieldInfo();

            var i = 0;
            fieldInfo.Id = rdr.IsDBNull(i) ? 0 : rdr.GetInt32(i);
            i++;
            fieldInfo.FormId = rdr.IsDBNull(i) ? 0 : rdr.GetInt32(i);
            i++;
            fieldInfo.Taxis = rdr.IsDBNull(i) ? 0 : rdr.GetInt32(i);
            i++;
            fieldInfo.Title = rdr.IsDBNull(i) ? string.Empty : rdr.GetString(i);
            i++;
            fieldInfo.Description = rdr.IsDBNull(i) ? string.Empty : rdr.GetString(i);
            i++;
            fieldInfo.PlaceHolder = rdr.IsDBNull(i) ? string.Empty : rdr.GetString(i);
            i++;
            fieldInfo.FieldType = rdr.IsDBNull(i) ? string.Empty : rdr.GetString(i);
            i++;
            fieldInfo.Settings = rdr.IsDBNull(i) ? string.Empty : rdr.GetString(i);

            return fieldInfo;
        }
    }
}
