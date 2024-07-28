// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.WorkItemTypePropertiesService
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.Azure.Boards.ProcessTemplates;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.Compatibility;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.DataModels;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.Rules;
using Microsoft.TeamFoundation.WorkItemTracking.Server.ProcessMetadata;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata
{
  public class WorkItemTypePropertiesService : IWorkItemTypePropertiesService, IVssFrameworkService
  {
    private ConcurrentDictionary<string, MetadataDbStampedCacheEntry<AdditionalWorkItemTypeProperties>> m_additionalWorkItemTypePropertiesCache = new ConcurrentDictionary<string, MetadataDbStampedCacheEntry<AdditionalWorkItemTypeProperties>>((IEqualityComparer<string>) TFStringComparer.WorkItemTypeName);
    private ConcurrentDictionary<string, MetadataDbStampedCacheEntry<IEnumerable<RuleRecord>>> m_rulesCache = new ConcurrentDictionary<string, MetadataDbStampedCacheEntry<IEnumerable<RuleRecord>>>((IEqualityComparer<string>) TFStringComparer.WorkItemTypeName);

    void IVssFrameworkService.ServiceStart(IVssRequestContext systemRequestContext)
    {
      TeamFoundationSqlNotificationService notificationService = systemRequestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection) ? systemRequestContext.GetService<TeamFoundationSqlNotificationService>() : throw new UnexpectedHostTypeException(systemRequestContext.ServiceHost.HostType);
      notificationService.RegisterNotification(systemRequestContext, "Default", SqlNotificationEventClasses.ProjectsProcessMigrated, new SqlNotificationCallback(this.OnMigrateProjectsProcess), false);
      notificationService.RegisterNotification(systemRequestContext, "Default", DBNotificationIds.WorkItemStateDefinitionModified, new SqlNotificationCallback(this.OnStateCustomization), false);
    }

    void IVssFrameworkService.ServiceEnd(IVssRequestContext systemRequestContext)
    {
      TeamFoundationSqlNotificationService service = systemRequestContext.GetService<TeamFoundationSqlNotificationService>();
      service.UnregisterNotification(systemRequestContext, "Default", SqlNotificationEventClasses.ProjectsProcessMigrated, new SqlNotificationCallback(this.OnMigrateProjectsProcess), false);
      service.UnregisterNotification(systemRequestContext, "Default", DBNotificationIds.WorkItemStateDefinitionModified, new SqlNotificationCallback(this.OnStateCustomization), false);
    }

    private void OnMigrateProjectsProcess(
      IVssRequestContext requestContext,
      Guid eventClass,
      string eventData)
    {
      this.ClearCache();
      requestContext.GetService<WorkItemTrackingOutOfBoxRulesCache>().Clear(requestContext);
    }

    internal void OnStateCustomization(
      IVssRequestContext requestContext,
      Guid eventClass,
      string eventData)
    {
      this.m_additionalWorkItemTypePropertiesCache.Clear();
    }

    private static string ConstructCacheKey(int projectId, string workItemTypeName) => projectId.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo) + "/" + workItemTypeName;

    public AdditionalWorkItemTypeProperties GetWorkItemTypeDetails(
      IVssRequestContext requestContext,
      Guid projectId,
      string workItemTypeName,
      WitReadReplicaContext? readReplicaContext = null)
    {
      WorkItemType typeByReferenceName = requestContext.GetService<IWorkItemTypeService>().GetWorkItemTypeByReferenceName(requestContext, projectId, workItemTypeName);
      return this.GetWorkItemTypeDetails(requestContext.WitContext(), typeByReferenceName, readReplicaContext);
    }

    public virtual AdditionalWorkItemTypeProperties GetWorkItemTypeDetails(
      IVssRequestContext requestContext,
      WorkItemType workItemType,
      WitReadReplicaContext? readReplicaContext = null)
    {
      return this.GetWorkItemTypeDetails(requestContext.WitContext(), workItemType, readReplicaContext);
    }

    public AdditionalWorkItemTypeProperties GetCustomWorkItemTypeDetails(
      IVssRequestContext requestContext,
      WorkItemType workItemType,
      ProcessWorkItemType sourceWorkItemType)
    {
      IReadOnlyCollection<ProcessFieldDefinition> fieldDefinitions = requestContext.GetService<IProcessFieldService>().GetAllOutOfBoxFieldDefinitions(requestContext);
      IEnumerable<FieldEntry> fields = workItemType.GetFields(requestContext, true);
      Dictionary<int, string> helpTextByFieldID = new Dictionary<int, string>();
      foreach (FieldEntry fieldEntry in fields)
      {
        FieldEntry field = fieldEntry;
        ProcessFieldDefinition processFieldDefinition = fieldDefinitions.Where<ProcessFieldDefinition>((Func<ProcessFieldDefinition, bool>) (f => f.ReferenceName == field.ReferenceName)).FirstOrDefault<ProcessFieldDefinition>();
        if (processFieldDefinition != null && processFieldDefinition.HelpText != null && processFieldDefinition?.HelpText != "")
          helpTextByFieldID.Add(field.FieldId, processFieldDefinition.HelpText);
      }
      AdditionalWorkItemTypeProperties customWorkItemType = AdditionalWorkItemTypeProperties.CreateForCustomWorkItemType(requestContext, workItemType, sourceWorkItemType, (IDictionary<int, string>) helpTextByFieldID);
      AdditionalWorkItemTypeProperties.AugmentWithTypeletFieldRules(requestContext, customWorkItemType, sourceWorkItemType.ProcessId, workItemType.ReferenceName);
      return customWorkItemType;
    }

    internal virtual AdditionalWorkItemTypeProperties GetWorkItemTypeDetails(
      WorkItemTrackingRequestContext witRequestContext,
      WorkItemType workItemType,
      WitReadReplicaContext? readReplicaContext)
    {
      ArgumentUtility.CheckForNull<WorkItemTrackingRequestContext>(witRequestContext, nameof (witRequestContext));
      return witRequestContext.RequestContext.TraceBlock<AdditionalWorkItemTypeProperties>(911301, 911302, "WorkItemType", nameof (WorkItemTypePropertiesService), nameof (GetWorkItemTypeDetails), (Func<AdditionalWorkItemTypeProperties>) (() =>
      {
        List<MetadataTable> tableNames = new List<MetadataTable>()
        {
          MetadataTable.HierarchyProperties,
          MetadataTable.Rules,
          MetadataTable.Actions,
          MetadataTable.WorkItemTypes,
          MetadataTable.ConstantSetsNonIdentity,
          MetadataTable.ConstantsNonIdentity
        };
        ProcessDescriptor descriptor = (ProcessDescriptor) null;
        IWorkItemTrackingProcessService service = witRequestContext.RequestContext.GetService<IWorkItemTrackingProcessService>();
        if (WorkItemTrackingFeatureFlags.IsSharedProcessEnabled(witRequestContext.RequestContext) && service.TryGetLatestProjectProcessDescriptor(witRequestContext.RequestContext, workItemType.ProjectId, out descriptor) && (descriptor.IsSystem || descriptor.IsDerived))
        {
          if (WorkItemTrackingFeatureFlags.IsProcessCustomizationEnabled(witRequestContext.RequestContext))
            tableNames.Remove(MetadataTable.Rules);
          if (descriptor.IsDerived)
            tableNames.Add(MetadataTable.Typelet);
        }
        tableNames.Add(MetadataTable.ScopedIdentity);
        MetadataDBStamps stamps = witRequestContext.MetadataDbStamps((IEnumerable<MetadataTable>) tableNames);
        int projectId = witRequestContext.TreeService.GetTreeNode(workItemType.ProjectId, workItemType.ProjectId).Id;
        return this.m_additionalWorkItemTypePropertiesCache.AddOrUpdate(WorkItemTypePropertiesService.ConstructCacheKey(projectId, workItemType.Name), (Func<string, MetadataDbStampedCacheEntry<AdditionalWorkItemTypeProperties>>) (_ =>
        {
          witRequestContext.GetOrAddCacheItem<bool>("RulesCacheRefreshed", (Func<bool>) (() => true));
          return this.GetMetadataDbStampedCacheEntry(witRequestContext, workItemType, stamps, projectId, readReplicaContext, descriptor);
        }), (Func<string, MetadataDbStampedCacheEntry<AdditionalWorkItemTypeProperties>, MetadataDbStampedCacheEntry<AdditionalWorkItemTypeProperties>>) ((_, existing) =>
        {
          if (existing.IsFresh(stamps))
            return this.CreateCloneWithNewWorkItemType(witRequestContext, workItemType, existing.Data, stamps);
          witRequestContext.GetOrAddCacheItem<bool>("RulesCacheRefreshed", (Func<bool>) (() => true));
          return this.GetMetadataDbStampedCacheEntry(witRequestContext, workItemType, stamps, projectId, readReplicaContext, descriptor);
        })).Data;
      }));
    }

    private MetadataDbStampedCacheEntry<AdditionalWorkItemTypeProperties> CreateCloneWithNewWorkItemType(
      WorkItemTrackingRequestContext witRequestContext,
      WorkItemType newWorkItemType,
      AdditionalWorkItemTypeProperties existingProperties,
      MetadataDBStamps stamps)
    {
      return witRequestContext.RequestContext.TraceBlock<MetadataDbStampedCacheEntry<AdditionalWorkItemTypeProperties>>(911303, 911304, "WorkItemType", nameof (WorkItemTypePropertiesService), nameof (CreateCloneWithNewWorkItemType), (Func<MetadataDbStampedCacheEntry<AdditionalWorkItemTypeProperties>>) (() => new MetadataDbStampedCacheEntry<AdditionalWorkItemTypeProperties>(AdditionalWorkItemTypeProperties.CreateClone(witRequestContext.RequestContext, existingProperties, newWorkItemType), stamps)));
    }

    private MetadataDbStampedCacheEntry<AdditionalWorkItemTypeProperties> GetMetadataDbStampedCacheEntry(
      WorkItemTrackingRequestContext witRequestContext,
      WorkItemType workItemType,
      MetadataDBStamps stamps,
      int projectId,
      WitReadReplicaContext? readReplicaContext,
      ProcessDescriptor processDescriptor = null)
    {
      IVssRequestContext requestContext = witRequestContext.RequestContext;
      return requestContext.TraceBlock<MetadataDbStampedCacheEntry<AdditionalWorkItemTypeProperties>>(911305, 911306, "WorkItemType", nameof (WorkItemTypePropertiesService), nameof (GetMetadataDbStampedCacheEntry), (Func<MetadataDbStampedCacheEntry<AdditionalWorkItemTypeProperties>>) (() =>
      {
        IWorkItemRulesService service = requestContext.GetService<IWorkItemRulesService>();
        AdditionalWorkItemTypeProperties itemTypeProperties;
        if (WorkItemTrackingFeatureFlags.IsProcessCustomizationEnabled(requestContext) && processDescriptor != null && (processDescriptor.IsSystem || processDescriptor.IsDerived) && !workItemType.IsCustomType)
        {
          WorkItemRulesAndHelpTextsDescriptor rulesAndHelpTexts = service.GetOutOfBoxRulesAndHelpTexts(requestContext, processDescriptor, workItemType.ReferenceName);
          itemTypeProperties = AdditionalWorkItemTypeProperties.CreateWithFieldRules(requestContext, workItemType, (IEnumerable<WorkItemFieldRule>) rulesAndHelpTexts.Rules, rulesAndHelpTexts.HelpTexts);
          if (workItemType.InheritedWorkItemType != null)
            AdditionalWorkItemTypeProperties.AugmentWithTypeletFieldRules(requestContext, itemTypeProperties, processDescriptor.TypeId, workItemType.InheritedWorkItemType.ReferenceName);
        }
        else
        {
          IEnumerable<RuleRecord> rules = this.GetRules(requestContext, projectId, workItemType.Name, readReplicaContext);
          if (WorkItemTrackingFeatureFlags.IsDebugValidUserGroupEnabled(requestContext))
          {
            CustomerIntelligenceService ciService = requestContext.GetService<CustomerIntelligenceService>();
            rules.Where<RuleRecord>((Func<RuleRecord, bool>) (r => r.If2FldID == 0 && r.ThenFldID == 24)).ForEach<RuleRecord>((Action<RuleRecord>) (r =>
            {
              CustomerIntelligenceData properties = new CustomerIntelligenceData();
              properties.Add("Wit", string.Format("WitId: {0}, WitRefName: {1}, WitName: {2}", (object) workItemType.Id, (object) workItemType.ReferenceName, (object) workItemType.Name));
              properties.Add("ProjectId", (double) projectId);
              properties.Add("RuleDetail", r.ToDebugString());
              ciService.Publish(requestContext, nameof (WorkItemTypePropertiesService), nameof (GetMetadataDbStampedCacheEntry), properties);
            }));
          }
          itemTypeProperties = AdditionalWorkItemTypeProperties.CreateWithRuleRecords(requestContext, workItemType, readReplicaContext, rules);
        }
        return new MetadataDbStampedCacheEntry<AdditionalWorkItemTypeProperties>(itemTypeProperties, stamps);
      }));
    }

    internal virtual IEnumerable<RuleRecord> GetRules(
      IVssRequestContext requestContext,
      int projectId,
      string workItemTypeName,
      WitReadReplicaContext? readReplicaContext)
    {
      return requestContext.TraceBlock<IEnumerable<RuleRecord>>(911307, 911308, "WorkItemType", nameof (WorkItemTypePropertiesService), nameof (GetRules), (Func<IEnumerable<RuleRecord>>) (() => CompatibilityRulesGenerator.GetRules(requestContext, projectId, workItemTypeName)));
    }

    public void ClearCache()
    {
      this.m_additionalWorkItemTypePropertiesCache.Clear();
      this.m_rulesCache.Clear();
    }
  }
}
