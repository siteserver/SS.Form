using System;
using System.Collections.Generic;
using System.Data;
using SiteServer.Plugin;
using SS.Form.Model;

namespace SS.Form.Provider
{
    public static class LogDao
    {
        public const string TableName = "ss_form_log";

        public static List<TableColumn> Columns => new List<TableColumn>
        {
            new TableColumn
            {
                AttributeName = nameof(LogInfo.Id),
                DataType = DataType.Integer
            },
            new TableColumn
            {
                AttributeName = nameof(LogInfo.FormId),
                DataType = DataType.Integer
            },
            new TableColumn
            {
                AttributeName = nameof(LogInfo.ItemIds),
                DataType = DataType.VarChar,
                DataLength = 200
            },
            new TableColumn
            {
                AttributeName = nameof(LogInfo.UniqueId),
                DataType = DataType.VarChar,
                DataLength = 200
            },
            new TableColumn
            {
                AttributeName = nameof(LogInfo.AddDate),
                DataType = DataType.DateTime
            },
            new TableColumn
            {
                AttributeName = nameof(LogInfo.AttributeValues),
                DataType = DataType.Text
            }
        };

        public static int Insert(LogInfo logInfo)
        {
            string sqlString = $@"INSERT INTO {TableName}
(
    {nameof(LogInfo.FormId)},
    {nameof(LogInfo.ItemIds)},
    {nameof(LogInfo.UniqueId)},
    {nameof(LogInfo.AddDate)},
    {nameof(LogInfo.AttributeValues)}
) VALUES (
    @{nameof(LogInfo.FormId)}, 
    @{nameof(LogInfo.ItemIds)}, 
    @{nameof(LogInfo.UniqueId)},
    @{nameof(LogInfo.AddDate)},
    @{nameof(LogInfo.AttributeValues)}
)";

            var parameters = new List<IDataParameter>
            {
                Context.DatabaseApi.GetParameter(nameof(logInfo.FormId), logInfo.FormId),
                Context.DatabaseApi.GetParameter(nameof(logInfo.ItemIds), logInfo.ItemIds),
                Context.DatabaseApi.GetParameter(nameof(logInfo.UniqueId), logInfo.UniqueId),
                Context.DatabaseApi.GetParameter(nameof(logInfo.AddDate), logInfo.AddDate),
                Context.DatabaseApi.GetParameter(nameof(logInfo.AttributeValues), logInfo.ToString())
            };

            return Context.DatabaseApi.ExecuteNonQueryAndReturnId(TableName, nameof(LogInfo.Id), Context.ConnectionString, sqlString, parameters.ToArray());
        }

        public static void DeleteByFormId(int formId)
        {
            if (formId <= 0) return;

            string sqlString = $"DELETE FROM {TableName} WHERE {nameof(LogInfo.FormId)} = {formId}";
            Context.DatabaseApi.ExecuteNonQuery(Context.ConnectionString, sqlString);
        }

        public static void Delete(List<int> logIdList)
        {
            if (logIdList == null || logIdList.Count <= 0) return;
            string sqlString =
                $"DELETE FROM {TableName} WHERE {nameof(LogInfo.Id)} IN ({string.Join(",", logIdList)})";
            Context.DatabaseApi.ExecuteNonQuery(Context.ConnectionString, sqlString);
        }

        public static void Delete(int logId)
        {
            if (logId <= 0) return;
            string sqlString =
                $"DELETE FROM {TableName} WHERE {nameof(LogInfo.Id)} = {logId}";
            Context.DatabaseApi.ExecuteNonQuery(Context.ConnectionString, sqlString);
        }

        public static int GetCount(int formId)
        {
            string sqlString =
                $"SELECT COUNT(*) FROM {TableName} WHERE {nameof(LogInfo.FormId)} = {formId}";

            var count = 0;

            using (var rdr = Context.DatabaseApi.ExecuteReader(Context.ConnectionString, sqlString))
            {
                if (rdr.Read() && !rdr.IsDBNull(0))
                {
                    count = rdr.GetInt32(0);
                }
                rdr.Close();
            }

            return count;
        }

