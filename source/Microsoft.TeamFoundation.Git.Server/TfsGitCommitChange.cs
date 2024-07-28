// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.TfsGitCommitChange
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.Common;

namespace Microsoft.TeamFoundation.Git.Server
{
  public class TfsGitCommitChange
  {
    internal TfsGitCommitChange()
    {
    }

    public TfsGitCommitChange(TfsGitDiffEntry entry)
    {
      ArgumentUtility.CheckForNull<TfsGitDiffEntry>(entry, nameof (entry));
      this.ChangeType = entry.ChangeType;
      this.ContentChanged = entry.ContentChanged;
      int num = entry.RelativePath.LastIndexOf('/');
      this.ParentPath = entry.RelativePath.Substring(0, num + 1);
      this.ChildItem = entry.RelativePath.Substring(num + 1);
      this.RenameSourceItemPath = entry.RenameSourceItemPath;
      this.ObjectType = entry.NewObjectId.HasValue ? entry.NewObjectType : entry.OldObjectType;
      this.ChangedObjectId = entry.NewObjectId;
      this.OriginalObjectId = entry.OldObjectId;
    }

    protected TfsGitCommitChange(TfsGitCommitChange change)
    {
      this.ContentChanged = change.ContentChanged;
      this.ObjectType = change.ObjectType;
      this.ParentPath = change.ParentPath;
      this.ChildItem = change.ChildItem;
      this.RenameSourceItemPath = change.RenameSourceItemPath;
      this.ChangeType = change.ChangeType;
      this.ChangedObjectId = change.ChangedObjectId;
      this.OriginalObjectId = change.OriginalObjectId;
    }

    internal virtual TfsGitCommitChangeWithId WithId(Sha1Id commitId) => new TfsGitCommitChangeWithId(commitId, this);

    public bool ContentChanged { get; set; }

    public GitObjectType ObjectType { get; set; }

    public string ParentPath { get; set; }

    public string ChildItem { get; set; }

    public string RenameSourceItemPath { get; set; }

    public TfsGitChangeType ChangeType { get; set; }

    public Sha1Id? ChangedObjectId { get; set; }

    public Sha1Id? OriginalObjectId { get; set; }
  }
}
