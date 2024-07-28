// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.WebServer.BuildGeneralSettingsController
// Assembly: Microsoft.TeamFoundation.Build2.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FDDA87C8-3548-4A75-AA18-4FB488450659
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.WebServer.dll

using Microsoft.TeamFoundation.Build2.Server;
using Microsoft.TeamFoundation.Build2.WebApiConverters;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System.Web.Http;

namespace Microsoft.TeamFoundation.Build2.WebServer
{
  [ControllerApiVersion(6.0)]
  [VersionedApiControllerCustomName(Area = "build", ResourceName = "generalSettings", ResourceVersion = 1)]
  [CheckWellFormedProject(Required = true)]
  public class BuildGeneralSettingsController : BuildApiController
  {
    [HttpGet]
    public virtual Microsoft.TeamFoundation.Build.WebApi.PipelineGeneralSettings GetBuildGeneralSettings()
    {
      ProjectInfo projectInfo = this.ProjectInfo;
      TeamProjectReference projectReference = projectInfo != null ? projectInfo.ToTeamProjectReference(this.TfsRequestContext) : (TeamProjectReference) null;
      return new ProjectPipelineGeneralSettingsHelper(this.TfsRequestContext, this.ProjectInfo.Id, false).GetEffectiveSettings().ToWebApiPipelineGeneralSettings((ISecuredObject) projectReference);
    }

    [HttpPatch]
    public virtual Microsoft.TeamFoundation.Build.WebApi.PipelineGeneralSettings UpdateBuildGeneralSettings(
      [FromBody] Microsoft.TeamFoundation.Build.WebApi.PipelineGeneralSettings newSettings)
    {
      ArgumentUtility.CheckForNull<Microsoft.TeamFoundation.Build.WebApi.PipelineGeneralSettings>(newSettings, nameof (newSettings));
      ProjectInfo projectInfo = this.ProjectInfo;
      TeamProjectReference projectReference = projectInfo != null ? projectInfo.ToTeamProjectReference(this.TfsRequestContext) : (TeamProjectReference) null;
      ProjectPipelineGeneralSettingsHelper generalSettingsHelper = new ProjectPipelineGeneralSettingsHelper(this.TfsRequestContext, this.ProjectInfo.Id, false);
      IVssRequestContext tfsRequestContext = this.TfsRequestContext;
      bool? nullable = newSettings.StatusBadgesArePrivate;
      bool? statusBadgesArePublic = nullable.HasValue ? new bool?(!nullable.GetValueOrDefault()) : new bool?();
      bool? enforceSettableVar = newSettings.EnforceSettableVar;
      bool? enforceJobAuthScope1 = newSettings.EnforceJobAuthScope;
      bool? scopeForReleases = newSettings.EnforceJobAuthScopeForReleases;
      bool? pipelineMetadata = newSettings.PublishPipelineMetadata;
      bool? referencedRepoScopedToken = newSettings.EnforceReferencedRepoScopedToken;
      bool? protectionEnabled = newSettings.ForkProtectionEnabled;
      bool? buildsEnabledForForks1 = newSettings.BuildsEnabledForForks;
      bool? authScopeForForks = newSettings.EnforceJobAuthScopeForForks;
      bool? secretsFromForks = newSettings.EnforceNoAccessToSecretsFromForks;
      bool? requiredForPullRequest = newSettings.IsCommentRequiredForPullRequest;
      bool? nonTeamMembersOnly = newSettings.RequireCommentsForNonTeamMembersOnly;
      bool? andNonContributors = newSettings.RequireCommentsForNonTeamMemberAndNonContributors;
      bool? pipelineCreation1 = newSettings.DisableClassicBuildPipelineCreation;
      bool? pipelineCreation2 = newSettings.DisableClassicReleasePipelineCreation;
      bool? impliedYamlCiTrigger = newSettings.DisableImpliedYAMLCiTrigger;
      nullable = new bool?();
      bool? auditEnforceSettableVar = nullable;
      bool? enforceJobAuthScope2 = enforceJobAuthScope1;
      bool? enforceJobAuthScopeForReleases = scopeForReleases;
      bool? publishPipelineMetadata = pipelineMetadata;
      bool? enforceReferencedRepoScopedToken = referencedRepoScopedToken;
      nullable = new bool?();
      bool? disableClassicPipelineCreation = nullable;
      bool? disableClassicBuildPipelineCreation = pipelineCreation1;
      bool? disableClassicReleasePipelineCreation = pipelineCreation2;
      bool? forkProtectionEnabled = protectionEnabled;
      bool? buildsEnabledForForks2 = buildsEnabledForForks1;
      bool? enforceJobAuthScopeForForks = authScopeForForks;
      bool? enforceNoAccessToSecretsFromForks = secretsFromForks;
      bool? isCommentRequiredForPullRequest = requiredForPullRequest;
      bool? requireCommentsForNonTeamMembersOnly = nonTeamMembersOnly;
      bool? requireCommentsForNonTeamMemberAndNonContributors = andNonContributors;
      nullable = new bool?();
      bool? enableShellTasksArgsSanitizing = nullable;
      nullable = new bool?();
      bool? enableShellTasksArgsSanitizingAudit = nullable;
      bool? disableImpliedYAMLCiTrigger = impliedYamlCiTrigger;
      generalSettingsHelper.UpdateSettings(tfsRequestContext, statusBadgesArePublic, enforceSettableVar, auditEnforceSettableVar, enforceJobAuthScope2, enforceJobAuthScopeForReleases, publishPipelineMetadata, enforceReferencedRepoScopedToken, disableClassicPipelineCreation, disableClassicBuildPipelineCreation, disableClassicReleasePipelineCreation, forkProtectionEnabled, buildsEnabledForForks2, enforceJobAuthScopeForForks, enforceNoAccessToSecretsFromForks, isCommentRequiredForPullRequest, requireCommentsForNonTeamMembersOnly, requireCommentsForNonTeamMemberAndNonContributors, enableShellTasksArgsSanitizing, enableShellTasksArgsSanitizingAudit, disableImpliedYAMLCiTrigger);
      return generalSettingsHelper.GetEffectiveSettings().ToWebApiPipelineGeneralSettings((ISecuredObject) projectReference);
    }
  }
}
