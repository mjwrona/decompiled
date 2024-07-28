// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Pipelines.Server.RecalculateResourceTriggersJob
// Assembly: Microsoft.TeamFoundation.Pipelines.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07451E6B-67F8-4956-AC64-CC041BD809B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Pipelines.Server.dll

using Microsoft.Azure.Pipelines.Deployment.Sdk.Server.Artifacts;
using Microsoft.TeamFoundation.Build2.Server;
using Microsoft.TeamFoundation.DistributedTask.Pipelines;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.TeamFoundation.Pipelines.Server
{
  public class RecalculateResourceTriggersJob : ITeamFoundationJobExtension
  {
    private const string TraceLayer = "RecalculateResourceTriggersJob";

    public TeamFoundationJobExecutionResult Run(
      IVssRequestContext requestContext,
      TeamFoundationJobDefinition jobDefinition,
      DateTime queueTime,
      out string resultMessage)
    {
      resultMessage = string.Empty;
      using (requestContext.TraceScope(nameof (RecalculateResourceTriggersJob), nameof (Run)))
      {
        try
        {
          if (!requestContext.IsFeatureEnabled("DistributedTask.EnablePipelineTriggers"))
          {
            requestContext.TraceInfo("PipelineTriggerMaterialization", "Pipeline trigger is not enabled.");
            return TeamFoundationJobExecutionResult.Succeeded;
          }
          requestContext.RequestTimeout = TimeSpan.FromMinutes(10.0);
          RecalculateResourceTriggersJobData resourceTriggersJobData = RecalculateResourceTriggersJobDataUtilities.DeserializeFromJsonXmlNode(jobDefinition.Data);
          Guid projectId = resourceTriggersJobData.ProjectId;
          List<int> definitionIds = resourceTriggersJobData.DefinitionIds;
          List<BuildDefinition> definitions = requestContext.GetService<BuildDefinitionService>().GetDefinitionsByIds(requestContext, projectId, (IEnumerable<int>) definitionIds, false, new DateTime?(), false, false, (ExcludePopulatingDefinitionResources) null).Where<BuildDefinition>((Func<BuildDefinition, bool>) (x =>
          {
            DefinitionQuality? definitionQuality1 = x.DefinitionQuality;
            DefinitionQuality definitionQuality2 = DefinitionQuality.Definition;
            return definitionQuality1.GetValueOrDefault() == definitionQuality2 & definitionQuality1.HasValue;
          })).Where<BuildDefinition>((Func<BuildDefinition, bool>) (x => x.QueueStatus != DefinitionQueueStatus.Disabled)).Where<BuildDefinition>((Func<BuildDefinition, bool>) (x => x.GetProcess<YamlProcess>().Type == 2)).ToList<BuildDefinition>();
          requestContext.TraceAlways(12030361, TraceLevel.Info, "Build2", nameof (RecalculateResourceTriggersJob), "Starting resource triggers recalculation for project: {0} and definitions: {1}. Definitions: {2} were excluded.", (object) projectId.ToString(), (object) string.Join<int>(",", (IEnumerable<int>) definitionIds), (object) string.Join<int>(",", definitionIds.Where<int>((Func<int, bool>) (x => !definitions.Any<BuildDefinition>((Func<BuildDefinition, bool>) (d => d.Id == x))))));
          bool flag = false;
          foreach (BuildDefinition definition in definitions)
          {
            string str = string.Format("Recalculating triggers for definition: {0} started.\n", (object) definition.Id);
            IArtifactYamlTriggerService service = requestContext.GetService<IArtifactYamlTriggerService>();
            YamlProcess process = definition.GetProcess<YamlProcess>();
            string format;
            try
            {
              PipelineResources authorizedResources = requestContext.GetService<IBuildResourceAuthorizationService>().GetAuthorizedResources(requestContext, definition.ProjectId, definition.Id).ToPipelineResources() ?? new PipelineResources();
              string latestSourceVersion = requestContext.GetService<IBuildSourceProviderService>().GetSourceProvider(requestContext, definition.Repository.Type).GetLatestSourceVersion(requestContext, definition, definition.Repository.DefaultBranch);
              RepositoryResource repositoryResource = definition.Repository.ToRepositoryResource(requestContext, PipelineConstants.SelfAlias, definition.Repository.DefaultBranch, latestSourceVersion);
              if (repositoryResource.Endpoint != null && !process.SupportsYamlRepositoryEndpointAuthorization())
                authorizedResources.Endpoints.Add(repositoryResource.Endpoint);
              PipelineBuilder pipelineBuilder = definition.GetPipelineBuilder(requestContext, authorizedResources, true);
              service.UpdateTriggers(requestContext, definition.ProjectId, definition.Id, repositoryResource, process.YamlFilename, pipelineBuilder, latestSourceVersion, definition.Repository.Url);
              FilteredBuildTriggerHelper.UpdateResourceRepositories(requestContext, new List<BuildDefinition>()
              {
                definition
              }, (RepositoryUpdateInfo) null, true);
              format = str + "Recalculation completed.";
            }
            catch (Exception ex)
            {
              flag = true;
              format = str + "Recalculation failed. Message:" + ex.Message + ".";
              requestContext.TraceException(12030360, "Build2", nameof (RecalculateResourceTriggersJob), ex);
            }
            requestContext.TraceAlways(12030361, TraceLevel.Info, "Build2", nameof (RecalculateResourceTriggersJob), format);
          }
          return flag ? TeamFoundationJobExecutionResult.PartiallySucceeded : TeamFoundationJobExecutionResult.Succeeded;
        }
        catch (Exception ex)
        {
          resultMessage = ex.Message;
          return TeamFoundationJobExecutionResult.Failed;
        }
      }
    }
  }
}
