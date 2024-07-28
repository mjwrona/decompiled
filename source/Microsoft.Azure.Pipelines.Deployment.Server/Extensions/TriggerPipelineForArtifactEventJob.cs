// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Deployment.Extensions.TriggerPipelineForArtifactEventJob
// Assembly: Microsoft.Azure.Pipelines.Deployment.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B08F1AB-5B33-41F6-908E-9A985FD0544C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.Pipelines.Deployment.Server.dll

using Microsoft.Azure.Pipelines.Deployment.Model;
using Microsoft.Azure.Pipelines.Deployment.Sdk.Server.Artifacts;
using Microsoft.Azure.Pipelines.Deployment.Services;
using Microsoft.Azure.Pipelines.Deployment.Utilities;
using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts;
using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.Constants;
using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.Models;
using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.WebHooks;
using Microsoft.TeamFoundation.DistributedTask.Pipelines;
using Microsoft.TeamFoundation.DistributedTask.Pipelines.Artifacts.WebHooks;
using Microsoft.TeamFoundation.DistributedTask.Pipelines.BuildProviders;
using Microsoft.TeamFoundation.Framework.Server;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

namespace Microsoft.Azure.Pipelines.Deployment.Extensions
{
  public class TriggerPipelineForArtifactEventJob : ITeamFoundationJobExtension
  {
    private const string c_layer = "TriggerPipelineForArtifactEventJob";
    private const string c_pipelineTriggerType = "pipelineTriggerType";
    private const string c_eventVariables = "Run.Variables";

