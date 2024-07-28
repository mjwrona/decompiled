// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.Bitmap.Roaring.ListWrapper
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Git.Server.Bitmap.Roaring
{
  internal class ListWrapper : 
    ITwoWayReadOnlyList<int>,
    IReadOnlyList<int>,
    IReadOnlyCollection<int>,
    IEnumerable<int>,
    IEnumerable
  {
    private readonly List<int> m_base;

    public ListWrapper(List<int> baseList) => this.m_base = baseList;

    public int this[int index] => this.m_base[index];

    public int Count => this.m_base.Count;

    public bool TryGetIndex(int value, out int index) => throw new NotSupportedException();

    public IEnumerator<int> GetEnumerator() => throw new NotSupportedException();

    IEnumerator IEnumerable.GetEnumerator() => throw new NotSupportedException();
  }
}
