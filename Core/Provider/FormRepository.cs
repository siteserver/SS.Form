using System;
using System.Collections.Generic;
using Datory;
using SiteServer.Plugin;
using SS.Form.Core.Model;

namespace SS.Form.Core.Provider
{
    public class FormRepository
    {
        private readonly Repository<FormInfo> _repository;

        public string TableName => _repository.TableName;

        public List<TableColumn> TableColumns => _repository.TableColumns;

        public FormRepository()
        {
            _repository = new Repository<FormInfo>(Context.Environment.DatabaseType, Context.Environment.ConnectionString);
        }

        public int Insert(FormInfo formInfo)
        {
            if (formInfo.SiteId == 0) return 0;
            if (formInfo.ChannelId == 0 && formInfo.ContentId == 0 && string.IsNullOrEmpty(formInfo.Title)) return 0;

            if (formInfo.ContentId == 0)
            {
                formInfo.Taxis = GetMaxTaxis(formInfo.SiteId) + 1;
            }

            formInfo.Id = _repository.Insert(formInfo);

            FormManager.ClearCache(formInfo.SiteId);

            return formInfo.Id;
        }

        public bool Update(FormInfo formInfo)
        {
            var updated = _repository.Update(formInfo);

            FormManager.UpdateCache(formInfo);

            return updated;
        }

        public void Delete(int siteId, int formId)
        {
            if (formId <= 0) return;

            _repository.Delete(formId);

            var fieldRepository = new FieldRepository();

            fieldRepository.DeleteByFormId(formId);
            LogManager.Repository.DeleteByFormId(formId);

            FormManager.ClearCache(siteId);
        }

        public FormInfo CreateDefaultForm(int siteId, int channelId, int contentId)
        {
            var formInfo = new FormInfo
            {
                SiteId = siteId,
                ChannelId = channelId,
                ContentId = contentId,
                Title = "默认表单",
                Description = string.Empty,
                IsReply = false,
                RepliedCount = 0,
                TotalCount = 0,
                AddDate = DateTime.Now,
                Settings = string.Empty
            };
            formInfo.Id = Insert(formInfo);

            FieldManager.Repository.Insert(siteId, new FieldInfo
            {
                FormId = formInfo.Id,
                Title = "姓名",
                PlaceHolder = "请输入您的姓名",
                FieldType = InputType.Text.Value,
                Validate = "required"
            });

            FieldManager.Repository.Insert(siteId, new FieldInfo
            {
                FormId = formInfo.Id,
                Title = "手机",
                PlaceHolder = "请输入您的手机号码",
                FieldType = InputType.Text.Value,
                Validate = "mobile"
            });

            FieldManager.Repository.Insert(siteId, new FieldInfo
            {
                FormId = formInfo.Id,
                Title = "邮箱",
                PlaceHolder = "请输入您的电子邮箱",
                FieldType = InputType.Text.Value,
                Validate = "email"
            });

            FieldManager.Repository.Insert(siteId, new FieldInfo
            {
                FormId = formInfo.Id,
                Title = "留言",
                PlaceHolder = "请输入您的留言",
                Validate = "required",
                FieldType = InputType.TextArea.Value
            });

            return formInfo;
        }

        public void UpdateTaxisToDown(int siteId, int formId)
        {
            var taxis = _repository.Get<int>(Q.Where("Id", formId));

            var dataInfo = _repository.Get(Q
                .Where("SiteId", siteId)
                .Where("Taxis", ">", taxis)
                .OrderBy("Taxis")
            );

            if (dataInfo == null) return;

            var higherId = dataInfo.Id;
            var higherTaxis = dataInfo.Taxis;

            SetTaxis(siteId, formId, higherTaxis);
            SetTaxis(siteId, higherId, taxis);
        }

        public void UpdateTaxisToUp(int siteId, int formId)
        {
            var taxis = _repository.Get<int>(Q.Where("Id", formId));

            var dataInfo = _repository.Get(Q
                .Where("SiteId", siteId)
                .Where("Taxis", "<", taxis)
                .OrderByDesc("Taxis")
            );

            if (dataInfo == null) return;

            var lowerId = dataInfo.Id;
            var lowerTaxis = dataInfo.Taxis;

            SetTaxis(siteId, formId, lowerTaxis);
            SetTaxis(siteId, lowerId, taxis);
        }

        private int GetMaxTaxis(int siteId)
        {
            return _repository.Max("Taxis", Q.Where("SiteId", siteId)) ?? 0;
        }

        private void SetTaxis(int siteId, int formId, int taxis)
        {
            _repository.Update(Q.Set("Taxis", taxis).Where("Id", formId));

            FormManager.ClearCache(siteId);
        }

        public IList<FormInfo> GetFormInfoList(int siteId)
        {
            return _repository.GetAll(Q.Where("SiteId", siteId).OrderByDesc("Taxis", "Id"));
        }

        public string GetImportTitle(int siteId, string title)
        {
            string importTitle;
            if (title.IndexOf("_", StringComparison.Ordinal) != -1)
            {
                var inputNameCount = 0;
                var lastInputName = title.Substring(title.LastIndexOf("_", StringComparison.Ordinal) + 1);
                var firstInputName = title.Substring(0, title.Length - lastInputName.Length);
                try
                {
                    inputNameCount = int.Parse(lastInputName);
                }
                catch
                {
                    // ignored
                }
                inputNameCount++;
                importTitle = firstInputName + inputNameCount;
            }
            else
            {
                importTitle = title + "_1";
            }

            var inputInfo = FormManager.GetFormInfoByTitle(siteId, title);
            if (inputInfo != null)
            {
                importTitle = GetImportTitle(siteId, importTitle);
            }

            return importTitle;
        }
    }
}
