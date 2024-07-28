// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.WorkItemTrackingFieldService
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.DataModels;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata
{
  public class WorkItemTrackingFieldService : WorkItemTrackingDictionaryService
  {
    private static readonly ReadOnlyCollection<MetadataTable> m_metadataTables = new List<MetadataTable>()
    {
      MetadataTable.Fields
    }.AsReadOnly();
    private static readonly Guid WorkItemFieldsChangedEventClass = new Guid("92778466-E528-4569-8736-A7CBD9983DB7");
    private readonly VssCachePerformanceProvider PerfProvider = new VssCachePerformanceProvider("WorkItemTrackingFieldCacheService");

    protected override IEnumerable<MetadataTable> MetadataTables => (IEnumerable<MetadataTable>) WorkItemTrackingFieldService.m_metadataTables;

    protected override void ServiceStart(IVssRequestContext systemRequestContext)
    {
      base.ServiceStart(systemRequestContext);
      this.RegisterSqlNotifications(systemRequestContext);
    }

    protected override void ServiceEnd(IVssRequestContext systemRequestContext)
    {
      this.UnregisterSqlNotifications(systemRequestContext);
      int count = this.GetAllFields(systemRequestContext, new bool?(false)).Count<FieldEntry>();
      if (count > 0)
        this.PerfProvider.NotifyCacheItemsRemoved(count, new MemoryCacheOperationStatistics(-count, 0L));
      base.ServiceEnd(systemRequestContext);
    }

    private void RegisterSqlNotifications(IVssRequestContext requestContext) => requestContext.GetService<ITeamFoundationSqlNotificationService>().RegisterNotification(requestContext, "Default", WorkItemTrackingFieldService.WorkItemFieldsChangedEventClass, new SqlNotificationCallback(this.OnWorkItemFieldsChanged), false);

    private void UnregisterSqlNotifications(IVssRequestContext requestContext) => requestContext.GetService<ITeamFoundationSqlNotificationService>().UnregisterNotification(requestContext, "Default", WorkItemTrackingFieldService.WorkItemFieldsChangedEventClass, new SqlNotificationCallback(this.OnWorkItemFieldsChanged), false);

    private void OnWorkItemFieldsChanged(
      IVssRequestContext requestContext,
      Guid eventClass,
      string eventData)
    {
      this.InvalidateCache(requestContext);
    }

    protected override CacheSnapshotBase CreateSnapshot(
      IVssRequestContext context,
      CacheSnapshotBase existingSnapshot)
    {
      MetadataDBStamps stamps = context.MetadataDbStamps(this.MetadataTables);
      return (CacheSnapshotBase) new WorkItemTrackingFieldService.FieldTypeCacheServiceImpl(context, this, stamps, (WorkItemTrackingFieldService.FieldTypeCacheServiceImpl) existingSnapshot);
    }

    public virtual IFieldTypeDictionary GetFieldsSnapshot(IVssRequestContext requestContext)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      return (IFieldTypeDictionary) this.GetSnapshot<WorkItemTrackingFieldService.FieldTypeCacheServiceImpl>(requestContext);
    }

    public virtual FieldEntry GetField(
      IVssRequestContext requestContext,
      string name,
      bool? checkFreshness = null)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckStringForNullOrEmpty(name, nameof (name));
      FieldEntry field;
      if (!this.GetSnapshot<WorkItemTrackingFieldService.FieldTypeCacheServiceImpl>(requestContext, checkFreshness.GetValueOrDefault()).TryGetField(name, out field))
      {
        this.PerfProvider.NotifyCacheLookupFailed();
        if ((checkFreshness.HasValue ? (!checkFreshness.Value ? 1 : 0) : 1) == 0)
          throw new WorkItemTrackingFieldDefinitionNotFoundException(name);
        if (!this.GetSnapshot<WorkItemTrackingFieldService.FieldTypeCacheServiceImpl>(requestContext).TryGetField(name, out field))
          throw new WorkItemTrackingFieldDefinitionNotFoundException(name);
      }
      else
        this.PerfProvider.NotifyCacheLookupSucceeded();
      return field;
    }

    public virtual bool TryGetField(
      IVssRequestContext requestContext,
      string name,
      out FieldEntry field,
      bool? checkFreshness = null)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckStringForNullOrEmpty(name, nameof (name));
      if (!this.GetSnapshot<WorkItemTrackingFieldService.FieldTypeCacheServiceImpl>(requestContext, checkFreshness.GetValueOrDefault()).TryGetField(name, out field))
      {
        this.PerfProvider.NotifyCacheLookupFailed();
        return (checkFreshness.HasValue ? (!checkFreshness.Value ? 1 : 0) : 1) != 0 && this.GetSnapshot<WorkItemTrackingFieldService.FieldTypeCacheServiceImpl>(requestContext).TryGetField(name, out field);
      }
      this.PerfProvider.NotifyCacheLookupSucceeded();
      return true;
    }

    public virtual FieldEntry GetFieldById(
      IVssRequestContext requestContext,
      int fieldId,
      bool? checkFreshness = null)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      FieldEntry field;
      if (!this.GetSnapshot<WorkItemTrackingFieldService.FieldTypeCacheServiceImpl>(requestContext, checkFreshness.GetValueOrDefault()).TryGetField(fieldId, out field))
      {
        this.PerfProvider.NotifyCacheLookupFailed();
        if ((checkFreshness.HasValue ? (!checkFreshness.Value ? 1 : 0) : 1) == 0)
          return (FieldEntry) null;
        if (!this.GetSnapshot<WorkItemTrackingFieldService.FieldTypeCacheServiceImpl>(requestContext).TryGetField(fieldId, out field))
          return (FieldEntry) null;
      }
      else
        this.PerfProvider.NotifyCacheLookupSucceeded();
      return field;
    }

    public virtual IEnumerable<FieldEntry> GetAllFields(
      IVssRequestContext requestContext,
      bool? checkFreshness = null)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      return (IEnumerable<FieldEntry>) this.GetSnapshot<WorkItemTrackingFieldService.FieldTypeCacheServiceImpl>(requestContext, checkFreshness.GetValueOrDefault()).GetAllFields();
    }

    public bool IsPersonField(IVssRequestContext requestContext, string name)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      FieldEntry field;
      return this.TryGetField(requestContext, name, out field) && field.IsPerson;
    }

    public int GetFieldType(IVssRequestContext requestContext, string name)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      FieldEntry field;
      return !this.TryGetField(requestContext, name, out field) ? -1 : field.FieldDataType;
    }

    public virtual IList<FieldEntry> GetFieldEntries(
      IVssRequestContext requestContext,
      long sinceFieldCacheStamp,
      out long maxCacheStamp,
      bool disableDataspaceRls = false,
      bool includeDeleted = false)
    {
      maxCacheStamp = 0L;
      PerformanceScenarioHelper performanceScenarioHelper = new PerformanceScenarioHelper(requestContext, "Field", "FieldTypeDictionary");
      List<FieldRecord> recordsIncremental;
      using (performanceScenarioHelper.Measure(nameof (GetFieldEntries)))
      {
        using (WorkItemTrackingMetadataComponent component = requestContext.CreateComponent<WorkItemTrackingMetadataComponent>())
        {
          WorkItemTrackingFieldService.TraceFieldDictionary(requestContext, 901533, string.Format("Field cache: Begin GetFieldRecordsIncremental(): sinceFieldCacheStamp={0}", (object) sinceFieldCacheStamp));
          recordsIncremental = component.GetFieldRecordsIncremental(sinceFieldCacheStamp, out maxCacheStamp, disableDataspaceRls, includeDeleted);
          WorkItemTrackingFieldService.TraceFieldDictionary(requestContext, 901533, "Field cache: End GetFieldRecordsIncremental(): " + string.Format("fetchCount={0}, sinceFieldCacheStamp={1}", (object) recordsIncremental.Count, (object) sinceFieldCacheStamp));
        }
      }
      performanceScenarioHelper.EndScenario();
      return (IList<FieldEntry>) recordsIncremental.Select<FieldRecord, WorkItemTrackingFieldService.FieldEntryImpl>((Func<FieldRecord, WorkItemTrackingFieldService.FieldEntryImpl>) (fr => new WorkItemTrackingFieldService.FieldEntryImpl(fr))).ToArray<WorkItemTrackingFieldService.FieldEntryImpl>();
    }

    public FieldEntry RestoreField(IVssRequestContext requestContext, FieldEntry dbFieldEntry)
    {
      using (WorkItemTrackingMetadataComponent component = requestContext.CreateComponent<WorkItemTrackingMetadataComponent>())
      {
        Guid id = requestContext.WitContext().RequestIdentity.Id;
        component.RestoreField(dbFieldEntry.FieldId, id);
      }
      requestContext.ResetMetadataDbStamps();
      return requestContext.GetService<WorkItemTrackingFieldService>().GetFieldById(requestContext, dbFieldEntry.FieldId, new bool?(true));
    }

    public virtual FieldEntry SetFieldLocked(
      IVssRequestContext requestContext,
      FieldEntry dbFieldEntry,
      bool isLocked)
    {
      using (WorkItemTrackingMetadataComponent component = requestContext.CreateComponent<WorkItemTrackingMetadataComponent>())
      {
        Guid id = requestContext.WitContext().RequestIdentity.Id;
        component.SetFieldLocked(dbFieldEntry.FieldId, id, isLocked);
      }
      requestContext.ResetMetadataDbStamps();
      return requestContext.GetService<WorkItemTrackingFieldService>().GetFieldById(requestContext, dbFieldEntry.FieldId, new bool?(true));
    }

    private static void TraceFieldDictionary(
      IVssRequestContext requestContext,
      int tracepoint,
      string message)
    {
      requestContext.Trace(tracepoint, TraceLevel.Info, "Field", "FieldTypeDictionary", message);
    }

    protected class FieldEntryImpl : FieldEntry
    {
      internal FieldEntryImpl(FieldRecord fieldRecord)
      {
        this.FieldId = fieldRecord.Id;
        this.ReferenceName = fieldRecord.ReferenceName;
        this.Name = fieldRecord.Name;
        this.ParentFieldId = fieldRecord.ParentFieldId;
        this.FieldDataType = fieldRecord.FieldDataType;
        this.Usage = fieldRecord.Usages;
        this.IsCore = fieldRecord.IsCore;
        this.SupportsTextQuery = fieldRecord.SupportsTextQuery.GetValueOrDefault();
        this.OftenQueriedAsText = fieldRecord.OftenQueriedAsText.GetValueOrDefault();
        this.IsReportable = fieldRecord.IsReportable;
        this.ReportingType = fieldRecord.ReportingType;
        this.ReportingFormula = fieldRecord.ReportingFormula;
        this.ReportingReferenceName = fieldRecord.ReportingReferenceName;
        this.ReportingName = fieldRecord.ReportingName;
        this.IsIdentity = fieldRecord.IsIdentity;
        this.ProcessId = fieldRecord.ProcessId;
        this.Description = fieldRecord.Descriptor;
        this.PickListId = fieldRecord.PickListId;
        this.IsHistoryEnabled = fieldRecord.IsHistoryEnabled;
        this.IsDeleted = fieldRecord.IsDeleted;
        this.IsLocked = fieldRecord.IsLocked;
      }
    }

    protected class FieldTypeCacheServiceImpl : CacheSnapshotBase, IFieldTypeDictionary
    {
      private static readonly IReadOnlyCollection<string> ProhibitedHistoryDisabledFieldsReferenceNames = (IReadOnlyCollection<string>) new HashSet<string>(WorkItemTrackingFieldService.FieldTypeCacheServiceImpl.GetStringConstantsValues<DatabaseCoreFieldRefName>().Concat<string>(WorkItemTrackingFieldService.FieldTypeCacheServiceImpl.GetStringConstantsValues<AnalyticsRevisionedFieldReferenceName>()));
      private WorkItemTrackingFieldService m_fieldService;
      private WorkItemTrackingFieldService.FieldTypeCacheServiceImpl m_existingSnapshot;
      private IList<FieldEntry> m_fieldEntries;
      private long m_maxCacheStamp;
      private Dictionary<string, FieldEntry> m_mapByName;
      private Dictionary<int, FieldEntry> m_mapById;
      private List<FieldEntry> m_allFields;
      private List<FieldEntry> m_coreFields;
      private ISet<int> m_historyDisabledFieldIds;

      public FieldTypeCacheServiceImpl(
        IVssRequestContext requestContext,
        WorkItemTrackingFieldService fieldService,
        MetadataDBStamps stamps,
        WorkItemTrackingFieldService.FieldTypeCacheServiceImpl existingSnapshot)
        : base(stamps)
      {
        this.m_fieldService = fieldService;
        this.m_existingSnapshot = existingSnapshot;
        this.Initialize(requestContext);
      }

      private void Initialize(IVssRequestContext requestContext) => requestContext.TraceBlock(901510, 901519, 901518, "Dictionaries", "FieldTypeDictionary", "FieldTypeCacheServiceImpl.Initialize", (Action) (() =>
      {
        long sinceFieldCacheStamp = 0;
        if (this.m_existingSnapshot != null)
          this.m_existingSnapshot.Stamps.TryGetValue(MetadataTable.Fields, out sinceFieldCacheStamp);
        this.m_fieldEntries = this.m_fieldService.GetFieldEntries(requestContext, sinceFieldCacheStamp, out this.m_maxCacheStamp, true);
      }));

      internal override void MarkSnapshotForUse(
        IVssRequestContext requestContext,
        CacheSnapshotBase snapshotToReplace)
      {
        this.m_mapByName = new Dictionary<string, FieldEntry>((IEqualityComparer<string>) TFStringComparer.WorkItemFieldReferenceName);
        this.m_mapById = new Dictionary<int, FieldEntry>();
        if (this.m_existingSnapshot != null)
          this.CopyFromSnapshot(this.m_existingSnapshot);
        this.PopulateFieldEntries(requestContext);
        this.UpdateCacheCounters((WorkItemTrackingFieldService.FieldTypeCacheServiceImpl) snapshotToReplace);
      }

      private void CopyFromSnapshot(
        WorkItemTrackingFieldService.FieldTypeCacheServiceImpl existingSnapshot)
      {
        foreach (KeyValuePair<int, FieldEntry> keyValuePair in existingSnapshot.m_mapById)
          this.m_mapById[keyValuePair.Key] = keyValuePair.Value;
      }

      private void PopulateFieldEntries(IVssRequestContext requestContext)
      {
        foreach (FieldEntry fieldEntry1 in (IEnumerable<FieldEntry>) this.m_fieldEntries)
        {
          if (fieldEntry1.IsDeleted)
          {
            FieldEntry fieldEntry2;
            if (this.m_mapById.TryGetValue(fieldEntry1.FieldId, out fieldEntry2))
            {
              WorkItemTrackingFieldService.TraceFieldDictionary(requestContext, 901530, string.Format("Field cache: Remove name={0}, refname={1}, id={2}", (object) fieldEntry2.Name, (object) fieldEntry2.ReferenceName, (object) fieldEntry2.FieldId));
              this.m_mapById.Remove(fieldEntry2.FieldId);
            }
          }
          else
          {
            WorkItemTrackingFieldService.TraceFieldDictionary(requestContext, 901531, string.Format("Field cache: AddOrUpdate name={0}, refname={1}, id={2}", (object) fieldEntry1.Name, (object) fieldEntry1.ReferenceName, (object) fieldEntry1.FieldId));
            this.m_mapById[fieldEntry1.FieldId] = fieldEntry1;
          }
        }
        this.m_allFields = this.m_mapById.Values.ToList<FieldEntry>();
        this.m_coreFields = this.m_allFields.Where<FieldEntry>((Func<FieldEntry, bool>) (f => f.IsCore)).ToList<FieldEntry>();
        this.m_historyDisabledFieldIds = (ISet<int>) new HashSet<int>(this.m_allFields.Where<FieldEntry>((Func<FieldEntry, bool>) (f => !f.IsHistoryEnabled && !WorkItemTrackingFieldService.FieldTypeCacheServiceImpl.ProhibitedHistoryDisabledFieldsReferenceNames.Contains<string>(f.ReferenceName))).Select<FieldEntry, int>((Func<FieldEntry, int>) (f => f.FieldId)));
        foreach (FieldEntry allField in this.m_allFields)
        {
          this.m_mapByName[allField.ReferenceName] = allField;
          this.m_mapByName[allField.Name] = allField;
        }
        if (this.m_maxCacheStamp > 0L)
        {
          this.Stamps = new MetadataDBStamps((IDictionary<MetadataTable, long>) new Dictionary<MetadataTable, long>((IDictionary<MetadataTable, long>) this.Stamps)
          {
            [MetadataTable.Fields] = this.m_maxCacheStamp
          });
          requestContext.UpdateMetadataDbStampForTable(MetadataTable.Fields, this.m_maxCacheStamp);
        }
        WorkItemTrackingFieldService.TraceFieldDictionary(requestContext, 901532, string.Format("Field cache: m_allFields={0}, m_coreFields={1}", (object) this.m_allFields.Count, (object) this.m_coreFields.Count));
      }

      private void UpdateCacheCounters(
        WorkItemTrackingFieldService.FieldTypeCacheServiceImpl snapshotToReplace)
      {
        if (snapshotToReplace == null)
        {
          int count = this.m_allFields.Count;
          this.m_fieldService.PerfProvider.NotifyCacheItemsAdded(count, new MemoryCacheOperationStatistics(count, 0L));
        }
        else
        {
          int count1 = snapshotToReplace.m_allFields.Count<FieldEntry>((Func<FieldEntry, bool>) (field => !this.m_mapById.ContainsKey(field.FieldId)));
          int num = this.m_allFields.Count<FieldEntry>((Func<FieldEntry, bool>) (field => !snapshotToReplace.m_mapById.ContainsKey(field.FieldId)));
          int count2 = this.m_fieldEntries.Count<FieldEntry>((Func<FieldEntry, bool>) (field => !field.IsDeleted && snapshotToReplace.m_mapById.ContainsKey(field.FieldId)));
          if (count1 > 0)
            this.m_fieldService.PerfProvider.NotifyCacheItemsRemoved(count1, new MemoryCacheOperationStatistics(-count1, 0L));
          if (num > 0)
            this.m_fieldService.PerfProvider.NotifyCacheItemsAdded(num, new MemoryCacheOperationStatistics(num, 0L));
          if (count2 <= 0)
            return;
          this.m_fieldService.PerfProvider.NotifyCacheItemsReplaced(count2, new MemoryCacheOperationStatistics(0, 0L));
        }
      }

      private static IEnumerable<string> GetStringConstantsValues<T>() => ((IEnumerable<FieldInfo>) typeof (T).GetFields(BindingFlags.Static | BindingFlags.Public)).Where<FieldInfo>((Func<FieldInfo, bool>) (fi => fi.IsLiteral && !fi.IsInitOnly)).Select<FieldInfo, string>((Func<FieldInfo, string>) (fi => fi.GetRawConstantValue().ToString()));

      public bool TryGetField(string name, out FieldEntry field)
      {
        ArgumentUtility.CheckStringForNullOrEmpty(name, nameof (name));
        return this.m_mapByName.TryGetValue(name, out field);
      }

      public bool TryGetField(int id, out FieldEntry field) => this.m_mapById.TryGetValue(id, out field);

      public bool TryGetFieldByNameOrId(string nameOrId, out FieldEntry field)
      {
        int result;
        if (int.TryParse(nameOrId, out result))
          return this.TryGetField(result, out field);
        return this.TryGetField(nameOrId, out field);
      }

      public FieldEntry GetField(int id)
      {
        FieldEntry field;
        if (this.m_mapById.TryGetValue(id, out field))
          return field;
        throw new WorkItemTrackingFieldDefinitionNotFoundException(id);
      }

      public FieldEntry GetField(string name)
      {
        ArgumentUtility.CheckStringForNullOrEmpty(name, nameof (name));
        FieldEntry field;
        if (this.m_mapByName.TryGetValue(name, out field))
          return field;
        throw new WorkItemTrackingFieldDefinitionNotFoundException(name);
      }

      public FieldEntry GetFieldByNameOrId(string nameOrId)
      {
        int result;
        if (int.TryParse(nameOrId, out result))
          return this.GetField(result);
        FieldEntry field;
        if (this.TryGetField(nameOrId, out field))
          return field;
        throw new WorkItemTrackingFieldDefinitionNotFoundException(nameOrId);
      }

      public virtual IReadOnlyCollection<FieldEntry> GetAllFields() => (IReadOnlyCollection<FieldEntry>) this.m_allFields;

      public IReadOnlyCollection<FieldEntry> GetCoreFields() => (IReadOnlyCollection<FieldEntry>) this.m_coreFields;

      public ISet<int> GetHistoryDisabledFieldIds() => this.m_historyDisabledFieldIds;

      internal int GetFieldType(string fieldName)
      {
        FieldEntry fieldEntry;
        return !this.m_mapByName.TryGetValue(fieldName, out fieldEntry) ? -1 : fieldEntry.FieldDataType;
      }
    }
  }
}
