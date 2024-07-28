// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.BuildRequestHelper
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.Azure.Pipelines.Server.ObjectModel;
using Microsoft.CSharp.RuntimeBinder;
using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server;
using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts;
using Microsoft.TeamFoundation.DistributedTask.Pipelines;
using Microsoft.TeamFoundation.DistributedTask.Pipelines.Artifacts.ContainerProviders;
using Microsoft.TeamFoundation.DistributedTask.Pipelines.Server;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Text;

namespace Microsoft.TeamFoundation.Build2.Server
{
  internal class BuildRequestHelper : OrchestrationHelper, IBuildRequestHelper
  {
    private const int c_maxContextRecursionDepth = 100;
    private IBuildSourceProvider m_sourceProvider;
    private BuildData m_build;
    private BuildRepository m_repository;
    private bool m_agentQueueResolved;
    private AgentPoolQueue m_agentPoolQueue;
    private static readonly string s_minGatedSupportedAgentVersion = "1.95.0";
    private static readonly string s_taskTimeoutSupportedAgentVersion = "1.101.0";
    private static readonly string s_gitHubEnterpriseSupportedAgentVersion = "2.124.0";
    private static readonly string s_bitbucketSupportedAgentVersion = "2.120.0";
    private static readonly string s_defaultBuildNumberFormat = "$(BuildId)";
    private static readonly char[] MultiplierValueSeparator = new char[1]
    {
      ','
    };
    public static readonly string MinServerSupportedAgentVersion = "1.98.1";
    private const string TraceLayer = "BuildRequestHelper";

    public BuildRequestHelper(
      IVssRequestContext requestContext,
      IBuildSourceProvider sourceProvider,
      BuildDefinition definition,
      BuildRepository repository,
      BuildData build,
      Microsoft.VisualStudio.Services.Identity.Identity requestedBy,
      Microsoft.VisualStudio.Services.Identity.Identity requestedFor,
      BuildRequestValidationFlags validationFlags)
      : base(requestContext, definition, requestedBy, requestedFor)
    {
      ArgumentUtility.CheckForNull<BuildRepository>(repository, nameof (repository));
      ArgumentUtility.CheckForNull<BuildData>(build, nameof (build));
      ArgumentUtility.CheckForNull<DateTime>(build.QueueTime, "QueueTime");
      if (string.IsNullOrEmpty(repository.DefaultBranch) && string.IsNullOrEmpty(build.SourceBranch))
        ArgumentUtility.CheckStringForNullOrEmpty(repository.DefaultBranch, "DefaultBranch");
      bool result = false;
      string str;
      if (definition.Repository.Properties.TryGetValue("skipSyncSource", out str))
        bool.TryParse(str, out result);
      if (!result)
      {
        string errorMessage = string.Empty;
        if (!string.IsNullOrEmpty(build.SourceBranch) && !string.IsNullOrEmpty(build.SourceVersion) && !validationFlags.HasFlag((Enum) BuildRequestValidationFlags.SkipSourceVersionValidation) && !sourceProvider.IsSourceVersionValidInSourceBranch(requestContext, build.ProjectId, repository, build.SourceBranch, build.SourceVersion, out errorMessage))
          throw new InvalidGitVersionSpec(errorMessage);
      }
      this.m_sourceProvider = sourceProvider;
      this.m_repository = repository;
      this.m_build = build;
      if (!string.IsNullOrEmpty(this.m_build.SourceBranch))
      {
        IDictionary<string, string> variables = BuildRequestHelper.DeserializeParameters(this.m_build.Parameters);
        this.m_build.SourceBranch = this.m_sourceProvider.NormalizeSourceBranch(this.m_build.SourceBranch, definition, variables);
      }
      this.m_build.Reason = this.m_sourceProvider.NormalizeBuildReason(this.m_build.Reason, this.m_build);
    }

    public BuildData QueueBuild(
      IVssRequestContext requestContext,
      IList<IBuildOption> options,
      IEnumerable<IBuildRequestValidator> validators,
      BuildRequestValidationFlags validationFlags,
      out IOrchestrationEnvironment environment,
      out IOrchestrationProcess orchestration,
      int? sourceBuildId = null)
    {
      AgentPoolQueue queue;
      if (!sourceBuildId.HasValue)
        this.SetupEnvironment(requestContext, options, this.m_build.Demands, ref validationFlags, out queue, out environment, out orchestration);
      else
        this.CloneEnvironment(requestContext, options, ref validationFlags, out queue, out environment, out orchestration);
      if (!this.CheckValidationResults(requestContext, validationFlags))
        return this.HandleValidationFailure(requestContext, validationFlags);
      if (this.m_build.JustInTime.PreviewRun)
        return new BuildData();
      if (validators.Any<IBuildRequestValidator>())
      {
        BuildRequestValidationContext validationContext = new BuildRequestValidationContext()
        {
          Build = this.m_build,
          Definition = this.m_definition,
          Process = orchestration,
          Environment = environment,
          Queue = queue,
          SourceProvider = this.m_sourceProvider
        };
        foreach (IBuildRequestValidator validator in validators)
          this.m_build.ValidationResults.Add(validator.ValidateRequest(requestContext, validationContext));
        if (!this.CheckValidationResults(requestContext, validationFlags))
          return this.HandleValidationFailure(requestContext, validationFlags);
      }
      this.SanitizeBuildParameters(this.GetPipelineBuilder(requestContext, queue).CreateBuildContext(new BuildOptions()), BuildRequestHelper.DeserializeParameters(this.m_build.Parameters));
      IEnumerable<ChangeData> changes1 = (IEnumerable<ChangeData>) null;
      bool changesCalculated = false;
      if (this.m_sourceProvider != null)
      {
        Lazy<Change> sourceChange = new Lazy<Change>((Func<Change>) (() =>
        {
          try
          {
            if (string.IsNullOrEmpty(this.m_build.SourceVersion))
              return (Change) null;
            return this.m_sourceProvider.GetChanges(requestContext, this.m_build.ProjectId, this.m_repository, (IEnumerable<string>) new string[1]
            {
              this.m_build.SourceVersion
            }).FirstOrDefault<Change>();
          }
          catch (Exception ex)
          {
            requestContext.TraceException(nameof (BuildRequestHelper), ex);
            return (Change) null;
          }
        }));
        List<Change> changes2;
        try
        {
          changesCalculated = this.m_sourceProvider.TryCalculateChangesWithValidation(requestContext, this.m_build, this.m_definition, 200, out changes2);
        }
        catch (Exception ex)
        {
          changes2 = new List<Change>();
          requestContext.TraceException(nameof (BuildRequestHelper), ex);
        }
        if (changesCalculated)
          changes1 = (IEnumerable<ChangeData>) this.m_sourceProvider.GetChangeData(requestContext, (IReadOnlyList<Change>) changes2, sourceChange);
        using (requestContext.CITimer("EnsureSourceVersionInfoPopulatedElapsedMilliseconds"))
        {
          string pullRequestId;
          if (this.m_build.TriggerInfo.TryGetValue("pr.number", out pullRequestId))
          {
            PullRequest pullRequest = (PullRequest) null;
            try
            {
              pullRequest = this.m_sourceProvider.GetPullRequest(requestContext, (IReadOnlyBuildData) this.m_build, pullRequestId);
            }
            catch (Exception ex)
            {
              requestContext.TraceException(12030199, nameof (BuildRequestHelper), ex);
            }
            if (pullRequest != null)
              this.m_build.SourceVersionInfo = pullRequest.ToSourceVersionInfo();
            else
              requestContext.TraceError(12030199, nameof (BuildRequestHelper), "Unable to retrieve pull request {0} from repository {1}", (object) pullRequestId, (object) this.m_build?.Repository?.Id);
          }
          else
          {
            Change change = (Change) null;
            if (changesCalculated)
              change = SourceVersionHelper.FindChange(requestContext, this.m_sourceProvider, changes2, this.m_build.SourceVersion);
            if (change == null)
              change = sourceChange.Value;
            if (change != null)
              this.m_build.SourceVersionInfo = change.ToSourceVersionInfo();
          }
        }
      }
      if (environment is PipelineEnvironment pipelineEnvironment1 && orchestration is PipelineProcess process)
      {
        if (requestContext.IsFeatureEnabled("DistributedTask.EnablePipelineTriggers"))
          ArtifactResolver.SetPipelineVersion(requestContext, pipelineEnvironment1.Resources, process, this.m_build.TriggerInfo);
        if (this.m_build.TriggerInfo.ContainsKey("ci.triggerRepository") && pipelineEnvironment1.Resources.Repositories != null)
          pipelineEnvironment1.Resources.Repositories.FirstOrDefault<Microsoft.TeamFoundation.DistributedTask.Pipelines.RepositoryResource>((Func<Microsoft.TeamFoundation.DistributedTask.Pipelines.RepositoryResource, bool>) (r => string.Equals(r.Id, this.m_build.TriggerInfo["ci.triggerRepository"], StringComparison.OrdinalIgnoreCase)))?.Properties.Set<bool>("system.istriggeringrepository", true);
        this.m_build.RepositoryResources = pipelineEnvironment1.Resources.Repositories.ToHashSet<Microsoft.TeamFoundation.DistributedTask.Pipelines.RepositoryResource>();
      }
      int maxConcurrentBuildsPerBranch = 1;
      ITeamFoundationFeatureAvailabilityService service = requestContext.GetService<ITeamFoundationFeatureAvailabilityService>();
      if (this.m_build.Reason == BuildReason.BatchedCI && service.IsFeatureEnabled(requestContext, "Build2.MaxConcurrentBuildsPerBranch"))
      {
        ContinuousIntegrationTrigger integrationTrigger = this.m_definition.Triggers.OfType<ContinuousIntegrationTrigger>().FirstOrDefault<ContinuousIntegrationTrigger>();
        if (integrationTrigger != null)
          maxConcurrentBuildsPerBranch = integrationTrigger.MaxConcurrentBuildsPerBranch;
      }
      BuildData buildData = (BuildData) null;
      using (Build2Component component = requestContext.CreateComponent<Build2Component>())
        buildData = component.QueueBuild(this.m_build, this.m_requestedBy.Id, this.m_requestedFor.Id, changesCalculated, changes1, maxConcurrentBuildsPerBranch, out AgentPoolQueue _);
      foreach (BuildRequestValidationResult validationResult in this.m_build.ValidationResults)
      {
        if (validationResult.ShowAuditInformation)
        {
          IVssRequestContext requestContext1 = requestContext;
          Dictionary<string, object> data = new Dictionary<string, object>();
          data["PipelineName"] = (object) buildData.Definition.Name;
          data["PipelineId"] = (object) buildData.Definition.Id.ToString();
          data["InsecureVariables"] = (object) validationResult.AuditInsecureSettableVariables;
          data["BuildNumber"] = (object) buildData.BuildNumber;
          data["BuildId"] = (object) buildData.Id.ToString();
          Guid projectId1 = buildData.Definition.ProjectId;
          Guid targetHostId = new Guid();
          Guid projectId2 = projectId1;
          requestContext1.LogAuditEvent("Pipelines.VariablesSetAtRuntime", data, targetHostId, projectId2);
        }
      }
      if (buildData == null)
        return (BuildData) null;
      BuildStatus? status = buildData.Status;
      BuildStatus buildStatus = BuildStatus.Completed;
      if (!(status.GetValueOrDefault() == buildStatus & status.HasValue))
      {
        bool isForkBuild = this.IsUntrustedForkedBuild();
        BuildAuthorizationScope scope = this.m_definition.JobAuthorizationScope;
        PipelineEnvironment environment1 = environment as PipelineEnvironment;
        bool flag = environment1 != null;
        if (isForkBuild && !BuildRequestHelper.AllowFullAccessTokenToFork(requestContext, this.m_definition) || this.IsYamlProvidedJustInTime(requestContext) && requestContext.IsFeatureEnabled("Build2.RestrictJustInTimeProcessToCurrentRun"))
        {
          scope = BuildAuthorizationScope.Project;
          if (flag)
            environment1.Options.SystemTokenScope = BuildRequestScopeHelper.GetRestrictiveOAuthScope(requestContext, buildData, environment1);
        }
        else if (requestContext.IsFeatureEnabled("Build2.RestrictPipelineWithModernScopes") && flag)
          environment1.Options.SystemTokenScope = BuildRequestScopeHelper.GetOAuthScope(requestContext, buildData, environment1, orchestration);
        if (new ProjectPipelineGeneralSettingsHelper(requestContext, buildData.ProjectId, true).EnforceJobAuthScope)
        {
          scope = BuildAuthorizationScope.Project;
          requestContext.TraceInfo(12030227, nameof (BuildRequestHelper), "Job authorization scope set at org or project level. Overriding scope to current project.");
        }
        if (flag)
        {
          (string Key, string Value) pipelineCacheClaim = BuildRequestClaimHelper.GetPipelineCacheClaim(requestContext, orchestration, buildData, environment1, isForkBuild);
          if (pipelineCacheClaim.Value != null)
            environment1.Options.SystemTokenCustomClaims.Add(pipelineCacheClaim.Key, pipelineCacheClaim.Value);
        }
        if (requestContext.IsFeatureEnabled("Build2.UseBuildIdCustomClaim") & flag)
        {
          Claim buildIdClaim = PipelineClaimHelper.GetBuildIdClaim(buildData.ProjectId, buildData.Id);
          environment1.Options.SystemTokenCustomClaims[buildIdClaim.Type] = buildIdClaim.Value;
        }
        Microsoft.VisualStudio.Services.Identity.Identity identity = this.m_tfBuildService.ProvisionServiceIdentity(requestContext, scope, this.m_definition.ProjectId);
        if (identity != null && orchestration.ProcessType == OrchestrationProcessType.Container)
        {
          foreach (TaskOrchestrationJob job in (orchestration as TaskOrchestrationContainer).GetJobs())
            job.ExecuteAs = identity.ToIdentityRef(requestContext, false);
        }
      }
      buildData.Uri = UriHelper.CreateBuildUri(buildData.Id);
      this.m_definition.BuildQueued(requestContext, buildData, orchestration);
      if (this.m_build.Properties.Count > 0 || buildData.Properties.Count > 0)
      {
        foreach (KeyValuePair<string, object> property in (IEnumerable<KeyValuePair<string, object>>) this.m_build.Properties)
          buildData.Properties.TryAdd<string, object>(property.Key, property.Value);
        requestContext.GetService<TeamFoundationPropertyService>().SetProperties(requestContext, buildData.CreateArtifactSpec(requestContext), buildData.Properties.Convert());
      }
      long buildContainer = requestContext.GetService<IBuildServiceInternal>().CreateBuildContainer(requestContext.Elevate(), buildData);
      string str = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}_{1}_{2}_{3}_{4}", (object) (requestContext.ExecutionEnvironment.IsHostedDeployment ? "VSTS" : "TFS"), (object) requestContext.ServiceHost.InstanceId, (object) "build", (object) this.m_build.Definition.Id, (object) this.m_build.Id);
      if (environment is PipelineEnvironment pipelineEnvironment2 && pipelineEnvironment2.Version > 1)
      {
        pipelineEnvironment2.SystemVariables[WellKnownDistributedTaskVariables.MsDeployUserAgent] = (VariableValue) str;
        pipelineEnvironment2.SystemVariables[WellKnownDistributedTaskVariables.AzureUserAgent] = (VariableValue) str;
        this.SetSystemVariables(pipelineEnvironment2.SystemVariables, buildContainer, buildData);
      }
      else
      {
        environment.Variables[WellKnownDistributedTaskVariables.MsDeployUserAgent] = (VariableValue) str;
        environment.Variables[WellKnownDistributedTaskVariables.AzureUserAgent] = (VariableValue) str;
        this.SetSystemVariables(environment.Variables, buildContainer, buildData);
      }
      if (this.IsYamlProvidedJustInTime(requestContext) && this.m_build.JustInTime.Configuration.Callbacks != null)
        requestContext.GetService<IBuildStatusCallbackService>().Store(requestContext.Elevate(), (IReadOnlyBuildData) buildData, this.m_build.JustInTime.Configuration.Callbacks);
      this.LogBuildTriggerDetails(requestContext, (PipelineEnvironment) environment, this.m_build, buildData);
      return buildData;
    }

