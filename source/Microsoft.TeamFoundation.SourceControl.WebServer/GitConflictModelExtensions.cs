// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebServer.GitConflictModelExtensions
// Assembly: Microsoft.TeamFoundation.SourceControl.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 47C05AF5-4412-4EF0-BF63-D475D8EECD03
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.SourceControl.WebServer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.Location.Server;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http.Routing;

namespace Microsoft.TeamFoundation.SourceControl.WebServer
{
  public static class GitConflictModelExtensions
  {
    public static List<Microsoft.TeamFoundation.SourceControl.WebApi.GitConflictUpdateResult> ToWebApiGitConflictUpdateResultList(
      this List<Microsoft.TeamFoundation.Git.Server.GitConflictUpdateResult> serverUpdateResultList,
      IVssRequestContext requestContext,
      RepoKey repoKey,
      GitMergeOriginRef conflictOrigin,
      UrlHelper urlHelper = null,
      ISecuredObject securedObject = null)
    {
      Dictionary<Guid, IdentityRef> dictionary = new Dictionary<Guid, IdentityRef>();
      IdentityLookupHelper.LoadIdentityRefs(requestContext, serverUpdateResultList.SelectMany<Microsoft.TeamFoundation.Git.Server.GitConflictUpdateResult, Guid>((Func<Microsoft.TeamFoundation.Git.Server.GitConflictUpdateResult, IEnumerable<Guid>>) (r => (IEnumerable<Guid>) new Guid[2]
      {
        r.UpdatedConflict.ResolvedBy,
        r.UpdatedConflict.ResolutionAuthor
      })), (IDictionary<Guid, IdentityRef>) dictionary);
      List<Microsoft.TeamFoundation.SourceControl.WebApi.GitConflictUpdateResult> updateResultList = new List<Microsoft.TeamFoundation.SourceControl.WebApi.GitConflictUpdateResult>(serverUpdateResultList.Count);
      foreach (Microsoft.TeamFoundation.Git.Server.GitConflictUpdateResult serverUpdateResult in serverUpdateResultList)
      {
        Microsoft.TeamFoundation.SourceControl.WebApi.GitConflictUpdateResult conflictUpdateResult = new Microsoft.TeamFoundation.SourceControl.WebApi.GitConflictUpdateResult()
        {
          ConflictId = serverUpdateResult.ConflictId
        };
        Exception error = serverUpdateResult.Error;
        conflictUpdateResult.CustomMessage = error?.Message;
        Microsoft.TeamFoundation.Git.Server.GitConflict updatedConflict = serverUpdateResult.UpdatedConflict;
        if (updatedConflict != null)
        {
          conflictUpdateResult.UpdateStatus = GitConflictUpdateStatus.Succeeded;
          conflictUpdateResult.UpdatedConflict = updatedConflict.ToWebApiGitConflict(requestContext, repoKey, conflictOrigin, urlHelper, securedObject, dictionary);
        }
        else
        {
          switch (error)
          {
            case JsonSerializationException _:
              conflictUpdateResult.UpdateStatus = GitConflictUpdateStatus.BadRequest;
              break;
            case InvalidOperationException _:
              conflictUpdateResult.UpdateStatus = GitConflictUpdateStatus.InvalidResolution;
              break;
            case NotSupportedException _:
              conflictUpdateResult.UpdateStatus = GitConflictUpdateStatus.UnsupportedConflictType;
              break;
            default:
              conflictUpdateResult.UpdateStatus = GitConflictUpdateStatus.NotFound;
              conflictUpdateResult.CustomMessage = Resources.Get("NotFound");
              break;
          }
        }
        updateResultList.Add(conflictUpdateResult);
      }
      return updateResultList;
    }

