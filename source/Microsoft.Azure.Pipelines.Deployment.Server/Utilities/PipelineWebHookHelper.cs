// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Deployment.Utilities.PipelineWebHookHelper
// Assembly: Microsoft.Azure.Pipelines.Deployment.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B08F1AB-5B33-41F6-908E-9A985FD0544C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.Pipelines.Deployment.Server.dll

using Microsoft.Azure.Pipelines.Deployment.Extensions;
using Microsoft.Azure.Pipelines.Deployment.Model;
using Microsoft.Azure.Pipelines.Deployment.Services;
using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts;
using Microsoft.TeamFoundation.DistributedTask.Pipelines;
using Microsoft.TeamFoundation.DistributedTask.Pipelines.Artifacts.WebHooks;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.Azure.Pipelines.Deployment.Utilities
{
  internal class PipelineWebHookHelper
  {
    public static bool UpdateWebHooks(
      IVssRequestContext requestContext,
      Guid projectId,
      IList<PipelineDefinitionTrigger> existingTriggers,
      IList<PipelineDefinitionTrigger> triggersFromYaml,
      PipelineResources resources,
      out string resultMessage,
      out string resultExceptionMessage)
    {
      resultMessage = string.Empty;
      resultExceptionMessage = string.Empty;
      bool flag = true;
      try
      {
        requestContext.ServiceName = "Pipeline";
        IArtifactService service = requestContext.GetService<IArtifactService>();
        foreach (IArtifactType artifactType in (IEnumerable<IArtifactType>) service.GetArtifactTypes(requestContext).ToList<IArtifactType>())
        {
          if (artifactType.IsIncomingWebHookArtifactType() && requestContext.IsFeatureEnabled("DistributedTask.BindIncomingWebHookLifetimeWithServiceConnection"))
          {
            requestContext.Trace(100161033, TraceLevel.Verbose, "Deployment", nameof (PipelineWebHookHelper), "Skipping update for incoming webhook.");
          }
          else
          {
            IList<PipelineDefinitionTrigger> matchingTriggers1 = ArtifactTriggerHelper.GetMatchingTriggers(existingTriggers, artifactType);
            IList<PipelineDefinitionTrigger> matchingTriggers2 = ArtifactTriggerHelper.GetMatchingTriggers(triggersFromYaml, artifactType);
            if (matchingTriggers1.Except<PipelineDefinitionTrigger>((IEnumerable<PipelineDefinitionTrigger>) matchingTriggers2).ToList<PipelineDefinitionTrigger>().Any<PipelineDefinitionTrigger>())
              flag = PipelineWebHookHelper.DeleteSubscription(requestContext, projectId, matchingTriggers1, matchingTriggers2, artifactType, resources, out resultMessage);
            if (flag)
            {
              List<PipelineDefinitionTrigger> list = matchingTriggers2.Except<PipelineDefinitionTrigger>((IEnumerable<PipelineDefinitionTrigger>) matchingTriggers1).ToList<PipelineDefinitionTrigger>();
              if (list.Any<PipelineDefinitionTrigger>())
                flag = PipelineWebHookHelper.CreateSubscription(requestContext, projectId, (IList<PipelineDefinitionTrigger>) list, service, resources, out resultMessage);
            }
          }
        }
      }
      catch (Exception ex)
      {
        resultExceptionMessage = ex.Message;
        return false;
      }
      return flag;
    }

    private static bool CreateSubscription(
      IVssRequestContext requestContext,
      Guid projectId,
      IList<PipelineDefinitionTrigger> triggers,
      IArtifactService artifactService,
      PipelineResources resources,
      out string resultMessage)
    {
      resultMessage = string.Empty;
      IWebHookService service1 = requestContext.GetService<IWebHookService>();
      IPipelineTriggerIssuesService service2 = requestContext.GetService<IPipelineTriggerIssuesService>();
      IList<PipelineTriggerIssues> pipelineTriggerIssuesList = (IList<PipelineTriggerIssues>) new List<PipelineTriggerIssues>();
      int pipelineDefinitionId = 0;
      foreach (PipelineDefinitionTrigger trigger in triggers.Where<PipelineDefinitionTrigger>((Func<PipelineDefinitionTrigger, bool>) (x => !x.ArtifactDefinition.ArtifactType.Equals("IncomingWebhook", StringComparison.OrdinalIgnoreCase))))
      {
        pipelineDefinitionId = trigger.PipelineDefinitionId;
        if (string.IsNullOrEmpty(trigger.ArtifactDefinition.UniqueResourceIdentifier))
          return false;
        IArtifactType artifactType = artifactService.GetArtifactType(requestContext, trigger.ArtifactDefinition.ArtifactType);
        IDictionary<string, string> dictionary = trigger.ToDictionary(requestContext, artifactType);
        if (service1.EnsureWebHookExists(requestContext, projectId, artifactType, trigger.ArtifactDefinition.UniqueResourceIdentifier, dictionary, out resultMessage) == null && requestContext.IsFeatureEnabled("DistributedTask.EnableYamlPipelineTriggerIssues"))
        {
          string errorMessage = DeploymentResources.ErrorTriggerConfiguration();
          foreach (PipelineTriggerIssues pipelineTriggerIssue in (IEnumerable<PipelineTriggerIssues>) PipelineTriggerIssuesExtension.ToPipelineTriggerIssues(requestContext, projectId, pipelineDefinitionId, resources, errorMessage, trigger.Alias, true))
            pipelineTriggerIssuesList.Add(pipelineTriggerIssue);
        }
      }
      foreach (PipelineDefinitionTrigger trigger in triggers.Where<PipelineDefinitionTrigger>((Func<PipelineDefinitionTrigger, bool>) (x => x.ArtifactDefinition.ArtifactType.Equals("IncomingWebhook", StringComparison.OrdinalIgnoreCase))))
      {
        IArtifactType artifactType = artifactService.GetArtifactType(requestContext, trigger.ArtifactDefinition.ArtifactType);
        IDictionary<string, string> dictionary = trigger.ToDictionary(requestContext, artifactType);
        service1.EnsureIncomingWebHookExists(requestContext, projectId, artifactType, trigger.ArtifactDefinition.UniqueResourceIdentifier, dictionary, out resultMessage);
      }
      if (!pipelineTriggerIssuesList.Any<PipelineTriggerIssues>())
        return true;
      service2.CreatePipelineTriggerIssues(requestContext, projectId, pipelineDefinitionId, pipelineTriggerIssuesList);
      return false;
    }

    private static bool DeleteSubscription(
      IVssRequestContext requestContext,
      Guid projectId,
      IList<PipelineDefinitionTrigger> oldTriggers,
      IList<PipelineDefinitionTrigger> newTriggers,
      IArtifactType artifactType,
      PipelineResources resources,
      out string resultMessage)
    {
      resultMessage = string.Empty;
      IList<PipelineDefinitionTrigger> deletableTriggers = PipelineWebHookHelper.GetDeletableTriggers(oldTriggers, newTriggers);
      IPipelineTriggerIssuesService service1 = requestContext.GetService<IPipelineTriggerIssuesService>();
      IList<PipelineTriggerIssues> pipelineTriggerIssuesList = (IList<PipelineTriggerIssues>) new List<PipelineTriggerIssues>();
      int pipelineDefinitionId = 0;
      if (!deletableTriggers.Any<PipelineDefinitionTrigger>())
        return true;
      IWebHookService service2 = requestContext.GetService<IWebHookService>();
      foreach (PipelineDefinitionTrigger trigger in (IEnumerable<PipelineDefinitionTrigger>) deletableTriggers)
      {
        WebHook webHook = service2.GetWebHook(requestContext, trigger.ArtifactDefinition.UniqueResourceIdentifier, true);
        if (webHook != null && !webHook.Subscriptions.Any<IWebHookSubscription>())
        {
          service2.DeleteWebHook(requestContext, projectId, artifactType, webHook, trigger.ToDictionary(), out resultMessage);
          if (resultMessage != string.Empty && requestContext.IsFeatureEnabled("DistributedTask.EnableYamlPipelineTriggerIssues"))
          {
            string errorMessage = DeploymentResources.ErrorTriggerConfiguration();
            foreach (PipelineTriggerIssues pipelineTriggerIssue in (IEnumerable<PipelineTriggerIssues>) PipelineTriggerIssuesExtension.ToPipelineTriggerIssues(requestContext, projectId, pipelineDefinitionId, resources, errorMessage, trigger.Alias, true))
              pipelineTriggerIssuesList.Add(pipelineTriggerIssue);
          }
        }
      }
      if (!pipelineTriggerIssuesList.Any<PipelineTriggerIssues>())
        return true;
      service1.CreatePipelineTriggerIssues(requestContext, projectId, pipelineDefinitionId, pipelineTriggerIssuesList);
      return false;
    }

    private static IList<PipelineDefinitionTrigger> GetDeletableTriggers(
      IList<PipelineDefinitionTrigger> oldTriggers,
      IList<PipelineDefinitionTrigger> newTriggers)
    {
      Dictionary<string, PipelineDefinitionTrigger> dictionary = new Dictionary<string, PipelineDefinitionTrigger>();
      IList<PipelineDefinitionTrigger> list = (IList<PipelineDefinitionTrigger>) oldTriggers.Except<PipelineDefinitionTrigger>((IEnumerable<PipelineDefinitionTrigger>) newTriggers).ToList<PipelineDefinitionTrigger>();
      foreach (PipelineDefinitionTrigger newTrigger in (IEnumerable<PipelineDefinitionTrigger>) newTriggers)
      {
        if (!string.IsNullOrEmpty(newTrigger?.ArtifactDefinition?.UniqueResourceIdentifier))
          dictionary[newTrigger.ArtifactDefinition.UniqueResourceIdentifier] = newTrigger;
      }
      IList<PipelineDefinitionTrigger> deletableTriggers = (IList<PipelineDefinitionTrigger>) new List<PipelineDefinitionTrigger>();
      foreach (PipelineDefinitionTrigger definitionTrigger in (IEnumerable<PipelineDefinitionTrigger>) list)
      {
        if (!string.IsNullOrEmpty(definitionTrigger?.ArtifactDefinition?.UniqueResourceIdentifier) && !dictionary.ContainsKey(definitionTrigger.ArtifactDefinition.UniqueResourceIdentifier))
          deletableTriggers.Add(definitionTrigger);
      }
      return deletableTriggers;
    }
  }
}
