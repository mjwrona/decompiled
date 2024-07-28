// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.ParallelExecutionOptions
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Microsoft.TeamFoundation.DistributedTask.Pipelines.Runtime;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines
{
  [DataContract]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class ParallelExecutionOptions
  {
    public ParallelExecutionOptions()
    {
    }

    private ParallelExecutionOptions(ParallelExecutionOptions optionsToCopy)
    {
      this.Matrix = optionsToCopy.Matrix;
      this.MaxConcurrency = optionsToCopy.MaxConcurrency;
    }

    [DataMember(EmitDefaultValue = false)]
    [JsonConverter(typeof (ExpressionValueJsonConverter<IDictionary<string, IDictionary<string, string>>>))]
    public ExpressionValue<IDictionary<string, IDictionary<string, string>>> Matrix { get; set; }

    [DataMember(EmitDefaultValue = false)]
    [JsonConverter(typeof (ExpressionValueJsonConverter<int>))]
    public ExpressionValue<int> MaxConcurrency { get; set; }

    public ParallelExecutionOptions Clone() => new ParallelExecutionOptions(this);

    internal JobExecutionContext CreateJobContext(
      PhaseExecutionContext context,
      string jobName,
      int attempt,
      ExpressionValue<string> container,
      IDictionary<string, ExpressionValue<string>> sidecarContainers,
      bool continueOnError,
      int timeoutInMinutes,
      int cancelTimeoutInMinutes,
      IJobFactory jobFactory)
    {
      JobExpansionOptions options = new JobExpansionOptions(jobName, attempt);
      return this.GenerateJobContexts(context, container, sidecarContainers, continueOnError, timeoutInMinutes, cancelTimeoutInMinutes, jobFactory, options).FirstOrDefault<JobExecutionContext>();
    }

    internal ExpandPhaseResult Expand(
      PhaseExecutionContext context,
      ExpressionValue<string> container,
      IDictionary<string, ExpressionValue<string>> sidecarContainers,
      bool continueOnError,
      int timeoutInMinutes,
      int cancelTimeoutInMinutes,
      IJobFactory jobFactory,
      JobExpansionOptions options)
    {
      IList<JobExecutionContext> jobContexts = this.GenerateJobContexts(context, container, sidecarContainers, continueOnError, timeoutInMinutes, cancelTimeoutInMinutes, jobFactory, options);
      ExpandPhaseResult expandPhaseResult = new ExpandPhaseResult();
      foreach (JobExecutionContext executionContext in (IEnumerable<JobExecutionContext>) jobContexts)
        expandPhaseResult.Jobs.Add(executionContext.Job);
      int num = this.GetMaxConcurrency(context);
      if (num <= 0)
        num = jobContexts.Count;
      expandPhaseResult.MaxConcurrency = num;
      return expandPhaseResult;
    }

    internal IList<JobExecutionContext> GenerateJobContexts(
      PhaseExecutionContext context,
      ExpressionValue<string> container,
      IDictionary<string, ExpressionValue<string>> sidecarContainers,
      bool continueOnError,
      int timeoutInMinutes,
      int cancelTimeoutInMinutes,
      IJobFactory jobFactory,
      JobExpansionOptions options)
    {
      List<JobExecutionContext> jobContexts = new List<JobExecutionContext>();
      if (this.Matrix != (ExpressionValue<IDictionary<string, IDictionary<string, string>>>) null)
      {
        IDictionary<string, IDictionary<string, string>> dictionary = context.Evaluate<IDictionary<string, IDictionary<string, string>>>("Matrix", this.Matrix, (IDictionary<string, IDictionary<string, string>>) null, false).Value;
        int count = dictionary != null ? dictionary.Count : 0;
        if (count > 0)
        {
          int positionInPhase = 1;
          foreach (KeyValuePair<string, IDictionary<string, string>> keyValuePair in (IEnumerable<KeyValuePair<string, IDictionary<string, string>>>) dictionary)
          {
            string key = keyValuePair.Key;
            string str1 = key;
            if (!PipelineUtilities.IsLegalNodeName(str1, !context.ExecutionOptions.DisallowWideCharactersInNodeNames))
            {
              string str2 = PipelineConstants.DefaultJobDisplayName + positionInPhase.ToString();
              context.Trace?.Info("\"" + str1 + "\" is not a legal node name; node will be named \"" + str2 + "\".");
              if (context.ExecutionOptions.EnforceLegalNodeNames)
                str1 = str2;
            }
            if (options == null || options.IsIncluded(str1))
              GenerateContext(Phase.GenerateDisplayName(context.Phase.Definition, key), str1, keyValuePair.Value, "MultiConfiguration", positionInPhase, count);
            ++positionInPhase;
          }
        }
      }
      else
      {
        int maxConcurrency = this.GetMaxConcurrency(context);
        if (maxConcurrency > 1)
        {
          if (options == null || options.Configurations == null || options.Configurations.Count == 0)
          {
            for (int positionInPhase = 1; positionInPhase <= maxConcurrency; ++positionInPhase)
            {
              string configuration = positionInPhase.ToString();
              GenerateContext(Phase.GenerateDisplayName(context.Phase.Definition, configuration), PipelineConstants.DefaultJobDisplayName + configuration, parallelExecutionType: "MultiMachine", positionInPhase: positionInPhase, totalJobsInPhase: maxConcurrency);
            }
          }
          else
          {
            foreach (string key in (IEnumerable<string>) options.Configurations.Keys)
            {
              string defaultJobDisplayName = PipelineConstants.DefaultJobDisplayName;
              int result;
              if (!key.StartsWith(defaultJobDisplayName, StringComparison.OrdinalIgnoreCase) || !int.TryParse(key.Substring(defaultJobDisplayName.Length), out result))
                throw new PipelineValidationException(PipelineStrings.PipelineNotValid());
              GenerateContext(Phase.GenerateDisplayName(context.Phase.Definition, result.ToString()), key, parallelExecutionType: "MultiMachine", positionInPhase: result, totalJobsInPhase: maxConcurrency);
            }
          }
        }
      }
      if (jobContexts.Count == 0)
      {
        string defaultJobName = PipelineConstants.DefaultJobName;
        if (options == null || options.IsIncluded(defaultJobName))
          GenerateContext(Phase.GenerateDisplayName(context.Phase.Definition), defaultJobName);
      }
      return (IList<JobExecutionContext>) jobContexts;

      void GenerateContext(
        string displayName,
        string configuration,
        IDictionary<string, string> configurationVariables = null,
        string parallelExecutionType = null,
        int positionInPhase = 1,
        int totalJobsInPhase = 1)
      {
        if (string.IsNullOrEmpty(configuration))
          configuration = PipelineConstants.DefaultJobName;
        JobExpansionOptions options = options;
        int attempt = options != null ? options.GetAttemptNumber(configuration) : -1;
        if (attempt < 1 && context.PreviousAttempts.Count > 0)
        {
          List<int> list = context.PreviousAttempts.SelectMany<PhaseAttempt, JobAttempt>((Func<PhaseAttempt, IEnumerable<JobAttempt>>) (x => (IEnumerable<JobAttempt>) x.Jobs)).Where<JobAttempt>((Func<JobAttempt, bool>) (x => x.Job.Name.Equals(configuration, StringComparison.OrdinalIgnoreCase))).Select<JobAttempt, int>((Func<JobAttempt, int>) (x => x.Job.Attempt)).ToList<int>();
          if (list.Count > 0)
            attempt = list.Max() + 1;
        }
        if (attempt < 1)
          attempt = 1;
        JobExecutionContext jobContext = context.CreateJobContext(configuration, attempt, positionInPhase, totalJobsInPhase);
        if (parallelExecutionType != null)
          jobContext.SetSystemVariables((IEnumerable<Variable>) new List<Variable>()
          {
            new Variable()
            {
              Name = WellKnownDistributedTaskVariables.ParallelExecutionType,
              Value = parallelExecutionType
            }
          });
        if (configurationVariables != null)
          jobContext.SetUserVariables(configurationVariables);
        jobContext.Job.Definition = jobFactory.CreateJob(jobContext, container, sidecarContainers, continueOnError, timeoutInMinutes, cancelTimeoutInMinutes, displayName);
        jobContexts.Add(jobContext);
        int count = jobContexts.Count;
        int? maxJobExpansion = context.ExecutionOptions.MaxJobExpansion;
        int valueOrDefault = maxJobExpansion.GetValueOrDefault();
        if (count > valueOrDefault & maxJobExpansion.HasValue)
          throw new MaxJobExpansionException(PipelineStrings.PhaseJobSlicingExpansionExceedLimit((object) jobContexts.Count.ToString(), (object) context.ExecutionOptions.MaxJobExpansion));
      }
    }

    private int GetMaxConcurrency(PhaseExecutionContext context)
    {
      int num = context.Evaluate<int>("MaxConcurrency", this.MaxConcurrency, 0).Value;
      int valueOrDefault = context.ExecutionOptions.MaxParallelism.GetValueOrDefault();
      return valueOrDefault > 0 && num > valueOrDefault ? valueOrDefault : num;
    }
  }
}
