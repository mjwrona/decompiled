// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Compatibility.WorkItemMetadataCompatibilityService
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 815CF582-E66F-43C7-9B50-57E1C71BBC84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.dll

using Microsoft.Azure.Boards.ProcessTemplates;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.Compatibility;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Compatibility
{
  internal class WorkItemMetadataCompatibilityService : IVssFrameworkService
  {
    private Dictionary<MetadataTable, WorkItemMetadataCompatibilityService.CompatibilityHandler> m_compatHandler = new Dictionary<MetadataTable, WorkItemMetadataCompatibilityService.CompatibilityHandler>()
    {
      {
        MetadataTable.WorkItemTypeUsages,
        WorkItemMetadataCompatibilityService.\u003C\u003EO.\u003C0\u003E__GenerateWorkItemTypeUsages ?? (WorkItemMetadataCompatibilityService.\u003C\u003EO.\u003C0\u003E__GenerateWorkItemTypeUsages = new WorkItemMetadataCompatibilityService.CompatibilityHandler(WorkItemMetadataGenerator.GenerateWorkItemTypeUsages))
      },
      {
        MetadataTable.Rules,
        WorkItemMetadataCompatibilityService.\u003C\u003EO.\u003C1\u003E__GenerateRules ?? (WorkItemMetadataCompatibilityService.\u003C\u003EO.\u003C1\u003E__GenerateRules = new WorkItemMetadataCompatibilityService.CompatibilityHandler(WorkItemMetadataGenerator.GenerateRules))
      },
      {
        MetadataTable.HierarchyProperties,
        WorkItemMetadataCompatibilityService.\u003C\u003EO.\u003C2\u003E__GenerateProperties ?? (WorkItemMetadataCompatibilityService.\u003C\u003EO.\u003C2\u003E__GenerateProperties = new WorkItemMetadataCompatibilityService.CompatibilityHandler(WorkItemMetadataGenerator.GenerateProperties))
      },
      {
        MetadataTable.WorkItemTypes,
        WorkItemMetadataCompatibilityService.\u003C\u003EO.\u003C3\u003E__GenerateWorkItemTypes ?? (WorkItemMetadataCompatibilityService.\u003C\u003EO.\u003C3\u003E__GenerateWorkItemTypes = new WorkItemMetadataCompatibilityService.CompatibilityHandler(WorkItemMetadataGenerator.GenerateWorkItemTypes))
      },
      {
        MetadataTable.ConstantSets,
        WorkItemMetadataCompatibilityService.\u003C\u003EO.\u003C4\u003E__GenerateConstantSets ?? (WorkItemMetadataCompatibilityService.\u003C\u003EO.\u003C4\u003E__GenerateConstantSets = new WorkItemMetadataCompatibilityService.CompatibilityHandler(WorkItemMetadataGenerator.GenerateConstantSets))
      },
      {
        MetadataTable.WorkItemTypeCategories,
        WorkItemMetadataCompatibilityService.\u003C\u003EO.\u003C5\u003E__GenerateWorkItemTypeCategories ?? (WorkItemMetadataCompatibilityService.\u003C\u003EO.\u003C5\u003E__GenerateWorkItemTypeCategories = new WorkItemMetadataCompatibilityService.CompatibilityHandler(WorkItemMetadataGenerator.GenerateWorkItemTypeCategories))
      },
      {
        MetadataTable.WorkItemTypeCategoryMembers,
        WorkItemMetadataCompatibilityService.\u003C\u003EO.\u003C6\u003E__GenerateWorkItemTypeCategoryMembers ?? (WorkItemMetadataCompatibilityService.\u003C\u003EO.\u003C6\u003E__GenerateWorkItemTypeCategoryMembers = new WorkItemMetadataCompatibilityService.CompatibilityHandler(WorkItemMetadataGenerator.GenerateWorkItemTypeCategoryMembers))
      },
      {
        MetadataTable.Actions,
        WorkItemMetadataCompatibilityService.\u003C\u003EO.\u003C7\u003E__GenerateWorkItemTypeActions ?? (WorkItemMetadataCompatibilityService.\u003C\u003EO.\u003C7\u003E__GenerateWorkItemTypeActions = new WorkItemMetadataCompatibilityService.CompatibilityHandler(WorkItemMetadataGenerator.GenerateWorkItemTypeActions))
      }
    };
    private static readonly IReadOnlyCollection<MetadataTable> s_backCompatTables = (IReadOnlyCollection<MetadataTable>) new MetadataTable[8]
    {
      MetadataTable.HierarchyProperties,
      MetadataTable.Rules,
      MetadataTable.WorkItemTypes,
      MetadataTable.WorkItemTypeUsages,
      MetadataTable.ConstantSets,
      MetadataTable.WorkItemTypeCategories,
      MetadataTable.WorkItemTypeCategoryMembers,
      MetadataTable.Actions
    };
    private const string c_workItemTypeDescriptorsKey = "WorkItemMetadataCompatibilityService_WorkItemTypeDescriptors";

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public void GetMetadata(
      IVssRequestContext requestContext,
      MetadataTable[] tablesRequested,
      long[] rowVersions,
      Payload payload,
      out int locale,
      out int comparisonStyle,
      out string callerIdentity,
      out string dbStamp,
      out int mode,
      IWorkItemTrackingConfigurationInfo witConfigurationInfo)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<MetadataTable[]>(tablesRequested, nameof (tablesRequested));
      ArgumentUtility.CheckForNull<long[]>(rowVersions, nameof (rowVersions));
      ArgumentUtility.CheckForNull<Payload>(payload, nameof (payload));
      Stopwatch stopwatch = Stopwatch.StartNew();
      try
      {
        requestContext.TraceEnter(900799, "DataAccessLayer", "MetadataCompatibility", nameof (GetMetadata));
        string str = requestContext.GetService<IVssRegistryService>().GetValue<string>(requestContext, (RegistryQuery) "/Service/WorkItemTracking/Settings/GetMetadataRequestedProjects", true, (string) null);
        IEnumerable<Guid> guids;
        if (str == null)
        {
          guids = (IEnumerable<Guid>) null;
        }
        else
        {
          Guid result;
          guids = ((IEnumerable<string>) str.Split(';')).Select<string, Guid>((Func<string, Guid>) (projectGuidString => !Guid.TryParse(projectGuidString, out result) ? Guid.Empty : result)).Where<Guid>((Func<Guid, bool>) (guid => !guid.Equals(Guid.Empty)));
        }
        IEnumerable<Guid> projectsToLoad = guids;
        IDictionary<MetadataTable, WorkItemMetadataCompatibilityService.MetadataTableDescriptor> tablesToRegen;
        long[] adjustedRowVersions = this.GetAdjustedRowVersions(requestContext, tablesRequested, rowVersions, out tablesToRegen);
        MetadataCompatibilityContext compatContext = (MetadataCompatibilityContext) null;
        if (tablesToRegen.Any<KeyValuePair<MetadataTable, WorkItemMetadataCompatibilityService.MetadataTableDescriptor>>())
          compatContext = new MetadataCompatibilityContextGenerator().GetCompatibilityContext(requestContext, projectsToLoad: projectsToLoad);
        this.GetMetadataInternal(requestContext, tablesRequested, adjustedRowVersions, payload, out locale, out comparisonStyle, out callerIdentity, out dbStamp, out mode, witConfigurationInfo);
        this.RegeneratePayloadTables(requestContext, compatContext, payload, tablesToRegen);
      }
      finally
      {
        requestContext.TraceLeave(900800, "DataAccessLayer", "MetadataCompatibility", nameof (GetMetadata));
        stopwatch.Stop();
        requestContext.RequestTimer.LastTracedBlockSpan = stopwatch.Elapsed;
      }
    }

    internal void RegeneratePayloadTables(
      IVssRequestContext requestContext,
      MetadataCompatibilityContext compatContext,
      Payload payload,
      IDictionary<MetadataTable, WorkItemMetadataCompatibilityService.MetadataTableDescriptor> tablesToRegen)
    {
      requestContext.TraceBlock(900803, 900804, "DataAccessLayer", "MetadataCompatibility", nameof (RegeneratePayloadTables), (Action) (() =>
      {
        ArgumentUtility.CheckForNull<Payload>(payload, nameof (payload));
        ArgumentUtility.CheckForNull<IDictionary<MetadataTable, WorkItemMetadataCompatibilityService.MetadataTableDescriptor>>(tablesToRegen, nameof (tablesToRegen));
        if (compatContext != null)
          this.EnsureTypeIdsAreUnique(requestContext, compatContext);
        Dictionary<MetadataTable, int> dictionary = new Dictionary<MetadataTable, int>();
        int localNewMaskBits = 0;
        foreach (KeyValuePair<MetadataTable, WorkItemMetadataCompatibilityService.MetadataTableDescriptor> keyValuePair in (IEnumerable<KeyValuePair<MetadataTable, WorkItemMetadataCompatibilityService.MetadataTableDescriptor>>) tablesToRegen.OrderBy<KeyValuePair<MetadataTable, WorkItemMetadataCompatibilityService.MetadataTableDescriptor>, int>((Func<KeyValuePair<MetadataTable, WorkItemMetadataCompatibilityService.MetadataTableDescriptor>, int>) (t => this.GetTableOrder(t.Key))))
        {
          MetadataTable key = keyValuePair.Key;
          WorkItemMetadataCompatibilityService.MetadataTableDescriptor descriptor = keyValuePair.Value;
          WorkItemMetadataCompatibilityService.CompatibilityHandler handler;
          if (this.TryGetCompatibilityHandler(key, out handler) && handler != null)
          {
            string name = key.ToString();
            PayloadTable payloadTable = payload.Tables[name];
            requestContext.TraceBlock(900801, 900802, "DataAccessLayer", "MetadataCompatibility", string.Format("Generate{0}", (object) name), (Action) (() => handler(requestContext, compatContext, payloadTable, descriptor, out localNewMaskBits)));
            CustomerIntelligenceData properties = new CustomerIntelligenceData();
            properties.Add("TableName", name);
            properties.Add("ElapsedTime", requestContext.LastTracedBlockElapsedMilliseconds());
            requestContext.GetService<CustomerIntelligenceService>().Publish(requestContext, nameof (WorkItemMetadataCompatibilityService), nameof (RegeneratePayloadTables), properties);
            if (localNewMaskBits > descriptor.Bucket.MaskBits)
              dictionary[key] = localNewMaskBits;
          }
        }
        if (!dictionary.Any<KeyValuePair<MetadataTable, int>>())
          return;
        this.UpdateWorkItemMetadataInformation(requestContext, (IEnumerable<KeyValuePair<MetadataTable, int>>) dictionary);
      }));
    }

    protected virtual void EnsureTypeIdsAreUnique(
      IVssRequestContext requestContext,
      MetadataCompatibilityContext compatContext)
    {
      int provisionedWorkItemTypeId;
      using (WorkItemTrackingMetadataComponent component = requestContext.CreateComponent<WorkItemTrackingMetadataComponent>())
        provisionedWorkItemTypeId = component.GetMaxProvisionedWorkItemTypeId();
      compatContext.SetupFakeWorkItemTypeIds(provisionedWorkItemTypeId);
    }

    protected virtual void GetMetadataInternal(
      IVssRequestContext requestContext,
      MetadataTable[] tablesRequested,
      long[] rowVersions,
      Payload payload,
      out int locale,
      out int comparisonStyle,
      out string callerIdentity,
      out string dbStamp,
      out int mode,
      IWorkItemTrackingConfigurationInfo witConfigurationInfo)
    {
      try
      {
        requestContext.TraceEnter(900827, "DataAccessLayer", "MetadataCompatibility", nameof (GetMetadataInternal));
        locale = comparisonStyle = mode = 0;
        callerIdentity = dbStamp = (string) null;
        new DataAccessLayerImpl(requestContext).GetMetadataInternal((IVssIdentity) requestContext.GetUserIdentity(), tablesRequested, rowVersions, payload, out locale, out comparisonStyle, out callerIdentity, out dbStamp, out mode, witConfigurationInfo);
      }
      finally
      {
        requestContext.TraceLeave(900828, "DataAccessLayer", "MetadataCompatibility", nameof (GetMetadataInternal));
      }
    }

    private void LogCacheStamps(
      IVssRequestContext requestContext,
      MetadataTable[] tablesRequested,
      long[] rowVersions)
    {
      CustomerIntelligenceService service = requestContext.GetService<CustomerIntelligenceService>();
      CustomerIntelligenceData properties = new CustomerIntelligenceData();
      properties.Add(nameof (LogCacheStamps), true);
      for (int index = 0; index < tablesRequested.Length; ++index)
        properties.Add(tablesRequested[index].ToString("F"), (double) rowVersions[index]);
      service.Publish(requestContext, nameof (WorkItemMetadataCompatibilityService), WorkItemMetadataTelemetry.Feature, properties);
    }

    protected virtual int GetIntegerRepresentingAutoStampDbState(IVssRequestContext requestContext) => 0 + 1 + 2 + 4;

    protected virtual bool IsBackCompatEnabled(
      IVssRequestContext requestContext,
      MetadataTable table)
    {
      return this.m_compatHandler.ContainsKey(table) && WorkItemTrackingFeatureFlags.IsProcessCustomizationEnabled(requestContext);
    }

    protected virtual bool TryGetCompatibilityHandler(
      MetadataTable table,
      out WorkItemMetadataCompatibilityService.CompatibilityHandler handler)
    {
      return this.m_compatHandler.TryGetValue(table, out handler);
    }

    protected virtual MetadataInformation GetMetadataInformation(IVssRequestContext requestContext) => requestContext.TraceBlock<MetadataInformation>(900807, 900808, "DataAccessLayer", "MetadataCompatibility", nameof (GetMetadataInformation), (Func<MetadataInformation>) (() =>
    {
      using (MetadataCompatibilityComponent component = requestContext.CreateComponent<MetadataCompatibilityComponent>())
        return MetadataInformation.Create(component.GetMetadataInformation());
    }));

    internal LegacyCachestampDescriptor GetLegacyCachestampDescriptor(
      IVssRequestContext requestContext,
      MetadataTable table,
      long bucketId)
    {
      using (MetadataCompatibilityComponent component = requestContext.CreateComponent<MetadataCompatibilityComponent>())
        return component.GetWorkItemMetadataBucketToCachestampMapping(table, bucketId);
    }

    protected virtual void UpdateWorkItemMetadataInformation(
      IVssRequestContext requestContext,
      IEnumerable<KeyValuePair<MetadataTable, int>> newMaskBits)
    {
      requestContext.TraceBlock(900809, 900810, "DataAccessLayer", "MetadataCompatibility", nameof (UpdateWorkItemMetadataInformation), (Action) (() =>
      {
        using (MetadataCompatibilityComponent component = requestContext.CreateComponent<MetadataCompatibilityComponent>())
          component.UpdateWorkItemMetadataInformation(newMaskBits);
        this.ForceRefreshClientMetadata(requestContext);
      }));
    }

    internal virtual void ForceRefreshClientMetadata(IVssRequestContext requestContext)
    {
      using (WorkItemTrackingMetadataComponent component = requestContext.CreateComponent<WorkItemTrackingMetadataComponent>())
        component.StampDb();
      requestContext.GetService<CustomerIntelligenceService>().Publish(requestContext, "DataAccessLayer", "MetadataCompatibility", "GetAdjustedRowVersions", nameof (ForceRefreshClientMetadata));
      this.IncreaseWorkItemMetadataBucketIds(requestContext);
    }

    internal virtual void IncreaseWorkItemMetadataBucketIds(
      IVssRequestContext requestContext,
      IEnumerable<MetadataTable> metadataTypes = null)
    {
      requestContext.TraceBlock(900812, 900813, "DataAccessLayer", "MetadataCompatibility", nameof (IncreaseWorkItemMetadataBucketIds), (Action) (() =>
      {
        ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
        using (MetadataCompatibilityComponent component = requestContext.CreateComponent<MetadataCompatibilityComponent>())
          component.IncreaseWorkItemMetadataBucketIds(metadataTypes == null || !metadataTypes.Any<MetadataTable>() ? (IEnumerable<MetadataTable>) WorkItemMetadataCompatibilityService.s_backCompatTables : metadataTypes);
      }));
    }

    internal virtual IEnumerable<FormProperties> GetFormProperties(
      IVssRequestContext requestContext,
      MetadataCompatibilityContext compatContext)
    {
      return (IEnumerable<FormProperties>) requestContext.TraceBlock<List<FormProperties>>(900825, 900826, "DataAccessLayer", "MetadataCompatibility", nameof (GetFormProperties), (Func<List<FormProperties>>) (() =>
      {
        ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
        ArgumentUtility.CheckForNull<MetadataCompatibilityContext>(compatContext, nameof (compatContext));
        List<FormProperties> formProperties = new List<FormProperties>();
        Dictionary<Guid, Dictionary<string, string>> dictionary1 = new Dictionary<Guid, Dictionary<string, string>>();
        foreach (MetadataProjectCompatibilityDescriptor projectDescriptor in (IEnumerable<MetadataProjectCompatibilityDescriptor>) compatContext.ProjectDescriptors)
        {
          Dictionary<string, string> dictionary2 = (Dictionary<string, string>) null;
          ProcessDescriptor processDescriptor;
          if (projectDescriptor.TryGetProcessDescriptor(requestContext, out processDescriptor) && !dictionary1.TryGetValue(processDescriptor.TypeId, out dictionary2))
          {
            dictionary2 = new Dictionary<string, string>((IEqualityComparer<string>) TFStringComparer.WorkItemTypeName);
            dictionary1[processDescriptor.TypeId] = dictionary2;
          }
          foreach (MetadataWorkItemTypeCompatibilityDescriptor typeDescriptor in (IEnumerable<MetadataWorkItemTypeCompatibilityDescriptor>) projectDescriptor.TypeDescriptors)
          {
            string form;
            if (dictionary2 == null)
              form = typeDescriptor.Form;
            else if (!dictionary2.TryGetValue(typeDescriptor.Type.Name, out form))
            {
              form = typeDescriptor.Form;
              dictionary2[typeDescriptor.Type.Name] = form;
            }
            if (!string.IsNullOrEmpty(form))
              formProperties.Add(new FormProperties()
              {
                WorkItemTypeId = typeDescriptor.Type.Id.Value,
                ProjectId = projectDescriptor.ProjectNode.Id,
                Form = form
              });
          }
        }
        return formProperties;
      }));
    }

    internal long[] GetAdjustedRowVersions(
      IVssRequestContext requestContext,
      MetadataTable[] tablesRequested,
      long[] rowVersions,
      out IDictionary<MetadataTable, WorkItemMetadataCompatibilityService.MetadataTableDescriptor> tablesToRegen)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<MetadataTable[]>(tablesRequested, nameof (tablesRequested));
      ArgumentUtility.CheckForNull<long[]>(rowVersions, nameof (rowVersions));
      try
      {
        requestContext.TraceEnter(900805, "DataAccessLayer", "MetadataCompatibility", nameof (GetAdjustedRowVersions));
        MetadataInformation metadataInformation = this.GetMetadataInformation(requestContext);
        long[] adjustedRowVersions = new long[rowVersions.Length];
        tablesToRegen = (IDictionary<MetadataTable, WorkItemMetadataCompatibilityService.MetadataTableDescriptor>) new Dictionary<MetadataTable, WorkItemMetadataCompatibilityService.MetadataTableDescriptor>();
        for (int index1 = 0; index1 < tablesRequested.Length; ++index1)
        {
          adjustedRowVersions[index1] = rowVersions[index1];
          MetadataTable metadataTable = tablesRequested[index1];
          if (this.IsBackCompatEnabled(requestContext, metadataTable))
          {
            MetadataBucket bucket = metadataInformation[metadataTable];
            long bucketId = bucket.ParseBucketId(rowVersions[index1]);
            if (bucketId > bucket.Id)
            {
              tablesToRegen = (IDictionary<MetadataTable, WorkItemMetadataCompatibilityService.MetadataTableDescriptor>) new Dictionary<MetadataTable, WorkItemMetadataCompatibilityService.MetadataTableDescriptor>();
              CustomerIntelligenceData properties = new CustomerIntelligenceData();
              properties.Add("MetadataTable", metadataTable.ToString());
              properties.Add("ClientBucketId", (double) bucketId);
              properties.Add("BackCompatBucketId", (double) bucket.Id);
              properties.Add("BackCompatMaskBits", (double) bucket.MaskBits);
              properties.Add("ClientVersion", (double) requestContext.GetClientVersion());
              requestContext.GetService<CustomerIntelligenceService>().Publish(requestContext, nameof (WorkItemMetadataCompatibilityService), "ClientCacheStampTooHigh", properties);
              return rowVersions;
            }
            if (bucketId < bucket.Id || rowVersions[index1] == 0L)
            {
              LegacyCachestampDescriptor metadataBucketToCachestampMapping = (LegacyCachestampDescriptor) null;
              switch (metadataTable)
              {
                case MetadataTable.Rules:
                  if (WorkItemTrackingFeatureFlags.IsFullRuleGenerationEnabled(requestContext) || rowVersions[index1] > 0L && WorkItemTrackingFeatureFlags.IsProcessCustomizationEnabled(requestContext))
                  {
                    adjustedRowVersions[index1] = long.MaxValue;
                    break;
                  }
                  break;
                case MetadataTable.ConstantSets:
                  if (rowVersions[index1] > 0L && WorkItemTrackingFeatureFlags.IsProcessCustomizationEnabled(requestContext))
                  {
                    metadataBucketToCachestampMapping = this.GetLegacyCachestampDescriptor(requestContext, metadataTable, bucketId);
                    if (WorkItemTrackingFeatureFlags.IsProcessCustomizationEnabled(requestContext))
                    {
                      bool? identityOnlyChanges = metadataBucketToCachestampMapping.IsFollowedByIdentityOnlyChanges;
                      bool flag = true;
                      if (identityOnlyChanges.GetValueOrDefault() == flag & identityOnlyChanges.HasValue)
                      {
                        long? cachestamp = metadataBucketToCachestampMapping.Cachestamp;
                        if (cachestamp.HasValue)
                        {
                          long[] numArray = adjustedRowVersions;
                          int index2 = index1;
                          cachestamp = metadataBucketToCachestampMapping.Cachestamp;
                          long num = cachestamp.Value;
                          numArray[index2] = num;
                          break;
                        }
                      }
                    }
                    adjustedRowVersions[index1] = 1L;
                    break;
                  }
                  goto default;
                default:
                  adjustedRowVersions[index1] = 0L;
                  break;
              }
              tablesToRegen[metadataTable] = new WorkItemMetadataCompatibilityService.MetadataTableDescriptor(bucket, rowVersions[index1], metadataBucketToCachestampMapping);
            }
            else
              adjustedRowVersions[index1] = long.MaxValue;
          }
        }
        return adjustedRowVersions;
      }
      finally
      {
        requestContext.TraceLeave(900806, "DataAccessLayer", "MetadataCompatibility", nameof (GetAdjustedRowVersions));
      }
    }

    private int GetTableOrder(MetadataTable table)
    {
      switch (table)
      {
        case MetadataTable.Hierarchy:
          return 12;
        case MetadataTable.Fields:
          return 6;
        case MetadataTable.HierarchyProperties:
          return 2;
        case MetadataTable.Constants:
          return 0;
        case MetadataTable.Rules:
          return 10;
        case MetadataTable.ConstantSets:
          return 1;
        case MetadataTable.FieldUsages:
          return 7;
        case MetadataTable.WorkItemTypes:
          return 3;
        case MetadataTable.WorkItemTypeUsages:
          return 8;
        case MetadataTable.Actions:
          return 9;
        case MetadataTable.LinkTypes:
          return 11;
        case MetadataTable.WorkItemTypeCategories:
          return 4;
        case MetadataTable.WorkItemTypeCategoryMembers:
          return 5;
        case MetadataTable.NewWitFormLayout:
          return 14;
        case MetadataTable.Typelet:
          return 13;
        default:
          return -1;
      }
    }

    internal struct MetadataTableDescriptor
    {
      private MetadataBucket m_metadataBucket;
      private long m_userCacheStamp;
      private LegacyCachestampDescriptor m_legacyCachestampDescriptor;

      public MetadataTableDescriptor(
        MetadataBucket bucket,
        long userCacheStamp,
        LegacyCachestampDescriptor metadataBucketToCachestampMapping = null)
      {
        ArgumentUtility.CheckForNull<MetadataBucket>(bucket, nameof (bucket));
        this.m_metadataBucket = bucket;
        this.m_userCacheStamp = userCacheStamp;
        this.m_legacyCachestampDescriptor = metadataBucketToCachestampMapping;
      }

      public MetadataBucket Bucket => this.m_metadataBucket;

      public long UserCacheStamp => this.m_userCacheStamp;

      public LegacyCachestampDescriptor LegacyCachestampDescriptor => this.m_legacyCachestampDescriptor;
    }

    protected delegate void CompatibilityHandler(
      IVssRequestContext requestContext,
      MetadataCompatibilityContext compatContext,
      PayloadTable payloadTable,
      WorkItemMetadataCompatibilityService.MetadataTableDescriptor tableDescriptor,
      out int newMaskBits);
  }
}
