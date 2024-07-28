// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Deployment.Extensions.PipelineWebHookNotificationJob
// Assembly: Microsoft.Azure.Pipelines.Deployment.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B08F1AB-5B33-41F6-908E-9A985FD0544C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.Pipelines.Deployment.Server.dll

using Microsoft.Azure.Pipelines.Deployment.Model;
using Microsoft.Azure.Pipelines.Deployment.Sdk.Server.Artifacts;
using Microsoft.Azure.Pipelines.Deployment.Services;
using Microsoft.Azure.Pipelines.Deployment.Utilities;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Notifications;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Xml;

namespace Microsoft.Azure.Pipelines.Deployment.Extensions
{
  public class PipelineWebHookNotificationJob : ITeamFoundationJobExtension
  {
    private const string c_layer = "PipelineWebHookNotificationJob";

    public TeamFoundationJobExecutionResult Run(
      IVssRequestContext requestContext,
      TeamFoundationJobDefinition jobDefinition,
      DateTime queueTime,
      out string resultMessage)
    {
      resultMessage = string.Empty;
      using (new MethodScope(requestContext, nameof (PipelineWebHookNotificationJob), nameof (Run)))
      {
        if (!requestContext.IsFeatureEnabled("DistributedTask.EnablePipelineTriggers"))
          return TeamFoundationJobExecutionResult.Succeeded;
        VssNotificationEvent notificationEvent = PipelineWebHookNotificationUtilities.DeserializeFromJsonXmlNode(jobDefinition.Data);
        EventScope eventScope = notificationEvent.Scopes.FirstOrDefault<EventScope>((Func<EventScope, bool>) (x => x.Type.Equals(VssNotificationEvent.ScopeNames.Project)));
        Guid projectId = eventScope != null ? eventScope.Id : Guid.Empty;
        PipelineStatusChangedEvent statusChangedEvent = JsonConvert.DeserializeObject<PipelineStatusChangedEvent>(notificationEvent.Data.ToString());
        if (statusChangedEvent == null)
          return TeamFoundationJobExecutionResult.Succeeded;
        IPipelineWebHookPublisherService service = requestContext.GetService<IPipelineWebHookPublisherService>();
        PipelineWebHookPublisher webHookPublisher1 = this.GetPipelineWebHookPublisher(projectId, statusChangedEvent?.Pipeline?.Id ?? -1);
        IVssRequestContext requestContext1 = requestContext;
        PipelineWebHookPublisher publisher = webHookPublisher1;
        PipelineWebHookPublisher webHookPublisher2 = service.GetWebHookPublisher(requestContext1, publisher);
        if (webHookPublisher2 == null || !string.IsNullOrEmpty(webHookPublisher2.PayloadUrl))
          return TeamFoundationJobExecutionResult.Succeeded;
        PipelineWebHookEventData webHookEventData = new PipelineWebHookEventData()
        {
          WebHookId = webHookPublisher2.WebHookId,
          EventData = notificationEvent.Data.ToString()
        };
        this.QueueJobToTriggerPipeline(requestContext, webHookEventData);
        return TeamFoundationJobExecutionResult.Succeeded;
      }
    }

    private void QueueJobToTriggerPipeline(
      IVssRequestContext requestContext,
      PipelineWebHookEventData webHookEventData)
    {
      XmlNode jsonXmlNode = webHookEventData.SerializeToJsonXmlNode();
      requestContext.GetService<TeamFoundationJobService>().QueueOneTimeJob(requestContext, typeof (TriggerPipelineForArtifactEventJob).Namespace, typeof (TriggerPipelineForArtifactEventJob).FullName, jsonXmlNode, JobPriorityLevel.Normal);
    }

    private PipelineWebHookPublisher GetPipelineWebHookPublisher(
      Guid projectId,
      int pipelineDefinitionId)
    {
      return new PipelineWebHookPublisher()
      {
        Project = new ProjectInfo() { Id = projectId },
        PipelineDefinitionId = pipelineDefinitionId
      };
    }
  }
}