    public static List<Microsoft.TeamFoundation.SourceControl.WebApi.GitConflict> ToWebApiGitConflictList(
      this List<Microsoft.TeamFoundation.Git.Server.GitConflict> serverConflictList,
      IVssRequestContext requestContext,
      RepoKey repoKey,
      GitMergeOriginRef conflictOrigin,
      UrlHelper urlHelper = null,
      ISecuredObject securedObject = null)
    {
      List<Microsoft.TeamFoundation.SourceControl.WebApi.GitConflictUpdateResult> conflictUpdateResultList = new List<Microsoft.TeamFoundation.SourceControl.WebApi.GitConflictUpdateResult>(serverConflictList.Count);
      Dictionary<Guid, IdentityRef> identityRefLookup = new Dictionary<Guid, IdentityRef>();
      IdentityLookupHelper.LoadIdentityRefs(requestContext, serverConflictList.SelectMany<Microsoft.TeamFoundation.Git.Server.GitConflict, Guid>((Func<Microsoft.TeamFoundation.Git.Server.GitConflict, IEnumerable<Guid>>) (r => (IEnumerable<Guid>) new Guid[2]
      {
        r.ResolvedBy,
        r.ResolutionAuthor
      })), (IDictionary<Guid, IdentityRef>) identityRefLookup);
      return serverConflictList.Select<Microsoft.TeamFoundation.Git.Server.GitConflict, Microsoft.TeamFoundation.SourceControl.WebApi.GitConflict>((Func<Microsoft.TeamFoundation.Git.Server.GitConflict, Microsoft.TeamFoundation.SourceControl.WebApi.GitConflict>) (c => c.ToWebApiGitConflict(requestContext, repoKey, conflictOrigin, urlHelper, securedObject, identityRefLookup))).ToList<Microsoft.TeamFoundation.SourceControl.WebApi.GitConflict>();
    }

