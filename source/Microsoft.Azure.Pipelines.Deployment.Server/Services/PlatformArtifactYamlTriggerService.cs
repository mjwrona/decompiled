// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Deployment.Services.PlatformArtifactYamlTriggerService
// Assembly: Microsoft.Azure.Pipelines.Deployment.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B08F1AB-5B33-41F6-908E-9A985FD0544C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.Pipelines.Deployment.Server.dll

using Microsoft.Azure.Pipelines.Deployment.Extensions;
using Microsoft.Azure.Pipelines.Deployment.Model;
using Microsoft.Azure.Pipelines.Deployment.Sdk.Server.Artifacts;
using Microsoft.Azure.Pipelines.Deployment.Utilities;
using Microsoft.TeamFoundation.DistributedTask.Pipelines;
using Microsoft.TeamFoundation.DistributedTask.Pipelines.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.Azure.Pipelines.Deployment.Services
{
  public sealed class PlatformArtifactYamlTriggerService : 
    IArtifactYamlTriggerService,
    IVssFrameworkService
  {
    private const string TraceLayer = "PlatformArtifactYamlTriggerService";

    public void AddTriggers(
      IVssRequestContext requestContext,
      Guid projectId,
      int definitionId,
      RepositoryResource repositoryResource,
      string yamlFileName,
      PipelineBuilder pipelineBuilder,
      string sourceVersion,
      Uri repositoryUrl)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
      ArgumentUtility.CheckForNull<RepositoryResource>(repositoryResource, nameof (repositoryResource));
      ArgumentUtility.CheckStringForNullOrEmpty(yamlFileName, nameof (yamlFileName));
      ArgumentUtility.CheckForNull<PipelineBuilder>(pipelineBuilder, nameof (pipelineBuilder));
      if (!requestContext.IsFeatureEnabled("DistributedTask.EnablePipelineTriggers"))
        return;
      using (PlatformArtifactYamlTriggerServiceTracer tracer = new PlatformArtifactYamlTriggerServiceTracer(requestContext, 100161030))
      {
        PipelineTriggerMaterializationRef materializationRef = PipelineTriggerIssuesExtension.toTriggerMaterializationRef(requestContext, projectId, definitionId, yamlFileName, sourceVersion, repositoryUrl, DateTime.Now);
        requestContext.GetService<IPipelineTriggerMaterializationService>().CreatePipelineTriggerMaterializationRef(requestContext, projectId, definitionId, materializationRef);
        tracer.AddTrace("Complete creating trigger materialization ref");
        PipelineResources resources = this.LoadYamlPipeline(requestContext, projectId, repositoryResource, yamlFileName, pipelineBuilder, definitionId, tracer);
        IList<PipelineDefinitionTrigger> definitionTriggers = resources.ToPipelineDefinitionTriggers(requestContext, projectId, definitionId);
        if (definitionTriggers.Any<PipelineDefinitionTrigger>())
        {
          tracer.AddTrace(string.Format("Project Id: {0}, Definition Id: {1}, Triggers: {2}", (object) projectId, (object) definitionId, (object) definitionTriggers.Select<PipelineDefinitionTrigger, string>((Func<PipelineDefinitionTrigger, string>) (trigger => trigger.Alias)).Aggregate<string>((Func<string, string, string>) ((i, j) => i + "," + j))));
          requestContext.GetService<IPipelineTriggerService>().CreatePipelineTriggers(requestContext, projectId, definitionId, definitionTriggers);
          tracer.AddTrace("Created triggers successfully");
          string resultMessage;
          string resultExceptionMessage;
          PipelineWebHookHelper.UpdateWebHooks(requestContext, projectId, (IList<PipelineDefinitionTrigger>) null, definitionTriggers, resources, out resultMessage, out resultExceptionMessage);
          tracer.AddTrace("Update webhook result: " + resultMessage + "; Exception: " + (resultExceptionMessage ?? "N/A"));
          if (!string.IsNullOrEmpty(resultExceptionMessage))
            this.CreatePipelineTriggerIssueError(requestContext, projectId, definitionId);
        }
        CustomerIntelligenceHelper.PublishPipelineResourcesUsage(requestContext, projectId, definitionId, resources);
      }
    }

    public void UpdateTriggers(
      IVssRequestContext requestContext,
      Guid projectId,
      int definitionId,
      RepositoryResource repositoryResource,
      string yamlFileName,
      PipelineBuilder pipelineBuilder,
      string sourceVersion,
      Uri repositoryUrl)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
      ArgumentUtility.CheckForNull<RepositoryResource>(repositoryResource, nameof (repositoryResource));
      ArgumentUtility.CheckStringForNullOrEmpty(yamlFileName, nameof (yamlFileName));
      ArgumentUtility.CheckForNull<PipelineBuilder>(pipelineBuilder, nameof (pipelineBuilder));
      if (!requestContext.IsFeatureEnabled("DistributedTask.EnablePipelineTriggers"))
        return;
      using (PlatformArtifactYamlTriggerServiceTracer tracer = new PlatformArtifactYamlTriggerServiceTracer(requestContext, 100161032))
      {
        tracer.AddTrace(string.Format("Project Id: {0}, Definition Id: {1}", (object) projectId, (object) definitionId));
        IPipelineTriggerMaterializationService service1 = requestContext.GetService<IPipelineTriggerMaterializationService>();
        service1.DeletePipelineTriggerMaterializationRef(requestContext, projectId, (IList<int>) new int[1]
        {
          definitionId
        }, true);
        tracer.AddTrace("Complete deleting trigger materialization ref");
        PipelineTriggerMaterializationRef materializationRef = PipelineTriggerIssuesExtension.toTriggerMaterializationRef(requestContext, projectId, definitionId, yamlFileName, sourceVersion, repositoryUrl, DateTime.Now);
        service1.CreatePipelineTriggerMaterializationRef(requestContext, projectId, definitionId, materializationRef);
        tracer.AddTrace("Complete creating trigger materialization ref");
        requestContext.GetService<IPipelineTriggerIssuesService>().DeletePipelineTriggerIssues(requestContext, projectId, (IList<int>) new int[1]
        {
          definitionId
        }, true);
        tracer.AddTrace("Complete deleting trigger issues");
        PipelineResources resources = this.LoadYamlPipeline(requestContext, projectId, repositoryResource, yamlFileName, pipelineBuilder, definitionId, tracer);
        IList<PipelineDefinitionTrigger> definitionTriggers = resources.ToPipelineDefinitionTriggers(requestContext, projectId, definitionId);
        IPipelineTriggerService service2 = requestContext.GetService<IPipelineTriggerService>();
        IList<PipelineDefinitionTrigger> pipelineTriggers = service2.GetPipelineTriggers(requestContext, projectId, definitionId);
        List<PipelineDefinitionTrigger> list1 = pipelineTriggers.Except<PipelineDefinitionTrigger>((IEnumerable<PipelineDefinitionTrigger>) definitionTriggers).ToList<PipelineDefinitionTrigger>();
        if (list1.Any<PipelineDefinitionTrigger>())
        {
          tracer.AddTrace("Remove triggers. Triggers: " + string.Join(",", list1.SelectMany<PipelineDefinitionTrigger, string>((Func<PipelineDefinitionTrigger, IEnumerable<string>>) (trigger => (IEnumerable<string>) new List<string>()
          {
            "{Definition id:",
            trigger.PipelineDefinitionId.ToString(),
            " Alias:",
            trigger.Alias,
            " Enabled:",
            trigger.TriggerContent?.Enabled.ToString(),
            "}"
          }))));
          foreach (PipelineDefinitionTrigger definitionTrigger in list1)
            service2.DeletePipelineTriggers(requestContext, projectId, (IList<int>) new int[1]
            {
              definitionId
            }, definitionTrigger.Alias);
          tracer.AddTrace("Completed successfully");
        }
        List<PipelineDefinitionTrigger> list2 = definitionTriggers.Except<PipelineDefinitionTrigger>((IEnumerable<PipelineDefinitionTrigger>) pipelineTriggers).ToList<PipelineDefinitionTrigger>();
        if (list2.Any<PipelineDefinitionTrigger>())
        {
          tracer.AddTrace("Add triggers. Triggers: " + string.Join(",", list2.SelectMany<PipelineDefinitionTrigger, string>((Func<PipelineDefinitionTrigger, IEnumerable<string>>) (trigger => (IEnumerable<string>) new List<string>()
          {
            "{Definition id:",
            trigger.PipelineDefinitionId.ToString(),
            " Alias:",
            trigger.Alias,
            " Enabled:",
            trigger.TriggerContent?.Enabled.ToString(),
            "}"
          }))));
          service2.CreatePipelineTriggers(requestContext, projectId, definitionId, (IList<PipelineDefinitionTrigger>) list2);
          tracer.AddTrace("Completed successfully");
        }
        requestContext.ServiceName = "Pipeline";
        string resultMessage;
        string resultExceptionMessage;
        PipelineWebHookHelper.UpdateWebHooks(requestContext, projectId, pipelineTriggers, definitionTriggers, resources, out resultMessage, out resultExceptionMessage);
        tracer.AddTrace("Update webhook result: " + resultMessage + "; Exception: " + (resultExceptionMessage ?? "N/A"));
        if (resultExceptionMessage != string.Empty)
          this.CreatePipelineTriggerIssueError(requestContext, projectId, definitionId);
        CustomerIntelligenceHelper.PublishPipelineResourcesUsage(requestContext, projectId, definitionId, resources);
      }
    }

    public void DeleteTriggers(
      IVssRequestContext requestContext,
      Guid projectId,
      IEnumerable<int> definitionIds)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
      if (!requestContext.IsFeatureEnabled("DistributedTask.EnablePipelineTriggers"))
        return;
      using (PlatformArtifactYamlTriggerServiceTracer triggerServiceTracer = new PlatformArtifactYamlTriggerServiceTracer(requestContext, 100161031))
      {
        triggerServiceTracer.AddTrace(string.Format("Project Id: {0}, Definition Ids: {1}", (object) projectId, (object) string.Join<int>(",", definitionIds)));
        IPipelineTriggerService service = requestContext.GetService<IPipelineTriggerService>();
        List<PipelineDefinitionTrigger> existingTriggers = new List<PipelineDefinitionTrigger>();
        foreach (int definitionId in definitionIds)
          existingTriggers.AddRange((IEnumerable<PipelineDefinitionTrigger>) service.GetPipelineTriggers(requestContext, projectId, definitionId));
        if (definitionIds != null && definitionIds.Any<int>())
        {
          triggerServiceTracer.AddTrace("Remove all triggers.");
          service.DeletePipelineTriggers(requestContext, projectId, (IList<int>) definitionIds.ToList<int>());
          triggerServiceTracer.AddTrace("Completed successfully");
          string resultMessage;
          string resultExceptionMessage;
          PipelineWebHookHelper.UpdateWebHooks(requestContext, projectId, (IList<PipelineDefinitionTrigger>) existingTriggers, (IList<PipelineDefinitionTrigger>) null, (PipelineResources) null, out resultMessage, out resultExceptionMessage);
          triggerServiceTracer.AddTrace("Update webhook result: " + resultMessage + "; Exception: " + (resultExceptionMessage ?? "N/A"));
        }
        requestContext.GetService<IPipelineTriggerIssuesService>().DeletePipelineTriggerIssues(requestContext, projectId, (IList<int>) definitionIds.ToList<int>());
        triggerServiceTracer.AddTrace("Complete deleting trigger issues");
        requestContext.GetService<IPipelineTriggerMaterializationService>().DeletePipelineTriggerMaterializationRef(requestContext, projectId, (IList<int>) definitionIds.ToList<int>(), false);
        triggerServiceTracer.AddTrace("Complete deleting trigger materialization ref");
        foreach (int definitionId in definitionIds)
          CustomerIntelligenceHelper.PublishPipelineResourcesUsage(requestContext, projectId, definitionId);
      }
    }

    private PipelineResources LoadYamlPipeline(
      IVssRequestContext requestContext,
      Guid projectId,
      RepositoryResource repository,
      string yamlFileName,
      PipelineBuilder builder,
      int pipelineDefinitionId,
      PlatformArtifactYamlTriggerServiceTracer tracer)
    {
      try
      {
        YamlPipelineLoadResult pipelineLoadResult = requestContext.GetService<IYamlPipelineLoaderService>().Load(requestContext, projectId, repository, yamlFileName, builder);
        if (pipelineLoadResult.Template.Errors.Any<PipelineValidationError>())
        {
          if (requestContext.IsFeatureEnabled("DistributedTask.EnableYamlPipelineTriggerIssues"))
            pipelineLoadResult.Template.Errors.ForEach<PipelineValidationError>((Action<PipelineValidationError>) (error =>
            {
              if ((error.Message == null || !error.Message.Contains("Unexpected value")) && (error.Code == null || !error.Code.Equals("ResourceValidationException")))
                return;
              this.CreatePipelineTriggerIssueError(requestContext, projectId, pipelineDefinitionId);
            }));
          tracer.AddTrace("Couple of error occurred while loading yaml " + yamlFileName);
          requestContext.TraceAlways(100161006, TraceLevel.Info, "Deployment", nameof (PlatformArtifactYamlTriggerService), pipelineLoadResult.Template.Errors.First<PipelineValidationError>().Message);
        }
        return pipelineLoadResult.Template.Resources;
      }
      catch (Exception ex)
      {
        tracer.AddTrace("Not able to load yam file " + yamlFileName);
        this.CreatePipelineTriggerIssueError(requestContext, projectId, pipelineDefinitionId);
        requestContext.TraceException(100161006, "Deployment", nameof (PlatformArtifactYamlTriggerService), ex);
        throw;
      }
    }

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    private void CreatePipelineTriggerIssueError(
      IVssRequestContext requestContext,
      Guid projectId,
      int pipelineDefinitionId)
    {
      if (!requestContext.IsFeatureEnabled("DistributedTask.EnableYamlPipelineTriggerIssues"))
        return;
      PipelineTriggerIssues pipelineTriggerIssues = new PipelineTriggerIssues()
      {
        PipelineDefinitionId = pipelineDefinitionId,
        Alias = "",
        BuildNumber = "",
        ErrorMessage = DeploymentResources.ErrorTriggerConfiguration(),
        isError = true
      };
      requestContext.GetService<IPipelineTriggerIssuesService>().CreatePipelineTriggerIssues(requestContext, projectId, pipelineDefinitionId, (IList<PipelineTriggerIssues>) new PipelineTriggerIssues[1]
      {
        pipelineTriggerIssues
      });
    }
  }
}
