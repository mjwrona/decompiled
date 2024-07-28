// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.ReorderedEntryList
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Git.Server
{
  internal class ReorderedEntryList : VirtualReadOnlyListBase<GitPackIndexEntry>
  {
    private readonly IReadOnlyList<GitPackIndexEntry> m_unsorted;
    private readonly IReadOnlyList<int> m_outerToInner;

    public ReorderedEntryList(
      IReadOnlyList<GitPackIndexEntry> inner,
      IReadOnlyList<int> outerToInner)
    {
      this.m_unsorted = inner;
      this.m_outerToInner = outerToInner;
      this.Count = outerToInner.Count;
    }

    public override int Count { get; }

    protected override GitPackIndexEntry DoGet(int index) => this.m_unsorted[this.m_outerToInner[index]];
  }
}
