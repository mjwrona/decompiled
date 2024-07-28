// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Admin.GitPermissionsManager
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Admin, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 31215F45-B8A9-42A7-99A7-F8CB77B7D405
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Admin.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.TeamFoundation.Server.WebAccess.Admin
{
  internal class GitPermissionsManager : SecurityNamespacePermissionsManager
  {
    private const char c_tokenDelimiter = '/';
    private const char c_refDelimiter = '^';

    public GitPermissionsManager(
      IVssRequestContext requestContext,
      Guid permissionsIdentifier,
      string token)
      : base(requestContext, permissionsIdentifier, GitPermissionsManager.GetCanonicalTokenFromWebToken(token))
    {
      if (!this.UserHasReadAccess)
        return;
      this.InheritPermissions = this.PermissionSets[GitConstants.GitSecurityNamespaceId].GetAccessControlList(requestContext).InheritPermissions;
    }

    public override bool CanEditAdminPermissions => true;

    public override bool CanTokenInheritPermissions => this.Token != "repoV2/";

    public override void ChangeInheritance(
      IVssRequestContext requestContext,
      bool inheritPermissions)
    {
      SecurityNamespacePermissionSet permissionSet = this.PermissionSets[GitConstants.GitSecurityNamespaceId];
      this.ChangeInheritance(requestContext, permissionSet, inheritPermissions);
    }

    protected override Dictionary<Guid, SecurityNamespacePermissionSet> CreatePermissionSets(
      IVssRequestContext requestContext)
    {
      Dictionary<Guid, SecurityNamespacePermissionSet> permissionSets = new Dictionary<Guid, SecurityNamespacePermissionSet>();
      int permissionsToShow = this.GetPermissionsToShow(requestContext);
      SecurityNamespacePermissionSet namespacePermissionSet = new SecurityNamespacePermissionSet(requestContext, GitConstants.GitSecurityNamespaceId, this.Token, permissionsToShow);
      permissionSets.Add(GitConstants.GitSecurityNamespaceId, namespacePermissionSet);
      return permissionSets;
    }

    protected override IAccessControlList GetParentAccessControlList(
      IVssRequestContext requestContext,
      string token,
      List<IdentityDescriptor> descriptors)
    {
      ArgumentUtility.CheckForNull<string>(token, nameof (token));
      string parentToken = GitPermissionsManager.GetParentToken(token);
      return token != null ? this.PermissionSets[NamespacePermissionSetConstants.Git].SecuredSecurityNamespace.QueryAccessControlLists(requestContext, parentToken, (IEnumerable<IdentityDescriptor>) descriptors, true, false).FirstOrDefault<IAccessControlList>() : (IAccessControlList) null;
    }

    protected override string GetTokenDisplayName(IVssRequestContext requestContext, string token)
    {
      string name = requestContext.ServiceHost.Name;
      Func<Guid, string> getProjectName = (Func<Guid, string>) (projectId => requestContext.GetService<IProjectService>().GetProjectName(requestContext, projectId));
      Func<Guid, string> getRepoName = (Func<Guid, string>) (repoId =>
      {
        using (ITfsGitRepository repositoryById = requestContext.GetService<ITeamFoundationGitRepositoryService>().FindRepositoryById(requestContext, repoId))
          return repositoryById.Name;
      });
      return GitPermissionsManager.GetTokenDisplayName(token, name, getProjectName, getRepoName);
    }

    private int GetPermissionsToShow(IVssRequestContext requestContext)
    {
      GitPermissionScope tokenScope = GitPermissionsUtil.GetTokenScope(this.Token);
      GitRepositoryPermissions permissionsToShow;
      switch (tokenScope)
      {
        case GitPermissionScope.Project:
          permissionsToShow = GitRepositoryPermissions.ProjectLevelPermissions;
          break;
        case GitPermissionScope.Repository:
          permissionsToShow = GitRepositoryPermissions.RepositoryLevelPermissions;
          break;
        case GitPermissionScope.Branch:
          permissionsToShow = GitRepositoryPermissions.BranchLevelPermissions;
          break;
        case GitPermissionScope.NonBranchRef:
          permissionsToShow = GitRepositoryPermissions.NonBranchRefLevelPermissions;
          break;
        case GitPermissionScope.BranchesRoot:
          permissionsToShow = GitRepositoryPermissions.BranchesRootLevelPermissions;
          break;
        default:
          permissionsToShow = GitRepositoryPermissions.None;
          break;
      }
      bool flag = false;
      IGitAdvSecService service = requestContext.GetService<IGitAdvSecService>();
      if (tokenScope == GitPermissionScope.Project)
      {
        flag = service.IsEnabledForAnyRepository(requestContext, new DateTime?());
      }
      else
      {
        Guid? repoIdFromToken = GitPermissionsUtil.GetRepoIdFromToken(this.Token);
        if (repoIdFromToken.HasValue)
        {
          using (ITfsGitRepository repositoryById = requestContext.GetService<ITeamFoundationGitRepositoryService>().FindRepositoryById(requestContext, repoIdFromToken.Value, true))
          {
            RepoKey key = repositoryById.Key;
            flag = service.IsEnabledForRepository(requestContext, key.GetProjectUri(), key.RepoId, new DateTime?());
          }
        }
      }
      if (!flag)
        permissionsToShow &= ~(GitRepositoryPermissions.ViewAdvSecAlerts | GitRepositoryPermissions.DismissAdvSecAlerts | GitRepositoryPermissions.ManageAdvSecScanning);
      return (int) permissionsToShow;
    }

    internal static string GetCanonicalTokenFromWebToken(string webToken)
    {
      string teamProjectGuid;
      string repoGuid;
      string refName;
      GitPermissionsManager.GetWebTokenParts(webToken, out teamProjectGuid, out repoGuid, out refName);
      string teamProjectUri = (string) null;
      if (!string.IsNullOrEmpty(teamProjectGuid))
        teamProjectUri = ProjectInfo.GetProjectUri(Guid.Parse(teamProjectGuid));
      Guid repositoryId = string.IsNullOrEmpty(repoGuid) ? Guid.Empty : Guid.Parse(repoGuid);
      return GitUtils.CalculateSecurable(teamProjectUri, repositoryId, refName);
    }

    internal static string GetTokenDisplayName(
      string securityToken,
      string collectionName,
      Func<Guid, string> getProjectName,
      Func<Guid, string> getRepoName)
    {
      ArgumentUtility.CheckForNull<string>(securityToken, nameof (securityToken));
      securityToken = securityToken.Trim('/');
      Encoding encoding = (Encoding) new UnicodeEncoding(false, false, true);
      string[] strArray = securityToken.Split('/');
      List<string> values = new List<string>(strArray.Length);
      values.Add(collectionName);
      for (int index = 1; index < strArray.Length; ++index)
      {
        string str = strArray[index];
        switch (index)
        {
          case 1:
            str = getProjectName(Guid.Parse(str));
            goto case 3;
          case 2:
            str = getRepoName(Guid.Parse(str));
            goto case 3;
          case 3:
          case 4:
            values.Add(str);
            continue;
          default:
            if (str.Length % 2 != 0)
              throw new InvalidOperationException("Bug in GitPermissionsManager: odd subtoken length");
            byte[] parsedObjectId;
            str = GitUtils.TryGetByteArrayFromString(str, str.Length, out parsedObjectId) ? encoding.GetString(parsedObjectId) : throw new InvalidOperationException("Bug in GitPermissionsManager: unparseable subtoken");
            goto case 3;
        }
      }
      return string.Join('/'.ToString(), (IEnumerable<string>) values);
    }

    internal static void GetWebTokenParts(
      string webToken,
      out string teamProjectGuid,
      out string repoGuid,
      out string refName)
    {
      teamProjectGuid = (string) null;
      repoGuid = (string) null;
      refName = (string) null;
      string str = webToken.Trim('/');
      string[] strArray = str.Split('/');
      if (strArray.Length > 1)
        teamProjectGuid = strArray[1];
      if (strArray.Length > 2)
        repoGuid = strArray[2];
      if (strArray.Length <= 3)
        return;
      int startIndex = strArray[0].Length + 1 + strArray[1].Length + 1 + strArray[2].Length + 1;
      refName = str.Substring(startIndex).Replace('^', '/');
    }

    internal static string GetParentToken(string securityToken)
    {
      securityToken = securityToken.Trim('/');
      int num = securityToken.LastIndexOf('/');
      return num < 0 ? (string) null : securityToken.Substring(0, num + 1);
    }
  }
}
