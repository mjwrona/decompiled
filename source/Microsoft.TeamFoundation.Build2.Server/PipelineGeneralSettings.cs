// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.PipelineGeneralSettings
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

namespace Microsoft.TeamFoundation.Build2.Server
{
  public struct PipelineGeneralSettings
  {
    public bool StatusBadgesArePublic;
    public bool EnforceSettableVar;
    public bool AuditEnforceSettableVar;
    public bool EnforceJobAuthScope;
    public bool EnforceJobAuthScopeForReleases;
    public bool PublishPipelineMetadata;
    public bool EnforceReferencedRepoScopedToken;
    public bool DisableStageChooser;
    public bool DisableClassicPipelineCreation;
    public bool EnableShellTasksArgsSanitizing;
    public bool EnableShellTasksArgsSanitizingAudit;
    public bool? DisableClassicBuildPipelineCreation;
    public bool? DisableClassicReleasePipelineCreation;
    public bool ForkProtectionEnabled;
    public bool BuildsEnabledForForks;
    public bool EnforceJobAuthScopeForForks;
    public bool EnforceNoAccessToSecretsFromForks;
    public bool IsCommentRequiredForPullRequest;
    public bool RequireCommentsForNonTeamMembersOnly;
    public bool RequireCommentsForNonTeamMemberAndNonContributors;
    public bool DisableImpliedYAMLCiTrigger;
    public static readonly PipelineGeneralSettings Default = new PipelineGeneralSettings()
    {
      StatusBadgesArePublic = true,
      EnforceSettableVar = false,
      AuditEnforceSettableVar = false,
      EnforceJobAuthScope = false,
      EnforceJobAuthScopeForReleases = false,
      PublishPipelineMetadata = false,
      EnforceReferencedRepoScopedToken = false,
      DisableStageChooser = false,
      DisableClassicPipelineCreation = false,
      DisableClassicBuildPipelineCreation = new bool?(false),
      DisableClassicReleasePipelineCreation = new bool?(false),
      ForkProtectionEnabled = PipelineTriggerSettings.Default.ForkProtectionEnabled,
      BuildsEnabledForForks = PipelineTriggerSettings.Default.BuildsEnabledForForks,
      EnforceJobAuthScopeForForks = PipelineTriggerSettings.Default.EnforceJobAuthScopeForForks,
      EnforceNoAccessToSecretsFromForks = PipelineTriggerSettings.Default.EnforceNoAccessToSecretsFromForks,
      IsCommentRequiredForPullRequest = PipelineTriggerSettings.Default.IsCommentRequiredForPullRequest,
      RequireCommentsForNonTeamMembersOnly = PipelineTriggerSettings.Default.RequireCommentsForNonTeamMembersOnly,
      RequireCommentsForNonTeamMemberAndNonContributors = PipelineTriggerSettings.Default.RequireCommentsForNonTeamMemberAndNonContributors,
      EnableShellTasksArgsSanitizing = false,
      EnableShellTasksArgsSanitizingAudit = false,
      DisableImpliedYAMLCiTrigger = false
    };
  }
}