    public TeamFoundationJobExecutionResult Run(
      IVssRequestContext requestContext,
      TeamFoundationJobDefinition jobDefinition,
      DateTime queueTime,
      out string resultMessage)
    {
      resultMessage = string.Empty;
      if (jobDefinition.Data == null)
      {
        resultMessage = string.Format("[{0}] Job data is missing", (object) 17000);
        return TeamFoundationJobExecutionResult.Failed;
      }
      using (new MethodScope(requestContext, nameof (TriggerPipelineForArtifactEventJob), nameof (Run)))
      {
        PipelineWebHookEventData eventData;
        PipelineStatusChangedEvent statusChangedEvent;
        try
        {
          eventData = PipelineWebHookNotificationUtilities.DeserializeFromWebHookEventDataXmlNode(jobDefinition.Data);
          statusChangedEvent = JsonConvert.DeserializeObject<PipelineStatusChangedEvent>(eventData.EventData);
        }
        catch (Exception ex)
        {
          resultMessage = string.Format("[{0}] {1}", (object) 18000, (object) ex.Message);
          return TeamFoundationJobExecutionResult.Failed;
        }
        try
        {
          if (statusChangedEvent == null)
          {
            resultMessage = string.Format("[{0}] No pipeline event found. Job ID: {1}", (object) 19000, (object) 0);
            return TeamFoundationJobExecutionResult.Failed;
          }
          requestContext.ServiceName = "Pipeline";
          WebHook webHook = requestContext.GetService<IWebHookService>().GetWebHook(requestContext, eventData.WebHookId);
          if (webHook == null)
          {
            resultMessage = string.Format("[{0}] Webhook not found. {1}", (object) 20000, (object) eventData.WebHookId);
            return TeamFoundationJobExecutionResult.Failed;
          }
          IArtifactType artifactType = requestContext.GetService<IArtifactService>().GetArtifactType(requestContext, webHook.ArtifactType);
          if (artifactType == null)
          {
            resultMessage = string.Format("[{0}] Artifact with type {1} not found", (object) 21000, (object) webHook.ArtifactType);
            return TeamFoundationJobExecutionResult.Failed;
          }
          IPipelineTfsBuildService service = requestContext.GetService<IPipelineTfsBuildService>();
          if (service == null)
          {
            resultMessage = string.Format("[{0}] No build service found. Request ID: {1}", (object) 22000, (object) requestContext.UniqueIdentifier);
            return TeamFoundationJobExecutionResult.Failed;
          }
          PipelineInfo pipelineInfo1 = statusChangedEvent.Run == null ? (PipelineInfo) null : service.GetPipelineInfo(requestContext, statusChangedEvent.ProjectId, statusChangedEvent.Run.Id);
          IList<PipelineDefinitionTrigger> pipelineTriggers = requestContext.GetService<IPipelineTriggerService>().GetPipelineTriggers(requestContext, webHook.UniqueArtifactIdentifier);
          if ((pipelineTriggers != null ? (!pipelineTriggers.Any<PipelineDefinitionTrigger>() ? 1 : 0) : 1) != 0)
          {
            resultMessage = "No definitions found. WebHookId: " + webHook.UniqueArtifactIdentifier;
            return TeamFoundationJobExecutionResult.Succeeded;
          }
          requestContext.Trace(100161024, TraceLevel.Info, "Deployment", nameof (TriggerPipelineForArtifactEventJob), "Definition triggers: {0}\nWebHookId: {1}\nEventMessage: {2},", (object) string.Join(",", pipelineTriggers.Select<PipelineDefinitionTrigger, string>((Func<PipelineDefinitionTrigger, string>) (trigger => string.Format("{0}({1})", (object) trigger.Alias, (object) trigger.PipelineDefinitionId)))), (object) eventData.WebHookId, (object) eventData.EventData);
          WebHookEventPayloadInputMapper inputMapper = new WebHookEventPayloadInputMapper(eventData.EventData, artifactType);
          IDictionary<Tuple<Guid, int>, bool> dictionary = (IDictionary<Tuple<Guid, int>, bool>) new Dictionary<Tuple<Guid, int>, bool>();
          foreach (PipelineDefinitionTrigger definitionTrigger in (IEnumerable<PipelineDefinitionTrigger>) pipelineTriggers)
          {
            Tuple<Guid, int> key = new Tuple<Guid, int>(definitionTrigger.Project.Id, definitionTrigger.PipelineDefinitionId);
            if (!dictionary.ContainsKey(key))
            {
              if (!ArtifactTriggerHelper.IsTriggerFilterMatches(definitionTrigger, inputMapper))
              {
                string str1;
                try
                {
                  inputMapper.GetValue("StageName", out str1);
                }
                catch (NullReferenceException ex)
                {
                  requestContext.TraceException(100161026, TraceLevel.Error, "Deployment", "ArtifactTrigger", (Exception) ex, string.Format("Null Reference exception was throwed by {0} on {1}. Details: {2}", (object) ex.TargetSite, (object) ex.Source, (object) ex.Message));
                  continue;
                }
                Guid projectId = statusChangedEvent.ProjectId;
                int? id = statusChangedEvent.Run?.Id;
                string str2;
                string str3 = inputMapper.GetValue("Version", out str2) ? string.Format("{0} {1} {2}", (object) str2, (object) projectId, (object) id) : "";
                requestContext.Trace(100161025, TraceLevel.Info, "Deployment", "ArtifactTrigger", string.Format("Cannot meet filter criteria for pipeline triggers. Definition: {0}, Build number: {1}", (object) definitionTrigger.PipelineDefinitionId, (object) str3));
                if (string.IsNullOrEmpty(str1) && requestContext.IsFeatureEnabled("DistributedTask.EnableYamlPipelineTriggerIssues"))
                {
                  PipelineTriggerIssues pipelineTriggerIssues = new PipelineTriggerIssues()
                  {
                    PipelineDefinitionId = definitionTrigger.PipelineDefinitionId,
                    Alias = definitionTrigger.Alias,
                    BuildNumber = str3,
                    ErrorMessage = DeploymentResources.CannotMeetFilterCriteriaForPipelineTriggers(),
                    isError = false
                  };
                  requestContext.GetService<IPipelineTriggerIssuesService>().CreatePipelineTriggerIssues(requestContext, definitionTrigger.Project.Id, definitionTrigger.PipelineDefinitionId, (IList<PipelineTriggerIssues>) new PipelineTriggerIssues[1]
                  {
                    pipelineTriggerIssues
                  });
                }
              }
              else if (TriggerPipelineForArtifactEventJob.HasSamePipelineRunUsedBySamePipelineDefinitionBefore(requestContext, definitionTrigger.Project.Id, definitionTrigger.PipelineDefinitionId, inputMapper))
              {
                string str4;
                inputMapper.GetValue("TriggeredByPipeline.ProjectId", out str4);
                string str5;
                inputMapper.GetValue("pipelineId", out str5);
                requestContext.Trace(100161025, TraceLevel.Info, "Deployment", "ArtifactTrigger", "Skip triggering new pipeline run for event as artifact with pipelineRunId {0} from project {1} is already used by pipeline definition {2} of project {3}", (object) str5, (object) str4, (object) definitionTrigger.PipelineDefinitionId, (object) definitionTrigger.Project.Id);
              }
              else
              {
                Microsoft.TeamFoundation.Build.WebApi.Build build = new Microsoft.TeamFoundation.Build.WebApi.Build()
                {
                  Reason = BuildReason.ResourceTrigger,
                  Definition = new DefinitionReference()
                  {
                    Id = definitionTrigger.PipelineDefinitionId
                  },
                  Project = new TeamProjectReference()
                  {
                    Id = definitionTrigger.Project.Id
                  },
                  TriggerInfo = {
                    {
                      "artifactType",
                      artifactType.Name
                    },
                    {
                      "alias",
                      definitionTrigger.Alias
                    }
                  },
                  Repository = new BuildRepository(),
                  RequestedFor = pipelineInfo1?.RequestedFor
                };
                build.Parameters = this.GetInternalVariables(eventData).Serialize<IDictionary<string, string>>();
                if (definitionTrigger.ArtifactDefinition.Project != null && definitionTrigger.ArtifactDefinition.Project.Id != Guid.Empty)
                  build.TriggerInfo[PipelinePropertyNames.ProjectId] = definitionTrigger.ArtifactDefinition.Project.Id.ToString();
                build.TriggerInfo["pipelineTriggerType"] = definitionTrigger.TriggerType.ToString();
                try
                {
                  artifactType.FillQueuePipelineDataParameters(requestContext, build, inputMapper);
                }
                catch (Exception ex)
                {
                  requestContext.TraceException(100161026, TraceLevel.Error, "Deployment", "ArtifactTrigger", ex, string.Format("Not able to fill queue pipeline data parameters. Project: {0} DefinitionId: {1}", (object) build.Project.Id, (object) build.Definition.Id));
                  continue;
                }
                try
                {
                  PipelineInfo pipelineInfo2 = service.QueuePipeline(requestContext, definitionTrigger.Project, build);
                  requestContext.Trace(100161024, TraceLevel.Info, "Deployment", "ArtifactTrigger", "Triggered pipeline {0} for artifact event. Definition {1}, Project {2}, Artifact Type: {3}", (object) pipelineInfo2.Id, (object) definitionTrigger.PipelineDefinitionId, (object) definitionTrigger.Project.Id, (object) artifactType.Name);
                }
                catch (InvalidGitVersionSpec ex) when (definitionTrigger.TriggerType == PipelineTriggerType.PipelineCompletion && pipelineInfo1 != null && pipelineInfo1.Reason == BuildReason.PullRequest)
                {
                }
                catch (Exception ex)
                {
                  string str6;
                  string str7 = inputMapper.GetValue("Version", out str6) ? str6 : "";
                  requestContext.TraceException(100161026, ex is DefinitionDisabledException ? TraceLevel.Info : TraceLevel.Error, "Deployment", "ArtifactTrigger", ex, string.Format("Cannot trigger pipeline for trigger event. Definition:{0}, Build:{1}", (object) definitionTrigger.PipelineDefinitionId, (object) str7));
                  if (requestContext.IsFeatureEnabled("DistributedTask.EnableYamlPipelineTriggerIssues"))
                  {
                    PipelineTriggerIssues pipelineTriggerIssues = new PipelineTriggerIssues()
                    {
                      PipelineDefinitionId = definitionTrigger.PipelineDefinitionId,
                      Alias = definitionTrigger.Alias,
                      BuildNumber = str7,
                      ErrorMessage = string.Format((IFormatProvider) CultureInfo.InvariantCulture, DeploymentResources.CannotTriggerPipelineForTriggerEvent((object) ex.Message)),
                      isError = !(ex is DefinitionDisabledException)
                    };
                    requestContext.GetService<IPipelineTriggerIssuesService>().CreatePipelineTriggerIssues(requestContext, definitionTrigger.Project.Id, definitionTrigger.PipelineDefinitionId, (IList<PipelineTriggerIssues>) new PipelineTriggerIssues[1]
                    {
                      pipelineTriggerIssues
                    });
                  }
                }
                finally
                {
                  dictionary[key] = true;
                }
              }
            }
          }
        }
        catch (Exception ex)
        {
          requestContext.TraceException(100161026, "Deployment", "ArtifactTrigger", ex);
          resultMessage = string.Format("[{0}] {1}", (object) 23000, (object) ex.Message);
          return TeamFoundationJobExecutionResult.Failed;
        }
      }
      return TeamFoundationJobExecutionResult.Succeeded;
    }

