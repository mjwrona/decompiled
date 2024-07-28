// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.GitConflictService
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Microsoft.TeamFoundation.Git.Server
{
  public abstract class GitConflictService
  {
    public static readonly int c_maxEncodedPathLength = 4096;

    public static List<GitConflict> QueryGitConflicts(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      GitConflictSourceType conflictSourceType,
      int conflictSourceId,
      int top = 1000,
      int skip = 0,
      int minConflictId = 0,
      bool includeObsolete = false,
      bool excludeResolved = false,
      bool onlyResolved = false)
    {
      using (GitConflictComponent component = requestContext.CreateComponent<GitConflictComponent>())
        return component.QueryGitConflicts(repository.Key, conflictSourceType, conflictSourceId, top, skip, includeObsolete, excludeResolved, onlyResolved, minConflictId);
    }

    public static GitConflict GetGitConflictById(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      GitConflictSourceType conflictSourceType,
      int conflictSourceId,
      int conflictId)
    {
      using (GitConflictComponent component = requestContext.CreateComponent<GitConflictComponent>())
        return component.GetGitConflictById(repository.Key, conflictSourceType, conflictSourceId, conflictId);
    }

    public GitConflict UpdateGitConflictResolution(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      GitConflictSourceType conflictSourceType,
      int conflictSourceId,
      int conflictId,
      GitResolutionStatus resolutionStatus,
      GitResolution resolution)
    {
      JObject conflictObj = JObject.FromObject((object) new
      {
        resolution = resolution,
        resolutionStatus = resolutionStatus
      });
      return this.UpdateGitConflictResolution(requestContext, repository, conflictSourceType, conflictSourceId, conflictId, conflictObj);
    }

    public GitConflict UpdateGitConflictResolution(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      GitConflictSourceType conflictSourceType,
      int conflictSourceId,
      int conflictId,
      JObject conflictObj)
    {
      conflictObj[nameof (conflictId)] = (JToken) conflictId;
      GitConflictUpdateResult conflictUpdateResult = this.UpdateGitConflictResolutions(requestContext, repository, conflictSourceType, conflictSourceId, new List<JObject>()
      {
        conflictObj
      }).FirstOrDefault<GitConflictUpdateResult>();
      if (conflictUpdateResult?.Error != null)
        throw conflictUpdateResult.Error;
      return conflictUpdateResult?.UpdatedConflict;
    }

    public List<GitConflictUpdateResult> UpdateGitConflictResolutions(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      GitConflictSourceType conflictSourceType,
      int conflictSourceId,
      List<JObject> conflictObjs)
    {
      repository.Permissions.CheckPullRequestContribute();
      repository.Permissions.CheckWrite(true);
      List<GitConflictUpdateResult> conflictUpdateResultList = new List<GitConflictUpdateResult>();
      try
      {
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        int? key = conflictObjs.GroupBy<JObject, int>(GitConflictService.\u003C\u003EO.\u003C0\u003E__ParseConflictId ?? (GitConflictService.\u003C\u003EO.\u003C0\u003E__ParseConflictId = new Func<JObject, int>(GitConflictService.ParseConflictId))).FirstOrDefault<IGrouping<int, JObject>>((Func<IGrouping<int, JObject>, bool>) (x => x.Count<JObject>() > 1))?.Key;
        if (key.HasValue)
          throw new GitArgumentException(string.Format(Resources.Get("GitDuplicateConflictIdSupplied"), (object) key));
      }
      catch (AggregateException ex)
      {
        throw ex.InnerExceptions.First<Exception>();
      }
      bool flag = false;
      Dictionary<int, GitConflict> dictionary = GitConflictService.QueryGitConflicts(requestContext, repository, conflictSourceType, conflictSourceId, int.MaxValue, includeObsolete: true).ToDictionary<GitConflict, int>((Func<GitConflict, int>) (x => x.ConflictId));
      foreach (JObject conflictObj in conflictObjs)
      {
        int conflictId = GitConflictService.ParseConflictId(conflictObj);
        GitConflictUpdateResult conflictUpdateResult = new GitConflictUpdateResult()
        {
          ConflictId = conflictId
        };
        conflictUpdateResultList.Add(conflictUpdateResult);
        GitConflict conflict = (GitConflict) null;
        if (dictionary.TryGetValue(conflictId, out conflict))
        {
          try
          {
            conflictUpdateResult.UpdatedConflict = GitConflictService.UpdateConflict(requestContext, repository, conflictObj, conflict);
          }
          catch (Exception ex) when (
          {
            // ISSUE: unable to correctly present filter
            int num;
            switch (ex)
            {
              case JsonSerializationException _:
              case NotSupportedException _:
                num = 1;
                break;
              default:
                num = ex is InvalidOperationException ? 1 : 0;
                break;
            }
            if ((uint) num > 0U)
            {
              SuccessfulFiltering;
            }
            else
              throw;
          }
          )
          {
            conflictUpdateResult.Error = ex;
            conflictUpdateResult.UpdatedConflict = (GitConflict) null;
          }
        }
      }
      using (GitConflictComponent component = requestContext.CreateComponent<GitConflictComponent>())
      {
        foreach (GitConflictUpdateResult conflictUpdateResult in conflictUpdateResultList)
        {
          if (conflictUpdateResult.UpdatedConflict != null && conflictUpdateResult.Error == null)
          {
            GitConflict gitConflict = component.UpdateGitConflictResolution(repository.Key, conflictUpdateResult.UpdatedConflict);
            conflictUpdateResult.UpdatedConflict = gitConflict;
            dictionary[conflictUpdateResult.UpdatedConflict.ConflictId] = gitConflict;
            if (gitConflict.ResolutionStatus == GitResolutionStatus.Resolved)
              flag = true;
          }
        }
      }
      if (flag)
        this.NotifySourceThatConflictWasResolved(requestContext, repository, conflictSourceId);
      return conflictUpdateResultList;
    }

    protected abstract void NotifySourceThatConflictWasResolved(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      int conflictSourceId);

    private static int ParseConflictId(JObject conflictObj)
    {
      string s = conflictObj["conflictId"]?.ToString();
      int result;
      if (!int.TryParse(s, out result))
        throw new JsonSerializationException(string.Format(Resources.Get("InvalidConflictId"), (object) s));
      return result;
    }

    internal static Sha1Id GetConflictResolutionHash(
      IVssRequestContext requestContext,
      TfsGitPullRequest pullRequest)
    {
      using (ITfsGitRepository repositoryById = requestContext.GetService<ITeamFoundationGitRepositoryService>().FindRepositoryById(requestContext, pullRequest.RepositoryId))
        return GitConflictService.GetConflictResolutionHash(requestContext, repositoryById, GitConflictSourceType.PullRequest, pullRequest.PullRequestId);
    }

    internal static Sha1Id GetConflictResolutionHash(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      GitConflictSourceType conflictSourceType,
      int conflictSourceId)
    {
      List<GitConflict> conflicts = GitConflictService.QueryGitConflicts(requestContext, repository, conflictSourceType, conflictSourceId, int.MaxValue);
      return GitConflictService.GetConflictResolutionHash(requestContext, conflicts);
    }

    internal static Sha1Id GetConflictResolutionHash(
      IVssRequestContext requestContext,
      List<GitConflict> conflicts)
    {
      conflicts.Take<GitConflict>(15).Select(c =>
      {
        int conflictId = c.ConflictId;
        int conflictType = (int) c.ConflictType;
        int resolutionAction = (int) c.ResolutionAction;
        int resolutionMergeType = (int) c.ResolutionMergeType;
        int resolutionError = (int) c.ResolutionError;
        Sha1Id sha1Id = c.ResolutionObjectId;
        string abbreviatedString1 = sha1Id.ToAbbreviatedString();
        sha1Id = c.MergeBaseCommitId;
        string abbreviatedString2 = sha1Id.ToAbbreviatedString();
        return new
        {
          id = conflictId,
          type = (GitConflictType) conflictType,
          action = (byte) resolutionAction,
          mType = (byte) resolutionMergeType,
          err = (byte) resolutionError,
          obj = abbreviatedString1,
          mBase = abbreviatedString2
        };
      }).ToArray();
      if (conflicts.Count == 0 || conflicts.Any<GitConflict>((Func<GitConflict, bool>) (c => c.ResolutionStatus != GitResolutionStatus.Resolved)))
        return Sha1Id.Empty;
      string s = string.Join(",", conflicts.OrderBy<GitConflict, int>((Func<GitConflict, int>) (c => c.ConflictId)).Select<GitConflict, string>((Func<GitConflict, string>) (c => string.Format("{0}:{1}", (object) c.ConflictId, (object) c.ResolvedDate.Ticks))));
      byte[] bytes = GitEncodingUtil.SafeUtf8NoBom.GetBytes(s);
      return new Sha1Id(SHA1.Create().ComputeHash(bytes));
    }

    private static GitConflict UpdateConflict(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      JObject conflictPatch,
      GitConflict conflict)
    {
      Guid result = Guid.Empty;
      GitResolutionError? nullable1 = new GitResolutionError?();
      GitResolutionStatus resolutionStatus1;
      Guid guid;
      DateTime dateTime;
      try
      {
        JToken jtoken = conflictPatch["resolutionStatus"];
        resolutionStatus1 = jtoken != null ? jtoken.ToObject<GitResolutionStatus>() : conflict.ResolutionStatus;
        if (resolutionStatus1 == GitResolutionStatus.Unresolved)
        {
          guid = Guid.Empty;
          dateTime = DateTime.MinValue;
          result = Guid.Empty;
        }
        else
        {
          nullable1 = new GitResolutionError?(GitResolutionError.None);
          guid = requestContext.GetUserId();
          dateTime = DateTime.UtcNow;
          string input = conflictPatch["resolution"]?[(object) "author"]?[(object) "id"]?.ToString();
          if (input != null && !Guid.TryParse(input, out result))
            throw new JsonSerializationException();
          if (result == Guid.Empty)
            result = guid;
        }
      }
      catch (Exception ex) when (!(ex is JsonSerializationException))
      {
        throw new JsonSerializationException();
      }
      switch (conflict.ConflictType)
      {
        case GitConflictType.AddAdd:
        case GitConflictType.EditEdit:
        case GitConflictType.RenameRename:
          GitResolutionMergeContent resolution1;
          try
          {
            resolution1 = conflictPatch["resolution"]?.ToObject<GitResolutionMergeContent>() ?? new GitResolutionMergeContent();
          }
          catch (Exception ex) when (!(ex is JsonSerializationException))
          {
            throw new JsonSerializationException();
          }
          conflict = GitConflictService.UpdateResolutionMergeContent(requestContext, repository, conflict, resolutionStatus1, resolution1);
          break;
        case GitConflictType.AddRename:
        case GitConflictType.DirectoryFile:
        case GitConflictType.FileDirectory:
        case GitConflictType.Rename2to1:
        case GitConflictType.RenameAdd:
          GitResolutionPathConflict resolution2;
          try
          {
            resolution2 = conflictPatch["resolution"]?.ToObject<GitResolutionPathConflict>() ?? new GitResolutionPathConflict();
          }
          catch (Exception ex) when (!(ex is JsonSerializationException))
          {
            throw new JsonSerializationException();
          }
          conflict = GitConflictService.UpdateResolutionPathConflict(conflict, resolutionStatus1, resolution2);
          break;
        case GitConflictType.DeleteEdit:
        case GitConflictType.DeleteRename:
        case GitConflictType.EditDelete:
        case GitConflictType.RenameDelete:
          GitResolutionPickOneAction resolution3;
          try
          {
            resolution3 = conflictPatch["resolution"]?.ToObject<GitResolutionPickOneAction>() ?? new GitResolutionPickOneAction();
          }
          catch (Exception ex) when (!(ex is JsonSerializationException))
          {
            throw new JsonSerializationException();
          }
          conflict = GitConflictService.UpdateResolutionPickOneAction(conflict, resolutionStatus1, resolution3);
          break;
        case GitConflictType.Rename1to2:
          GitResolutionRename1to2 resolution4;
          try
          {
            resolution4 = conflictPatch["resolution"]?.ToObject<GitResolutionRename1to2>() ?? new GitResolutionRename1to2();
          }
          catch (Exception ex) when (!(ex is JsonSerializationException))
          {
            throw new JsonSerializationException();
          }
          conflict = GitConflictService.UpdateResolutionRename1to2(conflict, resolutionStatus1, resolution4);
          break;
        default:
          throw new NotSupportedException(Resources.Get("UnsupportedConflictType"));
      }
      GitConflict gitConflict = conflict;
      Guid? nullable2 = new Guid?(guid);
      DateTime? nullable3 = new DateTime?(dateTime);
      GitResolutionError? nullable4 = nullable1;
      Guid? nullable5 = new Guid?(result);
      GitConflictSourceType? conflictSourceType = new GitConflictSourceType?();
      int? conflictSourceId = new int?();
      int? conflictId = new int?();
      Sha1Id? mergeBaseCommitId = new Sha1Id?();
      Sha1Id? mergeSourceCommitId = new Sha1Id?();
      Sha1Id? mergeTargetCommitId = new Sha1Id?();
      GitConflictType? conflictType = new GitConflictType?();
      Sha1Id? baseObjectId = new Sha1Id?();
      Sha1Id? baseObjectIdForTarget = new Sha1Id?();
      Sha1Id? sourceObjectId = new Sha1Id?();
      Sha1Id? targetObjectId = new Sha1Id?();
      GitResolutionStatus? resolutionStatus2 = new GitResolutionStatus?();
      GitResolutionError? resolutionError = nullable4;
      byte? resolutionAction = new byte?();
      byte? resolutionMergeType = new byte?();
      Sha1Id? resolutionObjectId = new Sha1Id?();
      Guid? resolvedBy = nullable2;
      DateTime? resolvedDate = nullable3;
      Guid? resolutionAuthor = nullable5;
      conflict = gitConflict.CopyAndUpdate(conflictSourceType, conflictSourceId, conflictId, mergeBaseCommitId, mergeSourceCommitId, mergeTargetCommitId, conflictType, baseObjectId, baseObjectIdForTarget, sourceObjectId, targetObjectId, resolutionStatus2, resolutionError, resolutionAction, resolutionMergeType, resolutionObjectId, resolvedBy, resolvedDate, resolutionAuthor);
      return conflict;
    }

    private static GitConflict UpdateResolutionPickOneAction(
      GitConflict conflict,
      GitResolutionStatus resolutionStatus,
      GitResolutionPickOneAction resolution)
    {
      if (resolutionStatus == GitResolutionStatus.PartiallyResolved)
        throw new InvalidOperationException(Resources.Get("PartiallyResolvedNotSuppored"));
      if (resolutionStatus == GitResolutionStatus.Resolved && resolution.Action == GitResolutionWhichAction.Undecided)
        throw new InvalidOperationException(Resources.Get("ConflictNotResolved"));
      GitConflict gitConflict = conflict;
      GitResolutionStatus? nullable1 = new GitResolutionStatus?(resolutionStatus);
      byte? nullable2 = new byte?((byte) resolution.Action);
      GitConflictSourceType? conflictSourceType = new GitConflictSourceType?();
      int? conflictSourceId = new int?();
      int? conflictId = new int?();
      Sha1Id? mergeBaseCommitId = new Sha1Id?();
      Sha1Id? mergeSourceCommitId = new Sha1Id?();
      Sha1Id? mergeTargetCommitId = new Sha1Id?();
      GitConflictType? conflictType = new GitConflictType?();
      Sha1Id? baseObjectId = new Sha1Id?();
      Sha1Id? baseObjectIdForTarget = new Sha1Id?();
      Sha1Id? sourceObjectId = new Sha1Id?();
      Sha1Id? targetObjectId = new Sha1Id?();
      GitResolutionStatus? resolutionStatus1 = nullable1;
      GitResolutionError? resolutionError = new GitResolutionError?();
      byte? resolutionAction = nullable2;
      byte? resolutionMergeType = new byte?();
      Sha1Id? resolutionObjectId = new Sha1Id?();
      Guid? resolvedBy = new Guid?();
      DateTime? resolvedDate = new DateTime?();
      Guid? resolutionAuthor = new Guid?();
      return gitConflict.CopyAndUpdate(conflictSourceType, conflictSourceId, conflictId, mergeBaseCommitId, mergeSourceCommitId, mergeTargetCommitId, conflictType, baseObjectId, baseObjectIdForTarget, sourceObjectId, targetObjectId, resolutionStatus1, resolutionError, resolutionAction, resolutionMergeType, resolutionObjectId, resolvedBy, resolvedDate, resolutionAuthor);
    }

    private static GitConflict UpdateResolutionPathConflict(
      GitConflict conflict,
      GitResolutionStatus resolutionStatus,
      GitResolutionPathConflict resolution)
    {
      if (resolutionStatus == GitResolutionStatus.PartiallyResolved)
        throw new InvalidOperationException(Resources.Get("PartiallyResolvedNotSuppored"));
      NormalizedGitPath resolutionPath = resolution.RenamePath != null ? new NormalizedGitPath(resolution.RenamePath) : (NormalizedGitPath) null;
      if (resolutionPath != (NormalizedGitPath) null && Encoding.UTF8.GetByteCount(resolutionPath.ToString()) > GitConflictService.c_maxEncodedPathLength)
        throw new InvalidOperationException(Resources.Format("PathTooLong", (object) "RenamePath", (object) GitConflictService.c_maxEncodedPathLength));
      if (resolutionStatus == GitResolutionStatus.Resolved)
      {
        switch (resolution.Action)
        {
          case GitResolutionPathConflictAction.Undecided:
            throw new InvalidOperationException(Resources.Get("ConflictNotResolved"));
          case GitResolutionPathConflictAction.KeepSourceRenameTarget:
          case GitResolutionPathConflictAction.KeepTargetRenameSource:
            if (resolutionPath == (NormalizedGitPath) null)
              throw new InvalidOperationException(Resources.Get("ConflictNotResolved"));
            break;
        }
      }
      if (resolution.Action != GitResolutionPathConflictAction.KeepSourceRenameTarget && resolution.Action != GitResolutionPathConflictAction.KeepTargetRenameSource && resolutionPath != (NormalizedGitPath) null)
        throw new InvalidOperationException(Resources.Get("RenamePathNotValid"));
      if (resolutionPath != (NormalizedGitPath) null)
      {
        string str = resolutionPath.ToString();
        if (str.IndexOf(char.MinValue) != -1 || str.Contains("//") || str.Contains("/./") || str.Contains("/../"))
          throw new InvalidOperationException(Resources.Get("ConflictResolutionPathNotValid"));
      }
      GitConflict gitConflict = conflict;
      GitResolutionStatus? nullable1 = new GitResolutionStatus?(resolutionStatus);
      byte? nullable2 = new byte?((byte) resolution.Action);
      GitConflictSourceType? conflictSourceType = new GitConflictSourceType?();
      int? conflictSourceId = new int?();
      int? conflictId = new int?();
      Sha1Id? mergeBaseCommitId = new Sha1Id?();
      Sha1Id? mergeSourceCommitId = new Sha1Id?();
      Sha1Id? mergeTargetCommitId = new Sha1Id?();
      GitConflictType? conflictType = new GitConflictType?();
      Sha1Id? baseObjectId = new Sha1Id?();
      Sha1Id? baseObjectIdForTarget = new Sha1Id?();
      Sha1Id? sourceObjectId = new Sha1Id?();
      Sha1Id? targetObjectId = new Sha1Id?();
      GitResolutionStatus? resolutionStatus1 = nullable1;
      GitResolutionError? resolutionError = new GitResolutionError?();
      byte? resolutionAction = nullable2;
      byte? resolutionMergeType = new byte?();
      Sha1Id? resolutionObjectId = new Sha1Id?();
      Guid? resolvedBy = new Guid?();
      DateTime? resolvedDate = new DateTime?();
      Guid? resolutionAuthor = new Guid?();
      return gitConflict.CopyAndUpdate(conflictSourceType, conflictSourceId, conflictId, mergeBaseCommitId, mergeSourceCommitId, mergeTargetCommitId, conflictType, baseObjectId, baseObjectIdForTarget, sourceObjectId, targetObjectId, resolutionStatus1, resolutionError, resolutionAction, resolutionMergeType, resolutionObjectId, resolvedBy, resolvedDate, resolutionAuthor).CopyAndUpdateResolutionPath(resolutionPath);
    }

    private static GitConflict UpdateResolutionMergeContent(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      GitConflict conflict,
      GitResolutionStatus resolutionStatus,
      GitResolutionMergeContent resolution)
    {
      if (resolutionStatus == GitResolutionStatus.PartiallyResolved && resolution.MergeType != GitResolutionMergeType.UserMerged)
        throw new InvalidOperationException(Resources.Get("PartiallyResolvedNotSuppored"));
      int num = resolution.UserMergedBlob != null ? 1 : (resolution.UserMergedContent != null ? 1 : 0);
      if (num != (resolution.MergeType == GitResolutionMergeType.UserMerged ? 1 : 0))
        throw new InvalidOperationException(Resources.Get("MergedContentNotValid"));
      if (resolutionStatus == GitResolutionStatus.Resolved && resolution.MergeType == GitResolutionMergeType.Undecided)
        throw new InvalidOperationException(Resources.Get("ConflictNotResolved"));
      Sha1Id sha1Id = Sha1Id.Empty;
      if (num != 0)
      {
        if (resolution.UserMergedBlob != null)
        {
          if (resolution.UserMergedContent != null)
            throw new InvalidOperationException(Resources.Get("MergedContentNotValid"));
          sha1Id = Sha1Id.Parse(resolution.UserMergedBlob.ObjectId);
        }
        else
          sha1Id = ((TeamFoundationGitPullRequestService) requestContext.GetService<ITeamFoundationGitPullRequestService>()).StoreMergedBlobForConflict(requestContext, repository, conflict.ConflictSourceId, conflict.ConflictId, resolution.UserMergedContent);
      }
      GitConflict gitConflict = conflict;
      GitResolutionStatus? nullable1 = new GitResolutionStatus?(resolutionStatus);
      byte? nullable2 = new byte?((byte) resolution.MergeType);
      Sha1Id? nullable3 = new Sha1Id?(sha1Id);
      GitConflictSourceType? conflictSourceType = new GitConflictSourceType?();
      int? conflictSourceId = new int?();
      int? conflictId = new int?();
      Sha1Id? mergeBaseCommitId = new Sha1Id?();
      Sha1Id? mergeSourceCommitId = new Sha1Id?();
      Sha1Id? mergeTargetCommitId = new Sha1Id?();
      GitConflictType? conflictType = new GitConflictType?();
      Sha1Id? baseObjectId = new Sha1Id?();
      Sha1Id? baseObjectIdForTarget = new Sha1Id?();
      Sha1Id? sourceObjectId = new Sha1Id?();
      Sha1Id? targetObjectId = new Sha1Id?();
      GitResolutionStatus? resolutionStatus1 = nullable1;
      GitResolutionError? resolutionError = new GitResolutionError?();
      byte? resolutionAction = new byte?();
      byte? resolutionMergeType = nullable2;
      Sha1Id? resolutionObjectId = nullable3;
      Guid? resolvedBy = new Guid?();
      DateTime? resolvedDate = new DateTime?();
      Guid? resolutionAuthor = new Guid?();
      return gitConflict.CopyAndUpdate(conflictSourceType, conflictSourceId, conflictId, mergeBaseCommitId, mergeSourceCommitId, mergeTargetCommitId, conflictType, baseObjectId, baseObjectIdForTarget, sourceObjectId, targetObjectId, resolutionStatus1, resolutionError, resolutionAction, resolutionMergeType, resolutionObjectId, resolvedBy, resolvedDate, resolutionAuthor);
    }

    private static GitConflict UpdateResolutionRename1to2(
      GitConflict conflict,
      GitResolutionStatus resolutionStatus,
      GitResolutionRename1to2 resolution)
    {
      if (resolutionStatus == GitResolutionStatus.PartiallyResolved)
        throw new InvalidOperationException(Resources.Get("PartiallyResolvedNotSuppored"));
      if (resolutionStatus == GitResolutionStatus.Resolved)
      {
        switch (resolution.Action)
        {
          case GitResolutionRename1to2Action.Undecided:
            throw new InvalidOperationException(Resources.Get("ConflictNotResolved"));
        }
      }
      GitConflict gitConflict = conflict;
      GitResolutionStatus? nullable1 = new GitResolutionStatus?(resolutionStatus);
      byte? nullable2 = new byte?((byte) resolution.Action);
      GitConflictSourceType? conflictSourceType = new GitConflictSourceType?();
      int? conflictSourceId = new int?();
      int? conflictId = new int?();
      Sha1Id? mergeBaseCommitId = new Sha1Id?();
      Sha1Id? mergeSourceCommitId = new Sha1Id?();
      Sha1Id? mergeTargetCommitId = new Sha1Id?();
      GitConflictType? conflictType = new GitConflictType?();
      Sha1Id? baseObjectId = new Sha1Id?();
      Sha1Id? baseObjectIdForTarget = new Sha1Id?();
      Sha1Id? sourceObjectId = new Sha1Id?();
      Sha1Id? targetObjectId = new Sha1Id?();
      GitResolutionStatus? resolutionStatus1 = nullable1;
      GitResolutionError? resolutionError = new GitResolutionError?();
      byte? resolutionAction = nullable2;
      byte? resolutionMergeType = new byte?();
      Sha1Id? resolutionObjectId = new Sha1Id?();
      Guid? resolvedBy = new Guid?();
      DateTime? resolvedDate = new DateTime?();
      Guid? resolutionAuthor = new Guid?();
      return gitConflict.CopyAndUpdate(conflictSourceType, conflictSourceId, conflictId, mergeBaseCommitId, mergeSourceCommitId, mergeTargetCommitId, conflictType, baseObjectId, baseObjectIdForTarget, sourceObjectId, targetObjectId, resolutionStatus1, resolutionError, resolutionAction, resolutionMergeType, resolutionObjectId, resolvedBy, resolvedDate, resolutionAuthor);
    }
  }
}
