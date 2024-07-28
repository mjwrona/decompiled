// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.CodeCoverageSummaryUpdater
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  public class CodeCoverageSummaryUpdater : ICoverageSummaryUpdater
  {
    private IBuildServiceHelper m_buildServiceHelper;

    public CodeCoverageSummaryUpdater()
    {
    }

    public CodeCoverageSummaryUpdater(IBuildServiceHelper buildServiceHelper) => this.BuildServiceHelper = buildServiceHelper;

    public void UpdateCodeCoverageSummary(
      TestManagementRequestContext context,
      PipelineContext pipelineContext,
      string buildPlatform,
      string buildFlavor,
      CoverageSummaryStatus summaryStatus)
    {
      using (new SimpleTimer(context.RequestContext, string.Format("CoverageMonitorJob: UpdateCodeCoverageSummary {0} {1}", (object) pipelineContext.Id, (object) pipelineContext.ProjectId)))
      {
        BuildConfiguration buildConfiguration = new BuildConfiguration()
        {
          BuildPlatform = buildPlatform,
          BuildFlavor = buildFlavor,
          BuildUri = pipelineContext.Uri,
          BuildId = pipelineContext.Id,
          TeamProjectName = context.ProjectServiceHelper.GetProjectName(pipelineContext.ProjectId)
        }.QueryWithPlatformAndFlavor(context.RequestContext, pipelineContext.ProjectId, pipelineContext.Id, buildPlatform, buildFlavor);
        buildConfiguration.BuildId = pipelineContext.Id;
        this.UpdateCodeCoverageSummary(context, buildConfiguration, pipelineContext.ProjectId, summaryStatus);
      }
    }

    public void UpdateCodeCoverageSummary(
      TestManagementRequestContext context,
      PipelineContext pipelineContext,
      string buildPlatform,
      string buildFlavor,
      CoverageSummaryStatus summaryStatus,
      CoverageDetailedSummaryStatus coverageDetailedSummaryStatus)
    {
      using (new SimpleTimer(context.RequestContext, string.Format("CoverageMonitorJob: UpdateCodeCoverageSummary {0} {1}", (object) pipelineContext.Id, (object) pipelineContext.ProjectId)))
      {
        BuildConfiguration buildConfiguration = new BuildConfiguration()
        {
          BuildPlatform = buildPlatform,
          BuildFlavor = buildFlavor,
          BuildUri = pipelineContext.Uri,
          BuildId = pipelineContext.Id,
          TeamProjectName = context.ProjectServiceHelper.GetProjectName(pipelineContext.ProjectId)
        }.QueryWithPlatformAndFlavor(context.RequestContext, pipelineContext.ProjectId, pipelineContext.Id, buildPlatform, buildFlavor);
        buildConfiguration.BuildId = pipelineContext.Id;
        this.UpdateCodeCoverageSummary(context, buildConfiguration, pipelineContext.ProjectId, summaryStatus, coverageDetailedSummaryStatus);
      }
    }

    public bool UpdateCodeCoverageSummary(
      TestManagementRequestContext context,
      BuildConfiguration buildConfiguration,
      Guid projectId,
      CoverageSummaryStatus summaryStatus)
    {
      Dictionary<string, object> data = new Dictionary<string, object>();
      try
      {
        using (new SimpleTimer(context.RequestContext, string.Format("UpdateCodeCoverageSummary {0} {1}", (object) buildConfiguration.BuildId, (object) projectId)))
        {
          IList<CodeCoverageStatistics> coverageStatistics = new MSCodeCoverageTransformer().GetCoverageStatistics(context, buildConfiguration);
          if (coverageStatistics == null || coverageStatistics.Count == 0)
          {
            string format = string.Format("Zero Modules in coverageStatistics. Build uri : {0}. Project : {1}, CoverageSummaryStatus : {2}", (object) buildConfiguration.BuildUri, (object) buildConfiguration.TeamProjectName, (object) summaryStatus);
            context.Logger.Warning(1015407, format);
            data.Add("CoverageStatisticsWarning", (object) format);
          }
          CodeCoverageData coverageData = new CodeCoverageData()
          {
            BuildFlavor = buildConfiguration.BuildFlavor,
            BuildPlatform = buildConfiguration.BuildPlatform,
            CoverageStats = coverageStatistics
          };
          BuildConfiguration buildRef = this.BuildServiceHelper.QueryBuildConfigurationByBuildUri(context.RequestContext, projectId, buildConfiguration.BuildUri);
          if (buildRef != null)
          {
            buildRef.BuildFlavor = buildConfiguration.BuildFlavor;
            buildRef.BuildPlatform = buildConfiguration.BuildPlatform;
            CodeCoverageSummary.AddOrUpdateSummaryWithStatus(context, buildConfiguration.TeamProjectName, buildRef, coverageData, summaryStatus);
            return true;
          }
          string format1 = string.Format("CoverageAnalyzer: Build not found. Build uri : {0}. Project : {1}, CoverageSummaryStatus : {2}", (object) buildConfiguration.BuildUri, (object) buildConfiguration.TeamProjectName, (object) summaryStatus);
          context.Logger.Warning(1015405, format1);
          data.Add("BuildConfigWarning", (object) format1);
          return false;
        }
      }
      finally
      {
        TelemetryLogger.Instance.PublishData(context.RequestContext, nameof (CodeCoverageSummaryUpdater), new CustomerIntelligenceData((IDictionary<string, object>) data));
      }
    }

    public bool UpdateCodeCoverageSummary(
      TestManagementRequestContext context,
      BuildConfiguration buildConfiguration,
      Guid projectId,
      CoverageSummaryStatus summaryStatus,
      CoverageDetailedSummaryStatus coverageDetailedSummaryStatus)
    {
      Dictionary<string, object> data = new Dictionary<string, object>();
      try
      {
        using (new SimpleTimer(context.RequestContext, string.Format("UpdateCodeCoverageSummary {0} {1}", (object) buildConfiguration.BuildId, (object) projectId)))
        {
          IList<CodeCoverageStatistics> coverageStatistics = new MSCodeCoverageTransformer().GetCoverageStatistics(context, buildConfiguration);
          if (coverageStatistics == null || coverageStatistics.Count == 0)
          {
            string format = string.Format("Zero Modules in coverageStatistics. Build uri : {0}. Project : {1}, CoverageSummaryStatus : {2}", (object) buildConfiguration.BuildUri, (object) buildConfiguration.TeamProjectName, (object) coverageDetailedSummaryStatus);
            context.Logger.Warning(1015407, format);
            data.Add("CoverageStatisticsWarning", (object) format);
          }
          CodeCoverageData coverageData = new CodeCoverageData()
          {
            BuildFlavor = buildConfiguration.BuildFlavor,
            BuildPlatform = buildConfiguration.BuildPlatform,
            CoverageStats = coverageStatistics
          };
          BuildConfiguration buildRef = this.BuildServiceHelper.QueryBuildConfigurationByBuildUri(context.RequestContext, projectId, buildConfiguration.BuildUri);
          if (buildRef != null)
          {
            buildRef.BuildFlavor = buildConfiguration.BuildFlavor;
            buildRef.BuildPlatform = buildConfiguration.BuildPlatform;
            CodeCoverageSummary.AddOrUpdateSummaryWithStatus(context, buildConfiguration.TeamProjectName, buildRef, coverageData, summaryStatus, coverageDetailedSummaryStatus);
            return true;
          }
          string format1 = string.Format("CoverageAnalyzer: Build not found. Build uri : {0}. Project : {1}, CoverageDetailedSummaryStatus : {2}", (object) buildConfiguration.BuildUri, (object) buildConfiguration.TeamProjectName, (object) coverageDetailedSummaryStatus);
          context.Logger.Warning(1015405, format1);
          data.Add("BuildConfigWarning", (object) format1);
          return false;
        }
      }
      finally
      {
        TelemetryLogger.Instance.PublishData(context.RequestContext, nameof (CodeCoverageSummaryUpdater), new CustomerIntelligenceData((IDictionary<string, object>) data));
      }
    }

    private IBuildServiceHelper BuildServiceHelper
    {
      get
      {
        if (this.m_buildServiceHelper == null)
          this.m_buildServiceHelper = (IBuildServiceHelper) new Microsoft.TeamFoundation.TestManagement.Server.BuildServiceHelper();
        return this.m_buildServiceHelper;
      }
      set => this.m_buildServiceHelper = value;
    }
  }
}
