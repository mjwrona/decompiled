// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.DistributedTask.DistributedTaskOrchestrator
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server;
using Microsoft.TeamFoundation.DistributedTask.Pipelines;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Common.Helpers;
using Microsoft.VisualStudio.Services.ReleaseManagement.Common.Security;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.DistributedTask
{
  public abstract class DistributedTaskOrchestrator : IDistributedTaskOrchestrator
  {
    public const int TracePoint = 1950021;
    private readonly Func<int, Uri> releaseUriCreator;
    private readonly Func<int, Uri> releaseEnvironmentUriCreator;
    private readonly Func<IVssRequestContext, string, Guid, Guid, string, TaskAgentPoolReference, Uri, IOrchestrationEnvironment, PlanTemplateType, IOrchestrationProcess, BuildOptions, Guid, TaskOrchestrationOwner, TaskOrchestrationOwner, Guid> runPlanFunc;
    private readonly Action<IVssRequestContext, string, Guid, Guid, TimeSpan> cancelPlanAction;
    private readonly Func<IVssRequestContext, Guid, bool, IList<string>> getEmailAddressesFunc;
    private const int JobExecutionMaxTimeoutInMinutes = 2880;

    public static IDistributedTaskOrchestrator CreateDistributedTaskOrchestrator(
      IVssRequestContext requestContext,
      Guid projectId,
      DeployPhaseTypes phaseType)
    {
      bool flag = requestContext.IsFeatureEnabled("VisualStudio.ReleaseManagement.UsePipelineOrchestrator.Phase");
      switch (phaseType)
      {
        case DeployPhaseTypes.AgentBasedDeployment:
          return flag ? (IDistributedTaskOrchestrator) new PipelineOrchestrator(requestContext, projectId) : (IDistributedTaskOrchestrator) new AgentPoolBasedOrchestrator(requestContext, projectId);
        case DeployPhaseTypes.RunOnServer:
          return (IDistributedTaskOrchestrator) new ServerExecutionOrchestrator(requestContext, projectId);
        case DeployPhaseTypes.MachineGroupBasedDeployment:
          return (IDistributedTaskOrchestrator) new MachinePoolBasedOrchestrator(requestContext, projectId);
        case DeployPhaseTypes.DeploymentGates:
          return (IDistributedTaskOrchestrator) new GreenlightingOrchestrator(requestContext, projectId);
        default:
          throw new NotSupportedException();
      }
    }

    protected IVssRequestContext RequestContext { get; }

    protected Guid ProjectId { get; }

    protected DistributedTaskOrchestrator()
    {
    }

    protected DistributedTaskOrchestrator(IVssRequestContext requestContext, Guid projectId)
      : this(requestContext, projectId, new Func<int, Uri>(new ArtifactUriCreator().CreateReleaseUri), new Func<int, Uri>(new ArtifactUriCreator().CreateReleaseEnvironmentUri), (Func<IVssRequestContext, string, Guid, Guid, string, TaskAgentPoolReference, Uri, IOrchestrationEnvironment, PlanTemplateType, IOrchestrationProcess, BuildOptions, Guid, TaskOrchestrationOwner, TaskOrchestrationOwner, Guid>) ((context, taskhubName, projectId1, planId, planGroup, taskAgentPoolReference, artifactUri, planEnvironment, planTemplateType, orchestrationProcess, validationOptions, requestedForId, definitionReference, ownerReference) => DistributedTaskOrchestrator.RunPlan(context, taskhubName, projectId1, planId, planGroup, taskAgentPoolReference, artifactUri, planEnvironment, planTemplateType, orchestrationProcess, validationOptions, requestedForId, definitionReference, ownerReference)), DistributedTaskOrchestrator.\u003C\u003EO.\u003C0\u003E__CancelPlan ?? (DistributedTaskOrchestrator.\u003C\u003EO.\u003C0\u003E__CancelPlan = new Action<IVssRequestContext, string, Guid, Guid, TimeSpan>(DistributedTaskOrchestrator.CancelPlan)), DistributedTaskOrchestrator.\u003C\u003EO.\u003C1\u003E__GetEmailAddresses ?? (DistributedTaskOrchestrator.\u003C\u003EO.\u003C1\u003E__GetEmailAddresses = new Func<IVssRequestContext, Guid, bool, IList<string>>(RmIdentityHelper.GetEmailAddresses)))
    {
      // ISSUE: reference to a compiler-generated field (out of statement scope)
      // ISSUE: reference to a compiler-generated field (out of statement scope)
      // ISSUE: reference to a compiler-generated field (out of statement scope)
      // ISSUE: reference to a compiler-generated field (out of statement scope)
    }

    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is by design")]
    protected DistributedTaskOrchestrator(
      IVssRequestContext requestContext,
      Guid projectId,
      Func<int, Uri> releaseUriCreatorFunc,
      Func<int, Uri> releaseEnvironmentUriCreatorFunc,
      Func<IVssRequestContext, string, Guid, Guid, string, TaskAgentPoolReference, Uri, IOrchestrationEnvironment, PlanTemplateType, IOrchestrationProcess, BuildOptions, Guid, TaskOrchestrationOwner, TaskOrchestrationOwner, Guid> runPlanFunc,
      Action<IVssRequestContext, string, Guid, Guid, TimeSpan> cancelPlanAction,
      Func<IVssRequestContext, Guid, bool, IList<string>> getEmailAddressesFunc)
    {
      this.RequestContext = requestContext;
      this.ProjectId = projectId;
      this.releaseUriCreator = releaseUriCreatorFunc;
      this.releaseEnvironmentUriCreator = releaseEnvironmentUriCreatorFunc;
      this.runPlanFunc = runPlanFunc;
      this.cancelPlanAction = cancelPlanAction;
      this.getEmailAddressesFunc = getEmailAddressesFunc;
    }

    public abstract string TaskHubName { get; }

    public virtual PlanTemplateType PlanTemplateType => PlanTemplateType.Designer;

    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", Justification = "This is not shown to the user and is for tracing")]
    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "This is not shown to the user and is for tracing")]
    public virtual Guid StartDeployment(AutomationEngineInput input)
    {
      if (input == null)
        throw new ArgumentNullException(nameof (input));
      this.TraceInformation("DistributedTaskOrchestrator: Starting orchestration for ReleaseId: {0}, ReleaseStepId: {1}, ReleaseDeployPhaseId: {2}, ProjectId: {3}", (object) input.ReleaseId, (object) input.ReleaseStepId, (object) input.ReleaseDeployPhaseId, (object) this.ProjectId);
      try
      {
        TaskAgentPoolReference agentPoolReference = this.GetTaskAgentPoolReference(input);
        Uri deployPhaseUri = ReleaseArtifactCreator.CreateDeployPhaseUri(new ReleaseArtifact()
        {
          ProjectId = this.ProjectId,
          ReleaseId = input.ReleaseId,
          EnvironmentId = input.EnvironmentId,
          ReleaseStepId = input.ReleaseStepId,
          ReleaseDeployPhaseId = input.ReleaseDeployPhaseId
        });
        this.RequestContext.Trace(1950021, TraceLevel.Verbose, "ReleaseManagementService", "DistributedTask", "DistributedTaskOrchestrator: Constructing GetTaskAgentPoolReference with Id {0}", (object) agentPoolReference.Id);
        IOrchestrationEnvironment environment = this.GetEnvironment(input);
        IOrchestrationProcess orchestrationContainer = this.CreateTaskOrchestrationContainer(input);
        Guid deploymentRequestedForId = input.ReleaseDeploymentRequestedForId;
        string planGroupName = this.GetPlanGroupName(input.ReleaseId);
        TaskOrchestrationOwner definitionReference = this.GetDefinitionReference(input);
        TaskOrchestrationOwner ownerReference = this.GetOwnerReference(input);
        Guid guid = this.runPlanFunc(this.RequestContext, this.TaskHubName, this.ProjectId, Guid.NewGuid(), planGroupName, agentPoolReference, deployPhaseUri, environment, this.PlanTemplateType, orchestrationContainer, new BuildOptions(), deploymentRequestedForId, definitionReference, ownerReference);
        this.TraceInformation("DistributedTaskOrchestrator: orchestration started for ReleaseId: {0}, ReleaseStepId: {1}, ReleaseDeployPhaseId: {2}, ProjectId: {3}, RunPlan: {4}", (object) input.ReleaseId, (object) input.ReleaseStepId, (object) input.ReleaseDeployPhaseId, (object) this.ProjectId, (object) guid);
        return guid;
      }
      catch (Exception ex)
      {
        this.TraceError("DistributedTaskOrchestrator: Start orchestration failed for ReleaseId: {0}, ReleaseStepId: {1}, ReleaseDeployPhaseId: {2}, with exception: {3}", (object) input.ReleaseId, (object) input.ReleaseStepId, (object) input.ReleaseDeployPhaseId, (object) ex.ToString());
        throw;
      }
    }

    public virtual bool CancelDeployment(Guid planId, TimeSpan jobCancelTimeout)
    {
      if (planId == Guid.Empty)
        return false;
      if (jobCancelTimeout.TotalMinutes < 1.0 || jobCancelTimeout.TotalMinutes > 60.0)
        jobCancelTimeout = TimeSpan.FromMinutes(1.0);
      this.cancelPlanAction(this.RequestContext, this.TaskHubName, this.ProjectId, planId, jobCancelTimeout);
      return true;
    }

    public virtual Dictionary<Guid, string> GetJobIdNameMap(
      IEnumerable<TimelineRecord> timelineRecords)
    {
      return new Dictionary<Guid, string>();
    }

    public IEnumerable<TimelineRecord> GetTimelineRecords(Guid runPlanId)
    {
      Timeline timeline = DistributedTaskOrchestrator.GetTaskHub(this.RequestContext, this.TaskHubName).GetTimeline(this.RequestContext.Elevate(), this.ProjectId, runPlanId, Guid.Empty, includeRecords: true);
      return timeline != null ? (IEnumerable<TimelineRecord>) timeline.Records : (IEnumerable<TimelineRecord>) new List<TimelineRecord>();
    }

    public IEnumerable<TaskLog> GetLogs(Guid runPlanId) => DistributedTaskOrchestrator.GetTaskHub(this.RequestContext, this.TaskHubName).GetLogs(this.RequestContext.Elevate(), this.ProjectId, runPlanId);

    public TaskLog GetLog(Guid runPlanId, int logId) => DistributedTaskOrchestrator.GetTaskHub(this.RequestContext, this.TaskHubName).GetLog(this.RequestContext.Elevate(), this.ProjectId, runPlanId, logId);

    public TeamFoundationDataReader GetLoglines(
      Guid planId,
      int logId,
      ref long startLine,
      ref long endLine,
      out long totalLines)
    {
      TaskHub taskHub = DistributedTaskOrchestrator.GetTaskHub(this.RequestContext, this.TaskHubName);
      ISecuredObject securedObject1 = this.RequestContext.GetSecuredObject();
      IVssRequestContext requestContext = this.RequestContext.Elevate();
      Guid projectId = this.ProjectId;
      Guid planId1 = planId;
      int logId1 = logId;
      ISecuredObject securedObject2 = securedObject1;
      ref long local1 = ref startLine;
      ref long local2 = ref endLine;
      ref long local3 = ref totalLines;
      return taskHub.GetLogLines(requestContext, projectId, planId1, logId1, securedObject2, ref local1, ref local2, out local3);
    }

    public IEnumerable<TaskAttachment> GetAttachments(Guid planId, string type) => (IEnumerable<TaskAttachment>) DistributedTaskOrchestrator.GetTaskHub(this.RequestContext, this.TaskHubName).GetAttachments(this.RequestContext.Elevate(), this.ProjectId, planId, type);

    public Stream GetAttachment(Guid planId, TaskAttachment attachment)
    {
      if (attachment == null)
        throw new ArgumentNullException(nameof (attachment));
      return DistributedTaskOrchestrator.GetTaskHub(this.RequestContext, this.TaskHubName).GetAttachment(this.RequestContext.Elevate(), this.ProjectId, planId, attachment.TimelineId, attachment.RecordId, attachment.Type, attachment.Name);
    }

    public abstract IOrchestrationProcess CreateTaskOrchestrationContainer(
      AutomationEngineInput input);

    public virtual TaskAgentPoolReference GetTaskAgentPoolReference(AutomationEngineInput input) => new TaskAgentPoolReference();

    public virtual int GetJobExecutionTimeout(int timeoutInMinutes) => timeoutInMinutes <= 0 ? 2880 : timeoutInMinutes;

    public IList<OutputVariableScope> GetOutputVariables(IList<Guid> planIds)
    {
      if (planIds == null)
        throw new ArgumentNullException(nameof (planIds));
      List<OutputVariableScope> outputVariables = new List<OutputVariableScope>();
      TaskHub taskHub = DistributedTaskOrchestrator.GetTaskHub(this.RequestContext, this.TaskHubName);
      foreach (Guid planId in (IEnumerable<Guid>) planIds)
      {
        OutputVariableScope planOutputVariables = taskHub.GetPlanOutputVariables(this.RequestContext, this.ProjectId, planId);
        if (planOutputVariables != null)
          outputVariables.Add(planOutputVariables);
      }
      return (IList<OutputVariableScope>) outputVariables;
    }

    public virtual IOrchestrationEnvironment GetEnvironment(AutomationEngineInput input)
    {
      if (input == null)
        throw new ArgumentNullException(nameof (input));
      PlanEnvironment environment = new PlanEnvironment();
      if (input.Variables != null)
      {
        foreach (KeyValuePair<string, ConfigurationVariableValue> variable in input.Variables)
        {
          if (variable.Value.IsSecret)
            environment.MaskHints.Add(new MaskHint()
            {
              Type = MaskType.Variable,
              Value = variable.Key
            });
          environment.Variables[variable.Key] = variable.Value.Value;
        }
      }
      foreach (KeyValuePair<string, string> systemVariable in (IEnumerable<KeyValuePair<string, string>>) this.GetSystemVariables(input))
        environment.Variables[systemVariable.Key] = systemVariable.Value;
      foreach (KeyValuePair<string, string> requestorVariable in (IEnumerable<KeyValuePair<string, string>>) this.GetPlanEnvironmentRequestorVariables(input))
        environment.Variables[requestorVariable.Key] = requestorVariable.Value;
      return (IOrchestrationEnvironment) environment;
    }

    protected static TaskHub GetTaskHub(IVssRequestContext context, string taskHubName) => context.GetService<IDistributedTaskHubService>().GetTaskHub(context, taskHubName);

    protected static DeploymentInput GetAgentDeploymentInput(
      DeployPhaseSnapshot phaseData,
      IDictionary<string, ConfigurationVariableValue> variables)
    {
      DeploymentInput deploymentInput = (DeploymentInput) phaseData.GetDeploymentInput(variables);
      deploymentInput.NormalizeArtifactsDownloadInput(phaseData);
      return deploymentInput;
    }

    protected IDictionary<string, string> GetPlanEnvironmentRequestorVariables(
      AutomationEngineInput input)
    {
      if (input == null)
        throw new ArgumentNullException(nameof (input));
      Dictionary<string, string> requestorVariables = new Dictionary<string, string>();
      string empty1 = string.Empty;
      string empty2 = string.Empty;
      IdentityRef identityRef1 = (IdentityRef) null;
      IdentityRef identityRef2 = (IdentityRef) null;
      List<string> identityIds = new List<string>();
      Guid guid = input.RequestedForId;
      identityIds.Add(guid.ToString());
      guid = input.ReleaseDeploymentRequestedForId;
      identityIds.Add(guid.ToString());
      IDictionary<string, IdentityRef> dictionary = identityIds.QueryIdentities(this.RequestContext, false);
      guid = input.RequestedForId;
      dictionary.TryGetValue(guid.ToString(), out identityRef1);
      guid = input.ReleaseDeploymentRequestedForId;
      dictionary.TryGetValue(guid.ToString(), out identityRef2);
      requestorVariables["release.requestedFor"] = identityRef1?.DisplayName ?? string.Empty;
      requestorVariables["release.deployment.requestedFor"] = identityRef2?.DisplayName ?? string.Empty;
      string str1 = this.getEmailAddressesFunc(this.RequestContext, input.RequestedForId, false).FirstOrDefault<string>() ?? string.Empty;
      string str2 = this.getEmailAddressesFunc(this.RequestContext, input.ReleaseDeploymentRequestedForId, false).FirstOrDefault<string>() ?? string.Empty;
      requestorVariables["release.requestedForEmail"] = str1;
      requestorVariables["release.deployment.requestedForEmail"] = str2;
      return (IDictionary<string, string>) requestorVariables;
    }

    public IDictionary<string, string> GetSystemVariables(AutomationEngineInput input)
    {
      if (input == null)
        throw new ArgumentNullException(nameof (input));
      Dictionary<string, string> systemVariables = new Dictionary<string, string>();
      string str1;
      input.Data.TryGetValue("ReleaseName", out str1);
      string str2;
      input.Data.TryGetValue("ReleaseReason", out str2);
      string str3;
      input.Data.TryGetValue("ReleaseDefinitionName", out str3);
      string str4;
      input.Data.TryGetValue("ReleaseDefinitionId", out str4);
      string str5;
      input.Data.TryGetValue("ReleaseDescription", out str5);
      string str6;
      input.Data.TryGetValue("ReleaseEnvironmentName", out str6);
      string str7;
      input.Data.TryGetValue("ReleaseDefinitionEnvironmentId", out str7);
      string str8;
      input.Data.TryGetValue("DeploymentId", out str8);
      string str9;
      input.Data.TryGetValue("DeploymentStartTime", out str9);
      systemVariables["release.releaseId"] = input.ReleaseId.ToString((IFormatProvider) CultureInfo.InvariantCulture);
      systemVariables["release.releaseName"] = str1;
      systemVariables["release.releaseDescription"] = str5;
      systemVariables["release.reason"] = str2;
      systemVariables["release.definitionName"] = str3;
      systemVariables["release.definitionId"] = str4;
      systemVariables["release.definitionEnvironmentId"] = str7;
      systemVariables["release.deploymentId"] = str8;
      systemVariables["release.deployment.startTime"] = str9;
      systemVariables["release.releaseUri"] = this.releaseUriCreator(input.ReleaseId).AbsoluteUri;
      systemVariables["release.environmentUri"] = this.releaseEnvironmentUriCreator(input.EnvironmentId).AbsoluteUri;
      int num = input.EnvironmentId;
      systemVariables["release.environmentId"] = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
      num = input.AttemptNumber;
      systemVariables["release.attemptNumber"] = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
      systemVariables["release.environmentName"] = str6;
      systemVariables["release.deployPhaseId"] = input.ReleaseDeployPhaseId.ToString((IFormatProvider) CultureInfo.InvariantCulture);
      Guid guid = input.RequestedForId;
      systemVariables["release.requestedForId"] = guid.ToString("D");
      guid = input.RequestedForId;
      systemVariables["requestedForId"] = guid.ToString("D");
      guid = input.ReleaseDeploymentRequestedForId;
      systemVariables["release.deployment.requestedForId"] = guid.ToString("D");
      systemVariables["release.releaseWebUrl"] = WebAccessUrlBuilder.GetReleaseWebAccessUri(this.RequestContext, this.ProjectId.ToString(), input.ReleaseId);
      return (IDictionary<string, string>) systemVariables;
    }

    protected void TraceInformation(string format, params object[] args) => this.Trace(TraceLevel.Info, format, args);

    protected void TraceError(string format, params object[] args) => this.Trace(TraceLevel.Error, format, args);

    private static void CancelPlan(
      IVssRequestContext context,
      string taskhubName,
      Guid projectId,
      Guid planId,
      TimeSpan jobCancelTimeout)
    {
      DistributedTaskOrchestrator.GetTaskHub(context, taskhubName).CancelPlan(context, projectId, planId, jobCancelTimeout, string.Empty);
    }

    private static Guid RunPlan(
      IVssRequestContext context,
      string taskhubName,
      Guid projectId,
      Guid planId,
      string planGroup,
      TaskAgentPoolReference taskAgentPoolReference,
      Uri artifactUri,
      IOrchestrationEnvironment planEnvironment,
      PlanTemplateType planTemplateType,
      IOrchestrationProcess orchestrationProcess,
      BuildOptions buildOptions,
      Guid requestedForId,
      TaskOrchestrationOwner definitionReference,
      TaskOrchestrationOwner ownerReference)
    {
      return DistributedTaskOrchestrator.GetTaskHub(context, taskhubName).RunPlan(context, taskAgentPoolReference, projectId, planId, planGroup, planTemplateType, artifactUri, planEnvironment, orchestrationProcess, buildOptions, requestedForId, definitionReference, ownerReference).PlanId;
    }

    protected TaskOrchestrationOwner GetOwnerReference(AutomationEngineInput input)
    {
      string releaseName;
      input.Data.TryGetValue("ReleaseName", out releaseName);
      string environmentName;
      input.Data.TryGetValue("ReleaseEnvironmentName", out environmentName);
      return TaskOrchestrationHelper.GetOwnerReference(this.RequestContext, this.ProjectId, input.ReleaseId, releaseName, input.EnvironmentId, environmentName);
    }

    protected TaskOrchestrationOwner GetDefinitionReference(AutomationEngineInput input)
    {
      string s;
      input.Data.TryGetValue("ReleaseDefinitionId", out s);
      string releaseDefinitionName;
      input.Data.TryGetValue("ReleaseDefinitionName", out releaseDefinitionName);
      int result;
      if (!int.TryParse(s, out result))
        result = 0;
      return TaskOrchestrationHelper.GetDefinitionReference(this.RequestContext, this.ProjectId, result, releaseDefinitionName);
    }

    protected string GetPlanGroupName(int releaseId) => !this.RequestContext.ExecutionEnvironment.IsOnPremisesDeployment ? string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}:{1}:{2}", (object) "Release", (object) this.ProjectId, (object) releaseId.ToString((IFormatProvider) CultureInfo.InvariantCulture)) : releaseId.ToString((IFormatProvider) CultureInfo.InvariantCulture);

    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", Justification = "This is not shown to the user and is for tracing")]
    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "This is not shown to the user and is for tracing")]
    private void Trace(TraceLevel traceLevel, string format, object[] args) => VssRequestContextExtensions.Trace(this.RequestContext, 1950021, traceLevel, "ReleaseManagementService", "DistributedTask", format, args);

    public ITimelineRecordContext StartJob(
      IVssRequestContext requestContext,
      Guid jobId,
      string jobName,
      Guid projectId,
      Guid planId)
    {
      return this.StartTimelineRecord(requestContext, jobId, "Job", jobName, new Guid?(), new int?(), true, projectId, planId);
    }

    public ITimelineRecordContext StartTask(
      IVssRequestContext requestContext,
      ITimelineRecordContext parentContext,
      Guid taskId,
      string taskName,
      Guid projectId,
      Guid planId)
    {
      if (parentContext == null)
        throw new ArgumentNullException(nameof (parentContext));
      return this.StartTimelineRecord(requestContext, taskId, "Task", taskName, new Guid?(parentContext.Id), new int?(), true, projectId, planId);
    }

    private ITimelineRecordContext StartTimelineRecord(
      IVssRequestContext requestContext,
      Guid recordId,
      string type,
      string name,
      Guid? parentId,
      int? order,
      bool discardIfEmpty,
      Guid projectId,
      Guid planId)
    {
      TaskHub taskHub = DistributedTaskOrchestrator.GetTaskHub(requestContext, this.TaskHubName);
      Timeline timeline = taskHub.GetTimeline(requestContext, projectId, planId, Guid.Empty);
      TimelineRecord timelineRecord = new TimelineRecord()
      {
        Id = recordId,
        Name = name,
        Order = order,
        RecordType = type,
        StartTime = new DateTime?(DateTime.UtcNow),
        State = new TimelineRecordState?(TimelineRecordState.InProgress),
        ParentId = parentId
      };
      Guid jobRecordId = !(type == "Job") ? parentId.Value : recordId;
      if (discardIfEmpty)
        return (ITimelineRecordContext) new LazyTimelineRecordContext(requestContext, taskHub, projectId, planId, timeline.Id, jobRecordId, timelineRecord);
      taskHub.UpdateTimeline(requestContext, projectId, planId, timeline.Id, (IList<TimelineRecord>) new TimelineRecord[1]
      {
        timelineRecord
      });
      return (ITimelineRecordContext) new TimelineRecordContext(requestContext, taskHub, projectId, planId, timeline.Id, jobRecordId, timelineRecord);
    }

    public virtual string GetLogRelativeFilePath(TimelineRecord timelineRecord, string folderName)
    {
      string fileName = timelineRecord != null ? timelineRecord.Name : throw new ArgumentNullException(nameof (timelineRecord));
      int? nullable = timelineRecord.Order;
      if (timelineRecord.RecordType == "Phase")
      {
        fileName = string.Format((IFormatProvider) CultureInfo.CurrentCulture, "DebugContext_{0}", (object) Guid.NewGuid().ToString("N").Substring(26));
        nullable = new int?(0);
      }
      string str = FileSpec.RemoveInvalidFileNameChars(fileName);
      return string.Format((IFormatProvider) CultureInfo.CurrentCulture, "{0}/{1}_{2}.log", (object) folderName, (object) nullable, (object) str);
    }
  }
}