        public static bool IsExists(int formId, string uniqueId)
        {
            var sqlString =
                $"SELECT Id FROM {TableName} WHERE {nameof(LogInfo.FormId)} = @{nameof(LogInfo.FormId)} AND {nameof(LogInfo.UniqueId)} = @{nameof(LogInfo.UniqueId)}";

            var parameters = new List<IDataParameter>
            {
                Context.DatabaseApi.GetParameter(nameof(LogInfo.FormId), formId),
                Context.DatabaseApi.GetParameter(nameof(LogInfo.UniqueId), uniqueId)
            };

            var exists = false;

            using (var rdr = Context.DatabaseApi.ExecuteReader(Context.ConnectionString, sqlString, parameters.ToArray()))
            {
                if (rdr.Read() && !rdr.IsDBNull(0))
                {
                    exists = true;
                }
                rdr.Close();
            }

            return exists;
        }

        public static List<LogInfo> GetFormLogInfoList(int formId, int totalCount, int limit, int offset)
        {
            var formLogInfoList = new List<LogInfo>();

            string sqlString =
                $@"SELECT {nameof(LogInfo.Id)},
    {nameof(LogInfo.FormId)},
    {nameof(LogInfo.ItemIds)},
    {nameof(LogInfo.UniqueId)},
    {nameof(LogInfo.AddDate)},
    {nameof(LogInfo.AttributeValues)}
            FROM {TableName} WHERE {nameof(LogInfo.FormId)} = {formId} ORDER BY {nameof(LogInfo.Id)} DESC";

            using (var rdr = Context.DatabaseApi.ExecuteReader(Context.ConnectionString, sqlString))
            {
                while (rdr.Read())
                {
                    var formLogInfo = GetFormItemInfo(rdr);
                    if (!string.IsNullOrEmpty(formLogInfo.AttributeValues))
                    {
                        formLogInfoList.Add(formLogInfo);
                    }
                }
                rdr.Close();
            }

            return formLogInfoList;
        }

        public static List<LogInfo> GetAllFormLogInfoList(int formId)
        {
            var formLogInfoList = new List<LogInfo>();

            string sqlString =
                $@"SELECT {nameof(LogInfo.Id)},
    {nameof(LogInfo.FormId)},
    {nameof(LogInfo.ItemIds)},
    {nameof(LogInfo.UniqueId)},
    {nameof(LogInfo.AddDate)},
    {nameof(LogInfo.AttributeValues)}
            FROM {TableName} WHERE {nameof(LogInfo.FormId)} = {formId}";

            using (var rdr = Context.DatabaseApi.ExecuteReader(Context.ConnectionString, sqlString))
            {
                while (rdr.Read())
                {
                    var formLogInfo = GetFormItemInfo(rdr);
                    if (!string.IsNullOrEmpty(formLogInfo.AttributeValues))
                    {
                        formLogInfoList.Add(formLogInfo);
                    }
                }
                rdr.Close();
            }

            return formLogInfoList;
        }

        public static LogInfo GetLogInfo(int logId)
        {
            LogInfo logInfo = null;

            string sqlString =
                $@"SELECT {nameof(LogInfo.Id)},
    {nameof(LogInfo.FormId)},
    {nameof(LogInfo.ItemIds)},
    {nameof(LogInfo.UniqueId)},
    {nameof(LogInfo.AddDate)},
    {nameof(LogInfo.AttributeValues)}
            FROM {TableName} WHERE {nameof(LogInfo.Id)} = {logId}";

            using (var rdr = Context.DatabaseApi.ExecuteReader(Context.ConnectionString, sqlString))
            {
                if (rdr.Read())
                {
                    logInfo = GetFormItemInfo(rdr);
                }
                rdr.Close();
            }

            return logInfo;
        }

        private static LogInfo GetFormItemInfo(IDataRecord rdr)
        {
            if (rdr == null) return null;

            var logInfo = new LogInfo();

            var i = 0;
            logInfo.Id = rdr.IsDBNull(i) ? 0 : rdr.GetInt32(i);
            i++;
            logInfo.FormId = rdr.IsDBNull(i) ? 0 : rdr.GetInt32(i);
            i++;
            logInfo.ItemIds = rdr.IsDBNull(i) ? string.Empty : rdr.GetString(i);
            i++;
            logInfo.UniqueId = rdr.IsDBNull(i) ? string.Empty : rdr.GetString(i);
            i++;
            logInfo.AddDate = rdr.IsDBNull(i) ? DateTime.Now : rdr.GetDateTime(i);
            i++;
            logInfo.AttributeValues = rdr.IsDBNull(i) ? string.Empty : rdr.GetString(i);

            logInfo.Load(logInfo.AttributeValues);

            return logInfo;
        }
    }
}
