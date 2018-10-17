using System.Collections.Generic;
using System.Data;
using SiteServer.Plugin;
using SS.Form.Core;
using SS.Form.Model;

namespace SS.Form.Provider
{
    public static class FieldDao
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

        public static int Insert(FieldInfo fieldInfo)
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
                Context.DatabaseApi.GetParameter(nameof(FieldInfo.FormId), fieldInfo.FormId),
                Context.DatabaseApi.GetParameter(nameof(FieldInfo.Taxis), fieldInfo.Taxis),
                Context.DatabaseApi.GetParameter(nameof(FieldInfo.Title), fieldInfo.Title),
                Context.DatabaseApi.GetParameter(nameof(FieldInfo.Description), fieldInfo.Description),
                Context.DatabaseApi.GetParameter(nameof(FieldInfo.PlaceHolder), fieldInfo.PlaceHolder),
                Context.DatabaseApi.GetParameter(nameof(FieldInfo.FieldType), fieldInfo.FieldType),
                Context.DatabaseApi.GetParameter(nameof(FieldInfo.Settings), fieldInfo.Settings)
            };

            return Context.DatabaseApi.ExecuteNonQueryAndReturnId(TableName, nameof(FieldInfo.Id), Context.ConnectionString, sqlString, parameters);
        }

        public static void Update(FieldInfo info)
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
                Context.DatabaseApi.GetParameter(nameof(FieldInfo.FormId), info.FormId),
                Context.DatabaseApi.GetParameter(nameof(FieldInfo.Taxis), info.Taxis),
                Context.DatabaseApi.GetParameter(nameof(FieldInfo.Title), info.Title),
                Context.DatabaseApi.GetParameter(nameof(FieldInfo.Description), info.Description),
                Context.DatabaseApi.GetParameter(nameof(FieldInfo.PlaceHolder), info.PlaceHolder),
                Context.DatabaseApi.GetParameter(nameof(FieldInfo.FieldType), info.FieldType),
                Context.DatabaseApi.GetParameter(nameof(FieldInfo.Settings), info.Settings),
                Context.DatabaseApi.GetParameter(nameof(FieldInfo.Id), info.Id)
            };

            Context.DatabaseApi.ExecuteNonQuery(Context.ConnectionString, sqlString, updateParms);
        }

        public static void Delete(int fieldId)
        {
            string sqlString = $"DELETE FROM {TableName} WHERE {nameof(FieldInfo.Id)} = @{nameof(FieldInfo.Id)}";

            var parms = new []
            {
                Context.DatabaseApi.GetParameter(nameof(FieldInfo.Id), fieldId)
            };

            Context.DatabaseApi.ExecuteNonQuery(Context.ConnectionString, sqlString, parms);

            FieldItemDao.DeleteByFieldId(fieldId);
        }

        public static void DeleteByFormId(int formId)
        {
            string sqlString = $"DELETE FROM {TableName} WHERE {nameof(FieldInfo.FormId)} = @{nameof(FieldInfo.FormId)}";

            var parms = new[]
            {
                Context.DatabaseApi.GetParameter(nameof(FieldInfo.FormId), formId)
            };

            Context.DatabaseApi.ExecuteNonQuery(Context.ConnectionString, sqlString, parms);

            FieldItemDao.DeleteByFormId(formId);
        }

        public static List<FieldInfo> GetFieldInfoList(int formId, bool isItems)
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
                Context.DatabaseApi.GetParameter(nameof(FieldInfo.FormId), formId)
            };

            using (var rdr = Context.DatabaseApi.ExecuteReader(Context.ConnectionString, sqlString, parameters))
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
                                var items = FieldItemDao.GetItemInfoList(fieldInfo.Id);
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

        public static bool IsTitleExists(int formId, string title)
        {
            var exists = false;

            string sqlString = $@"SELECT Id FROM {TableName} WHERE 
    {nameof(FieldInfo.FormId)} = @{nameof(FieldInfo.FormId)} AND 
    {nameof(FieldInfo.Title)} = @{nameof(FieldInfo.Title)}";

            var parms = new []
			{
                Context.DatabaseApi.GetParameter(nameof(FieldInfo.FormId), formId),
                Context.DatabaseApi.GetParameter(nameof(FieldInfo.Title), title)
            };

            using (var rdr = Context.DatabaseApi.ExecuteReader(Context.ConnectionString, sqlString, parms))
            {
                if (rdr.Read() && !rdr.IsDBNull(0))
                {
                    exists = true;
                }
                rdr.Close();
            }

            return exists;
        }

        public static int GetCount(int formId)
        {
            string sqlString = $@"SELECT COUNT(*) FROM {TableName} WHERE 
    {nameof(FieldInfo.FormId)} = @{nameof(FieldInfo.FormId)}";

            var parms = new[]
            {
                Context.DatabaseApi.GetParameter(nameof(FieldInfo.FormId), formId)
            };

            return Dao.GetIntResult(sqlString, parms);
        }

        public static FieldInfo GetFieldInfo(int id, bool isItems)
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
                Context.DatabaseApi.GetParameter(nameof(FieldInfo.Id), id)
			};

            using (var rdr = Context.DatabaseApi.ExecuteReader(Context.ConnectionString, sqlString, parms))
            {
                if (rdr.Read())
                {
                    fieldInfo = GetFieldInfo(rdr);
                }
                rdr.Close();
            }

            if (fieldInfo != null && isItems)
            {
                fieldInfo.Items = FieldItemDao.GetItemInfoList(fieldInfo.Id);
            }

            return fieldInfo;
        }

        public static FieldInfo GetFieldInfo(int formId, string title)
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
                Context.DatabaseApi.GetParameter(nameof(FieldInfo.FormId), formId),
                Context.DatabaseApi.GetParameter(nameof(FieldInfo.Title), title)
            };

            using (var rdr = Context.DatabaseApi.ExecuteReader(Context.ConnectionString, sqlString, parms))
            {
                if (rdr.Read())
                {
                    fieldInfo = GetFieldInfo(rdr);
                }
                rdr.Close();
            }

            return fieldInfo;
        }

        private static int GetMaxTaxis(int formId)
        {
            string sqlString =
                $"SELECT MAX(Taxis) AS MaxTaxis FROM {TableName} WHERE {nameof(FieldInfo.FormId)} = {formId}";
            var maxTaxis = 0;

            using (var rdr = Context.DatabaseApi.ExecuteReader(Context.ConnectionString, sqlString))
            {
                if (rdr.Read() && !rdr.IsDBNull(0))
                {
                    maxTaxis = rdr.GetInt32(0);
                }
                rdr.Close();
            }
            return maxTaxis;
        }

        public static void TaxisDown(int id)
        {
            var fieldInfo = GetFieldInfo(id, false);
            if (fieldInfo == null) return;

            var sqlString = Context.DatabaseApi.GetPageSqlString(TableName, "Id, Taxis", $"WHERE {nameof(FieldInfo.FormId)} = @{nameof(FieldInfo.FormId)} AND Taxis > (SELECT Taxis FROM {TableName} WHERE Id = @Id)", "ORDER BY Taxis", 0, 1);

            var higherId = 0;
            var higherTaxis = 0;

            var parms = new []
            {
                Context.DatabaseApi.GetParameter(nameof(FieldInfo.FormId), fieldInfo.FormId),
                Context.DatabaseApi.GetParameter(nameof(FieldInfo.Id), id)
            };

            using (var rdr = Context.DatabaseApi.ExecuteReader(Context.ConnectionString, sqlString, parms))
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

        public static void TaxisUp(int id)
        {
            var fieldInfo = GetFieldInfo(id, false);
            if (fieldInfo == null) return;

            var sqlString = Context.DatabaseApi.GetPageSqlString(TableName, "Id, Taxis", $"WHERE {nameof(FieldInfo.FormId)} = @{nameof(FieldInfo.FormId)} AND Taxis < (SELECT Taxis FROM {TableName} WHERE Id = @Id)", "ORDER BY Taxis DESC", 0, 1);
            var lowerId = 0;
            var lowerTaxis = 0;

            var parms = new []
            {
                Context.DatabaseApi.GetParameter(nameof(FieldInfo.FormId), fieldInfo.FormId),
                Context.DatabaseApi.GetParameter(nameof(FieldInfo.Id), id)
            };

            using (var rdr = Context.DatabaseApi.ExecuteReader(Context.ConnectionString, sqlString, parms))
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

        private static void SetTaxis(int id, int taxis)
        {
            var sqlString = $"UPDATE {TableName} SET Taxis = @Taxis WHERE Id = @Id";

            var parms = new []
			{
				Context.DatabaseApi.GetParameter(nameof(FieldInfo.Taxis), taxis),
                Context.DatabaseApi.GetParameter(nameof(FieldInfo.Id), id)
			};

            Context.DatabaseApi.ExecuteNonQuery(Context.ConnectionString, sqlString, parms);
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