    private void SetSystemVariables(
      IDictionary<string, VariableValue> variables,
      long containerId,
      BuildData newBuild)
    {
      variables["build.buildId"] = (VariableValue) newBuild.Id.ToString((IFormatProvider) CultureInfo.InvariantCulture);
      variables["build.buildUri"] = (VariableValue) newBuild.Uri.AbsoluteUri;
      variables["build.buildNumber"] = (VariableValue) newBuild.BuildNumber;
      variables["build.containerId"] = (VariableValue) containerId.ToString((IFormatProvider) CultureInfo.InvariantCulture);
      variables[WellKnownDistributedTaskVariables.IsScheduled] = (VariableValue) (newBuild.Reason == BuildReason.Schedule ? true.ToString() : false.ToString());
    }

    public static IDictionary<string, string> DeserializeParameters(string serializedParameters)
    {
      IDictionary<string, string> dictionary = (IDictionary<string, string>) null;
      if (!string.IsNullOrEmpty(serializedParameters))
      {
        try
        {
          dictionary = JsonUtility.FromString<IDictionary<string, string>>(serializedParameters);
        }
        catch (JsonReaderException ex)
        {
          throw new ArgumentException(BuildServerResources.FailedToDeserializeBuildParameters(), (Exception) ex).Expected("Build2");
        }
      }
      return dictionary;
    }

    public static bool AllowFullAccessTokenToFork(
      IVssRequestContext requestContext,
      BuildDefinition definition)
    {
      IEnumerable<PullRequestTrigger> source = definition.Triggers.OfType<PullRequestTrigger>();
      if (!requestContext.IsFeatureEnabled("Build2.AllowCentralizedPipelineControls"))
        return source.Any<PullRequestTrigger>((Func<PullRequestTrigger, bool>) (t => t.Forks.AllowFullAccessToken));
      return !new ProjectPipelineGeneralSettingsHelper(requestContext, definition.ProjectId, true).GetEffectivePipelineTriggerSettings().EnforceJobAuthScopeForForks && source.Any<PullRequestTrigger>((Func<PullRequestTrigger, bool>) (t => t.Forks.AllowFullAccessToken));
    }

    public static bool AllowSecretsToFork(
      IVssRequestContext requestContext,
      BuildDefinition definition)
    {
      IEnumerable<PullRequestTrigger> source = definition.Triggers.OfType<PullRequestTrigger>();
      if (!requestContext.IsFeatureEnabled("Build2.AllowCentralizedPipelineControls"))
        return source.Any<PullRequestTrigger>((Func<PullRequestTrigger, bool>) (t => t.Forks.AllowSecrets));
      return !new ProjectPipelineGeneralSettingsHelper(requestContext, definition.ProjectId, true).GetEffectivePipelineTriggerSettings().EnforceNoAccessToSecretsFromForks && source.Any<PullRequestTrigger>((Func<PullRequestTrigger, bool>) (t => t.Forks.AllowSecrets));
    }

    internal void SetupEnvironment(
      IVssRequestContext requestContext,
      IList<IBuildOption> options,
      List<Demand> demands,
      ref BuildRequestValidationFlags validationFlags,
      out AgentPoolQueue queue,
      out IOrchestrationEnvironment environment,
      out IOrchestrationProcess orchestration)
    {
      YamlProcess process1 = this.m_definition.Process as YamlProcess;
      DesignerProcess designerProcess = this.m_definition.Process as DesignerProcess;
      DockerProcess process2 = this.m_definition.Process as DockerProcess;
      BuildProcess process3 = this.m_definition.Process;
      if (process2 != null)
        designerProcess = process2.GenerateDesignerProcess(requestContext, this.m_build);
      if (process1 != null)
        this.SetUpYamlProcessEnvironment(requestContext, process1, ref validationFlags, out queue, out environment, out orchestration);
      else
        this.SetUpDesignerProcessEnvironment(requestContext, designerProcess, options, demands, out queue, out environment, out orchestration);
    }