    public static Microsoft.TeamFoundation.SourceControl.WebApi.GitConflict ToWebApiGitConflict(
      this Microsoft.TeamFoundation.Git.Server.GitConflict serverItem,
      IVssRequestContext requestContext,
      RepoKey repoKey,
      GitMergeOriginRef conflictOrigin,
      UrlHelper urlHelper = null,
      ISecuredObject securedObject = null,
      Dictionary<Guid, IdentityRef> identityRefLookup = null)
    {
      identityRefLookup = identityRefLookup ?? new Dictionary<Guid, IdentityRef>();
      IdentityLookupHelper.LoadIdentityRefs(requestContext, (IEnumerable<Guid>) new Guid[2]
      {
        serverItem.ResolutionAuthor,
        serverItem.ResolvedBy
      }, (IDictionary<Guid, IdentityRef>) identityRefLookup);
      IdentityRef identityRef1 = (IdentityRef) null;
      if (serverItem.ResolvedBy != Guid.Empty)
        identityRefLookup.TryGetValue(serverItem.ResolvedBy, out identityRef1);
      IdentityRef identityRef2 = (IdentityRef) null;
      if (serverItem.ResolutionAuthor != Guid.Empty)
        identityRefLookup.TryGetValue(serverItem.ResolutionAuthor, out identityRef2);
      IdentityRef resolutionAuthor = identityRef2 ?? identityRef1;
      Microsoft.TeamFoundation.SourceControl.WebApi.GitConflict webApiGitConflict;
      switch (serverItem.ConflictType)
      {
        case GitConflictType.AddAdd:
          webApiGitConflict = (Microsoft.TeamFoundation.SourceControl.WebApi.GitConflict) new GitConflictAddAdd()
          {
            SourceBlob = GitItemUtility.CreateMinimalGitBlobRef(requestContext, serverItem.SourceObjectId, repoKey, urlHelper),
            TargetBlob = GitItemUtility.CreateMinimalGitBlobRef(requestContext, serverItem.TargetObjectId, repoKey, urlHelper),
            Resolution = serverItem.ToWebApiGitResolutionMergeContent(requestContext, repoKey, urlHelper, resolutionAuthor)
          };
          break;
        case GitConflictType.AddRename:
          webApiGitConflict = (Microsoft.TeamFoundation.SourceControl.WebApi.GitConflict) new GitConflictAddRename()
          {
            BaseBlob = GitItemUtility.CreateMinimalGitBlobRef(requestContext, serverItem.BaseObjectId, repoKey, urlHelper),
            SourceBlob = GitItemUtility.CreateMinimalGitBlobRef(requestContext, serverItem.SourceObjectId, repoKey, urlHelper),
            TargetBlob = GitItemUtility.CreateMinimalGitBlobRef(requestContext, serverItem.TargetObjectId, repoKey, urlHelper),
            TargetOriginalPath = serverItem.TargetPath.ToString(),
            Resolution = serverItem.ToWebApiGitResolutionPathConflict(requestContext, urlHelper, resolutionAuthor)
          };
          break;
        case GitConflictType.DeleteEdit:
          webApiGitConflict = (Microsoft.TeamFoundation.SourceControl.WebApi.GitConflict) new GitConflictDeleteEdit()
          {
            BaseBlob = GitItemUtility.CreateMinimalGitBlobRef(requestContext, serverItem.BaseObjectId, repoKey, urlHelper),
            TargetBlob = GitItemUtility.CreateMinimalGitBlobRef(requestContext, serverItem.TargetObjectId, repoKey, urlHelper),
            Resolution = serverItem.ToWebApiGitResolutionPickOneAction(requestContext, urlHelper, resolutionAuthor)
          };
          break;
        case GitConflictType.DeleteRename:
          webApiGitConflict = (Microsoft.TeamFoundation.SourceControl.WebApi.GitConflict) new GitConflictDeleteRename()
          {
            BaseBlob = GitItemUtility.CreateMinimalGitBlobRef(requestContext, serverItem.BaseObjectId, repoKey, urlHelper),
            TargetBlob = GitItemUtility.CreateMinimalGitBlobRef(requestContext, serverItem.TargetObjectId, repoKey, urlHelper),
            TargetNewPath = serverItem.TargetPath.ToString(),
            Resolution = serverItem.ToWebApiGitResolutionPickOneAction(requestContext, urlHelper, resolutionAuthor)
          };
          break;
        case GitConflictType.DirectoryFile:
          webApiGitConflict = (Microsoft.TeamFoundation.SourceControl.WebApi.GitConflict) new GitConflictDirectoryFile()
          {
            SourceTree = GitItemUtility.CreateMinimalGitTreeRef(requestContext, serverItem.SourceObjectId, repoKey, urlHelper),
            TargetBlob = GitItemUtility.CreateMinimalGitBlobRef(requestContext, serverItem.TargetObjectId, repoKey, urlHelper),
            Resolution = serverItem.ToWebApiGitResolutionPathConflict(requestContext, urlHelper, resolutionAuthor)
          };
          break;
        case GitConflictType.DirectoryChild:
          webApiGitConflict = new Microsoft.TeamFoundation.SourceControl.WebApi.GitConflict();
          break;
        case GitConflictType.EditDelete:
          webApiGitConflict = (Microsoft.TeamFoundation.SourceControl.WebApi.GitConflict) new GitConflictEditDelete()
          {
            BaseBlob = GitItemUtility.CreateMinimalGitBlobRef(requestContext, serverItem.BaseObjectId, repoKey, urlHelper),
            SourceBlob = GitItemUtility.CreateMinimalGitBlobRef(requestContext, serverItem.SourceObjectId, repoKey, urlHelper),
            Resolution = serverItem.ToWebApiGitResolutionPickOneAction(requestContext, urlHelper, resolutionAuthor)
          };
          break;
        case GitConflictType.EditEdit:
          webApiGitConflict = (Microsoft.TeamFoundation.SourceControl.WebApi.GitConflict) new GitConflictEditEdit()
          {
            BaseBlob = GitItemUtility.CreateMinimalGitBlobRef(requestContext, serverItem.BaseObjectId, repoKey, urlHelper),
            SourceBlob = GitItemUtility.CreateMinimalGitBlobRef(requestContext, serverItem.SourceObjectId, repoKey, urlHelper),
            TargetBlob = GitItemUtility.CreateMinimalGitBlobRef(requestContext, serverItem.TargetObjectId, repoKey, urlHelper),
            Resolution = serverItem.ToWebApiGitResolutionMergeContent(requestContext, repoKey, urlHelper, resolutionAuthor)
          };
          break;
        case GitConflictType.FileDirectory:
          webApiGitConflict = (Microsoft.TeamFoundation.SourceControl.WebApi.GitConflict) new GitConflictFileDirectory()
          {
            SourceBlob = GitItemUtility.CreateMinimalGitBlobRef(requestContext, serverItem.SourceObjectId, repoKey, urlHelper),
            TargetTree = GitItemUtility.CreateMinimalGitTreeRef(requestContext, serverItem.TargetObjectId, repoKey, urlHelper),
            Resolution = serverItem.ToWebApiGitResolutionPathConflict(requestContext, urlHelper, resolutionAuthor)
          };
          break;
        case GitConflictType.Rename1to2:
          webApiGitConflict = (Microsoft.TeamFoundation.SourceControl.WebApi.GitConflict) new GitConflictRename1to2()
          {
            BaseBlob = GitItemUtility.CreateMinimalGitBlobRef(requestContext, serverItem.BaseObjectId, repoKey, urlHelper),
            SourceBlob = GitItemUtility.CreateMinimalGitBlobRef(requestContext, serverItem.SourceObjectId, repoKey, urlHelper),
            SourceNewPath = serverItem.SourcePath.ToString(),
            TargetBlob = GitItemUtility.CreateMinimalGitBlobRef(requestContext, serverItem.TargetObjectId, repoKey, urlHelper),
            TargetNewPath = serverItem.TargetPath.ToString(),
            Resolution = serverItem.ToWebApiGitResolutionRename1to2(requestContext, repoKey, urlHelper, resolutionAuthor)
          };
          break;
        case GitConflictType.Rename2to1:
          webApiGitConflict = (Microsoft.TeamFoundation.SourceControl.WebApi.GitConflict) new GitConflictRename2to1()
          {
            SourceOriginalPath = serverItem.SourcePath.ToString(),
            SourceOriginalBlob = GitItemUtility.CreateMinimalGitBlobRef(requestContext, serverItem.BaseObjectId, repoKey, urlHelper),
            SourceNewBlob = GitItemUtility.CreateMinimalGitBlobRef(requestContext, serverItem.SourceObjectId, repoKey, urlHelper),
            TargetOriginalPath = serverItem.TargetPath.ToString(),
            TargetOriginalBlob = GitItemUtility.CreateMinimalGitBlobRef(requestContext, serverItem.BaseObjectIdForTarget, repoKey, urlHelper),
            TargetNewBlob = GitItemUtility.CreateMinimalGitBlobRef(requestContext, serverItem.TargetObjectId, repoKey, urlHelper),
            Resolution = serverItem.ToWebApiGitResolutionPathConflict(requestContext, urlHelper, resolutionAuthor)
          };
          break;
        case GitConflictType.RenameAdd:
          webApiGitConflict = (Microsoft.TeamFoundation.SourceControl.WebApi.GitConflict) new GitConflictRenameAdd()
          {
            BaseBlob = GitItemUtility.CreateMinimalGitBlobRef(requestContext, serverItem.BaseObjectId, repoKey, urlHelper),
            SourceBlob = GitItemUtility.CreateMinimalGitBlobRef(requestContext, serverItem.SourceObjectId, repoKey, urlHelper),
            SourceOriginalPath = serverItem.SourcePath.ToString(),
            TargetBlob = GitItemUtility.CreateMinimalGitBlobRef(requestContext, serverItem.TargetObjectId, repoKey, urlHelper),
            Resolution = serverItem.ToWebApiGitResolutionPathConflict(requestContext, urlHelper, resolutionAuthor)
          };
          break;
        case GitConflictType.RenameDelete:
          webApiGitConflict = (Microsoft.TeamFoundation.SourceControl.WebApi.GitConflict) new GitConflictRenameDelete()
          {
            BaseBlob = GitItemUtility.CreateMinimalGitBlobRef(requestContext, serverItem.BaseObjectId, repoKey, urlHelper),
            SourceBlob = GitItemUtility.CreateMinimalGitBlobRef(requestContext, serverItem.SourceObjectId, repoKey, urlHelper),
            SourceNewPath = serverItem.SourcePath.ToString(),
            Resolution = serverItem.ToWebApiGitResolutionPickOneAction(requestContext, urlHelper, resolutionAuthor)
          };
          break;
        case GitConflictType.RenameRename:
          webApiGitConflict = (Microsoft.TeamFoundation.SourceControl.WebApi.GitConflict) new GitConflictRenameRename()
          {
            BaseBlob = GitItemUtility.CreateMinimalGitBlobRef(requestContext, serverItem.BaseObjectId, repoKey, urlHelper),
            OriginalPath = serverItem.SourcePath.ToString(),
            SourceBlob = GitItemUtility.CreateMinimalGitBlobRef(requestContext, serverItem.SourceObjectId, repoKey, urlHelper),
            TargetBlob = GitItemUtility.CreateMinimalGitBlobRef(requestContext, serverItem.TargetObjectId, repoKey, urlHelper),
            Resolution = serverItem.ToWebApiGitResolutionMergeContent(requestContext, repoKey, urlHelper, resolutionAuthor)
          };
          break;
        default:
          throw new InvalidArgumentValueException();
      }
      webApiGitConflict.MergeOrigin = conflictOrigin;
      webApiGitConflict.ConflictId = serverItem.ConflictId;
      webApiGitConflict.ConflictType = serverItem.ConflictType;
      webApiGitConflict.ConflictPath = serverItem.ConflictPath.ToString();
      webApiGitConflict.MergeBaseCommit = GitCommitUtility.CreateMinimalGitCommitRef(requestContext, serverItem.MergeBaseCommitId, repoKey, urlHelper);
      webApiGitConflict.MergeSourceCommit = GitCommitUtility.CreateMinimalGitCommitRef(requestContext, serverItem.MergeSourceCommitId, repoKey, urlHelper);
      webApiGitConflict.MergeTargetCommit = GitCommitUtility.CreateMinimalGitCommitRef(requestContext, serverItem.MergeTargetCommitId, repoKey, urlHelper);
      webApiGitConflict.ResolutionStatus = serverItem.ResolutionStatus;
      webApiGitConflict.ResolvedDate = serverItem.ResolvedDate;
      webApiGitConflict.ResolvedBy = identityRef1;
      webApiGitConflict.Url = GitConflictModelExtensions.GetPullRequestConflictRefUrl(requestContext, repoKey, serverItem.ConflictSourceId, serverItem.ConflictId);
      if (securedObject != null)
        webApiGitConflict.SetSecuredObject(securedObject);
      return webApiGitConflict;
    }

