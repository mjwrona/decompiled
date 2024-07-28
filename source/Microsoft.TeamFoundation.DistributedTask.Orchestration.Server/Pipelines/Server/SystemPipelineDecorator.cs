// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.Server.SystemPipelineDecorator
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server;
using Microsoft.TeamFoundation.DistributedTask.Pipelines.Runtime;
using Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml;
using Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.ObjectTemplating;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines.Server
{
  internal class SystemPipelineDecorator : IStepProvider
  {
    private const string c_layer = "SystemPipelineDecorator";
    private IVssRequestContext m_requestContext;

    public SystemPipelineDecorator(IVssRequestContext requestContext) => this.m_requestContext = requestContext;

    public IList<TaskStep> GetPreSteps(IPipelineContext context, IReadOnlyList<JobStep> steps)
    {
      if (context is JobExecutionContext executionContext && executionContext.Job.Definition?.Target is ServerTarget)
        return (IList<TaskStep>) Array.Empty<TaskStep>();
      YamlPipelineLoaderService.PipelineTraceWriter trace = new YamlPipelineLoaderService.PipelineTraceWriter(this.m_requestContext);
      ParseOptions parseOptions = ParseOptionsFactory.CreateParseOptions(this.m_requestContext, RetrieveOptions.Default);
      PipelineParser pipelineParser = new PipelineParser((ITraceWriter) trace, parseOptions, (IFileProviderFactory) null);
      PipelineStepsTemplate pipelineStepsTemplate;
      try
      {
        pipelineStepsTemplate = pipelineParser.LoadSystemPreSteps(context.Variables, context.ResourceStore, steps, this.m_requestContext.CancellationToken);
        if (pipelineStepsTemplate.Errors.Count == 0)
        {
          BuildOptions options = new BuildOptions()
          {
            ResolveTaskInputAliases = true,
            ValidateResources = true,
            ValidateStepNames = true,
            AllowEmptyQueueTarget = true
          };
          pipelineStepsTemplate.Errors.AddRange<PipelineValidationError, IList<PipelineValidationError>>((IEnumerable<PipelineValidationError>) context.Validate(pipelineStepsTemplate.Steps, (PhaseTarget) null, options));
        }
        if (context.Trace != null)
        {
          context.Trace.Info("########## " + TaskResources.SystemPipelineDecorators() + " ##########\n");
          context.Trace.Info(trace.Log());
        }
      }
      catch (Exception ex)
      {
        this.m_requestContext.TraceException(10016110, nameof (SystemPipelineDecorator), ex);
        throw;
      }
      if (pipelineStepsTemplate.Errors.Count > 0)
      {
        for (int index = 0; index < pipelineStepsTemplate.Errors.Count; ++index)
          this.m_requestContext.TraceError(10016110, nameof (SystemPipelineDecorator), "System steps template error {0} of {1}: {2}.", (object) (index + 1), (object) pipelineStepsTemplate.Errors.Count, (object) pipelineStepsTemplate.Errors[index].Message);
        throw new PipelineValidationException((IEnumerable<PipelineValidationError>) pipelineStepsTemplate.Errors);
      }
      return (IList<TaskStep>) pipelineStepsTemplate.Steps.Select<Step, TaskStep>((Func<Step, TaskStep>) (x => x as TaskStep)).ToList<TaskStep>();
    }

    public Dictionary<Guid, List<TaskStep>> GetPostTaskSteps(
      IPipelineContext context,
      IReadOnlyList<JobStep> steps)
    {
      return new Dictionary<Guid, List<TaskStep>>();
    }

    public IList<TaskStep> GetPostSteps(IPipelineContext context, IReadOnlyList<JobStep> steps) => (IList<TaskStep>) Array.Empty<TaskStep>();

    public Dictionary<Guid, List<TaskStep>> GetPostTargetTaskSteps(
      IPipelineContext context,
      IReadOnlyList<JobStep> steps)
    {
      return new Dictionary<Guid, List<TaskStep>>();
    }

    public Dictionary<Guid, List<TaskStep>> GetPreTargetTaskSteps(
      IPipelineContext context,
      IReadOnlyList<JobStep> steps)
    {
      return new Dictionary<Guid, List<TaskStep>>();
    }

    public Dictionary<Guid, List<string>> GetInputsToProvide(IPipelineContext context) => new Dictionary<Guid, List<string>>();

    public bool ResolveStep(
      IPipelineContext context,
      JobStep step,
      out IList<TaskStep> resolvedSteps)
    {
      resolvedSteps = (IList<TaskStep>) null;
      return false;
    }
  }
}
