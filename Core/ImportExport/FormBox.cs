using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Runtime.Remoting.Contexts;
using Atom.Core;
using SS.Form.Core.Model;
using SS.Form.Core.Utils;

namespace SS.Form.Core.ImportExport
{
    public static class FormBox
    {
        //private readonly int _siteId;
        //private readonly string _directoryPath;

        //public FormBox(int siteId, string directoryPath)
        //{
        //    _siteId = siteId;
        //    _directoryPath = directoryPath;
        //}

        private const string VersionFileName = "version.txt";

        public static bool IsHistoric(string directoryPath)
        {
            var isHistoric = true;
            if (FormUtils.IsFileExists(FormUtils.PathCombine(directoryPath, VersionFileName)))
            {
                isHistoric = false;
                FormUtils.DeleteFileIfExists(FormUtils.PathCombine(directoryPath, VersionFileName));
            }

            return isHistoric;
        }

        public static void ImportForm(int siteId, string directoryPath, bool overwrite)
        {
            if (!Directory.Exists(directoryPath)) return;
            var isHistoric = IsHistoric(directoryPath);

            var filePaths = Directory.GetFiles(directoryPath);

            foreach (var filePath in filePaths)
            {
                var feed = AtomFeed.Load(new FileStream(filePath, FileMode.Open));

                var formInfo = new FormInfo();

                foreach (var tableColumn in FormManager.Repository.TableColumns)
                {
                    var value = GenericUtils.GetValue(feed.AdditionalElements, tableColumn);
                    formInfo.Set(tableColumn.AttributeName, value);
                }

                formInfo.SiteId = siteId;
                formInfo.AddDate = DateTime.Now;

                if (isHistoric)
                {
                    formInfo.Title = AtomUtility.GetDcElementContent(feed.AdditionalElements, "InputName");
                }

                var srcFormInfo = FormManager.GetFormInfoByTitle(siteId, formInfo.Title);
                if (srcFormInfo != null)
                {
                    if (overwrite)
                    {
                        FormManager.Repository.Delete(siteId, srcFormInfo.Id);
                    }
                    else
                    {
                        formInfo.Title = FormManager.Repository.GetImportTitle(siteId, formInfo.Title);
                    }
                }

                formInfo.Id = FormManager.Repository.Insert(formInfo);

                var directoryName = AtomUtility.GetDcElementContent(feed.AdditionalElements, "Id");
                if (isHistoric)
                {
                    directoryName = AtomUtility.GetDcElementContent(feed.AdditionalElements, "InputID");
                }
                var titleAttributeNameDict = new NameValueCollection();
                if (!string.IsNullOrEmpty(directoryName))
                {
                    var fieldDirectoryPath = FormUtils.PathCombine(directoryPath, directoryName);
                    titleAttributeNameDict = ImportFields(siteId, formInfo.Id, fieldDirectoryPath, isHistoric);
                }

                foreach (AtomEntry entry in feed.Entries)
                {
                    var logInfo = new LogInfo();

                    foreach (var tableColumn in LogManager.Repository.TableColumns)
                    {
                        var value = GenericUtils.GetValue(entry.AdditionalElements, tableColumn);
                        logInfo.Set(tableColumn.AttributeName, value);
                    }

                    var attributes = AtomUtility.GetDcElementNameValueCollection(entry.AdditionalElements);
                    foreach (string entryName in attributes.Keys)
                    {
                        logInfo.Set(entryName, attributes[entryName]);
                    }

                    if (isHistoric)
                    {
                        foreach (var title in titleAttributeNameDict.AllKeys)
                        {
                            logInfo.Set(title, logInfo.Get(titleAttributeNameDict[title]));
                        }

                        logInfo.ReplyContent = AtomUtility.GetDcElementContent(entry.AdditionalElements, "Reply");
                    }

                    LogManager.Repository.Insert(formInfo, logInfo);
                }
            }
        }

