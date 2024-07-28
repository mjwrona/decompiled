// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManager.Actions.ForceMergeIndicesAction
// Assembly: Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManager.Actions, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DE7D0F19-C193-43CC-9602-3C8794FE9CA0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManager.Actions.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Indexer;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions;
using Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManagerAPI;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManager.Actions
{
  public class ForceMergeIndicesAction : AbstractAction
  {
    private readonly ElasticsearchFeedbackProcessor m_elasticsearchFeedbackProcessor;
    private readonly string m_methodName = "Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManager.DataProviders.EsIndexDocumentCountDataProvider.Initialize";
    private ISearchClusterManagementService m_clusterManagementService;
    private int m_cpuThreshold;
    private readonly DayOfWeek m_currentDay;
    private string m_esConnectionString;
    private ExecutionContext m_executionContext;
    private string m_indicesOptimized;
    internal MaintenanceJobStatus m_jobStatus;
    private int m_jvmUsageThreshold;
    private int m_memoryThreshold;
    private bool m_notOptimizedRecently;
    internal ISearchPlatform m_searchPlatform;
    internal ISearchPlatformFactory m_searchPlatformFactory;

    public ForceMergeIndicesAction(Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManagerAPI.ActionType actionType, ActionContext actionContext)
      : this(Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer.DataAccessFactory.GetInstance(), actionType, actionContext, SearchPlatformFactory.GetInstance(), DateTime.Now.DayOfWeek)
    {
    }

    [Info("InternalForTestPurpose")]
    internal ForceMergeIndicesAction(
      IDataAccessFactory dataAccessFactory,
      Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManagerAPI.ActionType healthAction,
      ActionContext actionContext,
      ISearchPlatformFactory searchPlatformFactory,
      DayOfWeek currentDay)
      : base(dataAccessFactory, healthAction, actionContext)
    {
      this.m_searchPlatformFactory = searchPlatformFactory;
      this.m_elasticsearchFeedbackProcessor = new ElasticsearchFeedbackProcessor();
      this.m_currentDay = currentDay;
    }

    protected internal MaintenanceJobStatus LastJobStatus { get; set; }

    public override bool IsLongRunning() => true;

    public override bool IsCompleted(IVssRequestContext requestContext)
    {
      List<string> indices = this.ActionContext.Indices;
      if ((indices != null ? (!indices.Any<string>() ? 1 : 0) : 1) != 0)
        return true;
      IReadOnlyCollection<CatIndicesRecord> records = this.m_searchPlatform.GetIndices(this.m_executionContext, indices)?.Records;
      if (records != null && records.Any<CatIndicesRecord>() && indices.Count == records.Count)
        return records.Count<CatIndicesRecord>((Func<CatIndicesRecord, bool>) (it => this.m_elasticsearchFeedbackProcessor.IsHavingHighDeletedDocCountPercentage(requestContext, it))) == 0;
      throw new SearchServiceException(FormattableString.Invariant(FormattableStringFactory.Create("Indices Information not matched with ES. Polled {0} information and got information for {1}", (object) indices.Count, (object) records?.Count)));
    }

    public override void Invoke(IVssRequestContext requestContext, out string resultMessage)
    {
      this.Initialize(requestContext);
      if (this.PreRunCheck(requestContext))
      {
        if (this.ResourceAvailabilityCheck())
        {
          List<string> indices = this.ActionContext.Indices;
          if (indices != null && indices.Any<string>())
          {
            this.m_indicesOptimized = indices.Aggregate<string>((Func<string, string, string>) ((i, j) => i + "," + j));
            this.m_clusterManagementService.ForceMergeIndicesAsync(indices);
            this.m_jobStatus = MaintenanceJobStatus.InProgress;
            resultMessage = FormattableString.Invariant(FormattableStringFactory.Create("Elasticsearch Index Optimize Request Sent for {0} indices", (object) this.m_indicesOptimized));
          }
          else
          {
            this.m_indicesOptimized = string.Empty;
            this.m_jobStatus = MaintenanceJobStatus.Succeeded;
            resultMessage = "Elasticsearch Index Optimze Not needed, as none of the Indices are having deleted document count beyond threshold";
          }
        }
        else
          resultMessage = FormattableString.Invariant(FormattableStringFactory.Create("{0} failed , will be retried in next iteration", (object) "ResourceAvailabilityCheck"));
      }
      else
        resultMessage = FormattableString.Invariant(FormattableStringFactory.Create("{0} failed , exiting now", (object) "PreRunCheck"));
    }

    [Info("InternalForTestPurpose")]
    internal void Initialize(IVssRequestContext requestContext)
    {
      this.m_esConnectionString = requestContext.GetConfigValue("/Service/ALMSearch/Settings/JobAgentSearchPlatformConnectionString");
      string platformSettings = requestContext.GetElasticsearchPlatformSettings("/Service/ALMSearch/Settings/JobAgentSearchPlatformSettings");
      this.m_clusterManagementService = this.m_searchPlatformFactory.CreateSearchClusterManagementService(this.m_esConnectionString, platformSettings, requestContext.ExecutionEnvironment.IsOnPremisesDeployment);
      this.m_searchPlatform = this.m_searchPlatformFactory.Create(this.m_esConnectionString, platformSettings, false);
      this.m_executionContext = requestContext.GetExecutionContext(TracerCICorrelationDetailsFactory.GetCICorrelationDetails(requestContext.ActivityId.ToString(), this.m_methodName, 0));
      this.m_cpuThreshold = requestContext.GetConfigValue<int>("/Service/ALMSearch/Settings/CpuThreshold", TeamFoundationHostType.Deployment, 70);
      this.m_memoryThreshold = requestContext.GetConfigValue<int>("/Service/ALMSearch/Settings/MemoryThreshold", TeamFoundationHostType.Deployment, 95);
      this.m_jvmUsageThreshold = requestContext.GetConfigValue<int>("/Service/ALMSearch/Settings/JvmUsageThreshold", TeamFoundationHostType.Deployment, 90);
      this.m_notOptimizedRecently = DateTime.Now - requestContext.GetConfigValue<DateTime>("/Service/ALMSearch/Settings/LastOptimizeDate", TeamFoundationHostType.Deployment) > TimeSpan.FromHours(72.0);
      this.LastJobStatus = requestContext.GetConfigValue<MaintenanceJobStatus>("/Service/ALMSearch/Settings/OptimizeOnGoingState", TeamFoundationHostType.Deployment, MaintenanceJobStatus.Succeeded);
    }

    [Info("InternalForTestPurpose")]
    internal bool ResourceAvailabilityCheck() => this.m_clusterManagementService.GetClusterResourceUsage().Nodes.Values.All<NodeStats>((Func<NodeStats, bool>) (it => (double) it.OperatingSystem.Cpu.Percent < (double) this.m_cpuThreshold && it.OperatingSystem.Memory.UsedPercent < this.m_memoryThreshold && it.Jvm.Memory.HeapUsedPercent < (long) this.m_jvmUsageThreshold));

    protected internal bool PreRunCheck(IVssRequestContext requestContext) => ((requestContext.GetConfigValue<bool>("/Service/ALMSearch/Settings/ForceMergeEsIndices", TeamFoundationHostType.Deployment) ? 1 : 0) | (!this.m_currentDay.IsWeekend() || !this.LastJobStatus.IsCompletedOrPending() ? (false ? 1 : 0) : (this.m_notOptimizedRecently ? 1 : 0))) != 0;
  }
}
