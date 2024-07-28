// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.GitPushUtil
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using System;

namespace Microsoft.TeamFoundation.Git.Server
{
  public class GitPushUtil
  {
    public static void CheckRefUpdateSuccess(
      IVssRequestContext requestContext,
      TfsGitRefUpdateResult refUpdate)
    {
      if (refUpdate.Succeeded)
        return;
      switch (refUpdate.Status)
      {
        case GitRefUpdateStatus.ForcePushRequired:
          throw new GitForcePushDeniedException();
        case GitRefUpdateStatus.StaleOldObjectId:
          throw new GitReferenceStaleException(refUpdate.Name);
        case GitRefUpdateStatus.InvalidRefName:
          throw new InvalidGitRefNameException(refUpdate.Name);
        case GitRefUpdateStatus.UnresolvableToCommit:
          throw new GitUnresolvableToCommitException(refUpdate.NewObjectId);
        case GitRefUpdateStatus.WritePermissionRequired:
          throw new GitNeedsPermissionException(requestContext.AuthenticatedUserName, GitRepositoryPermissions.GenericContribute, GitPermissionScope.Repository);
        case GitRefUpdateStatus.ManageNotePermissionRequired:
          throw new GitNeedsPermissionException(requestContext.AuthenticatedUserName, GitRepositoryPermissions.ManageNote, GitPermissionScope.Repository);
        case GitRefUpdateStatus.CreateBranchPermissionRequired:
          throw new GitNeedsPermissionException(requestContext.AuthenticatedUserName, GitRepositoryPermissions.CreateBranch, GitPermissionScope.Repository);
        case GitRefUpdateStatus.CreateTagPermissionRequired:
          throw new GitNeedsPermissionException(requestContext.AuthenticatedUserName, GitRepositoryPermissions.CreateTag, GitPermissionScope.Repository);
        case GitRefUpdateStatus.RejectedByPlugin:
          throw new GitRefUpdateRejectedByPluginException(refUpdate.CustomMessage);
        case GitRefUpdateStatus.Locked:
          ITeamFoundationIdentityService service = requestContext.GetService<ITeamFoundationIdentityService>();
          string displayName = IdentityHelper.Instance.GetUserIdentity(requestContext, service, refUpdate.IsLockedById ?? Guid.Empty)?.DisplayName;
          throw new GitRefLockedException(refUpdate.Name, displayName);
        case GitRefUpdateStatus.RefNameConflict:
          throw new GitRefNameConflictException(refUpdate.Name, refUpdate.ConflictingName);
        case GitRefUpdateStatus.RejectedByPolicy:
          throw new GitRefUpdateRejectedByPolicyException(refUpdate.CustomMessage);
        default:
          throw new NotSupportedException(Resources.Format("UnknownGitPushFailureFormat", (object) Enum.GetName(typeof (GitRefUpdateStatus), (object) refUpdate.Status)));
      }
    }
  }
}
