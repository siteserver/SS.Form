using System;
using System.Collections.Generic;
using System.Data;
using SiteServer.Plugin;
using SS.Form.Model;

namespace SS.Form.Provider
{
    public class LogDao
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

        private readonly string _connectionString;
        private readonly IDatabaseApi _helper;

        public LogDao(string connectionString, IDatabaseApi helper)
        {
            _connectionString = connectionString;
            _helper = helper;
        }

        public int Insert(LogInfo logInfo)
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
                _helper.GetParameter(nameof(logInfo.FormId), logInfo.FormId),
                _helper.GetParameter(nameof(logInfo.ItemIds), logInfo.ItemIds),
                _helper.GetParameter(nameof(logInfo.UniqueId), logInfo.UniqueId),
                _helper.GetParameter(nameof(logInfo.AddDate), logInfo.AddDate),
                _helper.GetParameter(nameof(logInfo.AttributeValues), logInfo.ToString())
            };

            return _helper.ExecuteNonQueryAndReturnId(TableName, nameof(LogInfo.Id), _connectionString, sqlString, parameters.ToArray());
        }

        public void DeleteByFormId(int formId)
        {
            if (formId <= 0) return;

            string sqlString = $"DELETE FROM {TableName} WHERE {nameof(LogInfo.FormId)} = {formId}";
            _helper.ExecuteNonQuery(_connectionString, sqlString);
        }

        public void Delete(List<int> logIdList)
        {
            if (logIdList == null || logIdList.Count <= 0) return;
            string sqlString =
                $"DELETE FROM {TableName} WHERE {nameof(LogInfo.Id)} IN ({string.Join(",", logIdList)})";
            _helper.ExecuteNonQuery(_connectionString, sqlString);
        }

        public void Delete(int logId)
        {
            if (logId <= 0) return;
            string sqlString =
                $"DELETE FROM {TableName} WHERE {nameof(LogInfo.Id)} = {logId}";
            _helper.ExecuteNonQuery(_connectionString, sqlString);
        }

        public int GetCount(int formId)
        {
            string sqlString =
                $"SELECT COUNT(*) FROM {TableName} WHERE {nameof(LogInfo.FormId)} = {formId}";

            var count = 0;

            using (var rdr = _helper.ExecuteReader(_connectionString, sqlString))
            {
                if (rdr.Read() && !rdr.IsDBNull(0))
                {
                    count = rdr.GetInt32(0);
                }
                rdr.Close();
            }

            return count;
        }

        public bool IsExists(int formId, string uniqueId)
        {
            var sqlString =
                $"SELECT Id FROM {TableName} WHERE {nameof(LogInfo.FormId)} = @{nameof(LogInfo.FormId)} AND {nameof(LogInfo.UniqueId)} = @{nameof(LogInfo.UniqueId)}";

            var parameters = new List<IDataParameter>
            {
                _helper.GetParameter(nameof(LogInfo.FormId), formId),
                _helper.GetParameter(nameof(LogInfo.UniqueId), uniqueId)
            };

            var exists = false;

            using (var rdr = _helper.ExecuteReader(_connectionString, sqlString, parameters.ToArray()))
            {
                if (rdr.Read() && !rdr.IsDBNull(0))
                {
                    exists = true;
                }
                rdr.Close();
            }

            return exists;
        }

        public List<LogInfo> GetFormLogInfoList(int formId, int totalCount, int limit, int offset)
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

            using (var rdr = _helper.ExecuteReader(_connectionString, sqlString))
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

        public List<LogInfo> GetAllFormLogInfoList(int formId)
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

            using (var rdr = _helper.ExecuteReader(_connectionString, sqlString))
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

        public LogInfo GetLogInfo(int logId)
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

            using (var rdr = _helper.ExecuteReader(_connectionString, sqlString))
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
