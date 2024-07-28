// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.TaskHub
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using BuildXL.Cache.ContentStore.Hashing;
using Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server;
using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.DistributedTask.Expressions;
using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts;
using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.PipelineProviders;
using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.DataAccess;
using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Events;
using Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks;
using Microsoft.TeamFoundation.DistributedTask.Pipelines;
using Microsoft.TeamFoundation.DistributedTask.Pipelines.Checkpoints;
using Microsoft.TeamFoundation.DistributedTask.Pipelines.Runtime;
using Microsoft.TeamFoundation.DistributedTask.Pipelines.Server;
using Microsoft.TeamFoundation.DistributedTask.Pipelines.SourceProviders;
using Microsoft.TeamFoundation.DistributedTask.Pipelines.Tasks;
using Microsoft.TeamFoundation.DistributedTask.Pipelines.Validation;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.FeatureAvailability.PipelineComponents;
using Microsoft.TeamFoundation.Framework.Server.Threading;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.VisualStudio.Services.BlobStore.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.DelegatedAuthorization;
using Microsoft.VisualStudio.Services.FileContainer;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Location;
using Microsoft.VisualStudio.Services.Location.Server;
using Microsoft.VisualStudio.Services.Orchestration;
using Microsoft.VisualStudio.Services.Orchestration.Server;
using Microsoft.VisualStudio.Services.PipelineCache.Common;
using Microsoft.VisualStudio.Services.Redis;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server
{
  public sealed class TaskHub
  {
    private IDictionary<string, IPhaseProviderExtension> m_phaseProviders;
    private const string c_runPipeline = "RunPipeline";
    private const string c_userScopes = "$Scp";
    private const int c_InitialLogTableExpiryHours = 168;
    private const int c_FinalLogTableExpiaryHours = 1;
    private const int c_DeleteBlobsBatchSize = 100;
    private const string c_DeleteBlobsBatchSizeRegistryPath = "/DistributedTask/Sdk/DeleteBlobsBatchSize";
    public const string c_teamName = "CIPlatform";
    private const int DefaultOidcTokenMaxValidTimeInMinutes = 5;
    private static readonly RegistryQuery s_allowedOidcTokenAudiences = (RegistryQuery) "/Service/DistributedTask/Settings/AllowedOidcTokenAudiences/*";
    private const string c_fullReferencesResolutionDuringOidcTokenGeneration = "FullResolution";
    private static readonly IReadOnlyCollection<Guid> WellKnownMicrosoftExtentionTaskIds = (IReadOnlyCollection<Guid>) new HashSet<Guid>()
    {
      new Guid("f8c97cf9-4e17-4244-b0fb-f540cea78153"),
      new Guid("4dae1f76-29d3-482f-97d5-e3189a8347c2"),
      new Guid("8cf7cac0-620b-11e5-b4cf-8565e60f4d27"),
      new Guid("92e6c372-4193-44e5-9db7-58d7d253f4d8"),
      new Guid("cbbf7f14-c386-4c1f-80a3-fe500e2bd976"),
      new Guid("2e371150-da5e-11e5-83da-0943b1acc572"),
      new Guid("cbbf7f14-c386-4c1f-80a3-fe500e2bd977")
    };

    public TaskHub(string name, string dataspaceCategory)
    {
      this.Name = name;
      this.DataspaceCategory = dataspaceCategory;
    }

    internal TaskHub()
    {
    }

    public string Name { get; internal set; }

    public int Version { get; internal set; }

    public int PipelineVersion { get; internal set; }

    public string DataspaceCategory { get; internal set; }

    public TaskHubExtension Extension { get; internal set; }

    public OrchestrationTracer GetCIAO(IVssRequestContext requestContext) => new OrchestrationTracer(requestContext, "DistributedTask", "Orchestration", this.Name, true);

    public TComponent CreateComponent<TComponent>(IVssRequestContext requestContext) where TComponent : class, ISqlResourceComponent, new() => requestContext.CreateComponent<TComponent>(this.DataspaceCategory);

    public void PopulateTeamProjectReferences(
      IVssRequestContext requestContext,
      TaskOrchestrationQueuedPlanGroup planGroup)
    {
      if (planGroup == null || planGroup.Project == null)
        return;
      ProjectInfo project = requestContext.GetService<IProjectService>().GetProject(requestContext, planGroup.Project.Id);
      planGroup.Project.Name = project.Name;
    }

    public async Task<TaskLog> AppendLogAsync(
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      Guid planId,
      int logId,
      Stream content,
      bool useBlob = true)
    {
      TaskLog taskLog1;
      using (new MethodScope(requestContext, nameof (TaskHub), nameof (AppendLogAsync)))
      {
        useBlob = useBlob && this.CanLogToBlobStore(requestContext);
        Microsoft.TeamFoundation.DistributedTask.WebApi.TaskOrchestrationPlanReference plan = this.EnsurePlanData(requestContext, scopeIdentifier, planId);
        this.Extension.CheckWritePermission(requestContext, plan.ScopeIdentifier, plan.PlanId, plan.ArtifactUri);
        TaskTrackingComponent trackingComponent = requestContext.CreateComponent<TaskTrackingComponent>(this.DataspaceCategory);
        Tuple<TaskLog, TaskLogPage> createPageResult;
        try
        {
          createPageResult = await trackingComponent.CreateLogPageAsync(plan.ScopeIdentifier, planId, logId);
        }
        finally
        {
          trackingComponent?.Dispose();
        }
        trackingComponent = (TaskTrackingComponent) null;
        long lineCount = 0;
        string blobFileId = (string) null;
        using (LogParsingStream lineCounter = new LogParsingStream(content, Encoding.UTF8))
        {
          int num;
          if (num == 1 || useBlob)
          {
            try
            {
              blobFileId = await requestContext.GetService<IBlobStoreLogService>().UploadLogAsync(requestContext, planId, createPageResult.Item1.Id, createPageResult.Item2.PageId, (Stream) lineCounter);
            }
            catch (Exception ex)
            {
              requestContext.TraceException(10016139, nameof (TaskHub), ex);
              blobFileId = (string) null;
              useBlob = false;
              lineCounter.Position = 0L;
            }
          }
          if (!useBlob)
          {
            string filePath = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/{1}", (object) createPageResult.Item1.Path, (object) createPageResult.Item2.PageId);
            requestContext.GetService<ITeamFoundationFileContainerService>().UploadFile(requestContext.Elevate(), plan.ContainerId, filePath, (Stream) lineCounter, plan.ScopeIdentifier);
          }
          lineCount = lineCounter.LineCount;
        }
        trackingComponent = requestContext.CreateComponent<TaskTrackingComponent>(this.DataspaceCategory);
        TaskLog taskLog2;
        try
        {
          taskLog2 = await trackingComponent.UpdateLogPageAsync(plan.ScopeIdentifier, planId, logId, createPageResult.Item2.PageId, lineCount, TaskLogPageState.Uploaded, blobFileId);
        }
        finally
        {
          trackingComponent?.Dispose();
        }
        trackingComponent = (TaskTrackingComponent) null;
        taskLog1 = taskLog2;
      }
      return taskLog1;
    }

    public async Task<TaskLog> AssociateLogAsync(
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      Guid planId,
      int logId,
      string serializedBlobId,
      int lineCount)
    {
      TaskLog taskLog1;
      using (new MethodScope(requestContext, nameof (TaskHub), nameof (AssociateLogAsync)))
      {
        Microsoft.TeamFoundation.DistributedTask.WebApi.TaskOrchestrationPlanReference plan = this.EnsurePlanData(requestContext, scopeIdentifier, planId);
        this.Extension.CheckWritePermission(requestContext, plan.ScopeIdentifier, plan.PlanId, plan.ArtifactUri);
        TaskTrackingComponent trackingComponent = requestContext.CreateComponent<TaskTrackingComponent>(this.DataspaceCategory);
        Tuple<TaskLog, TaskLogPage> createPageResult;
        try
        {
          createPageResult = await trackingComponent.CreateLogPageAsync(plan.ScopeIdentifier, planId, logId);
        }
        finally
        {
          trackingComponent?.Dispose();
        }
        trackingComponent = (TaskTrackingComponent) null;
        BlobIdentifier blobIdentifier = await requestContext.GetService<IBlobStoreLogService>().AddLogReferenceAsync(requestContext, planId, createPageResult.Item1.Id, createPageResult.Item2.PageId, serializedBlobId);
        trackingComponent = requestContext.CreateComponent<TaskTrackingComponent>(this.DataspaceCategory);
        TaskLog taskLog2;
        try
        {
          taskLog2 = await trackingComponent.UpdateLogPageAsync(plan.ScopeIdentifier, planId, logId, createPageResult.Item2.PageId, (long) lineCount, TaskLogPageState.Uploaded, blobIdentifier.ValueString);
        }
        finally
        {
          trackingComponent?.Dispose();
        }
        trackingComponent = (TaskTrackingComponent) null;
        taskLog1 = taskLog2;
      }
      return taskLog1;
    }

    public async Task CancelPlanAsync(
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      Guid planId,
      TimeSpan timeout,
      string reason)
    {
      TaskHub taskHub1 = this;
      MethodScope methodScope = new MethodScope(requestContext, nameof (TaskHub), nameof (CancelPlanAsync));
      try
      {
        ArgumentUtility.CheckForEmptyGuid(planId, nameof (planId));
        Microsoft.TeamFoundation.DistributedTask.WebApi.TaskOrchestrationPlanReference plan = taskHub1.EnsurePlanData(requestContext, scopeIdentifier, planId);
        try
        {
          if (plan.ProcessType == OrchestrationProcessType.Container)
          {
            if (plan.Version < 3)
            {
              await taskHub1.RaiseOrchestrationEventAsync(requestContext, plan.GetOrchestrationId(), "Canceled", (object) timeout, true);
            }
            else
            {
              TaskHub taskHub2 = taskHub1;
              IVssRequestContext requestContext1 = requestContext;
              string orchestrationId = plan.GetOrchestrationId();
              CanceledEvent eventData = new CanceledEvent();
              eventData.Reason = reason;
              eventData.Timeout = timeout;
              DateTime? fireAt = new DateTime?();
              await taskHub2.RaiseOrchestrationEventAsync(requestContext1, orchestrationId, "Canceled", (object) eventData, true, fireAt);
            }
          }
          else
          {
            TaskHub taskHub3 = taskHub1;
            IVssRequestContext requestContext2 = requestContext;
            string orchestrationId = plan.GetOrchestrationId();
            CanceledEvent eventData = new CanceledEvent();
            eventData.Reason = reason;
            DateTime? fireAt = new DateTime?();
            await taskHub3.RaiseOrchestrationEventAsync(requestContext2, orchestrationId, "Canceled", (object) eventData, true, fireAt);
          }
        }
        catch (OrchestrationSessionNotFoundException ex)
        {
          requestContext.TraceException(10015509, nameof (TaskHub), (Exception) ex);
          DateTime utcNow = DateTime.UtcNow;
          Microsoft.TeamFoundation.DistributedTask.WebApi.TaskOrchestrationPlanReference orchestrationPlanReference = taskHub1.EnsurePlanData(requestContext, scopeIdentifier, planId);
          UpdatePlanResult updatePlanResult = new UpdatePlanResult();
          using (TaskTrackingComponent trackingComponent = requestContext.CreateComponent<TaskTrackingComponent>(taskHub1.DataspaceCategory))
            updatePlanResult = await trackingComponent.UpdatePlanAsync(orchestrationPlanReference.ScopeIdentifier, orchestrationPlanReference.PlanId, new DateTime?(utcNow), new DateTime?(utcNow), new TaskOrchestrationPlanState?(TaskOrchestrationPlanState.Completed), new Microsoft.TeamFoundation.DistributedTask.WebApi.TaskResult?(Microsoft.TeamFoundation.DistributedTask.WebApi.TaskResult.Canceled), "Canceled", (IOrchestrationEnvironment) null);
          if (updatePlanResult.Timeline != null)
            TaskHub.TraceTimelineRecordUpdates(requestContext, (IList<Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord>) null, (IList<Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord>) updatePlanResult.Timeline.Records, nameof (CancelPlanAsync));
          throw new TaskOrchestrationPlanTerminatedException(TaskResources.PlanOrchestrationTerminated((object) planId.ToString("D")), (Exception) ex);
        }
      }
      finally
      {
        methodScope.Dispose();
      }
      methodScope = new MethodScope();
    }

    public async Task CancelNodeAsync(
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      Guid planId,
      string nodeIdentifier,
      string reason,
      TimeSpan timeout)
    {
      TaskHub taskHub1 = this;
      MethodScope methodScope = new MethodScope(requestContext, nameof (TaskHub), nameof (CancelNodeAsync));
      try
      {
        ArgumentUtility.CheckForEmptyGuid(planId, nameof (planId));
        ArgumentUtility.CheckStringForNullOrWhiteSpace(nodeIdentifier, nameof (nodeIdentifier));
        string orchestrationInstanceId = PipelineUtilities.GetOrchestrationInstanceId(planId, nodeIdentifier);
        try
        {
          TaskHub taskHub2 = taskHub1;
          IVssRequestContext requestContext1 = requestContext;
          string instanceId = orchestrationInstanceId;
          CanceledEvent eventData = new CanceledEvent();
          eventData.Reason = reason;
          eventData.Timeout = timeout;
          DateTime? fireAt = new DateTime?();
          await taskHub2.RaiseOrchestrationEventAsync(requestContext1, instanceId, "Canceled", (object) eventData, true, fireAt);
        }
        catch (OrchestrationSessionNotFoundException ex)
        {
          requestContext.TraceAlways(10015556, nameof (TaskHub), "Could not deliver event to node '{0}' -- no session found with instance id '{1}'", (object) nodeIdentifier, (object) orchestrationInstanceId);
        }
        orchestrationInstanceId = (string) null;
      }
      finally
      {
        methodScope.Dispose();
      }
      methodScope = new MethodScope();
    }

    public TaskOrchestrationPlan CreatePlan(
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      Guid planId,
      string planGroup,
      PlanTemplateType templateType,
      Uri artifactUri,
      IOrchestrationEnvironment environment,
      IOrchestrationProcess process,
      BuildOptions validationOptions,
      Guid requestedForId,
      TaskOrchestrationOwner definitionReference,
      TaskOrchestrationOwner ownerReference,
      string pipelineInitializationLog,
      string pipelineExpandedYaml)
    {
      using (new MethodScope(requestContext, nameof (TaskHub), nameof (CreatePlan)))
      {
        ArgumentUtility.CheckForNull<Uri>(artifactUri, nameof (artifactUri));
        ArgumentUtility.CheckForNull<IOrchestrationEnvironment>(environment, nameof (environment));
        ArgumentUtility.CheckForNull<IOrchestrationProcess>(process, nameof (process));
        IVssRegistryService service1 = requestContext.GetService<IVssRegistryService>();
        IVssRegistryService registryService1 = service1;
        IVssRequestContext requestContext1 = requestContext;
        RegistryQuery registryQuery = (RegistryQuery) ("/Service/Orchestration/Settings/" + this.Name + "/ForbidPlansCreation");
        ref RegistryQuery local1 = ref registryQuery;
        if (registryService1.GetValue<bool>(requestContext1, in local1))
        {
          requestContext.TraceAlways(10015568, nameof (TaskHub), "Creation of new plans is limited due to a registry setting. Organization is struggling to recover and feature is enabled by DRI to prevent any new runs.");
          throw new DistributedTaskException(TaskResources.CreateNewPlansForbidden());
        }
        this.GetCIAO(requestContext).TracePhaseStarted(planId.ToString("D"), "CIPlatform", nameof (CreatePlan));
        string securityToken;
        if (!this.Extension.TryGetSecurityToken(requestContext, scopeIdentifier, artifactUri, out securityToken))
          throw new DistributedTaskException(TaskResources.SecurityTokenNotFound((object) artifactUri, (object) this.Extension.GetType().FullName));
        if (environment is PipelineEnvironment pipelineEnvironment1)
        {
          pipelineEnvironment1.Options.EnforceLegalNodeNames = requestContext.IsFeatureEnabled("DistributedTask.LegalNodeNames");
          pipelineEnvironment1.Options.DisallowWideCharactersInNodeNames = true;
          if (requestContext.IsFeatureEnabled("DistributedTask.RunJobsWithDemandsOnSingleHostedPool"))
          {
            IVssRegistryService registryService2 = service1;
            IVssRequestContext requestContext2 = requestContext;
            registryQuery = (RegistryQuery) RegistryKeys.DemandsOnSingleHostedPoolBlockedRegistrySettingsPath;
            ref RegistryQuery local2 = ref registryQuery;
            if (!registryService2.GetValue<bool>(requestContext2, in local2, false))
            {
              pipelineEnvironment1.Options.RunJobsWithDemandsOnSingleHostedPool = true;
              goto label_10;
            }
          }
          pipelineEnvironment1.Options.RunJobsWithDemandsOnSingleHostedPool = false;
label_10:
          validationOptions.AllowHyphenNames = pipelineEnvironment1.Options.AllowHyphenNames;
          validationOptions.RestrictedNodeVersions = (IList<int>) this.GetRestrictedNodeTaskVersions(requestContext);
        }
        TaskHub.ProcessValidationResult validationResult = this.ValidateProcess(requestContext, scopeIdentifier, definitionReference, planId, environment, process, validationOptions);
        if (requestContext.IsFeatureEnabled("DistributedTask.EnableJustInTimeAuthorization") && environment is PipelineEnvironment pipelineEnvironment2)
          pipelineEnvironment2.Resources = validationResult.Resources;
        ITeamFoundationFileContainerService service2 = requestContext.GetService<ITeamFoundationFileContainerService>();
        long num1;
        if (process.ProcessType == OrchestrationProcessType.Container)
        {
          Microsoft.VisualStudio.Services.FileContainer.FileContainer fileContainer1 = service2.QueryContainers(requestContext, (IList<Uri>) new Uri[1]
          {
            artifactUri
          }, scopeIdentifier).FirstOrDefault<Microsoft.VisualStudio.Services.FileContainer.FileContainer>();
          if (fileContainer1 != null)
          {
            num1 = fileContainer1.Id;
          }
          else
          {
            try
            {
              string name = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Artifact Container for {0}", (object) artifactUri);
              num1 = service2.CreateContainer(requestContext, artifactUri, securityToken, name, (string) null, scopeIdentifier);
            }
            catch (ContainerAlreadyExistsException ex)
            {
              Microsoft.VisualStudio.Services.FileContainer.FileContainer fileContainer2 = service2.QueryContainers(requestContext, (IList<Uri>) new Uri[1]
              {
                artifactUri
              }, scopeIdentifier).FirstOrDefault<Microsoft.VisualStudio.Services.FileContainer.FileContainer>();
              if (fileContainer2 != null)
                num1 = fileContainer2.Id;
              else
                throw;
            }
          }
        }
        else
        {
          Uri pipelineUri = this.GetPipelineUri(scopeIdentifier, planId);
          num1 = service2.CreateContainer(requestContext, pipelineUri, artifactUri.ToString(), string.Format("pipelines.logs.{0}", (object) planId), (string) null, scopeIdentifier);
        }
        Microsoft.VisualStudio.Services.Identity.Identity userIdentity = requestContext.GetUserIdentity();
        TaskOrchestrationPlan orchestrationPlan = new TaskOrchestrationPlan();
        orchestrationPlan.ScopeIdentifier = scopeIdentifier;
        orchestrationPlan.PlanType = this.Name;
        orchestrationPlan.Version = validationResult.Version;
        orchestrationPlan.PlanId = planId;
        orchestrationPlan.PlanGroup = planGroup;
        orchestrationPlan.TemplateType = templateType;
        orchestrationPlan.ArtifactUri = artifactUri;
        orchestrationPlan.ContainerId = num1;
        orchestrationPlan.ProcessEnvironment = environment;
        orchestrationPlan.Process = process;
        orchestrationPlan.RequestedById = userIdentity.Id;
        orchestrationPlan.RequestedForId = requestedForId;
        orchestrationPlan.Definition = definitionReference;
        orchestrationPlan.Owner = ownerReference;
        TaskOrchestrationPlan plan = orchestrationPlan;
        bool createInitializationLog = !string.IsNullOrEmpty(pipelineInitializationLog);
        bool createExpandedYaml = !string.IsNullOrEmpty(pipelineExpandedYaml);
        if (environment is PipelineEnvironment pipelineEnvironment3)
        {
          if (requestContext.ExecutionEnvironment.IsHostedDeployment)
          {
            if (!requestContext.IsFeatureEnabled("DistributedTask.PipelineBillingModel2.SelfHosted.InfiniteResourceLimits"))
            {
              try
              {
                IList<ResourceLimit> resourceLimits = requestContext.GetService<ITaskHubLicenseService>().GetResourceLimits(requestContext);
                string paralleismTag = pipelineEnvironment3.SystemVariables.GetValueOrDefault<string, VariableValue>(WellKnownDistributedTaskVariables.JobParallelismTag)?.Value;
                if (paralleismTag == "Public")
                {
                  ResourceLimit resourceLimit = resourceLimits.Where<ResourceLimit>((Func<ResourceLimit, bool>) (x => x.HostId == requestContext.ServiceHost.InstanceId && x.IsHosted && x.ParallelismTag == "Public")).FirstOrDefault<ResourceLimit>();
                  pipelineEnvironment3.Options.MaxParallelism = (int?) resourceLimit?.TotalCount;
                }
                if (requestContext.IsFeatureEnabled("DistributedTask.LimitJobTimeoutToMMSLimit"))
                {
                  ResourceLimit resourceLimit = resourceLimits.Where<ResourceLimit>((Func<ResourceLimit, bool>) (x => x.HostId == requestContext.ServiceHost.InstanceId && x.IsHosted && x.ParallelismTag == paralleismTag)).FirstOrDefault<ResourceLimit>();
                  if (resourceLimit != null)
                    pipelineEnvironment3.Options.MaxHostedJobTimeout = new int?(resourceLimit.IsPremium ? PipelineConstants.ResourceLimits.PremiumAgentTimeout : PipelineConstants.ResourceLimits.FreeAgentTimeout);
                }
              }
              catch (Exception ex)
              {
                requestContext.TraceException(nameof (TaskHub), ex);
              }
            }
          }
          int num2 = service1.GetValue<int>(requestContext, in RegistryKeys.PipelinePhaseExpansionLimit, true, 256);
          if (num2 > 0)
            pipelineEnvironment3.Options.MaxJobExpansion = new int?(num2);
        }
        bool flag = requestContext.GetService<IPlanThrottleService>().ShouldThrottleNewPlans(requestContext, this, scopeIdentifier, definitionReference);
        if (flag)
        {
          requestContext.TraceAlways(10015558, nameof (TaskHub), "Throttling new plan: {0}", (object) planId);
          plan.State = TaskOrchestrationPlanState.Throttled;
        }
        using (TaskTrackingComponent component = requestContext.CreateComponent<TaskTrackingComponent>(this.DataspaceCategory))
          plan = component.CreatePlan(plan, validationResult.Timeline, (IEnumerable<TaskOrchestrationJob>) validationResult.ContainerJobs, (IEnumerable<TaskReferenceData>) validationResult.Tasks, (IEnumerable<Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineAttempt>) validationResult.Attempts, createInitializationLog, createExpandedYaml);
        if (plan != null)
        {
          if (createInitializationLog)
          {
            TaskLogReference initializationLog = plan.InitializationLog;
            if (initializationLog != null)
            {
              using (MemoryStream content = new MemoryStream())
              {
                using (StreamWriter streamWriter = new StreamWriter((Stream) content, Encoding.UTF8))
                {
                  streamWriter.Write(pipelineInitializationLog);
                  streamWriter.Flush();
                  content.Seek(0L, SeekOrigin.Begin);
                  this.AppendLog(requestContext, scopeIdentifier, planId, initializationLog.Id, (Stream) content);
                }
              }
            }
          }
          if (createExpandedYaml)
          {
            TaskLogReference expandedYaml = plan.ExpandedYaml;
            if (expandedYaml != null)
            {
              using (MemoryStream content = new MemoryStream())
              {
                using (StreamWriter streamWriter = new StreamWriter((Stream) content, Encoding.UTF8))
                {
                  streamWriter.Write(pipelineExpandedYaml);
                  streamWriter.Flush();
                  content.Seek(0L, SeekOrigin.Begin);
                  this.AppendLog(requestContext, scopeIdentifier, planId, expandedYaml.Id, (Stream) content);
                }
              }
            }
          }
        }
        requestContext.GetService<TaskOrchestrationPlanCache>().SetPlan(requestContext, plan.AsReference());
        if (requestContext.IsFeatureEnabled("DistributedTask.PlanLifecycleTracing"))
        {
          string orchestrationId = plan.GetOrchestrationId();
          using (requestContext.CreateOrchestrationIdScope(orchestrationId))
            requestContext.TraceAlways(10015547, nameof (TaskHub), "Created plan: {0}, planVersion: {1}, IsSystemContext: {2}, IsUserContext: {3}", (object) orchestrationId, (object) plan.Version, (object) requestContext.IsSystemContext, (object) requestContext.IsUserContext);
        }
        this.Extension.PlanCreated(requestContext, plan);
        requestContext.TraceSecureFileResources(plan);
        if (flag)
          requestContext.GetService<ITeamFoundationJobService>().QueueJobsNow(requestContext, (IEnumerable<Guid>) new Guid[1]
          {
            TaskConstants.QueueThrottledPlansJob
          });
        return plan;
      }
    }

    public async Task<TaskLog> CreateLogAsync(
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      Guid planId,
      string logPath)
    {
      TaskLog logAsync;
      using (new MethodScope(requestContext, nameof (TaskHub), nameof (CreateLogAsync)))
      {
        ArgumentUtility.CheckForEmptyGuid(planId, nameof (planId));
        ArgumentUtility.CheckStringForNullOrEmpty(logPath, nameof (logPath));
        Microsoft.TeamFoundation.DistributedTask.WebApi.TaskOrchestrationPlanReference orchestrationPlanReference = this.EnsurePlanData(requestContext, scopeIdentifier, planId);
        this.Extension.CheckWritePermission(requestContext, orchestrationPlanReference.ScopeIdentifier, orchestrationPlanReference.PlanId, orchestrationPlanReference.ArtifactUri);
        Microsoft.VisualStudio.Services.Identity.Identity userIdentity = requestContext.GetUserIdentity();
        using (TaskTrackingComponent trackingComponent = requestContext.CreateComponent<TaskTrackingComponent>(this.DataspaceCategory))
          logAsync = await trackingComponent.CreateLogAsync(orchestrationPlanReference.ScopeIdentifier, orchestrationPlanReference.PlanId, userIdentity.Id, logPath);
      }
      return logAsync;
    }

    public async Task<TaskAttachment> CreateAttachmentAsync(
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      Guid planId,
      Guid timelineId,
      Guid recordId,
      string type,
      string name,
      Stream content)
    {
      TaskAttachment taskAttachment;
      using (new MethodScope(requestContext, nameof (TaskHub), nameof (CreateAttachmentAsync)))
      {
        ArgumentUtility.CheckForEmptyGuid(planId, nameof (planId));
        ArgumentUtility.CheckForEmptyGuid(timelineId, nameof (timelineId));
        ArgumentUtility.CheckForEmptyGuid(recordId, nameof (recordId));
        string str1 = FileSpec.RemoveInvalidFileNameChars(type);
        string str2 = FileSpec.RemoveInvalidFileNameChars(name);
        ArgumentUtility.CheckStringForNullOrEmpty(str1, nameof (type));
        ArgumentUtility.CheckStringForNullOrEmpty(str2, nameof (name));
        Microsoft.TeamFoundation.DistributedTask.WebApi.TaskOrchestrationPlanReference plan = this.EnsurePlanData(requestContext, scopeIdentifier, planId);
        this.Extension.CheckWritePermission(requestContext, plan.ScopeIdentifier, plan.PlanId, plan.ArtifactUri);
        string attachmentPath = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "attachments\\{0:D}\\{1}\\{2}", (object) recordId, (object) str1, (object) str2);
        TaskAttachmentData attachmentData = (TaskAttachmentData) null;
        Microsoft.VisualStudio.Services.Identity.Identity userIdentity = requestContext.GetUserIdentity();
        using (TaskTrackingComponent trackingComponent = requestContext.CreateComponent<TaskTrackingComponent>(this.DataspaceCategory))
          attachmentData = await trackingComponent.CreateAttachmentAsync(plan.ScopeIdentifier, plan.PlanId, timelineId, recordId, str1, str2, attachmentPath, userIdentity.Id);
        requestContext.GetService<TeamFoundationFileContainerService>().UploadFile(requestContext.Elevate(), plan.ContainerId, attachmentData.Path, content, plan.ScopeIdentifier);
        attachmentData.AddLinks(requestContext, plan.ScopeIdentifier, this.Name, plan.PlanId);
        taskAttachment = attachmentData.ToTaskAttachment();
      }
      return taskAttachment;
    }

    public async Task<TaskAttachment> AssociateAttachmentAsync(
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      Guid planId,
      Guid timelineId,
      Guid recordId,
      string type,
      string name,
      string artifactHash,
      long length)
    {
      TaskAttachment taskAttachment;
      using (new MethodScope(requestContext, nameof (TaskHub), nameof (AssociateAttachmentAsync)))
      {
        ArgumentUtility.CheckForEmptyGuid(planId, nameof (planId));
        ArgumentUtility.CheckForEmptyGuid(timelineId, nameof (timelineId));
        ArgumentUtility.CheckForEmptyGuid(recordId, nameof (recordId));
        string str1 = FileSpec.RemoveInvalidFileNameChars(type);
        string str2 = FileSpec.RemoveInvalidFileNameChars(name);
        ArgumentUtility.CheckStringForNullOrEmpty(str1, nameof (type));
        ArgumentUtility.CheckStringForNullOrEmpty(str2, nameof (name));
        ArgumentUtility.CheckStringForNullOrEmpty(artifactHash, nameof (artifactHash));
        Microsoft.TeamFoundation.DistributedTask.WebApi.TaskOrchestrationPlanReference plan = this.EnsurePlanData(requestContext, scopeIdentifier, planId);
        this.Extension.CheckWritePermission(requestContext, plan.ScopeIdentifier, plan.PlanId, plan.ArtifactUri);
        string attachmentPath = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "attachments\\{0:D}\\{1}\\{2}", (object) recordId, (object) str1, (object) str2);
        TaskAttachmentData attachmentData = (TaskAttachmentData) null;
        Microsoft.VisualStudio.Services.Identity.Identity userIdentity = requestContext.GetUserIdentity();
        using (TaskTrackingComponent trackingComponent = requestContext.CreateComponent<TaskTrackingComponent>(this.DataspaceCategory))
          attachmentData = await trackingComponent.CreateAttachmentAsync(plan.ScopeIdentifier, plan.PlanId, timelineId, recordId, str1, str2, attachmentPath, userIdentity.Id);
        requestContext.GetService<TeamFoundationFileContainerService>().UploadFileFromArtifact(requestContext.Elevate(), plan.ContainerId, attachmentData.Path, artifactHash, length, plan.ScopeIdentifier);
        attachmentData.AddLinks(requestContext, plan.ScopeIdentifier, this.Name, plan.PlanId);
        taskAttachment = attachmentData.ToTaskAttachment();
      }
      return taskAttachment;
    }

    public async Task<Stream> GetAttachmentAsync(
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      Guid planId,
      Guid timelineId,
      Guid recordId,
      string type,
      string name)
    {
      using (new MethodScope(requestContext, nameof (TaskHub), nameof (GetAttachmentAsync)))
      {
        TaskAttachmentData attachmentInternalAsync = await this.GetAttachmentInternalAsync(requestContext, scopeIdentifier, planId, timelineId, recordId, type, name);
        if (attachmentInternalAsync == null)
          return (Stream) null;
        Microsoft.TeamFoundation.DistributedTask.WebApi.TaskOrchestrationPlanReference planData = this.GetPlanData(requestContext, scopeIdentifier, planId);
        TeamFoundationFileContainerService service = requestContext.GetService<TeamFoundationFileContainerService>();
        FileContainerItem fileContainerItem = service.QuerySpecificItems(requestContext, planData.ContainerId, (IEnumerable<string>) new string[1]
        {
          attachmentInternalAsync.Path
        }, planData.ScopeIdentifier).FirstOrDefault<FileContainerItem>();
        return fileContainerItem == null || fileContainerItem.ItemType != ContainerItemType.File ? (Stream) null : service.RetrieveFile(requestContext, planData.ContainerId, new Guid?(scopeIdentifier), fileContainerItem, false, out CompressionType _);
      }
    }

    public async Task<TaskAttachment> GetAttachmentMetadataAsync(
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      Guid planId,
      Guid timelineId,
      Guid recordId,
      string type,
      string name)
    {
      TaskAttachment taskAttachment;
      using (new MethodScope(requestContext, nameof (TaskHub), nameof (GetAttachmentMetadataAsync)))
        taskAttachment = (await this.GetAttachmentInternalAsync(requestContext, scopeIdentifier, planId, timelineId, recordId, type, name))?.ToTaskAttachment();
      return taskAttachment;
    }

    public async Task<IList<TaskAttachment>> GetAttachmentsAsync(
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      Guid planId,
      string type,
      Guid? timelineId = null,
      Guid? recordId = null)
    {
      using (new MethodScope(requestContext, nameof (TaskHub), nameof (GetAttachmentsAsync)))
      {
        ArgumentUtility.CheckForEmptyGuid(planId, nameof (planId));
        ArgumentUtility.CheckStringForNullOrEmpty(type, nameof (type));
        Microsoft.TeamFoundation.DistributedTask.WebApi.TaskOrchestrationPlanReference plan = this.GetPlanData(requestContext, scopeIdentifier, planId);
        if (plan == null || !this.Extension.HasReadPermission(requestContext, plan.ScopeIdentifier, plan.ArtifactUri))
          return (IList<TaskAttachment>) Array.Empty<TaskAttachment>();
        IList<TaskAttachmentData> attachmentsAsync1;
        using (TaskTrackingComponent trackingComponent = requestContext.CreateComponent<TaskTrackingComponent>(this.DataspaceCategory))
          attachmentsAsync1 = await trackingComponent.GetAttachmentsAsync(plan.ScopeIdentifier, plan.PlanId, timelineId, recordId, type);
        List<TaskAttachment> attachmentsAsync2 = new List<TaskAttachment>();
        foreach (TaskAttachmentData attachmentData in (IEnumerable<TaskAttachmentData>) attachmentsAsync1)
        {
          attachmentData.AddLinks(requestContext, plan.ScopeIdentifier, this.Name, plan.PlanId);
          attachmentsAsync2.Add(attachmentData.ToTaskAttachment());
        }
        return (IList<TaskAttachment>) attachmentsAsync2;
      }
    }

    public Microsoft.TeamFoundation.DistributedTask.WebApi.Timeline CreateTimeline(
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      Guid planId,
      Microsoft.TeamFoundation.DistributedTask.WebApi.Timeline timeline)
    {
      using (new MethodScope(requestContext, nameof (TaskHub), nameof (CreateTimeline)))
      {
        ArgumentUtility.CheckForEmptyGuid(planId, nameof (planId));
        ArgumentUtility.CheckForNull<Microsoft.TeamFoundation.DistributedTask.WebApi.Timeline>(timeline, nameof (timeline));
        Microsoft.TeamFoundation.DistributedTask.WebApi.TaskOrchestrationPlanReference orchestrationPlanReference = this.EnsurePlanData(requestContext, scopeIdentifier, planId);
        this.Extension.CheckWritePermission(requestContext, orchestrationPlanReference.ScopeIdentifier, orchestrationPlanReference.PlanId, orchestrationPlanReference.ArtifactUri);
        Microsoft.VisualStudio.Services.Identity.Identity userIdentity = requestContext.GetUserIdentity();
        using (TaskTrackingComponent component = requestContext.CreateComponent<TaskTrackingComponent>(this.DataspaceCategory))
          return component.CreateTimeline(orchestrationPlanReference.ScopeIdentifier, orchestrationPlanReference.PlanId, userIdentity.Id, timeline.Id, (IEnumerable<Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord>) timeline.Records);
      }
    }

    public async Task<IList<StageAttempt>> CreatePipelineRetryTimelines(
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      Guid planId,
      IList<string> stageNames,
      Guid requestedBy,
      bool forceRetryAllJobs = false)
    {
      TaskOrchestrationPlan planAsync = await this.GetPlanAsync(requestContext, scopeIdentifier, planId);
      if (planAsync == null)
        throw new TaskOrchestrationPlanNotFoundException(TaskResources.PlanNotFound((object) planId));
      return await requestContext.GetService<IPipelineRuntimeService>().CreateStageAttempts(requestContext, this.Name, scopeIdentifier, planAsync, stageNames, requestedBy, forceRetryAllJobs);
    }

    public void DeleteLog(
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      Guid planId,
      int logId)
    {
      using (new MethodScope(requestContext, nameof (TaskHub), nameof (DeleteLog)))
      {
        Microsoft.TeamFoundation.DistributedTask.WebApi.TaskOrchestrationPlanReference planData = this.GetPlanData(requestContext, scopeIdentifier, planId);
        if (planData == null)
          return;
        this.Extension.CheckDeletePermission(requestContext, planData.ScopeIdentifier, planData.PlanId, planData.ArtifactUri);
        using (TaskTrackingComponent component = requestContext.CreateComponent<TaskTrackingComponent>(this.DataspaceCategory))
          component.DeleteLog(planData.ScopeIdentifier, planData.PlanId, logId);
      }
    }

    public void DeletePlans(
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      IEnumerable<Guid> planIds)
    {
      using (new MethodScope(requestContext, nameof (TaskHub), nameof (DeletePlans)))
      {
        Microsoft.TeamFoundation.DistributedTask.WebApi.TaskOrchestrationPlanReference[] array = planIds.Select<Guid, Microsoft.TeamFoundation.DistributedTask.WebApi.TaskOrchestrationPlanReference>((Func<Guid, Microsoft.TeamFoundation.DistributedTask.WebApi.TaskOrchestrationPlanReference>) (x => this.GetPlanData(requestContext, scopeIdentifier, x))).Where<Microsoft.TeamFoundation.DistributedTask.WebApi.TaskOrchestrationPlanReference>((Func<Microsoft.TeamFoundation.DistributedTask.WebApi.TaskOrchestrationPlanReference, bool>) (x => x != null)).ToArray<Microsoft.TeamFoundation.DistributedTask.WebApi.TaskOrchestrationPlanReference>();
        List<Uri> artifactUris = new List<Uri>();
        foreach (Microsoft.TeamFoundation.DistributedTask.WebApi.TaskOrchestrationPlanReference orchestrationPlanReference in array)
        {
          this.Extension.CheckDeletePermission(requestContext, orchestrationPlanReference.ScopeIdentifier, orchestrationPlanReference.PlanId, orchestrationPlanReference.ArtifactUri);
          scopeIdentifier = orchestrationPlanReference.ScopeIdentifier;
          artifactUris.Add(this.GetPipelineUri(orchestrationPlanReference.ScopeIdentifier, orchestrationPlanReference.PlanId));
        }
        ITeamFoundationFileContainerService service = requestContext.GetService<ITeamFoundationFileContainerService>();
        List<Microsoft.VisualStudio.Services.FileContainer.FileContainer> source = service.QueryContainers(requestContext, (IList<Uri>) artifactUris, scopeIdentifier);
        if (source.Count > 0)
          service.DeleteContainers(requestContext, (IList<long>) source.Select<Microsoft.VisualStudio.Services.FileContainer.FileContainer, long>((Func<Microsoft.VisualStudio.Services.FileContainer.FileContainer, long>) (x => x.Id)).ToList<long>(), scopeIdentifier);
        List<LogBlobIdentifier> logBlobIdentifierList = new List<LogBlobIdentifier>();
        foreach (Guid planId in planIds)
        {
          IEnumerable<TaskLogPage> logPages;
          using (TaskTrackingComponent component = requestContext.CreateComponent<TaskTrackingComponent>(this.DataspaceCategory))
            logPages = component.GetLogs(scopeIdentifier, planId, true).LogPages;
          if (logPages != null)
          {
            foreach (TaskLogPage taskLogPage in logPages)
            {
              if (taskLogPage.BlobFileId != null)
                logBlobIdentifierList.Add(new LogBlobIdentifier()
                {
                  PlanId = planId,
                  LogId = taskLogPage.LogId,
                  PageId = taskLogPage.PageId,
                  BlobId = taskLogPage.BlobFileId
                });
            }
          }
        }
        if (logBlobIdentifierList.Count > 0)
        {
          IBlobStoreLogService blobLogStore = requestContext.GetService<IBlobStoreLogService>();
          int val1 = requestContext.GetService<IVssRegistryService>().GetValue<int>(requestContext, (RegistryQuery) "/DistributedTask/Sdk/DeleteBlobsBatchSize", true, 100);
          int count;
          for (int index = 0; index < logBlobIdentifierList.Count; index += count)
          {
            count = Math.Min(val1, logBlobIdentifierList.Count - index);
            List<LogBlobIdentifier> chunk = logBlobIdentifierList.GetRange(index, count);
            requestContext.RunSynchronously((Func<Task>) (() => blobLogStore.DeleteLogReferencesAsync(requestContext, (IEnumerable<LogBlobIdentifier>) chunk)));
          }
        }
        foreach (Guid planId in planIds)
          new PlanSecretStore(requestContext, planId).Delete();
        using (TaskTrackingComponent component = requestContext.CreateComponent<TaskTrackingComponent>(this.DataspaceCategory))
          component.DeletePlans(scopeIdentifier, planIds);
        if (!requestContext.IsFeatureEnabled("DistributedTask.PlanLifecycleTracing"))
          return;
        foreach (Microsoft.TeamFoundation.DistributedTask.WebApi.TaskOrchestrationPlanReference plan in array)
        {
          string orchestrationId = plan.GetOrchestrationId();
          using (requestContext.CreateOrchestrationIdScope(orchestrationId))
            requestContext.TraceAlways(10015546, nameof (TaskHub), "Deleted plan: {0}, planVersion: {1}, IsSystemContext: {2}, IsUserContext: {3}", (object) orchestrationId, (object) plan.Version, (object) requestContext.IsSystemContext, (object) requestContext.IsUserContext);
        }
      }
    }

    public void DeleteTimeline(
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      Guid planId,
      Guid timelineId)
    {
      using (new MethodScope(requestContext, nameof (TaskHub), nameof (DeleteTimeline)))
      {
        Microsoft.TeamFoundation.DistributedTask.WebApi.TaskOrchestrationPlanReference planData = this.GetPlanData(requestContext, scopeIdentifier, planId);
        if (planData == null)
          return;
        this.Extension.CheckDeletePermission(requestContext, planData.ScopeIdentifier, planData.PlanId, planData.ArtifactUri);
        using (TaskTrackingComponent component = requestContext.CreateComponent<TaskTrackingComponent>(this.DataspaceCategory))
          component.DeleteTimeline(planData.ScopeIdentifier, planData.PlanId, timelineId);
      }
    }

    public async Task<GetTaskOrchestrationJobResult> GetJobAsync(
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      Guid planId,
      Guid jobId)
    {
      using (new MethodScope(requestContext, nameof (TaskHub), nameof (GetJobAsync)))
      {
        Microsoft.TeamFoundation.DistributedTask.WebApi.TaskOrchestrationPlanReference planData = this.GetPlanData(requestContext, scopeIdentifier, planId);
        if (planData == null || !this.Extension.HasReadPermission(requestContext, planData.ScopeIdentifier, planData.ArtifactUri))
          return new GetTaskOrchestrationJobResult();
        using (TaskTrackingComponent trackingComponent = requestContext.CreateComponent<TaskTrackingComponent>(this.DataspaceCategory))
          return await trackingComponent.GetJobAsync(planData.ScopeIdentifier, planData.PlanId, jobId);
      }
    }

    private static ServerTaskExecutionContext GetDefaultServerTaskExecutionContext() => new ServerTaskExecutionContext()
    {
      CurrentSectionIndex = 0,
      HasMoreSections = false,
      UseExistingTimelineRecord = false
    };

    public void FeedReceived(
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      Guid planId,
      Guid timelineId,
      Guid jobTimelineRecordId,
      Guid stepTimelineRecordId,
      IList<string> lines)
    {
      using (new MethodScope(requestContext, nameof (TaskHub), nameof (FeedReceived)))
      {
        Microsoft.TeamFoundation.DistributedTask.WebApi.TaskOrchestrationPlanReference planData = this.GetPlanData(requestContext, scopeIdentifier, planId);
        if (planData == null)
          return;
        this.Extension.FeedReceived(requestContext, planData, timelineId, jobTimelineRecordId, stepTimelineRecordId, lines);
      }
    }

    public async Task FeedReceivedAsync(
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      Guid planId,
      Guid timelineId,
      Guid jobTimelineRecordId,
      TimelineRecordFeedLinesWrapper lines)
    {
      MethodScope methodScope = new MethodScope(requestContext, nameof (TaskHub), nameof (FeedReceivedAsync));
      try
      {
        Microsoft.TeamFoundation.DistributedTask.WebApi.TaskOrchestrationPlanReference planData = this.GetPlanData(requestContext, scopeIdentifier, planId);
        if (planData == null)
          return;
        await Task.WhenAll(this.Extension.FeedReceivedAsync(requestContext, planData, timelineId, jobTimelineRecordId, lines.StepId, (IList<string>) lines.Value), this.InsertLogLinesAsync(requestContext, scopeIdentifier, planId, timelineId, jobTimelineRecordId, lines));
      }
      finally
      {
        methodScope.Dispose();
      }
      methodScope = new MethodScope();
    }

    public TaskLog GetLog(
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      Guid planId,
      int logId)
    {
      using (new MethodScope(requestContext, nameof (TaskHub), nameof (GetLog)))
      {
        Microsoft.TeamFoundation.DistributedTask.WebApi.TaskOrchestrationPlanReference planData = this.GetPlanData(requestContext, scopeIdentifier, planId);
        if (planData == null || !this.Extension.HasReadPermission(requestContext, planData.ScopeIdentifier, planData.ArtifactUri))
          return (TaskLog) null;
        using (TaskTrackingComponent component = requestContext.CreateComponent<TaskTrackingComponent>(this.DataspaceCategory))
          return component.GetLog(planData.ScopeIdentifier, planData.PlanId, logId);
      }
    }

    public TeamFoundationDataReader GetLogLines(
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      Guid planId,
      int logId,
      ISecuredObject securedObject,
      ref long startLine,
      ref long endLine,
      out long totalLines)
    {
      using (new MethodScope(requestContext, nameof (TaskHub), nameof (GetLogLines)))
      {
        CommandGetLogLines disposableObject = (CommandGetLogLines) null;
        try
        {
          disposableObject = new CommandGetLogLines(requestContext, this, securedObject);
          disposableObject.Execute(scopeIdentifier, planId, logId, ref startLine, ref endLine, out totalLines);
        }
        catch (Exception ex)
        {
          disposableObject?.Dispose();
          throw;
        }
        return new TeamFoundationDataReader((IDisposable) disposableObject, new object[1]
        {
          (object) disposableObject.Lines
        });
      }
    }

    public Dictionary<int, TeamFoundationLogItemData> GetLogsReaders(
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      Guid planId,
      ISet<int> filterLogIds)
    {
      using (new MethodScope(requestContext, nameof (TaskHub), nameof (GetLogsReaders)))
      {
        Microsoft.TeamFoundation.DistributedTask.WebApi.TaskOrchestrationPlanReference planData = this.EnsurePlanData(requestContext, scopeIdentifier, planId);
        if (!this.Extension.HasReadPermission(requestContext, planData.ScopeIdentifier, planData.ArtifactUri))
          return new Dictionary<int, TeamFoundationLogItemData>(0);
        IEnumerable<TaskLog> logs1;
        Dictionary<int, List<TaskLogPage>> dictionary;
        using (TaskTrackingComponent component = requestContext.CreateComponent<TaskTrackingComponent>(this.DataspaceCategory))
        {
          GetLogsResult logs2 = component.GetLogs(scopeIdentifier, planId, true);
          logs1 = logs2.Logs;
          dictionary = logs2.LogPages.GroupBy<TaskLogPage, int>((Func<TaskLogPage, int>) (i => i.LogId)).ToDictionary<IGrouping<int, TaskLogPage>, int, List<TaskLogPage>>((Func<IGrouping<int, TaskLogPage>, int>) (i => i.Key), (Func<IGrouping<int, TaskLogPage>, List<TaskLogPage>>) (i => i.ToList<TaskLogPage>()));
        }
        Dictionary<int, TeamFoundationLogItemData> logsReaders = new Dictionary<int, TeamFoundationLogItemData>();
        IVssRequestContext requestContext1 = requestContext.Elevate();
        foreach (TaskLog log in logs1)
        {
          List<TaskLogPage> pages;
          if (filterLogIds.Contains(log.Id) && dictionary.TryGetValue(log.Id, out pages))
          {
            CommandGetLogLines2 disposableObject = (CommandGetLogLines2) null;
            try
            {
              disposableObject = new CommandGetLogLines2(requestContext1, planData, log, (IList<TaskLogPage>) pages);
              disposableObject.Execute();
            }
            catch (Exception ex)
            {
              disposableObject?.Dispose();
              throw;
            }
            logsReaders.Add(log.Id, new TeamFoundationLogItemData()
            {
              EndLine = log.LineCount,
              TotalLines = pages.Count == 0 ? 0L : log.LineCount,
              Reader = new TeamFoundationDataReader((IDisposable) disposableObject, new object[1]
              {
                (object) disposableObject.Lines
              })
            });
          }
        }
        return logsReaders;
      }
    }

    public IEnumerable<TaskLog> GetLogs(
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      Guid planId)
    {
      using (new MethodScope(requestContext, nameof (TaskHub), nameof (GetLogs)))
      {
        Microsoft.TeamFoundation.DistributedTask.WebApi.TaskOrchestrationPlanReference planData = this.GetPlanData(requestContext, scopeIdentifier, planId);
        if (planData == null || !this.Extension.HasReadPermission(requestContext, planData.ScopeIdentifier, planData.ArtifactUri))
          return (IEnumerable<TaskLog>) Array.Empty<TaskLog>();
        using (TaskTrackingComponent component = requestContext.CreateComponent<TaskTrackingComponent>(this.DataspaceCategory))
          return component.GetLogs(planData.ScopeIdentifier, planData.PlanId).Logs;
      }
    }

    public async Task<TaskOrchestrationPlan> GetPlanAsync(
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      Guid planId,
      bool includeSecretVariables = false)
    {
      using (new MethodScope(requestContext, nameof (TaskHub), nameof (GetPlanAsync)))
      {
        Microsoft.TeamFoundation.DistributedTask.WebApi.TaskOrchestrationPlanReference planData = this.GetPlanData(requestContext, scopeIdentifier, planId);
        if (planData == null || !this.Extension.HasReadPermission(requestContext, planData.ScopeIdentifier, planData.ArtifactUri))
          return (TaskOrchestrationPlan) null;
        TaskOrchestrationPlan planAsync;
        using (TaskTrackingComponent trackingComponent = requestContext.CreateComponent<TaskTrackingComponent>(this.DataspaceCategory))
          planAsync = await trackingComponent.GetPlanAsync(planData.ScopeIdentifier, planData.PlanId);
        if (!includeSecretVariables || !(planAsync?.ProcessEnvironment is PipelineEnvironment processEnvironment))
          return this.GetSafePlan(planAsync);
        PlanSecretStore planSecretStore = requestContext.IsSystemContext ? new PlanSecretStore(requestContext, planId) : throw new TaskOrchestrationPlanNotFoundException(TaskResources.PlanNotFound((object) planId));
        if (processEnvironment.Version < 2)
        {
          foreach (string key in (IEnumerable<string>) processEnvironment.Variables.Keys)
          {
            VariableValue variable = processEnvironment.Variables[key];
            if (variable.IsSecret)
              variable.Value = planSecretStore.GetVariable(key);
          }
        }
        else
        {
          foreach (IVariable userVariable in (IEnumerable<IVariable>) processEnvironment.UserVariables)
          {
            if (userVariable is Variable variable && variable.Secret)
              variable.Value = planSecretStore.GetVariable(variable.Name);
          }
        }
        return planAsync;
      }
    }

    public List<TaskOrchestrationPlan> GetPlans(
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      IList<Guid> planIds,
      IList<string> timelineRecordTypes)
    {
      ArgumentUtility.CheckForNull<IList<Guid>>(planIds, nameof (planIds));
      ArgumentUtility.CheckForNull<IList<string>>(timelineRecordTypes, nameof (timelineRecordTypes));
      if (planIds.Count == 0)
        return new List<TaskOrchestrationPlan>();
      using (new MethodScope(requestContext, nameof (TaskHub), nameof (GetPlans)))
      {
        IList<TaskOrchestrationPlan> source = (IList<TaskOrchestrationPlan>) null;
        using (TaskTrackingComponent component = requestContext.CreateComponent<TaskTrackingComponent>(this.DataspaceCategory))
          source = component.GetPlans(scopeIdentifier, planIds, timelineRecordTypes);
        return source.Where<TaskOrchestrationPlan>((Func<TaskOrchestrationPlan, bool>) (plan => this.Extension.HasReadPermission(requestContext, plan.ScopeIdentifier, plan.ArtifactUri))).Select<TaskOrchestrationPlan, TaskOrchestrationPlan>((Func<TaskOrchestrationPlan, TaskOrchestrationPlan>) (plan => this.GetSafePlan(plan))).ToList<TaskOrchestrationPlan>();
      }
    }

    public List<TaskOrchestrationPlan> GetRunningPlansByDefinition(
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      int definitionId,
      IList<string> timelineRecordTypes,
      int maxPlans = 100)
    {
      ArgumentUtility.CheckForNull<IList<string>>(timelineRecordTypes, nameof (timelineRecordTypes));
      using (new MethodScope(requestContext, nameof (TaskHub), nameof (GetRunningPlansByDefinition)))
      {
        IList<TaskOrchestrationPlan> source = (IList<TaskOrchestrationPlan>) null;
        using (TaskTrackingComponent component = requestContext.CreateComponent<TaskTrackingComponent>(this.DataspaceCategory))
          source = component.GetRunningPlansByDefinition(scopeIdentifier, definitionId, timelineRecordTypes, maxPlans);
        return source.Where<TaskOrchestrationPlan>((Func<TaskOrchestrationPlan, bool>) (plan => this.Extension.HasReadPermission(requestContext, plan.ScopeIdentifier, plan.ArtifactUri))).Select<TaskOrchestrationPlan, TaskOrchestrationPlan>(new Func<TaskOrchestrationPlan, TaskOrchestrationPlan>(this.GetSafePlan)).ToList<TaskOrchestrationPlan>();
      }
    }

    public void CacheSecurityTokens(
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      IDictionary<Uri, string> tokens)
    {
      this.Extension?.CacheSecurityTokens(requestContext, scopeIdentifier, tokens);
    }

    internal TaskOrchestrationQueuedPlanGroup GetQueuedPlanGroup(
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      string planGroup)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(planGroup, nameof (planGroup));
      using (new MethodScope(requestContext, nameof (TaskHub), nameof (GetQueuedPlanGroup)))
      {
        IList<TaskOrchestrationPlan> source = (IList<TaskOrchestrationPlan>) null;
        using (TaskTrackingComponent component = requestContext.CreateComponent<TaskTrackingComponent>(this.DataspaceCategory))
          source = component.GetPlansByPlanGroup(this.Name, scopeIdentifier, planGroup);
        if (!source.Any<TaskOrchestrationPlan>((Func<TaskOrchestrationPlan, bool>) (x => x.State != TaskOrchestrationPlanState.Completed)))
          return (TaskOrchestrationQueuedPlanGroup) null;
        TaskOrchestrationQueuedPlanGroup planGroup1 = new TaskOrchestrationQueuedPlanGroup();
        planGroup1.Project = new Microsoft.TeamFoundation.DistributedTask.WebApi.ProjectReference()
        {
          Id = source[0].ScopeIdentifier
        };
        planGroup1.PlanGroup = source[0].PlanGroup;
        planGroup1.Definition = source[0].Definition;
        planGroup1.Owner = source[0].Owner;
        planGroup1.QueuePosition = 0;
        planGroup1.Plans.AddRange(source.Select<TaskOrchestrationPlan, TaskOrchestrationQueuedPlan>((Func<TaskOrchestrationPlan, TaskOrchestrationQueuedPlan>) (x => x.AsQueuedPlan())));
        this.PopulateTeamProjectReferences(requestContext, planGroup1);
        return planGroup1;
      }
    }

    internal IList<TaskOrchestrationPlan> GetThrottledPlans(
      IVssRequestContext requestContext,
      int? count)
    {
      using (new MethodScope(requestContext, nameof (TaskHub), nameof (GetThrottledPlans)))
      {
        IList<TaskOrchestrationPlan> throttledPlans = (IList<TaskOrchestrationPlan>) null;
        using (TaskTrackingComponent component = requestContext.CreateComponent<TaskTrackingComponent>(this.DataspaceCategory))
          throttledPlans = component.GetPlansByState(this.Name, TaskOrchestrationPlanState.Throttled, count);
        return throttledPlans;
      }
    }

    public async Task<Microsoft.TeamFoundation.DistributedTask.WebApi.Timeline> GetTimelineAsync(
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      Guid planId,
      Guid timelineId,
      int changeId = 0,
      bool includeRecords = false,
      bool includeSecretVariables = false,
      bool includePreviousAttempts = true)
    {
      using (new MethodScope(requestContext, nameof (TaskHub), nameof (GetTimelineAsync)))
      {
        Microsoft.TeamFoundation.DistributedTask.WebApi.TaskOrchestrationPlanReference planData = this.GetPlanData(requestContext, scopeIdentifier, planId);
        if (planData == null || !this.Extension.HasReadPermission(requestContext, planData.ScopeIdentifier, planData.ArtifactUri))
          return (Microsoft.TeamFoundation.DistributedTask.WebApi.Timeline) null;
        Microsoft.TeamFoundation.DistributedTask.WebApi.Timeline timelineAsync;
        using (TaskTrackingComponent trackingComponent = requestContext.CreateComponent<TaskTrackingComponent>(this.DataspaceCategory))
          timelineAsync = await trackingComponent.GetTimelineAsync(planData.ScopeIdentifier, planData.PlanId, timelineId, changeId, includeRecords, includePreviousAttempts);
        if (timelineAsync != null & includeRecords & includeSecretVariables && timelineAsync.Records.Any<Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord>((Func<Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord, bool>) (r => r.Variables.Any<KeyValuePair<string, VariableValue>>((Func<KeyValuePair<string, VariableValue>, bool>) (v => v.Value.IsSecret)))))
          this.PopulateTimelineRecordSecretVariables(requestContext, planId, timelineAsync.Id, (IList<Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord>) timelineAsync.Records);
        return timelineAsync;
      }
    }

    public async Task<IList<Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord>> GetTimelineRecordsAsync(
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      Guid planId,
      Guid timelineId,
      IEnumerable<Guid> recordIds,
      bool includeSecretVariables = false)
    {
      using (new MethodScope(requestContext, nameof (TaskHub), nameof (GetTimelineRecordsAsync)))
      {
        Microsoft.TeamFoundation.DistributedTask.WebApi.TaskOrchestrationPlanReference planData = this.GetPlanData(requestContext, scopeIdentifier, planId);
        if (planData == null || !this.Extension.HasReadPermission(requestContext, planData.ScopeIdentifier, planData.ArtifactUri))
          return (IList<Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord>) null;
        IList<Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord> timelineRecordsAsync;
        using (TaskTrackingComponent tc = this.CreateComponent<TaskTrackingComponent>(requestContext))
          timelineRecordsAsync = await tc.GetTimelineRecordsAsync(scopeIdentifier, planId, timelineId, recordIds, true);
        if (timelineRecordsAsync != null & includeSecretVariables && timelineRecordsAsync.Any<Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord>((Func<Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord, bool>) (r => r.Variables.Any<KeyValuePair<string, VariableValue>>((Func<KeyValuePair<string, VariableValue>, bool>) (v => v.Value.IsSecret)))))
          this.PopulateTimelineRecordSecretVariables(requestContext, planId, timelineId, timelineRecordsAsync);
        return timelineRecordsAsync;
      }
    }

    private void PopulateTimelineRecordSecretVariables(
      IVssRequestContext requestContext,
      Guid planId,
      Guid timelineId,
      IList<Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord> records)
    {
      PlanSecretStore planSecretStore = new PlanSecretStore(requestContext, planId);
      foreach (Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord record in (IEnumerable<Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord>) records)
      {
        foreach (KeyValuePair<string, VariableValue> keyValuePair in record.Variables.Where<KeyValuePair<string, VariableValue>>((Func<KeyValuePair<string, VariableValue>, bool>) (x => x.Value.IsSecret)))
        {
          try
          {
            keyValuePair.Value.Value = planSecretStore.GetOutputVariable(timelineId, record.Id, keyValuePair.Key);
          }
          catch (StrongBoxItemNotFoundException ex)
          {
            requestContext.TraceError(10015523, "Orchestration", string.Format("Secret output variable {0} is missig for timeline record {1}. Lookup Key: {2}", (object) keyValuePair.Key, (object) record.Id, (object) keyValuePair.Key));
          }
        }
      }
    }

    public async Task<OutputVariableScope> GetPlanOutputVariablesAsync(
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      Guid planId,
      Microsoft.TeamFoundation.DistributedTask.WebApi.Timeline timeline = null)
    {
      using (new MethodScope(requestContext, nameof (TaskHub), nameof (GetPlanOutputVariablesAsync)))
      {
        if (timeline == null)
          timeline = await this.GetTimelineAsync(requestContext, scopeIdentifier, planId, Guid.Empty, includeRecords: true, includeSecretVariables: true, includePreviousAttempts: false);
        if (timeline == null)
          return (OutputVariableScope) null;
        timeline.Records.ToDictionary<Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord, Guid>((Func<Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord, Guid>) (k => k.Id));
        Dictionary<Guid, List<Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord>> dictionary = timeline.Records.GroupBy<Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord, Guid>((Func<Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord, Guid>) (x => x.ParentId ?? Guid.Empty)).ToDictionary<IGrouping<Guid, Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord>, Guid, List<Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord>>((Func<IGrouping<Guid, Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord>, Guid>) (x => x.Key), (Func<IGrouping<Guid, Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord>, List<Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord>>) (x => x.ToList<Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord>()));
        List<Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord> source;
        if (!dictionary.TryGetValue(Guid.Empty, out source))
          return (OutputVariableScope) null;
        OutputVariableScope planScope = new OutputVariableScope()
        {
          Id = planId,
          ScopeType = "Plan"
        };
        foreach (Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord record in source.Where<Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord>((Func<Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord, bool>) (x => planScope.IsValidChild(x.RecordType))))
          planScope.ChildScopes.AddRange((IEnumerable<OutputVariableScope>) TaskHub.ExpandChildren(requestContext, record, (IDictionary<Guid, List<Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord>>) dictionary));
        return planScope;
      }
    }

    public IEnumerable<Microsoft.TeamFoundation.DistributedTask.WebApi.Timeline> GetTimelines(
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      Guid planId)
    {
      using (new MethodScope(requestContext, nameof (TaskHub), nameof (GetTimelines)))
      {
        Microsoft.TeamFoundation.DistributedTask.WebApi.TaskOrchestrationPlanReference orchestrationPlanReference = this.EnsurePlanData(requestContext, scopeIdentifier, planId);
        if (!this.Extension.HasReadPermission(requestContext, orchestrationPlanReference.ScopeIdentifier, orchestrationPlanReference.ArtifactUri))
          return (IEnumerable<Microsoft.TeamFoundation.DistributedTask.WebApi.Timeline>) null;
        using (TaskTrackingComponent component = requestContext.CreateComponent<TaskTrackingComponent>(this.DataspaceCategory))
          return component.GetTimelines(orchestrationPlanReference.ScopeIdentifier, orchestrationPlanReference.PlanId);
      }
    }

    public void CreateScope(IVssRequestContext requestContext, Guid scopeIdentifier)
    {
      using (new MethodScope(requestContext, nameof (TaskHub), nameof (CreateScope)))
        ;
    }

    public void DeleteScope(IVssRequestContext requestContext, Guid scopeIdentifier)
    {
      using (new MethodScope(requestContext, nameof (TaskHub), nameof (DeleteScope)))
      {
        ArgumentUtility.CheckForEmptyGuid(scopeIdentifier, nameof (scopeIdentifier));
        using (TaskTrackingComponent component = requestContext.CreateComponent<TaskTrackingComponent>(this.DataspaceCategory))
          component.DeleteTeamProject(scopeIdentifier);
      }
    }

    public TaskOrchestrationPlan RunPlan(
      IVssRequestContext requestContext,
      Microsoft.TeamFoundation.DistributedTask.WebApi.TaskAgentPoolReference pool,
      Guid scopeIdentifier,
      Guid planId,
      string planGroup,
      PlanTemplateType templateType,
      Uri artifactUri,
      IOrchestrationEnvironment environment,
      IOrchestrationProcess process,
      BuildOptions validationOptions,
      Guid requestedForId,
      TaskOrchestrationOwner definitionReference,
      TaskOrchestrationOwner ownerReference,
      string pipelineInitializationLog = null,
      string pipelineExpandedYaml = null)
    {
      using (new MethodScope(requestContext, nameof (TaskHub), nameof (RunPlan)))
      {
        TaskOrchestrationPlan plan = this.CreatePlan(requestContext, scopeIdentifier, planId, planGroup, templateType, artifactUri, environment, process, validationOptions, requestedForId, definitionReference, ownerReference, pipelineInitializationLog, pipelineExpandedYaml);
        this.StartPlan(requestContext, pool, plan);
        return plan;
      }
    }

    public void StartPlan(
      IVssRequestContext requestContext,
      Microsoft.TeamFoundation.DistributedTask.WebApi.TaskAgentPoolReference pool,
      TaskOrchestrationPlan plan)
    {
      using (new MethodScope(requestContext, nameof (TaskHub), nameof (StartPlan)))
      {
        ArgumentUtility.CheckForNull<TaskOrchestrationPlan>(plan, nameof (plan));
        try
        {
          this.StartQueuedPlan(requestContext, plan, pool?.Id);
        }
        catch (Exception ex)
        {
          requestContext.TraceError(10016171, nameof (TaskHub), string.Format("Failed start queued plan '{0}' with exception: {1}", (object) plan.PlanId, (object) ex));
          throw;
        }
      }
    }

    internal void StartQueuedPlan(
      IVssRequestContext requestContext,
      TaskOrchestrationPlan plan,
      int? poolId,
      IList<StageAttempt> attempts = null)
    {
      using (new MethodScope(requestContext, nameof (TaskHub), nameof (StartQueuedPlan)))
      {
        OrchestrationTracer ciao = this.GetCIAO(requestContext);
        string orchestrationId = plan != null ? plan.GetOrchestrationId() : (string) null;
        if (plan.State == TaskOrchestrationPlanState.Throttled)
        {
          ciao.TracePhaseStarted(orchestrationId, "CIPlatform", "PlanThrottled");
        }
        else
        {
          KPIHelper.PublishDTPlanQueued(requestContext);
          ciao.TracePhaseStarted(orchestrationId, "CIPlatform", "StartPlan");
          try
          {
            string orchestrationName;
            string orchestrationVersion;
            object planInput = this.GetPlanInput(requestContext, poolId, plan, attempts, this.Name, out orchestrationName, out orchestrationVersion);
            OrchestrationHubDescription orchestrationHubDescription = this.EnsureOrchestrationHubExists(requestContext);
            OrchestrationService service = requestContext.GetService<OrchestrationService>();
            try
            {
              service.CreateOrchestrationInstance(requestContext, orchestrationHubDescription.HubName, orchestrationName, orchestrationVersion, plan.GetOrchestrationId(), planInput);
              requestContext.TraceAlways(10015520, nameof (TaskHub), "Started plan: {0}, planVersion: {1}, ArtifactUri: {2}", (object) plan.GetOrchestrationId(), (object) plan.Version, (object) plan.ArtifactUri);
              TelemetryFactory.GetLogger(requestContext).PublishTaskHubPlanStartedTelemetry(requestContext, this, plan);
            }
            catch (OrchestrationSessionExistsException ex)
            {
              throw new TaskOrchestrationPlanAlreadyStartedException(TaskResources.PlanAlreadyStarted((object) plan.PlanId, (object) plan.Version, (object) plan.PlanType), (Exception) ex);
            }
          }
          catch (UnableToDetermineTargetPoolException ex)
          {
            requestContext.TraceException(nameof (TaskHub), (Exception) ex);
            this.PlanCompleted(requestContext, plan.ScopeIdentifier, plan.PlanId, DateTime.UtcNow, Microsoft.TeamFoundation.DistributedTask.WebApi.TaskResult.Failed, typeof (UnableToDetermineTargetPoolException).Name);
          }
        }
      }
    }

    private void EnsureTimelineRecordPermission(
      IVssRequestContext requestContext,
      Microsoft.TeamFoundation.DistributedTask.WebApi.TaskOrchestrationPlanReference planData,
      IList<Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord> records)
    {
      if (requestContext.UsesCustomScopeToken())
      {
        Guid ancestorId = TaskHub.FindAncestorId(records);
        if (ancestorId != Guid.Empty && requestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(requestContext, ScopeTokenHelper.DistributedTaskSecurityNamespaceId).HasPermission(requestContext, ScopeTokenHelper.CreateUpdateTimelineRecordToken(planData.PlanId, ancestorId), 1, false))
          return;
      }
      this.Extension.CheckWritePermission(requestContext, planData.ScopeIdentifier, planData.PlanId, planData.ArtifactUri);
    }

    public async Task<Microsoft.TeamFoundation.DistributedTask.WebApi.Timeline> UpdateTimelineAsync(
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      Guid planId,
      Guid timelineId,
      IList<Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord> records,
      bool waitForNotifications = false)
    {
      Microsoft.TeamFoundation.DistributedTask.WebApi.Timeline timeline;
      using (new MethodScope(requestContext, nameof (TaskHub), nameof (UpdateTimelineAsync)))
      {
        ArgumentValidation.Validate((IEnumerable<Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord>) records, nameof (records));
        Microsoft.TeamFoundation.DistributedTask.WebApi.TaskOrchestrationPlanReference planData = this.EnsurePlanData(requestContext, scopeIdentifier, planId);
        this.EnsureTimelineRecordPermission(requestContext, planData, records);
        Dictionary<Guid, Dictionary<string, VariableValue>> secrets = this.ExtractSecrets(records);
        this.UpdateRecordIssueCount((IEnumerable<Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord>) records);
        Microsoft.TeamFoundation.DistributedTask.WebApi.Timeline updatedTimeline = (Microsoft.TeamFoundation.DistributedTask.WebApi.Timeline) null;
        Microsoft.VisualStudio.Services.Identity.Identity userIdentity = requestContext.GetUserIdentity();
        IVssRegistryService service1 = requestContext.GetService<IVssRegistryService>();
        int blockingPeriod = requestContext.IsFeatureEnabled("DistributedTask.BlockUpdateTimelinesAfterNMinutes") ? service1.GetValue<int>(requestContext, (RegistryQuery) RegistryKeys.PipelineBlockTimelinePeriod, true, 5) : 0;
        using (TaskTrackingComponent trackingComponent = requestContext.CreateComponent<TaskTrackingComponent>(this.DataspaceCategory))
          updatedTimeline = await trackingComponent.UpdateTimelineAsync(planData.ScopeIdentifier, planData.PlanId, timelineId, userIdentity.Id, records, blockingPeriod);
        TaskHub.TraceTimelineRecordUpdates(requestContext, records, (IList<Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord>) updatedTimeline.Records, nameof (UpdateTimelineAsync));
        this.StoreSecrets(requestContext, planId, timelineId, secrets);
        if (updatedTimeline.Records.Count > 0)
        {
          if (waitForNotifications)
            await this.Extension.TimelineRecordsUpdatedAsync(requestContext, planData, (Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineReference) updatedTimeline, (IEnumerable<Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord>) updatedTimeline.Records);
          else
            this.Extension.TimelineRecordsUpdated(requestContext, planData, (Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineReference) updatedTimeline, (IEnumerable<Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord>) updatedTimeline.Records);
          IEnumerable<Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord> source = updatedTimeline.Records.Where<Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord>((Func<Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord, bool>) (r =>
          {
            Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecordState? state = r.State;
            Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecordState timelineRecordState = Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecordState.Completed;
            return state.GetValueOrDefault() == timelineRecordState & state.HasValue && r.RecordType == "Stage";
          }));
          if (source.Any<Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord>())
          {
            ITeamFoundationEventService service2 = requestContext.GetService<ITeamFoundationEventService>();
            BatchStageCompletedEvent stageCompletedEvent = new BatchStageCompletedEvent()
            {
              PlanId = planData.PlanId,
              StageTimelineRecords = source,
              ScopeId = scopeIdentifier
            };
            IVssRequestContext requestContext1 = requestContext;
            BatchStageCompletedEvent notificationEvent = stageCompletedEvent;
            service2.PublishNotification(requestContext1, (object) notificationEvent);
          }
        }
        timeline = updatedTimeline;
      }
      return timeline;
    }

    public static void TraceTimelineRecordUpdates(
      IVssRequestContext requestContext,
      IList<Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord> recordsBeforeUpdate,
      IList<Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord> recordsAfterUpdate,
      string currentMethod)
    {
      bool shouldTraceFailed = requestContext.IsFeatureEnabled("DistributedTask.TraceFailedTimelineRecordUpdates");
      bool shouldTraceCanceled = requestContext.IsFeatureEnabled("DistributedTask.TraceCanceledTimelineRecordUpdates");
      bool shouldTraceSkipped = requestContext.IsFeatureEnabled("DistributedTask.TraceSkippedTimelineRecordUpdates");
      bool shouldTraceAbandoned = requestContext.IsFeatureEnabled("DistributedTask.TraceAbandonedTimelineRecordUpdates");
      if (!(shouldTraceFailed | shouldTraceCanceled | shouldTraceSkipped | shouldTraceAbandoned))
        return;
      if (requestContext.IsFeatureEnabled("DistributedTask.TraceTimelineRecordsBeforeUpdate") && recordsBeforeUpdate != null && recordsBeforeUpdate.Count > 0)
        TaskHub.TraceFilteredTimelineRecordUpdates(requestContext, recordsBeforeUpdate, currentMethod, true, shouldTraceFailed, shouldTraceCanceled, shouldTraceSkipped, shouldTraceAbandoned);
      if (recordsAfterUpdate == null || recordsAfterUpdate.Count <= 0)
        return;
      TaskHub.TraceFilteredTimelineRecordUpdates(requestContext, recordsAfterUpdate, currentMethod, false, shouldTraceFailed, shouldTraceCanceled, shouldTraceSkipped, shouldTraceAbandoned);
    }

    private static void TraceFilteredTimelineRecordUpdates(
      IVssRequestContext requestContext,
      IList<Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord> records,
      string currentMethod,
      bool isBeforeUpdate,
      bool shouldTraceFailed,
      bool shouldTraceCanceled,
      bool shouldTraceSkipped,
      bool shouldTraceAbandoned)
    {
      Dictionary<Microsoft.TeamFoundation.DistributedTask.WebApi.TaskResult?, List<Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord>> dictionary = records.Where<Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord>((Func<Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord, bool>) (record => record.Result.HasValue)).GroupBy<Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord, Microsoft.TeamFoundation.DistributedTask.WebApi.TaskResult?>((Func<Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord, Microsoft.TeamFoundation.DistributedTask.WebApi.TaskResult?>) (record => record.Result)).ToDictionary<IGrouping<Microsoft.TeamFoundation.DistributedTask.WebApi.TaskResult?, Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord>, Microsoft.TeamFoundation.DistributedTask.WebApi.TaskResult?, List<Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord>>((Func<IGrouping<Microsoft.TeamFoundation.DistributedTask.WebApi.TaskResult?, Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord>, Microsoft.TeamFoundation.DistributedTask.WebApi.TaskResult?>) (group => group.Key), (Func<IGrouping<Microsoft.TeamFoundation.DistributedTask.WebApi.TaskResult?, Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord>, List<Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord>>) (group => group.ToList<Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord>()));
      IList<Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord> timelineRecordList = (IList<Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord>) new List<Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord>();
      TaskHub.AddTimelineRecordsForTracing(shouldTraceFailed, Microsoft.TeamFoundation.DistributedTask.WebApi.TaskResult.Failed, dictionary, timelineRecordList);
      TaskHub.AddTimelineRecordsForTracing(shouldTraceCanceled, Microsoft.TeamFoundation.DistributedTask.WebApi.TaskResult.Canceled, dictionary, timelineRecordList);
      TaskHub.AddTimelineRecordsForTracing(shouldTraceSkipped, Microsoft.TeamFoundation.DistributedTask.WebApi.TaskResult.Skipped, dictionary, timelineRecordList);
      TaskHub.AddTimelineRecordsForTracing(shouldTraceAbandoned, Microsoft.TeamFoundation.DistributedTask.WebApi.TaskResult.Abandoned, dictionary, timelineRecordList);
      if (timelineRecordList.Count <= 0)
        return;
      requestContext.TraceAlways(10016174, nameof (TaskHub), "(" + currentMethod + (isBeforeUpdate ? ") BEFORE" : ") AFTER") + " DB update of records with results Failed: " + shouldTraceFailed.ToString() + ", Canceled: " + shouldTraceCanceled.ToString() + ", Skipped: " + shouldTraceSkipped.ToString() + ", Abandoned: " + shouldTraceAbandoned.ToString() + " - {0}", (object) timelineRecordList.Select<Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord, string>((Func<Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord, string>) (record => record.ToStringExtended())).Serialize<IEnumerable<string>>());
    }

    private static void AddTimelineRecordsForTracing(
      bool shouldAddRecords,
      Microsoft.TeamFoundation.DistributedTask.WebApi.TaskResult recordResult,
      Dictionary<Microsoft.TeamFoundation.DistributedTask.WebApi.TaskResult?, List<Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord>> recordsByResult,
      IList<Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord> recordsToBeTraced)
    {
      List<Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord> values;
      if (!shouldAddRecords || !recordsByResult.TryGetValue(new Microsoft.TeamFoundation.DistributedTask.WebApi.TaskResult?(recordResult), out values) || values.Count <= 0)
        return;
      recordsToBeTraced.AddRange<Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord, IList<Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord>>((IEnumerable<Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord>) values);
    }

    public async Task UpdateTimelineRecordAsync(
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      Guid planId,
      Guid timelineId,
      Guid recordId,
      Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecordState? state = null,
      DateTime? startTime = null,
      DateTime? finishTime = null,
      string currentOperation = null,
      int? percentComplete = null,
      Microsoft.TeamFoundation.DistributedTask.WebApi.TaskResult? result = null,
      string resultCode = null,
      TaskLogReference log = null,
      IEnumerable<Microsoft.TeamFoundation.DistributedTask.WebApi.Issue> issues = null,
      int? queueId = null,
      JObject agentSpecification = null)
    {
      TaskHub taskHub = this;
      MethodScope methodScope = new MethodScope(requestContext, nameof (TaskHub), nameof (UpdateTimelineRecordAsync));
      try
      {
        Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord timelineRecord = new Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord()
        {
          Id = recordId,
          CurrentOperation = currentOperation,
          PercentComplete = percentComplete,
          StartTime = startTime,
          FinishTime = finishTime,
          State = state,
          Result = result,
          ResultCode = resultCode,
          Log = log,
          QueueId = queueId,
          AgentSpecification = agentSpecification
        };
        if (issues != null)
          timelineRecord.Issues.AddRange(issues.Select<Microsoft.TeamFoundation.DistributedTask.WebApi.Issue, Microsoft.TeamFoundation.DistributedTask.WebApi.Issue>((Func<Microsoft.TeamFoundation.DistributedTask.WebApi.Issue, Microsoft.TeamFoundation.DistributedTask.WebApi.Issue>) (x => x.Clone())));
        Microsoft.TeamFoundation.DistributedTask.WebApi.Timeline timeline = await taskHub.UpdateTimelineAsync(requestContext, scopeIdentifier, planId, timelineId, (IList<Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord>) new Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord[1]
        {
          timelineRecord
        });
      }
      finally
      {
        methodScope.Dispose();
      }
      methodScope = new MethodScope();
    }

    private void UpdateRecordIssueCount(IEnumerable<Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord> records)
    {
      foreach (Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord record in records)
      {
        int? nullable = record.WarningCount;
        int valueOrDefault1 = nullable.GetValueOrDefault();
        nullable = record.ErrorCount;
        int valueOrDefault2 = nullable.GetValueOrDefault();
        if (valueOrDefault1 + valueOrDefault2 <= record.Issues.Count)
        {
          int num1 = 0;
          int num2 = 0;
          foreach (Microsoft.TeamFoundation.DistributedTask.WebApi.Issue issue in record.Issues)
          {
            if (issue.Type == Microsoft.TeamFoundation.DistributedTask.WebApi.IssueType.Error)
              ++num2;
            else if (issue.Type == Microsoft.TeamFoundation.DistributedTask.WebApi.IssueType.Warning)
              ++num1;
          }
          if (num1 > 0)
            record.WarningCount = new int?(num1);
          if (num2 > 0)
            record.ErrorCount = new int?(num2);
        }
      }
    }

    private bool IsLogLineStorageEnabled(IVssRequestContext requestContext) => requestContext.ExecutionEnvironment.IsHostedDeployment;

    private async Task CreateLogTableAsync(
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      Guid planId)
    {
      MethodScope methodScope = new MethodScope(requestContext, nameof (TaskHub), nameof (CreateLogTableAsync));
      try
      {
        if (this.IsLogLineStorageEnabled(requestContext))
        {
          try
          {
            Microsoft.TeamFoundation.DistributedTask.WebApi.TaskOrchestrationPlanReference planData = this.GetPlanData(requestContext, scopeIdentifier, planId);
            if (planData == null)
              return;
            string tableName = string.Format("ds{0}plan{1}", (object) this.DataspaceCategory, (object) planId.ToString("N").ToUpper());
            ILogLineService logLineService = requestContext.GetService<ILogLineService>();
            string storageAccountKey;
            logLineService.CreateLogTable(requestContext, tableName, out storageAccountKey);
            using (TaskTrackingComponent trackingComponent = requestContext.CreateComponent<TaskTrackingComponent>(this.DataspaceCategory))
            {
              DateTime utcNow = DateTime.UtcNow;
              DateTime expiryOn = utcNow.AddHours(168.0);
              if (await trackingComponent.CreateLogTableAsync(planData.ScopeIdentifier, planData.PlanId, storageAccountKey, tableName, utcNow, expiryOn) == null)
                logLineService.DeleteLogTable(requestContext, tableName, storageAccountKey);
            }
            tableName = (string) null;
            logLineService = (ILogLineService) null;
            storageAccountKey = (string) null;
          }
          catch (Exception ex)
          {
            requestContext.TraceException(10016211, nameof (TaskHub), ex);
          }
        }
      }
      finally
      {
        methodScope.Dispose();
      }
      methodScope = new MethodScope();
    }

    private async Task<PlanLogTable> ReadLogTableFromDatabaseAsync(
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      Guid planId)
    {
      Microsoft.TeamFoundation.DistributedTask.WebApi.TaskOrchestrationPlanReference planData = this.GetPlanData(requestContext, scopeIdentifier, planId);
      if (planData == null)
        return (PlanLogTable) null;
      using (TaskTrackingComponent trackingComponent = requestContext.CreateComponent<TaskTrackingComponent>(this.DataspaceCategory))
        return await trackingComponent.GetLogTableAsync(planData.ScopeIdentifier, planData.PlanId);
    }

    private async Task<PlanLogTable> GetLogTableAsync(
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      Guid planId)
    {
      TaskHub taskHub = this;
      using (new MethodScope(requestContext, nameof (TaskHub), nameof (GetLogTableAsync)))
      {
        try
        {
          return taskHub.GetPlanData(requestContext, scopeIdentifier, planId) == null ? (PlanLogTable) null : await requestContext.GetService<PlanLogTableMemoryCache>().GetLogTableAsync(requestContext, scopeIdentifier, planId, new Func<IVssRequestContext, Guid, Guid, Task<PlanLogTable>>(taskHub.ReadLogTableFromDatabaseAsync));
        }
        catch (Exception ex)
        {
          requestContext.TraceException(10016211, nameof (TaskHub), ex);
          return (PlanLogTable) null;
        }
      }
    }

    private async Task MarkLogTableAsCompletedAsync(
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      Guid planId)
    {
      MethodScope methodScope = new MethodScope(requestContext, nameof (TaskHub), nameof (MarkLogTableAsCompletedAsync));
      try
      {
        if (this.IsLogLineStorageEnabled(requestContext))
        {
          try
          {
            PlanLogTable logTableAsync = await this.GetLogTableAsync(requestContext, scopeIdentifier, planId);
            if (logTableAsync == null)
              return;
            using (TaskTrackingComponent trackingComponent = requestContext.CreateComponent<TaskTrackingComponent>(this.DataspaceCategory))
            {
              logTableAsync.CompletedOn = new DateTime?(DateTime.UtcNow);
              logTableAsync.ExpiryOn = new DateTime?(logTableAsync.CompletedOn.Value.AddHours(1.0));
              PlanLogTable planLogTable = await trackingComponent.UpdateLogTableAsync(scopeIdentifier, planId, logTableAsync);
            }
          }
          catch (Exception ex)
          {
            requestContext.TraceException(10016211, nameof (TaskHub), ex);
          }
        }
      }
      finally
      {
        methodScope.Dispose();
      }
      methodScope = new MethodScope();
    }

    private async Task<int> GetLogLinesPostFrequency(
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      Guid planId)
    {
      TaskOrchestrationPlan planAsync = await this.GetPlanAsync(requestContext, scopeIdentifier, planId);
      int? postFrequencyAsync = await this.Extension.GetLogLinePostFrequencyAsync(requestContext, planAsync);
      return postFrequencyAsync.HasValue ? postFrequencyAsync.Value : this.GetLogLinesSpeedConstants(requestContext).FastPostLines;
    }

    public (int SlowPostLines, int FastPostLines) GetLogLinesSpeedConstants(
      IVssRequestContext requestContext)
    {
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      int num = service.GetValue<int>(requestContext, (RegistryQuery) RegistryKeys.FastPostLinesSpeedKey, true, PipelineConstants.PostLinesFrequencyInMilliseconds.Fast);
      return (service.GetValue<int>(requestContext, (RegistryQuery) RegistryKeys.SlowPostLinesSpeedKey, true, PipelineConstants.PostLinesFrequencyInMilliseconds.Slow), num);
    }

    public async Task<TimelineRecordLogLineResult> GetLogLinesAsync(
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      Guid planId,
      Guid timelineId,
      Guid jobTimelineRecordId,
      Guid stepTimelineRecordId,
      string continuationToken,
      long? endLine = null,
      int? takeCount = null)
    {
      using (new MethodScope(requestContext, nameof (TaskHub), nameof (GetLogLinesAsync)))
      {
        int num;
        if (num != 0 && !this.IsLogLineStorageEnabled(requestContext))
          return (TimelineRecordLogLineResult) null;
        try
        {
          PlanLogTable logTableAsync = await this.GetLogTableAsync(requestContext, scopeIdentifier, planId);
          if (logTableAsync == null)
            return (TimelineRecordLogLineResult) null;
          requestContext.TraceAlways(10016212, nameof (TaskHub), "PlanId {0}, IsContinuationToken {1}", (object) planId, (object) (continuationToken != null));
          return requestContext.GetService<ILogLineService>().QueryLogLines(requestContext, logTableAsync.StorageKey, logTableAsync.TableName, planId, timelineId, jobTimelineRecordId, stepTimelineRecordId, continuationToken, endLine, takeCount);
        }
        catch (Exception ex)
        {
          requestContext.TraceException(10016211, nameof (TaskHub), ex);
          return (TimelineRecordLogLineResult) null;
        }
      }
    }

    public async Task InsertLogLinesAsync(
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      Guid planId,
      Guid timelineId,
      Guid jobTimelineRecordId,
      TimelineRecordFeedLinesWrapper lines)
    {
      MethodScope methodScope = new MethodScope(requestContext, nameof (TaskHub), nameof (InsertLogLinesAsync));
      try
      {
        if (this.IsLogLineStorageEnabled(requestContext))
        {
          if (lines.StartLine.HasValue)
          {
            try
            {
              PlanLogTable logTableAsync = await this.GetLogTableAsync(requestContext, scopeIdentifier, planId);
              if (logTableAsync == null)
                return;
              long currentLine = lines.StartLine.Value;
              IList<TimelineRecordLogLine> list = (IList<TimelineRecordLogLine>) lines.Value.Select<string, TimelineRecordLogLine>((Func<string, TimelineRecordLogLine>) (x => new TimelineRecordLogLine(x, currentLine++))).ToList<TimelineRecordLogLine>();
              requestContext.GetService<ILogLineService>().InsertLogLines(requestContext, logTableAsync.StorageKey, logTableAsync.TableName, planId, timelineId, jobTimelineRecordId, lines.StepId, list);
            }
            catch (Exception ex)
            {
              requestContext.TraceException(10016211, nameof (TaskHub), ex);
            }
          }
        }
      }
      finally
      {
        methodScope.Dispose();
      }
      methodScope = new MethodScope();
    }

    public async Task<IEnumerable<ResourceInfo>> GetReferencedResourcesAsync(
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      Guid planId,
      string nodeId,
      string nodeInstanceName = null)
    {
      using (new MethodScope(requestContext, nameof (TaskHub), nameof (GetReferencedResourcesAsync)))
      {
        List<ResourceInfo> result = new List<ResourceInfo>();
        bool useScopeTokenSecurity = requestContext.UsesCustomScopeToken();
        IVssRequestContext potentiallyElevatedContext = requestContext;
        if (useScopeTokenSecurity)
        {
          if (!requestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(requestContext, ScopeTokenHelper.DistributedTaskSecurityNamespaceId).HasPermission(requestContext, ScopeTokenHelper.CreateGetReferencedResourcesToken(planId, nodeId), 1, false))
            return (IEnumerable<ResourceInfo>) result;
          potentiallyElevatedContext = requestContext.Elevate();
          if (HttpContext.Current?.User is ClaimsPrincipal user)
          {
            Claim first = user.FindFirst(nameof (nodeInstanceName));
            if (first != null)
              nodeInstanceName = first.Value;
          }
        }
        ArgumentUtility.CheckForNull<string>(nodeInstanceName, nameof (nodeInstanceName));
        TaskOrchestrationPlan planAsync = await this.GetPlanAsync(potentiallyElevatedContext, scopeIdentifier, planId);
        if (planAsync == null || !(planAsync.Process is PipelineProcess process1) || !useScopeTokenSecurity && !this.Extension.HasReadPermission(potentiallyElevatedContext, planAsync.ScopeIdentifier, planAsync.ArtifactUri))
          return (IEnumerable<ResourceInfo>) result;
        IList<string> pathComponents = PipelineUtilities.GetPathComponents(nodeInstanceName);
        PipelineResources pipelineResources1 = new PipelineResources();
        List<Microsoft.TeamFoundation.DistributedTask.Pipelines.ResourceReference> resourceReferenceList = new List<Microsoft.TeamFoundation.DistributedTask.Pipelines.ResourceReference>();
        bool includeRepositoryReferences = this.Extension.IsEnforceReferencedRepoScopedTokenEnabled(requestContext, scopeIdentifier);
        PipelineProcess process2;
        if (pathComponents == null || pathComponents.Count<string>() == 0)
        {
          process2 = process1;
        }
        else
        {
          Microsoft.TeamFoundation.DistributedTask.Pipelines.IGraphNode nodeAtPath = process1.GetNodeAtPath(pathComponents);
          if (nodeAtPath is PhaseNode phaseNode)
            process2 = new PipelineProcess((IList<PhaseNode>) new PhaseNode[1]
            {
              phaseNode
            });
          else if (nodeAtPath is Stage stage)
          {
            process2 = new PipelineProcess((IList<Stage>) new Stage[1]
            {
              stage
            });
          }
          else
          {
            JobParameters jobParams;
            if (!JobParameters.TryParseInstanceName(nodeInstanceName, out jobParams))
              throw new TaskOrchestrationPlanTerminatedException("'" + nodeInstanceName + "' is not a valid job instance.");
            JobExecutionContext executionContext = await this.GetJobExecutionContext(requestContext, scopeIdentifier, planAsync, jobParams);
            PipelineResources pipelineResources2 = executionContext != null && executionContext.Job != null ? executionContext.ReferencedResources : throw new TaskOrchestrationPlanTerminatedException("'" + nodeInstanceName + "' is not a valid node instance.");
            if (executionContext.Job.Definition.Target is AgentQueueTarget target)
              pipelineResources2.AddAgentQueueReference(target.Queue);
            if (pipelineResources2 != null)
              result.AddRange(pipelineResources2.GetSecurableResources(includeRepositoryReferences).ToList<Microsoft.TeamFoundation.DistributedTask.Pipelines.ResourceReference>().Where<Microsoft.TeamFoundation.DistributedTask.Pipelines.ResourceReference>((Func<Microsoft.TeamFoundation.DistributedTask.Pipelines.ResourceReference, bool>) (x => x != null)).Select<Microsoft.TeamFoundation.DistributedTask.Pipelines.ResourceReference, ResourceInfo>((Func<Microsoft.TeamFoundation.DistributedTask.Pipelines.ResourceReference, ResourceInfo>) (x => new ResourceInfo()
              {
                TypeName = x.GetType().FullName,
                Name = x.Name?.ToString(),
                Id = GetId((object) x)
              })));
            return (IEnumerable<ResourceInfo>) result;
          }
        }
        PipelineBuilder builder = potentiallyElevatedContext.GetService<IPipelineBuilderService>().GetBuilder(potentiallyElevatedContext, planAsync);
        PipelineBuildContext buildContext = builder.CreateBuildContext(BuildOptions.None);
        buildContext.BuildOptions.ResolvePersistedStages = true;
        PipelineResources referencedResources = buildContext.Validate(process2).ReferencedResources;
        PipelineEnvironment environment = planAsync.GetEnvironment<PipelineEnvironment>();
        IEnumerable<Microsoft.TeamFoundation.DistributedTask.Pipelines.VariableGroupReference> variableGroupReferences;
        if (environment == null)
        {
          variableGroupReferences = (IEnumerable<Microsoft.TeamFoundation.DistributedTask.Pipelines.VariableGroupReference>) null;
        }
        else
        {
          IList<IVariable> userVariables = environment.UserVariables;
          variableGroupReferences = userVariables != null ? userVariables.Where<IVariable>((Func<IVariable, bool>) (x => x.Type == VariableType.Group)).Cast<Microsoft.TeamFoundation.DistributedTask.Pipelines.VariableGroupReference>() : (IEnumerable<Microsoft.TeamFoundation.DistributedTask.Pipelines.VariableGroupReference>) null;
        }
        IEnumerable<Microsoft.TeamFoundation.DistributedTask.Pipelines.VariableGroupReference> values = variableGroupReferences;
        if (values != null)
          referencedResources.VariableGroups.AddRange<Microsoft.TeamFoundation.DistributedTask.Pipelines.VariableGroupReference, ISet<Microsoft.TeamFoundation.DistributedTask.Pipelines.VariableGroupReference>>(values);
        IResourceStore store = builder.ResourceStore;
        List<Microsoft.TeamFoundation.DistributedTask.Pipelines.ResourceReference> list = referencedResources != null ? referencedResources.GetSecurableResources(includeRepositoryReferences).Select<Microsoft.TeamFoundation.DistributedTask.Pipelines.ResourceReference, Microsoft.TeamFoundation.DistributedTask.Pipelines.ResourceReference>((Func<Microsoft.TeamFoundation.DistributedTask.Pipelines.ResourceReference, Microsoft.TeamFoundation.DistributedTask.Pipelines.ResourceReference>) (x => store.GetSnappedReference(x))).ToList<Microsoft.TeamFoundation.DistributedTask.Pipelines.ResourceReference>() : (List<Microsoft.TeamFoundation.DistributedTask.Pipelines.ResourceReference>) null;
        if (list == null || list.Count<Microsoft.TeamFoundation.DistributedTask.Pipelines.ResourceReference>() == 0)
          return (IEnumerable<ResourceInfo>) result;
        result.AddRange(list.Where<Microsoft.TeamFoundation.DistributedTask.Pipelines.ResourceReference>((Func<Microsoft.TeamFoundation.DistributedTask.Pipelines.ResourceReference, bool>) (x => x != null)).Select<Microsoft.TeamFoundation.DistributedTask.Pipelines.ResourceReference, ResourceInfo>((Func<Microsoft.TeamFoundation.DistributedTask.Pipelines.ResourceReference, ResourceInfo>) (x => new ResourceInfo()
        {
          TypeName = x.GetType().FullName,
          Name = x.Name?.ToString(),
          Id = GetId((object) x)
        })));
        return (IEnumerable<ResourceInfo>) result;
      }

      static string GetId(object o)
      {
        string id = (string) null;
        switch (o)
        {
          case Microsoft.TeamFoundation.DistributedTask.Pipelines.ServiceEndpointReference endpointReference:
            id = endpointReference.Id.ToString("D");
            break;
          case AgentQueueReference agentQueueReference:
            id = agentQueueReference.Id.ToString();
            break;
          case AgentPoolReference agentPoolReference:
            id = agentPoolReference.Id.ToString();
            break;
          case Microsoft.TeamFoundation.DistributedTask.Pipelines.SecureFileReference secureFileReference:
            id = secureFileReference.Id.ToString("D");
            break;
          case Microsoft.TeamFoundation.DistributedTask.Pipelines.VariableGroupReference variableGroupReference:
            id = variableGroupReference.Id.ToString();
            break;
          case Microsoft.TeamFoundation.DistributedTask.Pipelines.EnvironmentReference environmentReference:
            id = environmentReference.Id.ToString();
            break;
          case RepositoryReference repositoryReference:
            id = repositoryReference.Id.ToString();
            break;
          case PersistedStageReference persistedStageReference:
            id = persistedStageReference.Id.ToString();
            break;
        }
        return id;
      }
    }

    public void RaiseJobCompletedEvent(
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      Guid planId,
      Guid jobId,
      JobCompletedEvent eventData)
    {
      using (new MethodScope(requestContext, nameof (TaskHub), nameof (RaiseJobCompletedEvent)))
      {
        ArgumentUtility.CheckForEmptyGuid(planId, nameof (planId));
        ArgumentUtility.CheckForEmptyGuid(jobId, nameof (jobId));
        ArgumentUtility.CheckForNull<JobCompletedEvent>(eventData, nameof (eventData));
        this.RaiseJobEvent(requestContext, scopeIdentifier, planId, jobId, eventData.Name, (JobEvent) eventData);
      }
    }

    public void RaiseTaskCompletedEvent(
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      Guid planId,
      Guid jobId,
      Guid taskId,
      TaskCompletedEvent eventData)
    {
      using (new MethodScope(requestContext, nameof (TaskHub), nameof (RaiseTaskCompletedEvent)))
      {
        ArgumentUtility.CheckForEmptyGuid(planId, nameof (planId));
        ArgumentUtility.CheckForEmptyGuid(jobId, nameof (jobId));
        ArgumentUtility.CheckForEmptyGuid(taskId, nameof (taskId));
        ArgumentUtility.CheckForNull<TaskCompletedEvent>(eventData, nameof (eventData));
        this.RaiseTaskEvent(requestContext, scopeIdentifier, planId, jobId, taskId, eventData.Name, (TaskEvent) eventData);
      }
    }

    public void RaiseAgentChangeEvent(
      IVssRequestContext requestContext,
      Guid planId,
      AgentChangeEvent eventData)
    {
      using (new MethodScope(requestContext, nameof (TaskHub), nameof (RaiseAgentChangeEvent)))
      {
        ArgumentUtility.CheckForEmptyGuid(planId, nameof (planId));
        ArgumentUtility.CheckForNull<AgentChangeEvent>(eventData, nameof (eventData));
        try
        {
          this.RaiseOrchestrationEvent(requestContext, planId.ToString("N"), eventData.EventType, (object) eventData);
        }
        catch (OrchestrationSessionNotFoundException ex)
        {
          throw new TaskOrchestrationPlanTerminatedException(TaskResources.PlanOrchestrationTerminated((object) planId.ToString("D")), (Exception) ex);
        }
      }
    }

    public void RaiseMachinesChangeEvent(
      IVssRequestContext requestContext,
      Guid planId,
      DeploymentMachinesChangeEvent eventData)
    {
      using (new MethodScope(requestContext, nameof (TaskHub), nameof (RaiseMachinesChangeEvent)))
      {
        ArgumentUtility.CheckForEmptyGuid(planId, nameof (planId));
        ArgumentUtility.CheckForNull<DeploymentMachinesChangeEvent>(eventData, nameof (eventData));
        try
        {
          this.RaiseOrchestrationEvent(requestContext, planId.ToString("N"), "MS.TF.DistributedTask.DeploymentMachinesChanged", (object) eventData);
        }
        catch (OrchestrationSessionNotFoundException ex)
        {
          throw new TaskOrchestrationPlanTerminatedException(TaskResources.PlanOrchestrationTerminated((object) planId.ToString("D")), (Exception) ex);
        }
      }
    }

    public void RaiseGateChangeEvent(
      IVssRequestContext requestContext,
      Guid planId,
      DeploymentGatesChangeEvent eventData)
    {
      using (new MethodScope(requestContext, nameof (TaskHub), nameof (RaiseGateChangeEvent)))
      {
        ArgumentUtility.CheckForEmptyGuid(planId, nameof (planId));
        ArgumentUtility.CheckForNull<DeploymentGatesChangeEvent>(eventData, nameof (eventData));
        try
        {
          this.RaiseOrchestrationEvent(requestContext, planId.ToString("N"), "MS.TF.DistributedTask.DeploymentGatesChanged", (object) eventData);
        }
        catch (OrchestrationSessionNotFoundException ex)
        {
          throw new TaskOrchestrationPlanTerminatedException(TaskResources.PlanOrchestrationTerminated((object) planId.ToString("D")), (Exception) ex);
        }
      }
    }

    public void RaiseFetchedExternalVariablesEvent(
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      Guid planId,
      Guid jobId,
      Guid taskId,
      ExternalVariablesDownloadEvent eventData)
    {
      using (new MethodScope(requestContext, nameof (TaskHub), nameof (RaiseFetchedExternalVariablesEvent)))
      {
        ArgumentUtility.CheckForEmptyGuid(planId, nameof (planId));
        ArgumentUtility.CheckForEmptyGuid(jobId, nameof (jobId));
        ArgumentUtility.CheckForEmptyGuid(taskId, nameof (taskId));
        ArgumentUtility.CheckForNull<ExternalVariablesDownloadEvent>(eventData, nameof (eventData));
        try
        {
          this.RaiseTaskEvent(requestContext, scopeIdentifier, planId, jobId, taskId, "FetchedExternalVariables", (TaskEvent) eventData);
        }
        catch (OrchestrationSessionNotFoundException ex)
        {
          throw new TaskOrchestrationPlanTerminatedException(TaskResources.PlanOrchestrationTerminated((object) planId.ToString("D")), (Exception) ex);
        }
      }
    }

    public async Task<Microsoft.TeamFoundation.DistributedTask.WebApi.Timeline> AddJobAsync(
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      Guid planId,
      Guid jobId,
      int attempt,
      string workerName,
      TaskOrchestrationJob job)
    {
      job.InstanceId = jobId;
      List<TaskReferenceData> taskReferences = TaskHub.GetTaskReferences(requestContext, job.Tasks.Select<TaskInstance, TaskInstance>((Func<TaskInstance, TaskInstance>) (x =>
      {
        x.InstanceId = Guid.NewGuid();
        return x;
      })));
      Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord timelineRecord = new Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord()
      {
        Id = job.InstanceId,
        Name = job.Name,
        RefName = job.RefName,
        RecordType = "Job",
        Order = new int?(attempt),
        WorkerName = workerName
      };
      Microsoft.VisualStudio.Services.Identity.Identity userIdentity = requestContext.GetUserIdentity();
      Microsoft.TeamFoundation.DistributedTask.WebApi.Timeline timeline;
      using (TaskTrackingComponent trackingComponent = requestContext.CreateComponent<TaskTrackingComponent>(this.DataspaceCategory))
        timeline = await trackingComponent.AddJobsAsync(scopeIdentifier, planId, userIdentity.Id, (IList<TaskOrchestrationJob>) new TaskOrchestrationJob[1]
        {
          job
        }, (IList<TaskReferenceData>) taskReferences, (IList<Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord>) new Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord[1]
        {
          timelineRecord
        });
      return timeline;
    }

    internal async Task LogIssueAsync(
      IVssRequestContext requestContext,
      Guid scopeId,
      Guid planId,
      Guid recordId,
      Microsoft.TeamFoundation.DistributedTask.WebApi.IssueType issueType,
      string message)
    {
      TaskHub taskHub = this;
      MethodScope methodScope = new MethodScope(requestContext, nameof (TaskHub), nameof (LogIssueAsync));
      try
      {
        TaskOrchestrationPlan plan = await taskHub.GetPlanAsync(requestContext, scopeId, planId);
        if (plan == null)
          return;
        IList<Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord> timelineRecordsAsync = await taskHub.GetTimelineRecordsAsync(requestContext, scopeId, planId, plan.Timeline.Id, (IEnumerable<Guid>) new Guid[1]
        {
          recordId
        });
        if (timelineRecordsAsync == null)
          return;
        Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord record = timelineRecordsAsync.FirstOrDefault<Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord>((Func<Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord, bool>) (x => x.Id == recordId));
        if (record == null)
          return;
        record.AddIssue(issueType, message);
        Microsoft.TeamFoundation.DistributedTask.WebApi.Timeline timeline = await taskHub.UpdateTimelineAsync(requestContext, plan.ScopeIdentifier, planId, plan.Timeline.Id, (IList<Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord>) new Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord[1]
        {
          record
        });
        plan = (TaskOrchestrationPlan) null;
      }
      finally
      {
        methodScope.Dispose();
      }
      methodScope = new MethodScope();
    }

    internal async Task JobCompletedAsync(
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      Guid planId,
      Guid jobId,
      Microsoft.TeamFoundation.DistributedTask.WebApi.TaskResult? result = null)
    {
      TaskHub taskHub = this;
      MethodScope methodScope = new MethodScope(requestContext, nameof (TaskHub), nameof (JobCompletedAsync));
      try
      {
        TaskOrchestrationPlan plan;
        using (TaskTrackingComponent trackingComponent = requestContext.CreateComponent<TaskTrackingComponent>(taskHub.DataspaceCategory))
          plan = await trackingComponent.GetPlanAsync(scopeIdentifier, planId);
        if (plan == null)
          throw new TaskOrchestrationPlanNotFoundException(TaskResources.PlanNotFound((object) planId));
        Microsoft.TeamFoundation.DistributedTask.WebApi.Timeline timeline = await taskHub.GetTimelineAsync(requestContext, scopeIdentifier, plan.PlanId, plan.Timeline.Id, includeRecords: true, includePreviousAttempts: false);
        PipelineProcess process = plan.GetProcess<PipelineProcess>();
        bool isSingleJobPipeline = taskHub.IsSingleJobPipeline(process);
        List<Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord> records1 = timeline.Records;
        Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord jobRecord = records1 != null ? records1.SingleOrDefault<Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord>((Func<Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord, bool>) (tr => tr.Id.Equals(jobId))) : (Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord) null;
        List<Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord> records2 = timeline.Records;
        Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord phaseRecord = records2 != null ? records2.SingleOrDefault<Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord>((Func<Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord, bool>) (tr => tr.Id.Equals((object) (Guid?) jobRecord?.ParentId))) : (Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord) null;
        List<Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord> records3 = timeline.Records;
        Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord timelineRecord1 = records3 != null ? records3.SingleOrDefault<Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord>((Func<Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord, bool>) (tr => tr.Id.Equals((object) (Guid?) phaseRecord?.ParentId))) : (Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord) null;
        List<Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord> taskRecords = timeline.Records.Where<Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord>((Func<Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord, bool>) (tr =>
        {
          Guid? parentId = tr.ParentId;
          Guid guid = jobId;
          if (!parentId.HasValue)
            return false;
          return !parentId.HasValue || parentId.GetValueOrDefault() == guid;
        })).ToList<Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord>();
        string displayName = Microsoft.TeamFoundation.DistributedTask.Pipelines.Phase.GenerateDisplayName(timelineRecord1?.Name, jobRecord?.Name);
        try
        {
          await taskHub.Extension.JobCompletedAsync(requestContext, plan, jobRecord, (IReadOnlyList<Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord>) taskRecords, displayName, isSingleJobPipeline);
        }
        catch (Exception ex)
        {
          requestContext.TraceException(nameof (TaskHub), ex);
        }
        if (requestContext.IsFeatureEnabled("DistributedTask.EnableTelemetryTimelineIssueFiltering"))
        {
          IEnumerable<Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord> source = timeline.Records.Where<Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord>((Func<Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord, bool>) (x => x.Task != null));
          IList<TaskDefinition> taskDefinitions = taskHub.GetTaskDefinitions(requestContext, (IList<Microsoft.TeamFoundation.DistributedTask.WebApi.TaskReference>) source.Select<Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord, Microsoft.TeamFoundation.DistributedTask.WebApi.TaskReference>((Func<Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord, Microsoft.TeamFoundation.DistributedTask.WebApi.TaskReference>) (x => x.Task)).ToList<Microsoft.TeamFoundation.DistributedTask.WebApi.TaskReference>());
          foreach (Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord timelineRecord2 in source)
          {
            Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord record = timelineRecord2;
            if (!TaskHub.WellKnownMicrosoftExtentionTaskIds.Contains<Guid>(record.Task.Id))
            {
              TaskDefinition taskDefinition = taskDefinitions != null ? taskDefinitions.FirstOrDefault<TaskDefinition>((Func<TaskDefinition, bool>) (x => x.Id == record.Task.Id && x.Version.ToString().Equals(record.Task.Version, StringComparison.OrdinalIgnoreCase))) : (TaskDefinition) null;
              if (taskDefinition == null)
                requestContext.TraceError(10016206, nameof (TaskHub), string.Format("Unable to find task definition for task {0} version {1} during the timeline processing.", (object) record.Task.Id, (object) record.Task.Version));
              else if (!taskDefinition.ServerOwned)
              {
                record.Issues.Clear();
                requestContext.TraceInfo(10016205, nameof (TaskHub), string.Format("Remove issues for not a server-owned task: ID {0}, version {1}.", (object) record.Task.Id, (object) record.Task.Version));
              }
            }
          }
        }
        TelemetryFactory.GetLogger(requestContext).PublishTaskHubJobCompletedTelemetry(requestContext, taskHub, plan, timeline, jobId);
        KPIHelper.PublishDTAReliabilityMetric(requestContext, timeline);
        Microsoft.TeamFoundation.DistributedTask.WebApi.TaskResult? nullable = result;
        Microsoft.TeamFoundation.DistributedTask.WebApi.TaskResult taskResult = Microsoft.TeamFoundation.DistributedTask.WebApi.TaskResult.Abandoned;
        if (nullable.GetValueOrDefault() == taskResult & nullable.HasValue && taskHub.IsLogLineStorageEnabled(requestContext))
          await taskHub.CheckLogs(requestContext, scopeIdentifier, planId, timeline.Id, jobId, (IList<Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord>) taskRecords);
        plan = (TaskOrchestrationPlan) null;
        timeline = (Microsoft.TeamFoundation.DistributedTask.WebApi.Timeline) null;
        taskRecords = (List<Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord>) null;
      }
      finally
      {
        methodScope.Dispose();
      }
      methodScope = new MethodScope();
    }

    private async Task CheckLogs(
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      Guid planId,
      Guid timelineId,
      Guid jobId,
      IList<Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord> steps)
    {
      TaskHub taskHub1 = this;
      foreach (Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord step1 in (IEnumerable<Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord>) steps)
      {
        Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord step = step1;
        TimelineRecordLogLineResult logLinesAsync = await taskHub1.GetLogLinesAsync(requestContext, scopeIdentifier, planId, timelineId, jobId, step.Id, (string) null);
        if (logLinesAsync == null)
        {
          requestContext.TraceError(10016137, nameof (TaskHub), "Error fetching log lines " + string.Format("from table store Plan:{0} JobId:{1} TimelineId:{2} StepId:{3} StepResult:{4}", (object) planId, (object) jobId, (object) timelineId, (object) step.Id, (object) step.Result));
        }
        else
        {
          long fileLogEnd = 0;
          TaskLogReference logReference = step.Log;
          if (logReference != null)
            fileLogEnd = taskHub1.GetLog(requestContext, scopeIdentifier, planId, step.Log.Id).LineCount - 1L;
          if ((logLinesAsync.Lines.Count > 0 ? logLinesAsync.Lines.Max<TimelineRecordLogLine>((Func<TimelineRecordLogLine, long>) (x => x.LineNumber)) : 0L) > fileLogEnd)
          {
            requestContext.TraceAlways(10016134, nameof (TaskHub), "Detected incomplete log message. " + string.Format("Plan:{0} JobId:{1} TimelineId:{2} StepId:{3} StepResult:{4}", (object) planId, (object) jobId, (object) timelineId, (object) step.Id, (object) step.Result));
            DateTime utcNow = DateTime.UtcNow;
            using (MemoryStream ms = new MemoryStream())
            {
              using (StreamWriter sw = new StreamWriter((Stream) ms, Encoding.UTF8))
              {
                sw.WriteLine(string.Format("{0:O} ##[section]{1}", (object) utcNow, (object) TaskResources.AgentLastWordsMessage()));
                foreach (TimelineRecordLogLine timelineRecordLogLine in logLinesAsync.Lines.Where<TimelineRecordLogLine>((Func<TimelineRecordLogLine, bool>) (x => x.LineNumber > fileLogEnd)))
                  sw.WriteLine(timelineRecordLogLine.Line);
                sw.Flush();
                ms.Seek(0L, SeekOrigin.Begin);
                if (logReference == null)
                {
                  logReference = (TaskLogReference) await taskHub1.CreateLogAsync(requestContext, scopeIdentifier, planId, string.Format((IFormatProvider) CultureInfo.CurrentCulture, "logs\\{0:D}", (object) step.Id));
                  TaskHub taskHub2 = taskHub1;
                  IVssRequestContext requestContext1 = requestContext;
                  Guid scopeIdentifier1 = scopeIdentifier;
                  Guid planId1 = planId;
                  Guid timelineId1 = planId;
                  Guid id = step.Id;
                  TaskLogReference taskLogReference = logReference;
                  Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecordState? state = new Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecordState?();
                  DateTime? startTime = new DateTime?();
                  DateTime? finishTime = new DateTime?();
                  int? percentComplete = new int?();
                  Microsoft.TeamFoundation.DistributedTask.WebApi.TaskResult? result = new Microsoft.TeamFoundation.DistributedTask.WebApi.TaskResult?();
                  TaskLogReference log = taskLogReference;
                  int? queueId = new int?();
                  await taskHub2.UpdateTimelineRecordAsync(requestContext1, scopeIdentifier1, planId1, timelineId1, id, state, startTime, finishTime, percentComplete: percentComplete, result: result, log: log, queueId: queueId);
                }
                TaskLog taskLog = await taskHub1.AppendLogAsync(requestContext, scopeIdentifier, planId, logReference.Id, (Stream) ms);
              }
            }
          }
          logReference = (TaskLogReference) null;
          step = (Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord) null;
        }
      }
    }

    internal async Task StageCompletedAsync(
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      Guid planId,
      Guid stageId,
      Microsoft.TeamFoundation.DistributedTask.WebApi.TaskResult? result = null)
    {
      MethodScope methodScope = new MethodScope(requestContext, nameof (TaskHub), nameof (StageCompletedAsync));
      try
      {
        IList<Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord> timelineRecordsAsync;
        using (TaskTrackingComponent tc = this.CreateComponent<TaskTrackingComponent>(requestContext))
          timelineRecordsAsync = await tc.GetTimelineRecordsAsync(scopeIdentifier, planId, planId, (IEnumerable<Guid>) new Guid[1]
          {
            stageId
          });
        Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord stageRecord = timelineRecordsAsync != null ? timelineRecordsAsync.SingleOrDefault<Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord>((Func<Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord, bool>) (tr => tr.Id == stageId)) : (Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord) null;
        if (stageRecord == null)
        {
          requestContext.TraceError(10016133, nameof (TaskHub), string.Format("Unable to free resources. Stage record is null for ID: {0}", (object) stageId));
          return;
        }
        if (this.Extension.AreExclusiveLocksSupported(requestContext))
        {
          List<ResourceLockRequest> resourceLockRequestList = await requestContext.GetService<IDistributedTaskResourceLockService>().FreeResourceLocksAsync(requestContext, planId, stageRecord.RefName, new int?(stageRecord.Attempt));
          if (resourceLockRequestList.Count > 0)
            requestContext.TraceAlways(10016132, nameof (TaskHub), string.Format("Stage {0} completed, freeing {1} resources.", (object) stageRecord.RefName, (object) resourceLockRequestList.Count));
        }
        stageRecord = (Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord) null;
      }
      finally
      {
        methodScope.Dispose();
      }
      methodScope = new MethodScope();
    }

    internal async Task JobStartedAsync(
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      Guid planId,
      Guid jobId)
    {
      MethodScope methodScope = new MethodScope(requestContext, nameof (TaskHub), nameof (JobStartedAsync));
      try
      {
        TaskOrchestrationPlan plan;
        using (TaskTrackingComponent trackingComponent = requestContext.CreateComponent<TaskTrackingComponent>(this.DataspaceCategory))
          plan = await trackingComponent.GetPlanAsync(scopeIdentifier, planId);
        Microsoft.TeamFoundation.DistributedTask.WebApi.Timeline timelineAsync = await this.GetTimelineAsync(requestContext, scopeIdentifier, plan.PlanId, plan.Timeline.Id, includeRecords: true, includePreviousAttempts: false);
        List<Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord> records1 = timelineAsync.Records;
        Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord jobRecord = records1 != null ? records1.SingleOrDefault<Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord>((Func<Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord, bool>) (tr => tr.Id.Equals(jobId))) : (Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord) null;
        List<Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord> records2 = timelineAsync.Records;
        Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord phaseRecord = records2 != null ? records2.SingleOrDefault<Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord>((Func<Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord, bool>) (tr => tr.Id.Equals((object) (Guid?) jobRecord?.ParentId))) : (Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord) null;
        List<Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord> records3 = timelineAsync.Records;
        Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord timelineRecord = records3 != null ? records3.SingleOrDefault<Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord>((Func<Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord, bool>) (tr => tr.Id.Equals((object) (Guid?) phaseRecord?.ParentId))) : (Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord) null;
        List<Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord> list = timelineAsync.Records.Where<Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord>((Func<Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord, bool>) (tr =>
        {
          Guid? parentId = tr.ParentId;
          Guid guid = jobId;
          if (!parentId.HasValue)
            return false;
          return !parentId.HasValue || parentId.GetValueOrDefault() == guid;
        })).ToList<Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord>();
        string displayName = Microsoft.TeamFoundation.DistributedTask.Pipelines.Phase.GenerateDisplayName(timelineRecord?.Name, jobRecord?.Name);
        await this.Extension.JobStartedAsync(requestContext, plan, jobId, jobRecord, (IReadOnlyList<Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord>) list, displayName);
        plan = (TaskOrchestrationPlan) null;
      }
      finally
      {
        methodScope.Dispose();
      }
      methodScope = new MethodScope();
    }

    internal async Task<Microsoft.TeamFoundation.DistributedTask.WebApi.Timeline> CreateAttemptAsync(
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      Guid planId,
      Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineAttempt attempt,
      Microsoft.TeamFoundation.DistributedTask.WebApi.Timeline newTimeline)
    {
      Guid userId = requestContext.GetUserId(true);
      Microsoft.TeamFoundation.DistributedTask.WebApi.Timeline pipelineAttemptAsync;
      using (TaskTrackingComponent component = requestContext.CreateComponent<TaskTrackingComponent>(this.DataspaceCategory))
        pipelineAttemptAsync = await component.CreatePipelineAttemptAsync(scopeIdentifier, planId, (IList<Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineAttempt>) new Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineAttempt[1]
        {
          attempt
        }, newTimeline, userId);
      return pipelineAttemptAsync;
    }

    internal async Task CreateJobAttemptAsync(
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      Guid planId,
      string stageName,
      string phaseName,
      string jobName,
      int attempt,
      int previousAttempt)
    {
      Microsoft.TeamFoundation.DistributedTask.WebApi.TaskOrchestrationPlanReference planData = this.EnsurePlanData(requestContext, scopeIdentifier, planId);
      PipelineIdGenerator idGenerator = new PipelineIdGenerator(planData.Version < 4);
      Guid previousJobId = idGenerator.GetJobInstanceId(stageName, phaseName, jobName, previousAttempt, 1);
      IList<Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord> timelineRecordsAsync;
      using (TaskTrackingComponent tc = this.CreateComponent<TaskTrackingComponent>(requestContext))
        timelineRecordsAsync = await tc.GetTimelineRecordsAsync(scopeIdentifier, planId, planId, (IEnumerable<Guid>) new Guid[1]
        {
          previousJobId
        });
      if (timelineRecordsAsync.Count <= 0)
        throw new TaskOrchestrationJobNotFoundException(TaskResources.JobNotFound((object) previousJobId, (object) planId));
      Microsoft.TeamFoundation.DistributedTask.WebApi.Timeline newTimeline = new Microsoft.TeamFoundation.DistributedTask.WebApi.Timeline();
      Guid jobInstanceId = idGenerator.GetJobInstanceId(stageName, phaseName, jobName, attempt, 1);
      string jobIdentifier = idGenerator.GetJobIdentifier(stageName, phaseName, jobName, 1);
      newTimeline.Records.Add(this.ResetRecord(timelineRecordsAsync.FirstOrDefault<Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord>(), jobInstanceId, attempt));
      Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineAttempt attempt1 = new Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineAttempt()
      {
        Identifier = jobIdentifier,
        Attempt = attempt,
        RecordId = jobInstanceId
      };
      Microsoft.TeamFoundation.DistributedTask.WebApi.Timeline attemptAsync = await this.CreateAttemptAsync(requestContext, scopeIdentifier, planId, attempt1, newTimeline);
      if (attemptAsync == null)
      {
        planData = (Microsoft.TeamFoundation.DistributedTask.WebApi.TaskOrchestrationPlanReference) null;
        idGenerator = (PipelineIdGenerator) null;
      }
      else if (attemptAsync.Records.Count <= 0)
      {
        planData = (Microsoft.TeamFoundation.DistributedTask.WebApi.TaskOrchestrationPlanReference) null;
        idGenerator = (PipelineIdGenerator) null;
      }
      else
      {
        this.Extension.TimelineRecordsUpdated(requestContext, planData, (Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineReference) attemptAsync, (IEnumerable<Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord>) attemptAsync.Records);
        planData = (Microsoft.TeamFoundation.DistributedTask.WebApi.TaskOrchestrationPlanReference) null;
        idGenerator = (PipelineIdGenerator) null;
      }
    }

    private Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord ResetRecord(
      Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord record,
      Guid newId,
      int attempt)
    {
      return new Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord()
      {
        Attempt = attempt,
        Id = newId,
        Identifier = record.Identifier,
        Name = record.Name,
        Order = record.Order,
        ParentId = record.ParentId,
        RecordType = record.RecordType,
        RefName = record.RefName,
        State = new Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecordState?(Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecordState.Pending)
      };
    }

    internal async Task PlanCompletedAsync(
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      Guid planId,
      DateTime finishTime,
      Microsoft.TeamFoundation.DistributedTask.WebApi.TaskResult result,
      string resultCode)
    {
      TaskHub taskHub = this;
      MethodScope methodScope = new MethodScope(requestContext, nameof (TaskHub), nameof (PlanCompletedAsync));
      try
      {
        KPIHelper.PublishDTPlanCompleted(requestContext);
        OrchestrationTracer ciao = taskHub.GetCIAO(requestContext);
        string planIdString = planId.ToString("D");
        ciao.TracePhaseStarted(planIdString, "CIPlatform", nameof (PlanCompletedAsync));
        Microsoft.TeamFoundation.DistributedTask.WebApi.TaskOrchestrationPlanReference planData = taskHub.EnsurePlanData(requestContext, scopeIdentifier, planId);
        UpdatePlanResult updateResult = new UpdatePlanResult();
        TaskTrackingComponent trackingComponent = requestContext.CreateComponent<TaskTrackingComponent>(taskHub.DataspaceCategory);
        try
        {
          updateResult = await trackingComponent.UpdatePlanAsync(planData.ScopeIdentifier, planData.PlanId, new DateTime?(), new DateTime?(finishTime), new TaskOrchestrationPlanState?(TaskOrchestrationPlanState.Completed), new Microsoft.TeamFoundation.DistributedTask.WebApi.TaskResult?(result), resultCode, (IOrchestrationEnvironment) null);
        }
        finally
        {
          trackingComponent?.Dispose();
        }
        trackingComponent = (TaskTrackingComponent) null;
        if (updateResult.Timeline != null)
        {
          TaskHub.TraceTimelineRecordUpdates(requestContext, (IList<Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord>) null, (IList<Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord>) updateResult.Timeline.Records, nameof (PlanCompletedAsync));
          taskHub.Extension.TimelineRecordsUpdated(requestContext, (Microsoft.TeamFoundation.DistributedTask.WebApi.TaskOrchestrationPlanReference) updateResult.Plan, (Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineReference) updateResult.Timeline, (IEnumerable<Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord>) updateResult.Timeline.Records);
        }
        try
        {
          await taskHub.Extension.PlanCompletedAsync(requestContext, updateResult.Plan);
        }
        catch (Exception ex)
        {
          requestContext.TraceException(nameof (TaskHub), ex);
        }
        await taskHub.MarkLogTableAsCompletedAsync(requestContext, planData.ScopeIdentifier, planData.PlanId);
        trackingComponent = requestContext.CreateComponent<TaskTrackingComponent>(taskHub.DataspaceCategory);
        try
        {
          await trackingComponent.DeletePlanContextsAsync(planData.ScopeIdentifier, planData.PlanId);
        }
        finally
        {
          trackingComponent?.Dispose();
        }
        trackingComponent = (TaskTrackingComponent) null;
        if (taskHub.Extension.AreExclusiveLocksSupported(requestContext))
        {
          List<ResourceLockRequest> resourceLockRequestList = await requestContext.GetService<IDistributedTaskResourceLockService>().FreeResourceLocksAsync(requestContext, planId);
          if (resourceLockRequestList != null && resourceLockRequestList.Count > 0)
            requestContext.TraceAlways(10016136, nameof (TaskHub), string.Format("Freeing {0} resources left over at plan completion.", (object) resourceLockRequestList.Count));
        }
        TelemetryFactory.GetLogger(requestContext).PublishTaskHubPlanCompletedTelemetry(requestContext, taskHub, updateResult.Plan);
        ITeamFoundationEventService service = requestContext.GetService<ITeamFoundationEventService>();
        PlanCompletedEvent planCompletedEvent = new PlanCompletedEvent()
        {
          Plan = updateResult.Plan
        };
        IVssRequestContext requestContext1 = requestContext;
        PlanCompletedEvent notificationEvent = planCompletedEvent;
        service.PublishNotification(requestContext1, (object) notificationEvent);
        if (taskHub.Name == "Build")
          ciao.TracePhaseStarted(planIdString, "CIPlatform", "PlanCompleted");
        else
          ciao.TraceCompleted(planIdString, "CIPlatform", "PlanCompleted");
        requestContext.TraceAlways(10015557, nameof (TaskHub), "Completed plan: {0}", (object) planIdString);
        if (requestContext.GetService<IPlanThrottleService>().ShouldThrottleNewPlans(requestContext, taskHub, scopeIdentifier, planData.Definition))
          requestContext.GetService<ITeamFoundationJobService>().QueueJobsNow(requestContext, (IEnumerable<Guid>) new Guid[1]
          {
            TaskConstants.QueueThrottledPlansJob
          });
        ciao = (OrchestrationTracer) null;
        planIdString = (string) null;
        planData = (Microsoft.TeamFoundation.DistributedTask.WebApi.TaskOrchestrationPlanReference) null;
        updateResult = new UpdatePlanResult();
      }
      finally
      {
        methodScope.Dispose();
      }
      methodScope = new MethodScope();
    }

    internal async Task PlanStartedAsync(
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      Guid planId,
      DateTime startTime)
    {
      TaskHub taskHub = this;
      MethodScope methodScope = new MethodScope(requestContext, nameof (TaskHub), nameof (PlanStartedAsync));
      try
      {
        KPIHelper.PublishDTPlanStarted(requestContext);
        taskHub.GetCIAO(requestContext).TracePhaseStarted(planId.ToString("D"), "CIPlatform", nameof (PlanStartedAsync));
        Microsoft.TeamFoundation.DistributedTask.WebApi.TaskOrchestrationPlanReference planData = taskHub.EnsurePlanData(requestContext, scopeIdentifier, planId);
        UpdatePlanResult updateResult = new UpdatePlanResult();
        TaskTrackingComponent trackingComponent = requestContext.CreateComponent<TaskTrackingComponent>(taskHub.DataspaceCategory);
        try
        {
          updateResult = await trackingComponent.UpdatePlanAsync(planData.ScopeIdentifier, planData.PlanId, new DateTime?(startTime), new DateTime?(), new TaskOrchestrationPlanState?(TaskOrchestrationPlanState.InProgress), new Microsoft.TeamFoundation.DistributedTask.WebApi.TaskResult?(), (string) null, (IOrchestrationEnvironment) null);
        }
        finally
        {
          trackingComponent?.Dispose();
        }
        trackingComponent = (TaskTrackingComponent) null;
        if (updateResult.Timeline != null)
          TaskHub.TraceTimelineRecordUpdates(requestContext, (IList<Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord>) null, (IList<Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord>) updateResult.Timeline.Records, "PlanStartedAsync_1");
        PipelineEnvironment pipelineEnvironment = updateResult.Plan.ProcessEnvironment as PipelineEnvironment;
        if (pipelineEnvironment != null && pipelineEnvironment.Version > 1 && updateResult.Plan.TemplateType == PlanTemplateType.Designer)
        {
          PipelineResources pipelineResources = pipelineEnvironment.Resources.Clone();
          IResourceStore resourceStore = requestContext.GetService<IPipelineBuilderService>().GetResourceStore(requestContext, planData.ScopeIdentifier, pipelineEnvironment.Resources);
          List<PipelineValidationError> errors = new List<PipelineValidationError>();
          TaskHub.ValidateDeclaredResources(requestContext, planData.ScopeIdentifier, pipelineEnvironment, resourceStore, (IList<PipelineValidationError>) errors, false);
          if (errors.Count > 0)
            throw new PipelineValidationException((IEnumerable<PipelineValidationError>) errors);
          if (pipelineResources.Repositories.Any<RepositoryResource>((Func<RepositoryResource, bool>) (x => !string.Equals(x.Version, pipelineEnvironment.Resources.Repositories.Single<RepositoryResource>((Func<RepositoryResource, bool>) (y => string.Equals(y.Alias, x.Alias))).Version, StringComparison.OrdinalIgnoreCase))))
          {
            requestContext.TraceAlways(0, nameof (TaskHub), "repository resource updated after plan compilation");
            trackingComponent = requestContext.CreateComponent<TaskTrackingComponent>(taskHub.DataspaceCategory);
            try
            {
              updateResult = await trackingComponent.UpdatePlanAsync(planData.ScopeIdentifier, planData.PlanId, new DateTime?(), new DateTime?(), new TaskOrchestrationPlanState?(), new Microsoft.TeamFoundation.DistributedTask.WebApi.TaskResult?(), (string) null, (IOrchestrationEnvironment) pipelineEnvironment);
            }
            finally
            {
              trackingComponent?.Dispose();
            }
            trackingComponent = (TaskTrackingComponent) null;
            if (updateResult.Timeline != null)
              TaskHub.TraceTimelineRecordUpdates(requestContext, (IList<Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord>) null, (IList<Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord>) updateResult.Timeline.Records, "PlanStartedAsync_2");
          }
        }
        await taskHub.CreateLogTableAsync(requestContext, planData.ScopeIdentifier, planData.PlanId);
        try
        {
          await taskHub.Extension.PlanStartedAsync(requestContext, updateResult.Plan);
        }
        catch (TaskOrchestrationPlanCanceledException ex)
        {
          Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord record = (await taskHub.GetTimelineAsync(requestContext, planData.ScopeIdentifier, updateResult.Plan.PlanId, updateResult.Plan.Timeline.Id, includeRecords: true)).Records.FirstOrDefault<Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord>((Func<Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord, bool>) (x => x.RecordType == "Job"));
          if (record != null)
          {
            record.AddIssue(Microsoft.TeamFoundation.DistributedTask.WebApi.IssueType.Error, ex.Message);
            Microsoft.TeamFoundation.DistributedTask.WebApi.Timeline timeline = await taskHub.UpdateTimelineAsync(requestContext, planData.ScopeIdentifier, updateResult.Plan.PlanId, updateResult.Plan.Timeline.Id, (IList<Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord>) new Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord[1]
            {
              record
            });
          }
          await taskHub.CancelPlanAsync(requestContext, planData.ScopeIdentifier, planId, TimeSpan.FromSeconds(60.0), ex.Message);
        }
        planData = (Microsoft.TeamFoundation.DistributedTask.WebApi.TaskOrchestrationPlanReference) null;
        updateResult = new UpdatePlanResult();
      }
      finally
      {
        methodScope.Dispose();
      }
      methodScope = new MethodScope();
    }

    internal Task CancelAgentJobAsync(
      IVssRequestContext requestContext,
      int poolId,
      long requestId,
      TaskAgentReference agent,
      Guid scopeIdentifier,
      Guid planId,
      Guid jobId,
      TimeSpan timeout)
    {
      using (new MethodScope(requestContext, nameof (TaskHub), nameof (CancelAgentJobAsync)))
      {
        requestContext.AssertAsyncExecutionEnabled();
        JobCancelMessage jobCancelMessage = new JobCancelMessage(jobId, timeout);
        return requestContext.GetService<IDistributedTaskPoolService>().SendAgentMessageAsync(requestContext, poolId, requestId, jobCancelMessage.GetAgentMessage());
      }
    }

    public async Task SendAgentJobMetadataUpdateToPlanAsync(
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      Guid planId,
      JobMetadataMessage jobMetadataMessage)
    {
      foreach (Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord timelineRecord in (await this.GetTimelineAsync(requestContext, scopeIdentifier, planId, planId, includeRecords: true)).Records.Where<Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord>((Func<Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord, bool>) (x =>
      {
        if (!(x.RecordType == "Job"))
          return false;
        Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecordState? state = x.State;
        Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecordState timelineRecordState = Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecordState.InProgress;
        return state.GetValueOrDefault() == timelineRecordState & state.HasValue;
      })))
      {
        if (timelineRecord.QueueId.HasValue)
        {
          Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecordState? state = timelineRecord.State;
          Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecordState timelineRecordState = Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecordState.InProgress;
          if (state.GetValueOrDefault() == timelineRecordState & state.HasValue)
          {
            JobMetadataMessage jobMetadataMessage1 = jobMetadataMessage.Clone();
            jobMetadataMessage1.JobId = timelineRecord.Id;
            try
            {
              await this.RaiseJobEventAsync(requestContext, scopeIdentifier, planId, timelineRecord.Id, "JobMetadataUpdate", (JobEvent) new JobMetadataEvent(jobMetadataMessage1, timelineRecord.Id), ensureOrchestrationExists: false);
            }
            catch (Exception ex)
            {
              requestContext.TraceException(nameof (TaskHub), ex);
            }
          }
        }
      }
    }

    internal Task SendAgentJobMetadataUpdateAsync(
      IVssRequestContext requestContext,
      int poolId,
      long requestId,
      JobMetadataMessage jobMetadataMessage)
    {
      using (new MethodScope(requestContext, nameof (TaskHub), nameof (SendAgentJobMetadataUpdateAsync)))
      {
        requestContext.AssertAsyncExecutionEnabled();
        return requestContext.GetService<IDistributedTaskPoolService>().SendAgentMessageAsync(requestContext, poolId, requestId, jobMetadataMessage.GetAgentMessage());
      }
    }

    internal async Task SendAgentJobAsync(
      IVssRequestContext requestContext,
      int poolId,
      long requestId,
      TaskAgentReference agent,
      Guid scopeIdentifier,
      Guid planId,
      Guid jobId,
      TaskOrchestrationJobAttempt jobAttempt = null)
    {
      MethodScope methodScope = new MethodScope(requestContext, nameof (TaskHub), nameof (SendAgentJobAsync));
      try
      {
        if (requestContext.ExecutionEnvironment.IsHostedDeployment && !string.IsNullOrEmpty(agent?.AccessPoint))
        {
          AccessMapping accessMapping = requestContext.GetService<ILocationService>().GetAccessMapping(requestContext, agent.AccessPoint);
          if (accessMapping != null)
            requestContext.SetClientAccessMapping(accessMapping);
          else
            requestContext.TraceError(10015540, nameof (TaskHub), "Access mapping for moniker '" + agent.AccessPoint + "' is NULL.");
        }
        TaskHub.JobRequestData data = await this.PrepareJobRequestAsync(requestContext, scopeIdentifier, planId, jobId, jobAttempt, agent?.Name);
        await this.SendAgentJobRequestAsync(requestContext, poolId, requestId, jobId, data, agent);
        await this.JobStartedAsync(requestContext, data.Plan.ScopeIdentifier, data.Plan.PlanId, jobId);
        data = new TaskHub.JobRequestData();
      }
      finally
      {
        methodScope.Dispose();
      }
      methodScope = new MethodScope();
    }

    internal async Task SendAgentJobAsync(
      IVssRequestContext requestContext,
      int poolId,
      long requestId,
      TaskAgentReference agent,
      Guid scopeIdentifier,
      Guid planId,
      JobParameters parameters)
    {
      MethodScope methodScope = new MethodScope(requestContext, nameof (TaskHub), nameof (SendAgentJobAsync));
      try
      {
        if (requestContext.ExecutionEnvironment.IsHostedDeployment && !string.IsNullOrEmpty(agent?.AccessPoint))
        {
          AccessMapping accessMapping = requestContext.GetService<ILocationService>().GetAccessMapping(requestContext, agent.AccessPoint);
          if (accessMapping != null)
            requestContext.SetClientAccessMapping(accessMapping);
          else
            requestContext.TraceError(10015540, nameof (TaskHub), "Access mapping for moniker '" + agent.AccessPoint + "' is NULL.");
        }
        TaskHub.JobRequestData data = await this.PrepareJobRequestAsync(requestContext, scopeIdentifier, planId, parameters);
        await this.SendAgentJobRequestAsync(requestContext, poolId, requestId, data.Job.Id, data, agent);
        await this.JobStartedAsync(requestContext, data.Plan.ScopeIdentifier, data.Plan.PlanId, data.Job.Id);
        data = new TaskHub.JobRequestData();
      }
      finally
      {
        methodScope.Dispose();
      }
      methodScope = new MethodScope();
    }

    internal async Task<ExecuteTaskResponse> ExecuteServerTaskAsync(
      IVssRequestContext requestContext,
      ServerTaskExecutionContext taskExecutionContext,
      Guid scopeId,
      Guid planId,
      Guid jobId,
      Guid? taskId = null)
    {
      TaskHub taskHub = this;
      ExecuteTaskResponse executeTaskResponse1;
      using (new MethodScope(requestContext, nameof (TaskHub), nameof (ExecuteServerTaskAsync)))
      {
        TaskHub.JobRequestData data = await taskHub.PrepareJobRequestAsync(requestContext, scopeId, planId, jobId, (TaskOrchestrationJobAttempt) null, (string) null);
        if (!taskId.HasValue && data.ContainerJob.Tasks.Count != 1)
          throw new DistributedTaskException(TaskResources.UnsupportedTaskCountForServerJob());
        TaskInstance taskInstance = !taskId.HasValue || data.ContainerJob.Tasks.SingleOrDefault<TaskInstance>((Func<TaskInstance, bool>) (t =>
        {
          Guid instanceId = t.InstanceId;
          Guid? nullable = taskId;
          return nullable.HasValue && instanceId == nullable.GetValueOrDefault();
        })) == null ? data.ContainerJob.Tasks.Single<TaskInstance>() : data.ContainerJob.Tasks.Single<TaskInstance>((Func<TaskInstance, bool>) (t =>
        {
          Guid instanceId = t.InstanceId;
          Guid? nullable = taskId;
          return nullable.HasValue && instanceId == nullable.GetValueOrDefault();
        }));
        TaskDefinition definition = taskHub.GetTaskDefinition(requestContext, taskInstance.Id, taskInstance.Version);
        TaskHub.SetDefaultInputValues(taskInstance.Inputs, definition);
        IServerTaskHandler handler;
        if (!TaskHub.TryGetServerJobHandler(requestContext, definition, out handler))
          throw new ServerExecutionHandlerNotFoundException(TaskResources.TaskExecutionDefinitionInvalid((object) definition.Id));
        TaskEventsConfig taskEventsConfig = TaskHub.ExpandAndValidateTaskEventsConfiguration(definition, taskInstance.Inputs, handler.HandlerName);
        if ((!requestContext.IsFeatureEnabled("DistributedTask.DoNotCreateTimelineRecordForServerTask") ? 1 : (taskEventsConfig == null ? 0 : (taskEventsConfig.All.Count<KeyValuePair<string, TaskEventConfig>>((Func<KeyValuePair<string, TaskEventConfig>, bool>) (x => x.Value.IsEnabled())) == 0 ? 1 : 0))) != 0)
        {
          Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord timelineRecord = new Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord()
          {
            Id = taskInstance.InstanceId,
            Name = taskInstance.DisplayName,
            RecordType = "Task",
            StartTime = new DateTime?(DateTime.UtcNow),
            State = new Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecordState?(Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecordState.Pending),
            ParentId = new Guid?(jobId),
            RefName = taskInstance.RefName
          };
          Microsoft.TeamFoundation.DistributedTask.WebApi.Timeline timeline = await taskHub.UpdateTimelineAsync(requestContext, scopeId, planId, data.Plan.Timeline.Id, (IList<Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord>) new Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord[1]
          {
            timelineRecord
          });
        }
        ServerTaskRequestMessage taskRequestMessage = new ServerTaskRequestMessage(data.Plan.AsReference(), data.Plan.Timeline, data.ContainerJob.InstanceId, data.ContainerJob.Name, data.ContainerJob.RefName, data.Environment, taskInstance, definition);
        taskHub.SetSystemVariablesForServerTask(taskRequestMessage);
        TelemetryFactory.GetLogger(requestContext).PublishTaskHubExecuteTaskTelemetry(requestContext, taskHub, data.AuthorizationId, taskRequestMessage, data.Plan?.TemplateType.ToString() ?? string.Empty);
        ExecuteTaskResponse executeTaskResponse2 = await handler.ExecuteTaskAsync(requestContext, taskExecutionContext, taskRequestMessage);
        foreach (KeyValuePair<string, TaskEventConfig> taskEvent in executeTaskResponse2.TaskEvents.All)
          TaskHub.ResolveTaskEventVariables(taskEvent, taskInstance.Inputs);
        executeTaskResponse1 = executeTaskResponse2;
      }
      return executeTaskResponse1;
    }

    internal async Task<IList<ServerTaskSectionExecutionOptions>> GetHandlerSectionExecutionOptionsAsync(
      IVssRequestContext requestContext,
      Guid scopeId,
      Guid planId,
      Guid jobId,
      Guid taskId)
    {
      IList<ServerTaskSectionExecutionOptions> executionOptions;
      using (new MethodScope(requestContext, nameof (TaskHub), nameof (GetHandlerSectionExecutionOptionsAsync)))
      {
        TaskHub.JobRequestData jobRequestData = await this.PrepareJobRequestAsync(requestContext, scopeId, planId, jobId, (TaskOrchestrationJobAttempt) null, (string) null);
        TaskInstance taskInstance = jobRequestData.ContainerJob.Tasks.SingleOrDefault<TaskInstance>((Func<TaskInstance, bool>) (t => t.InstanceId == taskId)) != null ? jobRequestData.ContainerJob.Tasks.Single<TaskInstance>((Func<TaskInstance, bool>) (t => t.InstanceId == taskId)) : jobRequestData.ContainerJob.Tasks.Single<TaskInstance>();
        TaskDefinition taskDefinition = this.GetTaskDefinition(requestContext, taskInstance.Id, taskInstance.Version);
        IServerTaskHandler handler;
        if (!TaskHub.TryGetServerJobHandler(requestContext, taskDefinition, out handler))
          throw new ServerExecutionHandlerNotFoundException(TaskResources.TaskExecutionDefinitionInvalid((object) taskDefinition.Id));
        executionOptions = handler.GetTaskSectionExecutionOptions(requestContext, jobRequestData.Environment, taskDefinition, taskInstance);
      }
      return executionOptions;
    }

    internal async Task<CancelTaskResponse> CancelServerTaskAsync(
      IVssRequestContext requestContext,
      ServerTaskExecutionContext taskExecutionContext,
      Guid scopeId,
      Guid planId,
      Guid jobId,
      Guid taskId,
      TaskCanceledReasonType reasonType)
    {
      CancelTaskResponse cancelTaskResponse;
      using (new MethodScope(requestContext, nameof (TaskHub), nameof (CancelServerTaskAsync)))
      {
        TaskHub.JobRequestData jobRequestData = await this.PrepareJobRequestAsync(requestContext, scopeId, planId, jobId, (TaskOrchestrationJobAttempt) null, (string) null);
        TaskInstance taskInstance = jobRequestData.ContainerJob.Tasks.Single<TaskInstance>((Func<TaskInstance, bool>) (t => t.InstanceId == taskId));
        TaskDefinition taskDefinition = this.GetTaskDefinition(requestContext, taskInstance.Id, taskInstance.Version);
        TaskHub.SetDefaultInputValues(taskInstance.Inputs, taskDefinition);
        IServerTaskHandler handler;
        if (!TaskHub.TryGetServerJobHandler(requestContext, taskDefinition, out handler))
          throw new ServerExecutionHandlerNotFoundException(TaskResources.TaskExecutionDefinitionInvalid((object) taskDefinition.Id));
        ServerTaskRequestMessage taskRequestMessage = new ServerTaskRequestMessage(jobRequestData.Plan.AsReference(), jobRequestData.Plan.Timeline, jobId, jobRequestData.ContainerJob.Name, jobRequestData.ContainerJob.RefName, jobRequestData.Environment, taskInstance, taskDefinition);
        this.SetSystemVariablesForServerTask(taskRequestMessage);
        cancelTaskResponse = await handler.CancelTaskAsync(requestContext, taskExecutionContext, taskRequestMessage, reasonType);
      }
      return cancelTaskResponse;
    }

    internal async Task<Job> ExpandJobAsync(
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      Guid planId,
      JobParameters parameters)
    {
      Job definition;
      using (new MethodScope(requestContext, nameof (TaskHub), nameof (ExpandJobAsync)))
      {
        TaskOrchestrationPlan plan = await this.GetPlanAsync(requestContext, scopeIdentifier, planId);
        if (plan == null)
          throw new TaskOrchestrationPlanNotFoundException(TaskResources.PlanNotFound((object) planId));
        JobExecutionContext jobContext = await CreateJobContext();
        if (jobContext == null || jobContext.Job == null)
          throw new TaskOrchestrationJobNotFoundException(TaskResources.JobNotFound((object) (parameters.PhaseName + "." + parameters.Name), (object) planId));
        int num = 1;
        Guid instanceId = jobContext.GetInstanceId();
        List<Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord> taskRecords = new List<Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord>();
        foreach (TaskStep taskStep in jobContext.Job.Definition.Steps.OfType<TaskStep>())
          taskRecords.Add(new Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord()
          {
            Id = taskStep.Id,
            Order = new int?(num++),
            Name = taskStep.DisplayName,
            ParentId = new Guid?(instanceId),
            RecordType = "Task",
            RefName = taskStep.Name,
            State = new Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecordState?(Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecordState.Pending)
          });
        if (taskRecords.Count > 0)
        {
          Microsoft.VisualStudio.Services.Identity.Identity userIdentity = requestContext.GetUserIdentity();
          Microsoft.TeamFoundation.DistributedTask.WebApi.Timeline timeline;
          using (TaskTrackingComponent trackingComponent = requestContext.CreateComponent<TaskTrackingComponent>(this.DataspaceCategory))
            timeline = await trackingComponent.UpdateTimelineAsync(scopeIdentifier, planId, plan.Timeline.Id, userIdentity.Id, (IList<Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord>) taskRecords);
          TaskHub.TraceTimelineRecordUpdates(requestContext, (IList<Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord>) taskRecords, (IList<Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord>) timeline.Records, nameof (ExpandJobAsync));
          if (timeline != null && timeline.Records.Count > 0)
            this.Extension.TimelineRecordsUpdated(requestContext, (Microsoft.TeamFoundation.DistributedTask.WebApi.TaskOrchestrationPlanReference) plan, (Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineReference) timeline, (IEnumerable<Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord>) timeline.Records);
        }
        definition = jobContext.Job.Definition;
        // ISSUE: variable of a compiler-generated type
        TaskHub.\u003C\u003Ec__DisplayClass107_0 displayClass1070;

        async Task<JobExecutionContext> CreateJobContext()
        {
          TaskHub.PipelineTraceWriter trace = new TaskHub.PipelineTraceWriter();
          PipelineProcess process = plan.GetProcess<PipelineProcess>();
          string stageName = parameters.StageName ?? PipelineConstants.DefaultJobName;
          Stage stage1 = process.Stages.SingleOrDefault<Stage>((Func<Stage, bool>) (x => x.Name.Equals(stageName, StringComparison.OrdinalIgnoreCase)));
          // ISSUE: reference to a compiler-generated method
          PhaseNode phaseNode = stage1 != null ? stage1.Phases.SingleOrDefault<PhaseNode>(new Func<PhaseNode, bool>(displayClass1070.\u003CExpandJobAsync\u003Eb__2)) : (PhaseNode) null;
          if (phaseNode == null)
            return (JobExecutionContext) null;
          StageInstance stage2 = new StageInstance(stage1, parameters.StageAttempt);
          PhaseInstance phase1 = new PhaseInstance(phaseNode, parameters.PhaseAttempt);
          PhaseExecutionContext executionContext = await this.CreatePhaseExecutionContextAsync(requestContext, plan, stage2, phase1, (IPipelineTraceWriter) trace, includeOutputs: true);
          switch (phaseNode)
          {
            case Microsoft.TeamFoundation.DistributedTask.Pipelines.Phase phase:
              return phase.CreateJobContext(executionContext, parameters.Name, parameters.Attempt);
            case ProviderPhase providerPhase:
              string jobInstanceName = executionContext.IdGenerator.GetJobInstanceName(parameters.StageName, parameters.PhaseName, parameters.Name, parameters.Attempt, parameters.CheckRerunAttempt);
              JobInstance jobInstanceAsync = await this.GetJobInstanceAsync(requestContext, plan.ScopeIdentifier, plan.PlanId, jobInstanceName, true);
              return jobInstanceAsync != null ? providerPhase.CreateJobContext(executionContext, jobInstanceAsync) : (JobExecutionContext) null;
            default:
              throw new NotSupportedException(phaseNode.GetType().FullName);
          }
        }
      }
      return definition;
    }

    internal async Task<ExecuteTaskResponse> ExecuteServerTaskAsync(
      IVssRequestContext requestContext,
      Guid scopeId,
      Guid planId,
      TaskParameters parameters)
    {
      TaskHub taskHub = this;
      ExecuteTaskResponse executeTaskResponse1;
      using (new MethodScope(requestContext, nameof (TaskHub), nameof (ExecuteServerTaskAsync)))
      {
        JobParameters parameters1 = new JobParameters()
        {
          Attempt = parameters.JobAttempt,
          Name = parameters.JobName,
          StageAttempt = parameters.StageAttempt,
          StageName = parameters.StageName,
          PhaseAttempt = parameters.PhaseAttempt,
          PhaseName = parameters.PhaseName,
          CheckRerunAttempt = parameters.CheckRerunAttempt
        };
        TaskHub.JobRequestData jobData = await taskHub.PrepareJobRequestAsync(requestContext, scopeId, planId, parameters1);
        JobExecutionContext jobContext = jobData.Context;
        await taskHub.SetJobOutputsAsync(requestContext, jobData.Plan, jobContext);
        CreateTaskResult createResult = jobData.Job.CreateTask(jobContext, parameters.Name);
        IServerTaskHandler handler;
        if (!TaskHub.TryGetServerJobHandler(requestContext, createResult.Definition, out handler))
          throw new ServerExecutionHandlerNotFoundException(TaskResources.TaskExecutionDefinitionInvalid((object) createResult.Definition.Id));
        JobEnvironment environment = new JobEnvironment(jobContext.Variables, jobData.MaskHints, jobData.Resources);
        ServerTaskRequestMessage taskRequest = new ServerTaskRequestMessage(jobData.Plan.AsReference(), jobData.Plan.Timeline, jobData.Job.Id, jobData.Job.DisplayName, jobData.Job.Name, environment, createResult.Task.ToLegacyTaskInstance(), createResult.Definition);
        TelemetryFactory.GetLogger(requestContext).PublishTaskHubExecuteTaskTelemetry(requestContext, taskHub, jobData.AuthorizationId, taskRequest, jobData.Plan.TemplateType.ToString());
        ExecuteTaskResponse executeTaskResponse2 = await handler.ExecuteTaskAsync(requestContext, TaskHub.GetDefaultServerTaskExecutionContext(), taskRequest);
        foreach (KeyValuePair<string, TaskEventConfig> taskEvent in executeTaskResponse2.TaskEvents.All)
          TaskHub.ResolveTaskEventVariables(taskEvent, createResult.Task.Inputs);
        executeTaskResponse1 = executeTaskResponse2;
      }
      return executeTaskResponse1;
    }

    internal async Task<CancelTaskResponse> CancelServerTaskAsync(
      IVssRequestContext requestContext,
      Guid scopeId,
      Guid planId,
      TaskParameters parameters,
      TaskCanceledReasonType reasonType)
    {
      CancelTaskResponse cancelTaskResponse;
      using (new MethodScope(requestContext, nameof (TaskHub), nameof (CancelServerTaskAsync)))
      {
        JobParameters parameters1 = new JobParameters()
        {
          Attempt = parameters.JobAttempt,
          Name = parameters.JobName,
          StageAttempt = parameters.StageAttempt,
          StageName = parameters.StageName,
          PhaseAttempt = parameters.PhaseAttempt,
          PhaseName = parameters.PhaseName,
          CheckRerunAttempt = parameters.CheckRerunAttempt
        };
        TaskHub.JobRequestData jobData = await this.PrepareJobRequestAsync(requestContext, scopeId, planId, parameters1);
        JobExecutionContext jobContext = jobData.Context;
        await this.SetJobOutputsAsync(requestContext, jobData.Plan, jobContext);
        CreateTaskResult task = jobData.Job.CreateTask(jobContext, parameters.Name);
        IServerTaskHandler handler;
        if (!TaskHub.TryGetServerJobHandler(requestContext, task.Definition, out handler))
          throw new ServerExecutionHandlerNotFoundException(TaskResources.TaskExecutionDefinitionInvalid((object) task.Definition.Id));
        JobEnvironment environment = new JobEnvironment(jobContext.Variables, jobData.MaskHints, jobData.Resources);
        ServerTaskRequestMessage taskRequest = new ServerTaskRequestMessage(jobData.Plan.AsReference(), jobData.Plan.Timeline, jobData.Job.Id, jobData.Job.DisplayName, jobData.Job.Name, environment, task.Task.ToLegacyTaskInstance(), task.Definition);
        cancelTaskResponse = await handler.CancelTaskAsync(requestContext, TaskHub.GetDefaultServerTaskExecutionContext(), taskRequest, reasonType);
      }
      return cancelTaskResponse;
    }

    internal string GetJobName(
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      Guid planId,
      Guid jobId)
    {
      Microsoft.TeamFoundation.DistributedTask.WebApi.TaskOrchestrationPlanReference planData = this.GetPlanData(requestContext, scopeIdentifier, planId);
      return this.Extension.GetJobName(requestContext, planData, jobId);
    }

    internal TaskOrchestrationOwnerReferences GetOwnerReferences(
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      Guid planId,
      Guid jobId)
    {
      using (new MethodScope(requestContext, nameof (TaskHub), nameof (GetOwnerReferences)))
      {
        Microsoft.TeamFoundation.DistributedTask.WebApi.TaskOrchestrationPlanReference planReference = this.EnsurePlanData(requestContext, scopeIdentifier, planId);
        if (planReference.Owner == null || planReference.Definition == null)
          this.Extension.PopulatePlanOwnerReferences(requestContext, planReference);
        return new TaskOrchestrationOwnerReferences()
        {
          Definition = planReference.Definition,
          Owner = planReference.Owner
        };
      }
    }

    internal async Task RaiseJobEventAsync(
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      Guid planId,
      Guid jobId,
      string eventName,
      JobEvent eventData,
      DateTime? fireAt = null,
      bool ensureOrchestrationExists = true)
    {
      TaskHub taskHub1 = this;
      MethodScope methodScope = new MethodScope(requestContext, nameof (TaskHub), nameof (RaiseJobEventAsync));
      try
      {
        Microsoft.TeamFoundation.DistributedTask.WebApi.TaskOrchestrationPlanReference plan = taskHub1.EnsurePlanData(requestContext, scopeIdentifier, planId);
        if (plan.ProcessType == OrchestrationProcessType.Container)
        {
          requestContext.Trace(10016121, TraceLevel.Info, "DistributedTask", nameof (TaskHub), string.Format("{0} Hub received event '{1}' for plan with Id : {2}", (object) taskHub1.Name, (object) eventName, (object) planId));
          await taskHub1.RaiseOrchestrationEventAsync(requestContext, plan.GetJobOrchestrationId(jobId), eventName, ConvertToInput(eventData), fireAt: fireAt);
        }
        else
        {
          string empty = string.Empty;
          string jobOrchestrationId;
          if (plan.Version < 4)
          {
            jobOrchestrationId = plan.GetJobOrchestrationId(jobId);
          }
          else
          {
            IList<Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord> timelineRecordsAsync;
            using (TaskTrackingComponent tc = taskHub1.CreateComponent<TaskTrackingComponent>(requestContext))
              timelineRecordsAsync = await tc.GetTimelineRecordsAsync(scopeIdentifier, planId, planId, (IEnumerable<Guid>) new Guid[1]
              {
                jobId
              });
            if (timelineRecordsAsync.Count <= 0)
              throw new TaskOrchestrationJobNotFoundException(TaskResources.JobNotFound((object) jobId, (object) planId));
            if (timelineRecordsAsync[0].Identifier == null)
            {
              requestContext.TraceError(10016138, nameof (TaskHub), "Event received for timeline record without an Identifier. This is most likely caused by an event being posted to an older attempt.");
              return;
            }
            jobOrchestrationId = plan.GetJobOrchestrationId(timelineRecordsAsync[0]);
          }
          TaskHub taskHub2 = taskHub1;
          IVssRequestContext requestContext1 = requestContext;
          string instanceId = jobOrchestrationId;
          string eventName1 = eventName;
          JobEvent eventData1 = eventData;
          object input = ConvertToInput(eventData1 != null ? eventData1.TrimForRuntime() : (JobEvent) null);
          int num = ensureOrchestrationExists ? 1 : 0;
          DateTime? fireAt1 = fireAt;
          await taskHub2.RaiseOrchestrationEventAsync(requestContext1, instanceId, eventName1, input, num != 0, fireAt1);
        }
        plan = (Microsoft.TeamFoundation.DistributedTask.WebApi.TaskOrchestrationPlanReference) null;
      }
      finally
      {
        methodScope.Dispose();
      }
      methodScope = new MethodScope();

      static object ConvertToInput(JobEvent jobEvent)
      {
        if (!(jobEvent is JobCanceledEvent jobCanceledEvent))
          return (object) jobEvent;
        return (object) new CanceledEvent()
        {
          Reason = jobCanceledEvent.Reason,
          Timeout = jobCanceledEvent.Timeout
        };
      }
    }

    internal async Task RaiseTaskEventAsync(
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      Guid planId,
      Guid jobId,
      Guid taskId,
      string eventName,
      TaskEvent eventData,
      DateTime? fireAt = null)
    {
      TaskHub taskHub1 = this;
      MethodScope methodScope = new MethodScope(requestContext, nameof (TaskHub), nameof (RaiseTaskEventAsync));
      try
      {
        Microsoft.TeamFoundation.DistributedTask.WebApi.TaskOrchestrationPlanReference plan = taskHub1.EnsurePlanData(requestContext, scopeIdentifier, planId);
        if (plan.ProcessType == OrchestrationProcessType.Container)
        {
          string instanceId = plan.Version >= 10 ? plan.GetServerTaskOrchestrationId(jobId, taskId) : plan.GetJobOrchestrationId(jobId);
          await taskHub1.RaiseOrchestrationEventAsync(requestContext, instanceId, eventName, (object) eventData, true, fireAt);
        }
        else
        {
          string empty = string.Empty;
          string str;
          if (plan.Version < 2)
            str = plan.GetJobOrchestrationId(jobId);
          else if (plan.Version < 4)
          {
            str = plan.GetServerTaskOrchestrationId(jobId, taskId);
          }
          else
          {
            IList<Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord> timelineRecordsAsync;
            using (TaskTrackingComponent tc = taskHub1.CreateComponent<TaskTrackingComponent>(requestContext))
              timelineRecordsAsync = await tc.GetTimelineRecordsAsync(scopeIdentifier, planId, planId, (IEnumerable<Guid>) new Guid[2]
              {
                jobId,
                taskId
              });
            if (timelineRecordsAsync.Count != 2)
              throw new TaskOrchestrationJobNotFoundException(TaskResources.JobNotFound((object) jobId, (object) planId));
            str = plan.GetServerTaskOrchestrationId(timelineRecordsAsync.Single<Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord>((Func<Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord, bool>) (x => x.Id == jobId)), timelineRecordsAsync.Single<Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord>((Func<Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord, bool>) (x => x.Id == taskId)));
          }
          TaskHub taskHub2 = taskHub1;
          IVssRequestContext requestContext1 = requestContext;
          string instanceId = str;
          string eventName1 = eventName;
          TaskEvent eventData1 = eventData;
          JobEvent eventData2 = eventData1 != null ? eventData1.TrimForRuntime() : (JobEvent) null;
          DateTime? fireAt1 = fireAt;
          await taskHub2.RaiseOrchestrationEventAsync(requestContext1, instanceId, eventName1, (object) eventData2, true, fireAt1);
        }
        plan = (Microsoft.TeamFoundation.DistributedTask.WebApi.TaskOrchestrationPlanReference) null;
      }
      finally
      {
        methodScope.Dispose();
      }
      methodScope = new MethodScope();
    }

    internal async Task RaiseOrchestrationEventAsync(
      IVssRequestContext requestContext,
      string instanceId,
      string eventName,
      object eventData,
      bool ensureOrchestrationExist = false,
      DateTime? fireAt = null)
    {
      MethodScope methodScope = new MethodScope(requestContext, nameof (TaskHub), nameof (RaiseOrchestrationEventAsync));
      try
      {
        using (requestContext.CreateOrchestrationIdScope(instanceId))
        {
          this.TraceIfExternal(requestContext, eventName, eventData, fireAt);
          TaskHub.TraceReasonIfCanceledEvent(requestContext, eventData);
          OrchestrationHubDescription orchestrationHubDescription = this.EnsureOrchestrationHubExists(requestContext);
          OrchestrationService service = requestContext.GetService<OrchestrationService>();
          IVssRequestContext requestContext1 = requestContext;
          string hubName = orchestrationHubDescription.HubName;
          OrchestrationInstance instance = new OrchestrationInstance();
          instance.InstanceId = instanceId;
          string eventName1 = eventName;
          object eventData1 = eventData;
          int num = ensureOrchestrationExist ? 1 : 0;
          DateTime? fireAt1 = fireAt;
          await service.RaiseEventAsync(requestContext1, hubName, instance, eventName1, eventData1, num != 0, fireAt1);
        }
      }
      finally
      {
        methodScope.Dispose();
      }
      methodScope = new MethodScope();
    }

    private void TraceIfExternal(
      IVssRequestContext requestContext,
      string eventName,
      object eventData,
      DateTime? fireAt)
    {
      if (!requestContext.IsFeatureEnabled("DistributedTask.TraceExternallyRaisedTaskHubEvents"))
        return;
      try
      {
        string authenticationMechanism = requestContext.GetAuthenticationMechanism();
        if ((!(authenticationMechanism != "") ? 0 : (authenticationMechanism != "S2S_ServicePrincipal" ? 1 : 0)) == 0)
          return;
        requestContext.TraceAlways(10015562, nameof (TaskHub), JsonConvert.SerializeObject((object) new
        {
          Message = "External orchestration event received by AT!",
          AuthMechanism = authenticationMechanism,
          EventName = eventName,
          FireAt = fireAt
        }));
      }
      catch (Exception ex)
      {
        requestContext.TraceException(10015562, nameof (TaskHub), ex);
      }
    }

    public static void TraceReasonIfCanceledEvent(
      IVssRequestContext requestContext,
      object eventData)
    {
      if (!requestContext.IsFeatureEnabled("DistributedTask.TraceCancelationReason"))
        return;
      try
      {
        if (!(eventData is CanceledEvent canceledEvent))
          return;
        requestContext.TraceAlways(10015563, TraceLevel.Warning, "DistributedTask", nameof (TaskHub), "'Canceled' event raised. Reason: " + canceledEvent.Reason + ". StackTrace: " + System.Environment.StackTrace);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(10015563, nameof (TaskHub), ex);
      }
    }

    internal void ResolveVariablesInTaskDisplayName(
      JobEnvironment environment,
      TaskOrchestrationJob job)
    {
      HashSet<string> secretVariables = new HashSet<string>(environment.MaskHints.Where<MaskHint>((Func<MaskHint, bool>) (hint => hint.Type == MaskType.Variable)).Select<MaskHint, string>((Func<MaskHint, string>) (hint => hint.Value)), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      Dictionary<string, string> dictionary = environment.Variables.Where<KeyValuePair<string, string>>((Func<KeyValuePair<string, string>, bool>) (v => !secretVariables.Contains(v.Key))).ToDictionary<KeyValuePair<string, string>, string, string>((Func<KeyValuePair<string, string>, string>) (v => v.Key), (Func<KeyValuePair<string, string>, string>) (v => v.Value), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      foreach (TaskInstance task in job.Tasks)
      {
        if (!string.IsNullOrEmpty(task.DisplayName))
          task.DisplayName = VariableUtility.ExpandVariables(task.DisplayName, (IDictionary<string, string>) dictionary, false);
      }
    }

    internal Microsoft.TeamFoundation.DistributedTask.WebApi.TaskOrchestrationPlanReference EnsurePlanData(
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      Guid planId)
    {
      return this.GetPlanData(requestContext, scopeIdentifier, planId) ?? throw new TaskOrchestrationPlanNotFoundException(TaskResources.PlanNotFound((object) planId));
    }

    private static void CheckCollectionReadPermission(IVssRequestContext requestContext) => requestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(requestContext, FrameworkSecurity.FrameworkNamespaceId).CheckPermission(requestContext, FrameworkSecurity.FrameworkNamespaceToken, 1);

    private static string GetAccessToken(Microsoft.TeamFoundation.DistributedTask.WebApi.EndpointAuthorization authorization)
    {
      if (authorization == null || string.IsNullOrEmpty(authorization.Scheme) || authorization.Parameters.Count == 0)
        return (string) null;
      string scheme = authorization.Scheme;
      string str;
      return (scheme == "OAuth" || scheme == "OAuthWrap" || scheme == "OAuth2") && authorization.Parameters.TryGetValue("AccessToken", out str) ? str : (string) null;
    }

    private static bool TryGetServerJobHandler(
      IVssRequestContext requestContext,
      TaskDefinition definition,
      out IServerTaskHandler handler)
    {
      handler = requestContext.GetExtension<IServerTaskHandler>((Func<IServerTaskHandler, bool>) (x => definition.Execution.Keys.Contains<string>(x.HandlerName, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)));
      return handler != null;
    }

    private static EventsConfig GetServerTaskEventsConfig(
      TaskDefinition definition,
      string handlerName)
    {
      return definition.Execution.FirstOrDefault<KeyValuePair<string, JObject>>((Func<KeyValuePair<string, JObject>, bool>) (x => string.Equals(x.Key, handlerName, StringComparison.InvariantCultureIgnoreCase))).Value?.ToObject<ServerExecutionDefinition>()?.EventsConfig;
    }

    private static TaskEventsConfig ExpandAndValidateTaskEventsConfiguration(
      TaskDefinition definition,
      IDictionary<string, string> taskInput,
      string handlerName)
    {
      EventsConfig taskEventsConfig1 = TaskHub.GetServerTaskEventsConfig(definition, handlerName);
      Dictionary<string, string> dictionary = new Dictionary<string, string>();
      if (taskEventsConfig1 is JobEventsConfig jobEventsConfig)
      {
        foreach (KeyValuePair<string, JobEventConfig> keyValuePair in jobEventsConfig.All)
        {
          if (!TimeSpan.TryParse(keyValuePair.Value.Timeout, (IFormatProvider) CultureInfo.InvariantCulture, out TimeSpan _))
            dictionary.Add(keyValuePair.Key, keyValuePair.Value.Timeout);
        }
      }
      else if (taskEventsConfig1 is TaskEventsConfig taskEventsConfig2)
      {
        foreach (KeyValuePair<string, TaskEventConfig> taskEvent in taskEventsConfig2.All)
        {
          TaskHub.ResolveTaskEventVariables(taskEvent, taskInput);
          if (!TimeSpan.TryParse(taskEvent.Value.Timeout, (IFormatProvider) CultureInfo.InvariantCulture, out TimeSpan _))
            dictionary.Add(taskEvent.Key, taskEvent.Value.Timeout);
        }
      }
      if (dictionary.Count > 0)
      {
        string str = string.Join(",", (IEnumerable<string>) dictionary.Keys);
        throw new TaskDefinitionInvalidException(TaskResources.TimeoutFormatNotValid((object) string.Join(",", (IEnumerable<string>) dictionary.Values), (object) str, (object) definition.Id));
      }
      return taskEventsConfig1 as TaskEventsConfig;
    }

    private static void ResolveTaskEventVariables(
      KeyValuePair<string, TaskEventConfig> taskEvent,
      IDictionary<string, string> taskInputs)
    {
      taskEvent.Value.Enabled = VariableUtility.ExpandVariables((JToken) taskEvent.Value.Enabled, taskInputs) as JValue;
    }

    private static Uri GetVssClientUrl(
      IVssRequestContext requestContext,
      Guid serviceAreaIdentifier)
    {
      return requestContext.ExecutionEnvironment.IsOnPremisesDeployment ? TaskHub.GetVssPublicUrl(requestContext, serviceAreaIdentifier) : new Uri(requestContext.GetService<ILocationService>().GetLocationServiceUrl(requestContext, serviceAreaIdentifier, AccessMappingConstants.ClientAccessMappingMoniker), UriKind.Absolute);
    }

    private static Uri GetVssPublicUrl(
      IVssRequestContext requestContext,
      Guid serviceAreaIdentifier)
    {
      return new Uri(requestContext.GetService<ILocationService>().GetLocationServiceUrl(requestContext, serviceAreaIdentifier, AccessMappingConstants.PublicAccessMappingMoniker), UriKind.Absolute);
    }

    private static bool ContainsRollbackAtSubContainers(TaskOrchestrationContainer implementation) => !implementation.Children.All<TaskOrchestrationItem>(TaskHub.\u003C\u003EO.\u003C0\u003E__IsValidSubContainer ?? (TaskHub.\u003C\u003EO.\u003C0\u003E__IsValidSubContainer = new Func<TaskOrchestrationItem, bool>(TaskHub.IsValidSubContainer)));

    private static bool IsValidSubContainer(TaskOrchestrationItem item)
    {
      if (!(item is TaskOrchestrationContainer))
        return true;
      TaskOrchestrationContainer orchestrationContainer = item as TaskOrchestrationContainer;
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      return orchestrationContainer.Rollback == null && orchestrationContainer.Children.All<TaskOrchestrationItem>(TaskHub.\u003C\u003EO.\u003C0\u003E__IsValidSubContainer ?? (TaskHub.\u003C\u003EO.\u003C0\u003E__IsValidSubContainer = new Func<TaskOrchestrationItem, bool>(TaskHub.IsValidSubContainer)));
    }

    private TaskDefinition GetTaskDefinition(
      IVssRequestContext requestContext,
      Guid taskId,
      string version)
    {
      using (new MethodScope(requestContext, nameof (TaskHub), nameof (GetTaskDefinition)))
        return requestContext.GetService<IDistributedTaskPoolService>().GetTaskDefinition(requestContext, taskId, version) ?? throw new TaskDefinitionNotFoundException(TaskResources.TaskDefinitionNotFound((object) taskId, (object) version));
    }

    private IList<TaskDefinition> GetTaskDefinitions(
      IVssRequestContext requestContext,
      IList<Microsoft.TeamFoundation.DistributedTask.WebApi.TaskReference> taskReferences)
    {
      List<TaskDefinition> taskDefinitions = new List<TaskDefinition>();
      foreach (Microsoft.TeamFoundation.DistributedTask.WebApi.TaskReference taskReference in (IEnumerable<Microsoft.TeamFoundation.DistributedTask.WebApi.TaskReference>) taskReferences)
      {
        try
        {
          TaskDefinition taskDefinition = this.GetTaskDefinition(requestContext, taskReference.Id, taskReference.Version);
          taskDefinitions.Add(taskDefinition);
        }
        catch (TaskDefinitionNotFoundException ex)
        {
          requestContext.TraceException(10016207, nameof (TaskHub), (Exception) ex);
        }
      }
      return (IList<TaskDefinition>) taskDefinitions;
    }

    private async Task<TaskAttachmentData> GetAttachmentInternalAsync(
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      Guid planId,
      Guid timelineId,
      Guid recordId,
      string type,
      string name)
    {
      using (new MethodScope(requestContext, nameof (TaskHub), nameof (GetAttachmentInternalAsync)))
      {
        ArgumentUtility.CheckForEmptyGuid(planId, nameof (planId));
        ArgumentUtility.CheckForEmptyGuid(timelineId, nameof (timelineId));
        ArgumentUtility.CheckForEmptyGuid(recordId, nameof (recordId));
        ArgumentUtility.CheckStringForNullOrEmpty(type, nameof (type));
        ArgumentUtility.CheckStringForNullOrEmpty(name, nameof (name));
        Microsoft.TeamFoundation.DistributedTask.WebApi.TaskOrchestrationPlanReference plan = this.GetPlanData(requestContext, scopeIdentifier, planId);
        if (plan == null || !this.Extension.HasReadPermission(requestContext, plan.ScopeIdentifier, plan.ArtifactUri))
          return (TaskAttachmentData) null;
        TaskAttachmentData attachmentData = (TaskAttachmentData) null;
        using (TaskTrackingComponent trackingComponent = requestContext.CreateComponent<TaskTrackingComponent>(this.DataspaceCategory))
          attachmentData = await trackingComponent.GetAttachmentAsync(plan.ScopeIdentifier, plan.PlanId, timelineId, recordId, type, name);
        if (attachmentData != null)
          attachmentData.AddLinks(requestContext, plan.ScopeIdentifier, this.Name, plan.PlanId);
        return attachmentData;
      }
    }

    private string GetStoreFeedLinePrefix(Guid taskId) => string.Format("{0:D}/", (object) taskId);

    private static List<TaskReferenceData> GetTaskReferences(
      IVssRequestContext requestContext,
      IEnumerable<TaskInstance> tasks)
    {
      List<TaskReferenceData> taskReferences = new List<TaskReferenceData>();
      foreach (TaskInstance task in tasks)
      {
        TaskReferenceData taskReference = TaskHub.GetTaskReference(requestContext, task.InstanceId, task.Id, task.Version);
        if (taskReference != null)
          taskReferences.Add(taskReference);
      }
      return taskReferences;
    }

    private static List<TaskReferenceData> GetTaskReferences(
      IVssRequestContext requestContext,
      IEnumerable<JobStep> steps)
    {
      List<TaskReferenceData> taskReferences1 = new List<TaskReferenceData>();
      foreach (JobStep step in steps)
      {
        if (step.Type == StepType.Task)
        {
          TaskStep taskStep = step as TaskStep;
          TaskReferenceData taskReference = TaskHub.GetTaskReference(requestContext, taskStep.Id, taskStep.Reference.Id, taskStep.Reference.Version);
          if (taskReference != null)
            taskReferences1.Add(taskReference);
        }
        else if (step.Type == StepType.Group)
        {
          List<TaskReferenceData> taskReferences2 = TaskHub.GetTaskReferences(requestContext, (IEnumerable<JobStep>) (step as GroupStep).Steps);
          if (taskReferences2 != null)
            taskReferences1.AddRange((IEnumerable<TaskReferenceData>) taskReferences2);
        }
      }
      return taskReferences1;
    }

    private static TaskReferenceData GetTaskReference(
      IVssRequestContext requestContext,
      Guid id,
      Guid taskId,
      string taskVersion)
    {
      TaskDefinition taskDefinition = requestContext.GetService<IDistributedTaskPoolService>().GetTaskDefinition(requestContext, taskId, taskVersion);
      if (taskDefinition == null)
        return (TaskReferenceData) null;
      return new TaskReferenceData()
      {
        InstanceId = id,
        Task = new Microsoft.TeamFoundation.DistributedTask.WebApi.TaskReference()
        {
          Id = taskDefinition.Id,
          Name = taskDefinition.Name,
          Version = (string) taskDefinition.Version
        }
      };
    }

    private void EnsureLibraryPermission(
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      Microsoft.VisualStudio.Services.Identity.Identity serviceIdentity)
    {
      IVssSecurityNamespace securityNamespace = requestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(requestContext, LibrarySecurityProvider.LibraryNamespaceId);
      string rootToken = LibrarySecurityProvider.GetRootToken(new Guid?(scopeIdentifier));
      if (securityNamespace == null || securityNamespace.HasPermissionExpect(requestContext, rootToken, 1, true, false))
        return;
      AccessControlEntry accessControlEntry1 = new AccessControlEntry(serviceIdentity.Descriptor, 1, 0);
      IAccessControlList accessControlList = securityNamespace.QueryAccessControlList(requestContext, rootToken, (IEnumerable<IdentityDescriptor>) new IdentityDescriptor[1]
      {
        serviceIdentity.Descriptor
      }, false);
      if (accessControlList != null)
      {
        IAccessControlEntry accessControlEntry2 = accessControlList.QueryAccessControlEntry(serviceIdentity.Descriptor);
        if (accessControlEntry2 != null)
          accessControlEntry1.Allow |= accessControlEntry2.Allow;
      }
      securityNamespace.SetAccessControlEntry(requestContext, rootToken, (IAccessControlEntry) accessControlEntry1, false);
    }

    private async Task EnsurePipelineCachePermissionAsync(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Identity.Identity serviceIdentity,
      IEnumerable<Guid> taskIds,
      IDictionary<string, VariableValue> variables)
    {
      if (!TaskHub.IsPipelineCachingEnabled(requestContext, taskIds, variables))
        return;
      await this.EnsurePipelineCachePermissionInnerAsync(requestContext, serviceIdentity);
    }

    private async Task EnsurePipelineCachePermissionAsync(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Identity.Identity serviceIdentity,
      IEnumerable<Guid> taskIds,
      IDictionary<string, string> variables)
    {
      if (!TaskHub.IsPipelineCachingEnabled(requestContext, taskIds, variables))
        return;
      await this.EnsurePipelineCachePermissionInnerAsync(requestContext, serviceIdentity);
    }

    public static bool IsPipelineCachingEnabled(
      IVssRequestContext requestContext,
      IEnumerable<Guid> taskIds,
      IList<IVariable> variables)
    {
      return TaskHub.IsPipelineCachingEnabled(requestContext, taskIds, (Func<string, string>) (varName => variables.OfType<Variable>().LastOrDefault<Variable>((Func<Variable, bool>) (v => v.Name == varName))?.Value));
    }

    public static bool IsPipelineCachingEnabled(
      IVssRequestContext requestContext,
      IEnumerable<Guid> taskIds,
      IDictionary<string, string> variables)
    {
      return TaskHub.IsPipelineCachingEnabled(requestContext, taskIds, (Func<string, string>) (v =>
      {
        string str = (string) null;
        variables.TryGetValue(v, out str);
        return str;
      }));
    }

    public static bool IsPipelineCachingEnabled(
      IVssRequestContext requestContext,
      IEnumerable<Guid> taskIds,
      IDictionary<string, VariableValue> variables)
    {
      return TaskHub.IsPipelineCachingEnabled(requestContext, taskIds, (Func<string, string>) (v =>
      {
        VariableValue variableValue = (VariableValue) null;
        variables.TryGetValue(v, out variableValue);
        return variableValue?.Value;
      }));
    }

    private static bool IsPipelineCachingEnabled(
      IVssRequestContext requestContext,
      IEnumerable<Guid> taskIds,
      Func<string, string> fetchVarValue)
    {
      if (taskIds != null && taskIds.Any<Guid>((Func<Guid, bool>) (t => t == PipelineCacheConstants.PipelineCacheTaskId)))
        return true;
      if (requestContext.IsFeatureEnabled("Artifact.Features.PipelineCache.EnableByVariable"))
      {
        string str = fetchVarValue("EnablePipelineCache");
        bool result;
        if (str != null && bool.TryParse(str, out result))
          return result;
      }
      return false;
    }

    private async Task EnsurePipelineCachePermissionInnerAsync(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Identity.Identity serviceIdentity)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      vssRequestContext.GetService<IInstanceManagementService>().GetHostInstanceMapping(vssRequestContext, requestContext.ServiceHost.InstanceId, Microsoft.VisualStudio.Services.Content.Common.ServiceInstanceTypes.Artifact);
      TimeSpan NumOfMillisecondsDelayOnRetry = TimeSpan.FromSeconds(1.0);
      ITeamFoundationSecurityService securityService = requestContext.GetService<ITeamFoundationSecurityService>();
      IVssSecurityNamespace pipelineCacheNamespace = (IVssSecurityNamespace) null;
      for (int i = 0; i < 45; ++i)
      {
        pipelineCacheNamespace = securityService.GetSecurityNamespace(requestContext, SecurityDefintions.NamespaceId);
        if (pipelineCacheNamespace == null)
          await Task.Delay(NumOfMillisecondsDelayOnRetry).ConfigureAwait(true);
        else
          break;
      }
      string token = "$";
      if (pipelineCacheNamespace.HasPermissionExpect(requestContext, token, 3, true, false))
      {
        securityService = (ITeamFoundationSecurityService) null;
        pipelineCacheNamespace = (IVssSecurityNamespace) null;
      }
      else
      {
        AccessControlEntry accessControlEntry1 = new AccessControlEntry(serviceIdentity.Descriptor, 3, 0);
        IAccessControlList accessControlList = pipelineCacheNamespace.QueryAccessControlList(requestContext, token, (IEnumerable<IdentityDescriptor>) new IdentityDescriptor[1]
        {
          serviceIdentity.Descriptor
        }, false);
        if (accessControlList != null)
        {
          IAccessControlEntry accessControlEntry2 = accessControlList.QueryAccessControlEntry(serviceIdentity.Descriptor);
          if (accessControlEntry2 != null)
            accessControlEntry1.Allow |= accessControlEntry2.Allow;
        }
        pipelineCacheNamespace.SetAccessControlEntry(requestContext, token, (IAccessControlEntry) accessControlEntry1, false);
        securityService = (ITeamFoundationSecurityService) null;
        pipelineCacheNamespace = (IVssSecurityNamespace) null;
      }
    }

    private async Task<TaskHub.JobRequestData> PrepareJobRequestAsync(
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      Guid planId,
      Guid jobId,
      TaskOrchestrationJobAttempt jobAttempt,
      string workerName)
    {
      TaskHub hub = this;
      TaskHub.JobRequestData jobRequestData;
      using (new MethodScope(requestContext, nameof (TaskHub), nameof (PrepareJobRequestAsync)))
      {
        Microsoft.TeamFoundation.DistributedTask.WebApi.TaskOrchestrationPlanReference planData = hub.EnsurePlanData(requestContext, scopeIdentifier, planId);
        TaskOrchestrationPlan plan = (TaskOrchestrationPlan) null;
        TaskOrchestrationJob job = (TaskOrchestrationJob) null;
        if (jobAttempt != null && !jobAttempt.TemplateJobId.Equals(jobId))
        {
          GetTaskOrchestrationJobResult jobAsync = await hub.GetJobAsync(requestContext, scopeIdentifier, planId, jobAttempt.TemplateJobId);
          job = jobAsync.Job;
          plan = jobAsync.Plan;
          if (plan == null)
            throw new TaskOrchestrationPlanNotFoundException(TaskResources.PlanNotFound((object) planId));
          if (job == null)
            throw new TaskOrchestrationJobNotFoundException(TaskResources.JobNotFound((object) jobId, (object) planId));
          Microsoft.TeamFoundation.DistributedTask.WebApi.Timeline timeline = await hub.AddJobAsync(requestContext, scopeIdentifier, planId, jobId, jobAttempt.AttemptId, workerName, job);
        }
        else
        {
          GetTaskOrchestrationJobResult jobAsync = await hub.GetJobAsync(requestContext, planData.ScopeIdentifier, planData.PlanId, jobId);
          job = jobAsync.Job;
          plan = jobAsync.Plan;
          if (plan == null)
            throw new TaskOrchestrationPlanNotFoundException(TaskResources.PlanNotFound((object) planId));
          if (job == null)
            throw new TaskOrchestrationJobNotFoundException(TaskResources.JobNotFound((object) jobId, (object) planId));
          if (workerName != null)
          {
            Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord timelineRecord = new Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord()
            {
              Id = job.InstanceId,
              Name = job.Name,
              RefName = job.RefName,
              RecordType = "Job",
              WorkerName = workerName
            };
            Microsoft.TeamFoundation.DistributedTask.WebApi.Timeline timeline = await hub.UpdateTimelineAsync(requestContext, scopeIdentifier, planId, plan.Timeline.Id, (IList<Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord>) new List<Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord>()
            {
              timelineRecord
            });
          }
        }
        JobEnvironment environment = hub.CreateJobEnvironment(requestContext, scopeIdentifier, planId, plan, job);
        Microsoft.VisualStudio.Services.Identity.Identity systemIdentity = (Microsoft.VisualStudio.Services.Identity.Identity) null;
        systemIdentity = job.ExecuteAs == null ? hub.Extension.GetJobServiceIdentity(requestContext, plan, (IDictionary<string, VariableValue>) new VariablesDictionary(environment.Variables)) : requestContext.GetService<IdentityService>().GetIdentity(requestContext, job.ExecuteAs.Id);
        IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
        using (IVssRequestContext impersonationContext = vssRequestContext.GetService<ITeamFoundationHostManagementService>().BeginUserRequest(vssRequestContext, requestContext.ServiceHost.InstanceId, systemIdentity.Descriptor))
        {
          hub.EnsureLibraryPermission(impersonationContext, planData.ScopeIdentifier, systemIdentity);
          TaskHub taskHub = hub;
          IVssRequestContext requestContext1 = impersonationContext;
          Microsoft.VisualStudio.Services.Identity.Identity serviceIdentity = systemIdentity;
          List<TaskInstance> tasks = job.Tasks;
          IEnumerable<Guid> taskIds = tasks != null ? tasks.Select<TaskInstance, Guid>((Func<TaskInstance, Guid>) (t => t.Id)) : (IEnumerable<Guid>) null;
          IDictionary<string, string> variables = environment.Variables;
          await taskHub.EnsurePipelineCachePermissionAsync(requestContext1, serviceIdentity, taskIds, variables);
          bool flag = false;
          try
          {
            hub.Extension.CheckWritePermission(impersonationContext, planData.ScopeIdentifier, planData.PlanId, planData.ArtifactUri);
            flag = true;
          }
          catch (TaskOrchestrationPlanSecurityException ex)
          {
            requestContext.TraceException(10015503, nameof (TaskHub), (Exception) ex);
          }
          if (!flag)
          {
            hub.Extension.SetPermissions(requestContext, plan, systemIdentity);
            try
            {
              hub.Extension.CheckWritePermission(impersonationContext, planData.ScopeIdentifier, planData.PlanId, planData.ArtifactUri);
              requestContext.TraceError(10015504, nameof (TaskHub), "Successfully corrected plan security error");
            }
            catch (TaskOrchestrationPlanSecurityException ex)
            {
              requestContext.TraceException(10015503, nameof (TaskHub), (Exception) ex);
              throw;
            }
          }
        }
        Dictionary<string, string> dictionary1 = new Dictionary<string, string>()
        {
          {
            "jobref",
            TaskHub.JobRefClaimValue(planId, job.InstanceId)
          }
        };
        TaskHub taskHub1 = hub;
        IVssRequestContext requestContext2 = requestContext;
        TimeSpan? executionTimeout = job.ExecutionTimeout;
        int jobTimeoutInMinutes;
        if (!executionTimeout.HasValue)
        {
          jobTimeoutInMinutes = 0;
        }
        else
        {
          executionTimeout = job.ExecutionTimeout;
          jobTimeoutInMinutes = Convert.ToInt32(executionTimeout.Value.TotalMinutes);
        }
        Microsoft.VisualStudio.Services.Identity.Identity identity = systemIdentity;
        string valueOrDefault = environment.Variables.GetValueOrDefault<string, string>(WellKnownDistributedTaskVariables.AccessTokenScope);
        string pipelinePhaseIdentifier = plan.ArtifactUri?.ToString();
        Dictionary<string, string> additionalClaims = dictionary1;
        TaskHub.JobAuthorizationResult jobAuthorization = taskHub1.GetJobAuthorization(requestContext2, jobTimeoutInMinutes, 0, identity, "SystemVssConnection", valueOrDefault, pipelinePhaseIdentifier, additionalClaims: (IDictionary<string, string>) additionalClaims);
        environment.SystemConnection = jobAuthorization.Endpoint != null ? jobAuthorization.Endpoint : throw new CreatePersonalAccessTokenFailedException(TaskResources.FailedToObtainJobAuthorization((object) job.Name, (object) plan.PlanType, (object) systemIdentity.DisplayName));
        Dictionary<string, VariableValue> modernVariables = new Dictionary<string, VariableValue>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        HashSet<MaskHint> modernMaskhints = new HashSet<MaskHint>();
        JobResources modernJobResources = new JobResources();
        environment.Extract(modernVariables, modernMaskhints, modernJobResources);
        if (VariableUtility.GetEnableAccessTokenType((IDictionary<string, VariableValue>) modernVariables) == EnableAccessTokenType.Variable)
          modernVariables[WellKnownDistributedTaskVariables.AccessToken] = (VariableValue) TaskHub.GetAccessToken(environment.SystemConnection.Authorization);
        if (!modernVariables.ContainsKey(WellKnownDistributedTaskVariables.RetainDefaultEncoding))
          modernVariables[WellKnownDistributedTaskVariables.RetainDefaultEncoding] = (VariableValue) "false";
        if (!modernVariables.ContainsKey(WellKnownDistributedTaskVariables.LogToBlobstorageService) && hub.CanLogToBlobStore(requestContext))
          modernVariables[WellKnownDistributedTaskVariables.LogToBlobstorageService] = (VariableValue) "true";
        if (!modernVariables.ContainsKey(WellKnownDistributedTaskVariables.UploadBuildArtifactsToBlob) && hub.IsBlobstoreUploadForBuildArtifactsSupported(requestContext))
          modernVariables[WellKnownDistributedTaskVariables.UploadBuildArtifactsToBlob] = (VariableValue) "true";
        if (!modernVariables.ContainsKey(WellKnownDistributedTaskVariables.UploadTimelineAttachmentsToBlob) && hub.IsBlobstoreUploadForTimelineAttachmentsSupported(requestContext))
          modernVariables[WellKnownDistributedTaskVariables.UploadTimelineAttachmentsToBlob] = (VariableValue) "true";
        if (requestContext.IsFeatureEnabled("DistributedTask.Agent.ReadOnlyVariables"))
          modernVariables[WellKnownDistributedTaskVariables.ReadOnlyVariables] = (VariableValue) "true";
        if (requestContext.IsFeatureEnabled("DistributedTask.Agent.UseWorkspaceId"))
          modernVariables[WellKnownDistributedTaskVariables.UseWorkspaceId] = (VariableValue) "true";
        if (requestContext.IsFeatureEnabled("DistributedTask.ReducePostLinesSpeed"))
        {
          Dictionary<string, VariableValue> dictionary = modernVariables;
          int linesPostFrequency = await hub.GetLogLinesPostFrequency(requestContext, scopeIdentifier, planId);
          dictionary[WellKnownDistributedTaskVariables.PostLinesSpeed] = (VariableValue) linesPostFrequency.ToString();
          dictionary = (Dictionary<string, VariableValue>) null;
        }
        modernVariables[WellKnownDistributedTaskVariables.TaskRestrictionEnforcementMode] = (VariableValue) "Enabled";
        TaskHubVariableHelper.AddTestResultLogParserVariables(requestContext, modernJobResources.Repositories, (IDictionary<string, VariableValue>) modernVariables);
        TaskHubVariableHelper.AddAutoPublishTestResultVariables(requestContext, modernJobResources.Repositories, (IDictionary<string, VariableValue>) modernVariables);
        ReadOnlyCollection<TaskInstance> readonlyTasks = new ReadOnlyCollection<TaskInstance>((IList<TaskInstance>) job.Tasks);
        modernJobResources.Endpoints.AddRange(await hub.GetReferencedServiceEndpointsAsync(requestContext, plan.ScopeIdentifier, environment, (IList<TaskInstance>) readonlyTasks));
        modernJobResources.SecureFiles.AddRange(await hub.GetReferencedSecureFilesAsync(requestContext, plan.ScopeIdentifier, (IList<TaskInstance>) readonlyTasks));
        await hub.Extension.PrepareJobAsync(requestContext, plan, (IDictionary<string, VariableValue>) modernVariables, modernMaskhints, modernJobResources);
        hub.AddEndpointsParametersToMaskHints(requestContext, modernJobResources.Endpoints, modernMaskhints);
        foreach (SecureFile secureFile in modernJobResources.SecureFiles)
          modernMaskhints.Add(new MaskHint()
          {
            Type = MaskType.Regex,
            Value = Regex.Escape(secureFile.Ticket)
          });
        environment = new JobEnvironment((IDictionary<string, VariableValue>) modernVariables, modernMaskhints.ToList<MaskHint>(), modernJobResources);
        hub.ResolveVariablesInTaskDisplayName(environment, job);
        jobRequestData = new TaskHub.JobRequestData()
        {
          AuthorizationId = jobAuthorization.AuthorizationId,
          Plan = plan,
          Environment = environment,
          ContainerJob = job
        };
      }
      return jobRequestData;
    }

    private async Task<TaskHub.JobRequestData> PrepareJobRequestAsync(
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      Guid planId,
      JobParameters parameters)
    {
      TaskHub.JobRequestData jobRequestData;
      using (new MethodScope(requestContext, nameof (TaskHub), nameof (PrepareJobRequestAsync)))
      {
        TaskOrchestrationPlan plan = await this.GetPlanAsync(requestContext, scopeIdentifier, planId);
        if (plan == null)
          throw new TaskOrchestrationPlanNotFoundException(TaskResources.PlanNotFound((object) planId));
        Job job = (Job) null;
        JobExecutionContext context = await this.GetJobExecutionContext(requestContext, scopeIdentifier, plan, parameters);
        job = context.Job.Definition;
        if (plan.Version < 2)
          this.Extension.SetAdditionalSystemVariables(requestContext, context.Variables);
        this.Extension.UpdateSystemVariables(requestContext, context.Variables);
        Microsoft.VisualStudio.Services.Identity.Identity systemIdentity = this.SelectBuildIdentity(requestContext, plan, context);
        IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
        using (IVssRequestContext impersonationContext = vssRequestContext.GetService<ITeamFoundationHostManagementService>().BeginUserRequest(vssRequestContext, requestContext.ServiceHost.InstanceId, systemIdentity.Descriptor))
        {
          this.EnsureLibraryPermission(impersonationContext, plan.ScopeIdentifier, systemIdentity);
          await this.EnsurePipelineCachePermissionAsync(impersonationContext, systemIdentity, job.Steps.OfType<TaskStep>().Select<TaskStep, Guid>((Func<TaskStep, Guid>) (s => s.Reference.Id)), context.Variables);
          bool flag = false;
          try
          {
            this.Extension.CheckWritePermission(impersonationContext, plan.ScopeIdentifier, plan.PlanId, plan.ArtifactUri);
            flag = true;
          }
          catch (TaskOrchestrationPlanSecurityException ex)
          {
            requestContext.TraceException(10015503, nameof (TaskHub), (Exception) ex);
          }
          if (!flag)
          {
            this.Extension.SetPermissions(requestContext, plan, systemIdentity);
            try
            {
              this.Extension.CheckWritePermission(impersonationContext, plan.ScopeIdentifier, plan.PlanId, plan.ArtifactUri);
              requestContext.TraceError(10015504, nameof (TaskHub), "Successfully corrected plan security error");
            }
            catch (TaskOrchestrationPlanSecurityException ex)
            {
              requestContext.TraceException(10015503, nameof (TaskHub), (Exception) ex);
              throw;
            }
          }
        }
        try
        {
          RepositoryResource repositoryResource = context.ResourceStore.Repositories.GetAll().FirstOrDefault<RepositoryResource>((Func<RepositoryResource, bool>) (r => r.Alias != PipelineConstants.SelfAlias && r.Properties.TryGetValue<string>("system.istriggeringrepository", out string _)));
          if (repositoryResource != null)
          {
            requestContext.TraceInfo(10015559, nameof (TaskHub), "Adding resource repository name " + repositoryResource.Name + " to the context");
            context.ReferencedResources.Repositories.Add(repositoryResource);
          }
        }
        catch (Exception ex)
        {
          requestContext.TraceException(nameof (TaskHub), ex);
        }
        JobResources jobResources = context.ResourceStore.GetJobResources(context.ReferencedResources);
        List<RepositoryResource> localRepositories = (List<RepositoryResource>) null;
        if (this.Extension.IsEnforceReferencedRepoScopedTokenEnabled(requestContext, scopeIdentifier))
          localRepositories = jobResources.Repositories.Where<RepositoryResource>((Func<RepositoryResource, bool>) (x => x.Type.Equals(Microsoft.TeamFoundation.DistributedTask.Pipelines.RepositoryTypes.Git, StringComparison.OrdinalIgnoreCase) && x.Endpoint == null)).ToList<RepositoryResource>();
        Dictionary<string, string> additionalClaims = new Dictionary<string, string>(context.ExecutionOptions.SystemTokenCustomClaims)
        {
          {
            "jobref",
            TaskHub.JobRefClaimValue(planId, job.Id)
          }
        };
        if (requestContext.IsFeatureEnabled("DistributedTask.SetGenerateIdTokenUrlInSystemEndpointData"))
        {
          ILocationService locationService = requestContext.GetService<ILocationService>();
          jobResources.Endpoints.Where<Microsoft.TeamFoundation.DistributedTask.WebApi.ServiceEndpoint>((Func<Microsoft.TeamFoundation.DistributedTask.WebApi.ServiceEndpoint, bool>) (endpoint => string.Equals(endpoint.Authorization.Scheme, "WorkloadIdentityFederation"))).ForEach<Microsoft.TeamFoundation.DistributedTask.WebApi.ServiceEndpoint>((Action<Microsoft.TeamFoundation.DistributedTask.WebApi.ServiceEndpoint>) (endpoint =>
          {
            Uri resourceUri = locationService.GetResourceUri(requestContext, "distributedtask", TaskResourceIds.OidcToken, (object) new
            {
              scopeIdentifier = scopeIdentifier,
              hubname = plan.PlanType,
              planId = plan.PlanId,
              jobId = job.Id,
              serviceConnectionId = endpoint.Id
            }, true, true);
            endpoint.Authorization.Parameters["generateOidcTokenUrl"] = resourceUri.AbsoluteUri;
          }));
        }
        TaskHub.JobAuthorizationResult jobAuthorization = this.GetJobAuthorization(requestContext, job.TimeoutInMinutes, job.CancelTimeoutInMinutes, systemIdentity, "SystemVssConnection", context.ExecutionOptions.SystemTokenScope, plan.ArtifactUri?.ToString(), (IList<RepositoryResource>) localRepositories, (IDictionary<string, string>) additionalClaims);
        if (jobAuthorization.Endpoint == null)
          throw new CreatePersonalAccessTokenFailedException(TaskResources.FailedToObtainJobAuthorization((object) job.Name, (object) plan.PlanType, (object) systemIdentity.DisplayName));
        jobResources.Endpoints.Add(jobAuthorization.Endpoint);
        if (context.EnvironmentVersion > 1)
        {
          ISourceProviderService service = requestContext.GetService<ISourceProviderService>();
          foreach (RepositoryResource repository1 in jobResources.Repositories)
          {
            ISourceProvider sourceProvider = service.GetSourceProvider(requestContext, repository1.Type);
            Microsoft.TeamFoundation.DistributedTask.WebApi.ServiceEndpoint serviceEndpoint = (Microsoft.TeamFoundation.DistributedTask.WebApi.ServiceEndpoint) null;
            if (repository1.Endpoint != null)
              serviceEndpoint = context.ResourceStore.GetEndpoint(repository1.Endpoint);
            IVssRequestContext requestContext1 = requestContext;
            JobExecutionContext context1 = context;
            RepositoryResource repository2 = repository1;
            Microsoft.TeamFoundation.DistributedTask.WebApi.ServiceEndpoint endpoint = serviceEndpoint;
            sourceProvider.PopulateJobData(requestContext1, context1, repository2, endpoint);
          }
        }
        switch (VariableUtility.GetEnableAccessTokenType(context.Variables))
        {
          case EnableAccessTokenType.Variable:
            context.Variables[WellKnownDistributedTaskVariables.AccessToken] = (VariableValue) TaskHub.GetAccessToken(jobAuthorization.Endpoint.Authorization);
            break;
          case EnableAccessTokenType.SecretVariable:
            context.Variables[WellKnownDistributedTaskVariables.AccessToken] = new VariableValue(TaskHub.GetAccessToken(jobAuthorization.Endpoint.Authorization), true);
            break;
        }
        if (requestContext.IsFeatureEnabled("DistributedTask.EnableDynamicTasksAndAgentFeatureFlags"))
        {
          IPipelineComponentsFeatureFlagService service = requestContext.GetService<IPipelineComponentsFeatureFlagService>();
          Stopwatch stopwatch = new Stopwatch();
          stopwatch.Start();
          foreach (FeatureAvailabilityInformation feature in service.GetFeatures(requestContext, PipelineComponentType.Agent))
            context.Variables[feature.Name] = (VariableValue) feature.IsEnabled.ToString();
          foreach (FeatureAvailabilityInformation feature in service.GetFeatures(requestContext, PipelineComponentType.Tasks))
            context.Variables[feature.Name] = (VariableValue) feature.IsEnabled.ToString();
          stopwatch.Stop();
          long elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
          if (elapsedMilliseconds > 250L)
            requestContext.TraceWarning(10016208, nameof (TaskHub), "Pipeline component flags were fetched by {0}ms which is longer than expected.", (object) elapsedMilliseconds);
        }
        if (!context.Variables.ContainsKey(WellKnownDistributedTaskVariables.RetainDefaultEncoding))
          context.Variables[WellKnownDistributedTaskVariables.RetainDefaultEncoding] = (VariableValue) "false";
        if (!context.Variables.ContainsKey(WellKnownDistributedTaskVariables.LogToBlobstorageService) && this.CanLogToBlobStore(requestContext))
          context.Variables[WellKnownDistributedTaskVariables.LogToBlobstorageService] = (VariableValue) "true";
        if (!context.Variables.ContainsKey(WellKnownDistributedTaskVariables.UploadBuildArtifactsToBlob) && this.IsBlobstoreUploadForBuildArtifactsSupported(requestContext))
          context.Variables[WellKnownDistributedTaskVariables.UploadBuildArtifactsToBlob] = (VariableValue) "true";
        if (!context.Variables.ContainsKey(WellKnownDistributedTaskVariables.UploadTimelineAttachmentsToBlob) && this.IsBlobstoreUploadForTimelineAttachmentsSupported(requestContext))
          context.Variables[WellKnownDistributedTaskVariables.UploadTimelineAttachmentsToBlob] = (VariableValue) "true";
        if (requestContext.IsFeatureEnabled("DistributedTask.Agent.ReadOnlyVariables"))
          context.Variables[WellKnownDistributedTaskVariables.ReadOnlyVariables] = (VariableValue) "true";
        if (requestContext.IsFeatureEnabled("DistributedTask.Agent.UseWorkspaceId"))
          context.Variables[WellKnownDistributedTaskVariables.UseWorkspaceId] = (VariableValue) "true";
        if (requestContext.IsFeatureEnabled("DistributedTask.ReducePostLinesSpeed"))
        {
          IDictionary<string, VariableValue> dictionary = context.Variables;
          int linesPostFrequency = await this.GetLogLinesPostFrequency(requestContext, scopeIdentifier, planId);
          dictionary[WellKnownDistributedTaskVariables.PostLinesSpeed] = (VariableValue) linesPostFrequency.ToString();
          dictionary = (IDictionary<string, VariableValue>) null;
        }
        if (!context.Variables.ContainsKey(WellKnownDistributedTaskVariables.ContinueAfterCancelProcessTreeKillAttempt) && requestContext.IsFeatureEnabled("DistributedTask.Agent.ContinueAfterCancelProcessTreeKillAttempt"))
          context.Variables[WellKnownDistributedTaskVariables.ContinueAfterCancelProcessTreeKillAttempt] = (VariableValue) "true";
        if (!context.Variables.ContainsKey(WellKnownDistributedTaskVariables.DockerActionRetries) && requestContext.IsFeatureEnabled("DistributedTask.Agent.DockerActionRetries"))
          context.Variables[WellKnownDistributedTaskVariables.DockerActionRetries] = (VariableValue) "true";
        if (!context.Variables.ContainsKey(WellKnownDistributedTaskVariables.UseMsalLibrary) && requestContext.IsFeatureEnabled("DistributedTask.Agent.UseMsalLibrary"))
          context.Variables[WellKnownDistributedTaskVariables.UseMsalLibrary] = (VariableValue) "true";
        if (!context.Variables.ContainsKey(WellKnownDistributedTaskVariables.EnableNodeWarnings) && requestContext.IsFeatureEnabled("DistributedTask.Agent.EnableNodeWarnings"))
          context.Variables[WellKnownDistributedTaskVariables.EnableNodeWarnings] = (VariableValue) "true";
        IVssRegistryService service1 = requestContext.GetService<IVssRegistryService>();
        if (requestContext.IsFeatureEnabled("DistributedTask.Node6LockdownAllowed") && service1.GetValue<bool>(requestContext, (RegistryQuery) "/Service/DistributedTask/Settings/DisableNode6Tasks", false))
          context.Variables[WellKnownDistributedTaskVariables.DisableNode6Tasks] = (VariableValue) "true";
        if (!context.Variables.ContainsKey(WellKnownDistributedTaskVariables.MajorUpgradeDisabled) && requestContext.IsFeatureEnabled("DistributedTask.Agent.MajorUpgradeDisabled"))
          context.Variables[WellKnownDistributedTaskVariables.MajorUpgradeDisabled] = (VariableValue) "true";
        if (!context.Variables.ContainsKey(WellKnownDistributedTaskVariables.AgentFailOnIncompatibleOS) && requestContext.IsFeatureEnabled("DistributedTask.Agent.AgentFailOnIncompatibleOS"))
          context.Variables[WellKnownDistributedTaskVariables.AgentFailOnIncompatibleOS] = (VariableValue) "true";
        if (!context.Variables.ContainsKey(WellKnownDistributedTaskVariables.EnableFCSItemPathFix) && requestContext.IsFeatureEnabled("DistributedTask.Agent.EnableFCSItemPathFix"))
          context.Variables[WellKnownDistributedTaskVariables.EnableFCSItemPathFix] = (VariableValue) "true";
        if (!context.Variables.ContainsKey(WellKnownDistributedTaskVariables.PreferRedirectGetContainerItem) && requestContext.IsFeatureEnabled("DistributedTask.PreferRedirectGetContainerItem"))
          context.Variables[WellKnownDistributedTaskVariables.PreferRedirectGetContainerItem] = (VariableValue) "true";
        if (!context.Variables.ContainsKey(WellKnownDistributedTaskVariables.AgentEnablePipelineArtifactLargeChunkSize) && requestContext.IsFeatureEnabled("DistributedTask.Agent.AgentEnablePipelineArtifactLargeChunkSize"))
          context.Variables[WellKnownDistributedTaskVariables.AgentEnablePipelineArtifactLargeChunkSize] = (VariableValue) "true";
        if (!context.Variables.ContainsKey(WellKnownDistributedTaskVariables.DisableDrainQueuesAfterTask) && requestContext.IsFeatureEnabled("DistributedTask.Agent.DisableDrainQueuesAfterTask"))
          context.Variables[WellKnownDistributedTaskVariables.DisableDrainQueuesAfterTask] = (VariableValue) "true";
        if (!context.Variables.ContainsKey(WellKnownDistributedTaskVariables.EnableBuildArtifactsPlusSignWorkaround) && requestContext.IsFeatureEnabled("DistributedTask.PipelineTasks.EnableBuildArtifactsPlusSignWorkaround"))
          context.Variables[WellKnownDistributedTaskVariables.EnableBuildArtifactsPlusSignWorkaround] = (VariableValue) "true";
        if (!context.Variables.ContainsKey(WellKnownDistributedTaskVariables.AgentIgnoreVSTSTaskLib) && requestContext.IsFeatureEnabled("DistributedTask.Agent.IgnoreVSTSTaskLib"))
          context.Variables[WellKnownDistributedTaskVariables.AgentIgnoreVSTSTaskLib] = (VariableValue) "true";
        if (!context.Variables.ContainsKey(WellKnownDistributedTaskVariables.FailDeprecatedTask) && requestContext.IsFeatureEnabled("DistributedTask.Agent.FailDeprecatedTask"))
          context.Variables[WellKnownDistributedTaskVariables.FailDeprecatedTask] = (VariableValue) "true";
        if (requestContext.IsFeatureEnabled("DistributedTask.AllowEnableShellTasksArgsSanitizingToggle"))
        {
          bool flag1;
          bool flag2;
          try
          {
            PipelineGeneralSettings result = requestContext.GetClient<BuildHttpClient>().GetBuildGeneralSettingsAsync(plan.ScopeIdentifier.ToString()).Result;
            bool? nullable = result.EnableShellTasksArgsSanitizing;
            bool flag3 = true;
            flag1 = nullable.GetValueOrDefault() == flag3 & nullable.HasValue;
            nullable = result.EnableShellTasksArgsSanitizingAudit;
            bool flag4 = true;
            flag2 = nullable.GetValueOrDefault() == flag4 & nullable.HasValue;
          }
          catch (Exception ex)
          {
            requestContext.TraceException(10016203, nameof (TaskHub), ex);
            flag1 = false;
            flag2 = false;
          }
          int num = context.Variables.ContainsKey(WellKnownDistributedTaskVariables.MSRC75787EnableSecureArgs) ? 0 : (flag1 ? 1 : (service1.GetValue<bool>(requestContext, (RegistryQuery) RegistryKeys.EnableShellTasksArgsSanitizing, false) ? 1 : 0));
          if (num != 0)
            context.Variables[WellKnownDistributedTaskVariables.MSRC75787EnableSecureArgs] = (VariableValue) "true";
          if ((num != 0 ? 0 : (context.Variables.ContainsKey(WellKnownDistributedTaskVariables.MSRC75787EnableSecureArgsAudit) ? 0 : (flag2 ? 1 : (service1.GetValue<bool>(requestContext, (RegistryQuery) RegistryKeys.EnableShellTasksArgsSanitizingAudit, false) ? 1 : 0)))) != 0)
            context.Variables[WellKnownDistributedTaskVariables.MSRC75787EnableSecureArgsAudit] = (VariableValue) "true";
        }
        if (!context.Variables.ContainsKey(WellKnownDistributedTaskVariables.MSRC75787EnableTelemetry) && requestContext.IsFeatureEnabled("DistributedTask.MSRC75787EnableTelemetry"))
          context.Variables[WellKnownDistributedTaskVariables.MSRC75787EnableTelemetry] = (VariableValue) "true";
        if (!context.Variables.ContainsKey(WellKnownDistributedTaskVariables.MSRC75787EnableNewAgentNewProcessHandlerSanitizer) && requestContext.IsFeatureEnabled("DistributedTask.MSRC75787EnableNewAgentNewProcessHandlerSanitizer"))
          context.Variables[WellKnownDistributedTaskVariables.MSRC75787EnableNewAgentNewProcessHandlerSanitizer] = (VariableValue) "true";
        if (requestContext.IsFeatureEnabled("DistributedTask.CheckForTaskDeprecation"))
          context.Variables[WellKnownDistributedTaskVariables.CheckForTaskDeprecation] = (VariableValue) "true";
        else
          context.Variables[WellKnownDistributedTaskVariables.CheckForTaskDeprecation] = (VariableValue) "false";
        if (requestContext.IsFeatureEnabled("DistributedTask.Agent.MountWorkspace"))
          context.Variables[WellKnownDistributedTaskVariables.MountWorkspace] = (VariableValue) "true";
        else
          context.Variables[WellKnownDistributedTaskVariables.MountWorkspace] = (VariableValue) "false";
        if (!context.Variables.ContainsKey(WellKnownDistributedTaskVariables.FailJobWhenAgentDies) && requestContext.IsFeatureEnabled("DistributedTask.Agent.FailJobWhenAgentDies"))
          context.Variables[WellKnownDistributedTaskVariables.FailJobWhenAgentDies] = (VariableValue) "true";
        if (!context.Variables.ContainsKey(WellKnownDistributedTaskVariables.EnableNewPowerShellInvokeProcessCmdlet) && requestContext.IsFeatureEnabled("DistributedTask.EnableNewPowerShellInvokeProcessCmdlet"))
          context.Variables[WellKnownDistributedTaskVariables.EnableNewPowerShellInvokeProcessCmdlet] = (VariableValue) "true";
        if (!context.Variables.ContainsKey(WellKnownDistributedTaskVariables.UseMsdeployTokenAuth) && requestContext.IsFeatureEnabled("DistributedTask.Agent.UseMsdeployTokenAuth"))
          context.Variables[WellKnownDistributedTaskVariables.UseMsdeployTokenAuth] = (VariableValue) "true";
        if (!context.Variables.ContainsKey(WellKnownDistributedTaskVariables.FixPossibleGitOutOfMemoryProblem) && requestContext.IsFeatureEnabled("DistributedTask.Agent.FixPossibleGitOutOfMemoryProblem"))
          context.Variables[WellKnownDistributedTaskVariables.FixPossibleGitOutOfMemoryProblem] = (VariableValue) "true";
        if (!context.Variables.ContainsKey(WellKnownDistributedTaskVariables.UseMaskingPerformanceEnhancements) && requestContext.IsFeatureEnabled("DistributedTask.Agent.UseMaskingPerformanceEnhancements"))
          context.Variables[WellKnownDistributedTaskVariables.UseMaskingPerformanceEnhancements] = (VariableValue) "true";
        if (!context.Variables.ContainsKey(WellKnownDistributedTaskVariables.AgentDockerInitOption) && requestContext.IsFeatureEnabled("DistributedTask.Agent.DockerInitOption"))
          context.Variables[WellKnownDistributedTaskVariables.AgentDockerInitOption] = (VariableValue) "true";
        if (!context.Variables.ContainsKey(WellKnownDistributedTaskVariables.HighTaskFailRateDetected) && requestContext.IsFeatureEnabled("DistributedTask.HighTaskFailRateDetected"))
          context.Variables[WellKnownDistributedTaskVariables.HighTaskFailRateDetected] = (VariableValue) "true";
        if (!context.Variables.ContainsKey(WellKnownDistributedTaskVariables.RetireAzureRMPowerShellModule) && requestContext.IsFeatureEnabled("DistributedTask.Tasks.RetireAzureRMPowerShellModule"))
          context.Variables[WellKnownDistributedTaskVariables.RetireAzureRMPowerShellModule] = (VariableValue) "true";
        if (!context.Variables.ContainsKey(WellKnownDistributedTaskVariables.FailDeprecatedBuildTask) && requestContext.IsFeatureEnabled("DistributedTask.Agent.FailDeprecatedBuildTask"))
          context.Variables[WellKnownDistributedTaskVariables.FailDeprecatedBuildTask] = (VariableValue) "true";
        if (!context.Variables.ContainsKey(WellKnownDistributedTaskVariables.LogTaskNameInUserAgent) && requestContext.IsFeatureEnabled("DistributedTask.Agent.LogTaskNameInUserAgent"))
          context.Variables[WellKnownDistributedTaskVariables.LogTaskNameInUserAgent] = (VariableValue) "true";
        context.Variables[WellKnownDistributedTaskVariables.TaskRestrictionEnforcementMode] = (VariableValue) "Enabled";
        TaskHubVariableHelper.AddTestResultLogParserVariables(requestContext, jobResources.Repositories, context.Variables);
        TaskHubVariableHelper.AddAutoPublishTestResultVariables(requestContext, jobResources.Repositories, context.Variables);
        if (jobResources.SecureFiles.Count > 0)
        {
          IList<SecureFile> secureFilesAsync = await requestContext.GetService<ISecureFileService>().GetSecureFilesAsync(requestContext, scopeIdentifier, jobResources.SecureFiles.Select<SecureFile, Guid>((Func<SecureFile, Guid>) (x => x.Id)).Distinct<Guid>(), true);
          jobResources.SecureFiles.Clear();
          jobResources.SecureFiles.AddRange((IEnumerable<SecureFile>) secureFilesAsync);
        }
        requestContext.TraceDownloadSecureFileTasks(scopeIdentifier, job, jobResources);
        HashSet<MaskHint> maskHints = new HashSet<MaskHint>();
        if (context.EnvironmentVersion < 2)
        {
          await this.Extension.PrepareJobAsync(requestContext, plan, context.Variables, maskHints, jobResources);
        }
        else
        {
          await this.Extension.PreparePipelineJobAsync(requestContext, plan, context.Variables, maskHints, jobResources);
          if (context.ExecutionOptions.RestrictSecrets)
          {
            context.Variables[WellKnownDistributedTaskVariables.RestrictSecrets] = (VariableValue) bool.TrueString;
            jobResources.SecureFiles.Clear();
            foreach (KeyValuePair<string, VariableValue> keyValuePair in context.Variables.Where<KeyValuePair<string, VariableValue>>((Func<KeyValuePair<string, VariableValue>, bool>) (k => k.Value.IsSecret)).ToList<KeyValuePair<string, VariableValue>>())
              ((ICollection<KeyValuePair<string, VariableValue>>) context.Variables).Remove(keyValuePair);
            jobResources.Endpoints.Clear();
            foreach (Resource repository in jobResources.Repositories)
              repository.Endpoint = (Microsoft.TeamFoundation.DistributedTask.Pipelines.ServiceEndpointReference) null;
            foreach (Resource container in jobResources.Containers)
              container.Endpoint = (Microsoft.TeamFoundation.DistributedTask.Pipelines.ServiceEndpointReference) null;
            jobResources.Endpoints.Add(jobAuthorization.Endpoint);
          }
        }
        this.AddEndpointsParametersToMaskHints(requestContext, jobResources.Endpoints, maskHints);
        jobRequestData = new TaskHub.JobRequestData()
        {
          AuthorizationId = jobAuthorization.AuthorizationId,
          Context = context,
          Job = job,
          Plan = plan,
          MaskHints = maskHints.ToList<MaskHint>(),
          Resources = jobResources
        };
      }
      return jobRequestData;
    }

    private Microsoft.VisualStudio.Services.Identity.Identity SelectBuildIdentity(
      IVssRequestContext requestContext,
      TaskOrchestrationPlan plan,
      JobExecutionContext context)
    {
      return context.Job.Definition.ExecuteAs != null ? requestContext.GetService<IdentityService>().GetIdentity(requestContext, context.Job.Definition.ExecuteAs.Id) : this.Extension.GetJobServiceIdentity(requestContext, plan, context.Variables);
    }

    private static string JobRefClaimValue(Guid planId, Guid jobId) => string.Format("{0}:{1}", (object) planId, (object) jobId);

    private bool CanLogToBlobStore(IVssRequestContext requestContext) => requestContext.IsFeatureEnabled("DistributedTask.Agent.LogToBlobstorageService") && this.IsDevFabBlobInstalled(requestContext);

    private bool IsDevFabBlobInstalled(IVssRequestContext requestContext)
    {
      if (requestContext.ExecutionEnvironment.IsDevFabricDeployment)
      {
        try
        {
          requestContext.GetClient<BlobStore2HttpClient>();
        }
        catch
        {
          return false;
        }
      }
      return true;
    }

    private bool IsBlobstoreUploadForBuildArtifactsSupported(IVssRequestContext requestContext) => requestContext.ExecutionEnvironment.IsHostedDeployment && requestContext.IsFeatureEnabled("DistributedTask.Agent.UploadBuildArtifactsToBlob") && this.IsDevFabBlobInstalled(requestContext);

    private bool IsBlobstoreUploadForTimelineAttachmentsSupported(IVssRequestContext requestContext) => requestContext.ExecutionEnvironment.IsHostedDeployment && requestContext.IsFeatureEnabled("DistributedTask.Agent.UploadTimelineAttachmentsToBlob") && this.IsDevFabBlobInstalled(requestContext);

    private async Task<JobExecutionContext> GetJobExecutionContext(
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      TaskOrchestrationPlan plan,
      JobParameters parameters)
    {
      JobExecutionContext executionContext1 = (JobExecutionContext) null;
      TaskHub.PipelineTraceWriter trace = new TaskHub.PipelineTraceWriter();
      PipelineProcess process = plan.GetProcess<PipelineProcess>();
      string stageName = parameters.StageName ?? PipelineConstants.DefaultJobName;
      Stage stage1 = process.Stages.SingleOrDefault<Stage>((Func<Stage, bool>) (x => x.Name.Equals(stageName, StringComparison.OrdinalIgnoreCase)));
      PhaseNode phaseNode = stage1 != null ? stage1.Phases.SingleOrDefault<PhaseNode>((Func<PhaseNode, bool>) (x => x.Name.Equals(parameters.PhaseName, StringComparison.OrdinalIgnoreCase))) : (PhaseNode) null;
      if (phaseNode != null)
      {
        StageInstance stage2 = new StageInstance(stage1, parameters.StageAttempt);
        PhaseInstance phase1 = new PhaseInstance(phaseNode, parameters.PhaseAttempt);
        PhaseExecutionContext phaseContext = await this.CreatePhaseExecutionContextAsync(requestContext, plan, stage2, phase1, (IPipelineTraceWriter) trace, includeOutputs: true, includeSecrets: true);
        bool flag = requestContext.IsFeatureEnabled("DistributedTask.EnableInputsInjectionForTaskDecorators");
        switch (phaseNode)
        {
          case Microsoft.TeamFoundation.DistributedTask.Pipelines.Phase phase2:
            phase2.inputsInjectionFeatureFlagEnabled = flag;
            executionContext1 = phase2.CreateJobContext(phaseContext, parameters.Name, parameters.Attempt);
            break;
          case ProviderPhase providerPhase:
            string jobInstanceName = phaseContext.IdGenerator.GetJobInstanceName(parameters.StageName, parameters.PhaseName, parameters.Name, parameters.Attempt, parameters.CheckRerunAttempt);
            executionContext1 = providerPhase.CreateJobContext(phaseContext, await this.GetJobInstanceAsync(requestContext, plan.ScopeIdentifier, plan.PlanId, jobInstanceName, true) ?? throw new TaskOrchestrationJobNotFoundException(TaskResources.JobNotFound((object) jobInstanceName, (object) plan.PlanId)));
            jobInstanceName = (string) null;
            providerPhase = (ProviderPhase) null;
            break;
          default:
            throw new NotSupportedException(phaseNode.GetType().FullName);
        }
        phaseContext = (PhaseExecutionContext) null;
      }
      JobExecutionContext executionContext2 = executionContext1?.Job != null ? executionContext1 : throw new TaskOrchestrationJobNotFoundException(TaskResources.JobNotFound((object) (parameters.PhaseName + "." + parameters.Name), (object) plan.PlanId));
      phaseNode = (PhaseNode) null;
      return executionContext2;
    }

    public async Task<PhaseExecutionContext> CreatePhaseExecutionContextAsync(
      IVssRequestContext requestContext,
      Guid scopeId,
      Guid planId,
      PhaseConditionContext conditionContext,
      bool includeOutputs = false)
    {
      TaskOrchestrationPlan planAsync = await this.GetPlanAsync(requestContext, scopeId, planId);
      if (planAsync == null)
        throw new TaskOrchestrationPlanNotFoundException(TaskResources.PlanNotFound((object) planId));
      TaskHub.PipelineTraceWriter trace = new TaskHub.PipelineTraceWriter();
      PipelineIdGenerator idGenerator = new PipelineIdGenerator(planAsync.Version < 4);
      PipelineProcess process = planAsync.GetProcess<PipelineProcess>();
      string stageName = conditionContext.StageName ?? PipelineConstants.DefaultJobName;
      Stage stage1 = process.Stages.SingleOrDefault<Stage>((Func<Stage, bool>) (x => x.Name.Equals(stageName, StringComparison.OrdinalIgnoreCase)));
      if (stage1 != null)
      {
        PhaseNode phase1 = string.IsNullOrEmpty(conditionContext.PhaseName) ? stage1.Phases.SingleOrDefault<PhaseNode>((Func<PhaseNode, bool>) (x => idGenerator.GetPhaseInstanceId(stageName, conditionContext.PhaseName, conditionContext.PhaseAttempt) == conditionContext.PhaseId)) : stage1.Phases.SingleOrDefault<PhaseNode>((Func<PhaseNode, bool>) (x => x.Name.Equals(conditionContext.PhaseName, StringComparison.OrdinalIgnoreCase)));
        if (phase1 != null)
        {
          StageInstance stage2 = new StageInstance(stage1, conditionContext.StageAttempt);
          PhaseInstance phase2 = new PhaseInstance(phase1, conditionContext.PhaseAttempt);
          Dictionary<string, PhaseInstance> dictionary = conditionContext.Dependencies.ToDictionary<KeyValuePair<string, PhaseExecutionState>, string, PhaseInstance>((Func<KeyValuePair<string, PhaseExecutionState>, string>) (x => x.Key), (Func<KeyValuePair<string, PhaseExecutionState>, PhaseInstance>) (x => x.Value.ToInstance()), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
          return await this.CreatePhaseExecutionContextAsync(requestContext, planAsync, stage2, phase2, (IPipelineTraceWriter) trace, conditionContext.State, (IDictionary<string, PhaseInstance>) dictionary, includeOutputs, evaluateCounters: false);
        }
      }
      return (PhaseExecutionContext) null;
    }

    internal async Task<StageExecutionContext> CreateStageExecutionContextAsync(
      IVssRequestContext requestContext,
      StageConditionContext conditionContext,
      bool includeOutputs = false)
    {
      TaskOrchestrationPlan planAsync = await this.GetPlanAsync(requestContext, conditionContext.ScopeId, conditionContext.PlanId);
      if (planAsync == null)
        throw new TaskOrchestrationPlanNotFoundException(TaskResources.PlanNotFound((object) conditionContext.PlanId));
      TaskHub.PipelineTraceWriter trace = new TaskHub.PipelineTraceWriter();
      PipelineProcess process = planAsync.GetProcess<PipelineProcess>();
      string stageName = conditionContext.StageName ?? PipelineConstants.DefaultJobName;
      Stage stage1 = process.Stages.SingleOrDefault<Stage>((Func<Stage, bool>) (x => x.Name.Equals(stageName, StringComparison.OrdinalIgnoreCase)));
      if (stage1 == null)
        return (StageExecutionContext) null;
      StageInstance stage2 = new StageInstance(stage1, conditionContext.StageAttempt);
      Dictionary<string, StageInstance> dictionary = conditionContext.Dependencies.ToDictionary<KeyValuePair<string, StageExecutionState>, string, StageInstance>((Func<KeyValuePair<string, StageExecutionState>, string>) (x => x.Key), (Func<KeyValuePair<string, StageExecutionState>, StageInstance>) (x => x.Value.ToInstance()), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      return await this.CreateStageExecutionContextAsync(requestContext, planAsync, stage2, (IPipelineTraceWriter) trace, conditionContext.State, (IDictionary<string, StageInstance>) dictionary, includeOutputs, evaluateCounters: false);
    }

    internal async Task<StageExecutionContext> CreateStageExecutionContextAsync(
      IVssRequestContext requestContext,
      TaskOrchestrationPlan plan,
      StageInstance stage,
      IPipelineTraceWriter trace = null,
      PipelineState state = PipelineState.InProgress,
      IDictionary<string, StageInstance> dependencies = null,
      bool includeOutputs = false,
      bool includeSecrets = false,
      bool evaluateCounters = true)
    {
      StageExecutionContext executionContext;
      using (new MethodScope(requestContext, nameof (TaskHub), nameof (CreateStageExecutionContextAsync)))
      {
        if (stage.Definition.DependsOn.Count > 0)
        {
          Microsoft.TeamFoundation.DistributedTask.WebApi.Timeline timeline = (Microsoft.TeamFoundation.DistributedTask.WebApi.Timeline) null;
          if (dependencies == null)
          {
            dependencies = (IDictionary<string, StageInstance>) new Dictionary<string, StageInstance>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
            timeline = await this.GetTimelineAsync(requestContext, plan.ScopeIdentifier, plan.PlanId, Guid.Empty, includeRecords: true, includeSecretVariables: true);
            List<Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord> timelineRecordList;
            if (timeline == null)
            {
              timelineRecordList = (List<Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord>) null;
            }
            else
            {
              List<Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord> records = timeline.Records;
              if (records == null)
              {
                timelineRecordList = (List<Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord>) null;
              }
              else
              {
                IEnumerable<Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord> source = records.Where<Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord>((Func<Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord, bool>) (x => x.RecordType == "Stage"));
                timelineRecordList = source != null ? source.ToList<Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord>() : (List<Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord>) null;
              }
            }
            List<Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord> source1 = timelineRecordList;
            foreach (string str in (IEnumerable<string>) stage.Definition.DependsOn)
            {
              string nodeName = str;
              Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord timelineRecord = source1 != null ? source1.FirstOrDefault<Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord>((Func<Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord, bool>) (x => string.Equals(nodeName, x.RefName, StringComparison.OrdinalIgnoreCase))) : (Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord) null;
              dependencies[nodeName] = new StageInstance(nodeName, ((Microsoft.TeamFoundation.DistributedTask.WebApi.TaskResult?) timelineRecord?.Result).GetValueOrDefault());
            }
          }
          if (includeOutputs)
          {
            OutputVariableScope outputVariablesAsync = await this.GetPlanOutputVariablesAsync(requestContext, plan.ScopeIdentifier, plan.PlanId, timeline);
            if (outputVariablesAsync != null)
            {
              foreach (OutputVariableScope outputVariableScope in outputVariablesAsync.FindAll((Predicate<OutputVariableScope>) (x => x.ScopeType == "Stage" && dependencies.ContainsKey(x.Name))))
              {
                StageInstance dependency = dependencies[outputVariableScope.Name];
                dependency.Result = new Microsoft.TeamFoundation.DistributedTask.WebApi.TaskResult?(outputVariableScope.Result.GetValueOrDefault());
                foreach (KeyValuePair<string, VariableValue> keyValuePair in outputVariableScope.ChildScopes.SelectMany<OutputVariableScope, KeyValuePair<string, VariableValue>>((Func<OutputVariableScope, IEnumerable<KeyValuePair<string, VariableValue>>>) (x => (IEnumerable<KeyValuePair<string, VariableValue>>) x.Flatten())))
                  dependency.Outputs[keyValuePair.Key] = keyValuePair.Value;
              }
            }
          }
        }
        PipelineEnvironment environment = plan.GetEnvironment<PipelineEnvironment>();
        executionContext = requestContext.GetService<IPipelineBuilderService>().GetBuilder(requestContext, plan).CreateStageExecutionContext(stage, dependencies, state, includeSecrets, trace, environment.Options);
      }
      return executionContext;
    }

    internal async Task<PhaseExecutionContext> CreatePhaseExecutionContextAsync(
      IVssRequestContext requestContext,
      TaskOrchestrationPlan plan,
      StageInstance stage,
      PhaseInstance phase,
      IPipelineTraceWriter trace = null,
      PipelineState state = PipelineState.InProgress,
      IDictionary<string, PhaseInstance> dependencies = null,
      bool includeOutputs = false,
      bool includeSecrets = false,
      bool evaluateCounters = true)
    {
      PhaseExecutionContext executionContextAsync;
      using (new MethodScope(requestContext, nameof (TaskHub), nameof (CreatePhaseExecutionContextAsync)))
      {
        Dictionary<string, IDictionary<string, PhaseInstance>> stageDependencies = new Dictionary<string, IDictionary<string, PhaseInstance>>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        if (phase.Definition.DependsOn.Count > 0 || stage.Definition.DependsOn.Count > 0)
        {
          Microsoft.TeamFoundation.DistributedTask.WebApi.Timeline timeline = (Microsoft.TeamFoundation.DistributedTask.WebApi.Timeline) null;
          if (dependencies == null | includeOutputs)
          {
            timeline = await this.GetTimelineAsync(requestContext, plan.ScopeIdentifier, plan.PlanId, Guid.Empty, includeRecords: true, includeSecretVariables: true);
            List<Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord> records = timeline?.Records;
            if (dependencies == null)
            {
              dependencies = (IDictionary<string, PhaseInstance>) new Dictionary<string, PhaseInstance>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
              Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord stageRecord = records != null ? records.FirstOrDefault<Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord>((Func<Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord, bool>) (x => string.Equals(stage.Name, x.RefName, StringComparison.OrdinalIgnoreCase) && x.RecordType == "Stage")) : (Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord) null;
              foreach (string str in (IEnumerable<string>) phase.Definition.DependsOn)
              {
                string nodeName = str;
                Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord timelineRecord = records != null ? records.FirstOrDefault<Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord>((Func<Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord, bool>) (x =>
                {
                  if (!string.Equals(nodeName, x.RefName, StringComparison.OrdinalIgnoreCase))
                    return false;
                  if (stageRecord == null)
                    return true;
                  Guid? parentId = x.ParentId;
                  Guid id = stageRecord.Id;
                  if (!parentId.HasValue)
                    return false;
                  return !parentId.HasValue || parentId.GetValueOrDefault() == id;
                })) : (Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord) null;
                dependencies[nodeName] = new PhaseInstance(nodeName, ((Microsoft.TeamFoundation.DistributedTask.WebApi.TaskResult?) timelineRecord?.Result).GetValueOrDefault());
              }
            }
            foreach (string str in (IEnumerable<string>) stage.Definition.DependsOn)
            {
              string stageName = str;
              Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord previousStageRecord = records != null ? records.FirstOrDefault<Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord>((Func<Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord, bool>) (x => string.Equals(stageName, x.RefName, StringComparison.OrdinalIgnoreCase) && x.RecordType == "Stage")) : (Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord) null;
              if (previousStageRecord != null)
              {
                IEnumerable<Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord> timelineRecords = records != null ? records.Where<Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord>((Func<Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord, bool>) (x =>
                {
                  Guid? parentId = x.ParentId;
                  Guid id = previousStageRecord.Id;
                  return (parentId.HasValue ? (parentId.HasValue ? (parentId.GetValueOrDefault() == id ? 1 : 0) : 1) : 0) != 0 && x.RecordType == "Phase";
                })) : (IEnumerable<Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord>) null;
                if (timelineRecords != null)
                {
                  foreach (Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord timelineRecord in timelineRecords)
                  {
                    IDictionary<string, PhaseInstance> dictionary;
                    if (!stageDependencies.TryGetValue(stageName, out dictionary))
                    {
                      dictionary = (IDictionary<string, PhaseInstance>) new Dictionary<string, PhaseInstance>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
                      stageDependencies[stageName] = dictionary;
                    }
                    dictionary[timelineRecord.RefName] = new PhaseInstance(timelineRecord.RefName, ((Microsoft.TeamFoundation.DistributedTask.WebApi.TaskResult?) timelineRecord?.Result).GetValueOrDefault());
                  }
                }
              }
            }
          }
          if (includeOutputs)
          {
            OutputVariableScope outputVariablesAsync = await this.GetPlanOutputVariablesAsync(requestContext, plan.ScopeIdentifier, plan.PlanId, timeline);
            OutputVariableScope outputVariableScope1 = outputVariablesAsync != null ? outputVariablesAsync.FindAll((Predicate<OutputVariableScope>) (x => x.ScopeType == "Stage" && string.Equals(stage.Name, x.Name, StringComparison.OrdinalIgnoreCase))).FirstOrDefault<OutputVariableScope>() : (OutputVariableScope) null;
            if (outputVariablesAsync != null && (outputVariableScope1 != null || stage.Name == PipelineConstants.DefaultJobName))
            {
              foreach (OutputVariableScope outputVariableScope2 in (outputVariableScope1 ?? outputVariablesAsync).FindAll((Predicate<OutputVariableScope>) (x => x.ScopeType == "Phase" && dependencies.ContainsKey(x.Name))))
              {
                PhaseInstance dependency = dependencies[outputVariableScope2.Name];
                dependency.Result = new Microsoft.TeamFoundation.DistributedTask.WebApi.TaskResult?(outputVariableScope2.Result.GetValueOrDefault());
                foreach (KeyValuePair<string, VariableValue> keyValuePair in outputVariableScope2.ChildScopes.SelectMany<OutputVariableScope, KeyValuePair<string, VariableValue>>((Func<OutputVariableScope, IEnumerable<KeyValuePair<string, VariableValue>>>) (x => (IEnumerable<KeyValuePair<string, VariableValue>>) x.Flatten())))
                  dependency.Outputs[keyValuePair.Key] = keyValuePair.Value;
              }
            }
            IEnumerable<OutputVariableScope> all = outputVariablesAsync?.FindAll((Predicate<OutputVariableScope>) (x => x.ScopeType == "Stage" && stage.Definition.DependsOn.Contains(x.Name)));
            if (all != null)
            {
              foreach (OutputVariableScope outputVariableScope3 in all)
              {
                OutputVariableScope otherStageScope = outputVariableScope3;
                foreach (OutputVariableScope outputVariableScope4 in otherStageScope.FindAll((Predicate<OutputVariableScope>) (x => x.ScopeType == "Phase" && stageDependencies.ContainsKey(otherStageScope.Name) && stageDependencies[otherStageScope.Name].ContainsKey(x.Name))))
                {
                  PhaseInstance phaseInstance = stageDependencies[otherStageScope.Name][outputVariableScope4.Name];
                  phaseInstance.Result = new Microsoft.TeamFoundation.DistributedTask.WebApi.TaskResult?(outputVariableScope4.Result.GetValueOrDefault());
                  foreach (KeyValuePair<string, VariableValue> keyValuePair in outputVariableScope4.ChildScopes.SelectMany<OutputVariableScope, KeyValuePair<string, VariableValue>>((Func<OutputVariableScope, IEnumerable<KeyValuePair<string, VariableValue>>>) (x => (IEnumerable<KeyValuePair<string, VariableValue>>) x.Flatten())))
                    phaseInstance.Outputs[keyValuePair.Key] = keyValuePair.Value;
                }
              }
            }
          }
        }
        PipelineEnvironment environment = plan.GetEnvironment<PipelineEnvironment>();
        PipelineBuilder builder = requestContext.GetService<IPipelineBuilderService>().GetBuilder(requestContext, plan);
        IDictionary<string, bool> featureFlags = PipelineFeatureFlagDictionaryFactory.Create(requestContext);
        PhaseExecutionContext phaseContext = builder.CreatePhaseExecutionContext(stage, phase, dependencies, (IDictionary<string, IDictionary<string, PhaseInstance>>) stageDependencies, state, includeSecrets, trace, environment.Options, featureFlags);
        StageInstance stageInstance = stage;
        if ((stageInstance != null ? (stageInstance.Attempt > 1 ? 1 : 0) : 0) != 0)
        {
          IPipelineRuntimeService service = requestContext.GetService<IPipelineRuntimeService>();
          string phaseIdentifier = phaseContext.IdGenerator.GetPhaseIdentifier(stage.Name, phase.Name);
          IVssRequestContext requestContext1 = requestContext;
          string name1 = this.Name;
          Guid scopeIdentifier = plan.ScopeIdentifier;
          Guid planId = plan.PlanId;
          string name2 = stage.Name;
          string[] includedPhases = new string[1]
          {
            phaseIdentifier
          };
          phaseContext.PreviousAttempts.AddRange<PhaseAttempt, IList<PhaseAttempt>>((await service.GetAllStageAttemptsAsync(requestContext1, name1, scopeIdentifier, planId, name2, (IList<string>) includedPhases)).Where<StageAttempt>((Func<StageAttempt, bool>) (x => x.Stage.Attempt < stage.Attempt)).SelectMany<StageAttempt, PhaseAttempt>((Func<StageAttempt, IEnumerable<PhaseAttempt>>) (x => (IEnumerable<PhaseAttempt>) x.Phases)).Where<PhaseAttempt>((Func<PhaseAttempt, bool>) (x => string.Equals(x.Phase.Name, phase.Name, StringComparison.OrdinalIgnoreCase))));
        }
        executionContextAsync = phaseContext;
      }
      return executionContextAsync;
    }

    public async Task<JobExecutionContext> CreateJobExecutionContextAsync(
      IVssRequestContext requestContext,
      Guid scopeId,
      Guid planId,
      TaskConditionContext conditionContext,
      bool simple = false)
    {
      if (simple)
      {
        JobExecutionContext executionContextAsync = new JobExecutionContext(conditionContext.State, (IPipelineIdGenerator) new PipelineIdGenerator(this.EnsurePlanData(requestContext, scopeId, planId).Version < 4));
        executionContextAsync.Variables[WellKnownDistributedTaskVariables.JobStatus] = (VariableValue) conditionContext.Result.ToString();
        return executionContextAsync;
      }
      TaskOrchestrationPlan plan = await this.GetPlanAsync(requestContext, scopeId, planId);
      PipelineProcess pipelineProcess = plan != null ? plan.GetProcess<PipelineProcess>() : throw new TaskOrchestrationPlanNotFoundException(TaskResources.PlanNotFound((object) planId));
      string stageName = conditionContext.StageName ?? PipelineConstants.DefaultJobName;
      Stage stage1 = pipelineProcess.Stages.SingleOrDefault<Stage>((Func<Stage, bool>) (x => x.Name.Equals(stageName, StringComparison.OrdinalIgnoreCase)));
      PhaseNode phaseNode = stage1 != null ? stage1.Phases.SingleOrDefault<PhaseNode>((Func<PhaseNode, bool>) (x => x.Name.Equals(conditionContext.PhaseName, StringComparison.OrdinalIgnoreCase))) : (PhaseNode) null;
      if (phaseNode == null)
        return (JobExecutionContext) null;
      StageInstance stage2 = new StageInstance(stage1, conditionContext.StageAttempt);
      PhaseInstance phase1 = new PhaseInstance(phaseNode, conditionContext.PhaseAttempt);
      PhaseExecutionContext phaseContext = await this.CreatePhaseExecutionContextAsync(requestContext, plan, stage2, phase1, state: conditionContext.State, includeOutputs: true);
      JobExecutionContext jobContext = (JobExecutionContext) null;
      switch (phaseNode)
      {
        case Microsoft.TeamFoundation.DistributedTask.Pipelines.Phase phase2:
          jobContext = phase2.CreateJobContext(phaseContext, conditionContext.JobName, conditionContext.JobAttempt);
          break;
        case ProviderPhase providerPhase:
          string jobInstanceName = phaseContext.IdGenerator.GetJobInstanceName(conditionContext.StageName, conditionContext.PhaseName, conditionContext.JobName, conditionContext.JobAttempt);
          jobContext = providerPhase.CreateJobContext(phaseContext, await this.GetJobInstanceAsync(requestContext, scopeId, plan.PlanId, jobInstanceName, true) ?? throw new TaskOrchestrationJobNotFoundException(TaskResources.JobNotFound((object) jobInstanceName, (object) planId)));
          jobInstanceName = (string) null;
          providerPhase = (ProviderPhase) null;
          break;
        default:
          throw new NotSupportedException(phaseNode.GetType().Name);
      }
      jobContext.SetSystemVariables((IDictionary<string, VariableValue>) new Dictionary<string, VariableValue>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
      {
        [WellKnownDistributedTaskVariables.JobStatus] = (VariableValue) conditionContext.Result.ToString()
      });
      await this.SetJobOutputsAsync(requestContext, plan, jobContext);
      return jobContext;
    }

    private async Task SetJobOutputsAsync(
      IVssRequestContext requestContext,
      TaskOrchestrationPlan plan,
      JobExecutionContext context)
    {
      OutputVariableScope outputVariablesAsync = await this.GetPlanOutputVariablesAsync(requestContext, plan.ScopeIdentifier, plan.PlanId);
      if (outputVariablesAsync == null)
        ;
      else
      {
        VariablesDictionary variables = new VariablesDictionary();
        foreach (OutputVariableScope outputVariableScope in outputVariablesAsync.FindAll((Predicate<OutputVariableScope>) (x => x.ScopeType == "Job" && x.Id == context.GetInstanceId())))
        {
          foreach (KeyValuePair<string, VariableValue> keyValuePair in outputVariableScope.ChildScopes.SelectMany<OutputVariableScope, KeyValuePair<string, VariableValue>>((Func<OutputVariableScope, IEnumerable<KeyValuePair<string, VariableValue>>>) (x => (IEnumerable<KeyValuePair<string, VariableValue>>) x.Flatten())))
            variables[keyValuePair.Key] = keyValuePair.Value;
        }
        context.SetUserVariables((IDictionary<string, string>) variables);
      }
    }

    private JobEnvironment CreateJobEnvironment(
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      Guid planId,
      TaskOrchestrationPlan plan,
      TaskOrchestrationJob job)
    {
      JobEnvironment jobEnvironment = new JobEnvironment(plan.Environment);
      PlanSecretStore planSecretStore = new PlanSecretStore(requestContext, planId);
      foreach (MaskHint maskHint in plan.Environment.MaskHints.Where<MaskHint>((Func<MaskHint, bool>) (x => x.Type == MaskType.Variable)))
        jobEnvironment.Variables[maskHint.Value] = planSecretStore.GetVariable(maskHint.Value);
      foreach (KeyValuePair<string, string> variable in (IEnumerable<KeyValuePair<string, string>>) job.Variables)
        jobEnvironment.Variables[variable.Key] = variable.Value;
      jobEnvironment.Variables[WellKnownDistributedTaskVariables.PlanId] = planId.ToString("D");
      jobEnvironment.Variables[WellKnownDistributedTaskVariables.JobId] = job.InstanceId.ToString("D");
      jobEnvironment.Variables[WellKnownDistributedTaskVariables.TimelineId] = plan.Timeline.Id.ToString("D");
      string absoluteUri = TaskHub.GetVssClientUrl(requestContext, Microsoft.VisualStudio.Services.WebApi.ServiceInstanceTypes.TFS).AbsoluteUri;
      jobEnvironment.Variables[WellKnownDistributedTaskVariables.TFCollectionUrl] = absoluteUri;
      jobEnvironment.Variables[WellKnownDistributedTaskVariables.TaskDefinitionsUrl] = absoluteUri;
      return jobEnvironment;
    }

    private async Task SendAgentJobRequestAsync(
      IVssRequestContext requestContext,
      int poolId,
      long requestId,
      Guid jobId,
      TaskHub.JobRequestData data,
      TaskAgentReference agent)
    {
      TaskHub taskHub = this;
      MethodScope methodScope = new MethodScope(requestContext, nameof (TaskHub), nameof (SendAgentJobRequestAsync));
      try
      {
        Microsoft.TeamFoundation.DistributedTask.Pipelines.AgentJobRequestMessage jobRequestMessage = (Microsoft.TeamFoundation.DistributedTask.Pipelines.AgentJobRequestMessage) null;
        Microsoft.TeamFoundation.DistributedTask.WebApi.AgentJobRequestMessage message = (Microsoft.TeamFoundation.DistributedTask.WebApi.AgentJobRequestMessage) null;
        if (data.ContainerJob != null)
        {
          message = new Microsoft.TeamFoundation.DistributedTask.WebApi.AgentJobRequestMessage(data.Plan.AsReference(), data.Plan.Timeline, jobId, data.ContainerJob.Name, data.ContainerJob.RefName, data.Environment, (IEnumerable<TaskInstance>) data.ContainerJob.Tasks)
          {
            RequestId = requestId
          };
          jobRequestMessage = AgentJobRequestMessageUtil.Convert(message);
        }
        else if (data.Job != null)
          jobRequestMessage = new Microsoft.TeamFoundation.DistributedTask.Pipelines.AgentJobRequestMessage(data.Plan.AsReference(), data.Plan.Timeline, jobId, data.Job.DisplayName, data.Job.Name, data.Job.Container, data.Job.SidecarContainers, data.Context.Variables, (IList<MaskHint>) data.MaskHints, data.Resources, data.Job.Workspace, (IEnumerable<JobStep>) data.Job.Steps)
          {
            RequestId = requestId
          };
        TelemetryFactory.GetLogger(requestContext).PublishTaskHubSendJobTelemetry(requestContext, taskHub, poolId, data.AuthorizationId, jobRequestMessage, agent, data.Plan.TemplateType.ToString());
        PackageVersion version;
        TaskAgentMessage agentMessage;
        if (string.IsNullOrEmpty(agent?.Version) || !PackageVersion.TryParse(agent?.Version, out version) || version.CompareTo(new PackageVersion("2.137.0")) < 0 || !requestContext.IsFeatureEnabled("DistributedTask.PipelineJobMessage"))
        {
          if (message == null && jobRequestMessage != null)
            message = AgentJobRequestMessageUtil.Convert(jobRequestMessage);
          agentMessage = message.GetAgentMessage();
        }
        else
          agentMessage = jobRequestMessage.GetAgentMessage();
        await requestContext.GetService<IDistributedTaskPoolService>().SendAgentMessageAsync(requestContext, poolId, requestId, agentMessage);
      }
      finally
      {
        methodScope.Dispose();
      }
      methodScope = new MethodScope();
    }

    private void AddEndpointsParametersToMaskHints(
      IVssRequestContext requestContext,
      List<Microsoft.TeamFoundation.DistributedTask.WebApi.ServiceEndpoint> serviceEndpoints,
      HashSet<MaskHint> maskHints)
    {
      using (new MethodScope(requestContext, nameof (TaskHub), nameof (AddEndpointsParametersToMaskHints)))
      {
        Dictionary<string, Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpointType> dictionary = new Dictionary<string, Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpointType>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        IEnumerable<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpointType> serviceEndpointTypes = requestContext.GetService<IServiceEndpointTypesService2>().GetServiceEndpointTypes(requestContext, (string) null, (string) null);
        foreach (Microsoft.TeamFoundation.DistributedTask.WebApi.ServiceEndpoint serviceEndpoint1 in serviceEndpoints)
        {
          Microsoft.TeamFoundation.DistributedTask.WebApi.ServiceEndpoint serviceEndpoint = serviceEndpoint1;
          Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpointType endpointType = (Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpointType) null;
          if (!string.IsNullOrEmpty(serviceEndpoint.Type) && !dictionary.TryGetValue(serviceEndpoint.Type, out endpointType))
          {
            endpointType = serviceEndpointTypes.FirstOrDefault<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpointType>((Func<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpointType, bool>) (t => string.Equals(t.Name, serviceEndpoint.Type, StringComparison.OrdinalIgnoreCase)));
            if (endpointType != null)
              dictionary.Add(serviceEndpoint.Type, endpointType);
          }
          IDictionary<string, string> filteredEndpointData = serviceEndpoint.GetFilteredEndpointData(endpointType, true);
          IDictionary<string, string> authorizationParameters = serviceEndpoint.GetFilteredAuthorizationParameters(endpointType, true);
          List<string> stringList = new List<string>();
          stringList.AddRange((IEnumerable<string>) filteredEndpointData.Values);
          stringList.AddRange((IEnumerable<string>) authorizationParameters.Values);
          foreach (string str in stringList)
          {
            if (!string.IsNullOrEmpty(str))
            {
              maskHints.Add(new MaskHint()
              {
                Type = MaskType.Regex,
                Value = Regex.Escape(str)
              });
              maskHints.Add(new MaskHint()
              {
                Type = MaskType.Regex,
                Value = Regex.Escape(JsonUtility.ToString((object) str))
              });
            }
          }
        }
      }
    }

    private static List<OutputVariableScope> ExpandChildren(
      IVssRequestContext requestContext,
      Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord record,
      IDictionary<Guid, List<Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord>> recordsByParent)
    {
      List<OutputVariableScope> outputVariableScopeList = new List<OutputVariableScope>();
      if (string.IsNullOrEmpty(record.RefName))
      {
        requestContext.TraceError(10015526, "Orchestration", string.Format("Record {0} (name: {1}, type: {2}) does not have a reference name.", (object) record.Id, (object) record.Name, (object) record.RecordType));
        return outputVariableScopeList;
      }
      OutputVariableScope scope = new OutputVariableScope()
      {
        Id = record.Id,
        Name = record.RefName,
        ScopeType = record.RecordType,
        Result = record.Result
      };
      foreach (KeyValuePair<string, VariableValue> variable in (IEnumerable<KeyValuePair<string, VariableValue>>) record.Variables)
        scope.Variables[variable.Key] = variable.Value;
      List<Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord> source;
      if (recordsByParent.TryGetValue(record.Id, out source))
      {
        foreach (Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord record1 in source.Where<Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord>((Func<Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord, bool>) (x => scope.IsValidChild(x.RecordType))))
        {
          List<OutputVariableScope> collection = TaskHub.ExpandChildren(requestContext, record1, recordsByParent);
          if (record.RecordType == "Task")
            outputVariableScopeList.AddRange((IEnumerable<OutputVariableScope>) collection);
          else
            scope.ChildScopes.AddRange((IEnumerable<OutputVariableScope>) collection);
        }
      }
      if (scope.Variables.Count > 0 || scope.ChildScopes.Count > 0)
        outputVariableScopeList.Add(scope);
      return outputVariableScopeList;
    }

    private IPhaseProviderExtension GetPhaseProviderExtension(
      IVssRequestContext requestContext,
      string provider)
    {
      using (new MethodScope(requestContext, nameof (TaskHub), nameof (GetPhaseProviderExtension)))
      {
        if (this.m_phaseProviders == null || !this.m_phaseProviders.Any<KeyValuePair<string, IPhaseProviderExtension>>() || !this.m_phaseProviders.ContainsKey(provider))
        {
          this.m_phaseProviders = (IDictionary<string, IPhaseProviderExtension>) new Dictionary<string, IPhaseProviderExtension>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
          IDisposableReadOnlyList<IPhaseProviderExtension> extensions = requestContext.GetExtensions<IPhaseProviderExtension>(ExtensionLifetime.Service);
          if (extensions != null && extensions.Any<IPhaseProviderExtension>())
            this.m_phaseProviders = (IDictionary<string, IPhaseProviderExtension>) extensions.ToDictionary<IPhaseProviderExtension, string>((Func<IPhaseProviderExtension, string>) (p => p.Provider), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        }
        IPhaseProviderExtension providerExtension;
        if (this.m_phaseProviders.TryGetValue(provider, out providerExtension))
          return providerExtension;
        throw new ArgumentException(TaskResources.PhaseProviderExtensionNotFound((object) provider));
      }
    }

    internal async Task StartProviderPhaseAsync(
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      Guid planId,
      string stageName,
      int stageAttempt,
      string phaseName,
      int phaseAttempt,
      string provider)
    {
      MethodScope methodScope = new MethodScope(requestContext, nameof (TaskHub), nameof (StartProviderPhaseAsync));
      try
      {
        ArgumentUtility.CheckForEmptyGuid(scopeIdentifier, nameof (scopeIdentifier));
        ArgumentUtility.CheckForEmptyGuid(planId, nameof (planId));
        IPhaseProviderExtension phaseProvider = this.GetPhaseProviderExtension(requestContext, provider);
        TaskOrchestrationPlan plan = await this.GetPlanAsync(requestContext, scopeIdentifier, planId);
        if (plan == null)
          throw new TaskOrchestrationPlanNotFoundException(TaskResources.PlanNotFound((object) planId));
        Stage stage = plan.GetProcess<PipelineProcess>().Stages.SingleOrDefault<Stage>((Func<Stage, bool>) (x => x.Name.Equals(stageName, StringComparison.OrdinalIgnoreCase)));
        PhaseNode phase = stage != null ? stage.Phases.FirstOrDefault<PhaseNode>((Func<PhaseNode, bool>) (x => x.Name.Equals(phaseName, StringComparison.OrdinalIgnoreCase))) : (PhaseNode) null;
        if (phase == null)
          throw new ArgumentException(TaskResources.PhaseNotFound((object) phaseName, (object) planId));
        TaskHub.PipelineTraceWriter trace = new TaskHub.PipelineTraceWriter();
        StageInstance stageInstance = new StageInstance(stage, stageAttempt);
        PhaseInstance phaseInstance = new PhaseInstance(phase, phaseAttempt);
        PhaseExecutionContext executionContextAsync = await this.CreatePhaseExecutionContextAsync(requestContext, plan, stageInstance, phaseInstance, (IPipelineTraceWriter) trace);
        string phaseOrchestrationId = PipelineUtilities.GetPhaseOrchestrationId(executionContextAsync, plan.PlanId);
        string projectName = requestContext.GetService<IProjectService>().GetProjectName(requestContext, plan.ScopeIdentifier);
        await phaseProvider.StartPhaseAsync(requestContext, new ProviderPhaseRequest()
        {
          PlanId = plan.PlanId,
          PlanType = plan.PlanType,
          PlanVersion = plan.Version,
          ServiceOwner = requestContext.ServiceInstanceType(),
          PhaseOrchestrationId = phaseOrchestrationId,
          ProviderPhase = phase as ProviderPhase,
          Project = new Microsoft.TeamFoundation.DistributedTask.WebApi.ProjectReference()
          {
            Id = plan.ScopeIdentifier,
            Name = projectName
          },
          Pipeline = plan.Definition.Clone(),
          Run = plan.Owner.Clone(),
          Stage = new PipelineGraphNodeReference(stageInstance.Identifier, stageInstance.Name, stageInstance.Attempt),
          Phase = new PipelineGraphNodeReference(phaseInstance.Identifier, phaseInstance.Name, phaseInstance.Attempt),
          Variables = executionContextAsync.Variables
        }, executionContextAsync);
        phaseProvider = (IPhaseProviderExtension) null;
        plan = (TaskOrchestrationPlan) null;
        phase = (PhaseNode) null;
        stageInstance = (StageInstance) null;
        phaseInstance = (PhaseInstance) null;
      }
      finally
      {
        methodScope.Dispose();
      }
      methodScope = new MethodScope();
    }

    internal async Task CancelProviderPhaseAsync(
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      Guid planId,
      string stageName,
      int stageAttempt,
      string phaseName,
      int phaseAttempt,
      string provider,
      string reason)
    {
      ArgumentUtility.CheckForEmptyGuid(scopeIdentifier, nameof (scopeIdentifier));
      ArgumentUtility.CheckForEmptyGuid(planId, nameof (planId));
      IPhaseProviderExtension phaseProvider = this.GetPhaseProviderExtension(requestContext, provider);
      TaskOrchestrationPlan planAsync = await this.GetPlanAsync(requestContext, scopeIdentifier, planId);
      if (planAsync == null)
        throw new TaskOrchestrationPlanNotFoundException(TaskResources.PlanNotFound((object) planId));
      Stage stage = planAsync.GetProcess<PipelineProcess>().Stages.SingleOrDefault<Stage>((Func<Stage, bool>) (x => x.Name.Equals(stageName, StringComparison.OrdinalIgnoreCase)));
      if ((stage != null ? stage.Phases.FirstOrDefault<PhaseNode>((Func<PhaseNode, bool>) (x => x.Name.Equals(phaseName, StringComparison.OrdinalIgnoreCase))) : (PhaseNode) null) == null)
        throw new ArgumentException(TaskResources.PhaseNotFound((object) phaseName, (object) planId));
      string phaseInstanceName = new PipelineIdGenerator().GetPhaseInstanceName(stageName, phaseName, phaseAttempt);
      await phaseProvider.CancelPhaseAsync(requestContext, string.Format("{0}.{1}", (object) planAsync.PlanId, (object) phaseInstanceName.ToLowerInvariant()), reason);
      phaseProvider = (IPhaseProviderExtension) null;
    }

    internal async Task ProviderJobCompletedAsync(
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      Guid planId,
      string stageName,
      int stageAttempt,
      string phaseName,
      int phaseAttempt,
      string jobInstanceName,
      string provider,
      Microsoft.TeamFoundation.DistributedTask.WebApi.TaskResult result)
    {
      MethodScope methodScope = new MethodScope(requestContext, nameof (TaskHub), nameof (ProviderJobCompletedAsync));
      try
      {
        IPhaseProviderExtension phaseProvider = this.GetPhaseProviderExtension(requestContext, provider);
        TaskOrchestrationPlan plan = await this.GetPlanAsync(requestContext, scopeIdentifier, planId);
        if (plan == null)
          throw new TaskOrchestrationPlanNotFoundException(TaskResources.PlanNotFound((object) planId));
        Stage stage = plan.GetProcess<PipelineProcess>().Stages.SingleOrDefault<Stage>((Func<Stage, bool>) (x => x.Name.Equals(stageName, StringComparison.OrdinalIgnoreCase)));
        if ((stage != null ? stage.Phases.FirstOrDefault<PhaseNode>((Func<PhaseNode, bool>) (x => x.Name.Equals(phaseName, StringComparison.OrdinalIgnoreCase))) : (PhaseNode) null) == null)
          throw new ArgumentException(TaskResources.PhaseNotFound((object) phaseName, (object) planId));
        string phaseInstanceName = new PipelineIdGenerator().GetPhaseInstanceName(stageName, phaseName, phaseAttempt);
        JobInstance jobInstanceAsync = await this.GetJobInstanceAsync(requestContext, scopeIdentifier, plan.PlanId, jobInstanceName);
        if (jobInstanceAsync == null)
          throw new TaskOrchestrationJobNotFoundException(TaskResources.JobNotFound((object) jobInstanceName, (object) planId));
        jobInstanceAsync.State = PipelineState.Completed;
        jobInstanceAsync.FinishTime = new DateTime?(DateTime.UtcNow);
        jobInstanceAsync.Result = new Microsoft.TeamFoundation.DistributedTask.WebApi.TaskResult?(result);
        await phaseProvider.JobCompletedAsync(requestContext, string.Format("{0}.{1}", (object) plan.PlanId, (object) phaseInstanceName.ToLowerInvariant()), jobInstanceAsync);
        await this.DeleteJobInstanceAsync(requestContext, scopeIdentifier, plan.PlanId, jobInstanceName);
        phaseProvider = (IPhaseProviderExtension) null;
        plan = (TaskOrchestrationPlan) null;
        phaseInstanceName = (string) null;
      }
      finally
      {
        methodScope.Dispose();
      }
      methodScope = new MethodScope();
    }

    public async Task ClearJobInstanceAsync(
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      Guid planId,
      string stageName,
      int stageAttempt,
      string phaseName,
      int phaseAttempt,
      string jobInstanceName)
    {
      MethodScope methodScope = new MethodScope(requestContext, nameof (TaskHub), nameof (ClearJobInstanceAsync));
      try
      {
        TaskOrchestrationPlan plan = await this.GetPlanAsync(requestContext, scopeIdentifier, planId);
        if (plan == null)
          throw new TaskOrchestrationPlanNotFoundException(TaskResources.PlanNotFound((object) planId));
        Stage stage = plan.GetProcess<PipelineProcess>().Stages.SingleOrDefault<Stage>((Func<Stage, bool>) (x => x.Name.Equals(stageName, StringComparison.OrdinalIgnoreCase)));
        if ((stage != null ? stage.Phases.FirstOrDefault<PhaseNode>((Func<PhaseNode, bool>) (x => x.Name.Equals(phaseName, StringComparison.OrdinalIgnoreCase))) : (PhaseNode) null) == null)
          throw new ArgumentException(TaskResources.PhaseNotFound((object) phaseName, (object) planId));
        if (await this.GetJobInstanceAsync(requestContext, scopeIdentifier, plan.PlanId, jobInstanceName) == null)
          throw new TaskOrchestrationJobNotFoundException(TaskResources.JobNotFound((object) jobInstanceName, (object) planId));
        await this.DeleteJobInstanceAsync(requestContext, scopeIdentifier, plan.PlanId, jobInstanceName);
        plan = (TaskOrchestrationPlan) null;
      }
      finally
      {
        methodScope.Dispose();
      }
      methodScope = new MethodScope();
    }

    internal async Task ProviderJobStartedAsync(
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      Guid planId,
      JobParameters parameters,
      JobStartedEventData eventData)
    {
      MethodScope methodScope = new MethodScope(requestContext, nameof (TaskHub), nameof (ProviderJobStartedAsync));
      try
      {
        TaskOrchestrationPlan plan = await this.GetPlanAsync(requestContext, scopeIdentifier, planId);
        if (plan == null)
          throw new TaskOrchestrationPlanNotFoundException(TaskResources.PlanNotFound((object) planId));
        TaskHub.PipelineTraceWriter pipelineTraceWriter = new TaskHub.PipelineTraceWriter();
        PipelineProcess process = plan.GetProcess<PipelineProcess>();
        string stageName = parameters.StageName ?? PipelineConstants.DefaultJobName;
        Stage stage = process.Stages.SingleOrDefault<Stage>((Func<Stage, bool>) (x => x.Name.Equals(stageName, StringComparison.OrdinalIgnoreCase)));
        PhaseNode phase = stage != null ? stage.Phases.SingleOrDefault<PhaseNode>((Func<PhaseNode, bool>) (x => x.Name.Equals(parameters.PhaseName, StringComparison.OrdinalIgnoreCase))) : (PhaseNode) null;
        if (phase == null)
          throw new ArgumentException(TaskResources.PhaseNotFound((object) parameters.PhaseName, (object) planId));
        PipelineIdGenerator pipelineIdGenerator = new PipelineIdGenerator();
        string phaseInstanceName = pipelineIdGenerator.GetPhaseInstanceName(stageName, parameters.PhaseName, parameters.PhaseAttempt);
        string jobInstanceName = pipelineIdGenerator.GetJobInstanceName(parameters.StageName, parameters.PhaseName, parameters.Name, parameters.Attempt, 1);
        JobInstance jobInstanceAsync = await this.GetJobInstanceAsync(requestContext, plan.ScopeIdentifier, plan.PlanId, jobInstanceName);
        if (jobInstanceAsync == null)
          throw new TaskOrchestrationJobNotFoundException(TaskResources.JobNotFound((object) jobInstanceName, (object) planId));
        jobInstanceAsync.State = PipelineState.InProgress;
        jobInstanceAsync.StartTime = new DateTime?(DateTime.UtcNow);
        JobInstance job = await this.UpdateJobInstanceAsync(requestContext, plan.ScopeIdentifier, plan.PlanId, jobInstanceAsync);
        if (phase is ProviderPhase)
          await this.GetPhaseProviderExtension(requestContext, ((ProviderPhase) phase).Provider).JobStartedAsync(requestContext, string.Format("{0}.{1}", (object) plan.PlanId, (object) phaseInstanceName.ToLowerInvariant()), job, eventData);
        plan = (TaskOrchestrationPlan) null;
        phase = (PhaseNode) null;
        phaseInstanceName = (string) null;
        jobInstanceName = (string) null;
      }
      finally
      {
        methodScope.Dispose();
      }
      methodScope = new MethodScope();
    }

    internal async Task<string> GenerateOidcTokenAsync(
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      Guid planId,
      Guid jobId,
      string requestedAudience = null)
    {
      using (new MethodScope(requestContext, nameof (TaskHub), nameof (GenerateOidcTokenAsync)))
      {
        ArgumentUtility.CheckForEmptyGuid(scopeIdentifier, nameof (scopeIdentifier));
        ArgumentUtility.CheckForEmptyGuid(planId, nameof (planId));
        ArgumentUtility.CheckForEmptyGuid(jobId, nameof (jobId));
        using (new TraceWatch(requestContext, 10016186, TraceLevel.Warning, TimeSpan.FromSeconds(5.0), "DistributedTask", nameof (TaskHub), "Generating pipeline IdToken for plan '{0}', job '{1}' in project '{2}' took too long (Threshold 5s)", new object[3]
        {
          (object) planId,
          (object) jobId,
          (object) scopeIdentifier
        }))
        {
          if (!requestContext.IsFeatureEnabled("DistributedTask.IssuePipelineIdTokens"))
            return (string) null;
          TaskHub.EnsureCallerIsBuildIdentity(requestContext);
          TaskHub.EnsureSessionTokenIssuedForJob(requestContext, planId, jobId);
          Microsoft.TeamFoundation.DistributedTask.WebApi.TaskOrchestrationPlanReference planReference = this.EnsurePlanData(requestContext, scopeIdentifier, planId);
          IDictionary<string, VariableValue> systemVariables = (await this.GetPipelineEnvironmentAsync(requestContext, planReference)).SystemVariables;
          ITimelineRecordReference timelineRecordReference = await this.EnsureJobIsRunning(requestContext, planReference, jobId);
          string projectName;
          string audience = requestContext.GetService<IProjectService>().TryGetProjectName(requestContext, scopeIdentifier, out projectName) ? TaskHub.SelectOidcTokenAudience(requestContext, requestedAudience) : throw new ProjectDoesNotExistException(scopeIdentifier.ToString("D"));
          string name = planReference.Definition.Name;
          string subject = "p://" + requestContext.ServiceHost.Name + "/" + projectName + "/" + name;
          PipelineOidcFederationClaims claims = new PipelineOidcFederationClaims()
          {
            Subject = subject,
            Audience = audience,
            OrganizationId = requestContext.ServiceHost.InstanceId,
            ProjectId = scopeIdentifier,
            PipelineDefinitionId = planReference.Definition.Id,
            RepositoryId = systemVariables.GetValueOrDefault<string, VariableValue>("build.repository.id")?.Value,
            RepositoryUri = systemVariables.GetValueOrDefault<string, VariableValue>("build.repository.uri")?.Value,
            RepositoryVersion = systemVariables.GetValueOrDefault<string, VariableValue>("Build.SourceVersion")?.Value,
            RepositoryRef = systemVariables.GetValueOrDefault<string, VariableValue>("Build.SourceBranch")?.Value
          };
          TimeSpan lifespan = TaskHub.SelectOidcTokenLifespan(requestContext);
          try
          {
            return this.ExecuteWithSetFlagToSkipPermissionsCheck<string>(requestContext, (Func<string>) (() =>
            {
              SessionToken oidcToken = SessionTokenGenerator.GenerateOidcToken(requestContext.Elevate(), scopeIdentifier, (IOidcFederationClaims) claims, lifespan);
              requestContext.TraceInfo(10016180, nameof (TaskHub), "Generated pipeline OpenIdConnect token for plan '{0}', job '{1}', subject '{2}', audience '{3}' with validity '{4}'", (object) planId, (object) jobId, (object) subject, (object) audience, (object) oidcToken.ValidTo);
              return oidcToken.Token;
            }));
          }
          catch (Exception ex)
          {
            requestContext.TraceException(10016181, nameof (TaskHub), ex);
            throw;
          }
        }
      }
    }

    internal async Task<string> GenerateOidcTokenAsync(
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      Guid planId,
      Guid jobId,
      Guid serviceConnectionId,
      string requestedAudience = null)
    {
      using (new MethodScope(requestContext, nameof (TaskHub), nameof (GenerateOidcTokenAsync)))
      {
        ArgumentUtility.CheckForEmptyGuid(scopeIdentifier, nameof (scopeIdentifier));
        ArgumentUtility.CheckForEmptyGuid(planId, nameof (planId));
        ArgumentUtility.CheckForEmptyGuid(jobId, nameof (jobId));
        ArgumentUtility.CheckForEmptyGuid(serviceConnectionId, nameof (serviceConnectionId));
        using (new TraceWatch(requestContext, 10016186, TraceLevel.Warning, TimeSpan.FromSeconds(5.0), "DistributedTask", nameof (TaskHub), "Generating endpoint IdToken for plan '{0}', job '{1}' and endpoint '{2}' in project '{3}' took too long (Threshold 5s)", new object[4]
        {
          (object) planId,
          (object) jobId,
          (object) serviceConnectionId,
          (object) scopeIdentifier
        }))
        {
          if (!await GlobalContributedFeatureStateResolver.IsFeatureEnabled(requestContext, "ms.vss-distributedtask-web.workload-identity-federation", Microsoft.VisualStudio.Services.WebApi.ServiceInstanceTypes.TFS))
            return (string) null;
          TaskHub.EnsureCallerIsBuildIdentity(requestContext);
          TaskHub.EnsureSessionTokenIssuedForJob(requestContext, planId, jobId);
          Microsoft.TeamFoundation.DistributedTask.Pipelines.ServiceEndpointReference serviceEndpointReference = await this.EnsureJobReferencesServiceConnection(requestContext, scopeIdentifier, planId, jobId, serviceConnectionId);
          IOidcFederationClaims oidcFederationClaims = await this.CreateOidcFederationClaims(requestContext, scopeIdentifier, serviceEndpointReference, requestedAudience);
          TimeSpan lifespan = TaskHub.SelectOidcTokenLifespan(requestContext);
          try
          {
            return this.ExecuteWithSetFlagToSkipPermissionsCheck<string>(requestContext, (Func<string>) (() =>
            {
              SessionToken oidcToken = SessionTokenGenerator.GenerateOidcToken(requestContext.Elevate(), scopeIdentifier, oidcFederationClaims, lifespan);
              requestContext.TraceInfo(10016180, nameof (TaskHub), "Generated endpoint OpenIdConnect token for plan '{0}', job '{1}', subject '{2}', audience '{3}', endpoint '{4}' with validity '{5}'", (object) planId, (object) jobId, (object) oidcFederationClaims.Subject, (object) oidcFederationClaims.Audience, (object) serviceConnectionId, (object) oidcToken.ValidTo);
              return oidcToken.Token;
            }));
          }
          catch (Exception ex)
          {
            requestContext.TraceException(10016181, nameof (TaskHub), ex);
            throw;
          }
        }
      }
    }

    private T ExecuteWithSetFlagToSkipPermissionsCheck<T>(
      IVssRequestContext requestContext,
      Func<T> action)
    {
      object obj = (object) null;
      string str;
      if (requestContext.TryGetItem<string>("$Scp", out str) && str != "app_token")
      {
        requestContext.RootContext.Items.TryGetValue(RequestContextItemsKeys.IsFrameworkIdentityReadPermissionCheckComplete, out obj);
        requestContext.RootContext.Items[RequestContextItemsKeys.IsFrameworkIdentityReadPermissionCheckComplete] = (object) true;
      }
      try
      {
        return action();
      }
      finally
      {
        if (!string.IsNullOrWhiteSpace(str) && str != "app_token")
        {
          if (obj == null)
            requestContext.RootContext.Items.Remove(RequestContextItemsKeys.IsFrameworkIdentityReadPermissionCheckComplete);
          else
            requestContext.RootContext.Items[RequestContextItemsKeys.IsFrameworkIdentityReadPermissionCheckComplete] = obj;
        }
      }
    }

    private static void EnsureCallerIsBuildIdentity(IVssRequestContext requestContext)
    {
      if (!requestContext.GetUserIdentity().IsBuildIdentity())
      {
        requestContext.TraceWarning(10016182, nameof (TaskHub), "Non-build identity '{0}' attempted to generate OpenIdConnect token", (object) requestContext.GetUserIdentity().Id);
        throw new Microsoft.TeamFoundation.DistributedTask.WebApi.AccessDeniedException("Only build identities are allowed to request OpenIdConnect tokens.");
      }
    }

    private static void EnsureSessionTokenIssuedForJob(
      IVssRequestContext requestContext,
      Guid planId,
      Guid jobId)
    {
      ClaimsIdentity claimsIdentity;
      if (requestContext.Items.TryGetValue<ClaimsIdentity>(RequestContextItemsKeys.AuthorizedClaimsIdentity, out claimsIdentity))
      {
        foreach (Claim claim in claimsIdentity.Claims)
        {
          if (claim.Type == "jobref" && claim.Value == TaskHub.JobRefClaimValue(planId, jobId))
            return;
        }
      }
      requestContext.TraceWarning(10016183, nameof (TaskHub), "Build token does not have a matching claim '{0}' to request IdTokens for plan '{1}' and job '{2}'", (object) "jobref", (object) planId, (object) jobId);
      throw new Microsoft.TeamFoundation.DistributedTask.WebApi.AccessDeniedException("Not allowed to request OpenIdConnect token.");
    }

    private static TimeSpan SelectOidcTokenLifespan(IVssRequestContext requestContext) => TimeSpan.FromMinutes((double) requestContext.GetService<IVssRegistryService>().GetValue<int>(requestContext, in RegistryKeys.OidcTokenMaxValidTime, true, 5));

    private async Task<IOidcFederationClaims> CreateOidcFederationClaims(
      IVssRequestContext requestContext,
      Guid projectId,
      Microsoft.TeamFoundation.DistributedTask.Pipelines.ServiceEndpointReference serviceEndpointReference,
      string requestedAudience)
    {
      string audience = TaskHub.SelectOidcTokenAudience(requestContext, requestedAudience);
      IOidcFederationClaims federationClaims;
      try
      {
        federationClaims = (IOidcFederationClaims) await OidcFederationClaims.CreateOidcFederationClaims(requestContext, projectId, serviceEndpointReference.Id, audience);
      }
      catch (Exception ex)
      {
        requestContext.TraceException("Orchestration", ex);
        throw new EndpointNotFoundException(string.Format("Could not construct the subject for {0} service endpoint", (object) serviceEndpointReference.Id));
      }
      return federationClaims;
    }

    private static string SelectOidcTokenAudience(
      IVssRequestContext requestContext,
      string audience)
    {
      string str = "api://AzureADTokenExchange";
      if (!string.IsNullOrEmpty(audience))
      {
        string a = audience.TrimEnd('/', '\\');
        RegistryEntryCollection registryEntryCollection = requestContext.GetService<IVssRegistryService>().ReadEntriesFallThru(requestContext, in TaskHub.s_allowedOidcTokenAudiences);
        bool flag = false;
        foreach (RegistryEntry registryEntry in registryEntryCollection)
        {
          string b = registryEntry.GetValue<string>();
          if (string.Equals(a, b, StringComparison.OrdinalIgnoreCase))
          {
            str = b;
            flag = true;
            break;
          }
        }
        if (!flag)
          throw new TaskOrchestrationPlanSecurityException("Can't issue ID_TOKEN for audience '" + audience + "'.");
      }
      return str;
    }

    private async Task<ITimelineRecordReference> EnsureJobIsRunning(
      IVssRequestContext requestContext,
      Microsoft.TeamFoundation.DistributedTask.WebApi.TaskOrchestrationPlanReference planReference,
      Guid jobId)
    {
      ITimelineRecordReference timelineRecordReference;
      if (requestContext.IsFeatureEnabled("DistributedTask.UseQuickGetTimelineRecordsForWorkloadIdentityFederation"))
        timelineRecordReference = await this.GetTimelineRecordReferenceAsync(requestContext, planReference.ScopeIdentifier, planReference.PlanId, jobId);
      else
        timelineRecordReference = (ITimelineRecordReference) await this.GetTimelineRecordAsync(requestContext, planReference.ScopeIdentifier, planReference.PlanId, jobId);
      Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecordState? nullable = timelineRecordReference != null ? timelineRecordReference.State : throw new TaskOrchestrationJobNotFoundException(string.Format("Job {0} doesn't exist.", (object) jobId));
      Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecordState timelineRecordState = Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecordState.InProgress;
      if (!(nullable.GetValueOrDefault() == timelineRecordState & nullable.HasValue))
        throw new TaskOrchestrationPlanSecurityException(string.Format("Can't issue ID_TOKEN for job in '{0}' state.", (object) timelineRecordReference.State));
      return timelineRecordReference;
    }

    public async Task CompleteProviderPhaseAsync(
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      Guid planId,
      string stageName,
      string phaseName,
      int phaseAttempt,
      Microsoft.TeamFoundation.DistributedTask.WebApi.TaskResult result)
    {
      MethodScope methodScope = new MethodScope(requestContext, nameof (TaskHub), nameof (CompleteProviderPhaseAsync));
      try
      {
        ArgumentUtility.CheckForEmptyGuid(scopeIdentifier, nameof (scopeIdentifier));
        ArgumentUtility.CheckForEmptyGuid(planId, nameof (planId));
        string phaseOrchestrationId = TaskHub.GetPhaseOrchestrationId(planId, stageName, phaseName);
        using (requestContext.CreateOrchestrationIdScope(phaseOrchestrationId))
        {
          int num;
          if (num != 0 && this.EnsurePlanData(requestContext, scopeIdentifier, planId) == null)
            throw new TaskOrchestrationPlanNotFoundException(TaskResources.PlanNotFound((object) planId));
          try
          {
            await this.RaiseOrchestrationEventAsync(requestContext, phaseOrchestrationId, "Completed", (object) result, true);
          }
          catch (OrchestrationSessionNotFoundException ex)
          {
            throw new TaskOrchestrationPlanTerminatedException(TaskResources.PlanOrchestrationTerminated((object) phaseOrchestrationId), (Exception) ex);
          }
        }
        phaseOrchestrationId = (string) null;
      }
      finally
      {
        methodScope.Dispose();
      }
      methodScope = new MethodScope();
    }

    public async Task<JobInstance> QueueProviderJobAsync(
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      Guid planId,
      string stageName,
      string phaseName,
      int phaseAttempt,
      JobInstance jobInstance,
      bool raiseJobQueuedEvent = true)
    {
      TaskHub taskHub1 = this;
      JobInstance jobInstance1;
      using (new MethodScope(requestContext, nameof (TaskHub), nameof (QueueProviderJobAsync)))
      {
        ArgumentUtility.CheckForEmptyGuid(scopeIdentifier, nameof (scopeIdentifier));
        ArgumentUtility.CheckForEmptyGuid(planId, nameof (planId));
        string phaseOrchestrationId = TaskHub.GetPhaseOrchestrationId(planId, stageName, phaseName);
        using (requestContext.CreateOrchestrationIdScope(phaseOrchestrationId))
        {
          TaskOrchestrationPlan plan = await taskHub1.GetPlanAsync(requestContext, scopeIdentifier, planId);
          PipelineProcess pipeline = plan != null ? plan.GetProcess<PipelineProcess>() : throw new TaskOrchestrationPlanNotFoundException(TaskResources.PlanNotFound((object) planId));
          if (stageName == null)
            stageName = PipelineConstants.DefaultJobName;
          Stage stage = pipeline.Stages.SingleOrDefault<Stage>((Func<Stage, bool>) (x => x.Name.Equals(stageName, StringComparison.OrdinalIgnoreCase)));
          Stage stage1 = stage;
          PhaseNode phase1 = stage1 != null ? stage1.Phases.FirstOrDefault<PhaseNode>((Func<PhaseNode, bool>) (x => x.Name.Equals(phaseName, StringComparison.OrdinalIgnoreCase))) : (PhaseNode) null;
          if (phase1 == null)
            throw new ArgumentException(TaskResources.PhaseNotFound((object) phaseName, (object) planId));
          TaskHub.PipelineTraceWriter trace = new TaskHub.PipelineTraceWriter();
          StageInstance stage2 = new StageInstance(stage);
          PhaseInstance phase2 = new PhaseInstance(phase1, phaseAttempt);
          PhaseExecutionContext phaseExecutionContext = await taskHub1.CreatePhaseExecutionContextAsync(requestContext, plan, stage2, phase2, (IPipelineTraceWriter) trace);
          JobExecutionContext jobExecutionContext = phaseExecutionContext.CreateJobContext(jobInstance);
          string jobInstanceName = jobExecutionContext.GetInstanceName();
          List<PipelineValidationError> validationErrors = new List<PipelineValidationError>();
          if (await taskHub1.GetJobInstanceAsync(requestContext, plan.ScopeIdentifier, plan.PlanId, jobInstanceName) != null)
          {
            validationErrors.Add(new PipelineValidationError(TaskResources.JobNotFound((object) jobInstanceName, (object) planId)));
          }
          else
          {
            bool flag = requestContext.IsFeatureEnabled("DistributedTask.DisableYamlDemandsLatestAgent");
            IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
            BuildOptions options = new BuildOptions()
            {
              DemandLatestAgent = !flag,
              MinimumAgentVersion = flag ? service.GetValue<string>(requestContext, (RegistryQuery) "/Service/DistributedTask/Pipelines/MinAgentVersion", true, "2.163.1") : (string) null,
              MinimumAgentVersionDemandSource = flag ? AgentFeatureDemands.YamlPipelinesDemandSource() : (DemandSource) null,
              ResolveTaskInputAliases = true,
              ValidateResources = true,
              ValidateStepNames = true
            };
            validationErrors.AddRange((IEnumerable<PipelineValidationError>) jobExecutionContext.Validate((IList<Step>) jobInstance.Definition.Steps.OfType<Step>().ToList<Step>(), jobInstance.Definition.Target, options));
          }
          if (validationErrors.Count > 0)
          {
            List<Microsoft.TeamFoundation.DistributedTask.WebApi.Issue> issueList = new List<Microsoft.TeamFoundation.DistributedTask.WebApi.Issue>();
            foreach (PipelineValidationError pipelineValidationError in validationErrors)
              issueList.Add(new Microsoft.TeamFoundation.DistributedTask.WebApi.Issue()
              {
                Type = Microsoft.TeamFoundation.DistributedTask.WebApi.IssueType.Error,
                Message = pipelineValidationError.Message
              });
            TaskHub taskHub2 = taskHub1;
            IVssRequestContext requestContext1 = requestContext;
            Guid scopeIdentifier1 = scopeIdentifier;
            Guid planId1 = planId;
            Guid timelineId = planId;
            Guid instanceId = phaseExecutionContext.GetInstanceId();
            IEnumerable<Microsoft.TeamFoundation.DistributedTask.WebApi.Issue> issues1 = (IEnumerable<Microsoft.TeamFoundation.DistributedTask.WebApi.Issue>) issueList;
            Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecordState? state = new Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecordState?();
            DateTime? startTime = new DateTime?();
            DateTime? finishTime = new DateTime?();
            int? percentComplete = new int?();
            Microsoft.TeamFoundation.DistributedTask.WebApi.TaskResult? result = new Microsoft.TeamFoundation.DistributedTask.WebApi.TaskResult?();
            IEnumerable<Microsoft.TeamFoundation.DistributedTask.WebApi.Issue> issues2 = issues1;
            int? queueId = new int?();
            await taskHub2.UpdateTimelineRecordAsync(requestContext1, scopeIdentifier1, planId1, timelineId, instanceId, state, startTime, finishTime, percentComplete: percentComplete, result: result, issues: issues2, queueId: queueId);
            throw new PipelineValidationException((IEnumerable<PipelineValidationError>) validationErrors);
          }
          jobInstance.Definition.Id = jobExecutionContext.GetInstanceId();
          foreach (JobStep step in (IEnumerable<JobStep>) jobInstance.Definition.Steps)
            step.Id = jobExecutionContext.IdGenerator.GetTaskInstanceId(stageName, phaseName, jobInstance.Definition.Name, jobInstance.Attempt, step.Name, jobInstance.CheckRerunAttempt);
          List<TaskReferenceData> tasks = new List<TaskReferenceData>();
          tasks.AddRange((IEnumerable<TaskReferenceData>) TaskHub.GetTaskReferences(requestContext, (IEnumerable<JobStep>) jobInstance.Definition.Steps));
          List<Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord> timelineRecordList1 = new List<Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord>();
          if (jobInstance.State == PipelineState.Completed)
          {
            List<Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord> timelineRecordList2 = timelineRecordList1;
            Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord timelineRecord = new Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord();
            timelineRecord.Attempt = jobInstance.Attempt;
            timelineRecord.Id = jobInstance.Definition.Id;
            timelineRecord.Identifier = jobInstance.Identifier;
            timelineRecord.Name = jobInstance.Definition.DisplayName;
            timelineRecord.ParentId = new Guid?(phaseExecutionContext.GetInstanceId());
            timelineRecord.RefName = jobInstance.Definition.Name;
            timelineRecord.RecordType = "Job";
            timelineRecord.State = new Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecordState?(Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecordState.Completed);
            timelineRecord.Result = new Microsoft.TeamFoundation.DistributedTask.WebApi.TaskResult?(jobInstance.Result.GetValueOrDefault());
            DateTime? nullable = jobInstance.StartTime;
            timelineRecord.StartTime = new DateTime?(nullable ?? DateTime.UtcNow);
            nullable = jobInstance.FinishTime;
            timelineRecord.FinishTime = new DateTime?(nullable ?? DateTime.UtcNow);
            timelineRecordList2.Add(timelineRecord);
          }
          else
          {
            jobInstance.State = PipelineState.NotStarted;
            timelineRecordList1.Add(new Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord()
            {
              Attempt = jobInstance.Attempt,
              Id = jobInstance.Definition.Id,
              Identifier = jobInstance.Identifier,
              Name = jobInstance.Definition.DisplayName,
              ParentId = new Guid?(phaseExecutionContext.GetInstanceId()),
              RefName = jobInstance.Definition.Name,
              RecordType = "Job",
              State = new Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecordState?(Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecordState.Pending)
            });
          }
          List<Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineAttempt> list = timelineRecordList1.Select<Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord, Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineAttempt>((Func<Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord, Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineAttempt>) (x => new Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineAttempt()
          {
            Identifier = x.Identifier,
            Attempt = x.Attempt,
            RecordId = x.Id
          })).ToList<Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineAttempt>();
          Microsoft.VisualStudio.Services.Identity.Identity userIdentity = requestContext.GetUserIdentity();
          Microsoft.TeamFoundation.DistributedTask.WebApi.Timeline timeline;
          using (TaskTrackingComponent trackingComponent = requestContext.CreateComponent<TaskTrackingComponent>(taskHub1.DataspaceCategory))
            timeline = await trackingComponent.AddJobsAsync(scopeIdentifier, planId, userIdentity.Id, (IList<TaskOrchestrationJob>) Array.Empty<TaskOrchestrationJob>(), (IList<TaskReferenceData>) tasks, (IList<Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord>) timelineRecordList1, (IEnumerable<Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineAttempt>) list);
          if (timeline != null)
            taskHub1.Extension.TimelineRecordsUpdated(requestContext, (Microsoft.TeamFoundation.DistributedTask.WebApi.TaskOrchestrationPlanReference) plan, (Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineReference) timeline, (IEnumerable<Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord>) timeline.Records);
          Dictionary<string, string> jobDisplayNames = new Dictionary<string, string>();
          jobDisplayNames.Add(jobInstance.Identifier, Microsoft.TeamFoundation.DistributedTask.Pipelines.Phase.GenerateDisplayName(stage, job: jobInstance.Definition));
          Dictionary<string, string> jobIds = new Dictionary<string, string>()
          {
            [jobInstance.Identifier] = jobInstance.Definition.Id.ToString()
          };
          bool flag1 = taskHub1.IsSingleJobPipeline(pipeline);
          await taskHub1.Extension.JobsCreatedAsync(requestContext, phaseExecutionContext, (IReadOnlyList<JobInstance>) new JobInstance[1]
          {
            jobInstance
          }, plan, (IDictionary<string, string>) jobDisplayNames, (IDictionary<string, string>) jobIds, timeline, (flag1 ? 1 : 0) != 0);
          if (jobInstance.State == PipelineState.NotStarted)
          {
            jobInstance = await taskHub1.AddJobInstanceAsync(requestContext, plan.ScopeIdentifier, plan.PlanId, jobInstance);
            if (raiseJobQueuedEvent)
            {
              try
              {
                await taskHub1.RaiseOrchestrationEventAsync(requestContext, phaseOrchestrationId, "JobQueued", (object) new JobExecutionState(jobInstance), true);
              }
              catch (OrchestrationSessionNotFoundException ex)
              {
                throw new TaskOrchestrationPlanTerminatedException(TaskResources.PlanOrchestrationTerminated((object) phaseOrchestrationId), (Exception) ex);
              }
            }
          }
          jobInstance1 = jobInstance;
        }
      }
      return jobInstance1;
    }

    public async Task<JobInstance> CancelProviderJobAsync(
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      Guid planId,
      string stageName,
      string phaseName,
      int phaseAttempt,
      JobInstance job)
    {
      TaskHub taskHub1 = this;
      JobInstance jobInstance1;
      using (new MethodScope(requestContext, nameof (TaskHub), nameof (CancelProviderJobAsync)))
      {
        ArgumentUtility.CheckForEmptyGuid(scopeIdentifier, nameof (scopeIdentifier));
        ArgumentUtility.CheckForEmptyGuid(planId, nameof (planId));
        string jobInstanceName = PipelineUtilities.GetJobInstanceName(job.Identifier, job.Attempt);
        string jobOrchestrationId = string.Format("{0}.{1}", (object) planId, (object) jobInstanceName.ToLowerInvariant());
        using (requestContext.CreateOrchestrationIdScope(jobOrchestrationId))
        {
          Microsoft.TeamFoundation.DistributedTask.WebApi.TaskOrchestrationPlanReference plan = taskHub1.EnsurePlanData(requestContext, scopeIdentifier, planId);
          if (plan == null)
            throw new TaskOrchestrationPlanNotFoundException(TaskResources.PlanNotFound((object) planId));
          JobInstance jobInstance = await taskHub1.GetJobInstanceAsync(requestContext, scopeIdentifier, plan.PlanId, jobInstanceName);
          if (jobInstance == null)
            throw new TaskOrchestrationJobNotFoundException(TaskResources.JobNotFound((object) jobInstanceName, (object) planId));
          if (jobInstance.State != PipelineState.Canceling && job.State == PipelineState.Canceling)
          {
            jobInstance.State = PipelineState.Canceling;
            jobInstance = await taskHub1.UpdateJobInstanceAsync(requestContext, scopeIdentifier, plan.PlanId, jobInstance);
            try
            {
              TaskHub taskHub2 = taskHub1;
              IVssRequestContext requestContext1 = requestContext;
              string instanceId = jobOrchestrationId;
              CanceledEvent eventData = new CanceledEvent();
              eventData.Reason = TaskResources.Canceled();
              DateTime? fireAt = new DateTime?();
              await taskHub2.RaiseOrchestrationEventAsync(requestContext1, instanceId, "JobCanceled", (object) eventData, true, fireAt);
            }
            catch (OrchestrationSessionNotFoundException ex)
            {
              throw new TaskOrchestrationPlanTerminatedException(TaskResources.PlanOrchestrationTerminated((object) jobOrchestrationId), (Exception) ex);
            }
          }
          jobInstance1 = jobInstance;
        }
      }
      return jobInstance1;
    }

    public async Task<JobInstance> GetJobInstanceAsync(
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      Guid planId,
      string jobInstanceName,
      bool populateSecrets = false,
      bool neverLoadSecrets = false)
    {
      JobInstance jobInstanceAsync;
      using (new MethodScope(requestContext, nameof (TaskHub), nameof (GetJobInstanceAsync)))
      {
        ArgumentUtility.CheckForEmptyGuid(scopeIdentifier, nameof (scopeIdentifier));
        ArgumentUtility.CheckForEmptyGuid(planId, nameof (planId));
        ArgumentUtility.CheckStringForNullOrEmpty(jobInstanceName, nameof (jobInstanceName));
        this.EnsurePlanData(requestContext, scopeIdentifier, planId);
        JobInstance toSerialize = (JobInstance) null;
        IMutableDictionaryCacheContainer<string, string> jobCache = this.GetJobInstanceRedisCache(requestContext);
        string toDeserialize;
        if (jobCache == null || !jobCache.TryGet<string, string>(requestContext, string.Format("{0}.{1}", (object) planId, (object) jobInstanceName.ToLowerInvariant()), out toDeserialize) || string.IsNullOrEmpty(toDeserialize))
        {
          using (TaskTrackingComponent trackingComponent = requestContext.CreateComponent<TaskTrackingComponent>(this.DataspaceCategory))
          {
            if (trackingComponent.SupportStoreJobInstance)
              toSerialize = await trackingComponent.GetPlanContextAsync<JobInstance>(scopeIdentifier, planId, jobInstanceName);
          }
          if (toSerialize != null && jobCache != null)
            jobCache.Set(requestContext, (IDictionary<string, string>) new Dictionary<string, string>()
            {
              {
                string.Format("{0}.{1}", (object) planId, (object) jobInstanceName.ToLowerInvariant()),
                JsonUtility.ToString((object) toSerialize)
              }
            });
        }
        else
          toSerialize = JsonUtility.FromString<JobInstance>(toDeserialize);
        bool flag = !neverLoadSecrets && (populateSecrets || !requestContext.IsFeatureEnabled("DistributedTask.OnlyLoadJobInstanceSecretsWhenNeeded"));
        if (toSerialize != null & flag)
        {
          PlanSecretStore planSecretStore = new PlanSecretStore(requestContext, planId);
          foreach (IVariable variable1 in (IEnumerable<IVariable>) toSerialize.Definition.Variables)
          {
            if (variable1 is Variable variable2 && variable2.Secret)
              variable2.Value = planSecretStore.GetVariable(variable2.Name);
          }
        }
        jobInstanceAsync = toSerialize;
      }
      return jobInstanceAsync;
    }

    internal async Task<Job> GetAgentRequestJobAsync(
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      string orchestrationId)
    {
      Guid planGuid;
      string jobInstanceName;
      if (!OrchestrationUtilities.TryParseSessionId(orchestrationId, out planGuid, out jobInstanceName))
        throw new TaskOrchestrationJobNotFoundException(TaskResources.InvalidOrchestrationId((object) orchestrationId));
      Microsoft.TeamFoundation.DistributedTask.WebApi.TaskOrchestrationPlanReference planData = this.GetPlanData(requestContext, scopeIdentifier, planGuid);
      if (planData == null || !this.Extension.HasReadPermission(requestContext, planData.ScopeIdentifier, planData.ArtifactUri))
        throw new TaskOrchestrationJobNotFoundException(TaskResources.PlanNotFound((object) planGuid));
      Job definition = (await this.GetJobInstanceAsync(requestContext, scopeIdentifier, planGuid, jobInstanceName) ?? throw new TaskOrchestrationJobNotFoundException(TaskResources.JobNotFound((object) jobInstanceName, (object) planGuid))).Definition;
      jobInstanceName = (string) null;
      return definition;
    }

    internal async Task DeleteJobInstanceAsync(
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      Guid planId,
      string jobInstanceName)
    {
      MethodScope methodScope = new MethodScope(requestContext, nameof (TaskHub), nameof (DeleteJobInstanceAsync));
      try
      {
        ArgumentUtility.CheckForEmptyGuid(scopeIdentifier, nameof (scopeIdentifier));
        ArgumentUtility.CheckForEmptyGuid(planId, nameof (planId));
        ArgumentUtility.CheckStringForNullOrEmpty(jobInstanceName, nameof (jobInstanceName));
        this.EnsurePlanData(requestContext, scopeIdentifier, planId);
        IMutableDictionaryCacheContainer<string, string> jobCache = this.GetJobInstanceRedisCache(requestContext);
        try
        {
          using (TaskTrackingComponent trackingComponent = requestContext.CreateComponent<TaskTrackingComponent>(this.DataspaceCategory))
          {
            if (trackingComponent.SupportStoreJobInstance)
              await trackingComponent.DeletePlanContextAsync(scopeIdentifier, planId, jobInstanceName);
          }
        }
        finally
        {
          jobCache?.Invalidate(requestContext, (IEnumerable<string>) new string[1]
          {
            string.Format("{0}.{1}", (object) planId, (object) jobInstanceName.ToLowerInvariant())
          });
        }
        jobCache = (IMutableDictionaryCacheContainer<string, string>) null;
      }
      finally
      {
        methodScope.Dispose();
      }
      methodScope = new MethodScope();
    }

    internal async Task<JobInstance> AddJobInstanceAsync(
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      Guid planId,
      JobInstance jobInstance)
    {
      JobInstance jobInstance1;
      using (new MethodScope(requestContext, nameof (TaskHub), nameof (AddJobInstanceAsync)))
      {
        ArgumentUtility.CheckForEmptyGuid(scopeIdentifier, nameof (scopeIdentifier));
        ArgumentUtility.CheckForEmptyGuid(planId, nameof (planId));
        ArgumentUtility.CheckStringForNullOrEmpty(jobInstance?.Identifier, "jobInstance.Identifier");
        this.EnsurePlanData(requestContext, scopeIdentifier, planId);
        List<VariableSecret> secrets = new List<VariableSecret>();
        Dictionary<string, string> source = new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        foreach (Variable variable in jobInstance.Definition.Variables.OfType<Variable>().Where<Variable>((Func<Variable, bool>) (x => x.Value != null && x.Secret)))
        {
          source[variable.Name] = variable.Value;
          variable.Value = (string) null;
        }
        secrets.AddRange(source.Select<KeyValuePair<string, string>, VariableSecret>((Func<KeyValuePair<string, string>, VariableSecret>) (x => new VariableSecret()
        {
          Name = x.Key,
          Value = x.Value
        })));
        if (secrets.Count > 0)
          new PlanSecretStore(requestContext, planId).SetValues((IList<VariableSecret>) secrets);
        JobInstance toSerialize = jobInstance;
        IMutableDictionaryCacheContainer<string, string> jobCache = this.GetJobInstanceRedisCache(requestContext);
        string jobInstanceName = PipelineUtilities.GetJobInstanceName(jobInstance.Identifier, jobInstance.Attempt);
        using (TaskTrackingComponent trackingComponent = requestContext.CreateComponent<TaskTrackingComponent>(this.DataspaceCategory))
        {
          if (trackingComponent.SupportStoreJobInstance)
            toSerialize = await trackingComponent.AddPlanContextAsync<JobInstance>(scopeIdentifier, planId, jobInstanceName, jobInstance);
        }
        if (jobCache != null)
          jobCache.Set(requestContext, (IDictionary<string, string>) new Dictionary<string, string>()
          {
            {
              string.Format("{0}.{1}", (object) planId, (object) jobInstanceName.ToLowerInvariant()),
              JsonUtility.ToString((object) toSerialize)
            }
          });
        jobInstance1 = toSerialize;
      }
      return jobInstance1;
    }

    internal async Task<JobInstance> UpdateJobInstanceAsync(
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      Guid planId,
      JobInstance jobInstance)
    {
      JobInstance jobInstance1;
      using (new MethodScope(requestContext, nameof (TaskHub), nameof (UpdateJobInstanceAsync)))
      {
        ArgumentUtility.CheckForEmptyGuid(scopeIdentifier, nameof (scopeIdentifier));
        ArgumentUtility.CheckForEmptyGuid(planId, nameof (planId));
        ArgumentUtility.CheckStringForNullOrEmpty(jobInstance?.Identifier, "jobInstance.Identifier");
        this.EnsurePlanData(requestContext, scopeIdentifier, planId);
        List<VariableSecret> secrets = new List<VariableSecret>();
        Dictionary<string, string> source = new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        foreach (Variable variable in jobInstance.Definition.Variables.OfType<Variable>().Where<Variable>((Func<Variable, bool>) (x => x.Value != null && x.Secret)))
        {
          source[variable.Name] = variable.Value;
          variable.Value = (string) null;
        }
        secrets.AddRange(source.Select<KeyValuePair<string, string>, VariableSecret>((Func<KeyValuePair<string, string>, VariableSecret>) (x => new VariableSecret()
        {
          Name = x.Key,
          Value = x.Value
        })));
        if (secrets.Count > 0)
          new PlanSecretStore(requestContext, planId).SetValues((IList<VariableSecret>) secrets);
        JobInstance toSerialize = jobInstance;
        IMutableDictionaryCacheContainer<string, string> jobCache = this.GetJobInstanceRedisCache(requestContext);
        string jobInstanceName = PipelineUtilities.GetJobInstanceName(jobInstance.Identifier, jobInstance.Attempt);
        using (TaskTrackingComponent trackingComponent = requestContext.CreateComponent<TaskTrackingComponent>(this.DataspaceCategory))
        {
          if (trackingComponent.SupportStoreJobInstance)
            toSerialize = await trackingComponent.UpdatePlanContextAsync<JobInstance>(scopeIdentifier, planId, jobInstanceName, jobInstance);
        }
        if (jobCache != null)
          jobCache.Set(requestContext, (IDictionary<string, string>) new Dictionary<string, string>()
          {
            {
              string.Format("{0}.{1}", (object) planId, (object) jobInstanceName.ToLowerInvariant()),
              JsonUtility.ToString((object) toSerialize)
            }
          });
        jobInstance1 = toSerialize;
      }
      return jobInstance1;
    }

    private IMutableDictionaryCacheContainer<string, string> GetJobInstanceRedisCache(
      IVssRequestContext requestContext)
    {
      IRedisCacheService service = requestContext.GetService<IRedisCacheService>();
      if (!service.IsEnabled(requestContext))
        return (IMutableDictionaryCacheContainer<string, string>) null;
      requestContext.GetService<IVssRegistryService>();
      return service.GetVolatileDictionaryContainer<string, string, TaskHub.OrchestrationJobSecurityToken>(requestContext, new Guid("4de68b3a-95b4-421a-927c-ecdfe1b1028b"));
    }

    internal async Task<ExpandPhaseResult> ExpandPhaseAsync(
      IVssRequestContext requestContext,
      Guid scopeId,
      Guid planId,
      string stageName,
      int stageAttempt,
      string phaseName,
      int phaseAttempt,
      IDictionary<string, PhaseExecutionState> dependencies,
      IList<string> configurations = null)
    {
      TaskHub taskHub1 = this;
      ExpandPhaseResult expandPhaseResult;
      using (new MethodScope(requestContext, nameof (TaskHub), nameof (ExpandPhaseAsync)))
      {
        using (new TraceWatch(requestContext, 10015528, TraceLevel.Error, TimeSpan.FromSeconds(5.0), "DistributedTask", nameof (TaskHub), "Expanding phase {0} for plan {1} in project {2} (Threshold 5s)", new object[3]
        {
          (object) phaseName,
          (object) planId,
          (object) scopeId
        }))
        {
          TaskOrchestrationPlan plan = await taskHub1.GetPlanAsync(requestContext, scopeId, planId);
          PipelineProcess pipeline = plan.GetProcess<PipelineProcess>();
          if (stageName == null)
            stageName = PipelineConstants.DefaultJobName;
          Stage stage = pipeline.Stages.SingleOrDefault<Stage>((Func<Stage, bool>) (x => x.Name.Equals(stageName, StringComparison.OrdinalIgnoreCase)));
          Stage stage1 = stage;
          PhaseNode phase = stage1 != null ? stage1.Phases.FirstOrDefault<PhaseNode>((Func<PhaseNode, bool>) (x => x.Name.Equals(phaseName, StringComparison.OrdinalIgnoreCase))) : (PhaseNode) null;
          if (phase == null)
            throw new ArgumentException(TaskResources.PhaseNotFound((object) phaseName, (object) planId));
          DateTime nowUtc = DateTime.UtcNow;
          TaskHub.PipelineTraceWriter trace = new TaskHub.PipelineTraceWriter();
          StageInstance stageInstance = new StageInstance(stage, stageAttempt);
          PhaseInstance phaseInstance = new PhaseInstance(phase, phaseAttempt);
          IDictionary<string, PhaseExecutionState> source = dependencies;
          Dictionary<string, PhaseInstance> dictionary = source != null ? source.ToDictionary<KeyValuePair<string, PhaseExecutionState>, string, PhaseInstance>((Func<KeyValuePair<string, PhaseExecutionState>, string>) (x => x.Key), (Func<KeyValuePair<string, PhaseExecutionState>, PhaseInstance>) (x => x.Value.ToInstance()), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) : (Dictionary<string, PhaseInstance>) null;
          PhaseExecutionContext executionContext = await taskHub1.CreatePhaseExecutionContextAsync(requestContext, plan, stageInstance, phaseInstance, (IPipelineTraceWriter) trace, dependencies: (IDictionary<string, PhaseInstance>) dictionary, includeOutputs: true);
          ExpandPhaseResult expansionResult = (ExpandPhaseResult) null;
          IList<Microsoft.TeamFoundation.DistributedTask.WebApi.Issue> expansionIssues = (IList<Microsoft.TeamFoundation.DistributedTask.WebApi.Issue>) null;
          JobExpansionOptions options = (JobExpansionOptions) null;
          bool flag = requestContext.IsFeatureEnabled("DistributedTask.EnableInputsInjectionForTaskDecorators");
          IList<string> stringList = configurations;
          if ((stringList != null ? (stringList.Count > 0 ? 1 : 0) : 0) != 0)
            options = new JobExpansionOptions((ICollection<string>) configurations);
          Guid phaseId = executionContext.GetInstanceId();
          try
          {
            if (!(phase is Microsoft.TeamFoundation.DistributedTask.Pipelines.Phase phase1))
              throw new NotSupportedException("Unexpected phase type " + phase?.GetType().FullName);
            phase1.inputsInjectionFeatureFlagEnabled = flag;
            expansionResult = phase1.Expand(executionContext, options);
          }
          catch (Exception ex)
          {
            if (ex is MaxJobExpansionException)
              requestContext.TraceError(10016104, nameof (TaskHub), string.Format("Phase '{0}' expansion exceed limitation. Plan: {1} Definition: {2} Owner: {3}", (object) phase.Name, (object) plan.PlanId, (object) plan.Definition?.Id, (object) plan.Owner?.Id));
            string str = ex is AggregateException aggregateException ? aggregateException.Flatten().Message : ex.Message;
            trace.Info("##[error]" + str);
            expansionIssues = (IList<Microsoft.TeamFoundation.DistributedTask.WebApi.Issue>) new List<Microsoft.TeamFoundation.DistributedTask.WebApi.Issue>()
            {
              new Microsoft.TeamFoundation.DistributedTask.WebApi.Issue() { Type = Microsoft.TeamFoundation.DistributedTask.WebApi.IssueType.Error, Message = str }
            };
            throw;
          }
          finally
          {
            if (trace.GetWarnings().Count > 0)
            {
              if (expansionIssues == null)
                expansionIssues = (IList<Microsoft.TeamFoundation.DistributedTask.WebApi.Issue>) new List<Microsoft.TeamFoundation.DistributedTask.WebApi.Issue>();
              foreach (string warning in trace.GetWarnings())
                expansionIssues.Add(new Microsoft.TeamFoundation.DistributedTask.WebApi.Issue()
                {
                  Type = Microsoft.TeamFoundation.DistributedTask.WebApi.IssueType.Warning,
                  Message = warning
                });
            }
            string phaseExpandTrace = trace.GetInfo();
            if (!string.IsNullOrEmpty(phaseExpandTrace))
            {
              TaskLog taskLog = await taskHub1.CreateLogAsync(requestContext, scopeId, planId, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "logs\\{0:D}", (object) phaseId));
              using (MemoryStream ms = new MemoryStream())
              {
                using (StreamWriter sw = new StreamWriter((Stream) ms, Encoding.UTF8))
                {
                  sw.WriteLine(nowUtc.ToString("O") + " ##[section]Starting: Prepare job " + phaseName);
                  sw.Write(phaseExpandTrace);
                  sw.WriteLine(DateTime.UtcNow.ToString("O") + " ##[section]Finishing: Prepare job " + phaseName);
                  sw.Flush();
                  ms.Seek(0L, SeekOrigin.Begin);
                  taskLog = await taskHub1.AppendLogAsync(requestContext, scopeId, planId, taskLog.Id, (Stream) ms);
                }
              }
              TaskHub taskHub2 = taskHub1;
              IVssRequestContext requestContext1 = requestContext;
              Guid scopeIdentifier = scopeId;
              Guid planId1 = planId;
              Guid timelineId = planId;
              Guid recordId = phaseId;
              TaskLogReference taskLogReference = (TaskLogReference) taskLog;
              IEnumerable<Microsoft.TeamFoundation.DistributedTask.WebApi.Issue> issues1 = (IEnumerable<Microsoft.TeamFoundation.DistributedTask.WebApi.Issue>) expansionIssues;
              Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecordState? state = new Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecordState?();
              DateTime? startTime = new DateTime?();
              DateTime? finishTime = new DateTime?();
              int? percentComplete = new int?();
              Microsoft.TeamFoundation.DistributedTask.WebApi.TaskResult? result = new Microsoft.TeamFoundation.DistributedTask.WebApi.TaskResult?();
              TaskLogReference log = taskLogReference;
              IEnumerable<Microsoft.TeamFoundation.DistributedTask.WebApi.Issue> issues2 = issues1;
              int? queueId = new int?();
              await taskHub2.UpdateTimelineRecordAsync(requestContext1, scopeIdentifier, planId1, timelineId, recordId, state, startTime, finishTime, percentComplete: percentComplete, result: result, log: log, issues: issues2, queueId: queueId);
            }
            phaseExpandTrace = (string) null;
          }
          if (expansionResult.Jobs.Count > 0)
          {
            int num = 1;
            List<Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord> jobRecords = new List<Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord>();
            List<TaskReferenceData> taskReferences = new List<TaskReferenceData>();
            List<JobInstance> jobInstances = new List<JobInstance>();
            Dictionary<string, string> gitHubChecksDisplaynames = new Dictionary<string, string>();
            Dictionary<string, string> jobIds = new Dictionary<string, string>();
            foreach (JobInstance job in (IEnumerable<JobInstance>) expansionResult.Jobs)
            {
              taskReferences.AddRange((IEnumerable<TaskReferenceData>) TaskHub.GetTaskReferences(requestContext, (IEnumerable<JobStep>) job.Definition.Steps));
              Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord timelineRecord = new Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord()
              {
                Attempt = job.Attempt,
                Id = executionContext.IdGenerator.GetJobInstanceId(stageName, phaseName, job.Name, job.Attempt),
                Identifier = executionContext.IdGenerator.GetJobIdentifier(stageInstance?.Name, phaseInstance.Name, job.Name),
                Name = job.Definition.DisplayName,
                Order = new int?(num++),
                ParentId = new Guid?(phaseId),
                RefName = job.Name,
                RecordType = "Job"
              };
              if (job.Definition.Target is AgentQueueTarget target)
              {
                timelineRecord.QueueId = target?.Queue?.Id;
                timelineRecord.AgentSpecification = target?.AgentSpecification;
              }
              jobRecords.Add(timelineRecord);
              JobInstance jobInstance = await taskHub1.AddJobInstanceAsync(requestContext, scopeId, planId, job);
              jobInstances.Add(job);
              gitHubChecksDisplaynames.Add(job.Identifier, Microsoft.TeamFoundation.DistributedTask.Pipelines.Phase.GenerateDisplayName(stage, job: job.Definition));
              jobIds.Add(job.Identifier, executionContext.IdGenerator.GetJobInstanceId(stageName, phaseName, job.Name, job.Attempt).ToString());
            }
            List<Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineAttempt> list = jobRecords.Select<Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord, Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineAttempt>((Func<Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord, Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineAttempt>) (x => new Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineAttempt()
            {
              Identifier = x.Identifier,
              Attempt = x.Attempt,
              RecordId = x.Id
            })).ToList<Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineAttempt>();
            Microsoft.VisualStudio.Services.Identity.Identity userIdentity = requestContext.GetUserIdentity();
            Microsoft.TeamFoundation.DistributedTask.WebApi.Timeline timeline;
            using (TaskTrackingComponent trackingComponent = requestContext.CreateComponent<TaskTrackingComponent>(taskHub1.DataspaceCategory))
              timeline = await trackingComponent.AddJobsAsync(scopeId, planId, userIdentity.Id, (IList<TaskOrchestrationJob>) Array.Empty<TaskOrchestrationJob>(), (IList<TaskReferenceData>) taskReferences, (IList<Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord>) jobRecords, (IEnumerable<Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineAttempt>) list);
            if (timeline != null)
              taskHub1.Extension.TimelineRecordsUpdated(requestContext, (Microsoft.TeamFoundation.DistributedTask.WebApi.TaskOrchestrationPlanReference) plan, (Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineReference) timeline, (IEnumerable<Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord>) timeline.Records);
            if (jobInstances.Count > 0)
            {
              bool isSingleJobPipeline = taskHub1.IsSingleJobPipeline(pipeline);
              await taskHub1.Extension.JobsCreatedAsync(requestContext, executionContext, (IReadOnlyList<JobInstance>) jobInstances, plan, (IDictionary<string, string>) gitHubChecksDisplaynames, (IDictionary<string, string>) jobIds, timeline, isSingleJobPipeline);
            }
            jobRecords = (List<Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord>) null;
            taskReferences = (List<TaskReferenceData>) null;
            jobInstances = (List<JobInstance>) null;
            gitHubChecksDisplaynames = (Dictionary<string, string>) null;
            jobIds = (Dictionary<string, string>) null;
          }
          if (phase is Microsoft.TeamFoundation.DistributedTask.Pipelines.Phase)
          {
            using (new TraceWatch(requestContext, 10015569, TraceLevel.Error, TimeSpan.FromSeconds(2.0), "DistributedTask", nameof (TaskHub), "Publish TaskHub PhaseStarted Telemetry for phase {0} for plan {1} in project {2} (Threshold 2s)", new object[3]
            {
              (object) phaseName,
              (object) planId,
              (object) scopeId
            }))
              TelemetryFactory.GetLogger(requestContext).PublishTaskHubPhaseStartedTelemetry(requestContext, plan, phase as Microsoft.TeamFoundation.DistributedTask.Pipelines.Phase, expansionResult);
          }
          using (new TraceWatch(requestContext, 10015570, TraceLevel.Error, TimeSpan.FromSeconds(2.0), "DistributedTask", nameof (TaskHub), "Save artifact traceability data for jobs in phase {0} for plan {1} in project {2} (Threshold 2s)", new object[3]
          {
            (object) phaseName,
            (object) planId,
            (object) scopeId
          }))
          {
            foreach (JobInstance job in (IEnumerable<JobInstance>) expansionResult.Jobs)
              taskHub1.SaveArtifactTraceabilityDataForJob(requestContext, executionContext, job, plan);
          }
          expandPhaseResult = expansionResult;
        }
      }
      return expandPhaseResult;
    }

    internal void SaveArtifactTraceabilityDataForJob(
      IVssRequestContext requestContext,
      PhaseExecutionContext executionContext,
      JobInstance jobInstance,
      TaskOrchestrationPlan plan)
    {
      if (!requestContext.IsFeatureEnabled("DistributedTask.EnableArtifactTraceability"))
        return;
      string phaseOrchestrationId = PipelineUtilities.GetPhaseOrchestrationId(executionContext, plan.PlanId);
      requestContext.GetService<IDistributedTaskArtifactTraceabilityService>().SaveArtifactTraceabilityDataForJob(requestContext, executionContext?.ResourceStore, plan, jobInstance?.Definition, phaseOrchestrationId);
    }

    private async Task<Microsoft.TeamFoundation.DistributedTask.Pipelines.ServiceEndpointReference> EnsureJobReferencesServiceConnection(
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      Guid planId,
      Guid jobId,
      Guid serviceConnectionId)
    {
      TaskHub hub = this;
      using (PerformanceTimer timer = PerformanceTimer.StartMeasure(requestContext, "TaskHub.EnsureJobReferencesServiceConnection"))
      {
        Microsoft.TeamFoundation.DistributedTask.WebApi.TaskOrchestrationPlanReference planReference = hub.EnsurePlanData(requestContext, scopeIdentifier, planId);
        ITimelineRecordReference timelineRecordReference = await hub.EnsureJobIsRunning(requestContext, planReference, jobId);
        if (planReference.ProcessType == OrchestrationProcessType.Container && requestContext.IsFeatureEnabled("DistributedTask.ExtractServiceEndpointsFromTasksForIdTokenGeneration"))
        {
          GetTaskOrchestrationJobResult jobAsync = await hub.GetJobAsync(requestContext, scopeIdentifier, planId, jobId);
          JobEnvironment jobEnvironment = hub.CreateJobEnvironment(requestContext, scopeIdentifier, planId, jobAsync.Plan, jobAsync.Job);
          Microsoft.TeamFoundation.DistributedTask.WebApi.ServiceEndpoint serviceEndpoint = (await hub.GetReferencedServiceEndpointsAsync(requestContext, scopeIdentifier, jobEnvironment, (IList<TaskInstance>) jobAsync.Job.Tasks)).FirstOrDefault<Microsoft.TeamFoundation.DistributedTask.WebApi.ServiceEndpoint>((Func<Microsoft.TeamFoundation.DistributedTask.WebApi.ServiceEndpoint, bool>) (e => e.Id == serviceConnectionId));
          if (serviceEndpoint == null)
          {
            requestContext.TraceWarning(10016187, nameof (TaskHub), "Container plan '{0}' was not authorized to access endpoint '{1}'", (object) planReference.PlanId, (object) serviceConnectionId.ToString("D"));
            throw new EndpointNotFoundException(TaskResources.EndpointNotFound((object) serviceConnectionId));
          }
          timer.AddProperty("FullResolution", (object) false);
          Microsoft.TeamFoundation.DistributedTask.Pipelines.ServiceEndpointReference endpointReference = new Microsoft.TeamFoundation.DistributedTask.Pipelines.ServiceEndpointReference();
          endpointReference.Name = (ExpressionValue<string>) serviceEndpoint.Name;
          endpointReference.Id = serviceEndpoint.Id;
          return endpointReference;
        }
        if (!requestContext.IsFeatureEnabled("DistributedTask.RestrictServiceEndpointsToJobForIdTokenGeneration"))
        {
          Microsoft.TeamFoundation.DistributedTask.Pipelines.ServiceEndpointReference endpointReference = (await hub.GetPipelineEnvironmentAsync(requestContext, planReference)).Resources.Endpoints.Where<Microsoft.TeamFoundation.DistributedTask.Pipelines.ServiceEndpointReference>((Func<Microsoft.TeamFoundation.DistributedTask.Pipelines.ServiceEndpointReference, bool>) (x => x.Id == serviceConnectionId)).FirstOrDefault<Microsoft.TeamFoundation.DistributedTask.Pipelines.ServiceEndpointReference>();
          if (endpointReference != null)
          {
            timer.AddProperty("FullResolution", (object) false);
            return endpointReference;
          }
        }
        timer.AddProperty("FullResolution", (object) true);
        string nodeId = timelineRecordReference.Id.ToString();
        string nodeInstanceName = timelineRecordReference.Identifier;
        Guid result;
        Microsoft.TeamFoundation.DistributedTask.Pipelines.ServiceEndpointReference endpointReference1 = (await hub.GetReferencedResourcesAsync(requestContext, planReference.ScopeIdentifier, planReference.PlanId, nodeId, nodeInstanceName)).Where<ResourceInfo>((Func<ResourceInfo, bool>) (x => x.TypeName == typeof (Microsoft.TeamFoundation.DistributedTask.Pipelines.ServiceEndpointReference).FullName)).Select<ResourceInfo, Guid>((Func<ResourceInfo, Guid>) (x => !Guid.TryParse(x.Id, out result) ? Guid.Empty : result)).Where<Guid>((Func<Guid, bool>) (x => x == serviceConnectionId)).Select<Guid, Microsoft.TeamFoundation.DistributedTask.Pipelines.ServiceEndpointReference>((Func<Guid, Microsoft.TeamFoundation.DistributedTask.Pipelines.ServiceEndpointReference>) (x => new Microsoft.TeamFoundation.DistributedTask.Pipelines.ServiceEndpointReference()
        {
          Id = x
        })).FirstOrDefault<Microsoft.TeamFoundation.DistributedTask.Pipelines.ServiceEndpointReference>();
        if (endpointReference1 == null)
        {
          requestContext.TraceWarning(10016184, nameof (TaskHub), "Plan '{0}' was not authorized to access endpoint '{1}' or its node '{2}' did not reference it", (object) planReference.PlanId, (object) serviceConnectionId.ToString("D"), (object) nodeInstanceName);
          throw new EndpointNotFoundException(TaskResources.EndpointNotFound((object) serviceConnectionId));
        }
        requestContext.TraceInfo(10016185, nameof (TaskHub), "Node '{0}' of plan '{1}' was authorized to access endpoint {2} after just-in-time authorization resolution", (object) nodeInstanceName, (object) planReference.PlanId, (object) serviceConnectionId.ToString("D"));
        return endpointReference1;
      }
    }

    private async Task<Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord> GetTimelineRecordAsync(
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      Guid planId,
      Guid recordId)
    {
      Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord timelineRecordAsync;
      using (new MethodScope(requestContext, nameof (TaskHub), nameof (GetTimelineRecordAsync)))
      {
        using (TaskTrackingComponent ttc = this.CreateComponent<TaskTrackingComponent>(requestContext))
          timelineRecordAsync = (await ttc.GetTimelineRecordsAsync(scopeIdentifier, planId, Guid.Empty, (IEnumerable<Guid>) new Guid[1]
          {
            recordId
          })).FirstOrDefault<Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord>();
      }
      return timelineRecordAsync;
    }

    private async Task<ITimelineRecordReference> GetTimelineRecordReferenceAsync(
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      Guid planId,
      Guid recordId)
    {
      ITimelineRecordReference recordReferenceAsync;
      using (new MethodScope(requestContext, nameof (TaskHub), nameof (GetTimelineRecordReferenceAsync)))
      {
        using (TaskTrackingComponent ttc = this.CreateComponent<TaskTrackingComponent>(requestContext))
          recordReferenceAsync = (ITimelineRecordReference) await ttc.GetTimelineRecordReferenceAsync(scopeIdentifier, planId, recordId);
      }
      return recordReferenceAsync;
    }

    private PlanEnvironment GetSafePlanEnvironment(PlanEnvironment environment)
    {
      if (environment != null)
      {
        foreach (MaskHint maskHint in environment.MaskHints)
        {
          if (environment.Variables.ContainsKey(maskHint.Value))
            environment.Variables[maskHint.Value] = (string) null;
        }
      }
      return environment;
    }

    private TaskOrchestrationPlan GetSafePlan(TaskOrchestrationPlan plan)
    {
      if (plan?.Environment != null)
        plan.Environment = this.GetSafePlanEnvironment(plan.Environment);
      return plan;
    }

    private TaskHub.JobAuthorizationResult GetJobAuthorization(
      IVssRequestContext requestContext,
      int jobTimeoutInMinutes,
      int jobCancelTimeoutInMinutes,
      Microsoft.VisualStudio.Services.Identity.Identity identity,
      string name,
      string scope = null,
      string pipelinePhaseIdentifier = null,
      IList<RepositoryResource> localRepositories = null,
      IDictionary<string, string> additionalClaims = null)
    {
      using (new MethodScope(requestContext, nameof (TaskHub), nameof (GetJobAuthorization)))
      {
        TimeSpan tokenDuration = TaskConstants.JobAccessTokenDuration;
        if (jobTimeoutInMinutes > 0)
          tokenDuration = TimeSpan.FromMinutes((double) (jobTimeoutInMinutes + jobCancelTimeoutInMinutes + 5));
        Guid guid = Guid.Empty;
        Microsoft.TeamFoundation.DistributedTask.WebApi.ServiceEndpoint serviceEndpoint = (Microsoft.TeamFoundation.DistributedTask.WebApi.ServiceEndpoint) null;
        IDictionary<string, string> customClaims = additionalClaims ?? (IDictionary<string, string>) new Dictionary<string, string>();
        if (requestContext.IsFeatureEnabled("DistributedTask.EnablePipelinePhaseIdentifierClaim"))
          customClaims.Add("ppid", pipelinePhaseIdentifier);
        if (requestContext.OrchestrationId != null)
          customClaims.Add("orchid", requestContext.OrchestrationId);
        if (localRepositories != null)
        {
          int num = requestContext.GetService<IVssRegistryService>().GetValue<int>(requestContext, (RegistryQuery) RegistryKeys.MaxReferencedReposRegistrySettingsPath, false, 20);
          if (localRepositories.Count<RepositoryResource>() > num)
            throw new DistributedTaskException(TaskResources.OverScopedRepositoryLimit((object) num));
          IEnumerable<string> values = localRepositories.Select<RepositoryResource, string>((Func<RepositoryResource, string>) (x => x.Id));
          customClaims.Add("repoIds", string.Join(",", values));
        }
        SessionToken selfDescribingJwt = SessionTokenGenerator.GenerateSelfDescribingJwt(requestContext, tokenDuration, identity, scope, customClaims);
        if (selfDescribingJwt != null)
        {
          Uri vssClientUrl = TaskHub.GetVssClientUrl(requestContext, Guid.Empty);
          serviceEndpoint = new Microsoft.TeamFoundation.DistributedTask.WebApi.ServiceEndpoint()
          {
            Name = name,
            Url = vssClientUrl,
            Authorization = new Microsoft.TeamFoundation.DistributedTask.WebApi.EndpointAuthorization()
            {
              Scheme = "OAuth",
              Parameters = {
                {
                  "AccessToken",
                  selfDescribingJwt.Token
                }
              }
            },
            Data = {
              {
                "ServerId",
                requestContext.ServiceHost.InstanceId.ToString("D")
              },
              {
                "ServerName",
                requestContext.ServiceHost.Name
              }
            }
          };
          guid = selfDescribingJwt.AuthorizationId;
        }
        return new TaskHub.JobAuthorizationResult()
        {
          AuthorizationId = guid,
          Endpoint = serviceEndpoint
        };
      }
    }

    private Uri GetPipelineUri(Guid scopeIdentifier, Guid planId) => new Uri(string.Format("pipelines://{0}/{1}/{2}", (object) this.Extension.HubName, (object) scopeIdentifier, (object) planId), UriKind.Absolute);

    private object GetPlanInput(
      IVssRequestContext requestContext,
      int? poolId,
      TaskOrchestrationPlan plan,
      IList<StageAttempt> attempts,
      string taskHub,
      out string orchestrationName,
      out string orchestrationVersion)
    {
      orchestrationName = this.GetPlanOrchestrationName(plan);
      switch (plan.Process.ProcessType)
      {
        case OrchestrationProcessType.Container:
          if (!poolId.HasValue)
          {
            poolId = this.Extension.GetTargetPoolForPlan(requestContext, plan);
            if (!poolId.HasValue)
              throw new UnableToDetermineTargetPoolException(TaskResources.UnableToDetermineTargetPool((object) plan.PlanId));
          }
          orchestrationVersion = plan.Version.ToString("n1", (IFormatProvider) NumberFormatInfo.InvariantInfo);
          return (object) new RunPlanInput3()
          {
            PoolId = poolId.Value,
            ScopeId = plan.ScopeIdentifier,
            PlanId = plan.PlanId,
            Implementation = plan.GetProcess<TaskOrchestrationContainer>()
          };
        case OrchestrationProcessType.Pipeline:
          orchestrationVersion = plan.Version < 3 ? "2.0" : "3.0";
          int num = requestContext.GetService<IVssRegistryService>().GetValue<int>(requestContext, (RegistryQuery) string.Format("/Service/Orchestration/Settings/ActivityDispatcher/{0}/CountShards", (object) taskHub), true, 1);
          PipelineActivityShardKey activityShardKey = new PipelineActivityShardKey()
          {
            ScopeId = plan.ScopeIdentifier,
            DefinitionId = plan.Definition.Id,
            OwnerId = plan.Owner.Id
          };
          RunPipelineInput planInput = new RunPipelineInput();
          planInput.ActivityDispatcherShardsCount = num;
          planInput.ScopeId = plan.ScopeIdentifier;
          planInput.PlanId = plan.PlanId;
          planInput.PlanVersion = plan.Version;
          planInput.Pipeline = attempts == null || attempts.Count <= 0 ? new PipelineExecutionState(plan.GetProcess<PipelineProcess>()) : new PipelineExecutionState(attempts);
          planInput.ShardKey = activityShardKey;
          return (object) planInput;
        default:
          throw new NotSupportedException();
      }
    }

    private string GetPlanOrchestrationName(TaskOrchestrationPlan plan)
    {
      switch (plan.Process.ProcessType)
      {
        case OrchestrationProcessType.Container:
          return this.Extension.OrchestrationName;
        case OrchestrationProcessType.Pipeline:
          return "RunPipeline";
        default:
          throw new NotSupportedException();
      }
    }

    private OrchestrationHubDescription EnsureOrchestrationHubExists(
      IVssRequestContext requestContext)
    {
      OrchestrationHubDescription orchestrationHubDescription = (OrchestrationHubDescription) null;
      OrchestrationService service = requestContext.GetService<OrchestrationService>();
      try
      {
        orchestrationHubDescription = service.GetHubDescription(requestContext, this.Name);
      }
      catch (OrchestrationHubNotFoundException ex)
      {
      }
      if (orchestrationHubDescription == null && this.Name != "DistributedTask")
        orchestrationHubDescription = service.GetHubDescription(requestContext, "DistributedTask");
      if (orchestrationHubDescription == null)
      {
        OrchestrationHubDescription description = new OrchestrationHubDescription()
        {
          CompressionSettings = new CompressionSettings()
          {
            Style = CompressionStyle.Threshold,
            ThresholdInBytes = 32768
          },
          HubName = this.Name,
          HubType = "DistributedTask"
        };
        try
        {
          orchestrationHubDescription = service.CreateHub(requestContext, description);
        }
        catch (OrchestrationHubExistsException ex)
        {
          requestContext.TraceInfo(10015510, nameof (TaskHub), "An orchestration hub already exists with name {0}", (object) description.HubName);
          orchestrationHubDescription = service.GetHubDescription(requestContext, description.HubName);
        }
      }
      return orchestrationHubDescription;
    }

    private Microsoft.TeamFoundation.DistributedTask.WebApi.TaskOrchestrationPlanReference GetPlanData(
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      Guid planId)
    {
      if (!requestContext.IsFeatureEnabled("DistributedTask.TaskOrchestrationPlanMemoryCache"))
        return requestContext.GetService<TaskOrchestrationPlanCache>().GetPlan(requestContext, scopeIdentifier, planId, new Func<IVssRequestContext, Guid, Guid, Microsoft.TeamFoundation.DistributedTask.WebApi.TaskOrchestrationPlanReference>(this.ReadFromDatabase));
      Microsoft.TeamFoundation.DistributedTask.WebApi.TaskOrchestrationPlanReference plan = requestContext.GetService<TaskOrchestrationPlanMemoryCache>().GetPlan(requestContext, scopeIdentifier, planId, new Func<IVssRequestContext, Guid, Guid, Microsoft.TeamFoundation.DistributedTask.WebApi.TaskOrchestrationPlanReference>(this.ReadFromDatabase));
      if (plan != null)
        return plan;
      requestContext.TraceError("TaskHub.GetPlanData", string.Format("Plan with id: {0} not found in scope: {1}. Stack: {2}", (object) planId, (object) scopeIdentifier, (object) new StackTrace()));
      return plan;
    }

    private Microsoft.TeamFoundation.DistributedTask.WebApi.TaskOrchestrationPlanReference ReadFromDatabase(
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      Guid planId)
    {
      using (TaskTrackingComponent component = requestContext.CreateComponent<TaskTrackingComponent>(this.DataspaceCategory))
        return component.GetPlanData(scopeIdentifier, planId);
    }

    private static void SetDefaultInputValues(
      IDictionary<string, string> taskInputs,
      TaskDefinition definition)
    {
      foreach (TaskInputDefinition taskInputDefinition in definition.Inputs.Where<TaskInputDefinition>((Func<TaskInputDefinition, bool>) (x => x != null)))
      {
        string key = taskInputDefinition.Name?.Trim() ?? string.Empty;
        if (!string.IsNullOrEmpty(key) && !taskInputs.ContainsKey(key))
          taskInputs[key] = taskInputDefinition?.DefaultValue?.Trim() ?? string.Empty;
      }
    }

    private void SetSystemVariablesForServerTask(ServerTaskRequestMessage request)
    {
      request.Environment.Variables[WellKnownDistributedTaskVariables.CollectionUrl] = request.Environment.SystemConnection.Url.ToString();
      request.Environment.Variables[WellKnownDistributedTaskVariables.TaskInstanceId] = request.TaskInstance.InstanceId.ToString("D");
      request.Environment.Variables[WellKnownDistributedTaskVariables.TaskInstanceName] = request.TaskInstance.DisplayName;
      request.Environment.Variables[WellKnownDistributedTaskVariables.HubVersion] = this.Version.ToString();
    }

    private void SetSystemVariables(
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      IDictionary<string, VariableValue> variables)
    {
      ArgumentUtility.CheckForNull<IDictionary<string, VariableValue>>(variables, "environment");
      variables[WellKnownDistributedTaskVariables.System] = (VariableValue) this.Extension.HubName.ToLowerInvariant();
      variables[WellKnownDistributedTaskVariables.HostType] = (VariableValue) this.Extension.HubName.ToLowerInvariant();
      variables[WellKnownDistributedTaskVariables.CollectionId] = (VariableValue) requestContext.ServiceHost.InstanceId.ToString("D");
      variables[WellKnownDistributedTaskVariables.ServerType] = (VariableValue) (requestContext.ExecutionEnvironment.IsHostedDeployment ? "Hosted" : "OnPremises");
      if (scopeIdentifier != Guid.Empty)
      {
        variables[WellKnownDistributedTaskVariables.TeamProjectId] = (VariableValue) scopeIdentifier.ToString("D");
        string projectName = requestContext.GetService<IProjectService>().GetProjectName(requestContext, scopeIdentifier);
        variables[WellKnownDistributedTaskVariables.TeamProject] = (VariableValue) projectName;
      }
      CultureInfo culture = requestContext.ServiceHost.GetCulture(requestContext);
      variables[WellKnownDistributedTaskVariables.Culture] = (VariableValue) culture.Name;
      string absoluteUri1 = TaskHub.GetVssPublicUrl(requestContext, Microsoft.VisualStudio.Services.WebApi.ServiceInstanceTypes.TFS).AbsoluteUri;
      variables[WellKnownDistributedTaskVariables.TFCollectionUrl] = (VariableValue) absoluteUri1;
      variables[WellKnownDistributedTaskVariables.TaskDefinitionsUrl] = (VariableValue) absoluteUri1;
      string absoluteUri2 = TaskHub.GetVssPublicUrl(requestContext, Guid.Empty).AbsoluteUri;
      variables[WellKnownDistributedTaskVariables.CollectionUrl] = (VariableValue) absoluteUri2;
      if (requestContext.ExecutionEnvironment.IsHostedDeployment)
      {
        string str = TimeZoneInfo.ConvertTime(DateTimeOffset.Now, requestContext.GetService<ICollectionPreferencesService>().GetCollectionTimeZone(requestContext) ?? TimeZoneInfo.Local).ToString(ExpressionConstants.DateTimeFormat, (IFormatProvider) CultureInfo.InvariantCulture);
        variables[WellKnownDistributedTaskVariables.PipelineStartTime] = (VariableValue) str;
      }
      else
        variables[WellKnownDistributedTaskVariables.PipelineStartTime] = (VariableValue) DateTime.Now.ToString(ExpressionConstants.DateTimeFormat, (IFormatProvider) CultureInfo.InvariantCulture);
      this.Extension.SetAdditionalSystemVariables(requestContext, variables);
    }

    private Dictionary<Guid, Dictionary<string, VariableValue>> ExtractSecrets(
      IList<Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord> records)
    {
      Dictionary<Guid, Dictionary<string, VariableValue>> secrets = new Dictionary<Guid, Dictionary<string, VariableValue>>();
      if (records != null && records.Count > 0)
      {
        foreach (Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord record in (IEnumerable<Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord>) records)
        {
          if (!secrets.ContainsKey(record.Id))
            secrets[record.Id] = new Dictionary<string, VariableValue>();
          foreach (KeyValuePair<string, VariableValue> keyValuePair in record.Variables.Where<KeyValuePair<string, VariableValue>>((Func<KeyValuePair<string, VariableValue>, bool>) (v => v.Value.IsSecret)))
          {
            VariableValue variableValue = keyValuePair.Value.Clone();
            keyValuePair.Value.Value = (string) null;
            secrets[record.Id][keyValuePair.Key] = variableValue;
          }
        }
      }
      return secrets;
    }

    private void StoreSecrets(
      IVssRequestContext requestContext,
      Guid planId,
      Guid timelineId,
      Dictionary<Guid, Dictionary<string, VariableValue>> secrets)
    {
      using (new MethodScope(requestContext, nameof (TaskHub), nameof (StoreSecrets)))
      {
        if (secrets == null)
          return;
        List<VariableSecret> secrets1 = new List<VariableSecret>();
        foreach (KeyValuePair<Guid, Dictionary<string, VariableValue>> secret in secrets)
        {
          if (secret.Value != null && secret.Value.Any<KeyValuePair<string, VariableValue>>((Func<KeyValuePair<string, VariableValue>, bool>) (v => v.Value.Value != null)))
          {
            foreach (KeyValuePair<string, VariableValue> keyValuePair in secret.Value)
            {
              List<VariableSecret> variableSecretList = secrets1;
              OutputVariableSecret outputVariableSecret = new OutputVariableSecret();
              outputVariableSecret.TimelineId = timelineId;
              outputVariableSecret.RecordId = secret.Key;
              outputVariableSecret.Value = keyValuePair.Value.Value;
              outputVariableSecret.Name = keyValuePair.Key;
              variableSecretList.Add((VariableSecret) outputVariableSecret);
            }
          }
        }
        if (secrets1.Count <= 0)
          return;
        new PlanSecretStore(requestContext, planId).SetValues((IList<VariableSecret>) secrets1);
      }
    }

    private TaskHub.ProcessValidationResult ValidateProcess(
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      TaskOrchestrationOwner definition,
      Guid planId,
      IOrchestrationEnvironment environment,
      IOrchestrationProcess process,
      BuildOptions options)
    {
      switch (process)
      {
        case TaskOrchestrationContainer _:
          this.SetSystemVariables(requestContext, scopeIdentifier, environment.Variables);
          return this.ValidateContainerProcess(requestContext, scopeIdentifier, planId, environment as PlanEnvironment, process as TaskOrchestrationContainer);
        case PipelineProcess _:
          return this.ValidatePipelineProcess(requestContext, scopeIdentifier, definition != null ? definition.Id : 0, planId, environment as PipelineEnvironment, process as PipelineProcess, options);
        default:
          throw new NotSupportedException();
      }
    }

    private TaskHub.ProcessValidationResult ValidateContainerProcess(
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      Guid planId,
      PlanEnvironment environment,
      TaskOrchestrationContainer implementation)
    {
      if (TaskHub.ContainsRollbackAtSubContainers(implementation))
        throw new DistributedTaskException(TaskResources.UnsupportedRollbackContainers());
      if (!this.Extension.IsValidContainer(implementation))
        throw new DistributedTaskException(TaskResources.InvalidContainer((object) this.Extension.OrchestrationName));
      int num = 1;
      TaskHub.ProcessValidationResult validationResult = new TaskHub.ProcessValidationResult(planId, this.Version);
      Queue<TaskOrchestrationItem> orchestrationItemQueue = new Queue<TaskOrchestrationItem>((IEnumerable<TaskOrchestrationItem>) implementation.Children);
      while (orchestrationItemQueue.Count > 0)
      {
        TaskOrchestrationItem orchestrationItem = orchestrationItemQueue.Dequeue();
        TaskOrchestrationJob orchestrationJob1 = orchestrationItem as TaskOrchestrationJob;
        TaskOrchestrationContainer orchestrationContainer = orchestrationItem as TaskOrchestrationContainer;
        if (orchestrationJob1 != null)
        {
          TaskOrchestrationJob orchestrationJob2 = orchestrationJob1.Clone();
          foreach (TaskInstance task in orchestrationJob2.Tasks)
          {
            if (string.IsNullOrEmpty(task.RefName))
              task.RefName = Guid.NewGuid().ToString("N");
            task.RefName = NameValidation.Sanitize(task.RefName);
          }
          validationResult.ContainerJobs.Add(orchestrationJob2);
          validationResult.Timeline.Records.Add(new Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord()
          {
            Id = orchestrationJob2.InstanceId,
            Name = orchestrationJob2.Name,
            RefName = orchestrationJob2.RefName,
            RecordType = "Job",
            Order = new int?(num++)
          });
          validationResult.Tasks.AddRange((IEnumerable<TaskReferenceData>) TaskHub.GetTaskReferences(requestContext, (IEnumerable<TaskInstance>) orchestrationJob1.Tasks));
          orchestrationJob1.Tasks.Clear();
          orchestrationJob1.Variables.Clear();
        }
        else if (orchestrationContainer != null)
        {
          foreach (TaskOrchestrationItem child in orchestrationContainer.Children)
            orchestrationItemQueue.Enqueue(child);
        }
      }
      this.StoreSecrets(requestContext, planId, planId, (IOrchestrationEnvironment) environment, (Action<string>) (variable => environment.MaskHints.Add(new MaskHint()
      {
        Type = MaskType.Variable,
        Value = variable
      })));
      return validationResult;
    }

    private TaskHub.ProcessValidationResult ValidatePipelineProcess(
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      int definitionId,
      Guid planId,
      PipelineEnvironment environment,
      PipelineProcess pipeline,
      BuildOptions options)
    {
      ArgumentUtility.CheckForNull<PipelineEnvironment>(environment, nameof (environment));
      ArgumentUtility.CheckForNull<PipelineProcess>(pipeline, nameof (pipeline));
      int version = environment.Version;
      options.RollupStepDemands = true;
      TaskHub.ProcessValidationResult validationResult = new TaskHub.ProcessValidationResult(planId, this.PipelineVersion);
      int num = requestContext.IsFeatureEnabled("DistributedTask.EnableJustInTimeAuthorization") ? 1 : 0;
      if (num != 0)
        this.ClearSecurableResources(environment);
      PipelineBuilder builder = requestContext.GetService<IPipelineBuilderService>().GetBuilder(requestContext, scopeIdentifier, this.Extension.HubName, validationResult.Version, definitionId, planId, environment);
      IList<PipelineValidationError> errors = builder.Validate(pipeline, options);
      if (num != 0)
        validationResult.Resources = builder.ResourceStore.GetAuthorizedResources();
      if (errors.Count == 0)
        TaskHub.ValidateDeclaredResources(requestContext, scopeIdentifier, environment, builder.ResourceStore, errors, resolveVersion: options == null || options.ResolveResourceVersions);
      if (errors.Count > 0)
        throw new PipelineValidationException((IEnumerable<PipelineValidationError>) errors);
      if (!requestContext.IsFeatureEnabled("DistributedTask.EnableProviderPhase") && pipeline.Stages.Any<Stage>((Func<Stage, bool>) (s => s.Phases.Any<PhaseNode>((Func<PhaseNode, bool>) (p => p.Type == PhaseType.Provider)))))
        throw new NotSupportedException("Provider phase is not supported");
      PipelineIdGenerator idGenerator = new PipelineIdGenerator(validationResult.Version < 4);
      foreach (StageAttempt stageAttempt in (IEnumerable<StageAttempt>) new PipelineAttemptBuilder((IPipelineIdGenerator) idGenerator, pipeline, Array.Empty<Microsoft.TeamFoundation.DistributedTask.WebApi.Timeline>()).Initialize())
      {
        validationResult.Attempts.Add(new Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineAttempt()
        {
          Attempt = stageAttempt.Stage.Attempt,
          Identifier = stageAttempt.Stage.Name,
          RecordId = idGenerator.GetStageInstanceId(stageAttempt.Stage.Name, stageAttempt.Stage.Attempt)
        });
        validationResult.Timeline.Records.AddRange((IEnumerable<Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord>) stageAttempt.Timeline.Records);
      }
      this.StoreSecrets(requestContext, scopeIdentifier, planId, environment);
      return validationResult;
    }

    private void ClearSecurableResources(PipelineEnvironment environment)
    {
      PipelineResources resources = environment.Resources;
      resources.Endpoints.Clear();
      resources.Queues.Clear();
      resources.Pools.Clear();
      resources.VariableGroups.Clear();
      resources.Files.Clear();
      resources.Environments.Clear();
      resources.PersistedStages.Clear();
    }

    private static void ValidateDeclaredResources(
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      PipelineEnvironment environment,
      IResourceStore resources,
      IList<PipelineValidationError> errors,
      bool validateResource = true,
      bool resolveVersion = true)
    {
      foreach (PipelineResource pipeline in (IEnumerable<PipelineResource>) environment.Resources.Pipelines)
      {
        PipelineProvider pipelineProvider = new PipelineProvider();
        Microsoft.TeamFoundation.DistributedTask.WebApi.ServiceEndpoint endpoint = (Microsoft.TeamFoundation.DistributedTask.WebApi.ServiceEndpoint) null;
        if (pipeline.Endpoint != null)
          endpoint = resources.GetEndpoint(pipeline.Endpoint);
        try
        {
          if (validateResource)
            pipelineProvider.Validate(requestContext, scopeIdentifier, pipeline, endpoint);
          if (resolveVersion)
          {
            pipelineProvider.ResolveVersion(requestContext, scopeIdentifier, pipeline, endpoint);
            environment.UserVariables.AddRange<IVariable, IList<IVariable>>((IEnumerable<IVariable>) pipelineProvider.GetPipelineVariables(requestContext, pipeline));
          }
        }
        catch (Exception ex) when (ex is PipelineProviderException || ex is ResourceValidationException)
        {
          requestContext.TraceError(10016107, nameof (TaskHub), ex.Message);
          errors.Add(new PipelineValidationError(ex.GetType().Name, ex.Message));
        }
      }
      foreach (RepositoryResource repository in (IEnumerable<RepositoryResource>) environment.Resources.Repositories)
      {
        ISourceProvider sourceProvider = requestContext.GetService<ISourceProviderService>().GetSourceProvider(requestContext, repository.Type);
        Microsoft.TeamFoundation.DistributedTask.WebApi.ServiceEndpoint endpoint = (Microsoft.TeamFoundation.DistributedTask.WebApi.ServiceEndpoint) null;
        if (repository.Endpoint != null)
          endpoint = resources.GetEndpoint(repository.Endpoint);
        try
        {
          if (validateResource)
            sourceProvider.Validate(requestContext, scopeIdentifier, repository, endpoint);
          if (resolveVersion)
            sourceProvider.ResolveVersion(requestContext, scopeIdentifier, repository, endpoint);
        }
        catch (Exception ex) when (ex is Microsoft.TeamFoundation.DistributedTask.Pipelines.SourceProviders.ExternalSourceProviderException || ex is ResourceValidationException)
        {
          requestContext.TraceError(10016106, nameof (TaskHub), ex.Message);
          errors.Add(new PipelineValidationError(ex.GetType().Name, ex.Message));
        }
      }
    }

    private void StoreSecrets(
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      Guid planId,
      PipelineEnvironment environment)
    {
      List<VariableSecret> secrets = new List<VariableSecret>();
      if (environment.Version < 2)
      {
        foreach (KeyValuePair<string, VariableValue> keyValuePair in environment.Variables.Where<KeyValuePair<string, VariableValue>>((Func<KeyValuePair<string, VariableValue>, bool>) (x => x.Value != null && x.Value.Value != null && x.Value.IsSecret)))
        {
          secrets.Add(new VariableSecret()
          {
            Name = keyValuePair.Key,
            Value = keyValuePair.Value.Value
          });
          keyValuePair.Value.Value = (string) null;
        }
      }
      else
      {
        Dictionary<string, string> source = new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        foreach (Variable variable in environment.UserVariables.OfType<Variable>().Where<Variable>((Func<Variable, bool>) (x => x.Value != null && x.Secret)))
        {
          source[variable.Name] = variable.Value;
          variable.Value = (string) null;
        }
        foreach (KeyValuePair<string, VariableValue> keyValuePair in environment.SystemVariables.Where<KeyValuePair<string, VariableValue>>((Func<KeyValuePair<string, VariableValue>, bool>) (x =>
        {
          if (x.Value?.Value == null)
            return false;
          VariableValue variableValue = x.Value;
          return variableValue != null && variableValue.IsSecret;
        })))
        {
          source[keyValuePair.Key] = keyValuePair.Value.Value;
          keyValuePair.Value.Value = (string) null;
        }
        secrets.AddRange(source.Select<KeyValuePair<string, string>, VariableSecret>((Func<KeyValuePair<string, string>, VariableSecret>) (x => new VariableSecret()
        {
          Name = x.Key,
          Value = x.Value
        })));
      }
      if (secrets.Count <= 0)
        return;
      new PlanSecretStore(requestContext, planId).SetValues((IList<VariableSecret>) secrets);
    }

    private void StoreSecrets(
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      Guid planId,
      IOrchestrationEnvironment environment,
      Action<string> recordSecret)
    {
      List<VariableSecret> secrets = new List<VariableSecret>();
      foreach (KeyValuePair<string, VariableValue> keyValuePair in environment.Variables.Where<KeyValuePair<string, VariableValue>>((Func<KeyValuePair<string, VariableValue>, bool>) (x => x.Value != null && x.Value.Value != null && x.Value.IsSecret)))
      {
        secrets.Add(new VariableSecret()
        {
          Name = keyValuePair.Key,
          Value = keyValuePair.Value.Value
        });
        keyValuePair.Value.Value = (string) null;
        if (recordSecret != null)
          recordSecret(keyValuePair.Key);
      }
      if (secrets.Count <= 0)
        return;
      new PlanSecretStore(requestContext, planId).SetValues((IList<VariableSecret>) secrets);
    }

    private List<int> GetRestrictedNodeTaskVersions(IVssRequestContext requestContext)
    {
      List<int> nodeTaskVersions = new List<int>();
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      if (requestContext.IsFeatureEnabled("DistributedTask.Node6LockdownAllowed") && service.GetValue<bool>(requestContext, (RegistryQuery) "/Service/DistributedTask/Settings/DisableNode6Tasks", false))
        nodeTaskVersions.Add(6);
      return nodeTaskVersions;
    }

    private bool IsSingleJobPipeline(PipelineProcess pipeline)
    {
      if (pipeline == null || pipeline.Stages == null)
        return true;
      if (pipeline.Stages.Count > 1)
        return false;
      Stage stage = pipeline.Stages.FirstOrDefault<Stage>();
      if (stage == null || stage.Phases == null)
        return true;
      if (stage.Phases.Count > 1)
        return false;
      PhaseNode phaseNode = stage.Phases.FirstOrDefault<PhaseNode>();
      if (phaseNode == null)
        return true;
      if (phaseNode.Target is AgentQueueTarget target)
      {
        ParallelExecutionOptions execution = target.Execution;
        if (execution == null || execution == null || execution.Matrix == (ExpressionValue<IDictionary<string, IDictionary<string, string>>>) null && (execution.MaxConcurrency == (ExpressionValue<int>) null || execution.MaxConcurrency == (ExpressionValue<int>) 1))
          return true;
      }
      return false;
    }

    private static string GetPhaseOrchestrationId(Guid planId, string stageName, string phaseName)
    {
      string phaseInstanceName = new PipelineIdGenerator().GetPhaseInstanceName(stageName, phaseName, 1);
      return string.Format("{0}.{1}", (object) planId, (object) phaseInstanceName.ToLowerInvariant());
    }

    internal static Guid FindAncestorId(IList<Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord> records)
    {
      if (records == null || records.Count == 0)
        return Guid.Empty;
      Guid empty = Guid.Empty;
      foreach (Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord record in (IEnumerable<Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord>) records)
      {
        Guid? parentId = record.ParentId;
        if (!parentId.HasValue)
          return Guid.Empty;
        if (empty == Guid.Empty)
        {
          parentId = record.ParentId;
          empty = parentId.Value;
        }
        else
        {
          Guid guid1 = empty;
          parentId = record.ParentId;
          Guid guid2 = parentId.Value;
          if (guid1 != guid2)
            return Guid.Empty;
        }
      }
      return empty;
    }

    private async Task<PipelineEnvironment> GetPipelineEnvironmentAsync(
      IVssRequestContext requestContext,
      Microsoft.TeamFoundation.DistributedTask.WebApi.TaskOrchestrationPlanReference planRef)
    {
      PipelineEnvironment environmentAsync;
      using (new MethodScope(requestContext, nameof (TaskHub), nameof (GetPipelineEnvironmentAsync)))
      {
        if (!this.Extension.HasReadPermission(requestContext, planRef.ScopeIdentifier, planRef.ArtifactUri))
          throw new TaskOrchestrationPlanNotFoundException(TaskResources.PlanNotFound((object) planRef.PlanId));
        using (TaskTrackingComponent trackingComponent = requestContext.CreateComponent<TaskTrackingComponent>(this.DataspaceCategory))
        {
          IOrchestrationEnvironment processEnvironment = (await trackingComponent.GetPlanAsync(planRef.ScopeIdentifier, planRef.PlanId)).ProcessEnvironment;
          environmentAsync = processEnvironment is PipelineEnvironment ? processEnvironment as PipelineEnvironment : throw new EnvironmentNotFoundException(processEnvironment.ProcessType.ToString());
        }
      }
      return environmentAsync;
    }

    internal sealed class OrchestrationJobSecurityToken
    {
    }

    private struct JobAuthorizationResult
    {
      public Guid AuthorizationId;
      public Microsoft.TeamFoundation.DistributedTask.WebApi.ServiceEndpoint Endpoint;
    }

    internal class PipelineTraceWriter : IPipelineTraceWriter, ITraceWriter
    {
      private int m_indent;
      private readonly StringBuilder m_info = new StringBuilder();
      private readonly List<string> m_warnings = new List<string>();

      public void Info(string message)
      {
        this.m_info.Append(DateTime.UtcNow.ToString("O") + " ");
        if (this.m_indent > 0)
          this.m_info.Append(' ', this.m_indent);
        this.m_info.AppendLine(message);
      }

      public void Verbose(string message)
      {
      }

      public void AddWarning(string message) => this.m_warnings.Add(message);

      public string GetInfo() => this.m_info.ToString();

      public List<string> GetWarnings() => this.m_warnings;

      public void EnterProperty(string propertyName)
      {
        this.Info(propertyName + ":");
        this.m_indent += 2;
      }

      public void LeaveProperty(string propertyName) => this.m_indent -= 2;
    }

    internal struct JobRequestData
    {
      public TaskOrchestrationPlan Plan { get; set; }

      public Guid AuthorizationId { get; set; }

      public TaskOrchestrationJob ContainerJob { get; set; }

      public JobEnvironment Environment { get; set; }

      public Job Job { get; set; }

      public JobExecutionContext Context { get; set; }

      public List<MaskHint> MaskHints { get; set; }

      public JobResources Resources { get; set; }
    }

    private class ProcessValidationResult
    {
      public ProcessValidationResult(Guid planId, int version)
      {
        this.Timeline = new Microsoft.TeamFoundation.DistributedTask.WebApi.Timeline(planId);
        this.ContainerJobs = new List<TaskOrchestrationJob>();
        this.Attempts = new List<Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineAttempt>();
        this.Tasks = new List<TaskReferenceData>();
        this.Version = version;
      }

      public List<TaskOrchestrationJob> ContainerJobs { get; }

      public List<TaskReferenceData> Tasks { get; }

      public Microsoft.TeamFoundation.DistributedTask.WebApi.Timeline Timeline { get; set; }

      public List<Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineAttempt> Attempts { get; }

      public int Version { get; }

      public PipelineResources Resources { get; set; }
    }
  }
}
