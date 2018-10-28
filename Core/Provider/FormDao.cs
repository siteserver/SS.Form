using System;
using System.Collections.Generic;
using System.Data;
using SiteServer.Plugin;
using SS.Form.Core.Model;

namespace SS.Form.Core.Provider
{
    public static class FormDao
    {
        public const string TableName = "ss_form";

        public static List<TableColumn> Columns => new List<TableColumn>
        {
            new TableColumn
            {
                AttributeName = nameof(FormInfo.Id),
                DataType = DataType.Integer,
                IsIdentity = true,
                IsPrimaryKey = true
            },
            new TableColumn
            {
                AttributeName = nameof(FormInfo.SiteId),
                DataType = DataType.Integer
            },
            new TableColumn
            {
                AttributeName = nameof(FormInfo.ChannelId),
                DataType = DataType.Integer
            },
            new TableColumn
            {
                AttributeName = nameof(FormInfo.ContentId),
                DataType = DataType.Integer
            },
            new TableColumn
            {
                AttributeName = nameof(FormInfo.Title),
                DataType = DataType.VarChar,
                DataLength = 200
            },
            new TableColumn
            {
                AttributeName = nameof(FormInfo.Description),
                DataType = DataType.VarChar,
                DataLength = 200
            },
            new TableColumn
            {
                AttributeName = nameof(FormInfo.Taxis),
                DataType = DataType.Integer
            },
            new TableColumn
            {
                AttributeName = nameof(FormInfo.IsReply),
                DataType = DataType.Boolean
            },
            new TableColumn
            {
                AttributeName = nameof(FormInfo.RepliedCount),
                DataType = DataType.Integer
            },
            new TableColumn
            {
                AttributeName = nameof(FormInfo.TotalCount),
                DataType = DataType.Integer
            },
            new TableColumn
            {
                AttributeName = nameof(FormInfo.AddDate),
                DataType = DataType.DateTime
            },
            new TableColumn
            {
                AttributeName = nameof(FormInfo.Settings),
                DataType = DataType.Text
            }
        };

        public static int Insert(FormInfo formInfo)
        {
            if (formInfo.SiteId == 0) return 0;
            if (formInfo.ChannelId == 0 && formInfo.ContentId == 0 && string.IsNullOrEmpty(formInfo.Title)) return 0;

            int formId;

            if (formInfo.ContentId == 0)
            {
                formInfo.Taxis = GetMaxTaxis(formInfo.SiteId) + 1;
            }

            var sqlString = $@"INSERT INTO {TableName}
           ({nameof(FormInfo.SiteId)}, 
            {nameof(FormInfo.ChannelId)}, 
            {nameof(FormInfo.ContentId)}, 
            {nameof(FormInfo.Title)}, 
            {nameof(FormInfo.Description)}, 
            {nameof(FormInfo.Taxis)}, 
            {nameof(FormInfo.IsReply)}, 
            {nameof(FormInfo.RepliedCount)}, 
            {nameof(FormInfo.TotalCount)}, 
            {nameof(FormInfo.AddDate)}, 
            {nameof(FormInfo.Settings)})
     VALUES
           (@{nameof(FormInfo.SiteId)}, 
            @{nameof(FormInfo.ChannelId)}, 
            @{nameof(FormInfo.ContentId)}, 
            @{nameof(FormInfo.Title)}, 
            @{nameof(FormInfo.Description)}, 
            @{nameof(FormInfo.Taxis)}, 
            @{nameof(FormInfo.IsReply)}, 
            @{nameof(FormInfo.RepliedCount)}, 
            @{nameof(FormInfo.TotalCount)}, 
            @{nameof(FormInfo.AddDate)}, 
            @{nameof(FormInfo.Settings)})";

            var parameters = new List<IDataParameter>
            {
                Context.DatabaseApi.GetParameter(nameof(formInfo.SiteId), formInfo.SiteId),
                Context.DatabaseApi.GetParameter(nameof(formInfo.ChannelId), formInfo.ChannelId),
                Context.DatabaseApi.GetParameter(nameof(formInfo.ContentId), formInfo.ContentId),
                Context.DatabaseApi.GetParameter(nameof(formInfo.Title), formInfo.Title),
                Context.DatabaseApi.GetParameter(nameof(formInfo.Description), formInfo.Description),
                Context.DatabaseApi.GetParameter(nameof(formInfo.Taxis), formInfo.Taxis),
                Context.DatabaseApi.GetParameter(nameof(formInfo.IsReply), formInfo.IsReply),
                Context.DatabaseApi.GetParameter(nameof(formInfo.RepliedCount), formInfo.RepliedCount),
                Context.DatabaseApi.GetParameter(nameof(formInfo.TotalCount), formInfo.TotalCount),
                Context.DatabaseApi.GetParameter(nameof(formInfo.AddDate), formInfo.AddDate),
                Context.DatabaseApi.GetParameter(nameof(formInfo.Settings), formInfo.Settings)
            };

            using (var conn = Context.DatabaseApi.GetConnection(Context.ConnectionString))
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        formId = Context.DatabaseApi.ExecuteNonQueryAndReturnId(TableName, nameof(FormInfo.Id), trans, sqlString, parameters.ToArray());

                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }

            FormManager.ClearCache(formInfo.SiteId);

            return formId;
        }

        public static void Update(FormInfo formInfo)
        {
            var sqlString = $@"UPDATE {TableName} SET
                {nameof(FormInfo.SiteId)} = @{nameof(FormInfo.SiteId)}, 
                {nameof(FormInfo.ChannelId)} = @{nameof(FormInfo.ChannelId)}, 
                {nameof(FormInfo.ContentId)} = @{nameof(FormInfo.ContentId)}, 
                {nameof(FormInfo.Title)} = @{nameof(FormInfo.Title)}, 
                {nameof(FormInfo.Description)} = @{nameof(FormInfo.Description)},
                {nameof(FormInfo.Taxis)} = @{nameof(FormInfo.Taxis)}, 
                {nameof(FormInfo.IsReply)} = @{nameof(FormInfo.IsReply)}, 
                {nameof(FormInfo.RepliedCount)} = @{nameof(FormInfo.RepliedCount)}, 
                {nameof(FormInfo.TotalCount)} = @{nameof(FormInfo.TotalCount)}, 
                {nameof(FormInfo.AddDate)} = @{nameof(FormInfo.AddDate)}, 
                {nameof(FormInfo.Settings)} = @{nameof(FormInfo.Settings)}
            WHERE {nameof(FormInfo.Id)} = @{nameof(FormInfo.Id)}";

            var parameters = new List<IDataParameter>
            {
                Context.DatabaseApi.GetParameter(nameof(formInfo.SiteId), formInfo.SiteId),
                Context.DatabaseApi.GetParameter(nameof(formInfo.ChannelId), formInfo.ChannelId),
                Context.DatabaseApi.GetParameter(nameof(formInfo.ContentId), formInfo.ContentId),
                Context.DatabaseApi.GetParameter(nameof(formInfo.Title), formInfo.Title),
                Context.DatabaseApi.GetParameter(nameof(formInfo.Description), formInfo.Description),
                Context.DatabaseApi.GetParameter(nameof(formInfo.Taxis), formInfo.Taxis),
                Context.DatabaseApi.GetParameter(nameof(formInfo.IsReply), formInfo.IsReply),
                Context.DatabaseApi.GetParameter(nameof(formInfo.RepliedCount), formInfo.RepliedCount > 0 ? formInfo.RepliedCount : 0),
                Context.DatabaseApi.GetParameter(nameof(formInfo.TotalCount), formInfo.TotalCount > 0 ? formInfo.TotalCount : 0),
                Context.DatabaseApi.GetParameter(nameof(formInfo.AddDate), formInfo.AddDate),
                Context.DatabaseApi.GetParameter(nameof(formInfo.Settings), formInfo.Additional.ToString()),
                Context.DatabaseApi.GetParameter(nameof(formInfo.Id), formInfo.Id)
            };

            Context.DatabaseApi.ExecuteNonQuery(Context.ConnectionString, sqlString, parameters.ToArray());

            FormManager.UpdateCache(formInfo);
        }
        
