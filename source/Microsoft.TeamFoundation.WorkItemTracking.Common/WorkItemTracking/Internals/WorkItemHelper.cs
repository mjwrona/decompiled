// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Internals.WorkItemHelper
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E674B254-26A0-4D88-8FF3-86800090789C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Common.dll

using Microsoft.TeamFoundation.WorkItemTracking.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Xml;

namespace Microsoft.TeamFoundation.WorkItemTracking.Internals
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class WorkItemHelper
  {
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static bool LoadWorkItemFieldData(
      IRowSetCollectionHelper2 tables,
      IWorkItemOpenFieldDataHelper helper)
    {
      return WorkItemHelper.LoadWorkItemFieldData((IRowSetCollectionHelper) tables, helper);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static bool LoadWorkItemFieldData(
      IRowSetCollectionHelper tables,
      IWorkItemOpenFieldDataHelper helper)
    {
      TeamFoundationTrace.Verbose("--> LoadWorkItemFieldData()");
      IRowSetHelper table1 = tables["WorkItemInfo"];
      TeamFoundationTrace.Verbose("Latest revision count: {0}", (object) table1.RowCount);
      if (table1.RowCount == 0)
      {
        TeamFoundationTrace.Verbose("<-- LoadWorkItemFieldData()");
        return false;
      }
      if (table1.RowCount != 1)
        throw new ApplicationException(InternalsResourceStrings.Get("UnrecognizedServerData"));
      TeamFoundationTrace.Verbose("Loading latest revision");
      Dictionary<int, object> latestData = WorkItemHelper.ReadTable(table1, helper, true).First<Dictionary<int, object>>();
      List<Dictionary<int, object>> dictionaryList = WorkItemHelper.LoadRevisions(tables, helper, latestData);
      Dictionary<int, object> dictionary1 = new Dictionary<int, object>((IDictionary<int, object>) latestData);
      for (int index = dictionaryList.Count - 1; index >= 0; --index)
      {
        Dictionary<int, object> dictionary2 = dictionaryList[index];
        HashSet<int> intSet = new HashSet<int>();
        foreach (int key in dictionary2.Keys)
        {
          object objA = dictionary2[key];
          object objB;
          if (dictionary1.TryGetValue(key, out objB) && object.Equals(objA, objB))
          {
            intSet.Add(key);
            objA = (object) null;
          }
          dictionary1[key] = objA;
        }
        foreach (int key in intSet)
        {
          if (key != -42)
            dictionary2[key] = (object) null;
        }
      }
      TeamFoundationTrace.Verbose("Loading long texts");
      IRowSetHelper table2 = tables["Texts"];
      int num = 0;
      int[] array = new int[table2.RowCount];
      for (int index = 0; index < array.Length; ++index)
        array[index] = index;
      Array.Sort<int>(array, (IComparer<int>) new WorkItemHelper.LongTextComparer(table2, "AddedDate"));
      int trackTimeFieldId = !helper.HasField(3) ? -4 : 3;
      for (int index = 0; index < array.Length; ++index)
      {
        int row = array[index];
        int key = (int) table2[row, "FldID"];
        string str = (string) table2[row, "Words"];
        DateTime dt = (DateTime) table2[row, "AddedDate"];
        if (str == string.Empty)
          str = (string) null;
        num = WorkItemHelper.FindRevisionIndexByDate(trackTimeFieldId, dictionaryList, latestData, dt, num);
        if (key == 54)
        {
          if (num >= 0 && num < dictionaryList.Count)
            dictionaryList[num][key] = (object) str;
          else
            latestData[key] = (object) str;
        }
        else
        {
          if (num > 0)
          {
            object obj = (object) null;
            latestData.TryGetValue(key, out obj);
            dictionaryList[num - 1][key] = obj;
          }
          latestData[key] = (object) str;
        }
      }
      helper.SetLatestData(latestData);
      helper.SetRevisionData(dictionaryList);
      TeamFoundationTrace.Verbose("<-- LoadWorkItemFieldData()");
      return true;
    }

    private static List<Dictionary<int, object>> LoadRevisions(
      IRowSetCollectionHelper tables,
      IWorkItemOpenFieldDataHelper helper,
      Dictionary<int, object> latestData)
    {
      TeamFoundationTrace.Verbose("Loading revisions");
      int rev = (int) latestData[8];
      IRowSetHelper rowset = (IRowSetHelper) null;
      IRowSetCollectionHelper2 collectionHelper2 = tables as IRowSetCollectionHelper2;
      bool flag;
      if (collectionHelper2 != null)
      {
        flag = collectionHelper2.TryGetRowSet("Revisions", out rowset);
      }
      else
      {
        rowset = tables["Revisions"];
        flag = true;
      }
      return !flag ? new List<Dictionary<int, object>>() : WorkItemHelper.ReadTable(rowset, helper, false).Where<Dictionary<int, object>>((Func<Dictionary<int, object>, bool>) (dict => (int) dict[8] != rev)).OrderBy<Dictionary<int, object>, int>((Func<Dictionary<int, object>, int>) (dict => (int) dict[8])).ToList<Dictionary<int, object>>();
    }

    private static IEnumerable<Dictionary<int, object>> ReadTable(
      IRowSetHelper rowset,
      IWorkItemOpenFieldDataHelper helper,
      bool isLatest)
    {
      int row = 0;
      for (int rowCount = rowset.RowCount; row < rowCount; ++row)
        yield return WorkItemHelper.ReadRow(rowset, helper, isLatest, row);
    }

    private static Dictionary<int, object> ReadRow(
      IRowSetHelper rowset,
      IWorkItemOpenFieldDataHelper helper,
      bool isLatest,
      int row)
    {
      Dictionary<int, object> dictionary = new Dictionary<int, object>();
      for (int column = 0; column < rowset.ColumnCount; ++column)
      {
        string fieldName = rowset[column];
        object obj = rowset[row, column];
        if (obj is DBNull)
          obj = (object) null;
        int fieldId = helper.GetFieldId(fieldName);
        if ((isLatest || obj != null) && fieldId != 0)
          dictionary[fieldId] = obj;
      }
      return dictionary;
    }

    private static int FindRevisionIndexByDate(
      int trackTimeFieldId,
      List<Dictionary<int, object>> revisions,
      Dictionary<int, object> latestData,
      DateTime dt,
      int startIndex)
    {
      for (int index = startIndex; index < revisions.Count; ++index)
      {
        if (dt <= (DateTime) revisions[index][trackTimeFieldId])
          return index;
      }
      return dt <= (DateTime) latestData[trackTimeFieldId] ? revisions.Count : -1;
    }

    [Obsolete("Do not use this method. This is deprecated.")]
    public static XmlElement GenerateWorkItemUpdatePackage(
      XmlDocument xdoc,
      int id,
      int rev,
      int tempId,
      IWorkItemSaveFieldDataHelper fHelper,
      IWorkItemSaveLinkDataHelper lHelper,
      bool mergeLinks,
      out List<XmlElement> xlinks,
      IEnumerable<Guid> addedTags,
      IEnumerable<Guid> removedTags)
    {
      return WorkItemHelper.GenerateWorkItemUpdatePackage(xdoc, id, rev, tempId, fHelper, lHelper, mergeLinks, out xlinks, (IEnumerable<Guid>) null, (IEnumerable<Guid>) null, (ClientCapabilities) 0);
    }

    [Obsolete("Do not use this method. This is deprecated.")]
    public static XmlElement GenerateWorkItemUpdatePackage(
      XmlDocument xdoc,
      int id,
      int rev,
      int tempId,
      IWorkItemSaveFieldDataHelper fHelper,
      IWorkItemSaveLinkDataHelper lHelper,
      bool mergeLinks,
      out List<XmlElement> xlinks,
      IEnumerable<Guid> addedTags,
      IEnumerable<Guid> removedTags,
      ClientCapabilities capabilities)
    {
      TeamFoundationTrace.Verbose("--> GenerateWorkItemUpdatePackage()");
      TeamFoundationTrace.Verbose("Id: {0}, Rev: {1}, TempId: {2}", (object) id.ToString((IFormatProvider) CultureInfo.InvariantCulture), (object) rev.ToString((IFormatProvider) CultureInfo.InvariantCulture), (object) tempId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      bool flag1 = id <= 0;
      XmlElement element1 = xdoc.CreateElement(flag1 ? "InsertWorkItem" : "UpdateWorkItem");
      element1.SetAttribute("ObjectType", "WorkItem");
      element1.SetAttribute("ClientCapabilities", XmlConvert.ToString((int) capabilities));
      if (flag1)
      {
        element1.SetAttribute("TempID", XmlConvert.ToString(-tempId));
      }
      else
      {
        element1.SetAttribute("WorkItemID", XmlConvert.ToString(id));
        element1.SetAttribute("Revision", XmlConvert.ToString(rev));
      }
      bool flag2 = false;
      XmlElement element2 = xdoc.CreateElement("Columns");
      XmlElement element3 = xdoc.CreateElement("ComputedColumns");
      WorkItemHelper.AddColumn(element3, "System.PersonId", (string) null, (string) null);
      WorkItemHelper.AddColumn(element3, "System.RevisedDate", (string) null, (string) null);
      WorkItemHelper.AddColumn(element3, "System.ChangedDate", (string) null, (string) null);
      if (fHelper.HasField(3))
      {
        WorkItemHelper.AddColumn(element3, "System.AuthorizedDate", (string) null, (string) null);
        WorkItemHelper.AddColumn(element3, "System.Watermark", (string) null, (string) null);
      }
      TeamFoundationTrace.Verbose("Adding updated fields");
      Dictionary<int, object> fieldUpdates = fHelper.FieldUpdates;
      if (fieldUpdates != null)
      {
        foreach (KeyValuePair<int, object> keyValuePair in fieldUpdates)
        {
          int key = keyValuePair.Key;
          if (fHelper.HasField(key))
          {
            string fieldReferenceName = fHelper.GetFieldReferenceName(key);
            if (key == 9)
              flag2 = true;
            if (keyValuePair.Value is ServerDefaultFieldValue)
            {
              switch ((keyValuePair.Value as ServerDefaultFieldValue).Type)
              {
                case ServerDefaultType.ServerDateTime:
                  bool flag3 = fHelper.HasField(3);
                  if (flag3 && key != 3 || !flag3 && key != -4)
                  {
                    WorkItemHelper.AddColumn(element2, fieldReferenceName, "ServerDateTime", (string) null);
                    WorkItemHelper.AddColumn(element3, fieldReferenceName, (string) null, (string) null);
                    continue;
                  }
                  continue;
                case ServerDefaultType.CallerIdentity:
                  if (key != -6)
                  {
                    WorkItemHelper.AddColumn(element2, fieldReferenceName, "String", fHelper.UserDisplayName);
                    WorkItemHelper.AddColumn(element3, fieldReferenceName, (string) null, (string) null);
                    continue;
                  }
                  continue;
                case ServerDefaultType.RandomGuid:
                  WorkItemHelper.AddColumn(element2, fieldReferenceName, "ServerRandomGuid", (string) null);
                  WorkItemHelper.AddColumn(element3, fieldReferenceName, (string) null, (string) null);
                  continue;
                default:
                  throw new Exception("Unexpected server default value type");
              }
            }
            else
            {
              string packageType;
              string xml = WorkItemHelper.ConvertToXml(keyValuePair.Value, out packageType);
              if (fHelper.IsLongTextField(key))
              {
                XmlElement element4 = xdoc.CreateElement("InsertText");
                element1.AppendChild((XmlNode) element4);
                element4.SetAttribute("FieldName", fieldReferenceName);
                element4.SetAttribute("FieldDisplayName", fHelper.GetFieldName(key));
                element4.AppendChild((XmlNode) xdoc.CreateTextNode(xml));
              }
              else
                WorkItemHelper.AddColumn(element2, fieldReferenceName, packageType, xml);
            }
          }
        }
      }
      xlinks = new List<XmlElement>();
      if (lHelper != null)
      {
        lHelper.ResetWorkItemLinkInfo();
        TeamFoundationTrace.Verbose("Adding deleted links");
        IEnumerable<LinkInfo> deletedLinks = lHelper.DeletedLinks;
        if (deletedLinks != null)
        {
          foreach (LinkInfo linkInfo in deletedLinks)
          {
            if (linkInfo.FieldId == 50)
            {
              AttachmentInfo attachmentInfo = (AttachmentInfo) linkInfo;
              XmlElement element5 = xdoc.CreateElement("RemoveFile");
              element5.SetAttribute("FileID", XmlConvert.ToString(attachmentInfo.ExtId));
              element1.AppendChild((XmlNode) element5);
            }
            else if (linkInfo.FieldId == 58 || linkInfo.FieldId == 51)
            {
              FileLinkInfo fileLinkInfo = (FileLinkInfo) linkInfo;
              XmlElement element6 = xdoc.CreateElement("RemoveResourceLink");
              element6.SetAttribute("LinkID", XmlConvert.ToString(fileLinkInfo.ExtId));
              element1.AppendChild((XmlNode) element6);
            }
            else if (linkInfo.FieldId == 37)
            {
              WorkItemLinkInfo link = (WorkItemLinkInfo) linkInfo;
              if (link.LinkType == 0)
              {
                XmlElement element7 = xdoc.CreateElement("RemoveRelation");
                element7.SetAttribute("WorkItemID", XmlConvert.ToString(link.TargetId));
                element1.AppendChild((XmlNode) element7);
              }
              else
              {
                XmlElement element8 = xdoc.CreateElement("DeleteWorkItemLink");
                element8.SetAttribute("SourceID", XmlConvert.ToString(link.SourceId));
                element8.SetAttribute("TargetID", XmlConvert.ToString(link.TargetId));
                element8.SetAttribute("LinkType", XmlConvert.ToString(link.LinkType));
                element8.SetAttribute("AutoMerge", XmlConvert.ToString(mergeLinks));
                xlinks.Add(element8);
                lHelper.AddWorkItemLinkInfo(link);
              }
            }
          }
        }
        TeamFoundationTrace.Verbose("Adding added links");
        IEnumerable<LinkInfo> addedLinks = lHelper.AddedLinks;
        if (addedLinks != null)
        {
          foreach (LinkInfo linkInfo in addedLinks)
          {
            XmlElement newChild = (XmlElement) null;
            if (linkInfo.FieldId == 50)
            {
              AttachmentInfo attachmentInfo = (AttachmentInfo) linkInfo;
              newChild = xdoc.CreateElement("InsertFile");
              newChild.SetAttribute("FieldName", "System.AttachedFiles");
              newChild.SetAttribute("OriginalName", attachmentInfo.Attribute);
              newChild.SetAttribute("FileName", attachmentInfo.Path);
              newChild.SetAttribute("CreationDate", XmlConvert.ToString(attachmentInfo.CreationDate, XmlDateTimeSerializationMode.Utc));
              newChild.SetAttribute("LastWriteDate", XmlConvert.ToString(attachmentInfo.LastWriteDate, XmlDateTimeSerializationMode.Utc));
              newChild.SetAttribute("FileSize", XmlConvert.ToString(attachmentInfo.Length));
              element1.AppendChild((XmlNode) newChild);
            }
            else if (linkInfo.FieldId == 51)
            {
              FileLinkInfo fileLinkInfo = (FileLinkInfo) linkInfo;
              newChild = xdoc.CreateElement("InsertResourceLink");
              newChild.SetAttribute("FieldName", "System.LinkedFiles");
              newChild.SetAttribute("DisplayName", fileLinkInfo.Attribute);
              newChild.SetAttribute("Location", fileLinkInfo.Path);
              element1.AppendChild((XmlNode) newChild);
            }
            else if (linkInfo.FieldId == 58)
            {
              FileLinkInfo fileLinkInfo = (FileLinkInfo) linkInfo;
              newChild = xdoc.CreateElement("InsertResourceLink");
              newChild.SetAttribute("FieldName", "System.BISLinks");
              newChild.SetAttribute("LinkType", fileLinkInfo.Attribute);
              newChild.SetAttribute("Location", fileLinkInfo.Path);
              element1.AppendChild((XmlNode) newChild);
            }
            else if (linkInfo.FieldId == 37)
            {
              WorkItemLinkInfo link = (WorkItemLinkInfo) linkInfo;
              if (link.LinkType == 0)
              {
                newChild = xdoc.CreateElement("CreateRelation");
                newChild.SetAttribute("WorkItemID", XmlConvert.ToString(link.TargetId));
                element1.AppendChild((XmlNode) newChild);
              }
              else
              {
                newChild = xdoc.CreateElement("InsertWorkItemLink");
                newChild.SetAttribute("SourceID", XmlConvert.ToString(link.SourceId));
                newChild.SetAttribute("TargetID", XmlConvert.ToString(link.TargetId));
                newChild.SetAttribute("LinkType", XmlConvert.ToString(link.LinkType));
                newChild.SetAttribute("Lock", XmlConvert.ToString(link.IsLocked));
                if (mergeLinks)
                  newChild.SetAttribute("AutoMerge", XmlConvert.ToString(mergeLinks));
                xlinks.Add(newChild);
                lHelper.AddWorkItemLinkInfo(link);
              }
            }
            if (newChild != null && !string.IsNullOrEmpty(linkInfo.Comment))
              newChild.SetAttribute("Comment", linkInfo.Comment);
          }
        }
        TeamFoundationTrace.Verbose("Adding updated links");
        IEnumerable<KeyValuePair<LinkInfo, LinkUpdate>> updatedLinks = lHelper.UpdatedLinks;
        if (updatedLinks != null)
        {
          foreach (KeyValuePair<LinkInfo, LinkUpdate> keyValuePair in updatedLinks)
          {
            LinkInfo key = keyValuePair.Key;
            XmlElement xmlElement = (XmlElement) null;
            if (key.FieldId == 50)
            {
              AttachmentInfo attachmentInfo = (AttachmentInfo) key;
              xmlElement = xdoc.CreateElement("UpdateResourceLink");
              xmlElement.SetAttribute("FileID", XmlConvert.ToString(attachmentInfo.ExtId));
              element1.AppendChild((XmlNode) xmlElement);
            }
            else if (key.FieldId == 58 || key.FieldId == 51)
            {
              FileLinkInfo fileLinkInfo = (FileLinkInfo) key;
              xmlElement = xdoc.CreateElement("UpdateResourceLink");
              xmlElement.SetAttribute("LinkID", XmlConvert.ToString(fileLinkInfo.ExtId));
              element1.AppendChild((XmlNode) xmlElement);
            }
            else if (key.FieldId == 37)
            {
              WorkItemLinkInfo link = (WorkItemLinkInfo) key;
              if (link.LinkType == 0)
              {
                xmlElement = xdoc.CreateElement("UpdateRelation");
                xmlElement.SetAttribute("WorkItemID", XmlConvert.ToString(link.TargetId));
                element1.AppendChild((XmlNode) xmlElement);
              }
              else
              {
                xmlElement = xdoc.CreateElement("UpdateWorkItemLink");
                xmlElement.SetAttribute("SourceID", XmlConvert.ToString(link.SourceId));
                xmlElement.SetAttribute("TargetID", XmlConvert.ToString(link.TargetId));
                xmlElement.SetAttribute("LinkType", XmlConvert.ToString(link.LinkType));
                xmlElement.SetAttribute("Lock", XmlConvert.ToString(link.IsLocked));
                xlinks.Add(xmlElement);
                lHelper.AddWorkItemLinkInfo(link);
              }
            }
            if (xmlElement != null)
              keyValuePair.Value.Submit(xmlElement);
          }
        }
      }
      if (addedTags != null && addedTags.Any<Guid>())
      {
        XmlElement element9 = xdoc.CreateElement("AddedTags");
        WorkItemHelper.AddTags(element9, addedTags);
        element1.AppendChild((XmlNode) element9);
      }
      if (removedTags != null && removedTags.Any<Guid>())
      {
        XmlElement element10 = xdoc.CreateElement("RemovedTags");
        WorkItemHelper.AddTags(element10, removedTags);
        element1.AppendChild((XmlNode) element10);
      }
      if (((element1.ChildNodes.Count > 0 ? 1 : (fHelper.IsDirty ? 1 : 0)) | (flag1 ? 1 : 0)) != 0)
      {
        element1.AppendChild((XmlNode) element2);
        element1.AppendChild((XmlNode) element3);
        if (!flag2)
          WorkItemHelper.AddColumn(element2, "System.ChangedBy", "String", fHelper.UserDisplayName);
        TeamFoundationTrace.Verbose("<-- GenerateWorkItemUpdatePackage()");
        TeamFoundationTrace.Verbose("UpdateElement: {0}", (object) element1.OuterXml);
        return element1;
      }
      TeamFoundationTrace.Verbose("<-- GenerateWorkItemUpdatePackage()");
      return (XmlElement) null;
    }

    private static void AddTags(XmlElement element, IEnumerable<Guid> tags)
    {
      XmlDocument ownerDocument = element.OwnerDocument;
      foreach (Guid tag in tags)
      {
        XmlElement element1 = ownerDocument.CreateElement("Tag");
        element1.SetAttribute("Id", tag.ToString());
        element.AppendChild((XmlNode) element1);
      }
    }

    public static XmlElement GenerateWorkItemUpdatePackage(
      XmlDocument xdoc,
      int id,
      int rev,
      int tempId,
      IWorkItemSaveFieldDataHelper fHelper,
      IWorkItemSaveLinkDataHelper lHelper,
      bool mergeLinks,
      out List<XmlElement> xlinks)
    {
      return WorkItemHelper.GenerateWorkItemUpdatePackage(xdoc, id, rev, tempId, fHelper, lHelper, mergeLinks, out xlinks, (ClientCapabilities) 0);
    }

    public static XmlElement GenerateWorkItemUpdatePackage(
      XmlDocument xdoc,
      int id,
      int rev,
      int tempId,
      IWorkItemSaveFieldDataHelper fHelper,
      IWorkItemSaveLinkDataHelper lHelper,
      bool mergeLinks,
      out List<XmlElement> xlinks,
      ClientCapabilities capabilities)
    {
      TeamFoundationTrace.Verbose("--> GenerateWorkItemUpdatePackage()");
      TeamFoundationTrace.Verbose("Id: {0}, Rev: {1}, TempId: {2}", (object) id.ToString((IFormatProvider) CultureInfo.InvariantCulture), (object) rev.ToString((IFormatProvider) CultureInfo.InvariantCulture), (object) tempId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      bool flag1 = id <= 0;
      XmlElement element1 = xdoc.CreateElement(flag1 ? "InsertWorkItem" : "UpdateWorkItem");
      element1.SetAttribute("ObjectType", "WorkItem");
      element1.SetAttribute("ClientCapabilities", XmlConvert.ToString((int) capabilities));
      if (flag1)
      {
        element1.SetAttribute("TempID", XmlConvert.ToString(-tempId));
      }
      else
      {
        element1.SetAttribute("WorkItemID", XmlConvert.ToString(id));
        element1.SetAttribute("Revision", XmlConvert.ToString(rev));
      }
      bool flag2 = false;
      XmlElement element2 = xdoc.CreateElement("Columns");
      XmlElement element3 = xdoc.CreateElement("ComputedColumns");
      WorkItemHelper.AddColumn(element3, "System.PersonId", (string) null, (string) null);
      WorkItemHelper.AddColumn(element3, "System.RevisedDate", (string) null, (string) null);
      WorkItemHelper.AddColumn(element3, "System.ChangedDate", (string) null, (string) null);
      if (fHelper.HasField(3))
      {
        WorkItemHelper.AddColumn(element3, "System.AuthorizedDate", (string) null, (string) null);
        WorkItemHelper.AddColumn(element3, "System.Watermark", (string) null, (string) null);
      }
      TeamFoundationTrace.Verbose("Adding updated fields");
      Dictionary<int, object> fieldUpdates = fHelper.FieldUpdates;
      if (fieldUpdates != null)
      {
        foreach (KeyValuePair<int, object> keyValuePair in fieldUpdates)
        {
          int key = keyValuePair.Key;
          if (fHelper.HasField(key))
          {
            string fieldReferenceName = fHelper.GetFieldReferenceName(key);
            if (key == 9)
              flag2 = true;
            if (keyValuePair.Value is ServerDefaultFieldValue)
            {
              switch ((keyValuePair.Value as ServerDefaultFieldValue).Type)
              {
                case ServerDefaultType.ServerDateTime:
                  bool flag3 = fHelper.HasField(3);
                  if (flag3 && key != 3 || !flag3 && key != -4)
                  {
                    WorkItemHelper.AddColumn(element2, fieldReferenceName, "ServerDateTime", (string) null);
                    WorkItemHelper.AddColumn(element3, fieldReferenceName, (string) null, (string) null);
                    continue;
                  }
                  continue;
                case ServerDefaultType.CallerIdentity:
                  if (key != -6)
                  {
                    WorkItemHelper.AddColumn(element2, fieldReferenceName, "String", fHelper.UserDisplayName);
                    WorkItemHelper.AddColumn(element3, fieldReferenceName, (string) null, (string) null);
                    continue;
                  }
                  continue;
                case ServerDefaultType.RandomGuid:
                  WorkItemHelper.AddColumn(element2, fieldReferenceName, "ServerRandomGuid", (string) null);
                  WorkItemHelper.AddColumn(element3, fieldReferenceName, (string) null, (string) null);
                  continue;
                default:
                  throw new Exception("Unexpected server default value type");
              }
            }
            else
            {
              string packageType;
              string xml = WorkItemHelper.ConvertToXml(keyValuePair.Value, out packageType);
              if (fHelper.IsLongTextField(key))
              {
                XmlElement element4 = xdoc.CreateElement("InsertText");
                element1.AppendChild((XmlNode) element4);
                element4.SetAttribute("FieldName", fieldReferenceName);
                element4.SetAttribute("FieldDisplayName", fHelper.GetFieldName(key));
                element4.AppendChild((XmlNode) xdoc.CreateTextNode(xml));
              }
              else
                WorkItemHelper.AddColumn(element2, fieldReferenceName, packageType, xml);
            }
          }
        }
      }
      xlinks = new List<XmlElement>();
      if (lHelper != null)
      {
        lHelper.ResetWorkItemLinkInfo();
        TeamFoundationTrace.Verbose("Adding deleted links");
        IEnumerable<LinkInfo> deletedLinks = lHelper.DeletedLinks;
        if (deletedLinks != null)
        {
          foreach (LinkInfo linkInfo in deletedLinks)
          {
            if (linkInfo.FieldId == 50)
            {
              FileLinkInfo fileLinkInfo = (FileLinkInfo) linkInfo;
              XmlElement element5 = xdoc.CreateElement("RemoveFile");
              element5.SetAttribute("FileID", XmlConvert.ToString(fileLinkInfo.ExtId));
              element1.AppendChild((XmlNode) element5);
            }
            else if (linkInfo.FieldId == 58 || linkInfo.FieldId == 51)
            {
              FileLinkInfo fileLinkInfo = (FileLinkInfo) linkInfo;
              XmlElement element6 = xdoc.CreateElement("RemoveResourceLink");
              element6.SetAttribute("LinkID", XmlConvert.ToString(fileLinkInfo.ExtId));
              element1.AppendChild((XmlNode) element6);
            }
            else if (linkInfo.FieldId == 37)
            {
              WorkItemLinkInfo link = (WorkItemLinkInfo) linkInfo;
              if (link.LinkType == 0)
              {
                XmlElement element7 = xdoc.CreateElement("RemoveRelation");
                element7.SetAttribute("WorkItemID", XmlConvert.ToString(link.TargetId));
                element1.AppendChild((XmlNode) element7);
              }
              else
              {
                XmlElement element8 = xdoc.CreateElement("DeleteWorkItemLink");
                element8.SetAttribute("SourceID", XmlConvert.ToString(link.SourceId));
                element8.SetAttribute("TargetID", XmlConvert.ToString(link.TargetId));
                element8.SetAttribute("LinkType", XmlConvert.ToString(link.LinkType));
                element8.SetAttribute("AutoMerge", XmlConvert.ToString(mergeLinks));
                xlinks.Add(element8);
                lHelper.AddWorkItemLinkInfo(link);
              }
            }
          }
        }
        TeamFoundationTrace.Verbose("Adding added links");
        IEnumerable<LinkInfo> addedLinks = lHelper.AddedLinks;
        if (addedLinks != null)
        {
          foreach (LinkInfo linkInfo in addedLinks)
          {
            XmlElement newChild = (XmlElement) null;
            if (linkInfo.FieldId == 50)
            {
              AttachmentInfo attachmentInfo = (AttachmentInfo) linkInfo;
              newChild = xdoc.CreateElement("InsertFile");
              newChild.SetAttribute("FieldName", "System.AttachedFiles");
              newChild.SetAttribute("OriginalName", attachmentInfo.Attribute);
              newChild.SetAttribute("FileName", attachmentInfo.Path);
              newChild.SetAttribute("CreationDate", XmlConvert.ToString(attachmentInfo.CreationDate, XmlDateTimeSerializationMode.Utc));
              newChild.SetAttribute("LastWriteDate", XmlConvert.ToString(attachmentInfo.LastWriteDate, XmlDateTimeSerializationMode.Utc));
              newChild.SetAttribute("FileSize", XmlConvert.ToString(attachmentInfo.Length));
              element1.AppendChild((XmlNode) newChild);
            }
            else if (linkInfo.FieldId == 51)
            {
              FileLinkInfo fileLinkInfo = (FileLinkInfo) linkInfo;
              newChild = xdoc.CreateElement("InsertResourceLink");
              newChild.SetAttribute("FieldName", "System.LinkedFiles");
              newChild.SetAttribute("DisplayName", fileLinkInfo.Attribute);
              newChild.SetAttribute("Location", fileLinkInfo.Path);
              element1.AppendChild((XmlNode) newChild);
            }
            else if (linkInfo.FieldId == 58)
            {
              FileLinkInfo fileLinkInfo = (FileLinkInfo) linkInfo;
              newChild = xdoc.CreateElement("InsertResourceLink");
              newChild.SetAttribute("FieldName", "System.BISLinks");
              newChild.SetAttribute("LinkType", fileLinkInfo.Attribute);
              newChild.SetAttribute("Location", fileLinkInfo.Path);
              element1.AppendChild((XmlNode) newChild);
            }
            else if (linkInfo.FieldId == 37)
            {
              WorkItemLinkInfo link = (WorkItemLinkInfo) linkInfo;
              if (link.LinkType == 0)
              {
                newChild = xdoc.CreateElement("CreateRelation");
                newChild.SetAttribute("WorkItemID", XmlConvert.ToString(link.TargetId));
                element1.AppendChild((XmlNode) newChild);
              }
              else
              {
                newChild = xdoc.CreateElement("InsertWorkItemLink");
                newChild.SetAttribute("SourceID", XmlConvert.ToString(link.SourceId));
                newChild.SetAttribute("TargetID", XmlConvert.ToString(link.TargetId));
                newChild.SetAttribute("LinkType", XmlConvert.ToString(link.LinkType));
                newChild.SetAttribute("Lock", XmlConvert.ToString(link.IsLocked));
                if (mergeLinks)
                  newChild.SetAttribute("AutoMerge", XmlConvert.ToString(mergeLinks));
                xlinks.Add(newChild);
                lHelper.AddWorkItemLinkInfo(link);
              }
            }
            if (newChild != null && !string.IsNullOrEmpty(linkInfo.Comment))
              newChild.SetAttribute("Comment", linkInfo.Comment);
          }
        }
        TeamFoundationTrace.Verbose("Adding updated links");
        IEnumerable<KeyValuePair<LinkInfo, LinkUpdate>> updatedLinks = lHelper.UpdatedLinks;
        if (updatedLinks != null)
        {
          foreach (KeyValuePair<LinkInfo, LinkUpdate> keyValuePair in updatedLinks)
          {
            LinkInfo key = keyValuePair.Key;
            XmlElement xmlElement = (XmlElement) null;
            if (key.FieldId == 50)
            {
              AttachmentInfo attachmentInfo = (AttachmentInfo) key;
              xmlElement = xdoc.CreateElement("UpdateResourceLink");
              xmlElement.SetAttribute("FileID", XmlConvert.ToString(attachmentInfo.ExtId));
              element1.AppendChild((XmlNode) xmlElement);
            }
            else if (key.FieldId == 58 || key.FieldId == 51)
            {
              FileLinkInfo fileLinkInfo = (FileLinkInfo) key;
              xmlElement = xdoc.CreateElement("UpdateResourceLink");
              xmlElement.SetAttribute("LinkID", XmlConvert.ToString(fileLinkInfo.ExtId));
              element1.AppendChild((XmlNode) xmlElement);
            }
            else if (key.FieldId == 37)
            {
              WorkItemLinkInfo link = (WorkItemLinkInfo) key;
              if (link.LinkType == 0)
              {
                xmlElement = xdoc.CreateElement("UpdateRelation");
                xmlElement.SetAttribute("WorkItemID", XmlConvert.ToString(link.TargetId));
                element1.AppendChild((XmlNode) xmlElement);
              }
              else
              {
                xmlElement = xdoc.CreateElement("UpdateWorkItemLink");
                xmlElement.SetAttribute("SourceID", XmlConvert.ToString(link.SourceId));
                xmlElement.SetAttribute("TargetID", XmlConvert.ToString(link.TargetId));
                xmlElement.SetAttribute("LinkType", XmlConvert.ToString(link.LinkType));
                xmlElement.SetAttribute("Lock", XmlConvert.ToString(link.IsLocked));
                xlinks.Add(xmlElement);
                lHelper.AddWorkItemLinkInfo(link);
              }
            }
            if (xmlElement != null)
              keyValuePair.Value.Submit(xmlElement);
          }
        }
      }
      if (((element1.ChildNodes.Count > 0 ? 1 : (fHelper.IsDirty ? 1 : 0)) | (flag1 ? 1 : 0)) != 0)
      {
        element1.AppendChild((XmlNode) element2);
        element1.AppendChild((XmlNode) element3);
        if (!flag2)
          WorkItemHelper.AddColumn(element2, "System.ChangedBy", "String", fHelper.UserDisplayName);
        TeamFoundationTrace.Verbose("<-- GenerateWorkItemUpdatePackage()");
        TeamFoundationTrace.Verbose("UpdateElement: {0}", (object) element1.OuterXml);
        return element1;
      }
      TeamFoundationTrace.Verbose("<-- GenerateWorkItemUpdatePackage()");
      return (XmlElement) null;
    }

    private static string ConvertToXml(object value, out string packageType)
    {
      packageType = "String";
      switch (value)
      {
        case null:
          return string.Empty;
        case DateTime dateTime:
          packageType = "DateTime";
          return XmlConvert.ToString(dateTime, XmlDateTimeSerializationMode.Unspecified);
        case int num1:
          packageType = "Int32";
          return XmlConvert.ToString(num1);
        case double num2:
          packageType = "Double";
          return XmlConvert.ToString(num2);
        case string _:
          return (string) value;
        case Guid guid:
          packageType = "Guid";
          return guid.ToString("D");
        case bool flag:
          packageType = "Boolean";
          return XmlConvert.ToString(flag ? 1 : 0);
        default:
          throw new Exception("UnExpected value");
      }
    }

    private static void AddColumn(XmlElement element, string colName, string type, string value)
    {
      XmlDocument ownerDocument = element.OwnerDocument;
      string name = type == null ? "ComputedColumn" : "Column";
      XmlElement element1 = ownerDocument.CreateElement(name);
      element1.SetAttribute("Column", colName);
      if (type != null)
      {
        element.AppendChild((XmlNode) element1);
        element1.SetAttribute("Type", type);
        if (value == null)
          return;
        XmlElement element2 = ownerDocument.CreateElement("Value");
        element2.AppendChild((XmlNode) ownerDocument.CreateTextNode(value));
        element1.AppendChild((XmlNode) element2);
      }
      else
      {
        string xpath = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}[@{1}=\"{2}\"]", (object) "ComputedColumn", (object) "Column", (object) colName);
        if (element.SelectSingleNode(xpath) != null)
          return;
        element.AppendChild((XmlNode) element1);
      }
    }

    private static class WorkItemRowSetNames
    {
      public const string Latest = "WorkItemInfo";
      public const string Revisions = "Revisions";
      public const string Keywords = "Keywords";
      public const string Texts = "Texts";
      public const string Files = "Files";
      public const string Relations = "Relations";
      public const string RelationRevisions = "RelationRevisions";
    }

    private class LongTextComparer : IComparer<int>
    {
      private IRowSetHelper m_rowset;
      private string m_column;
      private Comparer m_comparer;

      public LongTextComparer(IRowSetHelper rowset, string column)
      {
        this.m_rowset = rowset;
        this.m_column = column;
        this.m_comparer = new Comparer(CultureInfo.InvariantCulture);
      }

      public int Compare(int x, int y) => this.m_comparer.Compare(this.m_rowset[x, this.m_column], this.m_rowset[y, this.m_column]);
    }
  }
}