    public static GitResolutionPickOneAction ToWebApiGitResolutionPickOneAction(
      this Microsoft.TeamFoundation.Git.Server.GitConflict serverItem,
      IVssRequestContext requestContext,
      UrlHelper urlHelper,
      IdentityRef resolutionAuthor)
    {
      GitResolutionPickOneAction resolutionPickOneAction = new GitResolutionPickOneAction();
      resolutionPickOneAction.Action = (GitResolutionWhichAction) serverItem.ResolutionAction;
      resolutionPickOneAction.Author = resolutionAuthor;
      return resolutionPickOneAction;
    }

    public static GitResolutionMergeContent ToWebApiGitResolutionMergeContent(
      this Microsoft.TeamFoundation.Git.Server.GitConflict serverItem,
      IVssRequestContext requestContext,
      RepoKey repoKey,
      UrlHelper urlHelper,
      IdentityRef resolutionAuthor)
    {
      GitResolutionMergeContent resolutionMergeContent = new GitResolutionMergeContent();
      resolutionMergeContent.MergeType = (GitResolutionMergeType) serverItem.ResolutionMergeType;
      resolutionMergeContent.UserMergedBlob = GitItemUtility.CreateMinimalGitBlobRef(requestContext, serverItem.ResolutionObjectId, repoKey, urlHelper);
      resolutionMergeContent.Author = resolutionAuthor;
      return resolutionMergeContent;
    }