    private static bool HasSamePipelineRunUsedBySamePipelineDefinitionBefore(
      IVssRequestContext requestContext,
      Guid pipelineDefinitionProjectId,
      int pipelineDefinitionId,
      WebHookEventPayloadInputMapper inputMapper)
    {
      string input;
      Guid result1;
      string s1;
      int result2;
      string s2;
      int result3;
      return !requestContext.IsFeatureEnabled("DistributedTask.DisableCheckPipelineRunsUsingExistingPipelineRun") && requestContext.IsFeatureEnabled("DistributedTask.EnableTagBasedFilteringForPipelineTriggers") && inputMapper.GetValue("TriggeredByPipeline.ProjectId", out input) && Guid.TryParse(input, out result1) && inputMapper.GetValue("pipelineId", out s1) && int.TryParse(s1, out result2) && inputMapper.GetValue("PipelineDefinitionId", out s2) && int.TryParse(s2, out result3) && requestContext.GetService<IArtifactTraceabilityService>()?.GetArtifactTraceabilityService(ArtifactTraceabilityConstants.ArtifactTraceabilityDeploymentService).GetPipelineRunsUsingExistingPipelineRun(requestContext, pipelineDefinitionProjectId, pipelineDefinitionId, result1, result3, result2).Count<CDPipelineRunData>() > 0;
    }

    private IDictionary<string, string> GetInternalVariables(PipelineWebHookEventData eventData)
    {
      Dictionary<string, JToken> source = JObject.Parse(eventData.EventData).SelectToken("Run.Variables")?.ToObject<Dictionary<string, JToken>>();
      return source == null ? (IDictionary<string, string>) new Dictionary<string, string>() : (IDictionary<string, string>) source.ToDictionary<KeyValuePair<string, JToken>, string, string>((Func<KeyValuePair<string, JToken>, string>) (pair => pair.Key), (Func<KeyValuePair<string, JToken>, string>) (pair => (string) pair.Value.SelectToken("Value")));
    }
  }
}
