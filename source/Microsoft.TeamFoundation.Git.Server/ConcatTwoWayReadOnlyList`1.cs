// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.ConcatTwoWayReadOnlyList`1
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using System.Collections;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Git.Server
{
  internal class ConcatTwoWayReadOnlyList<T> : 
    ConcatReadOnlyListBase<T, ITwoWayReadOnlyList<T>>,
    ITwoWayReadOnlyList<T>,
    IReadOnlyList<T>,
    IReadOnlyCollection<T>,
    IEnumerable<T>,
    IEnumerable
  {
    public ConcatTwoWayReadOnlyList(IEnumerable<ITwoWayReadOnlyList<T>> sublists)
      : base(sublists)
    {
    }

    public bool TryGetIndex(T value, out int index)
    {
      for (int iSublist = 0; iSublist < this.Sublists.Length; ++iSublist)
      {
        if (this.Sublists[iSublist].TryGetIndex(this.ToRawValue(value, iSublist), out index))
        {
          index += this.Offsets[iSublist];
          return true;
        }
      }
      index = -1;
      return false;
    }

    protected virtual T ToRawValue(T rawValue, int iSublist) => rawValue;
  }
}
