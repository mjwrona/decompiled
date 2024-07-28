// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebServer.GitPermissionsUtility
// Assembly: Microsoft.TeamFoundation.SourceControl.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 47C05AF5-4412-4EF0-BF63-D475D8EECD03
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.SourceControl.WebServer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.SourceControl.WebServer
{
  public static class GitPermissionsUtility
  {
    public static string CreateSecurityToken(
      IVssRequestContext requestContext,
      ITfsGitRepository repo,
      out IVssSecurityNamespace securityNamespace)
    {
      ISecuredLocalSecurityService service = requestContext.GetService<ISecuredLocalSecurityService>();
      securityNamespace = service.GetSecurityNamespace(requestContext, GitConstants.GitSecurityNamespaceId);
      return GitUtils.CalculateSecurable(repo.Key.GetProjectUri(), repo.Key.RepoId, (string) null);
    }

    private static void VerifyPermissions(
      IVssRequestContext requestContext,
      IdentityDescriptor identityDescriptor,
      string displayName,
      IVssSecurityNamespace securityNamespace,
      string securityToken)
    {
      if (string.IsNullOrEmpty(identityDescriptor?.Identifier))
        throw new InvalidArgumentValueException(Resources.Get("InvalidReviewer"));
      using (IVssRequestContext userContext = requestContext.CreateUserContext(identityDescriptor))
      {
        if (!securityNamespace.HasPermission(userContext, securityToken, 2, false))
          throw new GitPullRequestReviewerPermission(displayName);
      }
    }

    public static void VerifyPermissionsForIdentities(
      IVssRequestContext requestContext,
      IEnumerable<TeamFoundationIdentity> identities,
      IVssSecurityNamespace securityNamespace,
      string securityToken,
      bool checkTeams,
      bool checkAadGroups)
    {
      foreach (TeamFoundationIdentity identity in identities)
      {
        if (identity == null)
          throw new InvalidArgumentValueException(Resources.Get("InvalidReviewer"));
        if (checkTeams || !identity.IsContainer || checkAadGroups && AadIdentityHelper.IsAadGroup(identity.Descriptor))
          GitPermissionsUtility.VerifyPermissions(requestContext, identity.Descriptor, identity.DisplayName, securityNamespace, securityToken);
      }
    }

    public static void VerifyPermissionsForTfid(
      IVssRequestContext requestContext,
      Guid tfId,
      IVssSecurityNamespace securityNamespace,
      string securityToken,
      bool checkTeams,
      bool checkAadGroups)
    {
      TeamFoundationIdentity[] identities = requestContext.GetService<ITeamFoundationIdentityService>().ReadIdentities(requestContext, new Guid[1]
      {
        tfId
      });
      GitPermissionsUtility.VerifyPermissionsForIdentities(requestContext, (IEnumerable<TeamFoundationIdentity>) identities, securityNamespace, securityToken, checkTeams, checkAadGroups);
    }
  }
}