        public static NameValueCollection ImportFields(int siteId, int formId, string styleDirectoryPath, bool isHistoric)
        {
            var titleAttributeNameDict = new NameValueCollection();

            if (!Directory.Exists(styleDirectoryPath)) return titleAttributeNameDict;

            var filePaths = Directory.GetFiles(styleDirectoryPath);
            foreach (var filePath in filePaths)
            {
                var feed = AtomFeed.Load(new FileStream(filePath, FileMode.Open));

                var title = AtomUtility.GetDcElementContent(feed.AdditionalElements, nameof(FieldInfo.Title));
                if (isHistoric)
                {
                    var attributeName = AtomUtility.GetDcElementContent(feed.AdditionalElements, "AttributeName");
                    title = AtomUtility.GetDcElementContent(feed.AdditionalElements, "DisplayName");

                    titleAttributeNameDict[title] = attributeName;
                }
                var fieldType = AtomUtility.GetDcElementContent(feed.AdditionalElements, nameof(FieldInfo.FieldType));
                if (isHistoric)
                {
                    fieldType = AtomUtility.GetDcElementContent(feed.AdditionalElements, "InputType");
                }
                var taxis = FormUtils.ToIntWithNegative(AtomUtility.GetDcElementContent(feed.AdditionalElements, "Taxis"), 0);

                var fieldInfo = new FieldInfo
                {
                    FormId = formId,
                    Taxis = taxis,
                    Title = title,
                    FieldType = fieldType
                };

                var fieldItems = new List<FieldItemInfo>();
                foreach (AtomEntry entry in feed.Entries)
                {
                    var itemValue = AtomUtility.GetDcElementContent(entry.AdditionalElements, "ItemValue");
                    var isSelected = FormUtils.ToBool(AtomUtility.GetDcElementContent(entry.AdditionalElements, "IsSelected"), false);

                    fieldItems.Add(new FieldItemInfo
                    {
                        FormId = formId,
                        FieldId = 0,
                        Value = itemValue,
                        IsSelected = isSelected
                    });
                }

                if (fieldItems.Count > 0)
                {
                    fieldInfo.Items = fieldItems;
                }

                if (FieldManager.Repository.IsTitleExists(formId, title))
                {
                    FieldManager.Repository.Delete(formId, title);
                }
                FieldManager.Repository.Insert(siteId, fieldInfo);
            }

            return titleAttributeNameDict;
        }

        public static void ExportForm(int siteId, string directoryPath, int formId)
        {
            var formInfo = FormManager.GetFormInfo(siteId, formId);
            var filePath = FormUtils.PathCombine(directoryPath, formInfo.Id + ".xml");

            var feed = AtomUtility.GetEmptyFeed();

            foreach (var tableColumn in FormManager.Repository.TableColumns)
            {
                GenericUtils.SetValue(feed.AdditionalElements, tableColumn, formInfo);
            }

            var styleDirectoryPath = FormUtils.PathCombine(directoryPath, formInfo.Id.ToString());
            ExportFields(formInfo.Id, styleDirectoryPath);

            var logInfoList = LogManager.Repository.GetLogInfoList(formInfo.Id, false, 0, 0);
            foreach (var logInfo in logInfoList)
            {
                var entry = GenericUtils.GetAtomEntry(logInfo);
                feed.Entries.Add(entry);
            }
            feed.Save(filePath);

            FormUtils.WriteText(FormUtils.PathCombine(directoryPath, VersionFileName), Plugin.PluginVersion);
        }

        private static AtomFeed ExportFieldInfo(FieldInfo fieldInfo)
        {
            var feed = AtomUtility.GetEmptyFeed();

            foreach (var tableColumn in FieldManager.Repository.TableColumns)
            {
                GenericUtils.SetValue(feed.AdditionalElements, tableColumn, fieldInfo);
            }

            return feed;
        }

        public static void ExportFields(int formId, string styleDirectoryPath)
        {

            FormUtils.DeleteDirectoryIfExists(styleDirectoryPath);
            FormUtils.CreateDirectoryIfNotExists(styleDirectoryPath);

            var fieldInfoList = FieldManager.GetFieldInfoList(formId);
            foreach (var fieldInfo in fieldInfoList)
            {
                var filePath = FormUtils.PathCombine(styleDirectoryPath, fieldInfo.Id + ".xml");
                var feed = ExportFieldInfo(fieldInfo);
                if (fieldInfo.Items != null && fieldInfo.Items.Count > 0)
                {
                    foreach (var itemInfo in fieldInfo.Items)
                    {
                        var entry = ExportTableStyleItemInfo(itemInfo);
                        feed.Entries.Add(entry);
                    }
                }
                feed.Save(filePath);
            }
        }

        private static AtomEntry ExportTableStyleItemInfo(FieldItemInfo styleItemInfo)
        {
            var entry = AtomUtility.GetEmptyEntry();

            foreach (var tableColumn in FieldManager.ItemRepository.TableColumns)
            {
                GenericUtils.SetValue(entry.AdditionalElements, tableColumn, styleItemInfo);
            }

            return entry;
        }
    }
}
