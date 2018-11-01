using System;
using System.Collections.Generic;
using System.Data;
using SiteServer.Plugin;
using SS.Form.Core.Model;
using SS.Form.Core.Utils;

namespace SS.Form.Core.Provider
{
    public static class LogDao
    {
        public const string TableName = "ss_form_log";

        public static List<TableColumn> Columns => new List<TableColumn>
        {
            new TableColumn
            {
                AttributeName = nameof(LogInfo.Id),
                DataType = DataType.Integer,
                IsIdentity = true,
                IsPrimaryKey = true
            },
            new TableColumn
            {
                AttributeName = nameof(LogInfo.FormId),
                DataType = DataType.Integer
            },
            new TableColumn
            {
                AttributeName = nameof(LogInfo.AddDate),
                DataType = DataType.DateTime
            },
            new TableColumn
            {
                AttributeName = nameof(LogInfo.IsReplied),
                DataType = DataType.Boolean
            },
            new TableColumn
            {
                AttributeName = nameof(LogInfo.ReplyDate),
                DataType = DataType.DateTime
            },
            new TableColumn
            {
                AttributeName = nameof(LogInfo.ReplyContent),
                DataType = DataType.Text
            },
            new TableColumn
            {
                AttributeName = nameof(LogInfo.AttributeValues),
                DataType = DataType.Text
            }
        };

        public static int Insert(FormInfo formInfo, LogInfo logInfo)
        {
            var sqlString = $@"INSERT INTO {TableName}
(
    {nameof(LogInfo.FormId)},
    {nameof(LogInfo.AddDate)},
    {nameof(LogInfo.IsReplied)},
    {nameof(LogInfo.ReplyDate)},
    {nameof(LogInfo.ReplyContent)},
    {nameof(LogInfo.AttributeValues)}
) VALUES (
    @{nameof(LogInfo.FormId)},
    @{nameof(LogInfo.AddDate)},
    @{nameof(LogInfo.IsReplied)},
    @{nameof(LogInfo.ReplyDate)},
    @{nameof(LogInfo.ReplyContent)},
    @{nameof(LogInfo.AttributeValues)}
)";

            var parameters = new List<IDataParameter>
            {
                Context.DatabaseApi.GetParameter(nameof(logInfo.FormId), logInfo.FormId),
                Context.DatabaseApi.GetParameter(nameof(logInfo.AddDate), logInfo.AddDate),
                Context.DatabaseApi.GetParameter(nameof(logInfo.IsReplied), logInfo.IsReplied),
                Context.DatabaseApi.GetParameter(nameof(logInfo.ReplyDate), logInfo.ReplyDate),
                Context.DatabaseApi.GetParameter(nameof(logInfo.ReplyContent), logInfo.ReplyContent),
                Context.DatabaseApi.GetParameter(nameof(logInfo.AttributeValues), logInfo.ToString())
            };

            var logId = Context.DatabaseApi.ExecuteNonQueryAndReturnId(TableName, nameof(LogInfo.Id), Context.ConnectionString, sqlString, parameters.ToArray());

            formInfo.TotalCount += 1;
            FormDao.Update(formInfo);

            return logId;
        }

        public static void Update(LogInfo logInfo)
        {
            var sqlString = $@"UPDATE {TableName} SET
    {nameof(LogInfo.AttributeValues)} = @{nameof(LogInfo.AttributeValues)}
WHERE Id = @Id";

            var parameters = new List<IDataParameter>
            {
                Context.DatabaseApi.GetParameter(nameof(logInfo.AttributeValues), logInfo.ToString()),
                Context.DatabaseApi.GetParameter(nameof(logInfo.Id), logInfo.Id)
            };

            Context.DatabaseApi.ExecuteNonQuery(Context.ConnectionString, sqlString, parameters.ToArray());
        }

        public static void Reply(FormInfo formInfo, LogInfo logInfo)
        {
            var sqlString = $@"UPDATE {TableName} SET
    {nameof(LogInfo.IsReplied)} = @{nameof(LogInfo.IsReplied)},
    {nameof(LogInfo.ReplyDate)} = @{nameof(LogInfo.ReplyDate)},
    {nameof(LogInfo.ReplyContent)} = @{nameof(LogInfo.ReplyContent)}
WHERE Id = @Id";

            var parameters = new List<IDataParameter>
            {
                Context.DatabaseApi.GetParameter(nameof(LogInfo.IsReplied), true),
                Context.DatabaseApi.GetParameter(nameof(LogInfo.ReplyDate), DateTime.Now),
                Context.DatabaseApi.GetParameter(nameof(LogInfo.ReplyContent), logInfo.ReplyContent),
                Context.DatabaseApi.GetParameter(nameof(LogInfo.Id), logInfo.Id)
            };

            if (!logInfo.IsReplied)
            {
                formInfo.RepliedCount += 1;
                FormDao.Update(formInfo);
            }

            Context.DatabaseApi.ExecuteNonQuery(Context.ConnectionString, sqlString, parameters.ToArray());
        }

        public static void DeleteByFormId(int formId)
        {
            if (formId <= 0) return;

            var sqlString = $"DELETE FROM {TableName} WHERE {nameof(LogInfo.FormId)} = {formId}";
            Context.DatabaseApi.ExecuteNonQuery(Context.ConnectionString, sqlString);
        }

