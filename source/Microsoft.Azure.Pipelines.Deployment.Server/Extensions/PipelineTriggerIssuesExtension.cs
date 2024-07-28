// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Deployment.Extensions.PipelineTriggerIssuesExtension
// Assembly: Microsoft.Azure.Pipelines.Deployment.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B08F1AB-5B33-41F6-908E-9A985FD0544C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.Pipelines.Deployment.Server.dll

using Microsoft.Azure.Pipelines.Deployment.Model;
using Microsoft.TeamFoundation.DistributedTask.Pipelines;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.Azure.Pipelines.Deployment.Extensions
{
  public static class PipelineTriggerIssuesExtension
  {
    public static IList<PipelineTriggerIssues> ToPipelineTriggerIssues(
      IVssRequestContext requestContext,
      Guid projectId,
      int pipelineDefinitionId,
      PipelineResources resources,
      string errorMessage,
      string triggerAlias,
      bool isError)
    {
      IList<PipelineTriggerIssues> pipelineTriggerIssues = (IList<PipelineTriggerIssues>) new List<PipelineTriggerIssues>();
      foreach (PipelineResource resource in (IEnumerable<PipelineResource>) (resources?.Pipelines ?? (ISet<PipelineResource>) new HashSet<PipelineResource>()))
      {
        if (resource.Trigger != null && resource.Alias == triggerAlias)
          pipelineTriggerIssues.Add(PipelineTriggerIssuesExtension.ToPipelineDefinitionTriggerIssues(requestContext, projectId, resource, pipelineDefinitionId, errorMessage, isError));
      }
      foreach (ContainerResource resource in (IEnumerable<ContainerResource>) (resources?.Containers ?? (ISet<ContainerResource>) new HashSet<ContainerResource>()))
      {
        if (resource.Trigger != null && resource.Alias == triggerAlias)
          pipelineTriggerIssues.Add(PipelineTriggerIssuesExtension.ToPipelineDefinitionTriggerIssues(requestContext, projectId, resource, pipelineDefinitionId, errorMessage, isError));
      }
      return pipelineTriggerIssues;
    }

    private static PipelineTriggerIssues ToPipelineDefinitionTriggerIssues(
      IVssRequestContext requestContext,
      Guid projectId,
      PipelineResource resource,
      int pipelineDefinitionId,
      string errorMessage,
      bool isError)
    {
      return new PipelineTriggerIssues()
      {
        PipelineDefinitionId = pipelineDefinitionId,
        Alias = resource.Alias,
        BuildNumber = resource.Version,
        ErrorMessage = errorMessage,
        isError = isError
      };
    }

    private static PipelineTriggerIssues ToPipelineDefinitionTriggerIssues(
      IVssRequestContext requestContext,
      Guid projectId,
      ContainerResource resource,
      int pipelineDefinitionId,
      string errorMessage,
      bool isError)
    {
      return new PipelineTriggerIssues()
      {
        PipelineDefinitionId = pipelineDefinitionId,
        Alias = resource.Alias,
        BuildNumber = resource.Image,
        ErrorMessage = errorMessage,
        isError = isError
      };
    }

    public static PipelineTriggerMaterializationRef toTriggerMaterializationRef(
      IVssRequestContext requestContext,
      Guid projectId,
      int pipelineDefinitionId,
      string yamlFileName,
      string commitId,
      Uri repositoryUrl,
      DateTime lastMaterializedDate)
    {
      return new PipelineTriggerMaterializationRef()
      {
        YAMLFileName = yamlFileName,
        CommitId = commitId,
        RepositoryUrl = repositoryUrl != (Uri) null ? repositoryUrl.ToString() : "",
        LastMaterializedDate = lastMaterializedDate
      };
    }
  }
}
