using System;
using System.Collections.Generic;
using System.Web.Http;
using SiteServer.Plugin;
using SS.Form.Core;
using SS.Form.Core.Model;
using SS.Form.Core.Utils;

namespace SS.Form.Controllers.Pages
{
    [RoutePrefix("pages/fieldsLayerModel")]
    public class PagesFieldsLayerModelController : ApiController
    {
        private const string Route = "";

        [HttpGet, Route(Route)]
        public IHttpActionResult Get()
        {
            try
            {
                var request = Context.AuthenticatedRequest;

                var formInfo = FormManager.GetFormInfoByGet(request);
                if (formInfo == null) return NotFound();
                if (!request.IsAdminLoggin || !request.AdminPermissions.HasSitePermissions(formInfo.SiteId, FormUtils.PluginId)) return Unauthorized();

                var fieldId = request.GetQueryInt("fieldId");
                var fieldInfo = FieldManager.GetFieldInfo(formInfo.Id, fieldId) ?? new FieldInfo();

                var isRapid = true;
                var rapidValues = string.Empty;
                if (fieldInfo.Items.Count == 0)
                {
                    fieldInfo.Items.Add(new FieldItemInfo
                    {
                        Value = string.Empty,
                        IsSelected = false
                    });
                }
                else
                {
                    var isSelected = false;
                    var list = new List<string>();
                    foreach (var item in fieldInfo.Items)
                    {
                        list.Add(item.Value);
                        if (item.IsSelected)
                        {
                            isSelected = true;
                        }
                    }

                    isRapid = !isSelected;
                    rapidValues = string.Join("\r\n", list);
                }

                return Ok(new
                {
                    Value = fieldInfo,
                    IsRapid = isRapid,
                    RapidValues = rapidValues
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost, Route(Route)]
        public IHttpActionResult Submit()
        {
            try
            {
                var request = Context.AuthenticatedRequest;

                var formInfo = FormManager.GetFormInfoByPost(request);
                if (formInfo == null) return NotFound();
                if (!request.IsAdminLoggin || !request.AdminPermissions.HasSitePermissions(formInfo.SiteId, FormUtils.PluginId)) return Unauthorized();

                var fieldId = request.GetPostInt("fieldId");
                var isRapid = request.GetPostBool("isRapid");
                var rapidValues = request.GetPostString("rapidValues");
                var rapidValueArray = rapidValues.Split('\n');
                var rapidValueList = new List<string>();
                foreach (var item in rapidValueArray)
                {
                    if (!string.IsNullOrEmpty(item))
                    {
                        rapidValueList.Add(item.Trim());
                    }
                }

                var fieldInfo = request.GetPostObject<FieldInfo>("fieldInfo");
                var itemInfoList = request.GetPostObject<List<FieldItemInfo>>("items");
                fieldInfo.Items = itemInfoList;

                var fieldInfoDatabase =
                    FieldManager.GetFieldInfo(formInfo.Id, fieldId) ??
                    new FieldInfo();

                string errorMessage;
                var isSuccess = fieldInfoDatabase.Id == 0 ? InsertFieldInfo(formInfo.SiteId, formInfo.Id, fieldInfo, isRapid, rapidValueList, out errorMessage) : UpdateFieldInfo(fieldInfoDatabase, fieldInfo, isRapid, rapidValueList, out errorMessage);

                if (!isSuccess)
                {
                    return BadRequest(errorMessage);
                }

                return Ok(new{});
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        private static bool InsertFieldInfo(int siteId, int formId, FieldInfo body, bool isRapid, List<string> rapidValues, out string errorMessage)
        {
            errorMessage = string.Empty;

            if (string.IsNullOrEmpty(body.Title))
            {
                errorMessage = "操作失败，字段名不能为空！";
                return false;
            }

            if (FieldManager.Repository.IsTitleExists(formId, body.Title))
            {
                errorMessage = $@"显示样式添加失败：字段名""{body.Title}""已存在";
                return false;
            }

            var fieldInfo = new FieldInfo
            {
                FormId = formId,
                Title = body.Title,
                Taxis = body.Taxis,
                FieldType = body.FieldType,
                Columns = body.Columns,
                Height = body.Height,
                Items = new List<FieldItemInfo>()
            };

            if (body.FieldType == InputType.CheckBox.Value || body.FieldType == InputType.Radio.Value || body.FieldType == InputType.SelectMultiple.Value || body.FieldType == InputType.SelectOne.Value)
            {
                if (isRapid)
                {
                    foreach (var rapidValue in rapidValues)
                    {
                        var itemInfo = new FieldItemInfo
                        {
                            FormId = formId,
                            Value = rapidValue
                        };
                        fieldInfo.Items.Add(itemInfo);
                    }
                }
                else
                {
                    var isHasSelected = false;
                    foreach (var styleItem in body.Items)
                    {
                        if (body.FieldType != InputType.SelectMultiple.Value && body.FieldType != InputType.CheckBox.Value && isHasSelected && styleItem.IsSelected)
                        {
                            errorMessage = "操作失败，只能有一个初始化时选定项！";
                            return false;
                        }
                        if (styleItem.IsSelected) isHasSelected = true;

                        var itemInfo = new FieldItemInfo
                        {
                            Value = styleItem.Value,
                            IsSelected = styleItem.IsSelected
                        };
                        fieldInfo.Items.Add(itemInfo);
                    }
                }
            }

            FieldManager.Repository.Insert(siteId, fieldInfo);

            return true;
        }

        private static bool UpdateFieldInfo(FieldInfo fieldInfo, FieldInfo body, bool isRapid, List<string> rapidValues, out string errorMessage)
        {
            errorMessage = string.Empty;

            fieldInfo.Title = body.Title;
            fieldInfo.Taxis = body.Taxis;
            fieldInfo.FieldType = body.FieldType;
            fieldInfo.Columns = body.Columns;
            fieldInfo.Height = body.Height;
            fieldInfo.Items = new List<FieldItemInfo>();

            if (body.FieldType == InputType.CheckBox.Value || body.FieldType == InputType.Radio.Value || body.FieldType == InputType.SelectMultiple.Value || body.FieldType == InputType.SelectOne.Value)
            {
                if (isRapid)
                {
                    foreach (var rapidValue in rapidValues)
                    {
                        var itemInfo = new FieldItemInfo
                        {
                            FieldId = fieldInfo.Id,
                            Value = rapidValue
                        };
                        fieldInfo.Items.Add(itemInfo);
                    }
                }
                else
                {
                    var isHasSelected = false;
                    foreach (var styleItem in body.Items)
                    {
                        if (body.FieldType != InputType.SelectMultiple.Value && body.FieldType != InputType.CheckBox.Value && isHasSelected && styleItem.IsSelected)
                        {
                            errorMessage = "操作失败，只能有一个初始化时选定项！";
                            return false;
                        }
                        if (styleItem.IsSelected) isHasSelected = true;

                        var itemInfo = new FieldItemInfo
                        {
                            FieldId = fieldInfo.Id,
                            Value = styleItem.Value,
                            IsSelected = styleItem.IsSelected
                        };
                        fieldInfo.Items.Add(itemInfo);
                    }
                }
            }

            FieldManager.Repository.Update(fieldInfo, true);
            
            return true;
        }
    }
}
