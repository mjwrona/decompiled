// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItemComponent
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccess;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.Comments;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Social.WebApi;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems
{
  internal class WorkItemComponent : WorkItemTrackingResourceComponent
  {
    private static readonly IDictionary<int, SqlExceptionFactory> s_sqlExceptionFactories = (IDictionary<int, SqlExceptionFactory>) new Dictionary<int, SqlExceptionFactory>();
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[53]
    {
      (IComponentCreator) new ComponentCreator<WorkItemComponent>(2),
      (IComponentCreator) new ComponentCreator<WorkItemComponent2>(3),
      (IComponentCreator) new ComponentCreator<WorkItemComponent3>(4),
      (IComponentCreator) new ComponentCreator<WorkItemComponent4>(5),
      (IComponentCreator) new ComponentCreator<WorkItemComponent5>(6),
      (IComponentCreator) new ComponentCreator<WorkItemComponent6>(7),
      (IComponentCreator) new ComponentCreator<WorkItemComponent7>(8),
      (IComponentCreator) new ComponentCreator<WorkItemComponent8>(9),
      (IComponentCreator) new ComponentCreator<WorkItemComponent9>(10),
      (IComponentCreator) new ComponentCreator<WorkItemComponent10>(11),
      (IComponentCreator) new ComponentCreator<WorkItemComponent11>(12),
      (IComponentCreator) new ComponentCreator<WorkItemComponent12>(13),
      (IComponentCreator) new ComponentCreator<WorkItemComponent13>(14),
      (IComponentCreator) new ComponentCreator<WorkItemComponent14>(15),
      (IComponentCreator) new ComponentCreator<WorkItemComponent15>(16),
      (IComponentCreator) new ComponentCreator<WorkItemComponent16>(17),
      (IComponentCreator) new ComponentCreator<WorkItemComponent17>(18),
      (IComponentCreator) new ComponentCreator<WorkItemComponent18>(19),
      (IComponentCreator) new ComponentCreator<WorkItemComponent19>(20),
      (IComponentCreator) new ComponentCreator<WorkItemComponent20>(21),
      (IComponentCreator) new ComponentCreator<WorkItemComponent21>(22),
      (IComponentCreator) new ComponentCreator<WorkItemComponent22>(23),
      (IComponentCreator) new ComponentCreator<WorkItemComponent23>(24),
      (IComponentCreator) new ComponentCreator<WorkItemComponent24>(25),
      (IComponentCreator) new ComponentCreator<WorkItemComponent25>(26),
      (IComponentCreator) new ComponentCreator<WorkItemComponent26>(27),
      (IComponentCreator) new ComponentCreator<WorkItemComponent27>(28),
      (IComponentCreator) new ComponentCreator<WorkItemComponent28>(29),
      (IComponentCreator) new ComponentCreator<WorkItemComponent29>(30),
      (IComponentCreator) new ComponentCreator<WorkItemComponent30>(31),
      (IComponentCreator) new ComponentCreator<WorkItemComponent31>(32),
      (IComponentCreator) new ComponentCreator<WorkItemComponent32>(33),
      (IComponentCreator) new ComponentCreator<WorkItemComponent33>(34),
      (IComponentCreator) new ComponentCreator<WorkItemComponent34>(35),
      (IComponentCreator) new ComponentCreator<WorkItemComponent35>(36),
      (IComponentCreator) new ComponentCreator<WorkItemComponent36>(37),
      (IComponentCreator) new ComponentCreator<WorkItemComponent37>(38),
      (IComponentCreator) new ComponentCreator<WorkItemComponent38>(39),
      (IComponentCreator) new ComponentCreator<WorkItemComponent39>(40),
      (IComponentCreator) new ComponentCreator<WorkItemComponent40>(41),
      (IComponentCreator) new ComponentCreator<WorkItemComponent41>(42),
      (IComponentCreator) new ComponentCreator<WorkItemComponent42>(43),
      (IComponentCreator) new ComponentCreator<WorkItemComponent43>(44),
      (IComponentCreator) new ComponentCreator<WorkItemComponent44>(45),
      (IComponentCreator) new ComponentCreator<WorkItemComponent45>(46),
      (IComponentCreator) new ComponentCreator<WorkItemComponent46>(47),
      (IComponentCreator) new ComponentCreator<WorkItemComponent46>(48),
      (IComponentCreator) new ComponentCreator<WorkItemComponent46>(49),
      (IComponentCreator) new ComponentCreator<WorkItemComponent46>(50),
      (IComponentCreator) new ComponentCreator<WorkItemComponent51>(51),
      (IComponentCreator) new ComponentCreator<WorkItemComponent53>(52),
      (IComponentCreator) new ComponentCreator<WorkItemComponent53>(53),
      (IComponentCreator) new ComponentCreator<WorkItemComponent54>(54)
    }, "WorkItem", "WorkItem");

    protected static string GetIdentityDisplayName(
      IDataReader reader,
      ref SqlColumnBinder displayPartColumn,
      ref SqlColumnBinder identityDisplayNameColumn,
      ref SqlColumnBinder hasUniqueIdentityDisplayNameColumn,
      IdentityDisplayType identityDisplayType)
    {
      string identityDisplayName = identityDisplayNameColumn.GetString(reader, true);
      switch (identityDisplayType)
      {
        case IdentityDisplayType.DisplayName:
          if (string.IsNullOrEmpty(identityDisplayName))
          {
            identityDisplayName = displayPartColumn.GetString(reader, true);
            break;
          }
          break;
        case IdentityDisplayType.ComboDisplayNameWhenNeeded:
          if (!hasUniqueIdentityDisplayNameColumn.GetBoolean(reader, true) || string.IsNullOrEmpty(identityDisplayName))
          {
            identityDisplayName = displayPartColumn.GetString(reader, true);
            break;
          }
          break;
        case IdentityDisplayType.ComboDisplayName:
          identityDisplayName = displayPartColumn.GetString(reader, true);
          break;
      }
      return identityDisplayName;
    }

    protected virtual SqlParameter BindCoreFieldUpdates(
      string parameterName,
      IEnumerable<WorkItemCoreFieldUpdatesRecord> rows)
    {
      return this.BindBasicTvp<WorkItemCoreFieldUpdatesRecord>((WorkItemTrackingResourceComponent.TvpRecordBinder<WorkItemCoreFieldUpdatesRecord>) new WorkItemComponent.WorkItemCoreFieldValuesTableRecordBinder(), parameterName, rows);
    }

    protected virtual SqlParameter BindCustomFieldUpdates(
      string parameterName,
      IEnumerable<WorkItemCustomFieldUpdateRecord> rows)
    {
      return this.BindBasicTvp<WorkItemCustomFieldUpdateRecord>((WorkItemTrackingResourceComponent.TvpRecordBinder<WorkItemCustomFieldUpdateRecord>) new WorkItemComponent.WorkItemFieldUpdateTableRecordBinder(), parameterName, rows);
    }

    protected virtual SqlParameter BindWorkItemTextFieldUpdates(
      string parameterName,
      IEnumerable<WorkItemTextFieldUpdateRecord> rows)
    {
      return this.BindBasicTvp<WorkItemTextFieldUpdateRecord>((WorkItemTrackingResourceComponent.TvpRecordBinder<WorkItemTextFieldUpdateRecord>) new WorkItemComponent.WorkItemTextFieldUpdateTableRecordBinder(), parameterName, rows);
    }

    protected virtual SqlParameter BindWorkItemLinkUpdates(
      string parameterName,
      IEnumerable<WorkItemLinkUpdateRecord> rows)
    {
      return this.BindBasicTvp<WorkItemLinkUpdateRecord>((WorkItemTrackingResourceComponent.TvpRecordBinder<WorkItemLinkUpdateRecord>) new WorkItemComponent.WorkItemLinkUpdateTableRecordBinder(), parameterName, rows);
    }

    protected virtual SqlParameter BindWorkItemResourceLinkUpdates(
      string parameterName,
      IEnumerable<WorkItemResourceLinkUpdateRecord> rows)
    {
      return this.BindBasicTvp<WorkItemResourceLinkUpdateRecord>((WorkItemTrackingResourceComponent.TvpRecordBinder<WorkItemResourceLinkUpdateRecord>) new WorkItemComponent.WorkItemResourceLinkUpdateTableRecordBinder(), parameterName, rows);
    }

    protected virtual void BindWorkItemTeamProjectChangeUpdates(
      string parameterName,
      IEnumerable<WorkItemTeamProjectChangeRecord> rows)
    {
    }

    protected SqlParameter BindWorkItemTagUpdates(string parameterName) => this.BindXml(parameterName, string.Empty);

    protected virtual SqlParameter BindPendingSetMembershipChecks(
      string parameterName,
      IEnumerable<PendingSetMembershipCheckRecord> rows)
    {
      return this.BindBasicTvp<PendingSetMembershipCheckRecord>((WorkItemTrackingResourceComponent.TvpRecordBinder<PendingSetMembershipCheckRecord>) new WorkItemComponent.WorkItemPendingSetMembershipCheckTableRecordBinder(), parameterName, rows);
    }

    static WorkItemComponent()
    {
      WorkItemComponent.s_sqlExceptionFactories[600182] = new SqlExceptionFactory(typeof (WorkItemDateInFutureException), (Func<IVssRequestContext, int, SqlException, SqlError, Exception>) ((r, i, ex, e) => (Exception) new WorkItemDateInFutureException()));
      WorkItemComponent.s_sqlExceptionFactories[600188] = new SqlExceptionFactory(typeof (WorkItemPickListNotFoundException), (Func<IVssRequestContext, int, SqlException, SqlError, Exception>) ((r, i, ex, e) =>
      {
        Guid result;
        Guid.TryParse(ex.Message, out result);
        return (Exception) new WorkItemPickListNotFoundException(result);
      }));
      WorkItemComponent.s_sqlExceptionFactories[600189] = new SqlExceptionFactory(typeof (ArgumentNullException), (Func<IVssRequestContext, int, SqlException, SqlError, Exception>) ((r, i, ex, e) => (Exception) new ArgumentNullException("listName")));
      WorkItemComponent.s_sqlExceptionFactories[600190] = new SqlExceptionFactory(typeof (ArgumentNullException), (Func<IVssRequestContext, int, SqlException, SqlError, Exception>) ((r, i, ex, e) => (Exception) new ArgumentNullException("items")));
      WorkItemComponent.s_sqlExceptionFactories[600279] = new SqlExceptionFactory(typeof (WorkItemLinksLimitExceededException), (Func<IVssRequestContext, int, SqlException, SqlError, Exception>) ((r, i, ex, e) => (Exception) new WorkItemLinksLimitExceededException(r.WitContext().ServerSettings.WorkItemLinksLimit)));
      WorkItemComponent.s_sqlExceptionFactories[600281] = new SqlExceptionFactory(typeof (WorkItemRemoteLinksLimitExceededException), (Func<IVssRequestContext, int, SqlException, SqlError, Exception>) ((r, i, ex, e) => (Exception) new WorkItemRemoteLinksLimitExceededException(r.WitContext().ServerSettings.WorkItemRemoteLinksLimit)));
      WorkItemComponent.s_sqlExceptionFactories[600122] = new SqlExceptionFactory(typeof (WorkItemRevisionMismatchException), (Func<IVssRequestContext, int, SqlException, SqlError, Exception>) ((r, i, ex, e) => (Exception) new WorkItemRevisionMismatchException(i)));
    }

    protected virtual bool DatasetBinderBindsTitle => true;

    protected virtual bool UpdateBindsTags => true;

    protected virtual bool BindIncludeCountFields => false;

    protected override IDictionary<int, SqlExceptionFactory> TranslatedExceptions => WorkItemComponent.s_sqlExceptionFactories;

    public virtual string RemoveUnusedContent(bool readOnlyMode = false)
    {
      this.PrepareStoredProcedure(nameof (RemoveUnusedContent));
      object obj = this.ExecuteScalar();
      return obj != null ? obj.ToString() : string.Empty;
    }

    public virtual string RemoveDestroyedContent() => string.Empty;

    internal virtual void BindparametersForGetWorkItemsProc(
      IEnumerable<int> workItemIds,
      bool includeCountFields,
      bool includeCustomFields,
      bool includeTextFields,
      bool includeResourceLinks,
      bool includeWorkItemLinks,
      bool includeHistory,
      bool sortLinks,
      int maxLongTextLength,
      int maxRevisionLongTextLength,
      DateTime? asOf,
      DateTime? revisionsSince,
      bool includeComments,
      bool includeCommentHistory)
    {
      this.BindInt32Table("@workItemIds", workItemIds);
      this.BindBoolean("@includeCustomFields", includeCustomFields);
      this.BindBoolean("@includeTextFields", includeTextFields);
      this.BindBoolean("@includeResourceLinks", includeResourceLinks);
      this.BindBoolean("@includeWorkItemLinks", includeWorkItemLinks);
      this.BindBoolean("@includeHistory", includeHistory);
      if (this.BindIncludeCountFields)
        this.BindBoolean("@includeCountFields", includeCountFields);
      this.BindMaxLongTextLength(maxLongTextLength);
      this.BindMaxRevisionLongTextLength(maxRevisionLongTextLength);
    }

    public virtual IEnumerable<WorkItemDataset> GetWorkItemDatasets(
      IEnumerable<int> workItemIds,
      bool includeCountFields,
      bool includeCustomFields,
      bool includeTextFields,
      bool includeResourceLinks,
      bool includeWorkItemLinks,
      bool includeHistory,
      bool sortLinks,
      int maxLongTextLength,
      int maxRevisionLongTextLength,
      IdentityDisplayType identityDisplayType)
    {
      return this.GetWorkItemDatasets(workItemIds, includeCountFields, includeCustomFields, includeTextFields, includeResourceLinks, includeWorkItemLinks, includeHistory, sortLinks, maxLongTextLength, maxRevisionLongTextLength, identityDisplayType, new DateTime?(), new DateTime?(), false, false);
    }

    public virtual IEnumerable<WorkItemDataset> GetWorkItemDatasets(
      IEnumerable<int> workItemIds,
      bool includeCountFields,
      bool includeCustomFields,
      bool includeTextFields,
      bool includeResourceLinks,
      bool includeWorkItemLinks,
      bool includeHistory,
      bool sortLinks,
      int maxLongTextLength,
      int maxRevisionLongTextLength,
      IdentityDisplayType identityDisplayType,
      DateTime? asOf,
      DateTime? revisionsSince,
      bool includeComments,
      bool includeCommentHistory)
    {
      IFieldTypeDictionary fieldDictionary = this.RequestContext.WitContext().FieldDictionary;
      workItemIds = (IEnumerable<int>) workItemIds.Distinct<int>().ToArray<int>();
      this.PrepareStoredProcedure("prc_GetWorkItems");
      this.BindparametersForGetWorkItemsProc(workItemIds, includeCountFields, includeCustomFields, includeTextFields, includeResourceLinks, includeWorkItemLinks, includeHistory, sortLinks, maxLongTextLength, maxRevisionLongTextLength, asOf, revisionsSince, includeComments, includeCommentHistory);
      return this.ExecuteUnknown<IEnumerable<WorkItemDataset>>((System.Func<IDataReader, IEnumerable<WorkItemDataset>>) (reader =>
      {
        WorkItemDataset[] array = this.GetWorkItemDataSetBinder(this.DatasetBinderBindsTitle, includeCountFields, identityDisplayType).BindAll(reader).ToArray<WorkItemDataset>();
        List<WorkItemCustomFieldValue> entities1 = new List<WorkItemCustomFieldValue>();
        List<WorkItemResourceLinkInfo> source = new List<WorkItemResourceLinkInfo>();
        List<WorkItemResourceLinkInfo> entities2 = new List<WorkItemResourceLinkInfo>();
        List<WorkItemLinkInfo> workItemLinkInfoList1 = new List<WorkItemLinkInfo>();
        List<WorkItemCommentVersionRecord> workItemComments = new List<WorkItemCommentVersionRecord>();
        Dictionary<int, WorkItemDataset> dictionary = ((IEnumerable<WorkItemDataset>) array).ToDictionary<WorkItemDataset, int>((System.Func<WorkItemDataset, int>) (wfv => wfv.Id));
        if (includeCustomFields)
        {
          reader.NextResult();
          entities1.AddRange(this.GetWorkItemCustomFieldValueBinder(this.RequestContext.WitContext().FieldDictionary, identityDisplayType).BindAll(reader));
        }
        if (includeTextFields)
        {
          reader.NextResult();
          entities1.AddRange(new WorkItemComponent.WorkItemTextFieldValueBinder().BindAll(reader));
        }
        if (includeResourceLinks)
        {
          reader.NextResult();
          List<WorkItemResourceLinkInfo> list = new WorkItemComponent.WorkItemResourceLinkBinder().BindAll(reader).ToList<WorkItemResourceLinkInfo>();
          source.AddRange(list.Where<WorkItemResourceLinkInfo>((System.Func<WorkItemResourceLinkInfo, bool>) (rl => rl.ResourceType != ResourceLinkType.InlineImage)));
          entities2.AddRange(list.Where<WorkItemResourceLinkInfo>((System.Func<WorkItemResourceLinkInfo, bool>) (rl => rl.ResourceType == ResourceLinkType.InlineImage)));
        }
        if (includeWorkItemLinks)
        {
          reader.NextResult();
          workItemLinkInfoList1.AddRange(this.GetWorkItemLinkBinder(identityDisplayType).BindAll(reader));
          workItemLinkInfoList1 = this.FilterWorkItemLinksWithAsOf(workItemLinkInfoList1, asOf);
        }
        if (includeHistory || asOf.HasValue)
        {
          reader.NextResult();
          WorkItemComponent.FillRevisions(this.RequestContext, (IEnumerable<WorkItemRevisionDataset>) this.GetWorkItemDataSetBinder(this.DatasetBinderBindsTitle, includeCountFields, identityDisplayType).BindAll(reader), dictionary);
          if (includeCustomFields)
          {
            reader.NextResult();
            entities1.AddRange(this.GetWorkItemCustomFieldValueBinder(this.RequestContext.WitContext().FieldDictionary, identityDisplayType).BindAll(reader));
          }
          if (includeTextFields)
          {
            reader.NextResult();
            entities1.AddRange(new WorkItemComponent.WorkItemTextFieldValueBinder().BindAll(reader));
          }
          if (includeResourceLinks)
          {
            reader.NextResult();
            List<WorkItemResourceLinkInfo> list = new WorkItemComponent.WorkItemResourceLinkBinder().BindAll(reader).ToList<WorkItemResourceLinkInfo>();
            source.AddRange(list.Where<WorkItemResourceLinkInfo>((System.Func<WorkItemResourceLinkInfo, bool>) (rl => rl.ResourceType != ResourceLinkType.InlineImage)));
            entities2.AddRange(list.Where<WorkItemResourceLinkInfo>((System.Func<WorkItemResourceLinkInfo, bool>) (rl => rl.ResourceType == ResourceLinkType.InlineImage)));
          }
          if (includeWorkItemLinks)
          {
            reader.NextResult();
            workItemLinkInfoList1.AddRange(this.GetWorkItemLinkBinder(identityDisplayType).BindAll(reader));
          }
          if (includeComments)
          {
            reader.NextResult();
            WorkItemComponent.WorkItemCommentVersionBinder commentVersionBinder = new WorkItemComponent.WorkItemCommentVersionBinder();
            workItemComments.AddRange(commentVersionBinder.BindAll(reader));
          }
        }
        List<WorkItemResourceLinkInfo> resourceLinkInfoList = this.StitchLinks<WorkItemResourceLinkInfo>(source.GroupBy<WorkItemResourceLinkInfo, string>((System.Func<WorkItemResourceLinkInfo, string>) (l => l.Location)));
        List<WorkItemLinkInfo> workItemLinkInfoList2 = this.StitchLinks<WorkItemLinkInfo>(workItemLinkInfoList1.GroupBy<WorkItemLinkInfo, string>((System.Func<WorkItemLinkInfo, string>) (l => string.Format("{0}_{1}", (object) l.TargetId, (object) l.LinkType))));
        WorkItemComponent.UpdateLinkDates((IDictionary<int, WorkItemDataset>) dictionary, (IEnumerable<IRevisedWorkItemEntity>) resourceLinkInfoList);
        WorkItemComponent.UpdateLinkDates((IDictionary<int, WorkItemDataset>) dictionary, (IEnumerable<IRevisedWorkItemEntity>) workItemLinkInfoList2);
        if (sortLinks)
        {
          resourceLinkInfoList.Sort();
          workItemLinkInfoList2.Sort();
        }
        WorkItemComponent.FillRevisedEntities<WorkItemCustomFieldValue>(entities1, dictionary, (Action<WorkItemRevisionDataset, WorkItemCustomFieldValue>) ((revision, fieldValue) =>
        {
          if (fieldValue is WorkItemCustomFieldValue.WorkItemLargeTextCustomFieldValue customFieldValue2)
          {
            revision.Fields[fieldValue.FieldId] = (object) new WorkItemLargeTextValue()
            {
              Text = customFieldValue2.StringValue,
              IsHtml = customFieldValue2.IsHtml
            };
          }
          else
          {
            revision.Fields[fieldValue.FieldId] = fieldValue.Value;
            if (!fieldValue.IsIdentityField || !fieldValue.GuidValue.HasValue)
              return;
            Guid? guidValue = fieldValue.GuidValue;
            Guid empty = Guid.Empty;
            if ((guidValue.HasValue ? (guidValue.HasValue ? (guidValue.GetValueOrDefault() != empty ? 1 : 0) : 0) : 1) == 0 || revision.IdentityFields.ContainsKey(fieldValue.FieldId))
              return;
            Dictionary<int, Guid> identityFields = revision.IdentityFields;
            int fieldId = fieldValue.FieldId;
            guidValue = fieldValue.GuidValue;
            Guid guid = guidValue.Value;
            identityFields[fieldId] = guid;
          }
        }));
        WorkItemComponent.FillRevisedEntities<WorkItemResourceLinkInfo>(resourceLinkInfoList, dictionary, (Action<WorkItemRevisionDataset, WorkItemResourceLinkInfo>) ((revision, resourceLink) => revision.ResourceLinks.Add(resourceLink)), (Action<WorkItemDataset, IEnumerable<WorkItemResourceLinkInfo>>) ((dataset, links) => dataset.AllResourceLinks.AddRange(links)));
        WorkItemComponent.FillRevisedEntities<WorkItemResourceLinkInfo>(entities2, dictionary, (Action<WorkItemRevisionDataset, WorkItemResourceLinkInfo>) ((revision, resourceLink) => revision.HiddenResourceLinks.Add(resourceLink)), (Action<WorkItemDataset, IEnumerable<WorkItemResourceLinkInfo>>) ((dataset, links) => dataset.AllHiddenResourceLinks.AddRange(links)));
        WorkItemComponent.FillRevisedEntities<WorkItemLinkInfo>(workItemLinkInfoList2, dictionary, (Action<WorkItemRevisionDataset, WorkItemLinkInfo>) ((revision, workItemLink) =>
        {
          if (asOf.HasValue && workItemLink.RevisedDate != SharedVariables.FutureDateTimeValue)
          {
            DateTime revisedDate = workItemLink.RevisedDate;
            DateTime? nullable = asOf;
            if ((nullable.HasValue ? (revisedDate >= nullable.GetValueOrDefault() ? 1 : 0) : 0) != 0)
              workItemLink.RevisedDate = SharedVariables.FutureDateTimeValue;
          }
          revision.AllLinks.Add(workItemLink);
        }));
        this.UpdateWorkItemCommentRevisions(workItemComments, dictionary);
        return (IEnumerable<WorkItemDataset>) array;
      }));
    }

    protected virtual List<WorkItemLinkInfo> FilterWorkItemLinksWithAsOf(
      List<WorkItemLinkInfo> workItemLinks,
      DateTime? asOf)
    {
      return asOf.HasValue ? workItemLinks.Where<WorkItemLinkInfo>((System.Func<WorkItemLinkInfo, bool>) (l =>
      {
        DateTime authorizedDate = l.AuthorizedDate;
        DateTime? nullable1 = asOf;
        if ((nullable1.HasValue ? (authorizedDate <= nullable1.GetValueOrDefault() ? 1 : 0) : 0) == 0)
          return false;
        DateTime revisedDate = l.RevisedDate;
        DateTime? nullable2 = asOf;
        return nullable2.HasValue && revisedDate > nullable2.GetValueOrDefault();
      })).ToList<WorkItemLinkInfo>() : workItemLinks;
    }

    private List<T> StitchLinks<T>(IEnumerable<IGrouping<string, T>> linkGroups) where T : IRevisedWorkItemEntity
    {
      List<T> objList = new List<T>();
      foreach (IEnumerable<T> linkGroup in linkGroups)
      {
        T[] array = linkGroup.OrderBy<T, DateTime>((System.Func<T, DateTime>) (l => l.AuthorizedDate)).ToArray<T>();
        DateTime dateTime = DateTime.MinValue;
        bool flag1 = true;
        for (int index = 0; index < array.Length; ++index)
        {
          if (flag1)
          {
            dateTime = array[index].AuthorizedDate;
            flag1 = false;
          }
          int num = index == array.Length - 1 ? 1 : 0;
          bool flag2 = num == 0 && array[index].RevisedDate == array[index + 1].AuthorizedDate;
          if (num != 0 || !flag2)
          {
            T obj = array[index];
            obj.AuthorizedDate = dateTime;
            objList.Add(obj);
            flag1 = true;
          }
        }
      }
      return objList;
    }

    private void UpdateWorkItemCommentRevisions(
      List<WorkItemCommentVersionRecord> workItemComments,
      Dictionary<int, WorkItemDataset> workItemDatasetMap)
    {
      if (workItemDatasetMap?.Values == null)
        return;
      bool enableCommentVersions = WorkItemTrackingFeatureFlags.IsCommentServiceReadsFromNewStorageEnabled(this.RequestContext);
      foreach (WorkItemDataset dataset in workItemDatasetMap.Values)
      {
        if (dataset != null)
        {
          WorkItemComponent.SetWorkItemComment(enableCommentVersions, workItemComments, (WorkItemRevisionDataset) dataset);
          List<WorkItemRevisionDataset> revisions = dataset.Revisions;
          // ISSUE: explicit non-virtual call
          if ((revisions != null ? (__nonvirtual (revisions.Count) > 0 ? 1 : 0) : 0) != 0)
          {
            foreach (WorkItemRevisionDataset revision in dataset.Revisions)
              WorkItemComponent.SetWorkItemComment(enableCommentVersions, workItemComments, revision);
          }
        }
      }
    }

    protected static void UpdateLinkDates(
      IDictionary<int, WorkItemDataset> workItemDatasetMap,
      IEnumerable<IRevisedWorkItemEntity> links)
    {
      foreach (IRevisedWorkItemEntity link1 in links)
      {
        IRevisedWorkItemEntity link = link1;
        WorkItemDataset workItemDataset;
        if (workItemDatasetMap.TryGetValue(link.Id, out workItemDataset))
        {
          WorkItemRevisionDataset itemRevisionDataset = workItemDataset.AuthorizedDate == link.AuthorizedDate ? (WorkItemRevisionDataset) workItemDataset : workItemDataset.Revisions.FirstOrDefault<WorkItemRevisionDataset>((System.Func<WorkItemRevisionDataset, bool>) (r => r.AuthorizedDate == link.AuthorizedDate));
          DateTime dateTime;
          if (itemRevisionDataset != null && itemRevisionDataset.Fields != null && itemRevisionDataset.Fields.TryGetValue<int, DateTime>(-4, out dateTime))
            link.AuthorizedDate = dateTime;
        }
      }
    }

    protected static void FillRevisions(
      IVssRequestContext requestContext,
      IEnumerable<WorkItemRevisionDataset> revisions,
      Dictionary<int, WorkItemDataset> workItemDatasetMap)
    {
      foreach (IGrouping<int, WorkItemRevisionDataset> source in revisions.GroupBy<WorkItemRevisionDataset, int>((System.Func<WorkItemRevisionDataset, int>) (r => r.Id)))
      {
        WorkItemDataset workItemDataset;
        if (!workItemDatasetMap.TryGetValue(source.Key, out workItemDataset))
          throw new WorkItemTrackingSqlDataNotFoundException(string.Format("Could not find workitemdataset with work item revision id : {0}", (object) source.Key));
        IOrderedEnumerable<WorkItemRevisionDataset> collection = source.OrderBy<WorkItemRevisionDataset, DateTime>((System.Func<WorkItemRevisionDataset, DateTime>) (r => r.RevisedDate));
        ITreeDictionary treeService = requestContext.WitContext().TreeService;
        foreach (WorkItemRevisionDataset itemRevisionDataset in (IEnumerable<WorkItemRevisionDataset>) collection)
        {
          try
          {
            if (itemRevisionDataset.ProjectId != Guid.Empty)
            {
              itemRevisionDataset.Fields[-42] = (object) requestContext.WitContext().GetProjectName(itemRevisionDataset.ProjectId);
              TreeNode treeNode1 = treeService.GetTreeNode(itemRevisionDataset.ProjectId, (int) itemRevisionDataset.Fields[-2]);
              if (treeNode1 != null)
                itemRevisionDataset.Fields[-7] = (object) treeNode1.GetPath(requestContext);
              TreeNode treeNode2 = treeService.GetTreeNode(itemRevisionDataset.ProjectId, (int) itemRevisionDataset.Fields[-104]);
              if (treeNode2 != null)
                itemRevisionDataset.Fields[-105] = (object) treeNode2.GetPath(requestContext);
            }
          }
          catch (Exception ex)
          {
            requestContext.TraceException(904932, TraceLevel.Info, nameof (WorkItemComponent), nameof (FillRevisions), ex);
          }
        }
        workItemDataset.Revisions.AddRange((IEnumerable<WorkItemRevisionDataset>) collection);
      }
    }

    protected static void FillRevisedEntities<TEntity>(
      List<TEntity> entities,
      Dictionary<int, WorkItemDataset> workItemDatasetMap,
      Action<WorkItemRevisionDataset, TEntity> revisionDatasetAction,
      Action<WorkItemDataset, IEnumerable<TEntity>> datasetAction = null)
      where TEntity : IRevisedWorkItemEntity
    {
      using (IEnumerator<IGrouping<int, TEntity>> enumerator = entities.GroupBy<TEntity, int>((System.Func<TEntity, int>) (e => e.Id)).GetEnumerator())
      {
label_20:
        while (enumerator.MoveNext())
        {
          IGrouping<int, TEntity> current = enumerator.Current;
          WorkItemDataset workItemDataset;
          if (!workItemDatasetMap.TryGetValue(current.Key, out workItemDataset))
            throw new WorkItemTrackingSqlDataNotFoundException(string.Format("Could not find workitemdataset with entity id : {0}", (object) current.Key));
          TEntity[] array = current.OrderByDescending<TEntity, DateTime>((System.Func<TEntity, DateTime>) (e => e.RevisedDate)).ToArray<TEntity>();
          WorkItemRevisionDataset currentRev = (WorkItemRevisionDataset) workItemDataset;
          int count = workItemDataset.Revisions.Count;
          int index = 0;
          if (datasetAction != null)
            datasetAction(workItemDataset, (IEnumerable<TEntity>) array);
          List<TEntity> source = new List<TEntity>();
          while (true)
          {
            while (index < array.Length)
            {
              TEntity entity = array[index];
              if (entity.RevisedDate > currentRev.AuthorizedDate)
              {
                ++index;
                source.Add(entity);
              }
              else
                break;
            }
            List<TEntity> list = source.Where<TEntity>((System.Func<TEntity, bool>) (e => currentRev.RevisedDate > e.AuthorizedDate)).ToList<TEntity>();
            foreach (TEntity entity in list)
            {
              if (entity.SpansMultipleRevisions)
                revisionDatasetAction(currentRev, entity);
              else if (entity.AuthorizedDate == currentRev.AuthorizedDate)
                revisionDatasetAction(currentRev, entity);
            }
            source = list;
            --count;
            if (count >= 0 && count < workItemDataset.Revisions.Count)
              currentRev = workItemDataset.Revisions[count];
            else
              goto label_20;
          }
        }
      }
    }

    public virtual WorkItemUpdateResultSet UpdateWorkItems(
      IVssIdentity userIdentity,
      bool bypassRules,
      bool isAdmin,
      int trendInterval,
      bool dualSave,
      WorkItemUpdateDataset updateDataset,
      int workItemLinksLimit,
      int workItemRemoteLinksLimit)
    {
      ArgumentUtility.CheckForNull<IVssIdentity>(userIdentity, nameof (userIdentity));
      this.PrepareStoredProcedure("prc_UpdateWorkItems");
      this.BindUpdateWorkItemsParameter(userIdentity, bypassRules, isAdmin, trendInterval, dualSave, updateDataset, workItemLinksLimit, workItemRemoteLinksLimit);
      return this.ExecuteUnknown<WorkItemUpdateResultSet>((System.Func<IDataReader, WorkItemUpdateResultSet>) (reader => this.GetUpdateWorkItemsResultsReader(bypassRules, isAdmin, updateDataset).Read(reader)));
    }

    protected virtual void BindUpdateWorkItemsParameter(
      IVssIdentity userIdentity,
      bool bypassRules,
      bool isAdmin,
      int trendInterval,
      bool dualSave,
      WorkItemUpdateDataset updateDataset,
      int workItemLinksLimit,
      int workItemRemoteLinksLimit)
    {
      this.BindIdentityColumn(userIdentity);
      this.BindInt("@trendInterval", trendInterval);
      this.BindBoolean("@bypassRules", bypassRules);
      this.BindBoolean("@isAdmin", isAdmin);
      this.BindBoolean("@dualSave", dualSave);
      this.BindCoreFieldUpdates("@coreFieldUpdates", (IEnumerable<WorkItemCoreFieldUpdatesRecord>) updateDataset.CoreFieldUpdates);
      this.BindCustomFieldUpdates("@customFieldUpdates", (IEnumerable<WorkItemCustomFieldUpdateRecord>) updateDataset.CustomFieldUpdates);
      this.BindWorkItemTextFieldUpdates("@textFieldUpdates", (IEnumerable<WorkItemTextFieldUpdateRecord>) updateDataset.TextFieldUpdates);
      this.BindWorkItemLinkUpdates("@workItemLinkUpdates", (IEnumerable<WorkItemLinkUpdateRecord>) updateDataset.WorkItemLinkUpdates);
      this.BindWorkItemResourceLinkUpdates("@resourceLinkUpdates", (IEnumerable<WorkItemResourceLinkUpdateRecord>) updateDataset.ResourceLinkUpdates);
      this.BindWorkItemTeamProjectChangeUpdates("@teamProjectChangeUpdates", (IEnumerable<WorkItemTeamProjectChangeRecord>) updateDataset.TeamProjectChanges);
      if (this.UpdateBindsTags)
        this.BindWorkItemTagUpdates("@tags");
      this.BindPendingSetMembershipChecks("@pendingSetMembershipChecks", (IEnumerable<PendingSetMembershipCheckRecord>) updateDataset.PendingSetMembershipChecks);
    }

    internal virtual WorkItemComponent.UpdateWorkItemsResultsReader GetUpdateWorkItemsResultsReader(
      bool bypassRules,
      bool isAdmin,
      WorkItemUpdateDataset updateDataset)
    {
      return new WorkItemComponent.UpdateWorkItemsResultsReader(bypassRules, isAdmin, updateDataset);
    }

    public virtual IEnumerable<WorkItemFieldValues> GetWorkItemFieldValues(
      IEnumerable<WorkItemIdRevisionPair> workItemIdRevPairs,
      IEnumerable<int> wideFields,
      IEnumerable<int> longFields,
      IEnumerable<int> textFields,
      bool byRevision,
      DateTime? asOf,
      int maxLongTextLength,
      IdentityDisplayType identityDisplayType,
      bool disableProjectionLevelThree,
      out int wideTableProjectionLevel,
      int useTableVarThreshold = 4)
    {
      wideTableProjectionLevel = -1;
      workItemIdRevPairs = (IEnumerable<WorkItemIdRevisionPair>) workItemIdRevPairs.Distinct<WorkItemIdRevisionPair>().ToArray<WorkItemIdRevisionPair>();
      IEnumerable<WorkItemDataset> workItemDatasets = this.GetWorkItemDatasets(workItemIdRevPairs.Select<WorkItemIdRevisionPair, int>((System.Func<WorkItemIdRevisionPair, int>) (p => p.Id)), true, longFields.Any<int>(), textFields.Any<int>(), false, false, byRevision || asOf.HasValue, false, maxLongTextLength, maxLongTextLength, identityDisplayType, new DateTime?(), new DateTime?(), false, false);
      if (!byRevision && !asOf.HasValue)
        return (IEnumerable<WorkItemFieldValues>) workItemDatasets;
      if (!byRevision && asOf.HasValue)
      {
        DateTime asofDate = asOf.Value;
        return (IEnumerable<WorkItemFieldValues>) workItemDatasets.Select<WorkItemDataset, WorkItemRevisionDataset>((System.Func<WorkItemDataset, WorkItemRevisionDataset>) (dataset => dataset.AuthorizedDate <= asofDate ? (WorkItemRevisionDataset) dataset : dataset.Revisions.FirstOrDefault<WorkItemRevisionDataset>((System.Func<WorkItemRevisionDataset, bool>) (revision => revision.AuthorizedDate <= asofDate && revision.RevisedDate > asofDate)))).Where<WorkItemRevisionDataset>((System.Func<WorkItemRevisionDataset, bool>) (dataset => dataset != null));
      }
      Dictionary<int, IEnumerable<int>> revMap = workItemIdRevPairs.GroupBy<WorkItemIdRevisionPair, int>((System.Func<WorkItemIdRevisionPair, int>) (rp => rp.Id)).ToDictionary<IGrouping<int, WorkItemIdRevisionPair>, int, IEnumerable<int>>((System.Func<IGrouping<int, WorkItemIdRevisionPair>, int>) (rpg => rpg.Key), (System.Func<IGrouping<int, WorkItemIdRevisionPair>, IEnumerable<int>>) (rpg => rpg.Select<WorkItemIdRevisionPair, int>((System.Func<WorkItemIdRevisionPair, int>) (rp => rp.Revision))));
      IEnumerable<int> source;
      return workItemDatasets.SelectMany<WorkItemDataset, WorkItemFieldValues>((System.Func<WorkItemDataset, IEnumerable<WorkItemFieldValues>>) (dataset => !revMap.TryGetValue(dataset.Id, out source) ? Enumerable.Empty<WorkItemFieldValues>() : (IEnumerable<WorkItemFieldValues>) source.Select<int, WorkItemRevisionDataset>((System.Func<int, WorkItemRevisionDataset>) (revRequired =>
      {
        if (revRequired < 1)
          return (WorkItemRevisionDataset) null;
        if (revRequired > dataset.Revisions.Count)
          return (WorkItemRevisionDataset) null;
        return dataset.Rev == revRequired ? (WorkItemRevisionDataset) dataset : dataset.Revisions[revRequired - 1];
      })).Where<WorkItemRevisionDataset>((System.Func<WorkItemRevisionDataset, bool>) (ds => ds != null))));
    }

    public virtual IEnumerable<KeyValuePair<int, int>> UpdateReconciledWorkItems(
      IEnumerable<WorkItemIdRevisionPair> workItemIdRevPairs,
      IEnumerable<WorkItemCustomFieldUpdateRecord> updateRecords,
      IVssIdentity callerIdentity,
      IVssIdentity changedByIdentity,
      int trendInterval,
      bool dualSave,
      bool continueWhenDateTimeShiftDetected = true,
      bool skipWITChangeDateUpdate = false)
    {
      throw new NotSupportedException();
    }

    protected virtual void BindUpdateReconciledWorkItemsChangedByColumn(
      IVssIdentity changedByIdentity)
    {
      throw new NotSupportedException();
    }

    protected virtual void BindIdentityColumn(IVssIdentity caller, string parameterName = "@userSID") => this.BindString(parameterName, caller.Descriptor.Identifier, 256, false, SqlDbType.NVarChar);

    public virtual IEnumerable<int> DestroyWorkItems(
      IVssIdentity userIdentity,
      IEnumerable<int> workItemIds,
      int batchSize = 200)
    {
      this.PrepareDynamicProcedure("dynprc_DestroyWorkItems", "\r\n                SET NOCOUNT ON\r\n                SET XACT_ABORT ON\r\n\r\n                DECLARE @userConstId INT\r\n                DECLARE @status INT\r\n                DECLARE @now DATETIME = GETUTCDATE()\r\n\r\n                -- Return if we cannot find the user in WIT database\r\n                EXEC @status = dbo.RebuildCallersViews @partitionId, @userConstId output, @userSid\r\n                IF @status <> 0 RETURN\r\n\r\n                BEGIN TRAN\r\n\r\n                EXEC @status = dbo.DestroyWorkItems @partitionId, @workItemIds, @userConstId, @now\r\n                IF @status <> 0\r\n                BEGIN\r\n                    ROLLBACK\r\n                    RETURN\r\n                END\r\n\r\n                COMMIT\r\n                ");
      this.BindIdentityColumn(userIdentity, "@userSid");
      this.BindInt32Table("@workItemIds", workItemIds);
      try
      {
        this.ExecuteNonQueryEx();
      }
      catch (SqlException ex)
      {
        SqlError error = ex.Errors[0];
        if (TeamFoundationServiceException.ExtractInt(error, "error") == 600304)
          throw new WorkItemDestroyException((IEnumerable<int>) ((IEnumerable<string>) TeamFoundationServiceException.ExtractString(error, "DestroyFailures").Split(new char[1]
          {
            ';'
          }, StringSplitOptions.RemoveEmptyEntries)).Select<string, int>((System.Func<string, int>) (id => int.Parse(id))).ToArray<int>());
        throw;
      }
      return Enumerable.Empty<int>();
    }

    protected virtual void BindMaxLongTextLength(int maxLongTextLength)
    {
    }

    protected virtual void BindMaxRevisionLongTextLength(int maxLongTextLength)
    {
    }

    public virtual IReadOnlyCollection<UriQueryResultEntry> GetWorkItemIdsForArtifactUris(
      IReadOnlyCollection<string> artifactUris,
      DateTime? asOfDate)
    {
      ArgumentUtility.CheckForNull<IReadOnlyCollection<string>>(artifactUris, nameof (artifactUris));
      this.PrepareStoredProcedure("prc_GetWorkItemIdsForArtifactUris", 3600);
      this.BindStringTable("@artifactUris", (IEnumerable<string>) artifactUris);
      this.BindNullableDateTime("@asOfDate", asOfDate);
      string[] array = artifactUris.ToArray<string>();
      IEnumerable<UriQueryResultRecord> queryResultRecords = this.ExecuteUnknown<IEnumerable<UriQueryResultRecord>>((System.Func<IDataReader, IEnumerable<UriQueryResultRecord>>) (reader => new WorkItemComponent.QueryUriResultBinder().BindAll(reader)));
      Dictionary<string, UriQueryResultEntry> dictionary = new Dictionary<string, UriQueryResultEntry>((IEqualityComparer<string>) StringComparer.InvariantCulture);
      foreach (UriQueryResultRecord queryResultRecord in queryResultRecords)
      {
        UriQueryResultEntry queryResultEntry1 = (UriQueryResultEntry) null;
        if (!dictionary.TryGetValue(queryResultRecord.Uri, out queryResultEntry1))
        {
          queryResultEntry1 = new UriQueryResultEntry();
          queryResultEntry1.Uri = queryResultRecord.Uri;
          queryResultEntry1.WorkItems = (IList<WorkItemQueryResultEntry>) new List<WorkItemQueryResultEntry>();
          dictionary.Add(queryResultRecord.Uri, queryResultEntry1);
        }
        int? nullable = queryResultRecord.WorkItemId;
        if (nullable.HasValue)
        {
          IList<WorkItemQueryResultEntry> workItems = queryResultEntry1.WorkItems;
          WorkItemQueryResultEntry queryResultEntry2 = new WorkItemQueryResultEntry();
          nullable = queryResultRecord.WorkItemId;
          queryResultEntry2.Id = nullable.Value;
          nullable = queryResultRecord.AreaId;
          queryResultEntry2.AreaId = nullable.Value;
          workItems.Add(queryResultEntry2);
        }
      }
      List<UriQueryResultEntry> idsForArtifactUris = new List<UriQueryResultEntry>();
      foreach (string key in array)
        idsForArtifactUris.Add(dictionary[key]);
      return (IReadOnlyCollection<UriQueryResultEntry>) idsForArtifactUris;
    }

    public virtual Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels.WorkItemComment GetWorkItemComment(
      int workItemId,
      int revision)
    {
      new List<int>() { workItemId };
      WorkItemTrackingRequestContext trackingRequestContext = this.RequestContext.WitContext();
      IEnumerable<WorkItemFieldValues> workItemFieldValues = this.GetWorkItemFieldValues(Enumerable.Repeat<WorkItemIdRevisionPair>(new WorkItemIdRevisionPair()
      {
        Id = workItemId,
        Revision = revision
      }, 1), (IEnumerable<int>) new int[3]{ 8, 9, -4 }, (IEnumerable<int>) new int[0], (IEnumerable<int>) new int[1]
      {
        54
      }, true, new DateTime?(), trackingRequestContext.ServerSettings.MaxLongTextSize, this.RequestContext.GetIdentityDisplayType(), false, out int _);
      if (workItemFieldValues == null)
        return (Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels.WorkItemComment) null;
      WorkItemFieldValues commentRevisionFields = ((IEnumerable<WorkItemFieldValues>) workItemFieldValues.Where<WorkItemFieldValues>((System.Func<WorkItemFieldValues, bool>) (r => r.Rev == revision && r.Fields.ContainsKey(54) && !string.IsNullOrEmpty((r.Fields[54] as WorkItemLargeTextValue).Text))).ToArray<WorkItemFieldValues>()).FirstOrDefault<WorkItemFieldValues>();
      return commentRevisionFields == null ? (Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels.WorkItemComment) null : WorkItemComponent.CreateWorkItemComment(workItemId, commentRevisionFields);
    }

    private static Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels.WorkItemComment CreateWorkItemComment(
      int workItemId,
      WorkItemFieldValues commentRevisionFields)
    {
      string text = ((WorkItemLargeTextValue) commentRevisionFields.Fields[54]).Text;
      int rev = commentRevisionFields.Rev;
      string field1 = commentRevisionFields.Fields[9] as string;
      Guid createdByTeamFoundationId = commentRevisionFields.IdentityFields.ContainsKey(9) ? commentRevisionFields.IdentityFields[9] : Guid.Empty;
      DateTime field2 = (DateTime) commentRevisionFields.Fields[-4];
      return new Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels.WorkItemComment(workItemId, text, rev, field1, field1, createdByTeamFoundationId, field2);
    }

    private static void SetWorkItemComment(
      bool enableCommentVersions,
      List<WorkItemCommentVersionRecord> workItemComments,
      WorkItemRevisionDataset dataset)
    {
      if (enableCommentVersions)
      {
        WorkItemCommentVersionRecord commentVersionRecord = workItemComments != null ? workItemComments.FirstOrDefault<WorkItemCommentVersionRecord>((System.Func<WorkItemCommentVersionRecord, bool>) (c =>
        {
          int commentId = c.CommentId;
          int? nullable = dataset.WorkItemCommentVersion?.CommentId;
          int valueOrDefault1 = nullable.GetValueOrDefault();
          if (!(commentId == valueOrDefault1 & nullable.HasValue))
            return false;
          int version = c.Version;
          nullable = dataset.WorkItemCommentVersion?.Version;
          int valueOrDefault2 = nullable.GetValueOrDefault();
          return version == valueOrDefault2 & nullable.HasValue;
        })) : (WorkItemCommentVersionRecord) null;
        if (commentVersionRecord == null)
          return;
        dataset.WorkItemCommentVersion = commentVersionRecord;
      }
      else
        dataset.WorkItemCommentVersion = (WorkItemCommentVersionRecord) null;
    }

    public virtual Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels.WorkItemComments GetWorkItemComments(
      int workItemId,
      int fromRevision,
      int count,
      CommentSortOrder sort)
    {
      List<int> workItemIds = new List<int>() { workItemId };
      WorkItemTrackingRequestContext trackingRequestContext = this.RequestContext.WitContext();
      WorkItemDataset workItemDataset = this.GetWorkItemDatasets((IEnumerable<int>) workItemIds, false, false, true, false, false, true, false, trackingRequestContext.ServerSettings.MaxLongTextSize, trackingRequestContext.ServerSettings.MaxRevisionLongTextSize, this.RequestContext.GetIdentityDisplayType(), new DateTime?(), new DateTime?(), false, false).FirstOrDefault<WorkItemDataset>();
      if (workItemDataset == null)
        return (Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels.WorkItemComments) null;
      List<WorkItemRevisionDataset> source = workItemDataset.Revisions != null ? workItemDataset.Revisions : new List<WorkItemRevisionDataset>();
      source.Add((WorkItemRevisionDataset) workItemDataset);
      WorkItemRevisionDataset[] array = source.Where<WorkItemRevisionDataset>((System.Func<WorkItemRevisionDataset, bool>) (rev => rev.Fields.ContainsKey(54) && !string.IsNullOrEmpty((rev.Fields[54] as WorkItemLargeTextValue).Text))).ToArray<WorkItemRevisionDataset>();
      if (sort == CommentSortOrder.Desc)
        array = ((IEnumerable<WorkItemRevisionDataset>) array).Reverse<WorkItemRevisionDataset>().ToArray<WorkItemRevisionDataset>();
      int index = Array.FindIndex<WorkItemRevisionDataset>(array, (Predicate<WorkItemRevisionDataset>) (d =>
      {
        if (sort == CommentSortOrder.Asc && d.Rev >= fromRevision)
          return true;
        return sort == CommentSortOrder.Desc && d.Rev <= fromRevision;
      }));
      if (index == -1)
        return new Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels.WorkItemComments(Enumerable.Empty<Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels.WorkItemComment>(), array.Length, 0);
      IEnumerable<WorkItemRevisionDataset> itemRevisionDatasets = ((IEnumerable<WorkItemRevisionDataset>) array).Skip<WorkItemRevisionDataset>(index).Take<WorkItemRevisionDataset>(count);
      List<Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels.WorkItemComment> comments = new List<Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels.WorkItemComment>();
      foreach (WorkItemRevisionDataset commentRevisionFields in itemRevisionDatasets)
      {
        Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels.WorkItemComment workItemComment = WorkItemComponent.CreateWorkItemComment(workItemId, (WorkItemFieldValues) commentRevisionFields);
        comments.Add(workItemComment);
      }
      return new Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels.WorkItemComments((IEnumerable<Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels.WorkItemComment>) comments, array.Length, array.Length - index);
    }

    public virtual IEnumerable<DestroyedWorkItemQueryResultEntry> GetDestroyedWorkItemIds(
      long rowVersion,
      int batchSize)
    {
      this.PrepareStoredProcedure(nameof (GetDestroyedWorkItemIds));
      this.BindLong("@rowVersion", rowVersion);
      return this.ExecuteUnknown<IEnumerable<DestroyedWorkItemQueryResultEntry>>((System.Func<IDataReader, IEnumerable<DestroyedWorkItemQueryResultEntry>>) (reader => new WorkItemComponent.DestroyedWorkItemQueryResultBinder().BindAll(reader)));
    }

    public virtual WorkItemUpdateResultSet ValidatePendingSetMembershipChecks(
      IEnumerable<PendingSetMembershipCheckRecord> pendingSetMembershipChecks)
    {
      throw new NotSupportedException();
    }

    public virtual string RemoveDeletedProcesses(Guid changedBy) => "RemoveDeletedProcesses job not executed";

    protected virtual WorkItemComponent.WorkItemDatasetBinder<WorkItemDataset> GetWorkItemDataSetBinder(
      bool bindTitle,
      bool bindCountFields,
      IdentityDisplayType identityDisplayType)
    {
      return new WorkItemComponent.WorkItemDatasetBinder<WorkItemDataset>(bindTitle, bindCountFields, this.GetIdentityDisplayType(identityDisplayType), new System.Func<int, Guid>(((TeamFoundationSqlResourceComponent) this).GetDataspaceIdentifier));
    }

    protected virtual WorkItemComponent.WorkItemCustomFieldValueBinder GetWorkItemCustomFieldValueBinder(
      IFieldTypeDictionary fieldsDictionary,
      IdentityDisplayType identityDisplayType)
    {
      return new WorkItemComponent.WorkItemCustomFieldValueBinder(fieldsDictionary, this.GetIdentityDisplayType(identityDisplayType));
    }

    protected virtual WorkItemComponent.WorkItemLinkBinder GetWorkItemLinkBinder(
      IdentityDisplayType identityDisplayType = IdentityDisplayType.ComboDisplayName)
    {
      return new WorkItemComponent.WorkItemLinkBinder(this.GetIdentityDisplayType(identityDisplayType));
    }

    protected virtual WorkItemComponent.WorkItemFieldValuesBinder<WorkItemFieldValues> GetWorkItemFieldValuesBinder(
      IEnumerable<int> wideTableFields,
      IdentityDisplayType identityDisplayType,
      bool disableProjectionLevelThree)
    {
      return new WorkItemComponent.WorkItemFieldValuesBinder<WorkItemFieldValues>(wideTableFields, this.GetIdentityDisplayType(identityDisplayType), new System.Func<int, Guid>(((TeamFoundationSqlResourceComponent) this).GetDataspaceIdentifier), disableProjectionLevelThree);
    }

    protected virtual IdentityDisplayType GetIdentityDisplayType(
      IdentityDisplayType identityDisplayType)
    {
      return this.RequestContext.IsClientOm() ? IdentityDisplayType.DisplayName : identityDisplayType;
    }

    public virtual IEnumerable<WorkItemLinkUpdateResultRecord> GetPendingWorkItemRemoteLink() => throw new NotImplementedException();

    public virtual void UpdatePendingWorkItemRemoteLink(
      IEnumerable<WorkItemLinkUpdateRecord> updates)
    {
      throw new NotImplementedException();
    }

    public virtual (int workItemCount, int revisionCount) BackfillCommentCount(int batchSize) => (0, 0);

    public virtual void ResetCommentCount(int batchSize)
    {
    }

    public virtual void ProvisionSystemField(
      string refName,
      string localizedFieldName,
      int fieldId,
      int type,
      int reportingType,
      out string duplicateFieldRefName)
    {
      duplicateFieldRefName = (string) null;
    }

    public virtual IEnumerable<DeletedProjectWithRemoteLink> GetDeletedProjectsWithRemoteLink() => throw new NotImplementedException();

    public virtual void DeleteRemoteLinksWhoseRemoteProjectDeleted(
      IEnumerable<(Guid RemoteHostId, Guid RemoteProjectId)> remoteProjects,
      Guid teamFoundationId)
    {
      throw new NotImplementedException();
    }

    public virtual void CreateOrUpdateWorkItemReactionsAggregateCount(
      int workItemId,
      SocialEngagementType socialEngagementType,
      int incrementCounterValue)
    {
      throw new NotImplementedException();
    }

    public virtual IList<WorkItemReactionsCount> GetSortedWorkItemReactionsCount(
      IEnumerable<int> workItemIds,
      SocialEngagementType socialEngagementType)
    {
      throw new NotImplementedException();
    }

    public virtual IList<WorkItemCommentUpdateRecord> AddWorkItemComments(
      Guid modifiedBy,
      DateTime modifiedDate,
      IEnumerable<WorkItemCommentUpdateRecord> comments,
      List<WorkItemResourceLinkUpdateRecord> linkUpdateRecords,
      IEnumerable<WorkItemMentionRecord> mentions,
      bool updateLegacyText = false)
    {
      throw new NotImplementedException();
    }

    public virtual IList<WorkItemCommentUpdateRecord> UpdateWorkItemComments(
      Guid modifiedBy,
      DateTime modifiedDate,
      IEnumerable<WorkItemCommentUpdateRecord> comments,
      List<WorkItemResourceLinkUpdateRecord> linkUpdateRecords,
      IEnumerable<WorkItemMentionRecord> mentions,
      bool updateLegacyText = false,
      bool updateMentions = false)
    {
      throw new NotImplementedException();
    }

    public virtual List<WorkItemDependencyGraph> LoadDependencyGraphForWorkItem(int workItemId) => new List<WorkItemDependencyGraph>();

    public virtual List<WorkItemDependencyGraph> LoadDependencyGraphForWorkItems(int[] workItemIds) => new List<WorkItemDependencyGraph>();

    public virtual List<int> GetWorkItemViolations(List<int> workItemIds) => new List<int>();

    public virtual void AddPendingWorkItem(int workItemId)
    {
    }

    public virtual void UpdatePendingWorkItemsForViolations()
    {
    }

    public virtual void BackupDependencyViolationsTable(DateTime backupDate)
    {
    }

    public virtual List<WorkItemDependencyInformation> GetDependencyInformationForWorkItems(
      List<int> workItemIds)
    {
      return new List<WorkItemDependencyInformation>();
    }

    public virtual void AddPendingWorkItems(List<int> workItemIds)
    {
    }

    public virtual int GetWorkItemUpdateId(int workItemId) => throw new NotImplementedException();

    private static class ProjectionLevels
    {
      public const int MinimalProjectionLevel = 0;
      public const int BasicProjectionLevel = 1;
      public const int ExtendedProjectionLevel = 2;
      public const int OldCountProjectionLevel = 3;
      public const int ParentProjectionLevel = 3;
      public const int NewCountProjectionLevel = 4;
    }

    protected class WorkItemFieldValuesBinder<TWorkItemFieldValues> : 
      WorkItemTrackingObjectBinder<TWorkItemFieldValues>
      where TWorkItemFieldValues : WorkItemFieldValues, new()
    {
      private static readonly HashSet<int> c_projectionFields_Level_0 = new HashSet<int>()
      {
        -3,
        -2,
        8,
        3,
        -5,
        -33
      };
      private static readonly HashSet<int> c_projectionFields_Level_1 = new HashSet<int>()
      {
        -104,
        25,
        2,
        22,
        24
      };
      private static readonly HashSet<int> c_projectionFields_Level_2 = new HashSet<int>()
      {
        32,
        33,
        -4,
        9,
        -1,
        -6,
        7
      };
      private static readonly HashSet<int> c_linkCountProjectionFields_Level = new HashSet<int>()
      {
        -31,
        -32,
        75,
        -34,
        -57
      };
      private SqlColumnBinder Id = new SqlColumnBinder("System.Id");
      private SqlColumnBinder Rev = new SqlColumnBinder("System.Rev");
      private SqlColumnBinder WorkItemType = new SqlColumnBinder("System.WorkItemType");
      private SqlColumnBinder AuthorizedDate = new SqlColumnBinder("System.AuthorizedDate");
      private SqlColumnBinder RevisedDate = new SqlColumnBinder("System.RevisedDate");
      private SqlColumnBinder AuthorizedAs = new SqlColumnBinder("System.AuthorizedAs");
      private SqlColumnBinder PersonId = new SqlColumnBinder("System.PersonId");
      private SqlColumnBinder CreatedDate = new SqlColumnBinder("System.CreatedDate");
      private SqlColumnBinder CreatedBy = new SqlColumnBinder("System.CreatedBy");
      private SqlColumnBinder ChangedDate = new SqlColumnBinder("System.ChangedDate");
      private SqlColumnBinder ChangedBy = new SqlColumnBinder("System.ChangedBy");
      private SqlColumnBinder AreaId = new SqlColumnBinder("System.AreaId");
      private SqlColumnBinder IterationId = new SqlColumnBinder("System.IterationId");
      private SqlColumnBinder AssignedTo = new SqlColumnBinder("System.AssignedTo");
      private SqlColumnBinder State = new SqlColumnBinder("System.State");
      private SqlColumnBinder Reason = new SqlColumnBinder("System.Reason");
      private SqlColumnBinder Watermark = new SqlColumnBinder("System.Watermark");
      private SqlColumnBinder AttachedFileCount = new SqlColumnBinder("System.AttachedFileCount");
      private SqlColumnBinder HyperlinkCount = new SqlColumnBinder("System.HyperlinkCount");
      private SqlColumnBinder ExternalLinkCount = new SqlColumnBinder("System.ExternalLinkCount");
      private SqlColumnBinder RelatedLinkCount = new SqlColumnBinder("System.RelatedLinkCount");
      private SqlColumnBinder Title = new SqlColumnBinder("System.Title");
      private bool? m_hasTitleColumn;
      private bool m_disableProjectionLevelThree;
      protected int m_projectionLevel;
      protected bool m_bindTitle;
      protected IdentityDisplayType m_identityDisplayType;
      protected System.Func<int, Guid> m_projectResolver;

      protected WorkItemFieldValuesBinder(
        int projectionLevel,
        bool bindTitle,
        IdentityDisplayType identityDisplayType,
        System.Func<int, Guid> projectResolver)
      {
        this.m_projectionLevel = projectionLevel;
        this.m_bindTitle = bindTitle;
        this.m_identityDisplayType = identityDisplayType;
        this.m_projectResolver = projectResolver;
      }

      public WorkItemFieldValuesBinder(
        IEnumerable<int> wideTableFields,
        IdentityDisplayType identityDisplayType,
        System.Func<int, Guid> projectResolver,
        bool disableProjectionLevelThree)
      {
        this.m_projectionLevel = this.CalculateProjectionLevel(wideTableFields);
        this.m_identityDisplayType = identityDisplayType;
        this.m_projectResolver = projectResolver;
        this.m_disableProjectionLevelThree = disableProjectionLevelThree;
      }

      public int ProjectionLevel => this.m_projectionLevel;

      public override TWorkItemFieldValues Bind(IDataReader reader)
      {
        TWorkItemFieldValues workItemFieldValues = new TWorkItemFieldValues();
        workItemFieldValues.Fields[-3] = (object) this.Id.GetInt32(reader);
        workItemFieldValues.Fields[-2] = (object) this.AreaId.GetInt32(reader);
        workItemFieldValues.Fields[8] = (object) this.Rev.GetInt32(reader);
        workItemFieldValues.Fields[3] = (object) this.AuthorizedDate.GetDateTime(reader);
        workItemFieldValues.Fields[-5] = (object) this.RevisedDate.GetDateTime(reader);
        if (this.m_projectionLevel >= 1)
        {
          workItemFieldValues.Fields[-104] = (object) this.IterationId.GetInt32(reader);
          workItemFieldValues.Fields[25] = (object) this.WorkItemType.GetString(reader, true);
          workItemFieldValues.Fields[2] = (object) this.State.GetString(reader, true);
          workItemFieldValues.Fields[22] = (object) this.Reason.GetString(reader, true);
          workItemFieldValues.Fields[24] = this.m_identityDisplayType == IdentityDisplayType.DisplayName ? (object) IdentityHelper.GetDisplayNameFromDistinctDisplayName(this.AssignedTo.GetString(reader, true)) : (object) this.AssignedTo.GetString(reader, true);
        }
        if (this.m_projectionLevel >= 2)
        {
          workItemFieldValues.Fields[32] = (object) this.CreatedDate.GetDateTime(reader);
          workItemFieldValues.Fields[33] = this.m_identityDisplayType == IdentityDisplayType.DisplayName ? (object) IdentityHelper.GetDisplayNameFromDistinctDisplayName(this.CreatedBy.GetString(reader, true)) : (object) this.CreatedBy.GetString(reader, true);
          workItemFieldValues.Fields[-4] = (object) this.ChangedDate.GetDateTime(reader);
          workItemFieldValues.Fields[9] = this.m_identityDisplayType == IdentityDisplayType.DisplayName ? (object) IdentityHelper.GetDisplayNameFromDistinctDisplayName(this.ChangedBy.GetString(reader, true)) : (object) this.ChangedBy.GetString(reader, true);
          workItemFieldValues.Fields[-1] = this.m_identityDisplayType == IdentityDisplayType.DisplayName ? (object) IdentityHelper.GetDisplayNameFromDistinctDisplayName(this.AuthorizedAs.GetString(reader, true)) : (object) this.AuthorizedAs.GetString(reader, true);
          workItemFieldValues.Fields[-6] = (object) this.PersonId.GetInt32(reader);
          workItemFieldValues.Fields[7] = (object) (this.Watermark.ColumnExists(reader) ? this.Watermark.GetInt32(reader) : 0);
        }
        if (this.m_projectionLevel >= this.CountFieldsProjectionLevel)
        {
          if (this.m_disableProjectionLevelThree)
          {
            workItemFieldValues.Fields[-31] = (object) 0;
            workItemFieldValues.Fields[-32] = (object) 0;
            workItemFieldValues.Fields[-57] = (object) 0;
            workItemFieldValues.Fields[75] = (object) 0;
          }
          else
          {
            workItemFieldValues.Fields[-31] = (object) this.AttachedFileCount.GetInt32(reader);
            workItemFieldValues.Fields[-32] = (object) this.HyperlinkCount.GetInt32(reader);
            workItemFieldValues.Fields[-57] = (object) this.ExternalLinkCount.GetInt32(reader);
            workItemFieldValues.Fields[75] = (object) this.RelatedLinkCount.GetInt32(reader);
          }
        }
        if (this.m_bindTitle)
        {
          if (!this.m_hasTitleColumn.HasValue)
            this.m_hasTitleColumn = new bool?(this.Title.ColumnExists(reader));
          if (this.m_hasTitleColumn.Value)
            workItemFieldValues.Fields[1] = (object) this.Title.GetString(reader, true);
        }
        return workItemFieldValues;
      }

      protected virtual int CountFieldsProjectionLevel => 3;

      protected virtual int CalculateProjectionLevel(IEnumerable<int> wideFields)
      {
        int projectionLevel = 0;
        foreach (int wideField in wideFields)
        {
          if (WorkItemComponent.WorkItemFieldValuesBinder<TWorkItemFieldValues>.c_linkCountProjectionFields_Level.Contains(wideField))
          {
            projectionLevel = this.CountFieldsProjectionLevel;
            break;
          }
          if (projectionLevel < 2 && WorkItemComponent.WorkItemFieldValuesBinder<TWorkItemFieldValues>.c_projectionFields_Level_2.Contains(wideField))
            projectionLevel = 2;
          else if (projectionLevel < 1 && WorkItemComponent.WorkItemFieldValuesBinder<TWorkItemFieldValues>.c_projectionFields_Level_1.Contains(wideField))
            projectionLevel = 1;
        }
        return projectionLevel;
      }
    }

    protected class WorkItemDatasetBinder<TWorkItemFieldValues> : 
      WorkItemComponent.WorkItemFieldValuesBinder<TWorkItemFieldValues>
      where TWorkItemFieldValues : WorkItemFieldValues, new()
    {
      public WorkItemDatasetBinder(
        bool bindTitle,
        bool bindCountFields,
        IdentityDisplayType identityDisplayType,
        System.Func<int, Guid> projectResolver)
        : base(bindCountFields ? 3 : 2, bindTitle, identityDisplayType, projectResolver)
      {
      }
    }

    protected class WorkItemCommentBinder : WorkItemTrackingObjectBinder<Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels.WorkItemComment>
    {
      private SqlColumnBinder Id = new SqlColumnBinder("System.Id");
      private SqlColumnBinder Text = new SqlColumnBinder(nameof (Text));
      private SqlColumnBinder Rev = new SqlColumnBinder("System.Rev");
      private SqlColumnBinder IdentityDisplayName = new SqlColumnBinder("System.CreatedBy_IdentityDisplayName");
      private SqlColumnBinder IdentityDisplayPart = new SqlColumnBinder("System.CreatedBy_IdentityDisplayPart");
      private SqlColumnBinder IdentityTeamFoundationId = new SqlColumnBinder("System.CreatedBy_IdentityTeamFoundationId");
      private SqlColumnBinder ChangedDate = new SqlColumnBinder("System.ChangedDate");

      public override Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels.WorkItemComment Bind(
        IDataReader reader)
      {
        int int32_1 = this.Id.GetInt32(reader);
        string str1 = this.Text.GetString(reader, true);
        int int32_2 = this.Rev.GetInt32(reader);
        string str2 = this.IdentityDisplayName.GetString(reader, true);
        string str3 = this.IdentityDisplayPart.GetString(reader, true);
        Guid guid = this.IdentityTeamFoundationId.GetGuid(reader, true);
        DateTime dateTime = this.ChangedDate.GetDateTime(reader);
        string text = str1;
        int revisionId = int32_2;
        string createdByDisplayName = str2;
        string createdByDisplayPart = str3;
        Guid createdByTeamFoundationId = guid;
        DateTime revisionDate = dateTime;
        return new Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels.WorkItemComment(int32_1, text, revisionId, createdByDisplayName, createdByDisplayPart, createdByTeamFoundationId, revisionDate);
      }
    }

    protected class WorkItemCustomFieldValueBinder : 
      WorkItemTrackingObjectBinder<WorkItemCustomFieldValue>
    {
      private SqlColumnBinder Id = new SqlColumnBinder(nameof (Id));
      private SqlColumnBinder FieldId = new SqlColumnBinder(nameof (FieldId));
      private SqlColumnBinder AuthorizedDate = new SqlColumnBinder(nameof (AuthorizedDate));
      private SqlColumnBinder RevisedDate = new SqlColumnBinder(nameof (RevisedDate));
      private SqlColumnBinder IntValue = new SqlColumnBinder(nameof (IntValue));
      private SqlColumnBinder FloatValue = new SqlColumnBinder(nameof (FloatValue));
      private SqlColumnBinder DateTimeValue = new SqlColumnBinder(nameof (DateTimeValue));
      private SqlColumnBinder GuidValue = new SqlColumnBinder(nameof (GuidValue));
      private SqlColumnBinder BitValue = new SqlColumnBinder(nameof (BitValue));
      protected SqlColumnBinder StringValue = new SqlColumnBinder(nameof (StringValue));
      private SqlColumnBinder TextValue = new SqlColumnBinder(nameof (TextValue));
      private SqlColumnBinder FieldType = new SqlColumnBinder(nameof (FieldType));
      protected bool? m_hasTextValueColumn;
      protected IdentityDisplayType m_identityDisplayType;
      protected IFieldTypeDictionary m_fieldsDictionary;

      public WorkItemCustomFieldValueBinder(
        IFieldTypeDictionary fieldsDictionary,
        IdentityDisplayType identityDisplayType)
      {
        this.m_fieldsDictionary = fieldsDictionary;
        this.m_identityDisplayType = identityDisplayType;
      }

      public override WorkItemCustomFieldValue Bind(IDataReader reader)
      {
        WorkItemCustomFieldValue customFieldValue = new WorkItemCustomFieldValue()
        {
          Id = this.Id.GetInt32(reader),
          FieldId = this.FieldId.GetInt32(reader),
          FieldType = this.FieldType.GetInt32(reader, 0, 0),
          AuthorizedDate = this.AuthorizedDate.GetDateTime(reader),
          RevisedDate = this.RevisedDate.GetDateTime(reader)
        };
        string str = this.StringValue.GetString(reader, true);
        if (str == null)
        {
          if (!this.m_hasTextValueColumn.HasValue)
            this.m_hasTextValueColumn = new bool?(this.TextValue.ColumnExists(reader));
          if (this.m_hasTextValueColumn.Value)
            str = this.TextValue.GetString(reader, true);
          if (str == null)
          {
            if (!this.IntValue.IsNull(reader))
              customFieldValue.IntValue = new int?(this.IntValue.GetInt32(reader));
            else if (!this.DateTimeValue.IsNull(reader))
              customFieldValue.DateTimeValue = new DateTime?(this.DateTimeValue.GetDateTime(reader));
            else if (!this.FloatValue.IsNull(reader))
              customFieldValue.FloatValue = new double?(this.FloatValue.GetDouble(reader));
            else if (!this.GuidValue.IsNull(reader))
              customFieldValue.GuidValue = new Guid?(this.GuidValue.GetGuid(reader));
            else if (!this.BitValue.IsNull(reader))
              customFieldValue.BitValue = new bool?(this.BitValue.GetBoolean(reader));
          }
          else
            customFieldValue.StringValue = str;
        }
        else
          customFieldValue.StringValue = str;
        return customFieldValue;
      }
    }

    protected class WorkItemTextFieldValueBinder : 
      WorkItemTrackingObjectBinder<WorkItemCustomFieldValue>
    {
      private SqlColumnBinder Id = new SqlColumnBinder(nameof (Id));
      private SqlColumnBinder FieldId = new SqlColumnBinder(nameof (FieldId));
      private SqlColumnBinder AuthorizedDate = new SqlColumnBinder(nameof (AuthorizedDate));
      private SqlColumnBinder RevisedDate = new SqlColumnBinder(nameof (RevisedDate));
      private SqlColumnBinder Text = new SqlColumnBinder(nameof (Text));
      private SqlColumnBinder IsHtml = new SqlColumnBinder(nameof (IsHtml));

      public override WorkItemCustomFieldValue Bind(IDataReader reader)
      {
        WorkItemCustomFieldValue.WorkItemLargeTextCustomFieldValue customFieldValue = new WorkItemCustomFieldValue.WorkItemLargeTextCustomFieldValue();
        customFieldValue.Id = this.Id.GetInt32(reader);
        customFieldValue.FieldId = this.FieldId.GetInt32(reader);
        customFieldValue.AuthorizedDate = this.AuthorizedDate.GetDateTime(reader);
        customFieldValue.RevisedDate = this.RevisedDate.GetDateTime(reader);
        customFieldValue.StringValue = this.Text.GetString(reader, true);
        customFieldValue.IsHtml = this.IsHtml.GetBoolean(reader);
        return (WorkItemCustomFieldValue) customFieldValue;
      }
    }

    protected class WorkItemResourceLinkBinder : 
      WorkItemTrackingObjectBinder<WorkItemResourceLinkInfo>
    {
      private SqlColumnBinder SourceId = new SqlColumnBinder(nameof (SourceId));
      private SqlColumnBinder ResourceType = new SqlColumnBinder(nameof (ResourceType));
      private SqlColumnBinder ResourceId = new SqlColumnBinder(nameof (ResourceId));
      private SqlColumnBinder AuthorizedDate = new SqlColumnBinder(nameof (AuthorizedDate));
      private SqlColumnBinder RevisedDate = new SqlColumnBinder(nameof (RevisedDate));
      private SqlColumnBinder Location = new SqlColumnBinder(nameof (Location));
      private SqlColumnBinder Name = new SqlColumnBinder(nameof (Name));
      private SqlColumnBinder CreatedDate = new SqlColumnBinder(nameof (CreatedDate));
      private SqlColumnBinder ModifiedDate = new SqlColumnBinder(nameof (ModifiedDate));
      private SqlColumnBinder Length = new SqlColumnBinder(nameof (Length));
      private SqlColumnBinder Comment = new SqlColumnBinder(nameof (Comment));

      public override WorkItemResourceLinkInfo Bind(IDataReader reader) => new WorkItemResourceLinkInfo()
      {
        SourceId = this.SourceId.GetInt32(reader),
        ResourceType = (ResourceLinkType) this.ResourceType.GetInt32(reader),
        ResourceId = this.ResourceId.GetInt32(reader),
        AuthorizedDate = this.AuthorizedDate.GetDateTime(reader),
        RevisedDate = this.RevisedDate.GetDateTime(reader),
        Location = this.Location.GetString(reader, true),
        Name = this.Name.GetString(reader, true),
        ResourceCreatedDate = this.CreatedDate.GetDateTime(reader),
        ResourceModifiedDate = this.ModifiedDate.GetDateTime(reader),
        ResourceSize = this.Length.GetInt32(reader),
        Comment = this.Comment.GetString(reader, true)
      };
    }

    protected class WorkItemResourceLinkUpdateResultRecordBinder : 
      WorkItemTrackingObjectBinder<WorkItemResourceLinkUpdateResultRecord>
    {
      private SqlColumnBinder Status = new SqlColumnBinder(nameof (Status));
      private SqlColumnBinder Order = new SqlColumnBinder(nameof (Order));
      private SqlColumnBinder UpdateType = new SqlColumnBinder(nameof (UpdateType));
      private SqlColumnBinder SourceId = new SqlColumnBinder(nameof (SourceId));
      private SqlColumnBinder ResourceType = new SqlColumnBinder(nameof (ResourceType));
      private SqlColumnBinder ResourceId = new SqlColumnBinder(nameof (ResourceId));
      private SqlColumnBinder Location = new SqlColumnBinder(nameof (Location));
      private SqlColumnBinder Name = new SqlColumnBinder(nameof (Name));
      private SqlColumnBinder Comment = new SqlColumnBinder(nameof (Comment));

      public override WorkItemResourceLinkUpdateResultRecord Bind(IDataReader reader) => new WorkItemResourceLinkUpdateResultRecord()
      {
        Order = this.Order.GetInt32(reader),
        UpdateType = (LinkUpdateType) this.UpdateType.GetByte(reader),
        Status = this.Status.GetInt32(reader),
        SourceId = this.SourceId.GetInt32(reader),
        ResourceType = this.ResourceType.GetInt32(reader, -1),
        ResourceId = this.ResourceId.GetInt32(reader, -1),
        Location = this.Location.GetString(reader, true),
        Name = this.Name.GetString(reader, (string) null),
        Comment = this.Comment.GetString(reader, (string) null)
      };
    }

    protected class WorkItemLinkBinder : WorkItemTrackingObjectBinder<WorkItemLinkInfo>
    {
      private SqlColumnBinder SourceId = new SqlColumnBinder(nameof (SourceId));
      private SqlColumnBinder TargetId = new SqlColumnBinder(nameof (TargetId));
      private SqlColumnBinder LinkType = new SqlColumnBinder(nameof (LinkType));
      private SqlColumnBinder AuthorizedDate = new SqlColumnBinder(nameof (AuthorizedDate));
      private SqlColumnBinder RevisedDate = new SqlColumnBinder(nameof (RevisedDate));
      private SqlColumnBinder AuthorizedBy = new SqlColumnBinder(nameof (AuthorizedBy));
      private SqlColumnBinder AuthorizedById = new SqlColumnBinder(nameof (AuthorizedById));
      private SqlColumnBinder RevisedBy = new SqlColumnBinder(nameof (RevisedBy));
      private SqlColumnBinder RevisedById = new SqlColumnBinder(nameof (RevisedById));
      private SqlColumnBinder Comment = new SqlColumnBinder(nameof (Comment));
      private SqlColumnBinder Locked = new SqlColumnBinder(nameof (Locked));
      private SqlColumnBinder TargetProjectId = new SqlColumnBinder(nameof (TargetProjectId));
      protected IdentityDisplayType identityDisplayType;

      public WorkItemLinkBinder(IdentityDisplayType identityDisplayType) => this.identityDisplayType = identityDisplayType;

      public override WorkItemLinkInfo Bind(IDataReader reader) => new WorkItemLinkInfo()
      {
        SourceId = this.SourceId.GetInt32(reader),
        TargetId = this.TargetId.GetInt32(reader),
        LinkType = (int) this.LinkType.GetInt16(reader),
        AuthorizedDate = this.AuthorizedDate.GetDateTime(reader),
        RevisedDate = this.RevisedDate.GetDateTime(reader),
        AuthorizedBy = this.identityDisplayType == IdentityDisplayType.DisplayName ? IdentityHelper.GetDisplayNameFromDistinctDisplayName(this.AuthorizedBy.GetString(reader, true)) : this.AuthorizedBy.GetString(reader, true),
        AuthorizedById = this.AuthorizedById.GetInt32(reader),
        RevisedBy = this.identityDisplayType == IdentityDisplayType.DisplayName ? IdentityHelper.GetDisplayNameFromDistinctDisplayName(this.RevisedBy.GetString(reader, true)) : this.RevisedBy.GetString(reader, true),
        RevisedById = this.RevisedById.GetInt32(reader),
        Comment = this.Comment.GetString(reader, true),
        IsLocked = this.Locked.GetBoolean(reader),
        TargetProjectId = this.TargetProjectId.GetGuid(reader, true, Guid.Empty)
      };
    }

    protected class WorkItemRemoteLinkRecordBinder : 
      WorkItemTrackingObjectBinder<WorkItemLinkUpdateResultRecord>
    {
      private SqlColumnBinder SourceId = new SqlColumnBinder(nameof (SourceId));
      private SqlColumnBinder TargetId = new SqlColumnBinder(nameof (TargetId));
      private SqlColumnBinder DataspaceId = new SqlColumnBinder(nameof (DataspaceId));
      private SqlColumnBinder TargetDataspaceId = new SqlColumnBinder(nameof (TargetDataspaceId));
      private SqlColumnBinder LinkType = new SqlColumnBinder(nameof (LinkType));
      private SqlColumnBinder Timestamp = new SqlColumnBinder(nameof (Timestamp));
      private SqlColumnBinder AuthorizedByTeamFoundationId = new SqlColumnBinder("AuthorizedBy_TeamFoundationId");
      private SqlColumnBinder Comment = new SqlColumnBinder(nameof (Comment));
      private SqlColumnBinder RemoteHostId = new SqlColumnBinder(nameof (RemoteHostId));
      private SqlColumnBinder RemoteProjectId = new SqlColumnBinder(nameof (RemoteProjectId));
      private SqlColumnBinder RemoteStatus = new SqlColumnBinder(nameof (RemoteStatus));
      private SqlColumnBinder RemoteStatusMessage = new SqlColumnBinder(nameof (RemoteStatusMessage));
      private SqlColumnBinder RemoteWatermark = new SqlColumnBinder(nameof (RemoteWatermark));
      protected System.Func<int, Guid> m_projectResolver;

      public WorkItemRemoteLinkRecordBinder(System.Func<int, Guid> projectResolver) => this.m_projectResolver = projectResolver;

      public override WorkItemLinkUpdateResultRecord Bind(IDataReader reader)
      {
        int int32_1 = this.SourceId.GetInt32(reader);
        int int32_2 = this.TargetId.GetInt32(reader);
        int int32_3 = this.DataspaceId.GetInt32(reader);
        int int32_4 = this.TargetDataspaceId.GetInt32(reader);
        short int16 = this.LinkType.GetInt16(reader);
        long int64 = this.Timestamp.GetInt64(reader);
        Guid guid1 = this.AuthorizedByTeamFoundationId.GetGuid(reader, true);
        string str1 = this.Comment.GetString(reader, true);
        Guid guid2 = this.RemoteHostId.GetGuid(reader, true);
        Guid guid3 = this.RemoteProjectId.GetGuid(reader, true);
        byte? nullableByte = this.RemoteStatus.GetNullableByte(reader);
        RemoteStatus? nullable = nullableByte.HasValue ? new RemoteStatus?((RemoteStatus) nullableByte.GetValueOrDefault()) : new RemoteStatus?();
        string str2 = this.RemoteStatusMessage.GetString(reader, true);
        long? nullableInt64 = this.RemoteWatermark.GetNullableInt64(reader);
        Guid guid4 = this.m_projectResolver(int32_3 == 0 ? int32_4 : int32_3);
        return new WorkItemLinkUpdateResultRecord()
        {
          SourceId = int32_1,
          TargetId = int32_2,
          DataspaceId = int32_3,
          TargetDataspaceId = int32_4,
          LinkType = (int) int16,
          Timestamp = int64,
          AuthorizedByTfid = guid1,
          Comment = str1,
          RemoteHostId = new Guid?(guid2),
          RemoteProjectId = new Guid?(guid3),
          RemoteStatus = nullable,
          RemoteStatusMessage = str2,
          RemoteWatermark = nullableInt64,
          LocalProjectId = guid4
        };
      }
    }

    protected class WorkItemRemoteLinkRecordBinder2 : 
      WorkItemComponent.WorkItemRemoteLinkRecordBinder
    {
      private SqlColumnBinder AuthorizedDate = new SqlColumnBinder(nameof (AuthorizedDate));

      public WorkItemRemoteLinkRecordBinder2(System.Func<int, Guid> projectResolver)
        : base(projectResolver)
      {
      }

      public override WorkItemLinkUpdateResultRecord Bind(IDataReader reader) => base.Bind(reader) with
      {
        AuthorizedDate = this.AuthorizedDate.GetDateTime(reader)
      };
    }

    protected class QueryUriResultBinder : WorkItemTrackingObjectBinder<UriQueryResultRecord>
    {
      private SqlColumnBinder UriColumn = new SqlColumnBinder("Uri");
      private SqlColumnBinder WorkItemIdColumn = new SqlColumnBinder("WorkItemId");
      private SqlColumnBinder AreaIdColumn = new SqlColumnBinder("AreaId");

      public override UriQueryResultRecord Bind(IDataReader reader) => new UriQueryResultRecord()
      {
        Uri = this.UriColumn.GetString(reader, false),
        WorkItemId = this.WorkItemIdColumn.IsNull(reader) ? new int?() : new int?(this.WorkItemIdColumn.GetInt32(reader)),
        AreaId = this.AreaIdColumn.IsNull(reader) ? new int?() : new int?(this.AreaIdColumn.GetInt32(reader))
      };
    }

    protected class DestroyedWorkItemQueryResultBinder : 
      WorkItemTrackingObjectBinder<DestroyedWorkItemQueryResultEntry>
    {
      private SqlColumnBinder WorkItemIdColumn = new SqlColumnBinder("ID");
      private SqlColumnBinder RowVersionColumn = new SqlColumnBinder("RowVersion");

      public override DestroyedWorkItemQueryResultEntry Bind(IDataReader reader) => new DestroyedWorkItemQueryResultEntry()
      {
        WorkItemId = this.WorkItemIdColumn.GetInt32(reader),
        RowVersion = this.RowVersionColumn.GetInt64(reader)
      };
    }

    protected class WorkItemFieldValuesBinder2<TWorkItemFieldValues> : 
      WorkItemComponent.WorkItemFieldValuesBinder<TWorkItemFieldValues>
      where TWorkItemFieldValues : WorkItemFieldValues, new()
    {
      private WorkItemComponent.IWorkItemFieldValueDatasetBinder<TWorkItemFieldValues> workItemFieldValueDatasetBinder = (WorkItemComponent.IWorkItemFieldValueDatasetBinder<TWorkItemFieldValues>) new WorkItemComponent.WorkItemFieldValueDatasetBinder<TWorkItemFieldValues>();

      protected WorkItemFieldValuesBinder2(
        int projectionLevel,
        bool bindTitle,
        IdentityDisplayType identityDisplayType,
        System.Func<int, Guid> projectResolver)
        : base(projectionLevel, bindTitle, identityDisplayType, projectResolver)
      {
      }

      public WorkItemFieldValuesBinder2(
        IEnumerable<int> wideTableFields,
        IdentityDisplayType identityDisplayType,
        System.Func<int, Guid> projectResolver,
        bool disableProjectionLevelThree)
        : base(wideTableFields, identityDisplayType, projectResolver, disableProjectionLevelThree)
      {
      }

      public override TWorkItemFieldValues Bind(IDataReader reader)
      {
        TWorkItemFieldValues values = base.Bind(reader);
        return this.workItemFieldValueDatasetBinder.Bind(reader, values, this.m_projectionLevel, this.m_identityDisplayType, this.m_projectResolver);
      }
    }

    protected class WorkItemFieldValuesBinder3<TWorkItemFieldValues> : 
      WorkItemComponent.WorkItemFieldValuesBinder2<TWorkItemFieldValues>
      where TWorkItemFieldValues : WorkItemFieldValues, new()
    {
      private WorkItemComponent.IWorkItemFieldValueDatasetBinder<TWorkItemFieldValues> workItemFieldValueDatasetBinder = (WorkItemComponent.IWorkItemFieldValueDatasetBinder<TWorkItemFieldValues>) new WorkItemComponent.WorkItemFieldValueDatasetBinder2<TWorkItemFieldValues>();

      protected WorkItemFieldValuesBinder3(
        int projectionLevel,
        bool bindTitle,
        IdentityDisplayType identityDisplayType,
        System.Func<int, Guid> projectResolver)
        : base(projectionLevel, bindTitle, identityDisplayType, projectResolver)
      {
      }

      public WorkItemFieldValuesBinder3(
        IEnumerable<int> wideTableFields,
        IdentityDisplayType identityDisplayType,
        System.Func<int, Guid> projectResolver,
        bool disableProjectionLevelThree)
        : base(wideTableFields, identityDisplayType, projectResolver, disableProjectionLevelThree)
      {
      }

      public override TWorkItemFieldValues Bind(IDataReader reader)
      {
        TWorkItemFieldValues values = base.Bind(reader);
        return this.workItemFieldValueDatasetBinder.Bind(reader, values, this.m_projectionLevel, this.m_identityDisplayType, this.m_projectResolver);
      }
    }

    protected class WorkItemFieldValuesBinder4<TWorkItemFieldValues> : 
      WorkItemComponent.WorkItemFieldValuesBinder3<TWorkItemFieldValues>
      where TWorkItemFieldValues : WorkItemFieldValues, new()
    {
      private WorkItemComponent.IWorkItemFieldValueDatasetBinder<TWorkItemFieldValues> workItemFieldValueDatasetBinder = (WorkItemComponent.IWorkItemFieldValueDatasetBinder<TWorkItemFieldValues>) new WorkItemComponent.WorkItemFieldValueDatasetBinder3<TWorkItemFieldValues>();

      protected WorkItemFieldValuesBinder4(
        int projectionLevel,
        bool bindTitle,
        IdentityDisplayType identityDisplayType,
        System.Func<int, Guid> projectResolver)
        : base(projectionLevel, bindTitle, identityDisplayType, projectResolver)
      {
      }

      public WorkItemFieldValuesBinder4(
        IEnumerable<int> wideTableFields,
        IdentityDisplayType identityDisplayType,
        System.Func<int, Guid> projectResolver,
        bool disableProjectionLevelThree)
        : base(wideTableFields, identityDisplayType, projectResolver, disableProjectionLevelThree)
      {
      }

      public override TWorkItemFieldValues Bind(IDataReader reader)
      {
        TWorkItemFieldValues values = base.Bind(reader);
        return this.workItemFieldValueDatasetBinder.Bind(reader, values, this.m_projectionLevel, this.m_identityDisplayType, this.m_projectResolver);
      }
    }

    protected class WorkItemFieldValuesBinder5<TWorkItemFieldValues> : 
      WorkItemComponent.WorkItemFieldValuesBinder4<TWorkItemFieldValues>
      where TWorkItemFieldValues : WorkItemFieldValues, new()
    {
      private WorkItemComponent.IWorkItemFieldValueDatasetBinder<TWorkItemFieldValues> workItemFieldValueDatasetBinder = (WorkItemComponent.IWorkItemFieldValueDatasetBinder<TWorkItemFieldValues>) new WorkItemComponent.WorkItemFieldValueDatasetBinder4<TWorkItemFieldValues>();

      protected WorkItemFieldValuesBinder5(
        int projectionLevel,
        bool bindTitle,
        IdentityDisplayType identityDisplayType,
        System.Func<int, Guid> projectResolver)
        : base(projectionLevel, bindTitle, identityDisplayType, projectResolver)
      {
      }

      public WorkItemFieldValuesBinder5(
        IEnumerable<int> wideTableFields,
        IdentityDisplayType identityDisplayType,
        System.Func<int, Guid> projectResolver,
        bool disableProjectionLevelThree)
        : base(wideTableFields, identityDisplayType, projectResolver, disableProjectionLevelThree)
      {
      }

      public override TWorkItemFieldValues Bind(IDataReader reader)
      {
        TWorkItemFieldValues values = base.Bind(reader);
        return this.workItemFieldValueDatasetBinder.Bind(reader, values, this.m_projectionLevel, this.m_identityDisplayType, this.m_projectResolver);
      }
    }

    protected class WorkItemFieldValuesBinder6<TWorkItemFieldValues> : 
      WorkItemComponent.WorkItemFieldValuesBinder5<TWorkItemFieldValues>
      where TWorkItemFieldValues : WorkItemFieldValues, new()
    {
      private WorkItemComponent.IWorkItemFieldValueDatasetBinder<TWorkItemFieldValues> workItemFieldValueDatasetBinder;

      protected WorkItemFieldValuesBinder6(
        int projectionLevel,
        bool bindTitle,
        IdentityDisplayType identityDisplayType,
        System.Func<int, Guid> projectResolver,
        IFieldTypeDictionary fieldsDictionary)
        : base(projectionLevel, bindTitle, identityDisplayType, projectResolver)
      {
        this.workItemFieldValueDatasetBinder = (WorkItemComponent.IWorkItemFieldValueDatasetBinder<TWorkItemFieldValues>) new WorkItemComponent.WorkItemFieldValueDatasetBinder5<TWorkItemFieldValues>(fieldsDictionary);
      }

      public WorkItemFieldValuesBinder6(
        IEnumerable<int> wideTableFields,
        IdentityDisplayType identityDisplayType,
        System.Func<int, Guid> projectResolver,
        bool disableProjectionLevelThree,
        IFieldTypeDictionary fieldsDictionary)
        : base(wideTableFields, identityDisplayType, projectResolver, disableProjectionLevelThree)
      {
        this.workItemFieldValueDatasetBinder = (WorkItemComponent.IWorkItemFieldValueDatasetBinder<TWorkItemFieldValues>) new WorkItemComponent.WorkItemFieldValueDatasetBinder5<TWorkItemFieldValues>(fieldsDictionary);
      }

      public override TWorkItemFieldValues Bind(IDataReader reader)
      {
        TWorkItemFieldValues values = base.Bind(reader);
        return this.workItemFieldValueDatasetBinder.Bind(reader, values, this.m_projectionLevel, this.m_identityDisplayType, this.m_projectResolver);
      }
    }

    protected class WorkItemFieldValuesBinder7<TWorkItemFieldValues> : 
      WorkItemComponent.WorkItemFieldValuesBinder6<TWorkItemFieldValues>
      where TWorkItemFieldValues : WorkItemFieldValues, new()
    {
      private WorkItemComponent.IWorkItemFieldValueDatasetBinder<TWorkItemFieldValues> workItemFieldValueDatasetBinder;

      protected WorkItemFieldValuesBinder7(
        int projectionLevel,
        bool bindTitle,
        IdentityDisplayType identityDisplayType,
        System.Func<int, Guid> projectResolver,
        IFieldTypeDictionary fieldsDictionary)
        : base(projectionLevel, bindTitle, identityDisplayType, projectResolver, fieldsDictionary)
      {
        this.workItemFieldValueDatasetBinder = (WorkItemComponent.IWorkItemFieldValueDatasetBinder<TWorkItemFieldValues>) new WorkItemComponent.WorkItemFieldValueDatasetBinder6<TWorkItemFieldValues>(fieldsDictionary, this.CountFieldsProjectionLevel);
      }

      public WorkItemFieldValuesBinder7(
        IEnumerable<int> wideTableFields,
        IdentityDisplayType identityDisplayType,
        System.Func<int, Guid> projectResolver,
        bool disableProjectionLevelThree,
        IFieldTypeDictionary fieldsDictionary)
        : base(wideTableFields, identityDisplayType, projectResolver, disableProjectionLevelThree, fieldsDictionary)
      {
        this.workItemFieldValueDatasetBinder = (WorkItemComponent.IWorkItemFieldValueDatasetBinder<TWorkItemFieldValues>) new WorkItemComponent.WorkItemFieldValueDatasetBinder6<TWorkItemFieldValues>(fieldsDictionary, this.CountFieldsProjectionLevel, disableProjectionLevelThree);
      }

      public override TWorkItemFieldValues Bind(IDataReader reader)
      {
        TWorkItemFieldValues values = base.Bind(reader);
        return this.workItemFieldValueDatasetBinder.Bind(reader, values, this.m_projectionLevel, this.m_identityDisplayType, this.m_projectResolver);
      }
    }

    protected class WorkItemFieldValuesBinder8<TWorkItemFieldValues> : 
      WorkItemComponent.WorkItemFieldValuesBinder7<TWorkItemFieldValues>
      where TWorkItemFieldValues : WorkItemFieldValues, new()
    {
      private WorkItemComponent.IWorkItemFieldValueDatasetBinder<TWorkItemFieldValues> workItemFieldValueDatasetBinder;

      protected WorkItemFieldValuesBinder8(
        int projectionLevel,
        bool bindTitle,
        IdentityDisplayType identityDisplayType,
        System.Func<int, Guid> projectResolver,
        IFieldTypeDictionary fieldsDictionary)
        : base(projectionLevel, bindTitle, identityDisplayType, projectResolver, fieldsDictionary)
      {
        this.workItemFieldValueDatasetBinder = (WorkItemComponent.IWorkItemFieldValueDatasetBinder<TWorkItemFieldValues>) new WorkItemComponent.WorkItemFieldValueDatasetBinder7<TWorkItemFieldValues>(fieldsDictionary, this.CountFieldsProjectionLevel);
      }

      public WorkItemFieldValuesBinder8(
        IEnumerable<int> wideTableFields,
        IdentityDisplayType identityDisplayType,
        System.Func<int, Guid> projectResolver,
        bool disableProjectionLevelThree,
        IFieldTypeDictionary fieldsDictionary)
        : base(wideTableFields, identityDisplayType, projectResolver, disableProjectionLevelThree, fieldsDictionary)
      {
        this.workItemFieldValueDatasetBinder = (WorkItemComponent.IWorkItemFieldValueDatasetBinder<TWorkItemFieldValues>) new WorkItemComponent.WorkItemFieldValueDatasetBinder7<TWorkItemFieldValues>(fieldsDictionary, this.CountFieldsProjectionLevel, disableProjectionLevelThree);
      }

      public override TWorkItemFieldValues Bind(IDataReader reader)
      {
        TWorkItemFieldValues values = base.Bind(reader);
        return this.workItemFieldValueDatasetBinder.Bind(reader, values, this.m_projectionLevel, this.m_identityDisplayType, this.m_projectResolver);
      }
    }

    protected class WorkItemFieldValuesBinder9<TWorkItemFieldValues> : 
      WorkItemComponent.WorkItemFieldValuesBinder8<TWorkItemFieldValues>
      where TWorkItemFieldValues : WorkItemFieldValues, new()
    {
      private WorkItemComponent.IWorkItemFieldValueDatasetBinder<TWorkItemFieldValues> workItemFieldValueDatasetBinder;

      protected WorkItemFieldValuesBinder9(
        int projectionLevel,
        bool bindTitle,
        IdentityDisplayType identityDisplayType,
        System.Func<int, Guid> projectResolver,
        IFieldTypeDictionary fieldsDictionary)
        : base(projectionLevel, bindTitle, identityDisplayType, projectResolver, fieldsDictionary)
      {
        this.workItemFieldValueDatasetBinder = (WorkItemComponent.IWorkItemFieldValueDatasetBinder<TWorkItemFieldValues>) new WorkItemComponent.WorkItemFieldValueDatasetBinder8<TWorkItemFieldValues>(fieldsDictionary, this.CountFieldsProjectionLevel);
      }

      public WorkItemFieldValuesBinder9(
        IEnumerable<int> wideTableFields,
        IdentityDisplayType identityDisplayType,
        System.Func<int, Guid> projectResolver,
        bool disableProjectionLevelThree,
        IFieldTypeDictionary fieldsDictionary)
        : base(wideTableFields, identityDisplayType, projectResolver, disableProjectionLevelThree, fieldsDictionary)
      {
        this.workItemFieldValueDatasetBinder = (WorkItemComponent.IWorkItemFieldValueDatasetBinder<TWorkItemFieldValues>) new WorkItemComponent.WorkItemFieldValueDatasetBinder8<TWorkItemFieldValues>(fieldsDictionary, this.CountFieldsProjectionLevel, disableProjectionLevelThree);
      }

      public override TWorkItemFieldValues Bind(IDataReader reader)
      {
        TWorkItemFieldValues values = base.Bind(reader);
        return this.workItemFieldValueDatasetBinder.Bind(reader, values, this.m_projectionLevel, this.m_identityDisplayType, this.m_projectResolver);
      }
    }

    protected class WorkItemFieldValuesBinder10<TWorkItemFieldValues> : 
      WorkItemComponent.WorkItemFieldValuesBinder9<TWorkItemFieldValues>
      where TWorkItemFieldValues : WorkItemFieldValues, new()
    {
      private WorkItemComponent.IWorkItemFieldValueDatasetBinder<TWorkItemFieldValues> workItemFieldValueDatasetBinder;

      protected WorkItemFieldValuesBinder10(
        int projectionLevel,
        bool bindTitle,
        IdentityDisplayType identityDisplayType,
        System.Func<int, Guid> projectResolver,
        IFieldTypeDictionary fieldsDictionary)
        : base(projectionLevel, bindTitle, identityDisplayType, projectResolver, fieldsDictionary)
      {
        this.workItemFieldValueDatasetBinder = (WorkItemComponent.IWorkItemFieldValueDatasetBinder<TWorkItemFieldValues>) new WorkItemComponent.WorkItemFieldValueDatasetBinder9<TWorkItemFieldValues>(fieldsDictionary);
      }

      public WorkItemFieldValuesBinder10(
        IEnumerable<int> wideTableFields,
        IdentityDisplayType identityDisplayType,
        System.Func<int, Guid> projectResolver,
        bool disableProjectionLevelThree,
        IFieldTypeDictionary fieldsDictionary)
        : base(wideTableFields, identityDisplayType, projectResolver, disableProjectionLevelThree, fieldsDictionary)
      {
        this.workItemFieldValueDatasetBinder = (WorkItemComponent.IWorkItemFieldValueDatasetBinder<TWorkItemFieldValues>) new WorkItemComponent.WorkItemFieldValueDatasetBinder9<TWorkItemFieldValues>(fieldsDictionary, disableProjectionLevelThree);
      }

      public override TWorkItemFieldValues Bind(IDataReader reader)
      {
        TWorkItemFieldValues values = base.Bind(reader);
        return this.workItemFieldValueDatasetBinder.Bind(reader, values, this.m_projectionLevel, this.m_identityDisplayType, this.m_projectResolver);
      }

      protected override int CountFieldsProjectionLevel => 4;

      protected override int CalculateProjectionLevel(IEnumerable<int> wideFields)
      {
        int projectionLevel = base.CalculateProjectionLevel(wideFields);
        if (projectionLevel <= 2 && wideFields.Any<int>((System.Func<int, bool>) (wideFieldId => wideFieldId == -35)))
          projectionLevel = 3;
        return projectionLevel;
      }
    }

    protected class WorkItemDatasetBinder2<TWorkItemFieldValues> : 
      WorkItemComponent.WorkItemDatasetBinder<TWorkItemFieldValues>
      where TWorkItemFieldValues : WorkItemFieldValues, new()
    {
      private WorkItemComponent.IWorkItemFieldValueDatasetBinder<TWorkItemFieldValues> workItemFieldValueDatasetBinder = (WorkItemComponent.IWorkItemFieldValueDatasetBinder<TWorkItemFieldValues>) new WorkItemComponent.WorkItemFieldValueDatasetBinder<TWorkItemFieldValues>();

      public WorkItemDatasetBinder2(
        bool bindTitle,
        bool bindCountFields,
        IdentityDisplayType identityDisplayType,
        System.Func<int, Guid> projectResolver)
        : base(bindTitle, bindCountFields, identityDisplayType, projectResolver)
      {
      }

      public override TWorkItemFieldValues Bind(IDataReader reader)
      {
        TWorkItemFieldValues values = base.Bind(reader);
        return this.workItemFieldValueDatasetBinder.Bind(reader, values, this.m_projectionLevel, this.m_identityDisplayType, this.m_projectResolver);
      }
    }

    protected class WorkItemDatasetBinder3<TWorkItemFieldValues> : 
      WorkItemComponent.WorkItemDatasetBinder2<TWorkItemFieldValues>
      where TWorkItemFieldValues : WorkItemFieldValues, new()
    {
      private WorkItemComponent.IWorkItemFieldValueDatasetBinder<TWorkItemFieldValues> workItemFieldValueDatasetBinder = (WorkItemComponent.IWorkItemFieldValueDatasetBinder<TWorkItemFieldValues>) new WorkItemComponent.WorkItemFieldValueDatasetBinder2<TWorkItemFieldValues>();

      public WorkItemDatasetBinder3(
        bool bindTitle,
        bool bindCountFields,
        IdentityDisplayType identityDisplayType,
        System.Func<int, Guid> projectResolver)
        : base(bindTitle, bindCountFields, identityDisplayType, projectResolver)
      {
      }

      public override TWorkItemFieldValues Bind(IDataReader reader)
      {
        TWorkItemFieldValues values = base.Bind(reader);
        return this.workItemFieldValueDatasetBinder.Bind(reader, values, this.m_projectionLevel, this.m_identityDisplayType, this.m_projectResolver);
      }
    }

    protected class WorkItemDatasetBinder4<TWorkItemFieldValues> : 
      WorkItemComponent.WorkItemDatasetBinder3<TWorkItemFieldValues>
      where TWorkItemFieldValues : WorkItemFieldValues, new()
    {
      private WorkItemComponent.IWorkItemFieldValueDatasetBinder<TWorkItemFieldValues> workItemFieldValueDatasetBinder = (WorkItemComponent.IWorkItemFieldValueDatasetBinder<TWorkItemFieldValues>) new WorkItemComponent.WorkItemFieldValueDatasetBinder3<TWorkItemFieldValues>();

      public WorkItemDatasetBinder4(
        bool bindTitle,
        bool bindCountFields,
        IdentityDisplayType identityDisplayType,
        System.Func<int, Guid> projectResolver)
        : base(bindTitle, bindCountFields, identityDisplayType, projectResolver)
      {
      }

      public override TWorkItemFieldValues Bind(IDataReader reader)
      {
        TWorkItemFieldValues values = base.Bind(reader);
        return this.workItemFieldValueDatasetBinder.Bind(reader, values, this.m_projectionLevel, this.m_identityDisplayType, this.m_projectResolver);
      }
    }

    protected class WorkItemDatasetBinder5<TWorkItemFieldValues> : 
      WorkItemComponent.WorkItemDatasetBinder4<TWorkItemFieldValues>
      where TWorkItemFieldValues : WorkItemFieldValues, new()
    {
      private WorkItemComponent.IWorkItemFieldValueDatasetBinder<TWorkItemFieldValues> workItemFieldValueDatasetBinder = (WorkItemComponent.IWorkItemFieldValueDatasetBinder<TWorkItemFieldValues>) new WorkItemComponent.WorkItemFieldValueDatasetBinder4<TWorkItemFieldValues>();

      public WorkItemDatasetBinder5(
        bool bindTitle,
        bool bindCountFields,
        IdentityDisplayType identityDisplayType,
        System.Func<int, Guid> projectResolver)
        : base(bindTitle, bindCountFields, identityDisplayType, projectResolver)
      {
      }

      public override TWorkItemFieldValues Bind(IDataReader reader)
      {
        TWorkItemFieldValues values = base.Bind(reader);
        return this.workItemFieldValueDatasetBinder.Bind(reader, values, this.m_projectionLevel, this.m_identityDisplayType, this.m_projectResolver);
      }
    }

    protected class WorkItemDatasetBinder6<TWorkItemFieldValues> : 
      WorkItemComponent.WorkItemDatasetBinder5<TWorkItemFieldValues>
      where TWorkItemFieldValues : WorkItemFieldValues, new()
    {
      private WorkItemComponent.IWorkItemFieldValueDatasetBinder<TWorkItemFieldValues> workItemFieldValueDatasetBinder;

      public WorkItemDatasetBinder6(
        bool bindTitle,
        bool bindCountFields,
        IdentityDisplayType identityDisplayType,
        System.Func<int, Guid> projectResolver,
        IFieldTypeDictionary fieldsDictionary)
        : base(bindTitle, bindCountFields, identityDisplayType, projectResolver)
      {
        this.workItemFieldValueDatasetBinder = (WorkItemComponent.IWorkItemFieldValueDatasetBinder<TWorkItemFieldValues>) new WorkItemComponent.WorkItemFieldValueDatasetBinder5<TWorkItemFieldValues>(fieldsDictionary);
      }

      public override TWorkItemFieldValues Bind(IDataReader reader)
      {
        TWorkItemFieldValues values = base.Bind(reader);
        return this.workItemFieldValueDatasetBinder.Bind(reader, values, this.m_projectionLevel, this.m_identityDisplayType, this.m_projectResolver);
      }
    }

    protected class WorkItemDatasetBinder7<TWorkItemFieldValues> : 
      WorkItemComponent.WorkItemDatasetBinder6<TWorkItemFieldValues>
      where TWorkItemFieldValues : WorkItemFieldValues, new()
    {
      private WorkItemComponent.IWorkItemFieldValueDatasetBinder<TWorkItemFieldValues> workItemFieldValueDatasetBinder;

      public WorkItemDatasetBinder7(
        bool bindTitle,
        bool bindCountFields,
        IdentityDisplayType identityDisplayType,
        System.Func<int, Guid> projectResolver,
        IFieldTypeDictionary fieldsDictionary)
        : base(bindTitle, bindCountFields, identityDisplayType, projectResolver, fieldsDictionary)
      {
        this.workItemFieldValueDatasetBinder = (WorkItemComponent.IWorkItemFieldValueDatasetBinder<TWorkItemFieldValues>) new WorkItemComponent.WorkItemFieldValueDatasetBinder6<TWorkItemFieldValues>(fieldsDictionary, this.CountFieldsProjectionLevel);
      }

      public override TWorkItemFieldValues Bind(IDataReader reader)
      {
        TWorkItemFieldValues values = base.Bind(reader);
        return this.workItemFieldValueDatasetBinder.Bind(reader, values, this.m_projectionLevel, this.m_identityDisplayType, this.m_projectResolver);
      }
    }

    protected class WorkItemDatasetBinder8<TWorkItemFieldValues> : 
      WorkItemComponent.WorkItemDatasetBinder7<TWorkItemFieldValues>
      where TWorkItemFieldValues : WorkItemFieldValues, new()
    {
      private WorkItemComponent.IWorkItemFieldValueDatasetBinder<TWorkItemFieldValues> workItemFieldValueDatasetBinder;

      public WorkItemDatasetBinder8(
        bool bindTitle,
        bool bindCountFields,
        IdentityDisplayType identityDisplayType,
        System.Func<int, Guid> projectResolver,
        IFieldTypeDictionary fieldsDictionary)
        : base(bindTitle, bindCountFields, identityDisplayType, projectResolver, fieldsDictionary)
      {
        this.workItemFieldValueDatasetBinder = (WorkItemComponent.IWorkItemFieldValueDatasetBinder<TWorkItemFieldValues>) new WorkItemComponent.WorkItemFieldValueDatasetBinder7<TWorkItemFieldValues>(fieldsDictionary, this.CountFieldsProjectionLevel);
      }

      public override TWorkItemFieldValues Bind(IDataReader reader)
      {
        TWorkItemFieldValues values = base.Bind(reader);
        return this.workItemFieldValueDatasetBinder.Bind(reader, values, this.m_projectionLevel, this.m_identityDisplayType, this.m_projectResolver);
      }
    }

    protected class WorkItemDatasetBinder9<TWorkItemFieldValues> : 
      WorkItemComponent.WorkItemDatasetBinder8<TWorkItemFieldValues>
      where TWorkItemFieldValues : WorkItemFieldValues, new()
    {
      private WorkItemComponent.IWorkItemFieldValueDatasetBinder<TWorkItemFieldValues> workItemFieldValueDatasetBinder;

      public WorkItemDatasetBinder9(
        bool bindTitle,
        bool bindCountFields,
        IdentityDisplayType identityDisplayType,
        System.Func<int, Guid> projectResolver,
        IFieldTypeDictionary fieldsDictionary)
        : base(bindTitle, bindCountFields, identityDisplayType, projectResolver, fieldsDictionary)
      {
        this.workItemFieldValueDatasetBinder = (WorkItemComponent.IWorkItemFieldValueDatasetBinder<TWorkItemFieldValues>) new WorkItemComponent.WorkItemFieldValueDatasetBinder9<TWorkItemFieldValues>(fieldsDictionary);
      }

      public override TWorkItemFieldValues Bind(IDataReader reader)
      {
        TWorkItemFieldValues values = base.Bind(reader);
        return this.workItemFieldValueDatasetBinder.Bind(reader, values, this.m_projectionLevel, this.m_identityDisplayType, this.m_projectResolver);
      }
    }

    internal interface IWorkItemFieldValueDatasetBinder<TWorkItemFieldValues> where TWorkItemFieldValues : WorkItemFieldValues, new()
    {
      TWorkItemFieldValues Bind(
        IDataReader reader,
        TWorkItemFieldValues values,
        int projectionLevel,
        IdentityDisplayType identityDisplayType,
        System.Func<int, Guid> projectResolver);
    }

    protected class WorkItemFieldValueDatasetBinder<TWorkItemFieldValues> : 
      WorkItemComponent.IWorkItemFieldValueDatasetBinder<TWorkItemFieldValues>
      where TWorkItemFieldValues : WorkItemFieldValues, new()
    {
      private SqlColumnBinder AssignedToIdentityDisplayName = new SqlColumnBinder("System.AssignedTo_IdentityDisplayName");
      private SqlColumnBinder AssignedToHasUniqueIdentityDisplayName = new SqlColumnBinder("System.AssignedTo_HasUniqueIdentityDisplayName");
      private SqlColumnBinder CreatedByIdentityDisplayName = new SqlColumnBinder("System.CreatedBy_IdentityDisplayName");
      private SqlColumnBinder CreatedByHasUniqueIdentityDisplayName = new SqlColumnBinder("System.CreatedBy_HasUniqueIdentityDisplayName");
      private SqlColumnBinder ChangedByIdentityDisplayName = new SqlColumnBinder("System.ChangedBy_IdentityDisplayName");
      private SqlColumnBinder ChangedByHasUniqueIdentityDisplayName = new SqlColumnBinder("System.ChangedBy_HasUniqueIdentityDisplayName");
      private SqlColumnBinder AuthorizedAsIdentityDisplayName = new SqlColumnBinder("System.AuthorizedAs_IdentityDisplayName");
      private SqlColumnBinder AuthorizedAsHasUniqueIdentityDisplayName = new SqlColumnBinder("System.AuthorizedAs_HasUniqueIdentityDisplayName");
      private SqlColumnBinder ChangedBy = new SqlColumnBinder("System.ChangedBy");
      private SqlColumnBinder AssignedTo = new SqlColumnBinder("System.AssignedTo");
      private SqlColumnBinder AuthorizedAs = new SqlColumnBinder("System.AuthorizedAs");
      private SqlColumnBinder CreatedBy = new SqlColumnBinder("System.CreatedBy");

      public TWorkItemFieldValues Bind(
        IDataReader reader,
        TWorkItemFieldValues values,
        int projectionLevel,
        IdentityDisplayType identityDisplayType,
        System.Func<int, Guid> projectResolver)
      {
        if (projectionLevel >= 1)
          values.Fields[24] = (object) WorkItemComponent.GetIdentityDisplayName(reader, ref this.AssignedTo, ref this.AssignedToIdentityDisplayName, ref this.AssignedToHasUniqueIdentityDisplayName, identityDisplayType);
        if (projectionLevel >= 2)
        {
          values.Fields[33] = (object) WorkItemComponent.GetIdentityDisplayName(reader, ref this.CreatedBy, ref this.CreatedByIdentityDisplayName, ref this.CreatedByHasUniqueIdentityDisplayName, identityDisplayType);
          values.Fields[9] = (object) WorkItemComponent.GetIdentityDisplayName(reader, ref this.ChangedBy, ref this.ChangedByIdentityDisplayName, ref this.ChangedByHasUniqueIdentityDisplayName, identityDisplayType);
          values.Fields[-1] = (object) WorkItemComponent.GetIdentityDisplayName(reader, ref this.AuthorizedAs, ref this.AuthorizedAsIdentityDisplayName, ref this.AuthorizedAsHasUniqueIdentityDisplayName, identityDisplayType);
        }
        return values;
      }
    }

    protected class WorkItemFieldValueDatasetBinder2<TWorkItemFieldValues> : 
      WorkItemComponent.IWorkItemFieldValueDatasetBinder<TWorkItemFieldValues>
      where TWorkItemFieldValues : WorkItemFieldValues, new()
    {
      private SqlColumnBinder AssignedToTeamFoundationId = new SqlColumnBinder("System.AssignedTo_TeamFoundationId");
      private SqlColumnBinder CreatedByTeamFoundationId = new SqlColumnBinder("System.CreatedBy_TeamFoundationId");
      private SqlColumnBinder ChangedByTeamFoundationId = new SqlColumnBinder("System.ChangedBy_TeamFoundationId");
      private SqlColumnBinder AuthorizedAsTeamFoundationId = new SqlColumnBinder("System.AuthorizedAs_TeamFoundationId");

      public virtual TWorkItemFieldValues Bind(
        IDataReader reader,
        TWorkItemFieldValues values,
        int projectionLevel,
        IdentityDisplayType identityDisplayType,
        System.Func<int, Guid> projectResolver)
      {
        if (projectionLevel >= 1)
          values.IdentityFields[24] = this.AssignedToTeamFoundationId.GetGuid(reader, true);
        if (projectionLevel >= 2)
        {
          values.IdentityFields[33] = this.CreatedByTeamFoundationId.GetGuid(reader, true);
          values.IdentityFields[9] = this.ChangedByTeamFoundationId.GetGuid(reader, true);
          values.IdentityFields[-1] = this.AuthorizedAsTeamFoundationId.GetGuid(reader, true);
        }
        return values;
      }
    }

    protected class WorkItemFieldValueDatasetBinder3<TWorkItemFieldValues> : 
      WorkItemComponent.WorkItemFieldValueDatasetBinder2<TWorkItemFieldValues>
      where TWorkItemFieldValues : WorkItemFieldValues, new()
    {
      private SqlColumnBinder IsDeleted = new SqlColumnBinder("System.IsDeleted");

      public override TWorkItemFieldValues Bind(
        IDataReader reader,
        TWorkItemFieldValues values,
        int projectionLevel,
        IdentityDisplayType identityDisplayType,
        System.Func<int, Guid> projectResolver)
      {
        base.Bind(reader, values, projectionLevel, identityDisplayType, projectResolver);
        values.Fields[-404] = (object) this.IsDeleted.GetBoolean(reader);
        return values;
      }
    }

    protected class WorkItemFieldValueDatasetBinder4<TWorkItemFieldValues> : 
      WorkItemComponent.WorkItemFieldValueDatasetBinder3<TWorkItemFieldValues>
      where TWorkItemFieldValues : WorkItemFieldValues, new()
    {
      private SqlColumnBinder DataspaceId = new SqlColumnBinder(nameof (DataspaceId));

      public override TWorkItemFieldValues Bind(
        IDataReader reader,
        TWorkItemFieldValues values,
        int projectionLevel,
        IdentityDisplayType identityDisplayType,
        System.Func<int, Guid> projectResolver)
      {
        base.Bind(reader, values, projectionLevel, identityDisplayType, projectResolver);
        values.ProjectId = projectResolver(this.DataspaceId.GetInt32(reader));
        return values;
      }
    }

    protected class WorkItemFieldValueDatasetBinder5<TWorkItemFieldValues> : 
      WorkItemComponent.WorkItemFieldValueDatasetBinder4<TWorkItemFieldValues>
      where TWorkItemFieldValues : WorkItemFieldValues, new()
    {
      private SqlColumnBinder CommentCount = new SqlColumnBinder("System.CommentCount");
      protected IFieldTypeDictionary fieldsDictionary;

      public WorkItemFieldValueDatasetBinder5(IFieldTypeDictionary fieldsDictionary) => this.fieldsDictionary = fieldsDictionary;

      public override TWorkItemFieldValues Bind(
        IDataReader reader,
        TWorkItemFieldValues values,
        int projectionLevel,
        IdentityDisplayType identityDisplayType,
        System.Func<int, Guid> projectResolver)
      {
        base.Bind(reader, values, projectionLevel, identityDisplayType, projectResolver);
        FieldEntry field = (FieldEntry) null;
        if (this.fieldsDictionary != null && this.fieldsDictionary.TryGetField(-33, out field) && field != null)
          values.Fields[-33] = (object) this.CommentCount.GetNullableInt32(reader).GetValueOrDefault();
        return values;
      }
    }

    protected class WorkItemFieldValueDatasetBinder6<TWorkItemFieldValues> : 
      WorkItemComponent.WorkItemFieldValueDatasetBinder5<TWorkItemFieldValues>
      where TWorkItemFieldValues : WorkItemFieldValues, new()
    {
      private SqlColumnBinder RemoteLinkCount = new SqlColumnBinder("System.RemoteLinkCount");
      protected bool m_disableProjectionLevelThree;
      protected int m_countProjectionLevel;

      public WorkItemFieldValueDatasetBinder6(
        IFieldTypeDictionary fieldsDictionary,
        int countProjectionLevel,
        bool disableProjectionLevelThree = false)
        : base(fieldsDictionary)
      {
        this.m_disableProjectionLevelThree = disableProjectionLevelThree;
        this.m_countProjectionLevel = countProjectionLevel;
      }

      public override TWorkItemFieldValues Bind(
        IDataReader reader,
        TWorkItemFieldValues values,
        int projectionLevel,
        IdentityDisplayType identityDisplayType,
        System.Func<int, Guid> projectResolver)
      {
        base.Bind(reader, values, projectionLevel, identityDisplayType, projectResolver);
        if (this.fieldsDictionary != null && this.fieldsDictionary.TryGetField(-34, out FieldEntry _) && projectionLevel >= this.m_countProjectionLevel)
        {
          if (this.m_disableProjectionLevelThree)
            values.Fields[-34] = (object) 0;
          else
            values.Fields[-34] = (object) this.RemoteLinkCount.GetInt32(reader);
        }
        return values;
      }
    }

    protected class WorkItemFieldValueDatasetBinder7<TWorkItemFieldValues> : 
      WorkItemComponent.WorkItemFieldValueDatasetBinder6<TWorkItemFieldValues>
      where TWorkItemFieldValues : WorkItemFieldValues, new()
    {
      private SqlColumnBinder CommentId = new SqlColumnBinder(nameof (CommentId));
      private SqlColumnBinder CommentVersion = new SqlColumnBinder(nameof (CommentVersion));

      public WorkItemFieldValueDatasetBinder7(
        IFieldTypeDictionary fieldsDictionary,
        int countProjectionLevel,
        bool disableProjectionLevelThree = false)
        : base(fieldsDictionary, countProjectionLevel, disableProjectionLevelThree)
      {
      }

      public override TWorkItemFieldValues Bind(
        IDataReader reader,
        TWorkItemFieldValues values,
        int projectionLevel,
        IdentityDisplayType identityDisplayType,
        System.Func<int, Guid> projectResolver)
      {
        base.Bind(reader, values, projectionLevel, identityDisplayType, projectResolver);
        int? nullableInt32_1 = this.CommentId.GetNullableInt32(reader);
        int? nullableInt32_2 = this.CommentVersion.GetNullableInt32(reader);
        if (nullableInt32_1.HasValue && nullableInt32_2.HasValue)
          values.WorkItemCommentVersion = new WorkItemCommentVersionRecord()
          {
            WorkItemId = values.Id,
            CommentId = nullableInt32_1.Value,
            Version = nullableInt32_2.Value
          };
        return values;
      }
    }

    protected class WorkItemFieldValueDatasetBinder8<TWorkItemFieldValues> : 
      WorkItemComponent.WorkItemFieldValueDatasetBinder7<TWorkItemFieldValues>
      where TWorkItemFieldValues : WorkItemFieldValues, new()
    {
      private SqlColumnBinder CommentOriginalRev = new SqlColumnBinder(nameof (CommentOriginalRev));

      public WorkItemFieldValueDatasetBinder8(
        IFieldTypeDictionary fieldsDictionary,
        int countProjectionLevel,
        bool disableProjectionLevelThree = false)
        : base(fieldsDictionary, countProjectionLevel, disableProjectionLevelThree)
      {
      }

      public override TWorkItemFieldValues Bind(
        IDataReader reader,
        TWorkItemFieldValues values,
        int projectionLevel,
        IdentityDisplayType identityDisplayType,
        System.Func<int, Guid> projectResolver)
      {
        base.Bind(reader, values, projectionLevel, identityDisplayType, projectResolver);
        int? nullableInt32 = this.CommentOriginalRev.GetNullableInt32(reader);
        if (nullableInt32.HasValue && values.WorkItemCommentVersion != null)
          values.WorkItemCommentVersion.CommentOriginalRev = nullableInt32.Value;
        return values;
      }
    }

    protected class WorkItemFieldValueDatasetBinder9<TWorkItemFieldValues> : 
      WorkItemComponent.WorkItemFieldValueDatasetBinder8<TWorkItemFieldValues>
      where TWorkItemFieldValues : WorkItemFieldValues, new()
    {
      private SqlColumnBinder ParentColumn = new SqlColumnBinder("System.Parent");

      public WorkItemFieldValueDatasetBinder9(
        IFieldTypeDictionary fieldsDictionary,
        bool disableProjectionLevelThree = false)
        : base(fieldsDictionary, 4, disableProjectionLevelThree)
      {
      }

      public override TWorkItemFieldValues Bind(
        IDataReader reader,
        TWorkItemFieldValues values,
        int projectionLevel,
        IdentityDisplayType identityDisplayType,
        System.Func<int, Guid> projectResolver)
      {
        base.Bind(reader, values, projectionLevel, identityDisplayType, projectResolver);
        if (this.fieldsDictionary != null && this.fieldsDictionary.TryGetField(-35, out FieldEntry _) && projectionLevel >= 3)
        {
          if (this.m_disableProjectionLevelThree)
            values.Fields[-35] = (object) null;
          else
            values.Fields[-35] = (object) this.ParentColumn.GetNullableInt32(reader);
        }
        return values;
      }
    }

    protected class WorkItemCustomFieldValueBinder2 : 
      WorkItemComponent.WorkItemCustomFieldValueBinder
    {
      private SqlColumnBinder IdentityValue = new SqlColumnBinder(nameof (IdentityValue));
      private SqlColumnBinder IdentityValueHasUniqueIdentityDisplayName = new SqlColumnBinder("IdentityValue_HasUniqueIdentityDisplayName");

      public WorkItemCustomFieldValueBinder2(
        IFieldTypeDictionary fieldsDictionary,
        IdentityDisplayType identityDisplayType)
        : base(fieldsDictionary, identityDisplayType)
      {
      }

      public override WorkItemCustomFieldValue Bind(IDataReader reader)
      {
        WorkItemCustomFieldValue customFieldValue = base.Bind(reader);
        if (this.m_fieldsDictionary != null && customFieldValue.StringValue != null && this.m_fieldsDictionary.GetField(customFieldValue.FieldId).IsIdentity)
          customFieldValue.StringValue = WorkItemComponent.GetIdentityDisplayName(reader, ref this.StringValue, ref this.IdentityValue, ref this.IdentityValueHasUniqueIdentityDisplayName, this.m_identityDisplayType);
        return customFieldValue;
      }
    }

    protected class WorkItemCustomFieldValueBinder3 : 
      WorkItemComponent.WorkItemCustomFieldValueBinder
    {
      private SqlColumnBinder IdentityValueTeamFoundationId = new SqlColumnBinder("IdentityValue_TeamFoundationId");
      private SqlColumnBinder IsIdentityField = new SqlColumnBinder("IsIdentity");
      private SqlColumnBinder IdentityValue = new SqlColumnBinder(nameof (IdentityValue));
      private SqlColumnBinder IdentityValueHasUniqueIdentityDisplayName = new SqlColumnBinder("IdentityValue_HasUniqueIdentityDisplayName");

      public WorkItemCustomFieldValueBinder3(IdentityDisplayType identityDisplayType)
        : base((IFieldTypeDictionary) null, identityDisplayType)
      {
      }

      public override WorkItemCustomFieldValue Bind(IDataReader reader)
      {
        WorkItemCustomFieldValue customFieldValue = base.Bind(reader);
        customFieldValue.IsIdentityField = this.IsIdentityField.GetBoolean(reader, false);
        if (customFieldValue.StringValue != null && customFieldValue.IsIdentityField)
        {
          customFieldValue.StringValue = WorkItemComponent.GetIdentityDisplayName(reader, ref this.StringValue, ref this.IdentityValue, ref this.IdentityValueHasUniqueIdentityDisplayName, this.m_identityDisplayType);
          customFieldValue.GuidValue = new Guid?(this.IdentityValueTeamFoundationId.GetGuid(reader, true));
        }
        return customFieldValue;
      }
    }

    protected class WorkItemLinkBinder2 : WorkItemComponent.WorkItemLinkBinder
    {
      private SqlColumnBinder AuthorizedBy = new SqlColumnBinder(nameof (AuthorizedBy));
      private SqlColumnBinder AuthorizedByIdentityDisplayName = new SqlColumnBinder("AuthorizedBy_IdentityDisplayName");
      private SqlColumnBinder AuthorizedByHasUniqueIdentityDisplayName = new SqlColumnBinder("AuthorizedBy_HasUniqueIdentityDisplayName");
      private SqlColumnBinder RevisedBy = new SqlColumnBinder(nameof (RevisedBy));
      private SqlColumnBinder RevisedByIdentityDisplayName = new SqlColumnBinder("RevisedBy_IdentityDisplayName");
      private SqlColumnBinder RevisedByHasUniqueIdentityDisplayName = new SqlColumnBinder("RevisedBy_HasUniqueIdentityDisplayName");

      public WorkItemLinkBinder2(IdentityDisplayType identityDisplayType = IdentityDisplayType.ComboDisplayNameWhenNeeded)
        : base(identityDisplayType)
      {
      }

      public override WorkItemLinkInfo Bind(IDataReader reader)
      {
        WorkItemLinkInfo workItemLinkInfo = base.Bind(reader);
        workItemLinkInfo.AuthorizedBy = WorkItemComponent.GetIdentityDisplayName(reader, ref this.AuthorizedBy, ref this.AuthorizedByIdentityDisplayName, ref this.AuthorizedByHasUniqueIdentityDisplayName, this.identityDisplayType);
        workItemLinkInfo.RevisedBy = WorkItemComponent.GetIdentityDisplayName(reader, ref this.RevisedBy, ref this.RevisedByIdentityDisplayName, ref this.RevisedByHasUniqueIdentityDisplayName, this.identityDisplayType);
        return workItemLinkInfo;
      }
    }

    protected class WorkItemLinkBinder3 : WorkItemComponent.WorkItemLinkBinder2
    {
      private SqlColumnBinder AuthorizedByTeamFoundationId = new SqlColumnBinder("AuthorizedBy_TeamFoundationId");
      private SqlColumnBinder RevisedByTeamFoundationId = new SqlColumnBinder("RevisedBy_TeamFoundationId");

      public WorkItemLinkBinder3(IdentityDisplayType identityDisplayType = IdentityDisplayType.ComboDisplayNameWhenNeeded)
        : base(identityDisplayType)
      {
      }

      public override WorkItemLinkInfo Bind(IDataReader reader)
      {
        WorkItemLinkInfo workItemLinkInfo = base.Bind(reader);
        workItemLinkInfo.AuthorizedByTfid = this.AuthorizedByTeamFoundationId.GetGuid(reader, true);
        workItemLinkInfo.RevisedByTfid = this.RevisedByTeamFoundationId.GetGuid(reader, true);
        return workItemLinkInfo;
      }
    }

    protected class WorkItemLinkBinder4 : WorkItemComponent.WorkItemLinkBinder3
    {
      private SqlColumnBinder RemoteHostId = new SqlColumnBinder(nameof (RemoteHostId));
      private SqlColumnBinder RemoteProjectId = new SqlColumnBinder(nameof (RemoteProjectId));
      private SqlColumnBinder RemoteStatus = new SqlColumnBinder(nameof (RemoteStatus));
      private SqlColumnBinder RemoteStatusMessage = new SqlColumnBinder(nameof (RemoteStatusMessage));
      private SqlColumnBinder RemoteWatermark = new SqlColumnBinder(nameof (RemoteWatermark));

      public WorkItemLinkBinder4(IdentityDisplayType identityDisplayType = IdentityDisplayType.ComboDisplayNameWhenNeeded)
        : base(identityDisplayType)
      {
      }

      public override WorkItemLinkInfo Bind(IDataReader reader)
      {
        WorkItemLinkInfo workItemLinkInfo = base.Bind(reader);
        workItemLinkInfo.RemoteHostId = this.RemoteHostId.GetNullableGuid(reader);
        workItemLinkInfo.RemoteProjectId = this.RemoteProjectId.GetNullableGuid(reader);
        byte? nullableByte = this.RemoteStatus.GetNullableByte(reader);
        workItemLinkInfo.RemoteStatus = nullableByte.HasValue ? new RemoteStatus?((RemoteStatus) nullableByte.GetValueOrDefault()) : new RemoteStatus?();
        workItemLinkInfo.RemoteStatusMessage = this.RemoteStatusMessage.GetString(reader, true);
        workItemLinkInfo.RemoteWatermark = this.RemoteWatermark.GetNullableInt64(reader);
        return workItemLinkInfo;
      }
    }

    protected class WorkItemLinkBinder5 : WorkItemComponent.WorkItemLinkBinder4
    {
      private SqlColumnBinder TimeStamp = new SqlColumnBinder(nameof (TimeStamp));

      public WorkItemLinkBinder5(IdentityDisplayType identityDisplayType = IdentityDisplayType.ComboDisplayNameWhenNeeded)
        : base(identityDisplayType)
      {
      }

      public override WorkItemLinkInfo Bind(IDataReader reader)
      {
        WorkItemLinkInfo workItemLinkInfo = base.Bind(reader);
        workItemLinkInfo.TimeStamp = this.TimeStamp.GetInt64(reader);
        return workItemLinkInfo;
      }
    }

    protected class DeletedProjectWithRemoteLinkBinder : 
      WorkItemTrackingObjectBinder<DeletedProjectWithRemoteLink>
    {
      private SqlColumnBinder LocalProjectId = new SqlColumnBinder(nameof (LocalProjectId));
      private SqlColumnBinder RemoteHostId = new SqlColumnBinder(nameof (RemoteHostId));

      public override DeletedProjectWithRemoteLink Bind(IDataReader reader)
      {
        Guid guid1 = this.LocalProjectId.GetGuid(reader, false);
        Guid guid2 = this.RemoteHostId.GetGuid(reader, false);
        return new DeletedProjectWithRemoteLink()
        {
          LocalProjectId = guid1,
          RemoteHostId = guid2
        };
      }
    }

    protected class WorkItemReactionsCountBinder : 
      WorkItemTrackingObjectBinder<WorkItemReactionsCount>
    {
      private SqlColumnBinder WorkItemId = new SqlColumnBinder(nameof (WorkItemId));
      private SqlColumnBinder ReactionsCount = new SqlColumnBinder(nameof (ReactionsCount));

      public override WorkItemReactionsCount Bind(IDataReader reader)
      {
        int int32_1 = this.WorkItemId.GetInt32(reader);
        int int32_2 = this.ReactionsCount.GetInt32(reader);
        return new WorkItemReactionsCount()
        {
          WorkItemId = int32_1,
          TotalReactionsCount = int32_2
        };
      }
    }

    protected class WorkItemCommentUpdateRecordObjectBinder : 
      WorkItemTrackingObjectBinder<WorkItemCommentUpdateRecord>
    {
      private SqlColumnBinder artifactId = new SqlColumnBinder("ArtifactId");
      private SqlColumnBinder artifactRevision = new SqlColumnBinder("ArtifactRevision");
      private SqlColumnBinder watermark = new SqlColumnBinder("Watermark");
      private SqlColumnBinder artifactKind = new SqlColumnBinder("ArtifactKind");
      private SqlColumnBinder commentId = new SqlColumnBinder("CommentId");
      private SqlColumnBinder version = new SqlColumnBinder("Version");
      private SqlColumnBinder text = new SqlColumnBinder("Text");
      private SqlColumnBinder renderedText = new SqlColumnBinder("RenderedText");
      private SqlColumnBinder format = new SqlColumnBinder("Format");
      private SqlColumnBinder createdBy = new SqlColumnBinder("CreatedBy");
      private SqlColumnBinder createdDate = new SqlColumnBinder("CreatedDate");
      private SqlColumnBinder createdOnBehalfOf = new SqlColumnBinder("CreatedOnBehalfOf");
      private SqlColumnBinder createdOnBehalfDate = new SqlColumnBinder("CreatedOnBehalfDate");
      private SqlColumnBinder modifiedBy = new SqlColumnBinder("ModifiedBy");
      private SqlColumnBinder modifiedDate = new SqlColumnBinder("ModifiedDate");
      private SqlColumnBinder isDeleted = new SqlColumnBinder("IsDeleted");

      public override WorkItemCommentUpdateRecord Bind(IDataReader reader) => new WorkItemCommentUpdateRecord()
      {
        CommentId = this.commentId.GetInt32(reader),
        ArtifactKind = this.artifactKind.GetGuid(reader, false),
        ArtifactRevision = this.artifactRevision.GetInt32(reader),
        ArtifactId = this.artifactId.GetString(reader, false),
        Version = this.version.GetInt32(reader),
        Format = new WorkItemCommentFormat?((WorkItemCommentFormat) this.format.GetByte(reader)),
        Text = this.text.GetString(reader, false),
        RenderedText = this.renderedText.GetString(reader, true),
        CreatedBy = this.createdBy.GetGuid(reader, false),
        CreatedDate = this.createdDate.GetDateTime(reader),
        CreatedOnBehalfOf = this.createdOnBehalfOf.GetString(reader, false),
        CreatedOnBehalfDate = new DateTime?(this.createdOnBehalfDate.GetDateTime(reader)),
        ModifiedBy = this.modifiedBy.GetGuid(reader, false),
        ModifiedDate = this.modifiedDate.GetDateTime(reader),
        IsDeleted = this.isDeleted.GetBoolean(reader, false),
        Watermark = this.watermark.GetInt32(reader)
      };
    }

    protected class WorkItemMentionRecordObjectBinder : 
      WorkItemTrackingObjectBinder<WorkItemMentionRecord>
    {
      private SqlColumnBinder sourceId = new SqlColumnBinder("SourceId");
      private SqlColumnBinder sourceType = new SqlColumnBinder("SourceType");
      private SqlColumnBinder rawText = new SqlColumnBinder("RawText");
      private SqlColumnBinder artifactId = new SqlColumnBinder("ArtifactId");
      private SqlColumnBinder artifactType = new SqlColumnBinder("ArtifactType");
      private SqlColumnBinder commentId = new SqlColumnBinder("CommentId");
      private SqlColumnBinder mentionAction = new SqlColumnBinder("MentionAction");
      private SqlColumnBinder targetId = new SqlColumnBinder("TargetId");
      private SqlColumnBinder storageKey = new SqlColumnBinder("StorageKey");
      private SqlColumnBinder normalizedSourceId = new SqlColumnBinder("NormalizedSourceId");
      private SqlColumnBinder updateState = new SqlColumnBinder("UpdateState");

      public override WorkItemMentionRecord Bind(IDataReader reader) => new WorkItemMentionRecord()
      {
        SourceId = this.sourceId.GetString(reader, false),
        SourceType = this.sourceType.GetString(reader, false),
        RawText = this.rawText.GetString(reader, false),
        ArtifactId = this.artifactId.GetString(reader, false),
        ArtifactType = this.artifactType.GetString(reader, false),
        CommentId = this.commentId.GetInt32(reader),
        MentionAction = this.mentionAction.GetString(reader, false),
        TargetId = this.targetId.GetString(reader, false),
        StorageKey = this.storageKey.GetNullableGuid(reader),
        NormalizedSourceId = this.normalizedSourceId.GetString(reader, false),
        UpdateState = (WorkItemMentionUpdateState) this.updateState.GetByte(reader)
      };
    }

    protected class WorkItemCommentVersionBinder : 
      WorkItemTrackingObjectBinder<WorkItemCommentVersionRecord>
    {
      private SqlColumnBinder artifactId = new SqlColumnBinder("ArtifactId");
      private SqlColumnBinder commentId = new SqlColumnBinder("CommentId");
      private SqlColumnBinder version = new SqlColumnBinder("Version");
      private SqlColumnBinder text = new SqlColumnBinder("Text");
      private SqlColumnBinder renderedText = new SqlColumnBinder("RenderedText");
      private SqlColumnBinder format = new SqlColumnBinder("Format");
      private SqlColumnBinder createdBy = new SqlColumnBinder("CreatedBy");
      private SqlColumnBinder createdDate = new SqlColumnBinder("CreatedDate");
      private SqlColumnBinder createdOnBehalfOf = new SqlColumnBinder("CreatedOnBehalfOf");
      private SqlColumnBinder createdOnBehalfDate = new SqlColumnBinder("CreatedOnBehalfDate");
      private SqlColumnBinder modifiedBy = new SqlColumnBinder("ModifiedBy");
      private SqlColumnBinder modifiedDate = new SqlColumnBinder("ModifiedDate");
      private SqlColumnBinder isDeleted = new SqlColumnBinder("IsDeleted");

      public override WorkItemCommentVersionRecord Bind(IDataReader reader) => new WorkItemCommentVersionRecord()
      {
        WorkItemId = int.Parse(this.artifactId.GetString(reader, false)),
        CommentId = this.commentId.GetInt32(reader),
        Version = this.version.GetInt32(reader),
        Text = this.text.GetString(reader, false),
        RenderedText = this.renderedText.GetString(reader, true, ""),
        Format = this.format.GetByte(reader),
        CreatedBy = this.createdBy.GetGuid(reader, false),
        CreatedDate = this.createdDate.GetDateTime(reader),
        CreatedOnBehalfOf = this.createdOnBehalfOf.GetString(reader, false),
        CreatedOnBehalfDate = this.createdOnBehalfDate.GetDateTime(reader),
        ModifiedBy = this.modifiedBy.GetGuid(reader, false),
        ModifiedDate = this.modifiedDate.GetDateTime(reader),
        IsDeleted = this.isDeleted.GetBoolean(reader)
      };
    }

    protected class WorkItemDependencyGraphBinder : 
      WorkItemTrackingObjectBinder<WorkItemDependencyGraph>
    {
      private SqlColumnBinder sourceId = new SqlColumnBinder("SourceId");
      private SqlColumnBinder targetId = new SqlColumnBinder("TargetId");
      private SqlColumnBinder sourceProjectId = new SqlColumnBinder("SourceProjectId");
      private SqlColumnBinder targetProjectId = new SqlColumnBinder("TargetProjectId");
      private SqlColumnBinder sourceIterationId = new SqlColumnBinder("SourceIterationId");
      private SqlColumnBinder targetIterationId = new SqlColumnBinder("TargetIterationId");
      private SqlColumnBinder linkType = new SqlColumnBinder("LinkType");
      private SqlColumnBinder iterationError = new SqlColumnBinder("IterationError");
      private SqlColumnBinder targetDateError = new SqlColumnBinder("TargetDateError");
      private SqlColumnBinder rootWorkItemId = new SqlColumnBinder("RootWorkItemId");

      public override WorkItemDependencyGraph Bind(IDataReader reader) => new WorkItemDependencyGraph()
      {
        SourceId = this.sourceId.GetInt32(reader),
        TargetId = this.targetId.GetInt32(reader),
        SourceProjectId = this.sourceProjectId.GetGuid(reader),
        TargetProjectId = this.targetProjectId.GetGuid(reader),
        SourceIterationId = this.sourceIterationId.GetGuid(reader, true),
        TargetIterationId = this.targetIterationId.GetGuid(reader, true),
        LinkType = this.linkType.GetInt32(reader),
        IterationError = this.iterationError.GetInt32(reader) == 1,
        TargetDateError = this.targetDateError.GetInt32(reader) == 1,
        RootWorkItemId = this.rootWorkItemId.GetInt32(reader, 0, 0)
      };
    }

    protected class WorkItemDependencyViolationsBinder : WorkItemTrackingObjectBinder<int>
    {
      private SqlColumnBinder id = new SqlColumnBinder("Id");

      public override int Bind(IDataReader reader) => this.id.GetInt32(reader);
    }

    protected class WorkItemDependencyBinder : 
      WorkItemTrackingObjectBinder<WorkItemDependencyInformation>
    {
      private SqlColumnBinder id = new SqlColumnBinder("Id");
      private SqlColumnBinder hasError = new SqlColumnBinder("HasError");
      private SqlColumnBinder hasDependency = new SqlColumnBinder("HasDependency");

      public override WorkItemDependencyInformation Bind(IDataReader reader) => new WorkItemDependencyInformation()
      {
        Id = this.id.GetInt32(reader),
        HasError = this.hasError.GetInt32(reader) == 1,
        HasDependency = this.hasDependency.GetInt32(reader) == 1
      };
    }

    private class WorkItemCoreFieldValuesTableRecordBinder : 
      WorkItemTrackingResourceComponent.TvpRecordBinder<WorkItemCoreFieldUpdatesRecord>
    {
      private static readonly SqlMetaData[] s_metadata = new SqlMetaData[12]
      {
        new SqlMetaData("Id", SqlDbType.Int),
        new SqlMetaData("Rev", SqlDbType.Int),
        new SqlMetaData("WorkItemType", SqlDbType.NVarChar, 256L),
        new SqlMetaData("AreaId", SqlDbType.Int),
        new SqlMetaData("IterationId", SqlDbType.Int),
        new SqlMetaData("CreatedBy", SqlDbType.NVarChar, 256L),
        new SqlMetaData("CreatedDate", SqlDbType.DateTime),
        new SqlMetaData("ChangedBy", SqlDbType.NVarChar, 256L),
        new SqlMetaData("ChangedDate", SqlDbType.DateTime),
        new SqlMetaData("State", SqlDbType.NVarChar, 256L),
        new SqlMetaData("Reason", SqlDbType.NVarChar, 256L),
        new SqlMetaData("AssignedTo", SqlDbType.NVarChar, 256L)
      };

      public override string TypeName => "typ_WorkItemCoreFieldValuesTable";

      protected override SqlMetaData[] TvpMetadata => WorkItemComponent.WorkItemCoreFieldValuesTableRecordBinder.s_metadata;

      public override void SetRecordValues(
        WorkItemTrackingSqlDataRecord record,
        WorkItemCoreFieldUpdatesRecord workItem)
      {
        record.SetInt32(0, workItem.Id);
        record.SetInt32(1, workItem.Revision);
        record.SetString(2, workItem.WorkItemType);
        record.SetInt32(3, workItem.AreaId);
        record.SetInt32(4, workItem.IterationId);
        record.SetNullableString(5, workItem.CreatedBy);
        SqlDateTime minValue;
        if (workItem.Revision == 1)
        {
          WorkItemTrackingSqlDataRecord trackingSqlDataRecord = record;
          minValue = SqlDateTime.MinValue;
          DateTime dateTime = minValue.Value;
          trackingSqlDataRecord.SetDateTime(6, dateTime);
        }
        else
          record.SetDBNull(6);
        record.SetString(7, workItem.ChangedBy);
        WorkItemTrackingSqlDataRecord trackingSqlDataRecord1 = record;
        minValue = SqlDateTime.MinValue;
        DateTime dateTime1 = minValue.Value;
        trackingSqlDataRecord1.SetDateTime(8, dateTime1);
        record.SetNullableString(9, workItem.State);
        record.SetNullableString(10, workItem.Reason);
        record.SetNullableString(11, workItem.AssignedTo);
      }
    }

    private class WorkItemFieldUpdateTableRecordBinder : 
      WorkItemTrackingResourceComponent.TvpRecordBinder<WorkItemCustomFieldUpdateRecord>
    {
      private static readonly SqlMetaData[] s_metadata = new SqlMetaData[9]
      {
        new SqlMetaData("Id", SqlDbType.Int),
        new SqlMetaData("FieldId", SqlDbType.Int),
        new SqlMetaData("TrendOption", SqlDbType.TinyInt),
        new SqlMetaData("IntValue", SqlDbType.Int),
        new SqlMetaData("FloatValue", SqlDbType.Float),
        new SqlMetaData("DateTimeValue", SqlDbType.DateTime),
        new SqlMetaData("GuidValue", SqlDbType.UniqueIdentifier),
        new SqlMetaData("BitValue", SqlDbType.Bit),
        new SqlMetaData("StringValue", SqlDbType.NVarChar, 256L)
      };

      public override string TypeName => "typ_WorkItemFieldUpdateTable";

      protected override SqlMetaData[] TvpMetadata => WorkItemComponent.WorkItemFieldUpdateTableRecordBinder.s_metadata;

      public override void SetRecordValues(
        WorkItemTrackingSqlDataRecord record,
        WorkItemCustomFieldUpdateRecord update)
      {
        record.SetInt32(0, update.WorkItemId);
        record.SetInt32(1, update.Field.FieldId);
        for (int ordinal = 3; ordinal < 9; ++ordinal)
          record.SetDBNull(ordinal);
        object obj1 = update.Value;
        if (obj1 is TrendDataValue)
        {
          object obj2 = obj1 != TrendDataValue.Increase ? (obj1 != TrendDataValue.Decrease ? (object) 0 : (object) -1) : (object) 1;
          record.SetByte(2, (byte) (update.TrendOption | TrendDataUpdateOption.ImplicitTrendDataUpdate));
          record.SetInt32(3, (int) obj2);
        }
        else
        {
          record.SetByte(2, (byte) update.TrendOption);
          if (obj1 == null)
            return;
          switch (update.Field.SqlType)
          {
            case SqlDbType.Bit:
              if (obj1 is bool flag)
              {
                record.SetBoolean(7, flag);
                break;
              }
              record.SetBoolean(7, Convert.ToBoolean(obj1));
              break;
            case SqlDbType.DateTime:
              if (obj1 is DateTime dateTime)
              {
                record.SetDateTime(5, dateTime);
                break;
              }
              record.SetDateTime(5, DateTime.SpecifyKind(Convert.ToDateTime(obj1), DateTimeKind.Utc));
              break;
            case SqlDbType.Float:
              if (obj1 is double num1)
              {
                record.SetDouble(4, num1);
                break;
              }
              record.SetDouble(4, Convert.ToDouble(obj1));
              break;
            case SqlDbType.Int:
              if (obj1 is int num2)
              {
                record.SetInt32(3, num2);
                break;
              }
              record.SetInt32(3, Convert.ToInt32(obj1));
              break;
            case SqlDbType.UniqueIdentifier:
              if (obj1 is Guid guid)
              {
                record.SetGuid(6, guid);
                break;
              }
              record.SetGuid(6, new Guid(obj1.ToString()));
              break;
            default:
              if (obj1 is string)
              {
                record.SetString(8, (string) obj1);
                break;
              }
              record.SetString(8, Convert.ToString(obj1, (IFormatProvider) CultureInfo.InvariantCulture));
              break;
          }
        }
      }
    }

    private class WorkItemTextFieldUpdateTableRecordBinder : 
      WorkItemTrackingResourceComponent.TvpRecordBinder<WorkItemTextFieldUpdateRecord>
    {
      private static readonly SqlMetaData[] s_metadata = new SqlMetaData[5]
      {
        new SqlMetaData("Id", SqlDbType.Int),
        new SqlMetaData("FieldId", SqlDbType.Int),
        new SqlMetaData("Revision", SqlDbType.Int),
        new SqlMetaData("IsHtml", SqlDbType.Bit),
        new SqlMetaData("Value", SqlDbType.NText)
      };

      public override string TypeName => "typ_WorkItemTextFieldUpdateTable";

      protected override SqlMetaData[] TvpMetadata => WorkItemComponent.WorkItemTextFieldUpdateTableRecordBinder.s_metadata;

      public override void SetRecordValues(
        WorkItemTrackingSqlDataRecord record,
        WorkItemTextFieldUpdateRecord update)
      {
        record.SetInt32(0, update.WorkItemId);
        record.SetInt32(1, update.FieldId);
        record.SetInt32(2, update.Revision);
        record.SetBoolean(3, update.IsHtml);
        record.SetString(4, update.Text ?? "");
      }
    }

    private class WorkItemLinkUpdateTableRecordBinder : 
      WorkItemTrackingResourceComponent.TvpRecordBinder<WorkItemLinkUpdateRecord>
    {
      private static readonly SqlMetaData[] s_metadata = new SqlMetaData[9]
      {
        new SqlMetaData("Order", SqlDbType.Int),
        new SqlMetaData("UpdateType", SqlDbType.TinyInt),
        new SqlMetaData("SourceId", SqlDbType.Int),
        new SqlMetaData("SourceAreaId", SqlDbType.Int),
        new SqlMetaData("TargetId", SqlDbType.Int),
        new SqlMetaData("TargetAreaId", SqlDbType.Int),
        new SqlMetaData("LinkType", SqlDbType.Int),
        new SqlMetaData("Locked", SqlDbType.Bit),
        new SqlMetaData("Comment", SqlDbType.NVarChar, (long) byte.MaxValue)
      };

      public override string TypeName => "typ_WorkItemLinkUpdateTable";

      protected override SqlMetaData[] TvpMetadata => WorkItemComponent.WorkItemLinkUpdateTableRecordBinder.s_metadata;

      public override void SetRecordValues(
        WorkItemTrackingSqlDataRecord record,
        WorkItemLinkUpdateRecord update)
      {
        record.SetInt32(0, update.Order);
        record.SetByte(1, (byte) update.UpdateType);
        record.SetInt32(2, update.SourceId);
        record.SetInt32(3, update.SourceAreaId);
        record.SetInt32(4, update.TargetId);
        record.SetInt32(5, update.TargetAreaId);
        record.SetInt32(6, update.LinkType);
        record.SetNullableBool(7, update.Locked);
        record.SetString(8, update.Comment ?? string.Empty);
      }
    }

    private class WorkItemResourceLinkUpdateTableRecordBinder : 
      WorkItemTrackingResourceComponent.TvpRecordBinder<WorkItemResourceLinkUpdateRecord>
    {
      private static readonly SqlMetaData[] s_metadata = new SqlMetaData[11]
      {
        new SqlMetaData("Order", SqlDbType.Int),
        new SqlMetaData("UpdateType", SqlDbType.TinyInt),
        new SqlMetaData("SourceId", SqlDbType.Int),
        new SqlMetaData("ResourceType", SqlDbType.Int),
        new SqlMetaData("ResourceId", SqlDbType.Int),
        new SqlMetaData("Location", SqlDbType.NVarChar, 256L),
        new SqlMetaData("Name", SqlDbType.NVarChar, (long) byte.MaxValue),
        new SqlMetaData("Length", SqlDbType.Int),
        new SqlMetaData("CreatedDate", SqlDbType.DateTime),
        new SqlMetaData("ModifiedDate", SqlDbType.DateTime),
        new SqlMetaData("Comment", SqlDbType.NVarChar, (long) byte.MaxValue)
      };

      public override string TypeName => "typ_WorkItemResourceLinkUpdateTable";

      protected override SqlMetaData[] TvpMetadata => WorkItemComponent.WorkItemResourceLinkUpdateTableRecordBinder.s_metadata;

      public override void SetRecordValues(
        WorkItemTrackingSqlDataRecord record,
        WorkItemResourceLinkUpdateRecord update)
      {
        record.SetInt32(0, update.Order);
        record.SetByte(1, (byte) update.UpdateType);
        record.SetInt32(2, update.SourceId);
        if (update.Type.HasValue)
          record.SetInt32(3, (int) update.Type.Value);
        else
          record.SetDBNull(3);
        record.SetNullableInt32(4, update.ResourceId);
        record.SetNullableString(5, update.Location);
        record.SetString(6, update.Name ?? "");
        record.SetInt32(7, update.Length.GetValueOrDefault());
        record.SetDateTime(8, update.CreationDate.GetValueOrDefault(SqlDateTime.MinValue.Value));
        record.SetDateTime(9, update.LastModifiedDate.GetValueOrDefault(SqlDateTime.MinValue.Value));
        record.SetString(10, update.Comment ?? "");
      }
    }

    private class WorkItemPendingSetMembershipCheckTableRecordBinder : 
      WorkItemTrackingResourceComponent.TvpRecordBinder<PendingSetMembershipCheckRecord>
    {
      private static readonly SqlMetaData[] s_metadata = new SqlMetaData[9]
      {
        new SqlMetaData("Id", SqlDbType.Int),
        new SqlMetaData("FieldId", SqlDbType.Int),
        new SqlMetaData("Value", SqlDbType.NVarChar, 256L),
        new SqlMetaData("UnionGroup", SqlDbType.Int),
        new SqlMetaData("SetId", SqlDbType.Int),
        new SqlMetaData("IncludeTop", SqlDbType.Bit),
        new SqlMetaData("Direct", SqlDbType.Bit),
        new SqlMetaData("ExcludeGroups", SqlDbType.Bit),
        new SqlMetaData("Prohibited", SqlDbType.Bit)
      };

      public override string TypeName => "typ_WorkItemPendingSetMembershipCheckTable";

      protected override SqlMetaData[] TvpMetadata => WorkItemComponent.WorkItemPendingSetMembershipCheckTableRecordBinder.s_metadata;

      public override void SetRecordValues(
        WorkItemTrackingSqlDataRecord record,
        PendingSetMembershipCheckRecord membershipCheck)
      {
        record.SetInt32(0, membershipCheck.WorkItemId);
        record.SetInt32(1, membershipCheck.FieldId);
        record.SetNullableString(2, membershipCheck.Value);
        record.SetInt32(3, membershipCheck.UnionGroup);
        record.SetInt32(4, membershipCheck.SetReference.Id);
        record.SetBoolean(5, membershipCheck.SetReference.IncludeTop);
        record.SetBoolean(6, membershipCheck.SetReference.Direct);
        record.SetBoolean(7, membershipCheck.SetReference.ExcludeGroups);
        record.SetBoolean(8, membershipCheck.Prohibited);
      }
    }

    internal class UpdateWorkItemsResultsReader
    {
      protected bool bypassRules;
      protected bool isAdmin;
      protected WorkItemUpdateDataset updateDataset;
      protected WorkItemUpdateResultSet result;
      protected bool tieCoreFieldUpdatesWithResourceLinkUpdates;
      protected bool isMaxAddedLinksCountReturned;

      internal UpdateWorkItemsResultsReader(
        bool bypassRules,
        bool isAdmin,
        WorkItemUpdateDataset updateDataset)
      {
        this.bypassRules = bypassRules;
        this.isAdmin = isAdmin;
        this.updateDataset = updateDataset;
        this.tieCoreFieldUpdatesWithResourceLinkUpdates = true;
        this.isMaxAddedLinksCountReturned = false;
      }

      internal virtual WorkItemUpdateResultSet Read(IDataReader reader)
      {
        this.result = new WorkItemUpdateResultSet()
        {
          Success = false,
          FailureReason = "Default"
        };
        this.CheckAndReadChangedByInfo(reader);
        if (!this.CheckAndReadPendingSetMembershipChecks(reader))
        {
          this.result.FailureReason = "CheckAndReadPendingSetMembershipChecks failed";
          return this.result;
        }
        if (this.updateDataset.CoreFieldUpdates.Any<WorkItemCoreFieldUpdatesRecord>())
        {
          reader.NextResult();
          if (!this.ReadCoreFieldUpdates(reader))
          {
            this.result.FailureReason = "ReadCoreFieldUpdates failed";
            return this.result;
          }
          if (this.tieCoreFieldUpdatesWithResourceLinkUpdates && !this.CheckAndReadResourceLinkUpdates(reader))
          {
            this.result.FailureReason = "CheckAndReadResourceLinkUpdates core failed";
            return this.result;
          }
        }
        if (!this.tieCoreFieldUpdatesWithResourceLinkUpdates && !this.CheckAndReadResourceLinkUpdates(reader))
        {
          this.result.FailureReason = "CheckAndReadResourceLinkUpdates non-core failed";
          return this.result;
        }
        if (!this.CheckAndReadWorkItemLinkUpdates(reader))
        {
          this.result.FailureReason = "CheckAndReadWorkItemLinkUpdates failed";
          return this.result;
        }
        this.CheckAndReadWatermark(reader);
        this.result.Success = true;
        return this.result;
      }

      protected virtual void CheckAndReadChangedByInfo(IDataReader reader)
      {
        this.result.ChangedById = reader.Read() ? reader.GetInt32(0) : throw new WorkItemTrackingSqlDataBindingException("No result rows in ChangedBy");
        this.result.ChangedDate = DateTime.SpecifyKind(reader.GetDateTime(1), DateTimeKind.Utc);
        if (reader.Read())
          throw new WorkItemTrackingSqlDataBindingException("Too many rows in ChangedBy");
      }

      protected virtual bool CheckAndReadPendingSetMembershipChecks(IDataReader reader)
      {
        if ((!this.bypassRules || !this.isAdmin) && this.updateDataset.PendingSetMembershipChecks.Any<PendingSetMembershipCheckRecord>())
        {
          reader.NextResult();
          this.result.SetMembershipCheckResults = (IEnumerable<PendingSetMembershipCheckResultRecord>) WorkItemTrackingResourceComponent.Bind<PendingSetMembershipCheckResultRecord>(reader, (System.Func<IDataReader, PendingSetMembershipCheckResultRecord>) (r => new PendingSetMembershipCheckResultRecord()
          {
            WorkItemId = r.GetInt32(0),
            FieldId = r.GetInt32(1),
            Status = r.GetInt32(2)
          })).ToArray<PendingSetMembershipCheckResultRecord>();
          if (this.result.SetMembershipCheckResults.Any<PendingSetMembershipCheckResultRecord>())
            return false;
        }
        return true;
      }

      protected virtual bool ReadCoreFieldUpdates(IDataReader reader)
      {
        this.result.CoreFieldUpdatesResults = (IEnumerable<WorkItemCoreFieldUpdatesResultRecord>) WorkItemTrackingResourceComponent.Bind<WorkItemCoreFieldUpdatesResultRecord>(reader, (System.Func<IDataReader, WorkItemCoreFieldUpdatesResultRecord>) (r => new WorkItemCoreFieldUpdatesResultRecord()
        {
          TempId = r.GetInt32(0),
          Id = r.GetInt32(1),
          Status = r.GetInt32(2)
        })).ToArray<WorkItemCoreFieldUpdatesResultRecord>();
        return !this.result.CoreFieldUpdatesResults.Any<WorkItemCoreFieldUpdatesResultRecord>((System.Func<WorkItemCoreFieldUpdatesResultRecord, bool>) (cfur => cfur.Status != 0));
      }

      protected virtual bool CheckAndReadResourceLinkUpdates(IDataReader reader)
      {
        if (this.updateDataset.ResourceLinkUpdates.Any<WorkItemResourceLinkUpdateRecord>())
        {
          reader.NextResult();
          this.result.ResourceLinkUpdateResults = (IEnumerable<WorkItemResourceLinkUpdateResultRecord>) new WorkItemComponent.WorkItemResourceLinkUpdateResultRecordBinder().BindAll(reader).ToArray<WorkItemResourceLinkUpdateResultRecord>();
          if (this.result.ResourceLinkUpdateResults.Any<WorkItemResourceLinkUpdateResultRecord>((System.Func<WorkItemResourceLinkUpdateResultRecord, bool>) (rlur => rlur.Status != 0)))
            return false;
        }
        return true;
      }

      protected virtual bool CheckAndReadWorkItemLinkUpdates(IDataReader reader)
      {
        if (this.updateDataset.WorkItemLinkUpdates.Any<WorkItemLinkUpdateRecord>())
        {
          reader.NextResult();
          this.result.LinkUpdateResults = (IEnumerable<WorkItemLinkUpdateResultRecord>) WorkItemTrackingResourceComponent.Bind<WorkItemLinkUpdateResultRecord>(reader, (System.Func<IDataReader, WorkItemLinkUpdateResultRecord>) (r => new WorkItemLinkUpdateResultRecord()
          {
            Order = r.GetInt32(0),
            UpdateType = (LinkUpdateType) r.GetByte(1),
            UpdateTypeExecuted = r.IsDBNull(2) ? new LinkUpdateType?() : new LinkUpdateType?((LinkUpdateType) r.GetByte(2)),
            Status = r.GetInt32(3),
            SourceId = r.GetInt32(4),
            TargetId = r.GetInt32(5),
            LinkType = r.GetInt32(6)
          })).ToArray<WorkItemLinkUpdateResultRecord>();
          if (this.isMaxAddedLinksCountReturned)
          {
            reader.NextResult();
            if (reader.Read())
              this.result.MaxAddedLinksCount = reader.GetInt32(0);
          }
          if (this.result.LinkUpdateResults.Any<WorkItemLinkUpdateResultRecord>((System.Func<WorkItemLinkUpdateResultRecord, bool>) (wlur => wlur.Status != 0)))
            return false;
        }
        return true;
      }

      protected virtual void CheckAndReadWatermark(IDataReader reader)
      {
        if (!reader.NextResult())
          return;
        this.result.Watermark = reader.Read() ? reader.GetInt32(0) : throw new WorkItemTrackingSqlDataBindingException("The watermark table has no records! - should have one");
        if (reader.Read())
          throw new WorkItemTrackingSqlDataBindingException("The watermark table has more than one record - should have only one");
      }
    }
  }
}
