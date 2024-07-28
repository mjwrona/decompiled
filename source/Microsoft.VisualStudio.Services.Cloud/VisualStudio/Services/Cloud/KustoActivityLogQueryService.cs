// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.KustoActivityLogQueryService
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.VisualStudio.Services.Cloud
{
  public class KustoActivityLogQueryService : 
    KustoQueryServiceBase,
    IKustoActivityLogQueryService,
    IVssFrameworkService
  {
    private static readonly string[] s_activityLog = new string[1]
    {
      KustoWellKnownDataSources.ActivityLog
    };
    private static readonly string[] s_activityLog_XEventDataRPCCompleted = new string[2]
    {
      KustoWellKnownDataSources.ActivityLog,
      KustoWellKnownDataSources.XEventDataRPCCompleted
    };
    private static readonly string[] s_activityLog_ActivityLogMapping_XEventDataRPCCompleted = new string[3]
    {
      KustoWellKnownDataSources.ActivityLog,
      KustoWellKnownDataSources.ActivityLogMapping,
      KustoWellKnownDataSources.XEventDataRPCCompleted
    };
    private static readonly string[] s_activityLog_XEventDataRPCCompleted_OrchestrationPlanContext = new string[3]
    {
      KustoWellKnownDataSources.ActivityLog,
      KustoWellKnownDataSources.XEventDataRPCCompleted,
      KustoWellKnownDataSources.OrchestrationPlanContext
    };

    protected override string Layer => nameof (KustoActivityLogQueryService);

    public override void ServiceStart(IVssRequestContext systemRequestContext) => systemRequestContext.CheckProjectCollectionRequestContext();

    public override void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public IDataReader QueryActivityLogGeneral(IVssRequestContext requestContext, string query) => this.Query(requestContext, KustoActivityLogQueryService.s_activityLog, query, this.GetKustoQueryConfig(requestContext));

    public IEnumerable<ActivityLogAggregated> QueryActivityLogAggregated(
      IVssRequestContext requestContext,
      string query)
    {
      return this.Query<ActivityLogAggregated>(requestContext, KustoActivityLogQueryService.s_activityLog, query, this.GetKustoQueryConfig(requestContext));
    }

    public IDataReader QueryActivityLogXEventDataRPCCompletedGeneral(
      IVssRequestContext requestContext,
      string query,
      bool needActivityLogMapping = false)
    {
      return this.Query(requestContext, needActivityLogMapping ? KustoActivityLogQueryService.s_activityLog_ActivityLogMapping_XEventDataRPCCompleted : KustoActivityLogQueryService.s_activityLog_XEventDataRPCCompleted, query, this.GetKustoQueryConfig(requestContext));
    }

    public IEnumerable<ActivityLogXEventDataAggregated> QueryActivityLogXEventDataRPCCompletedAggregated(
      IVssRequestContext requestContext,
      string query)
    {
      return this.Query<ActivityLogXEventDataAggregated>(requestContext, KustoActivityLogQueryService.s_activityLog_XEventDataRPCCompleted, query, this.GetKustoQueryConfig(requestContext));
    }

    protected override KustoQueryRestriction GetQueryRestrictions(
      IVssRequestContext requestContext,
      string[] requestedTables)
    {
      return (KustoQueryRestriction) new KustoActivityLogQueryRestriction(requestContext, requestedTables, this.GetKustoQueryConfig(requestContext));
    }

    public IDataReader QueryActivityLogXEventDataRPCCompletedOrchestrationPlanContextGeneral(
      IVssRequestContext requestContext,
      string query)
    {
      return this.Query(requestContext, KustoActivityLogQueryService.s_activityLog_XEventDataRPCCompleted_OrchestrationPlanContext, query, this.GetKustoQueryConfig(requestContext));
    }

    public bool ReadDataReader(IVssRequestContext requestContext, IDataReader dataReader) => this.ReadDataReader(requestContext, this.GetKustoQueryConfig(requestContext), dataReader);

    private KustoQueryConfig GetKustoQueryConfig(IVssRequestContext requestContext)
    {
      if (requestContext.IsFeatureEnabled("Utilization.UsageSummary.UnionVSOEUCluster"))
        return new KustoQueryConfig()
        {
          KustoClusterDestination = KustoClusterDestination.UNION,
          QueryType = KustoQueryType.MacroExpand
        };
      return new KustoQueryConfig()
      {
        KustoClusterDestination = KustoClusterDestination.VSO,
        QueryType = KustoQueryType.Default
      };
    }
  }
}
