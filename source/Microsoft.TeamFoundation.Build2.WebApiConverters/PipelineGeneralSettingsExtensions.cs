// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.WebApiConverters.PipelineGeneralSettingsExtensions
// Assembly: Microsoft.TeamFoundation.Build2.WebApiConverters, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9963E502-0ADF-445A-89CE-AAA11161F2F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.WebApiConverters.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;

namespace Microsoft.TeamFoundation.Build2.WebApiConverters
{
  public static class PipelineGeneralSettingsExtensions
  {
    public static Microsoft.TeamFoundation.Build.WebApi.PipelineGeneralSettings ToWebApiPipelineGeneralSettings(
      this Microsoft.TeamFoundation.Build2.Server.PipelineGeneralSettings srvPipelineSettings,
      ISecuredObject securedObject)
    {
      ArgumentUtility.CheckForNull<ISecuredObject>(securedObject, nameof (securedObject));
      return new Microsoft.TeamFoundation.Build.WebApi.PipelineGeneralSettings(securedObject)
      {
        StatusBadgesArePrivate = new bool?(!srvPipelineSettings.StatusBadgesArePublic),
        EnforceSettableVar = new bool?(srvPipelineSettings.EnforceSettableVar),
        EnforceJobAuthScope = new bool?(srvPipelineSettings.EnforceJobAuthScope),
        EnforceJobAuthScopeForReleases = new bool?(srvPipelineSettings.EnforceJobAuthScopeForReleases),
        PublishPipelineMetadata = new bool?(srvPipelineSettings.PublishPipelineMetadata),
        EnforceReferencedRepoScopedToken = new bool?(srvPipelineSettings.EnforceReferencedRepoScopedToken),
        DisableClassicPipelineCreation = new bool?(srvPipelineSettings.DisableClassicPipelineCreation),
        DisableClassicBuildPipelineCreation = srvPipelineSettings.DisableClassicBuildPipelineCreation,
        DisableClassicReleasePipelineCreation = srvPipelineSettings.DisableClassicReleasePipelineCreation,
        ForkProtectionEnabled = new bool?(srvPipelineSettings.ForkProtectionEnabled),
        BuildsEnabledForForks = new bool?(srvPipelineSettings.BuildsEnabledForForks),
        EnforceJobAuthScopeForForks = new bool?(srvPipelineSettings.EnforceJobAuthScopeForForks),
        EnforceNoAccessToSecretsFromForks = new bool?(srvPipelineSettings.EnforceNoAccessToSecretsFromForks),
        IsCommentRequiredForPullRequest = new bool?(srvPipelineSettings.IsCommentRequiredForPullRequest),
        RequireCommentsForNonTeamMembersOnly = new bool?(srvPipelineSettings.RequireCommentsForNonTeamMembersOnly),
        RequireCommentsForNonTeamMemberAndNonContributors = new bool?(srvPipelineSettings.RequireCommentsForNonTeamMemberAndNonContributors),
        EnableShellTasksArgsSanitizing = new bool?(srvPipelineSettings.EnableShellTasksArgsSanitizing),
        EnableShellTasksArgsSanitizingAudit = new bool?(srvPipelineSettings.EnableShellTasksArgsSanitizingAudit),
        DisableImpliedYAMLCiTrigger = new bool?(srvPipelineSettings.DisableImpliedYAMLCiTrigger)
      };
    }
  }
}
