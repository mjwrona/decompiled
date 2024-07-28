// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.TaskHubExtension
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Pipelines;
using Microsoft.TeamFoundation.DistributedTask.Pipelines.Runtime;
using Microsoft.TeamFoundation.DistributedTask.Server;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Orchestration;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Globalization;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server
{
  [InheritedExport]
  public abstract class TaskHubExtension : IDisposable
  {
    private const string c_area = "DistributedTask";
    private const string c_layer = "SecurityExtension";

    public abstract string HubName { get; }

    public virtual string OrchestrationName => "RunPlan";

    public virtual string OrchestrationVersionKey => "HubVersion";

    public virtual int DefaultOrchestrationVersion => 5;

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    public virtual string GetJobName(
      IVssRequestContext requestContext,
      TaskOrchestrationPlanReference plan,
      Guid jobId)
    {
      return (string) null;
    }

    public virtual Task NewAttemptCreatedAsync(
      IVssRequestContext requestContext,
      TaskOrchestrationPlan plan,
      IList<StageAttempt> stages)
    {
      return Task.CompletedTask;
    }

    public virtual void PopulatePlanOwnerReferences(
      IVssRequestContext requestContext,
      TaskOrchestrationPlanReference planReference)
    {
    }

    public virtual IDictionary<string, object> GetJobTelemetryDetails(
      IVssRequestContext requestContext,
      TaskOrchestrationPlanReference plan,
      IDictionary<string, string> jobEnvironmentVariables,
      out string jobRequestingUser,
      out Guid jobRequesterUserId)
    {
      jobRequestingUser = string.Empty;
      jobRequesterUserId = Guid.Empty;
      return (IDictionary<string, object>) null;
    }

    public virtual Guid GetScopeIdentifier(IVssRequestContext requestContext, Uri artifactUri) => Guid.Empty;

    public virtual void JobCompleted(
      IVssRequestContext requestContext,
      TaskOrchestrationPlan plan,
      Guid jobId,
      TaskResult? result = null)
    {
    }

    public virtual Task JobCompletedAsync(
      IVssRequestContext requestContext,
      TaskOrchestrationPlan plan,
      TimelineRecord jobRecord,
      IReadOnlyList<TimelineRecord> taskRecords,
      string jobDisplayName,
      bool isSingleJobPipeline)
    {
      this.JobCompleted(requestContext, plan, jobRecord.Id, jobRecord.Result);
      return Task.CompletedTask;
    }

    public virtual void JobStarted(
      IVssRequestContext requestContext,
      TaskOrchestrationPlan plan,
      Guid jobId)
    {
    }

    public virtual Task JobStartedAsync(
      IVssRequestContext requestContext,
      TaskOrchestrationPlan plan,
      Guid jobId,
      TimelineRecord jobRecord,
      IReadOnlyList<TimelineRecord> taskRecords,
      string jobDisplayName)
    {
      this.JobStarted(requestContext, plan, jobId);
      return Task.CompletedTask;
    }

    public virtual void PlanCreated(IVssRequestContext requestContext, TaskOrchestrationPlan plan)
    {
    }

    public virtual void PlanStarted(IVssRequestContext requestContext, TaskOrchestrationPlan plan)
    {
    }

    public virtual Task PlanStartedAsync(
      IVssRequestContext requestContext,
      TaskOrchestrationPlan plan)
    {
      this.PlanStarted(requestContext, plan);
      return Task.CompletedTask;
    }

    public virtual void PlanCompleted(IVssRequestContext requestContext, TaskOrchestrationPlan plan)
    {
    }

    public virtual Task PlanCompletedAsync(
      IVssRequestContext requestContext,
      TaskOrchestrationPlan plan)
    {
      this.PlanCompleted(requestContext, plan);
      return Task.CompletedTask;
    }

    public virtual bool AlwaysRaisePlanEvents(IVssRequestContext requestContext) => true;

    public virtual void TimelineRecordsUpdated(
      IVssRequestContext requestContext,
      TaskOrchestrationPlanReference plan,
      TimelineReference timeline,
      IEnumerable<TimelineRecord> timelineRecords)
    {
    }

    public virtual Task TimelineRecordsUpdatedAsync(
      IVssRequestContext requestContext,
      TaskOrchestrationPlanReference plan,
      TimelineReference timeline,
      IEnumerable<TimelineRecord> timelineRecords)
    {
      this.TimelineRecordsUpdated(requestContext, plan, timeline, timelineRecords);
      return Task.CompletedTask;
    }

    public virtual void FeedReceived(
      IVssRequestContext requestContext,
      TaskOrchestrationPlanReference plan,
      Guid timelineId,
      Guid jobTimelineRecordId,
      Guid stepTimelineRecordId,
      IList<string> lines)
    {
    }

    public virtual Task FeedReceivedAsync(
      IVssRequestContext requestContext,
      TaskOrchestrationPlanReference plan,
      Guid timelineId,
      Guid jobTimelineRecordId,
      Guid stepTimelineRecordId,
      IList<string> lines)
    {
      this.FeedReceived(requestContext, plan, timelineId, jobTimelineRecordId, stepTimelineRecordId, lines);
      return Task.CompletedTask;
    }

    public virtual bool IsEnforceReferencedRepoScopedTokenEnabled(
      IVssRequestContext requestContext,
      Guid projectId)
    {
      return false;
    }

    public virtual bool AreExclusiveLocksSupported(IVssRequestContext requestContext) => false;

    public virtual Microsoft.VisualStudio.Services.Identity.Identity GetJobServiceIdentity(
      IVssRequestContext requestContext,
      TaskOrchestrationPlan plan,
      IDictionary<string, VariableValue> variables)
    {
      Guid result = Guid.Empty;
      VariableValue variableValue;
      if (variables != null && (!variables.TryGetValue("Job.AuthorizeAsId", out variableValue) || !Guid.TryParse(variableValue.Value, out result)))
        result = Guid.Empty;
      return result == Guid.Empty ? (Microsoft.VisualStudio.Services.Identity.Identity) null : requestContext.GetService<IdentityService>().GetIdentity(requestContext, result);
    }

    public virtual Uri GetArtifactLocation(
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      Uri artifactUri)
    {
      return artifactUri;
    }

    public virtual Task JobsCreatedAsync(
      IVssRequestContext requestContext,
      PhaseExecutionContext executionContext,
      IReadOnlyList<JobInstance> jobInstances,
      TaskOrchestrationPlan plan,
      IDictionary<string, string> jobDisplayNames,
      IDictionary<string, string> jobIds,
      Timeline timeline,
      bool isSingleJobPipeline)
    {
      return Task.CompletedTask;
    }

    public virtual void PrepareJob(
      IVssRequestContext requestContext,
      TaskOrchestrationPlan plan,
      IDictionary<string, VariableValue> variables,
      HashSet<MaskHint> maskHints,
      JobResources resources)
    {
    }

    public virtual Task PrepareJobAsync(
      IVssRequestContext requestContext,
      TaskOrchestrationPlan plan,
      IDictionary<string, VariableValue> variables,
      HashSet<MaskHint> maskHints,
      JobResources resources)
    {
      this.PrepareJob(requestContext, plan, variables, maskHints, resources);
      return Task.CompletedTask;
    }

    public virtual Task PreparePipelineJobAsync(
      IVssRequestContext requestContext,
      TaskOrchestrationPlan plan,
      IDictionary<string, VariableValue> variables,
      HashSet<MaskHint> maskHints,
      JobResources resources)
    {
      return Task.CompletedTask;
    }

    public virtual void SetPermissions(
      IVssRequestContext requestContext,
      TaskOrchestrationPlan plan,
      Microsoft.VisualStudio.Services.Identity.Identity serviceIdentity)
    {
    }

    public virtual bool IsValidContainer(TaskOrchestrationContainer container) => true;

    public virtual void AddTaskOrchestrations(
      IVssRequestContext requestContext,
      OrchestrationRuntime runtime,
      TaskHub taskHub)
    {
    }

    public virtual void UpdateSystemVariables(
      IVssRequestContext requestContext,
      IDictionary<string, VariableValue> variables)
    {
    }

    public virtual void SetAdditionalSystemVariables(
      IVssRequestContext requestContext,
      IDictionary<string, VariableValue> variables)
    {
    }

    public virtual int? GetTargetPoolForPlan(
      IVssRequestContext requestContext,
      TaskOrchestrationPlan plan)
    {
      return new int?();
    }

    public virtual Task<int?> GetLogLinePostFrequencyAsync(
      IVssRequestContext requestContext,
      TaskOrchestrationPlan plan)
    {
      return Task.FromResult<int?>(new int?());
    }

    internal void CheckDeletePermission(
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      Guid planId,
      Uri artifactUri)
    {
      if (!this.HasDeletePermission(requestContext, scopeIdentifier, artifactUri))
        throw new TaskOrchestrationPlanSecurityException(TaskResources.PlanSecurityDeleteError((object) (requestContext.GetUserIdentity()?.Id.ToString() ?? FrameworkResources.AnonymousPrincipalName()), (object) planId));
    }

    internal void CheckWritePermission(
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      Guid planId,
      Uri artifactUri)
    {
      if (!this.HasWritePermission(requestContext, scopeIdentifier, artifactUri))
        throw new TaskOrchestrationPlanSecurityException(TaskResources.PlanSecurityWriteError((object) (requestContext.GetUserIdentity()?.Id.ToString() ?? FrameworkResources.AnonymousPrincipalName()), (object) planId));
    }

    internal void CacheSecurityTokens(
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      IDictionary<Uri, string> tokens)
    {
      TaskHubExtension.SecurityTokenCacheService service = requestContext.GetService<TaskHubExtension.SecurityTokenCacheService>();
      foreach (KeyValuePair<Uri, string> token in (IEnumerable<KeyValuePair<Uri, string>>) tokens)
        service.Set(requestContext, scopeIdentifier, token.Key, token.Value);
    }

    internal virtual bool TryGetSecurityToken(
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      Uri artifactUri,
      out string securityToken)
    {
      using (new MethodScope(requestContext, "SecurityExtension", nameof (TryGetSecurityToken)))
      {
        TaskHubExtension.SecurityTokenCacheService service = requestContext.GetService<TaskHubExtension.SecurityTokenCacheService>();
        if (!service.TryGetValue(requestContext, scopeIdentifier, artifactUri, out securityToken))
        {
          try
          {
            securityToken = this.GetSecurityToken(requestContext.Elevate(), scopeIdentifier, artifactUri);
          }
          catch (Exception ex)
          {
            requestContext.TraceException(0, "DistributedTask", "SecurityExtension", ex);
            securityToken = (string) null;
          }
          if (string.IsNullOrEmpty(securityToken))
          {
            requestContext.TraceError("SecurityExtension", "Failed to retrieve security token for artifact {0}", (object) artifactUri);
          }
          else
          {
            requestContext.TraceVerbose("SecurityExtension", "Successfully retrieved security token {0} for artifact {1}", (object) securityToken, (object) artifactUri);
            service.Set(requestContext, scopeIdentifier, artifactUri, securityToken);
          }
        }
        return securityToken != null;
      }
    }

    internal bool HasDeletePermission(
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      Uri artifactUri)
    {
      if (requestContext.IsSystemContext)
        return true;
      string securityToken;
      return this.TryGetSecurityToken(requestContext, scopeIdentifier, artifactUri, out securityToken) && this.HasDeletePermission(requestContext, scopeIdentifier, artifactUri, securityToken);
    }

    internal bool HasReadPermission(
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      Uri artifactUri)
    {
      if (requestContext.IsSystemContext)
        return true;
      string securityToken;
      return this.TryGetSecurityToken(requestContext, scopeIdentifier, artifactUri, out securityToken) && this.HasReadPermission(requestContext, scopeIdentifier, artifactUri, securityToken);
    }

    internal bool HasWritePermission(
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      Uri artifactUri)
    {
      if (requestContext.IsSystemContext)
        return true;
      string securityToken;
      return this.TryGetSecurityToken(requestContext, scopeIdentifier, artifactUri, out securityToken) && this.HasWritePermission(requestContext, scopeIdentifier, artifactUri, securityToken);
    }

    protected virtual void Dispose(bool disposing)
    {
    }

    protected abstract string GetSecurityToken(
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      Uri artifactUri);

    protected abstract bool HasDeletePermission(
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      Uri artifactUri,
      string securityToken);

    protected abstract bool HasReadPermission(
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      Uri artifactUri,
      string securityToken);

    protected abstract bool HasWritePermission(
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      Uri artifactUri,
      string securityToken);

    private sealed class SecurityTokenCacheService : VssCacheService
    {
      private VssMemoryCacheList<string, string> m_cache;

      public bool TryGetValue(
        IVssRequestContext requestContext,
        Guid scopeIdentifier,
        Uri artifactUri,
        out string token)
      {
        return this.m_cache.TryGetValue(this.GetCacheKey(scopeIdentifier, artifactUri), out token);
      }

      public void Set(
        IVssRequestContext requestContext,
        Guid scopeIdentifier,
        Uri artifactUri,
        string token)
      {
        this.m_cache[this.GetCacheKey(scopeIdentifier, artifactUri)] = token;
      }

      protected override void ServiceStart(IVssRequestContext systemRequestContext)
      {
        base.ServiceStart(systemRequestContext);
        this.m_cache = new VssMemoryCacheList<string, string>((IVssCachePerformanceProvider) this, 128);
      }

      protected override void ServiceEnd(IVssRequestContext systemRequestContext)
      {
        this.m_cache.Clear();
        base.ServiceEnd(systemRequestContext);
      }

      private string GetCacheKey(Guid scopeIdentifier, Uri artifactUri) => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0:N}_{1}", (object) scopeIdentifier, (object) artifactUri);
    }
  }
}
