// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayerImpl
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 815CF582-E66F-43C7-9B50-57E1C71BBC84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.dll

using Microsoft.Azure.Boards.CssNodes;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql;
using Microsoft.TeamFoundation.WorkItemTracking.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Internals;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Compatibility;
using Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer;
using Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.MetadataProcessing;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Identity;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.Compatibility;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.DataModels;
using Microsoft.VisualStudio.Services.CircuitBreaker;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Xml;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  internal class DataAccessLayerImpl : IDataAccessLayer
  {
    internal const string s_Area = "WorkItemTracking";
    internal const string s_MetadataFullCommandKey = "DataAccessLayerImpl.GetMetadata_Full";
    internal const string s_MetadataIncrementalCommandKey = "DataAccessLayerImpl.GetMetadata_Incremental";
    private IVssServiceHost m_serviceHost;
    private IVssRequestContext m_requestContext;
    private const int MetadataLockTimeout = 30000;
    private const int MetadataLockStaggerDuration = 30000;
    private const int c_maxStaleViewRetries = 1;
    private const int c_maxTvpMismatchRetries = 5;
    private const int c_maxColumnMismatchRetries = 5;
    private const int c_maxAreaIdChangedRetries = 3;
    private ProvisionHelper m_provisionHelper;
    internal static readonly CommandPropertiesSetter DefaultCommandPropertiesForGetMetadataFull = new CommandPropertiesSetter().WithCircuitBreakerDisabled(false).WithFallbackDisabled(false).WithCircuitBreakerForceClosed(false).WithCircuitBreakerForceOpen(false).WithCircuitBreakerRequestVolumeThreshold(20).WithCircuitBreakerErrorThresholdPercentage(50).WithCircuitBreakerMinBackoff(TimeSpan.FromSeconds(0.0)).WithCircuitBreakerMaxBackoff(TimeSpan.FromSeconds(30.0)).WithCircuitBreakerDeltaBackoff(TimeSpan.FromMilliseconds(300.0)).WithExecutionTimeout(TimeSpan.FromSeconds(120.0)).WithMetricsHealthSnapshotInterval(TimeSpan.FromSeconds(0.5)).WithMetricsRollingStatisticalWindowInMilliseconds(10000).WithMetricsRollingStatisticalWindowBuckets(10).WithExecutionMaxConcurrentRequests(8);
    internal static readonly CommandPropertiesSetter DefaultCommandPropertiesForGetMetadataIncremental = new CommandPropertiesSetter().WithCircuitBreakerDisabled(false).WithFallbackDisabled(false).WithCircuitBreakerForceClosed(false).WithCircuitBreakerForceOpen(false).WithCircuitBreakerRequestVolumeThreshold(20).WithCircuitBreakerErrorThresholdPercentage(50).WithCircuitBreakerMinBackoff(TimeSpan.FromSeconds(0.0)).WithCircuitBreakerMaxBackoff(TimeSpan.FromSeconds(30.0)).WithCircuitBreakerDeltaBackoff(TimeSpan.FromMilliseconds(300.0)).WithExecutionTimeout(TimeSpan.FromSeconds(120.0)).WithMetricsHealthSnapshotInterval(TimeSpan.FromSeconds(0.5)).WithMetricsRollingStatisticalWindowInMilliseconds(10000).WithMetricsRollingStatisticalWindowBuckets(10).WithExecutionMaxConcurrentRequests(16);
    internal static readonly Dictionary<MetadataTable, PayloadTableSchema> MetadataPayloadTableSchemas = new Dictionary<MetadataTable, PayloadTableSchema>()
    {
      {
        MetadataTable.Hierarchy,
        new PayloadTableSchema(new PayloadTableSchema.Column[9]
        {
          new PayloadTableSchema.Column("AreaID", typeof (int)),
          new PayloadTableSchema.Column("fDeleted", typeof (bool)),
          new PayloadTableSchema.Column("TypeID", typeof (int)),
          new PayloadTableSchema.Column("Name", typeof (string)),
          new PayloadTableSchema.Column("ParentID", typeof (int)),
          new PayloadTableSchema.Column("fAdminOnly", typeof (bool)),
          new PayloadTableSchema.Column("StructureType", typeof (int)),
          new PayloadTableSchema.Column("GUID", typeof (string)),
          new PayloadTableSchema.Column("CacheStamp", typeof (ulong))
        })
      },
      {
        MetadataTable.Fields,
        new PayloadTableSchema(new PayloadTableSchema.Column[15]
        {
          new PayloadTableSchema.Column("FldID", typeof (int)),
          new PayloadTableSchema.Column("Type", typeof (int)),
          new PayloadTableSchema.Column("ParentFldID", typeof (int)),
          new PayloadTableSchema.Column("Name", typeof (string)),
          new PayloadTableSchema.Column("ReferenceName", typeof (string)),
          new PayloadTableSchema.Column("fEditable", typeof (bool)),
          new PayloadTableSchema.Column("fSemiEditable", typeof (bool)),
          new PayloadTableSchema.Column("ReportingType", typeof (int)),
          new PayloadTableSchema.Column("ReportingFormula", typeof (int)),
          new PayloadTableSchema.Column("ReportingName", typeof (string)),
          new PayloadTableSchema.Column("ReportingReferenceName", typeof (string)),
          new PayloadTableSchema.Column("fReportingEnabled", typeof (bool)),
          new PayloadTableSchema.Column("Cachestamp", typeof (ulong)),
          new PayloadTableSchema.Column("fDeleted", typeof (bool)),
          new PayloadTableSchema.Column("fSupportsTextQuery", typeof (bool))
        })
      },
      {
        MetadataTable.HierarchyProperties,
        new PayloadTableSchema(new PayloadTableSchema.Column[8]
        {
          new PayloadTableSchema.Column("PropID", typeof (int)),
          new PayloadTableSchema.Column("AreaID", typeof (int)),
          new PayloadTableSchema.Column("TreeType", typeof (int)),
          new PayloadTableSchema.Column("Name", typeof (string)),
          new PayloadTableSchema.Column("Value", typeof (string)),
          new PayloadTableSchema.Column("fDeleted", typeof (bool)),
          new PayloadTableSchema.Column("CacheStamp", typeof (ulong)),
          new PayloadTableSchema.Column("WorkItemTypeID", typeof (int))
        })
      },
      {
        MetadataTable.Constants,
        new PayloadTableSchema(new PayloadTableSchema.Column[6]
        {
          new PayloadTableSchema.Column("ConstID", typeof (int)),
          new PayloadTableSchema.Column("DisplayName", typeof (string)),
          new PayloadTableSchema.Column("String", typeof (string)),
          new PayloadTableSchema.Column("fDeleted", typeof (bool)),
          new PayloadTableSchema.Column("Sid", typeof (string)),
          new PayloadTableSchema.Column("Cachestamp", typeof (ulong))
        })
      },
      {
        MetadataTable.Rules,
        new PayloadTableSchema(new PayloadTableSchema.Column[27]
        {
          new PayloadTableSchema.Column("RuleID", typeof (int)),
          new PayloadTableSchema.Column("RootTreeID", typeof (int)),
          new PayloadTableSchema.Column("AreaID", typeof (int)),
          new PayloadTableSchema.Column("PersonID", typeof (int)),
          new PayloadTableSchema.Column("ObjectTypeScopeID", typeof (int)),
          new PayloadTableSchema.Column("Fld1ID", typeof (int)),
          new PayloadTableSchema.Column("Fld1IsConstID", typeof (int)),
          new PayloadTableSchema.Column("Fld1WasConstID", typeof (int)),
          new PayloadTableSchema.Column("Fld2ID", typeof (int)),
          new PayloadTableSchema.Column("Fld2IsConstID", typeof (int)),
          new PayloadTableSchema.Column("Fld2WasConstID", typeof (int)),
          new PayloadTableSchema.Column("Fld3ID", typeof (int)),
          new PayloadTableSchema.Column("Fld3IsConstID", typeof (int)),
          new PayloadTableSchema.Column("Fld3WasConstID", typeof (int)),
          new PayloadTableSchema.Column("Fld4ID", typeof (int)),
          new PayloadTableSchema.Column("Fld4IsConstID", typeof (int)),
          new PayloadTableSchema.Column("Fld4WasConstID", typeof (int)),
          new PayloadTableSchema.Column("IfFldID", typeof (int)),
          new PayloadTableSchema.Column("IfConstID", typeof (int)),
          new PayloadTableSchema.Column("If2FldID", typeof (int)),
          new PayloadTableSchema.Column("If2ConstID", typeof (int)),
          new PayloadTableSchema.Column("ThenFldID", typeof (int)),
          new PayloadTableSchema.Column("ThenConstID", typeof (int)),
          new PayloadTableSchema.Column("RuleFlags1", typeof (int)),
          new PayloadTableSchema.Column("RuleFlags2", typeof (int)),
          new PayloadTableSchema.Column("fDeleted", typeof (bool)),
          new PayloadTableSchema.Column("Cachestamp", typeof (ulong))
        })
      },
      {
        MetadataTable.ConstantSets,
        new PayloadTableSchema(new PayloadTableSchema.Column[5]
        {
          new PayloadTableSchema.Column("RuleSetID", typeof (int)),
          new PayloadTableSchema.Column("ParentID", typeof (int)),
          new PayloadTableSchema.Column("ConstID", typeof (int)),
          new PayloadTableSchema.Column("fDeleted", typeof (bool)),
          new PayloadTableSchema.Column("Cachestamp", typeof (ulong))
        })
      },
      {
        MetadataTable.FieldUsages,
        new PayloadTableSchema(new PayloadTableSchema.Column[10]
        {
          new PayloadTableSchema.Column("FldUsageID", typeof (int)),
          new PayloadTableSchema.Column("fDeleted", typeof (bool)),
          new PayloadTableSchema.Column("ObjectID", typeof (int)),
          new PayloadTableSchema.Column("FldID", typeof (int)),
          new PayloadTableSchema.Column("DirectObjectID", typeof (int)),
          new PayloadTableSchema.Column("fOftenQueried", typeof (bool)),
          new PayloadTableSchema.Column("fCore", typeof (bool)),
          new PayloadTableSchema.Column("Cachestamp", typeof (ulong)),
          new PayloadTableSchema.Column("fOftenQueriedAsText", typeof (bool)),
          new PayloadTableSchema.Column("fSupportsTextQuery", typeof (bool))
        })
      },
      {
        MetadataTable.WorkItemTypes,
        new PayloadTableSchema(new PayloadTableSchema.Column[6]
        {
          new PayloadTableSchema.Column("WorkItemTypeID", typeof (int)),
          new PayloadTableSchema.Column("NameConstantID", typeof (int)),
          new PayloadTableSchema.Column("ProjectID", typeof (int)),
          new PayloadTableSchema.Column("DescriptionID", typeof (int)),
          new PayloadTableSchema.Column("fDeleted", typeof (bool)),
          new PayloadTableSchema.Column("Cachestamp", typeof (ulong))
        })
      },
      {
        MetadataTable.WorkItemTypeUsages,
        new PayloadTableSchema(new PayloadTableSchema.Column[6]
        {
          new PayloadTableSchema.Column("WorkItemTypeUsageID", typeof (int)),
          new PayloadTableSchema.Column("FieldID", typeof (int)),
          new PayloadTableSchema.Column("WorkItemTypeID", typeof (int)),
          new PayloadTableSchema.Column("fGreyOut", typeof (bool)),
          new PayloadTableSchema.Column("fDeleted", typeof (bool)),
          new PayloadTableSchema.Column("Cachestamp", typeof (ulong))
        })
      },
      {
        MetadataTable.Actions,
        new PayloadTableSchema(new PayloadTableSchema.Column[7]
        {
          new PayloadTableSchema.Column("ActionID", typeof (int)),
          new PayloadTableSchema.Column("Name", typeof (string)),
          new PayloadTableSchema.Column("WorkItemTypeID", typeof (int)),
          new PayloadTableSchema.Column("FromStateConstID", typeof (int)),
          new PayloadTableSchema.Column("ToStateConstID", typeof (int)),
          new PayloadTableSchema.Column("fDeleted", typeof (bool)),
          new PayloadTableSchema.Column("Cachestamp", typeof (ulong))
        })
      },
      {
        MetadataTable.LinkTypes,
        new PayloadTableSchema(new PayloadTableSchema.Column[8]
        {
          new PayloadTableSchema.Column("ReferenceName", typeof (string)),
          new PayloadTableSchema.Column("ForwardName", typeof (string)),
          new PayloadTableSchema.Column("ForwardID", typeof (short)),
          new PayloadTableSchema.Column("ReverseName", typeof (string)),
          new PayloadTableSchema.Column("ReverseID", typeof (short)),
          new PayloadTableSchema.Column("Rules", typeof (int)),
          new PayloadTableSchema.Column("fDeleted", typeof (bool)),
          new PayloadTableSchema.Column("CacheStamp", typeof (ulong))
        })
      },
      {
        MetadataTable.WorkItemTypeCategories,
        new PayloadTableSchema(new PayloadTableSchema.Column[7]
        {
          new PayloadTableSchema.Column("WorkItemTypeCategoryID", typeof (int)),
          new PayloadTableSchema.Column("ProjectID", typeof (int)),
          new PayloadTableSchema.Column("Name", typeof (string)),
          new PayloadTableSchema.Column("ReferenceName", typeof (string)),
          new PayloadTableSchema.Column("DefaultWorkItemTypeID", typeof (int)),
          new PayloadTableSchema.Column("fDeleted", typeof (bool)),
          new PayloadTableSchema.Column("Cachestamp", typeof (ulong))
        })
      },
      {
        MetadataTable.WorkItemTypeCategoryMembers,
        new PayloadTableSchema(new PayloadTableSchema.Column[5]
        {
          new PayloadTableSchema.Column("WorkItemTypeCategoryMemberID", typeof (int)),
          new PayloadTableSchema.Column("WorkItemTypeCategoryID", typeof (int)),
          new PayloadTableSchema.Column("WorkItemTypeID", typeof (int)),
          new PayloadTableSchema.Column("fDeleted", typeof (bool)),
          new PayloadTableSchema.Column("Cachestamp", typeof (ulong))
        })
      }
    };
    internal static Action<PayloadTable, long> AddDummyFieldRowToTable = (Action<PayloadTable, long>) ((table, newCacheStamp) =>
    {
      string str1 = "ZZZWITDummyFieldZZZ";
      string str2 = "System.ZZZWITDummyFieldZZZ";
      PayloadTable.PayloadRow payloadRow = table.AddNewPayloadRow();
      payloadRow[0] = (object) -58;
      payloadRow[1] = (object) 226;
      payloadRow[2] = (object) 224;
      payloadRow[3] = (object) str1;
      payloadRow[4] = (object) str2;
      payloadRow[5] = (object) false;
      payloadRow[6] = (object) false;
      payloadRow[7] = (object) 0;
      payloadRow[8] = (object) 0;
      payloadRow[9] = (object) str1;
      payloadRow[10] = (object) str2;
      payloadRow[11] = (object) false;
      payloadRow[12] = (object) (ulong) newCacheStamp;
      payloadRow[13] = (object) true;
      payloadRow[14] = (object) false;
    });
    internal static Action<PayloadTable, long> AddDummyFieldUsageRowToTable = (Action<PayloadTable, long>) ((table, newCacheStamp) =>
    {
      PayloadTable.PayloadRow payloadRow = table.AddNewPayloadRow();
      payloadRow[0] = (object) -1;
      payloadRow[1] = (object) true;
      payloadRow[2] = (object) -100;
      payloadRow[3] = (object) -58;
      payloadRow[4] = (object) 0;
      payloadRow[5] = (object) false;
      payloadRow[6] = (object) false;
      payloadRow[7] = (object) (ulong) newCacheStamp;
      payloadRow[8] = (object) false;
      payloadRow[9] = (object) false;
    });
    internal static Action<PayloadTable, long> AddDummyLinkTypeRowToTable = (Action<PayloadTable, long>) ((table, newCacheStamp) =>
    {
      string str3 = "ZZZWITDummyLinkTypeZZZ";
      string str4 = "System.LinkTypes.ZZZWITDummyLinkTypeZZZ";
      PayloadTable.PayloadRow payloadRow = table.AddNewPayloadRow();
      payloadRow[0] = (object) str4;
      payloadRow[1] = (object) str3;
      payloadRow[2] = (object) (short) 5000;
      payloadRow[3] = (object) str3;
      payloadRow[4] = (object) (short) 5000;
      payloadRow[5] = (object) 3;
      payloadRow[6] = (object) true;
      payloadRow[7] = (object) (ulong) newCacheStamp;
    });
    internal static Action<PayloadTable, long> AddDummyHierarchyPropertiesRowToTable = (Action<PayloadTable, long>) ((table, newCacheStamp) =>
    {
      PayloadTable.PayloadRow payloadRow = table.AddNewPayloadRow();
      payloadRow[0] = (object) -1;
      payloadRow[1] = (object) -1;
      payloadRow[2] = (object) 99;
      payloadRow[3] = (object) "ZZZDummyTreePropertyZZZ";
      payloadRow[4] = (object) "";
      payloadRow[5] = (object) true;
      payloadRow[6] = (object) (ulong) newCacheStamp;
      payloadRow[7] = (object) -1;
    });
    internal static Action<PayloadTable, long> AddDummyConstantsRowToTable = (Action<PayloadTable, long>) ((table, newCacheStamp) =>
    {
      PayloadTable.PayloadRow payloadRow = table.AddNewPayloadRow();
      payloadRow[0] = (object) -1;
      payloadRow[1] = (object) "ZZZDummyConstZZZ";
      payloadRow[2] = (object) "ZZZDummyConstZZZ";
      payloadRow[3] = (object) true;
      payloadRow[4] = (object) null;
      payloadRow[5] = (object) (ulong) newCacheStamp;
    });
    internal static Action<PayloadTable, long> AddDummyRulesRowToTable = (Action<PayloadTable, long>) ((table, newCacheStamp) =>
    {
      PayloadTable.PayloadRow payloadRow = table.AddNewPayloadRow();
      payloadRow[0] = (object) -1;
      payloadRow[1] = (object) -1;
      payloadRow[2] = (object) -1;
      payloadRow[3] = (object) -1;
      payloadRow[4] = (object) -1;
      payloadRow[5] = (object) -1;
      payloadRow[6] = (object) -1;
      payloadRow[7] = (object) -1;
      payloadRow[8] = (object) -1;
      payloadRow[9] = (object) -1;
      payloadRow[10] = (object) -1;
      payloadRow[11] = (object) -1;
      payloadRow[12] = (object) -1;
      payloadRow[13] = (object) -1;
      payloadRow[14] = (object) -1;
      payloadRow[15] = (object) -1;
      payloadRow[16] = (object) -1;
      payloadRow[17] = (object) -1;
      payloadRow[18] = (object) -1;
      payloadRow[19] = (object) -1;
      payloadRow[20] = (object) -1;
      payloadRow[21] = (object) -1;
      payloadRow[22] = (object) -1;
      payloadRow[23] = (object) -1;
      payloadRow[24] = (object) -1;
      payloadRow[25] = (object) true;
      payloadRow[26] = (object) (ulong) newCacheStamp;
    });
    internal static Action<PayloadTable, long> AddDummyConstantSetsToTable = (Action<PayloadTable, long>) ((table, newCacheStamp) =>
    {
      PayloadTable.PayloadRow payloadRow = table.AddNewPayloadRow();
      payloadRow[0] = (object) -1;
      payloadRow[1] = (object) -1;
      payloadRow[2] = (object) -1;
      payloadRow[3] = (object) true;
      payloadRow[4] = (object) (ulong) newCacheStamp;
    });
    internal static Action<PayloadTable, long> AddDummyWorkItemTypesRowToTable = (Action<PayloadTable, long>) ((table, newCacheStamp) =>
    {
      PayloadTable.PayloadRow payloadRow = table.AddNewPayloadRow();
      payloadRow[0] = (object) -1;
      payloadRow[1] = (object) -1;
      payloadRow[2] = (object) -1;
      payloadRow[3] = (object) -1;
      payloadRow[4] = (object) true;
      payloadRow[5] = (object) (ulong) newCacheStamp;
    });
    internal static Action<PayloadTable, long> AddDummyWorkItemTypeUsagesRowToTable = (Action<PayloadTable, long>) ((table, newCacheStamp) =>
    {
      PayloadTable.PayloadRow payloadRow = table.AddNewPayloadRow();
      payloadRow[0] = (object) -1;
      payloadRow[1] = (object) -1;
      payloadRow[2] = (object) -1;
      payloadRow[3] = (object) true;
      payloadRow[4] = (object) true;
      payloadRow[5] = (object) (ulong) newCacheStamp;
    });

    public DataAccessLayerImpl(IVssRequestContext requestContext)
    {
      this.m_requestContext = requestContext;
      this.m_serviceHost = requestContext.ServiceHost;
    }

    public void GetMetadata(
      IVssIdentity user,
      MetadataTable[] tablesRequested,
      long[] rowVersions,
      Payload metadataPayload,
      out int locale,
      out int comparisonStyle,
      out string callerIdentity,
      out string dbStamp,
      out int mode)
    {
      long totalMilliseconds = (long) this.m_requestContext.RequestTimer.ExecutionSpan.TotalMilliseconds;
      this.m_requestContext.TraceEnter(900023, "DataAccessLayer", nameof (DataAccessLayerImpl), nameof (GetMetadata));
      this.m_requestContext.Trace(900109, TraceLevel.Verbose, "DataAccessLayer", nameof (DataAccessLayerImpl), "Server:{0}, ClientUserVsid:{1}", (object) this.m_requestContext.VirtualPath(), (object) user?.Id);
      DataAccessLayerImpl.ValidateArgsObjectNotNull((object) user, "clientUser", "InvalidUserException");
      this.ValidateArgsRequireMetadataParms(tablesRequested, rowVersions, metadataPayload);
      IWorkItemTrackingConfigurationInfo configurationInfo = this.m_requestContext.GetService<WorkItemTrackingConfigurationSettingService>().GetConfigurationInfo(this.m_requestContext);
      bool flag1 = ((IEnumerable<long>) rowVersions).Any<long>((Func<long, bool>) (rv => rv != 0L));
      bool flag2 = false;
      if (!flag1 && !configurationInfo.MetadataFilterEnabled)
        flag2 = this.m_requestContext.IsFeatureEnabled("WorkItemTracking.Server.EnableMetadataFileCache");
      this.m_requestContext.Trace(909703, TraceLevel.Info, "DataAccessLayer", "DataAccessLayerMetadataCaching", "UseCache:{0},Incremental{0}", (object) flag2, (object) flag1);
      try
      {
        VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.PerformanceCounters.GetMetadataRequests").Increment();
        if (flag2)
          this.GetCachedMetadata(user, tablesRequested, rowVersions, metadataPayload, out locale, out comparisonStyle, out callerIdentity, out dbStamp, out mode, configurationInfo);
        else if (WorkItemTrackingFeatureFlags.IsProcessCustomizationEnabled(this.m_requestContext))
          this.m_requestContext.GetService<WorkItemMetadataCompatibilityService>().GetMetadata(this.m_requestContext, tablesRequested, rowVersions, metadataPayload, out locale, out comparisonStyle, out callerIdentity, out dbStamp, out mode, configurationInfo);
        else
          this.GetMetadataInternal(user, tablesRequested, rowVersions, metadataPayload, out locale, out comparisonStyle, out callerIdentity, out dbStamp, out mode, configurationInfo);
        this.m_requestContext.Trace(900110, TraceLevel.Verbose, "DataAccessLayer", nameof (DataAccessLayerImpl), "Local:{0},Comparison:{1},Identity:{2}", (object) locale.ToString((IFormatProvider) CultureInfo.InvariantCulture), (object) comparisonStyle.ToString((IFormatProvider) CultureInfo.InvariantCulture), (object) callerIdentity);
      }
      finally
      {
        VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.PerformanceCounters.GetMetadataRequests").Decrement();
        this.m_requestContext.TraceLeave(900536, "DataAccessLayer", nameof (DataAccessLayerImpl), nameof (GetMetadata));
      }
      WorkItemKpiTracer.TraceKpi(this.m_requestContext, (WorkItemTrackingKpi) new GetMetadataTimeKpi(this.m_requestContext, !flag1, this.m_requestContext.RequestTimer.ExecutionSpan.TotalMilliseconds - (double) totalMilliseconds));
      WorkItemTrackingTelemetry.TraceCustomerIntelligence(this.m_requestContext, WorkItemMetadataTelemetry.Feature, (object) flag1, (object) this.GetTableSizes(tablesRequested, metadataPayload));
    }

    internal void GetMetadataInternal(
      IVssIdentity user,
      MetadataTable[] tablesRequested,
      long[] rowVersions,
      Payload metadataPayload,
      out int locale,
      out int comparisonStyle,
      out string callerIdentity,
      out string dbStamp,
      out int mode,
      IWorkItemTrackingConfigurationInfo witConfigurationInfo)
    {
      bool incrementalRequest = rowVersions == null || ((IEnumerable<long>) rowVersions).Any<long>((Func<long, bool>) (rv => rv != 0L));
      string type = incrementalRequest ? "incremental" : "full";
      bool metadataStampsChecked = false;
      MetadataDBStamps userMetadataDbStamps = (MetadataDBStamps) null;
      bool metadataAlreadyUpToDate = this.IsMetadataUptodate(tablesRequested, rowVersions, metadataPayload, incrementalRequest, witConfigurationInfo, out metadataStampsChecked, out userMetadataDbStamps);
      using (SqlBatchBuilder sqlBatch = (SqlBatchBuilder) this.CreateSqlBatch())
      {
        Func<DataAccessLayerImpl.GetMetadataOutputParameters> run = (Func<DataAccessLayerImpl.GetMetadataOutputParameters>) (() =>
        {
          DataAccessLayerImpl.GetMetadataOutputParameters metadataInternal = new DataAccessLayerImpl.GetMetadataOutputParameters();
          this.GetSqlElement<DalPersonIdElement>(sqlBatch).JoinBatch((ElementGroup) null, user);
          DalGetPersonNameElement sqlElement1 = this.GetSqlElement<DalGetPersonNameElement>(sqlBatch);
          sqlElement1.JoinBatch((ElementGroup) null);
          DalGetLocaleElement sqlElement2 = this.GetSqlElement<DalGetLocaleElement>(sqlBatch);
          sqlElement2.JoinBatch();
          int resultTableCount = sqlBatch.ResultTableCount;
          DalGetDbStampElement getDbStampElement = (DalGetDbStampElement) null;
          int num = metadataPayload == null ? 0 : (!metadataAlreadyUpToDate ? 1 : 0);
          if (num == 0 || metadataPayload == null || !witConfigurationInfo.MetadataFilterEnabled || sqlBatch.Version < 9)
          {
            getDbStampElement = this.GetSqlElement<DalGetDbStampElement>(sqlBatch);
            getDbStampElement.JoinBatch();
          }
          DalMetadataSelectElement metadataSelectElement = (DalMetadataSelectElement) null;
          if (num != 0)
          {
            metadataSelectElement = this.GetSqlElement<DalMetadataSelectElement>(sqlBatch);
            metadataSelectElement.JoinBatch(tablesRequested, rowVersions);
          }
          sqlBatch.ExecuteBatch(int.MaxValue);
          metadataInternal.locale = sqlElement2.GetLocale();
          metadataInternal.mode = 2;
          metadataInternal.comparisonStyle = sqlElement2.GetComparisonStyle();
          metadataInternal.callerIdentity = sqlElement1.GetPersonName();
          metadataInternal.dbStamp = getDbStampElement == null ? metadataSelectElement.GetDbStamp() : getDbStampElement.GetDbStamp();
          if (metadataSelectElement != null)
          {
            metadataSelectElement.GetResults(metadataPayload);
            this.AttachMetadataPayloadProcessors(metadataPayload, sqlBatch.Version);
            this.AddDummyRecordsToPayload(userMetadataDbStamps, metadataPayload);
          }
          else if (metadataPayload != null & metadataAlreadyUpToDate)
            this.FillEmptyMetadataSchemaRows(sqlBatch, tablesRequested, metadataPayload);
          return metadataInternal;
        });
        string str = (incrementalRequest ? "DataAccessLayerImpl.GetMetadata_Incremental" : "DataAccessLayerImpl.GetMetadata_Full") + DataAccessLayerImpl.RemoveIllegalCharactersFromCommandKey(sqlBatch.Component.InitialCatalog) + (sqlBatch.Component.ApplicationIntent == ApplicationIntent.ReadOnly ? "ReadOnly" : "ReadWrite");
        CommandSetter setter = CommandSetter.WithGroupKey((CommandGroupKey) "WorkItemTracking").AndCommandKey((CommandKey) str).AndCommandPropertiesDefaults(incrementalRequest ? this.GetMetadataIncrementalCommandPropertiesSetter() : this.GetMetadataFullCommandPropertiesSetter());
        try
        {
          DataAccessLayerImpl.GetMetadataOutputParameters outputParameters = !this.m_requestContext.ExecutionEnvironment.IsHostedDeployment ? run() : new CommandService<DataAccessLayerImpl.GetMetadataOutputParameters>(this.m_requestContext, setter, run).Execute();
          locale = outputParameters.locale;
          comparisonStyle = outputParameters.comparisonStyle;
          callerIdentity = outputParameters.callerIdentity;
          dbStamp = outputParameters.dbStamp;
          mode = outputParameters.mode;
        }
        catch (CircuitBreakerExceededConcurrencyException ex)
        {
          throw new WorkItemTrackingGetMetadataTooManyConcurrentUsersException(type);
        }
        catch (CircuitBreakerExceededExecutionLimitException ex)
        {
          throw new WorkItemTrackingGetMetadataServerBusyException(type);
        }
        catch (CircuitBreakerShortCircuitException ex)
        {
          throw new WorkItemTrackingGetMetadataServerBusyException(type);
        }
      }
    }

    protected virtual WorkItemBatchBuilder CreateSqlBatch() => new WorkItemBatchBuilder(this.m_requestContext);

    protected virtual CommandPropertiesSetter GetMetadataIncrementalCommandPropertiesSetter() => DataAccessLayerImpl.DefaultCommandPropertiesForGetMetadataIncremental;

    protected virtual CommandPropertiesSetter GetMetadataFullCommandPropertiesSetter() => DataAccessLayerImpl.DefaultCommandPropertiesForGetMetadataFull;

    private bool IsMetadataUptodate(
      MetadataTable[] tablesRequested,
      long[] rowVersions,
      Payload metadataPayload,
      bool incrementalRequest,
      IWorkItemTrackingConfigurationInfo witConfigurationInfo,
      out bool metadataStampsChecked,
      out MetadataDBStamps userMetadataDbStamps)
    {
      userMetadataDbStamps = (MetadataDBStamps) null;
      bool flag = false;
      metadataStampsChecked = false;
      if (metadataPayload != null & incrementalRequest)
      {
        if (!witConfigurationInfo.MetadataFilterEnabled)
        {
          try
          {
            MetadataDBStamps currentDbStamps = this.m_requestContext.MetadataDbStamps().SubSet((IEnumerable<MetadataTable>) tablesRequested);
            Dictionary<MetadataTable, long> stamps = new Dictionary<MetadataTable, long>(tablesRequested.Length);
            for (int index = 0; index < tablesRequested.Length; ++index)
              stamps.Add(tablesRequested[index], rowVersions[index]);
            userMetadataDbStamps = new MetadataDBStamps((IDictionary<MetadataTable, long>) stamps);
            flag = userMetadataDbStamps.AreClientStampsFresh(currentDbStamps);
            metadataStampsChecked = true;
            this.m_requestContext.Trace(900111, TraceLevel.Verbose, "DataAccessLayer", nameof (DataAccessLayerImpl), "Successful call : Result of metadataAlreadyUpToDate: {0}", (object) flag.ToString());
          }
          catch
          {
            this.m_requestContext.Trace(900111, TraceLevel.Verbose, "DataAccessLayer", nameof (DataAccessLayerImpl), "Found exception during execution : Result of metadataAlreadyUpToDate: {0}", (object) flag.ToString());
          }
        }
      }
      return flag;
    }

    private bool IsHierarchyMetadataUptodate(
      MetadataTable[] tablesRequested,
      long[] rowVersions,
      Payload metadataPayload,
      bool incrementalRequest,
      IWorkItemTrackingConfigurationInfo witConfigurationInfo)
    {
      bool flag = false;
      if (metadataPayload != null & incrementalRequest && !witConfigurationInfo.MetadataFilterEnabled)
      {
        MetadataDBStamps currentDbStamps = this.m_requestContext.MetadataDbStamps().SubSet((IEnumerable<MetadataTable>) new List<MetadataTable>()
        {
          MetadataTable.Hierarchy
        });
        Dictionary<MetadataTable, long> stamps = new Dictionary<MetadataTable, long>(1);
        for (int index = 0; index < tablesRequested.Length; ++index)
        {
          if (tablesRequested[index] == MetadataTable.Hierarchy)
          {
            stamps.Add(tablesRequested[index], rowVersions[index]);
            break;
          }
        }
        flag = stamps.Count == 0 || new MetadataDBStamps((IDictionary<MetadataTable, long>) stamps).AreClientStampsFresh(currentDbStamps);
      }
      return flag;
    }

    private void FillEmptyMetadataSchemaRows(
      SqlBatchBuilder sqlBatch,
      MetadataTable[] tablesRequested,
      Payload metadataPayload)
    {
      for (int index = 0; index < tablesRequested.Length; ++index)
      {
        PayloadTable empty = PayloadTable.CreateEmpty(DataAccessLayerImpl.MetadataPayloadTableSchemas[tablesRequested[index]]);
        empty.TableName = tablesRequested[index].ToString();
        metadataPayload.Tables.Add(empty);
      }
      metadataPayload.SqlAccess = sqlBatch.ResultPayload.SqlAccess;
      metadataPayload.SqlExceptionHandler = sqlBatch.ResultPayload.SqlExceptionHandler;
      metadataPayload.SqlTypeExeptionHandler = sqlBatch.ResultPayload.SqlTypeExeptionHandler;
      this.AttachMetadataPayloadProcessors(metadataPayload, sqlBatch.Version);
    }

    private static string RemoveIllegalCharactersFromCommandKey(string input) => !string.IsNullOrWhiteSpace(input) ? input.Replace('_', '-') : input;

    private void AddDummyRecordsToPayload(
      MetadataDBStamps userMetadataDbStamps,
      Payload metadataPayload)
    {
      if (userMetadataDbStamps == null)
        return;
      if (metadataPayload == null)
      {
        this.m_requestContext.Trace(900115, TraceLevel.Verbose, "DataAccessLayer", nameof (DataAccessLayerImpl), "metadataPayload parameter null, cannot add dummy records");
      }
      else
      {
        MetadataDBStamps metadataDbStamps = this.m_requestContext.MetadataDbStamps();
        foreach (KeyValuePair<MetadataTable, long> userMetadataDbStamp in (ReadOnlyDictionary<MetadataTable, long>) userMetadataDbStamps)
        {
          KeyValuePair<MetadataTable, long> clientStamp = userMetadataDbStamp;
          if (metadataDbStamps.ContainsKey(clientStamp.Key))
          {
            long num = metadataDbStamps[clientStamp.Key];
            if (num > clientStamp.Value)
            {
              switch (clientStamp.Key)
              {
                case MetadataTable.Fields:
                  DataAccessLayerImpl.AddDummyFieldRowToTable(metadataPayload.Tables[clientStamp.Key.ToString()], num);
                  continue;
                case MetadataTable.HierarchyProperties:
                  if (WorkItemTrackingFeatureFlags.MetadataNewDummyRowsEnabled(this.m_requestContext))
                  {
                    DataAccessLayerImpl.AddDummyHierarchyPropertiesRowToTable(metadataPayload.Tables[clientStamp.Key.ToString()], num);
                    continue;
                  }
                  continue;
                case MetadataTable.Constants:
                  if (WorkItemTrackingFeatureFlags.MetadataNewDummyRowsEnabled(this.m_requestContext))
                  {
                    DataAccessLayerImpl.AddDummyConstantsRowToTable(metadataPayload.Tables[clientStamp.Key.ToString()], num);
                    continue;
                  }
                  continue;
                case MetadataTable.Rules:
                  if (WorkItemTrackingFeatureFlags.MetadataNewDummyRowsEnabled(this.m_requestContext))
                  {
                    DataAccessLayerImpl.AddDummyRulesRowToTable(metadataPayload.Tables[clientStamp.Key.ToString()], num);
                    continue;
                  }
                  continue;
                case MetadataTable.ConstantSets:
                  if (WorkItemTrackingFeatureFlags.MetadataNewDummyRowsEnabled(this.m_requestContext))
                  {
                    DataAccessLayerImpl.AddDummyConstantSetsToTable(metadataPayload.Tables[clientStamp.Key.ToString()], num);
                    continue;
                  }
                  continue;
                case MetadataTable.FieldUsages:
                  DataAccessLayerImpl.AddDummyFieldUsageRowToTable(metadataPayload.Tables[clientStamp.Key.ToString()], num);
                  continue;
                case MetadataTable.WorkItemTypes:
                  if (WorkItemTrackingFeatureFlags.MetadataNewDummyRowsEnabled(this.m_requestContext))
                  {
                    DataAccessLayerImpl.AddDummyWorkItemTypesRowToTable(metadataPayload.Tables[clientStamp.Key.ToString()], num);
                    continue;
                  }
                  continue;
                case MetadataTable.WorkItemTypeUsages:
                  if (WorkItemTrackingFeatureFlags.MetadataNewDummyRowsEnabled(this.m_requestContext))
                  {
                    DataAccessLayerImpl.AddDummyWorkItemTypeUsagesRowToTable(metadataPayload.Tables[clientStamp.Key.ToString()], num);
                    continue;
                  }
                  continue;
                case MetadataTable.LinkTypes:
                  DataAccessLayerImpl.AddDummyLinkTypeRowToTable(metadataPayload.Tables[clientStamp.Key.ToString()], num);
                  continue;
                default:
                  this.m_requestContext.TraceConditionally(900114, TraceLevel.Verbose, "DataAccessLayer", nameof (DataAccessLayerImpl), (Func<string>) (() => string.Format("Cannot create dummy record for table: {0}", (object) clientStamp.Key.ToString())));
                  continue;
              }
            }
          }
        }
      }
    }

    private void TraceClientAndLatestDbStamps(
      MetadataDBStamps clientStamps,
      MetadataDBStamps currentDbStamps)
    {
      Func<MetadataDBStamps, string> func = (Func<MetadataDBStamps, string>) (stamps =>
      {
        List<string> values = new List<string>();
        foreach (KeyValuePair<MetadataTable, long> stamp in (ReadOnlyDictionary<MetadataTable, long>) stamps)
          values.Add(stamp.Key.ToString() + ":" + stamp.Value.ToString());
        return string.Join(", ", (IEnumerable<string>) values);
      });
      this.m_requestContext.Trace(900113, TraceLevel.Verbose, "DataAccessLayer", nameof (DataAccessLayerImpl), "clientStamps: " + func(clientStamps) + ". " + "currentDbStamps: " + func(currentDbStamps));
    }

    private void GetCachedMetadata(
      IVssIdentity user,
      MetadataTable[] tablesRequested,
      long[] rowVersions,
      Payload metadataPayload,
      out int locale,
      out int comparisonStyle,
      out string callerIdentity,
      out string dbStamp,
      out int mode,
      IWorkItemTrackingConfigurationInfo witConfigurationInfo)
    {
      this.GetMetadataInternal(user, (MetadataTable[]) null, (long[]) null, (Payload) null, out locale, out comparisonStyle, out callerIdentity, out dbStamp, out mode, witConfigurationInfo);
      using (DataAccessLayerImpl.MetadataDownloadState metadataDownloadState = new DataAccessLayerImpl.MetadataDownloadState(this))
      {
        string virtualFilename = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0:X8}.witcache", (object) this.m_requestContext.MetadataDbStamps().GetMax());
        ITeamFoundationFileCacheService service = this.m_requestContext.GetService<ITeamFoundationFileCacheService>();
        VirtualFileInformation fileInfo = new VirtualFileInformation(this.m_requestContext.ServiceHost.InstanceId, virtualFilename, "WorkItemTrackingMetadata");
        if (!service.RetrieveFileFromCache<VirtualFileInformation>(this.m_requestContext, fileInfo, (IDownloadState<VirtualFileInformation>) metadataDownloadState, false))
        {
          this.m_requestContext.Trace(909704, TraceLevel.Info, "DataAccessLayer", "DataAccessLayerMetadataCaching", "Miss File Cache");
          ILockName lockName = this.m_requestContext.ServiceHost.CreateLockName(fileInfo.FileIdentifier);
          int millisecondsTimeout = 30000 + new Random().Next(0, 30000);
          bool flag = this.m_requestContext.LockManager.TryGetLock(lockName, millisecondsTimeout);
          try
          {
            if (!service.RetrieveFileFromCache<VirtualFileInformation>(this.m_requestContext, fileInfo, (IDownloadState<VirtualFileInformation>) metadataDownloadState, false))
            {
              this.m_requestContext.Trace(909705, TraceLevel.Info, "DataAccessLayer", "DataAccessLayerMetadataCaching", "Miss File Cache after getting lock, calling DB (LockHeld:{0})", (object) flag);
              if (WorkItemTrackingFeatureFlags.IsProcessCustomizationEnabled(this.m_requestContext))
                this.m_requestContext.GetService<WorkItemMetadataCompatibilityService>().GetMetadata(this.m_requestContext, tablesRequested, rowVersions, metadataPayload, out locale, out comparisonStyle, out callerIdentity, out dbStamp, out mode, witConfigurationInfo);
              else
                this.GetMetadataInternal(user, tablesRequested, rowVersions, metadataPayload, out locale, out comparisonStyle, out callerIdentity, out dbStamp, out mode, witConfigurationInfo);
              using (MemoryStream memoryStream = new MemoryStream())
              {
                this.SerializePayload(metadataPayload, (Stream) memoryStream);
                memoryStream.Seek(0L, SeekOrigin.Begin);
                fileInfo.Length = memoryStream.Length;
                this.m_requestContext.Trace(909708, TraceLevel.Info, "DataAccessLayer", "DataAccessLayerMetadataCaching", "Cache File Size {0}", (object) memoryStream.Length);
                service.RetrieveFileFromDatabase<VirtualFileInformation>(this.m_requestContext, fileInfo, (IDownloadState<VirtualFileInformation>) metadataDownloadState, false, (Stream) memoryStream, true);
                memoryStream.Seek(0L, SeekOrigin.Begin);
                using (StreamReader streamReader = new StreamReader((Stream) memoryStream))
                  metadataPayload.SetPreserializedData(streamReader.ReadToEnd());
              }
            }
            else
            {
              this.m_requestContext.Trace(909707, TraceLevel.Info, "DataAccessLayer", "DataAccessLayerMetadataCaching", "Hit File Cache after getting lock (LockHeld:{0})", (object) flag);
              this.FillPayloadFromStream(metadataPayload, metadataDownloadState.GetTargetStream());
            }
          }
          finally
          {
            if (flag)
              this.m_requestContext.LockManager.ReleaseLock(lockName);
          }
        }
        else
        {
          this.m_requestContext.Trace(909706, TraceLevel.Info, "DataAccessLayer", "DataAccessLayerMetadataCaching", "Hit File Cache");
          this.FillPayloadFromStream(metadataPayload, metadataDownloadState.GetTargetStream());
        }
      }
    }

    private void SerializePayload(Payload metadataPayload, Stream payloadStream)
    {
      Stream output = payloadStream;
      using (XmlWriter writer = XmlWriter.Create(output, new XmlWriterSettings()
      {
        Indent = false,
        OmitXmlDeclaration = true,
        ConformanceLevel = ConformanceLevel.Fragment
      }))
        metadataPayload.WriteCacheXml(writer);
    }

    private void FillPayloadFromStream(Payload metadataPayload, Stream payloadStream)
    {
      Stream input = payloadStream;
      using (XmlReader reader = XmlReader.Create(input, new XmlReaderSettings()
      {
        IgnoreProcessingInstructions = true,
        DtdProcessing = DtdProcessing.Prohibit,
        XmlResolver = (XmlResolver) null,
        ConformanceLevel = ConformanceLevel.Fragment
      }))
        metadataPayload.ReadXml(reader);
    }

    private void AttachMetadataPayloadProcessors(Payload payload, int version)
    {
      if (payload == null)
        return;
      PayloadTable payloadTable1 = (PayloadTable) null;
      try
      {
        payloadTable1 = payload.Tables["Hierarchy"];
      }
      catch (ArgumentOutOfRangeException ex)
      {
      }
      payloadTable1?.AddPayloadProcessor((PayloadProcessor) new ClassificationNodeProcessor(this.m_requestContext));
      if (!this.m_requestContext.ExecutionEnvironment.IsHostedDeployment)
        return;
      PayloadTable payloadTable2 = (PayloadTable) null;
      try
      {
        payloadTable2 = payload.Tables["Constants"];
      }
      catch (ArgumentOutOfRangeException ex)
      {
      }
      payloadTable2?.AddPayloadProcessor((PayloadProcessor) new ConstantsProcessor(this.m_requestContext));
    }

    protected virtual TElement GetSqlElement<TElement>(SqlBatchBuilder sqlBatch) where TElement : DalSqlElement, new() => DalSqlElement.GetElement<TElement>(sqlBatch);

    public XmlElement BulkUpdate(
      XmlElement updateElement,
      MetadataTable[] tablesRequested,
      long[] rowVersions,
      Payload metadataPayload,
      bool bisNotification,
      out string dbStamp,
      out bool bulkUpdateSuccess)
    {
      return this.BulkUpdate(updateElement, tablesRequested, rowVersions, metadataPayload, bisNotification, out dbStamp, out bulkUpdateSuccess, false);
    }

    public XmlElement BulkUpdate(
      XmlElement updateElement,
      MetadataTable[] tablesRequested,
      long[] rowVersions,
      Payload metadataPayload,
      bool bisNotification,
      out string dbStamp,
      out bool bulkUpdateSuccess,
      bool bypassRules)
    {
      return this.UpdateImpl(updateElement, tablesRequested, rowVersions, metadataPayload, bisNotification, out dbStamp, true, out bulkUpdateSuccess, (IVssIdentity) null, false, bypassRules, false, true);
    }

    public XmlElement Update(
      XmlElement updateElement,
      MetadataTable[] tablesRequested,
      long[] rowVersions,
      Payload metadataPayload,
      bool bisNotification,
      out string dbStamp,
      bool bypassRules)
    {
      this.m_requestContext.TraceEnter(900027, "DataAccessLayer", nameof (DataAccessLayerImpl), nameof (Update));
      bool bulkUpdateSuccess = false;
      return this.UpdateImpl(updateElement, tablesRequested, rowVersions, metadataPayload, bisNotification, out dbStamp, false, out bulkUpdateSuccess, (IVssIdentity) null, false, bypassRules, false, true);
    }

    public XmlElement Update(XmlElement package, bool overwrite = false, bool provisionRules = true)
    {
      this.m_requestContext.TraceEnter(900412, "DataAccessLayer", nameof (DataAccessLayerImpl), nameof (Update));
      bool bulkUpdateSuccess = false;
      return this.UpdateImpl(package, (MetadataTable[]) null, (long[]) null, (Payload) null, false, out string _, false, out bulkUpdateSuccess, (IVssIdentity) null, overwrite, false, false, provisionRules);
    }

    public XmlElement UpdateImpl(
      XmlElement updateElement,
      MetadataTable[] tablesRequested,
      long[] rowVersions,
      Payload metadataPayload,
      bool bisNotification,
      out string dbStamp,
      bool bulkUpdate,
      out bool bulkUpdateSuccess,
      IVssIdentity user,
      bool overwrite = false,
      bool bypassRules = false,
      bool validationOnly = false,
      bool provisionRules = true)
    {
      bulkUpdateSuccess = true;
      if (user == null)
        user = (IVssIdentity) this.m_requestContext.GetUserIdentity();
      DataAccessLayerImpl.ValidateArgsObjectNotNull((object) user, "clientUser", "InvalidUserException");
      this.ValidateArgsMetadataParms(tablesRequested, rowVersions, metadataPayload);
      if (updateElement == null)
        throw new ArgumentNullException(DalResourceStrings.Get("NullUpdateElementException"), nameof (updateElement));
      this.m_requestContext.Trace(900116, TraceLevel.Verbose, "DataAccessLayer", nameof (DataAccessLayerImpl), "UpdateElement: {0}", (object) updateElement.OuterXml);
      XmlDocument xmlDocument = (XmlDocument) null;
      try
      {
        VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.PerformanceCounters.UpdateRequests").Increment();
        string str = updateElement.GetAttribute("DisableNotifications").Trim();
        bool result;
        if (!string.IsNullOrEmpty(str) && bool.TryParse(str, out result))
        {
          this.m_requestContext.Trace(900117, TraceLevel.Verbose, "DataAccessLayer", nameof (DataAccessLayerImpl), "DisableNotifications=", (object) result.ToString());
          if (result)
            bisNotification = false;
        }
        Microsoft.TeamFoundation.WorkItemTracking.Server.Update update = (Microsoft.TeamFoundation.WorkItemTracking.Server.Update) null;
        bool flag = false;
        int num1 = 0;
        int num2 = 0;
        int num3 = 0;
        int num4 = 0;
        while (true)
        {
          xmlDocument = new XmlDocument();
          XmlElement element = xmlDocument.CreateElement("UpdateResults");
          XmlNode outputNode = xmlDocument.AppendChild((XmlNode) element);
          IDictionary<MetadataTable, WorkItemMetadataCompatibilityService.MetadataTableDescriptor> tablesToRegen = (IDictionary<MetadataTable, WorkItemMetadataCompatibilityService.MetadataTableDescriptor>) null;
          using (SqlBatchBuilder sqlBatch = (SqlBatchBuilder) new WorkItemBatchBuilder(this.m_requestContext, true, DatabaseConnectionType.Dbo))
          {
            sqlBatch.ExecutionTimeout = new int?(3600);
            update = new Microsoft.TeamFoundation.WorkItemTracking.Server.Update(this.m_requestContext.WitContext(), sqlBatch, outputNode, bisNotification, bulkUpdate, user, overwrite);
            long[] rowVersions1 = rowVersions;
            MetadataCompatibilityContext compatContext = (MetadataCompatibilityContext) null;
            if (tablesRequested != null && rowVersions != null)
            {
              rowVersions1 = this.m_requestContext.GetService<WorkItemMetadataCompatibilityService>().GetAdjustedRowVersions(this.m_requestContext, tablesRequested, rowVersions, out tablesToRegen);
              if (tablesToRegen.Any<KeyValuePair<MetadataTable, WorkItemMetadataCompatibilityService.MetadataTableDescriptor>>())
                compatContext = new MetadataCompatibilityContextGenerator().GetCompatibilityContext(this.m_requestContext);
            }
            update.BuildBatch(updateElement, tablesRequested, rowVersions1, bypassRules, validationOnly, provisionRules);
            try
            {
              sqlBatch.ExecuteBatch();
            }
            catch (LegacyBatchSaveException ex)
            {
              flag = true;
            }
            catch (DatabaseConfigurationException ex)
            {
              if (ex.InnerException != null && ex.InnerException is SqlException && (((SqlException) ex.InnerException).Number == 500 || ((SqlException) ex.InnerException).Number == 2766) && num1 < 5)
              {
                this.m_requestContext.Trace(900405, TraceLevel.Info, "DataAccessLayer", nameof (DataAccessLayerImpl), "Refreshing field metadata and retrying batch due to table-valued parameter column count mismatch.");
                this.m_requestContext.ResetMetadataDbStamps();
                this.m_requestContext.GetService<WorkItemTrackingFieldService>().InvalidateCache(this.m_requestContext);
                ++num1;
                continue;
              }
              throw;
            }
            catch (LegacyGeneralSqlException ex)
            {
              if (num3 < 5 && ex.ErrorCode == 207)
              {
                this.m_requestContext.Trace(900442, TraceLevel.Info, "DataAccessLayer", nameof (DataAccessLayerImpl), "Refreshing field metadata and retrying batch due to column mismatch.");
                this.m_requestContext.GetService<WorkItemTrackingFieldService>().InvalidateCache(this.m_requestContext);
                ++num3;
                continue;
              }
              if (ex.ErrorCode == 4012)
              {
                WorkItemTrackingService service = this.m_requestContext.GetService<WorkItemTrackingService>();
                int lcid = Thread.CurrentThread.CurrentCulture.LCID;
                if (service.IsCultureSupportedByDatabase(lcid))
                {
                  service.MarkCultureUnsupportedByDatabase(lcid);
                  continue;
                }
              }
              throw;
            }
            catch (LegacySqlErrorViewBindingErrorException ex)
            {
              if (num2 < 1 && this.HandleStaleViewsException(updateElement, user, (Exception) ex))
              {
                ++num2;
                continue;
              }
              throw;
            }
            catch (LegacyValidationException ex)
            {
              if (ex.ErrorId == 600312 && num4++ < 3)
              {
                this.m_requestContext.Trace(900696, TraceLevel.Verbose, "DataAccessLayer", nameof (DataAccessLayerImpl), "Link authorization failed due to area ID change.");
                continue;
              }
              throw;
            }
            if (flag)
            {
              bulkUpdateSuccess = false;
              update.GetBulkUpdateFailResults(out dbStamp);
              break;
            }
            update.GetResults(this.m_requestContext, metadataPayload, out dbStamp);
            if (metadataPayload != null)
            {
              this.AttachMetadataPayloadProcessors(metadataPayload, sqlBatch.Version);
              if (tablesToRegen != null)
              {
                this.m_requestContext.GetService<WorkItemMetadataCompatibilityService>().RegeneratePayloadTables(this.m_requestContext, compatContext, metadataPayload, tablesToRegen);
                break;
              }
              break;
            }
            break;
          }
        }
        try
        {
          if (update != null)
          {
            if (update.MetaDataChanged)
              this.m_requestContext.GetService<ITeamFoundationEventService>().PublishNotification(this.m_requestContext, (object) new LegacyWitMetadataChangedEvent());
          }
        }
        catch (Exception ex)
        {
          this.m_requestContext.TraceException(900500, "DataAccessLayer", nameof (DataAccessLayerImpl), ex);
        }
      }
      finally
      {
        VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.PerformanceCounters.UpdateRequests").Decrement();
      }
      this.m_requestContext.Trace(900501, TraceLevel.Verbose, "DataAccessLayer", nameof (DataAccessLayerImpl), xmlDocument.InnerXml);
      this.m_requestContext.TraceLeave(900027, "DataAccessLayer", nameof (DataAccessLayerImpl), "Update");
      return xmlDocument?.DocumentElement;
    }

    public bool HandleStaleViewsException(XmlElement package, IVssIdentity user, Exception e)
    {
      bool flag = false;
      if (e is LegacySqlErrorViewBindingErrorException)
      {
        try
        {
          this.TryRefreshReadViews(user);
        }
        catch (Exception ex)
        {
          this.m_requestContext.Trace(900386, TraceLevel.Error, "DataAccessLayer", nameof (DataAccessLayerImpl), "Failed on TryRefreshReadViews. {0}:{1}", (object) ex.GetType().ToString(), (object) ex.Message);
          flag = true;
        }
        if (!flag)
        {
          if (package != null)
          {
            try
            {
              this.TryRefreshWriteViews(user, package);
            }
            catch (Exception ex)
            {
              this.m_requestContext.Trace(900387, TraceLevel.Error, "DataAccessLayer", nameof (DataAccessLayerImpl), "Failed on TryRefreshWriteViews. {0}:{1}", (object) ex.GetType().ToString(), (object) ex.Message);
              flag = true;
            }
          }
        }
      }
      return flag;
    }

    public void TryRefreshReadViews(IVssIdentity user)
    {
      this.m_requestContext.TraceEnter(900028, "DataAccessLayer", nameof (DataAccessLayerImpl), nameof (TryRefreshReadViews));
      this.m_requestContext.Trace(900118, TraceLevel.Verbose, "DataAccessLayer", nameof (DataAccessLayerImpl), "Server:{0}, ClientUserVsid:{1}", (object) this.m_requestContext.VirtualPath(), (object) user?.Id);
      DataAccessLayerImpl.ValidateArgsObjectNotNull((object) user, "clientUser", "InvalidUserException");
      using (SqlBatchBuilder sqlBatch = (SqlBatchBuilder) new WorkItemBatchBuilder(this.m_requestContext))
      {
        DalSqlElement.GetElement<DalPersonIdElement>(sqlBatch).JoinBatch((ElementGroup) null, user);
        sqlBatch.ExecuteBatch();
      }
      this.m_requestContext.TraceLeave(900029, "DataAccessLayer", nameof (DataAccessLayerImpl), nameof (TryRefreshReadViews));
    }

    public void TryRefreshWriteViews(IVssIdentity user, XmlElement failedUpdateBatch)
    {
      this.m_requestContext.TraceEnter(900030, "DataAccessLayer", nameof (DataAccessLayerImpl), nameof (TryRefreshWriteViews));
      this.m_requestContext.Trace(900119, TraceLevel.Verbose, "DataAccessLayer", nameof (DataAccessLayerImpl), "Server:{0}, ClientUserVsid:{1}", (object) this.m_requestContext.VirtualPath(), (object) user?.Id);
      DataAccessLayerImpl.ValidateArgsObjectNotNull((object) user, "clientUser", "InvalidUserException");
      int workItemAreaId = 0;
      int workItemId = 0;
      this.ExtractWorkItemDataFromBatch(failedUpdateBatch, out workItemAreaId, out workItemId);
      if (workItemAreaId == 0 && workItemId == 0)
      {
        this.m_requestContext.Trace(900031, TraceLevel.Verbose, "DataAccessLayer", nameof (DataAccessLayerImpl), "TryRefreshWriteViews was called with the RefreshWrite option using a batch with no identifiers");
      }
      else
      {
        using (SqlBatchBuilder sqlBatch = (SqlBatchBuilder) new WorkItemBatchBuilder(this.m_requestContext))
        {
          DalSqlElement.GetElement<DalPersonIdElement>(sqlBatch).JoinBatch((ElementGroup) null, user);
          sqlBatch.ExecuteBatch();
        }
        this.m_requestContext.TraceLeave(900032, "DataAccessLayer", nameof (DataAccessLayerImpl), nameof (TryRefreshWriteViews));
      }
    }

    public ExtendedAccessControlListData GetQueryAccessControlList(
      Guid queryItemId,
      bool getMetadata)
    {
      this.m_requestContext.TraceEnter(900038, "DataAccessLayer", nameof (DataAccessLayerImpl), nameof (GetQueryAccessControlList));
      DataAccessLayerImpl.ValidateArgsObjectNotNull((object) queryItemId, nameof (queryItemId));
      try
      {
        VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.PerformanceCounters.GetStoredQueryRequests").Increment();
        IVssSecurityNamespace queryItemSecurity = (IVssSecurityNamespace) this.m_requestContext.GetService<WorkItemTrackingService>().QueryItemSecurity;
        IdentityDescriptor userContext = this.m_requestContext.UserContext;
        ServerQueryItem serverQueryItem = new ServerQueryItem(queryItemId);
        QueryItemMethods.PopulateSecurityInfo(this.m_requestContext, serverQueryItem);
        if (serverQueryItem.SecurityToken == null)
          throw new LegacyValidationException(this.m_requestContext, DalResourceStrings.Manager, 600289, "DeniedOrNotExist", "QueryHierarchyItemDoesNotExist", (object[]) null);
        ExtendedAccessControlListData accessControlList1 = new ExtendedAccessControlListData();
        if (getMetadata)
          accessControlList1.Metadata = QueryItemMethods.GetAclMetadata(this.m_requestContext);
        if (!serverQueryItem.Existing.IsPublic.Value)
        {
          if (serverQueryItem.Existing.Owner != (IdentityDescriptor) null && serverQueryItem.Existing.Owner.Identifier != null && IdentityDescriptorComparer.Instance.Equals(userContext, serverQueryItem.Existing.Owner))
          {
            AccessControlEntryData controlEntryData = new AccessControlEntryData();
            controlEntryData.Descriptor = userContext;
            int num = 7;
            controlEntryData.ExtendedInfo = new AceExtendedInformation();
            controlEntryData.ExtendedInfo.EffectiveAllow = num;
            bool flag;
            if (serverQueryItem.Existing.ParentId != Guid.Empty)
            {
              controlEntryData.ExtendedInfo.InheritedAllow = num;
              flag = true;
            }
            else
            {
              controlEntryData.Allow = num;
              flag = false;
            }
            accessControlList1.Token = serverQueryItem.Id.ToString();
            accessControlList1.InheritPermissions = flag;
            accessControlList1.Permissions.Add(controlEntryData);
            accessControlList1.IsEditable = false;
            return accessControlList1;
          }
          throw new LegacyValidationException(this.m_requestContext, DalResourceStrings.Manager, 600288, "AccessException", "QueryHierarchyItemAccessException", new object[3]
          {
            (object) this.m_requestContext.DomainUserName,
            (object) QueryItemMethods.GetPermissionDisplayStrings(this.m_requestContext, serverQueryItem, 1),
            (object) serverQueryItem.Existing.QueryName
          });
        }
        QueryItemMethods.CheckPermissionForAnyChildren(this.m_requestContext, serverQueryItem, 1);
        IAccessControlList accessControlList2 = queryItemSecurity.QueryAccessControlList(this.m_requestContext, serverQueryItem.SecurityToken, (IEnumerable<IdentityDescriptor>) null, true);
        if (accessControlList2 == null)
        {
          accessControlList1.Token = serverQueryItem.Id.ToString();
          accessControlList1.InheritPermissions = true;
          accessControlList1.IsEditable = false;
          return accessControlList1;
        }
        Dictionary<IdentityDescriptor, AccessControlEntryData> dictionary = new Dictionary<IdentityDescriptor, AccessControlEntryData>((IEqualityComparer<IdentityDescriptor>) IdentityDescriptorComparer.Instance);
        foreach (IAccessControlEntry accessControlEntry in accessControlList2.AccessControlEntries)
        {
          AccessControlEntryData controlEntryData = new AccessControlEntryData(accessControlEntry);
          dictionary.Add(controlEntryData.Descriptor, controlEntryData);
        }
        if (!accessControlList2.InheritPermissions)
        {
          int length = accessControlList2.Token.LastIndexOf(QueryItemSecurityConstants.PathSeparator);
          if (length > -1)
          {
            string token = accessControlList2.Token.Substring(0, length);
            foreach (IAccessControlEntry accessControlEntry in queryItemSecurity.QueryAccessControlList(this.m_requestContext, token, (IEnumerable<IdentityDescriptor>) null, true).AccessControlEntries)
            {
              IAccessControlEntry ace = accessControlList2.QueryAccessControlEntry(accessControlEntry.Descriptor);
              AccessControlEntryData controlEntryData;
              if (!dictionary.TryGetValue(ace.Descriptor, out controlEntryData))
              {
                controlEntryData = new AccessControlEntryData(ace);
                dictionary.Add(controlEntryData.Descriptor, controlEntryData);
              }
              int updatedAllow;
              int updatedDeny;
              SecurityUtility.MergePermissions(accessControlEntry.InheritedAllow, accessControlEntry.InheritedDeny, accessControlEntry.Allow, accessControlEntry.Deny, 0, out updatedAllow, out updatedDeny);
              controlEntryData.ExtendedInfo.InheritedAllow = updatedAllow;
              controlEntryData.ExtendedInfo.InheritedDeny = updatedDeny;
            }
          }
        }
        accessControlList1.IsEditable = QueryItemMethods.HasPermission(this.m_requestContext, serverQueryItem, 8);
        accessControlList1.Token = serverQueryItem.Id.ToString();
        accessControlList1.InheritPermissions = accessControlList2.InheritPermissions;
        accessControlList1.Permissions.AddRange((IEnumerable<AccessControlEntryData>) dictionary.Values);
        return accessControlList1;
      }
      finally
      {
        VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.PerformanceCounters.GetStoredQueryRequests").Decrement();
        this.m_requestContext.TraceLeave(900544, "DataAccessLayer", nameof (DataAccessLayerImpl), nameof (GetQueryAccessControlList));
      }
    }

    public Microsoft.VisualStudio.Services.Identity.Identity GetQueryItemOwner(
      IVssRequestContext requestContext,
      Guid queryItemId)
    {
      ServerQueryItem serverQueryItem = new ServerQueryItem(queryItemId);
      QueryItemMethods.PopulateSecurityInfo(requestContext, serverQueryItem);
      QueryItemMethods.CheckPermission(requestContext, serverQueryItem, 1);
      if (serverQueryItem.Existing.Owner != (IdentityDescriptor) null && !string.IsNullOrEmpty(serverQueryItem.Existing.Owner.Identifier))
      {
        IList<Microsoft.VisualStudio.Services.Identity.Identity> identityList = requestContext.GetService<IdentityService>().ReadIdentities(requestContext, (IList<IdentityDescriptor>) new IdentityDescriptor[1]
        {
          serverQueryItem.Existing.Owner
        }, QueryMembership.None, (IEnumerable<string>) null);
        if (identityList.Count > 0 && identityList[0] != null)
          return identityList[0];
      }
      return (Microsoft.VisualStudio.Services.Identity.Identity) null;
    }

    public void DeleteQueryItem(Guid queryId)
    {
      this.m_requestContext.TraceEnter(900047, "DataAccessLayer", nameof (DataAccessLayerImpl), nameof (DeleteQueryItem));
      this.m_requestContext.Trace(900130, TraceLevel.Verbose, "DataAccessLayer", nameof (DataAccessLayerImpl), "Server: {0}, UserSid: {1}", (object) this.m_requestContext.VirtualPath(), (object) this.m_requestContext.UserContext.Identifier);
      this.m_requestContext.Trace(900131, TraceLevel.Verbose, "DataAccessLayer", nameof (DataAccessLayerImpl), "QueryId: {0}", (object) queryId.ToString("D"));
      XmlElement updatePackage = this.CreateUpdatePackage();
      QueryItemHelper.Delete(updatePackage, queryId);
      this.Update(updatePackage, false, true);
      this.m_requestContext.TraceLeave(900048, "DataAccessLayer", nameof (DataAccessLayerImpl), nameof (DeleteQueryItem));
    }

    public void UpdateQueryItem(Guid id, Guid parentId, string name, string queryText)
    {
      this.m_requestContext.TraceEnter(900049, "DataAccessLayer", nameof (DataAccessLayerImpl), nameof (UpdateQueryItem));
      this.m_requestContext.Trace(900132, TraceLevel.Verbose, "DataAccessLayer", nameof (DataAccessLayerImpl), "Server: {0}, UserSid: {1}", (object) this.m_requestContext.VirtualPath(), (object) this.m_requestContext.UserContext.Identifier);
      this.m_requestContext.Trace(900133, TraceLevel.Verbose, "DataAccessLayer", nameof (DataAccessLayerImpl), "Id: {0}, ParentId: {1}, Name: {2}, QueryText: {3}", (object) id.ToString("D"), (object) parentId.ToString("D"), (object) name, (object) queryText);
      this.ValidateQueryItemParamters(ref name, queryText);
      XmlElement updatePackage = this.CreateUpdatePackage();
      QueryItemHelper.Update(updatePackage, id, parentId, name, queryText, (string) null, (string) null);
      this.Update(updatePackage, false, true);
      this.m_requestContext.TraceLeave(900050, "DataAccessLayer", nameof (DataAccessLayerImpl), nameof (UpdateQueryItem));
    }

    public void UpdateQueryItemOwner(
      IVssRequestContext requestContext,
      Guid queryItemId,
      IdentityDescriptor descriptor)
    {
      XmlElement updatePackage = this.CreateUpdatePackage();
      QueryItemHelper.Update(updatePackage, queryItemId, Guid.Empty, (string) null, (string) null, descriptor.Identifier, descriptor.IdentityType);
      this.Update(updatePackage, false, true);
    }

    public void CreateQueryItem(Guid id, Guid parentId, string name, string queryText)
    {
      this.m_requestContext.TraceEnter(900051, "DataAccessLayer", nameof (DataAccessLayerImpl), nameof (CreateQueryItem));
      this.m_requestContext.Trace(900134, TraceLevel.Verbose, "DataAccessLayer", nameof (DataAccessLayerImpl), "Server: {0}, UserSid: {1}", (object) this.m_requestContext.VirtualPath(), (object) this.m_requestContext.UserContext.Identifier);
      this.m_requestContext.Trace(900135, TraceLevel.Verbose, "DataAccessLayer", nameof (DataAccessLayerImpl), "Id: {0}, ParentId: {1}, Name: {2}, QueryText: {3}", (object) id.ToString("D"), (object) parentId.ToString("D"), (object) name, (object) queryText);
      this.ValidateQueryItemParamters(ref name, queryText);
      XmlElement updatePackage = this.CreateUpdatePackage();
      QueryItemHelper.Create(updatePackage, id, parentId, name, queryText, this.m_requestContext.UserContext.Identifier, this.m_requestContext.UserContext.IdentityType);
      this.Update(updatePackage, false, true);
      this.m_requestContext.TraceLeave(900052, "DataAccessLayer", nameof (DataAccessLayerImpl), nameof (CreateQueryItem));
    }

    private QueryItem LoadQueryItemFromPayloadRow(PayloadTable.PayloadRow row, bool folder) => new QueryItem()
    {
      Id = (Guid) row["ID"],
      ParentId = row["ParentID"] != null ? (Guid) row["ParentID"] : Guid.Empty,
      Name = (string) row[folder ? "Name" : "QueryName"],
      QueryText = (string) row[folder ? "Text" : "QueryText"]
    };

    private void ValidateQueryItemParamters(ref string name, string queryText)
    {
      if (!string.IsNullOrEmpty(name))
        name = QueryItemHelper.CheckNameIsValid(name);
      if (string.IsNullOrEmpty(queryText))
        return;
      try
      {
        WiqlTextHelper.ValidateWiqlTextRequirements(this.m_requestContext, queryText);
        QueryItemHelper.ValidateWiql((WiqlAdapter) null, queryText);
        this.CheckIfSaveIsProhibited(queryText);
      }
      catch (SyntaxException ex)
      {
        throw new LegacyValidationException(ex.Details, (Exception) ex);
      }
    }

    private void CheckIfSaveIsProhibited(string wiql)
    {
      if (!this.UsesTeamlessCurrentIteration(wiql))
        return;
      if (WorkItemTrackingFeatureFlags.IsVisualStudio(this.m_requestContext))
        throw new SyntaxException(DalResourceStrings.Get("CannotUpdateCurrentIterationQueryInVS"));
      throw new SyntaxException(DalResourceStrings.Get("CurrentIteration_TeamRequired"));
    }

    private bool UsesTeamlessCurrentIteration(string wiql)
    {
      bool usesTeamless = false;
      try
      {
        Parser.ParseSyntax(wiql).Walk((Action<Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node>) (n =>
        {
          if (!(n is NodeVariable nodeVariable2) || !string.Equals(nodeVariable2.Value, "currentiteration", StringComparison.OrdinalIgnoreCase) || nodeVariable2.Parameters.Arguments.Any<NodeItem>())
            return;
          usesTeamless = true;
        }));
      }
      catch
      {
      }
      return usesTeamless;
    }

    private XmlElement CreateUpdatePackage()
    {
      XmlDocument xmlDocument = new XmlDocument();
      XmlElement element = xmlDocument.CreateElement("Package");
      xmlDocument.AppendChild((XmlNode) element);
      return element;
    }

    public Artifact[] GetArtifacts(IVssIdentity user, string[] artifactUriList)
    {
      this.m_requestContext.Trace(900053, TraceLevel.Verbose, "DataAccessLayer", nameof (DataAccessLayerImpl), nameof (GetArtifacts));
      DataAccessLayerImpl.ValidateArgsObjectNotNull((object) user, "clientUser", "InvalidUserException");
      return new InverseQuery(this.m_requestContext, user).GetArtifacts(artifactUriList);
    }

    public Artifact[] GetReferencingArtifacts(IVssIdentity user, string[] artifactUriList)
    {
      this.m_requestContext.Trace(900054, TraceLevel.Verbose, "DataAccessLayer", nameof (DataAccessLayerImpl), nameof (GetReferencingArtifacts));
      DataAccessLayerImpl.ValidateArgsObjectNotNull((object) user, "clientUser", "InvalidUserException");
      return new InverseQuery(this.m_requestContext, user).GetReferencingArtifacts(artifactUriList);
    }

    public Artifact[] GetReferencingArtifacts(
      IVssIdentity user,
      string[] artifactUriList,
      LinkFilter[] linkFilterList)
    {
      this.m_requestContext.Trace(900055, TraceLevel.Verbose, "DataAccessLayer", nameof (DataAccessLayerImpl), "GetReferencingArtifacts(LinkFilterList)");
      DataAccessLayerImpl.ValidateArgsObjectNotNull((object) user, "clientUser", "InvalidUserException");
      InverseQuery inverseQuery = new InverseQuery(this.m_requestContext, user);
      return linkFilterList != null ? inverseQuery.GetReferencingArtifacts(artifactUriList, linkFilterList) : inverseQuery.GetReferencingArtifacts(artifactUriList);
    }

    public string[] GetReferencingWorkitemUris(IVssIdentity user, string artifactUri)
    {
      this.m_requestContext.Trace(900056, TraceLevel.Verbose, "DataAccessLayer", nameof (DataAccessLayerImpl), nameof (GetReferencingWorkitemUris));
      DataAccessLayerImpl.ValidateArgsObjectNotNull((object) user, "clientUser", "InvalidUserException");
      return new InverseQuery(this.m_requestContext, user).GetReferencingWorkitemUris(artifactUri);
    }

    public TeamFoundationDataReader GetWorkItemIds(
      IVssRequestContext context,
      long rowVersion,
      bool destroyed)
    {
      CommandGetWorkItemIds disposableObject = (CommandGetWorkItemIds) null;
      try
      {
        disposableObject = new CommandGetWorkItemIds(context, destroyed);
        disposableObject.Execute(rowVersion);
        return new TeamFoundationDataReader((IDisposable) disposableObject, new object[1]
        {
          (object) disposableObject.WorkItemIds
        });
      }
      catch (Exception ex)
      {
        disposableObject?.Dispose();
        throw;
      }
    }

    public TeamFoundationDataReader GetWorkItemLinkChanges(
      IVssRequestContext context,
      long rowVersion)
    {
      CommandGetWorkItemLinkChanges disposableObject = (CommandGetWorkItemLinkChanges) null;
      try
      {
        disposableObject = new CommandGetWorkItemLinkChanges(context, this.ShouldBypassWorkItemReadPermissions(context));
        disposableObject.Execute(rowVersion);
        return new TeamFoundationDataReader((IDisposable) disposableObject, new object[1]
        {
          (object) disposableObject.WorkItemLinkChanges
        });
      }
      catch (Exception ex)
      {
        disposableObject?.Dispose();
        throw;
      }
    }

    public WorkItemLinkChange[] GetWorkItemLinkChanges(
      IVssRequestContext context,
      long rowVersion,
      int batchSize,
      Guid? projectId,
      IEnumerable<string> types,
      IEnumerable<string> linkTypes,
      ref DateTime? createdDateWatermark,
      ref DateTime? removedDateWatermark,
      out long maxReadRowVersion,
      out int totalRowCount)
    {
      using (CommandGetWorkItemLinkChanges workItemLinkChanges = new CommandGetWorkItemLinkChanges(context, this.ShouldBypassWorkItemReadPermissions(context)))
      {
        workItemLinkChanges.Execute(rowVersion, batchSize, projectId, types, linkTypes, createdDateWatermark, removedDateWatermark);
        WorkItemLinkChange[] array = workItemLinkChanges.WorkItemLinkChanges.ToArray<WorkItemLinkChange>();
        maxReadRowVersion = workItemLinkChanges.MaxReadRowVersionUnfiltered;
        totalRowCount = workItemLinkChanges.TotalResultCountUnfiltered;
        createdDateWatermark = new DateTime?(workItemLinkChanges.MaxCreatedDateWatermark);
        removedDateWatermark = new DateTime?(workItemLinkChanges.MaxRemovedDateWatermark);
        return array;
      }
    }

    public long GetLinkTimeStampForDateTime(DateTime dateTime)
    {
      using (DalSqlResourceComponent component = DalSqlResourceComponent.CreateComponent(this.m_requestContext))
        return component.GetWorkItemLinkTimestampForDate(dateTime);
    }

    public Tuple<DateTime, DateTime> GetLinkDateTimeForTimeStamp(long timestamp)
    {
      using (DalSqlResourceComponent component = DalSqlResourceComponent.CreateComponent(this.m_requestContext))
        return component.GetWorkItemLinkDateForTimestamp(timestamp);
    }

    public void SyncBisGroupsAndUsers(string projectUri)
    {
      this.m_requestContext.TraceEnter(900060, "DataAccessLayer", nameof (DataAccessLayerImpl), nameof (SyncBisGroupsAndUsers));
      DataAccessLayerImpl.ValidateArgsStringNotNullOrEmpty(projectUri, "projectURI", "InvalidProjectURIException");
      new SyncBase(this.m_requestContext).ProcessIdentityChanges();
      this.m_requestContext.TraceLeave(900061, "DataAccessLayer", nameof (DataAccessLayerImpl), nameof (SyncBisGroupsAndUsers));
    }

    public bool ProjectDelete(string projectUri, bool witOnly = false)
    {
      this.m_requestContext.TraceEnter(900064, "DataAccessLayer", nameof (DataAccessLayerImpl), nameof (ProjectDelete));
      DataAccessLayerImpl.ValidateArgsStringNotNullOrEmpty(projectUri, "projectURI", "InvalidProjectURIException");
      string toolSpecificId = LinkingUtilities.DecodeUri(projectUri).ToolSpecificId;
      Guid author = this.m_requestContext.GetService<TeamFoundationSqlNotificationService>().Author;
      using (SqlBatchBuilder sqlBatch = (SqlBatchBuilder) new WorkItemBatchBuilder(this.m_requestContext))
      {
        DalSqlElement.GetElement<DalPersonIdElement>(sqlBatch).JoinBatch((ElementGroup) null, (IVssIdentity) this.m_requestContext.GetUserIdentity());
        DalSqlElement.GetElement<DalBeginLocalTranElement>(sqlBatch).JoinBatch((ElementGroup) null);
        DalSqlElement.GetElement<DalDeleteProjectElement>(sqlBatch).JoinBatch(toolSpecificId, witOnly);
        DalSqlElement.GetElement<DalEventChangeElement>(sqlBatch).JoinBatch(DBNotificationIds.WorkItemTrackingProvisionedMetadataChanged, DalSqlElement.DEFAULT_VALUE, author);
        DalSqlElement.GetElement<DalEventChangeElement>(sqlBatch).JoinBatch("34B8C348-F677-4A6B-AAE3-2C4EDFC9E959", DalSqlElement.DEFAULT_VALUE, author);
        DalSqlElement.GetElement<DalEventChangeElement>(sqlBatch).JoinBatch("92778466-E528-4569-8736-A7CBD9983DB7", DalSqlElement.DEFAULT_VALUE, author);
        DalSqlElement.GetElement<DalEndLocalTranElement>(sqlBatch).JoinBatch((ElementGroup) null);
        sqlBatch.ExecuteBatch();
      }
      this.m_requestContext.ResetMetadataDbStamps();
      this.m_requestContext.GetService<WorkItemTrackingFieldService>().InvalidateCache(this.m_requestContext);
      this.m_requestContext.GetService<WorkItemTrackingTreeService>().InvalidateCache(this.m_requestContext);
      this.m_requestContext.ResetMetadataDbStamps();
      this.m_requestContext.GetService<WorkItemMetadataCompatibilityService>().ForceRefreshClientMetadata(this.m_requestContext);
      try
      {
        this.m_requestContext.GetService<TeamFoundationSecurityService>().GetSecurityNamespace(this.m_requestContext, WitProvisionSecurity.NamespaceId).RemoveAccessControlLists(this.m_requestContext, (IEnumerable<string>) new string[1]
        {
          "$/" + toolSpecificId
        }, true);
      }
      catch (DataspaceNotFoundException ex)
      {
        this.m_requestContext.Trace(909701, TraceLevel.Warning, "DataAccessLayer", nameof (DataAccessLayerImpl), ex.Message);
      }
      this.m_requestContext.TraceLeave(900546, "DataAccessLayer", nameof (DataAccessLayerImpl), nameof (ProjectDelete));
      return true;
    }

    public void StampDb(IVssIdentity user)
    {
      this.m_requestContext.TraceEnter(900065, "DataAccessLayer", nameof (DataAccessLayerImpl), nameof (StampDb));
      DataAccessLayerImpl.ValidateArgsObjectNotNull((object) user, "clientUser", "InvalidUserException");
      using (SqlBatchBuilder sqlBatch = (SqlBatchBuilder) new WorkItemBatchBuilder(this.m_requestContext))
      {
        Trace.WriteLine("Update DB Stamp - Server Name:" + this.m_requestContext.VirtualPath());
        DalSqlElement.GetElement<UpdateDbStamp>(sqlBatch).StampDb(user);
      }
      this.m_requestContext.TraceLeave(900547, "DataAccessLayer", nameof (DataAccessLayerImpl), nameof (StampDb));
    }

    public PayloadTable GetSequenceIds()
    {
      this.m_requestContext.TraceEnter(900066, "DataAccessLayer", nameof (DataAccessLayerImpl), nameof (GetSequenceIds));
      PayloadTable outputTable;
      using (SqlBatchBuilder sqlBatch = (SqlBatchBuilder) new WorkItemBatchBuilder(this.m_requestContext))
      {
        DalGetSequenceIds element = DalSqlElement.GetElement<DalGetSequenceIds>(sqlBatch);
        element.JoinBatch();
        sqlBatch.ExecuteBatch();
        outputTable = element.GetOutputTable();
      }
      this.m_requestContext.TraceLeave(900548, "DataAccessLayer", nameof (DataAccessLayerImpl), nameof (GetSequenceIds));
      return outputTable;
    }

    public IEnumerable<string> GetMissingIdentities(IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity> groups)
    {
      this.m_requestContext.TraceEnter(900070, "DataAccessLayer", nameof (DataAccessLayerImpl), nameof (GetMissingIdentities));
      DataAccessLayerImpl.ValidateArgsObjectNotNull((object) groups, nameof (groups));
      IEnumerable<string> missingIdentities;
      using (DalSqlResourceComponent component = DalSqlResourceComponent.CreateComponent(this.m_requestContext))
        missingIdentities = component.GetMissingIdentities(groups);
      this.m_requestContext.TraceLeave(900071, "DataAccessLayer", nameof (DataAccessLayerImpl), nameof (GetMissingIdentities));
      return missingIdentities;
    }

    public void AddNewBuild(string buildName, string project, string buildDefinitionName)
    {
      this.m_requestContext.TraceEnter(900067, "DataAccessLayer", nameof (DataAccessLayerImpl), nameof (AddNewBuild));
      this.m_requestContext.Trace(900136, TraceLevel.Verbose, "DataAccessLayer", nameof (DataAccessLayerImpl), "Server:{0}, buildName:{1}, project:{2}", (object) this.m_requestContext.VirtualPath(), (object) buildName, (object) project);
      DataAccessLayerImpl.ValidateArgsStringNotNullOrEmpty(buildName, "BuildName", "InvalidBuildNameException");
      DataAccessLayerImpl.ValidateArgsStringNotNullOrEmpty(project, "Project", "InvalidProjectException");
      DataAccessLayerImpl.ValidateArgsStringNotNullOrEmpty(buildDefinitionName, "Uri", "InvalidURIException");
      int maxBuildListSize = this.m_requestContext.GetService<WorkItemTrackingConfigurationSettingService>().GetConfigurationInfo(this.m_requestContext).MaxBuildListSize;
      using (DalSqlResourceComponent component = DalSqlResourceComponent.CreateComponent(this.m_requestContext))
        component.AddNewBuild(buildName, project, buildDefinitionName, maxBuildListSize);
      this.m_requestContext.TraceLeave(900549, "DataAccessLayer", nameof (DataAccessLayerImpl), nameof (AddNewBuild));
    }

    public void DestroyAttachments(IEnumerable<int> workItemIds)
    {
      ArgumentUtility.CheckForNull<IEnumerable<int>>(workItemIds, nameof (workItemIds));
      this.m_requestContext.TraceEnter(900662, "DataAccessLayer", nameof (DataAccessLayerImpl), nameof (DestroyAttachments));
      try
      {
        if (workItemIds.Count<int>() <= 0)
          return;
        if (workItemIds.Any<int>((Func<int, bool>) (id => id <= 0)))
          throw new ArgumentException(DalResourceStrings.Get("InvalidWorkItemIdException"), nameof (workItemIds));
        this.m_requestContext.GetService<TeamFoundationSecurityService>().GetSecurityNamespace(this.m_requestContext, WorkItemTrackingSecurityConstants.NamespaceGuid).CheckPermission(this.m_requestContext, "WorkItemTrackingPrivileges", 2, false);
        string comment = DalResourceStrings.Format("AttachmentsDestoyedComment", (object) this.m_requestContext.GetUserIdentity().DisplayName, (object) "$$DATETIME$$");
        using (DalSqlResourceComponent component = DalSqlResourceComponent.CreateComponent(this.m_requestContext))
          component.DestroyAttachments((IVssIdentity) this.m_requestContext.GetUserIdentity(), workItemIds, comment, true);
        TeamFoundationWorkItemAttachmentService service = this.m_requestContext.GetService<TeamFoundationWorkItemAttachmentService>();
        service.DeleteOrphanAttachments(this.m_requestContext);
        if (!this.m_requestContext.IsFeatureEnabled("WorkItemTracking.Server.DeleteNoCurrentReferenceAttachments"))
          return;
        service.DeleteAttachmentsWithNoCurrentReference(this.m_requestContext);
      }
      finally
      {
        this.m_requestContext.TraceLeave(900663, "DataAccessLayer", nameof (DataAccessLayerImpl), nameof (DestroyAttachments));
      }
    }

    private static void ValidateArgsObjectNotNull(
      object argValue,
      string argName,
      string msgIfEmpty = null)
    {
      if (argValue != null)
        return;
      if (msgIfEmpty != null)
        throw new ArgumentNullException(argName, DalResourceStrings.Get(msgIfEmpty));
      throw new ArgumentNullException(argName);
    }

    private static void ValidateArgsStringNotNullOrEmpty(
      string argValue,
      string argName,
      string msgIfEmpty)
    {
      if (argValue == null)
        throw new ArgumentNullException(argName);
      if (argValue.Trim().Length == 0)
        throw new ArgumentException(DalResourceStrings.Get(msgIfEmpty), argName);
    }

    private void ValidateArgsMetadataParms(
      MetadataTable[] tablesRequested,
      long[] rowVersions,
      Payload metadataPayload)
    {
      if (tablesRequested != null && tablesRequested.Length != 0 && metadataPayload == null)
        throw new ArgumentNullException(nameof (metadataPayload));
      if (tablesRequested != null && rowVersions != null && tablesRequested.Length != rowVersions.Length)
        throw new ArgumentException(DalResourceStrings.Get("InvalidRowVersionsProvidedException"), nameof (tablesRequested));
    }

    private void ValidateArgsRequireMetadataParms(
      MetadataTable[] tablesRequested,
      long[] rowVersions,
      Payload metadataPayload)
    {
      if (tablesRequested == null)
        throw new ArgumentNullException(nameof (tablesRequested));
      if (tablesRequested.Length == 0)
        throw new ArgumentException(DalResourceStrings.Get("InvalidMetadataTablesRequestedException"), nameof (tablesRequested));
      if (rowVersions == null)
        throw new ArgumentNullException(nameof (rowVersions));
      if (rowVersions.Length == 0)
        throw new ArgumentException(DalResourceStrings.Get("InvalidRowVersionsProvidedException"), nameof (rowVersions));
      if (tablesRequested.Length != rowVersions.Length)
        throw new ArgumentException(DalResourceStrings.Get("InvalidRowVersionsProvidedException"), nameof (tablesRequested));
      if (metadataPayload == null)
        throw new ArgumentNullException(nameof (metadataPayload));
    }

    internal static string TranslatePath(string path)
    {
      string[] strArray = path.Split('\\');
      StringBuilder stringBuilder = new StringBuilder();
      for (int index = 0; index < strArray.Length; ++index)
      {
        if (index != 3 && strArray[index].Length != 0)
        {
          stringBuilder.Append("\\");
          stringBuilder.Append(strArray[index]);
        }
      }
      return stringBuilder.ToString();
    }

    private void ExtractWorkItemDataFromBatch(
      XmlElement updateElement,
      out int workItemAreaId,
      out int workItemId)
    {
      this.m_requestContext.TraceEnter(900081, "DataAccessLayer", nameof (DataAccessLayerImpl), "ExtractIdentifierFromBatch");
      workItemAreaId = 0;
      workItemId = 0;
      XmlNodeList childNodes = updateElement.ChildNodes;
      if (childNodes != null)
      {
        if (childNodes.Count == 0)
          return;
        foreach (XmlNode node in childNodes)
        {
          if (node.NodeType == XmlNodeType.Element)
          {
            string x = node.Name;
            if (!string.IsNullOrEmpty(x))
              x = x.Trim();
            if (!string.IsNullOrEmpty(x))
            {
              if (TFStringComparer.UpdateAction.Equals(x, "InsertWorkItem"))
              {
                workItemAreaId = DataAccessLayerImpl.GetAreaId(node);
                break;
              }
              if (TFStringComparer.UpdateAction.Equals(x, "UpdateWorkItem"))
              {
                workItemId = DataAccessLayerImpl.GetWorkItemId(node);
                break;
              }
            }
          }
        }
      }
      this.m_requestContext.TraceLeave(900082, "DataAccessLayer", nameof (DataAccessLayerImpl), "ExtractIdentifierFromBatch");
    }

    private static int GetWorkItemId(XmlNode node)
    {
      int result = 0;
      if (node.Attributes != null)
      {
        XmlAttribute namedItem = (XmlAttribute) node.Attributes.GetNamedItem("WorkItemID");
        string s = string.Empty;
        if (namedItem != null)
          s = namedItem.Value;
        if (!string.IsNullOrEmpty(s))
          s = s.Trim();
        if (!string.IsNullOrEmpty(s))
          int.TryParse(s, out result);
      }
      return result;
    }

    private static int GetAreaId(XmlNode node)
    {
      int result = 0;
      XmlNodeList xmlNodeList = node.SelectNodes("Columns/Column");
      if (xmlNodeList != null)
      {
        foreach (XmlNode xmlNode1 in xmlNodeList)
        {
          if (xmlNode1.Attributes != null)
          {
            XmlAttribute namedItem = (XmlAttribute) xmlNode1.Attributes.GetNamedItem("Column");
            if (namedItem != null)
            {
              string x = namedItem.Value;
              if (!string.IsNullOrEmpty(x))
                x = x.Trim();
              if (!string.IsNullOrEmpty(x))
              {
                string y = "System.AreaId";
                if (TFStringComparer.WorkItemFieldReferenceName.Equals(x, y))
                {
                  string s = string.Empty;
                  XmlNode xmlNode2 = xmlNode1.SelectSingleNode("Value");
                  if (xmlNode2 != null)
                  {
                    s = xmlNode2.InnerText;
                    if (!string.IsNullOrEmpty(s))
                      s = s.Trim();
                  }
                  if (!string.IsNullOrEmpty(s))
                  {
                    int.TryParse(s, out result);
                    break;
                  }
                }
              }
            }
          }
        }
      }
      return result;
    }

    internal static bool IsCreatingProject(IVssRequestContext tfRequestContext, string projectUri)
    {
      tfRequestContext.TraceEnter(900420, "DataAccessLayer", "QueryItemMethods", nameof (IsCreatingProject));
      try
      {
        IProjectService service1 = tfRequestContext.GetService<IProjectService>();
        try
        {
          Guid id;
          CommonStructureUtils.CheckAndNormalizeUri(projectUri, nameof (projectUri), true, out id);
          ProjectInfo project = service1.GetProject(tfRequestContext.Elevate(), id);
          if (project != null)
          {
            if (project.State == ProjectState.New)
            {
              tfRequestContext.Trace(900421, TraceLevel.Verbose, "DataAccessLayer", "QueryItemMethods", "Project {0} is being created.", (object) projectUri);
              IdentityService service2 = tfRequestContext.GetService<IdentityService>();
              Microsoft.VisualStudio.Services.Identity.Identity identity = service2.ReadIdentities(tfRequestContext.Elevate(), IdentitySearchFilter.AdministratorsGroup, project.Uri, QueryMembership.None, (IEnumerable<string>) null).FirstOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>();
              bool flag = false;
              if (identity != null)
                flag = service2.IsMemberOrSame(tfRequestContext, identity.Descriptor);
              tfRequestContext.Trace(900422, TraceLevel.Verbose, "DataAccessLayer", "QueryItemMethods", "isProjectAdmin: {0}.", (object) flag);
              return flag;
            }
          }
        }
        catch (TeamFoundationServiceException ex)
        {
          tfRequestContext.Trace(900276, TraceLevel.Verbose, "DataAccessLayer", nameof (DataAccessLayerImpl), "DataAccessLayerImpl.IsCreatingProject: Project doesn't exist");
        }
        catch (ProjectException ex)
        {
          tfRequestContext.Trace(900277, TraceLevel.Verbose, "DataAccessLayer", nameof (DataAccessLayerImpl), "DataAccessLayerImpl.IsCreatingProject: Project doesn't exist");
        }
        return false;
      }
      finally
      {
        tfRequestContext.TraceLeave(900434, "DataAccessLayer", "QueryItemMethods", nameof (IsCreatingProject));
      }
    }

    private bool ShouldBypassWorkItemReadPermissions(IVssRequestContext context)
    {
      IAuthorizationProviderFactory extension = context.GetExtension<IAuthorizationProviderFactory>();
      return context.IsSystemContext || extension.IsPermitted(context, PermissionNamespaces.Global, "SYNCHRONIZE_READ", context.UserContext);
    }

    private IDictionary<string, int> GetTableSizes(
      MetadataTable[] tablesRequested,
      Payload metadataPayload)
    {
      Dictionary<string, int> tableSizes = new Dictionary<string, int>(tablesRequested.Length);
      foreach (MetadataTable metadataTable in tablesRequested)
      {
        string str = metadataTable.ToString();
        PayloadTable table;
        if (metadataPayload.Tables.TryGetTable(str, out table) && table.IsLoaded)
          tableSizes[str] = table.RowCount;
      }
      return (IDictionary<string, int>) tableSizes;
    }

    public void GetFields(
      out IEnumerable<FieldDefinitionRecord> fieldDefinitions,
      out IEnumerable<FieldUsageRecord> fieldUsages)
    {
      this.m_requestContext.TraceEnter(900087, "DataAccessLayer", nameof (DataAccessLayerImpl), nameof (GetFields));
      List<FieldDefinitionRecord> items;
      using (ScopedMetadataComponent component = ScopedMetadataComponent.CreateComponent(this.m_requestContext))
      {
        using (ResultCollection fields = component.GetFields())
        {
          items = fields.GetCurrent<FieldDefinitionRecord>().Items;
          fields.NextResult();
          fieldUsages = (IEnumerable<FieldUsageRecord>) fields.GetCurrent<FieldUsageRecord>().Items;
        }
      }
      fieldDefinitions = (IEnumerable<FieldDefinitionRecord>) items;
      this.m_requestContext.TraceLeave(900088, "DataAccessLayer", nameof (DataAccessLayerImpl), nameof (GetFields));
    }

    public IEnumerable<RuleRecord> GetRules(int projectId, string workItemTypeName) => this.m_requestContext.TraceBlock<IEnumerable<RuleRecord>>(900091, 900092, "DataAccessLayer", nameof (DataAccessLayerImpl), nameof (GetRules), (Func<IEnumerable<RuleRecord>>) (() =>
    {
      this.m_requestContext.Trace(900144, TraceLevel.Verbose, "DataAccessLayer", nameof (DataAccessLayerImpl), "ProjectId: {0}, WorkItemTypeName: {1}", (object) projectId.ToString((IFormatProvider) CultureInfo.InvariantCulture), (object) workItemTypeName);
      using (WorkItemTrackingMetadataComponent component = this.m_requestContext.CreateComponent<WorkItemTrackingMetadataComponent>())
        return component.GetRules(projectId, workItemTypeName);
    }));

    public IDictionary<ConstantSetReference, SetRecord[]> GetConstantSets(
      IEnumerable<ConstantSetReference> setReferences)
    {
      return this.m_requestContext.TraceBlock<IDictionary<ConstantSetReference, SetRecord[]>>(900093, 900094, "DataAccessLayer", nameof (DataAccessLayerImpl), nameof (GetConstantSets), (Func<IDictionary<ConstantSetReference, SetRecord[]>>) (() =>
      {
        using (WorkItemTrackingMetadataComponent component = this.m_requestContext.CreateComponent<WorkItemTrackingMetadataComponent>())
          return component.GetConstantSets(setReferences);
      }));
    }

    public Dictionary<int, string> GetConstants(IEnumerable<int> constantIds) => this.m_requestContext.TraceBlock<Dictionary<int, string>>(900095, 900096, "DataAccessLayer", nameof (DataAccessLayerImpl), nameof (GetConstants), (Func<Dictionary<int, string>>) (() =>
    {
      using (ScopedMetadataComponent component = ScopedMetadataComponent.CreateComponent(this.m_requestContext))
        return component.GetConstants(constantIds);
    }));

    public IEnumerable<string> GetConstantSet(
      int setId,
      bool direct,
      bool includeGroups,
      bool includeTop)
    {
      return this.m_requestContext.TraceBlock<IEnumerable<string>>(900097, 900098, "DataAccessLayer", nameof (DataAccessLayerImpl), nameof (GetConstantSet), (Func<IEnumerable<string>>) (() =>
      {
        this.m_requestContext.Trace(900148, TraceLevel.Verbose, "DataAccessLayer", nameof (DataAccessLayerImpl), "SetId: {0}, Direct: {1}, IncludeGroups: {2}, IncludeTop: {3}", (object) setId.ToString((IFormatProvider) CultureInfo.InvariantCulture), (object) direct.ToString(), (object) includeGroups.ToString(), (object) includeTop.ToString());
        using (WorkItemTrackingMetadataComponent component = this.m_requestContext.CreateComponent<WorkItemTrackingMetadataComponent>())
        {
          ConstantSetReference key = new ConstantSetReference(setId, direct, includeGroups, includeTop);
          SetRecord[] source;
          if (!component.GetConstantSets((IEnumerable<ConstantSetReference>) new ConstantSetReference[1]
          {
            key
          }).TryGetValue(key, out source))
            return Enumerable.Empty<string>();
          string[] array = ((IEnumerable<SetRecord>) source).Select<SetRecord, string>((Func<SetRecord, string>) (sr => sr.Item)).Distinct<string>((IEqualityComparer<string>) StringComparer.CurrentCultureIgnoreCase).ToArray<string>();
          Array.Sort<string>(array, (IComparer<string>) StringComparer.CurrentCultureIgnoreCase);
          return (IEnumerable<string>) array;
        }
      }));
    }

    public IEnumerable<string> GetGlobalAndProjectGroups(int projectId, bool includeGlobal)
    {
      this.m_requestContext.TraceEnter(900105, "DataAccessLayer", nameof (DataAccessLayerImpl), nameof (GetGlobalAndProjectGroups));
      this.m_requestContext.Trace(900156, TraceLevel.Verbose, "DataAccessLayer", nameof (DataAccessLayerImpl), "ProjectId: {0}, IncludeGlobal: {1}", (object) projectId.ToString((IFormatProvider) CultureInfo.InvariantCulture), (object) includeGlobal.ToString());
      IEnumerable<string> andProjectGroups;
      using (ScopedMetadataComponent component = ScopedMetadataComponent.CreateComponent(this.m_requestContext))
        andProjectGroups = component.GetGlobalAndProjectGroups(projectId, includeGlobal);
      this.m_requestContext.TraceLeave(900106, "DataAccessLayer", nameof (DataAccessLayerImpl), nameof (GetGlobalAndProjectGroups));
      return andProjectGroups;
    }

    public ConstantRecord[] GetConstantRecords(
      string[] searchValues,
      ConstantRecordSearchFactor searchFactor,
      bool includeInactiveIdentities = false)
    {
      return this.m_requestContext.TraceBlock<ConstantRecord[]>(900107, 900108, "DataAccessLayer", nameof (DataAccessLayerImpl), "GetConstantRecords()", (Func<ConstantRecord[]>) (() =>
      {
        ILookup<int, ConstantRecord> lookup = this.GetConstantRecordsAll(searchValues, searchFactor, includeInactiveIdentities).ToLookup<ConstantRecord, int>((Func<ConstantRecord, int>) (cr => cr.Index));
        return Enumerable.Range(0, searchValues.Length).Select<int, ConstantRecord>((Func<int, ConstantRecord>) (index => lookup[index].LastOrDefault<ConstantRecord>())).ToArray<ConstantRecord>();
      }));
    }

    public List<ConstantRecord> GetConstantRecordsAll(
      string[] searchValues,
      ConstantRecordSearchFactor searchFactor,
      bool includeInactiveIdentities = false)
    {
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) searchValues, nameof (searchValues));
      if (((IEnumerable<string>) searchValues).Any<string>((Func<string, bool>) (sv => sv == null)))
        throw new ArgumentNullException(nameof (searchValues));
      if (searchFactor == ConstantRecordSearchFactor.TeamFoundationId)
        searchValues = ((IEnumerable<string>) searchValues).Select<string, string>((Func<string, string>) (sv =>
        {
          Guid result;
          if (!Guid.TryParse(sv, out result))
            throw new ArgumentException(ServerResources.GetConstantRecordInvalidGuidString(), nameof (searchValues));
          return result.ToString("D");
        })).ToArray<string>();
      using (ScopedMetadataComponent component = ScopedMetadataComponent.CreateComponent(this.m_requestContext))
        return component.GetConstantRecords(searchValues, searchFactor, includeInactiveIdentities);
    }

    public MetadataDBStamps GetMetadataTableTimestamps(
      IEnumerable<MetadataTable> tableNames,
      int projectId)
    {
      return this.m_requestContext.MetadataDbStamps(tableNames);
    }

    public IEnumerable<long> GetQueryItemsTimestamps(int projectId)
    {
      using (ScopedMetadataComponent component = ScopedMetadataComponent.CreateComponent(this.m_requestContext))
        return component.GetQueryItemsTimestamps(projectId);
    }

    public List<WorkItemTypeAction> GetWorkItemTypeActions(string workItemType, string projectName)
    {
      if (WorkItemTrackingFeatureFlags.IsProcessCustomizationEnabled(this.m_requestContext))
        return this.m_requestContext.GetService<TeamFoundationWorkItemTrackingMetadataService>().GetWorkItemTypeActions(this.m_requestContext, projectName, workItemType).Select<Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.DataModels.WorkItemTypeAction, WorkItemTypeAction>((Func<Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.DataModels.WorkItemTypeAction, WorkItemTypeAction>) (a => new WorkItemTypeAction(a.WorkItemType, a.Name, a.FromState, a.ToState))).ToList<WorkItemTypeAction>();
      this.m_requestContext.TraceEnter(900057, "DataAccessLayer", nameof (DataAccessLayerImpl), nameof (GetWorkItemTypeActions));
      try
      {
        using (ScopedMetadataComponent component = ScopedMetadataComponent.CreateComponent(this.m_requestContext))
        {
          using (ResultCollection workItemTypeActions = component.GetWorkItemTypeActions(workItemType, projectName))
            return workItemTypeActions.GetCurrent<WorkItemTypeAction>().Items;
        }
      }
      finally
      {
        this.m_requestContext.TraceLeave(900058, "DataAccessLayer", nameof (DataAccessLayerImpl), nameof (GetWorkItemTypeActions));
      }
    }

    public void AddWorkItemTypeActions(string projectName, IEnumerable<WorkItemTypeAction> actions)
    {
      this.m_requestContext.TraceEnter(900682, "DataAccessLayer", nameof (DataAccessLayerImpl), nameof (AddWorkItemTypeActions));
      try
      {
        ArgumentUtility.CheckStringForNullOrEmpty(projectName, nameof (projectName));
        ArgumentUtility.CheckForNull<IEnumerable<WorkItemTypeAction>>(actions, nameof (actions));
        if (actions.Count<WorkItemTypeAction>() == 0)
          return;
        this.ProvisionHelper.Authorize(true);
        using (ScopedMetadataComponent component = ScopedMetadataComponent.CreateComponent(this.m_requestContext))
          component.AddWorkItemTypeActions(actions, projectName);
      }
      finally
      {
        this.m_requestContext.TraceLeave(900683, "DataAccessLayer", nameof (DataAccessLayerImpl), nameof (AddWorkItemTypeActions));
      }
    }

    public void SetDisplayForm(string projectName, string workItemType, XmlNode displayForm)
    {
      this.m_requestContext.TraceEnter(900688, "DataAccessLayer", nameof (DataAccessLayerImpl), nameof (SetDisplayForm));
      try
      {
        ArgumentUtility.CheckStringForNullOrEmpty(projectName, nameof (projectName));
        ArgumentUtility.CheckStringForNullOrEmpty(workItemType, nameof (workItemType));
        ArgumentUtility.CheckForNull<XmlNode>(displayForm, nameof (displayForm));
        this.ProvisionHelper.Authorize(true);
        using (ScopedMetadataComponent component = ScopedMetadataComponent.CreateComponent(this.m_requestContext))
          component.SetDisplayForm(projectName, workItemType, displayForm.OuterXml);
      }
      finally
      {
        this.m_requestContext.TraceLeave(900689, "DataAccessLayer", nameof (DataAccessLayerImpl), nameof (SetDisplayForm));
      }
    }

    private ProvisionHelper ProvisionHelper
    {
      get
      {
        if (this.m_provisionHelper == null)
          this.m_provisionHelper = new ProvisionHelper(this.m_requestContext);
        return this.m_provisionHelper;
      }
    }

    private class MetadataDownloadState : IDownloadState<VirtualFileInformation>, IDisposable
    {
      private bool m_disposedValue;
      private DataAccessLayerImpl m_dal;
      private Stream m_fileStream;

      public MetadataDownloadState(DataAccessLayerImpl dal)
      {
        ArgumentUtility.CheckForNull<DataAccessLayerImpl>(dal, nameof (dal));
        this.m_dal = dal;
      }

      public Stream CacheMiss(
        FileCacheService fileCacheService,
        VirtualFileInformation fileInfo,
        bool compressOutput)
      {
        throw new NotSupportedException();
      }

      public bool TransmitChunk(
        VirtualFileInformation fileInformation,
        byte[] chunk,
        long offset,
        long length)
      {
        return true;
      }

      public bool TransmitFile(
        VirtualFileInformation fileInfo,
        string path,
        long offset,
        long length)
      {
        this.CheckDisposed();
        this.m_fileStream = (Stream) File.OpenRead(path);
        this.m_fileStream.Seek(offset, SeekOrigin.Begin);
        return true;
      }

      public Stream GetTargetStream() => this.m_fileStream;

      private void CheckDisposed()
      {
        if (this.m_disposedValue)
          throw new ObjectDisposedException(this.GetType().FullName);
      }

      protected virtual void Dispose(bool disposing)
      {
        if (this.m_disposedValue)
          return;
        if (disposing)
        {
          if (this.m_fileStream != null)
          {
            this.m_fileStream.Dispose();
            this.m_fileStream = (Stream) null;
          }
          this.m_dal = (DataAccessLayerImpl) null;
        }
        this.m_disposedValue = true;
      }

      public void Dispose() => this.Dispose(true);
    }

    private class GetMetadataOutputParameters
    {
      internal int locale;
      internal int comparisonStyle;
      internal string callerIdentity;
      internal string dbStamp;
      internal int mode;
    }
  }
}
