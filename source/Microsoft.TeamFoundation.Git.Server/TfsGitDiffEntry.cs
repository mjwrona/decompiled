// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.TfsGitDiffEntry
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.SourceControl.WebApi;

namespace Microsoft.TeamFoundation.Git.Server
{
  public class TfsGitDiffEntry
  {
    private static readonly ByteArrayEqualityComparer s_byteArrayComparer = new ByteArrayEqualityComparer();

    internal TfsGitDiffEntry(TreeEntryAndPath oldEntry, TreeEntryAndPath newEntry)
    {
      if (oldEntry == null)
      {
        this.RelativePath = newEntry.RelativePath;
        this.NewObjectId = new Sha1Id?(newEntry.Entry.ObjectId);
        this.NewObjectType = newEntry.Entry.ObjectType;
        this.ChangeType = TfsGitChangeType.Add;
      }
      else if (newEntry == null)
      {
        this.RelativePath = oldEntry.RelativePath;
        this.OldObjectId = new Sha1Id?(oldEntry.Entry.ObjectId);
        this.OldObjectType = oldEntry.Entry.ObjectType;
        this.ChangeType = TfsGitChangeType.Delete;
      }
      else
      {
        this.RelativePath = oldEntry.RelativePath;
        this.OldObjectId = new Sha1Id?(oldEntry.Entry.ObjectId);
        this.OldObjectType = oldEntry.Entry.ObjectType;
        this.NewObjectId = new Sha1Id?(newEntry.Entry.ObjectId);
        this.NewObjectType = newEntry.Entry.ObjectType;
        this.ChangeType = TfsGitChangeType.Edit;
      }
      this.ContentChanged = true;
    }

    private TfsGitDiffEntry()
    {
    }

    internal static TfsGitDiffEntry CreateRenameEntry(
      TfsGitDiffEntry deleteEntry,
      TfsGitDiffEntry addEntry)
    {
      TfsGitDiffEntry renameEntry = new TfsGitDiffEntry();
      renameEntry.RelativePath = addEntry.RelativePath;
      renameEntry.OldObjectId = deleteEntry.OldObjectId;
      renameEntry.OldObjectType = deleteEntry.OldObjectType;
      renameEntry.NewObjectId = addEntry.NewObjectId;
      renameEntry.NewObjectType = addEntry.NewObjectType;
      renameEntry.RenameSourceItemPath = deleteEntry.RelativePath;
      Sha1Id? newObjectId = renameEntry.NewObjectId;
      Sha1Id? oldObjectId = renameEntry.OldObjectId;
      if ((newObjectId.HasValue == oldObjectId.HasValue ? (newObjectId.HasValue ? (newObjectId.GetValueOrDefault() == oldObjectId.GetValueOrDefault() ? 1 : 0) : 1) : 0) != 0)
      {
        renameEntry.ChangeType = TfsGitChangeType.Rename;
      }
      else
      {
        renameEntry.ContentChanged = true;
        renameEntry.ChangeType = TfsGitChangeType.Edit | TfsGitChangeType.Rename;
      }
      return renameEntry;
    }

    internal static TfsGitDiffEntry CreateRenameSourceEntry(TfsGitDiffEntry deleteEntry) => new TfsGitDiffEntry()
    {
      RelativePath = deleteEntry.RelativePath,
      OldObjectId = deleteEntry.OldObjectId,
      OldObjectType = deleteEntry.OldObjectType,
      ChangeType = TfsGitChangeType.Delete | TfsGitChangeType.SourceRename,
      ContentChanged = true
    };

    public string RelativePath { get; internal set; }

    public Sha1Id? OldObjectId { get; private set; }

    public Sha1Id? NewObjectId { get; private set; }

    public GitObjectType OldObjectType { get; private set; }

    public GitObjectType NewObjectType { get; private set; }

    public TfsGitChangeType ChangeType { get; private set; }

    public bool ContentChanged { get; internal set; }

    public string RenameSourceItemPath { get; internal set; }
  }
}
