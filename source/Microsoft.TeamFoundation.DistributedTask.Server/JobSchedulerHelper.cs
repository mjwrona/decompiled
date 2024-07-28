// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.JobSchedulerHelper
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Pipelines;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.DistributedTask.Server
{
  public static class JobSchedulerHelper
  {
    public static bool ShouldAutoInjectDownloadArtifactStep(Dictionary<string, string> jobMetaData)
    {
      string str;
      return !jobMetaData.TryGetValue(DeploymentStrategyRunTimeConstants.LifeCycleHookType, out str) || str.Equals(typeof (DeployHook).ToString());
    }

    public static int GetJobOrder(Dictionary<string, string> jobMetaData)
    {
      string s;
      int result;
      return jobMetaData.TryGetValue(DeploymentStrategyRunTimeConstants.JobOrder, out s) && int.TryParse(s, out result) && result > 0 ? result : 1;
    }

    public static IList<TaskStep> GetDecoratorPreJobSteps(
      IVssRequestContext requestContext,
      IPipelineContext context,
      IReadOnlyList<JobStep> steps)
    {
      List<TaskStep> decoratorPreJobSteps = new List<TaskStep>();
      foreach (IStepProvider pipelineDecorator in JobSchedulerHelper.GetPipelineDecorators(requestContext, context))
        decoratorPreJobSteps.AddRange((IEnumerable<TaskStep>) pipelineDecorator.GetPreSteps(context, steps));
      return (IList<TaskStep>) decoratorPreJobSteps;
    }

    public static IList<TaskStep> GetDecoratorPostJobSteps(
      IVssRequestContext requestContext,
      IPipelineContext context,
      IReadOnlyList<JobStep> steps)
    {
      List<TaskStep> decoratorPostJobSteps = new List<TaskStep>();
      foreach (IStepProvider pipelineDecorator in JobSchedulerHelper.GetPipelineDecorators(requestContext, context))
        decoratorPostJobSteps.AddRange((IEnumerable<TaskStep>) pipelineDecorator.GetPostSteps(context, steps));
      return (IList<TaskStep>) decoratorPostJobSteps;
    }

    private static List<IStepProvider> GetPipelineDecorators(
      IVssRequestContext requestContext,
      IPipelineContext context)
    {
      List<IStepProvider> pipelineDecorators = new List<IStepProvider>();
      if (context.StepProviders != null && requestContext.IsFeatureEnabled("Pipelines.Environments.EnableDecoratorForDeploymentJob"))
        pipelineDecorators = context.StepProviders.Where<IStepProvider>((Func<IStepProvider, bool>) (sp => sp is IPipelineDecorator)).ToList<IStepProvider>();
      return pipelineDecorators;
    }
  }
}
