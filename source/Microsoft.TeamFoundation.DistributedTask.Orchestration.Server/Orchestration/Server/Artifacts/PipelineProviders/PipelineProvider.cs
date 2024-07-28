// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.PipelineProviders.PipelineProvider
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Pipelines;
using Microsoft.TeamFoundation.DistributedTask.Pipelines.BuildProviders;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.PipelineProviders
{
  public class PipelineProvider : IPipelineProvider
  {
    private readonly string c_branchRef = "refs/heads/";

    public PipelineInfo GetLatestPipelineInfo(
      IVssRequestContext requestContext,
      Guid projectId,
      PipelineResource pipeline,
      ServiceEndpoint endpoint)
    {
      PipelineInfo latestPipelineInfo = (PipelineInfo) null;
      string project = pipeline.Properties.Get<string>(PipelinePropertyNames.Project);
      if (string.IsNullOrEmpty(project))
        project = projectId.ToString();
      string definition = pipeline.Properties.Get<string>(PipelinePropertyNames.Source);
      string branch1 = pipeline.Properties.Get<string>(PipelinePropertyNames.Branch);
      IList<string> tags = pipeline.Properties.Get<IList<string>>(PipelinePropertyNames.Tags);
      IPipelineTfsBuildService service = requestContext.GetService<IPipelineTfsBuildService>();
      if (branch1 != null && !branch1.StartsWith(this.c_branchRef, StringComparison.OrdinalIgnoreCase))
      {
        string branch2 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}{1}", (object) this.c_branchRef, (object) branch1);
        latestPipelineInfo = service.GetLatestPipelineInfo(requestContext, project, definition, branch2, tags);
      }
      if (latestPipelineInfo == null)
        latestPipelineInfo = service.GetLatestPipelineInfo(requestContext, project, definition, branch1, tags);
      return latestPipelineInfo;
    }

    public PipelineInfo GetPipelineInfo(
      IVssRequestContext requestContext,
      Guid projectId,
      PipelineResource pipeline,
      string pipelineNumber)
    {
      string project = pipeline.Properties.Get<string>(PipelinePropertyNames.Project);
      if (string.IsNullOrEmpty(project))
        project = projectId.ToString();
      string definition = pipeline.Properties.Get<string>(PipelinePropertyNames.Source);
      return requestContext.GetService<IPipelineTfsBuildService>().GetPipelineInfo(requestContext, project, definition, pipelineNumber);
    }

    public void Validate(
      IVssRequestContext requestContext,
      Guid projectId,
      PipelineResource pipeline,
      ServiceEndpoint endpoint)
    {
      string project = pipeline.Properties.Get<string>(PipelinePropertyNames.Project);
      if (string.IsNullOrEmpty(project) && projectId == Guid.Empty)
        throw new ResourceValidationException(TaskResources.PipelineResourceProjectInputRequired(), PipelinePropertyNames.Project);
      if (string.IsNullOrEmpty(project))
        project = projectId.ToString();
      string definitionName = pipeline.Properties.Get<string>(PipelinePropertyNames.Source);
      if (string.IsNullOrEmpty(definitionName))
        throw new ResourceValidationException(TaskResources.PipelineResourceSourceInputRequired(), PipelinePropertyNames.Source);
      if (endpoint == null && pipeline.Endpoint != null)
        throw new ResourceValidationException(TaskResources.PipelineResourceInvalidConnectionInput((object) pipeline.Endpoint.Name), "Endpoint");
      IList<string> stringList = pipeline.Properties.Get<IList<string>>(PipelinePropertyNames.Tags);
      if (stringList != null && stringList.Count > 0)
      {
        foreach (string str in (IEnumerable<string>) stringList)
        {
          if (string.IsNullOrEmpty(str))
            throw new ResourceValidationException(TaskResources.PipelineResourceTagMustBeNonEmpty(), PipelinePropertyNames.Tags);
        }
      }
      this.ValidateStageFilter(pipeline);
      IPipelineTfsBuildService service = requestContext.GetService<IPipelineTfsBuildService>();
      try
      {
        service.GetDefinitionId(requestContext, project, definitionName);
      }
      catch (Exception ex)
      {
        throw new ResourceValidationException(TaskResources.PipelineResourceMustBeValid((object) pipeline.Alias));
      }
    }

    private void ValidateStageFilter(PipelineResource pipeline)
    {
      IList<string> stringList = pipeline.Properties.Get<IList<string>>(PipelinePropertyNames.Stages);
      if (stringList == null || stringList.Count <= 0)
        return;
      foreach (string str in (IEnumerable<string>) stringList)
      {
        if (string.IsNullOrWhiteSpace(str))
          throw new ResourceValidationException(TaskResources.PipelineResourceStageMustBeNonEmpty(), PipelinePropertyNames.Stages);
      }
    }

    public IList<IVariable> GetPipelineVariables(
      IVssRequestContext requestContext,
      PipelineResource pipelineResource)
    {
      PipelineInfo pipelineInfo = this.GetPipelineInfo(requestContext, pipelineResource);
      IList<IVariable> pipelineVariables = (IList<IVariable>) new List<IVariable>();
      string variableValue1;
      pipelineResource.Properties.TryGetValue<string>(PipelinePropertyNames.Project, out variableValue1);
      string variableValue2;
      pipelineResource.Properties.TryGetValue<string>(PipelinePropertyNames.ProjectId, out variableValue2);
      this.AddVariableToPipelineVariables(pipelineVariables, pipelineResource.Alias, "projectName", variableValue1);
      this.AddVariableToPipelineVariables(pipelineVariables, pipelineResource.Alias, "projectID", variableValue2);
      this.AddVariableToPipelineVariables(pipelineVariables, pipelineResource.Alias, "pipelineID", pipelineInfo.DefinitionId.ToString());
      this.AddVariableToPipelineVariables(pipelineVariables, pipelineResource.Alias, "pipelineName", pipelineInfo.DefinitionName);
      this.AddVariableToPipelineVariables(pipelineVariables, pipelineResource.Alias, "runName", pipelineInfo.PipelineNumber);
      this.AddVariableToPipelineVariables(pipelineVariables, pipelineResource.Alias, "runID", pipelineInfo.Id.ToString());
      this.AddVariableToPipelineVariables(pipelineVariables, pipelineResource.Alias, "runURI", pipelineInfo.Uri.ToString());
      this.AddVariableToPipelineVariables(pipelineVariables, pipelineResource.Alias, "sourceBranch", pipelineInfo.SourceBranch);
      this.AddVariableToPipelineVariables(pipelineVariables, pipelineResource.Alias, "sourceCommit", pipelineInfo.SourceCommit);
      this.AddVariableToPipelineVariables(pipelineVariables, pipelineResource.Alias, "sourceProvider", pipelineInfo.RepositoryType);
      this.AddVariableToPipelineVariables(pipelineVariables, pipelineResource.Alias, "requestedFor", pipelineInfo.RequestedFor?.DisplayName);
      this.AddVariableToPipelineVariables(pipelineVariables, pipelineResource.Alias, "requestedForID", pipelineInfo.RequestedFor?.Id);
      return pipelineVariables;
    }

    private PipelineInfo GetPipelineInfo(
      IVssRequestContext requestContext,
      PipelineResource pipelineResource)
    {
      IPipelineTfsBuildService service = requestContext.GetService<IPipelineTfsBuildService>();
      Guid guid;
      pipelineResource.Properties.TryGetValue<Guid>(PipelinePropertyNames.ProjectId, out guid);
      int num;
      pipelineResource.Properties.TryGetValue<int>(PipelinePropertyNames.PipelineId, out num);
      IVssRequestContext requestContext1 = requestContext;
      Guid projectId = guid;
      int pipelineId = num;
      return service.GetPipelineInfo(requestContext1, projectId, pipelineId);
    }

    private void AddVariableToPipelineVariables(
      IList<IVariable> pipelineVariables,
      string alias,
      string variable,
      string variableValue)
    {
      if (string.IsNullOrEmpty(variableValue))
        return;
      string variableKey = WellKnownBuildArtifactVariables.GetVariableKey(alias, variable);
      pipelineVariables.Add((IVariable) new Variable()
      {
        Name = variableKey,
        Value = variableValue
      });
    }
  }
}