    public static GitResolutionPathConflict ToWebApiGitResolutionPathConflict(
      this Microsoft.TeamFoundation.Git.Server.GitConflict serverItem,
      IVssRequestContext requestContext,
      UrlHelper urlHelper,
      IdentityRef resolutionAuthor)
    {
      GitResolutionPathConflict resolutionPathConflict = new GitResolutionPathConflict();
      resolutionPathConflict.Action = (GitResolutionPathConflictAction) serverItem.ResolutionAction;
      resolutionPathConflict.RenamePath = serverItem.ResolutionPath?.ToString();
      resolutionPathConflict.Author = resolutionAuthor;
      return resolutionPathConflict;
    }

    public static GitResolutionRename1to2 ToWebApiGitResolutionRename1to2(
      this Microsoft.TeamFoundation.Git.Server.GitConflict serverItem,
      IVssRequestContext requestContext,
      RepoKey repoKey,
      UrlHelper urlHelper,
      IdentityRef resolutionAuthor)
    {
      GitResolutionRename1to2 resolutionRename1to2 = new GitResolutionRename1to2();
      resolutionRename1to2.Action = (GitResolutionRename1to2Action) serverItem.ResolutionAction;
      resolutionRename1to2.MergeType = (GitResolutionMergeType) serverItem.ResolutionMergeType;
      resolutionRename1to2.UserMergedBlob = GitItemUtility.CreateMinimalGitBlobRef(requestContext, serverItem.ResolutionObjectId, repoKey, urlHelper);
      resolutionRename1to2.Author = resolutionAuthor;
      return resolutionRename1to2;
    }

    public static string GetPullRequestConflictRefUrl(
      IVssRequestContext requestContext,
      RepoKey repoKey,
      int pullRequestId,
      int conflictId,
      UrlHelper urlHelper = null)
    {
      if (urlHelper != null)
        return urlHelper.RestLink(requestContext, GitWebApiConstants.PullRequestConflictsLocationId, RouteValuesFactory.PRConflict(repoKey, pullRequestId, conflictId));
      try
      {
        return requestContext.GetService<ILocationService>().GetResourceUri(requestContext, "git", GitWebApiConstants.PullRequestConflictsLocationId, RouteValuesFactory.PRConflict(repoKey, pullRequestId, conflictId)).AbsoluteUri;
      }
      catch
      {
        return (string) null;
      }
    }
  }
}