    private void LogBuildTriggerDetails(
      IVssRequestContext requestContext,
      PipelineEnvironment environment,
      BuildData build,
      BuildData queuedBuild)
    {
      try
      {
        object obj1 = (object) new ExpandoObject();
        if (environment != null)
        {
          VariableValue variableValue1;
          if (environment.SystemVariables.TryGetValue("build.sourceVersion", out variableValue1))
          {
            // ISSUE: reference to a compiler-generated field
            if (BuildRequestHelper.\u003C\u003Eo__7.\u003C\u003Ep__0 == null)
            {
              // ISSUE: reference to a compiler-generated field
              BuildRequestHelper.\u003C\u003Eo__7.\u003C\u003Ep__0 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.SetMember(CSharpBinderFlags.None, "source_version", typeof (BuildRequestHelper), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
              {
                CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
                CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null)
              }));
            }
            // ISSUE: reference to a compiler-generated field
            // ISSUE: reference to a compiler-generated field
            object obj2 = BuildRequestHelper.\u003C\u003Eo__7.\u003C\u003Ep__0.Target((CallSite) BuildRequestHelper.\u003C\u003Eo__7.\u003C\u003Ep__0, obj1, variableValue1.Value);
          }
          VariableValue variableValue2;
          if (environment.SystemVariables.TryGetValue("build.sourceBranch", out variableValue2))
          {
            // ISSUE: reference to a compiler-generated field
            if (BuildRequestHelper.\u003C\u003Eo__7.\u003C\u003Ep__1 == null)
            {
              // ISSUE: reference to a compiler-generated field
              BuildRequestHelper.\u003C\u003Eo__7.\u003C\u003Ep__1 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.SetMember(CSharpBinderFlags.None, "source_branch", typeof (BuildRequestHelper), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
              {
                CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
                CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null)
              }));
            }
            // ISSUE: reference to a compiler-generated field
            // ISSUE: reference to a compiler-generated field
            object obj3 = BuildRequestHelper.\u003C\u003Eo__7.\u003C\u003Ep__1.Target((CallSite) BuildRequestHelper.\u003C\u003Eo__7.\u003C\u003Ep__1, obj1, variableValue2.Value);
          }
          VariableValue variableValue3;
          if (environment.SystemVariables.TryGetValue("build.reason", out variableValue3))
          {
            // ISSUE: reference to a compiler-generated field
            if (BuildRequestHelper.\u003C\u003Eo__7.\u003C\u003Ep__2 == null)
            {
              // ISSUE: reference to a compiler-generated field
              BuildRequestHelper.\u003C\u003Eo__7.\u003C\u003Ep__2 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.SetMember(CSharpBinderFlags.None, "build_reason", typeof (BuildRequestHelper), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
              {
                CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
                CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null)
              }));
            }
            // ISSUE: reference to a compiler-generated field
            // ISSUE: reference to a compiler-generated field
            object obj4 = BuildRequestHelper.\u003C\u003Eo__7.\u003C\u003Ep__2.Target((CallSite) BuildRequestHelper.\u003C\u003Eo__7.\u003C\u003Ep__2, obj1, variableValue3.Value);
          }
          VariableValue variableValue4;
          if (environment.SystemVariables.TryGetValue("system.pullRequest.isFork", out variableValue4))
          {
            // ISSUE: reference to a compiler-generated field
            if (BuildRequestHelper.\u003C\u003Eo__7.\u003C\u003Ep__3 == null)
            {
              // ISSUE: reference to a compiler-generated field
              BuildRequestHelper.\u003C\u003Eo__7.\u003C\u003Ep__3 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.SetMember(CSharpBinderFlags.None, "is_fork", typeof (BuildRequestHelper), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
              {
                CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
                CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null)
              }));
            }
            // ISSUE: reference to a compiler-generated field
            // ISSUE: reference to a compiler-generated field
            object obj5 = BuildRequestHelper.\u003C\u003Eo__7.\u003C\u003Ep__3.Target((CallSite) BuildRequestHelper.\u003C\u003Eo__7.\u003C\u003Ep__3, obj1, variableValue4.Value);
          }
          VariableValue variableValue5;
          if (environment.SystemVariables.TryGetValue("build.queuedBy", out variableValue5))
          {
            // ISSUE: reference to a compiler-generated field
            if (BuildRequestHelper.\u003C\u003Eo__7.\u003C\u003Ep__4 == null)
            {
              // ISSUE: reference to a compiler-generated field
              BuildRequestHelper.\u003C\u003Eo__7.\u003C\u003Ep__4 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.SetMember(CSharpBinderFlags.None, "queued_by", typeof (BuildRequestHelper), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
              {
                CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
                CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null)
              }));
            }
            // ISSUE: reference to a compiler-generated field
            // ISSUE: reference to a compiler-generated field
            object obj6 = BuildRequestHelper.\u003C\u003Eo__7.\u003C\u003Ep__4.Target((CallSite) BuildRequestHelper.\u003C\u003Eo__7.\u003C\u003Ep__4, obj1, variableValue5.Value);
          }
          VariableValue variableValue6;
          if (environment.SystemVariables.TryGetValue("build.requestedFor", out variableValue6))
          {
            // ISSUE: reference to a compiler-generated field
            if (BuildRequestHelper.\u003C\u003Eo__7.\u003C\u003Ep__5 == null)
            {
              // ISSUE: reference to a compiler-generated field
              BuildRequestHelper.\u003C\u003Eo__7.\u003C\u003Ep__5 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.SetMember(CSharpBinderFlags.None, "requested_for", typeof (BuildRequestHelper), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
              {
                CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
                CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null)
              }));
            }
            // ISSUE: reference to a compiler-generated field
            // ISSUE: reference to a compiler-generated field
            object obj7 = BuildRequestHelper.\u003C\u003Eo__7.\u003C\u003Ep__5.Target((CallSite) BuildRequestHelper.\u003C\u003Eo__7.\u003C\u003Ep__5, obj1, variableValue6.Value);
          }
          VariableValue variableValue7;
          if (environment.SystemVariables.TryGetValue("build.definitionName", out variableValue7))
          {
            // ISSUE: reference to a compiler-generated field
            if (BuildRequestHelper.\u003C\u003Eo__7.\u003C\u003Ep__6 == null)
            {
              // ISSUE: reference to a compiler-generated field
              BuildRequestHelper.\u003C\u003Eo__7.\u003C\u003Ep__6 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.SetMember(CSharpBinderFlags.None, "definition_name", typeof (BuildRequestHelper), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
              {
                CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
                CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null)
              }));
            }
            // ISSUE: reference to a compiler-generated field
            // ISSUE: reference to a compiler-generated field
            object obj8 = BuildRequestHelper.\u003C\u003Eo__7.\u003C\u003Ep__6.Target((CallSite) BuildRequestHelper.\u003C\u003Eo__7.\u003C\u003Ep__6, obj1, variableValue7.Value);
          }
        }
        if (build != null)
        {
          Microsoft.TeamFoundation.DistributedTask.Pipelines.RepositoryResource repositoryResource = build.RepositoryResources.FirstOrDefault<Microsoft.TeamFoundation.DistributedTask.Pipelines.RepositoryResource>((Func<Microsoft.TeamFoundation.DistributedTask.Pipelines.RepositoryResource, bool>) (r => r.Properties.TryGetValue<string>("system.istriggeringrepository", out string _)));
          if (repositoryResource != null)
          {
            // ISSUE: reference to a compiler-generated field
            if (BuildRequestHelper.\u003C\u003Eo__7.\u003C\u003Ep__7 == null)
            {
              // ISSUE: reference to a compiler-generated field
              BuildRequestHelper.\u003C\u003Eo__7.\u003C\u003Ep__7 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.SetMember(CSharpBinderFlags.None, "repository_name", typeof (BuildRequestHelper), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
              {
                CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
                CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null)
              }));
            }
            // ISSUE: reference to a compiler-generated field
            // ISSUE: reference to a compiler-generated field
            object obj9 = BuildRequestHelper.\u003C\u003Eo__7.\u003C\u003Ep__7.Target((CallSite) BuildRequestHelper.\u003C\u003Eo__7.\u003C\u003Ep__7, obj1, repositoryResource.Name);
            // ISSUE: reference to a compiler-generated field
            if (BuildRequestHelper.\u003C\u003Eo__7.\u003C\u003Ep__8 == null)
            {
              // ISSUE: reference to a compiler-generated field
              BuildRequestHelper.\u003C\u003Eo__7.\u003C\u003Ep__8 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.SetMember(CSharpBinderFlags.None, "repository_type", typeof (BuildRequestHelper), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
              {
                CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
                CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null)
              }));
            }
            // ISSUE: reference to a compiler-generated field
            // ISSUE: reference to a compiler-generated field
            object obj10 = BuildRequestHelper.\u003C\u003Eo__7.\u003C\u003Ep__8.Target((CallSite) BuildRequestHelper.\u003C\u003Eo__7.\u003C\u003Ep__8, obj1, repositoryResource.Type);
          }
          // ISSUE: reference to a compiler-generated field
          if (BuildRequestHelper.\u003C\u003Eo__7.\u003C\u003Ep__9 == null)
          {
            // ISSUE: reference to a compiler-generated field
            BuildRequestHelper.\u003C\u003Eo__7.\u003C\u003Ep__9 = CallSite<Func<CallSite, object, int, object>>.Create(Binder.SetMember(CSharpBinderFlags.None, "build_id", typeof (BuildRequestHelper), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
            {
              CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
              CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null)
            }));
          }
          // ISSUE: reference to a compiler-generated field
          // ISSUE: reference to a compiler-generated field
          object obj11 = BuildRequestHelper.\u003C\u003Eo__7.\u003C\u003Ep__9.Target((CallSite) BuildRequestHelper.\u003C\u003Eo__7.\u003C\u003Ep__9, obj1, queuedBuild.Id);
          // ISSUE: reference to a compiler-generated field
          if (BuildRequestHelper.\u003C\u003Eo__7.\u003C\u003Ep__10 == null)
          {
            // ISSUE: reference to a compiler-generated field
            BuildRequestHelper.\u003C\u003Eo__7.\u003C\u003Ep__10 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.SetMember(CSharpBinderFlags.None, "build_number", typeof (BuildRequestHelper), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
            {
              CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
              CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null)
            }));
          }
          // ISSUE: reference to a compiler-generated field
          // ISSUE: reference to a compiler-generated field
          object obj12 = BuildRequestHelper.\u003C\u003Eo__7.\u003C\u003Ep__10.Target((CallSite) BuildRequestHelper.\u003C\u003Eo__7.\u003C\u003Ep__10, obj1, queuedBuild.BuildNumber);
          // ISSUE: reference to a compiler-generated field
          if (BuildRequestHelper.\u003C\u003Eo__7.\u003C\u003Ep__11 == null)
          {
            // ISSUE: reference to a compiler-generated field
            BuildRequestHelper.\u003C\u003Eo__7.\u003C\u003Ep__11 = CallSite<Func<CallSite, object, Guid, object>>.Create(Binder.SetMember(CSharpBinderFlags.None, "project_id", typeof (BuildRequestHelper), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
            {
              CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
              CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null)
            }));
          }
          // ISSUE: reference to a compiler-generated field
          // ISSUE: reference to a compiler-generated field
          object obj13 = BuildRequestHelper.\u003C\u003Eo__7.\u003C\u003Ep__11.Target((CallSite) BuildRequestHelper.\u003C\u003Eo__7.\u003C\u003Ep__11, obj1, queuedBuild.ProjectId);
          // ISSUE: reference to a compiler-generated field
          if (BuildRequestHelper.\u003C\u003Eo__7.\u003C\u003Ep__12 == null)
          {
            // ISSUE: reference to a compiler-generated field
            BuildRequestHelper.\u003C\u003Eo__7.\u003C\u003Ep__12 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.SetMember(CSharpBinderFlags.None, "project_name", typeof (BuildRequestHelper), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
            {
              CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
              CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null)
            }));
          }
          // ISSUE: reference to a compiler-generated field
          // ISSUE: reference to a compiler-generated field
          object obj14 = BuildRequestHelper.\u003C\u003Eo__7.\u003C\u003Ep__12.Target((CallSite) BuildRequestHelper.\u003C\u003Eo__7.\u003C\u003Ep__12, obj1, queuedBuild.GetProjectName(requestContext));
          // ISSUE: reference to a compiler-generated field
          if (BuildRequestHelper.\u003C\u003Eo__7.\u003C\u003Ep__13 == null)
          {
            // ISSUE: reference to a compiler-generated field
            BuildRequestHelper.\u003C\u003Eo__7.\u003C\u003Ep__13 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.SetMember(CSharpBinderFlags.None, "build_uri", typeof (BuildRequestHelper), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
            {
              CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
              CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null)
            }));
          }
          // ISSUE: reference to a compiler-generated field
          // ISSUE: reference to a compiler-generated field
          object obj15 = BuildRequestHelper.\u003C\u003Eo__7.\u003C\u003Ep__13.Target((CallSite) BuildRequestHelper.\u003C\u003Eo__7.\u003C\u003Ep__13, obj1, queuedBuild.Uri.ToString());
        }
        // ISSUE: reference to a compiler-generated field
        if (BuildRequestHelper.\u003C\u003Eo__7.\u003C\u003Ep__15 == null)
        {
          // ISSUE: reference to a compiler-generated field
          BuildRequestHelper.\u003C\u003Eo__7.\u003C\u003Ep__15 = CallSite<Func<CallSite, object, string>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (string), typeof (BuildRequestHelper)));
        }
        // ISSUE: reference to a compiler-generated field
        Func<CallSite, object, string> target = BuildRequestHelper.\u003C\u003Eo__7.\u003C\u003Ep__15.Target;
        // ISSUE: reference to a compiler-generated field
        CallSite<Func<CallSite, object, string>> p15 = BuildRequestHelper.\u003C\u003Eo__7.\u003C\u003Ep__15;
        // ISSUE: reference to a compiler-generated field
        if (BuildRequestHelper.\u003C\u003Eo__7.\u003C\u003Ep__14 == null)
        {
          // ISSUE: reference to a compiler-generated field
          BuildRequestHelper.\u003C\u003Eo__7.\u003C\u003Ep__14 = CallSite<Func<CallSite, Type, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "SerializeObject", (IEnumerable<Type>) null, typeof (BuildRequestHelper), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, (string) null),
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        object obj16 = BuildRequestHelper.\u003C\u003Eo__7.\u003C\u003Ep__14.Target((CallSite) BuildRequestHelper.\u003C\u003Eo__7.\u003C\u003Ep__14, typeof (JsonConvert), obj1);
        string format = target((CallSite) p15, obj16);
        requestContext.TraceAlways(12030289, TraceLevel.Verbose, "Build2", nameof (BuildRequestHelper), format);
      }
      catch (Exception ex)
      {
        requestContext.TraceAlways(0, TraceLevel.Error, "Build2", nameof (BuildRequestHelper), "Error occured while logging the trigger details: " + ex.ToString());
      }
    }

    private void SetUpYamlProcessEnvironment(
      IVssRequestContext requestContext,
      YamlProcess yamlProcess,
      ref BuildRequestValidationFlags validationFlags,
      out AgentPoolQueue queue,
      out IOrchestrationEnvironment environment,
      out IOrchestrationProcess orchestration)
    {
      queue = (AgentPoolQueue) null;
      environment = (IOrchestrationEnvironment) null;
      orchestration = (IOrchestrationProcess) null;
      if (!this.CheckValidationResults(requestContext, validationFlags))
        return;
      if (requestContext.IsFeatureEnabled("Build2.DisableQueueYamlBuild"))
        this.m_build.ValidationResults.Add(new BuildRequestValidationResult()
        {
          Result = ValidationResult.Error,
          Message = "Queuing a build for a YAML definition is disabled."
        });
      else if (!this.m_sourceProvider.GetAttributes(requestContext).SupportsYamlDefinition)
      {
        this.m_build.ValidationResults.Add(new BuildRequestValidationResult()
        {
          Result = ValidationResult.Error,
          Message = "YAML build definitions are not supported for repository type '" + this.m_sourceProvider.GetAttributes(requestContext).Name + "'."
        });
      }
      else
      {
        if (string.IsNullOrEmpty(this.m_build.SourceVersion))
        {
          try
          {
            this.m_build.SourceVersion = this.m_sourceProvider.GetLatestSourceVersion(requestContext, this.m_definition, this.m_build.SourceBranch);
          }
          catch (Exception ex)
          {
            this.m_build.ValidationResults.Add(new BuildRequestValidationResult()
            {
              Result = ValidationResult.Error,
              Message = BuildServerResources.ErrorResolvingVersionFromBranch((object) this.m_build.SourceBranch, (object) ex.Message)
            });
            return;
          }
        }
        if (string.IsNullOrEmpty(this.m_build.SourceVersion))
        {
          this.m_build.ValidationResults.Add(new BuildRequestValidationResult()
          {
            Result = ValidationResult.Error,
            Message = BuildServerResources.UnableToResolveVersionFromRef((object) this.m_build.SourceBranch)
          });
        }
        else
        {
          queue = this.GetQueue(requestContext);
          PipelineResources pipelineResources = requestContext.GetService<IBuildResourceAuthorizationService>().GetAuthorizedResources(requestContext, this.m_definition.ProjectId, this.m_definition.Id).ToPipelineResources();
          bool flag = yamlProcess.SupportsYamlRepositoryEndpointAuthorization();
          bool includeCheckoutOptions = requestContext.IsFeatureEnabled("DistributedTask.IncludeCheckoutOptions");
          string a;
          string sourceVersion;
          Microsoft.TeamFoundation.DistributedTask.Pipelines.RepositoryResource repository = !this.m_build.TriggerInfo.TryGetValue("pr.triggerRepository", out a) || string.Equals(a, this.m_definition.Repository.Id, StringComparison.OrdinalIgnoreCase) || !this.m_definition.Repository.Properties.TryGetValue("sourceVersion", out sourceVersion) ? this.m_definition.Repository.ToRepositoryResource(requestContext, PipelineConstants.SelfAlias, this.m_build.SourceBranch, this.m_build.SourceVersion, includeCheckoutOptions) : this.m_definition.Repository.ToRepositoryResource(requestContext, PipelineConstants.SelfAlias, this.m_definition.Repository.DefaultBranch, sourceVersion, includeCheckoutOptions);
          if (repository.Endpoint != null && !flag)
            pipelineResources.Endpoints.Add(repository.Endpoint);
          PipelineBuilder pipelineBuilder = this.GetPipelineBuilder(requestContext, queue, pipelineResources, evaluateCounters: !requestContext.IsFeatureEnabled("Build2.DoNotIncreaseCountersWhenDownloadingYAML") || !this.m_build.JustInTime.PreviewRun);
          pipelineBuilder.SystemVariables[WellKnownDistributedTaskVariables.EnableAccessToken] = (VariableValue) "SecretVariable";
          string str;
          bool result;
          if (((!this.m_definition.Repository.Properties.TryGetValue("skipSyncSource", out str) ? 0 : (bool.TryParse(str, out result) ? 1 : 0)) & (result ? 1 : 0)) != 0)
            pipelineBuilder.SystemVariables["agent.source.skip"] = (VariableValue) bool.TrueString;
          string yamlFilename = this.GetYamlFilename(requestContext, yamlProcess);
          ArtifactResolver.SetPipelineTemplateParameter(requestContext, this.m_build.TriggerInfo, this.m_build.TemplateParameters);
          YamlPipelineLoadResult pipelineLoadResult = this.LoadYamlPipelineTemplate(requestContext, repository, yamlFilename, pipelineBuilder, ref validationFlags);
          if (pipelineLoadResult == null || this.m_build.StagesToSkip.Count > 0 && !this.ValidateStagesToSkip(pipelineLoadResult.Template.Stages))
            return;
          if (this.IsUntrustedForkedBuild() && !BuildRequestHelper.AllowSecretsToFork(requestContext, this.m_definition))
            pipelineLoadResult.Environment.Options.RestrictSecrets = true;
          this.UpdatePipelineResources(requestContext, pipelineLoadResult.Environment, pipelineLoadResult.Template);
          this.AddContainerResourceVariables(requestContext, pipelineLoadResult.Environment);
          environment = (IOrchestrationEnvironment) pipelineLoadResult.Environment;
          IList<Microsoft.TeamFoundation.DistributedTask.Pipelines.Stage> stages = pipelineLoadResult.Template.Stages;
          if (this.IsSkippedStagesEnabled(requestContext) && this.m_build.StagesToSkip.Count > 0)
          {
            stages.ForEach<Microsoft.TeamFoundation.DistributedTask.Pipelines.Stage>((Action<Microsoft.TeamFoundation.DistributedTask.Pipelines.Stage>) (s => s.Skip = this.m_build.StagesToSkip.Contains(s.Name)));
            requestContext.TraceAlways(12030273, TraceLevel.Info, "Build2", nameof (BuildRequestHelper), "Manually skipped stages: " + string.Join(",", (IEnumerable<string>) this.m_build.StagesToSkip));
          }
          orchestration = (IOrchestrationProcess) new PipelineProcess(stages);
          this.PipelineInitializationLog = pipelineLoadResult.Template.InitializationLog;
          this.PipelineExpandedYaml = pipelineLoadResult.Template.ExpandedYaml;
        }
      }
    }

    private bool IsSkippedStagesEnabled(IVssRequestContext requestContext) => !requestContext.GetService<IVssRegistryService>().GetValue<bool>(requestContext, (RegistryQuery) "/Service/Build/Settings/OrgSettings/DisableStageChooser", false);

    private void AddContainerResourceVariables(
      IVssRequestContext requestContext,
      PipelineEnvironment environment)
    {
      IList<IVariable> variables = new ContainerProvider().GetVariables(requestContext, this.m_build.ProjectId, environment.Resources.Containers, this.m_build.TriggerInfo);
      environment.UserVariables.AddRange<IVariable, IList<IVariable>>((IEnumerable<IVariable>) variables);
    }

    private void UpdatePipelineResources(
      IVssRequestContext requestContext,
      PipelineEnvironment environment,
      PipelineTemplate template)
    {
      if (this.m_build.Resources == null)
        return;
      if (requestContext.IsFeatureEnabled("Build2.SetPipelineResources") && this.m_build.Resources.Pipelines.Count > 0)
      {
        foreach (KeyValuePair<string, PipelineResourceParameters> pipeline in this.m_build.Resources.Pipelines)
        {
          KeyValuePair<string, PipelineResourceParameters> pair = pipeline;
          Microsoft.TeamFoundation.DistributedTask.Pipelines.PipelineResource pipelineResource = environment.Resources.Pipelines.FirstOrDefault<Microsoft.TeamFoundation.DistributedTask.Pipelines.PipelineResource>((Func<Microsoft.TeamFoundation.DistributedTask.Pipelines.PipelineResource, bool>) (p => p.Alias.Equals(pair.Key, StringComparison.OrdinalIgnoreCase)));
          if (pipelineResource != null)
            pipelineResource.Version = pair.Value.Version;
        }
      }
      if (requestContext.IsFeatureEnabled("Build2.SetBuildResources") && this.m_build.Resources.Builds.Count > 0)
      {
        foreach (KeyValuePair<string, BuildResourceParameters> build1 in this.m_build.Resources.Builds)
        {
          KeyValuePair<string, BuildResourceParameters> pair = build1;
          BuildResource build2 = environment.Resources.Builds.FirstOrDefault<BuildResource>((Func<BuildResource, bool>) (p => p.Alias.Equals(pair.Key, StringComparison.OrdinalIgnoreCase)));
          if (build2 != null)
            ArtifactResolver.SetBuildResourceVersion(requestContext, build2, template?.Stages, pair.Value.Version);
        }
      }
      if (requestContext.IsFeatureEnabled("Build2.SetContainerResources") && this.m_build.Resources.Containers.Count > 0)
      {
        foreach (KeyValuePair<string, ContainerResourceParameters> container in this.m_build.Resources.Containers)
        {
          KeyValuePair<string, ContainerResourceParameters> pair = container;
          Microsoft.TeamFoundation.DistributedTask.Pipelines.ContainerResource containerResource = environment.Resources.Containers.FirstOrDefault<Microsoft.TeamFoundation.DistributedTask.Pipelines.ContainerResource>((Func<Microsoft.TeamFoundation.DistributedTask.Pipelines.ContainerResource, bool>) (p => p.Alias.Equals(pair.Key, StringComparison.OrdinalIgnoreCase)));
          if (containerResource != null)
            ArtifactResolver.SetContainerResourceVersion(containerResource, pair.Value.Version);
        }
      }
      if (!requestContext.IsFeatureEnabled("Build2.SetPackageResources") || this.m_build.Resources.Packages.Count <= 0)
        return;
      foreach (KeyValuePair<string, PackageResourceParameters> package1 in this.m_build.Resources.Packages)
      {
        KeyValuePair<string, PackageResourceParameters> pair = package1;
        PackageResource package2 = environment.Resources.Packages.FirstOrDefault<PackageResource>((Func<PackageResource, bool>) (p => p.Alias.Equals(pair.Key, StringComparison.OrdinalIgnoreCase)));
        if (package2 != null)
          ArtifactResolver.SetPackageResourceVersion(requestContext, package2, template?.Stages, pair.Value.Version);
      }
    }

    private bool ValidateStagesToSkip(IList<Microsoft.TeamFoundation.DistributedTask.Pipelines.Stage> pipelineStages)
    {
      HashSet<string> second = new HashSet<string>(pipelineStages.Select<Microsoft.TeamFoundation.DistributedTask.Pipelines.Stage, string>((Func<Microsoft.TeamFoundation.DistributedTask.Pipelines.Stage, string>) (s => s.Name)), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      List<string> list = this.m_build.StagesToSkip.Except<string>((IEnumerable<string>) second).Select<string, string>((Func<string, string>) (stage => BuildServerResources.InvalidStageToSkip((object) stage))).ToList<string>();
      if (this.m_build.StagesToSkip.Count - list.Count >= second.Count)
        list.Add(BuildServerResources.SkippingAllStages());
      this.m_build.ValidationResults.AddRange(list.Select<string, BuildRequestValidationResult>((Func<string, BuildRequestValidationResult>) (x => new BuildRequestValidationResult()
      {
        Message = x,
        Result = ValidationResult.Error
      })));
      return list.Count == 0;
    }

    private void SetUpDesignerProcessEnvironment(
      IVssRequestContext requestContext,
      DesignerProcess designerProcess,
      IList<IBuildOption> options,
      List<Demand> demands,
      out AgentPoolQueue queue,
      out IOrchestrationEnvironment environment,
      out IOrchestrationProcess orchestration)
    {
      queue = this.GetQueue(requestContext);
      environment = (IOrchestrationEnvironment) null;
      orchestration = (IOrchestrationProcess) null;
      AgentSpecification agentSpecification = this.GetAgentSpecification(requestContext, designerProcess);
      PipelineBuildResult pipelineBuildResult = this.BuildPipelineProcess(requestContext, designerProcess, queue, options, (IReadOnlyList<Demand>) demands, agentSpecification);
      if (pipelineBuildResult.Errors.Count > 0)
      {
        this.m_build.ValidationResults.AddRange(pipelineBuildResult.Errors.Select<PipelineValidationError, BuildRequestValidationResult>((Func<PipelineValidationError, BuildRequestValidationResult>) (x => new BuildRequestValidationResult()
        {
          Message = x.Message,
          Result = ValidationResult.Error
        })));
      }
      else
      {
        if (this.IsUntrustedForkedBuild() && !BuildRequestHelper.AllowSecretsToFork(requestContext, this.m_definition))
          pipelineBuildResult.Environment.Options.RestrictSecrets = true;
        environment = (IOrchestrationEnvironment) pipelineBuildResult.Environment;
        orchestration = (IOrchestrationProcess) pipelineBuildResult.Process;
        if (!(this.m_definition.Triggers.FirstOrDefault<BuildTrigger>((Func<BuildTrigger, bool>) (x => x.TriggerType == DefinitionTriggerType.GatedCheckIn)) is GatedCheckInTrigger))
          return;
        if (designerProcess.Phases.Count != 1)
          this.m_build.ValidationResults.Add(new BuildRequestValidationResult()
          {
            Message = BuildServerResources.GatedCheckinRequiresSinglePhase((object) this.m_definition.Name),
            Result = ValidationResult.Error
          });
        else if (!(designerProcess.Phases[0].Target is AgentPoolQueueTarget))
        {
          this.m_build.ValidationResults.Add(new BuildRequestValidationResult()
          {
            Message = BuildServerResources.GatedCheckinRequiresAgentPhase((object) this.m_definition.Name),
            Result = ValidationResult.Error
          });
        }
        else
        {
          if (!((designerProcess.Phases[0].Target as AgentPoolQueueTarget).ExecutionOptions is MultipleAgentExecutionOptions))
            return;
          this.m_build.ValidationResults.Add(new BuildRequestValidationResult()
          {
            Message = BuildServerResources.MultiAgentNotSupportedForGatedCheckin((object) this.m_definition.Name),
            Result = ValidationResult.Error
          });
        }
      }
    }

    private void CloneEnvironment(
      IVssRequestContext requestContext,
      IList<IBuildOption> options,
      ref BuildRequestValidationFlags validationFlags,
      out AgentPoolQueue queue,
      out IOrchestrationEnvironment environment,
      out IOrchestrationProcess orchestration)
    {
      TaskOrchestrationPlan originalPlan = this.GetOriginalPlan(requestContext);
      if (originalPlan == null)
      {
        this.SetupEnvironment(requestContext, options, this.m_build.Demands, ref validationFlags, out queue, out environment, out orchestration);
      }
      else
      {
        if (this.m_definition.Process is YamlProcess process)
        {
          PipelineResources pipelineResources = requestContext.GetService<IBuildResourceAuthorizationService>().GetAuthorizedResources(requestContext, this.m_definition.ProjectId, this.m_definition.Id).ToPipelineResources();
          PipelineBuilder pipelineBuilder = this.GetPipelineBuilder(requestContext, this.m_definition.Queue, pipelineResources);
          Microsoft.TeamFoundation.DistributedTask.Pipelines.RepositoryResource repositoryResource = this.m_definition.Repository.ToRepositoryResource(requestContext, PipelineConstants.SelfAlias, this.m_build.SourceBranch, this.m_build.SourceVersion);
          string yamlFilename = this.GetYamlFilename(requestContext, process);
          this.LoadYamlPipelineTemplate(requestContext, repositoryResource, yamlFilename, pipelineBuilder, ref validationFlags);
        }
        else
        {
          PipelineBuildContext buildContext = this.GetPipelineBuilder(requestContext, this.m_definition.Queue).CreateBuildContext(BuildOptions.None);
          this.m_build.BuildNumber = new BuildRequestHelper.BuildNumberFormatter(requestContext, this.m_definition, this.m_build, BuildRequestHelper.s_defaultBuildNumberFormat).Format((IPipelineContext) buildContext, this.m_definition.BuildNumberFormat);
        }
        environment = originalPlan.ProcessEnvironment;
        orchestration = originalPlan.Process;
        queue = this.m_definition.Queue;
      }
      this.m_build.OrchestrationPlan = (TaskOrchestrationPlanReference) null;
    }

    internal virtual TaskOrchestrationPlan GetOriginalPlan(IVssRequestContext requestContext) => requestContext.GetService<IDistributedTaskHubService>().GetTaskHub(requestContext, "Build").GetPlan(requestContext, this.m_build.ProjectId, this.m_build.OrchestrationPlan.PlanId, true);

    private IReadOnlyList<JustInTimeConfigurationFile> GetJustInTimeConfigurations(
      IVssRequestContext requestContext)
    {
      return !this.IsYamlProvidedJustInTime(requestContext) ? (IReadOnlyList<JustInTimeConfigurationFile>) null : (IReadOnlyList<JustInTimeConfigurationFile>) new List<JustInTimeConfigurationFile>((IEnumerable<JustInTimeConfigurationFile>) this.m_build.JustInTime.Configuration.Files);
    }

    private string GetYamlFilename(IVssRequestContext requestContext, YamlProcess yamlProcess) => !this.IsYamlProvidedJustInTime(requestContext) ? yamlProcess.YamlFilename : this.m_build.JustInTime.Configuration.MainFilePath;

    private YamlPipelineLoadResult LoadYamlPipelineTemplate(
      IVssRequestContext requestContext,
      Microsoft.TeamFoundation.DistributedTask.Pipelines.RepositoryResource repository,
      string fileName,
      PipelineBuilder environment,
      ref BuildRequestValidationFlags validationFlags)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(fileName, nameof (fileName));
      if (this.m_build == null || this.m_build.ValidationResults == null)
        requestContext.TraceAlways(12030294, TraceLevel.Error, "Build2", nameof (BuildRequestHelper), string.Format("m_build value is: {0} and m_build.ValidationResults value is: {1}", (object) this.m_build, (object) this.m_build.ValidationResults));
      try
      {
        IDictionary<string, Microsoft.TeamFoundation.DistributedTask.Pipelines.RepositoryResource> repositoryOverrides = (IDictionary<string, Microsoft.TeamFoundation.DistributedTask.Pipelines.RepositoryResource>) null;
        if (this.m_build.Resources != null)
          repositoryOverrides = (IDictionary<string, Microsoft.TeamFoundation.DistributedTask.Pipelines.RepositoryResource>) this.m_build.Resources.Repositories.ToDictionary<KeyValuePair<string, RepositoryResourceParameters>, string, Microsoft.TeamFoundation.DistributedTask.Pipelines.RepositoryResource>((Func<KeyValuePair<string, RepositoryResourceParameters>, string>) (pair => pair.Key), (Func<KeyValuePair<string, RepositoryResourceParameters>, Microsoft.TeamFoundation.DistributedTask.Pipelines.RepositoryResource>) (pair =>
          {
            return new Microsoft.TeamFoundation.DistributedTask.Pipelines.RepositoryResource()
            {
              Alias = pair.Key,
              Name = pair.Key,
              Ref = pair.Value.RefName,
              Version = pair.Value.Version
            };
          }));
        AgentPoolQueue queue = this.GetQueue(requestContext);
        CheckoutOptions checkoutOptions = requestContext.IsFeatureEnabled("DistributedTask.IncludeCheckoutOptions") ? (CheckoutOptions) null : this.m_definition.Repository.ToCheckoutOptions();
        Microsoft.TeamFoundation.DistributedTask.Pipelines.WorkspaceOptions workspaceOptions = this.m_definition.Repository.ToWorkspaceOptions();
        YamlPipelineLoadResult pipelineLoadResult = requestContext.GetService<IYamlPipelineLoaderService>().Load(requestContext, this.m_build.ProjectId, repository, fileName, environment, queue?.Id, checkoutOptions, workspaceOptions, (IYamlNameFormatter) new BuildRequestHelper.BuildNumberFormatter(requestContext, this.m_definition, this.m_build, "$(date:yyyyMMdd)$(rev:.r)"), validateResources: false, yamlOverride: this.m_build.JustInTime.YamlOverride, templateParameters: (IDictionary<string, object>) this.m_build.TemplateParameters, repositoryOverrides: repositoryOverrides);
        if (pipelineLoadResult == null)
          requestContext.TraceAlways(12030294, TraceLevel.Error, "Build2", nameof (BuildRequestHelper), string.Format("loadResult value is: {0}", (object) pipelineLoadResult));
        string prTriggeringRepository;
        if (this.m_build.TriggerInfo.TryGetValue("pr.triggerRepository", out prTriggeringRepository) && pipelineLoadResult.Environment != null && pipelineLoadResult.Environment.Resources != null && pipelineLoadResult.Environment.Resources.Repositories != null)
        {
          Microsoft.TeamFoundation.DistributedTask.Pipelines.RepositoryResource repositoryResource = pipelineLoadResult.Environment.Resources.Repositories.Where<Microsoft.TeamFoundation.DistributedTask.Pipelines.RepositoryResource>((Func<Microsoft.TeamFoundation.DistributedTask.Pipelines.RepositoryResource, bool>) (r => r?.Id == prTriggeringRepository)).FirstOrDefault<Microsoft.TeamFoundation.DistributedTask.Pipelines.RepositoryResource>();
          if (repositoryResource != null)
          {
            repositoryResource.Version = this.m_build.SourceVersion;
            repositoryResource.Ref = this.m_build.SourceBranch;
            repositoryResource.Properties.Set<bool>("system.istriggeringrepository", true);
          }
        }
        this.m_build.BuildNumber = pipelineLoadResult.Template.Name;
        this.m_build.AppendCommitMessageToRunName = pipelineLoadResult.Template.AppendCommitMessageToRunName;
        if (pipelineLoadResult.Template.Errors.Count == 0)
          return pipelineLoadResult;
        foreach (PipelineValidationError error in (IEnumerable<PipelineValidationError>) pipelineLoadResult.Template.Errors)
          this.m_build.ValidationResults.Add(new BuildRequestValidationResult()
          {
            Result = ValidationResult.Error,
            Message = error.Message
          });
        return (YamlPipelineLoadResult) null;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(nameof (BuildRequestHelper), ex);
        if (ex is YamlFileNotFoundException)
          validationFlags &= ~BuildRequestValidationFlags.QueueFailedBuild;
        BuildData build = this.m_build;
        if (build != null)
        {
          // ISSUE: explicit non-virtual call
          List<BuildRequestValidationResult> validationResults = __nonvirtual (build.ValidationResults);
          if (validationResults != null)
          {
            // ISSUE: explicit non-virtual call
            __nonvirtual (validationResults.Add(new BuildRequestValidationResult()
            {
              Result = ValidationResult.Error,
              Message = BuildServerResources.YamlLoadError((object) ex.Message)
            }));
          }
        }
        return (YamlPipelineLoadResult) null;
      }
    }

    internal PipelineBuildResult BuildPipelineProcess(
      IVssRequestContext requestContext,
      DesignerProcess designerProcess,
      AgentPoolQueue defaultQueue,
      IList<IBuildOption> options,
      IReadOnlyList<Demand> queueTimeDemands,
      AgentSpecification agentSpecification)
    {
      using (requestContext.TraceScope(nameof (BuildRequestHelper), nameof (BuildPipelineProcess)))
      {
        PipelineBuilder pipelineBuilder = this.GetPipelineBuilder(requestContext, defaultQueue, defaultAgentSpecification: agentSpecification);
        BuildOptions options1 = new BuildOptions()
        {
          ValidateTaskInputs = true
        };
        PipelineBuildContext buildContext = pipelineBuilder.CreateBuildContext(options1);
        PipelineBuildResult pipelineBuildResult = pipelineBuilder.Build(designerProcess.ToPipelineProcess(requestContext, this.m_definition, (IPipelineContext) buildContext, (IEnumerable<Demand>) queueTimeDemands, this.m_build.SourceBranch, this.m_build.SourceVersion).Stages, options1);
        pipelineBuildResult.Environment.Resources.MergeWith(pipelineBuildResult.ReferencedResources);
        this.m_build.BuildNumber = new BuildRequestHelper.BuildNumberFormatter(requestContext, this.m_definition, this.m_build, BuildRequestHelper.s_defaultBuildNumberFormat).Format((IPipelineContext) buildContext, this.m_definition.BuildNumberFormat);
        return pipelineBuildResult;
      }
    }

    private TaskOrchestrationContainer CreateContainerProcess(
      IVssRequestContext requestContext,
      PlanEnvironment environment,
      DesignerProcess designerProcess,
      IList<IBuildOption> options,
      List<Demand> demands,
      List<TaskInstance> tasksToInjectUpFront,
      bool includeTaskDemands = true)
    {
      using (requestContext.TraceScope(nameof (BuildRequestHelper), nameof (CreateContainerProcess)))
      {
        TaskOrchestrationContainer container = new TaskOrchestrationContainer()
        {
          Parallel = false
        };
        foreach (Phase phase in designerProcess.Phases)
        {
          Dictionary<string, string> variables = new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
          string executionMode = "Agent";
          if (phase.Target is ServerTarget)
          {
            executionMode = "Server";
            variables[WellKnownDistributedTaskVariables.EnableAccessToken] = "true";
          }
          string name = string.IsNullOrEmpty(phase.Name) ? BuildServerResources.BuildJobName() : phase.Name;
          string referenceName = string.IsNullOrEmpty(phase.Name) ? BuildServerResources.BuildJobName() : phase.Name;
          TaskOrchestrationJob job = this.CreateJob(requestContext, name, referenceName, executionMode, this.m_definition.JobTimeoutInMinutes, this.m_build.Reason, demands, phase, variables, tasksToInjectUpFront, includeTaskDemands);
          if (job == null)
            return (TaskOrchestrationContainer) null;
          AgentPoolQueueTarget target = phase.Target as AgentPoolQueueTarget;
          IVariableMultiplierExecutionOptions multiplierOptions = phase.Target.GetMultiplierOptions();
          IDictionary<string, List<string>> multiplierValues = (IDictionary<string, List<string>>) null;
          MultipleAgentExecutionOptions executionOptions = target?.ExecutionOptions as MultipleAgentExecutionOptions;
          if (multiplierOptions != null && multiplierOptions.Multipliers.Count > 0)
            multiplierValues = MultiplierHelpers.GetMultiplierValues(((IOrchestrationEnvironment) environment).Variables, (IReadOnlyList<string>) multiplierOptions.Multipliers);
          if (executionOptions != null)
          {
            TaskOrchestrationContainer orchestrationContainer = new TaskOrchestrationContainer()
            {
              Parallel = executionOptions.MaxConcurrency > 1,
              MaxConcurrency = executionOptions.MaxConcurrency,
              ContinueOnError = executionOptions.ContinueOnError
            };
            orchestrationContainer.Children.AddRange((IEnumerable<TaskOrchestrationItem>) MultiplierHelpers.MultiplyJob(job, executionOptions.MaxConcurrency));
            container.Children.Add((TaskOrchestrationItem) orchestrationContainer);
          }
          else if (multiplierValues == null || multiplierValues.Count == 0)
          {
            container.Children.Add((TaskOrchestrationItem) job);
          }
          else
          {
            TaskOrchestrationContainer orchestrationContainer = new TaskOrchestrationContainer()
            {
              Parallel = multiplierOptions.MaxConcurrency > 1,
              MaxConcurrency = multiplierOptions.MaxConcurrency,
              ContinueOnError = multiplierOptions.ContinueOnError
            };
            orchestrationContainer.Children.AddRange((IEnumerable<TaskOrchestrationItem>) MultiplierHelpers.MultiplyJob(job, multiplierValues));
            container.Children.Add((TaskOrchestrationItem) orchestrationContainer);
          }
        }
        this.m_definition.ApplyPreValidationOptions(options, (IOrchestrationEnvironment) environment);
        this.m_build.PopulateHostedAgentImageId(container);
        this.m_build.BuildNumber = new BuildRequestHelper.BuildNumberFormatter(requestContext, this.m_definition, this.m_build, BuildRequestHelper.s_defaultBuildNumberFormat).Format((IDictionary<string, VariableValue>) new VariablesDictionary(environment.Variables), this.m_definition.BuildNumberFormat);
        return container;
      }
    }

    public AgentPoolQueue GetQueue(IVssRequestContext requestContext)
    {
      if (this.m_agentQueueResolved)
        return this.m_agentPoolQueue;
      if (this.m_build.QueueId.HasValue)
      {
        IDistributedTaskPoolService service = requestContext.GetService<IDistributedTaskPoolService>();
        this.m_agentPoolQueue = service.GetAgentQueue(requestContext, this.m_build.ProjectId, this.m_build.QueueId.Value).AsServerBuildQueue();
        if (this.m_agentPoolQueue == null)
        {
          BuildProcess process = this.m_definition.Process;
          switch (process != null ? process.Type : 1)
          {
            case 1:
              this.m_build.ValidationResults.Add(new BuildRequestValidationResult()
              {
                Result = ValidationResult.Error,
                Message = BuildServerResources.QueueNotFound((object) this.m_build.QueueId.Value)
              });
              break;
            case 2:
              string queueName = requestContext.IsFeatureEnabled("Build2.NewDefaultQueue") ? "Azure Pipelines" : "Hosted Ubuntu 1604";
              TaskAgentQueue queue = (service.GetAgentQueues(requestContext, this.m_build.ProjectId, queueName).SingleOrDefault<TaskAgentQueue>() ?? service.GetAgentQueues(requestContext, this.m_build.ProjectId, "Default").SingleOrDefault<TaskAgentQueue>()) ?? service.GetAgentQueues(requestContext, this.m_build.ProjectId).FirstOrDefault<TaskAgentQueue>();
              this.m_agentPoolQueue = queue != null ? queue.AsServerBuildQueue() : (AgentPoolQueue) null;
              break;
          }
        }
      }
      else
        this.m_build.ValidationResults.Add(new BuildRequestValidationResult()
        {
          Result = ValidationResult.Error,
          Message = BuildServerResources.NoQueueSpecified()
        });
      this.m_agentQueueResolved = true;
      return this.m_agentPoolQueue;
    }

    private AgentSpecification GetAgentSpecification(
      IVssRequestContext requestContext,
      DesignerProcess designerProcess)
    {
      if (this.m_build.AgentSpecification != null)
        return this.m_build.AgentSpecification;
      int? queueId = this.m_build.QueueId;
      int? id = this.m_definition.Queue?.Id;
      if (!(queueId.GetValueOrDefault() == id.GetValueOrDefault() & queueId.HasValue == id.HasValue))
        return (AgentSpecification) null;
      return designerProcess.Target?.AgentSpecification;
    }

    internal override void SetupEnvironment(
      IVssRequestContext requestContext,
      PipelineBuilder builder)
    {
      if (builder.EnvironmentVersion < 2)
      {
        foreach (KeyValuePair<string, string> environmentVariable in (IEnumerable<KeyValuePair<string, string>>) this.m_sourceProvider.GetJobEnvironmentVariables(requestContext, this.m_build))
          builder.SystemVariables[environmentVariable.Key] = (VariableValue) environmentVariable.Value;
      }
      this.PopulateWellKnownVariables(requestContext, builder.SystemVariables);
      VariablesDictionary variablesDictionary = new VariablesDictionary();
      IDictionary<string, string> dictionary = BuildRequestHelper.DeserializeParameters(this.m_build.Parameters);
      if (dictionary != null && dictionary.Count > 0)
      {
        bool flag = requestContext.IsTracing(12030119, TraceLevel.Info, "Build2", nameof (BuildRequestHelper));
        foreach (KeyValuePair<string, string> keyValuePair in (IEnumerable<KeyValuePair<string, string>>) dictionary)
        {
          if (flag && string.Equals(keyValuePair.Key, WellKnownDistributedTaskVariables.EnableAccessToken, StringComparison.OrdinalIgnoreCase))
            requestContext.TraceInfo(12030119, nameof (BuildRequestHelper), "Detected");
          builder.UserVariables.Add((IVariable) new Microsoft.TeamFoundation.DistributedTask.Pipelines.Variable()
          {
            Name = keyValuePair.Key,
            Value = keyValuePair.Value,
            Readonly = true
          });
        }
      }
      if (this.m_build.JustInTime.Secrets != null)
      {
        foreach (KeyValuePair<string, string> secret in this.m_build.JustInTime.Secrets)
          builder.UserVariables.Add((IVariable) new Microsoft.TeamFoundation.DistributedTask.Pipelines.Variable()
          {
            Name = secret.Key,
            Value = secret.Value,
            Secret = true
          });
      }
      if (this.m_build.Reason != BuildReason.BuildCompletion)
        return;
      IBuildDefinitionService service = requestContext.GetService<IBuildDefinitionService>();
      this.SetBuildCompletionEnvironmentVariables(requestContext, service, builder.SystemVariables);
    }

    internal void SanitizeBuildParameters(
      PipelineBuildContext context,
      IDictionary<string, string> deserializedParameters)
    {
      Lazy<List<string>> lazy = new Lazy<List<string>>((Func<List<string>>) (() => new List<string>()));
      if (deserializedParameters == null || deserializedParameters.Count <= 0)
        return;
      foreach (string key in (IEnumerable<string>) deserializedParameters.Keys)
      {
        VariableValue variableValue;
        if (context.Variables.TryGetValue(key, out variableValue) && variableValue.IsSecret)
          lazy.Value.Add(key);
      }
      if (lazy.IsValueCreated)
      {
        foreach (string key in lazy.Value)
          deserializedParameters.Remove(key);
      }
      if (deserializedParameters != null && deserializedParameters.Count == 0)
      {
        this.m_build.Parameters = (string) null;
      }
      else
      {
        if (!lazy.IsValueCreated || lazy.Value.Count <= 0)
          return;
        this.m_build.Parameters = JsonUtility.ToString((object) deserializedParameters);
      }
    }

    internal static string SanitizeDiagnosticsParameters(
      string parameters,
      out HashSet<string> internalRuntimeVariables)
    {
      internalRuntimeVariables = new HashSet<string>();
      IDictionary<string, string> dictionary = BuildRequestHelper.DeserializeParameters(parameters);
      if (dictionary == null && !string.IsNullOrEmpty(parameters))
        throw new ArgumentException(BuildServerResources.InvalidBuildParametersJson((object) parameters)).Expected("Build2");
      if (dictionary == null || dictionary != null && dictionary.Count == 0)
        return parameters;
      Dictionary<string, string> toSerialize = new Dictionary<string, string>();
      foreach (KeyValuePair<string, string> keyValuePair in (IEnumerable<KeyValuePair<string, string>>) dictionary)
      {
        if (BuildRequestHelper.IsDiagnosticsParameter(keyValuePair.Key))
        {
          internalRuntimeVariables.Add(keyValuePair.Key);
          toSerialize.Add(keyValuePair.Key, BuildRequestHelper.SanitizeBooleanParameter(keyValuePair.Value).ToString());
        }
        else
          toSerialize.Add(keyValuePair.Key, keyValuePair.Value);
      }
      return JsonUtility.ToString((object) toSerialize);
    }

    private static bool IsDiagnosticsParameter(string paramKey) => string.Equals(paramKey, "agent.diagnostic", StringComparison.OrdinalIgnoreCase) || string.Equals(paramKey, "system.debug", StringComparison.OrdinalIgnoreCase);

    private static bool SanitizeBooleanParameter(string value)
    {
      bool result;
      bool.TryParse(value, out result);
      return result;
    }

    internal override PlanEnvironment BuildContainerEnvironment(
      IVssRequestContext requestContext,
      out List<TaskInstance> tasksToInject)
    {
      IList<Microsoft.TeamFoundation.DistributedTask.WebApi.VariableGroup> variableGroups = (IList<Microsoft.TeamFoundation.DistributedTask.WebApi.VariableGroup>) null;
      if (this.m_definition.VariableGroups.Count > 0)
        variableGroups = requestContext.GetService<IVariableGroupService>().GetVariableGroups(requestContext.Elevate(), this.m_definition.ProjectId, (IList<int>) this.m_definition.VariableGroups.Select<VariableGroup, int>((Func<VariableGroup, int>) (x => x.Id)).Distinct<int>().ToList<int>());
      IBuildDefinitionService service = requestContext.GetService<IBuildDefinitionService>();
      Dictionary<string, string> dictionary1 = new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      service.ReadSecretVariables(requestContext, this.m_definition, (IDictionary<string, string>) dictionary1, (IDictionary<string, string>) null);
      IDictionary<string, string> dictionary2 = BuildRequestHelper.DeserializeParameters(this.m_build.Parameters);
      if (dictionary2 != null && dictionary2.Count > 0 && requestContext.IsTracing(12030119, TraceLevel.Info, "Build2", nameof (BuildRequestHelper)))
      {
        foreach (KeyValuePair<string, string> keyValuePair in (IEnumerable<KeyValuePair<string, string>>) dictionary2)
        {
          if (string.Equals(keyValuePair.Key, WellKnownDistributedTaskVariables.EnableAccessToken, StringComparison.OrdinalIgnoreCase))
            requestContext.TraceInfo(12030119, nameof (BuildRequestHelper), "Detected");
        }
      }
      VariablesDictionary variablesDictionary = new VariablesDictionary();
      foreach (KeyValuePair<string, string> environmentVariable in (IEnumerable<KeyValuePair<string, string>>) this.m_sourceProvider.GetJobEnvironmentVariables(requestContext, this.m_build))
        variablesDictionary[environmentVariable.Key] = (VariableValue) environmentVariable.Value;
      this.PopulateWellKnownVariables(requestContext, (IDictionary<string, VariableValue>) variablesDictionary);
      this.m_definition.AddProcessParametersAsVariables(requestContext, (IDictionary<string, VariableValue>) variablesDictionary);
      PlanEnvironment environment = new PlanEnvironment();
      if (this.m_build.Reason == BuildReason.BuildCompletion)
        this.SetBuildCompletionEnvironmentVariables(requestContext, service, (IDictionary<string, VariableValue>) variablesDictionary);
      VariablesHelper.PopulateVariables(requestContext, this.m_definition.ProjectId, (IOrchestrationEnvironment) environment, variableGroups, (IDictionary<string, BuildDefinitionVariable>) this.m_definition.Variables, dictionary2, (IDictionary<string, string>) variablesDictionary, (IDictionary<string, string>) dictionary1, out tasksToInject);
      if (dictionary2 != null && dictionary2.Count > 0)
        this.m_build.Parameters = dictionary2.Count != 0 ? JsonUtility.ToString((object) dictionary2) : (string) null;
      return environment;
    }

    internal override void PopulateWellKnownVariables(
      IVssRequestContext requestContext,
      IDictionary<string, VariableValue> variables)
    {
      base.PopulateWellKnownVariables(requestContext, variables);
      string str1 = (string) null;
      string str2 = (string) null;
      string str3 = (string) null;
      try
      {
        this.m_build.TriggerInfo.TryGetValue("ci.sourceSha", out str1);
        if (this.m_build.TriggerInfo.TryGetValue("ci.sourceBranch", out str2))
        {
          str2 = GitRefspecHelper.NormalizeSourceBranch(str2);
          str3 = this.GetShortBranchName(str2);
        }
        string str4;
        if (this.m_build.TriggerInfo.TryGetValue("ci.message", out str4))
          variables["build.sourceVersionMessage"] = (VariableValue) str4;
        string str5;
        if (this.m_build.TriggerInfo.TryGetValue("scheduleName", out str5))
          variables["build.cronSchedule.displayName"] = (VariableValue) str5;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(nameof (BuildRequestHelper), ex);
      }
      variables["build.sourceVersion"] = (VariableValue) (str1 ?? this.m_build.SourceVersion);
      variables["build.sourceBranch"] = (VariableValue) (str2 ?? this.m_build.SourceBranch);
      variables["build.sourceBranchName"] = (VariableValue) (str3 ?? this.m_build.GetShortBranchName());
      variables["build.reason"] = (VariableValue) this.m_build.Reason.ToString();
      variables["system.pullRequest.isFork"] = (VariableValue) (this.IsForkBuild() || this.IsForkBuildResourceTriggered()).ToString();
      variables[WellKnownDistributedTaskVariables.JobParallelismTag] = (VariableValue) this.GetJobParallelismType(requestContext);
    }

    private bool IsForkBuild()
    {
      bool result;
      return bool.TryParse(this.m_build.TriggerInfo.GetValueOrDefault<string, string>("pr.isFork", bool.FalseString), out result) & result;
    }

    private bool IsForkBuildResourceTriggered()
    {
      string falseString = bool.FalseString;
      if (this.m_build.Reason == BuildReason.ResourceTrigger)
        BuildRequestHelper.DeserializeParameters(this.m_build.Parameters)?.TryGetValue("system.pullRequest.isFork", out falseString);
      bool result;
      return bool.TryParse(falseString, out result) & result;
    }

    private bool IsAzureRepoBuild() => this.m_build.Repository != null && string.Equals(this.m_build.Repository.Type, "TfsGit", StringComparison.OrdinalIgnoreCase);

    private bool IsUntrustedForkedBuild() => (this.IsForkBuildResourceTriggered() || this.IsForkBuild()) && !this.IsAzureRepoBuild();

    private string GetJobParallelismType(IVssRequestContext requestContext)
    {
      if (requestContext.GetService<IProjectService>().GetProjectVisibility(requestContext, this.m_build.ProjectId) == ProjectVisibility.Public)
      {
        if (this.m_sourceProvider != null)
        {
          try
          {
            if (this.m_sourceProvider.GetRepositoryVisibility(requestContext, this.m_build.ProjectId, this.m_repository) == RepositoryVisibility.Public)
              return "Public";
          }
          catch (ExternalSourceProviderException ex)
          {
            requestContext.TraceException("Service", (Exception) ex);
          }
        }
      }
      return "Private";
    }

    private bool CheckValidationResults(
      IVssRequestContext requestContext,
      BuildRequestValidationFlags validationFlags)
    {
      return !this.m_build.ValidationResults.Any<BuildRequestValidationResult>((Func<BuildRequestValidationResult, bool>) (vr =>
      {
        if (vr.Result == ValidationResult.Error)
          return true;
        return validationFlags.HasFlag((Enum) BuildRequestValidationFlags.WarningsAsErrors) && vr.Result == ValidationResult.Warning && !vr.Ignorable;
      }));
    }

    private BuildData HandleValidationFailure(
      IVssRequestContext requestContext,
      BuildRequestValidationFlags validationFlags)
    {
      this.m_build.Status = new BuildStatus?(BuildStatus.Completed);
      this.m_build.Result = !requestContext.IsFeatureEnabled("Build2.ReportUnreachableExternalRepo") ? new BuildResult?(BuildResult.Failed) : new BuildResult?(validationFlags.HasFlag((Enum) BuildRequestValidationFlags.QueueInconclusiveBuild) ? BuildResult.Canceled : BuildResult.Failed);
      this.m_build.StartTime = new DateTime?(DateTime.UtcNow);
      this.m_build.FinishTime = new DateTime?(DateTime.UtcNow);
      if (this.m_environment is PipelineEnvironment environment)
        this.m_build.RepositoryResources = environment.Resources.Repositories.ToHashSet<Microsoft.TeamFoundation.DistributedTask.Pipelines.RepositoryResource>();
      if (!validationFlags.HasFlag((Enum) BuildRequestValidationFlags.QueueFailedBuild) && (!requestContext.IsFeatureEnabled("Build2.ReportUnreachableExternalRepo") || !validationFlags.HasFlag((Enum) BuildRequestValidationFlags.QueueInconclusiveBuild)))
        return this.m_build;
      BuildDefinition definition = requestContext.GetService<BuildDefinitionService>().GetDefinition(requestContext, this.m_build.ProjectId, this.m_build.Definition.Id);
      if (definition.Repository != null && definition.Repository.Id != null)
        return requestContext.GetService<BuildService>().AddBuild(requestContext, this.m_build, this.m_requestedBy, this.m_requestedFor, (IEnumerable<ChangeData>) null);
      requestContext.TraceError(12030081, "Repository information is null for definition {0}", definition.Name);
      return (BuildData) null;
    }

    private TaskOrchestrationJob CreateJob(
      IVssRequestContext requestContext,
      string name,
      string referenceName,
      string executionMode,
      int timeoutInMinutes,
      BuildReason reason,
      List<Demand> queueTimeDemands,
      Phase phase,
      Dictionary<string, string> variables,
      List<TaskInstance> tasksToInjectUpFront,
      bool includeTaskDemands = true)
    {
      string str = (string) null;
      if (reason == BuildReason.CheckInShelveset)
        str = BuildRequestHelper.s_minGatedSupportedAgentVersion;
      if (DemandMinimumVersion.CompareVersion(BuildRequestHelper.MinServerSupportedAgentVersion, str) > 0)
        str = BuildRequestHelper.MinServerSupportedAgentVersion;
      if (phase.Steps.Any<BuildDefinitionStep>((Func<BuildDefinitionStep, bool>) (step => step.Enabled && step.TimeoutInMinutes > 0)) && DemandMinimumVersion.CompareVersion(BuildRequestHelper.s_taskTimeoutSupportedAgentVersion, BuildRequestHelper.MinServerSupportedAgentVersion) > 0)
        str = BuildRequestHelper.s_taskTimeoutSupportedAgentVersion;
      string versionForRepository = this.GetMinimumAgentVersionForRepository(str);
      TaskHub taskHub = requestContext.GetService<IDistributedTaskHubService>().GetTaskHub(requestContext, "Build");
      List<Demand> demands = new List<Demand>();
      if (queueTimeDemands != null)
        demands.AddRange((IEnumerable<Demand>) queueTimeDemands);
      else
        demands.AddRange((IEnumerable<Demand>) this.m_definition.Demands);
      demands.AddRange((IEnumerable<Demand>) this.m_sourceProvider.GetDemands(requestContext));
      if (phase.Target is AgentPoolQueueTarget target)
        demands.AddRange((IEnumerable<Demand>) target.Demands);
      List<TaskInstance> taskInstanceList = new List<TaskInstance>();
      if (tasksToInjectUpFront != null && tasksToInjectUpFront.Count > 0)
        taskInstanceList.AddRange((IEnumerable<TaskInstance>) tasksToInjectUpFront);
      taskInstanceList.AddRange(phase.Steps.Where<BuildDefinitionStep>((Func<BuildDefinitionStep, bool>) (x => x.Enabled)).Select<BuildDefinitionStep, TaskInstance>((Func<BuildDefinitionStep, TaskInstance>) (x => x.ToTaskInstance())));
      List<TaskInstance> source = new List<TaskInstance>();
      IVssRequestContext requestContext1 = requestContext;
      string jobName = name;
      string jobRefName = referenceName;
      List<TaskInstance> tasks = taskInstanceList;
      ref List<TaskInstance> local1 = ref source;
      TaskOrchestrationJob job;
      ref TaskOrchestrationJob local2 = ref job;
      List<Microsoft.TeamFoundation.DistributedTask.WebApi.Demand> list = demands.ToDistributedTaskDemands().ToList<Microsoft.TeamFoundation.DistributedTask.WebApi.Demand>();
      int jobTimeoutInMinutes = timeoutInMinutes;
      string minAgentVersion = versionForRepository;
      string executionMode1 = executionMode;
      int num = includeTaskDemands ? 1 : 0;
      if (!taskHub.TryCreateJob(requestContext1, jobName, jobRefName, tasks, out local1, out local2, list, jobTimeoutInMinutes, minAgentVersion, executionMode1, num != 0))
      {
        if (source.Count > 0)
        {
          foreach (TaskInstance taskInstance in source.Take<TaskInstance>(5))
            requestContext.TraceError(12030027, "Service", "The build definition references task {0} at version {1}, but the task definition could not be found.", (object) taskInstance.Id, (object) taskInstance.Version);
          CustomerIntelligenceData properties = new CustomerIntelligenceData();
          properties.Add("Build Definition Name", this.m_definition.Name);
          properties.Add("Build Project Name", this.m_definition.ProjectName);
          StringBuilder stringBuilder = new StringBuilder();
          foreach (TaskInstance taskInstance in source)
            stringBuilder.Append(taskInstance.Name).Append(taskInstance.Version);
          properties.Add("Tasks with missing definitions", (object) stringBuilder);
          requestContext.GetService<CustomerIntelligenceService>().Publish(requestContext, "Build2", nameof (BuildRequestHelper), properties);
        }
        this.m_build.ValidationResults.Add(new BuildRequestValidationResult()
        {
          Result = ValidationResult.Error,
          Message = BuildServerResources.InvalidBuildStep()
        });
        return (TaskOrchestrationJob) null;
      }
      if (variables != null)
      {
        foreach (KeyValuePair<string, string> variable in variables)
          job.Variables[variable.Key] = variable.Value;
      }
      return job;
    }

    private string GetMinimumAgentVersionForRepository(string minSupportedAgentVersion)
    {
      string semanticVersion1 = (string) null;
      if (this.m_definition.Repository != null)
      {
        switch (this.m_definition.Repository.Type)
        {
          case "GitHubEnterprise":
            semanticVersion1 = BuildRequestHelper.s_gitHubEnterpriseSupportedAgentVersion;
            break;
          case "Bitbucket":
            semanticVersion1 = BuildRequestHelper.s_bitbucketSupportedAgentVersion;
            break;
        }
        if (semanticVersion1 != null && DemandMinimumVersion.CompareVersion(semanticVersion1, BuildRequestHelper.MinServerSupportedAgentVersion) > 0)
          minSupportedAgentVersion = semanticVersion1;
      }
      return minSupportedAgentVersion;
    }

    private void SetBuildCompletionEnvironmentVariables(
      IVssRequestContext requestContext,
      IBuildDefinitionService definitionService,
      IDictionary<string, VariableValue> variables)
    {
      TriggeredByBuild triggeredByBuild = this.m_build.TriggeredByBuild;
      if (triggeredByBuild == null)
        return;
      IDictionary<string, VariableValue> dictionary1 = variables;
      int num = triggeredByBuild.DefinitionId;
      VariableValue variableValue1 = (VariableValue) num.ToString();
      dictionary1["build.triggeredBy.definitionId"] = variableValue1;
      BuildDefinition definition = definitionService.GetDefinition(requestContext, triggeredByBuild.ProjectId, triggeredByBuild.DefinitionId);
      if (definition != null)
        variables["build.triggeredBy.definitionName"] = (VariableValue) definition.Name;
      IDictionary<string, VariableValue> dictionary2 = variables;
      num = triggeredByBuild.BuildId;
      VariableValue variableValue2 = (VariableValue) num.ToString();
      dictionary2["build.triggeredBy.buildId"] = variableValue2;
      BuildData buildById = requestContext.GetService<IBuildService>().GetBuildById(requestContext, this.m_build.ProjectId, triggeredByBuild.BuildId);
      if (buildById != null)
        variables["build.triggeredBy.buildNumber"] = (VariableValue) buildById.BuildNumber;
      variables["build.triggeredBy.projectId"] = (VariableValue) triggeredByBuild.ProjectId.ToString();
    }

    private bool IsYamlProvidedJustInTime(IVssRequestContext requestContext)
    {
      if (this.m_build.JustInTime.Configuration == null)
        return false;
      return this.m_definition.Process is YamlProcess || this.m_definition.Process is JustInTimeProcess;
    }

    private string GetShortBranchName(string branchName)
    {
      if (!string.IsNullOrEmpty(branchName))
      {
        branchName = branchName.TrimEnd('/');
        int num = branchName.LastIndexOf('/');
        if (num >= 0 && num < branchName.Length)
          branchName = branchName.Substring(num + 1);
      }
      if (branchName == null)
        branchName = string.Empty;
      return branchName;
    }

    public string PipelineInitializationLog { get; set; }

    public string PipelineExpandedYaml { get; set; }

    private class BuildNumberFormatter : IYamlNameFormatter
    {
      private readonly IVssRequestContext m_requestContext;
      private readonly BuildDefinition m_definition;
      private readonly BuildData m_build;
      private readonly string m_defaultFormat;

      public BuildNumberFormatter(
        IVssRequestContext requestContext,
        BuildDefinition definition,
        BuildData build,
        string defaultFormat)
      {
        this.m_build = build;
        this.m_definition = definition;
        this.m_defaultFormat = defaultFormat;
        this.m_requestContext = requestContext;
      }

      public string Format(IPipelineContext context, string nameFormat) => this.Format(context.Variables, nameFormat);

      internal string Format(IDictionary<string, VariableValue> variables, string nameFormat)
      {
        if (string.IsNullOrEmpty(nameFormat))
          nameFormat = this.m_defaultFormat;
        string str;
        try
        {
          str = BuildPatternFormatter.FormatBuildNumber(this.m_definition, this.m_build, variables, nameFormat, this.m_requestContext.GetCurrentCollectionTime(this.m_build.QueueTime.Value));
        }
        catch (BuildNumberFormatException ex)
        {
          this.m_requestContext.TraceException("Service", (Exception) ex);
          str = BuildPatternFormatter.FormatBuildNumber(this.m_definition, this.m_build, variables, this.m_defaultFormat, this.m_requestContext.GetCurrentCollectionTime(this.m_build.QueueTime.Value));
          this.m_build.ValidationResults.Add(new BuildRequestValidationResult()
          {
            Result = ValidationResult.Warning,
            Message = ex.Message
          });
        }
        return str;
      }
    }
  }
}
