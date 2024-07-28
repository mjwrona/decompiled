// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.VersionControl.VersionControlAdminViewModel
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.VersionControl, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AEC6FD7F-E72C-4C65-8428-206D27D3BF89
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.VersionControl.dll

using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.TeamFoundation.VersionControl.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess.VersionControl
{
  [DataContract]
  public class VersionControlAdminViewModel
  {
    [DataMember(Name = "tfsRepositoryPermissionSet")]
    public Guid TfsRepositoryPermissionSet { get; set; }

    [DataMember(Name = "gitRepositoryPermissionSet")]
    public Guid GitRepositoryPermissionSet { get; set; }

    [DataMember(Name = "projectGuid")]
    public Guid ProjectGuid { get; set; }

    [DataMember(Name = "projectUri")]
    public string ProjectUri { get; set; }

    [DataMember(Name = "projectVersionControlInfo")]
    public VersionControlProjectInfo ProjectVersionControlInfo { get; set; }

    [DataMember(Name = "gitRepositories")]
    public IList<GitRepository> GitRepositories { get; set; }

    [DataMember(Name = "defaultGitRepositoryId")]
    public Guid DefaultGitRepositoryId { get; set; }

    internal static VersionControlAdminViewModel Create(
      TfsWebContext webContext,
      VersionControlRepositoryInfoFactory repositoryInfoFactory)
    {
      VersionControlAdminViewModel controlAdminViewModel = new VersionControlAdminViewModel();
      controlAdminViewModel.ProjectGuid = webContext.CurrentProjectGuid;
      controlAdminViewModel.ProjectUri = webContext.CurrentProjectUri?.AbsoluteUri ?? string.Empty;
      controlAdminViewModel.ProjectVersionControlInfo = repositoryInfoFactory.ProjectVersionControlInfo;
      controlAdminViewModel.TfsRepositoryPermissionSet = SecurityConstants.RepositorySecurityNamespaceGuid;
      controlAdminViewModel.GitRepositoryPermissionSet = GitConstants.GitSecurityNamespaceId;
      if (controlAdminViewModel.ProjectVersionControlInfo != null && controlAdminViewModel.ProjectVersionControlInfo.SupportsGit)
      {
        controlAdminViewModel.GitRepositories = (IList<GitRepository>) repositoryInfoFactory.GitRepositories.ToList<GitRepository>();
        GitRepository gitRepository = controlAdminViewModel.GitRepositories.FirstOrDefault<GitRepository>((Func<GitRepository, bool>) (repo => string.Equals(repo.Name, repositoryInfoFactory.DefaultGitRepositoryName, StringComparison.OrdinalIgnoreCase)));
        if (gitRepository != null)
          controlAdminViewModel.DefaultGitRepositoryId = gitRepository.Id;
      }
      return controlAdminViewModel;
    }
  }
}
