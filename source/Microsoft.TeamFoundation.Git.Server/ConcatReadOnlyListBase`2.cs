// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.ConcatReadOnlyListBase`2
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Git.Server
{
  internal class ConcatReadOnlyListBase<T, TList> : 
    IReadOnlyList<T>,
    IReadOnlyCollection<T>,
    IEnumerable<T>,
    IEnumerable
    where TList : IReadOnlyList<T>
  {
    protected ConcatReadOnlyListBase(IEnumerable<TList> sublists)
    {
      ArgumentUtility.CheckForNull<IEnumerable<TList>>(sublists, nameof (sublists));
      this.Sublists = sublists.ToArray<TList>();
      this.Offsets = new int[this.Sublists.Length];
      for (int index = 0; index < this.Sublists.Length; ++index)
      {
        if (this.Count > int.MaxValue - this.Sublists[index].Count)
          throw new ArgumentException(string.Format("{0}[{1}].Count", (object) nameof (sublists), (object) index));
        this.Offsets[index] = this.Count;
        this.Count = this.Count + this.Sublists[index].Count;
      }
    }

    public T this[int index]
    {
      get
      {
        ArgumentUtility.CheckForOutOfRange(index, nameof (index), 0, this.Count - 1);
        int num1 = 0;
        for (int iSublist = 0; iSublist < this.Sublists.Length; ++iSublist)
        {
          int num2 = num1 + this.Sublists[iSublist].Count;
          if (index < num2)
            return this.FromRawValue(this.Sublists[iSublist][index - num1], iSublist);
          num1 = num2;
        }
        throw new Exception("This should never happen unless sublist counts are not idempotent.");
      }
    }

    public int Count { get; }

    public IEnumerator<T> GetEnumerator()
    {
      for (int i = 0; i < this.Sublists.Length; ++i)
      {
        foreach (T rawValue in this.Sublists[i])
          yield return this.FromRawValue(rawValue, i);
      }
    }

    IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.GetEnumerator();

    protected virtual T FromRawValue(T rawValue, int iSublist) => rawValue;

    protected TList[] Sublists { get; }

    protected int[] Offsets { get; }
  }
}