        public static void Delete(FormInfo formInfo, LogInfo logInfo)
        {
            var sqlString =
                $"DELETE FROM {TableName} WHERE {nameof(LogInfo.Id)} = {logInfo.Id}";
            Context.DatabaseApi.ExecuteNonQuery(Context.ConnectionString, sqlString);

            if (logInfo.IsReplied)
            {
                formInfo.RepliedCount -= 1;
            }
            formInfo.TotalCount -= 1;
            FormDao.Update(formInfo);
        }

        private static int GetIntResult(string sqlString)
        {
            var count = 0;

            using (var conn = Context.DatabaseApi.GetConnection(Context.ConnectionString))
            {
                conn.Open();
                using (var rdr = Context.DatabaseApi.ExecuteReader(conn, sqlString))
                {
                    if (rdr.Read() && !rdr.IsDBNull(0))
                    {
                        count = rdr.GetInt32(0);
                    }
                    rdr.Close();
                }
            }
            return count;
        }

        public static int GetCount(int formId)
        {
            var sqlString =
                $"SELECT COUNT(*) FROM {TableName} WHERE {nameof(LogInfo.FormId)} = {formId}";
            return GetIntResult(sqlString);
        }

        public static List<LogInfo> GetLogInfoList(FormInfo formInfo, bool isRepliedOnly, int page)
        {
            List<LogInfo> logInfoList;
            if (formInfo.TotalCount == 0)
            {
                logInfoList = new List<LogInfo>();
            }
            else if (formInfo.TotalCount <= FormUtils.PageSize)
            {
                logInfoList = GetLogInfoList(formInfo.Id, isRepliedOnly, 0, formInfo.TotalCount);
            }
            else
            {
                if (page == 0) page = 1;
                var offset = (page - 1) * FormUtils.PageSize;
                var limit = formInfo.TotalCount - offset > FormUtils.PageSize ? FormUtils.PageSize : formInfo.TotalCount - offset;
                logInfoList = GetLogInfoList(formInfo.Id, isRepliedOnly, offset, limit);
            }

            return logInfoList;
        }

        public static List<LogInfo> GetLogInfoList(int formId, bool isRepliedOnly, int offset, int limit)
        {
            var formLogInfoList = new List<LogInfo>();

            var whereString = $"WHERE {nameof(LogInfo.FormId)} = @{nameof(LogInfo.FormId)}";
            if (isRepliedOnly)
            {
                whereString += $" AND {nameof(LogInfo.IsReplied)} = @{nameof(LogInfo.IsReplied)}";
            }
            var sqlString = Context.DatabaseApi.GetPageSqlString(TableName, $@"{nameof(LogInfo.Id)},
    {nameof(LogInfo.FormId)},
    {nameof(LogInfo.AddDate)},
    {nameof(LogInfo.IsReplied)},
    {nameof(LogInfo.ReplyDate)},
    {nameof(LogInfo.ReplyContent)},
    {nameof(LogInfo.AttributeValues)}", whereString,
                $"ORDER BY {nameof(LogInfo.IsReplied)}, {nameof(LogInfo.Id)} DESC", offset, limit);

            var parameters = new List<IDataParameter>
            {
                Context.DatabaseApi.GetParameter(nameof(LogInfo.FormId), formId)
            };
            if (isRepliedOnly)
            {
                parameters.Add(Context.DatabaseApi.GetParameter(nameof(LogInfo.IsReplied), true));
            }

            using (var rdr = Context.DatabaseApi.ExecuteReader(Context.ConnectionString, sqlString, parameters.ToArray()))
            {
                while (rdr.Read())
                {
                    var formLogInfo = GetLogInfo(rdr);
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

            var sqlString =
                $@"SELECT {nameof(LogInfo.Id)},
    {nameof(LogInfo.FormId)},
    {nameof(LogInfo.AddDate)},
    {nameof(LogInfo.IsReplied)},
    {nameof(LogInfo.ReplyDate)},
    {nameof(LogInfo.ReplyContent)},
    {nameof(LogInfo.AttributeValues)}
            FROM {TableName} WHERE {nameof(LogInfo.Id)} = {logId}";

            using (var rdr = Context.DatabaseApi.ExecuteReader(Context.ConnectionString, sqlString))
            {
                if (rdr.Read())
                {
                    logInfo = GetLogInfo(rdr);
                }
                rdr.Close();
            }

            return logInfo;
        }

        private static LogInfo GetLogInfo(IDataRecord rdr)
        {
            if (rdr == null) return null;

            var logInfo = new LogInfo();

            var i = 0;
            logInfo.Id = rdr.IsDBNull(i) ? 0 : rdr.GetInt32(i);
            i++;
            logInfo.FormId = rdr.IsDBNull(i) ? 0 : rdr.GetInt32(i);
            i++;
            logInfo.AddDate = rdr.IsDBNull(i) ? DateTime.Now : rdr.GetDateTime(i);
            i++;
            logInfo.IsReplied = !rdr.IsDBNull(i) && rdr.GetBoolean(i);
            i++;
            logInfo.ReplyDate = rdr.IsDBNull(i) ? DateTime.Now : rdr.GetDateTime(i);
            i++;
            logInfo.ReplyContent = rdr.IsDBNull(i) ? string.Empty : rdr.GetString(i);
            i++;
            logInfo.AttributeValues = rdr.IsDBNull(i) ? string.Empty : rdr.GetString(i);

            logInfo.Load(logInfo.AttributeValues);

            return logInfo;
        }
    }
}
