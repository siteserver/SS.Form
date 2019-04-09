using System;
using System.Collections.Generic;
using Datory;
using SiteServer.Plugin;
using SS.Form.Core.Model;
using SS.Form.Core.Utils;

namespace SS.Form.Core.Provider
{
    public class LogRepository : Repository<LogInfo>
    {
        public LogRepository() : base(Context.Environment.Database)
        {

        }

        public int Insert(FormInfo formInfo, LogInfo logInfo)
        {
            logInfo.Id = base.Insert(logInfo);

            formInfo.TotalCount += 1;
            FormManager.Repository.Update(formInfo);

            return logInfo.Id;
        }

        //        public static int Insert(FormInfo formInfo, LogInfo logInfo)
        //        {
        //            var sqlString = $@"INSERT INTO {TableName}
        //(
        //    {nameof(LogInfo.FormId)},
        //    {nameof(LogInfo.AddDate)},
        //    {nameof(LogInfo.IsReplied)},
        //    {nameof(LogInfo.ReplyDate)},
        //    {nameof(LogInfo.ReplyContent)},
        //    {nameof(LogInfo.AttributeValues)}
        //) VALUES (
        //    @{nameof(LogInfo.FormId)},
        //    @{nameof(LogInfo.AddDate)},
        //    @{nameof(LogInfo.IsReplied)},
        //    @{nameof(LogInfo.ReplyDate)},
        //    @{nameof(LogInfo.ReplyContent)},
        //    @{nameof(LogInfo.AttributeValues)}
        //)";

        //            var parameters = new List<IDataParameter>
        //            {
        //                Context.DatabaseApi.GetParameter(nameof(logInfo.FormId), logInfo.FormId),
        //                Context.DatabaseApi.GetParameter(nameof(logInfo.AddDate), logInfo.AddDate),
        //                Context.DatabaseApi.GetParameter(nameof(logInfo.IsReplied), logInfo.IsReplied),
        //                Context.DatabaseApi.GetParameter(nameof(logInfo.ReplyDate), logInfo.ReplyDate),
        //                Context.DatabaseApi.GetParameter(nameof(logInfo.ReplyContent), logInfo.ReplyContent),
        //                Context.DatabaseApi.GetParameter(nameof(logInfo.AttributeValues), logInfo.ToString())
        //            };

        //            var logId = Context.DatabaseApi.ExecuteNonQueryAndReturnId(TableName, nameof(LogInfo.Id), Context.ConnectionString, sqlString, parameters.ToArray());

        //            formInfo.TotalCount += 1;
        //            FormDao.Update(formInfo);

        //            return logId;
        //        }

        //        public static void Update(LogInfo logInfo)
        //        {
        //            var sqlString = $@"UPDATE {TableName} SET
        //    {nameof(LogInfo.AttributeValues)} = @{nameof(LogInfo.AttributeValues)}
        //WHERE Id = @Id";

        //            var parameters = new List<IDataParameter>
        //            {
        //                Context.DatabaseApi.GetParameter(nameof(logInfo.AttributeValues), logInfo.ToString()),
        //                Context.DatabaseApi.GetParameter(nameof(logInfo.Id), logInfo.Id)
        //            };

        //            Context.DatabaseApi.ExecuteNonQuery(Context.ConnectionString, sqlString, parameters.ToArray());
        //        }

        public void Reply(FormInfo formInfo, LogInfo logInfo)
        {
            Update(Q
                .Set(nameof(LogInfo.IsReplied), true)
                .Set(nameof(LogInfo.ReplyDate), DateTime.Now)
                .Set(nameof(LogInfo.ReplyContent), logInfo.ReplyContent)
                .Where("Id", logInfo.Id)
            );

            if (!logInfo.IsReplied)
            {
                formInfo.RepliedCount += 1;
                FormManager.Repository.Update(formInfo);
            }
        }

