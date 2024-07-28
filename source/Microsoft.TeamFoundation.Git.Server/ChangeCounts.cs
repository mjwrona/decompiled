// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.ChangeCounts
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.SourceControl.WebApi;

namespace Microsoft.TeamFoundation.Git.Server
{
  public sealed class ChangeCounts
  {
    internal ChangeCounts(int adds, int edits, int deletes)
    {
      this.Adds = adds;
      this.Edits = edits;
      this.Deletes = deletes;
    }

    public int Adds { get; }

    public int Edits { get; }

    public int Deletes { get; }

    internal sealed class Builder
    {
      private int m_adds;
      private int m_edits;
      private int m_deletes;

      public void Add(TfsGitDiffEntry entry) => this.Add(entry.NewObjectId.HasValue ? entry.NewObjectType : entry.OldObjectType, entry.ChangeType);

      public void Add(TfsGitCommitChange change) => this.Add(change.ObjectType, change.ChangeType);

      private void Add(GitObjectType objectType, TfsGitChangeType changeType)
      {
        if (objectType == GitObjectType.Tree)
          return;
        if ((changeType & TfsGitChangeType.Add) == TfsGitChangeType.Add)
          ++this.m_adds;
        else if ((changeType & TfsGitChangeType.Edit) == TfsGitChangeType.Edit)
        {
          ++this.m_edits;
        }
        else
        {
          if ((changeType & TfsGitChangeType.Delete) != TfsGitChangeType.Delete || (changeType & TfsGitChangeType.SourceRename) == TfsGitChangeType.SourceRename)
            return;
          ++this.m_deletes;
        }
      }

      public ChangeCounts Create() => new ChangeCounts(this.m_adds, this.m_edits, this.m_deletes);
    }
  }
}
