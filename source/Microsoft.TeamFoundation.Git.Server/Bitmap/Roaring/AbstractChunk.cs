// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.Bitmap.Roaring.AbstractChunk
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Git.Server.Bitmap.Roaring
{
  internal abstract class AbstractChunk : IChunk, IEnumerable<ushort>, IEnumerable
  {
    private bool m_readOnly;

    public bool IsReadOnly
    {
      get => this.m_readOnly;
      set => this.m_readOnly = value || !this.m_readOnly ? value : throw new InvalidOperationException();
    }

    public abstract int Count { get; }

    public abstract ushort LowerBound { get; }

    public abstract ushort UpperBound { get; }

    public abstract int CountRuns { get; }

    public abstract bool Add(ushort index);

    public abstract bool Contains(ushort index);

    public abstract IEnumerator<ushort> GetEnumerator();

    public abstract int GetSize();

    public abstract IChunk Optimize();

    public abstract IChunk Duplicate();

    IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.GetEnumerator();

    protected void EnsureMutable()
    {
      if (this.IsReadOnly)
        throw new InvalidUseOfReadOnlyBitmapException();
    }
  }
}
