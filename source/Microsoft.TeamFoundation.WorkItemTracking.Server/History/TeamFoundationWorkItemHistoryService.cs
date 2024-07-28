// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.History.TeamFoundationWorkItemHistoryService
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.DataModels;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.History
{
  public class TeamFoundationWorkItemHistoryService : 
    ITeamFoundationWorkItemHistoryService,
    IVssFrameworkService
  {
    void IVssFrameworkService.ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    void IVssFrameworkService.ServiceStart(IVssRequestContext systemRequestContext)
    {
      if (!systemRequestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
        throw new UnexpectedHostTypeException(systemRequestContext.ServiceHost.HostType);
    }

    public IEnumerable<WorkItemFieldHistoryRecord> GetFieldHistory(
      IVssRequestContext requestContext,
      long timeStamp)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      using (HistoryComponent component = requestContext.CreateComponent<HistoryComponent>())
        return (IEnumerable<WorkItemFieldHistoryRecord>) component.GetFieldHistory(timeStamp).ToList<WorkItemFieldHistoryRecord>();
    }

    public IEnumerable<IDictionary<FieldEntry, object>> GetFieldValuesHistory(
      IVssRequestContext requestContext,
      IEnumerable<string> fieldReferences,
      IEnumerable<WorkItemIdRevisionPair> idRevPairs)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<IEnumerable<string>>(fieldReferences, nameof (fieldReferences));
      ArgumentUtility.CheckForNull<IEnumerable<WorkItemIdRevisionPair>>(idRevPairs, nameof (idRevPairs));
      ITeamFoundationWorkItemService service = requestContext.GetService<ITeamFoundationWorkItemService>();
      HashSet<string> uniqueFieldReferences = new HashSet<string>((IEqualityComparer<string>) TFStringComparer.WorkItemFieldReferenceName);
      uniqueFieldReferences.UnionWith(fieldReferences);
      if (uniqueFieldReferences.Contains("System.AreaPath"))
        uniqueFieldReferences.Add("System.AreaId");
      if (uniqueFieldReferences.Contains("System.IterationPath"))
        uniqueFieldReferences.Add("System.IterationId");
      IVssRequestContext requestContext1 = requestContext;
      IEnumerable<WorkItemIdRevisionPair> workItemIdRevPairs = idRevPairs;
      HashSet<string> fields = uniqueFieldReferences;
      return CommonWITUtils.BatchResponse<WorkItemFieldData, IDictionary<FieldEntry, object>>((Func<IEnumerable<WorkItemFieldData>, IEnumerable<IDictionary<FieldEntry, object>>>) (fieldValues => (IEnumerable<IDictionary<FieldEntry, object>>) this.ConvertWorkItemFieldValuesToFieldEntryDictionaries(requestContext, fieldValues, (IEnumerable<string>) uniqueFieldReferences)), service.GetWorkItemFieldValues(requestContext1, workItemIdRevPairs, (IEnumerable<string>) fields, workItemRetrievalMode: WorkItemRetrievalMode.All), 200);
    }

    public ICollection<WorkItemIdRevisionPair> GetChangedRevisions(
      IVssRequestContext requestContext,
      int watermark,
      int batchSize = 200,
      DateTime? startDate = null)
    {
      return this.GetChangedRevisions(requestContext, new Guid?(), (IEnumerable<string>) null, watermark, batchSize, startDate);
    }

    public ICollection<WorkItemIdRevisionPair> GetChangedRevisions(
      IVssRequestContext requestContext,
      Guid? projectId,
      IEnumerable<string> types,
      int watermark,
      int batchSize = 200,
      DateTime? startDate = null)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForOutOfRange(batchSize, nameof (batchSize), 0);
      using (HistoryComponent replicaAwareComponent = requestContext.CreateReadReplicaAwareComponent<HistoryComponent>(new WitReadReplicaContext?(new WitReadReplicaContext("WorkItemTracking.Server.ReadFromReadReplica"))))
        return (ICollection<WorkItemIdRevisionPair>) replicaAwareComponent.GetChangedWorkItems(watermark, batchSize, projectId, types, startDate).ToList<WorkItemIdRevisionPair>();
    }

    public ICollection<WorkItemIdRevisionPair> GetChangedRevisionsPageable(
      IVssRequestContext requestContext,
      Guid? projectId,
      IEnumerable<string> types,
      bool includeLatestOnly,
      bool includeDiscussionChangesOnly,
      bool useLegacyDiscussionChanges,
      WorkItemIdRevisionPair idRevPair,
      int batchSize = 200,
      bool includeDiscussionHistory = false)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      requestContext.CheckServiceHostType(TeamFoundationHostType.ProjectCollection);
      ArgumentUtility.CheckForOutOfRange(batchSize, nameof (batchSize), 0);
      if (includeDiscussionChangesOnly && types != null && types.Any<string>())
        throw new ArgumentException(Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.TypeFilteringNotSupportedForDiscussions());
      using (HistoryComponent replicaAwareComponent = requestContext.CreateReadReplicaAwareComponent<HistoryComponent>(new WitReadReplicaContext?(new WitReadReplicaContext("WorkItemTracking.Server.ReadFromReadReplica"))))
        return (ICollection<WorkItemIdRevisionPair>) replicaAwareComponent.GetChangedWorkItemsPageable(idRevPair, batchSize, projectId, types, includeLatestOnly, includeDiscussionChangesOnly, useLegacyDiscussionChanges, includeDiscussionHistory).ToList<WorkItemIdRevisionPair>();
    }

    public WorkItemIdRevisionPair GetWorkItemWatermarkForDate(
      IVssRequestContext requestContext,
      DateTime date)
    {
      using (HistoryComponent component = requestContext.CreateComponent<HistoryComponent>())
        return component.GetWorkItemWatermarkForDate(date);
    }

    public IEnumerable<IDictionary<FieldEntry, object>> GetFieldValuesHistory(
      IVssRequestContext requestContext,
      IEnumerable<string> fieldReferences,
      int watermark,
      int batchSize = 200)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<IEnumerable<string>>(fieldReferences, nameof (fieldReferences));
      ArgumentUtility.CheckForOutOfRange(batchSize, nameof (batchSize), 0);
      ICollection<WorkItemIdRevisionPair> changedRevisions = this.GetChangedRevisions(requestContext, watermark, batchSize, new DateTime?());
      return this.GetFieldValuesHistory(requestContext, fieldReferences, (IEnumerable<WorkItemIdRevisionPair>) changedRevisions);
    }

    public IEnumerable<WorkItemDestroyHistoryRecord> GetWorkItemDestroyHistory(
      IVssRequestContext requestContext,
      long timeStamp,
      int batchSize = 200)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForOutOfRange(batchSize, nameof (batchSize), 0);
      using (HistoryComponent replicaAwareComponent = requestContext.CreateReadReplicaAwareComponent<HistoryComponent>(new WitReadReplicaContext?(new WitReadReplicaContext("WorkItemTracking.Server.ReadFromReadReplica"))))
        return (IEnumerable<WorkItemDestroyHistoryRecord>) replicaAwareComponent.GetWorkItemDestroyHistory(timeStamp, batchSize).ToList<WorkItemDestroyHistoryRecord>();
    }

    public IEnumerable<WorkItemLinkHistoryRecord> GetWorkItemLinkHistory(
      IVssRequestContext requestContext,
      long timeStamp,
      int batchSize = 1000,
      DateTime? startDate = null)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForOutOfRange(batchSize, nameof (batchSize), 0);
      using (HistoryComponent component = requestContext.CreateComponent<HistoryComponent>())
        return (IEnumerable<WorkItemLinkHistoryRecord>) component.GetWorkItemLinkHistory(timeStamp, batchSize, startDate).ToList<WorkItemLinkHistoryRecord>();
    }

    public IEnumerable<WorkItemLinkTypeHistoryRecord> GetWorkItemLinkTypeHistory(
      IVssRequestContext requestContext,
      long timeStamp)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      using (HistoryComponent component = requestContext.CreateComponent<HistoryComponent>())
        return (IEnumerable<WorkItemLinkTypeHistoryRecord>) component.GetWorkItemLinkTypeHistory(timeStamp).ToList<WorkItemLinkTypeHistoryRecord>();
    }

    public IEnumerable<WorkItemTypeCategoryHistoryRecord> GetWorkItemTypeCategoryHistory(
      IVssRequestContext requestContext,
      long timeStamp)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      using (HistoryComponent component = requestContext.CreateComponent<HistoryComponent>())
        return (IEnumerable<WorkItemTypeCategoryHistoryRecord>) component.GetWorkItemTypeCategoryHistory(timeStamp).ToList<WorkItemTypeCategoryHistoryRecord>();
    }

    public IEnumerable<WorkItemTypeCategoryMemberHistoryRecord> GetWorkItemTypeCategoryMemberHistory(
      IVssRequestContext requestContext,
      long timeStamp)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      using (HistoryComponent component = requestContext.CreateComponent<HistoryComponent>())
        return (IEnumerable<WorkItemTypeCategoryMemberHistoryRecord>) component.GetWorkItemTypeCategoryMemberHistory(timeStamp).ToList<WorkItemTypeCategoryMemberHistoryRecord>();
    }

    public IEnumerable<WorkItemTypeRenameHistoryRecord> GetWorkItemTypeRenameHistory(
      IVssRequestContext requestContext,
      long timeStamp)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      using (HistoryComponent component = requestContext.CreateComponent<HistoryComponent>())
        return (IEnumerable<WorkItemTypeRenameHistoryRecord>) component.GetWorkItemTypeRenameHistory(timeStamp).ToList<WorkItemTypeRenameHistoryRecord>();
    }

    private ICollection<IDictionary<FieldEntry, object>> ConvertWorkItemFieldValuesToFieldEntryDictionaries(
      IVssRequestContext requestContext,
      IEnumerable<WorkItemFieldData> workItemFieldValues,
      IEnumerable<string> fieldReferences)
    {
      WorkItemTrackingRequestContext witRequestContext = requestContext.WitContext();
      IFieldTypeDictionary fieldDictionary = witRequestContext.FieldDictionary;
      FieldEntry[] fieldEntries = fieldReferences.Select<string, FieldEntry>((Func<string, FieldEntry>) (fr => fieldDictionary.GetField(fr))).ToArray<FieldEntry>();
      List<KeyValuePair<string, WorkItemPersonFieldValue>> dataList = new List<KeyValuePair<string, WorkItemPersonFieldValue>>();
      WorkItemTrackingTreeService service = requestContext.GetService<WorkItemTrackingTreeService>();
      IEnumerable<int> ids = workItemFieldValues.SelectMany<WorkItemFieldData, int>((Func<WorkItemFieldData, IEnumerable<int>>) (values => values.LatestData.Where<KeyValuePair<int, object>>((Func<KeyValuePair<int, object>, bool>) (fieldValue =>
      {
        if (fieldValue.Value == null)
          return false;
        return fieldValue.Key == -2 || fieldValue.Key == -104;
      })).Select<KeyValuePair<int, object>, int>((Func<KeyValuePair<int, object>, int>) (fieldValue => (int) fieldValue.Value))));
      IDictionary<int, TreeNode> nodeDict = service.LegacyGetTreeNodes(requestContext, ids, true);
      List<IDictionary<FieldEntry, object>> list = workItemFieldValues.Select<WorkItemFieldData, IDictionary<FieldEntry, object>>((Func<WorkItemFieldData, IDictionary<FieldEntry, object>>) (wifv => (IDictionary<FieldEntry, object>) ((IEnumerable<FieldEntry>) fieldEntries).ToDictionary<FieldEntry, FieldEntry, object>((Func<FieldEntry, FieldEntry>) (fe => fe), (Func<FieldEntry, object>) (fe =>
      {
        object fieldValue = wifv.GetFieldValue(witRequestContext, fe.FieldId);
        if (fieldValue != null && (fe.FieldDataType & 24) == 24)
        {
          WorkItemPersonFieldValue entryDictionaries = new WorkItemPersonFieldValue();
          dataList.Add(new KeyValuePair<string, WorkItemPersonFieldValue>((string) fieldValue, entryDictionaries));
          return (object) entryDictionaries;
        }
        if (fe.FieldId == -7)
          return (object) nodeDict[wifv.AreaId].GetPath(requestContext);
        return fe.FieldId == -105 ? (object) nodeDict[wifv.IterationId].GetPath(requestContext) : fieldValue;
      })))).ToList<IDictionary<FieldEntry, object>>();
      IEnumerable<PersonNameConstantRecord> recordsFromPersonNames = requestContext.GetService<ITeamFoundationWorkItemTrackingMetadataService>().GetConstantRecordsFromPersonNames(requestContext, dataList.Select<KeyValuePair<string, WorkItemPersonFieldValue>, string>((Func<KeyValuePair<string, WorkItemPersonFieldValue>, string>) (kvp => kvp.Key)));
      Dictionary<string, PersonNameConstantRecord> dictionary = new Dictionary<string, PersonNameConstantRecord>(recordsFromPersonNames.Count<PersonNameConstantRecord>());
      foreach (PersonNameConstantRecord nameConstantRecord in recordsFromPersonNames)
        dictionary.TryAdd<string, PersonNameConstantRecord>(nameConstantRecord.FieldDisplayName, nameConstantRecord);
      foreach (KeyValuePair<string, WorkItemPersonFieldValue> keyValuePair in dataList)
      {
        WorkItemPersonFieldValue personFieldValue = keyValuePair.Value;
        personFieldValue.DisplayName = keyValuePair.Key;
        PersonNameConstantRecord nameConstantRecord;
        if (dictionary.TryGetValue(keyValuePair.Key, out nameConstantRecord))
        {
          personFieldValue.ConstantId = nameConstantRecord.Id;
          personFieldValue.DisplayName = nameConstantRecord.FieldDisplayName;
          personFieldValue.TeamFoundationId = new Guid?(nameConstantRecord.TeamFoundationId);
        }
      }
      return (ICollection<IDictionary<FieldEntry, object>>) list;
    }

    public virtual IReadOnlyCollection<WorkItemResourceLinkInfo> GetWorkItemResourceLinks(
      IVssRequestContext requestContext,
      IEnumerable<WorkItemIdRevisionPair> workItemIdRevPairs,
      bool includeExtendedProperties = false)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<IEnumerable<WorkItemIdRevisionPair>>(workItemIdRevPairs, nameof (workItemIdRevPairs));
      if (!workItemIdRevPairs.Any<WorkItemIdRevisionPair>())
        return (IReadOnlyCollection<WorkItemResourceLinkInfo>) Array.Empty<WorkItemResourceLinkInfo>();
      using (HistoryComponent component = requestContext.CreateComponent<HistoryComponent>())
        return component.GetWorkItemResourceLinks(workItemIdRevPairs, includeExtendedProperties);
    }

    public virtual IReadOnlyCollection<WorkItemResourceLinkInfo> GetWorkItemResourceLinks(
      IVssRequestContext requestContext,
      IEnumerable<int> workItemIds,
      bool includeExtendedProperties = false)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<IEnumerable<int>>(workItemIds, nameof (workItemIds));
      if (!workItemIds.Any<int>())
        return (IReadOnlyCollection<WorkItemResourceLinkInfo>) Array.Empty<WorkItemResourceLinkInfo>();
      using (HistoryComponent component = requestContext.CreateComponent<HistoryComponent>())
        return component.GetWorkItemResourceLinks(workItemIds, includeExtendedProperties);
    }

    public virtual IReadOnlyCollection<WorkItemResourceLinkInfo> GetWorkItemResourceLinksByResourceIds(
      IVssRequestContext requestContext,
      IEnumerable<KeyValuePair<int, int>> workItemIdResourceIdPairs,
      bool includeExtendedProperties = false)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<IEnumerable<KeyValuePair<int, int>>>(workItemIdResourceIdPairs, nameof (workItemIdResourceIdPairs));
      if (!workItemIdResourceIdPairs.Any<KeyValuePair<int, int>>())
        return (IReadOnlyCollection<WorkItemResourceLinkInfo>) Array.Empty<WorkItemResourceLinkInfo>();
      using (HistoryComponent component = requestContext.CreateComponent<HistoryComponent>())
        return component.GetWorkItemResourceLinksByResourceIds(workItemIdResourceIdPairs, includeExtendedProperties);
    }

    public int? GetWorkItemMinWatermark(IVssRequestContext requestContext, int fieldId)
    {
      using (HistoryComponent component = requestContext.CreateComponent<HistoryComponent>())
        return component.GetWorkItemMinWatermark(fieldId);
    }
  }
}
