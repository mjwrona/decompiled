// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.VersionControl.VersionControlViewModel
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.VersionControl, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AEC6FD7F-E72C-4C65-8428-206D27D3BF89
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.VersionControl.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server;
using Microsoft.TeamFoundation.Server.WebAccess.Presentation;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.TeamFoundation.VersionControl.Common;
using Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server.FeatureManagement;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess.VersionControl
{
  [DataContract]
  public class VersionControlViewModel : VersionControlSecuredObject
  {
    private static readonly string s_sshRegistryRoot = "/Configuration/SshServer/";
    private static readonly string s_sshEnabledRegistrySetting = string.Format("{0}{1}", (object) VersionControlViewModel.s_sshRegistryRoot, (object) "Enabled");
    private static readonly string s_sshPortRegistrySetting = string.Format("{0}{1}", (object) VersionControlViewModel.s_sshRegistryRoot, (object) "Port");
    private static readonly RegistryQuery s_sshRegistryQuery = new RegistryQuery(string.Format("{0}...", (object) VersionControlViewModel.s_sshRegistryRoot));

    [DataMember(Name = "repositoryPermissionSet")]
    public Guid RepositoryPermissionSet { get; set; }

    [DataMember(Name = "projectGuid")]
    public Guid ProjectGuid { get; set; }

    [DataMember(Name = "projectUri")]
    public string ProjectUri { get; set; }

    [DataMember(Name = "projectVersionControlInfo")]
    public VersionControlProjectInfo ProjectVersionControlInfo { get; set; }

    [DataMember(Name = "gitRepository")]
    public GitRepository Repository { get; set; }

    [DataMember(Name = "isEmptyRepository")]
    public bool IsEmptyRepository { get; set; }

    [DataMember(Name = "defaultGitBranchName")]
    public string DefaultGitBranchName { get; set; }

    [DataMember(Name = "vcUserPreferences")]
    public VersionControlUserPreferences VCUserPreferences { get; set; }

    [DataMember(Name = "reviewMode")]
    public bool ReviewMode { get; set; }

    [DataMember(Name = "openInVsLink")]
    public string OpenInVsLink { get; set; }

    [DataMember(Name = "deletedUserDefaultBranchName")]
    public string DeletedUserDefaultBranchName { get; set; }

    [DataMember(Name = "sshUrl")]
    public string SshUrl { get; set; }

    [DataMember(Name = "sshEnabled")]
    public bool SshEnabled { get; set; }

    [DataMember(Name = "cloneUrl")]
    public string CloneUrl { get; set; }

    [DataMember(Name = "activeImportRequest")]
    public Microsoft.TeamFoundation.SourceControl.WebApi.GitImportRequest ActiveImportRequest { get; set; }

    [DataMember(Name = "notFoundRepoName", EmitDefaultValue = false)]
    public string NotFoundRepoName { get; set; }

    [DataMember(Name = "disabledRepoName", EmitDefaultValue = false)]
    public string DisabledRepoName { get; set; }

    [DataMember(Name = "showCloneButtonOnL2Header")]
    public bool ShowCloneButtonOnL2Header { get; set; }

    public static VersionControlViewModel Create(
      VersionControlRepositoryInfoFactory repositoryInfoFactory,
      GitRepository gitRepository = null)
    {
      VersionControlViewModel model = new VersionControlViewModel();
      model.Repository = gitRepository;
      model.VCUserPreferences = VersionControlUserPreferences.GetUserPreferences(repositoryInfoFactory.RequestContext);
      model.ProjectVersionControlInfo = repositoryInfoFactory.ProjectVersionControlInfo;
      if (repositoryInfoFactory.Project != null)
      {
        model.ProjectGuid = repositoryInfoFactory.Project.Id;
        model.ProjectUri = repositoryInfoFactory.Project.Uri;
      }
      Guid? currentRepositoryGuid = new Guid?();
      if (gitRepository == null)
      {
        model.RepositoryPermissionSet = SecurityConstants.RepositorySecurityNamespaceGuid;
      }
      else
      {
        model.RepositoryPermissionSet = GitConstants.GitSecurityNamespaceId;
        GitRepositoryInfo gitRepositoryById = repositoryInfoFactory.GetGitRepositoryById(gitRepository.Id);
        model.DefaultGitBranchName = gitRepositoryById.GitProvider.GetDefaultBranchName(true, model.Repository.DefaultBranch);
        if (string.IsNullOrEmpty(model.DefaultGitBranchName))
        {
          model.IsEmptyRepository = true;
          model.ActiveImportRequest = gitRepositoryById.GitProvider.GetActiveImportRequest();
        }
        currentRepositoryGuid = new Guid?(gitRepositoryById.GitProvider.Repository.Key.RepoId);
      }
      model.OpenInVsLink = VisualStudioLinkHelper.GetOpenInVisualStudioUri(repositoryInfoFactory.RequestContext, NavigationContextLevels.Project, currentRepositoryGuid);
      if (model.Repository != null)
      {
        model.CloneUrl = string.IsNullOrEmpty(model.Repository.RemoteUrl) ? string.Empty : new Uri(model.Repository.RemoteUrl).AbsoluteUri;
        model.SshUrl = model.Repository.SshUrl;
        model.SshEnabled = model.SshUrl != string.Empty;
        repositoryInfoFactory.RequestContext.GetService<IContributedFeatureService>();
        model.ShowCloneButtonOnL2Header = true;
      }
      VersionControlViewModel.SetSecuredObject(model);
      return model;
    }

    public override void SetSecuredObject(ISecuredObject securedObject)
    {
      base.SetSecuredObject(securedObject);
      this.ProjectVersionControlInfo?.SetSecuredObject(securedObject);
      this.Repository?.SetSecuredObject(securedObject);
      this.VCUserPreferences?.SetSecuredObject(securedObject);
      this.ActiveImportRequest?.SetSecuredObject(securedObject);
    }

    private static void SetSecuredObject(VersionControlViewModel model)
    {
      if (model.Repository == null)
        return;
      ISecuredObject repositoryReadOnly = GitSecuredObjectFactory.CreateRepositoryReadOnly(model.Repository.ProjectReference.Id, model.Repository.Id);
      model.SetSecuredObject(repositoryReadOnly);
    }
  }
}
