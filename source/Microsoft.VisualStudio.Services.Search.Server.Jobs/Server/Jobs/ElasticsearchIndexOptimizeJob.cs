// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Jobs.ElasticsearchIndexOptimizeJob
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 23ACAECB-A4CB-4AA5-8366-092C41D8D4A8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Jobs.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Indexer;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions;
using Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using Nest;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.Server.Jobs
{
  public class ElasticsearchIndexOptimizeJob : AbstractElasticsearchJobs
  {
    private int m_cpuThreshold;
    private int m_memoryThreshold;
    private int m_jvmUsageThreshold;
    private bool m_notOptimizedRecently;
    private readonly DayOfWeek m_currentDay;

    public ElasticsearchIndexOptimizeJob()
      : this(Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer.DataAccessFactory.GetInstance(), Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.SearchPlatformFactory.GetInstance(), DateTime.Now.DayOfWeek)
    {
    }

    [Info("InternalForTestPurpose")]
    internal ElasticsearchIndexOptimizeJob(
      IDataAccessFactory dataAccessFactory,
      ISearchPlatformFactory searchPlatformFactory,
      DayOfWeek currentDay)
      : base(dataAccessFactory, searchPlatformFactory)
    {
      this.m_currentDay = currentDay;
    }

    [Info("InternalForTestPurpose")]
    internal bool ResourceAvailabilityCheck(out string resultMessage)
    {
      NodesStatsResponse clusterResourceUsage = this.SearchClusterStateService.GetClusterResourceUsage();
      bool cpuThresholdBreached = false;
      bool memoryThresholdBreached = false;
      bool jvmHeapUsedPercBreached = false;
      string message = "";
      clusterResourceUsage.Nodes.Values.ForEach<NodeStats>((Action<NodeStats>) (it =>
      {
        if ((double) it.OperatingSystem.Cpu.Percent > (double) this.m_cpuThreshold)
        {
          cpuThresholdBreached = true;
          message += FormattableString.Invariant(FormattableStringFactory.Create("CPU Percentage for a node ({0}) is above the threshold ({1}). ", (object) it.OperatingSystem.Cpu.Percent, (object) this.m_cpuThreshold));
        }
        if (it.OperatingSystem.Memory.UsedPercent > this.m_memoryThreshold)
        {
          memoryThresholdBreached = true;
          message += FormattableString.Invariant(FormattableStringFactory.Create("Memory used for a node ({0}) is above the threshold ({1}). ", (object) it.OperatingSystem.Memory.UsedPercent, (object) this.m_memoryThreshold));
        }
        if (it.Jvm.Memory.HeapUsedPercent <= (long) this.m_jvmUsageThreshold)
          return;
        jvmHeapUsedPercBreached = true;
        message += FormattableString.Invariant(FormattableStringFactory.Create("JVM heap memory used for a node ({0}) is above the threshold ({1}). ", (object) it.Jvm.Memory.HeapUsedPercent, (object) this.m_jvmUsageThreshold));
      }));
      resultMessage = message;
      return !(cpuThresholdBreached | jvmHeapUsedPercBreached | memoryThresholdBreached);
    }

    protected internal override void Initialize(
      IVssRequestContext requestContext,
      TeamFoundationJobDefinition jobDefinition)
    {
      base.Initialize(requestContext, jobDefinition);
      this.TracePoint = 1083040;
      this.SearchEventIdentifier = SearchEventId.ElasticsearchIndexOptimize;
      this.m_cpuThreshold = requestContext.GetConfigValue<int>("/Service/ALMSearch/Settings/CpuThreshold", TeamFoundationHostType.Deployment, 70);
      this.m_memoryThreshold = requestContext.GetConfigValue<int>("/Service/ALMSearch/Settings/MemoryThreshold", TeamFoundationHostType.Deployment, 95);
      this.m_jvmUsageThreshold = requestContext.GetConfigValue<int>("/Service/ALMSearch/Settings/JvmUsageThreshold", TeamFoundationHostType.Deployment, 90);
      this.m_lastJobStatus = requestContext.GetConfigValue<MaintenanceJobStatus>("/Service/ALMSearch/Settings/OptimizeOnGoingState", TeamFoundationHostType.Deployment, MaintenanceJobStatus.Succeeded);
      this.m_notOptimizedRecently = DateTime.Now - requestContext.GetConfigValue<DateTime>("/Service/ALMSearch/Settings/LastOptimizeDate", TeamFoundationHostType.Deployment) > TimeSpan.FromHours(72.0);
      this.ExecutionContext = requestContext.GetExecutionContext(TracerCICorrelationDetailsFactory.GetCICorrelationDetails(requestContext.ActivityId.ToString(), jobDefinition.Name, 51));
    }

    internal void ForceMergeElasticsearchIndices(
      IVssRequestContext requestContext,
      out string resultMessage)
    {
      requestContext.SetConfigValue<bool>("/Service/ALMSearch/Settings/ForceMergeEsIndices", false);
      List<string> docsBeyondThreshold = this.GetIndicesHavingDeletedDocsBeyondThreshold();
      string str;
      if (docsBeyondThreshold.Any<string>())
      {
        str = docsBeyondThreshold.Aggregate<string>((Func<string, string, string>) ((i, j) => i + "," + j));
        this.SearchClusterStateService.ForceMergeIndicesAsync(docsBeyondThreshold);
        this.UpdateMaintenanceJobStatus(requestContext, MaintenanceJobStatus.InProgress);
        resultMessage = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Elasticsearch Index Optimize Request Sent for {0} indices", (object) str);
      }
      else
      {
        str = string.Empty;
        this.UpdateMaintenanceJobStatus(requestContext, MaintenanceJobStatus.Succeeded);
        resultMessage = FormattableString.Invariant(FormattableStringFactory.Create("Elasticsearch Index Optimze Not needed, as none of the Indices are having deleted document count beyond threshold"));
      }
      requestContext.SetConfigValue<string>("/Service/ALMSearch/Settings/OptimizedIndices", str);
      requestContext.SetConfigValue<DateTime>("/Service/ALMSearch/Settings/LastOptimizeDate", DateTime.Now);
    }

    protected internal override TeamFoundationJobExecutionResult InvokeJob(
      IVssRequestContext requestContext,
      out string resultMessage)
    {
      if (this.ResourceAvailabilityCheck(out resultMessage))
        this.ForceMergeElasticsearchIndices(requestContext, out resultMessage);
      else
        this.UpdateMaintenanceJobStatus(requestContext, MaintenanceJobStatus.Pending);
      return TeamFoundationJobExecutionResult.Succeeded;
    }

    protected internal override bool PreRunCheck(IVssRequestContext requestContext) => ((requestContext.GetConfigValue<bool>("/Service/ALMSearch/Settings/ForceMergeEsIndices", TeamFoundationHostType.Deployment) ? 1 : 0) | (!this.m_currentDay.IsWeekend() || this.m_lastJobStatus == MaintenanceJobStatus.InProgress ? (false ? 1 : 0) : (this.m_notOptimizedRecently ? 1 : 0))) != 0;

    protected internal override void UpdateMaintenanceJobStatus(
      IVssRequestContext requestContext,
      MaintenanceJobStatus maintenanceJobStatus)
    {
      this.m_lastJobStatus = maintenanceJobStatus;
      requestContext.SetConfigValue<MaintenanceJobStatus>("/Service/ALMSearch/Settings/OptimizeOnGoingState", this.m_lastJobStatus);
    }

    protected internal override void MonitorOnGoingJob(IVssRequestContext requestContext)
    {
      if (this.m_lastJobStatus != MaintenanceJobStatus.InProgress)
        return;
      string configValue = requestContext.GetConfigValue<string>("/Service/ALMSearch/Settings/OptimizedIndices");
      List<string> stringList;
      if (configValue == null)
        stringList = (List<string>) null;
      else
        stringList = ((IEnumerable<string>) configValue.Split(',')).ToList<string>();
      if (stringList == null)
        stringList = new List<string>();
      List<string> second = stringList;
      if (!this.GetIndicesHavingDeletedDocsBeyondThreshold().Intersect<string>((IEnumerable<string>) second).Any<string>())
      {
        this.UpdateMaintenanceJobStatus(requestContext, MaintenanceJobStatus.Succeeded);
      }
      else
      {
        if (!this.m_notOptimizedRecently)
          return;
        this.UpdateMaintenanceJobStatus(requestContext, MaintenanceJobStatus.Failed);
      }
    }

    protected internal override string QueueSelfJob(IVssRequestContext requestContext, int delay) => "";

    private List<string> GetIndicesHavingDeletedDocsBeyondThreshold() => this.SearchPlatform.GetIndices(this.ExecutionContext).Records.Where<CatIndicesRecord>((Func<CatIndicesRecord, bool>) (i => this.ElasticsearchFeedbackProcessor.IsHavingHighDeletedDocCountPercentage(this.ExecutionContext.RequestContext, i))).Select<CatIndicesRecord, string>((Func<CatIndicesRecord, string>) (j => j.Index)).ToList<string>();
  }
}
