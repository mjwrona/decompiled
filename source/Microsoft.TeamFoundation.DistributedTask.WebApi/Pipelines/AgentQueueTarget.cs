// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.AgentQueueTarget
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Microsoft.TeamFoundation.DistributedTask.Expressions;
using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server;
using Microsoft.TeamFoundation.DistributedTask.Pipelines.Runtime;
using Microsoft.TeamFoundation.DistributedTask.Pipelines.Validation;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines
{
  [DataContract]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class AgentQueueTarget : PhaseTarget
  {
    [DataMember(Name = "SidecarContainers", EmitDefaultValue = false)]
    private IDictionary<string, ExpressionValue<string>> m_sidecarContainers;

    public AgentQueueTarget()
      : base(PhaseTargetType.Queue)
    {
    }

    private AgentQueueTarget(AgentQueueTarget targetToClone)
      : base((PhaseTarget) targetToClone)
    {
      this.Queue = targetToClone.Queue?.Clone();
      this.Execution = targetToClone.Execution?.Clone();
      this.Workspace = targetToClone.Workspace?.Clone();
      if (targetToClone.AgentSpecification != null)
        this.AgentSpecification = new JObject(targetToClone.AgentSpecification);
      IDictionary<string, ExpressionValue<string>> sidecarContainers = targetToClone.SidecarContainers;
      if ((sidecarContainers != null ? (sidecarContainers.Count > 0 ? 1 : 0) : 0) == 0)
        return;
      this.m_sidecarContainers = (IDictionary<string, ExpressionValue<string>>) new Dictionary<string, ExpressionValue<string>>(targetToClone.SidecarContainers, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    }

    [DataMember(EmitDefaultValue = false)]
    [JsonConverter(typeof (AgentQueueTarget.QueueJsonConverter))]
    public AgentQueueReference Queue { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public JObject AgentSpecification { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public ParallelExecutionOptions Execution { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public WorkspaceOptions Workspace { get; set; }

    [DataMember(EmitDefaultValue = false)]
    [JsonConverter(typeof (ExpressionValueJsonConverter<string>))]
    public ExpressionValue<string> Container { get; set; }

    public IDictionary<string, ExpressionValue<string>> SidecarContainers
    {
      get
      {
        if (this.m_sidecarContainers == null)
          this.m_sidecarContainers = (IDictionary<string, ExpressionValue<string>>) new Dictionary<string, ExpressionValue<string>>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        return this.m_sidecarContainers;
      }
    }

    public override PhaseTarget Clone() => (PhaseTarget) new AgentQueueTarget(this);

    public override bool IsValid(TaskDefinition task)
    {
      ArgumentUtility.CheckForNull<TaskDefinition>(task, nameof (task));
      return task.RunsOn.Contains<string>("Agent", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    }

    internal AgentQueueTarget Evaluate(JobExecutionContext context, ValidationResult result)
    {
      string empty = string.Empty;
      string str1;
      try
      {
        str1 = context.ExpandVariables(this.Queue?.Name?.GetValue((IPipelineContext) context).Value, false);
      }
      catch (ExpressionException ex)
      {
        result.Errors.Add(new PipelineValidationError(ex.Message));
        return (AgentQueueTarget) null;
      }
      AgentQueueTarget agentQueueTarget1 = this.Clone() as AgentQueueTarget;
      JObject agentSpecification = this.AgentSpecification;
      if (agentSpecification != null)
      {
        agentSpecification = context.Evaluate(this.AgentSpecification).Value;
        agentQueueTarget1.AgentSpecification = agentSpecification;
      }
      if (string.IsNullOrEmpty(str1) && agentSpecification != null)
      {
        JToken jtoken;
        agentSpecification.TryGetValue("vmImage", out jtoken);
        if (jtoken != null && jtoken.Type == JTokenType.String)
        {
          string str2;
          if (!context.ExecutionOptions.RunJobsWithDemandsOnSingleHostedPool && this.Demands.Count > 0)
          {
            string str3 = AgentQueueTarget.PoolNameForVMImage(jtoken.Value<string>());
            if (str3 != null)
              agentSpecification.Remove("vmImage");
            if (str3 == null)
              str3 = "Azure Pipelines";
            str2 = str3;
          }
          else
            str2 = "Azure Pipelines";
          AgentQueueTarget agentQueueTarget2 = agentQueueTarget1;
          AgentQueueReference agentQueueReference = new AgentQueueReference();
          agentQueueReference.Name = (ExpressionValue<string>) str2;
          agentQueueTarget2.Queue = agentQueueReference;
        }
      }
      else
        agentQueueTarget1.Queue.Name = (ExpressionValue<string>) str1;
      return agentQueueTarget1;
    }

    internal static bool IsProbablyExpressionOrMacro(string s) => ExpressionValue.IsExpression(s) || VariableUtility.IsVariable(s);

    internal bool IsLiteral()
    {
      AgentQueueReference queue = this.Queue;
      if (queue != null)
      {
        ExpressionValue<string> name = queue.Name;
        if (name != (ExpressionValue<string>) null && (!name.IsLiteral || VariableUtility.IsVariable(name.Literal)))
          return false;
      }
      JObject agentSpecification = this.AgentSpecification;
      return agentSpecification == null || IsLiteral(agentSpecification);

      static bool IsLiteral(JObject o)
      {
        foreach (KeyValuePair<string, JToken> keyValuePair in o)
        {
          switch (keyValuePair.Value.Type)
          {
            case JTokenType.Object:
              if (!IsLiteral(keyValuePair.Value.Value<JObject>()))
                return false;
              continue;
            case JTokenType.String:
              if (AgentQueueTarget.IsProbablyExpressionOrMacro(keyValuePair.Value.Value<string>()))
                return false;
              continue;
            default:
              continue;
          }
        }
        return true;
      }
    }

    internal static string PoolNameForVMImage(string vmImageValue)
    {
      string upperInvariant = (vmImageValue ?? string.Empty).ToUpperInvariant();
      if (upperInvariant != null)
      {
        switch (upperInvariant.Length)
        {
          case 7:
            if (upperInvariant == "WIN1803")
              goto label_32;
            else
              goto label_32;
          case 11:
            switch (upperInvariant[5])
            {
              case ' ':
                switch (upperInvariant)
                {
                  case "MACOS 10.14":
                    break;
                  default:
                    goto label_32;
                }
                break;
              case '-':
                switch (upperInvariant)
                {
                  case "MACOS-10.14":
                    break;
                  default:
                    goto label_32;
                }
                break;
              default:
                goto label_32;
            }
            return "Hosted macOS";
          case 12:
            switch (upperInvariant[5])
            {
              case ' ':
                if (upperInvariant == "MACOS LATEST")
                  goto label_32;
                else
                  goto label_32;
              case '-':
                if (upperInvariant == "MACOS-LATEST")
                  goto label_32;
                else
                  goto label_32;
              case 'U':
                switch (upperInvariant)
                {
                  case "UBUNTU 16.04":
                  case "UBUNTU-16.04":
                    return "Hosted Ubuntu 1604";
                  default:
                    goto label_32;
                }
              case 'W':
                if (upperInvariant == "WINDOWS-2019")
                  goto label_30;
                else
                  goto label_32;
              default:
                goto label_32;
            }
          case 13:
            switch (upperInvariant[6])
            {
              case ' ':
                if (upperInvariant == "UBUNTU LATEST")
                  goto label_32;
                else
                  goto label_32;
              case '-':
                if (upperInvariant == "UBUNTU-LATEST")
                  goto label_32;
                else
                  goto label_32;
              default:
                goto label_32;
            }
          case 14:
            switch (upperInvariant[7])
            {
              case ' ':
                if (upperInvariant == "WINDOWS LATEST")
                  goto label_30;
                else
                  goto label_32;
              case '-':
                if (upperInvariant == "WINDOWS-LATEST")
                  goto label_30;
                else
                  goto label_32;
              case 'W':
                if (upperInvariant == "VS2017-WIN2016")
                  break;
                goto label_32;
              default:
                goto label_32;
            }
            break;
          case 16:
            if (upperInvariant == "VS2015-WIN2012R2")
              goto label_32;
            else
              goto label_32;
          case 17:
            if (upperInvariant == "XCODE9-MACOS10.13")
              goto label_32;
            else
              goto label_32;
          case 18:
            if (upperInvariant == "XCODE10-MACOS10.13")
              goto label_32;
            else
              goto label_32;
          case 19:
            switch (upperInvariant[7])
            {
              case ' ':
                if (upperInvariant == "WINDOWS SERVER 1803")
                  goto label_32;
                else
                  goto label_32;
              case '-':
                if (upperInvariant == "WINDOWS-2019-VS2019")
                  goto label_30;
                else
                  goto label_32;
              default:
                goto label_32;
            }
          case 22:
            if (upperInvariant == "XCODE 9 ON MACOS 10.13")
              goto label_32;
            else
              goto label_32;
          case 23:
            if (upperInvariant == "XCODE 10 ON MACOS 10.13")
              goto label_32;
            else
              goto label_32;
          case 41:
            if (upperInvariant == "VISUAL STUDIO 2017 ON WINDOWS SERVER 2016")
              break;
            goto label_32;
          case 43:
            if (upperInvariant == "VISUAL STUDIO 2015 ON WINDOWS SERVER 2012R2")
              goto label_32;
            else
              goto label_32;
          default:
            goto label_32;
        }
        return "Hosted VS2017";
label_30:
        return "Hosted Windows 2019 with VS2019";
      }
label_32:
      return (string) null;
    }

    private TaskAgentPoolReference ValidateQueue(
      IPipelineContext context,
      ValidationResult result,
      BuildOptions buildOptions)
    {
      int num = 0;
      string str = (string) null;
      bool flag = false;
      AgentQueueReference queue = this.Queue;
      if (queue != null)
      {
        num = queue.Id;
        ExpressionValue<string> name = queue.Name;
        if (name != (ExpressionValue<string>) null && (buildOptions.EnableResourceExpressions || name.IsLiteral))
        {
          try
          {
            str = name.GetValue(context).Value;
            flag = !name.IsLiteral && string.IsNullOrEmpty(str);
          }
          catch (Exception ex)
          {
            str = (string) null;
            if (buildOptions.ValidateExpressions)
            {
              if (buildOptions.ValidateResources)
                result.Errors.Add(new PipelineValidationError(ex.Message));
            }
          }
          if (buildOptions.EnableResourceExpressions && str != null && VariableUtility.IsVariable(str))
          {
            str = context.ExpandVariables(str);
            if (VariableUtility.IsVariable(str))
              flag = true;
          }
        }
      }
      if (flag || num == 0 && string.IsNullOrEmpty(str))
      {
        if (!buildOptions.AllowEmptyQueueTarget && buildOptions.ValidateResources && (!flag || buildOptions.ValidateExpressions))
        {
          if (!string.IsNullOrEmpty(str))
          {
            result.Errors.Add(new PipelineValidationError(PipelineStrings.QueueNotFoundByName((object) str)));
          }
          else
          {
            ExpressionValue<string> name = queue?.Name;
            if (name == (ExpressionValue<string>) null || name.IsLiteral)
              result.Errors.Add(new PipelineValidationError(PipelineStrings.QueueNotDefined()));
            else if (name != (ExpressionValue<string>) null)
              result.Errors.Add(new PipelineValidationError(PipelineStrings.QueueNotFoundByName((object) name.Expression)));
          }
        }
      }
      else
      {
        result.AddQueueReference(num, str);
        if (buildOptions.ValidateResources)
        {
          TaskAgentQueue taskAgentQueue = (TaskAgentQueue) null;
          IResourceStore resourceStore = context.ResourceStore;
          if (resourceStore != null)
          {
            if (num != 0)
            {
              taskAgentQueue = resourceStore.GetQueue(num);
              if (taskAgentQueue == null)
              {
                result.UnauthorizedResources.Queues.Add(new AgentQueueReference()
                {
                  Id = num
                });
                result.Errors.Add(new PipelineValidationError(PipelineStrings.QueueNotFound((object) num)));
              }
            }
            else if (!string.IsNullOrEmpty(str))
            {
              taskAgentQueue = resourceStore.GetQueue(str);
              if (taskAgentQueue == null)
              {
                ISet<AgentQueueReference> queues = result.UnauthorizedResources.Queues;
                AgentQueueReference agentQueueReference = new AgentQueueReference();
                agentQueueReference.Name = (ExpressionValue<string>) str;
                queues.Add(agentQueueReference);
                result.Errors.Add(new PipelineValidationError(PipelineStrings.QueueNotFoundByName((object) str)));
              }
            }
          }
          if (taskAgentQueue != null)
          {
            this.Queue.Id = taskAgentQueue.Id;
            return taskAgentQueue.Pool;
          }
        }
      }
      return (TaskAgentPoolReference) null;
    }

    internal override void Validate(
      IPipelineContext context,
      BuildOptions buildOptions,
      ValidationResult result,
      IList<Step> steps,
      ISet<Demand> taskDemands)
    {
      TaskAgentPoolReference agentPoolReference = this.ValidateQueue(context, result, buildOptions);
      bool flag1 = agentPoolReference == null || !agentPoolReference.IsHosted;
      bool flag2 = false;
      bool flag3 = false;
      int num1 = 0;
      int num2 = 0;
      bool flag4 = true;
      List<string> repositoryAliases = AgentQueueTarget.GetValidRepositoryAliases(context?.ResourceStore?.Repositories);
      for (int index = 0; index < steps.Count; ++index)
      {
        Step step1 = steps[index];
        if (step1.Type == StepType.Task)
        {
          TaskStep step2 = step1 as TaskStep;
          if (step2.Name.StartsWith("__system_"))
          {
            if (flag4)
              ++num2;
          }
          else if (step2.IsCheckoutTask())
          {
            flag4 = false;
            ++num1;
            if (context.EnvironmentVersion < 2)
            {
              if (index > 0 && index - num2 > 0)
                result.Errors.Add(new PipelineValidationError(PipelineStrings.CheckoutMustBeTheFirstStep()));
            }
            else if (index > 0)
              flag2 = true;
            string repository;
            if (step2.Inputs.TryGetValue(PipelineConstants.CheckoutTaskInputs.Repository, out repository) && !repositoryAliases.Contains<string>(repository, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase))
              result.Errors.Add(new PipelineValidationError(PipelineStrings.CheckoutStepRepositoryNotSupported((object) step2.Inputs[PipelineConstants.CheckoutTaskInputs.Repository])));
            if (this.IsCheckoutNone(repository))
              flag3 = true;
          }
          else
            flag4 = false;
        }
      }
      if (num1 > 1 & flag3)
        result.Errors.Add(new PipelineValidationError(PipelineStrings.CheckoutMultipleRepositoriesCannotIncludeNone()));
      if (num1 > 1)
        taskDemands.Add((Demand) AgentFeatureDemands.MultiRepoCheckoutDemand());
      if (flag2)
        taskDemands.Add((Demand) AgentFeatureDemands.AdvancedCheckoutDemand());
      DemandMinimumVersion demandMinimumVersion = AgentQueueTarget.ResolveAgentVersionDemand(this.Demands, AgentQueueTarget.ResolveAgentVersionDemand(taskDemands));
      if (this.Container != (ExpressionValue<string>) null)
        flag1 = false;
      if (flag1 && buildOptions.RollupStepDemands)
        this.Demands.UnionWith((IEnumerable<Demand>) taskDemands);
      if (demandMinimumVersion != null)
        this.Demands.Add((Demand) demandMinimumVersion);
      if (agentPoolReference == null || !agentPoolReference.IsHosted || agentPoolReference == null)
        return;
      bool? isLegacy = agentPoolReference.IsLegacy;
      bool flag5 = false;
      if (!(isLegacy.GetValueOrDefault() == flag5 & isLegacy.HasValue) || AgentSpecificationUtility.GetVMImageProperties(this).Count<string>() != 0)
        return;
      int num3 = context.FeatureFlags.GetValueOrDefault<string, bool>("DistributedTask.DoNotAddDefaultVmImageToExistingAgentSpecifcations") ? 1 : 0;
      bool valueOrDefault = context.FeatureFlags.GetValueOrDefault<string, bool>("DistributedTask.DoNotAddDefaultVmImageForNonYamlPipelines");
      bool flag6 = true;
      if (num3 != 0 && this.AgentSpecification != null)
        flag6 = false;
      if (valueOrDefault && demandMinimumVersion?.Source?.SourceName != "YAML Pipelines")
        flag6 = false;
      if (!flag6)
        return;
      if (this.AgentSpecification == null)
        this.AgentSpecification = new JObject();
      this.AgentSpecification[TaskAgentRequestConstants.VmImage] = (JToken) TaskAgentRequestConstants.DefaultVmImage;
    }

    private bool IsCheckoutNone(string repository) => string.Equals(repository, PipelineConstants.NoneAlias, StringComparison.OrdinalIgnoreCase);

    private static List<string> GetValidRepositoryAliases(IRepositoryStore repoStore)
    {
      List<string> repositoryAliases = new List<string>()
      {
        PipelineConstants.SelfAlias,
        PipelineConstants.NoneAlias,
        PipelineConstants.DesignerRepo
      };
      if (repoStore != null)
        repositoryAliases.AddRange(repoStore.GetAll().Select<RepositoryResource, string>((Func<RepositoryResource, string>) (x => x.Alias)));
      return repositoryAliases;
    }

    private static DemandMinimumVersion ResolveAgentVersionDemand(
      ISet<Demand> demands,
      DemandMinimumVersion currentMinimumVersion = null)
    {
      DemandMinimumVersion demandMinimumVersion = DemandMinimumVersion.MaxAndRemove(demands);
      return demandMinimumVersion != null && (currentMinimumVersion == null || DemandMinimumVersion.CompareVersion(demandMinimumVersion.Value, currentMinimumVersion.Value) > 0) ? demandMinimumVersion : currentMinimumVersion;
    }

    internal override JobExecutionContext CreateJobContext(
      PhaseExecutionContext context,
      string jobName,
      int attempt,
      bool continueOnError,
      int timeoutInMinutes,
      int cancelTimeoutInMinutes,
      IJobFactory jobFactory)
    {
      context.Trace?.EnterProperty(nameof (CreateJobContext));
      JobExecutionContext jobContext = (this.Execution ?? new ParallelExecutionOptions()).CreateJobContext(context, jobName, attempt, this.Container, this.SidecarContainers, continueOnError, timeoutInMinutes, cancelTimeoutInMinutes, jobFactory);
      context.Trace?.LeaveProperty(nameof (CreateJobContext));
      if (jobContext != null)
        jobContext.Job.Definition.Workspace = this.Workspace?.Clone();
      return jobContext;
    }

    internal override ExpandPhaseResult Expand(
      PhaseExecutionContext context,
      bool continueOnError,
      int timeoutInMinutes,
      int cancelTimeoutInMinutes,
      IJobFactory jobFactory,
      JobExpansionOptions options)
    {
      context.Trace?.EnterProperty(nameof (Expand));
      ExpandPhaseResult expandPhaseResult = (this.Execution ?? new ParallelExecutionOptions()).Expand(context, this.Container, this.SidecarContainers, continueOnError, timeoutInMinutes, cancelTimeoutInMinutes, jobFactory, options);
      context.Trace?.LeaveProperty(nameof (Expand));
      foreach (JobInstance job in (IEnumerable<JobInstance>) expandPhaseResult.Jobs)
        job.Definition.Workspace = this.Workspace?.Clone();
      return expandPhaseResult;
    }

    [System.Runtime.Serialization.OnSerializing]
    private void OnSerializing(StreamingContext context)
    {
      IDictionary<string, ExpressionValue<string>> sidecarContainers = this.m_sidecarContainers;
      if ((sidecarContainers != null ? (sidecarContainers.Count == 0 ? 1 : 0) : 0) == 0)
        return;
      this.m_sidecarContainers = (IDictionary<string, ExpressionValue<string>>) null;
    }

    private sealed class QueueJsonConverter : VssSecureJsonConverter
    {
      public override bool CanWrite => false;

      public override bool CanConvert(System.Type objectType) => objectType.Equals(typeof (AgentQueueReference));

      public override object ReadJson(
        JsonReader reader,
        System.Type objectType,
        object existingValue,
        JsonSerializer serializer)
      {
        using (JsonReader reader1 = JObject.Load(reader).CreateReader())
        {
          AgentQueueReference target = new AgentQueueReference();
          serializer.Populate(reader1, (object) target);
          return (object) target;
        }
      }

      public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) => throw new NotImplementedException();
    }
  }
}
