// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.ProjectPipelineGeneralSettingsHelper
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.VisualStudio.Services.Settings;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Build2.Server
{
  public class ProjectPipelineGeneralSettingsHelper
  {
    private Guid m_projectId;
    private ProjectVisibility m_projectVisibility;
    private PipelineGeneralSettings m_projectLevelPipelineGeneralSettings;
    private PipelineGeneralSettings m_orgLevelPipelineGeneralSettings;
    private static readonly HashSet<string> _highRiskSettings = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
    {
      nameof (EnforceSettableVar)
    };

    public ProjectPipelineGeneralSettingsHelper(
      IVssRequestContext requestContext,
      Guid projectId,
      bool ignoreViewPermissions)
    {
      this.m_projectId = projectId;
      this.m_projectVisibility = requestContext.GetService<IProjectService>().GetProjectVisibility(requestContext, projectId);
      if (!ignoreViewPermissions)
      {
        ISettingsService service = requestContext.GetService<ISettingsService>();
        service.HasReadPermission(requestContext, SettingsUserScope.AllUsers, "Project", projectId.ToString(), true);
        this.HasEditPermission = service.HasWritePermission(requestContext, SettingsUserScope.AllUsers, "Project", projectId.ToString());
      }
      this.LoadSettings(requestContext, projectId);
    }

    public bool HasEditPermission { get; }

    public bool StatusBadgesArePublic => this.m_orgLevelPipelineGeneralSettings.StatusBadgesArePublic && this.m_projectLevelPipelineGeneralSettings.StatusBadgesArePublic || this.m_projectVisibility == ProjectVisibility.Public;

    public bool EnforceSettableVar => this.m_orgLevelPipelineGeneralSettings.EnforceSettableVar || this.m_projectLevelPipelineGeneralSettings.EnforceSettableVar;

    public bool AuditEnforceSettableVar => this.m_orgLevelPipelineGeneralSettings.AuditEnforceSettableVar || this.m_projectLevelPipelineGeneralSettings.AuditEnforceSettableVar;

    public bool EnforceJobAuthScope => this.m_orgLevelPipelineGeneralSettings.EnforceJobAuthScope || this.m_projectLevelPipelineGeneralSettings.EnforceJobAuthScope;

    public bool EnforceJobAuthScopeForReleases => this.m_orgLevelPipelineGeneralSettings.EnforceJobAuthScopeForReleases || this.m_projectLevelPipelineGeneralSettings.EnforceJobAuthScopeForReleases;

    public bool PublishPipelineMetadata => this.m_projectLevelPipelineGeneralSettings.PublishPipelineMetadata;

    public bool EnforceReferencedRepoScopedToken => this.m_orgLevelPipelineGeneralSettings.EnforceReferencedRepoScopedToken || this.m_projectLevelPipelineGeneralSettings.EnforceReferencedRepoScopedToken;

    public bool DisableClassicPipelineCreation => this.m_orgLevelPipelineGeneralSettings.DisableClassicPipelineCreation || this.m_projectLevelPipelineGeneralSettings.DisableClassicPipelineCreation;

    public bool DisableClassicBuildPipelineCreation
    {
      get
      {
        bool? pipelineCreation1 = this.m_orgLevelPipelineGeneralSettings.DisableClassicBuildPipelineCreation;
        bool flag1 = true;
        if (pipelineCreation1.GetValueOrDefault() == flag1 & pipelineCreation1.HasValue)
          return true;
        bool? pipelineCreation2 = this.m_projectLevelPipelineGeneralSettings.DisableClassicBuildPipelineCreation;
        bool flag2 = true;
        return pipelineCreation2.GetValueOrDefault() == flag2 & pipelineCreation2.HasValue;
      }
    }

    public bool DisableClassicReleasePipelineCreation
    {
      get
      {
        bool? pipelineCreation1 = this.m_orgLevelPipelineGeneralSettings.DisableClassicReleasePipelineCreation;
        bool flag1 = true;
        if (pipelineCreation1.GetValueOrDefault() == flag1 & pipelineCreation1.HasValue)
          return true;
        bool? pipelineCreation2 = this.m_projectLevelPipelineGeneralSettings.DisableClassicReleasePipelineCreation;
        bool flag2 = true;
        return pipelineCreation2.GetValueOrDefault() == flag2 & pipelineCreation2.HasValue;
      }
    }

    private bool ForkProtectionEnabled => this.m_orgLevelPipelineGeneralSettings.ForkProtectionEnabled || this.m_projectLevelPipelineGeneralSettings.ForkProtectionEnabled;

    private bool BuildsEnabledForForks
    {
      get
      {
        if ((!this.m_orgLevelPipelineGeneralSettings.ForkProtectionEnabled ? 0 : (!this.m_projectLevelPipelineGeneralSettings.ForkProtectionEnabled ? 1 : 0)) != 0)
          return this.m_orgLevelPipelineGeneralSettings.BuildsEnabledForForks;
        return ((this.m_orgLevelPipelineGeneralSettings.ForkProtectionEnabled ? 0 : (this.m_projectLevelPipelineGeneralSettings.ForkProtectionEnabled ? 1 : 0)) != 0 || this.m_orgLevelPipelineGeneralSettings.BuildsEnabledForForks) && this.m_projectLevelPipelineGeneralSettings.BuildsEnabledForForks;
      }
    }

    private bool EnforceJobAuthScopeForForks
    {
      get
      {
        if ((!this.m_orgLevelPipelineGeneralSettings.ForkProtectionEnabled ? 0 : (!this.m_projectLevelPipelineGeneralSettings.ForkProtectionEnabled ? 1 : 0)) != 0)
          return this.m_orgLevelPipelineGeneralSettings.BuildsEnabledForForks && this.m_orgLevelPipelineGeneralSettings.EnforceJobAuthScopeForForks;
        if ((this.m_orgLevelPipelineGeneralSettings.ForkProtectionEnabled ? 0 : (this.m_projectLevelPipelineGeneralSettings.ForkProtectionEnabled ? 1 : 0)) != 0)
          return this.m_projectLevelPipelineGeneralSettings.BuildsEnabledForForks && this.m_projectLevelPipelineGeneralSettings.EnforceJobAuthScopeForForks;
        if (!this.BuildsEnabledForForks)
          return false;
        return this.m_orgLevelPipelineGeneralSettings.EnforceJobAuthScopeForForks || this.m_projectLevelPipelineGeneralSettings.EnforceJobAuthScopeForForks;
      }
    }

    private bool EnforceNoAccessToSecretsFromForks
    {
      get
      {
        if ((!this.m_orgLevelPipelineGeneralSettings.ForkProtectionEnabled ? 0 : (!this.m_projectLevelPipelineGeneralSettings.ForkProtectionEnabled ? 1 : 0)) != 0)
          return this.m_orgLevelPipelineGeneralSettings.BuildsEnabledForForks && this.m_orgLevelPipelineGeneralSettings.EnforceNoAccessToSecretsFromForks;
        if ((this.m_orgLevelPipelineGeneralSettings.ForkProtectionEnabled ? 0 : (this.m_projectLevelPipelineGeneralSettings.ForkProtectionEnabled ? 1 : 0)) != 0)
          return this.m_projectLevelPipelineGeneralSettings.BuildsEnabledForForks && this.m_projectLevelPipelineGeneralSettings.EnforceNoAccessToSecretsFromForks;
        if (!this.BuildsEnabledForForks)
          return false;
        return this.m_orgLevelPipelineGeneralSettings.EnforceNoAccessToSecretsFromForks || this.m_projectLevelPipelineGeneralSettings.EnforceNoAccessToSecretsFromForks;
      }
    }

    private bool IsCommentRequiredForPullRequest
    {
      get
      {
        if ((!this.m_orgLevelPipelineGeneralSettings.ForkProtectionEnabled ? 0 : (!this.m_projectLevelPipelineGeneralSettings.ForkProtectionEnabled ? 1 : 0)) != 0)
          return this.m_orgLevelPipelineGeneralSettings.BuildsEnabledForForks && this.m_orgLevelPipelineGeneralSettings.IsCommentRequiredForPullRequest;
        if ((this.m_orgLevelPipelineGeneralSettings.ForkProtectionEnabled ? 0 : (this.m_projectLevelPipelineGeneralSettings.ForkProtectionEnabled ? 1 : 0)) != 0)
          return this.m_projectLevelPipelineGeneralSettings.BuildsEnabledForForks && this.m_projectLevelPipelineGeneralSettings.IsCommentRequiredForPullRequest;
        if (!this.BuildsEnabledForForks)
          return false;
        return this.m_orgLevelPipelineGeneralSettings.IsCommentRequiredForPullRequest || this.m_projectLevelPipelineGeneralSettings.IsCommentRequiredForPullRequest;
      }
    }

    private bool RequireCommentsForNonTeamMembersOnly
    {
      get
      {
        if ((!this.m_orgLevelPipelineGeneralSettings.ForkProtectionEnabled ? 0 : (!this.m_projectLevelPipelineGeneralSettings.ForkProtectionEnabled ? 1 : 0)) != 0)
          return this.m_orgLevelPipelineGeneralSettings.IsCommentRequiredForPullRequest && this.m_orgLevelPipelineGeneralSettings.RequireCommentsForNonTeamMembersOnly;
        if ((this.m_orgLevelPipelineGeneralSettings.ForkProtectionEnabled ? 0 : (this.m_projectLevelPipelineGeneralSettings.ForkProtectionEnabled ? 1 : 0)) != 0)
          return this.m_projectLevelPipelineGeneralSettings.IsCommentRequiredForPullRequest && this.m_projectLevelPipelineGeneralSettings.RequireCommentsForNonTeamMembersOnly;
        bool flag = PipelineTriggerSettings.GetCommentTriggerOption(this.m_orgLevelPipelineGeneralSettings.IsCommentRequiredForPullRequest, this.m_orgLevelPipelineGeneralSettings.RequireCommentsForNonTeamMembersOnly, this.m_orgLevelPipelineGeneralSettings.RequireCommentsForNonTeamMemberAndNonContributors) > PipelineTriggerSettings.GetCommentTriggerOption(this.m_projectLevelPipelineGeneralSettings.IsCommentRequiredForPullRequest, this.m_projectLevelPipelineGeneralSettings.RequireCommentsForNonTeamMembersOnly, this.m_projectLevelPipelineGeneralSettings.RequireCommentsForNonTeamMemberAndNonContributors);
        if (!this.IsCommentRequiredForPullRequest)
          return false;
        return !flag ? this.m_projectLevelPipelineGeneralSettings.RequireCommentsForNonTeamMembersOnly : this.m_orgLevelPipelineGeneralSettings.RequireCommentsForNonTeamMembersOnly;
      }
    }

    private bool RequireCommentsForNonTeamMemberAndNonContributors
    {
      get
      {
        if ((!this.m_orgLevelPipelineGeneralSettings.ForkProtectionEnabled ? 0 : (!this.m_projectLevelPipelineGeneralSettings.ForkProtectionEnabled ? 1 : 0)) != 0)
          return this.m_orgLevelPipelineGeneralSettings.RequireCommentsForNonTeamMemberAndNonContributors;
        if ((this.m_orgLevelPipelineGeneralSettings.ForkProtectionEnabled ? 0 : (this.m_projectLevelPipelineGeneralSettings.ForkProtectionEnabled ? 1 : 0)) != 0)
          return this.m_projectLevelPipelineGeneralSettings.RequireCommentsForNonTeamMemberAndNonContributors;
        bool flag = PipelineTriggerSettings.GetCommentTriggerOption(this.m_orgLevelPipelineGeneralSettings.IsCommentRequiredForPullRequest, this.m_orgLevelPipelineGeneralSettings.RequireCommentsForNonTeamMembersOnly, this.m_orgLevelPipelineGeneralSettings.RequireCommentsForNonTeamMemberAndNonContributors) > PipelineTriggerSettings.GetCommentTriggerOption(this.m_projectLevelPipelineGeneralSettings.IsCommentRequiredForPullRequest, this.m_projectLevelPipelineGeneralSettings.RequireCommentsForNonTeamMembersOnly, this.m_projectLevelPipelineGeneralSettings.RequireCommentsForNonTeamMemberAndNonContributors);
        if (!this.IsCommentRequiredForPullRequest)
          return false;
        return !flag ? this.m_projectLevelPipelineGeneralSettings.RequireCommentsForNonTeamMemberAndNonContributors : this.m_orgLevelPipelineGeneralSettings.RequireCommentsForNonTeamMemberAndNonContributors;
      }
    }

    public bool EnableShellTasksArgsSanitizing => this.m_orgLevelPipelineGeneralSettings.EnableShellTasksArgsSanitizing || this.m_projectLevelPipelineGeneralSettings.EnableShellTasksArgsSanitizing;

    public bool EnableShellTasksArgsSanitizingAudit => this.m_orgLevelPipelineGeneralSettings.EnableShellTasksArgsSanitizingAudit || this.m_projectLevelPipelineGeneralSettings.EnableShellTasksArgsSanitizingAudit;

    public bool DisableImpliedYAMLCiTrigger => this.m_orgLevelPipelineGeneralSettings.DisableImpliedYAMLCiTrigger || this.m_projectLevelPipelineGeneralSettings.DisableImpliedYAMLCiTrigger;

    public PipelineGeneralSettings GetEffectiveSettings() => new PipelineGeneralSettings()
    {
      StatusBadgesArePublic = this.StatusBadgesArePublic,
      EnforceSettableVar = this.EnforceSettableVar,
      AuditEnforceSettableVar = this.AuditEnforceSettableVar,
      EnforceJobAuthScope = this.EnforceJobAuthScope,
      EnforceJobAuthScopeForReleases = this.EnforceJobAuthScopeForReleases,
      PublishPipelineMetadata = this.PublishPipelineMetadata,
      EnforceReferencedRepoScopedToken = this.EnforceReferencedRepoScopedToken,
      DisableClassicPipelineCreation = this.DisableClassicPipelineCreation,
      DisableClassicBuildPipelineCreation = new bool?(this.DisableClassicBuildPipelineCreation),
      DisableClassicReleasePipelineCreation = new bool?(this.DisableClassicReleasePipelineCreation),
      ForkProtectionEnabled = this.ForkProtectionEnabled,
      BuildsEnabledForForks = this.BuildsEnabledForForks,
      EnforceJobAuthScopeForForks = this.EnforceJobAuthScopeForForks,
      EnforceNoAccessToSecretsFromForks = this.EnforceNoAccessToSecretsFromForks,
      IsCommentRequiredForPullRequest = this.IsCommentRequiredForPullRequest,
      RequireCommentsForNonTeamMembersOnly = this.RequireCommentsForNonTeamMembersOnly,
      RequireCommentsForNonTeamMemberAndNonContributors = this.RequireCommentsForNonTeamMemberAndNonContributors,
      EnableShellTasksArgsSanitizing = this.EnableShellTasksArgsSanitizing,
      EnableShellTasksArgsSanitizingAudit = this.EnableShellTasksArgsSanitizingAudit,
      DisableImpliedYAMLCiTrigger = this.DisableImpliedYAMLCiTrigger
    };

    public PipelineTriggerSettings GetEffectivePipelineTriggerSettings() => new PipelineTriggerSettings()
    {
      ForkProtectionEnabled = this.ForkProtectionEnabled,
      BuildsEnabledForForks = this.ForkProtectionEnabled ? this.BuildsEnabledForForks : PipelineTriggerSettings.Disabled.BuildsEnabledForForks,
      EnforceJobAuthScopeForForks = this.ForkProtectionEnabled ? this.EnforceJobAuthScopeForForks : PipelineTriggerSettings.Disabled.EnforceJobAuthScopeForForks,
      EnforceNoAccessToSecretsFromForks = this.ForkProtectionEnabled ? this.EnforceNoAccessToSecretsFromForks : PipelineTriggerSettings.Disabled.EnforceNoAccessToSecretsFromForks,
      IsCommentRequiredForPullRequest = this.ForkProtectionEnabled ? this.IsCommentRequiredForPullRequest : PipelineTriggerSettings.Disabled.IsCommentRequiredForPullRequest,
      RequireCommentsForNonTeamMembersOnly = this.ForkProtectionEnabled ? this.RequireCommentsForNonTeamMembersOnly : PipelineTriggerSettings.Disabled.RequireCommentsForNonTeamMembersOnly,
      RequireCommentsForNonTeamMemberAndNonContributors = this.ForkProtectionEnabled ? this.RequireCommentsForNonTeamMemberAndNonContributors : PipelineTriggerSettings.Disabled.RequireCommentsForNonTeamMemberAndNonContributors
    };

    public PipelineTriggerSettings GetEffectiveSettingsOfPullRequestTrigger(
      IVssRequestContext requestContext,
      PullRequestTrigger trigger,
      bool isGitHubRepository = false)
    {
      if (!isGitHubRepository || !requestContext.IsFeatureEnabled("Build2.AllowCentralizedPipelineControls") || !this.ForkProtectionEnabled)
        return new PipelineTriggerSettings()
        {
          ForkProtectionEnabled = PipelineTriggerSettings.Disabled.ForkProtectionEnabled,
          BuildsEnabledForForks = trigger.Forks.Enabled,
          EnforceJobAuthScopeForForks = !trigger.Forks.AllowFullAccessToken,
          EnforceNoAccessToSecretsFromForks = !trigger.Forks.AllowSecrets,
          IsCommentRequiredForPullRequest = trigger.IsCommentRequiredForPullRequest,
          RequireCommentsForNonTeamMembersOnly = trigger.RequireCommentsForNonTeamMembersOnly,
          RequireCommentsForNonTeamMemberAndNonContributors = trigger.RequireCommentsForNonTeamMemberAndNonContributors
        };
      bool flag1 = this.BuildsEnabledForForks && trigger.Forks.Enabled;
      bool flag2 = flag1 ? this.EnforceJobAuthScopeForForks || !trigger.Forks.AllowFullAccessToken : PipelineTriggerSettings.Disabled.EnforceJobAuthScopeForForks;
      bool flag3 = flag1 ? this.EnforceNoAccessToSecretsFromForks || !trigger.Forks.AllowSecrets : PipelineTriggerSettings.Disabled.EnforceNoAccessToSecretsFromForks;
      bool flag4 = this.IsCommentRequiredForPullRequest || trigger.IsCommentRequiredForPullRequest;
      PipelineTriggerSettings.CommentTriggerOption commentTriggerOption = PipelineTriggerSettings.GetCommentTriggerOption(trigger.IsCommentRequiredForPullRequest, trigger.RequireCommentsForNonTeamMembersOnly, trigger.RequireCommentsForNonTeamMemberAndNonContributors);
      int num = PipelineTriggerSettings.GetCommentTriggerOption(this.IsCommentRequiredForPullRequest, this.RequireCommentsForNonTeamMembersOnly, this.RequireCommentsForNonTeamMemberAndNonContributors) > commentTriggerOption ? 1 : 0;
      bool flag5 = num != 0 ? this.RequireCommentsForNonTeamMembersOnly : trigger.IsCommentRequiredForPullRequest && trigger.RequireCommentsForNonTeamMembersOnly;
      bool flag6 = num != 0 ? this.RequireCommentsForNonTeamMemberAndNonContributors : trigger.IsCommentRequiredForPullRequest && trigger.RequireCommentsForNonTeamMemberAndNonContributors;
      return new PipelineTriggerSettings()
      {
        ForkProtectionEnabled = this.ForkProtectionEnabled,
        BuildsEnabledForForks = flag1,
        EnforceJobAuthScopeForForks = flag2,
        EnforceNoAccessToSecretsFromForks = flag3,
        IsCommentRequiredForPullRequest = flag4,
        RequireCommentsForNonTeamMembersOnly = flag5,
        RequireCommentsForNonTeamMemberAndNonContributors = flag6
      };
    }

    private void LoadSettings(IVssRequestContext requestContext, Guid projectId)
    {
      this.m_projectLevelPipelineGeneralSettings = requestContext.GetService<ISettingsService>().GetValue<PipelineGeneralSettings>(requestContext, SettingsUserScope.AllUsers, "Project", projectId.ToString(), "Pipelines/General/Settings", PipelineGeneralSettings.Default);
      if (this.m_projectLevelPipelineGeneralSettings.DisableClassicPipelineCreation && !this.m_projectLevelPipelineGeneralSettings.DisableClassicBuildPipelineCreation.HasValue)
        this.m_projectLevelPipelineGeneralSettings.DisableClassicBuildPipelineCreation = new bool?(true);
      if (this.m_projectLevelPipelineGeneralSettings.DisableClassicPipelineCreation && !this.m_projectLevelPipelineGeneralSettings.DisableClassicReleasePipelineCreation.HasValue)
        this.m_projectLevelPipelineGeneralSettings.DisableClassicReleasePipelineCreation = new bool?(true);
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      this.m_orgLevelPipelineGeneralSettings = new PipelineGeneralSettings()
      {
        StatusBadgesArePublic = service.GetValue<bool>(requestContext, (RegistryQuery) "/Service/Build/Settings/OrgSettings/StatusBadgesArePublic", PipelineGeneralSettings.Default.StatusBadgesArePublic),
        EnforceSettableVar = service.GetValue<bool>(requestContext, (RegistryQuery) "/Service/Build/Settings/OrgSettings/EnforceSettableVar", PipelineGeneralSettings.Default.EnforceSettableVar),
        AuditEnforceSettableVar = service.GetValue<bool>(requestContext, (RegistryQuery) "/Service/Build/Settings/OrgSettings/AuditEnforceSettableVar", PipelineGeneralSettings.Default.AuditEnforceSettableVar),
        EnforceJobAuthScope = service.GetValue<bool>(requestContext, (RegistryQuery) "/Service/Build/Settings/OrgSettings/EnforceJobAuthScope", PipelineGeneralSettings.Default.EnforceJobAuthScope),
        EnforceJobAuthScopeForReleases = service.GetValue<bool>(requestContext, (RegistryQuery) "/Service/Build/Settings/OrgSettings/EnforceJobAuthScopeForReleases", PipelineGeneralSettings.Default.EnforceJobAuthScopeForReleases),
        EnforceReferencedRepoScopedToken = service.GetValue<bool>(requestContext, (RegistryQuery) "/Service/Build/Settings/OrgSettings/EnforceReferencedRepoScopedToken", PipelineGeneralSettings.Default.EnforceReferencedRepoScopedToken),
        DisableClassicPipelineCreation = service.GetValue<bool>(requestContext, (RegistryQuery) "/Service/Build/Settings/OrgSettings/DisableClassicPipelineCreation", PipelineGeneralSettings.Default.DisableClassicPipelineCreation),
        DisableClassicBuildPipelineCreation = service.GetValue<bool?>(requestContext, (RegistryQuery) "/Service/Build/Settings/OrgSettings/DisableClassicBuildPipelineCreation", PipelineGeneralSettings.Default.DisableClassicBuildPipelineCreation),
        DisableClassicReleasePipelineCreation = service.GetValue<bool?>(requestContext, (RegistryQuery) "/Service/Build/Settings/OrgSettings/DisableClassicReleasePipelineCreation", PipelineGeneralSettings.Default.DisableClassicReleasePipelineCreation),
        ForkProtectionEnabled = service.GetValue<bool>(requestContext, (RegistryQuery) "/Service/Build/Settings/OrgSettings/ForkProtectionEnabled", PipelineTriggerSettings.Default.ForkProtectionEnabled),
        BuildsEnabledForForks = service.GetValue<bool>(requestContext, (RegistryQuery) "/Service/Build/Settings/OrgSettings/BuildsEnabledForForks", PipelineGeneralSettings.Default.BuildsEnabledForForks),
        EnforceJobAuthScopeForForks = service.GetValue<bool>(requestContext, (RegistryQuery) "/Service/Build/Settings/OrgSettings/EnforceJobAuthScopeForForks", PipelineGeneralSettings.Default.EnforceJobAuthScopeForForks),
        EnforceNoAccessToSecretsFromForks = service.GetValue<bool>(requestContext, (RegistryQuery) "/Service/Build/Settings/OrgSettings/EnforceNoAccessToSecretsFromForks", PipelineGeneralSettings.Default.EnforceNoAccessToSecretsFromForks),
        IsCommentRequiredForPullRequest = service.GetValue<bool>(requestContext, (RegistryQuery) "/Service/Build/Settings/OrgSettings/IsCommentRequiredForPullRequest", PipelineGeneralSettings.Default.IsCommentRequiredForPullRequest),
        RequireCommentsForNonTeamMembersOnly = service.GetValue<bool>(requestContext, (RegistryQuery) "/Service/Build/Settings/OrgSettings/RequireCommentsForNonTeamMembersOnly", PipelineGeneralSettings.Default.RequireCommentsForNonTeamMembersOnly),
        RequireCommentsForNonTeamMemberAndNonContributors = service.GetValue<bool>(requestContext, (RegistryQuery) "/Service/Build/Settings/OrgSettings/RequireCommentsForNonTeamMemberAndNonContributors", PipelineGeneralSettings.Default.RequireCommentsForNonTeamMemberAndNonContributors),
        EnableShellTasksArgsSanitizing = service.GetValue<bool>(requestContext, (RegistryQuery) Microsoft.TeamFoundation.Build.Common.RegistryKeys.EnableShellTasksArgsSanitizing, PipelineGeneralSettings.Default.EnableShellTasksArgsSanitizing),
        EnableShellTasksArgsSanitizingAudit = service.GetValue<bool>(requestContext, (RegistryQuery) Microsoft.TeamFoundation.Build.Common.RegistryKeys.EnableShellTasksArgsSanitizingAudit, PipelineGeneralSettings.Default.EnableShellTasksArgsSanitizingAudit),
        DisableImpliedYAMLCiTrigger = service.GetValue<bool>(requestContext, (RegistryQuery) "/Service/Build/Settings/OrgSettings/DisableImpliedYAMLCiTrigger", PipelineGeneralSettings.Default.DisableImpliedYAMLCiTrigger)
      };
      if (requestContext.IsFeatureEnabled("Build2.NewTogglesToDisableClassicPipelineCreation"))
        return;
      this.m_projectLevelPipelineGeneralSettings.DisableClassicBuildPipelineCreation = new bool?(this.m_projectLevelPipelineGeneralSettings.DisableClassicPipelineCreation);
      this.m_projectLevelPipelineGeneralSettings.DisableClassicReleasePipelineCreation = new bool?(this.m_projectLevelPipelineGeneralSettings.DisableClassicPipelineCreation);
      this.m_orgLevelPipelineGeneralSettings.DisableClassicBuildPipelineCreation = new bool?(this.m_orgLevelPipelineGeneralSettings.DisableClassicPipelineCreation);
      this.m_orgLevelPipelineGeneralSettings.DisableClassicReleasePipelineCreation = new bool?(this.m_orgLevelPipelineGeneralSettings.DisableClassicPipelineCreation);
    }

    public void UpdateSettings(
      IVssRequestContext requestContext,
      bool? statusBadgesArePublic = null,
      bool? enforceSettableVar = null,
      bool? auditEnforceSettableVar = null,
      bool? enforceJobAuthScope = null,
      bool? enforceJobAuthScopeForReleases = null,
      bool? publishPipelineMetadata = null,
      bool? enforceReferencedRepoScopedToken = null,
      bool? disableClassicPipelineCreation = null,
      bool? disableClassicBuildPipelineCreation = null,
      bool? disableClassicReleasePipelineCreation = null,
      bool? forkProtectionEnabled = null,
      bool? buildsEnabledForForks = null,
      bool? enforceJobAuthScopeForForks = null,
      bool? enforceNoAccessToSecretsFromForks = null,
      bool? isCommentRequiredForPullRequest = null,
      bool? requireCommentsForNonTeamMembersOnly = null,
      bool? requireCommentsForNonTeamMemberAndNonContributors = null,
      bool? enableShellTasksArgsSanitizing = null,
      bool? enableShellTasksArgsSanitizingAudit = null,
      bool? disableImpliedYAMLCiTrigger = null)
    {
      ISettingsService service = requestContext.GetService<ISettingsService>();
      service.HasWritePermission(requestContext, SettingsUserScope.AllUsers, "Project", this.m_projectId.ToString(), true);
      this.LoadSettings(requestContext, this.m_projectId);
      List<(string, bool, bool)> source1 = new List<(string, bool, bool)>();
      if (this.m_projectVisibility != ProjectVisibility.Public)
        source1.Add(ProjectPipelineGeneralSettingsHelper.GetUpdatedProjectSetting("StatusBadgesArePublic", ref this.m_projectLevelPipelineGeneralSettings.StatusBadgesArePublic, statusBadgesArePublic, !this.m_orgLevelPipelineGeneralSettings.StatusBadgesArePublic));
      source1.Add(ProjectPipelineGeneralSettingsHelper.GetUpdatedProjectSetting("EnforceSettableVar", ref this.m_projectLevelPipelineGeneralSettings.EnforceSettableVar, enforceSettableVar, this.m_orgLevelPipelineGeneralSettings.EnforceSettableVar));
      source1.Add(ProjectPipelineGeneralSettingsHelper.GetUpdatedProjectSetting("AuditEnforceSettableVar", ref this.m_projectLevelPipelineGeneralSettings.AuditEnforceSettableVar, auditEnforceSettableVar, this.m_orgLevelPipelineGeneralSettings.AuditEnforceSettableVar));
      source1.Add(ProjectPipelineGeneralSettingsHelper.GetUpdatedProjectSetting("EnforceJobAuthScope", ref this.m_projectLevelPipelineGeneralSettings.EnforceJobAuthScope, enforceJobAuthScope, this.m_orgLevelPipelineGeneralSettings.EnforceJobAuthScope));
      source1.Add(ProjectPipelineGeneralSettingsHelper.GetUpdatedProjectSetting("EnforceJobAuthScopeForReleases", ref this.m_projectLevelPipelineGeneralSettings.EnforceJobAuthScopeForReleases, enforceJobAuthScopeForReleases, this.m_orgLevelPipelineGeneralSettings.EnforceJobAuthScopeForReleases));
      source1.Add(ProjectPipelineGeneralSettingsHelper.GetUpdatedProjectSetting("PublishPipelineMetadata", ref this.m_projectLevelPipelineGeneralSettings.PublishPipelineMetadata, publishPipelineMetadata, false));
      source1.Add(ProjectPipelineGeneralSettingsHelper.GetUpdatedProjectSetting("EnforceReferencedRepoScopedToken", ref this.m_projectLevelPipelineGeneralSettings.EnforceReferencedRepoScopedToken, enforceReferencedRepoScopedToken, this.m_orgLevelPipelineGeneralSettings.EnforceReferencedRepoScopedToken));
      source1.Add(ProjectPipelineGeneralSettingsHelper.GetUpdatedProjectSetting("DisableClassicPipelineCreation", ref this.m_projectLevelPipelineGeneralSettings.DisableClassicPipelineCreation, disableClassicPipelineCreation, this.m_orgLevelPipelineGeneralSettings.DisableClassicPipelineCreation));
      source1.Add(ProjectPipelineGeneralSettingsHelper.GetUpdatedProjectSetting("DisableClassicBuildPipelineCreation", ref this.m_projectLevelPipelineGeneralSettings.DisableClassicBuildPipelineCreation, disableClassicBuildPipelineCreation, this.m_orgLevelPipelineGeneralSettings.DisableClassicBuildPipelineCreation));
      source1.Add(ProjectPipelineGeneralSettingsHelper.GetUpdatedProjectSetting("DisableClassicReleasePipelineCreation", ref this.m_projectLevelPipelineGeneralSettings.DisableClassicReleasePipelineCreation, disableClassicReleasePipelineCreation, this.m_orgLevelPipelineGeneralSettings.DisableClassicReleasePipelineCreation));
      source1.Add(ProjectPipelineGeneralSettingsHelper.GetUpdatedProjectSetting("EnableShellTasksArgsSanitizing", ref this.m_projectLevelPipelineGeneralSettings.EnableShellTasksArgsSanitizing, enableShellTasksArgsSanitizing, this.m_orgLevelPipelineGeneralSettings.EnableShellTasksArgsSanitizing));
      source1.Add(ProjectPipelineGeneralSettingsHelper.GetUpdatedProjectSetting("EnableShellTasksArgsSanitizingAudit", ref this.m_projectLevelPipelineGeneralSettings.EnableShellTasksArgsSanitizingAudit, enableShellTasksArgsSanitizingAudit, this.m_orgLevelPipelineGeneralSettings.EnableShellTasksArgsSanitizingAudit));
      source1.Add(ProjectPipelineGeneralSettingsHelper.GetUpdatedProjectSetting("DisableImpliedYAMLCiTrigger", ref this.m_projectLevelPipelineGeneralSettings.DisableImpliedYAMLCiTrigger, disableImpliedYAMLCiTrigger, this.m_orgLevelPipelineGeneralSettings.DisableImpliedYAMLCiTrigger));
      PipelineTriggerSettings.Optional update = new PipelineTriggerSettings.Optional()
      {
        ForkProtectionEnabled = forkProtectionEnabled,
        BuildsEnabledForForks = buildsEnabledForForks,
        EnforceJobAuthScopeForForks = enforceJobAuthScopeForForks,
        EnforceNoAccessToSecretsFromForks = enforceNoAccessToSecretsFromForks,
        IsCommentRequiredForPullRequest = isCommentRequiredForPullRequest,
        RequireCommentsForNonTeamMembersOnly = requireCommentsForNonTeamMembersOnly,
        RequireCommentsForNonTeamMemberAndNonContributors = requireCommentsForNonTeamMemberAndNonContributors
      };
      PipelineTriggerSettings currentLevelSettings = new PipelineTriggerSettings()
      {
        ForkProtectionEnabled = this.m_projectLevelPipelineGeneralSettings.ForkProtectionEnabled,
        BuildsEnabledForForks = this.m_projectLevelPipelineGeneralSettings.BuildsEnabledForForks,
        EnforceJobAuthScopeForForks = this.m_projectLevelPipelineGeneralSettings.EnforceJobAuthScopeForForks,
        EnforceNoAccessToSecretsFromForks = this.m_projectLevelPipelineGeneralSettings.EnforceNoAccessToSecretsFromForks,
        IsCommentRequiredForPullRequest = this.m_projectLevelPipelineGeneralSettings.IsCommentRequiredForPullRequest,
        RequireCommentsForNonTeamMembersOnly = this.m_projectLevelPipelineGeneralSettings.RequireCommentsForNonTeamMembersOnly,
        RequireCommentsForNonTeamMemberAndNonContributors = this.m_projectLevelPipelineGeneralSettings.RequireCommentsForNonTeamMemberAndNonContributors
      };
      PipelineTriggerSettings upperLevelSettings = new PipelineTriggerSettings(this.m_orgLevelPipelineGeneralSettings.ForkProtectionEnabled, this.m_orgLevelPipelineGeneralSettings.BuildsEnabledForForks, this.m_orgLevelPipelineGeneralSettings.EnforceJobAuthScopeForForks, this.m_orgLevelPipelineGeneralSettings.EnforceNoAccessToSecretsFromForks, this.m_orgLevelPipelineGeneralSettings.IsCommentRequiredForPullRequest, this.m_orgLevelPipelineGeneralSettings.RequireCommentsForNonTeamMembersOnly, this.m_orgLevelPipelineGeneralSettings.RequireCommentsForNonTeamMemberAndNonContributors);
      PipelineTriggerSettingsValidatorHelper.ValidateAndFixPipelineTriggerSettingsUpdate(ref update, currentLevelSettings, upperLevelSettings);
      source1.Add(ProjectPipelineGeneralSettingsHelper.GetUpdatedProjectSetting("ForkProtectionEnabled", ref this.m_projectLevelPipelineGeneralSettings.ForkProtectionEnabled, update.ForkProtectionEnabled, update.ForkProtectionEnabled.HasValue && !update.ForkProtectionEnabled.Value && this.m_orgLevelPipelineGeneralSettings.ForkProtectionEnabled));
      source1.Add(ProjectPipelineGeneralSettingsHelper.GetUpdatedProjectSetting("BuildsEnabledForForks", ref this.m_projectLevelPipelineGeneralSettings.BuildsEnabledForForks, update.BuildsEnabledForForks, this.m_orgLevelPipelineGeneralSettings.ForkProtectionEnabled && !this.m_orgLevelPipelineGeneralSettings.BuildsEnabledForForks));
      source1.Add(ProjectPipelineGeneralSettingsHelper.GetUpdatedProjectSetting("EnforceJobAuthScopeForForks", ref this.m_projectLevelPipelineGeneralSettings.EnforceJobAuthScopeForForks, update.EnforceJobAuthScopeForForks, this.m_orgLevelPipelineGeneralSettings.ForkProtectionEnabled && this.m_orgLevelPipelineGeneralSettings.EnforceJobAuthScopeForForks));
      source1.Add(ProjectPipelineGeneralSettingsHelper.GetUpdatedProjectSetting("EnforceNoAccessToSecretsFromForks", ref this.m_projectLevelPipelineGeneralSettings.EnforceNoAccessToSecretsFromForks, update.EnforceNoAccessToSecretsFromForks, this.m_orgLevelPipelineGeneralSettings.ForkProtectionEnabled && this.m_orgLevelPipelineGeneralSettings.EnforceNoAccessToSecretsFromForks));
      source1.Add(ProjectPipelineGeneralSettingsHelper.GetUpdatedProjectSetting("IsCommentRequiredForPullRequest", ref this.m_projectLevelPipelineGeneralSettings.IsCommentRequiredForPullRequest, update.IsCommentRequiredForPullRequest, this.m_orgLevelPipelineGeneralSettings.ForkProtectionEnabled && this.m_orgLevelPipelineGeneralSettings.IsCommentRequiredForPullRequest && this.m_projectLevelPipelineGeneralSettings.IsCommentRequiredForPullRequest));
      PipelineTriggerSettings.CommentTriggerOption commentTriggerOption = PipelineTriggerSettings.GetCommentTriggerOption(this.m_orgLevelPipelineGeneralSettings.IsCommentRequiredForPullRequest, this.m_orgLevelPipelineGeneralSettings.RequireCommentsForNonTeamMembersOnly, this.m_orgLevelPipelineGeneralSettings.RequireCommentsForNonTeamMemberAndNonContributors);
      bool flag = PipelineTriggerSettings.GetCommentTriggerOption(update.IsCommentRequiredForPullRequest.GetValueOrDefault(this.m_projectLevelPipelineGeneralSettings.IsCommentRequiredForPullRequest), update.RequireCommentsForNonTeamMembersOnly.GetValueOrDefault(this.m_projectLevelPipelineGeneralSettings.RequireCommentsForNonTeamMembersOnly), update.RequireCommentsForNonTeamMemberAndNonContributors.GetValueOrDefault(this.m_projectLevelPipelineGeneralSettings.RequireCommentsForNonTeamMemberAndNonContributors)) > commentTriggerOption;
      source1.Add(ProjectPipelineGeneralSettingsHelper.GetUpdatedProjectSetting("RequireCommentsForNonTeamMembersOnly", ref this.m_projectLevelPipelineGeneralSettings.RequireCommentsForNonTeamMembersOnly, update.RequireCommentsForNonTeamMembersOnly, this.m_orgLevelPipelineGeneralSettings.ForkProtectionEnabled && (!this.IsCommentRequiredForPullRequest || commentTriggerOption == PipelineTriggerSettings.CommentTriggerOption.All)));
      List<(string, bool, bool)> valueTupleList = source1;
      ref bool local = ref this.m_projectLevelPipelineGeneralSettings.RequireCommentsForNonTeamMemberAndNonContributors;
      bool? andNonContributors = update.RequireCommentsForNonTeamMemberAndNonContributors;
      int num;
      if (this.m_orgLevelPipelineGeneralSettings.ForkProtectionEnabled)
      {
        if (this.IsCommentRequiredForPullRequest)
        {
          switch (commentTriggerOption)
          {
            case PipelineTriggerSettings.CommentTriggerOption.NonTeamMembers:
              num = !flag ? 1 : 0;
              goto label_9;
            case PipelineTriggerSettings.CommentTriggerOption.All:
              break;
            default:
              num = 0;
              goto label_9;
          }
        }
        num = 1;
      }
      else
        num = 0;
label_9:
      (string, bool, bool) updatedProjectSetting = ProjectPipelineGeneralSettingsHelper.GetUpdatedProjectSetting("RequireCommentsForNonTeamMemberAndNonContributors", ref local, andNonContributors, num != 0);
      valueTupleList.Add(updatedProjectSetting);
      IEnumerable<(string, bool, bool)> source2 = source1.Where<(string, bool, bool)>((Func<(string, bool, bool), bool>) (x => x.Updated));
      if (source2.Count<(string, bool, bool)>() <= 0)
        return;
      service.SetValue(requestContext, SettingsUserScope.AllUsers, "Project", this.m_projectId.ToString(), "Pipelines/General/Settings", (object) this.m_projectLevelPipelineGeneralSettings);
      string projectName = requestContext.GetService<IProjectService>().GetProjectName(requestContext, this.m_projectId);
      foreach ((string, bool, bool) tuple in source2)
      {
        IVssRequestContext requestContext1 = requestContext;
        Dictionary<string, object> data = new Dictionary<string, object>();
        data["SettingName"] = (object) tuple.Item1;
        data["OldValue"] = (object) !tuple.Item3;
        data["NewValue"] = (object) tuple.Item3;
        data["ProjectName"] = (object) projectName;
        Guid projectId1 = this.m_projectId;
        Guid targetHostId = new Guid();
        Guid projectId2 = projectId1;
        requestContext1.LogAuditEvent("Pipelines.ProjectSettings", data, targetHostId, projectId2);
        this.PublishSettingChangedTelemetry(requestContext, tuple.Item1, !tuple.Item3, tuple.Item3);
        if (tuple.Item1 == "DisableClassicPipelineCreation")
          this.PublishTelemetryForDisableClassicPipelineCreationToggle(requestContext, tuple.Item3);
        if (tuple.Item1 == "DisableClassicBuildPipelineCreation")
          this.PublishTelemetryForDisableClassicBuildPipelineCreationToggle(requestContext, tuple.Item3);
        if (tuple.Item1 == "DisableClassicBuildPipelineCreation")
          this.PublishTelemetryForDisableClassicReleasePipelineCreationToggle(requestContext, tuple.Item3);
      }
    }

    private void PublishTelemetryForDisableClassicPipelineCreationToggle(
      IVssRequestContext requestContext,
      bool newValue)
    {
      CustomerIntelligenceData properties = new CustomerIntelligenceData();
      properties.Add("EventType", "UpdateDisablePipelineCreationToggle");
      properties.Add("Level", "Project");
      properties.Add("ProjectId", (object) this.m_projectId);
      properties.Add("NewValue", newValue);
      requestContext.GetService<CustomerIntelligenceService>().Publish(requestContext, "Build2", "DisableClassicPipelineCreation", properties);
    }

    private void PublishTelemetryForDisableClassicBuildPipelineCreationToggle(
      IVssRequestContext requestContext,
      bool newValue)
    {
      CustomerIntelligenceData properties = new CustomerIntelligenceData();
      properties.Add("EventType", "UpdateDisablePipelineCreationToggle");
      properties.Add("Level", "Project");
      properties.Add("ProjectId", (object) this.m_projectId);
      properties.Add("NewValue", newValue);
      requestContext.GetService<CustomerIntelligenceService>().Publish(requestContext, "Build2", "DisableClassicBuildPipelineCreation", properties);
    }

    private void PublishTelemetryForDisableClassicReleasePipelineCreationToggle(
      IVssRequestContext requestContext,
      bool newValue)
    {
      CustomerIntelligenceData properties = new CustomerIntelligenceData();
      properties.Add("EventType", "UpdateDisablePipelineCreationToggle");
      properties.Add("Level", "Project");
      properties.Add("ProjectId", (object) this.m_projectId);
      properties.Add("NewValue", newValue);
      requestContext.GetService<CustomerIntelligenceService>().Publish(requestContext, "Build2", "DisableClassicReleasePipelineCreation", properties);
    }

    private void PublishSettingChangedTelemetry(
      IVssRequestContext requestContext,
      string settingName,
      bool oldValue,
      bool newValue)
    {
      CustomerIntelligenceData properties = new CustomerIntelligenceData();
      properties.Add("EventType", "TogglePipelineSetting");
      properties.Add("SettingName", settingName);
      properties.Add("Level", "Project");
      properties.Add("IsHighRiskSetting", ProjectPipelineGeneralSettingsHelper.IsHighRiskSetting(settingName));
      properties.Add("ProjectId", (object) this.m_projectId);
      properties.Add("OldValue", oldValue);
      properties.Add("NewValue", newValue);
      requestContext.GetService<CustomerIntelligenceService>().Publish(requestContext, "Build2", "PipelineSettingsTelemetry", properties);
    }

    private static bool IsHighRiskSetting(string settingName) => ProjectPipelineGeneralSettingsHelper._highRiskSettings.Contains(settingName);

    private static (string SettingName, bool Updated, bool NewValue) GetUpdatedProjectSetting(
      string settingName,
      ref bool currentValue,
      bool? newValue,
      bool orgEnforced)
    {
      bool flag = false;
      if (newValue.HasValue)
      {
        int num1 = currentValue ? 1 : 0;
        bool? nullable = newValue;
        int num2 = nullable.GetValueOrDefault() ? 1 : 0;
        if (!(num1 == num2 & nullable.HasValue) && !orgEnforced)
        {
          currentValue = newValue.Value;
          flag = true;
        }
      }
      return (settingName, flag, currentValue);
    }

    private static (string SettingName, bool Updated, bool NewValue) GetUpdatedProjectSetting(
      string settingName,
      ref bool? currentValue,
      bool? newValue,
      bool? orgEnforced)
    {
      bool flag1 = false;
      bool? nullable1;
      if (newValue.HasValue)
      {
        bool? nullable2 = currentValue;
        nullable1 = newValue;
        if (!(nullable2.GetValueOrDefault() == nullable1.GetValueOrDefault() & nullable2.HasValue == nullable1.HasValue))
        {
          nullable1 = orgEnforced;
          bool flag2 = true;
          if (!(nullable1.GetValueOrDefault() == flag2 & nullable1.HasValue))
          {
            currentValue = new bool?(newValue.Value);
            flag1 = true;
          }
        }
      }
      string str = settingName;
      int num1 = flag1 ? 1 : 0;
      nullable1 = currentValue;
      bool flag3 = true;
      int num2 = nullable1.GetValueOrDefault() == flag3 & nullable1.HasValue ? 1 : 0;
      return (str, num1 != 0, num2 != 0);
    }
  }
}
