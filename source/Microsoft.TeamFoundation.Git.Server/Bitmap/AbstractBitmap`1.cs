// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.Bitmap.AbstractBitmap`1
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Git.Server.Bitmap
{
  internal abstract class AbstractBitmap<T> : 
    IBitmap<T>,
    IReadOnlyBitmap<T>,
    ISet<T>,
    ICollection<T>,
    IEnumerable<T>,
    IEnumerable
  {
    public AbstractBitmap(ITwoWayReadOnlyList<T> objectList)
    {
      ArgumentUtility.CheckForNull<ITwoWayReadOnlyList<T>>(objectList, nameof (objectList));
      this.FullObjectList = objectList;
    }

    public abstract IEnumerable<int> Indices { get; }

    public abstract int Count { get; }

    public abstract bool AddIndex(int index);

    public abstract bool ContainsIndex(int index);

    public abstract int GetSize();

    public ITwoWayReadOnlyList<T> FullObjectList { get; }

    public bool IsReadOnly { get; private set; }

    public virtual void MakeReadOnly() => this.IsReadOnly = true;

    protected void CheckForReadOnly()
    {
      if (this.IsReadOnly)
        throw new InvalidUseOfReadOnlyBitmapException();
    }

    bool ICollection<T>.Contains(T item)
    {
      int index;
      return this.FullObjectList.TryGetIndex(item, out index) && this.ContainsIndex(index);
    }

    public IEnumerator<T> GetEnumerator()
    {
      foreach (int index in this.Indices)
        yield return this.FullObjectList[index];
    }

    IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.GetEnumerator();

    void ICollection<T>.CopyTo(T[] array, int arrayIndex)
    {
      foreach (T obj in this)
        array[arrayIndex++] = obj;
    }

    public bool Add(T item) => this.AddIndex(this.FullObjectList.GetIndex<T>(item));

    bool ISet<T>.Add(T item) => this.Add(item);

    void ISet<T>.UnionWith(IEnumerable<T> other) => other.ForEach<T>((Action<T>) (item => this.Add(item)));

    void ICollection<T>.Clear() => throw new NotSupportedException();

    void ISet<T>.ExceptWith(IEnumerable<T> other) => throw new NotSupportedException();

    void ISet<T>.IntersectWith(IEnumerable<T> other) => throw new NotSupportedException();

    bool ISet<T>.IsProperSubsetOf(IEnumerable<T> other) => throw new NotSupportedException();

    bool ISet<T>.IsProperSupersetOf(IEnumerable<T> other) => throw new NotSupportedException();

    bool ISet<T>.IsSubsetOf(IEnumerable<T> other) => throw new NotSupportedException();

    bool ISet<T>.IsSupersetOf(IEnumerable<T> other) => throw new NotSupportedException();

    bool ISet<T>.Overlaps(IEnumerable<T> other) => throw new NotSupportedException();

    bool ICollection<T>.Remove(T item) => throw new NotSupportedException();

    bool ISet<T>.SetEquals(IEnumerable<T> other) => throw new NotSupportedException();

    void ISet<T>.SymmetricExceptWith(IEnumerable<T> other) => throw new NotSupportedException();

    void ICollection<T>.Add(T item) => throw new NotSupportedException();
  }
}
