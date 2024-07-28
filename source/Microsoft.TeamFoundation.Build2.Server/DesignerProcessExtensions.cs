// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.DesignerProcessExtensions
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.DistributedTask.Pipelines;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Build2.Server
{
  internal static class DesignerProcessExtensions
  {
    private static readonly Guid AllowScriptsAuthAccessBuildOptionId = new Guid("57578776-4C22-4526-AEB0-86B6DA17EE9C");

    public static PipelineProcess ToPipelineProcess(
      this DesignerProcess process,
      IVssRequestContext requestContext,
      BuildDefinition definition,
      IPipelineContext context,
      IEnumerable<Demand> additionalDemands = null,
      string sourceBranch = null,
      string sourceVersion = null)
    {
      using (requestContext.TraceScope(nameof (DesignerProcessExtensions), nameof (ToPipelineProcess)))
      {
        IBuildSourceProvider sourceProvider = requestContext.GetService<IBuildSourceProviderService>().GetSourceProvider(requestContext, definition.Repository.Type);
        string str1 = (string) null;
        List<Demand> demands = new List<Demand>();
        if (additionalDemands != null)
          demands.AddRange(additionalDemands);
        else
          demands.AddRange((IEnumerable<Demand>) definition.Demands);
        demands.AddRange((IEnumerable<Demand>) sourceProvider.GetDemands(requestContext));
        int num;
        if (definition == null)
        {
          num = 0;
        }
        else
        {
          List<BuildOption> options = definition.Options;
          bool? nullable = options != null ? new bool?(options.Any<BuildOption>((Func<BuildOption, bool>) (o => o.Definition.Id == DesignerProcessExtensions.AllowScriptsAuthAccessBuildOptionId && o.Enabled))) : new bool?();
          bool flag = true;
          num = nullable.GetValueOrDefault() == flag & nullable.HasValue ? 1 : 0;
        }
        bool flag1 = num != 0;
        if (context.EnvironmentVersion > 1)
        {
          RepositoryResource repositoryResource = definition.Repository.ToRepositoryResource(requestContext, PipelineConstants.DesignerRepo, sourceBranch, sourceVersion);
          context.ResourceStore.Repositories.Add(repositoryResource);
          if (repositoryResource.Endpoint != null)
            context.ResourceStore.Endpoints.Authorize(repositoryResource.Endpoint);
        }
        List<PhaseNode> phases = new List<PhaseNode>();
        for (int index = 0; index < process.Phases.Count; ++index)
        {
          Phase phase1 = process.Phases[index];
          Microsoft.TeamFoundation.DistributedTask.Pipelines.Phase phase2 = new Microsoft.TeamFoundation.DistributedTask.Pipelines.Phase();
          phase2.Condition = phase1.Condition;
          phase2.DisplayName = string.IsNullOrEmpty(phase1.Name) ? BuildServerResources.BuildJobName() : phase1.Name;
          phase2.Name = process.SupportsPhaseDependencies() ? phase1.RefName : string.Format("Phase_{0}", (object) index);
          Microsoft.TeamFoundation.DistributedTask.Pipelines.Phase phase3 = phase2;
          phase3.Steps.AddRange<Step, IList<Step>>(phase1.Steps.Select<BuildDefinitionStep, Step>((Func<BuildDefinitionStep, Step>) (step => step.ToStep())));
          if (phase1.Target == null)
            phase1.Target = (PhaseTarget) new AgentPoolQueueTarget();
          int timeoutInMinutes1 = phase1.JobTimeoutInMinutes;
          if (timeoutInMinutes1 == 0)
            timeoutInMinutes1 = definition.JobTimeoutInMinutes;
          int timeoutInMinutes2 = phase1.JobCancelTimeoutInMinutes;
          if (timeoutInMinutes2 == 0)
            timeoutInMinutes2 = definition.JobCancelTimeoutInMinutes;
          AgentPoolQueueTarget target1 = phase1.Target as AgentPoolQueueTarget;
          ServerTarget target2 = phase1.Target as ServerTarget;
          if (target1 != null)
          {
            string workspaceCleanOption = string.Empty;
            if (context.EnvironmentVersion > 1)
            {
              TaskStep checkoutTask = definition.Repository.ToCheckoutTask(requestContext, flag1 || target1.AllowScriptsAuthAccessOption);
              phase3.Steps.Insert(0, (Step) checkoutTask);
              bool result1;
              string str2;
              RepositoryCleanOptions result2;
              if (bool.TryParse(definition.Repository.Clean, out result1) & result1 && definition.Repository.Properties.TryGetValue("cleanOptions", out str2) && Enum.TryParse<RepositoryCleanOptions>(str2, out result2))
              {
                target1.Workspace = new WorkspaceOptions();
                switch (result2)
                {
                  case RepositoryCleanOptions.SourceAndOutputDir:
                    workspaceCleanOption = PipelineConstants.WorkspaceCleanOptions.Outputs;
                    break;
                  case RepositoryCleanOptions.SourceDir:
                    workspaceCleanOption = PipelineConstants.WorkspaceCleanOptions.Resources;
                    break;
                  case RepositoryCleanOptions.AllBuildDir:
                    workspaceCleanOption = PipelineConstants.WorkspaceCleanOptions.All;
                    break;
                }
              }
            }
            bool isGatedTrigger = definition.Triggers.Any<BuildTrigger>((Func<BuildTrigger, bool>) (x => x.TriggerType == DefinitionTriggerType.GatedCheckIn));
            phase3.Target = target1.ToPipelinePhaseTarget(requestContext, context.Variables, (IReadOnlyList<Demand>) demands, timeoutInMinutes1, timeoutInMinutes2, isGatedTrigger, workspaceCleanOption);
            if (flag1 || target1.AllowScriptsAuthAccessOption)
              phase3.Variables.Add((IVariable) new Variable()
              {
                Name = WellKnownDistributedTaskVariables.EnableAccessToken,
                Value = "true"
              });
          }
          else if (target2 != null)
            phase3.Target = target2.ToPipelinePhaseTarget(requestContext, context.Variables, timeoutInMinutes1, timeoutInMinutes2);
          if (!process.SupportsPhaseDependencies())
          {
            if (str1 != null)
              phase3.DependsOn.Add(str1);
            str1 = phase3.Name;
          }
          else
            phase3.DependsOn.AddRange<string, ISet<string>>(phase1.Dependencies.Select<Dependency, string>((Func<Dependency, string>) (d => d.Scope)));
          phases.Add((PhaseNode) phase3);
        }
        return new PipelineProcess((IList<PhaseNode>) phases);
      }
    }
  }
}