        public static void Delete(int siteId, int formId)
        {
            if (formId <= 0) return;

            var sqlString = $"DELETE FROM {TableName} WHERE {nameof(FormInfo.Id)} = {formId}";
            Context.DatabaseApi.ExecuteNonQuery(Context.ConnectionString, sqlString);

            FieldDao.DeleteByFormId(formId);
            LogDao.DeleteByFormId(formId);

            FormManager.ClearCache(siteId);
        }

        public static FormInfo CreateDefaultForm(int siteId, int channelId, int contentId)
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
                Additional = new FormSettings(string.Empty)
            };
            formInfo.Id = Insert(formInfo);

            FieldDao.Insert(siteId, new FieldInfo
            {
                FormId = formInfo.Id,
                Title = "姓名",
                PlaceHolder = "请输入您的姓名",
                FieldType = InputType.Text.Value,
                Validate = "required",
                Settings = string.Empty
            });
            FieldDao.Insert(siteId, new FieldInfo
            {
                FormId = formInfo.Id,
                Title = "手机",
                PlaceHolder = "请输入您的手机号码",
                FieldType = InputType.Text.Value,
                Validate = "mobile",
                Settings = string.Empty
            });
            FieldDao.Insert(siteId, new FieldInfo
            {
                FormId = formInfo.Id,
                Title = "邮箱",
                PlaceHolder = "请输入您的电子邮箱",
                FieldType = InputType.Text.Value,
                Validate = "email",
                Settings = string.Empty
            });

            FieldDao.Insert(siteId, new FieldInfo
            {
                FormId = formInfo.Id,
                Title = "留言",
                PlaceHolder = "请输入您的留言",
                Validate = "required",
                FieldType = InputType.TextArea.Value
            });

            return formInfo;
        }

        public static void UpdateTaxisToUp(int siteId, int formId)
        {
            var sqlString = Context.DatabaseApi.GetPageSqlString(TableName, $"{nameof(FormInfo.Id)}, {nameof(FormInfo.Taxis)}", $"WHERE (({nameof(FormInfo.Taxis)} > (SELECT {nameof(FormInfo.Taxis)} FROM {TableName} WHERE {nameof(FormInfo.Id)} = {formId})) AND {nameof(FormInfo.SiteId)} ={siteId})", $"ORDER BY {nameof(FormInfo.Taxis)}", 0, 1);

            var higherId = 0;
            var higherTaxis = 0;

            using (var rdr = Context.DatabaseApi.ExecuteReader(Context.ConnectionString, sqlString))
            {
                if (rdr.Read())
                {
                    higherId = rdr.GetInt32(0);
                    higherTaxis = rdr.GetInt32(1);
                }
                rdr.Close();
            }

            var selectedTaxis = GetTaxis(formId);

            if (higherId != 0)
            {
                SetTaxis(siteId, formId, higherTaxis);
                SetTaxis(siteId, higherId, selectedTaxis);
            }
        }

        public static void UpdateTaxisToDown(int siteId, int formId)
        {
            var sqlString = Context.DatabaseApi.GetPageSqlString(TableName, $"{nameof(FormInfo.Id)}, {nameof(FormInfo.Taxis)}", $"WHERE (({nameof(FormInfo.Taxis)} < (SELECT {nameof(FormInfo.Taxis)} FROM {TableName} WHERE ({nameof(FormInfo.Id)} = {formId}))) AND {nameof(FormInfo.SiteId)} = {siteId})", $"ORDER BY {nameof(FormInfo.Taxis)} DESC", 0, 1);

            var lowerId = 0;
            var lowerTaxis = 0;

            using (var rdr = Context.DatabaseApi.ExecuteReader(Context.ConnectionString, sqlString))
            {
                if (rdr.Read())
                {
                    lowerId = rdr.GetInt32(0);
                    lowerTaxis = rdr.GetInt32(1);
                }
                rdr.Close();
            }

            var selectedTaxis = GetTaxis(formId);

            if (lowerId != 0)
            {
                SetTaxis(siteId, formId, lowerTaxis);
                SetTaxis(siteId, lowerId, selectedTaxis);
            }
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

        private static int GetMaxTaxis(int siteId)
        {
            var sqlString =
                $"SELECT MAX({nameof(FormInfo.Taxis)}) FROM {TableName} WHERE {nameof(FormInfo.SiteId)} = {siteId}";
            return GetIntResult(sqlString);
        }

        private static int GetTaxis(int formId)
        {
            var sqlString = $"SELECT {nameof(FormInfo.Taxis)} FROM {TableName} WHERE ({nameof(FormInfo.Id)} = {formId})";
            return GetIntResult(sqlString);
        }

        private static void SetTaxis(int siteId, int formId, int taxis)
        {
            var sqlString = $"UPDATE {TableName} SET {nameof(FormInfo.Taxis)} = {taxis} WHERE {nameof(FormInfo.Id)} = {formId}";
            Context.DatabaseApi.ExecuteNonQuery(Context.ConnectionString, sqlString);

            FormManager.ClearCache(siteId);
        }

        private static FormInfo GetFormInfo(IDataRecord rdr)
        {
            if (rdr == null) return null;
            
            var formInfo = new FormInfo();

            var i = 0;
            formInfo.Id = rdr.IsDBNull(i) ? 0 : rdr.GetInt32(i);
            i++;
            formInfo.SiteId = rdr.IsDBNull(i) ? 0 : rdr.GetInt32(i);
            i++;
            formInfo.ChannelId = rdr.IsDBNull(i) ? 0 : rdr.GetInt32(i);
            i++;
            formInfo.ContentId = rdr.IsDBNull(i) ? 0 : rdr.GetInt32(i);
            i++;
            formInfo.Title = rdr.IsDBNull(i) ? string.Empty : rdr.GetString(i);
            i++;
            formInfo.Description = rdr.IsDBNull(i) ? string.Empty : rdr.GetString(i);
            i++;
            formInfo.Taxis = rdr.IsDBNull(i) ? 0 : rdr.GetInt32(i);
            i++;
            formInfo.IsReply = !rdr.IsDBNull(i) && rdr.GetBoolean(i);
            i++;
            formInfo.RepliedCount = rdr.IsDBNull(i) ? 0 : rdr.GetInt32(i);
            i++;
            formInfo.TotalCount = rdr.IsDBNull(i) ? 0 : rdr.GetInt32(i);
            i++;
            formInfo.AddDate = rdr.IsDBNull(i) ? DateTime.Now : rdr.GetDateTime(i);
            i++;
            formInfo.Settings = rdr.IsDBNull(i) ? string.Empty : rdr.GetString(i);

            return formInfo;
        }

        public static List<FormInfo> GetFormInfoList(int siteId)
        {
            var list = new List<FormInfo>();

            var sqlString = $@"SELECT {nameof(FormInfo.Id)}, 
            {nameof(FormInfo.SiteId)}, 
            {nameof(FormInfo.ChannelId)}, 
            {nameof(FormInfo.ContentId)}, 
            {nameof(FormInfo.Title)}, 
            {nameof(FormInfo.Description)}, 
            {nameof(FormInfo.Taxis)}, 
            {nameof(FormInfo.IsReply)}, 
            {nameof(FormInfo.RepliedCount)}, 
            {nameof(FormInfo.TotalCount)}, 
            {nameof(FormInfo.AddDate)}, 
            {nameof(FormInfo.Settings)}
            FROM {TableName}
            WHERE {nameof(FormInfo.SiteId)} = {siteId} 
            ORDER BY {nameof(FormInfo.Taxis)} DESC, {nameof(FormInfo.Id)} DESC";

            using (var rdr = Context.DatabaseApi.ExecuteReader(Context.ConnectionString, sqlString))
            {
                while (rdr.Read())
                {
                    var formInfo = GetFormInfo(rdr);
                    formInfo.Additional = new FormSettings(formInfo.Settings);

                    list.Add(formInfo);
                }
                rdr.Close();
            }

            return list;
        }
    }
}
