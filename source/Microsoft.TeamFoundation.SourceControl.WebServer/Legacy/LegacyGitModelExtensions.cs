// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebServer.Legacy.LegacyGitModelExtensions
// Assembly: Microsoft.TeamFoundation.SourceControl.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 47C05AF5-4412-4EF0-BF63-D475D8EECD03
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.SourceControl.WebServer.dll

using Microsoft.TeamFoundation.Common.Internal;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server;
using Microsoft.TeamFoundation.SourceControl.WebApi.Legacy;
using Microsoft.TeamFoundation.VersionControl.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.SourceControl.WebServer.Legacy
{
  public static class LegacyGitModelExtensions
  {
    private static Dictionary<TfsGitChangeType, ChangeType> s_changeTypeMap = new Dictionary<TfsGitChangeType, ChangeType>()
    {
      {
        TfsGitChangeType.Add,
        ChangeType.Add
      },
      {
        TfsGitChangeType.Delete,
        ChangeType.Delete
      },
      {
        TfsGitChangeType.Edit,
        ChangeType.Edit
      },
      {
        TfsGitChangeType.Rename,
        ChangeType.Rename
      },
      {
        TfsGitChangeType.SourceRename,
        ChangeType.SourceRename
      }
    };

    public static Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.GitCommit ToLegacyGitCommit(
      this TfsGitCommitMetadata metadata,
      ChangeCounts changeCounts)
    {
      Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.GitCommit gitCommit = new Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.GitCommit();
      gitCommit.OwnerDisplayName = metadata.Author;
      gitCommit.Author = LegacyGitModelExtensions.CreateIdentityReference(metadata.Author, metadata.AuthorTime);
      gitCommit.CreationDate = metadata.AuthorTime;
      gitCommit.Committer = LegacyGitModelExtensions.CreateIdentityReference(metadata.Committer, metadata.CommitTime);
      gitCommit.SortDate = metadata.CommitTime;
      gitCommit.CommitTime = metadata.CommitTime;
      Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.GitCommit legacyGitCommit = gitCommit;
      if (metadata.ShortComment != null && metadata.ShortComment.Length >= 100)
      {
        legacyGitCommit.Comment = StringUtil.Truncate(metadata.ShortComment, 100, false);
        legacyGitCommit.CommentTruncated = true;
      }
      else
      {
        legacyGitCommit.Comment = metadata.ShortComment;
        legacyGitCommit.CommentTruncated = metadata.ShortCommentIsTruncated;
      }
      legacyGitCommit.ChangeCounts = new Dictionary<Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.VersionControlChangeType, int>();
      if (changeCounts != null)
      {
        legacyGitCommit.ChangeCounts.Add(Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.VersionControlChangeType.Add, changeCounts.Adds);
        legacyGitCommit.ChangeCounts.Add(Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.VersionControlChangeType.Edit, changeCounts.Edits);
        legacyGitCommit.ChangeCounts.Add(Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.VersionControlChangeType.Delete, changeCounts.Deletes);
      }
      legacyGitCommit.CommitId = new GitObjectId(metadata.CommitId.ToByteArray());
      legacyGitCommit.Version = "GC" + legacyGitCommit.CommitId.Full;
      return legacyGitCommit;
    }

    public static string GetCommitUrl(string baseUrl, TfsGitCommit commit) => (string) null;

    public static string GetIdentityUrl(string author) => (string) null;

    public static string GetTreeUrl(string baseUrl, TfsGitTree tree) => (string) null;

    public static Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.GitCommit ToLegacyGitCommit(
      this TfsGitCommit commit,
      IVssRequestContext requestContext)
    {
      Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.GitCommit legacyGitCommit = new Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.GitCommit();
      legacyGitCommit.Comment = commit.GetComment();
      string nameAndEmail1 = commit.GetAuthor().NameAndEmail;
      DateTime time = commit.GetAuthor().Time;
      legacyGitCommit.OwnerDisplayName = nameAndEmail1;
      legacyGitCommit.CreationDate = time;
      legacyGitCommit.Author = LegacyGitModelExtensions.CreateIdentityReference(nameAndEmail1, time);
      string nameAndEmail2 = commit.GetCommitter().NameAndEmail;
      legacyGitCommit.CommitTime = commit.GetCommitter().Time;
      legacyGitCommit.SortDate = legacyGitCommit.CommitTime;
      legacyGitCommit.Committer = LegacyGitModelExtensions.CreateIdentityReference(nameAndEmail2, legacyGitCommit.CommitTime);
      legacyGitCommit.CommitId = new GitObjectId(commit.ObjectId.ToByteArray());
      legacyGitCommit.Version = "GC" + legacyGitCommit.CommitId.Full;
      string baseUrl = GitServerUtils.GetPublicBaseUrl(requestContext);
      legacyGitCommit.Parents = (IEnumerable<GitObjectReference>) commit.GetParents().Select<TfsGitCommit, GitObjectReference>((Func<TfsGitCommit, GitObjectReference>) (pc =>
      {
        return new GitObjectReference()
        {
          ObjectId = new GitObjectId(pc.ObjectId.ToByteArray()),
          Url = LegacyGitModelExtensions.GetCommitUrl(baseUrl, pc)
        };
      })).ToList<GitObjectReference>();
      legacyGitCommit.Tree = commit.GetTree().ToLegacyGitItem();
      return legacyGitCommit;
    }

    public static GitBranchDiff ToLegacyBranchDiff(
      this TfsGitCommitLineageDiff lineageDiff,
      IdentityRef isLockedBy)
    {
      return new GitBranchDiff(lineageDiff.Metadata.ToLegacyGitCommit((ChangeCounts) null), lineageDiff.RefName, isLockedBy, lineageDiff.AheadCount, lineageDiff.BehindCount);
    }

    public static Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.GitItem ToLegacyGitItem(
      this TfsGitCommitHistoryEntry historyEntry)
    {
      Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.GitItem legacyGitItem = historyEntry.Change.ToLegacyGitItem();
      legacyGitItem.ChangeDate = historyEntry.Commit.AuthorTime;
      return legacyGitItem;
    }

    public static Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.GitItem ToLegacyGitItem(
      this TfsGitCommitChangeWithId change)
    {
      Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.GitItem legacyGitItem = new Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.GitItem(change.CommitId.ToByteArray());
      legacyGitItem.ServerItem = "/" + (change.ParentPath + change.ChildItem).Trim('/');
      legacyGitItem.IsFolder = change.ObjectType == Microsoft.TeamFoundation.SourceControl.WebApi.GitObjectType.Tree;
      legacyGitItem.GitObjectType = LegacyGitModelExtensions.ConvertTfsGitObjectType(change.ObjectType);
      return legacyGitItem;
    }

    public static Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.GitItem ToLegacyGitItem(
      this TfsGitTree tree)
    {
      Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.GitItem legacyGitItem = new Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.GitItem(tree.ObjectId.ToByteArray());
      legacyGitItem.IsFolder = true;
      legacyGitItem.GitObjectType = Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.GitObjectType.Tree;
      return legacyGitItem;
    }

    public static GitItemMetadata ToLegacyGitItemMetadata(this TfsGitCommitHistoryEntry historyEntry) => new GitItemMetadata()
    {
      Item = historyEntry.ToLegacyGitItem(),
      Comment = historyEntry.Commit.ShortComment,
      OwnerDisplayName = historyEntry.Commit.Author,
      Owner = historyEntry.Commit.Author,
      CommitId = new GitObjectId(historyEntry.Commit.CommitId.ToByteArray())
    };

    public static GitIdentityReference CreateIdentityReference(string id, DateTime date)
    {
      GitIdentityReference identityReference = new GitIdentityReference();
      identityReference.Id = id;
      identityReference.Date = new DateTime?(date);
      return identityReference;
    }

    public static Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.VersionControlChangeType ConvertChangeType(
      TfsGitChangeType tfsChangeType)
    {
      Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.VersionControlChangeType controlChangeType = Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.VersionControlChangeType.None;
      foreach (KeyValuePair<TfsGitChangeType, ChangeType> changeType in LegacyGitModelExtensions.s_changeTypeMap)
      {
        if (tfsChangeType.HasFlag((Enum) changeType.Key))
          controlChangeType |= (Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.VersionControlChangeType) changeType.Value;
      }
      return controlChangeType;
    }

    public static Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.GitObjectType ConvertTfsGitObjectType(
      Microsoft.TeamFoundation.SourceControl.WebApi.GitObjectType tfsType)
    {
      switch (tfsType)
      {
        case Microsoft.TeamFoundation.SourceControl.WebApi.GitObjectType.Bad:
          return Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.GitObjectType.Bad;
        case Microsoft.TeamFoundation.SourceControl.WebApi.GitObjectType.Commit:
          return Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.GitObjectType.Commit;
        case Microsoft.TeamFoundation.SourceControl.WebApi.GitObjectType.Tree:
          return Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.GitObjectType.Tree;
        case Microsoft.TeamFoundation.SourceControl.WebApi.GitObjectType.Blob:
          return Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.GitObjectType.Blob;
        case Microsoft.TeamFoundation.SourceControl.WebApi.GitObjectType.Tag:
          return Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.GitObjectType.Tag;
        case Microsoft.TeamFoundation.SourceControl.WebApi.GitObjectType.Ext2:
          return Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.GitObjectType.Ext2;
        case Microsoft.TeamFoundation.SourceControl.WebApi.GitObjectType.OfsDelta:
          return Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.GitObjectType.OfsDelta;
        case Microsoft.TeamFoundation.SourceControl.WebApi.GitObjectType.RefDelta:
          return Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.GitObjectType.RefDelta;
        default:
          return Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.GitObjectType.Bad;
      }
    }
  }
}
