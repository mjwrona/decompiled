// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.BuildProcessValidator
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server;
using Microsoft.TeamFoundation.DistributedTask.Pipelines;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Build2.Server
{
  public class BuildProcessValidator
  {
    public void Validate(
      IVssRequestContext requestContext,
      BuildDefinition definition,
      BuildDefinition originalDefinition = null,
      DefinitionUpdateOptions options = null)
    {
      if (definition.Repository != null)
        requestContext.GetService<IBuildSourceProviderService>().GetSourceProvider(requestContext, definition.Repository.Type).SetRepositoryDefaultInfo(requestContext, definition.ProjectId, definition.Repository);
      this.ValidateVariableGroupsPermissions(requestContext, definition, originalDefinition);
      switch (definition.Process.Type)
      {
        case 2:
          break;
        case 3:
          break;
        case 4:
          break;
        default:
          this.ValidateDesignerProcess(requestContext, definition, originalDefinition);
          break;
      }
    }

    private PipelineBuildResult BuildPipeline(
      IVssRequestContext requestContext,
      BuildDefinition definition,
      PipelineResources authorizedResources = null,
      bool authorizeNewResources = false,
      BuildOptions options = null)
    {
      PipelineBuilder pipelineBuilder = definition.GetPipelineBuilder(requestContext, authorizedResources, authorizeNewResources);
      PipelineBuildContext buildContext = pipelineBuilder.CreateBuildContext(options);
      return pipelineBuilder.Build(definition.GetProcess<DesignerProcess>().ToPipelineProcess(requestContext, definition, (IPipelineContext) buildContext).Stages, options);
    }

    private void ValidateDesignerProcess(
      IVssRequestContext requestContext,
      BuildDefinition definition,
      BuildDefinition originalDefinition)
    {
      definition.AllSteps().FixLatestMajorVersions(requestContext);
      PipelineResources authorizedResources = new PipelineResources();
      definition.GetProcess<DesignerProcess>();
      if (originalDefinition != null)
      {
        DesignerProcess process = originalDefinition.GetProcess<DesignerProcess>();
        if (process != null)
        {
          List<BuildDefinitionStep> second = new List<BuildDefinitionStep>();
          foreach (BuildDefinitionStep allStep in originalDefinition.AllSteps())
          {
            if (allStep.IsTaskGroup())
            {
              try
              {
                TaskGroup taskGroup = requestContext.GetService<IDistributedTaskPoolService>().GetTaskGroup(requestContext, definition.ProjectId, allStep.TaskDefinition.Id, allStep.TaskDefinition.VersionSpec, new bool?(false));
                if (taskGroup != null)
                {
                  if (!taskGroup.Disabled)
                    continue;
                }
                second.Add(allStep);
              }
              catch (MetaTaskDefinitionNotFoundException ex)
              {
                second.Add(allStep);
              }
            }
          }
          if (second.Count > 0)
          {
            foreach (Phase phase in process.Phases)
            {
              List<BuildDefinitionStep> list = phase.Steps.Except<BuildDefinitionStep>((IEnumerable<BuildDefinitionStep>) second).ToList<BuildDefinitionStep>();
              phase.Steps.Clear();
              phase.Steps.AddRange((IEnumerable<BuildDefinitionStep>) list);
            }
          }
        }
        if (originalDefinition.VariableGroups.Count > 0)
        {
          IList<Microsoft.TeamFoundation.DistributedTask.WebApi.VariableGroup> variableGroups = requestContext.GetService<IVariableGroupService>().GetVariableGroups(requestContext, originalDefinition.ProjectId, (IList<int>) originalDefinition.VariableGroups.Select<VariableGroup, int>((Func<VariableGroup, int>) (vg => vg.Id)).ToList<int>());
          IDictionary<int, bool> dictionary = (IDictionary<int, bool>) null;
          if (variableGroups != null && variableGroups.Count > 0)
            dictionary = (IDictionary<int, bool>) variableGroups.ToDictionary<Microsoft.TeamFoundation.DistributedTask.WebApi.VariableGroup, int, bool>((Func<Microsoft.TeamFoundation.DistributedTask.WebApi.VariableGroup, int>) (x => x.Id), (Func<Microsoft.TeamFoundation.DistributedTask.WebApi.VariableGroup, bool>) (x => true));
          List<VariableGroup> collection = new List<VariableGroup>();
          if (dictionary != null)
          {
            foreach (VariableGroup variableGroup in originalDefinition.VariableGroups)
            {
              if (dictionary.ContainsKey(variableGroup.Id))
                collection.Add(variableGroup);
            }
          }
          originalDefinition.VariableGroups.Clear();
          originalDefinition.VariableGroups.AddRange((IEnumerable<VariableGroup>) collection);
        }
        PipelineBuildResult pipelineBuildResult = this.BuildPipeline(requestContext, originalDefinition);
        pipelineBuildResult.ReferencedResources.Repositories.Clear();
        authorizedResources.MergeWith(pipelineBuildResult.ReferencedResources);
      }
      BuildOptions options = new BuildOptions()
      {
        ValidateTaskInputs = true,
        ValidateResources = true
      };
      options.ValidateStepNames = requestContext.IsFeatureEnabled("DistributedTask.TaskOutputVariables");
      if (definition.Queue == null || definition.Queue.Id == 0)
        options.AllowEmptyQueueTarget = true;
      PipelineBuildResult pipelineBuildResult1 = this.BuildPipeline(requestContext, definition, authorizedResources, true, options);
      if (pipelineBuildResult1.Errors.Count > 0)
        throw new PipelineValidationException((IEnumerable<PipelineValidationError>) pipelineBuildResult1.Errors);
    }

    private void ValidateVariableGroupsPermissions(
      IVssRequestContext requestContext,
      BuildDefinition definition,
      BuildDefinition originalDefinition)
    {
      List<VariableGroup> source = originalDefinition?.VariableGroups ?? new List<VariableGroup>();
      IEnumerable<int> ints1 = definition.VariableGroups.Select<VariableGroup, int>((Func<VariableGroup, int>) (vg => vg.Id));
      if (!ints1.Any<int>())
        return;
      IVariableGroupService service = requestContext.GetService<IVariableGroupService>();
      IEnumerable<int> second1 = source.Select<VariableGroup, int>((Func<VariableGroup, int>) (vg => vg.Id));
      IEnumerable<int> ints2 = ints1.Except<int>(second1);
      if (!ints2.Any<int>())
        return;
      IEnumerable<int> second2 = service.GetVariableGroups(requestContext, definition.ProjectId, string.Empty, VariableGroupActionFilter.Use, new int?(0), queryOrder: VariableGroupQueryOrder.IdAscending).Select<Microsoft.TeamFoundation.DistributedTask.WebApi.VariableGroup, int>((Func<Microsoft.TeamFoundation.DistributedTask.WebApi.VariableGroup, int>) (vg => vg.Id));
      IEnumerable<int> ints3 = ints2.Except<int>(second2);
      if (ints3.Any<int>())
        throw new VariableGroupsAccessDeniedException(BuildServerResources.VariableGroupsAccessDenied((object) string.Join<int>(", ", ints3)));
    }
  }
}
