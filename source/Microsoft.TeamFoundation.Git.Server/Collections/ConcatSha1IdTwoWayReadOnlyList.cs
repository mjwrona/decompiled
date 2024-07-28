// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.Collections.ConcatSha1IdTwoWayReadOnlyList
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using System.Collections;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Git.Server.Collections
{
  internal class ConcatSha1IdTwoWayReadOnlyList : 
    ConcatTwoWayReadOnlyList<Sha1Id>,
    ISha1IdTwoWayReadOnlyList,
    ITwoWayReadOnlyList<Sha1Id>,
    IReadOnlyList<Sha1Id>,
    IReadOnlyCollection<Sha1Id>,
    IEnumerable<Sha1Id>,
    IEnumerable
  {
    public ConcatSha1IdTwoWayReadOnlyList(IEnumerable<ISha1IdTwoWayReadOnlyList> sublists)
      : base((IEnumerable<ITwoWayReadOnlyList<Sha1Id>>) sublists)
    {
    }

    public IEnumerable<Sha1Id> FindBetween(Sha1Id min, Sha1Id max)
    {
      ConcatSha1IdTwoWayReadOnlyList twoWayReadOnlyList = this;
      for (int i = 0; i < twoWayReadOnlyList.Sublists.Length; ++i)
      {
        foreach (Sha1Id sha1Id in ((ISha1IdTwoWayReadOnlyList) twoWayReadOnlyList.Sublists[i]).FindBetween(min, max))
          yield return sha1Id;
      }
    }
  }
}