        //        public static void Reply(FormInfo formInfo, LogInfo logInfo)
        //        {
        //            var sqlString = $@"UPDATE {TableName} SET
        //    {nameof(LogInfo.IsReplied)} = @{nameof(LogInfo.IsReplied)},
        //    {nameof(LogInfo.ReplyDate)} = @{nameof(LogInfo.ReplyDate)},
        //    {nameof(LogInfo.ReplyContent)} = @{nameof(LogInfo.ReplyContent)}
        //WHERE Id = @Id";

        //            var parameters = new List<IDataParameter>
        //            {
        //                Context.DatabaseApi.GetParameter(nameof(LogInfo.IsReplied), true),
        //                Context.DatabaseApi.GetParameter(nameof(LogInfo.ReplyDate), DateTime.Now),
        //                Context.DatabaseApi.GetParameter(nameof(LogInfo.ReplyContent), logInfo.ReplyContent),
        //                Context.DatabaseApi.GetParameter(nameof(LogInfo.Id), logInfo.Id)
        //            };

        //            if (!logInfo.IsReplied)
        //            {
        //                formInfo.RepliedCount += 1;
        //                FormDao.Update(formInfo);
        //            }

        //            Context.DatabaseApi.ExecuteNonQuery(Context.ConnectionString, sqlString, parameters.ToArray());
        //        }

        public void DeleteByFormId(int formId)
        {
            if (formId <= 0) return;

            Delete(Q.Where("FormId", formId));
        }

        //public static void DeleteByFormId(int formId)
        //{
        //    if (formId <= 0) return;

        //    var sqlString = $"DELETE FROM {TableName} WHERE {nameof(LogInfo.FormId)} = {formId}";
        //    Context.DatabaseApi.ExecuteNonQuery(Context.ConnectionString, sqlString);
        //}

        public void Delete(FormInfo formInfo, LogInfo logInfo)
        {
            Delete(logInfo.Id);

            if (logInfo.IsReplied)
            {
                formInfo.RepliedCount -= 1;
            }
            formInfo.TotalCount -= 1;
            FormManager.Repository.Update(formInfo);
        }

        //public static void Delete(FormInfo formInfo, LogInfo logInfo)
        //{
        //    var sqlString =
        //        $"DELETE FROM {TableName} WHERE {nameof(LogInfo.Id)} = {logInfo.Id}";
        //    Context.DatabaseApi.ExecuteNonQuery(Context.ConnectionString, sqlString);

        //    if (logInfo.IsReplied)
        //    {
        //        formInfo.RepliedCount -= 1;
        //    }
        //    formInfo.TotalCount -= 1;
        //    FormDao.Update(formInfo);
        //}

        public int GetCount(int formId)
        {
            return Count(Q.Where("FormId", formId));
        }

        //public static int GetCount(int formId)
        //{
        //    var sqlString =
        //        $"SELECT COUNT(*) FROM {TableName} WHERE {nameof(LogInfo.FormId)} = {formId}";
        //    return GetIntResult(sqlString);
        //}

        public IList<LogInfo> GetLogInfoList(FormInfo formInfo, bool isRepliedOnly, int page)
        {
            if (formInfo.TotalCount == 0)
            {
                return new List<LogInfo>();
            }

            if (formInfo.TotalCount <= FormUtils.PageSize)
            {
                return GetLogInfoList(formInfo.Id, isRepliedOnly, 0, formInfo.TotalCount);
            }

            if (page == 0) page = 1;
            var offset = (page - 1) * FormUtils.PageSize;
            var limit = formInfo.TotalCount - offset > FormUtils.PageSize ? FormUtils.PageSize : formInfo.TotalCount - offset;
            return GetLogInfoList(formInfo.Id, isRepliedOnly, offset, limit);
        }

