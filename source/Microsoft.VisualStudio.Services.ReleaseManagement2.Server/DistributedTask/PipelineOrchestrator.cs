// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.DistributedTask.PipelineOrchestrator
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server;
using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Releases;
using Microsoft.TeamFoundation.DistributedTask.Pipelines;
using Microsoft.TeamFoundation.DistributedTask.Pipelines.Server;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.DistributedTask
{
  public class PipelineOrchestrator : DistributedTaskOrchestrator
  {
    public PipelineOrchestrator(IVssRequestContext requestContext, Guid projectId)
      : base(requestContext, projectId)
    {
    }

    public override string TaskHubName => "Release";

    public override IOrchestrationProcess CreateTaskOrchestrationContainer(
      AutomationEngineInput input)
    {
      throw new NotImplementedException();
    }

    public override Guid StartDeployment(AutomationEngineInput input)
    {
      TaskOrchestrationOwner orchestrationOwner = input != null ? this.GetDefinitionReference(input) : throw new ArgumentNullException(nameof (input));
      TaskOrchestrationOwner ownerReference1 = this.GetOwnerReference(input);
      Guid guid = Guid.NewGuid();
      Uri deployPhaseUri = ReleaseArtifactCreator.CreateDeployPhaseUri(new ReleaseArtifact()
      {
        ProjectId = this.ProjectId,
        ReleaseId = input.ReleaseId,
        EnvironmentId = input.EnvironmentId,
        ReleaseStepId = input.ReleaseStepId,
        ReleaseDeployPhaseId = input.ReleaseDeployPhaseId
      });
      PipelineBuildResult pipelineBuildResult = this.BuildPipelineProcess(input);
      if (!this.RequestContext.IsFeatureEnabled("VisualStudio.ReleaseManagement.PipelineOrchestrator.DisableReleaseIdCustomClaim"))
        pipelineBuildResult.Environment.Options.SystemTokenCustomClaims.Add(ReleaseClaimHelper.GetReleaseIdClaim(this.ProjectId, input.ReleaseId));
      string planGroupName = this.GetPlanGroupName(input.ReleaseId);
      bool flag = !this.RequestContext.IsFeatureEnabled("VisualStudio.ReleaseManagement.PipelineOrchestrator.SkipResourceValidation");
      IVssRequestContext vssRequestContext = this.RequestContext.Elevate();
      TaskHub taskHub = vssRequestContext.GetService<IDistributedTaskHubService>().GetTaskHub(vssRequestContext, this.TaskHubName);
      BuildOptions buildOptions = new BuildOptions()
      {
        ValidateResources = flag
      };
      IVssRequestContext requestContext = vssRequestContext;
      TaskAgentPoolReference pool = new TaskAgentPoolReference();
      Guid projectId = this.ProjectId;
      Guid planId = guid;
      string planGroup = planGroupName;
      Uri artifactUri = deployPhaseUri;
      PipelineEnvironment environment = pipelineBuildResult.Environment;
      Microsoft.TeamFoundation.DistributedTask.Pipelines.PipelineProcess process = pipelineBuildResult.Process;
      BuildOptions validationOptions = buildOptions;
      Guid requestedForId = input.RequestedForId;
      TaskOrchestrationOwner definitionReference = orchestrationOwner;
      TaskOrchestrationOwner ownerReference2 = ownerReference1;
      return taskHub.RunPlan(requestContext, pool, projectId, planId, planGroup, PlanTemplateType.Designer, artifactUri, (IOrchestrationEnvironment) environment, (IOrchestrationProcess) process, validationOptions, requestedForId, definitionReference, ownerReference2).PlanId;
    }

    public override Dictionary<Guid, string> GetJobIdNameMap(
      IEnumerable<TimelineRecord> timelineRecords)
    {
      Dictionary<Guid, string> jobIdNameMap = new Dictionary<Guid, string>();
      foreach (TimelineRecord timelineRecord in timelineRecords.Where<TimelineRecord>((Func<TimelineRecord, bool>) (x => x.RecordType == "Job")))
        jobIdNameMap[timelineRecord.Id] = timelineRecord.Name;
      return jobIdNameMap;
    }

    private static bool HasServerPhase(Microsoft.TeamFoundation.DistributedTask.Pipelines.PipelineProcess process) => process.Stages.SelectMany<Stage, PhaseNode>((Func<Stage, IEnumerable<PhaseNode>>) (x => (IEnumerable<PhaseNode>) x.Phases)).Any<PhaseNode>((Func<PhaseNode, bool>) (x => (x is Phase phase ? phase.Target : (PhaseTarget) null) is ServerTarget));

    private PipelineBuildResult BuildPipelineProcess(AutomationEngineInput input)
    {
      PipelineResources authorizedResources = new PipelineResources();
      PipelineBuilder builder = this.RequestContext.GetService<IPipelineBuilderService>().GetBuilder(this.RequestContext, this.ProjectId, "Release", 0, authorizedResources, true);
      foreach (KeyValuePair<string, string> systemVariable in (IEnumerable<KeyValuePair<string, string>>) this.GetSystemVariables(input))
        builder.SystemVariables.Add(systemVariable.Key, (VariableValue) systemVariable.Value);
      foreach (KeyValuePair<string, string> requestorVariable in (IEnumerable<KeyValuePair<string, string>>) this.GetPlanEnvironmentRequestorVariables(input))
        builder.SystemVariables.Add(requestorVariable.Key, (VariableValue) requestorVariable.Value);
      this.SetAdditionalSystemVariables(input, builder);
      ReleaseSecretsService service = this.RequestContext.GetService<ReleaseSecretsService>();
      bool flag1 = PipelineOrchestrator.HasServerPhase((Microsoft.TeamFoundation.DistributedTask.Pipelines.PipelineProcess) input.Process);
      IVssRequestContext requestContext = this.RequestContext;
      Guid projectId = this.ProjectId;
      int releaseId = input.ReleaseId;
      int environmentId = input.EnvironmentId;
      int releaseDeployPhaseId = input.ReleaseDeployPhaseId;
      int num = flag1 ? 1 : 0;
      IDictionary<string, string> secretVariables = service.GetSecretVariables(requestContext, projectId, releaseId, environmentId, releaseDeployPhaseId, num != 0);
      foreach (KeyValuePair<string, ConfigurationVariableValue> variable1 in input.Variables)
      {
        Variable variable2 = (Variable) null;
        string str;
        if (variable1.Value.IsSecret && secretVariables.TryGetValue(variable1.Key, out str))
          variable2 = new Variable()
          {
            Name = variable1.Key,
            Value = str,
            Secret = true
          };
        else if (!variable1.Value.IsSecret)
          variable2 = new Variable()
          {
            Name = variable1.Key,
            Value = variable1.Value.Value,
            Secret = false
          };
        if (variable2 != null)
          builder.UserVariables.Add((IVariable) variable2);
      }
      Microsoft.TeamFoundation.DistributedTask.Pipelines.PipelineProcess process = (Microsoft.TeamFoundation.DistributedTask.Pipelines.PipelineProcess) input.Process;
      bool flag2 = this.RequestContext.IsFeatureEnabled("VisualStudio.ReleaseManagement.PipelineOrchestrator.ValidateTaskInputs");
      PipelineBuildResult pipelineBuildResult = builder.Build(process.Stages, new BuildOptions()
      {
        ValidateTaskInputs = flag2
      });
      pipelineBuildResult.Environment.Resources.MergeWith(pipelineBuildResult.ReferencedResources);
      return pipelineBuildResult;
    }

    private void SetAdditionalSystemVariables(AutomationEngineInput input, PipelineBuilder builder)
    {
      string empty;
      if (!input.Data.TryGetValue("release.definitionId", out empty))
        empty = string.Empty;
      string str = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}_{1}_{2}_{3}_{4}_{5}_{6}", (object) (this.RequestContext.ExecutionEnvironment.IsHostedDeployment ? "VSTS" : "TFS"), (object) this.RequestContext.ServiceHost.InstanceId, (object) this.TaskHubName, (object) empty, (object) input.ReleaseId, (object) input.EnvironmentId, (object) input.AttemptNumber);
      if (!string.IsNullOrEmpty(str))
      {
        builder.SystemVariables.Add(WellKnownDistributedTaskVariables.AzureUserAgent, (VariableValue) str);
        builder.SystemVariables.Add(WellKnownDistributedTaskVariables.MsDeployUserAgent, (VariableValue) str);
      }
      string parallelismTag = ArtifactExtensions.GetParallelismTag(this.RequestContext, this.ProjectId, input.Artifacts);
      builder.SystemVariables.Add(WellKnownDistributedTaskVariables.JobParallelismTag, (VariableValue) parallelismTag);
      string name;
      if (!input.Data.TryGetValue("ReleaseEnvironmentTriggerReason", out name))
        name = Enum.GetName(typeof (DeploymentReason), (object) DeploymentReason.None);
      bool flag = name.Equals(Enum.GetName(typeof (DeploymentReason), (object) DeploymentReason.Scheduled), StringComparison.OrdinalIgnoreCase);
      builder.SystemVariables.Add(WellKnownDistributedTaskVariables.IsScheduled, (VariableValue) flag.ToString());
    }
  }
}
