// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.CoverageBuildEventListener
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.Build.WebApi.Events;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  public class CoverageBuildEventListener : IBuildEventListener
  {
    private ICoverageStorage tcmCoverageStorage;

    public CoverageBuildEventListener() => this.tcmCoverageStorage = (ICoverageStorage) new TcmAttachmentCoverageStorage();

    public CoverageBuildEventListener(ICoverageStorage tcmStorage) => this.tcmCoverageStorage = tcmStorage;

    public void HandleBuildsDeletedEvent(
      IVssRequestContext requestContext,
      BuildsDeletedEvent1 buildsDeletedEvent)
    {
      TestManagementRequestContext requestContext1 = new TestManagementRequestContext(requestContext);
      Dictionary<string, object> dictionary = new Dictionary<string, object>();
      try
      {
        dictionary.Add("ProjectId", (object) buildsDeletedEvent.ProjectId);
        dictionary.Add("DefinitionId", (object) buildsDeletedEvent.DefinitionId);
        dictionary.Add("BuildIds", (object) buildsDeletedEvent.BuildIds);
        dictionary.Add("Event", (object) "BuildDeletion");
        using (new SimpleTimer(requestContext1.RequestContext, "HandleBuildsDeletedEvent:DeleteContainer", dictionary))
          this.tcmCoverageStorage.DeleteContainer(requestContext1, buildsDeletedEvent.ProjectId, (IEnumerable<int>) buildsDeletedEvent.BuildIds, "ModuleCoverage", dictionary);
      }
      catch (Exception ex)
      {
        string str = string.Format("CoverageBuildEventListener: HandleBuildsDeletedEvent failed with {0}", (object) ex);
        requestContext1.Logger.Error(1015128, str);
        dictionary.Add("ErrorInHandleBuildsDeletedEvent", (object) ex);
        EventHandler<string> errorEventHandler = this.LogErrorEventHandler;
        if (errorEventHandler == null)
          return;
        errorEventHandler((object) this, str);
      }
      finally
      {
        CustomerIntelligenceData cid = new CustomerIntelligenceData((IDictionary<string, object>) dictionary);
        TelemetryLogger.Instance.PublishData(requestContext, "ModuleCoverageBuildEventListener", cid);
      }
    }

    public void HandleBuildCompletionEvent(
      IVssRequestContext requestContext,
      BuildCompletedEvent buildCompletedEvent)
    {
      TestManagementRequestContext tcmRequestContext = new TestManagementRequestContext(requestContext);
      Dictionary<string, object> dictionary = new Dictionary<string, object>();
      try
      {
        dictionary.Add("Event", (object) "BuildCompletion");
        PipelineContext pipelineContext = CommonHelper.CreatePipelineContext(tcmRequestContext, buildCompletedEvent.Build);
        CommonHelper.PopulateCiDataWithPipelineContext(pipelineContext, dictionary);
        string str = string.Format("The build {0} was canceled. Bailing out.", (object) buildCompletedEvent.Build.Id);
        BuildResult? result = buildCompletedEvent.Build.Result;
        BuildResult buildResult = BuildResult.Canceled;
        if (result.GetValueOrDefault() == buildResult & result.HasValue)
        {
          tcmRequestContext.Logger.Verbose(1015132, str);
          EventHandler<string> errorEventHandler = this.LogErrorEventHandler;
          if (errorEventHandler != null)
            errorEventHandler((object) this, str);
          if (!pipelineContext.IsPullRequestScenario)
            return;
          this.QueueCoverageStatusPublishJob(tcmRequestContext, buildCompletedEvent.Build, CoverageStatusCheckState.Failed, dictionary);
        }
        else
        {
          tcmRequestContext.Logger.Verbose(1015136, string.Format("The build result is set to '{0}' for build {1}. Marking the coverage status check as in progress and processing the coverage data.", (object) buildCompletedEvent.Build.Result, (object) buildCompletedEvent.Build.Id));
          CoverageJobHelper.QueueOneTimeJob<MergeInvokerJobData>(tcmRequestContext, new MergeInvokerJobData()
          {
            QueueTime = DateTime.UtcNow,
            PipelineContext = pipelineContext,
            JobInvoker = 1
          });
        }
      }
      catch (Exception ex)
      {
        string str = string.Format("CoverageBuildEventListener: HandleBuildCompletionEvent failed with {0}", (object) ex);
        dictionary.Add("Error", (object) str);
        tcmRequestContext.Logger.Error(1015134, str);
        EventHandler<string> errorEventHandler = this.LogErrorEventHandler;
        if (errorEventHandler == null)
          return;
        errorEventHandler((object) this, str);
      }
      finally
      {
        CustomerIntelligenceData cid = new CustomerIntelligenceData((IDictionary<string, object>) dictionary);
        TelemetryLogger.Instance.PublishData(requestContext, "ModuleCoverageBuildEventListener", cid);
      }
    }

    public void HandleBuildQueuedEvent(
      IVssRequestContext requestContext,
      BuildQueuedEvent buildQueuedEvent)
    {
      DateTime utcNow = DateTime.UtcNow;
      TestManagementRequestContext tcmRequestContext = new TestManagementRequestContext(requestContext);
      Dictionary<string, object> dictionary = new Dictionary<string, object>();
      try
      {
        PipelineContext pipelineContext = CommonHelper.CreatePipelineContext(tcmRequestContext, buildQueuedEvent.Build);
        CommonHelper.PopulateCiDataWithPipelineContext(pipelineContext, dictionary);
        dictionary.Add("Event", (object) "BuildQueued");
        tcmRequestContext.Logger.Verbose(1015137, string.Format("The build result is set to '{0}' for build {1}. Marking the coverage status check as Queued", (object) buildQueuedEvent.Build.Result, (object) buildQueuedEvent.Build.Id));
        if (pipelineContext.IsPullRequestScenario)
          this.QueueCoverageStatusPublishJob(tcmRequestContext, buildQueuedEvent.Build, CoverageStatusCheckState.Queued, dictionary);
        else
          dictionary.Add("isPolicyCheckSkipped", (object) true);
      }
      catch (Exception ex)
      {
        string str = string.Format("CoverageBuildEventListener: HandleBuildQueuedEvent failed with {0}", (object) ex);
        dictionary.Add("Error", (object) 1015135);
        tcmRequestContext.Logger.Error(1015135, str);
        EventHandler<string> errorEventHandler = this.LogErrorEventHandler;
        if (errorEventHandler == null)
          return;
        errorEventHandler((object) this, str);
      }
      finally
      {
        CustomerIntelligenceData cid = new CustomerIntelligenceData((IDictionary<string, object>) dictionary);
        TelemetryLogger.Instance.PublishData(requestContext, "ModuleCoverageBuildEventListener", cid);
      }
    }

    private void QueueCoverageStatusPublishJob(
      TestManagementRequestContext tcmRequestContext,
      Microsoft.TeamFoundation.Build.WebApi.Build build,
      CoverageStatusCheckState coverageStatusCheckState,
      Dictionary<string, object> ciData)
    {
      ciData.Add("CoverageStatusCheckState", (object) coverageStatusCheckState);
      PipelineContext pipelineContext = CommonHelper.CreatePipelineContext(tcmRequestContext, build);
      PublishCoveragePRStatusJobData jobData = new PublishCoveragePRStatusJobData()
      {
        PipelineContext = pipelineContext,
        CoverageStatusCheckState = coverageStatusCheckState
      };
      Guid guid = CoverageJobHelper.QueueOneTimeJob<PublishCoveragePRStatusJobData>(tcmRequestContext, jobData);
      ciData.Add("StatusPublishJob", (object) guid);
    }

    public event EventHandler<string> LogErrorEventHandler;
  }
}