        //public static List<LogInfo> GetLogInfoList(FormInfo formInfo, bool isRepliedOnly, int page)
        //{
        //    List<LogInfo> logInfoList;
        //    if (formInfo.TotalCount == 0)
        //    {
        //        logInfoList = new List<LogInfo>();
        //    }
        //    else if (formInfo.TotalCount <= FormUtils.PageSize)
        //    {
        //        logInfoList = GetLogInfoList(formInfo.Id, isRepliedOnly, 0, formInfo.TotalCount);
        //    }
        //    else
        //    {
        //        if (page == 0) page = 1;
        //        var offset = (page - 1) * FormUtils.PageSize;
        //        var limit = formInfo.TotalCount - offset > FormUtils.PageSize ? FormUtils.PageSize : formInfo.TotalCount - offset;
        //        logInfoList = GetLogInfoList(formInfo.Id, isRepliedOnly, offset, limit);
        //    }

        //    return logInfoList;
        //}

        public IList<LogInfo> GetLogInfoList(int formId, bool isRepliedOnly, int offset, int limit)
        {
            var q = Q
                .Where("FormId", formId)
                .Offset(offset)
                .Limit(limit)
                .OrderByDesc(nameof(LogInfo.IsReplied), "Id");

            if (isRepliedOnly)
            {
                q.Where(nameof(LogInfo.IsReplied), true);
            }

            return GetAll(q);
        }

    //    public static List<LogInfo> GetLogInfoList(int formId, bool isRepliedOnly, int offset, int limit)
    //    {
    //        var formLogInfoList = new List<LogInfo>();

    //        var whereString = $"WHERE {nameof(LogInfo.FormId)} = @{nameof(LogInfo.FormId)}";
    //        if (isRepliedOnly)
    //        {
    //            whereString += $" AND {nameof(LogInfo.IsReplied)} = @{nameof(LogInfo.IsReplied)}";
    //        }
    //        var sqlString = Context.DatabaseApi.GetPageSqlString(TableName, $@"{nameof(LogInfo.Id)},
    //{nameof(LogInfo.FormId)},
    //{nameof(LogInfo.AddDate)},
    //{nameof(LogInfo.IsReplied)},
    //{nameof(LogInfo.ReplyDate)},
    //{nameof(LogInfo.ReplyContent)},
    //{nameof(LogInfo.AttributeValues)}", whereString,
    //            $"ORDER BY {nameof(LogInfo.IsReplied)}, {nameof(LogInfo.Id)} DESC", offset, limit);

    //        var parameters = new List<IDataParameter>
    //        {
    //            Context.DatabaseApi.GetParameter(nameof(LogInfo.FormId), formId)
    //        };
    //        if (isRepliedOnly)
    //        {
    //            parameters.Add(Context.DatabaseApi.GetParameter(nameof(LogInfo.IsReplied), true));
    //        }

    //        using (var rdr = Context.DatabaseApi.ExecuteReader(Context.ConnectionString, sqlString, parameters.ToArray()))
    //        {
    //            while (rdr.Read())
    //            {
    //                var formLogInfo = GetLogInfo(rdr);
    //                if (!string.IsNullOrEmpty(formLogInfo.AttributeValues))
    //                {
    //                    formLogInfoList.Add(formLogInfo);
    //                }
    //            }
    //            rdr.Close();
    //        }

    //        return formLogInfoList;
    //    }

    //    public static LogInfo GetLogInfo(int logId)
    //    {
    //        LogInfo logInfo = null;

    //        var sqlString =
    //            $@"SELECT {nameof(LogInfo.Id)},
    //{nameof(LogInfo.FormId)},
    //{nameof(LogInfo.AddDate)},
    //{nameof(LogInfo.IsReplied)},
    //{nameof(LogInfo.ReplyDate)},
    //{nameof(LogInfo.ReplyContent)},
    //{nameof(LogInfo.AttributeValues)}
    //        FROM {TableName} WHERE {nameof(LogInfo.Id)} = {logId}";

    //        using (var rdr = Context.DatabaseApi.ExecuteReader(Context.ConnectionString, sqlString))
    //        {
    //            if (rdr.Read())
    //            {
    //                logInfo = GetLogInfo(rdr);
    //            }
    //            rdr.Close();
    //        }

    //        return logInfo;
    //    }
    }
}
