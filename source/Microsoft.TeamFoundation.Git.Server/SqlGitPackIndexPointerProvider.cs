// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.SqlGitPackIndexPointerProvider
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Framework.Server;

namespace Microsoft.TeamFoundation.Git.Server
{
  internal sealed class SqlGitPackIndexPointerProvider : IGitPackIndexPointerProvider
  {
    private readonly IVssRequestContext m_rc;
    private readonly OdbId m_odbId;

    public SqlGitPackIndexPointerProvider(IVssRequestContext rc, OdbId odbId)
    {
      this.m_rc = rc;
      this.m_odbId = odbId;
    }

    public Sha1Id? GetIndex() => this.GetIndex(OdbPointerType.Index);

    public Sha1Id? GetLastPackedIndex() => this.GetIndex(OdbPointerType.LastPackedIndex);

    public Sha1Id? TrySetIndex(Sha1Id? oldId, Sha1Id? newId, bool alsoSetLastPacked = false)
    {
      Sha1Id? nullable1;
      Sha1Id? nullable2 = nullable1 = this.UpdateIndexPointer(oldId, newId, OdbPointerType.Index);
      Sha1Id? nullable3 = newId;
      if ((nullable2.HasValue == nullable3.HasValue ? (nullable2.HasValue ? (nullable2.GetValueOrDefault() == nullable3.GetValueOrDefault() ? 1 : 0) : 1) : 0) == 0 || !alsoSetLastPacked)
        return nullable1;
      this.UpdateIndexPointer(this.GetLastPackedIndex(), newId, OdbPointerType.LastPackedIndex);
      return nullable1;
    }

    private Sha1Id? GetIndex(OdbPointerType pointerType)
    {
      using (GitOdbComponent gitOdbComponent = this.m_rc.CreateGitOdbComponent(this.m_odbId))
        return gitOdbComponent.ReadPointer(pointerType);
    }

    private Sha1Id? UpdateIndexPointer(Sha1Id? oldId, Sha1Id? newId, OdbPointerType pointerType)
    {
      using (GitOdbComponent gitOdbComponent = this.m_rc.CreateGitOdbComponent(this.m_odbId))
        return gitOdbComponent.UpdatePointer(pointerType, oldId, newId);
    }
  }
}
