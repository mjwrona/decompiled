// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.GetChangesResult
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Build2.Server
{
  public class GetChangesResult
  {
    private readonly bool m_changesTruncated;
    private readonly List<Change> m_changes = new List<Change>();
    private readonly int? m_nextChangeId;

    public GetChangesResult()
      : this(new List<Change>(), new int?(), false)
    {
    }

    public GetChangesResult(List<Change> changes, int? nextChangeId, bool changesTruncated)
    {
      this.m_changes.AddRange((IEnumerable<Change>) changes);
      this.m_nextChangeId = nextChangeId;
      this.m_changesTruncated = changesTruncated;
    }

    public List<Change> Changes => this.m_changes;

    public int? NextChangeId => this.m_nextChangeId;

    public bool ChangesTruncated => this.m_changesTruncated;
  }
}
