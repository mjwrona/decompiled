// Decompiled with JetBrains decompiler
// Type: <>z__ReadOnlyArray`1
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3AB5C9B-EB54-4477-A304-63BB297414A3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

internal sealed class \u003C\u003Ez__ReadOnlyArray<T> : 
  IEnumerable,
  IEnumerable<T>,
  IReadOnlyCollection<T>,
  IReadOnlyList<T>,
  ICollection<T>,
  IList<T>
{
  public \u003C\u003Ez__ReadOnlyArray(T[] items) => this._items = items;

  IEnumerator IEnumerable.GetEnumerator() => this._items.GetEnumerator();

  IEnumerator<T> IEnumerable<T>.GetEnumerator() => ((IEnumerable<T>) this._items).GetEnumerator();

  int IReadOnlyCollection<T>.Count => this._items.Length;

  [IndexerName("System.Collections.Generic.IReadOnlyList<T>.this[]")]
  T IReadOnlyList<T>.this[int index] => this._items[index];

  int ICollection<T>.Count => this._items.Length;

  bool ICollection<T>.IsReadOnly => true;

  void ICollection<T>.Add(T item) => throw new NotSupportedException();

  void ICollection<T>.Clear() => throw new NotSupportedException();

  bool ICollection<T>.Contains(T item) => ((ICollection<T>) this._items).Contains(item);

  void ICollection<T>.CopyTo(T[] array, int arrayIndex) => ((ICollection<T>) this._items).CopyTo(array, arrayIndex);

  bool ICollection<T>.Remove(T item) => throw new NotSupportedException();

  [IndexerName("System.Collections.Generic.IList<T>.this[]")]
  T IList<T>.this[int index]
  {
    get => this._items[index];
    set => throw new NotSupportedException();
  }

  int IList<T>.IndexOf(T item) => ((IList<T>) this._items).IndexOf(item);

  void IList<T>.Insert(int index, T item) => throw new NotSupportedException();

  void IList<T>.RemoveAt(int index) => throw new NotSupportedException();
}
