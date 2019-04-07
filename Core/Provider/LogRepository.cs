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
        public LogRepository() : base(Context.DatabaseType, Context.ConnectionString)
        {

        }

        public int Insert(FormInfo formInfo, LogInfo logInfo)
        {
            logInfo.Id = base.Insert(logInfo);

            formInfo.TotalCount += 1;
            FormManager.Repository.Update(formInfo);

            return logInfo.Id;
        }

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

        public void DeleteByFormId(int formId)
        {
            if (formId <= 0) return;

            Delete(Q.Where("FormId", formId));
        }

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

        public int GetCount(int formId)
        {
            return Count(Q.Where("FormId", formId));
        }

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
    }
}
