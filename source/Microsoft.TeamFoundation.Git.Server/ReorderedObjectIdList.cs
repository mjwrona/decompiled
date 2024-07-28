// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.ReorderedObjectIdList
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using System.Collections;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Git.Server
{
  internal class ReorderedObjectIdList : 
    VirtualReadOnlyListBase<Sha1Id>,
    ISha1IdTwoWayReadOnlyList,
    ITwoWayReadOnlyList<Sha1Id>,
    IReadOnlyList<Sha1Id>,
    IReadOnlyCollection<Sha1Id>,
    IEnumerable<Sha1Id>,
    IEnumerable
  {
    private readonly ISha1IdTwoWayReadOnlyList m_inner;
    private readonly IReadOnlyList<int> m_outerToInner;
    private readonly IReadOnlyList<int> m_innerToOuter;

    public ReorderedObjectIdList(
      ISha1IdTwoWayReadOnlyList inner,
      IReadOnlyList<int> outerToInner,
      IReadOnlyList<int> innerToOuter)
    {
      this.m_inner = inner;
      this.m_outerToInner = outerToInner;
      this.m_innerToOuter = innerToOuter;
      this.Count = this.m_inner.Count;
    }

    public override int Count { get; }

    public bool TryGetIndex(Sha1Id value, out int index)
    {
      if (this.m_inner.TryGetIndex(value, out index))
      {
        index = this.m_innerToOuter[index];
        return true;
      }
      index = -1;
      return false;
    }

    public IEnumerable<Sha1Id> FindBetween(Sha1Id min, Sha1Id max) => this.m_inner.FindBetween(min, max);

    protected override Sha1Id DoGet(int index) => this.m_inner[this.m_outerToInner[index]];
  }
}
