// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Pipelines.Server.GitHubAppPushHandlerJobExtension
// Assembly: Microsoft.TeamFoundation.Pipelines.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07451E6B-67F8-4956-AC64-CC041BD809B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Pipelines.Server.dll

using Microsoft.TeamFoundation.Build2.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Pipelines.Server.Providers;
using Microsoft.VisualStudio.Services.ExternalEvent;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Pipelines.Server
{
  internal class GitHubAppPushHandlerJobExtension : ITeamFoundationJobExtension
  {
    private const string c_layer = "GitHubAppPushHandlerJobExtension";

    public TeamFoundationJobExecutionResult Run(
      IVssRequestContext requestContext,
      TeamFoundationJobDefinition jobDefinition,
      DateTime queueTime,
      out string resultMessage)
    {
      GitHubAppPushHandlerJobData pushHandlerJobData = GitHubAppPushHandlerJobData.Deserialize(jobDefinition.Data);
      try
      {
        IPipelineSourceProvider provider = requestContext.GetService<IPipelineSourceProviderService>().GetProvider(pushHandlerJobData.PipelineProviderId);
        List<BuildDefinition> definitionsByIds = requestContext.GetService<BuildDefinitionService>().GetDefinitionsByIds(requestContext, pushHandlerJobData.ProjectId, (IEnumerable<int>) pushHandlerJobData.DefinitionIds, false, new DateTime?(), false, false, (ExcludePopulatingDefinitionResources) null);
        List<BuildData> filesAndQueueBuilds = BuildEventHelper.ParseCITriggersFromYamlFilesAndQueueBuilds(requestContext, definitionsByIds, provider, pushHandlerJobData.Push);
        resultMessage = string.Format("PipelineEvent {0} was successfully processed. Queued {1} builds.", (object) pushHandlerJobData.Push.PipelineEventId, (object) filesAndQueueBuilds.Count);
        return TeamFoundationJobExecutionResult.Succeeded;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(12030350, nameof (GitHubAppPushHandlerJobExtension), ex);
        PipelineEventLogger.LogException(requestContext, (IExternalGitEvent) pushHandlerJobData.Push, ex);
        resultMessage = string.Format("{0} thrown. PipelineEventId={1}", (object) ex.GetType(), (object) pushHandlerJobData?.Push?.PipelineEventId);
        return TeamFoundationJobExecutionResult.Failed;
      }
    }
  }
}
