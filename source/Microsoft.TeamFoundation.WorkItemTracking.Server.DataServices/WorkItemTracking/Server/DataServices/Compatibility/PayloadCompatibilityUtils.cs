// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.DataServices.Compatibility.PayloadCompatibilityUtils
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataServices, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 929F0284-16B2-4277-9F4A-B615689A77D1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataServices.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Common.Metadata;
using Microsoft.TeamFoundation.WorkItemTracking.Internals;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.TeamFoundation.WorkItemTracking.Server.QueryItems;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.DataServices.Compatibility
{
  public static class PayloadCompatibilityUtils
  {
    private static HashSet<int> s_fieldsToRemove = new HashSet<int>()
    {
      -7,
      -105,
      -1,
      100
    };
    private static readonly string[] ToolsNotSupportedInSOAP = new string[2]
    {
      "ReleaseManagement",
      "Wiki"
    };
    private static readonly KeyValuePair<string, Type>[] c_textTableSchema = new KeyValuePair<string, Type>[4]
    {
      new KeyValuePair<string, Type>("FldID", typeof (int)),
      new KeyValuePair<string, Type>("AddedDate", typeof (DateTime)),
      new KeyValuePair<string, Type>("Words", typeof (string)),
      new KeyValuePair<string, Type>("ExtID", typeof (int))
    };
    private static readonly KeyValuePair<string, Type>[] c_filesTableSchema = new KeyValuePair<string, Type>[12]
    {
      new KeyValuePair<string, Type>("FldID", typeof (int)),
      new KeyValuePair<string, Type>("RemovedDate", typeof (DateTime)),
      new KeyValuePair<string, Type>("AddedDate", typeof (DateTime)),
      new KeyValuePair<string, Type>("FilePath", typeof (string)),
      new KeyValuePair<string, Type>("OriginalName", typeof (string)),
      new KeyValuePair<string, Type>("ExtID", typeof (int)),
      new KeyValuePair<string, Type>("Comment", typeof (string)),
      new KeyValuePair<string, Type>("CreationDate", typeof (DateTime)),
      new KeyValuePair<string, Type>("LastWriteDate", typeof (DateTime)),
      new KeyValuePair<string, Type>("Length", typeof (int)),
      new KeyValuePair<string, Type>("AuthorizedAddedDate", typeof (DateTime)),
      new KeyValuePair<string, Type>("AuthorizedRemovedDate", typeof (DateTime))
    };
    private static KeyValuePair<string, Type>[] c_relationsTableSchema = new KeyValuePair<string, Type>[10]
    {
      new KeyValuePair<string, Type>("ID", typeof (int)),
      new KeyValuePair<string, Type>("Comment", typeof (string)),
      new KeyValuePair<string, Type>("Changed By", typeof (int)),
      new KeyValuePair<string, Type>("Changed Date", typeof (DateTime)),
      new KeyValuePair<string, Type>("Revised By", typeof (int)),
      new KeyValuePair<string, Type>("Revised Date", typeof (DateTime)),
      new KeyValuePair<string, Type>("LinkType", typeof (short)),
      new KeyValuePair<string, Type>("Lock", typeof (bool)),
      new KeyValuePair<string, Type>("AuthorizedAddedDate", typeof (DateTime)),
      new KeyValuePair<string, Type>("AuthorizedRemovedDate", typeof (DateTime))
    };
    private static readonly KeyValuePair<string, Type>[] c_textTableSchemaPaging = new KeyValuePair<string, Type>[4]
    {
      new KeyValuePair<string, Type>("AddedDate", typeof (DateTime)),
      new KeyValuePair<string, Type>("FldID", typeof (int)),
      new KeyValuePair<string, Type>("ID", typeof (int)),
      new KeyValuePair<string, Type>("Words", typeof (string))
    };
    private static readonly KeyValuePair<string, Type>[] c_longTextTableSchema = new KeyValuePair<string, Type>[5]
    {
      new KeyValuePair<string, Type>("ReferenceName", typeof (string)),
      new KeyValuePair<string, Type>("FldID", typeof (int)),
      new KeyValuePair<string, Type>("AddedDate", typeof (DateTime)),
      new KeyValuePair<string, Type>("Words", typeof (string)),
      new KeyValuePair<string, Type>("fHtml", typeof (int))
    };
    private static readonly KeyValuePair<string, Type>[] c_fieldsTableSchema = new KeyValuePair<string, Type>[3]
    {
      new KeyValuePair<string, Type>("ReferenceName", typeof (string)),
      new KeyValuePair<string, Type>("Name", typeof (string)),
      new KeyValuePair<string, Type>("Type", typeof (int))
    };
    private static readonly KeyValuePair<string, Type>[] c_historyTableSchema = new KeyValuePair<string, Type>[5]
    {
      new KeyValuePair<string, Type>("ChangedBy", typeof (string)),
      new KeyValuePair<string, Type>("FldId", typeof (int)),
      new KeyValuePair<string, Type>("AddedDate", typeof (DateTime)),
      new KeyValuePair<string, Type>("Words", typeof (string)),
      new KeyValuePair<string, Type>("fHtml", typeof (int))
    };
    private static readonly KeyValuePair<string, Type>[] c_externalLinksTableSchema = new KeyValuePair<string, Type>[4]
    {
      new KeyValuePair<string, Type>("Uri", typeof (string)),
      new KeyValuePair<string, Type>("Comment", typeof (string)),
      new KeyValuePair<string, Type>("AddedDate", typeof (DateTime)),
      new KeyValuePair<string, Type>("ArtifactName", typeof (string))
    };
    private static readonly KeyValuePair<string, Type>[] c_relatedLinksTableSchema = new KeyValuePair<string, Type>[4]
    {
      new KeyValuePair<string, Type>("ID", typeof (int)),
      new KeyValuePair<string, Type>("RelatedID", typeof (int)),
      new KeyValuePair<string, Type>("Comment", typeof (string)),
      new KeyValuePair<string, Type>("LinkType", typeof (string))
    };
    private static readonly KeyValuePair<string, Type>[] c_hyperLinksTableSchema = new KeyValuePair<string, Type>[3]
    {
      new KeyValuePair<string, Type>("Url", typeof (string)),
      new KeyValuePair<string, Type>("Comment", typeof (string)),
      new KeyValuePair<string, Type>("AddedDate", typeof (DateTime))
    };
    private static readonly KeyValuePair<string, Type>[] c_attachmentsTableSchema = new KeyValuePair<string, Type>[5]
    {
      new KeyValuePair<string, Type>("ID", typeof (int)),
      new KeyValuePair<string, Type>("FileName", typeof (string)),
      new KeyValuePair<string, Type>("AddedDate", typeof (DateTime)),
      new KeyValuePair<string, Type>("Length", typeof (int)),
      new KeyValuePair<string, Type>("Comment", typeof (string))
    };
    private static readonly KeyValuePair<string, Type>[] c_QueryItemsTableSchema = new KeyValuePair<string, Type>[9]
    {
      new KeyValuePair<string, Type>("ID", typeof (Guid)),
      new KeyValuePair<string, Type>("ParentID", typeof (Guid)),
      new KeyValuePair<string, Type>("Name", typeof (string)),
      new KeyValuePair<string, Type>("Text", typeof (string)),
      new KeyValuePair<string, Type>("fFolder", typeof (bool)),
      new KeyValuePair<string, Type>("fDeleted", typeof (bool)),
      new KeyValuePair<string, Type>("Cachestamp", typeof (ulong)),
      new KeyValuePair<string, Type>("OwnerIdentifier", typeof (string)),
      new KeyValuePair<string, Type>("OwnerType", typeof (string))
    };
    private static readonly KeyValuePair<string, Type>[] c_StoredQueryItemsTableSchema = new KeyValuePair<string, Type>[13]
    {
      new KeyValuePair<string, Type>("ID", typeof (Guid)),
      new KeyValuePair<string, Type>("ProjectID", typeof (int)),
      new KeyValuePair<string, Type>("fPublic", typeof (bool)),
      new KeyValuePair<string, Type>("Owner", typeof (string)),
      new KeyValuePair<string, Type>("OwnerType", typeof (string)),
      new KeyValuePair<string, Type>("QueryName", typeof (string)),
      new KeyValuePair<string, Type>("QueryText", typeof (string)),
      new KeyValuePair<string, Type>("Description", typeof (string)),
      new KeyValuePair<string, Type>("CreateTime", typeof (DateTime)),
      new KeyValuePair<string, Type>("LastWriteTime", typeof (DateTime)),
      new KeyValuePair<string, Type>("CacheStamp", typeof (ulong)),
      new KeyValuePair<string, Type>("fDeleted", typeof (bool)),
      new KeyValuePair<string, Type>("ParentID", typeof (Guid))
    };

    internal static void FillPayloadWithWorkItem(
      WorkItemTrackingRequestContext witRequestContext,
      WorkItem workItem,
      Payload workItemPayload,
      int minimumRevisionId,
      int revisionId,
      DateTime? asOfDate)
    {
      WorkItemRevision latestRevision = PayloadCompatibilityUtils.GetLatestRevision(workItem, revisionId, asOfDate);
      if (latestRevision == null)
      {
        PayloadCompatibilityUtils.FillPayloadWithWorkItemNotFoundOrNoAccess(workItemPayload);
      }
      else
      {
        IReadOnlyCollection<WorkItemRevision> revisionsToProcess = PayloadCompatibilityUtils.GetRevisionsToProcess(workItem, latestRevision, Math.Max(0, minimumRevisionId - 1));
        FieldEntry[] array1 = PayloadCompatibilityUtils.GetFields(witRequestContext).Where<FieldEntry>((System.Func<FieldEntry, bool>) (fe => !PayloadCompatibilityUtils.s_fieldsToRemove.Contains(fe.FieldId))).ToArray<FieldEntry>();
        FieldEntry[] array2 = ((IEnumerable<FieldEntry>) array1).Where<FieldEntry>((System.Func<FieldEntry, bool>) (f => f.StorageTarget == FieldStorageTarget.LongTexts)).ToArray<FieldEntry>();
        FieldEntry[] array3 = ((IEnumerable<FieldEntry>) array1).Except<FieldEntry>((IEnumerable<FieldEntry>) array2).ToArray<FieldEntry>();
        KeyValuePair<string, Type>[] array4 = ((IEnumerable<FieldEntry>) array3).Select<FieldEntry, KeyValuePair<string, Type>>((System.Func<FieldEntry, KeyValuePair<string, Type>>) (field => new KeyValuePair<string, Type>(field.ReferenceName, field.SystemType))).ToArray<KeyValuePair<string, Type>>();
        PayloadCompatibilityUtils.AddWorkItemInfoTable(witRequestContext, workItemPayload, (IEnumerable<KeyValuePair<string, Type>>) array4, (IEnumerable<FieldEntry>) array3, latestRevision);
        PayloadCompatibilityUtils.AddRevisionsTable(workItemPayload, (IEnumerable<KeyValuePair<string, Type>>) array4, latestRevision, revisionsToProcess, (IEnumerable<FieldEntry>) array3);
        PayloadCompatibilityUtils.AddKeyWordsTable(workItemPayload);
        IReadOnlyCollection<PayloadCompatibilityUtils.LongTextRecord> longTextRecords = PayloadCompatibilityUtils.GetLongTextRecords((IEnumerable<FieldEntry>) array2, revisionsToProcess, latestRevision);
        PayloadCompatibilityUtils.AddTextsTable(witRequestContext, "Texts", PayloadCompatibilityUtils.c_textTableSchema, workItemPayload, longTextRecords.Select<PayloadCompatibilityUtils.LongTextRecord, object[]>((System.Func<PayloadCompatibilityUtils.LongTextRecord, object[]>) (record => new object[4]
        {
          (object) record.FieldId,
          (object) record.AuthorizedDate,
          (object) record.Value,
          (object) 0
        })));
        PayloadCompatibilityUtils.AddFilesTable(workItemPayload, latestRevision, (IEnumerable<WorkItemRevision>) revisionsToProcess);
        PayloadCompatibilityUtils.AddRelationsTable(workItemPayload, workItem, latestRevision);
        PayloadCompatibilityUtils.AddRelationRevisionsTable(workItemPayload, latestRevision, (IEnumerable<WorkItemRevision>) revisionsToProcess);
      }
    }

    internal static void FillPayloadWithWorkItemXml(
      WorkItemTrackingRequestContext witRequestContext,
      WorkItem workItem,
      Payload workItemPayload,
      int revisionId)
    {
      WorkItemRevision latestRevision = PayloadCompatibilityUtils.GetLatestRevision(workItem, revisionId);
      if (latestRevision == null)
      {
        PayloadCompatibilityUtils.FillPayloadWithWorkItemXmlNotFoundOrNoAccess(workItemPayload);
      }
      else
      {
        IReadOnlyCollection<WorkItemRevision> revisionsToProcess = PayloadCompatibilityUtils.GetRevisionsToProcess(workItem, latestRevision);
        IReadOnlyCollection<FieldEntry> fields = PayloadCompatibilityUtils.GetFields(witRequestContext);
        FieldEntry[] array1 = fields.Where<FieldEntry>((System.Func<FieldEntry, bool>) (f => f.StorageTarget == FieldStorageTarget.LongTexts)).ToArray<FieldEntry>();
        FieldEntry[] array2 = fields.Except<FieldEntry>((IEnumerable<FieldEntry>) array1).ToArray<FieldEntry>();
        IReadOnlyCollection<PayloadCompatibilityUtils.LongTextRecord> longTextRecords = PayloadCompatibilityUtils.GetLongTextRecords((IEnumerable<FieldEntry>) array1, revisionsToProcess, latestRevision);
        KeyValuePair<string, Type>[] array3 = ((IEnumerable<FieldEntry>) array2).Select<FieldEntry, KeyValuePair<string, Type>>((System.Func<FieldEntry, KeyValuePair<string, Type>>) (field => new KeyValuePair<string, Type>(field.ReferenceName, field.SystemType))).ToArray<KeyValuePair<string, Type>>();
        PayloadCompatibilityUtils.AddWorkItemInfoTable(witRequestContext, workItemPayload, (IEnumerable<KeyValuePair<string, Type>>) array3, (IEnumerable<FieldEntry>) array2, latestRevision);
        PayloadCompatibilityUtils.AddTextsTable(witRequestContext, "Texts", PayloadCompatibilityUtils.c_longTextTableSchema, workItemPayload, longTextRecords.Select<PayloadCompatibilityUtils.LongTextRecord, object[]>((System.Func<PayloadCompatibilityUtils.LongTextRecord, object[]>) (record => new object[5]
        {
          (object) witRequestContext.FieldDictionary.GetField(record.FieldId).ReferenceName,
          (object) record.FieldId,
          (object) record.AuthorizedDate,
          (object) record.Value,
          (object) 0
        })));
        PayloadCompatibilityUtils.AddHistoryTable(workItemPayload, (IEnumerable<PayloadCompatibilityUtils.LongTextRecord>) longTextRecords);
        PayloadCompatibilityUtils.AddFilesTable(workItemPayload, latestRevision);
        PayloadCompatibilityUtils.AddHyperlinksTable(workItemPayload, latestRevision);
        PayloadCompatibilityUtils.AddRelatedLinksTable(witRequestContext, workItemPayload, latestRevision);
        PayloadCompatibilityUtils.AddFieldsTable(workItemPayload, fields);
        PayloadCompatibilityUtils.AddExternalLinks(workItemPayload, latestRevision);
      }
    }

    private static IReadOnlyCollection<FieldEntry> GetFields(
      WorkItemTrackingRequestContext witRequestContext)
    {
      IFieldTypeDictionary fieldDict = witRequestContext.FieldDictionary;
      HashSet<int> source = new HashSet<int>(fieldDict.GetAllFields().Where<FieldEntry>((System.Func<FieldEntry, bool>) (fld => !fld.IsIgnored)).Select<FieldEntry, int>((System.Func<FieldEntry, int>) (fld => fld.FieldId)));
      source.Add(-6);
      source.Remove(-57);
      source.Remove(-31);
      source.Remove(-32);
      source.Remove(75);
      source.Remove(-12);
      source.Remove(-35);
      source.Remove(58);
      return (IReadOnlyCollection<FieldEntry>) source.Select<int, FieldEntry>((System.Func<int, FieldEntry>) (fieldId => fieldDict.GetField(fieldId))).Where<FieldEntry>((System.Func<FieldEntry, bool>) (x => (x.Usage & InternalFieldUsages.WorkItem) == InternalFieldUsages.WorkItem)).ToArray<FieldEntry>();
    }

    private static IReadOnlyCollection<WorkItemRevision> GetRevisionsToProcess(
      WorkItem workItem,
      WorkItemRevision latestRevision,
      int startRevision = 0)
    {
      int num = latestRevision.Revision - 2;
      List<WorkItemRevision> revisionsToProcess = new List<WorkItemRevision>();
      for (int index = num; index >= startRevision; --index)
        revisionsToProcess.Add(workItem.Revisions[index]);
      return (IReadOnlyCollection<WorkItemRevision>) revisionsToProcess;
    }

    private static WorkItemRevision GetLatestRevision(WorkItem workItem, int revisionId) => PayloadCompatibilityUtils.GetLatestRevision(workItem, revisionId, new DateTime?());

    private static WorkItemRevision GetLatestRevision(
      WorkItem workItem,
      int revisionId,
      DateTime? asOfDate)
    {
      WorkItemRevision latestRevision;
      if (revisionId == 0)
      {
        latestRevision = !asOfDate.HasValue || workItem.AuthorizedDate <= asOfDate.Value ? (WorkItemRevision) workItem : workItem.Revisions.Reverse<WorkItemRevision>().FirstOrDefault<WorkItemRevision>((System.Func<WorkItemRevision, bool>) (workItemRevision => workItemRevision.AuthorizedDate <= asOfDate.Value));
      }
      else
      {
        int num = Math.Min(revisionId, workItem.Revision);
        if (workItem.Revision == num)
        {
          latestRevision = (WorkItemRevision) workItem;
        }
        else
        {
          if (num < 1 || workItem.Revisions.Count == 0)
            throw new ArgumentOutOfRangeException(nameof (revisionId));
          latestRevision = workItem.Revisions[num - 1];
        }
      }
      return latestRevision;
    }

    private static IReadOnlyCollection<PayloadCompatibilityUtils.LongTextRecord> GetLongTextRecords(
      IEnumerable<FieldEntry> longTextFields,
      IReadOnlyCollection<WorkItemRevision> revisionsToProcess,
      WorkItemRevision latestRevision)
    {
      List<PayloadCompatibilityUtils.LongTextRecord> longTextRecords = new List<PayloadCompatibilityUtils.LongTextRecord>();
      Dictionary<int, PayloadCompatibilityUtils.LongTextRecord> dictionary = new Dictionary<int, PayloadCompatibilityUtils.LongTextRecord>();
      foreach (WorkItemRevision workItemRevision in revisionsToProcess.Reverse<WorkItemRevision>().Concat<WorkItemRevision>((IEnumerable<WorkItemRevision>) new WorkItemRevision[1]
      {
        latestRevision
      }))
      {
        foreach (FieldEntry longTextField in longTextFields)
        {
          string str = workItemRevision.LatestData.GetValueOrDefault<string>(longTextField.FieldId) ?? "";
          bool flag = false;
          PayloadCompatibilityUtils.LongTextRecord longTextRecord;
          if (dictionary.TryGetValue(longTextField.FieldId, out longTextRecord))
            flag = longTextRecord.Value != str || longTextRecord.FieldId == 54;
          else if (!string.IsNullOrEmpty(str))
            flag = true;
          if (flag)
          {
            longTextRecord = new PayloadCompatibilityUtils.LongTextRecord()
            {
              FieldId = longTextField.FieldId,
              AuthorizedDate = workItemRevision.AuthorizedDate,
              Value = str,
              AuthorizedBy = workItemRevision.ModifiedBy
            };
            dictionary[longTextField.FieldId] = longTextRecord;
            longTextRecords.Add(longTextRecord);
          }
        }
      }
      return (IReadOnlyCollection<PayloadCompatibilityUtils.LongTextRecord>) longTextRecords;
    }

    private static void AddRelationRevisionsTable(
      Payload workItemPayload,
      WorkItemRevision latestRevision,
      IEnumerable<WorkItemRevision> revisionsToProcess)
    {
      PayloadTable table = new PayloadTable((PayloadTableConverter) null)
      {
        TableName = "RelationRevisions",
        InitializationTableSchema = (IPayloadTableSchema) DalGetWorkItemElement.c_workItemLinksWereTableSchema
      };
      HashSet<Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels.WorkItemLinkInfo> source = new HashSet<Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels.WorkItemLinkInfo>();
      foreach (WorkItemRevision workItemRevision in revisionsToProcess)
        source.UnionWith((IEnumerable<Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels.WorkItemLinkInfo>) workItemRevision.AllLinks);
      foreach (Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels.WorkItemLinkInfo allLink in latestRevision.AllLinks)
      {
        if (allLink.RevisedDate < latestRevision.RevisedDate)
          source.Add(allLink);
        else
          source.Remove(allLink);
      }
      table.Populate((IDataReader) new GenericDataReader((IEnumerable<KeyValuePair<string, Type>>) PayloadCompatibilityUtils.c_relationsTableSchema, (IEnumerable<IEnumerable<object>>) source.Select<Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels.WorkItemLinkInfo, object[]>((System.Func<Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels.WorkItemLinkInfo, object[]>) (link => new object[10]
      {
        (object) link.TargetId,
        (object) link.Comment ?? (object) DBNull.Value,
        (object) link.AuthorizedById,
        (object) link.AuthorizedDate,
        (object) link.RevisedById,
        (object) link.RevisedDate,
        (object) (short) link.LinkType,
        (object) link.IsLocked,
        (object) link.AuthorizedDate,
        (object) link.RevisedDate
      }))));
      workItemPayload.Tables.Add(table);
    }

    private static void AddRelationsTable(
      Payload workItemPayload,
      WorkItem workItem,
      WorkItemRevision latestRevision)
    {
      PayloadTable table = new PayloadTable((PayloadTableConverter) null)
      {
        TableName = "Relations",
        InitializationTableSchema = (IPayloadTableSchema) DalGetWorkItemElement.c_workItemLinksAreTableSchema
      };
      IEnumerable<Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels.WorkItemLinkInfo> source = workItem.Revision == latestRevision.Revision ? latestRevision.WorkItemLinks : latestRevision.AllLinks.Where<Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels.WorkItemLinkInfo>((System.Func<Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels.WorkItemLinkInfo, bool>) (l => l.AuthorizedDate <= latestRevision.AuthorizedDate));
      table.Populate((IDataReader) new GenericDataReader((IEnumerable<KeyValuePair<string, Type>>) PayloadCompatibilityUtils.c_relationsTableSchema, (IEnumerable<IEnumerable<object>>) source.Select<Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels.WorkItemLinkInfo, object[]>((System.Func<Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels.WorkItemLinkInfo, object[]>) (link => new object[10]
      {
        (object) link.TargetId,
        (object) link.Comment ?? (object) DBNull.Value,
        (object) link.AuthorizedById,
        (object) link.AuthorizedDate,
        (object) link.RevisedById,
        (object) link.RevisedDate,
        (object) (short) link.LinkType,
        (object) link.IsLocked,
        (object) link.AuthorizedDate,
        (object) link.RevisedDate
      }))));
      workItemPayload.Tables.Add(table);
    }

    private static void AddKeyWordsTable(Payload workItemPayload)
    {
      PayloadTable table = new PayloadTable((PayloadTableConverter) null)
      {
        TableName = "Keywords",
        InitializationTableSchema = (IPayloadTableSchema) DalGetWorkItemElement.c_zeroTableSchema
      };
      workItemPayload.Tables.Add(table);
      table.Populate((IDataReader) new GenericDataReader((IEnumerable<KeyValuePair<string, Type>>) new KeyValuePair<string, Type>[1]
      {
        new KeyValuePair<string, Type>("zero", typeof (int))
      }, (IEnumerable<IEnumerable<object>>) new object[1][]
      {
        new object[1]{ (object) 0 }
      }));
    }

    private static void AddRevisionsTable(
      Payload workItemPayload,
      IEnumerable<KeyValuePair<string, Type>> columnInfo,
      WorkItemRevision latestRevision,
      IReadOnlyCollection<WorkItemRevision> revisionsToProcess,
      IEnumerable<FieldEntry> regularFields)
    {
      PayloadTable table = new PayloadTable((PayloadTableConverter) null, new int?(revisionsToProcess.Count))
      {
        TableName = "Revisions",
        InitializationTableSchema = (IPayloadTableSchema) new PayloadTableSchema(columnInfo.Select<KeyValuePair<string, Type>, PayloadTableSchema.Column>((System.Func<KeyValuePair<string, Type>, PayloadTableSchema.Column>) (ci => new PayloadTableSchema.Column(ci.Key, ci.Value))).ToArray<PayloadTableSchema.Column>())
      };
      workItemPayload.Tables.Add(table);
      List<object[]> data = new List<object[]>(revisionsToProcess.Count<WorkItemRevision>());
      ReadOnlyDictionary<int, object> latestData1 = latestRevision.LatestData;
      FieldEntry[] array = regularFields.ToArray<FieldEntry>();
      int length = array.Length;
      int index1 = Array.FindIndex<FieldEntry>(array, (Predicate<FieldEntry>) (fe => fe.FieldId == -42));
      object[] objArray1 = new object[length];
      object[] objArray2 = new object[length];
      for (int index2 = 0; index2 < length; ++index2)
        objArray1[index2] = latestData1.GetValueOrDefault<object>(array[index2].FieldId);
      foreach (WorkItemFieldData workItemFieldData in (IEnumerable<WorkItemRevision>) revisionsToProcess)
      {
        ReadOnlyDictionary<int, object> latestData2 = workItemFieldData.LatestData;
        for (int index3 = 0; index3 < length; ++index3)
          objArray2[index3] = latestData2.GetValueOrDefault<object>(array[index3].FieldId);
        object[] objArray3 = new object[length];
        for (int index4 = 0; index4 < length; ++index4)
        {
          object objA = objArray1[index4];
          object objB = objArray2[index4];
          objArray3[index4] = index4 != index1 || objB == null ? (!object.Equals(objA, objB) ? (objB != null ? objB ?? (object) DBNull.Value : objA ?? (object) DBNull.Value) : (object) DBNull.Value) : objB;
        }
        data.Add(objArray3);
        object[] objArray4 = objArray1;
        objArray1 = objArray2;
        objArray2 = objArray4;
      }
      table.Populate((IDataReader) new GenericDataReader(columnInfo, (IEnumerable<IEnumerable<object>>) data));
    }

    private static void AddWorkItemInfoTable(
      WorkItemTrackingRequestContext witRequestContext,
      Payload workItemPayload,
      IEnumerable<KeyValuePair<string, Type>> columnInfo,
      IEnumerable<FieldEntry> regularFields,
      WorkItemRevision latestRevision)
    {
      PayloadTable table = new PayloadTable((PayloadTableConverter) null, new int?(1))
      {
        TableName = "WorkItemInfo",
        InitializationTableSchema = (IPayloadTableSchema) new PayloadTableSchema(columnInfo.Select<KeyValuePair<string, Type>, PayloadTableSchema.Column>((System.Func<KeyValuePair<string, Type>, PayloadTableSchema.Column>) (ci => new PayloadTableSchema.Column(ci.Key, ci.Value))).ToArray<PayloadTableSchema.Column>())
      };
      workItemPayload.Tables.Add(table);
      object[] array = regularFields.Select<FieldEntry, object>((System.Func<FieldEntry, object>) (fieldEntry =>
      {
        if (fieldEntry.FieldId == -7)
          return (object) latestRevision.GetAreaPath(witRequestContext.RequestContext);
        if (fieldEntry.FieldId == -105)
          return (object) latestRevision.GetIterationPath(witRequestContext.RequestContext);
        if (fieldEntry.FieldId == -42)
          return (object) latestRevision.GetProjectName(witRequestContext.RequestContext);
        return fieldEntry.FieldId == -1 ? (object) latestRevision.ModifiedBy : latestRevision.LatestData.GetValueOrDefault<object>(fieldEntry.FieldId) ?? (object) DBNull.Value;
      })).ToArray<object>();
      table.Populate((IDataReader) new GenericDataReader(columnInfo, (IEnumerable<IEnumerable<object>>) new object[1][]
      {
        array
      }));
    }

    private static void AddTextsTable(
      WorkItemTrackingRequestContext witRequestContext,
      string tableName,
      KeyValuePair<string, Type>[] tableSchema,
      Payload workItemPayload,
      IEnumerable<object[]> records)
    {
      PayloadTable table = new PayloadTable((PayloadTableConverter) null)
      {
        TableName = tableName,
        InitializationTableSchema = (IPayloadTableSchema) new PayloadTableSchema(((IEnumerable<KeyValuePair<string, Type>>) tableSchema).Select<KeyValuePair<string, Type>, PayloadTableSchema.Column>((System.Func<KeyValuePair<string, Type>, PayloadTableSchema.Column>) (ci => new PayloadTableSchema.Column(ci.Key, ci.Value))).ToArray<PayloadTableSchema.Column>())
      };
      workItemPayload.Tables.Add(table);
      table.Populate((IDataReader) new GenericDataReader((IEnumerable<KeyValuePair<string, Type>>) tableSchema, (IEnumerable<IEnumerable<object>>) records));
    }

    private static void AddFilesTable(Payload workItemPayload, WorkItemRevision latestRevision)
    {
      PayloadTable table = new PayloadTable((PayloadTableConverter) null)
      {
        TableName = "Files",
        InitializationTableSchema = (IPayloadTableSchema) new PayloadTableSchema(((IEnumerable<KeyValuePair<string, Type>>) PayloadCompatibilityUtils.c_attachmentsTableSchema).Select<KeyValuePair<string, Type>, PayloadTableSchema.Column>((System.Func<KeyValuePair<string, Type>, PayloadTableSchema.Column>) (ci => new PayloadTableSchema.Column(ci.Key, ci.Value))).ToArray<PayloadTableSchema.Column>())
      };
      table.Populate((IDataReader) new GenericDataReader((IEnumerable<KeyValuePair<string, Type>>) PayloadCompatibilityUtils.c_attachmentsTableSchema, (IEnumerable<IEnumerable<object>>) latestRevision.ResourceLinks.Where<WorkItemResourceLinkInfo>((System.Func<WorkItemResourceLinkInfo, bool>) (link => link.ResourceType == ResourceLinkType.Attachment)).OrderBy<WorkItemResourceLinkInfo, DateTime>((System.Func<WorkItemResourceLinkInfo, DateTime>) (file => file.AuthorizedDate)).Select<WorkItemResourceLinkInfo, object[]>((System.Func<WorkItemResourceLinkInfo, object[]>) (file => new object[5]
      {
        (object) file.ResourceId,
        (object) file.Name,
        (object) file.AuthorizedDate,
        (object) file.ResourceSize,
        (object) file.Comment
      }))));
      workItemPayload.Tables.Add(table);
    }

    private static void AddFilesTable(
      Payload workItemPayload,
      WorkItemRevision latestRevision,
      IEnumerable<WorkItemRevision> revisionsToProcess)
    {
      PayloadTable table = new PayloadTable((PayloadTableConverter) null)
      {
        TableName = "Files",
        InitializationTableSchema = (IPayloadTableSchema) DalGetWorkItemElement.c_attachmentsTableSchema
      };
      workItemPayload.Tables.Add(table);
      HashSet<WorkItemResourceLinkInfo> source1 = new HashSet<WorkItemResourceLinkInfo>((IEnumerable<WorkItemResourceLinkInfo>) latestRevision.ResourceLinks);
      foreach (WorkItemRevision workItemRevision in revisionsToProcess)
        source1.UnionWith((IEnumerable<WorkItemResourceLinkInfo>) workItemRevision.ResourceLinks);
      HashSet<WorkItemResourceLinkInfo> source2 = new HashSet<WorkItemResourceLinkInfo>(source1.Where<WorkItemResourceLinkInfo>((System.Func<WorkItemResourceLinkInfo, bool>) (link => PayloadCompatibilityUtils.LinkIsSupportedInSOAP(link))));
      table.Populate((IDataReader) new GenericDataReader((IEnumerable<KeyValuePair<string, Type>>) PayloadCompatibilityUtils.c_filesTableSchema, (IEnumerable<IEnumerable<object>>) source2.OrderBy<WorkItemResourceLinkInfo, DateTime>((System.Func<WorkItemResourceLinkInfo, DateTime>) (file => file.AuthorizedDate)).Select<WorkItemResourceLinkInfo, object[]>((System.Func<WorkItemResourceLinkInfo, object[]>) (file => new object[12]
      {
        (object) (int) file.ResourceType,
        (object) file.RevisedDate,
        (object) file.AuthorizedDate,
        (object) file.Location ?? (object) DBNull.Value,
        (object) file.Name ?? (object) DBNull.Value,
        (object) file.ResourceId,
        (object) file.Comment ?? (object) DBNull.Value,
        (object) file.ResourceCreatedDate,
        (object) file.ResourceModifiedDate,
        (object) file.ResourceSize,
        (object) file.AuthorizedDate,
        (object) file.RevisedDate
      }))));
    }

    private static bool LinkIsSupportedInSOAP(WorkItemResourceLinkInfo link)
    {
      bool flag = true;
      try
      {
        ArtifactId artifactId = LinkingUtilities.DecodeUri(link.Location);
        foreach (string str in PayloadCompatibilityUtils.ToolsNotSupportedInSOAP)
        {
          if (artifactId.Tool.Equals(str, StringComparison.OrdinalIgnoreCase))
          {
            flag = false;
            break;
          }
        }
      }
      catch (ArgumentException ex)
      {
        flag = true;
      }
      return flag;
    }

    private static void AddHistoryTable(
      Payload workItemPayload,
      IEnumerable<PayloadCompatibilityUtils.LongTextRecord> longTextRecords)
    {
      PayloadTable table = new PayloadTable((PayloadTableConverter) null)
      {
        TableName = "History",
        InitializationTableSchema = (IPayloadTableSchema) new PayloadTableSchema(((IEnumerable<KeyValuePair<string, Type>>) PayloadCompatibilityUtils.c_historyTableSchema).Select<KeyValuePair<string, Type>, PayloadTableSchema.Column>((System.Func<KeyValuePair<string, Type>, PayloadTableSchema.Column>) (kvp => new PayloadTableSchema.Column(kvp.Key, kvp.Value))).ToArray<PayloadTableSchema.Column>())
      };
      workItemPayload.Tables.Add(table);
      table.Populate((IDataReader) new GenericDataReader((IEnumerable<KeyValuePair<string, Type>>) PayloadCompatibilityUtils.c_historyTableSchema, (IEnumerable<IEnumerable<object>>) longTextRecords.Where<PayloadCompatibilityUtils.LongTextRecord>((System.Func<PayloadCompatibilityUtils.LongTextRecord, bool>) (record => record.FieldId == 54 && !string.IsNullOrEmpty(record.Value))).OrderByDescending<PayloadCompatibilityUtils.LongTextRecord, DateTime>((System.Func<PayloadCompatibilityUtils.LongTextRecord, DateTime>) (record => record.AuthorizedDate)).Select<PayloadCompatibilityUtils.LongTextRecord, object[]>((System.Func<PayloadCompatibilityUtils.LongTextRecord, object[]>) (record => new object[5]
      {
        (object) record.AuthorizedBy,
        (object) record.FieldId,
        (object) record.AuthorizedDate,
        (object) record.Value,
        (object) 0
      }))));
    }

    private static void AddHyperlinksTable(Payload workItemPayload, WorkItemRevision latestRevision)
    {
      PayloadTable table = new PayloadTable((PayloadTableConverter) null)
      {
        TableName = "Hyperlinks",
        InitializationTableSchema = (IPayloadTableSchema) new PayloadTableSchema(((IEnumerable<KeyValuePair<string, Type>>) PayloadCompatibilityUtils.c_hyperLinksTableSchema).Select<KeyValuePair<string, Type>, PayloadTableSchema.Column>((System.Func<KeyValuePair<string, Type>, PayloadTableSchema.Column>) (kvp => new PayloadTableSchema.Column(kvp.Key, kvp.Value))).ToArray<PayloadTableSchema.Column>())
      };
      workItemPayload.Tables.Add(table);
      table.Populate((IDataReader) new GenericDataReader((IEnumerable<KeyValuePair<string, Type>>) PayloadCompatibilityUtils.c_hyperLinksTableSchema, (IEnumerable<IEnumerable<object>>) latestRevision.ResourceLinks.Where<WorkItemResourceLinkInfo>((System.Func<WorkItemResourceLinkInfo, bool>) (link => link.ResourceType == ResourceLinkType.Hyperlink)).OrderByDescending<WorkItemResourceLinkInfo, DateTime>((System.Func<WorkItemResourceLinkInfo, DateTime>) (link => link.AuthorizedDate)).Select<WorkItemResourceLinkInfo, object[]>((System.Func<WorkItemResourceLinkInfo, object[]>) (link => new object[3]
      {
        (object) link.Location,
        (object) link.Comment,
        (object) link.AuthorizedDate
      }))));
    }

    private static void AddRelatedLinksTable(
      WorkItemTrackingRequestContext witRequestContext,
      Payload workItemPayload,
      WorkItemRevision latestRevision)
    {
      WorkItemTrackingLinkService linkService = witRequestContext.RequestContext.GetService<WorkItemTrackingLinkService>();
      PayloadTable table = new PayloadTable((PayloadTableConverter) null)
      {
        TableName = "RelatedLinks",
        InitializationTableSchema = (IPayloadTableSchema) new PayloadTableSchema(((IEnumerable<KeyValuePair<string, Type>>) PayloadCompatibilityUtils.c_relatedLinksTableSchema).Select<KeyValuePair<string, Type>, PayloadTableSchema.Column>((System.Func<KeyValuePair<string, Type>, PayloadTableSchema.Column>) (kvp => new PayloadTableSchema.Column(kvp.Key, kvp.Value))).ToArray<PayloadTableSchema.Column>())
      };
      workItemPayload.Tables.Add(table);
      table.Populate((IDataReader) new GenericDataReader((IEnumerable<KeyValuePair<string, Type>>) PayloadCompatibilityUtils.c_relatedLinksTableSchema, (IEnumerable<IEnumerable<object>>) latestRevision.WorkItemLinks.OrderByDescending<Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels.WorkItemLinkInfo, DateTime>((System.Func<Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels.WorkItemLinkInfo, DateTime>) (link => link.AuthorizedDate)).Select<Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels.WorkItemLinkInfo, object[]>((System.Func<Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels.WorkItemLinkInfo, object[]>) (link =>
      {
        MDWorkItemLinkType linkTypeById = linkService.GetLinkTypeById(witRequestContext.RequestContext, link.LinkType);
        return new object[4]
        {
          (object) link.SourceId,
          (object) link.TargetId,
          (object) link.Comment,
          link.LinkType > 0 ? (object) linkTypeById.ForwardEndName : (object) linkTypeById.ReverseEndName
        };
      }))));
    }

    private static void AddFieldsTable(
      Payload workItemPayload,
      IReadOnlyCollection<FieldEntry> fields)
    {
      PayloadTable table = new PayloadTable((PayloadTableConverter) null, new int?(fields.Count))
      {
        TableName = "Fields",
        InitializationTableSchema = (IPayloadTableSchema) new PayloadTableSchema(((IEnumerable<KeyValuePair<string, Type>>) PayloadCompatibilityUtils.c_fieldsTableSchema).Select<KeyValuePair<string, Type>, PayloadTableSchema.Column>((System.Func<KeyValuePair<string, Type>, PayloadTableSchema.Column>) (kvp => new PayloadTableSchema.Column(kvp.Key, kvp.Value))).ToArray<PayloadTableSchema.Column>())
      };
      workItemPayload.Tables.Add(table);
      table.Populate((IDataReader) new GenericDataReader((IEnumerable<KeyValuePair<string, Type>>) PayloadCompatibilityUtils.c_fieldsTableSchema, (IEnumerable<IEnumerable<object>>) fields.Select<FieldEntry, object[]>((System.Func<FieldEntry, object[]>) (field => new object[3]
      {
        (object) field.ReferenceName,
        (object) field.Name,
        (object) field.FieldDataType
      }))));
    }

    private static void AddExternalLinks(Payload workItemPayload, WorkItemRevision latestRevision)
    {
      PayloadTable table = new PayloadTable((PayloadTableConverter) null)
      {
        TableName = "ExternalLinks",
        InitializationTableSchema = (IPayloadTableSchema) new PayloadTableSchema(((IEnumerable<KeyValuePair<string, Type>>) PayloadCompatibilityUtils.c_externalLinksTableSchema).Select<KeyValuePair<string, Type>, PayloadTableSchema.Column>((System.Func<KeyValuePair<string, Type>, PayloadTableSchema.Column>) (kvp => new PayloadTableSchema.Column(kvp.Key, kvp.Value))).ToArray<PayloadTableSchema.Column>())
      };
      workItemPayload.Tables.Add(table);
      table.Populate((IDataReader) new GenericDataReader((IEnumerable<KeyValuePair<string, Type>>) PayloadCompatibilityUtils.c_externalLinksTableSchema, (IEnumerable<IEnumerable<object>>) latestRevision.ResourceLinks.Where<WorkItemResourceLinkInfo>((System.Func<WorkItemResourceLinkInfo, bool>) (link => link.ResourceType == ResourceLinkType.ArtifactLink)).OrderByDescending<WorkItemResourceLinkInfo, DateTime>((System.Func<WorkItemResourceLinkInfo, DateTime>) (link => link.AuthorizedDate)).Select<WorkItemResourceLinkInfo, object[]>((System.Func<WorkItemResourceLinkInfo, object[]>) (link => new object[4]
      {
        (object) link.Location,
        (object) link.Comment,
        (object) link.AuthorizedDate,
        (object) link.Name
      }))));
    }

    private static void FillPayloadWithWorkItemXmlNotFoundOrNoAccess(Payload workItemPayload)
    {
      workItemPayload.Tables.Add(PayloadCompatibilityUtils.CreateEmptyTable("WorkItemInfo"));
      workItemPayload.Tables.Add(PayloadCompatibilityUtils.CreateEmptyTable("Texts"));
      workItemPayload.Tables.Add(PayloadCompatibilityUtils.CreateEmptyTable("History"));
      workItemPayload.Tables.Add(PayloadCompatibilityUtils.CreateEmptyTable("Files"));
      workItemPayload.Tables.Add(PayloadCompatibilityUtils.CreateEmptyTable("Hyperlinks"));
      workItemPayload.Tables.Add(PayloadCompatibilityUtils.CreateEmptyTable("RelatedLinks"));
      workItemPayload.Tables.Add(PayloadCompatibilityUtils.CreateEmptyTable("Fields"));
      workItemPayload.Tables.Add(PayloadCompatibilityUtils.CreateEmptyTable("ExternalLinks"));
    }

    internal static void FillPayloadWithWorkItemPageData(
      WorkItemTrackingRequestContext witRequestContext,
      IReadOnlyCollection<WorkItemFieldData> pageData,
      Payload payload,
      FieldEntry[] columnFields,
      FieldEntry[] longTextFields)
    {
      if (columnFields != null && ((IEnumerable<FieldEntry>) columnFields).Any<FieldEntry>())
      {
        int columnCount = columnFields.Length;
        KeyValuePair<string, Type>[] array = ((IEnumerable<FieldEntry>) columnFields).Select<FieldEntry, KeyValuePair<string, Type>>((System.Func<FieldEntry, KeyValuePair<string, Type>>) (field => new KeyValuePair<string, Type>(field.ReferenceName, field.SystemType))).ToArray<KeyValuePair<string, Type>>();
        PayloadTableSchema payloadTableSchema = new PayloadTableSchema(((IEnumerable<KeyValuePair<string, Type>>) array).Select<KeyValuePair<string, Type>, PayloadTableSchema.Column>((System.Func<KeyValuePair<string, Type>, PayloadTableSchema.Column>) (ci => new PayloadTableSchema.Column(ci.Key, ci.Value))).ToArray<PayloadTableSchema.Column>());
        PayloadTable table = new PayloadTable((PayloadTableConverter) null, new int?(pageData.Count))
        {
          TableName = "Items",
          InitializationTableSchema = (IPayloadTableSchema) payloadTableSchema
        };
        payload.Tables.Add(table);
        table.Populate((IDataReader) new GenericDataReader((IEnumerable<KeyValuePair<string, Type>>) array, (IEnumerable<IEnumerable<object>>) pageData.Select<WorkItemFieldData, object[]>((System.Func<WorkItemFieldData, object[]>) (record =>
        {
          object[] objArray = new object[columnCount];
          int index1 = 0;
          for (int index2 = columnCount; index1 < index2; ++index1)
          {
            FieldEntry columnField = columnFields[index1];
            objArray[index1] = record.GetFieldValue(witRequestContext, columnField.FieldId, false) ?? (object) DBNull.Value;
          }
          return objArray;
        })).ToArray<object[]>()));
      }
      if (longTextFields == null || !((IEnumerable<FieldEntry>) longTextFields).Any<FieldEntry>())
        return;
      PayloadTable table1 = new PayloadTable((PayloadTableConverter) null)
      {
        TableName = "LongTextItems",
        InitializationTableSchema = (IPayloadTableSchema) new PayloadTableSchema(((IEnumerable<KeyValuePair<string, Type>>) PayloadCompatibilityUtils.c_textTableSchemaPaging).Select<KeyValuePair<string, Type>, PayloadTableSchema.Column>((System.Func<KeyValuePair<string, Type>, PayloadTableSchema.Column>) (kvp => new PayloadTableSchema.Column(kvp.Key, kvp.Value))).ToArray<PayloadTableSchema.Column>())
      };
      payload.Tables.Add(table1);
      IEnumerable<IEnumerable<object>> array1 = (IEnumerable<IEnumerable<object>>) pageData.SelectMany<WorkItemFieldData, object[]>((System.Func<WorkItemFieldData, IEnumerable<object[]>>) (record => ((IEnumerable<FieldEntry>) longTextFields).Select<FieldEntry, object[]>((System.Func<FieldEntry, object[]>) (ltf =>
      {
        string fieldValue = record.GetFieldValue<string>(witRequestContext, ltf.FieldId);
        if (fieldValue == null)
          return (object[]) null;
        return new object[4]
        {
          (object) record.GetFieldValue<DateTime>(witRequestContext, 3),
          (object) ltf.FieldId,
          (object) record.Id,
          (object) fieldValue
        };
      })).Where<object[]>((System.Func<object[], bool>) (data => data != null)))).ToArray<object[]>();
      table1.Populate((IDataReader) new GenericDataReader((IEnumerable<KeyValuePair<string, Type>>) PayloadCompatibilityUtils.c_textTableSchemaPaging, array1));
    }

    internal static void FillPayloadWithWorkItemNotFoundOrNoAccess(Payload workItemPayload)
    {
      workItemPayload.Tables.Add(PayloadCompatibilityUtils.CreateEmptyTable("WorkItemInfo"));
      workItemPayload.Tables.Add(PayloadCompatibilityUtils.CreateEmptyTable("Revisions"));
      workItemPayload.Tables.Add(PayloadCompatibilityUtils.CreateEmptyTable("Keywords"));
      workItemPayload.Tables.Add(PayloadCompatibilityUtils.CreateEmptyTable("Texts"));
      workItemPayload.Tables.Add(PayloadCompatibilityUtils.CreateEmptyTable("Files"));
      workItemPayload.Tables.Add(PayloadCompatibilityUtils.CreateEmptyTable("Relations"));
      workItemPayload.Tables.Add(PayloadCompatibilityUtils.CreateEmptyTable("RelationRevisions"));
    }

    private static PayloadTable CreateEmptyTable(string tableName)
    {
      KeyValuePair<string, Type>[] keyValuePairArray = new KeyValuePair<string, Type>[1]
      {
        new KeyValuePair<string, Type>("requiredColumn", typeof (string))
      };
      PayloadTableSchema payloadTableSchema = new PayloadTableSchema(((IEnumerable<KeyValuePair<string, Type>>) keyValuePairArray).Select<KeyValuePair<string, Type>, PayloadTableSchema.Column>((System.Func<KeyValuePair<string, Type>, PayloadTableSchema.Column>) (ci => new PayloadTableSchema.Column(ci.Key, ci.Value))).ToArray<PayloadTableSchema.Column>());
      PayloadTable emptyTable = new PayloadTable((PayloadTableConverter) null);
      emptyTable.TableName = tableName;
      emptyTable.InitializationTableSchema = (IPayloadTableSchema) payloadTableSchema;
      emptyTable.Populate((IDataReader) new GenericDataReader((IEnumerable<KeyValuePair<string, Type>>) keyValuePairArray, Enumerable.Empty<IEnumerable<object>>()));
      return emptyTable;
    }

    internal static void FillPayloadWithStoredQueryItems(
      Payload queriesPayload,
      IEnumerable<Query> storedQueryItems,
      int projectId)
    {
      if (queriesPayload == null)
        throw new ArgumentNullException(nameof (queriesPayload));
      ArgumentUtility.CheckForNull<IEnumerable<Query>>(storedQueryItems, nameof (storedQueryItems));
      if (projectId < 0)
        throw new ArgumentException("ProjectID");
      PayloadTable table = new PayloadTable((PayloadTableConverter) new QueryItemPayloadTableConverter(PayloadCompatibilityUtils.c_StoredQueryItemsTableSchema))
      {
        TableName = "StoredQueries",
        InitializationTableSchema = (IPayloadTableSchema) new PayloadTableSchema(((IEnumerable<KeyValuePair<string, Type>>) PayloadCompatibilityUtils.c_StoredQueryItemsTableSchema).Select<KeyValuePair<string, Type>, PayloadTableSchema.Column>((System.Func<KeyValuePair<string, Type>, PayloadTableSchema.Column>) (kvp => new PayloadTableSchema.Column(kvp.Key, kvp.Value))).ToArray<PayloadTableSchema.Column>())
      };
      table.Populate((IDataReader) new GenericDataReader((IEnumerable<KeyValuePair<string, Type>>) PayloadCompatibilityUtils.c_StoredQueryItemsTableSchema, (IEnumerable<IEnumerable<object>>) storedQueryItems.Select<Query, object[]>((System.Func<Query, object[]>) (query => PayloadCompatibilityUtils.ConvertQuery((Microsoft.TeamFoundation.WorkItemTracking.Server.QueryItems.QueryItem) query, projectId)))));
      queriesPayload.Tables.Add(table);
    }

    internal static void FillPayloadWithQueryItem(
      IVssRequestContext requestContext,
      Payload queryPayload,
      Microsoft.TeamFoundation.WorkItemTracking.Server.QueryItems.QueryItem queryItem)
    {
      PayloadTable table = new PayloadTable((PayloadTableConverter) new QueryItemPayloadTableConverter(PayloadCompatibilityUtils.c_StoredQueryItemsTableSchema))
      {
        TableName = "StoredQueries",
        InitializationTableSchema = (IPayloadTableSchema) new PayloadTableSchema(((IEnumerable<KeyValuePair<string, Type>>) PayloadCompatibilityUtils.c_StoredQueryItemsTableSchema).Select<KeyValuePair<string, Type>, PayloadTableSchema.Column>((System.Func<KeyValuePair<string, Type>, PayloadTableSchema.Column>) (kvp => new PayloadTableSchema.Column(kvp.Key, kvp.Value))).ToArray<PayloadTableSchema.Column>())
      };
      int id = requestContext.WitContext().TreeService.GetTreeNode(queryItem.ProjectId, queryItem.ProjectId).Id;
      table.Populate((IDataReader) new GenericDataReader((IEnumerable<KeyValuePair<string, Type>>) PayloadCompatibilityUtils.c_StoredQueryItemsTableSchema, (IEnumerable<IEnumerable<object>>) Enumerable.Repeat<object[]>(PayloadCompatibilityUtils.ConvertQueryForDefinition(requestContext, queryItem, id), 1)));
      queryPayload.Tables.Add(table);
    }

    internal static void FillPayloadWithQueryItems(
      Payload queriesPayload,
      IEnumerable<Microsoft.TeamFoundation.WorkItemTracking.Server.QueryItems.QueryItem> publicQueryItems,
      IEnumerable<Microsoft.TeamFoundation.WorkItemTracking.Server.QueryItems.QueryItem> privateQueryItems)
    {
      if (queriesPayload == null)
        throw new ArgumentNullException(nameof (queriesPayload));
      ArgumentUtility.CheckForNull<IEnumerable<Microsoft.TeamFoundation.WorkItemTracking.Server.QueryItems.QueryItem>>(publicQueryItems, nameof (publicQueryItems));
      ArgumentUtility.CheckForNull<IEnumerable<Microsoft.TeamFoundation.WorkItemTracking.Server.QueryItems.QueryItem>>(privateQueryItems, nameof (privateQueryItems));
      QueryItemPayloadTableConverter converter = new QueryItemPayloadTableConverter(PayloadCompatibilityUtils.c_QueryItemsTableSchema);
      PayloadTableSchema payloadTableSchema = new PayloadTableSchema(((IEnumerable<KeyValuePair<string, Type>>) PayloadCompatibilityUtils.c_QueryItemsTableSchema).Select<KeyValuePair<string, Type>, PayloadTableSchema.Column>((System.Func<KeyValuePair<string, Type>, PayloadTableSchema.Column>) (kvp => new PayloadTableSchema.Column(kvp.Key, kvp.Value))).ToArray<PayloadTableSchema.Column>());
      PayloadTable table1 = new PayloadTable((PayloadTableConverter) converter)
      {
        TableName = "PublicQueryItems",
        InitializationTableSchema = (IPayloadTableSchema) payloadTableSchema
      };
      PayloadTable table2 = new PayloadTable((PayloadTableConverter) converter)
      {
        TableName = "PrivateQueryItems",
        InitializationTableSchema = (IPayloadTableSchema) payloadTableSchema
      };
      table1.Populate((IDataReader) new GenericDataReader((IEnumerable<KeyValuePair<string, Type>>) PayloadCompatibilityUtils.c_QueryItemsTableSchema, (IEnumerable<IEnumerable<object>>) publicQueryItems.Select<Microsoft.TeamFoundation.WorkItemTracking.Server.QueryItems.QueryItem, object[]>((System.Func<Microsoft.TeamFoundation.WorkItemTracking.Server.QueryItems.QueryItem, object[]>) (queryItem => PayloadCompatibilityUtils.ConvertQueryItem(queryItem)))));
      table2.Populate((IDataReader) new GenericDataReader((IEnumerable<KeyValuePair<string, Type>>) PayloadCompatibilityUtils.c_QueryItemsTableSchema, (IEnumerable<IEnumerable<object>>) privateQueryItems.Select<Microsoft.TeamFoundation.WorkItemTracking.Server.QueryItems.QueryItem, object[]>((System.Func<Microsoft.TeamFoundation.WorkItemTracking.Server.QueryItems.QueryItem, object[]>) (queryItem => PayloadCompatibilityUtils.ConvertQueryItem(queryItem)))));
      queriesPayload.Tables.Add(table1);
      queriesPayload.Tables.Add(table2);
    }

    private static object[] ConvertQueryItem(Microsoft.TeamFoundation.WorkItemTracking.Server.QueryItems.QueryItem queryItem)
    {
      List<object> objectList = new List<object>();
      objectList.Add((object) queryItem.Id);
      if (queryItem.ParentId == Guid.Empty)
        objectList.Add((object) DBNull.Value);
      else
        objectList.Add((object) queryItem.ParentId);
      objectList.Add((object) queryItem.Name);
      if (queryItem is Query query)
        objectList.Add((object) query.Wiql);
      else
        objectList.Add((object) DBNull.Value);
      objectList.Add((object) (queryItem is QueryFolder));
      objectList.Add((object) queryItem.IsDeleted);
      objectList.Add((object) 0UL);
      objectList.Add((object) null);
      objectList.Add((object) null);
      return objectList.ToArray();
    }

    private static object[] ConvertQuery(Microsoft.TeamFoundation.WorkItemTracking.Server.QueryItems.QueryItem queryItem, int projectId)
    {
      Query query = queryItem as Query;
      List<object> objectList = new List<object>();
      objectList.Add((object) queryItem.Id);
      objectList.Add((object) projectId);
      objectList.Add((object) queryItem.IsPublic);
      objectList.Add((object) null);
      objectList.Add((object) null);
      objectList.Add((object) queryItem.Name);
      if (query != null)
        objectList.Add((object) query.Wiql);
      else
        objectList.Add((object) DBNull.Value);
      objectList.Add((object) DBNull.Value);
      objectList.Add((object) queryItem.CreatedDate);
      objectList.Add((object) queryItem.ModifiedDate);
      objectList.Add((object) 0UL);
      objectList.Add((object) queryItem.IsDeleted);
      objectList.Add((object) queryItem.ParentId);
      return objectList.ToArray();
    }

    private static object[] ConvertQueryForDefinition(
      IVssRequestContext requestContext,
      Microsoft.TeamFoundation.WorkItemTracking.Server.QueryItems.QueryItem queryItem,
      int projectId)
    {
      Query query = queryItem as Query;
      List<object> objectList = new List<object>();
      objectList.Add((object) queryItem.Id);
      objectList.Add((object) projectId);
      objectList.Add((object) queryItem.IsPublic);
      objectList.Add((object) requestContext.UserContext.Identifier);
      objectList.Add((object) requestContext.UserContext.IdentityType);
      objectList.Add((object) queryItem.Name);
      if (query != null)
        objectList.Add((object) query.Wiql);
      else
        objectList.Add((object) DBNull.Value);
      objectList.Add((object) DBNull.Value);
      objectList.Add((object) queryItem.CreatedDate);
      objectList.Add((object) queryItem.ModifiedDate);
      objectList.Add((object) 0UL);
      objectList.Add((object) queryItem.IsDeleted);
      objectList.Add((object) queryItem.ParentId);
      return objectList.ToArray();
    }

    private class LongTextRecord
    {
      public int FieldId { get; set; }

      public DateTime AuthorizedDate { get; set; }

      public string Value { get; set; }

      public string AuthorizedBy { get; set; }
    }
  }
}
