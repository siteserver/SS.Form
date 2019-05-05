using System;
using System.Collections.Generic;
using Datory;
using SiteServer.Plugin;
using SS.Form.Core.Model;
using SS.Form.Core.Utils;

namespace SS.Form.Core.Repositories
{
    public class LogRepository
    {
        private readonly Repository<LogInfo> _repository;

        public string TableName => _repository.TableName;

        public List<TableColumn> TableColumns => _repository.TableColumns;

        public LogRepository()
        {
            _repository = new Repository<LogInfo>(Context.Environment.DatabaseType, Context.Environment.ConnectionString);
        }

        private static class Attr
        {
            public const string Id = nameof(LogInfo.Id);

            public const string FormId = nameof(LogInfo.FormId);
        }

        public int Insert(FormInfo formInfo, LogInfo logInfo)
        {
            logInfo.FormId = formInfo.Id;
            logInfo.Id = _repository.Insert(logInfo);

            formInfo.TotalCount += 1;
            FormManager.Repository.Update(formInfo);

            return logInfo.Id;
        }

        public void Update(LogInfo logInfo)
        {
            _repository.Update(logInfo);
        }

        public LogInfo GetLogInfo(int logId)
        {
            return _repository.Get(logId);
        }

        public void Reply(FormInfo formInfo, LogInfo logInfo)
        {
            _repository.Update(Q
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

            _repository.Delete(Q.Where(Attr.FormId, formId));
        }

        public void Delete(FormInfo formInfo, LogInfo logInfo)
        {
            _repository.Delete(logInfo.Id);

            if (logInfo.IsReplied)
            {
                formInfo.RepliedCount -= 1;
            }
            formInfo.TotalCount -= 1;
            FormManager.Repository.Update(formInfo);
        }

        public int GetCount(int formId)
        {
            return _repository.Count(Q.Where(Attr.FormId, formId));
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
                .Where(Attr.FormId, formId)
                .OrderByDesc(nameof(LogInfo.IsReplied), Attr.Id);

            if (offset > 0)
            {
                q.Offset(offset);
            }

            if (limit > 0)
            {
                q.Limit(limit);
            }

            if (isRepliedOnly)
            {
                q.Where(nameof(LogInfo.IsReplied), true);
            }

            return _repository.GetAll(q);
        }
    }
}
