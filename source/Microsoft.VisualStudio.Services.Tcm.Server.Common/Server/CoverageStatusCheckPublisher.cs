// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.CoverageStatusCheckPublisher
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  public class CoverageStatusCheckPublisher
  {
    public CoverageStatusCheckResult UpdateCoverageStatusCheckResult(
      TestManagementRequestContext tcmRequestContext,
      PipelineContext pipelineContext,
      IVersionControlProvider versionControlProvider,
      CoverageMetrics coverageMetrics,
      Dictionary<string, FolderCoverageResult> folderCoverageResults)
    {
      Dictionary<string, object> data = new Dictionary<string, object>();
      CoverageStatusCheckEvaluator statusCheckEvaluator = new CoverageStatusCheckEvaluator();
      try
      {
        data.Add("ProjectId", (object) pipelineContext.ProjectId);
        data.Add("RepositoryId", (object) pipelineContext.RepositoryId);
        data.Add("BuildId", (object) pipelineContext.Id);
        data.Add("PullRequestId", (object) pipelineContext.PullRequestId);
        data.Add("PullRequestIterationId", (object) pipelineContext.PullRequestIterationId);
        CoverageStatusCheckConfiguration coverageSettings = pipelineContext.CodeCoverageSettings;
        string source;
        this.GetStatusCheckSourceAndName(tcmRequestContext, pipelineContext, out source, out string _);
        CoverageStatusCheckResult coverageStatusCheckResult = statusCheckEvaluator.Evaluate(tcmRequestContext, source, coverageMetrics, coverageSettings);
        if (coverageMetrics != null && !coverageMetrics.AggregatedDiffCoverage.Coverage.HasValue && coverageMetrics.AggregatedDiffCoverage.CoverageDataNotFound)
          data.Add("CoverageStatusCheckResultWhenTestsNotPresent", (object) coverageStatusCheckResult);
        if (pipelineContext.CodeCoverageSettings.IsFolderLevelPolicyEnabled)
        {
          if (!folderCoverageResults.IsNullOrEmpty<KeyValuePair<string, FolderCoverageResult>>())
          {
            try
            {
              coverageStatusCheckResult = statusCheckEvaluator.Evaluate(tcmRequestContext, coverageStatusCheckResult, folderCoverageResults);
            }
            catch (Exception ex)
            {
              tcmRequestContext.Logger.Error(1015999, string.Format("Failed to evaluate folder level policy : {0}", (object) ex));
            }
          }
        }
        return this.PublishCoverageStatusCheckState(tcmRequestContext, pipelineContext, versionControlProvider, coverageStatusCheckResult.State, coverageStatusCheckResult);
      }
      catch (Exception ex)
      {
        data.Add("Error", (object) 1015683);
        tcmRequestContext.Logger.Error(1015683, string.Format("Error while updating the status check results: {0}", (object) ex));
        throw;
      }
      finally
      {
        if (statusCheckEvaluator.CoverageStatusCheckResults != null)
        {
          foreach (CoverageStatusCheckResult statusCheckResult in statusCheckEvaluator.CoverageStatusCheckResults)
            data.Add(statusCheckResult.Name, (object) statusCheckResult.Properties);
        }
        CustomerIntelligenceData cid = new CustomerIntelligenceData((IDictionary<string, object>) data);
        TelemetryLogger.Instance.PublishData(tcmRequestContext.RequestContext, nameof (CoverageStatusCheckPublisher), cid);
      }
    }

    public CoverageStatusCheckResult PublishCoverageStatusCheckState(
      TestManagementRequestContext tcmRequestContext,
      PipelineContext pipelineContext,
      IVersionControlProvider versionControlProvider,
      CoverageStatusCheckState coverageStatusCheckState,
      CoverageStatusCheckResult coverageStatusCheckResult = null)
    {
      Dictionary<string, object> data = new Dictionary<string, object>();
      CoverageConfiguration coverageConfiguration = new CoverageConfiguration();
      try
      {
        data.Add("ProjectId", (object) pipelineContext.ProjectId);
        data.Add("RepositoryId", (object) pipelineContext.RepositoryId);
        data.Add("BuildId", (object) pipelineContext.Id);
        data.Add("PullRequestId", (object) pipelineContext.PullRequestId);
        data.Add("PullRequestIterationId", (object) pipelineContext.PullRequestIterationId);
        coverageStatusCheckResult = this.GetUpdatedCoverageStatusCheckResult(tcmRequestContext, pipelineContext, coverageStatusCheckState, coverageStatusCheckResult);
        data.Add("CoverageStatusCheckState", (object) coverageStatusCheckResult.State);
        data.Add("Description", (object) coverageStatusCheckResult.Description);
        versionControlProvider.CreatePullRequestStatus(tcmRequestContext, pipelineContext, coverageStatusCheckResult, CancellationToken.None);
        return coverageStatusCheckResult;
      }
      catch (Exception ex)
      {
        data.Add("Error", (object) 1015695);
        tcmRequestContext.Logger.Error(1015695, string.Format("Error while updating the status check results: {0}", (object) ex));
        throw;
      }
      finally
      {
        CustomerIntelligenceData cid = new CustomerIntelligenceData((IDictionary<string, object>) data);
        TelemetryLogger.Instance.PublishData(tcmRequestContext.RequestContext, nameof (CoverageStatusCheckPublisher), cid);
      }
    }

    private CoverageStatusCheckResult GetUpdatedCoverageStatusCheckResult(
      TestManagementRequestContext tcmRequestContext,
      PipelineContext pipelineContext,
      CoverageStatusCheckState coverageStatusCheckState,
      CoverageStatusCheckResult coverageStatusCheckResult = null)
    {
      string source;
      string name;
      this.GetStatusCheckSourceAndName(tcmRequestContext, pipelineContext, out source, out name);
      CoverageStatusCheckState coverageStatusCheckState1 = coverageStatusCheckResult != null ? coverageStatusCheckResult.State : coverageStatusCheckState;
      string description = coverageStatusCheckResult?.Description;
      if (coverageStatusCheckResult == null)
        coverageStatusCheckResult = CoverageStatusCheckResultHelper.GetCoverageStatusCheckResult(source, name, coverageStatusCheckState1, description);
      return coverageStatusCheckResult;
    }

    private void GetStatusCheckSourceAndName(
      TestManagementRequestContext tcmRequestContext,
      PipelineContext pipelineContext,
      out string source,
      out string name)
    {
      CoverageConfiguration coverageConfiguration = new CoverageConfiguration();
      source = pipelineContext.DefinitionName;
      if (string.IsNullOrWhiteSpace(source))
        source = coverageConfiguration.GetCoverageStatusCheckSource(tcmRequestContext.RequestContext);
      name = coverageConfiguration.GetCoverageStatusCheckName(tcmRequestContext.RequestContext);
    }
  }
}
